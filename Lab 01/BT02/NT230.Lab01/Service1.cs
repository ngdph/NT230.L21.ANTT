using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;

namespace NT230.Lab01
{
    public partial class Service1 : ServiceBase
    {
        System.Timers.Timer timer = new System.Timers.Timer();
        string[] desiredProcess = { "notepad.exe", "chrome.exe", "cmd.exe" };
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Khởi động process chrome
            StartProcess("chrome.exe");

            WriteToFile("Service is started at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000; //number in milisecinds
            timer.Enabled = true;


            //Process[] allProcesses = Process.GetProcesses();

            //foreach(Process singleProcess in allProcesses)
            //{
            //    if (this.desiredProcess.Contains(singleProcess.ProcessName))
            //    {

            //    }
            //}
        }

        protected void onCheckProcessStatus()
        {
            foreach (string processName in this.desiredProcess)
            {
                Process[] processes = Process.GetProcessesByName(processName);

                if (processes.Length == 0)
                {
                    //WriteToFile("Service " + process + " is not running at " + DateTime.Now);

                    StartProcess(processName);
                }
                else
                {
                    //WriteToFile("Service " + process + " is running at " + DateTime.Now);

                    KillProcess(processName);
                }
            }
        }

        protected override void OnStop()
        {
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            onCheckProcessStatus();
            WriteToFile("Service is recall at " + DateTime.Now);
        }

        protected void KillProcess(string processName)
        {
            //Process[] processes = Process.GetProcessesByName(processName);

            foreach (var p in Process.GetProcessesByName(processName))
            {
                p.Kill();
            }
        }

        protected void StartProcess(string processName)
        {
            Process.Start(processName);
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
    }
}
