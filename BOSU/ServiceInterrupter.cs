using System;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace BOSU
{
    class ServiceInterrupter
    {
        private static readonly string[] SvcList = { "wuauserv", "usoSvc", "bits" };

        private LogWriter logWriter = new LogWriter();

        private MainForm mainForm;

        public ServiceInterrupter(MainForm mainForm)
        {
            this.mainForm = mainForm;
        }
        private async Task RunTask()
        {
            int timeout = 3000;
            var delay = Convert.ToInt32(10000);
            mainForm.SendMessageToDialogBox("Starting..");
            mainForm.SendMessageToDialogBox($"Monitor Delay: {delay/1000} sec");

            while (true)
            {
                try
                {
                    foreach (var service in SvcList)
                    {
                        using (var serviceController = new ServiceController(service))
                        {
                            if (serviceController.Status != ServiceControllerStatus.Running) continue;
                        }

                        StopService(service, timeout);
                        /*
                        notifyIcon1.BalloonTipText = "Service: "+service+" was stopped.";
                        notifyIcon1.BalloonTipTitle = "BOSU";
                        */
                        mainForm.SendMessageToDialogBox($"Service: \"{service}\" was stopped.");
                    }
                }
                catch (Exception ex)
                {
                    mainForm.SendMessageToDialogBox($"Exception: {ex.Message}");
                    logWriter.LogWrite($"Exception: {ex.Message}");
                }

                GC.Collect();
                await Task.Delay(delay);
            }
        }

        private void StopService(string serviceName, int timeoutMilliseconds)
        {
            try
            {
                var timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                using (var serviceController = new ServiceController(serviceName))
                {
                    serviceController.Stop();
                    serviceController.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }
            }
            catch (Exception ex)
            {
                mainForm.SendMessageToDialogBox($"Exception: {ex.Message}");
            }
        }

        public async void Start() => await RunTask();
    }
}
