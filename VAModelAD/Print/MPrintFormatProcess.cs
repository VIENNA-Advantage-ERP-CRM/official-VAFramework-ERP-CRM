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
        private Decimal? _VAF_Print_Rpt_Layout_ID;
        /** Table	                */
        private Decimal? _VAF_TableView_ID;

        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                { ; }
                else if (name.Equals("VAF_Print_Rpt_Layout_ID"))
                    _VAF_Print_Rpt_Layout_ID = ((Decimal?)para[i].GetParameter());
                else if (name.Equals("VAF_TableView_ID"))
                    _VAF_TableView_ID = ((Decimal?)para[i].GetParameter());
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
            if (_VAF_TableView_ID != null && int.Parse(_VAF_TableView_ID.ToString()) > 0)
            {

                MVAFPrintRptLayout pf = MVAFPrintRptLayout.CreateFromTable(GetCtx(), int.Parse(_VAF_TableView_ID.ToString()), GetRecord_ID());
                AddLog(Utility.Util.GetValueOfInt(_VAF_TableView_ID.ToString()), null, pf.GetItemCount(), pf.GetName());
                return pf.GetName() + " #" + pf.GetItemCount();
            }
            else if (_VAF_Print_Rpt_Layout_ID != null && _VAF_Print_Rpt_Layout_ID > 0)
            {
                MVAFPrintRptLayout pf = MVAFPrintRptLayout.Copy(GetCtx(), Utility.Util.GetValueOfInt(_VAF_Print_Rpt_Layout_ID.ToString()), GetRecord_ID());
                AddLog(Utility.Util.GetValueOfInt(_VAF_Print_Rpt_Layout_ID.ToString()), null, pf.GetItemCount(), pf.GetName());
                return pf.GetName() + " #" + pf.GetItemCount();
            }
            else
                throw new Exception(msgInvalidArguments);
        }
    }
}
