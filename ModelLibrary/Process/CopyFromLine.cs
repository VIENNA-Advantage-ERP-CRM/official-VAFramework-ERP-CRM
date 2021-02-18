using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;

namespace VAdvantage.Process
{
   public class CopyFromLine : SvrProcess
    {
        private int _VAGL_JRNL_ID = 0;

        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("VAGL_JRNL_ID"))
                {
                    _VAGL_JRNL_ID = Util.GetValueOfInt((Decimal)para[i].GetParameter());
                }
                else
                {
                    log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
                }
            }
        }

        protected override string DoIt()
        {
            int To_VAGL_JRNL_ID = GetRecord_ID();
            log.Info("doIt - From VAGL_BatchJRNL_ID=" + _VAGL_JRNL_ID + " to " + To_VAGL_JRNL_ID);
            if (To_VAGL_JRNL_ID == 0)
            {
                throw new ArgumentException("Target VAGL_BatchJRNL_ID == 0");
            }
            if (_VAGL_JRNL_ID == 0)
            {
                throw new ArgumentException("Source VAGL_BatchJRNL_ID == 0");
            }
            VAdvantage.Model.MVAGLJRNL from = new VAdvantage.Model.MVAGLJRNL(GetCtx(), _VAGL_JRNL_ID, Get_Trx());
            VAdvantage.Model.MVAGLJRNL to = new VAdvantage.Model.MVAGLJRNL(GetCtx(), To_VAGL_JRNL_ID, Get_Trx());
            //
            int no = to.CopyLines(from, 'x');
            //
            return "@Copied@=" + no;
        }
    }
}
