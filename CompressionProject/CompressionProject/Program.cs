using System;
using System.Windows.Forms;

namespace CompressionProject
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var choice = MessageBox.Show(
                "اختر الدور:\n\nYES  → تشغيل كـ SERVER\nNO   → تشغيل كـ CLIENT",
                "Compression Project",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (choice == DialogResult.Yes)
                Application.Run(new ServerForm());
            else
                Application.Run(new ClientForm());
        }
    }
}