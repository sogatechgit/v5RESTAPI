using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using DataAccess;


namespace NgArbi.Models
{
    static public class RunModels
    {
        static public JObject Invoke(JProperty args)
        {

            /********************************************************************************
             * args contains all necessary parameters needed to execute application-specific
             * methods. 
             * Keys required to run methods:
             *   command - indicates the action that will be performed. The value
             *      assigned to this key can be used in a switch(command) statment
             *      to conditionally select which method must be invoked
             *   data - JObject or JArray object which will be consumed by the method
             *      invoked.
             ********************************************************************************/

            JObject ret = new JObject();
            JObject data = (JObject)args.Value;
            try
            {
                // convert property to JObject
                DALData.DAL.LogGlobalMessage("Entered Run Method!!! ", "runMode");

                // extract dommand code from the parameter object
                string cmd = (string)data["command"];
                DALData.DAL.LogGlobalMessage(cmd, "command");

                if (data.ContainsKey("data"))
                {
                    DALData.DAL.LogGlobalMessage(data.Count, "dataCount");
                }

                if (cmd == "calculate")
                {
                    ret.Add("calcResult",Calculate((int)data["processId"]));
                }
                else
                {
                    DALData.DAL.LogGlobalMessage(string.Format("Command {0} does nothing", cmd), "command");
                }

                ret.Add("InvokeStatus", "Success");
            }
            catch (Exception e)
            {
                ret.Add("ErrorDetails", e.ToString());
                ret.Add("InvokeStatus", "Error");
            }

            return ret;
        }


        static int Calculate(int key)
        {
            return key * 5;
            // This is an example of a method specific to the project

        }
    }
}