namespace _06_04_registry
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
            textBoxPath = new TextBox();
            textBoxName = new TextBox();
            textBoxValue = new TextBox();
            treeViewRegistry = new TreeView();
            buttonRead = new Button();
            buttonCreate = new Button();
            buttonDelete = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            buttonProtect = new Button();
            textBoxNKey = new TextBox();
            label4 = new Label();
            SuspendLayout();
            // 
            // textBoxPath
            // 
            textBoxPath.Location = new Point(74, 212);
            textBoxPath.Name = "textBoxPath";
            textBoxPath.Size = new Size(226, 23);
            textBoxPath.TabIndex = 1;
            // 
            // textBoxName
            // 
            textBoxName.Location = new Point(107, 280);
            textBoxName.Name = "textBoxName";
            textBoxName.Size = new Size(193, 23);
            textBoxName.TabIndex = 2;
            // 
            // textBoxValue
            // 
            textBoxValue.Location = new Point(74, 319);
            textBoxValue.Name = "textBoxValue";
            textBoxValue.Size = new Size(226, 23);
            textBoxValue.TabIndex = 3;
            // 
            // treeViewRegistry
            // 
            treeViewRegistry.Location = new Point(12, 34);
            treeViewRegistry.Name = "treeViewRegistry";
            treeViewRegistry.Size = new Size(351, 172);
            treeViewRegistry.TabIndex = 5;
            // 
            // buttonRead
            // 
            buttonRead.Location = new Point(55, 348);
            buttonRead.Name = "buttonRead";
            buttonRead.Size = new Size(75, 23);
            buttonRead.TabIndex = 7;
            buttonRead.Text = "Считать";
            buttonRead.UseVisualStyleBackColor = true;
            // 
            // buttonCreate
            // 
            buttonCreate.Location = new Point(155, 348);
            buttonCreate.Name = "buttonCreate";
            buttonCreate.Size = new Size(75, 23);
            buttonCreate.TabIndex = 8;
            buttonCreate.Text = "Создать";
            buttonCreate.UseVisualStyleBackColor = true;
            // 
            // buttonDelete
            // 
            buttonDelete.Location = new Point(245, 348);
            buttonDelete.Name = "buttonDelete";
            buttonDelete.Size = new Size(75, 23);
            buttonDelete.TabIndex = 9;
            buttonDelete.Text = "Удалить";
            buttonDelete.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 215);
            label1.Name = "label1";
            label1.Size = new Size(33, 15);
            label1.TabIndex = 10;
            label1.Text = "Путь";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(8, 288);
            label2.Name = "label2";
            label2.Size = new Size(93, 15);
            label2.TabIndex = 11;
            label2.Text = "Имя параметра";
            label2.Click += label2_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(8, 322);
            label3.Name = "label3";
            label3.Size = new Size(60, 15);
            label3.TabIndex = 12;
            label3.Text = "Значение";
            // 
            // buttonProtect
            // 
            buttonProtect.Location = new Point(100, 390);
            buttonProtect.Name = "buttonProtect";
            buttonProtect.Size = new Size(175, 23);
            buttonProtect.TabIndex = 13;
            buttonProtect.Text = "Защитить от удаления";
            buttonProtect.UseVisualStyleBackColor = true;
            // 
            // textBoxNKey
            // 
            textBoxNKey.Location = new Point(107, 241);
            textBoxNKey.Name = "textBoxNKey";
            textBoxNKey.Size = new Size(193, 23);
            textBoxNKey.TabIndex = 14;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(8, 244);
            label4.Name = "label4";
            label4.Size = new Size(70, 15);
            label4.TabIndex = 15;
            label4.Text = "Имя ключа";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(375, 450);
            Controls.Add(label4);
            Controls.Add(textBoxNKey);
            Controls.Add(buttonProtect);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(buttonDelete);
            Controls.Add(buttonCreate);
            Controls.Add(buttonRead);
            Controls.Add(treeViewRegistry);
            Controls.Add(textBoxValue);
            Controls.Add(textBoxName);
            Controls.Add(textBoxPath);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox textBoxPath;
        private TextBox textBoxName;
        private TextBox textBoxValue;
        private TreeView treeViewRegistry;
        private Button buttonRead;
        private Button buttonCreate;
        private Button buttonDelete;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button buttonProtect;
        private TextBox textBoxNKey;
        private Label label4;
    }
}
