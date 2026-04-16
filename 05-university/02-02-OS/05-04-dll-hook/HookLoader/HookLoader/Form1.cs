using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace HookLoader
{
    public partial class Form1 : Form
    {
        private Assembly _hookAssembly = null;
        private Type _hookType = null;
        private object _hookInstance = null;
        private EventInfo _keyPressedEvent = null;
        private Delegate _eventHandler = null;

        public Form1()
        {
            InitializeComponent();
            btnLoad.Click += BtnLoad_Click;
            btnStart.Click += BtnStart_Click;
            btnStop.Click += BtnStop_Click;
            UpdateUI();
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            string dllPath = Path.Combine(Application.StartupPath, "KeyboardHook.dll");

            if (!File.Exists(dllPath))
            {
                lblStatus.Text = "DLL не найдена";
                return;
            }

            try
            {
                
                _hookAssembly = Assembly.LoadFile(dllPath);
                _hookType = _hookAssembly.GetType("KeyboardHookLib.KeyboardHook");

                if (_hookType == null)
                {
                    lblStatus.Text = "Класс KeyboardHook не найден";
                    return;
                }

                
                _hookInstance = Activator.CreateInstance(_hookType);

                
                _keyPressedEvent = _hookType.GetEvent("KeyPressed");

                // обработчик события
                Type eventHandlerType = _keyPressedEvent.EventHandlerType;
                MethodInfo handlerMethod = typeof(Form1).GetMethod(nameof(OnKeyPressed), 
                    BindingFlags.NonPublic | BindingFlags.Instance);
                _eventHandler = Delegate.CreateDelegate(eventHandlerType, this, handlerMethod);


                _keyPressedEvent.AddEventHandler(_hookInstance, _eventHandler);

                lblStatus.Text = "DLL загружена";
                btnLoad.Enabled = false;
                btnStart.Enabled = true;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Ошибка: {ex.Message}";
            }
        }

        
        private void OnKeyPressed(object sender, object e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, object>(OnKeyPressed), sender, e);
                return;
            }

           
            Type argsType = e.GetType();
            PropertyInfo keyNameProp = argsType.GetProperty("KeyName");
            PropertyInfo timestampProp = argsType.GetProperty("Timestamp");

            string keyName = keyNameProp?.GetValue(e)?.ToString() ?? "?";
            DateTime time = (DateTime)(timestampProp?.GetValue(e) ?? DateTime.Now);


            txtLog.AppendText($"[{time:HH:mm:ss}] {keyName}{Environment.NewLine}");
            //txtLog.AppendText($"{keyName}");
        
        }


        private void BtnStart_Click(object sender, EventArgs e)
        {
            try
            {
                MethodInfo startMethod = _hookType.GetMethod("Start");
                startMethod.Invoke(_hookInstance, null);

                lblStatus.Text = "Хук +++";
                btnStart.Enabled = false;
                btnStop.Enabled = true;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Ошибка запуска: {ex.Message}";
            }
        }


        private void BtnStop_Click(object sender, EventArgs e)
        {
            try
            {
                MethodInfo stopMethod = _hookType.GetMethod("Stop");
                stopMethod.Invoke(_hookInstance, null);

                lblStatus.Text = "Хук остановлен";
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Ошибка остановки: {ex.Message}";
            }
        }

        private void UpdateUI()
        {
            btnLoad.Enabled = true;
            btnStart.Enabled = false;
            btnStop.Enabled = false;
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_hookInstance != null)
            {
                try
                {
                    MethodInfo stopMethod = _hookType.GetMethod("Stop");
                    stopMethod?.Invoke(_hookInstance, null);

                    if (_keyPressedEvent != null && _eventHandler != null)
                    {
                        _keyPressedEvent.RemoveEventHandler(_hookInstance, _eventHandler);
                    }

                    MethodInfo disposeMethod = _hookType.GetMethod("Dispose");
                    disposeMethod?.Invoke(_hookInstance, null);
                }
                catch { }
            }
            base.OnFormClosing(e);
        }
    }
}