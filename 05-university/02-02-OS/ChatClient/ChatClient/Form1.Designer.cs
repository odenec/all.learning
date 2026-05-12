namespace ChatClient
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
            tbServerIP = new TextBox();
            tbNick = new TextBox();
            tbMessage = new TextBox();
            btnSend = new Button();
            btnConnect = new Button();
            lbChat = new ListBox();
            SuspendLayout();
            // 
            // tbServerIP
            // 
            tbServerIP.Location = new Point(95, 28);
            tbServerIP.Name = "tbServerIP";
            tbServerIP.Size = new Size(100, 23);
            tbServerIP.TabIndex = 0;
            tbServerIP.Text = "127.0.0.1";
            // 
            // tbNick
            // 
            tbNick.Location = new Point(201, 28);
            tbNick.Name = "tbNick";
            tbNick.Size = new Size(100, 23);
            tbNick.TabIndex = 1;
            tbNick.Text = "User";
            // 
            // tbMessage
            // 
            tbMessage.Enabled = false;
            tbMessage.Location = new Point(95, 84);
            tbMessage.Name = "tbMessage";
            tbMessage.Size = new Size(658, 23);
            tbMessage.TabIndex = 2;
            // 
            // btnSend
            // 
            btnSend.Enabled = false;
            btnSend.Location = new Point(95, 127);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(100, 23);
            btnSend.TabIndex = 3;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(320, 27);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(75, 23);
            btnConnect.TabIndex = 4;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            // 
            // lbChat
            // 
            lbChat.FormattingEnabled = true;
            lbChat.HorizontalScrollbar = true;
            lbChat.Location = new Point(95, 173);
            lbChat.Name = "lbChat";
            lbChat.Size = new Size(658, 259);
            lbChat.TabIndex = 5;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lbChat);
            Controls.Add(btnConnect);
            Controls.Add(btnSend);
            Controls.Add(tbMessage);
            Controls.Add(tbNick);
            Controls.Add(tbServerIP);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tbServerIP;
        private TextBox tbNick;
        private TextBox tbMessage;
        private Button btnSend;
        private Button btnConnect;
        private ListBox lbChat;
    }
}
