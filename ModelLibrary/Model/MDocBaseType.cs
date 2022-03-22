/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MDocBaseType
 * Purpose        : Document Base Type Model
 * Class Used     : X_C_DocBaseType
 * Chronological    Development
 * Raghunandan      7-May-2009 
  ******************************************************/
//Constructor<> function is uncompleet
/******************************************************/
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Windows.Forms;
using System.Reflection;

namespace VAdvantage.Model
{

    public class MDocBaseType : X_C_DocBaseType
    {
        #region Private variables
        // AP Credit Memo = APC 
        public const String DOCBASETYPE_APCREDITMEMO = "APC";
        //AP Invoice = API */
        public const String DOCBASETYPE_APINVOICE = "API";
        // AP Payment = APP */
        public const String DOCBASETYPE_APPAYMENT = "APP";
        // AR Credit Memo = ARC */
        public const String DOCBASETYPE_ARCREDITMEMO = "ARC";
        // AR Pro Forma Invoice = ARF */
        public const String DOCBASETYPE_ARPROFORMAINVOICE = "ARF";
        // AR Invoice = ARI */
        public const String DOCBASETYPE_ARINVOICE = "ARI";
        // AR Receipt = ARR */
        public const String DOCBASETYPE_ARRECEIPT = "ARR";
        // Payment Allocation = CMA */
        public const String DOCBASETYPE_PAYMENTALLOCATION = "CMA";
        // Bank Statement = CMB */
        public const String DOCBASETYPE_BANKSTATEMENT = "CMB";
        // Cash Journal = CMC */
        public const String DOCBASETYPE_CASHJOURNAL = "CMC";
        // GL Document = GLD */
        public const String DOCBASETYPE_GLDOCUMENT = "GLD";
        // GL Journal = GLJ */
        public const String DOCBASETYPE_GLJOURNAL = "GLJ";
        // Material Physical Inventory = MMI */
        public const String DOCBASETYPE_MATERIALPHYSICALINVENTORY = "MMI";
        // Material Movement = MMM */
        public const String DOCBASETYPE_MATERIALMOVEMENT = "MMM";
        // Material Production = MMP */
        public const String DOCBASETYPE_MATERIALPRODUCTION = "MMP";
        // Material Receipt = MMR */
        public const String DOCBASETYPE_MATERIALRECEIPT = "MMR";
        // Material Delivery = MMS */
        public const String DOCBASETYPE_MATERIALDELIVERY = "MMS";
        // Match Invoice = MXI */
        public const String DOCBASETYPE_MATCHINVOICE = "MXI";
        // Match PO = MXP */
        public const String DOCBASETYPE_MATCHPO = "MXP";
        // Project Issue = PJI */
        public const String DOCBASETYPE_PROJECTISSUE = "PJI";
        // Purchase Order = POO */
        public const String DOCBASETYPE_PURCHASEORDER = "POO";
        // Purchase Requisition = POR */
        public const String DOCBASETYPE_PURCHASEREQUISITION = "POR";
        // Sales Order = SOO */
        public const String DOCBASETYPE_SALESORDER = "SOO";
        //Profit And Loss = PLA
        public const String DOCBASETYPE_PROFITLOSS = "PLA";
        //Income Tax = ITA
        public const String DOCBASETYPE_INCOMETAX = "ITA";
        //Lette of Credit = LOC
        public const String DOCBASETYPE_LETTEROFCREDIT = "LOC";
        //Inventory Provision = OIV
        public const String DOCBASETYPE_INVENTORYPOVISION = "OIV";
        //PDC Receivable = PDR
        public const String DOCBASETYPE_PDCRECEIVABLE = "PDR";
        //PDC Payable = PDP
        public const String DOCBASETYPE_PDCPAYABLE = "PDP";
        // Elimination Journal = ELJ
        public const String DOCBASETYPE_ELIMINATIONJOURNAL = "ELJ";
        //	Logger						
        private static VLogger _log = VLogger.GetVLogger(typeof(MDocBaseType).FullName);
        //Document Base Types			
        private static MDocBaseType[] s_docBaseTypes = null;


