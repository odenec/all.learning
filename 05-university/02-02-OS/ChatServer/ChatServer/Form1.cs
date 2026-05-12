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
        private TcpListener _listener;
        private Dictionary<string, TcpClient> _clients = new();
        private Dictionary<string, DateTime> _lastActivity = new();
        private readonly object _lock = new();
        private Thread _listenThread;
        private System.Windows.Forms.Timer _inactiveTimer;
        private UdpClient _udpBroadcaster;
        private System.Windows.Forms.Timer _broadcastTimer;

        public Form1()
        {
            InitializeComponent();
            StartServer();
            FormClosing += (s, e) => StopServer();
            btnKick.Click += btnKick_Click;

            // Проверка неактивных клиентов (каждые 30 секунд)
            _inactiveTimer = new System.Windows.Forms.Timer();
            _inactiveTimer.Interval = 30_000;
            _inactiveTimer.Tick += CheckInactiveClients;
            _inactiveTimer.Start();
        }

        private void StartServer()
        {
            try
            {
                _listener = new TcpListener(IPAddress.Any, 8888);
                _listener.Start();
                Log("Сервер запущен на порту 8888");

                _listenThread = new Thread(ListenForClients);
                _listenThread.IsBackground = true;
                _listenThread.Start();

                // UDP-широковещание для автообнаружения
                _udpBroadcaster = new UdpClient();
                _udpBroadcaster.EnableBroadcast = true;
                _broadcastTimer = new System.Windows.Forms.Timer();
                _broadcastTimer.Interval = 1000;
                _broadcastTimer.Tick += (s, args) =>
                {
                    try
                    {
                        byte[] data = Encoding.UTF8.GetBytes("CHAT_SERVER");
                        _udpBroadcaster.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, 8889));
                    }
                    catch { }
                };
                _broadcastTimer.Start();
                Log("UDP-объявления запущены на порту 8889");
            }
            catch (Exception ex)
            {
                Log("Ошибка запуска: " + ex.Message);
            }
        }

        private void ListenForClients()
        {
            while (true)
            {
                try
                {
                    var client = _listener.AcceptTcpClient();
                    var clientThread = new Thread(() => HandleClient(client));
                    clientThread.IsBackground = true;
                    clientThread.Start();
                }
                catch { break; }
            }
        }

        private void HandleClient(TcpClient client)
        {
            string clientName = null;
            try
            {
                var stream = client.GetStream();
                var reader = new StreamReader(stream, Encoding.UTF8);
                var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                clientName = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(clientName))
                {
                    client.Close();
                    return;
                }

                lock (_lock)
                {
                    if (_clients.ContainsKey(clientName))
                    {
                        try { writer.WriteLine("ERROR:Ник уже используется другим пользователем."); }
                        catch { }
                        client.Close();
                        clientName = null;
                        return;
                    }

                    _clients[clientName] = client;
                    _lastActivity[clientName] = DateTime.Now;
                }

                UpdateClientList();
                Log($"+ {clientName} подключился");
                Broadcast($"{clientName} зашёл в чат.");

                string message;
                while ((message = reader.ReadLine()) != null)
                {
                    lock (_lock)
                    {
                        if (_lastActivity.ContainsKey(clientName))
                            _lastActivity[clientName] = DateTime.Now;
                    }
                    Broadcast($"{clientName}: {message}");
                }
            }
            catch { }
            finally
            {
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
                client.Close();
            }
        }

        private void Broadcast(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            lock (_lock)
            {
                foreach (var kvp in _clients)
                {
                    try { kvp.Value.GetStream().Write(data, 0, data.Length); }
                    catch { }
                }
            }
        }

        private void UpdateClientList()
        {
            if (lbClients.InvokeRequired)
            {
                lbClients.Invoke(new Action(UpdateClientList));
                return;
            }
            lbClients.Items.Clear();
            lock (_lock)
                foreach (var name in _clients.Keys)
                    lbClients.Items.Add(name);
        }

        private void Log(string text)
        {
            if (tbLog.InvokeRequired)
            {
                tbLog.Invoke(new Action<string>(Log), text);
                return;
            }
            tbLog.AppendText($"{DateTime.Now:HH:mm:ss} {text}\r\n");
        }

        private void btnKick_Click(object sender, EventArgs e)
        {
            if (lbClients.SelectedItem == null) return;
            string target = lbClients.SelectedItem.ToString();
            lock (_lock)
            {
                if (_clients.TryGetValue(target, out var client))
                {
                    client.Close();
                    _clients.Remove(target);
                    _lastActivity.Remove(target);
                }
            }
            UpdateClientList();
            Log($"Администратор отключил {target}");
        }

        private void CheckInactiveClients(object sender, EventArgs e)
        {
            List<string> toKick = new();
            DateTime now = DateTime.Now;

            lock (_lock)
            {
                foreach (var kvp in _lastActivity)
                {
                    if ((now - kvp.Value).TotalMinutes >= 20)
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

        private void StopServer()
        {
            _listener?.Stop();
            _inactiveTimer?.Stop();
            _broadcastTimer?.Stop();
            _udpBroadcaster?.Close();
            lock (_lock)
            {
                foreach (var c in _clients.Values)
                    c.Close();
                _clients.Clear();
                _lastActivity.Clear();
            }
        }
    }
}