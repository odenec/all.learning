using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class Form1 : Form
    {
        private TcpClient _client;               // TCP-соединение
        private StreamReader _reader;            // для чтения данных от сервера
        private StreamWriter _writer;            // для отправки данных серверу
        private Thread _receiveThread;           // поток приёма сообщений
        private string _nickname;                // наш ник в чате
        private Thread _discoveryThread;         // поток обнаружения сервера
        private bool _serverFound;               // флаг: сервер найден?

        public Form1()
        {
            InitializeComponent();
            btnConnect.Click += btnConnect_Click; // подписываем кнопки
            btnSend.Click += btnSend_Click;
            FormClosing += Form1_FormClosing;

            tbMessage.Enabled = false;           // поле сообщений пока нельзя использовать
            btnSend.Enabled = false;

            StartServerDiscovery();              // сразу начинаем искать сервер
        }

        // Запускает фоновый поток для прослушивания UDP-объявлений сервера
        private void StartServerDiscovery()
        {
            if (_discoveryThread != null && _discoveryThread.IsAlive) // уже ищем
                return;

            _serverFound = false;
            _discoveryThread = new Thread(() =>
            {
                try
                {
                    using var listener = new UdpClient(8889); // слушаем порт 8889
                    listener.EnableBroadcast = true;          // разрешаем broadcast
                    IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                    while (!_serverFound)                     // пока не нашли сервер
                    {
                        byte[] data = listener.Receive(ref remote); // ждём пакет
                        string msg = Encoding.UTF8.GetString(data);
                        if (msg == "CHAT_SERVER")               // это наше объявление
                        {
                            _serverFound = true;
                            this.Invoke(new Action(() =>        // обновляем UI в основном потоке
                            {
                                tbServerIP.Text = remote.Address.ToString(); // подставляем IP
                                lbChat.Items.Add("Найден сервер в локальной сети: " + tbServerIP.Text);
                                btnConnect.Enabled = true;       // разрешаем подключиться
                            }));
                        }
                    }
                }
                catch { }
            });
            _discoveryThread.IsBackground = true;
            _discoveryThread.Start();
        }

        // Нажатие на кнопку "Подключиться"
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (_client != null && _client.Connected)          // уже подключены
                return;

            try
            {
                // Подключаемся к серверу (IP и порт 8888)
                _client = new TcpClient(tbServerIP.Text, 8888);
                var stream = _client.GetStream();
                _reader = new StreamReader(stream, Encoding.UTF8);
                _writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                // Включаем TCP Keep-Alive для быстрого обнаружения обрыва связи
                _client.Client.SetSocketOption(SocketOptionLevel.Socket,
                    SocketOptionName.KeepAlive, true);
                _client.Client.SetSocketOption(SocketOptionLevel.Tcp,
                    SocketOptionName.TcpKeepAliveInterval, 1);
                _client.Client.SetSocketOption(SocketOptionLevel.Tcp,
                    SocketOptionName.TcpKeepAliveTime, 1);

                _nickname = tbNick.Text;                       // запоминаем ник
                _writer.WriteLine(_nickname);                  // сразу отправляем серверу

                // Запускаем поток для получения сообщений
                _receiveThread = new Thread(ReceiveMessages);
                _receiveThread.IsBackground = true;
                _receiveThread.Start();

                // Меняем доступность элементов
                tbMessage.Enabled = true;
                btnSend.Enabled = true;
                btnConnect.Enabled = false;
                tbServerIP.Enabled = false;
                tbNick.Enabled = false;

                lbChat.Items.Add("Подключено к серверу (ожидание подтверждения...)");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения: " + ex.Message, "Ошибка");
            }
        }

        // Работает в отдельном потоке: читает сообщения от сервера
        private void ReceiveMessages()
        {
            try
            {
                string message;
                // Читаем строки, пока соединение живо
                while ((message = _reader.ReadLine()) != null)
                {
                    // Обработка сообщения об ошибке от сервера
                    if (message.StartsWith("ERROR:"))
                    {
                        string errorText = message.Substring(6);
                        this.Invoke(new Action(() =>
                        {
                            DisconnectUI();
                            lbChat.Items.Add("=== Отключено: " + errorText + " ===");
                        }));
                        MessageBox.Show(errorText, "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        CloseConnection();
                        return;
                    }

                    // Игнорируем собственные сообщения (их уже вывели локально как "Я: ...")
                    if (!string.IsNullOrEmpty(_nickname) && message.StartsWith(_nickname + ":"))
                        continue;

                    // Выводим сообщение в UI-потоке
                    lbChat.Invoke(new Action(() =>
                    {
                        lbChat.Items.Add(message);
                        lbChat.TopIndex = lbChat.Items.Count - 1; // автоскролл
                    }));
                }
            }
            catch { }  // соединение разорвано (штатно или аварийно)
            finally
            {
                // Гарантированно приводим интерфейс в исходное состояние
                this.Invoke(new Action(() =>
                {
                    DisconnectUI();
                    lbChat.Items.Add("=== Соединение с сервером потеряно ===");
                }));
                CloseConnection();
            }
        }

        // Возвращает интерфейс в состояние "можно подключаться заново"
        private void DisconnectUI()
        {
            tbMessage.Enabled = false;
            btnSend.Enabled = false;
            btnConnect.Enabled = true;
            tbServerIP.Enabled = true;
            tbNick.Enabled = true;

            // Перезапускаем поиск сервера (на случай смены сети или перезапуска сервера)
            StartServerDiscovery();
        }

        // Корректно закрывает TCP-соединение и обнуляет переменные
        private void CloseConnection()
        {
            try { _client?.Close(); } catch { }
            _client = null;
            _writer = null;
            _reader = null;
        }

        // Кнопка "Отправить"
        private void btnSend_Click(object sender, EventArgs e)
        {
            string msg = tbMessage.Text.Trim();
            if (string.IsNullOrEmpty(msg)) return;

            // Если соединение уже потеряно – сообщаем и не пытаемся отправить
            if (_writer == null || _client == null || !_client.Connected)
            {
                MessageBox.Show("Нет соединения с сервером.", "Не отправлено",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                _writer.WriteLine(msg);                     // шлём на сервер
                lbChat.Items.Add($"Я: {msg}");              // отображаем у себя
                tbMessage.Clear();                          // очищаем поле ввода
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка отправки: " + ex.Message, "Ошибка");
            }
        }

        // При закрытии формы разрываем соединение
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseConnection();
        }
    }
}