        /** Material Movement = PUT */
        public const String DOCBASETYPE_MATERIALPUTAWAY = "PUT";
        /** Material Movement = PCK */
        public const String DOCBASETYPE_MATERIALPICK = "PCK";
        /** Material Movement = RPL */
        public const String DOCBASETYPE_MATERIALREPLENISHMENT = "RPL";
        /** Work Order = WOO */
        public const String DOCBASETYPE_WORKORDER = "WOO";
        /** Work Order Transaction = WOT */
        public const String DOCBASETYPE_WORKORDERTRANSACTION = "WOT";
        /** Standard Cost Update = SCU **/
        public const String DOCBASETYPE_STANDARDCOSTUPDATE = "SCU";
        /** Standard Cost Update = FAM **/
        public const String DOCBASETYPE_FIXASSET = "FAM";

        /** Fixed Asset Impairment/Enhancement = FAI **/
        public const String DOCBASETYPE_FIXASSETIMPAIRMENT = "FAI";


        // Blanket Sales Order = SOO */
        public const String DOCBASETYPE_BLANKETSALESORDER = "BOO";  // Update on 17 July, 2017 by Sukhwinder.

        //Lakhwinder 29Jan2021
        public const String DOCBASETYPE_MoveConfirmation = "MMC";
        public const String DOCBASETYPE_ShipReceiptConfirmation = "SRC";

        #endregion

        /// <summary>
        ///Get all base types
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>array of base doc types</returns>
        public static MDocBaseType[] GetAll(Context ctx)
        {
            if (s_docBaseTypes != null)
                return s_docBaseTypes;
            //
            List<MDocBaseType> list = new List<MDocBaseType>();
            String sql = "SELECT * FROM C_DocbaseType WHERE IsActive='Y'";
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MDocBaseType(ctx, dr, null));
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            s_docBaseTypes = new MDocBaseType[list.Count];
            s_docBaseTypes = list.ToArray();
            return s_docBaseTypes;
        }


