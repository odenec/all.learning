namespace WinFormsApp1
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
            buttonLoad = new Button();
            textBoxA = new TextBox();
            textBoxB = new TextBox();
            buttonCalc = new Button();
            labelStatus = new Label();
            labelResult = new Label();
            SuspendLayout();
            // 
            // buttonLoad
            // 
            buttonLoad.Location = new Point(187, 260);
            buttonLoad.Name = "buttonLoad";
            buttonLoad.Size = new Size(194, 23);
            buttonLoad.TabIndex = 0;
            buttonLoad.Text = "Загрузить библиотеку";
            buttonLoad.UseVisualStyleBackColor = true;
            // 
            // textBoxA
            // 
            textBoxA.Enabled = false;
            textBoxA.Location = new Point(134, 175);
            textBoxA.Name = "textBoxA";
            textBoxA.Size = new Size(100, 23);
            textBoxA.TabIndex = 1;
            // 
            // textBoxB
            // 
            textBoxB.Enabled = false;
            textBoxB.Location = new Point(352, 176);
            textBoxB.Name = "textBoxB";
            textBoxB.Size = new Size(100, 23);
            textBoxB.TabIndex = 2;
            // 
            // buttonCalc
            // 
            buttonCalc.Enabled = false;
            buttonCalc.Location = new Point(263, 175);
            buttonCalc.Name = "buttonCalc";
            buttonCalc.Size = new Size(65, 23);
            buttonCalc.TabIndex = 3;
            buttonCalc.Text = "Сложить";
            buttonCalc.UseVisualStyleBackColor = true;
            // 
            // labelStatus
            // 
            labelStatus.AutoSize = true;
            labelStatus.Location = new Point(196, 295);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(38, 15);
            labelStatus.TabIndex = 4;
            labelStatus.Text = "label1";
            // 
            // labelResult
            // 
            labelResult.AutoSize = true;
            labelResult.Location = new Point(482, 179);
            labelResult.Name = "labelResult";
            labelResult.Size = new Size(38, 15);
            labelResult.TabIndex = 5;
            labelResult.Text = "label1";
            labelResult.Click += labelResult_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(labelResult);
            Controls.Add(labelStatus);
            Controls.Add(buttonCalc);
            Controls.Add(textBoxB);
            Controls.Add(textBoxA);
            Controls.Add(buttonLoad);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonLoad;
        private TextBox textBoxA;
        private TextBox textBoxB;
        private Button buttonCalc;
        private Label labelStatus;
        private Label labelResult;
    }
}
