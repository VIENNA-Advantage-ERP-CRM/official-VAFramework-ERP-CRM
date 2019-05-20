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
        private int _GL_Journal_ID = 0;

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
                else if (name.Equals("GL_Journal_ID"))
                {
                    _GL_Journal_ID = Util.GetValueOfInt((Decimal)para[i].GetParameter());
                }
                else
                {
                    log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
                }
            }
        }

        protected override string DoIt()
        {
            int To_GL_Journal_ID = GetRecord_ID();
            log.Info("doIt - From GL_JournalBatch_ID=" + _GL_Journal_ID + " to " + To_GL_Journal_ID);
            if (To_GL_Journal_ID == 0)
            {
                throw new ArgumentException("Target GL_JournalBatch_ID == 0");
            }
            if (_GL_Journal_ID == 0)
            {
                throw new ArgumentException("Source GL_JournalBatch_ID == 0");
            }
            VAdvantage.Model.MJournal from = new VAdvantage.Model.MJournal(GetCtx(), _GL_Journal_ID, Get_Trx());
            VAdvantage.Model.MJournal to = new VAdvantage.Model.MJournal(GetCtx(), To_GL_Journal_ID, Get_Trx());
            //
            int no = to.CopyLines(from, 'x');
            //
            return "@Copied@=" + no;
        }
    }
}
