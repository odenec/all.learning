using System;
using System.Security.AccessControl; // Классы для работы с правами доступа
using System.Security.Principal;     // SecurityIdentifier — идентификатор пользователя
using System.Windows.Forms;
using Microsoft.Win32;

namespace _06_04_registry
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent(); // Автосгенерированный метод настройки формы

            // Подписка на события
            Load += Form1_Load;
            buttonRead.Click += buttonRead_Click;
            buttonCreate.Click += buttonCreate_Click;
            buttonDelete.Click += buttonDelete_Click;
            buttonProtect.Click += buttonProtect_Click;
            treeViewRegistry.AfterSelect += treeViewRegistry_AfterSelect;
            treeViewRegistry.BeforeExpand += TreeViewRegistry_BeforeExpand;
        }

        // Полная перезагрузка дерева реестра
        private void ReloadTree()
        {
            treeViewRegistry.Nodes.Clear(); // Очищает все узлы дерева

            TreeNode hkcu = new TreeNode("HKCU"); // Создаёт корневой узел HKCU
            TreeNode hklm = new TreeNode("HKLM"); // Создаёт корневой узел HKLM

            treeViewRegistry.Nodes.Add(hkcu); // Добавляет узел HKCU в дерево
            treeViewRegistry.Nodes.Add(hklm); // Добавляет узел HKLM в дерево

            hkcu.Nodes.Add("..."); // Добавляет временный узел-заглушку для HKCU
            hklm.Nodes.Add("..."); // Добавляет временный узел-заглушку для HKLM
        }

        // Загрузка формы
        private void Form1_Load(object sender, EventArgs e)
        {
            ReloadTree(); // Загружает корневые узлы дерева
        }

        // Возвращает объект RegistryKey для указанной ветки
        RegistryKey GetRegistryKey(string hive)
        {
            if (hive == "HKCU") return Registry.CurrentUser;  // Registry.CurrentUser — доступ к HKEY_CURRENT_USER
            if (hive == "HKLM") return Registry.LocalMachine; // Registry.LocalMachine — доступ к HKEY_LOCAL_MACHINE
            return null; // Если ветка не распознана — возвращает null
        }

        // Извлекает название ветки из полного пути (например "HKCU\Software" → "HKCU")
        string GetHiveFromPath(string fullPath)
        {
            if (fullPath.StartsWith("HKCU")) return "HKCU"; // StartsWith проверяет начало строки
            if (fullPath.StartsWith("HKLM")) return "HKLM";
            return "";
        }

        // Обрезает название ветки из полного пути (например "HKCU\Software" → "Software")
        string GetPathWithoutHive(string fullPath, string hive)
        {
            if (fullPath.StartsWith(hive + "\\"))
                return fullPath.Substring(hive.Length + 1); // Substring обрезает строку с указанной позиции
            if (fullPath == hive) return "";
            return fullPath;
        }

        // Установка защиты ключа на уровне реестра (запрет удаления для всех)
        private void ProtectKey(string hive, string path)
        {
            RegistryKey key = GetRegistryKey(hive).OpenSubKey(path, true); // true — открывает для записи
            if (key == null) return;

            RegistrySecurity security = key.GetAccessControl(); // Получает текущие права доступа
            SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null); // "Все"

            // Добавляет правило: запретить Everyone удаление
            security.AddAccessRule(new RegistryAccessRule(everyone, RegistryRights.Delete, AccessControlType.Deny));
            key.SetAccessControl(security); // Применяет права к ключу
            key.Close(); // Закрывает ключ
        }

        // Снятие защиты ключа (разрешение удаления для всех)
        private void UnprotectKey(string hive, string path)
        {
            RegistryKey key = GetRegistryKey(hive).OpenSubKey(path, true); // true — открывает для записи
            if (key == null) return;

            RegistrySecurity security = key.GetAccessControl(); // Получает текущие права доступа
            SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null); // "Все"

            // Удаляет правило запрета на удаление
            security.RemoveAccessRule(new RegistryAccessRule(everyone, RegistryRights.Delete, AccessControlType.Deny));
            key.SetAccessControl(security); // Применяет изменения к ключу
            key.Close(); // Закрывает ключ
        }

        // Кнопка "Защита" — устанавливает или снимает защиту от удаления
        private void buttonProtect_Click(object sender, EventArgs e)
        {
            string fullPath = textBoxPath.Text; // Путь к ключу из поля

            if (string.IsNullOrWhiteSpace(fullPath)) // Проверка что путь не пустой
            {
                MessageBox.Show("Сначала выберите ключ в дереве");
                return;
            }

            if (fullPath == "HKCU" || fullPath == "HKLM") // Нельзя защитить корень
            {
                MessageBox.Show("Нельзя защитить корень ветки");
                return;
            }

            string hive = GetHiveFromPath(fullPath);  // Извлекает ветку
            string path = GetPathWithoutHive(fullPath, hive); // Извлекает путь без ветки

            // Открываем ключ для проверки текущих прав
            RegistryKey key = GetRegistryKey(hive).OpenSubKey(path, true);
            if (key == null) { MessageBox.Show("Ключ не найден"); return; }

            RegistrySecurity security = key.GetAccessControl(); // Получает права доступа
            bool isProtected = false; // Флаг наличия защиты

            // Проверяет, есть ли уже правило запрета удаления
            foreach (RegistryAccessRule rule in security.GetAccessRules(true, true, typeof(SecurityIdentifier)))
            {
                if (rule.RegistryRights == RegistryRights.Delete && rule.AccessControlType == AccessControlType.Deny)
                {
                    isProtected = true; // Защита уже стоит
                    break;
                }
            }
            key.Close(); // Закрывает ключ

            if (isProtected) // Если защита есть — снимаем
            {
                UnprotectKey(hive, path); // Снимает запрет на удаление
                MessageBox.Show("Защита снята с ключа: " + fullPath);
            }
            else // Если защиты нет — устанавливаем
            {
                ProtectKey(hive, path); // Устанавливает запрет на удаление
                MessageBox.Show("Защита установлена на ключ: " + fullPath);
            }
        }

        // Обработчик раскрытия узла дерева (ленивая загрузка)
        private void TreeViewRegistry_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.Count > 0 && e.Node.Nodes[0].Text != "...") return; // Уже загружен

            e.Node.Nodes.Clear(); // Очищает временные узлы

            try
            {
                string fullPath = e.Node.FullPath; // Полный путь узла
                string hive = GetHiveFromPath(fullPath);  // Получает ветку
                string path = GetPathWithoutHive(fullPath, hive); // Получает путь без ветки

                RegistryKey baseKey = GetRegistryKey(hive); // Корневой ключ
                RegistryKey key = (path == "") ? baseKey : baseKey.OpenSubKey(path); // Открывает ключ для чтения

                if (key == null) return;

                // Добавляет подключи
                foreach (string subKey in key.GetSubKeyNames()) // GetSubKeyNames — имена всех подключей
                {
                    TreeNode node = new TreeNode(subKey); // Создаёт узел
                    node.Nodes.Add("..."); // Добавляет заглушку
                    e.Node.Nodes.Add(node); // Добавляет узел в дерево
                }

                // Добавляет параметры
                foreach (string valueName in key.GetValueNames()) // GetValueNames — имена всех параметров
                {
                    TreeNode valueNode = new TreeNode("[Параметр] " + valueName); // Создаёт узел параметра
                    e.Node.Nodes.Add(valueNode); // Добавляет узел в дерево
                }
            }
            catch { MessageBox.Show("Ошибка открытия ветки"); }
        }

        // Обработчик выбора узла в дереве
        private void treeViewRegistry_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string fullPath = e.Node.FullPath; // Полный путь выбранного узла

            if (e.Node.Text.StartsWith("[Параметр] ")) // Если выбран параметр
            {
                string paramName = e.Node.Text.Replace("[Параметр] ", ""); // Убирает префикс
                textBoxName.Text = paramName; // Показывает имя параметра

                if (e.Node.Parent != null) // Parent — родительский узел
                    textBoxPath.Text = e.Node.Parent.FullPath; // Показывает путь родительского ключа
            }
            else // Если выбран ключ
            {
                textBoxPath.Text = fullPath; // Показывает полный путь ключа
                textBoxName.Text = "";       // Очищает поле имени параметра
            }
        }

        // Кнопка "Прочитать" — читает значение параметра из реестра
        private void buttonRead_Click(object sender, EventArgs e)
        {
            string fullPath = textBoxPath.Text; // Путь к ключу
            string name = textBoxName.Text;     // Имя параметра

            if (string.IsNullOrWhiteSpace(fullPath)) { MessageBox.Show("Выберите ключ в дереве"); return; }
            if (string.IsNullOrWhiteSpace(name)) { MessageBox.Show("Выберите параметр в дереве"); return; }

            string hive = GetHiveFromPath(fullPath);  // Извлекает ветку
            string path = GetPathWithoutHive(fullPath, hive); // Извлекает путь без ветки

            RegistryKey key = GetRegistryKey(hive)?.OpenSubKey(path); // OpenSubKey открывает ключ для чтения
            if (key == null) { MessageBox.Show("Ключ не найден"); return; }

            object value = key.GetValue(name); // GetValue читает значение параметра по имени
            MessageBox.Show(value == null ? "Параметр не найден" : "Значение: " + value);
            key.Close(); // Закрывает ключ
        }

        // Кнопка "Создать" — создаёт ключ или параметр в реестре
        private void buttonCreate_Click(object sender, EventArgs e)
        {
            string fullPath = textBoxPath.Text; // Путь к родительскому ключу
            string newKey = textBoxNKey.Text;   // Имя нового ключа (если нужно создать)
            string name = textBoxName.Text;     // Имя параметра
            string value = textBoxValue.Text;   // Значение параметра

            if (string.IsNullOrWhiteSpace(fullPath)) // Проверка пути
            {
                MessageBox.Show("Выберите ключ в дереве");
                return;
            }

            if (fullPath == "HKCU" || fullPath == "HKLM") // Проверка что не корень ветки
            {
                MessageBox.Show("Нельзя создавать в корне ветки");
                return;
            }

            if (string.IsNullOrWhiteSpace(name)) // Проверка имени параметра
            {
                MessageBox.Show("Введите имя параметра");
                return;
            }

            string hive = GetHiveFromPath(fullPath);
            string path = GetPathWithoutHive(fullPath, hive);

            // Если указано имя нового ключа — добавляем его к пути
            if (!string.IsNullOrWhiteSpace(newKey))
            {
                path = path + "\\" + newKey; // Собирает полный путь с новым ключом
            }

            RegistryKey key = GetRegistryKey(hive).CreateSubKey(path); // CreateSubKey создаёт или открывает ключ для записи
            key.SetValue(name, value); // SetValue устанавливает значение параметра
            key.Close(); // Закрывает ключ

            string message = string.IsNullOrWhiteSpace(newKey)
                ? "Параметр создан"
                : "Ключ и параметр созданы: " + path;

            MessageBox.Show(message);
            ReloadTree(); // Обновляет дерево
        }

        // Кнопка "Удалить" — удаляет ключ или параметр
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            string fullPath = textBoxPath.Text; // Путь к ключу
            string name = textBoxName.Text;     // Имя параметра

            if (string.IsNullOrWhiteSpace(fullPath)) { MessageBox.Show("Выберите ключ в дереве"); return; }
            if (fullPath == "HKCU" || fullPath == "HKLM") { MessageBox.Show("Нельзя удалить корень ветки"); return; }

            string hive = GetHiveFromPath(fullPath);
            string path = GetPathWithoutHive(fullPath, hive);

            try
            {
                if (!string.IsNullOrEmpty(name)) // Удаление параметра
                {
                    RegistryKey key = GetRegistryKey(hive).OpenSubKey(path, true); // true — открывает для записи
                    if (key != null) { key.DeleteValue(name); key.Close(); } // DeleteValue удаляет параметр
                    MessageBox.Show("Параметр удален");
                }
                else // Удаление ключа
                {
                    GetRegistryKey(hive).DeleteSubKeyTree(path); // DeleteSubKeyTree удаляет ключ со всем содержимым
                    MessageBox.Show("Ключ удален");
                }
                ReloadTree(); // Обновляет дерево
            }
            catch (Exception ex) { MessageBox.Show("Ошибка удаления: " + ex.Message); }
        }
private void label1_Click(object sender, EventArgs e) { }
private void label2_Click(object sender, EventArgs e) { }
    }
}
