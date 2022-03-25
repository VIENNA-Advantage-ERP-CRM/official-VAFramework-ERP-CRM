/********************************************************
 * Project Name   : VIS
 * Class Name     : DocActionModel
 * Purpose        : Return Doc Actions , depends upon current doc status and tableName.
 * Chronological    Development
 * Karan            
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Process;
using VAdvantage.Utility;
using VAdvantage.WF;

namespace VIS.Models
{
    public class DocActionModel
    {
        Ctx ctx = null;
        public DocActionModel(Ctx ctx)
        {
            this.ctx = ctx;
        }

        private static String[] _moduleClasses = new String[]{
            ".Common.",
            ".Model.M",
            ".Process.M",
            ".WF.M",
            ".Report.M",
            ".Print.M",
            ".CMFG.Model.M",
            ".CMRP.Model.M",
            ".CWMS.Model.M",
            ".Model.X_",};



        public DocAtions GetActions(int AD_Table_ID, int Record_ID, string docStatus, bool processing, string orderType, bool isSOTrx, string docAction, string tableName, List<string> _values, List<string> _names)
        {
            DocAtions action = new DocAtions();
            string[] options = null;
            int index = 0;
            string defaultV = "";
            action.DocStatus = docStatus;

            VLogger.Get().Fine("DocStatus=" + docStatus
               + ", DocAction=" + docAction + ", OrderType=" + orderType
               + ", IsSOTrx=" + isSOTrx + ", Processing=" + processing
               + ", AD_Table_ID=" + AD_Table_ID + ", Record_ID=" + Record_ID);
            options = new String[_values.Count()];
            String wfStatus = MWFActivity.GetActiveInfo(ctx, AD_Table_ID, Record_ID);
            if (wfStatus != null)
            {
                VLogger.Get().SaveError("WFActiveForRecord", wfStatus);
                action.Error = "WFActiveForRecord";
                return action;
            }

            //	Status Change
            if (!CheckStatus(tableName, Record_ID, docStatus))
            {
                VLogger.Get().SaveError("DocumentStatusChanged", "");
                action.Error = "DocumentStatusChanged";
                return action;
            }
            // if (processing != null)
            {
                bool locked = "Y".Equals(processing);
                if (!locked && processing.GetType() == typeof(Boolean))
                    locked = ((Boolean)processing);
                // do not show Unlock action on Production execution
                if (locked && !(AD_Table_ID == ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.Table_ID))
                    options[index++] = DocumentEngine.ACTION_UNLOCK;
            }

            //	Approval required           ..  NA
            if (docStatus.Equals(DocumentEngine.STATUS_NOTAPPROVED))
            {
                options[index++] = DocumentEngine.ACTION_PREPARE;
                options[index++] = DocumentEngine.ACTION_VOID;
            }
            //	Draft/Invalid				..  DR/IN
            else if (docStatus.Equals(DocumentEngine.STATUS_DRAFTED)
                || docStatus.Equals(DocumentEngine.STATUS_INVALID))
            {
                options[index++] = DocumentEngine.ACTION_COMPLETE;
                //	options[index++] = DocumentEngine.ACTION_Prepare;
                options[index++] = DocumentEngine.ACTION_VOID;
            }
            //	In Process                  ..  IP
            else if (docStatus.Equals(DocumentEngine.STATUS_INPROGRESS)
                || docStatus.Equals(DocumentEngine.STATUS_APPROVED))
            {
                options[index++] = DocumentEngine.ACTION_COMPLETE;
                options[index++] = DocumentEngine.ACTION_VOID;
            }
            //	Complete                    ..  CO
            else if (docStatus.Equals(DocumentEngine.STATUS_COMPLETED))
            {
                options[index++] = DocumentEngine.ACTION_CLOSE;
            }
            //	Waiting Payment
            else if (docStatus.Equals(DocumentEngine.STATUS_WAITINGPAYMENT)
                || docStatus.Equals(DocumentEngine.STATUS_WAITINGCONFIRMATION))
            {
                options[index++] = DocumentEngine.ACTION_VOID;
                options[index++] = DocumentEngine.ACTION_PREPARE;
            }
            //	Closed, Voided, REversed    ..  CL/VO/RE
            else if (docStatus.Equals(DocumentEngine.STATUS_CLOSED)
                || docStatus.Equals(DocumentEngine.STATUS_VOIDED)
                || docStatus.Equals(DocumentEngine.STATUS_REVERSED))

                return action;

            int refIndex = index;
            bool indexFromModule = true;
            GetActionFromModuleClass(AD_Table_ID, docStatus, out index, options);

            if (index == 0)
            {
                index = refIndex;
                indexFromModule = false;
            }

            /********************
             *  Order
             */
            if (AD_Table_ID == MOrder.Table_ID)
            {
                //	Draft                       ..  DR/IP/IN
                if (docStatus.Equals(DocumentEngine.STATUS_DRAFTED)
                    || docStatus.Equals(DocumentEngine.STATUS_INPROGRESS)
                    || docStatus.Equals(DocumentEngine.STATUS_INVALID))
                {
                    options[index++] = DocumentEngine.ACTION_PREPARE;

                    //JID_0213: Close option should not be visible before doc status is completed in doc process.
                    //options[index++] = DocumentEngine.ACTION_CLOSE;

                    //	Draft Sales Order Quote/Proposal - Process
                    if (isSOTrx
                        && ("OB".Equals(orderType) || "ON".Equals(orderType)))
                        docAction = DocumentEngine.ACTION_PREPARE;
                }
                //	Complete                    ..  CO
                else if (docStatus.Equals(DocumentEngine.STATUS_COMPLETED))
                {
                    options[index++] = DocumentEngine.ACTION_VOID;
                    options[index++] = DocumentEngine.ACTION_REACTIVATE;
                }
                else if (docStatus.Equals(DocumentEngine.STATUS_WAITINGPAYMENT))
                {
                    options[index++] = DocumentEngine.ACTION_REACTIVATE;
                    options[index++] = DocumentEngine.ACTION_CLOSE;
                }
            }
            /********************
             *  Shipment
             */
            else if (AD_Table_ID == MInOut.Table_ID)
            {
                //	Complete                    ..  CO
                if (docStatus.Equals(DocumentEngine.STATUS_COMPLETED))
                {
                    // options[index++] = DocumentEngine.ACTION_VOID;
                    options[index++] = DocumentEngine.ACTION_REVERSE_CORRECT;
                }
            }
            /********************
             *  Invoice
             */
            else if (AD_Table_ID == MInvoice.Table_ID)
            {
                //	Complete                    ..  CO
                if (docStatus.Equals(DocumentEngine.STATUS_COMPLETED))
                {
                    // options[index++] = DocumentEngine.ACTION_VOID;
                    options[index++] = DocumentEngine.ACTION_REVERSE_CORRECT;
                }
            }
            /********************
             *  Payment
             */
            else if (AD_Table_ID == MPayment.Table_ID)
            {
                //	Complete                    ..  CO
                if (docStatus.Equals(DocumentEngine.STATUS_COMPLETED))
                {
                    //options[index++] = DocumentEngine.ACTION_VOID;
                    options[index++] = DocumentEngine.ACTION_REVERSE_CORRECT;
                }
            }
            /********************
             *  GL Journal
             */
            else if (AD_Table_ID == MJournal.Table_ID || AD_Table_ID == MJournalBatch.Table_ID)
            {
                //	Complete                    ..  CO
                if (docStatus.Equals(DocumentEngine.STATUS_COMPLETED))
                {
                    options[index++] = DocumentEngine.ACTION_REVERSE_CORRECT;
                    //options[index++] = DocumentEngine.ACTION_REVERSE_ACCRUAL;
                }
            }
            /********************
             *  Allocation
             */
            else if (AD_Table_ID == MAllocationHdr.Table_ID)
            {
                //	Complete                    ..  CO
                if (docStatus.Equals(DocumentEngine.STATUS_COMPLETED))
                {
                    // options[index++] = DocumentEngine.ACTION_VOID;
                    options[index++] = DocumentEngine.ACTION_REVERSE_CORRECT;
                }
            }
            /********************
             *  Bank Statement
             */
            else if (AD_Table_ID == MBankStatement.Table_ID)
            {
                //	Complete                    ..  CO
                if (docStatus.Equals(DocumentEngine.STATUS_COMPLETED))
                {
                    options[index++] = DocumentEngine.ACTION_VOID;
                }
            }
            /********************
             *  Inventory Movement, Physical Inventory
             */
            else if (AD_Table_ID == MMovement.Table_ID
                || AD_Table_ID == MInventory.Table_ID)
            {
                //	Complete                    ..  CO
                if (docStatus.Equals(DocumentEngine.STATUS_COMPLETED))
                {
                    // SI_0622 : not to show VOID and REVERSE_CORRECT action on Physical Inventory
                    bool isPhysicalInventory = false;
                    if (AD_Table_ID == MInventory.Table_ID)
                    {
                        MInventory inventory = MInventory.Get(ctx, Record_ID);
                        isPhysicalInventory = !inventory.IsInternalUse();
                    }
                    if (!isPhysicalInventory)
                    {
                        //options[index++] = DocumentEngine.ACTION_VOID;
                        options[index++] = DocumentEngine.ACTION_REVERSE_CORRECT;
                    }
                }
            }
            // Team Forecast
            if (AD_Table_ID == MForecast.Table_ID)
            {
                if (docStatus.Equals(DocumentEngine.STATUS_COMPLETED))
                {
                    options[index++] = DocumentEngine.ACTION_VOID;
                    options[index++] = DocumentEngine.ACTION_REACTIVATE;
                }
            }
            // Master Forecast
            if (AD_Table_ID == X_C_MasterForecast.Table_ID)
            {
                if (docStatus.Equals(DocumentEngine.STATUS_COMPLETED))
                {
                    options[index++] = DocumentEngine.ACTION_VOID;
                    options[index++] = DocumentEngine.ACTION_REACTIVATE;
                }
            }
            // Provisional Invoice
            if (AD_Table_ID == MProvisionalInvoice.Table_ID)
            {
                //	Complete                    ..  CO
                if (docStatus.Equals(DocumentEngine.STATUS_COMPLETED))
                {
                    options[index++] = DocumentEngine.ACTION_REVERSE_CORRECT;
                }
            }

            // Added By Arpit
            else if (AD_Table_ID == MMovementConfirm.Table_ID)
            {
                //	Complete                    ..  CO
                if (docStatus.Equals(DocumentEngine.STATUS_COMPLETED))
                {
                    // SI_0630 : System should not allow to void the Move confirmation after its completion
                    //options[index++] = DocumentEngine.ACTION_VOID;
                }
            }
            //End

            /********************
             *  Requisition
             */
            else if (AD_Table_ID == X_M_Requisition.Table_ID)
            {
                //	Complete                    ..  CO
                if (docStatus.Equals(DocumentEngine.STATUS_COMPLETED))
                {
                    options[index++] = DocumentEngine.ACTION_VOID;
                    options[index++] = DocumentEngine.ACTION_REACTIVATE;
                }
            }

            //    /********************
            //*  Warehouse Task  New Add by raghu 11 april,2011
            //*/
            //    else if (AD_Table_ID == X_M_WarehouseTask.Table_ID
            //        || AD_Table_ID == X_M_TaskList.Table_ID)
            //    {
            //        //	Draft                       ..  DR/IP/IN
            //        if (docStatus.Equals(DocActionVariables.STATUS_DRAFTED)
            //            || docStatus.Equals(DocActionVariables.STATUS_INPROGRESS)
            //            || docStatus.Equals(DocActionVariables.STATUS_INVALID))
            //        {
            //            options[index++] = DocActionVariables.ACTION_PREPARE;
            //        }
            //        //	Complete                    ..  CO
            //        else if (docStatus.Equals(DocActionVariables.STATUS_COMPLETED))
            //        {
            //            options[index++] = DocActionVariables.ACTION_VOID;
            //            options[index++] = DocActionVariables.ACTION_REVERSE_CORRECT;
            //        }
            //    }
            /********************
         *  Work Order New Add by raghu 11 april,2011
         */
            else if (AD_Table_ID == ViennaAdvantage.Model.X_VAMFG_M_WorkOrder.Table_ID)
            {
                //	Draft                       ..  DR/IP/IN
                if (docStatus.Equals(DocActionVariables.STATUS_DRAFTED)
                    || docStatus.Equals(DocActionVariables.STATUS_INPROGRESS)
                    || docStatus.Equals(DocActionVariables.STATUS_INVALID))
                {
                    options[index++] = DocActionVariables.ACTION_PREPARE;
                }
                //	Complete                    ..  CO
                else if (docStatus.Equals(DocActionVariables.STATUS_COMPLETED))
                {
                    //options[index++] = DocActionVariables.ACTION_VOID;
                    options[index++] = DocActionVariables.ACTION_REACTIVATE;
                }
            }
            /********************
             *  Work Order Transaction New Add by raghu 11 april,2011
             */
            else if (AD_Table_ID == ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.Table_ID)
            {
                //	Draft                       ..  DR/IP/IN
                if (docStatus.Equals(DocActionVariables.STATUS_DRAFTED)
                    || docStatus.Equals(DocActionVariables.STATUS_INPROGRESS)
                    || docStatus.Equals(DocActionVariables.STATUS_INPROGRESS)
                    || docStatus.Equals(DocActionVariables.STATUS_INVALID))
                {
                    options[index++] = DocActionVariables.ACTION_PREPARE;
                }
                //	Complete                    ..  CO
                else if (docStatus.Equals(DocActionVariables.STATUS_COMPLETED))
                {
                    //options[index++] = DocActionVariables.ACTION_VOID;
                    options[index++] = DocActionVariables.ACTION_REVERSE_CORRECT;
                }

            }

            /* Obsolete Inventory New add by Amit 24-May-2016 */
            //else if (tableName == "VA024_ObsoleteInventory")
            //{
            //    // DR/IN
            //    if (docStatus.Equals(DocumentEngine.STATUS_DRAFTED)
            //        || docStatus.Equals(DocumentEngine.STATUS_INVALID))
            //    {
            //        options[index++] = DocumentEngine.ACTION_COMPLETE;
            //        options[index++] = DocumentEngine.ACTION_VOID;
            //    }
            //    //	Complete                   
            //    else if (docStatus.Equals(DocActionVariables.STATUS_COMPLETED))
            //    {
            //        options[index++] = DocActionVariables.ACTION_VOID;
            //        options[index++] = DocActionVariables.ACTION_REVERSE_CORRECT;
            //    }
            //}

            /***For Primary thread***/
            ///**
            // *	Fill actionCombo
            // */
            //for (int i = 0; i < index; i++)
            //{
            //    //	Serach for option and add it
            //    bool added = false;
            //    for (int j = 0; j < _values.Length && !added; j++)
            //        if (options[i].Equals(_values[j]))
            //        {
            //            //actionCombo.addItem(_names[j]);
            //            vcmbAction.Items.Add(_names[j]);
            //            added = true;
            //        }
            //}

            //	setDefault
            if (docAction.Equals("--"))		//	If None, suggest closing
                docAction = DocumentEngine.ACTION_CLOSE;

            // check applied to display Document Actions on Transactions based on the document type
            // and the setting "Check Document Action Access" on Role window
            int C_DocType_ID = 0;
            int C_DocTypeTarget_ID = 0;
            MTable table = MTable.Get(ctx, AD_Table_ID);
            PO po = table.GetPO(ctx, Record_ID, null);
            if (Util.GetValueOfInt(po.Get_Value("C_DocType_ID")) > 0)
            {
                C_DocType_ID = Util.GetValueOfInt(po.Get_Value("C_DocType_ID"));
            }
            if (Util.GetValueOfInt(po.Get_Value("C_DocTypeTarget_ID")) > 0)
            {
                C_DocTypeTarget_ID = Util.GetValueOfInt(po.Get_Value("C_DocTypeTarget_ID"));
                C_DocType_ID = C_DocTypeTarget_ID;
            }

            if (C_DocType_ID > 0)
            {
                String[] docActionHolder = new String[] { docAction };
                if (po is DocOptions)
                    index = ((DocOptions)po).customizeValidActions(docStatus, processing, orderType, isSOTrx ? "Y" : "N",
                            AD_Table_ID, docActionHolder, options, index);

                options = DocumentEngine.checkActionAccess(ctx, ctx.GetAD_Client_ID(), ctx.GetAD_Role_ID(), C_DocType_ID, options, ref index);
            }

            for (int i = 0; i < _values.Count() && defaultV.Equals(""); i++)
                if (docAction.Equals(_values[i]))
                    defaultV = _names[i];


            action.Options = options.ToList();
            if (indexFromModule)
            {
                action.Index = index + 1;
            }
            else
            {
                action.Index = index;
            }
            action.DefaultV = defaultV;

            return action;

            /***For Primary thread***/
            //if (!defaultV.Equals(""))
            //{
            //    //vcmbAction.SelectedValue = defaultV;
            //    vcmbAction.SelectedItem = defaultV;
            //}


        }


        /// <summary>
        /// Check Status Change
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="Record_ID">record id</param>
        /// <param name="docStatus">current doc status</param>
        /// <returns>true if status not changed</returns>
        private bool CheckStatus(String tableName, int Record_ID, String docStatus)
        {
            String sql = "SELECT COUNT(*) FROM " + tableName
                + " WHERE " + tableName + "_ID=" + Record_ID
                + " AND DocStatus='" + docStatus + "'";
            int result = Util.GetValueOfInt(DB.ExecuteScalar(sql));
            return result > 0;
        }


        public void GetActionFromModuleClass(int AD_Table_ID, string docStatus, out int index, string[] options)
        {
            /*********** Module Section  **************/


            #region GetActionFromModuleClass

            MTable mTable = new MTable(ctx, AD_Table_ID, null);
            //	Strip table name prefix (e.g. AD_) Customizations are 3/4
            String classNm = "DocActionSpecification";


            index = 0;
            Tuple<String, String, String> moduleInfo;
            Assembly asm = null;
            string namspace = "";
            if (Env.HasModulePrefix(mTable.GetTableName(), out moduleInfo))
            {
                asm = null;
                try
                {
                    asm = Assembly.Load(moduleInfo.Item1);
                }
                catch (Exception e)
                {
                    VLogger.Get().Info(e.Message);
                    asm = null;
                }

                if (asm != null)
                {
                    for (int i = 0; i < _moduleClasses.Length; i++)
                    {
                        namspace = moduleInfo.Item2 + _moduleClasses[i] + classNm;
                        if (_moduleClasses.Contains("X_"))
                        {
                            namspace = moduleInfo.Item2 + _moduleClasses[i] + mTable.GetTableName();
                        }

                        Type clazzsn = GetClassFromAsembly(asm, namspace);
                        if (clazzsn != null)
                        {
                            ConstructorInfo constructor = null;
                            try
                            {
                                constructor = clazzsn.GetConstructor(new Type[] { });
                            }
                            catch (Exception e)
                            {
                                VLogger.Get().Warning("No transaction Constructor for " + clazzsn.FullName + " (" + e.ToString() + ")");
                            }
                            if (constructor != null)
                            {
                                object o = constructor.Invoke(null);
                                MethodInfo mi = clazzsn.GetMethod("GetDocAtion");
                                if (mi != null)
                                {
                                    object[] input = new object[2];
                                    input[0] = AD_Table_ID;
                                    input[1] = docStatus;

                                    object res = mi.Invoke(o, input);
                                    string[] opt = res as string[];
                                    if (opt.Length > 0)
                                    {
                                        index = 0;
                                    }
                                    for (int j = 0; j < opt.Length; j++)
                                    {
                                        options[index++] = opt[j];
                                    }

                                }

                            }
                            //if (o is ModuleDocAction)
                            //{
                            //    string[] opt = ((ModuleDocAction)o).GetDocAtion(AD_Table_ID, docStatus);// .Invoke(new object[] { AD_Table_ID, docStatus });
                            //    if (opt.Length > 0)
                            //    {
                            //        index = 0;
                            //    }
                            //    for (int j = 0; j < opt.Length; j++)
                            //    {
                            //        options[index++] = opt[j];

                            //    }

                            //    break;
                            //}
                            //}
                            //return clazzsn;
                        }

                    }
                }
            }

            #endregion
            /*********** END  **************/
        }


        /// <summary>
        /// Get Class From Assembly
        /// </summary>
        /// <param name="asm">Assembly</param>
        /// <param name="className">Fully Qulified Class Name</param>
        /// <returns>Class Object</returns>
        private Type GetClassFromAsembly(Assembly asm, string className)
        {
            Type type = null;
            try
            {
                type = asm.GetType(className);
            }
            catch (Exception e)
            {
                VLogger.Get().Log(Level.SEVERE, e.Message);
            }
            //return type;
            if (type == null)
            {
                return null;
            }

            Type baseClass = type.BaseType;

            while (baseClass != null)
            {
                if (baseClass == typeof(ModuleDocAction) || baseClass == typeof(object))
                {
                    return type;
                }
                baseClass = baseClass.BaseType;
            }
            return null;
        }

        // Added by Bharat on 06 June 2017
        public List<Dictionary<string, object>> GetReference(string qry)
        {
            List<Dictionary<string, object>> retDic = null;
            DataSet ds = DB.ExecuteDataset(qry);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["Value"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    obj["Description"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Description"]);
                    retDic.Add(obj);
                }
            }
            return retDic;
        }

    }

    public interface ModuleDocAction
    {
        String[] GetDocAtion(int AD_Table_ID, String docStatus);

    }

    public class DocAtions
    {
        public List<string> Options { get; set; }
        public int Index { get; set; }
        public string DefaultV { get; set; }
        public string DocStatus { get; set; }
        public string Error { get; set; }
    }

}