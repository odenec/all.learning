using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace _02_04_window
{
    public partial class Form1 : Form
    {
        // WinAPI функции
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);  // Перечисляет все окна

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count); // Получает заголовок

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId); // id процесса

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow); // Показывает/скрывает/сворачивает/разворачивает

        [DllImport("user32.dll")]
        private static extern bool SetWindowText(IntPtr hWnd, string text); // Устанавливает новый заголовок окна

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);// Проверяет, видимо ли окно

        [DllImport("user32.dll")]
        private static extern bool CloseWindow(IntPtr hWnd);// Сворачивает

        // Константы для ShowWindow
        private const int SW_MINIMIZE = 6;
        private const int SW_RESTORE = 9;

        // Делегат для EnumWindows
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        // Класс для хранения информации об окне
        private class WindowInfo
        {
            public IntPtr Handle { get; set; }
            public string Title { get; set; }
            public uint ProcessId { get; set; }

            public override string ToString()
            {
                return Title;
            }
        }

        private List<WindowInfo> windows = new List<WindowInfo>();

        public Form1()
        {
            InitializeComponent();

            this.button1.Click += button1_Click;
            this.button2.Click += button2_Click;
            this.button3.Click += button3_Click;
            this.button4.Click += button4_Click;
            this.button5.Click += button5_Click;


            //загружаем список окон при запуске
            UpdateWindowList();
        }

        //  Получить список всех окон
        private void button1_Click(object sender, EventArgs e)
        {
            UpdateWindowList();
        }

        private void UpdateWindowList()
        {
            windows.Clear();
            listBox1.Items.Clear();

            EnumWindows(new EnumWindowsProc(EnumWindowCallback), IntPtr.Zero);

            foreach (var window in windows)
            {
                if (!string.IsNullOrWhiteSpace(window.Title))
                {
                    listBox1.Items.Add(window);
                }
            }

            if (listBox1.Items.Count == 0)
            {
                listBox1.Items.Add("Нет окон");
            }
        }

        private bool EnumWindowCallback(IntPtr hWnd, IntPtr lParam)
        {
            if (IsWindowVisible(hWnd))
            {
                const int maxLength = 256;
                StringBuilder title = new StringBuilder(maxLength);
                GetWindowText(hWnd, title, maxLength);

                if (title.Length > 0) //если title 0, то id
                {
                    GetWindowThreadProcessId(hWnd, out uint processId);

                    windows.Add(new WindowInfo
                    {
                        Handle = hWnd,
                        Title = title.ToString(),
                        ProcessId = processId
                    });
                }
            }
            return true;
        }

        // 2. Свернуть окно
        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null && listBox1.SelectedItem is WindowInfo)
            {
                WindowInfo selected = (WindowInfo)listBox1.SelectedItem;
                ShowWindow(selected.Handle, SW_MINIMIZE);
                UpdateWindowList();
            }
            else
            {
                MessageBox.Show("Выберите окно из списка", "Внимание");
            }
        }

        // 3. Развернуть окно
        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null && listBox1.SelectedItem is WindowInfo)
            {
                WindowInfo selected = (WindowInfo)listBox1.SelectedItem;
                ShowWindow(selected.Handle, SW_RESTORE);
                UpdateWindowList();
            }
            else
            {
                MessageBox.Show("Выберите окно из списка", "Внимание");
            }
        }

        // 4. Закрыть окно
        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null && listBox1.SelectedItem is WindowInfo)
            {
                WindowInfo selected = (WindowInfo)listBox1.SelectedItem;

                if (MessageBox.Show($"Закрыть окно \"{selected.Title}\"?",
                    "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CloseWindow(selected.Handle);
                    UpdateWindowList();
                }
            }
            else
            {
                MessageBox.Show("Выберите окно из списка", "Внимание");
            }
        }

        // 5. Переименовать окно
        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null || !(listBox1.SelectedItem is WindowInfo))
            {
                MessageBox.Show("Сначала выберите окно из списк", "Внимание");
                return;
            }

            string newName = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(newName))
            {
                MessageBox.Show("Введите новое имя окна в текстовое поле", "Внимание");
                return;
            }

            WindowInfo selected = (WindowInfo)listBox1.SelectedItem;

            if (SetWindowText(selected.Handle, newName))
            {
                MessageBox.Show($"Окно \"{selected.Title}\" переименовано в \"{newName}\"",
                    "Успех");
                UpdateWindowList();
                textBox1.Clear();
            }
            else
            {
                MessageBox.Show("Не удалось переименовать окно",
                    "Ошибка");
            }
        }
    }
}