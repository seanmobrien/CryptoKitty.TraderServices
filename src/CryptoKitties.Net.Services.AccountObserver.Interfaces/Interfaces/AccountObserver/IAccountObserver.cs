using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CryptoKitties.Net.Services.Models.TransationPublisher;
using Microsoft.ServiceFabric.Actors;

namespace CryptoKitties.Net.Services.Interfaces.TransactionPublisher
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface ITransactionPublisher : IActor
    {
        /// <summary>
        /// Get current sync status
        /// </summary>
        /// <returns></returns>
        Task<SyncStatus> SyncStatus();
     
        /// <summary>
        /// Start sync
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Start(CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Stop sync
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Stop(CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Pause
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Pause(CancellationToken cancellationToken = default(CancellationToken));
    }
}
