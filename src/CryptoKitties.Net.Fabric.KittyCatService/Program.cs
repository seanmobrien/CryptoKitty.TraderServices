using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using CryptoKitties.Net.Api;
using CryptoKitties.Net.Blockchain.RestClient;
using CryptoKitties.Net.Fabric.Remoting;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace CryptoKitties.Net.Fabric
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            var httpRequestFactory = new HttpClientRequestFactory();
            // The ServiceManifest.XML file defines one or more service type names.
            // Registering a service maps a service type name to a .NET type.
            // When Service Fabric creates an instance of this service type,
            // an instance of the class is created in this host process.
            /*
            try
            {               
                ServiceRuntime.RegisterServiceAsync("KittyContractDataServiceType",
                        context => new KittyContractDataService.KittyContractDataService(
                            context, // Stateful service context
                            new EtherscanApiClient(httpRequestFactory) // Etherscan API client
                        ))
                    .GetAwaiter()
                    .GetResult();

                KittyContractDataService.ServiceEventSource.Current.ServiceTypeRegistered(
                    Process.GetCurrentProcess().Id, typeof(KittyContractDataService.KittyContractDataService).Name);
            } catch (Exception e)
            {
                KittyContractDataService.ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
            }
            */
            // This line registers an Actor Service to host your actor class with the Service Fabric runtime.
            // The contents of your ServiceManifest.xml and ApplicationManifest.xml files
            // are automatically populated when you build this project.
            // For more information, see https://aka.ms/servicefabricactorsplatform
            try
            {                
                ActorRuntime.RegisterActorAsync<KittyCatService.KittyCatService>(
                   (context, actorType) => new ActorService(
                                context, actorType // Stateful service context and actor type
                            ))
                   .GetAwaiter().GetResult();
                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                KittyCatService.ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
