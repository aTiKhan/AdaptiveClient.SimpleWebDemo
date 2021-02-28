using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Autofac;
using LeaderAnalytics.AdaptiveClient;
using LeaderAnalytics.AdaptiveClient.Utilities;

namespace AdaptiveClient.WebDemo
{

    

    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegistrationHelper registrationHelper = new RegistrationHelper(builder);
            List<IEndPointConfiguration> endPoints = EndPointUtilities.LoadEndPoints("appsettings.json").ToList();
            builder.RegisterInstance(endPoints).SingleInstance(); // Register the endpoints with Autofac. Most applications will not need to do this. 

            // API Name is an arbitrary name of your choosing that AdaptiveClient uses to link interfaces (IUsersService) to the
            // EndPoints that expose them (Prod_SQL_01, Prod_WebAPI_01).  The API_Name used here must match the API_Name 
            // of related EndPoints in EndPoints.json file.

            // Register the endpoints with AdaptiveClient. All applications using AdaptiveClient need to do this.  Always register endPoints before registering clients.
            registrationHelper.RegisterEndPoints(endPoints); 

            registrationHelper.RegisterService<UsersService_WebAPI, IUsersService>(EndPointType.HTTP, API_Name.DemoAPI, DataProvider.HTTP);
            registrationHelper.RegisterService<UsersService_MSSQL, IUsersService>(EndPointType.InProcess, API_Name.DemoAPI, DataProvider.MSSQL);
            registrationHelper.RegisterService<UsersService_MySQL, IUsersService>(EndPointType.InProcess, API_Name.DemoAPI, DataProvider.MySQL);
            registrationHelper.RegisterLogger(logMessage => Logger.Message = logMessage);


            // EndPoint Validators
            // No servers for MSSQL or MySQL and we don't have an API url so register some mocks...
            Mock<IEndPointValidator> validatorMock = new Mock<IEndPointValidator>();
            validatorMock.Setup(x => x.IsInterfaceAlive(It.IsAny<IEndPointConfiguration>())).Returns(true);
            builder.RegisterInstance(validatorMock.Object).Keyed<IEndPointValidator>(EndPointType.InProcess + DataProvider.MSSQL);
            builder.RegisterInstance(validatorMock.Object).Keyed<IEndPointValidator>(EndPointType.InProcess + DataProvider.MySQL);
            builder.RegisterInstance(validatorMock.Object).Keyed<IEndPointValidator>(EndPointType.HTTP + DataProvider.HTTP);

            // In a real app...
            //registrationHelper.RegisterEndPointValidator<MSSQL_EndPointValidator>(EndPointType.InProcess, DataProvider.MSSQL);
            //registrationHelper.RegisterEndPointValidator<MySQL_EndPointValidator>(EndPointType.InProcess, DataProvider.MySQL);



            // WCF (will fail and fall back)
            var usersServiceMock = new Mock<IUsersService>();
            usersServiceMock.Setup(x => x.SaveUser(It.IsAny<User>())).Throws(new Exception("Cant find database server."));
            usersServiceMock.Setup(x => x.GetUserByID(It.IsAny<int>())).Throws(new Exception("Cant find database server."));
            builder.RegisterInstance(usersServiceMock.Object).Keyed<IUsersService>(EndPointType.WCF + DataProvider.HTTP);
            registrationHelper.RegisterLogger(logMessage => Logger.Message = logMessage);
        }
    }
}
