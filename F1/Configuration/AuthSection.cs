using System.Configuration;

namespace F1.Configuration
{
    /// <summary>
    /// The AuthSection configuration section defines the configurable properties
    /// used for authorisation with the live timing.
    /// </summary>
    public sealed class AuthSection : ConfigurationSection
    {
        public AuthSection()
        {
             
        }

        [ConfigurationProperty("userName", IsRequired=true)]
        //[RegexStringValidator(@".+@.+\..+")] // simple email validation
        public string UserName
        {
            get
            {
                return this["userName"] as string;
            }
        }


        [ConfigurationProperty("password", IsRequired=true)]
        //[StringValidator(MinLength=4)]
        public string Password
        {
            get
            {
                return this["password"] as string;
            }
        }
    }
}
