using System.Configuration;

namespace F1.Configuration
{
    /// <summary>
    /// The AuthSection configuration section defines the configurable properties
    /// used for authorisation with the live timing.
    /// </summary>
    public sealed class AuthSection : ConfigurationSection
    {
        [ConfigurationProperty("userName", IsRequired=true)]
        public string UserName
        {
            get
            {
                return this["userName"] as string;
            }
        }


        [ConfigurationProperty("password", IsRequired=true)]
        public string Password
        {
            get
            {
                return this["password"] as string;
            }
        }
    }
}
