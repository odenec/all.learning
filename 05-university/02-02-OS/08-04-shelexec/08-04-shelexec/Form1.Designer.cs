namespace _08_04_shelexec
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
            buttonBrowse = new Button();
            buttonRun = new Button();
            checkBoxAdmin = new CheckBox();
            SuspendLayout();
            // 
            // textBoxPath
            // 
            textBoxPath.Location = new Point(68, 172);
            textBoxPath.Name = "textBoxPath";
            textBoxPath.Size = new Size(215, 23);
            textBoxPath.TabIndex = 0;
            // 
            // buttonBrowse
            // 
            buttonBrowse.Location = new Point(68, 220);
            buttonBrowse.Name = "buttonBrowse";
            buttonBrowse.Size = new Size(215, 23);
            buttonBrowse.TabIndex = 1;
            buttonBrowse.Text = "Выбрать файл";
            buttonBrowse.UseVisualStyleBackColor = true;
            // 
            // buttonRun
            // 
            buttonRun.Location = new Point(68, 249);
            buttonRun.Name = "buttonRun";
            buttonRun.Size = new Size(215, 23);
            buttonRun.TabIndex = 2;
            buttonRun.Text = "Запустить";
            buttonRun.UseVisualStyleBackColor = true;
            // 
            // checkBoxAdmin
            // 
            checkBoxAdmin.AutoSize = true;
            checkBoxAdmin.Location = new Point(68, 278);
            checkBoxAdmin.Name = "checkBoxAdmin";
            checkBoxAdmin.Size = new Size(132, 19);
            checkBoxAdmin.TabIndex = 3;
            checkBoxAdmin.Text = "от администратора";
            checkBoxAdmin.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(345, 450);
            Controls.Add(checkBoxAdmin);
            Controls.Add(buttonRun);
            Controls.Add(buttonBrowse);
            Controls.Add(textBoxPath);
            Name = "Form1";
            Text = "shellexec";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBoxPath;
        private Button buttonBrowse;
        private Button buttonRun;
        private CheckBox checkBoxAdmin;
    }
}
