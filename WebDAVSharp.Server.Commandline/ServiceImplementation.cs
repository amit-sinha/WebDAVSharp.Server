using System.ServiceProcess;
using Common.Logging;
using WebDAVSharp.FileExample.Framework;
using WebDAVSharp.Server;
using WebDAVSharp.Server.Stores.DiskStore;
using System.Configuration;
using Common.Logging.Configuration;

namespace WebDAVSharp.FileExample
{
    /// <summary>
    ///     The actual implementation of the windows service goes here...
    /// </summary>
    [WindowsService("WebDavSharp.FileExample",
        DisplayName = "WebDavSharp.FileExample",
        Description = "WebDavSharp.FileExample",
        EventLogSource = "WebDavSharp.FileExample",
        StartMode = ServiceStartMode.Automatic)]
    public class ServiceImplementation : IWindowsService
    {
        public void Dispose()
        {
        }

        /// <summary>
        ///     This method is called when the service gets a request to start.
        /// </summary>
        /// <param name="args">Any command line arguments</param>
        public void OnStart(string[] args)
        {
            InitConsoleLogger();
            StartServer();
        }

        /// <summary>
        ///     This method is called when the service gets a request to stop.
        /// </summary>
        public void OnStop()
        {
        }

        /// <summary>
        ///     This method is called when a service gets a request to pause,
        ///     but not stop completely.
        /// </summary>
        public void OnPause()
        {
        }

        /// <summary>
        ///     This method is called when a service gets a request to resume
        ///     after a pause is issued.
        /// </summary>
        public void OnContinue()
        {
        }

        /// <summary>
        ///     This method is called when the machine the service is running on
        ///     is being shutdown.
        /// </summary>
        public void OnShutdown()
        {
        }
        private static void InitConsoleLogger()
        {
#if DEBUG
            // create properties
            NameValueCollection properties = new NameValueCollection
            {
                ["showDateTime"] = "true"
            };
            // set Adapter
            LogManager.Adapter =
                new Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter(properties);
#endif
        }

        /// <summary>
        /// Starts the server.
        /// Authentication used: Negotiate
        /// </summary>
        private static void StartServer()
        {
            string Localpath = ConfigurationManager.AppSettings["Localpath"];
            string Url = ConfigurationManager.AppSettings["StartupUrl"];

            var store = new WebDavDiskStore(Localpath);
            var server = new WebDavServer(store);
            server.Start(Url);
        }
    }
}