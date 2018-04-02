using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace CryptoKitties.Net.Fabric.Remoting
{
    public class DefaultServiceRemotingCallbackClient
        : IServiceRemotingCallbackClient
    {
        public virtual async Task<byte[]> RequestResponseAsync(ServiceRemotingMessageHeaders messageHeaders, byte[] requestBody)
        {
            var args = new ServiceRemotingMessageEventArgs(messageHeaders, requestBody);
            await Task.Run(() => ResponseRequested?.Invoke(this, args));
            return args.RequestBody;
        }

        public virtual void OneWayMessage(ServiceRemotingMessageHeaders messageHeaders, byte[] requestBody)
        {
            OneWayMessageReceived?.Invoke(this, new ServiceRemotingMessageEventArgs(messageHeaders, requestBody));
        }

        /// <summary>
        /// Raised when a request is recieved
        /// </summary>
        public event EventHandler<ServiceRemotingMessageEventArgs> ResponseRequested;
        public event EventHandler<ServiceRemotingMessageEventArgs> OneWayMessageReceived;
    }

    [Serializable]
    public class ServiceRemotingMessageEventArgs
        : EventArgs
    {
        public ServiceRemotingMessageEventArgs(ServiceRemotingMessageHeaders messageHeaders, byte[] requestBody)
        {
            MessageHeaders = messageHeaders;
            RequestBody = requestBody;
        }
        public ServiceRemotingMessageHeaders MessageHeaders { get; private set; }
        public byte[] RequestBody { get; private set; }
    }
}
