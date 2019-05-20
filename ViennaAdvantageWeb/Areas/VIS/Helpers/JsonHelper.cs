using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace VIS.Helpers
{
    public class JsonHelper
    {
        private static JavaScriptSerializer json = new JavaScriptSerializer();

        public static object Deserialize(string jsonString, Type type)
        {
            return json.Deserialize(jsonString, type);
        }

        public static string Serialize(object model)
        {
            return json.Serialize(model);
        }

    }
}