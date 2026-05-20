using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CompressionProject
{
    public class CompressionServer
    {
        private TcpListener listener;
        private bool running = false;
        private int port;

        public event Action<string> OnLog;

        public CompressionServer(int port = 9000)
        {
            this.port = port;
        }

        public void Start()
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            running = true;
            Log("✔ السيرفر شغال على port " + port);
            Log("⏳ في انتظار الاتصالات...");

            Thread acceptThread = new Thread(AcceptLoop);
            acceptThread.IsBackground = true;
            acceptThread.Start();
        }

        public void Stop()
        {
            running = false;
            listener?.Stop();
            Log("■ السيرفر وقف.");
        }

        private void AcceptLoop()
        {
            while (running)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    string clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                    Log("→ عميل جديد اتصل: " + clientIP);

                    Thread t = new Thread(() => HandleClient(client, clientIP));
                    t.IsBackground = true;
                    t.Start();
                }
                catch
                {
                    if (running) Log("✘ خطأ في قبول الاتصال");
                }
            }
        }

        private void HandleClient(TcpClient client, string clientIP)
        {
            try
            {
                using (NetworkStream stream = client.GetStream())
                {
                    
                    byte[] sizeBuf = new byte[8];
                    ReadExact(stream, sizeBuf, 8);
                    long fileSize = BitConverter.ToInt64(sizeBuf, 0);
                    Log("[" + clientIP + "] حجم الملف: " + FormatSize(fileSize));

                   
                    byte[] nameLenBuf = new byte[4];
                    ReadExact(stream, nameLenBuf, 4);
                    int nameLen = BitConverter.ToInt32(nameLenBuf, 0);
                    byte[] nameBytes = new byte[nameLen];
                    ReadExact(stream, nameBytes, nameLen);
                    string fileName = Encoding.UTF8.GetString(nameBytes);
                    Log("[" + clientIP + "] اسم الملف: " + fileName);

                   
                    byte[] fileData = new byte[fileSize];
                    ReadExact(stream, fileData, (int)fileSize);
                    Log("[" + clientIP + "] ✔ تم استقبال الملف");

                   
                    byte[] compressed = CompressData(fileData);
                    double ratio = (1.0 - (double)compressed.Length / fileSize) * 100;
                    Log("[" + clientIP + "] ✔ الضغط: " + FormatSize(fileSize) + " → " + FormatSize(compressed.Length) + " (وفّر " + ratio.ToString("F1") + "%)");

                    
                    byte[] compSizeBytes = BitConverter.GetBytes((long)compressed.Length);
                    stream.Write(compSizeBytes, 0, 8);

                    
                    stream.Write(compressed, 0, compressed.Length);
                    stream.Flush();
                    Log("[" + clientIP + "] ✔ تم الإرسال للعميل");
                }
            }
            catch (Exception ex)
            {
                Log("[" + clientIP + "] ✘ خطأ: " + ex.Message);
            }
            finally
            {
                client.Close();
            }
        }

        private byte[] CompressData(byte[] data)
        {
            using (var ms = new MemoryStream())
            {
                using (var gz = new GZipStream(ms, CompressionMode.Compress, true))
                    gz.Write(data, 0, data.Length);
                return ms.ToArray();
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