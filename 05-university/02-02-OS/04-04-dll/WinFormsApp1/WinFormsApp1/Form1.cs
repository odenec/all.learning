using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private Type _calculatorType = null;
        private object _calculatorInstance = null;

        public Form1()
        {
            InitializeComponent();
            buttonLoad.Click += buttonLoad_Click;
            buttonCalc.Click += buttonCalc_Click;
            labelResult.Text = $"Результат:";
            labelStatus.Text = $"Библиотека не загружена";
            UpdateControlsState();

        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            string dllPath = Path.Combine(Application.StartupPath, "ClassLibrary1.dll");

            if (!File.Exists(dllPath))
            {
                labelStatus.Text = "Файл не найден";
                return;
            }

            try
            {
                Assembly asm = Assembly.LoadFile(dllPath);
                _calculatorType = asm.GetType("ClassLibrary1.Calculator");

                if (_calculatorType == null)
                {
                    labelStatus.Text = "Класс Calculator не найден в DLL";
                    return;
                }

                _calculatorInstance = Activator.CreateInstance(_calculatorType);


                labelStatus.Text = "V Библиотека загружена";

                UpdateControlsState();
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Ошибка загрузки";
                _calculatorType = null;
                _calculatorInstance = null;
                UpdateControlsState();
            }
        }

        private void buttonCalc_Click(object sender, EventArgs e)
        {
            if (_calculatorType == null || _calculatorInstance == null)
            {
                labelStatus.Text = "Сначала загрузите библиотеку.";
                return;
            }

            try
            {
                if (!double.TryParse(textBoxA.Text, out double a) || //parse tut
                    !double.TryParse(textBoxB.Text, out double b))
                {
                    labelStatus.Text = "Введите корректные числа.";
                    return;
                }

                MethodInfo addMethod = _calculatorType.GetMethod("Add"); //тащим МЕТОД
                object result = addMethod.Invoke(_calculatorInstance, new object[] { a, b }); //тут и предаём и получаем

                labelResult.Text = $"Результат: {result}";
            }
            catch (Exception ex)
            {
                labelStatus.Text = $"Ошибка вычисления: {ex.Message}";
            }
        }



        private void UpdateControlsState()
        {
            bool isLoaded = (_calculatorType != null && _calculatorInstance != null);
            textBoxA.Enabled = isLoaded;
            textBoxB.Enabled = isLoaded;
            buttonCalc.Enabled = isLoaded;
        }

        private void labelResult_Click(object sender, EventArgs e)
        { 
        }
    }
}