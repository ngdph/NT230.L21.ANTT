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

namespace Lab02.BT04
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
	    // override thuộc tính để ghi đè hàm session thay đổi
            this.CanHandleSessionChangeEvent = true;
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
        }

        protected override void OnStop()
        {
        }

        protected override void OnSessionChange(System.ServiceProcess.SessionChangeDescription changeDescription)
        {
            if (changeDescription.Reason == SessionChangeReason.SessionLogon)
            {
                // Brute sessionId
                for (int session = 10; session > 0; --session)
                {
                    Thread t = new Thread(() =>
                    {
                        try
                        {
                            String title = "Red Alert";
                            String message = "18520165 - 18520333";
                            int resp;
                            WTSSendMessage(
                                IntPtr.Zero, session,
                                title, title.Length,
                                message, message.Length,
                                4, 0, out resp, true
                            );
                        }
                        catch { }
                    });
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                }
                base.OnSessionChange(changeDescription);
            }
        }
    }

}
