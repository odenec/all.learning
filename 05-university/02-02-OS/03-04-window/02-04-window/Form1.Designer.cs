namespace _02_04_window
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            listBox1 = new ListBox();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            textBox1 = new TextBox();
            SuspendLayout();
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.Location = new Point(68, 54);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(281, 94);
            listBox1.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(62, 173);
            button1.Name = "button1";
            button1.Size = new Size(287, 23);
            button1.TabIndex = 1;
            button1.Text = "Обновить список окон";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(62, 218);
            button2.Name = "button2";
            button2.Size = new Size(287, 23);
            button2.TabIndex = 2;
            button2.Text = "Свернуть окно";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(62, 265);
            button3.Name = "button3";
            button3.Size = new Size(287, 23);
            button3.TabIndex = 3;
            button3.Text = "Развернуть окно";
            button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Location = new Point(62, 310);
            button4.Name = "button4";
            button4.Size = new Size(287, 23);
            button4.TabIndex = 4;
            button4.Text = "Закрыть окно";
            button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            button5.Location = new Point(68, 388);
            button5.Name = "button5";
            button5.Size = new Size(281, 23);
            button5.TabIndex = 5;
            button5.Text = "Перетменовть";
            button5.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(68, 359);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "Новое имя окна";
            textBox1.Size = new Size(281, 23);
            textBox1.TabIndex = 6;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(textBox1);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(listBox1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox listBox1;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private TextBox textBox1;
    }
}
