using System;
using System.Runtime.Serialization;

namespace CryptoKitties.Net.Services.Models.TransationPublisher
{
    [DataContract]
    public class SyncStatus
    {
        [DataMember]
        public bool InSync { get; set; }
        [DataMember]
        public string NextBlock { get; set; }
        [DataMember]
        public bool Running { get; set; }
        [DataMember]
        public bool Paused { get; set; }
    }
}
