using System;
using System.Drawing;
using System.Windows.Forms;

namespace CompressionProject
{
    public class ServerForm : Form
    {
        private CompressionServer server;
        private int connectionCount = 0;

        // ── Controls ──
        private Label lblTitle, lblPortLabel, lblStatus, lblClients;
        private TextBox txtPort;
        private Button btnStart, btnStop;
        private RichTextBox txtLog;

        public ServerForm()
        {
            this.Text = "Compression Server";
            this.Size = new Size(580, 480);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 45);
            this.ForeColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            BuildControls();
        }

        private void BuildControls()
        {
            // ── Title ──
            lblTitle = new Label();
            lblTitle.Text = "Multi-threaded Compression Server";
            lblTitle.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(0, 200, 160);
            lblTitle.Location = new Point(15, 15);
            lblTitle.Size = new Size(540, 30);
            this.Controls.Add(lblTitle);

            // ── Separator ──
            var sep = new Label();
            sep.BorderStyle = BorderStyle.Fixed3D;
            sep.Location = new Point(15, 52);
            sep.Size = new Size(540, 2);
            this.Controls.Add(sep);

            // ── Port ──
            lblPortLabel = new Label();
            lblPortLabel.Text = "Port:";
            lblPortLabel.ForeColor = Color.Silver;
            lblPortLabel.Font = new Font("Segoe UI", 10);
            lblPortLabel.Location = new Point(15, 65);
            lblPortLabel.AutoSize = true;
            this.Controls.Add(lblPortLabel);

            txtPort = new TextBox();
            txtPort.Text = "9000";
            txtPort.Font = new Font("Segoe UI", 10);
            txtPort.Location = new Point(60, 62);
            txtPort.Size = new Size(80, 25);
            txtPort.BackColor = Color.FromArgb(50, 50, 70);
            txtPort.ForeColor = Color.White;
            this.Controls.Add(txtPort);

            // ── Buttons ──
            btnStart = new Button();
            btnStart.Text = "▶  Start Server";
            btnStart.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnStart.Location = new Point(160, 60);
            btnStart.Size = new Size(140, 32);
            btnStart.BackColor = Color.FromArgb(0, 160, 110);
            btnStart.ForeColor = Color.White;
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.Cursor = Cursors.Hand;
            btnStart.Click += BtnStart_Click;
            this.Controls.Add(btnStart);

            btnStop = new Button();
            btnStop.Text = "■  Stop Server";
            btnStop.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnStop.Location = new Point(315, 60);
            btnStop.Size = new Size(140, 32);
            btnStop.BackColor = Color.FromArgb(180, 50, 50);
            btnStop.ForeColor = Color.White;
            btnStop.FlatStyle = FlatStyle.Flat;
            btnStop.FlatAppearance.BorderSize = 0;
            btnStop.Cursor = Cursors.Hand;
            btnStop.Enabled = false;
            btnStop.Click += BtnStop_Click;
            this.Controls.Add(btnStop);

            // ── Status ──
            lblStatus = new Label();
            lblStatus.Text = "● Stopped";
            lblStatus.ForeColor = Color.OrangeRed;
            lblStatus.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblStatus.Location = new Point(15, 105);
            lblStatus.AutoSize = true;
            this.Controls.Add(lblStatus);

            lblClients = new Label();
            lblClients.Text = "Clients served: 0";
            lblClients.ForeColor = Color.Silver;
            lblClients.Font = new Font("Segoe UI", 9);
            lblClients.Location = new Point(160, 105);
            lblClients.AutoSize = true;
            this.Controls.Add(lblClients);

            // ── Log Label ──
            var lblLog = new Label();
            lblLog.Text = "Message Log:";
            lblLog.ForeColor = Color.Silver;
            lblLog.Font = new Font("Segoe UI", 9);
            lblLog.Location = new Point(15, 130);
            lblLog.AutoSize = true;
            this.Controls.Add(lblLog);

            // ── Log Box ──
            txtLog = new RichTextBox();
            txtLog.Location = new Point(15, 152);
            txtLog.Size = new Size(540, 280);
            txtLog.BackColor = Color.FromArgb(15, 15, 25);
            txtLog.ForeColor = Color.FromArgb(0, 210, 160);
            txtLog.Font = new Font("Consolas", 9);
            txtLog.ReadOnly = true;
            txtLog.BorderStyle = BorderStyle.FixedSingle;
            txtLog.ScrollBars = RichTextBoxScrollBars.Vertical;
            this.Controls.Add(txtLog);

            AppendLog("مرحباً! اضغط Start لتشغيل السيرفر.");
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtPort.Text, out int port))
            {
                MessageBox.Show("Port غير صحيح!", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            server = new CompressionServer(port);
            server.OnLog += AppendLog;
            server.Start();

            btnStart.Enabled = false;
            btnStop.Enabled = true;
            txtPort.Enabled = false;
            SetStatus("● Running", Color.FromArgb(0, 210, 110));
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            server?.Stop();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            txtPort.Enabled = true;
            SetStatus("● Stopped", Color.OrangeRed);
        }

        private void AppendLog(string msg)
        {
            if (txtLog.InvokeRequired) { txtLog.Invoke(new Action(() => AppendLog(msg))); return; }

            if (msg.Contains("عميل جديد"))
            {
                connectionCount++;
                lblClients.Text = "Clients served: " + connectionCount;
            }

            txtLog.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + msg + "\n");
            txtLog.ScrollToCaret();
        }

        private void SetStatus(string text, Color color)
        {
            if (lblStatus.InvokeRequired)
                lblStatus.Invoke(new Action(() => { lblStatus.Text = text; lblStatus.ForeColor = color; }));
            else
            { lblStatus.Text = text; lblStatus.ForeColor = color; }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            server?.Stop();
            base.OnFormClosing(e);
        }
    }
}