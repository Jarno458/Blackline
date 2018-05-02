using System.Xml.Serialization;

namespace Blackline.Models
{
    public class User
    {
        [XmlElement("userName")]
        public string UserName { get; set; }

        [XmlElement("password")]
        public string Password { get; set; }

        [XmlElement("level")]
        public int Level { get; set; }
    }
}