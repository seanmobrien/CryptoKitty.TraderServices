using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace CryptoKitties.Net.Fabric.KittyContractDataService.Interfaces
{
    /// <summary>
    /// The <see cref="IKittyContractDataService"/> interface exposes methods used to read kitty cat data off the blockchain
    /// </summary>
    public interface IKittyContractDataService
        : IService
    {
        /// <summary>
        /// Reads contract data for <paramref name="kittyId"/>.
        /// </summary>
        /// <param name="kittyId">A <see cref="long"/> identifying the kitty to read.</param>
        /// <param name="forceRefresh">If <c>true</c>, contract data will be forcefully refreshed.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the operation.</param>
        /// <returns>A <see cref="Models.KittyCatContractDataModel"/> containing kitty contract data.</returns>
        Task<Models.KittyCatContractDataModel> GetContractDataAsync(long kittyId, bool forceRefresh, CancellationToken cancellationToken);
    }
}
