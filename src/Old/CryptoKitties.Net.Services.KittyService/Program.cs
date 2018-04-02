using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using CryptoKitties.Net.Api;
using CryptoKitties.Net.Blockchain.RestClient;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace CryptoKitties.Net.Services.KittyService
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                // This line registers an Actor Service to host your actor class with the Service Fabric runtime.
                // The contents of your ServiceManifest.xml and ApplicationManifest.xml files
                // are automatically populated when you build this project.
                // For more information, see https://aka.ms/servicefabricactorsplatform
                ActorRuntime.RegisterActorAsync<KittyService>(
                   (context, actorType) => new ActorService(context, actorType,
                    // Create new kitty service while injecting needed services
                    (s,i) => new KittyService(s,i, new EtherscanApiClient(new HttpClientRequestFactory()))
                   )).GetAwaiter().GetResult();

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
