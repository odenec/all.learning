namespace _02_04_workerThread
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
            listBoxLog = new ListBox();
            btnStartPause = new Button();
            btnStop = new Button();
            cbPriority = new ComboBox();
            SuspendLayout();
            // 
            // listBoxLog
            // 
            listBoxLog.FormattingEnabled = true;
            listBoxLog.Location = new Point(26, 92);
            listBoxLog.Name = "listBoxLog";
            listBoxLog.Size = new Size(403, 139);
            listBoxLog.TabIndex = 0;
            // 
            // btnStartPause
            // 
            btnStartPause.Location = new Point(204, 286);
            btnStartPause.Name = "btnStartPause";
            btnStartPause.Size = new Size(75, 23);
            btnStartPause.TabIndex = 1;
            btnStartPause.Text = "start/pause";
            btnStartPause.UseVisualStyleBackColor = true;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(309, 286);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(75, 23);
            btnStop.TabIndex = 2;
            btnStop.Text = "stop";
            btnStop.UseVisualStyleBackColor = true;
            // 
            // cbPriority
            // 
            cbPriority.FormattingEnabled = true;
            cbPriority.Items.AddRange(new object[] { "Low, Normal, High" });
            cbPriority.Location = new Point(435, 92);
            cbPriority.Name = "cbPriority";
            cbPriority.Size = new Size(185, 23);
            cbPriority.TabIndex = 3;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(cbPriority);
            Controls.Add(btnStop);
            Controls.Add(btnStartPause);
            Controls.Add(listBoxLog);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private ListBox listBoxLog;
        private Button btnStartPause;
        private Button btnStop;
        private ComboBox cbPriority;
    }
}
