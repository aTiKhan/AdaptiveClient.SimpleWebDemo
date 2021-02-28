using System;
using System.Collections.Generic;
using System.Text;

namespace AdaptiveClient.WebDemo
{
    class UsersService_MySQL : IUsersService
    {
        public void SaveUser(User user)
        {
           // call database client here and insert using MySQL syntax....
        }

        public User GetUserByID(int id)
        {
            // call database client here and select using MySQL syntax...
            return new User { ID = id, Name = "Bob (retrieved from MySQL)" };
        }
    }
}
