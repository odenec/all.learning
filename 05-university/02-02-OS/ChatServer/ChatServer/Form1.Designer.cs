namespace ChatServer
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
            lbClients = new ListBox();
            tbLog = new TextBox();
            btnKick = new Button();
            label1 = new Label();
            SuspendLayout();
            // 
            // lbClients
            // 
            lbClients.FormattingEnabled = true;
            lbClients.Location = new Point(63, 39);
            lbClients.Name = "lbClients";
            lbClients.Size = new Size(177, 79);
            lbClients.TabIndex = 0;
            // 
            // tbLog
            // 
            tbLog.Location = new Point(63, 133);
            tbLog.Multiline = true;
            tbLog.Name = "tbLog";
            tbLog.ReadOnly = true;
            tbLog.Size = new Size(177, 286);
            tbLog.TabIndex = 1;
            // 
            // btnKick
            // 
            btnKick.Location = new Point(246, 39);
            btnKick.Name = "btnKick";
            btnKick.Size = new Size(75, 23);
            btnKick.TabIndex = 2;
            btnKick.Text = "Kick";
            btnKick.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(63, 21);
            label1.Name = "label1";
            label1.Size = new Size(97, 15);
            label1.TabIndex = 3;
            label1.Text = "Подключённые:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(334, 450);
            Controls.Add(label1);
            Controls.Add(btnKick);
            Controls.Add(tbLog);
            Controls.Add(lbClients);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox lbClients;
        private TextBox tbLog;
        private Button btnKick;
        private Label label1;
    }
}
