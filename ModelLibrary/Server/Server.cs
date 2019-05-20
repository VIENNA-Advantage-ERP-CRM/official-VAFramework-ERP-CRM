using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.Process;
using VAdvantage.DataBase;


namespace VAdvantage.Server
{
    public interface Server
    {
        String PostImmediate(IDictionary<string, string> ctx, int AD_Client_ID, int AD_Table_ID, int Record_ID, bool force, Trx trxName);
        string TryConnection(out string vconn);
        String GetDocumentNo(int AD_Client_ID, String TableName, Trx trxName);
        String GetDocumentNo(int C_DocType_ID, Trx trxName);
        //String GetDocumentNo(int AD_Client_ID, String TableName, Trx trxName);
        //String GetDocumentNo(int C_DocType_ID, Trx trxName);
        //GridWindowVO GetWindowVO(Ctx ctx, int WindowNo, int AD_Window_ID, int AD_Menu_ID);
        //ProcessInfo Process(Ctx ctx, ProcessInfo pi);
        //ProcessInfo Workflow(Ctx ctx, ProcessInfo pi, int AD_Workflow_ID);
        //bool PaymentOnline(Ctx ctx, int C_Payment_ID, int C_PaymentProcessor_ID, Trx trxName);
        //EMail CreateEMail(Ctx ctx, int AD_Client_ID, String toEMail, String toName, String subject, String message);
        //EMail CreateEMail(Ctx ctx, int AD_Client_ID, int AD_User_ID, String toEMail, String toName, String subject, String message);
        //String ExecuteTask(int AD_Task_ID);
        //int CacheReset(String tableName, int Record_ID);
        //bool UpdateLOB(String sql, int displayType, Object value);
        //String GetStatus();
    }
}
