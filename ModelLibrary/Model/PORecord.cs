/********************************************************
 * Module Name    : PO Delete
 * Purpose        : To delete various attachments/chats/etc. of a record
 * Class Used     : ---
 * Chronological Development
 * Veena Pandey     -Feb-2009
 ******************************************************/

using System;
//using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    /// <summary>
    /// To delete various attachments/chats/etc. of a record
    /// </summary>
   public  class PORecord
    {

        private static VLogger log = VLogger.GetVLogger(typeof(PORecord).FullName);

        //	Parent Table ID
        private static int[] _parents = new int[]{
        X_C_Order.Table_ID,
        X_CM_Container.Table_ID
        };

        //	Parent Table Names
        private static string[] _parentNames = new string[]{
        X_C_Order.Table_Name,
        X_CM_Container.Table_Name
        };

        // Child Table ID
        private static int[] _parentChilds = new int[]{
        X_C_OrderLine.Table_ID,
        X_CM_Container_Element.Table_ID
        };

        // Child Table Names
        private static string[] _parentChildNames = new string[]{
        X_C_OrderLine.Table_Name,
        X_CM_Container_Element.Table_Name
        };

        //	Cascade Table ID
        private static int[] _cascades = new int[]{
        X_AD_Attachment.Table_ID,
        X_AD_Archive.Table_ID,
        X_AD_Note.Table_ID,
        X_MailAttachment1.Table_ID,
        X_AppointmentsInfo.Table_ID,
         X_K_Index.Table_ID,

        };

        //	Cascade Table Names
        private static string[] _cascadeNames = new string[]{
        X_AD_Attachment.Table_Name,
        X_AD_Archive.Table_Name,
        X_AD_Note.Table_Name,
        X_MailAttachment1.Table_Name,
        X_AppointmentsInfo.Table_Name,
        X_K_Index.Table_Name 
        };

        //	Restrict Table ID
        private static int[] _restricts = new int[]{
        X_CM_Chat.Table_ID,
        X_R_Request.Table_ID
        };

        //	Restrict Table Names
        private static string[] _restrictNames = new string[]{
        X_CM_Chat.Table_Name,
            X_R_Request.Table_Name
        };

        /// <summary>
        /// Delete Cascade including (selected)parent relationships
        /// </summary>
        /// <param name="AD_Table_ID">table id</param>
        /// <param name="Record_ID">record id</param>
        /// <param name="trxName">transaction object</param>
        /// <returns>false if could not be deleted</returns>
        /// <author>Veena Pandey</author>
        public static bool DeleteCascade(int AD_Table_ID, int Record_ID, Trx trxName)
        {
            bool retval = true;
            //	Table Loop
            for (int i = 0; i < _cascades.Length; i++)
            {
                //	DELETE FROM table WHERE AD_Table_ID=#1 AND Record_ID=#2
                if (_cascades[i] != AD_Table_ID)
                {
                    StringBuilder sql = new StringBuilder("DELETE FROM ")
                        .Append(_cascadeNames[i])
                        .Append(" WHERE AD_Table_ID=" + AD_Table_ID + " AND Record_ID=" + Record_ID);
                    int no = DataBase.DB.ExecuteQuery(sql.ToString(), null, trxName);
                    if (no > 0)
                    {
                        log.Config(_cascadeNames[i] + " (" + AD_Table_ID + "/" + Record_ID + ") #" + no);
                    }
                    else if (no < 0)
                    {
                        log.Severe(_cascadeNames[i] + " (" + AD_Table_ID + "/" + Record_ID + ") #" + no);
                        retval = false;
                        break;
                        //return false;
                    }
                }
            }
            //	Parent Loop
            for (int j = 0; j < _parents.Length; j++)
            {
                // DELETE FROM AD_Attachment WHERE AD_Table_ID=1 AND Record_ID IN 
                //	(SELECT C_InvoiceLine_ID FROM C_InvoiceLine WHERE C_Invoice_ID=1)
                if (_parents[j] == AD_Table_ID)
                {
                    int AD_Table_IDchild = _parentChilds[j];
                    //Object[] param = new Object[]{new Integer(AD_Table_IDchild), new Integer(Record_ID)};
                    for (int i = 0; i < _cascades.Length; i++)
                    {
                        StringBuilder sql = new StringBuilder("DELETE FROM ")
                            .Append(_cascadeNames[i])
                            .Append(" WHERE AD_Table_ID=" + AD_Table_IDchild + " AND Record_ID IN (SELECT ")
                            .Append(_parentChildNames[j]).Append("_ID FROM ")
                            .Append(_parentChildNames[j]).Append(" WHERE ")
                            .Append(_parentNames[j]).Append("_ID=" + Record_ID + ")");
                        int no = DataBase.DB.ExecuteQuery(sql.ToString(), null, trxName);
                        if (no > 0)
                        {
                            log.Config(_cascadeNames[i] + " " + _parentNames[j] + " (" + AD_Table_ID + "/" + Record_ID + ") #" + no);
                        }
                        else if (no < 0)
                        {
                            log.Severe(_cascadeNames[i] + " " + _parentNames[j] + " (" + AD_Table_ID + "/" + Record_ID + ") #" + no);
                            retval = false;
                            break;
                            //return false;
                        }
                    }
                }
            }
            return retval;
        }


        /// <summary>
        /// Delete Cascade including (selected)parent relationships
        /// </summary>
        /// <param name="AD_Table_ID">table id</param>
        /// <param name="Record_ID">record id</param>
        /// <param name="trxName">transaction object</param>
        /// <returns>list of log</returns>
        public static System.Collections.Generic.List<String> DeletePOCascade(int AD_Table_ID, int Record_ID, Trx trx)
        {
            //	Table Loop

            System.Collections.Generic.List<String> retList = new System.Collections.Generic.List<string>();

            for (int i = 0; i < _cascades.Length; i++)
            {
                //	DELETE FROM table WHERE AD_Table_ID=#1 AND Record_ID=#2
                if (_cascades[i] != AD_Table_ID)
                {
                    StringBuilder sql = new StringBuilder("DELETE FROM ")
                        .Append(_cascadeNames[i])
                        .Append(" WHERE AD_Table_ID=" + AD_Table_ID + " AND Record_ID=" + Record_ID);
                    int no = DataBase.DB.ExecuteQuery(sql.ToString(), null, trx);
                    if (no > 0)
                    {
                        retList.Add(_cascadeNames[i] + " (" + AD_Table_ID + "/" + Record_ID + ") #" + no);
                        log.Config(_cascadeNames[i] + " (" + AD_Table_ID + "/" + Record_ID + ") #" + no);
                    }
                    else if (no < 0)
                    {
                        retList.Add(_cascadeNames[i] + " (" + AD_Table_ID + "/" + Record_ID + ") #" + no);
                        log.Severe(_cascadeNames[i] + " (" + AD_Table_ID + "/" + Record_ID + ") #" + no);
                        break;
                        //return retList;
                    }

                    if (_cascades[i] == X_AD_Attachment.Table_ID)// delete from file System
                    {
                        MAttachment.DeleteFileData(AD_Table_ID.ToString() + "_" + Record_ID.ToString());
                    }
                }
                else if(AD_Table_ID == X_AD_Attachment.Table_ID)
                {
                    MAttachment.DeleteFileData(AD_Table_ID.ToString() + "_" + Record_ID.ToString());
                }
            }
            //	Parent Loop
            for (int j = 0; j < _parents.Length; j++)
            {
                // DELETE FROM AD_Attachment WHERE AD_Table_ID=1 AND Record_ID IN 
                //	(SELECT C_InvoiceLine_ID FROM C_InvoiceLine WHERE C_Invoice_ID=1)
                if (_parents[j] == AD_Table_ID)
                {
                    int AD_Table_IDchild = _parentChilds[j];
                    //Object[] param = new Object[]{new Integer(AD_Table_IDchild), new Integer(Record_ID)};
                    for (int i = 0; i < _cascades.Length; i++)
                    {
                        StringBuilder sql = new StringBuilder("DELETE FROM ")
                            .Append(_cascadeNames[i])
                            .Append(" WHERE AD_Table_ID=" + AD_Table_IDchild + " AND Record_ID IN (SELECT ")
                            .Append(_parentChildNames[j]).Append("_ID FROM ")
                            .Append(_parentChildNames[j]).Append(" WHERE ")
                            .Append(_parentNames[j]).Append("_ID=" + Record_ID + ")");
                        int no = DataBase.DB.ExecuteQuery(sql.ToString(), null, trx);
                        if (no > 0)
                        {
                            retList.Add(_cascadeNames[i] + " " + _parentNames[j] + " (" + AD_Table_ID + "/" + Record_ID + ") #" + no);
                            log.Config(_cascadeNames[i] + " " + _parentNames[j] + " (" + AD_Table_ID + "/" + Record_ID + ") #" + no);
                        }
                        else if (no < 0)
                        {
                            retList.Add(_cascadeNames[i] + " " + _parentNames[j] + " (" + AD_Table_ID + "/" + Record_ID + ") #" + no);
                            log.Severe(_cascadeNames[i] + " " + _parentNames[j] + " (" + AD_Table_ID + "/" + Record_ID + ") #" + no);
                            break;
                           // return retList;
                        }
                    }
                }
            }
            return retList;
        }


        /// <summary>
        /// An entry Exists for restrict table/record combination
        /// </summary>
        /// <param name="AD_Table_ID">table id</param>
        /// <param name="Record_ID">record id</param>
        /// <param name="trxName">transaction</param>
        /// <returns>error message (Table Name) or null</returns>
        /// <author>Veena Pandey</author>
        public static string Exists(int AD_Table_ID, int Record_ID, Trx trx)
        {
            //	Table Loop only
            for (int i = 0; i < _restricts.Length; i++)
            {
                //	SELECT COUNT(*) FROM table WHERE AD_Table_ID=#1 AND Record_ID=#2
                StringBuilder sql = new StringBuilder("SELECT COUNT(*) FROM ")
                    .Append(_restrictNames[i])
                    .Append(" WHERE AD_Table_ID=" + AD_Table_ID + " AND Record_ID=" + Record_ID);
                int no = DataBase.DB.GetSQLValue(trx,sql.ToString());
                if (no > 0)
                    return _restrictNames[i];
            }
            return null;
        }
    }
}
