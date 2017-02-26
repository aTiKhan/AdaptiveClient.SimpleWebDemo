using System;
using System.Collections.Generic;
using System.Text;
using LeaderAnalytics.AdaptiveClient;

namespace AdaptiveClient.WebDemo
{

    public class Demo
    {
        private IAdaptiveClient<IUsersService> client;
        private HomeViewModel viewModel;

        public Demo(IAdaptiveClient<IUsersService> client)
        {
            this.client = client;
            this.viewModel = new HomeViewModel();
        }

        public HomeViewModel BuildViewModel()
        {
            // In this demo we make some service calls and populate
            // our ViewModel with info about what Endpoint was used.
            // Ideally we have access to a SQL server on the LAN but if 
            // that fails we fall back to a WebAPI server:

            User user = GetAUser(1);
            SaveAUser(user);
            return viewModel;
        }

        private User GetAUser(int id)
        {
            User user = client.Try(x => x.GetUserByID(id));
            viewModel.GetAUserResult = $"User {user.Name} was found.  EndPoint used was {client.CurrentEndPoint.Name}.";
            return user;
        }

        private void SaveAUser(User user)
        {
            client.Try(x => x.SaveUser(user));
            viewModel.SaveAUserResult = $"User {user.Name} was saved.  EndPoint used was {client.CurrentEndPoint.Name}.";
        }
    }
}
