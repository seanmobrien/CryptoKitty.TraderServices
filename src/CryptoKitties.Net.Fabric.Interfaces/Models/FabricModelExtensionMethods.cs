using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoKitties.Net.Fabric.Models;

namespace CryptoKitties.Net.Fabric.Models
{
    public static class KittyCatServiceExtensionMethods
    {
        public static KittyCatContractDataModel ToContractDataModel(this Blockchain.Models.Contracts.KittyResponseModel source)
        {
            return source == null
                ? default(KittyCatContractDataModel)
                : new KittyCatContractDataModel
                {
                    BirthTime = source.BirthTime.ToString(16),
                    CooldownIndex = source.CooldownIndex,
                    Genes = source.Genes.ToString(16),
                    IsGestating = source.IsGestating,
                    Generation = source.Generation,
                    IsReady = source.IsReady,
                    MatronId = source.MatronId,
                    NextActionAt = source.NextActionAt.ToString(16),
                    SireId = source.SireId,
                    SiringWithId = source.SiringWithId
                };
        }
    }
}
