using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeyboardHookLib
{
    public class KeyboardHook : IDisposable
    {
        
        public delegate void KeyPressedEventHandler(object sender, KeyPressedEventArgs e);
        public event KeyPressedEventHandler KeyPressed;

        
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        // Структуры для хука
        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]             
        private static extern IntPtr SetWindowsHookEx(                                      // Функция: УСТАНОВИТЬ глобальный хук
            int idHook,                                                                      // Тип хука (13 = клавиатурный низкоуровневый)
            LowLevelKeyboardProc lpfn,                                                        // Указатель на наш метод-обработчик нажатий
            IntPtr hMod,                                                                      // Дескриптор модуля, где лежит наш метод
            uint dwThreadId);                                                                 // ID потока (0 = хук для всей системы)

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]             
        [return: MarshalAs(UnmanagedType.Bool)]                                              // Говорим, что возвращается BOOL (true/false)
        private static extern bool UnhookWindowsHookEx(                                      // Функция: УДАЛИТЬ/ОСТАНОВИТЬ хук
            IntPtr hhk);                                                                      // Дескриптор хука, полученный при SetWindowsHookEx

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]              
        private static extern IntPtr CallNextHookEx(                                         // Функция: ПЕРЕДАТЬ событие следующему хуку в цепочке
            IntPtr hhk,                                                                       // Дескриптор ТЕКУЩЕГО хука
            int nCode,                                                                        // Код события (>=0 если надо обработать)
            IntPtr wParam,                                                                    // Тип сообщения (WM_KEYDOWN и т.д.)
            IntPtr lParam);                                                                   // Указатель на структуру с данными о нажатии

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]            
        private static extern IntPtr GetModuleHandle(                                        // Функция: ПОЛУЧИТЬ дескриптор загруженного модуля (EXE/DLL)
            string lpModuleName);                                                             // Имя модуля (null = текущий EXE)


        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;
        private bool _isHooked = false;

        public KeyboardHook()
        {
            _proc = HookCallback;
        }

        
        public void Start()
        {
            if (_isHooked) return;

            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                _hookID = SetWindowsHookEx(WH_KEYBOARD_LL, _proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
            _isHooked = true;
        }

        
        public void Stop()
        {
            if (!_isHooked) return;

            UnhookWindowsHookEx(_hookID);
            _hookID = IntPtr.Zero;
            _isHooked = false;
        }

        // Колбэк хука (вызывается при каждом нажатии)
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                KBDLLHOOKSTRUCT hookStruct = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                Keys key = (Keys)hookStruct.vkCode;

                // Вызываем событие с информацией о нажатой клавише
                KeyPressed?.Invoke(this, new KeyPressedEventArgs(key, hookStruct.vkCode));
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            Stop();
        }
    }

    // Класс для аргументов события
    public class KeyPressedEventArgs : EventArgs
    {
        public Keys Key { get; private set; }
        public int KeyCode { get; private set; }
        public string KeyName => Key.ToString();
        public DateTime Timestamp { get; private set; }

        public KeyPressedEventArgs(Keys key, int keyCode)
        {
            Key = key;
            KeyCode = keyCode;
            Timestamp = DateTime.Now;
        }
    }
}