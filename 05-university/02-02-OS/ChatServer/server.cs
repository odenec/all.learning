using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ChatServer
{
    public partial class Form1 : Form
    {
        // Слушатель входящих TCP-подключений
        private TcpListener _listener;
        // Все активные клиенты: ключ — ник, значение — TCP-соединение
        private Dictionary<string, TcpClient> _clients = new();
        // Время последней активности каждого клиента (для автоотключения)
        private Dictionary<string, DateTime> _lastActivity = new();
        // Объект блокировки для потокобезопасной работы со словарями
        private readonly object _lock = new();
        // Фоновый поток, принимающий новых клиентов
        private Thread _listenThread;
        // Таймер для периодической проверки неактивных клиентов
        private System.Windows.Forms.Timer _inactiveTimer;
        // UDP-клиент для широковещательных объявлений о сервере
        private UdpClient _udpBroadcaster;
        // Таймер для повторной отправки UDP-объявлений каждую секунду
        private System.Windows.Forms.Timer _broadcastTimer;

        public Form1()
        {
            InitializeComponent();
            StartServer();                                    // запускаем сервер при старте
            FormClosing += (s, e) => StopServer();           // при закрытии формы всё останавливаем
            btnKick.Click += btnKick_Click;                  // подписываем кнопку Kick

            // Таймер проверки неактивных клиентов (срабатывает каждые 30 секунд)
            _inactiveTimer = new System.Windows.Forms.Timer();
            _inactiveTimer.Interval = 30_000;                // 30 000 мс = 30 с
            _inactiveTimer.Tick += CheckInactiveClients;     // метод-обработчик
            _inactiveTimer.Start();                          // запускаем таймер
        }

        // Запускает TCP-слушатель и UDP-объявления
        private void StartServer()
        {
            try
            {
                // Создаём слушатель на всех сетевых интерфейсах, порт 8888
                _listener = new TcpListener(IPAddress.Any, 8888);
                _listener.Start();                           // начинаем слушать
                Log("Сервер запущен на порту 8888");

                // Запускаем поток для приёма клиентов
                _listenThread = new Thread(ListenForClients);
                _listenThread.IsBackground = true;           // поток не мешает закрытию программы
                _listenThread.Start();

                // Настраиваем UDP-широковещание
                _udpBroadcaster = new UdpClient();
                _udpBroadcaster.EnableBroadcast = true;      // разрешаем broadcast-пакеты
                _broadcastTimer = new System.Windows.Forms.Timer();
                _broadcastTimer.Interval = 1000;             // каждую секунду
                _broadcastTimer.Tick += (s, args) =>
                {
                    try
                    {
                        // Шлём пакет "CHAT_SERVER" на адрес 255.255.255.255, порт 8889
                        byte[] data = Encoding.UTF8.GetBytes("CHAT_SERVER");
                        _udpBroadcaster.Send(data, data.Length,
                            new IPEndPoint(IPAddress.Broadcast, 8889));
                    }
                    catch { }
                };
                _broadcastTimer.Start();                     // запускаем таймер
                Log("UDP-объявления запущены на порту 8889");
            }
            catch (Exception ex)
            {
                Log("Ошибка запуска: " + ex.Message);
            }
        }

        // Бесконечный цикл приёма новых TCP-клиентов
        private void ListenForClients()
        {
            while (true)
            {
                try
                {
                    var client = _listener.AcceptTcpClient(); // ждём подключения
                    // Для каждого клиента запускаем свой поток
                    var clientThread = new Thread(() => HandleClient(client));
                    clientThread.IsBackground = true;
                    clientThread.Start();
                }
                catch { break; } // если слушатель остановлен – выходим
            }
        }

        // Обрабатывает одного клиента (работает в отдельном потоке)
        private void HandleClient(TcpClient client)
        {
            string clientName = null;
            try
            {
                var stream = client.GetStream();              // получаем сетевой поток
                var reader = new StreamReader(stream, Encoding.UTF8);
                var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                clientName = reader.ReadLine();               // первая строка – ник
                if (string.IsNullOrWhiteSpace(clientName))    // пустой ник не допускаем
                {
                    client.Close();
                    return;
                }

                // Проверяем, нет ли уже такого ника в чате
                lock (_lock)
                {
                    if (_clients.ContainsKey(clientName))
                    {
                        // Отправляем ошибку и закрываем соединение
                        try { writer.WriteLine("ERROR:Ник уже используется другим пользователем."); }
                        catch { }
                        client.Close();
                        clientName = null; // чтобы finally не трогал существующего клиента
                        return;
                    }

                    // Добавляем нового клиента в словари
                    _clients[clientName] = client;
                    _lastActivity[clientName] = DateTime.Now;   // фиксируем время входа
                }

                UpdateClientList();                           // обновляем список на форме
                Log($"+ {clientName} подключился");           // пишем в лог
                Broadcast($"{clientName} зашёл в чат.");      // оповещаем всех

                // Читаем сообщения от клиента, пока они поступают
                string message;
                while ((message = reader.ReadLine()) != null)
                {
                    // Обновляем время последней активности
                    lock (_lock)
                    {
                        if (_lastActivity.ContainsKey(clientName))
                            _lastActivity[clientName] = DateTime.Now;
                    }
                    Broadcast($"{clientName}: {message}");    // рассылаем всем
                }
            }
            catch { }                                        // соединение разорвано
            finally
            {
                // При любом исходе удаляем клиента из всех структур
                if (clientName != null)
                {
                    lock (_lock)
                    {
                        _clients.Remove(clientName);
                        _lastActivity.Remove(clientName);
                    }
                    UpdateClientList();
                    Log($"- {clientName} отключился");
                    Broadcast($"{clientName} вышел из чата.");
                }
                client.Close();                              // закрываем TCP-соединение
            }
        }

        // Рассылает сообщение всем подключённым клиентам
        private void Broadcast(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            lock (_lock)
            {
                foreach (var kvp in _clients)
                {
                    try { kvp.Value.GetStream().Write(data, 0, data.Length); }
                    catch { } // игнорируем клиентов, которые уже отвалились
                }
            }
        }

        // Потокобезопасно обновляет список клиентов в ListBox
        private void UpdateClientList()
        {
            if (lbClients.InvokeRequired)                    // если мы не в UI-потоке
            {
                lbClients.Invoke(new Action(UpdateClientList)); // перевызываем в UI-потоке
                return;
            }
            lbClients.Items.Clear();
            lock (_lock)
                foreach (var name in _clients.Keys)
                    lbClients.Items.Add(name);
        }

        // Потокобезопасно добавляет сообщение в текстовый лог
        private void Log(string text)
        {
            if (tbLog.InvokeRequired)                        // если не в UI-потоке
            {
                tbLog.Invoke(new Action<string>(Log), text); // маршалируем вызов
                return;
            }
            tbLog.AppendText($"{DateTime.Now:HH:mm:ss} {text}\r\n");
        }

        // Обработчик кнопки Kick – отключает выбранного клиента
        private void btnKick_Click(object sender, EventArgs e)
        {
            if (lbClients.SelectedItem == null) return;      // ничего не выбрано
            string target = lbClients.SelectedItem.ToString();
            lock (_lock)
            {
                if (_clients.TryGetValue(target, out var client))
                {
                    client.Close();                          // закрываем соединение
                    _clients.Remove(target);
                    _lastActivity.Remove(target);
                }
            }
            UpdateClientList();                              // обновляем список
            Log($"Администратор отключил {target}");
        }

        // Проверяет, кто из клиентов неактивен более 20 минут, и отключает их
        private void CheckInactiveClients(object sender, EventArgs e)
        {
            List<string> toKick = new();
            DateTime now = DateTime.Now;

            lock (_lock)
            {
                foreach (var kvp in _lastActivity)
                {
                    if ((now - kvp.Value).TotalMinutes >= 20) // прошло больше 20 минут?
                        toKick.Add(kvp.Key);
                }
            }

            foreach (var name in toKick)
            {
                lock (_lock)
                {
                    if (_clients.TryGetValue(name, out var client))
                    {
                        try { client.Close(); } catch { }
                        _clients.Remove(name);
                        _lastActivity.Remove(name);
                    }
                }
                UpdateClientList();
                Log($"{name} отключён за неактивность (20 мин)");
                Broadcast($"{name} отключён за неактивность.");
            }
        }

        // Останавливает сервер и освобождает все ресурсы
        private void StopServer()
        {
            _listener?.Stop();                               // перестаём принимать клиентов
            _inactiveTimer?.Stop();
            _broadcastTimer?.Stop();
            _udpBroadcaster?.Close();
            lock (_lock)
            {
                foreach (var c in _clients.Values)
                    c.Close();                               // закрываем всех оставшихся
                _clients.Clear();
                _lastActivity.Clear();
            }
        }
    }
}