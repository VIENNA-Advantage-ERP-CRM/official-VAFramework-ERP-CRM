/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAccount
 * Purpose        : Account Object Entity to maintain all segment values.
 * Class Used     : X_C_ValidCombination
 * Chronological    Development
 * Raghunandan     06-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
namespace VAdvantage.Model
{
    public class MAccount : X_C_ValidCombination
    {
        /**	Logger						*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MAccount).FullName);
        /**	Account Segment				*/
        private MElementValue _accountEV = null;

        /// <summary>
        /// Get existing Account or create it 
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Client_ID">client id</param>
        /// <param name="AD_Org_ID"> organisation id</param>
        /// <param name="C_AcctSchema_ID"> account schema id</param>
        /// <param name="Account_ID">account id</param>
        /// <param name="C_SubAcct_ID">sub-Account id</param>
        /// <param name="M_Product_ID">product id</param>
        /// <param name="C_BPartner_ID">Bussness partner id</param>
        /// <param name="AD_OrgTrx_ID"> ordTax id</param>
        /// <param name="C_LocFrom_ID">C_LocFrom_ID</param>
        /// <param name="C_LocTo_ID"></param>
        /// <param name="C_SalesRegion_ID"></param>
        /// <param name="C_Project_ID"></param>
        /// <param name="C_Campaign_ID"></param>
        /// <param name="C_Activity_ID"></param>
        /// <param name="User1_ID"></param>
        /// <param name="User2_ID"></param>
        /// <param name="UserElement1_ID"></param>
        /// <param name="UserElement2_ID"></param>
        /// <returns>account or null</returns>
        public static MAccount Get(Ctx ctx, int AD_Client_ID, int AD_Org_ID, int C_AcctSchema_ID,
            int Account_ID, int C_SubAcct_ID, int M_Product_ID, int C_BPartner_ID, int AD_OrgTrx_ID,
            int C_LocFrom_ID, int C_LocTo_ID, int C_SalesRegion_ID, int C_Project_ID, int C_Campaign_ID,
            int C_Activity_ID, int User1_ID, int User2_ID, int UserElement1_ID, int UserElement2_ID)
        {
            MAccount existingAccount = null;
            //
            StringBuilder info = new StringBuilder();
            StringBuilder sql = new StringBuilder("SELECT * FROM C_ValidCombination "
                //	Mandatory fields
                + "WHERE AD_Client_ID=" + AD_Client_ID		//	#1
                + " AND AD_Org_ID=" + AD_Org_ID
                + " AND C_AcctSchema_ID=" + C_AcctSchema_ID
                + " AND Account_ID=" + Account_ID);		//	#4
            //	Optional fields
            if (C_SubAcct_ID == 0)
            {
                sql.Append(" AND C_SubAcct_ID IS NULL");
            }
            else
            {
                sql.Append(" AND C_SubAcct_ID=" + C_SubAcct_ID);
            }
            if (M_Product_ID == 0)
            {
                sql.Append(" AND M_Product_ID IS NULL");
            }
            else
            {
                sql.Append(" AND M_Product_ID=" + M_Product_ID);
            }
            if (C_BPartner_ID == 0)
            {
                sql.Append(" AND C_BPartner_ID IS NULL");
            }
            else
            {
                sql.Append(" AND C_BPartner_ID=" + C_BPartner_ID);
            }
            if (AD_OrgTrx_ID == 0)
            {
                sql.Append(" AND AD_OrgTrx_ID IS NULL");
            }
            else
            {
                sql.Append(" AND AD_OrgTrx_ID=" + AD_OrgTrx_ID);
            }
            if (C_LocFrom_ID == 0)
            {
                sql.Append(" AND C_LocFrom_ID IS NULL");
            }
            else
            {
                sql.Append(" AND C_LocFrom_ID=" + C_LocFrom_ID);
            }
            if (C_LocTo_ID == 0)
            {
                sql.Append(" AND C_LocTo_ID IS NULL");
            }
            else
            {
                sql.Append(" AND C_LocTo_ID=" + C_LocTo_ID);
            }
            if (C_SalesRegion_ID == 0)
            {
                sql.Append(" AND C_SalesRegion_ID IS NULL");
            }
            else
            {
                sql.Append(" AND C_SalesRegion_ID=" + C_SalesRegion_ID);
            }
            if (C_Project_ID == 0)
            {
                sql.Append(" AND C_Project_ID IS NULL");
            }
            else
            {
                sql.Append(" AND C_Project_ID=" + C_Project_ID);
            }
            if (C_Campaign_ID == 0)
            {
                sql.Append(" AND C_Campaign_ID IS NULL");
            }
            else
            {
                sql.Append(" AND C_Campaign_ID=" + C_Campaign_ID);
            }
            if (C_Activity_ID == 0)
            {
                sql.Append(" AND C_Activity_ID IS NULL");
            }
            else
            {
                sql.Append(" AND C_Activity_ID=" + C_Activity_ID);
            }
            if (User1_ID == 0)
            {
                sql.Append(" AND User1_ID IS NULL");
            }
            else
            {
                sql.Append(" AND User1_ID=" + User1_ID);
            }
            if (User2_ID == 0)
            {
                sql.Append(" AND User2_ID IS NULL");
            }
            else
            {
                sql.Append(" AND User2_ID=" + User2_ID);
            }
            if (UserElement1_ID == 0)
            {
                sql.Append(" AND UserElement1_ID IS NULL");
            }
            else
            {
                sql.Append(" AND UserElement1_ID=" + UserElement1_ID);
            }
            if (UserElement2_ID == 0)
            {
                sql.Append(" AND UserElement2_ID IS NULL");
            }
            else
            {
                sql.Append(" AND UserElement2_ID=" + UserElement2_ID);
            }
            sql.Append(" AND IsActive='Y'");
            //	sql.Append(" ORDER BY IsFullyQualified DESC");
            try
            {
                DataSet ds = null;
                //IDataReader dr=null;
                ds = DataBase.DB.ExecuteDataset(sql.ToString(), null, null);
                //  --  Mandatory Accounting fields
                info.Append("AD_Client_ID=").Append(AD_Client_ID).Append(",AD_Org_ID=").Append(AD_Org_ID);
                //	Schema
                info.Append(",C_AcctSchema_ID=").Append(C_AcctSchema_ID);
                //	Account
                info.Append(",Account_ID=").Append(Account_ID).Append(" ");

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                //if (dr.Read())
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    existingAccount = new MAccount(ctx, rs, null);
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, info + "\n" + sql, e);
            }
            //	Existing
            if (existingAccount != null)
                return existingAccount;
            //	New
            MAccount newAccount = new MAccount(ctx, 0, null);
            newAccount.SetClientOrg(AD_Client_ID, AD_Org_ID);
            newAccount.SetC_AcctSchema_ID(C_AcctSchema_ID);
            newAccount.SetAccount_ID(Account_ID);
            //	--  Optional Accounting fields
            newAccount.SetC_SubAcct_ID(C_SubAcct_ID);
            newAccount.SetM_Product_ID(M_Product_ID);
            newAccount.SetC_BPartner_ID(C_BPartner_ID);
            newAccount.SetAD_OrgTrx_ID(AD_OrgTrx_ID);
            newAccount.SetC_LocFrom_ID(C_LocFrom_ID);
            newAccount.SetC_LocTo_ID(C_LocTo_ID);
            newAccount.SetC_SalesRegion_ID(C_SalesRegion_ID);
            newAccount.SetC_Project_ID(C_Project_ID);
            newAccount.SetC_Campaign_ID(C_Campaign_ID);
            newAccount.SetC_Activity_ID(C_Activity_ID);
            newAccount.SetUser1_ID(User1_ID);
            newAccount.SetUser2_ID(User2_ID);
            newAccount.SetUserElement1_ID(UserElement1_ID);
            newAccount.SetUserElement2_ID(UserElement2_ID);
            //
            if (!newAccount.Save())
            {
                _log.Log(Level.SEVERE, "Could not create new account - " + info);
                return null;
            }
            _log.Fine("New: " + newAccount);
            return newAccount;
        }


