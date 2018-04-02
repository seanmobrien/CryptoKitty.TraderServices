namespace CryptoKitties.Net.Fabric.KittyCatService.Interfaces
{
    public interface IKittyCatServiceFactory
    {
        IKittyCatService GetService(long kittyId);
    }
}