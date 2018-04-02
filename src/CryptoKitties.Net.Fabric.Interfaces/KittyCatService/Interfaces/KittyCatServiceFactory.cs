using System;
using CryptoKitties.Net.Fabric.Remoting;
using Microsoft.ServiceFabric.Actors;

namespace CryptoKitties.Net.Fabric.KittyCatService.Interfaces
{
    public class KittyCatServiceFactory : IKittyCatServiceFactory
    {
        public KittyCatServiceFactory(string thumbprint)
        {
            _thumbprint = thumbprint;
        }

        private readonly string _thumbprint;
        public IKittyCatService GetService(long kittyId)
        {
            return RemotingExtensions.CreateActor<IKittyCatService>(
                ServiceAddress.KittyService, new ActorId(kittyId),
                _thumbprint, RemotingExtensions.Certificates.ServiceThumbprint, RemotingExtensions.Certificates.ServiceCommonName
            );
        }
    }
}
