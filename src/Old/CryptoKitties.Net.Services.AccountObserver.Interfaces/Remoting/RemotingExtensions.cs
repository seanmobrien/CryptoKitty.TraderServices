using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Client;

namespace CryptoKitties.Net.Services.Remoting
{
    public static class RemotingExtensions
    {
        public static TInterface CreateSecuredStatefulServiceProxy<TInterface>(string serviceName, ServicePartitionKey servicePartition, string thumbprint, string remoteThumbprint, string remoteCommonName)
            where TInterface : Microsoft.ServiceFabric.Services.Remoting.IService
        { return CreateSecuredStatefulServiceProxy<TInterface>(ServiceAddress.ServiceFabricUri(serviceName), servicePartition, thumbprint, remoteThumbprint, remoteCommonName); }

        public static TInterface CreateSecuredStatefulServiceProxy<TInterface>(Uri uri, ServicePartitionKey servicePartition, string thumbprint, string remoteThumbprint, string remoteCommonName)
            where TInterface : Microsoft.ServiceFabric.Services.Remoting.IService
        {
            return new ServiceProxyFactory(c => new FabricTransportServiceRemotingClientFactory(new FabricTransportRemotingSettings()
                    { SecurityCredentials = GetSecurityCredentials(thumbprint, remoteThumbprint, remoteCommonName) }
                ))
                .CreateServiceProxy<TInterface>(uri, servicePartition);
        }

        public static TInterface CreateActor<TInterface>(Uri serviceUri, ActorId actorId, string listenerName = null, IServiceRemotingCallbackClient callbackClient = default(IServiceRemotingCallbackClient))
            where TInterface : Microsoft.ServiceFabric.Actors.IActor
        {
            return CreateActor<TInterface>(serviceUri, actorId, Certificates.ServiceThumbprint,
                Certificates.ServiceThumbprint, Certificates.ServiceCommonName, listenerName, callbackClient);
        }


        public static TInterface CreateActor<TInterface>(Uri serviceUri, ActorId actorId, string thumbprint, string remoteThumbprint, string remoteCommonName, string listenerName = null, IServiceRemotingCallbackClient callbackClient = default(IServiceRemotingCallbackClient))
            where TInterface : Microsoft.ServiceFabric.Actors.IActor
        {
            return new ActorProxyFactory(c => new FabricTransportActorRemotingClientFactory(new FabricTransportRemotingSettings()
#if SECURE_ACTORS
                     { SecurityCredentials = GetSecurityCredentials(thumbprint, new[] { remoteThumbprint } , new[] { remoteCommonName }) }
#endif
                    , callbackClient ?? new DefaultServiceRemotingCallbackClient()))
                .CreateActorProxy<TInterface>(serviceUri, actorId, listenerName);
        }


        public static TInterface CreateActor<TInterface>(ActorId actorId, Uri serviceUri, string applicationName = null, string serviceName = null, string listenerName = null, IServiceRemotingCallbackClient callbackClient = default(IServiceRemotingCallbackClient))
            where TInterface : Microsoft.ServiceFabric.Actors.IActor
        {
            return CreateActor<TInterface>(actorId, serviceUri, Certificates.ServiceThumbprint,
                Certificates.ServiceThumbprint, Certificates.ServiceCommonName, applicationName, serviceName,
                listenerName, callbackClient);
        }

        public static TInterface CreateActor<TInterface>(ActorId actorId, Uri serviceUri, string thumbprint, string remoteThumbprint, string remoteCommonName, string applicationName = null, string serviceName = null, string listenerName = null, IServiceRemotingCallbackClient callbackClient = default(IServiceRemotingCallbackClient))
            where TInterface : Microsoft.ServiceFabric.Actors.IActor
        {
            return new ActorProxyFactory(c => new FabricTransportActorRemotingClientFactory(new FabricTransportRemotingSettings()
#if SECURE_ACTORS
                        { SecurityCredentials = GetSecurityCredentials(thumbprint, new[] { remoteThumbprint }, new[] { remoteCommonName }) }
#endif
                    , callbackClient ?? new DefaultServiceRemotingCallbackClient()))
                .CreateActorProxy<TInterface>(actorId, applicationName, serviceName, listenerName);
        }



        public static SecurityCredentials GetSecurityCredentials(string thumbprint, string remoteThumbprint, string remoteCommonName)
        {
            return GetSecurityCredentials(thumbprint, new[] { remoteThumbprint }, new[] { remoteCommonName });
        }
        public static SecurityCredentials GetSecurityCredentials(string thumbprint, IEnumerable<string> remoteThumbprint, IEnumerable<string> remoteCommonName)
        {
            // Provide certificate details.
            var x509Credentials = new X509Credentials
            {
                FindType = X509FindType.FindByThumbprint,
                FindValue = thumbprint,
                StoreLocation = StoreLocation.LocalMachine,
                StoreName = "My",
                ProtectionLevel = ProtectionLevel.EncryptAndSign
            };
            foreach (var cn in remoteThumbprint ?? new string[0])
                x509Credentials.RemoteCommonNames.Add(cn);
            foreach (var thumb in remoteThumbprint ?? new string[0])
                x509Credentials.RemoteCertThumbprints.Add(thumb);
            return x509Credentials;
        }

        public static class Certificates
        {
            public const string WebsiteThumbprint = "e00bffd9d5c0e43173e82d9121bafed4ec390bf6";
            public const string WebsiteCommonName = "CN=CryptoTraderDevWebCert";
            public const string ServiceThumbprint = "C82D49D4C77F468B1C8BBCC7E0A26A1A46562BD3";
            public const string ServiceCommonName = "CN=CryptoTraderDevServiceCert";
        }
    }
}
