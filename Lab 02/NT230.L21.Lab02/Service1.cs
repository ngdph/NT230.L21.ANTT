using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NT230.L21.Lab02
{
    public partial class Service1 : ServiceBase
    {
        private static extern Int32 SystemParametersInfo(UInt32 uiAction, UInt32 uiParam, String pvParam, UInt32 fWinIni);
        const int SPI_SETDESKWALLPAPER = 0x14;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;
        public Service1()
        {
            InitializeComponent();
        }

        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern void WTSSendMessage(
            IntPtr hServer,
            [MarshalAs(UnmanagedType.I4)] int SessionId,
            String pTitle,
            [MarshalAs(UnmanagedType.U4)] int TitleLength,
            String pMessage,
            [MarshalAs(UnmanagedType.U4)] int MessageLength,
            [MarshalAs(UnmanagedType.U4)] int Style,
            [MarshalAs(UnmanagedType.U4)] int Timeout,
            [MarshalAs(UnmanagedType.U4)] out int pResponse,
            bool bWait
        );

        protected override void OnStart(string[] args)
        {
            onChangeBackground();
        }

        protected override void OnStop()
        {
        }

        protected void onChangeBackground()
        {
            string fileUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b6/Image_created_with_a_mobile_phone.png/1024px-Image_created_with_a_mobile_phone.png";
            string fileName = System.IO.Path.GetFileName(new Uri(fileUrl).AbsolutePath);
            string fileLocation = "C:/" + fileName;

            new System.Net.WebClient().DownloadFile(fileUrl, fileLocation);
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, fileLocation, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            EventLog.WriteEntry("SimpleService.OnSessionChange", DateTime.Now.ToLongTimeString() +
                " - Session change notice received: " +
                changeDescription.Reason.ToString() + "  Session ID: " +
                changeDescription.SessionId.ToString());


            switch (changeDescription.Reason)
            {
                case SessionChangeReason.SessionLogon:
                    for (int session = 5; session > 0; --session)
                    {
                        Thread t = new Thread(() =>
                        {
                            try
                            {
                                String title = "Alert", msg = "18520165 - 18520333";
                                int resp;
                                WTSSendMessage(
                                    IntPtr.Zero, session,
                                    title, title.Length,
                                    msg, msg.Length,
                                    4, 0, out resp, true
                                );
                            }
                            catch { }
                        });
                        t.SetApartmentState(ApartmentState.STA);
                        t.Start();
                    }
                    base.OnSessionChange(changeDescription);
                    break;
            }
        }
    }
}

