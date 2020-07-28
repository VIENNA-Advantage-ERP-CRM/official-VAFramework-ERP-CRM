/********************************************************
    * Project Name   : VAdvantage
    * Class Name     : DropFullMoveContainerLines
    * Purpose        : Is used to drop all movement line when movement is of Full Movement container
                       In Future we will take Parameter as "Parent Container" -- based on this parameter -- we will drop line of selected parent and its child only
    * Class Used     : ProcessEngine.SvrProcess
    * Chronological    Development
    * Amit Bansal     23-Oct-2018
******************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;

namespace VAdvantage.Process
{
    class DropFullMoveContainerLines : SvrProcess
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(DropFullMoveContainerLines).FullName);

        protected override void Prepare()
        {
            ;
        }

        protected override string DoIt()
        {
            //Is used to drop all movement line when movement is of Full Movement container
            int no = DB.ExecuteQuery("DELETE FROM M_MovementLine WHERE M_Movement_ID = " + GetRecord_ID(), null, Get_Trx());
            _log.Info(no + " records delete from movement line, movement id =  " + GetRecord_ID());
            if (no >= 0)
                DB.ExecuteQuery("Update M_Movement SET DocStatus = 'DR' WHERE M_Movement_ID = " + GetRecord_ID(), null, Get_Trx());
            if (no <= 0)
                return Msg.GetMsg(GetCtx(), "VIS_NoRecordsFound"); // No document line found.
            return Msg.GetMsg(GetCtx(), "VIS_RecordsDeleted") + no; // All records on line deleted successfully - 
        }
    }
}
