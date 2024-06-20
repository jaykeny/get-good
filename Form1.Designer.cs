using System.Drawing;
using System.Windows.Forms;

namespace Get_Good
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private TextBox txtVoice;
        private TextBox txtChat;
        private Label lblVoice;
        private Label lblChat;

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            notifyIcon1 = new NotifyIcon(components);
            btnStart = new Button();
            btnStop = new Button();
            txtVoice = new TextBox();
            txtChat = new TextBox();
            lblVoice = new Label();
            lblChat = new Label();
            label1 = new Label();
            SuspendLayout();
            // 
            // notifyIcon1
            // 
            notifyIcon1.Text = "Process Manager";
            notifyIcon1.Visible = true;
            notifyIcon1.Click += notifyIcon1_Click;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(54, 242);
            btnStart.Margin = new Padding(4, 3, 4, 3);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(127, 27);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(426, 242);
            btnStop.Margin = new Padding(4, 3, 4, 3);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(131, 27);
            btnStop.TabIndex = 1;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // txtVoice
            // 
            txtVoice.AccessibleName = "";
            txtVoice.Location = new Point(54, 157);
            txtVoice.Margin = new Padding(4, 3, 4, 3);
            txtVoice.Name = "txtVoice";
            txtVoice.Size = new Size(233, 23);
            txtVoice.TabIndex = 0;
            // 
            // txtChat
            // 
            txtChat.Location = new Point(323, 157);
            txtChat.Margin = new Padding(4, 3, 4, 3);
            txtChat.Name = "txtChat";
            txtChat.Size = new Size(233, 23);
            txtChat.TabIndex = 1;
            // 
            // lblVoice
            // 
            lblVoice.AutoSize = true;
            lblVoice.ForeColor = Color.White;
            lblVoice.Location = new Point(54, 134);
            lblVoice.Margin = new Padding(4, 0, 4, 0);
            lblVoice.Name = "lblVoice";
            lblVoice.Size = new Size(72, 15);
            lblVoice.TabIndex = 0;
            lblVoice.Text = "Game Input:";
            // 
            // lblChat
            // 
            lblChat.AutoSize = true;
            lblChat.ForeColor = Color.White;
            lblChat.Location = new Point(323, 134);
            lblChat.Margin = new Padding(4, 0, 4, 0);
            lblChat.Name = "lblChat";
            lblChat.Size = new Size(66, 15);
            lblChat.TabIndex = 1;
            lblChat.Text = "Chat Input:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft Sans Serif", 36F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = SystemColors.Window;
            label1.Location = new Point(46, 24);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(240, 55);
            label1.TabIndex = 2;
            label1.Text = "Get Good";
            label1.TextAlign = ContentAlignment.TopCenter;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(615, 301);
            Controls.Add(label1);
            Controls.Add(lblVoice);
            Controls.Add(lblChat);
            Controls.Add(txtVoice);
            Controls.Add(txtChat);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            Name = "Form1";
            Text = "Get Good";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            Resize += Form1_Resize;
            ResumeLayout(false);
            PerformLayout();
        }

        private void StyleButtons()
        {
            // Style btnStart
            btnStart.BackColor = Color.FromArgb(34, 136, 204); // Blue color
            btnStart.ForeColor = Color.White;
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.Font = new Font("Arial", 10, FontStyle.Bold);
            btnStart.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 102, 204); // Darker blue when pressed

            // Style btnStop
            btnStop.BackColor = Color.FromArgb(220, 53, 69); // Red color
            btnStop.ForeColor = Color.White;
            btnStop.FlatStyle = FlatStyle.Flat;
            btnStop.FlatAppearance.BorderSize = 0;
            btnStop.Font = new Font("Arial", 10, FontStyle.Bold);
            btnStop.FlatAppearance.MouseDownBackColor = Color.FromArgb(185, 28, 49); // Darker red when pressed
        }

        private void AddExitButton()
        {
            // Create exit button
            Button btnExit = new Button();
            btnExit.Text = "X"; // Close symbol
            btnExit.BackColor = Color.FromArgb(220, 53, 69); // Red color
            btnExit.ForeColor = Color.White;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Font = new Font("Arial", 10, FontStyle.Bold);
            btnExit.FlatAppearance.MouseDownBackColor = Color.FromArgb(185, 28, 49); // Darker red when pressed
            btnExit.Size = new Size(30, 30);
            btnExit.Location = new Point(this.Width - btnExit.Width - 10, 10); // Adjust positioning

            // Handle click event
            btnExit.Click += (sender, e) =>
            {
                this.Close(); // Close the form
            };

            // Add button to form controls
            this.Controls.Add(btnExit);
        }

        private Label label1;
    }
}
