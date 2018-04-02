using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CryptoKitties.Net.Api;
using CryptoKitties.Net.Blockchain.RestClient;
using Microsoft.ServiceFabric.Services.Runtime;

namespace CryptoKitties.Net.Fabric.KittyContractDataService
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            var httpFactory = new HttpClientRequestFactory();
            try
            {
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                ServiceRuntime.RegisterServiceAsync("CryptoKitties.Net.Fabric.KittyContractDataServiceType",
                    context => new KittyContractDataService(context, new EtherscanApiClient(httpFactory))).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(KittyContractDataService).Name);

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
