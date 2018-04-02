using System;
using System.Runtime.Serialization;
namespace CryptoKitties.Net.Fabric.Models
{
    /// <summary>
    /// The <see cref="Timestamped{TValue}"/> class is used to track a timestamped complex type.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [DataContract]
    [Serializable]
    public class Timestamped<TValue>
    {
        /// <summary>
        /// Initializes a new <see cref="Timestamped{T}"/> instance.
        /// </summary>
        public Timestamped()
        { }
        /// <summary>
        /// Initializes a new <see cref="Timestamped{TValue}"/> instance.
        /// </summary>
        /// <param name="value"></param>
        public Timestamped(TValue value)
        {
            SetValue(value);
        }
        /// <summary>
        /// Initializes a new <see cref="Timestamped{T}"/> instance.
        /// </summary>
        /// <param name="value">The initial <see cref="Value"/> value.</param>
        /// <param name="asOf">The initial <see cref="AsOf"/> value.</param>
        public Timestamped(TValue value, DateTime asOf)
        {
            Value = value;
            AsOf = asOf;
        }
        /// <summary>
        /// The <see cref="DateTime"/> that <see cref="Value"/> was last updated.
        /// </summary>
        [DataMember]
        public DateTime AsOf { get; set; }
        /// <summary>
        /// The <typeparamref name="TValue"/> tracked by this instance.
        /// </summary>
        [DataMember]
        public TValue Value { get; set; }
        /// <summary>
        /// The current age of this instance.
        /// </summary>
        public TimeSpan Age => DateTime.UtcNow - AsOf;
        /// <summary>
        /// Updates <see cref="Value"/> and <see cref="AsOf"/> psuedo-atomically.
        /// </summary>
        /// <param name="value">The new <typeparam nameref="TValue">.</typeparam></param>
        public void SetValue(TValue value)
        {
            AsOf = DateTime.UtcNow;
            Value = value;
        }
        /// <summary>
        /// Determines whether or not this instance has expired given a timeout of <paramref name="expireAfter"/>.
        /// </summary>
        /// <param name="expireAfter">A <see cref="TimeSpan"/> to compare against.</param>
        /// <returns><c>true</c> if this instance has expired; otherwise, <c>false</c>.</returns>
        public bool IsExpired(TimeSpan expireAfter)
        {
            return expireAfter < Age;
        }
        /// <summary>
        /// Implicitly converts a <see cref="Timestamped{TValue}"/> into it&apos;s tracked <see cref="Value"/>.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator TValue(Timestamped<TValue> value)
        {
            return value == null ? default(TValue) : value.Value;
        }
    }
}
