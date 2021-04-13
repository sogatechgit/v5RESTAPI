﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using DataAccess;
using Newtonsoft.Json.Linq;
using _g = DataAccess.AppGlobals2;
using System.Diagnostics;


namespace NgArbi.Controllers
{
    // [Authorize] // commented to bypass authorization
    // [Authorize(Users = "SOGA-ALV\\alv")]
    public class ValuesController : ApiController
    {


        // GET api/values
        public IEnumerable<string> Get()
        {
            //DALTable tbl = new DALTable();

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        
        // POST api/values
        public JObject Post([FromBody]JObject value)
        {
            return value;
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }


    }
}
