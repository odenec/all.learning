namespace HookLoader
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
            btnStart = new Button();
            btnStop = new Button();
            btnLoad = new Button();
            txtLog = new TextBox();
            lblStatus = new Label();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Location = new Point(123, 69);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(135, 23);
            btnStart.TabIndex = 0;
            btnStart.Text = "Запустить хук";
            btnStart.UseVisualStyleBackColor = true;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(123, 111);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(135, 23);
            btnStop.TabIndex = 1;
            btnStop.Text = "Остановить хук";
            btnStop.UseVisualStyleBackColor = true;
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(123, 152);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(135, 23);
            btnLoad.TabIndex = 2;
            btnLoad.Text = "Загрузить DLL";
            btnLoad.UseVisualStyleBackColor = true;
            // 
            // txtLog
            // 
            txtLog.Location = new Point(282, 70);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(254, 105);
            txtLog.TabIndex = 3;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(358, 178);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(103, 15);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "DLL не загружена";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lblStatus);
            Controls.Add(txtLog);
            Controls.Add(btnLoad);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnStart;
        private Button btnStop;
        private Button btnLoad;
        private TextBox txtLog;
        private Label lblStatus;
    }
}
