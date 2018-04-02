using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Fabric.Description
{
    /// <summary>
    /// Extends the <c>System.Fabric.Description</c> namespace.
    /// </summary>
    public static class FabricDescriptionExtensionMethods
    {
        /// <summary>
        /// Reads a <see cref="TimeSpan"/> from <paramref name="config"/>.
        /// </summary>
        /// <param name="config">The <see cref="ConfigurationSection"/> being extended.</param>
        /// <param name="name">A <see cref="string"/> that contains the name of the vluae.</param>
        /// <param name="defaultValue">A <typparamref name="TValue"/> value used as default.</param>
        /// <returns>The resultig <see cref="TimeSpan"/></returns>
        public static TimeSpan ReadValue(this ConfigurationSection config, string name, TimeSpan defaultValue)
        {
            TimeSpan ret;
            var input = config.ReadValue(name);
            return input != null && TimeSpan.TryParse(input, out ret)
                ? ret
                : defaultValue;
        }
        /// <summary>
        /// Reads a <see cref="uint"/> from <paramref name="config"/>.
        /// </summary>
        /// <param name="config">The <see cref="ConfigurationSection"/> being extended.</param>
        /// <param name="name">A <see cref="string"/> that contains the name of the vluae.</param>
        /// <param name="defaultValue">A <typparamref name="TValue"/> value used as default.</param>
        /// <returns>The resultig <see cref="TimeSpan"/></returns>
        public static uint ReadValue(this ConfigurationSection config, string name, uint defaultValue)
        {
            uint ret;
            var input = config.ReadValue(name);
            return input != null && uint.TryParse(input, out ret)
                ? ret
                : defaultValue;
        }
        /// <summary>
        /// Reads a <see cref="TimeSpan"/> from <paramref name="config"/>.
        /// </summary>
        /// <param name="config">The <see cref="ConfigurationSection"/> being extended.</param>
        /// <param name="name">A <see cref="string"/> that contains the name of the vluae.</param>
        /// <param name="defaultValue">A <typparamref name="TValue"/> value used as default.</param>
        /// <returns>The resultig <see cref="TimeSpan"/></returns>
        public static string ReadValue(this ConfigurationSection config, string name, string defaultValue = default(string))
        {
            var param = config.Parameters.FirstOrDefault(x => x.Name == name);
            return param == null
                ? defaultValue
                : param.Value;
        }
    }
}
