using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Classes;
using VAdvantage.Utility;


namespace VIS.Helpers
{
    public class ContextHelper
    {

        public static Ctx Get(string jsonString)
        {
            Ctx ctx = new Context(jsonString);
            return ctx;
        }

    }
}