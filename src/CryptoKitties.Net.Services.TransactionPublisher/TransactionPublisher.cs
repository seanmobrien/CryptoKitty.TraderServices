using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace CryptoKitties.Net.Services.TransactionPublisher
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class TransactionPublisher : StatefulService
    {
        public TransactionPublisher(StatefulServiceContext context)
            : base(context)
        { }


        private Web3 Web3 { get; set; }
        private string ContractAddress { get; set; }

        private Event SaleEvent { get; set; }
        private Event AuctionEvent { get; set; }



        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            /*
            var listenerSettings = new FabricTransportRemotingListenerSettings
            {
                MaxMessageSize = 10000000,
                SecurityCredentials = RemotingExtensions.GetSecurityCredentials(
                    RemotingExtensions.Certificates.ServiceThumbprint,
                    RemotingExtensions.Certificates.WebsiteThumbprint,
                    RemotingExtensions.Certificates.WebsiteCommonName
                )
            };
            return new[] {
                new ServiceReplicaListener(context => new FabricTransportServiceRemotingListener(context, this, listenerSettings))
            };
            */
            return new ServiceReplicaListener[0];
        }
        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await OnActivateAsync();

            var doAgain = true;
            while (doAgain)
            {
                var logData = await GetLogs();

                // TODO: Process logs
            }
             
        }

        async Task<object> GetLogs()
        {
            throw new Exception();
        }






        class StateKeys
        {
            public const string CurrentBlock = "CurrentBlock";
        }
        async Task OnActivateAsync()
        {
            var configurationPackage = Context.CodePackageActivationContext.GetConfigurationPackageObject("Config");
            var config = configurationPackage.Settings.Sections["TransactionPublisherConfig"];

            Web3 = new Web3(config.ReadValue("EthereumConnection"));

            var sales = Web3.Eth.GetContract(Globals.Contracts.Address.SalesAuction,
                Globals.Contracts.ABI.SalesAuction);
            sales.GetEvent("xxx");


        }
    }
}