        // Added by Bharat for New elements UserElements1 to UserElement9
        public static MAccount Get(Ctx ctx, int AD_Client_ID, int AD_Org_ID, int C_AcctSchema_ID, int Account_ID, int C_SubAcct_ID, int M_Product_ID, int C_BPartner_ID, int AD_OrgTrx_ID,
            int C_LocFrom_ID, int C_LocTo_ID, int C_SalesRegion_ID, int C_Project_ID, int C_Campaign_ID, int C_Activity_ID, int User1_ID, int User2_ID, int UserElement1_ID, int UserElement2_ID,
            int UserElement3_ID, int UserElement4_ID, int UserElement5_ID, int UserElement6_ID, int UserElement7_ID, int UserElement8_ID, int UserElement9_ID)
        {
            MAccount existingAccount = null;
            //
            StringBuilder info = new StringBuilder();
            StringBuilder sql = new StringBuilder("SELECT * FROM C_ValidCombination "
                //	Mandatory fields
                + "WHERE AD_Client_ID=" + AD_Client_ID		//	#1
                + " AND AD_Org_ID=" + AD_Org_ID
                + " AND C_AcctSchema_ID=" + C_AcctSchema_ID
                + " AND Account_ID=" + Account_ID);		//	#4
            //	Optional fields
            if (C_SubAcct_ID == 0)
            {
                sql.Append(" AND C_SubAcct_ID IS NULL");
            }
            else
            {
                sql.Append(" AND C_SubAcct_ID=" + C_SubAcct_ID);
            }
            if (M_Product_ID == 0)
            {
                sql.Append(" AND M_Product_ID IS NULL");
            }
            else
            {
                sql.Append(" AND M_Product_ID=" + M_Product_ID);
            }
            if (C_BPartner_ID == 0)
            {
                sql.Append(" AND C_BPartner_ID IS NULL");
            }
            else
            {
                sql.Append(" AND C_BPartner_ID=" + C_BPartner_ID);
            }



            if (AD_OrgTrx_ID == 0)
            {
                sql.Append(" AND AD_OrgTrx_ID IS NULL");
            }
            else
            {
                sql.Append(" AND AD_OrgTrx_ID=" + AD_OrgTrx_ID);
            }
            if (C_LocFrom_ID == 0)
            {
                sql.Append(" AND C_LocFrom_ID IS NULL");
            }
            else
            {
                sql.Append(" AND C_LocFrom_ID=" + C_LocFrom_ID);
            }
            if (C_LocTo_ID == 0)
            {
                sql.Append(" AND C_LocTo_ID IS NULL");
            }
            else
            {
                sql.Append(" AND C_LocTo_ID=" + C_LocTo_ID);
            }
            if (C_SalesRegion_ID == 0)
            {
                sql.Append(" AND C_SalesRegion_ID IS NULL");
            }
            else
            {
                sql.Append(" AND C_SalesRegion_ID=" + C_SalesRegion_ID);
            }
            if (C_Project_ID == 0)
            {
                sql.Append(" AND C_Project_ID IS NULL");
            }
            else
            {
                sql.Append(" AND C_Project_ID=" + C_Project_ID);
            }
            if (C_Campaign_ID == 0)
            {
                sql.Append(" AND C_Campaign_ID IS NULL");
            }
            else
            {
                sql.Append(" AND C_Campaign_ID=" + C_Campaign_ID);
            }
            if (C_Activity_ID == 0)
            {
                sql.Append(" AND C_Activity_ID IS NULL");
            }
            else
            {
                sql.Append(" AND C_Activity_ID=" + C_Activity_ID);
            }
            if (User1_ID == 0)
            {
                sql.Append(" AND User1_ID IS NULL");
            }
            else
            {
                sql.Append(" AND User1_ID=" + User1_ID);
            }
            if (User2_ID == 0)
            {
                sql.Append(" AND User2_ID IS NULL");
            }
            else
            {
                sql.Append(" AND User2_ID=" + User2_ID);
            }
            if (UserElement1_ID == 0)
            {
                sql.Append(" AND UserElement1_ID IS NULL");
            }
            else
            {
                sql.Append(" AND UserElement1_ID=" + UserElement1_ID);
            }
            if (UserElement2_ID == 0)
            {
                sql.Append(" AND UserElement2_ID IS NULL");
            }
            else
            {
                sql.Append(" AND UserElement2_ID=" + UserElement2_ID);
            }
            if (UserElement3_ID == 0)
            {
                sql.Append(" AND UserElement3_ID IS NULL");
            }
            else
            {
                sql.Append(" AND UserElement3_ID=" + UserElement3_ID);
            }
            if (UserElement4_ID == 0)
            {
                sql.Append(" AND UserElement4_ID IS NULL");
            }
            else
            {
                sql.Append(" AND UserElement4_ID=" + UserElement4_ID);
            }
            if (UserElement5_ID == 0)
            {
                sql.Append(" AND UserElement5_ID IS NULL");
            }
            else
            {
                sql.Append(" AND UserElement5_ID=" + UserElement5_ID);
            }
            if (UserElement6_ID == 0)
            {
                sql.Append(" AND UserElement6_ID IS NULL");
            }
            else
            {
                sql.Append(" AND UserElement6_ID=" + UserElement6_ID);
            }
            if (UserElement7_ID == 0)
            {
                sql.Append(" AND UserElement7_ID IS NULL");
            }
            else
            {
                sql.Append(" AND UserElement7_ID=" + UserElement7_ID);
            }
            if (UserElement8_ID == 0)
            {
                sql.Append(" AND UserElement8_ID IS NULL");
            }
            else
            {
                sql.Append(" AND UserElement8_ID=" + UserElement8_ID);
            }
            if (UserElement9_ID == 0)
            {
                sql.Append(" AND UserElement9_ID IS NULL");
            }
            else
            {
                sql.Append(" AND UserElement9_ID=" + UserElement9_ID);
            }
            sql.Append(" AND IsActive='Y'");
            //	sql.Append(" ORDER BY IsFullyQualified DESC");
            try
            {
                DataSet ds = null;
                //IDataReader dr=null;
                ds = DataBase.DB.ExecuteDataset(sql.ToString(), null, null);
                //  --  Mandatory Accounting fields
                info.Append("AD_Client_ID=").Append(AD_Client_ID).Append(",AD_Org_ID=").Append(AD_Org_ID);
                //	Schema
                info.Append(",C_AcctSchema_ID=").Append(C_AcctSchema_ID);
                //	Account
                info.Append(",Account_ID=").Append(Account_ID).Append(" ");

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                //if (dr.Read())
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    existingAccount = new MAccount(ctx, rs, null);
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, info + "\n" + sql, e);
            }
            //	Existing
            if (existingAccount != null)
                return existingAccount;
            //	New
            MAccount newAccount = new MAccount(ctx, 0, null);
            newAccount.SetClientOrg(AD_Client_ID, AD_Org_ID);
            newAccount.SetC_AcctSchema_ID(C_AcctSchema_ID);
            newAccount.SetAccount_ID(Account_ID);
            //	--  Optional Accounting fields
            newAccount.SetC_SubAcct_ID(C_SubAcct_ID);
            newAccount.SetM_Product_ID(M_Product_ID);
            newAccount.SetC_BPartner_ID(C_BPartner_ID);
            newAccount.SetAD_OrgTrx_ID(AD_OrgTrx_ID);
            newAccount.SetC_LocFrom_ID(C_LocFrom_ID);
            newAccount.SetC_LocTo_ID(C_LocTo_ID);
            newAccount.SetC_SalesRegion_ID(C_SalesRegion_ID);
            newAccount.SetC_Project_ID(C_Project_ID);
            newAccount.SetC_Campaign_ID(C_Campaign_ID);
            newAccount.SetC_Activity_ID(C_Activity_ID);
            newAccount.SetUser1_ID(User1_ID);
            newAccount.SetUser2_ID(User2_ID);
            newAccount.SetUserElement1_ID(UserElement1_ID);
            newAccount.SetUserElement2_ID(UserElement2_ID);
            newAccount.SetUserElement3_ID(UserElement3_ID);
            newAccount.SetUserElement4_ID(UserElement4_ID);
            newAccount.SetUserElement5_ID(UserElement5_ID);
            newAccount.SetUserElement6_ID(UserElement6_ID);
            newAccount.SetUserElement7_ID(UserElement7_ID);
            newAccount.SetUserElement8_ID(UserElement8_ID);
            newAccount.SetUserElement9_ID(UserElement9_ID);

