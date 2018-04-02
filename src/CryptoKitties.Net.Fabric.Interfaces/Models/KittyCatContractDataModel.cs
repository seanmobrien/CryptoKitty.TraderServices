using System;
using System.Runtime.Serialization;

namespace CryptoKitties.Net.Fabric.Models
{
    [Serializable]
    [DataContract]
    public class KittyCatContractDataModel
    {
        [DataMember]
        public bool IsGestating { get; set; }
        [DataMember]
        public bool IsReady { get; set; }
        [DataMember]
        public int CooldownIndex { get; set; }
        [DataMember]
        public string NextActionAt { get; set; }
        [DataMember]
        public long SiringWithId { get; set; }
        [DataMember]
        public long MatronId { get; set; }
        [DataMember]
        public long SireId { get; set; }
        [DataMember]
        public string BirthTime { get; set; }
        [DataMember]
        public int Generation { get; set; }
        [DataMember]
        public string Genes { get; set; }
    }
}
