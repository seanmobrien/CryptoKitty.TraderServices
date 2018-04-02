using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CryptoKitties.Net.Blockchain;
using CryptoKitties.Net.Blockchain.Models.Contracts;
using CryptoKitties.Net.Blockchain.RestClient;
using CryptoKitties.Net.GeneScience.Models;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using CryptoKitties.Net.Services.KittyService.Interfaces;

namespace CryptoKitties.Net.Services.KittyService
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class KittyService : Actor, IKittyService
    {
        /// <summary>
        /// Initializes a new instance of KittyService
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public KittyService(ActorService actorService, ActorId actorId, IEtherscanApiClient etherscanApi)
            : base(actorService, actorId)
        {
            EtherscanApi = etherscanApi;
        }
        /// <summary>
        /// Returns the id of the kitty owned by this service.
        /// </summary>
        public long KittyId => Id.GetLongId();


        protected IEtherscanApiClient EtherscanApi { get; }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization

            return this.StateManager.TryAddStateAsync("count", 0);
        }


        /// <inheritdoc cref="IKittyService.GetContractDataAsync"/>
        public async Task<KittyResponseModel> GetContractDataAsync(bool refresh, CancellationToken cancellationToken)
        {
            if (!refresh)
            {
                var data = await this.StateManager.GetStateAsync<KittyResponseModel>(StateKeys.ContractData, cancellationToken);
                if (data != null) return data;
            }
            var response = await EtherscanApi.GetKitty(KittyId);
            cancellationToken.ThrowIfCancellationRequested();
            if (response == null) {  throw new Exception("Kitty data not found on blockchain - make sure node is fully synched."); }
            await this.StateManager.SetStateAsync(StateKeys.ContractData, response, cancellationToken);
            return response;
        }
        /// <inheritdoc cref="IKittyService.GetGenesAsync"/>
        public async Task<IEnumerable<GeneSet>> GetGenesAsync(bool all, CancellationToken cancellationToken)
        {
            var data = await GetContractDataAsync(false, cancellationToken);
            var splicer = new GeneScience.GeneSplicer(data.Genes);
            return all ? splicer.AllCattributes : splicer.KnownCattributes;
        }
        internal static class StateKeys
        {
            public const string ContractData = "ContractData";

        }
    }
}
