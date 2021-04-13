using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using DataAccess;
using System.Configuration;

namespace NgArbi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            /*************************** custom calls ******************************************/

            // Initialize data access connection and other properties
            // DAL.connectionString = ConfigurationManager.ConnectionStrings["cnsAppAPI"].ConnectionString;

            //************************** Transfer to Controller Start **************************
            DALGlobals.APP_SETTINGS = DataAccess.AppGlobals2.AppSetings;
            DALData.DAL.connectionString = ConfigurationManager.ConnectionStrings[DALGlobals.APP_SETTINGS["CONNECTION_NAME"]].ConnectionString;
            DALGlobals.GeneralRetObj = new ReturnObjectExternal();
            AppDataset.configPath = "";
            AppDataset.clientDevPath = "";

             AppDataset.Initialize(); // Initialize dataset
             //************************** Transfer to Controller End **************************


            //DALData.DAL.LogMessage("Application Started ..");
            //DALData.DAL.LogMessage("Schema Path: " + DataAccess.AppGlobals2.PATH_SCHEMA_CONFIG);
            //DALData.DAL.LogMessage("Client Tables Path: " + DataAccess.AppGlobals2.PATH_TARGET_TYPESCRIPT_PATH);
            //DALData.DAL.LogMessage(HttpContext.Current.Server.MapPath("App_Data"));


        }
    }
}
