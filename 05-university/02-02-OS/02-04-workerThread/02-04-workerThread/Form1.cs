using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace _02_04_workerThread
{
    public partial class Form1 : Form
    {
        //Получает ID потока Windows
        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();

        //Открывает поток по ID
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        //Меняет приоритет потока
        [DllImport("kernel32.dll")]
        static extern bool SetThreadPriority(IntPtr hThread, int nPriority);
        //Закрывает дескриптор потока
        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr hObject);

        private const uint THREAD_SET_INFORMATION = 0x0020;

        private Thread _workerThread;
        private bool _isStopping;
        private uint _workerThreadId;
        //pause
        private readonly ManualResetEvent _pauseEvent = new ManualResetEvent(true);

        public Form1()
        {
            InitializeComponent();

            cbPriority.Items.Clear();
            cbPriority.Items.Add("Low");
            cbPriority.Items.Add("Normal");
            cbPriority.Items.Add("High");
            cbPriority.SelectedIndex = 1;

            btnStartPause.Click += btnStartPause_Click;
            btnStop.Click += btnStop_Click;
            cbPriority.SelectedIndexChanged += cbPriority_SelectedIndexChanged;
        }

        private void ThreadTask()
        {
            _workerThreadId = GetCurrentThreadId();
            var rng = new Random();

            while (!_isStopping)
            {
                _pauseEvent.WaitOne();

                string data = $"[Data]: {rng.Next(100, 999)} | {DateTime.Now:HH:mm:ss.fff}";

                BeginInvoke(new Action(() =>
                {
                    listBoxLog.Items.Insert(0, data);
                    if (listBoxLog.Items.Count > 50)
                        listBoxLog.Items.RemoveAt(50);
                }));

                Thread.Sleep(500);
            }
        }

        private void btnStartPause_Click(object sender, EventArgs e)
        {
            if (_workerThread == null || !_workerThread.IsAlive)
            {
                _isStopping = false;
                _pauseEvent.Set();

                _workerThread = new Thread(ThreadTask);
                _workerThread.IsBackground = true;
                _workerThread.Start();

                btnStartPause.Text = "Pause";
                return;
            }

            if (_pauseEvent.WaitOne(0))
            {
                _pauseEvent.Reset();
                btnStartPause.Text = "Resume";
            }
            else
            {
                _pauseEvent.Set();
                btnStartPause.Text = "Pause";
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _isStopping = true;
            _pauseEvent.Set();
            btnStartPause.Text = "Start";
            listBoxLog.Items.Clear();
        }

        private void cbPriority_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_workerThread == null || !_workerThread.IsAlive)
                return;

            int priority = 0;

            switch (cbPriority.Text)
            {
                case "Low":
                    priority = -2;
                    break;
                case "High":
                    priority = 2;
                    break;
            }

            IntPtr hThread = OpenThread(THREAD_SET_INFORMATION, false, _workerThreadId);

            if (hThread != IntPtr.Zero)
            {
                SetThreadPriority(hThread, priority);
                CloseHandle(hThread);
            }
        }
    }
}