using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;

using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Globalization;
using VAdvantage.ProcessEngine;
using VAdvantage.Report;

namespace ViennaAdvantage.Process
{
    public class TestProcess : SvrProcess
    {
        protected override string DoIt()
        {
            log.SaveError("Process "  + System.DateTime.Now.ToString(), "");
            return "Executed" + System.DateTime.Now.ToString();
        }

        protected override void Prepare()
        {
            
        }
    }
}
