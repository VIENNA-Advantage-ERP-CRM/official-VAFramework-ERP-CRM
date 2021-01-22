/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : DocWorkflowMgr
 * Purpose        : 
 * Chronological    Development
 * Raghunandan      06-May-2009 
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;

using VAdvantage.Model;

using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.SqlExec;

namespace VAdvantage.WF
{
    public interface DocWorkflowMgr
    {

        /// <summary>
        ///Process Document Value Workflow
        /// </summary>
        /// <param name="document">document</param>
        /// <param name="VAF_TableView_ID">table</param>
        /// <returns> true if WF started</returns>
        bool Process(PO document, int VAF_TableView_ID);
    }
}
