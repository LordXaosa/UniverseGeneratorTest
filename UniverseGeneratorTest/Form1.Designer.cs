namespace UniverseGeneratorTest
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.universePanel = new System.Windows.Forms.Panel();
            this.cyclesTb = new System.Windows.Forms.TextBox();
            this.startBt = new System.Windows.Forms.Button();
            this.timeLbl = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // universePanel
            // 
            this.universePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.universePanel.AutoScroll = true;
            this.universePanel.BackColor = System.Drawing.Color.White;
            this.universePanel.Location = new System.Drawing.Point(13, 13);
            this.universePanel.Name = "universePanel";
            this.universePanel.Size = new System.Drawing.Size(941, 540);
            this.universePanel.TabIndex = 0;
            this.universePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.universePanel_Paint);
            // 
            // cyclesTb
            // 
            this.cyclesTb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cyclesTb.Location = new System.Drawing.Point(12, 559);
            this.cyclesTb.Name = "cyclesTb";
            this.cyclesTb.Size = new System.Drawing.Size(100, 20);
            this.cyclesTb.TabIndex = 1;
            this.cyclesTb.Text = "1000";
            // 
            // startBt
            // 
            this.startBt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.startBt.Location = new System.Drawing.Point(118, 557);
            this.startBt.Name = "startBt";
            this.startBt.Size = new System.Drawing.Size(75, 23);
            this.startBt.TabIndex = 2;
            this.startBt.Text = "Начать";
            this.startBt.UseVisualStyleBackColor = true;
            this.startBt.Click += new System.EventHandler(this.startBt_Click);
            // 
            // timeLbl
            // 
            this.timeLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.timeLbl.AutoSize = true;
            this.timeLbl.Location = new System.Drawing.Point(200, 562);
            this.timeLbl.Name = "timeLbl";
            this.timeLbl.Size = new System.Drawing.Size(0, 13);
            this.timeLbl.TabIndex = 3;
            // 
            // timer1
            // 
            this.timer1.Interval = 1;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(966, 591);
            this.Controls.Add(this.timeLbl);
            this.Controls.Add(this.startBt);
            this.Controls.Add(this.cyclesTb);
            this.Controls.Add(this.universePanel);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel universePanel;
        private System.Windows.Forms.TextBox cyclesTb;
        private System.Windows.Forms.Button startBt;
        private System.Windows.Forms.Label timeLbl;
        private System.Windows.Forms.Timer timer1;
    }
}

