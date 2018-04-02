using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CryptoKitties.Net.Blockchain.Models.Contracts;
using CryptoKitties.Net.GeneScience.Models;
using Microsoft.ServiceFabric.Actors;

namespace CryptoKitties.Net.Services.KittyService.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IKittyService : IActor
    {
        /// <summary>
        /// Returns kitty data stored on blockchain.
        /// </summary>
        /// <param name="refresh">Optional <c>bool</c> used to force reload of data.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
        /// <returns><see cref="KittyResponseModel"/> data</returns>
        Task<KittyResponseModel> GetContractDataAsync(bool refresh, CancellationToken cancellationToken);
        /// <summary>
        /// Returns kitty cattributes by category.
        /// </summary>
        /// <param name="all"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<GeneSet>> GetGenesAsync(bool all, CancellationToken cancellationToken);
    }
}