            //
            if (!newAccount.Save())
            {
                _log.Log(Level.SEVERE, "Could not create new account - " + info);
                return null;
            }
            _log.Fine("New: " + newAccount);
            return newAccount;
        }

        /// <summary>
        /// Get first with Alias
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_AcctSchema_ID"></param>
        /// <param name="alias"></param>
        /// <returns>account</returns>
        public static MAccount Get(Ctx ctx, int C_AcctSchema_ID, String alias)
        {
            MAccount retValue = null;
            String sql = "SELECT * FROM C_ValidCombination WHERE C_AcctSchema_ID=" + C_AcctSchema_ID + " AND Alias=" + alias;
            DataSet ds = new DataSet();
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    retValue = new MAccount(ctx, rs, null);
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            return retValue;
        }

        /// <summary>
        /// Get from existing Accounting fact
        /// </summary>
        /// <param name="fa">accounting fact</param>
        /// <returns>account</returns>
        public static MAccount Get(X_Fact_Acct fa)
        {
            MAccount acct = Get(fa.GetCtx(),
                fa.GetAD_Client_ID(), fa.GetAD_Org_ID(), fa.GetC_AcctSchema_ID(),
                fa.GetAccount_ID(), fa.GetC_SubAcct_ID(),
                fa.GetM_Product_ID(), fa.GetC_BPartner_ID(), fa.GetAD_OrgTrx_ID(),
                fa.GetC_LocFrom_ID(), fa.GetC_LocTo_ID(), fa.GetC_SalesRegion_ID(),
                fa.GetC_Project_ID(), fa.GetC_Campaign_ID(), fa.GetC_Activity_ID(),
                fa.GetUser1_ID(), fa.GetUser2_ID(), fa.GetUserElement1_ID(), fa.GetUserElement2_ID());
            return acct;
        }

        /// <summary>
        /// Factory: default combination
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_AcctSchema_ID">accounting schema</param>
        /// <param name="optionalNull">if true the optional values are null</param>
        /// <param name="trxName"></param>
        /// <returns>Account</returns>
        public static MAccount GetDefault(Ctx ctx, int C_AcctSchema_ID,
            bool optionalNull, Trx trxName)
        {
            MAcctSchema acctSchema = new MAcctSchema(ctx, C_AcctSchema_ID, trxName);
            return GetDefault(acctSchema, optionalNull);
        }

        /// <summary>
        /// Factory: default combination
        /// </summary>
        /// <param name="acctSchema">accounting schema</param>
        /// <param name="optionalNull">if true, the optional values are null</param>
        /// <returns>Account</returns>
        public static MAccount GetDefault(MAcctSchema acctSchema, bool optionalNull)
        {
            MAccount vc = new MAccount(acctSchema);
            //  Active Elements
            MAcctSchemaElement[] elements = acctSchema.GetAcctSchemaElements();
            for (int i = 0; i < elements.Length; i++)
            {
                MAcctSchemaElement ase = elements[i];
                String elementType = ase.GetElementType();
                int defaultValue = ase.GetDefaultValue();
                bool setValue = ase.IsMandatory() || (!ase.IsMandatory() && !optionalNull);
                //
                if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_Organization))
                    vc.SetAD_Org_ID(defaultValue);
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_Account))
                    vc.SetAccount_ID(defaultValue);
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_SubAccount) && setValue)
                    vc.SetC_SubAcct_ID(defaultValue);
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_BPartner) && setValue)
                    vc.SetC_BPartner_ID(defaultValue);
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_Product) && setValue)
                    vc.SetM_Product_ID(defaultValue);
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_Activity) && setValue)
                    vc.SetC_Activity_ID(defaultValue);
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_LocationFrom) && setValue)
                    vc.SetC_LocFrom_ID(defaultValue);
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_LocationTo) && setValue)
                    vc.SetC_LocTo_ID(defaultValue);
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_Campaign) && setValue)
                    vc.SetC_Campaign_ID(defaultValue);
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_OrgTrx) && setValue)
                    vc.SetAD_OrgTrx_ID(defaultValue);
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_Project) && setValue)
                    vc.SetC_Project_ID(defaultValue);
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_SalesRegion) && setValue)
                    vc.SetC_SalesRegion_ID(defaultValue);
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_UserList1) && setValue)
                    vc.SetUser1_ID(defaultValue);
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_UserList2) && setValue)
                    vc.SetUser2_ID(defaultValue);
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_UserElement1) && setValue)
                    vc.SetUserElement1_ID(defaultValue);
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_UserElement2) && setValue)
                    vc.SetUserElement2_ID(defaultValue);
            }
            _log.Fine("Client_ID=" + vc.GetAD_Client_ID() + ", Org_ID=" + vc.GetAD_Org_ID()
                + " - AcctSchema_ID=" + vc.GetC_AcctSchema_ID() + ", Account_ID=" + vc.GetAccount_ID());
            return vc;
        }

        /// <summary>
        /// Get Account
        /// </summary>
        /// <param name="ctx"> context</param>
        /// <param name="C_ValidCombination_ID">combination</param>
        /// <returns>Account</returns>
        public static MAccount Get(Ctx ctx, int C_ValidCombination_ID)
        {
            //	Maybe later cache
            return new MAccount(ctx, C_ValidCombination_ID, null);
        }

        /// <summary>
        /// Update Value/Description after change of 
        /// account element value/description.
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="where">where clause</param>
        /// <param name="trxName">transaction</param>
        public static void UpdateValueDescription(Ctx ctx, String where, Trx trxName)
        {
            String sql = "SELECT * FROM C_ValidCombination";
            if (where != null && where.Length > 0)
                sql += " WHERE " + where;
            sql += " ORDER BY C_ValidCombination_ID";
            int count = 0;
            int errors = 0;
            DataSet ds = new DataSet();
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    MAccount account = new MAccount(ctx, rs, trxName);
                    account.SetValueDescription();
                    if (account.Save())
                        count++;
                    else
                        errors++;
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            _log.Fine(where + " #" + count + ", Errors=" + errors);
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_ValidCombination_ID"></param>
        /// <param name="trxName"></param>
        public MAccount(Ctx ctx, int C_ValidCombination_ID, Trx trxName)
            : base(ctx, C_ValidCombination_ID, trxName)
        {

            if (C_ValidCombination_ID == 0)
            {
                //	setAccount_ID (0);
                //	setC_AcctSchema_ID (0);
                SetIsFullyQualified(false);
            }
        }

        /// <summary>
        /// Load constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rs"></param>
        /// <param name="trxName"></param>
        public MAccount(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="as1">as account schema</param>
        public MAccount(MAcctSchema as1)
            : this(as1.GetCtx(), 0, as1.Get_TrxName())
        {
            SetClientOrg(as1);
            SetC_AcctSchema_ID(as1.GetC_AcctSchema_ID());
        }

        /// <summary>
        /// Return String representation
        /// </summary>
        /// <returns>string</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MAccount=[");
            sb.Append(GetC_ValidCombination_ID());
            if (GetCombination() != null)
                sb.Append(",").Append(GetCombination());
            else
            {
                //	.Append(",Client=").Append(getAD_Client_ID())
                sb.Append(",Schema=").Append(GetC_AcctSchema_ID())
                    .Append(",Org=").Append(GetAD_Org_ID())
                    .Append(",Acct=").Append(GetAccount_ID())
                    .Append(" ");
                if (GetC_SubAcct_ID() != 0)
                    sb.Append(",C_SubAcct_ID=").Append(GetC_SubAcct_ID());
                if (GetM_Product_ID() != 0)
                    sb.Append(",M_Product_ID=").Append(GetM_Product_ID());
                if (GetC_BPartner_ID() != 0)
                    sb.Append(",C_BPartner_ID=").Append(GetC_BPartner_ID());
                if (GetAD_OrgTrx_ID() != 0)
                    sb.Append(",AD_OrgTrx_ID=").Append(GetAD_OrgTrx_ID());
                if (GetC_LocFrom_ID() != 0)
                    sb.Append(",C_LocFrom_ID=").Append(GetC_LocFrom_ID());
                if (GetC_LocTo_ID() != 0)
                    sb.Append(",C_LocTo_ID=").Append(GetC_LocTo_ID());
                if (GetC_SalesRegion_ID() != 0)
                    sb.Append(",C_SalesRegion_ID=").Append(GetC_SalesRegion_ID());
                if (GetC_Project_ID() != 0)
                    sb.Append(",C_Project_ID=").Append(GetC_Project_ID());
                if (GetC_Campaign_ID() != 0)
                    sb.Append(",C_Campaign_ID=").Append(GetC_Campaign_ID());
                if (GetC_Activity_ID() != 0)
                    sb.Append(",C_Activity_ID=").Append(GetC_Activity_ID());
                if (GetUser1_ID() != 0)
                    sb.Append(",User1_ID=").Append(GetUser1_ID());
                if (GetUser2_ID() != 0)
                    sb.Append(",User2_ID=").Append(GetUser2_ID());
                if (GetUserElement1_ID() != 0)
                    sb.Append(",UserElement1_ID=").Append(GetUserElement1_ID());
                if (GetUserElement2_ID() != 0)
                    sb.Append(",UserElement2_ID=").Append(GetUserElement2_ID());
            }
            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// 	Set Account_ID
        /// </summary>
        /// <param name="Account_ID">id</param>
        public new void SetAccount_ID(int Account_ID)
        {
            _accountEV = null;	//	reset
            base.SetAccount_ID(Account_ID);
        }

        /// <summary>
        /// Set Account_ID
        /// </summary>
        /// <returns>element value</returns>
        public MElementValue GetAccount()
        {
            if (_accountEV == null)
            {
                if (GetAccount_ID() != 0)
                    _accountEV = new MElementValue(GetCtx(), GetAccount_ID(), Get_TrxName());
            }
            return _accountEV;
        }

        /// <summary>
        /// Get Account Type
        /// </summary>
        /// <returns>Account Type of Account Element</returns>
        public String GetAccountType()
        {
            if (_accountEV == null)
                GetAccount();
            if (_accountEV == null)
            {
                //log.log(Level.SEVERE, "No ElementValue for Account_ID=" + getAccount_ID());
                return "";
            }
            return _accountEV.GetAccountType();
        }

        /// <summary>
        /// Is this a Balance Sheet Account
        /// </summary>
        /// <returns>bool</returns>
        public bool IsBalanceSheet()
        {
            String accountType = GetAccountType();
            return (MElementValue.ACCOUNTTYPE_Asset.Equals(accountType)
                || MElementValue.ACCOUNTTYPE_Liability.Equals(accountType)
                || MElementValue.ACCOUNTTYPE_OwnerSEquity.Equals(accountType));
        }

        /// <summary>
        /// Is this an Activa Account
        /// </summary>
        /// <returns>bool</returns>
        public bool IsActiva()
        {
            return MElementValue.ACCOUNTTYPE_Asset.Equals(GetAccountType());
        }

        /// <summary>
        /// Is this a Passiva Account
        /// </summary>
        /// <returns>bool</returns>
        public bool IsPassiva()
        {
            String accountType = GetAccountType();
            return (MElementValue.ACCOUNTTYPE_Liability.Equals(accountType)
                || MElementValue.ACCOUNTTYPE_OwnerSEquity.Equals(accountType));
        }

        /// <summary>
        /// Set Value and Description and Fully Qualified Flag for Combination
        /// </summary>
        public void SetValueDescription()
        {
            StringBuilder combi = new StringBuilder();
            StringBuilder descr = new StringBuilder();
            bool fullyQualified = true;
            //
            MAcctSchema as1 = new MAcctSchema(GetCtx(), GetC_AcctSchema_ID(), Get_TrxName());	//	In Trx!
            MAcctSchemaElement[] elements = MAcctSchemaElement.GetAcctSchemaElements(as1);
            for (int i = 0; i < elements.Length; i++)
            {
                if (i > 0)
                {
                    combi.Append(as1.GetSeparator());
                    descr.Append(as1.GetSeparator());
                }
                MAcctSchemaElement element = elements[i];
                String combiStr = "_";		//	not defined
                String descrStr = "_";

                if (MAcctSchemaElement.ELEMENTTYPE_Organization.Equals(element.GetElementType()))
                {
                    if (GetAD_Org_ID() != 0)
                    {
                        MOrg org = new MOrg(GetCtx(), GetAD_Org_ID(), Get_TrxName());	//	in Trx!
                        combiStr = org.GetValue();
                        descrStr = org.GetName();
                    }
                    else
                    {
                        combiStr = "*";
                        descrStr = "*";
                        fullyQualified = false;
                    }
                }
                else if (MAcctSchemaElement.ELEMENTTYPE_Account.Equals(element.GetElementType()))
                {
                    if (GetAccount_ID() != 0)
                    {
                        if (_accountEV == null)
                            _accountEV = new MElementValue(GetCtx(), GetAccount_ID(), Get_TrxName());
                        combiStr = _accountEV.GetValue();
                        descrStr = _accountEV.GetName();
                    }
                    else if (element.IsMandatory())
                    {
                        //log.warning("Mandatory Element missing: Account");
                        fullyQualified = false;
                    }
                }
                else if (MAcctSchemaElement.ELEMENTTYPE_SubAccount.Equals(element.GetElementType()))
                {
                    if (GetC_SubAcct_ID() != 0)
                    {
                        X_C_SubAcct sa = new X_C_SubAcct(GetCtx(), GetC_SubAcct_ID(), Get_TrxName());
                        combiStr = sa.GetValue();
                        descrStr = sa.GetName();
                    }
                }
                else if (MAcctSchemaElement.ELEMENTTYPE_Product.Equals(element.GetElementType()))
                {
                    if (GetM_Product_ID() != 0)
                    {
                        X_M_Product product = new X_M_Product(GetCtx(), GetM_Product_ID(), Get_TrxName());
                        combiStr = product.GetValue();
                        descrStr = product.GetName();
                    }
                    else if (element.IsMandatory())
                    {
                        //log.warning("Mandatory Element missing: Product");
                        fullyQualified = false;
                    }
                }
                else if (MAcctSchemaElement.ELEMENTTYPE_BPartner.Equals(element.GetElementType()))
                {
                    if (GetC_BPartner_ID() != 0)
                    {
                        X_C_BPartner partner = new X_C_BPartner(GetCtx(), GetC_BPartner_ID(), Get_TrxName());
                        combiStr = partner.GetValue();
                        descrStr = partner.GetName();
                    }
                    else if (element.IsMandatory())
                    {
                        //log.warning("Mandatory Element missing: Business Partner");
                        fullyQualified = false;
                    }
                }
                else if (MAcctSchemaElement.ELEMENTTYPE_OrgTrx.Equals(element.GetElementType()))
                {
                    if (GetAD_OrgTrx_ID() != 0)
                    {
                        MOrg org = new MOrg(GetCtx(), GetAD_OrgTrx_ID(), Get_TrxName());	// in Trx!
                        combiStr = org.GetValue();
                        descrStr = org.GetName();
                    }
                    else if (element.IsMandatory())
                    {
                        //log.warning("Mandatory Element missing: Trx Org");
                        fullyQualified = false;
                    }
                }
                else if (MAcctSchemaElement.ELEMENTTYPE_LocationFrom.Equals(element.GetElementType()))
                {
                    if (GetC_LocFrom_ID() != 0)
                    {
                        MLocation loc = new MLocation(GetCtx(), GetC_LocFrom_ID(), Get_TrxName());	//	in Trx!
                        combiStr = loc.GetPostal();
                        descrStr = loc.GetCity();
                    }
                    else if (element.IsMandatory())
                    {
                        //log.warning("Mandatory Element missing: Location From");
                        fullyQualified = false;
                    }
                }
                else if (MAcctSchemaElement.ELEMENTTYPE_LocationTo.Equals(element.GetElementType()))
                {
                    if (GetC_LocTo_ID() != 0)
                    {
                        MLocation loc = new MLocation(GetCtx(), GetC_LocFrom_ID(), Get_TrxName());	//	in Trx!
                        combiStr = loc.GetPostal();
                        descrStr = loc.GetCity();
                    }
                    else if (element.IsMandatory())
                    {
                        //log.warning("Mandatory Element missing: Location To");
                        fullyQualified = false;
                    }
                }
                else if (MAcctSchemaElement.ELEMENTTYPE_SalesRegion.Equals(element.GetElementType()))
                {
                    if (GetC_SalesRegion_ID() != 0)
                    {
                        MSalesRegion loc = new MSalesRegion(GetCtx(), GetC_SalesRegion_ID(), Get_TrxName());
                        combiStr = loc.GetValue();
                        descrStr = loc.GetName();
                    }
                    else if (element.IsMandatory())
                    {
                        //log.warning("Mandatory Element missing: SalesRegion");
                        fullyQualified = false;
                    }
                }
                else if (MAcctSchemaElement.ELEMENTTYPE_Project.Equals(element.GetElementType()))
                {
                    if (GetC_Project_ID() != 0)
                    {
                        X_C_Project project = new X_C_Project(GetCtx(), GetC_Project_ID(), Get_TrxName());
                        combiStr = project.GetValue();
                        descrStr = project.GetName();
                    }
                    else if (element.IsMandatory())
                    {
                        //log.warning("Mandatory Element missing: Project");
                        fullyQualified = false;
                    }
                }
                else if (MAcctSchemaElement.ELEMENTTYPE_Campaign.Equals(element.GetElementType()))
                {
                    if (GetC_Campaign_ID() != 0)
                    {
                        X_C_Campaign campaign = new X_C_Campaign(GetCtx(), GetC_Campaign_ID(), Get_TrxName());
                        combiStr = campaign.GetValue();
                        descrStr = campaign.GetName();
                    }
                    else if (element.IsMandatory())
                    {
                        //log.warning("Mandatory Element missing: Campaign");
                        fullyQualified = false;
                    }
                }
                else if (MAcctSchemaElement.ELEMENTTYPE_Activity.Equals(element.GetElementType()))
                {
                    if (GetC_Activity_ID() != 0)
                    {
                        X_C_Activity act = new X_C_Activity(GetCtx(), GetC_Activity_ID(), Get_TrxName());
                        combiStr = act.GetValue();
                        descrStr = act.GetName();
                    }
                    else if (element.IsMandatory())
                    {
                        //log.warning("Mandatory Element missing: Campaign");
                        fullyQualified = false;
                    }
                }
                else if (MAcctSchemaElement.ELEMENTTYPE_UserList1.Equals(element.GetElementType()))
                {
                    if (GetUser1_ID() != 0)
                    {
                        MElementValue ev = new MElementValue(GetCtx(), GetUser1_ID(), Get_TrxName());
                        combiStr = ev.GetValue();
                        descrStr = ev.GetName();
                    }
                }
                else if (MAcctSchemaElement.ELEMENTTYPE_UserList2.Equals(element.GetElementType()))
                {
                    if (GetUser2_ID() != 0)
                    {
                        MElementValue ev = new MElementValue(GetCtx(), GetUser2_ID(), Get_TrxName());
                        combiStr = ev.GetValue();
                        descrStr = ev.GetName();
                    }
                }
                else if (MAcctSchemaElement.ELEMENTTYPE_UserElement1.Equals(element.GetElementType()))
                {
                    if (GetUserElement1_ID() != 0 && element.GetAD_Column_ID() > 0)
                    {
                        string qry = DBFunctionCollection.GetTableAndColumnName(element.GetAD_Column_ID());
                        DataSet ds = DB.ExecuteDataset(qry);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            string tableName = Util.GetValueOfString(ds.Tables[0].Rows[0]["TableName"]);
                            int tableID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_Table_ID"]);
                            string columnName = Util.GetValueOfString(ds.Tables[0].Rows[0]["ColumnName"]);
                            qry = "SELECT Count(*) FROM AD_Column WHERE AD_Table_ID =" + tableID + " AND ColumnName='Value'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Value FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement1_ID();
                                combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                combiStr = Util.GetValueOfString(GetUserElement1_ID());
                                if (!String.IsNullOrEmpty(Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"])))
                                {
                                    qry = "SELECT " + Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"]) +
                                        " FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement1_ID();
                                    combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                                }
                            }
                            qry = "SELECT Count(*) FROM AD_Column WHERE AD_Table_ID =" + tableID + " AND ColumnName='Name'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Name FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement1_ID();
                                descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                descrStr = Util.GetValueOfString(GetUserElement1_ID());
                                if (!String.IsNullOrEmpty(Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"])))
                                {
                                    qry = "SELECT " + Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"]) +
                                        " FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement1_ID();
                                    descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                                }
                            }
                        }
                    }
                }
                else if (MAcctSchemaElement.ELEMENTTYPE_UserElement2.Equals(element.GetElementType()))
                {
                    if (GetUserElement2_ID() != 0 && element.GetAD_Column_ID() > 0)
                    {
                        string qry = DBFunctionCollection.GetTableAndColumnName(element.GetAD_Column_ID());
                        DataSet ds = DB.ExecuteDataset(qry);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            string tableName = Util.GetValueOfString(ds.Tables[0].Rows[0]["TableName"]);
                            int tableID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_Table_ID"]);
                            string columnName = Util.GetValueOfString(ds.Tables[0].Rows[0]["ColumnName"]);
                            qry = "SELECT Count(*) FROM AD_Column WHERE AD_Table_ID =" + tableID + " AND ColumnName='Value'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Value FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement2_ID();
                                combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                combiStr = Util.GetValueOfString(GetUserElement2_ID());
                                if (!String.IsNullOrEmpty(Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"])))
                                {
                                    qry = "SELECT " + Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"]) +
                                        " FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement2_ID();
                                    combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                                }
                            }
                            qry = "SELECT Count(*) FROM AD_Column WHERE AD_Table_ID =" + tableID + " AND ColumnName='Name'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Name FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement2_ID();
                                descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                descrStr = Util.GetValueOfString(GetUserElement2_ID());
                                if (!String.IsNullOrEmpty(Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"])))
                                {
                                    qry = "SELECT " + Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"]) +
                                        " FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement2_ID();
                                    descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                                }
                            }
                        }
                    }
                }
                else if (MAcctSchemaElement.ELEMENTTYPE_UserElement3.Equals(element.GetElementType()))
                {
                    if (GetUserElement3_ID() != 0 && element.GetAD_Column_ID() > 0)
                    {
                        string qry = DBFunctionCollection.GetTableAndColumnName(element.GetAD_Column_ID());
                        DataSet ds = DB.ExecuteDataset(qry);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            string tableName = Util.GetValueOfString(ds.Tables[0].Rows[0]["TableName"]);
                            int tableID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_Table_ID"]);
                            string columnName = Util.GetValueOfString(ds.Tables[0].Rows[0]["ColumnName"]);
                            qry = "SELECT Count(*) FROM AD_Column WHERE AD_Table_ID =" + tableID + " AND ColumnName='Value'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Value FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement3_ID();
                                combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                combiStr = Util.GetValueOfString(GetUserElement3_ID());
                                if (!String.IsNullOrEmpty(Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"])))
                                {
                                    qry = "SELECT " + Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"]) +
                                        " FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement3_ID();
                                    combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                                }
                            }
                            qry = "SELECT Count(*) FROM AD_Column WHERE AD_Table_ID =" + tableID + " AND ColumnName='Name'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Name FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement3_ID();
                                descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                descrStr = Util.GetValueOfString(GetUserElement3_ID());
                                if (!String.IsNullOrEmpty(Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"])))
                                {
                                    qry = "SELECT " + Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"]) +
                                        " FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement3_ID();
                                    descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                                }
                            }
                        }
                    }
                }
                else if (MAcctSchemaElement.ELEMENTTYPE_UserElement4.Equals(element.GetElementType()))
                {
                    if (GetUserElement4_ID() != 0 && element.GetAD_Column_ID() > 0)
                    {
                        string qry = DBFunctionCollection.GetTableAndColumnName(element.GetAD_Column_ID());
                        DataSet ds = DB.ExecuteDataset(qry);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            string tableName = Util.GetValueOfString(ds.Tables[0].Rows[0]["TableName"]);
                            int tableID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_Table_ID"]);
                            string columnName = Util.GetValueOfString(ds.Tables[0].Rows[0]["ColumnName"]);
                            qry = "SELECT Count(*) FROM AD_Column WHERE AD_Table_ID =" + tableID + " AND ColumnName='Value'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Value FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement4_ID();
                                combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                combiStr = Util.GetValueOfString(GetUserElement4_ID());
                                if (!String.IsNullOrEmpty(Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"])))
                                {
                                    qry = "SELECT " + Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"]) +
                                        " FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement4_ID();
                                    combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                                }
                            }
                            qry = "SELECT Count(*) FROM AD_Column WHERE AD_Table_ID =" + tableID + " AND ColumnName='Name'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Name FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement4_ID();
                                descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                descrStr = Util.GetValueOfString(GetUserElement4_ID());
                                if (!String.IsNullOrEmpty(Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"])))
                                {
                                    qry = "SELECT " + Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"]) +
                                        " FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement4_ID();
                                    descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                                }
                            }
                        }
                    }
                }
                else if (MAcctSchemaElement.ELEMENTTYPE_UserElement5.Equals(element.GetElementType()))
                {
                    if (GetUserElement5_ID() != 0 && element.GetAD_Column_ID() > 0)
                    {
                        string qry = DBFunctionCollection.GetTableAndColumnName(element.GetAD_Column_ID());
                        DataSet ds = DB.ExecuteDataset(qry);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            string tableName = Util.GetValueOfString(ds.Tables[0].Rows[0]["TableName"]);
                            int tableID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_Table_ID"]);
                            string columnName = Util.GetValueOfString(ds.Tables[0].Rows[0]["ColumnName"]);
                            qry = "SELECT Count(*) FROM AD_Column WHERE AD_Table_ID =" + tableID + " AND ColumnName='Value'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Value FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement5_ID();
                                combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                combiStr = Util.GetValueOfString(GetUserElement5_ID());
                                if (!String.IsNullOrEmpty(Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"])))
                                {
                                    qry = "SELECT " + Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"]) +
                                        " FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement5_ID();
                                    combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                                }
                            }
                            qry = "SELECT Count(*) FROM AD_Column WHERE AD_Table_ID =" + tableID + " AND ColumnName='Name'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Name FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement5_ID();
                                descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                descrStr = Util.GetValueOfString(GetUserElement5_ID());
                                if (!String.IsNullOrEmpty(Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"])))
                                {
                                    qry = "SELECT " + Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"]) +
                                        " FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement5_ID();
                                    descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                                }
                            }
                        }
                    }
                }
                else if (MAcctSchemaElement.ELEMENTTYPE_UserElement6.Equals(element.GetElementType()))
                {
                    if (GetUserElement6_ID() != 0 && element.GetAD_Column_ID() > 0)
                    {
                        string qry = DBFunctionCollection.GetTableAndColumnName(element.GetAD_Column_ID());
                        DataSet ds = DB.ExecuteDataset(qry);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            string tableName = Util.GetValueOfString(ds.Tables[0].Rows[0]["TableName"]);
                            int tableID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_Table_ID"]);
                            string columnName = Util.GetValueOfString(ds.Tables[0].Rows[0]["ColumnName"]);
                            qry = "SELECT Count(*) FROM AD_Column WHERE AD_Table_ID =" + tableID + " AND ColumnName='Value'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Value FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement6_ID();
                                combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                combiStr = Util.GetValueOfString(GetUserElement6_ID());
                                if (!String.IsNullOrEmpty(Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"])))
                                {
                                    qry = "SELECT " + Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"]) +
                                        " FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement6_ID();
                                    combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                                }
                            }
                            qry = "SELECT Count(*) FROM AD_Column WHERE AD_Table_ID =" + tableID + " AND ColumnName='Name'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Name FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement6_ID();
                                descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                descrStr = Util.GetValueOfString(GetUserElement6_ID());
                                if (!String.IsNullOrEmpty(Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"])))
                                {
                                    qry = "SELECT " + Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"]) +
                                        " FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement6_ID();
                                    descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                                }
                            }
                        }
                    }
                }
                else if (MAcctSchemaElement.ELEMENTTYPE_UserElement7.Equals(element.GetElementType()))
                {
                    if (GetUserElement7_ID() != 0 && element.GetAD_Column_ID() > 0)
                    {
                        string qry = DBFunctionCollection.GetTableAndColumnName(element.GetAD_Column_ID());
                        DataSet ds = DB.ExecuteDataset(qry);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            string tableName = Util.GetValueOfString(ds.Tables[0].Rows[0]["TableName"]);
                            int tableID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_Table_ID"]);
                            string columnName = Util.GetValueOfString(ds.Tables[0].Rows[0]["ColumnName"]);
                            qry = "SELECT Count(*) FROM AD_Column WHERE AD_Table_ID =" + tableID + " AND ColumnName='Value'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Value FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement7_ID();
                                combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                combiStr = Util.GetValueOfString(GetUserElement7_ID());
                                if (!String.IsNullOrEmpty(Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"])))
                                {
                                    qry = "SELECT " + Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"]) +
                                        " FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement7_ID();
                                    combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                                }
                            }
                            qry = "SELECT Count(*) FROM AD_Column WHERE AD_Table_ID =" + tableID + " AND ColumnName='Name'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Name FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement7_ID();
                                descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                descrStr = Util.GetValueOfString(GetUserElement7_ID());
                                if (!String.IsNullOrEmpty(Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"])))
                                {
                                    qry = "SELECT " + Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"]) +
                                        " FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement7_ID();
                                    descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                                }
                            }
                        }
                    }
                }
                else if (MAcctSchemaElement.ELEMENTTYPE_UserElement8.Equals(element.GetElementType()))
                {
                    if (GetUserElement8_ID() != 0 && element.GetAD_Column_ID() > 0)
                    {
                        string qry = DBFunctionCollection.GetTableAndColumnName(element.GetAD_Column_ID());
                        DataSet ds = DB.ExecuteDataset(qry);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            string tableName = Util.GetValueOfString(ds.Tables[0].Rows[0]["TableName"]);
                            int tableID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_Table_ID"]);
                            string columnName = Util.GetValueOfString(ds.Tables[0].Rows[0]["ColumnName"]);
                            qry = "SELECT Count(*) FROM AD_Column WHERE AD_Table_ID =" + tableID + " AND ColumnName='Value'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Value FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement8_ID();
                                combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                combiStr = Util.GetValueOfString(GetUserElement8_ID());
                                if (!String.IsNullOrEmpty(Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"])))
                                {
                                    qry = "SELECT " + Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"]) +
                                        " FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement8_ID();
                                    combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                                }
                            }
                            qry = "SELECT Count(*) FROM AD_Column WHERE AD_Table_ID =" + tableID + " AND ColumnName='Name'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Name FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement8_ID();
                                descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                descrStr = Util.GetValueOfString(GetUserElement8_ID());
                                if (!String.IsNullOrEmpty(Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"])))
                                {
                                    qry = "SELECT " + Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"]) +
                                        " FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement8_ID();
                                    descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                                }
                            }
                        }
                    }
                }
                else if (MAcctSchemaElement.ELEMENTTYPE_UserElement9.Equals(element.GetElementType()))
                {
                    if (GetUserElement9_ID() != 0 && element.GetAD_Column_ID() > 0)
                    {
                        string qry = DBFunctionCollection.GetTableAndColumnName(element.GetAD_Column_ID());
                        DataSet ds = DB.ExecuteDataset(qry);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            string tableName = Util.GetValueOfString(ds.Tables[0].Rows[0]["TableName"]);
                            int tableID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_Table_ID"]);
                            string columnName = Util.GetValueOfString(ds.Tables[0].Rows[0]["ColumnName"]);
                            qry = "SELECT Count(*) FROM AD_Column WHERE AD_Table_ID =" + tableID + " AND ColumnName='Value'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Value FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement9_ID();
                                combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                combiStr = Util.GetValueOfString(GetUserElement9_ID());
                                if (!String.IsNullOrEmpty(Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"])))
                                {
                                    qry = "SELECT " + Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"]) +
                                        " FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement9_ID();
                                    combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                                }
                            }
                            qry = "SELECT Count(*) FROM AD_Column WHERE AD_Table_ID =" + tableID + " AND ColumnName='Name'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Name FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement9_ID();
                                descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                descrStr = Util.GetValueOfString(GetUserElement9_ID());
                                if (!String.IsNullOrEmpty(Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"])))
                                {
                                    qry = "SELECT " + Util.GetValueOfString(ds.Tables[0].Rows[0]["identfierColumnName"]) +
                                        " FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement9_ID();
                                    descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                                }
                            }
                        }
                    }
                }
                combi.Append(combiStr);
                descr.Append(descrStr);
            }
            //	Set Values
            base.SetCombination(combi.ToString());
            base.SetDescription(descr.ToString());
            if (fullyQualified != IsFullyQualified())
            {
                SetIsFullyQualified(fullyQualified);
                _log.Fine("Combination=" + GetCombination() + " - " + GetDescription() + " - FullyQualified=" + fullyQualified);
            }
        }

        /// <summary>
        /// Validate combination
        /// </summary>
        /// <returns>true if valid</returns>
        public bool Validate()
        {
            bool ok = true;
            //	Validate Sub-Account
            if (GetC_SubAcct_ID() != 0)
            {
                X_C_SubAcct sa = new X_C_SubAcct(GetCtx(), GetC_SubAcct_ID(), Get_TrxName());
                if (sa.GetC_ElementValue_ID() != GetAccount_ID())
                {
                    log.SaveError("Error", "C_SubAcct.C_ElementValue_ID=" + sa.GetC_ElementValue_ID()
                        + "<>Account_ID=" + GetAccount_ID());
                    ok = false;
                }
            }
            return ok;
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            SetValueDescription();
            return Validate();
        }



    }
}
