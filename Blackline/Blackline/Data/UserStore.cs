using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Blackline.Models;

namespace Blackline.Data
{
    public class UserStore
    {
        private IList<User> _users;

        public IList<User> GetUsers()
        {
            return _users ?? (_users =  GetUsersCore());
        }

        private static IList<User> GetUsersCore()
        {
            var path = GetFilePath(@"Data\Users.xml");
            
            var serializer = new XmlSerializer(typeof(Users));
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                return ((Users) serializer.Deserialize(fileStream)).UserList;
            }
        }

	    static string GetFilePath(string path)
	    {
		    return Path.Combine(System.Web.HttpRuntime.BinDirectory, path);
	    }
	}
}