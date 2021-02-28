using AdaptiveClient.WebDemo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using LeaderAnalytics.AdaptiveClient;

namespace AdaptiveClient.WebDemo.Pages
{
    public class IndexModel : PageModel
    {
        public List<IEndPointConfiguration> EndPoints { get; private set; }
        public User DemoUser { get; private set; }
        public string SelectedEndPointName { get; private set; }
        private IAdaptiveClient<IUsersService> client;

        public IndexModel(IAdaptiveClient<IUsersService> client, List<IEndPointConfiguration> endPoints)
        {
            // EndPoints are injected here because this is a demo and we need to populate the dropdown list on this page.
            // AdaptiveClient does not require EndPoints be passed to any object so you should not inject EndPoints into 
            // your Controller, ViewModel, etc. unless you specifically need them.

            this.client = client;
            this.EndPoints = endPoints;
        }

        public void OnGet()
        {
            if (SelectedEndPointName == null)
                SelectedEndPointName = "Prod_MSSQL_01";

            GetUser();
        }

        public void OnPost()
        {
            SelectedEndPointName = Request.Form["SelectedEndPointName"];
            GetUser();
        }

        private void GetUser()
        {
            IEndPointConfiguration endPoint = EndPoints.First(x => x.Name == SelectedEndPointName);
            Logger.Message = null;

            if (SelectedEndPointName == "Prod_WCF_01")
            {
                // This end point was registered with mocks so it will fail and fall back to Prod_MSSQL_01
                // If the current endpoint is Prod_MSSQL_01 you won't see an error message because AdaptiveClient
                // will continue to use that endpoint.
                DemoUser = client.Try(usersService => usersService.GetUserByID(1), endPoint.Name, "Prod_MSSQL_01");
            }
            else
                DemoUser = client.Call(usersService => usersService.GetUserByID(1), endPoint.Name);
        }
    }
}
