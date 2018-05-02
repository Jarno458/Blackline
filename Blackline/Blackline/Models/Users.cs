using System.Collections.Generic;
using System.Xml.Serialization;

namespace Blackline.Models
{
    [XmlRoot("users")]
    public class Users
    {
        [XmlElement("user")]
        public List<User> UserList { get; set; }
    }
}