using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.ProcessEngine;

namespace VAdvantage.Print
{
    public class MPrintFormatProcess : ProcessEngine.SvrProcess
    {
        /** PrintFormat             */
        private Decimal? _AD_PrintFormat_ID;
        /** Table	                */
        private Decimal? _AD_Table_ID;

        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                { ; }
                else if (name.Equals("AD_PrintFormat_ID"))
                    _AD_PrintFormat_ID = ((Decimal?)para[i].GetParameter());
                else if (name.Equals("AD_Table_ID"))
                    _AD_Table_ID = ((Decimal?)para[i].GetParameter());
                else
                    log.Equals("prepare - Unknown Parameter=" + para[i].GetParameterName());
            }
        }   //  prepare

        /// <summary>
        /// doit
        /// </summary>
        /// <returns>info</returns>
        protected override string DoIt()
        {
            if (_AD_Table_ID != null && int.Parse(_AD_Table_ID.ToString()) > 0)
            {

                MPrintFormat pf = MPrintFormat.CreateFromTable(GetCtx(), int.Parse(_AD_Table_ID.ToString()), GetRecord_ID());
                AddLog(Utility.Util.GetValueOfInt(_AD_Table_ID.ToString()), null, pf.GetItemCount(), pf.GetName());
                return pf.GetName() + " #" + pf.GetItemCount();
            }
            else if (_AD_PrintFormat_ID != null && _AD_PrintFormat_ID > 0)
            {
                MPrintFormat pf = MPrintFormat.Copy(GetCtx(), Utility.Util.GetValueOfInt(_AD_PrintFormat_ID.ToString()), GetRecord_ID());
                AddLog(Utility.Util.GetValueOfInt(_AD_PrintFormat_ID.ToString()), null, pf.GetItemCount(), pf.GetName());
                return pf.GetName() + " #" + pf.GetItemCount();
            }
            else
                throw new Exception(msgInvalidArguments);
        }
    }
}
