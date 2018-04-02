using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoKitties.Net.Fabric;
using CryptoKitties.Net.Fabric.Remoting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Services.Client;

namespace CryptoKitties.Net.Website
{
    partial class Startup
    {
        public static class ServiceConfig
        {
            // This method gets called by the runtime. Use this method to add services to the container.
            public static void ConfigureServices(IServiceCollection services)
            {

                services.AddSingleton<Fabric.KittyCatService.Interfaces.IKittyCatServiceFactory>(new Fabric.KittyCatService.Interfaces.KittyCatServiceFactory(Fabric.Remoting.RemotingExtensions.Certificates.WebsiteThumbprint));
                services.AddTransient(svc => CreateSecuredStatefulServiceProxy<Fabric.KittyContractDataService.Interfaces.IKittyContractDataService>(ServiceAddress.KittyContractDataServiceName, new ServicePartitionKey(1L)));
            }


            private static TInterface CreateSecuredStatefulServiceProxy<TInterface>(string serviceName, ServicePartitionKey servicePartition)
                where TInterface : Microsoft.ServiceFabric.Services.Remoting.IService
            {
                return RemotingExtensions.CreateSecuredStatefulServiceProxy<TInterface>(
                    serviceName, servicePartition,
                    RemotingExtensions.Certificates.WebsiteThumbprint, RemotingExtensions.Certificates.ServiceThumbprint, RemotingExtensions.Certificates.ServiceCommonName
                );
            }

            public static TInterface CreateSecuredActorProxy<TInterface>(Uri serviceUri, ActorId actorId)
                where TInterface : IActor
            {
                return RemotingExtensions.CreateActor<TInterface>(
                    serviceUri, actorId,
                    RemotingExtensions.Certificates.WebsiteThumbprint, RemotingExtensions.Certificates.ServiceThumbprint, RemotingExtensions.Certificates.ServiceCommonName
                );
            }
        }
    }
}
