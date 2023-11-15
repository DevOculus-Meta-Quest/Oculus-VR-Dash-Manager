using System;
using System.ServiceProcess;

namespace OVR_Dash_Manager.Functions
{
    public class ServiceManager
    {
        public bool ManageService(string serviceName, bool startService)
        {
            try
            {
                using (ServiceController serviceController = new ServiceController(serviceName))
                {
                    if (startService && serviceController.Status == ServiceControllerStatus.Stopped)
                    {
                        // Start the service
                        serviceController.Start();
                        serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                    }
                    else if (!startService && serviceController.Status == ServiceControllerStatus.Running)
                    {
                        // Stop the service
                        serviceController.Stop();
                        serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log or handle exceptions as needed
                Console.WriteLine($"Error managing the service: {ex.Message}");
                return false;
            }
        }
    }
}