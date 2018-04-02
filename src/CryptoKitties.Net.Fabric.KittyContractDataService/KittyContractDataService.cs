using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CryptoKitties.Net.Blockchain;
using CryptoKitties.Net.Blockchain.RestClient;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace CryptoKitties.Net.Fabric.KittyContractDataService
{
    using Remoting;
    using Interfaces;
    using Models;
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class KittyContractDataService 
        : StatefulService
        , IKittyContractDataService
    {
        public KittyContractDataService(StatefulServiceContext context, IEtherscanApiClient etherscanApi)
            : base(context)
        {
            EtherscanApi = etherscanApi;
        }
        public TimeSpan RefreshTimeout { get; private set; }
        protected IEtherscanApiClient EtherscanApi { get; }

        #region State Helpers


        private async Task<IReliableDictionary<long, Timestamped<KittyCatContractDataModel>>> GetKittyDictionaryAsync()
        {
            var value = await StateManager.TryGetAsync<IReliableDictionary<long, Timestamped<KittyCatContractDataModel>>>(StateKeys.Kitties);
            if (!value.HasValue) { throw new InvalidOperationException("Kitty dictionary should never ever ever be null - failing instance."); }
            return value.Value;
        }
        private async Task<KittyCatContractDataModel> ReadKittyFromState(long id, ITransaction transaction = default(ITransaction))
        {
            var ownsTx = transaction == null;
            if (ownsTx)
            {
                transaction = StateManager.CreateTransaction();
            }
            try
            {
                var kitties = await GetKittyDictionaryAsync();
                var kitty = await kitties.TryGetValueAsync(transaction, id);
                return kitty.HasValue && !kitty.Value.IsExpired(RefreshTimeout)
                    ? kitty.Value
                    : default(KittyCatContractDataModel);
            }
            finally
            {
                // If we own the transaction then roll it back to prevent any unintentional updates
                if (ownsTx) { transaction.Dispose(); }
            }
        }
        /// <summary>
        /// Writes kitty to underlying state dictionary
        /// </summary>
        /// <param name="id">The kitties identifier.</param>
        /// <param name="kitty">The raw <see cref="Blockchain.Models.Contracts.KittyResponseModel"/> returned from the blockchain.</param>
        /// <returns>The persisted <see cref="KittyCatContractDataModel"/>.</returns>
        private async Task<KittyCatContractDataModel> WriteKittyToState(long id, Blockchain.Models.Contracts.KittyResponseModel kitty)
        {
            var ret = kitty.ToContractDataModel();
            var kitties = await GetKittyDictionaryAsync();
            var tx = StateManager.CreateTransaction();
            try
            {
                // Update dictionary then commit tx and return
                await kitties.SetAsync(tx, id, new Timestamped<KittyCatContractDataModel>(ret));
                await tx.CommitAsync();
            }
            finally
            {
                tx?.Dispose();
            }
            return ret;
        }

        #endregion

        #region IKittyContractDataService
        /// <inheritdoc cref="IKittyContractDataService.GetContractDataAsync"/>
        async Task<KittyCatContractDataModel> IKittyContractDataService.GetContractDataAsync(long id, bool refresh, CancellationToken cancellationToken)
        {
            if (!refresh)
            {
                var data = await ReadKittyFromState(id);
                cancellationToken.ThrowIfCancellationRequested();
                if (data != null) return data;
            }
            // Read kitty from blockchain and verify result
            var response = await EtherscanApi.GetKitty(id);
            cancellationToken.ThrowIfCancellationRequested();
            if (response == null) { throw new Exception("Kitty data not found on blockchain - make sure node is fully synched."); }
            // Write to internal state and return translated result
            return await WriteKittyToState(id, response);
        }

        #endregion

        #region Service Main Loop
        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
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
        }
        /// <summary>
        /// Called when the service is activated.  Note that this will be called for every activation, not just the first.
        /// </summary>
        /// <returns></returns>
        private async Task OnActivateAsync()
        {
            var configurationPackage = Context.CodePackageActivationContext.GetConfigurationPackageObject("Config");
            var feedConfig = configurationPackage.Settings.Sections["KittyContractDataServiceConfig"];

            RefreshTimeout = feedConfig.ReadValue("KittyRefreshTimeout", TimeSpan.FromMinutes(5));

            var kitties = await StateManager.TryGetAsync<IReliableDictionary<long, Timestamped<KittyCatContractDataModel>>>(StateKeys.Kitties);
            if (!kitties.HasValue)
            {
                await OnInitialActivation();
            }
        }
        /// <summary>
        /// Called very first time service is ever activated
        /// </summary>
        /// <returns></returns>
        private async Task OnInitialActivation()
        {
            // Use getoradd just in case some other instance beat us to it.
            var kitties = await StateManager.GetOrAddAsync<IReliableDictionary<long, Timestamped<KittyCatContractDataModel>>>(StateKeys.Kitties);
            // And thats all, folks!
        }
        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                await OnActivateAsync();
                var doAgain = true;
                while (doAgain)
                {
                    await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                    // Check for cancellation after spinning for a couple seconds
                    doAgain = !cancellationToken.IsCancellationRequested;
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.WriteToLog();
                throw;
            }
            finally
            {
                ServiceEventSource.Current.Message("Kitty contract data service is de-activating");
            }
        }
        #endregion
        /// <summary>
        /// Internal state keys
        /// </summary>
        internal static class StateKeys
        {
            /// <summary>
            /// Kitty dictionary
            /// </summary>
            public const string Kitties = "kitties";
        }
    }
}
