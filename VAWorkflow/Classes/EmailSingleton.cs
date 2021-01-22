using System;
using System.Data;
using System.IO;
using System.Threading;
using VAdvantage.Common;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VAWorkflow.Classes
{
    // Sealed restricts the inheritence
    public sealed class EmailSingleton
    {
        // Private variable which is readonly to implement thread safety and avoid race condition
        private static readonly Lazy<EmailSingleton> lazy = new Lazy<EmailSingleton>(() => new EmailSingleton());

        // Thread to run function 
        private static Thread thread = null;

        // Public property is used to return only one instance of the class leveraging on the private property
        public static EmailSingleton Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        // Private constructor ensures that object is not instantiated other than with in the class itself
        private EmailSingleton()
        { }

        // Public method which can be invoked through the singleton class
        public void StartEmailing()
        {
            if (thread == null || !thread.IsAlive)
            {
                thread = new Thread(Email);
                thread.IsBackground = true;
                thread.Start();
            }
        }

        // Private method called from with the class
        private static void Email()
        {
            // Run this itration infinetly
            try
            {
                while (true)
                {
                    // Get records from mail queue table to send them one by one as email
                    DataSet mailds = DB.ExecuteDataset("SELECT VAF_Org_ID, VAF_Client_ID, CreatedBy, VAF_Role_ID, ToEMail, ToName, MailSubject, MailMessage, IsHtmlEmail, VAF_TableView_ID, Record_ID, VAF_WFlow_Task_ID, VAF_WFlow_EventLog_ID, VAF_MailQueue_ID, VAF_WFlow_Handler_ID FROM VAF_MailQueue WHERE MailStatus = 'Q' AND ROWNUM <= 5 ORDER BY VAF_MailQueue_ID");

                    if (mailds != null && mailds.Tables.Count > 0 && mailds.Tables[0].Rows.Count > 0)
                    {
                        for (int m = 0; m < mailds.Tables[0].Rows.Count; m++)
                        {
                            int VAF_Org_ID = Util.GetValueOfInt(mailds.Tables[0].Rows[m]["VAF_Org_ID"]);
                            int VAF_Client_ID = Util.GetValueOfInt(mailds.Tables[0].Rows[m]["VAF_Client_ID"]);
                            int VAF_UserContact_ID = Util.GetValueOfInt(mailds.Tables[0].Rows[m]["CreatedBy"]);
                            int VAF_Role_ID = Util.GetValueOfInt(mailds.Tables[0].Rows[m]["VAF_Role_ID"]);

                            String toEMail = Util.GetValueOfString(mailds.Tables[0].Rows[m]["ToEMail"]);
                            String toName = Util.GetValueOfString(mailds.Tables[0].Rows[m]["ToName"]);
                            String subject = Util.GetValueOfString(mailds.Tables[0].Rows[m]["MailSubject"]);
                            String message = Util.GetValueOfString(mailds.Tables[0].Rows[m]["MailMessage"]);
                            FileInfo attachment = null;
                            bool isHtml = Util.GetValueOfString(mailds.Tables[0].Rows[m]["IsHtmlEmail"]) == "Y" ? true : false;
                            int VAF_TableView_ID = Util.GetValueOfInt(mailds.Tables[0].Rows[m]["VAF_TableView_ID"]);
                            int Record_ID = Util.GetValueOfInt(mailds.Tables[0].Rows[m]["Record_ID"]);
                            byte[] array = null;
                            String fileName = null;
                            int VAF_WFlow_Task_ID = Util.GetValueOfInt(mailds.Tables[0].Rows[m]["VAF_WFlow_Task_ID"]);
                            int VAF_WFlow_EventLog_ID = Util.GetValueOfInt(mailds.Tables[0].Rows[m]["VAF_WFlow_EventLog_ID"]);
                            int VAF_WFlow_Handler_ID = Util.GetValueOfInt(mailds.Tables[0].Rows[m]["VAF_WFlow_Handler_ID"]);
                            int VAF_MailQueue_ID = Util.GetValueOfInt(mailds.Tables[0].Rows[m]["VAF_MailQueue_ID"]);


                            // Create context
                            Ctx _ctx = new Ctx();
                            _ctx.SetVAF_Org_ID(VAF_Org_ID);
                            _ctx.SetVAF_Client_ID(VAF_Client_ID);
                            _ctx.SetVAF_UserContact_ID(VAF_UserContact_ID);
                            _ctx.SetVAF_Role_ID(VAF_Role_ID);


                            string sql = "SELECT VAF_WFLOW_NODE_ID FROM VAF_WFlow_Task WHERE VAF_WFlow_Task_ID=" + VAF_WFlow_Task_ID;
                            object nodeID = DB.ExecuteScalar(sql);
                            X_VAF_WFlow_Node node = null;

                            if (nodeID != null && nodeID != DBNull.Value)
                            {
                                node = new X_VAF_WFlow_Node(_ctx, Convert.ToInt32(nodeID), null);
                            }
                            FileInfo pdf = null;
                            MTable table = MTable.Get(_ctx, VAF_TableView_ID);
                            PO po = table.GetPO(_ctx, Record_ID, null);
                            if (node.IsAttachReport())
                            {
                                VAdvantage.Common.Common com = new Common();
                                pdf = com.GetPdfReportForMail(_ctx, null, VAF_TableView_ID, Record_ID, VAF_UserContact_ID, VAF_Client_ID,
                                   "", 0, VAF_WFlow_Task_ID);
                            }
                            else if (po is VAdvantage.Process.DocAction)
                            {
                                VAdvantage.Process.DocAction doc = (VAdvantage.Process.DocAction)po;
                                attachment = doc.CreatePDF();
                            }
                            // Check if there is m class for the record and table and fetch attachment from that class 
                            //MTable table = MTable.Get(_ctx, VAF_TableView_ID);
                            //PO po = table.GetPO(_ctx, Record_ID, null);

                            //if (po is VAdvantage.Process.DocAction) // MClass Implement DocAction
                            //{
                            //VAdvantage.Process.DocAction doc = (VAdvantage.Process.DocAction)po;
                            //attachment = doc.CreatePDF();
                            //VAdvantage.Common.Common com = new Common();
                            //FileInfo pdf = com.GetPdfReportForMail(_ctx, null, VAF_TableView_ID, Record_ID, VAF_UserContact_ID, VAF_Client_ID,
                            //   "", 0, VAF_WFlow_Task_ID);
                            if (pdf != null)
                            {
                                array = File.ReadAllBytes(pdf.FullName);
                                fileName = pdf.Name;
                            }
                            //}

                            // Create Mclient object and send Email 
                            MClient client = MClient.Get(_ctx, VAF_Client_ID);
                            bool mailsent = client.SendEMail(toEMail, toName, subject, message, attachment, isHtml, VAF_TableView_ID, Record_ID, array, fileName);

                            ViennaAdvantage.Model.X_VAF_MailQueue mailQueue = new ViennaAdvantage.Model.X_VAF_MailQueue(_ctx, VAF_MailQueue_ID, null);

                            if (mailsent)
                            {
                                mailQueue.SetMailStatus("S");
                                int act1 = DB.ExecuteQuery("UPDATE VAF_WFlow_Task SET WFSTATE = 'CC' WHERE VAF_WFlow_Task_ID = " + VAF_WFlow_Task_ID);
                                int aud1 = DB.ExecuteQuery("UPDATE VAF_WFlow_EventLog SET WFSTATE = 'CC' WHERE VAF_WFlow_EventLog_ID = " + VAF_WFlow_EventLog_ID);
                                int wpro = DB.ExecuteQuery("UPDATE VAF_WFlow_Handler SET WFSTATE = 'CC' WHERE VAF_WFlow_Handler_ID = " + VAF_WFlow_Handler_ID + " AND WFState = 'BK' ");
                            }
                            else
                            {
                                mailQueue.SetMailStatus("F");
                                VLogger.Get().Warning("Email not sent by singleton class, marking as failed");
                            }

                            bool mq = mailQueue.Save();

                            // Sleep thread by 1 second to let memory work properly
                            Thread.Sleep(1000);

                            // For testing thread abort problem 
                            //thread.Abort();
                        }
                        int deleteoldmails = DB.ExecuteQuery("DELETE FROM VAF_MailQueue WHERE (MailStatus = 'S' OR MailStatus = 'F') AND Created <=" + GlobalVariable.TO_DATE(DateTime.Now, false) + " - 7");
                    }
                    else
                    {
                        // Sleep thread by 5 seconds if there is no record in mail queue table
                        Thread.Sleep(60000 * 5);
                    }

                }
            }
            catch (Exception e)
            {
                VLogger.Get().Severe("Error in Email singleton class " + e.Message);
                try
                {
                    // Check if state is not running 
                    if (thread.ThreadState != ThreadState.Running)
                        Thread.ResetAbort();
                    else
                    {
                        thread.Abort();
                        Thread.ResetAbort();
                    }
                }
                catch (Exception ex)
                { }
                finally
                {
                    thread = null;
                }
            }
        }
    }
}
