using System;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace CryptoKitties.Net.Fabric
{
    public static class ServiceAddress
    {
        public const string AppFabricUriRoot = "fabric:/CryptoKitties.Net.Fabric/";

        public const string KittyServiceName = "KittyCatServiceActorService";
        public const string KittyContractDataServiceName = "CryptoKitties.Net.Fabric.KittyContractDataService";

        public const StatePersistence DefaultPersistence = StatePersistence.Volatile;
        public static Uri KittyService => ServiceFabricUri(KittyServiceName);
        public static Uri KittyContractDataService => ServiceFabricUri(KittyContractDataServiceName);
        public static Uri ServiceFabricUri(string serviceName)
        { return new Uri(AppFabricUriRoot + serviceName); }

    }
}
