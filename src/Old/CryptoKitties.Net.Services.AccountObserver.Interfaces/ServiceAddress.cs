using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace CryptoKitties.Net.Services
{
    public static class ServiceAddress
    {
        public const string AppFabricUriRoot = "fabric:/CryptoKitties.Net.Services/";

        public const string KittyServiceName = "CryptoKitties.Net.Services.KittyService";

        public const string TickerFeedServiceName = "TickerFeedService";
        public const string MarketTraderEventRouterName = "TraderMarketRouterActorService";
        //public const string TraderActorName = "TraderActorService";
        public const string TraderActorName = "Trader";
        public const string MarketTraderActorName = "MarketTraderActorService";

        public const StatePersistence DefaultPersistence = StatePersistence.Volatile;

        public static Uri KittyService => ServiceFabricUri(KittyServiceName);
        public static Uri ServiceFabricUri(string serviceName)
        { return new Uri(AppFabricUriRoot + serviceName); }

    }
}
