using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CryptoKitties.Net.Services.Interfaces.TransactionPublisher;
using CryptoKitties.Net.Services.Models.TransationPublisher;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace CryptoKitties.Net.Services.TransactionPublisher
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    [StatePersistence(StatePersistence.Persisted)]
    internal sealed class TransactionPublisher : Actor, ITransactionPublisher
    {
        public TransactionPublisher(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }
     
        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");



            /*
                var configurationPackage = Context.CodePackageActivationContext.GetConfigurationPackageObject("Config");
            var config = configurationPackage.Settings.Sections["TransactionPublisherConfig"];

            Web3 = new Web3(config.ReadValue("EthereumConnection"));

            var sales = Web3.Eth.GetContract(Globals.Contracts.Address.SalesAuction,
                Globals.Contracts.ABI.SalesAuction);
            sales.GetEvent("xxx");

             */

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization

            return this.StateManager.TryAddStateAsync("count", 0);
        }


        class StateKeys
        {
            public const string NextBlock = "NextBlock";
            public const string RunStatus = "RunStatus";
        }

        async Task<SyncStatus> ITransactionPublisher.SyncStatus()
        {
            var st = this.StateManager.GetStateAsync<int>(StateKeys.RunStatus);
            var ret = new SyncStatus
            {
                NextBlock = await this.StateManager.GetStateAsync<string>(StateKeys.NextBlock)
            };
            var status = await st;
            switch (status)
            {
                case 1:
                    ret.Running = true;
                    break;
                case 2:
                    ret.Paused = true;
                    break;
            }
            return ret;
        }

        async Task ITransactionPublisher.Start(CancellationToken cancellationToken)
        {
            var status = await this.StateManager.GetStateAsync<int>(StateKeys.RunStatus, cancellationToken);
            if (status != 1)
            {
                // Start looping       
            }
        }

        async Task ITransactionPublisher.Stop(CancellationToken cancellationToken)
        {
            var status = await this.StateManager.GetStateAsync<int>(StateKeys.RunStatus, cancellationToken);
            if (status != 1) throw new InvalidOperationException();
        }

        async Task ITransactionPublisher.Pause(CancellationToken cancellationToken)
        {
            var status = await this.StateManager.GetStateAsync<int>(StateKeys.RunStatus, cancellationToken);
            if (status != 1) throw new InvalidOperationException();

            await this.StateManager.SetStateAsync(StateKeys.RunStatus, cancellationToken);
        }
    }
}
