﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Web.Http.Cors;

namespace NgArbi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            //config.EnableCors(new EnableCorsAttribute("http://localhost:4802", headers: "*", methods: "*"));
            config.EnableCors(new EnableCorsAttribute(origins: ConfigurationManager.AppSettings["cors-domain"], headers: "*", methods: "*"));
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{table}/{key}/{keyField}/{includedFields}/{filter}/{sortFields}/{pageNumber}/{pageSize}/{requestConfig}",
                // to skip optional topics/parameters, supply a plus (-) character 
                // eg. skip keyField => ../api/app/<table code>/1/-/1,2,3
                // if supplied value for includedFields is 'df' - display fields, 
                //   columnInfo of the defined display fields will be resolved in the server
                // if supplied value for includedFields is comma delimited integers, 
                //   columnInfo of supplied indices will be resolved in the server
                defaults: new
                {
                    table = RouteParameter.Optional,
                    key = RouteParameter.Optional,
                    keyField = RouteParameter.Optional,
                    includedFields = RouteParameter.Optional,
                    filter = RouteParameter.Optional,
                    sortFields = RouteParameter.Optional,
                    pageNumber = RouteParameter.Optional,
                    pageSize = RouteParameter.Optional,
                    requestConfig = RouteParameter.Optional
                }
            );
        }
    }
}
