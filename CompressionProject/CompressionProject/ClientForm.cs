using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CompressionProject
{
    public class ClientForm : Form
    {
        private CompressionClient client;

        // ── Controls ──
        private Label lblTitle, lblIPLabel, lblPortLabel, lblFileLabel, lblStatus;
        private TextBox txtIP, txtPort, txtFilePath;
        private Button btnBrowse, btnSend;
        private ProgressBar progressBar;
        private Label lblPercent;
        private RichTextBox txtLog;

        public ClientForm()
        {
            this.Text = "Compression Client";
            this.Size = new Size(620, 560);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(28, 28, 42);
            this.ForeColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            BuildControls();

            client = new CompressionClient();
            client.OnLog += AppendLog;
            client.OnProgress += UpdateProgress;
            client.OnDone += OnDone;
        }

        private void BuildControls()
        {
            // ── Title ──
            lblTitle = new Label();
            lblTitle.Text = "File Compression Client";
            lblTitle.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(100, 149, 255);
            lblTitle.Location = new Point(15, 15);
            lblTitle.Size = new Size(580, 30);
            this.Controls.Add(lblTitle);

            var sep = new Label();
            sep.BorderStyle = BorderStyle.Fixed3D;
            sep.Location = new Point(15, 52);
            sep.Size = new Size(570, 2);
            this.Controls.Add(sep);

            // ── Server IP ──
            lblIPLabel = new Label();
            lblIPLabel.Text = "Server IP:";
            lblIPLabel.ForeColor = Color.Silver;
            lblIPLabel.Font = new Font("Segoe UI", 10);
            lblIPLabel.Location = new Point(15, 68);
            lblIPLabel.AutoSize = true;
            this.Controls.Add(lblIPLabel);

            txtIP = new TextBox();
            txtIP.Text = "127.0.0.1";
            txtIP.Font = new Font("Segoe UI", 10);
            txtIP.Location = new Point(95, 65);
            txtIP.Size = new Size(140, 25);
            txtIP.BackColor = Color.FromArgb(50, 50, 70);
            txtIP.ForeColor = Color.White;
            this.Controls.Add(txtIP);

            // ── Port ──
            lblPortLabel = new Label();
            lblPortLabel.Text = "Port:";
            lblPortLabel.ForeColor = Color.Silver;
            lblPortLabel.Font = new Font("Segoe UI", 10);
            lblPortLabel.Location = new Point(255, 68);
            lblPortLabel.AutoSize = true;
            this.Controls.Add(lblPortLabel);

            txtPort = new TextBox();
            txtPort.Text = "9000";
            txtPort.Font = new Font("Segoe UI", 10);
            txtPort.Location = new Point(298, 65);
            txtPort.Size = new Size(75, 25);
            txtPort.BackColor = Color.FromArgb(50, 50, 70);
            txtPort.ForeColor = Color.White;
            this.Controls.Add(txtPort);

            // ── File Selection ──
            lblFileLabel = new Label();
            lblFileLabel.Text = "الملف:";
            lblFileLabel.ForeColor = Color.Silver;
            lblFileLabel.Font = new Font("Segoe UI", 10);
            lblFileLabel.Location = new Point(15, 108);
            lblFileLabel.AutoSize = true;
            this.Controls.Add(lblFileLabel);

            txtFilePath = new TextBox();
            txtFilePath.Font = new Font("Segoe UI", 9);
            txtFilePath.Location = new Point(65, 105);
            txtFilePath.Size = new Size(390, 25);
            txtFilePath.BackColor = Color.FromArgb(50, 50, 70);
            txtFilePath.ForeColor = Color.White;
            txtFilePath.ReadOnly = true;
            this.Controls.Add(txtFilePath);

            btnBrowse = new Button();
            btnBrowse.Text = "📂 Browse";
            btnBrowse.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnBrowse.Location = new Point(468, 103);
            btnBrowse.Size = new Size(110, 28);
            btnBrowse.BackColor = Color.FromArgb(60, 80, 180);
            btnBrowse.ForeColor = Color.White;
            btnBrowse.FlatStyle = FlatStyle.Flat;
            btnBrowse.FlatAppearance.BorderSize = 0;
            btnBrowse.Cursor = Cursors.Hand;
            btnBrowse.Click += BtnBrowse_Click;
            this.Controls.Add(btnBrowse);

            // ── Send Button ──
            btnSend = new Button();
            btnSend.Text = "🚀  إرسال وضغط";
            btnSend.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnSend.Location = new Point(15, 148);
            btnSend.Size = new Size(200, 38);
            btnSend.BackColor = Color.FromArgb(0, 155, 100);
            btnSend.ForeColor = Color.White;
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.Cursor = Cursors.Hand;
            btnSend.Click += BtnSend_Click;
            this.Controls.Add(btnSend);

            // ── Status ──
            lblStatus = new Label();
            lblStatus.Text = "جاهز";
            lblStatus.ForeColor = Color.Silver;
            lblStatus.Font = new Font("Segoe UI", 9);
            lblStatus.Location = new Point(228, 160);
            lblStatus.AutoSize = true;
            this.Controls.Add(lblStatus);

            // ── Progress Bar ──
            progressBar = new ProgressBar();
            progressBar.Location = new Point(15, 198);
            progressBar.Size = new Size(570, 22);
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Style = ProgressBarStyle.Continuous;
            this.Controls.Add(progressBar);

            lblPercent = new Label();
            lblPercent.Text = "0%";
            lblPercent.ForeColor = Color.Silver;
            lblPercent.Font = new Font("Segoe UI", 9);
            lblPercent.Location = new Point(15, 224);
            lblPercent.AutoSize = true;
            this.Controls.Add(lblPercent);

            // ── Log ──
            var lblLog = new Label();
            lblLog.Text = "Message Log:";
            lblLog.ForeColor = Color.Silver;
            lblLog.Font = new Font("Segoe UI", 9);
            lblLog.Location = new Point(15, 248);
            lblLog.AutoSize = true;
            this.Controls.Add(lblLog);

            txtLog = new RichTextBox();
            txtLog.Location = new Point(15, 270);
            txtLog.Size = new Size(570, 245);
            txtLog.BackColor = Color.FromArgb(15, 15, 25);
            txtLog.ForeColor = Color.FromArgb(100, 149, 255);
            txtLog.Font = new Font("Consolas", 9);
            txtLog.ReadOnly = true;
            txtLog.BorderStyle = BorderStyle.FixedSingle;
            txtLog.ScrollBars = RichTextBoxScrollBars.Vertical;
            this.Controls.Add(txtLog);

            AppendLog("مرحباً! اختر ملف واضغط إرسال.");
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "اختر الملف المراد ضغطه";
                dlg.Filter = "All Files (*.*)|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtFilePath.Text = dlg.FileName;
                    long sz = new FileInfo(dlg.FileName).Length;
                    AppendLog("✔ تم اختيار: " + Path.GetFileName(dlg.FileName) +
                              " (" + FormatSize(sz) + ")");
                }
            }
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilePath.Text))
            {
                MessageBox.Show("من فضلك اختر ملفاً أولاً!", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(txtPort.Text, out int port))
            {
                MessageBox.Show("Port غير صحيح!", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnSend.Enabled = false;
            progressBar.Value = 0;
            lblPercent.Text = "0%";
            SetStatus("جاري الإرسال...");

            client.SendFile(txtIP.Text.Trim(), port, txtFilePath.Text);
        }

        private void UpdateProgress(int value)
        {
            if (progressBar.InvokeRequired) { progressBar.Invoke(new Action(() => UpdateProgress(value))); return; }
            progressBar.Value = Math.Min(value, 100);
            lblPercent.Text = value + "%";
        }

        private void OnDone(string savedPath)
        {
            if (this.InvokeRequired) { this.Invoke(new Action(() => OnDone(savedPath))); return; }

            btnSend.Enabled = true;
            SetStatus("✔ اكتمل!");

            var res = MessageBox.Show(
                "✔ تم الضغط بنجاح!\n\nالملف المضغوط:\n" + savedPath + "\n\nهل تريد فتح المجلد؟",
                "اكتمل",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (res == DialogResult.Yes)
                System.Diagnostics.Process.Start("explorer.exe", "/select,\"" + savedPath + "\"");
        }

        private void AppendLog(string msg)
        {
            if (txtLog.InvokeRequired) { txtLog.Invoke(new Action(() => AppendLog(msg))); return; }
            txtLog.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + msg + "\n");
            txtLog.ScrollToCaret();
        }

        private void SetStatus(string text)
        {
            if (lblStatus.InvokeRequired)
                lblStatus.Invoke(new Action(() => lblStatus.Text = text));
            else
                lblStatus.Text = text;
        }

        private string FormatSize(long bytes)
        {
            if (bytes < 1024) return bytes + " B";
            if (bytes < 1024 * 1024) return (bytes / 1024.0).ToString("F1") + " KB";
            return (bytes / (1024.0 * 1024)).ToString("F1") + " MB";
        }
    }
}