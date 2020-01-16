using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace PushNotificationService
{
    public partial class scheduler : ServiceBase
    {
        private Timer timer = null;
        public scheduler()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                timer = new Timer();
                this.timer.Interval = Convert.ToInt32(ConfigurationManager.AppSettings["TimerInterval"]);
                this.timer.Elapsed += new System.Timers.ElapsedEventHandler(this.timer_Tick);
                timer.Enabled = true;

                LogIt.WriteErrorLog("Notification Service Started\n");
            }
            catch (Exception ex)
            {
                LogIt.WriteErrorLog("Error: " + ex.Message + "@\n");
            }
        }
        private async void timer_Tick(object sender, ElapsedEventArgs e)
        {           
            //Write code here to do some job depends on your requirement
            ShortListNotification.SendShortListNotification();            
        }
        protected override void OnStop()
        {
            this.timer.Elapsed -= new System.Timers.ElapsedEventHandler(this.timer_Tick);
            timer.Enabled = false;
            timer = null;
            LogIt.WriteErrorLog("Service Stopped\n");
        }
    }
}
