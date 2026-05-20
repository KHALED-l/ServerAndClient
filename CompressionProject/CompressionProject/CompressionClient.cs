using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CompressionProject
{
    public class CompressionClient
    {
        public event Action<string> OnLog;
        public event Action<int> OnProgress;
        public event Action<string> OnDone;

        public void SendFile(string serverIP, int serverPort, string filePath)
        {
            Thread t = new Thread(() => DoSendFile(serverIP, serverPort, filePath));
            t.IsBackground = true;
            t.Start();
        }

        private void DoSendFile(string serverIP, int serverPort, string filePath)
        {
            TcpClient client = null;
            try
            {
                byte[] fileData = File.ReadAllBytes(filePath);
                long fileSize = fileData.Length;
                string fileName = Path.GetFileName(filePath);
                byte[] nameBytes = Encoding.UTF8.GetBytes(fileName);

                Log("→ جاري الاتصال بـ " + serverIP + ":" + serverPort + "...");
                client = new TcpClient();
                client.Connect(serverIP, serverPort);
                NetworkStream stream = client.GetStream();
                Log("✔ تم الاتصال!");

               
                stream.Write(BitConverter.GetBytes(fileSize), 0, 8);

                stream.Write(BitConverter.GetBytes(nameBytes.Length), 0, 4);
                stream.Write(nameBytes, 0, nameBytes.Length);

                
                Log("→ جاري الإرسال (" + FormatSize(fileSize) + ")...");
                int chunkSize = 8192, sent = 0;
                while (sent < fileData.Length)
                {
                    int toSend = Math.Min(chunkSize, fileData.Length - sent);
                    stream.Write(fileData, sent, toSend);
                    sent += toSend;
                    OnProgress?.Invoke((int)((double)sent / fileData.Length * 50));
                }
                stream.Flush();
                Log("✔ تم الإرسال! في انتظار الملف المضغوط...");

               
                byte[] compSizeBuf = new byte[8];
                ReadExact(stream, compSizeBuf, 8);
                long compSize = BitConverter.ToInt64(compSizeBuf, 0);
                Log("← حجم الملف المضغوط: " + FormatSize(compSize));

               
                byte[] compData = new byte[compSize];
                int received = 0;
                while (received < compSize)
                {
                    int toRead = (int)Math.Min(8192, compSize - received);
                    int r = stream.Read(compData, received, toRead);
                    if (r == 0) break;
                    received += r;
                    OnProgress?.Invoke(50 + (int)((double)received / compSize * 50));
                }

              
                string savePath = Path.Combine(
                    Path.GetDirectoryName(filePath),
                    Path.GetFileNameWithoutExtension(filePath) + ".gz");
                File.WriteAllBytes(savePath, compData);

                double ratio = (1.0 - (double)compSize / fileSize) * 100;
                Log("✔ تم الحفظ: " + savePath);
                Log("✔ نسبة الضغط: " + ratio.ToString("F1") + "%");
                OnProgress?.Invoke(100);
                OnDone?.Invoke(savePath);
            }
            catch (Exception ex)
            {
                Log("✘ خطأ: " + ex.Message);
            }
            finally
            {
                client?.Close();
            }
        }

        private void ReadExact(NetworkStream stream, byte[] buffer, int count)
        {
            int total = 0;
            while (total < count)
            {
                int r = stream.Read(buffer, total, count - total);
                if (r == 0) throw new Exception("Connection closed");
                total += r;
            }
        }

        private void Log(string msg) => OnLog?.Invoke(msg);

        private string FormatSize(long bytes)
        {
            if (bytes < 1024) return bytes + " B";
            if (bytes < 1024 * 1024) return (bytes / 1024.0).ToString("F1") + " KB";
            return (bytes / (1024.0 * 1024)).ToString("F1") + " MB";
        }
    }
}