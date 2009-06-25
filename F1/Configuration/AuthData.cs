using System;
using System.IO;
using System.Xml.Serialization;

namespace F1.Configuration
{
    /// <summary>
    /// A serializable type for storing and reading auth data from a file.
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName ="AuthData", IsNullable = false, Namespace="")]
    public class AuthData
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public static AuthData Load(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer( typeof(AuthData) );

            StreamReader reader = File.OpenText(fileName);

            return serializer.Deserialize(reader) as AuthData;
        }

        public void Save(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AuthData));

            using (TextWriter writer = new StreamWriter(fileName))
            {
                serializer.Serialize(writer, this);
                writer.Close();
            }
        }
    }
}