        public static MDocBaseType[] GetAll(Ctx ctx)
        {
            if (s_docBaseTypes != null)
                return s_docBaseTypes;
            //
            List<MDocBaseType> list = new List<MDocBaseType>();
            String sql = "SELECT * FROM C_DocbaseType WHERE IsActive='Y'";
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MDocBaseType(ctx, dr, null));
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            s_docBaseTypes = new MDocBaseType[list.Count];
            s_docBaseTypes = list.ToArray();
            return s_docBaseTypes;
        }


        /// <summary>
        ///Get Base Type for Table
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Table_ID">table</param>
        /// <returns>base type or null</returns>
        public static MDocBaseType GetForTable(Ctx ctx, int AD_Table_ID)
        {
            if (s_docBaseTypes == null)
                GetAll(ctx);
            for (int i = 0; i < s_docBaseTypes.Length; i++)
            {
                MDocBaseType dbt = s_docBaseTypes[i];
                if (dbt.GetAD_Table_ID() == AD_Table_ID)
                    return dbt;
            }
            return null;
        }

        /// <summary>
        ///Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_DocBaseType_ID">id</param>
        /// <param name="trxName">trx</param>
        public MDocBaseType(Ctx ctx, int C_DocBaseType_ID, Trx trxName)
            : base(ctx, C_DocBaseType_ID, trxName)
        {

        }

        /// <summary>
        ///Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">ctx</param>
        public MDocBaseType(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        ///Set Document Base Type
        /// </summary>
        /// <param name="DocBaseType">type</param>
        public new void SetDocBaseType(String docBaseType)
        {
            if (docBaseType != null)
                base.SetDocBaseType(docBaseType.ToUpper());
        }

        /// <summary>
        ///Check Document Base Type
        /// </summary>
        /// <returns>true if ok</returns>
        public bool CheckDocBaseType()
        {
            string s = GetDocBaseType();
            if (s == null || s.Length != 3)
                return false;
            if (!s.Equals(s.ToUpper()))
                SetDocBaseType(s.ToUpper());
            return true;
        }

        /// <summary>
        ///Get Table ID.
        /// </summary>
        /// <returns>table ID</returns>
        public new int GetAD_Table_ID()
        {
            //	Multiple mappings of table->type in FactAcctReset
            int AD_Table_ID = base.GetAD_Table_ID();
            if (AD_Table_ID > 0)
                return AD_Table_ID;
            //
            String dbt = GetDocBaseType();
            if (dbt.Equals(DOCBASETYPE_SALESORDER) || dbt.Equals(DOCBASETYPE_PURCHASEORDER))
                return X_C_Order.Table_ID;
            if (dbt.Equals(DOCBASETYPE_MATERIALRECEIPT) || dbt.Equals(DOCBASETYPE_MATERIALDELIVERY))
                return X_M_InOut.Table_ID;
            if (dbt.Equals(DOCBASETYPE_APINVOICE) || dbt.Equals(DOCBASETYPE_APCREDITMEMO)
                    || dbt.Equals(DOCBASETYPE_ARINVOICE) || dbt.Equals(DOCBASETYPE_ARCREDITMEMO)
                    || dbt.Equals(DOCBASETYPE_ARPROFORMAINVOICE))
                return X_C_Invoice.Table_ID;
            if (dbt.Equals(DOCBASETYPE_APPAYMENT) || dbt.Equals(DOCBASETYPE_ARRECEIPT))
                return X_C_Payment.Table_ID;
            if (dbt.Equals(DOCBASETYPE_PAYMENTALLOCATION))
                return X_C_AllocationHdr.Table_ID;
            if (dbt.Equals(DOCBASETYPE_CASHJOURNAL))
                return X_C_Cash.Table_ID;
            if (dbt.Equals(DOCBASETYPE_BANKSTATEMENT))
                return X_C_BankStatement.Table_ID;
            if (dbt.Equals(DOCBASETYPE_MATERIALPHYSICALINVENTORY))
                return X_M_Inventory.Table_ID;
            if (dbt.Equals(DOCBASETYPE_MATERIALMOVEMENT))
                return X_M_Movement.Table_ID;
            if (dbt.Equals(DOCBASETYPE_MATERIALPRODUCTION))
                return X_M_Production.Table_ID;
            if (dbt.Equals(DOCBASETYPE_GLJOURNAL))
                return X_GL_Journal.Table_ID;
            if (dbt.Equals(DOCBASETYPE_GLDOCUMENT))
                return 0;
            if (dbt.Equals(DOCBASETYPE_MATCHINVOICE))
                return X_M_MatchInv.Table_ID;
            if (dbt.Equals(DOCBASETYPE_MATCHPO))
                return X_M_MatchPO.Table_ID;
            if (dbt.Equals(DOCBASETYPE_PROJECTISSUE))
                return X_C_ProjectIssue.Table_ID;
            if (dbt.Equals(DOCBASETYPE_PURCHASEREQUISITION))
                return X_M_Requisition.Table_ID;
            /*********Manafacturing*************/
            if (dbt.Equals(DOCBASETYPE_MATERIALPICK) || dbt.Equals(DOCBASETYPE_MATERIALPUTAWAY)
                || dbt.Equals(DOCBASETYPE_MATERIALREPLENISHMENT))
                return X_M_WarehouseTask.Table_ID;
            if (dbt.Equals(DOCBASETYPE_WORKORDER))
                return X_M_WorkOrder.Table_ID;
            if (dbt.Equals(DOCBASETYPE_WORKORDERTRANSACTION))
                return X_M_WorkOrderTransaction.Table_ID;
            if (dbt.Equals(DOCBASETYPE_STANDARDCOSTUPDATE))
                return X_M_CostUpdate.Table_ID;
            /*********Manafacturing*************/
            //	Error
            log.Warning("No AD_Table_ID for " + GetName() + " (DocBaseType=" + dbt + ")");
            return 0;
        }

        /// <summary>
        ///Get Table Name
        /// </summary>
        /// <returns>table name</returns>
        public String GetTableName()
        {
            int AD_Table_ID = GetAD_Table_ID();
            if (AD_Table_ID == 0)
                return null;
            //
            if (AD_Table_ID == X_C_Order.Table_ID)
                return X_C_Order.Table_Name;
            if (AD_Table_ID == X_M_InOut.Table_ID)
                return X_M_InOut.Table_Name;
            if (AD_Table_ID == X_C_Invoice.Table_ID)
                return X_C_Invoice.Table_Name;
            if (AD_Table_ID == X_C_Payment.Table_ID)
                return X_C_Payment.Table_Name;
            if (AD_Table_ID == X_C_AllocationHdr.Table_ID)
                return X_C_AllocationHdr.Table_Name;
            if (AD_Table_ID == X_C_Cash.Table_ID)
                return X_C_Cash.Table_Name;
            if (AD_Table_ID == X_C_BankStatement.Table_ID)
                return X_C_BankStatement.Table_Name;
            if (AD_Table_ID == X_M_Inventory.Table_ID)
                return X_M_Inventory.Table_Name;
            if (AD_Table_ID == X_M_Movement.Table_ID)
                return X_M_Movement.Table_Name;
            if (AD_Table_ID == X_GL_Journal.Table_ID)
                return X_GL_Journal.Table_Name;
            if (AD_Table_ID == X_M_MatchInv.Table_ID)
                return X_M_MatchInv.Table_Name;
            if (AD_Table_ID == X_M_MatchPO.Table_ID)
                return X_M_MatchPO.Table_Name;
            if (AD_Table_ID == X_C_ProjectIssue.Table_ID)
                return X_C_ProjectIssue.Table_Name;
            if (AD_Table_ID == X_M_Requisition.Table_ID)
                return X_M_Requisition.Table_Name;

            /*********Manafacturing*************/
            if (AD_Table_ID == X_M_WarehouseTask.Table_ID)
                return X_M_WarehouseTask.Table_Name;
            if (AD_Table_ID == X_M_WorkOrder.Table_ID)
                return X_M_WorkOrder.Table_Name;
            if (AD_Table_ID == X_M_WorkOrderTransaction.Table_ID)
                return X_M_WorkOrderTransaction.Table_Name;
            if (AD_Table_ID == X_M_CostUpdate.Table_ID)
                return X_M_CostUpdate.Table_Name;
            /*********Manafacturing*************/
            //
            return MTable.GetTableName(GetCtx(), AD_Table_ID);
        }

        /// <summary>
        ///	Get Class Name
        /// </summary>
        /// <returns>class name</returns>
        public new String GetAccountingClassname()
        {
            String className = base.GetAccountingClassname();
            if (className != null && className.Length > 0)
            {
                /**************Manfacturing*******************/
                //Jun,06,2011/
              
                /**************Manfacturing*******************/
                //Jun,06,2011/
                return className;
            }
            String dbt = GetDocBaseType();
            if (dbt.Equals(DOCBASETYPE_SALESORDER) || dbt.Equals(DOCBASETYPE_PURCHASEORDER))
            {
                
                return "VAdvantage.Acct.Doc_Order";
            }
            if (dbt.Equals(DOCBASETYPE_MATERIALRECEIPT) || dbt.Equals(DOCBASETYPE_MATERIALDELIVERY))
            {
                
                return "VAdvantage.Acct.Doc_InOut";
            }
            if (dbt.Equals(DOCBASETYPE_APINVOICE) || dbt.Equals(DOCBASETYPE_APCREDITMEMO)
                || dbt.Equals(DOCBASETYPE_ARINVOICE) || dbt.Equals(DOCBASETYPE_ARCREDITMEMO)
                || dbt.Equals(DOCBASETYPE_ARPROFORMAINVOICE))
            {
              
                return "VAdvantage.Acct.Doc_Invoice";
            }
            if (dbt.Equals(DOCBASETYPE_APPAYMENT) || dbt.Equals(DOCBASETYPE_ARRECEIPT))
            {
                
                return "VAdvantage.Acct.Doc_Payment";
            }
            if (dbt.Equals(DOCBASETYPE_PAYMENTALLOCATION))
            {
                
                return "VAdvantage.Acct.Doc_Allocation";
            }
            if (dbt.Equals(DOCBASETYPE_CASHJOURNAL))
            {
                
                return "VAdvantage.Acct.Doc_Cash";
            }
            if (dbt.Equals(DOCBASETYPE_BANKSTATEMENT))
            {
                
                return "VAdvantage.Acct.Doc_Bank";
            }
            if (dbt.Equals(DOCBASETYPE_MATERIALPHYSICALINVENTORY))
            {
               
                return "VAdvantage.Acct.Doc_Inventory";
            }
            if (dbt.Equals(DOCBASETYPE_MATERIALMOVEMENT))
            {
               
                return "VAdvantage.Acct.Doc_Movement";
            }
            if (dbt.Equals(DOCBASETYPE_MATERIALPRODUCTION))
            {
                
                return "VAdvantage.Acct.Doc_Production";
            }
            if (dbt.Equals(DOCBASETYPE_GLJOURNAL))
            {
               
                return "VAdvantage.Acct.Doc_GLJournal";
            }
            if (dbt.Equals(DOCBASETYPE_GLDOCUMENT))
            {
                return null;
            }
            if (dbt.Equals(DOCBASETYPE_MATCHINVOICE))
            {
               
                return "VAdvantage.Acct.Doc_MatchInv";
            }
            if (dbt.Equals(DOCBASETYPE_MATCHPO))
            {
               
                return "VAdvantage.Acct.Doc_MatchPO";
            }
            if (dbt.Equals(DOCBASETYPE_PROJECTISSUE))
            {
               
                return "VAdvantage.Acct.Doc_ProjectIssue";
            }
            if (dbt.Equals(DOCBASETYPE_PURCHASEREQUISITION))
            {
                
                return "VAdvantage.Acct.Doc_Requisition";
            }
            /*********Manafacturing*************/
            if (dbt.Equals(DOCBASETYPE_MATERIALPICK) || dbt.Equals(DOCBASETYPE_MATERIALPUTAWAY)
                || dbt.Equals(DOCBASETYPE_MATERIALREPLENISHMENT))
            {
                return "ViennaAdvantage.CWMS.Acct.Doc_WarehouseTask";
            }
            if (dbt.Equals(DOCBASETYPE_WORKORDER))
            {
                return "ViennaAdvantage.CMFG.Acct.VAMFG_Doc_WorkOrder";
            }
            if (dbt.Equals(DOCBASETYPE_WORKORDERTRANSACTION))
            {
                return "ViennaAdvantage.CMFG.Acct.VAMFG_Doc_WorkOrderTransaction";
            }
            if (dbt.Equals(DOCBASETYPE_STANDARDCOSTUPDATE))
            {
                return "VAdvantage.Acct.Doc_CostUpdate";
            }
            /*********Manafacturing*************/
            //	Error
            return null;
        }

        /// <summary>
        ///Get Class
        /// </summary>
        /// <returns>class or null</returns>
        //protected Class<?> GetAccountingClass()
        protected Type GetAccountingClass()
        {
            String className = GetAccountingClassname();
            string name = GetName();
            if (className == null || className.Length == 0)
            {
                log.Warning("No ClassName defined");
                return null;
            }
            try
            {
                Type clazz = null;
                string acctName = className.Substring(className.LastIndexOf('.') + 1);

                clazz = Utility.ModuleTypeConatiner.GetClassType(className, acctName);
                /*********Manafacturing*************/
                return clazz;
            }
            catch (Exception e)
            {
                log.Warning("Error creating class for " + GetName() + ": - " + e.ToString());
            }
            return null;
        }

        /// <summary>
        ///Get Accounting Class
        /// </summary>
        /// <param name="ass">accounting schema array</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">trx</param>
        /// <returns>instance or null</returns>
        public AccountingInterface GetAccountingInstance(MAcctSchema[] ass, DataRow dr, Trx trxName)
        {
            //Class<?> clazz = getAccountingClass();
            Type clazz = GetAccountingClass();
            if (clazz == null)
            {
                return null;
            }
            try
            {
                //Constructor<?> constr = clazz.getConstructor(MAcctSchema[].class, ResultSet.class, String.class);
                System.Reflection.ConstructorInfo constr = clazz.GetConstructor(new Type[] { typeof(MAcctSchema[]), typeof(DataRow), typeof(Trx) });
                log.Info(constr.Name + ": Constructor check ");
                //AccountingInterface retValue = (AccountingInterface)constr.newInstance(ass, rs, trxName);
                AccountingInterface retValue = (AccountingInterface)constr.Invoke(new object[] { ass, dr, trxName });
                log.Info(retValue.ToString() + ": Constructor invoke check ");
                return retValue;
            }
            catch (Exception e)
            {
                log.Warning("Error instanciating " + GetName() + ": - " + e.ToString());
            }
            return null;
        }

        /// <summary>
        ///Before Save
        /// </summary>
        /// <param name="newRecord">newRecord new</param>
        /// <returns>true if can be saved</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetAD_Table_ID() == 0)
            {
                log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "AD_Table_ID"));
                return false;
            }
            String s = GetAccountingClassname();
            if (s == null || s.Length == 0)
            {
                log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "AccountingClassname"));
                return false;
            }
            if (!CheckDocBaseType())
                return false;
            return true;
        }

        /// <summary>
        ///String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MDocBaseType[")
                .Append(GetName())
                .Append(",AD_Table_ID=").Append(GetAD_Table_ID())
                .Append(", Class=").Append(GetAccountingClassname())
                .Append("]");
            return sb.ToString();
        }
    }
}



