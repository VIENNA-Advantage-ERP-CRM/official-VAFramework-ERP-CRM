/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MDocType
 * Purpose        : Document Type Model
 * Class Used     : X_C_DocType
 * Chronological    Development
 * Raghunandan      7-May-2009 
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Data;
using VAdvantage.DataBase;
using System.Collections;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MDocType : X_C_DocType
    {
        #region Private Variables
        // AP CreditMemo
        public const String POSTINGCODE_APCREDITMEMO = "APC-APC";
        //AP Invoice
        public const String POSTINGCODE_APINVOICE = "API-API";
        //AP Payment
        public const String POSTINGCODE_APPAYMENT = "APP-APP";
        //AR Credit Memo
        public const String POSTINGCODE_ARCREDITMEMO = "ARC-ARC";
        //AR Invoice
        public const String POSTINGCODE_ARINVOICE = "ARI-ARI";
        //AR Invoice Indirect
        public const String POSTINGCODE_ARINVOICEINDIRECT = "ARI-ARII";
        //AR Receipt
        public const String POSTINGCODE_ARRECEIPT = "ARR-ARR";
        //Allocation
        public const String POSTINGCODE_ALLOCATION = "CMA-A";
        //Bank Statement
        public const String POSTINGCODE_BANKSTATEMENT = "CMB-CMB";
        //Cash Journal
        public const String POSTINGCODE_CASHJOURNAL = "CMC-CMC";
        //GL Journal
        public const String POSTINGCODE_GLJOURNAL = "GLJ-GLJ";
        //GL Journal Batch
        public const String POSTINGCODE_GLJOURNALBATCH = "GLJ-GLJB";
        //Physical Inventory
        public const String POSTINGCODE_PHYSICALINVENTORY = "MMI-PI";
        //Material Movement
        public const String POSTINGCODE_MATERIALMOVEMENT = "MMM-MMM";
        //Material Production
        public const String POSTINGCODE_MATERIALPRODUCTION = "MMP-MMP";
        //MM Receipt
        public const String POSTINGCODE_MMRECEIPT = "MMR-MMR";
        //MM Vendor Return
        public const String POSTINGCODE_MMVENDORRETURN = "MMR-MMVR";
        //MM Customer Return
        public const String POSTINGCODE_MMCUSTOMERRETURN = "MMS-MMCR";
        //MM Shipment
        public const String POSTINGCODE_MMSHIPMENT = "MMS-MMS";
        //MM Shipment Indirect
        public const String POSTINGCODE_MMSHIPMENTINDIRECT = "MMS-MMSI";
        //Match Invoice
        public const String POSTINGCODE_MATCHINVOICE = "MXI-MXI";
        //Match PO
        public const String POSTINGCODE_MATCHPO = "MXP-MXP";
        //Project Issue
        public const String POSTINGCODE_PROJECTISSUE = "PJI-PJI";
        //Purchase Order
        public const String POSTINGCODE_PURCHASEORDER = "POO-POO";
        //Vendor RMA
        public const String POSTINGCODE_VENDORRMA = "POO-VRMA";
        //Purchase Requisition
        public const String POSTINGCODE_PURCHASEREQUISITION = "POR-POR";
        //** New **
        public const String POSTINGCODE_SALESORDER = "SOO";
        //Binding offer
        public const String POSTINGCODE_BINDINGOFFER = "SOO-BO";
        //Credit Order
        public const String POSTINGCODE_CREDITORDER = "SOO-CO";
        //Customer RMA
        public const String POSTINGCODE_CUSTOMERRMA = "SOO-CRMA";
        //Non binding offer
        public const String POSTINGCODE_NONBINDINGOFFER = "SOO-NBO";
        //POS Order
        public const String POSTINGCODE_POSORDER = "SOO-POS";
        //Prepay Order
        public const String POSTINGCODE_PREPAYORDER = "SOO-PPO";
        //Standard Order
        public const String POSTINGCODE_STANDARDORDER = "SOO-SO";
        //Warehouse Order
        public const String POSTINGCODE_WAREHOUSEORDER = "SOO-WO";
        //Blanket Sales Order
        public const String POSTINGCODE_BLANKETSALESORDER = "BSO";
        //Blanket Purchase Order
        public const String POSTINGCODE_BLANKETPURCHASESORDER = "BPO";
        //Release Sales Order
        public const String POSTINGCODE_RELEASESALESORDER = "RSO";
        //Release Purchase Order
        public const String POSTINGCODE_RELEASEPURCHASEORDER = "RPO";


        //Posting code for future update
        /*
        PUT	Material Movement(MATERIALPUTAWAY)
        PCK	Material Movement(MATERIALPICK)
        RPL	Material Movement(MATERIALREPLENISHMENT)
        WOO	Work Order
        WOT	Work Order Transaction
        SCU	Standard Cost Update
        FAM	FIXASSET*/


        #endregion

        //	Cache					
        static private CCache<int, MDocType> s_cache = new CCache<int, MDocType>("C_DocType", 20);
        /**	Static Logger	*/
        private static VLogger s_log = VLogger.GetVLogger(typeof(MDocType).FullName);

        /// <summary>
        ///Get Client Document Type with DocBaseType
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="DocBaseType">DocBaseType base document type</param>
        /// <returns>array of doc types</returns>
        static public MDocType[] GetOfDocBaseType(Ctx ctx, string docBaseType)
        {
            List<MDocType> list = new List<MDocType>();
            String sql = "SELECT * FROM C_DocType "
                + "WHERE AD_Client_ID=" + ctx.GetAD_Client_ID() + " AND DocBaseType='" + docBaseType + "' AND IsActive='Y'"
                + "ORDER BY C_DocType_ID";
            DataSet pstmt = null;
            try
            {
                pstmt = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = pstmt.Tables[0].Rows[i];
                    list.Add(new MDocType(ctx, rs, null));
                }
                pstmt = null;
            }
            catch (Exception e)
            {

                s_log.Log(Level.SEVERE, sql, e);
            }
            MDocType[] retValue = new MDocType[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        ///Get Client Document Types
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <returns>array of doc types</returns>
        public static MDocType[] GetOfClient(Ctx ctx)
        {
            List<MDocType> list = new List<MDocType>();
            String sql = "SELECT * FROM C_DocType WHERE AD_Client_ID=" + ctx.GetAD_Client_ID();
            DataSet pstmt = null;
            try
            {
                pstmt = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = pstmt.Tables[0].Rows[i];
                    list.Add(new MDocType(ctx, rs, null));
                }
                pstmt = null;
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);
            }
            MDocType[] retValue = new MDocType[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        ///Get Document Type (cached)
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="C_DocType_ID">id</param>
        /// <returns>document type</returns>
        static public MDocType Get(Ctx ctx, int C_DocType_ID)
        {
            int key = (int)C_DocType_ID;
            MDocType retValue = (MDocType)s_cache[key];
            if (retValue == null)
            {
                retValue = new MDocType(ctx, C_DocType_ID, null);
                s_cache.Add(key, retValue);
            }
            return retValue;
        }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="C_DocType_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MDocType(Ctx ctx, int C_DocType_ID, Trx trxName)
            : base(ctx, C_DocType_ID, trxName)
        {
            if (C_DocType_ID == 0)
            {
                //	setName (null);
                //	setPrintName (null);
                //	setDocBaseType (null);
                //	setGL_Category_ID (0);
                SetDocumentCopies(0);
                SetHasCharges(false);
                SetIsDefault(false);
                SetIsDocNoControlled(false);
                SetIsSOTrx(false);
                SetIsPickQAConfirm(false);
                SetIsShipConfirm(false);
                SetIsSplitWhenDifference(false);
                SetIsReturnTrx(false);
                SetIsCreateCounter(true);
                SetIsDefaultCounterDoc(false);
                SetIsIndexed(true);
                SetIsInTransit(false);
            }
        }

        /// <summary>
        ///Load Constructor
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MDocType(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        ///New Constructor
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="DocBaseType">DocBaseType document base type</param>
        /// <param name="Name">name</param>
        /// <param name="trxName">transaction</param>
        public MDocType(Ctx ctx, string docBaseType, string name, Trx trxName)
            : this(ctx, 0, trxName)
        {
            SetAD_Org_ID(0);
            SetDocBaseType(docBaseType);
            SetName(name);
            SetPrintName(name);
            SetGL_Category_ID();
        }

        /// <summary>
        /// Set Default GL Category
        /// </summary>
        public void SetGL_Category_ID()
        {
            String sql = "SELECT * FROM GL_Category WHERE AD_Client_ID=" + GetAD_Client_ID() + "AND IsDefault='Y'";

            int GL_Category_ID = DataBase.DB.GetSQLValue(Get_TrxName(), sql);
            if (GL_Category_ID == 0)
            {
                sql = "SELECT * FROM GL_Category WHERE AD_Client_ID=@param1";
                GL_Category_ID = DataBase.DB.GetSQLValue(Get_TrxName(), sql, GetAD_Client_ID());
            }
            SetGL_Category_ID(GL_Category_ID);
        }

        /// <summary>
        /// Set SOTrx based on document base type
        /// </summary>
        public void SetIsSOTrx()
        {
            bool isSOTrx = MDocBaseType.DOCBASETYPE_SALESORDER.Equals(GetDocBaseType())
                || MDocBaseType.DOCBASETYPE_MATERIALDELIVERY.Equals(GetDocBaseType())
                || (MDocBaseType.DOCBASETYPE_BLANKETSALESORDER.Equals(GetDocBaseType()) && GetValue()=="BSO")
                || GetDocBaseType().StartsWith("AR");
            base.SetIsSOTrx(isSOTrx);
        }

        /// <summary>
        ///String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MDocType[");
            sb.Append(Get_ID()).Append("-").Append(GetName())
                .Append(",DocNoSequence_ID=").Append(GetDocNoSequence_ID())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        ///Is this a Quotation (Binding)
        /// </summary>
        /// <returns>true if Quotation</returns>
        public bool IsQuotation()
        {
            return DOCSUBTYPESO_Quotation.Equals(GetDocSubTypeSO())
                && MDocBaseType.DOCBASETYPE_SALESORDER.Equals(GetDocBaseType());
        }

        /// <summary>
        ///Is this a Proposal (Not binding)
        /// </summary>
        /// <returns>true if proposal</returns>
        public bool IsProposal()
        {
            return DOCSUBTYPESO_Proposal.Equals(GetDocSubTypeSO())
                && MDocBaseType.DOCBASETYPE_SALESORDER.Equals(GetDocBaseType());
        }

        /// <summary>
        ///Is this a Proposal or Quotation
        /// </summary>
        /// <returns>true if proposal or quotation</returns>
        public bool IsOffer()
        {
            return (DOCSUBTYPESO_Proposal.Equals(GetDocSubTypeSO())
                    || DOCSUBTYPESO_Quotation.Equals(GetDocSubTypeSO()))
                && MDocBaseType.DOCBASETYPE_SALESORDER.Equals(GetDocBaseType());
        }

        /// <summary>
        ///Get Print Name
        /// </summary>
        /// <param name="AD_Language">language</param>
        /// <returns>print Name if available translated</returns>
        public string GetPrintName(string AD_Language)
        {
            if (AD_Language == null || AD_Language.Length == 0)
                return base.GetPrintName();
            string retValue = Get_Translation("PrintName", AD_Language);
            if (retValue != null)
                return retValue;
            return base.GetPrintName();
        }

        /// <summary>
        ///Before Save
        /// </summary>
        /// <param name="newRecord">newRecord new</param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            // JID_0811: commented as per point given by Ravikant and discussed with Mukesh sir.
            //if (GetAD_Org_ID() != 0)
            //    SetAD_Org_ID(0);

            //	Sync DocBaseType && Return Trx
            //	if (newRecord || is_ValueChanged("DocBaseType"))
            SetIsSOTrx();
            return true;
        }
    }
}
