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
        private TcpClient _client;
        private StreamReader _reader;
        private StreamWriter _writer;
        private Thread _receiveThread;
        private string _nickname;
        private Thread _discoveryThread;
        private bool _serverFound;

        public Form1()
        {
            InitializeComponent();
            btnConnect.Click += btnConnect_Click;
            btnSend.Click += btnSend_Click;
            FormClosing += Form1_FormClosing;

            tbMessage.Enabled = false;
            btnSend.Enabled = false;

            // Первый запуск поиска сервера
            StartServerDiscovery();
        }

        private void StartServerDiscovery()
        {
            // Если поток поиска уже работает — не запускаем повторно
            if (_discoveryThread != null && _discoveryThread.IsAlive)
                return;

            _serverFound = false;
            _discoveryThread = new Thread(() =>
            {
                try
                {
                    using var listener = new UdpClient(8889);
                    listener.EnableBroadcast = true;
                    IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                    while (!_serverFound)
                    {
                        byte[] data = listener.Receive(ref remote);
                        string msg = Encoding.UTF8.GetString(data);
                        if (msg == "CHAT_SERVER")
                        {
                            _serverFound = true;
                            this.Invoke(new Action(() =>
                            {
                                tbServerIP.Text = remote.Address.ToString();
                                lbChat.Items.Add("Найден сервер в локальной сети: " + tbServerIP.Text);
                                btnConnect.Enabled = true;
                            }));
                        }
                    }
                }
                catch { }
            });
            _discoveryThread.IsBackground = true;
            _discoveryThread.Start();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (_client != null && _client.Connected)
                return;

            try
            {
                _client = new TcpClient(tbServerIP.Text, 8888);
                var stream = _client.GetStream();
                _reader = new StreamReader(stream, Encoding.UTF8);
                _writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                _client.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, 1);
                _client.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, 1);
                
                _nickname = tbNick.Text;
                _writer.WriteLine(_nickname);

                _receiveThread = new Thread(ReceiveMessages);
                _receiveThread.IsBackground = true;
                _receiveThread.Start();

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

        private void ReceiveMessages()
        {
            try
            {
                string message;
                while ((message = _reader.ReadLine()) != null)
                {
                    if (message.StartsWith("ERROR:"))
                    {
                        string errorText = message.Substring(6);
                        this.Invoke(new Action(() =>
                        {
                            DisconnectUI();
                            lbChat.Items.Add("=== Отключено: " + errorText + " ===");
                        }));
                        MessageBox.Show(errorText, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        CloseConnection();
                        return;
                    }

                    if (!string.IsNullOrEmpty(_nickname) && message.StartsWith(_nickname + ":"))
                        continue;

                    lbChat.Invoke(new Action(() =>
                    {
                        lbChat.Items.Add(message);
                        lbChat.TopIndex = lbChat.Items.Count - 1;
                    }));
                }
            }
            catch { }
            finally
            {
                this.Invoke(new Action(() =>
                {
                    DisconnectUI();
                    lbChat.Items.Add("=== Соединение с сервером потеряно ===");
                }));
                CloseConnection();
            }
        }

        private void DisconnectUI()
        {
            tbMessage.Enabled = false;
            btnSend.Enabled = false;
            btnConnect.Enabled = true;
            tbServerIP.Enabled = true;
            tbNick.Enabled = true;

            // Перезапускаем поиск сервера
            StartServerDiscovery();
        }

        private void CloseConnection()
        {
            try { _client?.Close(); } catch { }
            _client = null;
            _writer = null;
            _reader = null;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string msg = tbMessage.Text.Trim();
            if (string.IsNullOrEmpty(msg))
                return;

            if (_writer == null || _client == null || !_client.Connected)
            {
                MessageBox.Show("Нет соединения с сервером.", "Не отправлено",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                _writer.WriteLine(msg);
                lbChat.Items.Add($"Я: {msg}");
                tbMessage.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка отправки: " + ex.Message, "Ошибка");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseConnection();
        }
    }
}