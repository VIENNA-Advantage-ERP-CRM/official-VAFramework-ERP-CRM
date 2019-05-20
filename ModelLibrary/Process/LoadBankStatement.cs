using System;
using System.Collections.Generic;
//using System.Linq;
using VAdvantage.Model;
using VAdvantage.Process;
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Text;
using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class LoadBankStatement : ProcessEngine.SvrProcess
    {
        public LoadBankStatement()
            : base()
        {

            log.Info("LoadBankStatement");
        }	//	LoadBankStatement

        /**	Client to be imported to			*/
        private int _AD_Client_ID = 0;

        /** Organization to be imported to			*/
        private int _AD_Org_ID = 0;

        /** Ban Statement Loader				*/
        private int _C_BankStmtLoader_ID = 0;

        /** File to be imported					*/
        private String fileName = "";

        /** Current context					*/
        private Ctx m_ctx;

        /** Current context					*/
        private MBankStatementLoader _controller = null;


        /// <summary>
        ///  Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            log.Info("");
            m_ctx = GetCtx();
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (name.Equals("C_BankStatementLoader_ID"))
                    _C_BankStmtLoader_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());
                else if (name.Equals("FileName"))
                    fileName = Utility.Util.GetValueOfString(para[i].GetParameter());
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
            _AD_Client_ID = m_ctx.GetAD_Client_ID();
            log.Info("AD_Client_ID=" + _AD_Client_ID);
            _AD_Org_ID = m_ctx.GetAD_Org_ID();
            log.Info("AD_Org_ID=" + _AD_Org_ID);
            log.Info("C_BankStatementLoader_ID=" + _C_BankStmtLoader_ID);
        }	//	prepare


        /// <summary>
        /// message
        /// </summary>
        /// <returns></returns>
        protected override String DoIt()
        {
            log.Info("LoadBankStatement.doIt");
            String message = "@Error@";

            _controller = new MBankStatementLoader(m_ctx, _C_BankStmtLoader_ID, fileName, Get_TrxName());
            log.Info(_controller.ToString());

            if (_controller == null || _controller.Get_ID() == 0)
                log.Log(Level.SEVERE, "Invalid Loader");

            // Start loading bank statement lines
            else if (!_controller.LoadLines())
                log.Log(Level.SEVERE, _controller.GetErrorMessage() + " - " + _controller.GetErrorDescription());

            else
            {
                log.Info("Imported=" + _controller.GetLoadCount());
                AddLog(0, null, Utility.Util.GetValueOfDecimal(_controller.GetLoadCount()), "@Loaded@");
                message = "@OK@";
            }

            return message;
        }	//	doIt

    }
}
