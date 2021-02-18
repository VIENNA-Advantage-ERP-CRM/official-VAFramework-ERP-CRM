/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVABAccount
 * Purpose        : Account Object Entity to maintain all segment values.
 * Class Used     : X_VAB_Acct_ValidParameter
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
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
namespace VAdvantage.Model
{
    public class MVABAccount : X_VAB_Acct_ValidParameter
    {
        /**	Logger						*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MVABAccount).FullName);
        /**	Account Segment				*/
        private MVABAcctElement _accountEV = null;

        /// <summary>
        /// Get existing Account or create it 
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Client_ID">client id</param>
        /// <param name="VAF_Org_ID"> organisation id</param>
        /// <param name="VAB_AccountBook_ID"> account schema id</param>
        /// <param name="Account_ID">account id</param>
        /// <param name="VAB_SubAcct_ID">sub-Account id</param>
        /// <param name="VAM_Product_ID">product id</param>
        /// <param name="VAB_BusinessPartner_ID">Bussness partner id</param>
        /// <param name="VAF_OrgTrx_ID"> ordTax id</param>
        /// <param name="C_LocFrom_ID">C_LocFrom_ID</param>
        /// <param name="C_LocTo_ID"></param>
        /// <param name="VAB_SalesRegionState_ID"></param>
        /// <param name="VAB_Project_ID"></param>
        /// <param name="VAB_Promotion_ID"></param>
        /// <param name="VAB_BillingCode_ID"></param>
        /// <param name="User1_ID"></param>
        /// <param name="User2_ID"></param>
        /// <param name="UserElement1_ID"></param>
        /// <param name="UserElement2_ID"></param>
        /// <returns>account or null</returns>
        public static MVABAccount Get(Ctx ctx, int VAF_Client_ID, int VAF_Org_ID, int VAB_AccountBook_ID,
            int Account_ID, int VAB_SubAcct_ID, int VAM_Product_ID, int VAB_BusinessPartner_ID, int VAF_OrgTrx_ID,
            int C_LocFrom_ID, int C_LocTo_ID, int VAB_SalesRegionState_ID, int VAB_Project_ID, int VAB_Promotion_ID,
            int VAB_BillingCode_ID, int User1_ID, int User2_ID, int UserElement1_ID, int UserElement2_ID)
        {
            MVABAccount existingAccount = null;
            //
            StringBuilder info = new StringBuilder();
            StringBuilder sql = new StringBuilder("SELECT * FROM VAB_Acct_ValidParameter "
                //	Mandatory fields
                + "WHERE VAF_Client_ID=" + VAF_Client_ID		//	#1
                + " AND VAF_Org_ID=" + VAF_Org_ID
                + " AND VAB_AccountBook_ID=" + VAB_AccountBook_ID
                + " AND Account_ID=" + Account_ID);		//	#4
            //	Optional fields
            if (VAB_SubAcct_ID == 0)
            {
                sql.Append(" AND VAB_SubAcct_ID IS NULL");
            }
            else
            {
                sql.Append(" AND VAB_SubAcct_ID=" + VAB_SubAcct_ID);
            }
            if (VAM_Product_ID == 0)
            {
                sql.Append(" AND VAM_Product_ID IS NULL");
            }
            else
            {
                sql.Append(" AND VAM_Product_ID=" + VAM_Product_ID);
            }
            if (VAB_BusinessPartner_ID == 0)
            {
                sql.Append(" AND VAB_BusinessPartner_ID IS NULL");
            }
            else
            {
                sql.Append(" AND VAB_BusinessPartner_ID=" + VAB_BusinessPartner_ID);
            }
            if (VAF_OrgTrx_ID == 0)
            {
                sql.Append(" AND VAF_OrgTrx_ID IS NULL");
            }
            else
            {
                sql.Append(" AND VAF_OrgTrx_ID=" + VAF_OrgTrx_ID);
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
            if (VAB_SalesRegionState_ID == 0)
            {
                sql.Append(" AND VAB_SalesRegionState_ID IS NULL");
            }
            else
            {
                sql.Append(" AND VAB_SalesRegionState_ID=" + VAB_SalesRegionState_ID);
            }
            if (VAB_Project_ID == 0)
            {
                sql.Append(" AND VAB_Project_ID IS NULL");
            }
            else
            {
                sql.Append(" AND VAB_Project_ID=" + VAB_Project_ID);
            }
            if (VAB_Promotion_ID == 0)
            {
                sql.Append(" AND VAB_Promotion_ID IS NULL");
            }
            else
            {
                sql.Append(" AND VAB_Promotion_ID=" + VAB_Promotion_ID);
            }
            if (VAB_BillingCode_ID == 0)
            {
                sql.Append(" AND VAB_BillingCode_ID IS NULL");
            }
            else
            {
                sql.Append(" AND VAB_BillingCode_ID=" + VAB_BillingCode_ID);
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
                ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql.ToString(), null, null);
                //  --  Mandatory Accounting fields
                info.Append("VAF_Client_ID=").Append(VAF_Client_ID).Append(",VAF_Org_ID=").Append(VAF_Org_ID);
                //	Schema
                info.Append(",VAB_AccountBook_ID=").Append(VAB_AccountBook_ID);
                //	Account
                info.Append(",Account_ID=").Append(Account_ID).Append(" ");

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                //if (dr.Read())
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    existingAccount = new MVABAccount(ctx, rs, null);
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
            MVABAccount newAccount = new MVABAccount(ctx, 0, null);
            newAccount.SetClientOrg(VAF_Client_ID, VAF_Org_ID);
            newAccount.SetVAB_AccountBook_ID(VAB_AccountBook_ID);
            newAccount.SetAccount_ID(Account_ID);
            //	--  Optional Accounting fields
            newAccount.SetVAB_SubAcct_ID(VAB_SubAcct_ID);
            newAccount.SetVAM_Product_ID(VAM_Product_ID);
            newAccount.SetVAB_BusinessPartner_ID(VAB_BusinessPartner_ID);
            newAccount.SetVAF_OrgTrx_ID(VAF_OrgTrx_ID);
            newAccount.SetC_LocFrom_ID(C_LocFrom_ID);
            newAccount.SetC_LocTo_ID(C_LocTo_ID);
            newAccount.SetVAB_SalesRegionState_ID(VAB_SalesRegionState_ID);
            newAccount.SetVAB_Project_ID(VAB_Project_ID);
            newAccount.SetVAB_Promotion_ID(VAB_Promotion_ID);
            newAccount.SetVAB_BillingCode_ID(VAB_BillingCode_ID);
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
        public static MVABAccount Get(Ctx ctx, int VAF_Client_ID, int VAF_Org_ID, int VAB_AccountBook_ID, int Account_ID, int VAB_SubAcct_ID, int VAM_Product_ID, int VAB_BusinessPartner_ID, int VAF_OrgTrx_ID,
            int C_LocFrom_ID, int C_LocTo_ID, int VAB_SalesRegionState_ID, int VAB_Project_ID, int VAB_Promotion_ID, int VAB_BillingCode_ID, int User1_ID, int User2_ID, int UserElement1_ID, int UserElement2_ID,
            int UserElement3_ID, int UserElement4_ID, int UserElement5_ID, int UserElement6_ID, int UserElement7_ID, int UserElement8_ID, int UserElement9_ID)
        {
            MVABAccount existingAccount = null;
            //
            StringBuilder info = new StringBuilder();
            StringBuilder sql = new StringBuilder("SELECT * FROM VAB_Acct_ValidParameter "
                //	Mandatory fields
                + "WHERE VAF_Client_ID=" + VAF_Client_ID		//	#1
                + " AND VAF_Org_ID=" + VAF_Org_ID
                + " AND VAB_AccountBook_ID=" + VAB_AccountBook_ID
                + " AND Account_ID=" + Account_ID);		//	#4
            //	Optional fields
            if (VAB_SubAcct_ID == 0)
            {
                sql.Append(" AND VAB_SubAcct_ID IS NULL");
            }
            else
            {
                sql.Append(" AND VAB_SubAcct_ID=" + VAB_SubAcct_ID);
            }
            if (VAM_Product_ID == 0)
            {
                sql.Append(" AND VAM_Product_ID IS NULL");
            }
            else
            {
                sql.Append(" AND VAM_Product_ID=" + VAM_Product_ID);
            }
            if (VAB_BusinessPartner_ID == 0)
            {
                sql.Append(" AND VAB_BusinessPartner_ID IS NULL");
            }
            else
            {
                sql.Append(" AND VAB_BusinessPartner_ID=" + VAB_BusinessPartner_ID);
            }

         

            if (VAF_OrgTrx_ID == 0)
            {
                sql.Append(" AND VAF_OrgTrx_ID IS NULL");
            }
            else
            {
                sql.Append(" AND VAF_OrgTrx_ID=" + VAF_OrgTrx_ID);
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
            if (VAB_SalesRegionState_ID == 0)
            {
                sql.Append(" AND VAB_SalesRegionState_ID IS NULL");
            }
            else
            {
                sql.Append(" AND VAB_SalesRegionState_ID=" + VAB_SalesRegionState_ID);
            }
            if (VAB_Project_ID == 0)
            {
                sql.Append(" AND VAB_Project_ID IS NULL");
            }
            else
            {
                sql.Append(" AND VAB_Project_ID=" + VAB_Project_ID);
            }
            if (VAB_Promotion_ID == 0)
            {
                sql.Append(" AND VAB_Promotion_ID IS NULL");
            }
            else
            {
                sql.Append(" AND VAB_Promotion_ID=" + VAB_Promotion_ID);
            }
            if (VAB_BillingCode_ID == 0)
            {
                sql.Append(" AND VAB_BillingCode_ID IS NULL");
            }
            else
            {
                sql.Append(" AND VAB_BillingCode_ID=" + VAB_BillingCode_ID);
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
                ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql.ToString(), null, null);
                //  --  Mandatory Accounting fields
                info.Append("VAF_Client_ID=").Append(VAF_Client_ID).Append(",VAF_Org_ID=").Append(VAF_Org_ID);
                //	Schema
                info.Append(",VAB_AccountBook_ID=").Append(VAB_AccountBook_ID);
                //	Account
                info.Append(",Account_ID=").Append(Account_ID).Append(" ");

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                //if (dr.Read())
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    existingAccount = new MVABAccount(ctx, rs, null);
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
            MVABAccount newAccount = new MVABAccount(ctx, 0, null);
            newAccount.SetClientOrg(VAF_Client_ID, VAF_Org_ID);
            newAccount.SetVAB_AccountBook_ID(VAB_AccountBook_ID);
            newAccount.SetAccount_ID(Account_ID);
            //	--  Optional Accounting fields
            newAccount.SetVAB_SubAcct_ID(VAB_SubAcct_ID);
            newAccount.SetVAM_Product_ID(VAM_Product_ID);
            newAccount.SetVAB_BusinessPartner_ID(VAB_BusinessPartner_ID);
            newAccount.SetVAF_OrgTrx_ID(VAF_OrgTrx_ID);
            newAccount.SetC_LocFrom_ID(C_LocFrom_ID);
            newAccount.SetC_LocTo_ID(C_LocTo_ID);
            newAccount.SetVAB_SalesRegionState_ID(VAB_SalesRegionState_ID);
            newAccount.SetVAB_Project_ID(VAB_Project_ID);
            newAccount.SetVAB_Promotion_ID(VAB_Promotion_ID);
            newAccount.SetVAB_BillingCode_ID(VAB_BillingCode_ID);
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
        /// <param name="VAB_AccountBook_ID"></param>
        /// <param name="alias"></param>
        /// <returns>account</returns>
        public static MVABAccount Get(Ctx ctx, int VAB_AccountBook_ID, String alias)
        {
            MVABAccount retValue = null;
            String sql = "SELECT * FROM VAB_Acct_ValidParameter WHERE VAB_AccountBook_ID=" + VAB_AccountBook_ID + " AND Alias=" + alias;
            DataSet ds = new DataSet();
            try
            {
                ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    retValue = new MVABAccount(ctx, rs, null);
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
        public static MVABAccount Get(X_Actual_Acct_Detail fa)
        {
            MVABAccount acct = Get(fa.GetCtx(),
                fa.GetVAF_Client_ID(), fa.GetVAF_Org_ID(), fa.GetVAB_AccountBook_ID(),
                fa.GetAccount_ID(), fa.GetVAB_SubAcct_ID(),
                fa.GetVAM_Product_ID(), fa.GetVAB_BusinessPartner_ID(), fa.GetVAF_OrgTrx_ID(),
                fa.GetC_LocFrom_ID(), fa.GetC_LocTo_ID(), fa.GetVAB_SalesRegionState_ID(),
                fa.GetVAB_Project_ID(), fa.GetVAB_Promotion_ID(), fa.GetVAB_BillingCode_ID(),
                fa.GetUser1_ID(), fa.GetUser2_ID(), fa.GetUserElement1_ID(), fa.GetUserElement2_ID());
            return acct;
        }

        /// <summary>
        /// Factory: default combination
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAB_AccountBook_ID">accounting schema</param>
        /// <param name="optionalNull">if true the optional values are null</param>
        /// <param name="trxName"></param>
        /// <returns>Account</returns>
        public static MVABAccount GetDefault(Ctx ctx, int VAB_AccountBook_ID,
            bool optionalNull, Trx trxName)
        {
            MVABAccountBook acctSchema = new MVABAccountBook(ctx, VAB_AccountBook_ID, trxName);
            return GetDefault(acctSchema, optionalNull);
        }

        /// <summary>
        /// Factory: default combination
        /// </summary>
        /// <param name="acctSchema">accounting schema</param>
        /// <param name="optionalNull">if true, the optional values are null</param>
        /// <returns>Account</returns>
        public static MVABAccount GetDefault(MVABAccountBook acctSchema, bool optionalNull)
        {
            MVABAccount vc = new MVABAccount(acctSchema);
            //  Active Elements
            MVABAccountBookElement[] elements = acctSchema.GetAcctSchemaElements();
            for (int i = 0; i < elements.Length; i++)
            {
                MVABAccountBookElement ase = elements[i];
                String elementType = ase.GetElementType();
                int defaultValue = ase.GetDefaultValue();
                bool setValue = ase.IsMandatory() || (!ase.IsMandatory() && !optionalNull);
                //
                if (elementType.Equals(MVABAccountBookElement.ELEMENTTYPE_Organization))
                    vc.SetVAF_Org_ID(defaultValue);
                else if (elementType.Equals(MVABAccountBookElement.ELEMENTTYPE_Account))
                    vc.SetAccount_ID(defaultValue);
                else if (elementType.Equals(MVABAccountBookElement.ELEMENTTYPE_SubAccount) && setValue)
                    vc.SetVAB_SubAcct_ID(defaultValue);
                else if (elementType.Equals(MVABAccountBookElement.ELEMENTTYPE_BPartner) && setValue)
                    vc.SetVAB_BusinessPartner_ID(defaultValue);
                else if (elementType.Equals(MVABAccountBookElement.ELEMENTTYPE_Product) && setValue)
                    vc.SetVAM_Product_ID(defaultValue);
                else if (elementType.Equals(MVABAccountBookElement.ELEMENTTYPE_Activity) && setValue)
                    vc.SetVAB_BillingCode_ID(defaultValue);
                else if (elementType.Equals(MVABAccountBookElement.ELEMENTTYPE_LocationFrom) && setValue)
                    vc.SetC_LocFrom_ID(defaultValue);
                else if (elementType.Equals(MVABAccountBookElement.ELEMENTTYPE_LocationTo) && setValue)
                    vc.SetC_LocTo_ID(defaultValue);
                else if (elementType.Equals(MVABAccountBookElement.ELEMENTTYPE_Campaign) && setValue)
                    vc.SetVAB_Promotion_ID(defaultValue);
                else if (elementType.Equals(MVABAccountBookElement.ELEMENTTYPE_OrgTrx) && setValue)
                    vc.SetVAF_OrgTrx_ID(defaultValue);
                else if (elementType.Equals(MVABAccountBookElement.ELEMENTTYPE_Project) && setValue)
                    vc.SetVAB_Project_ID(defaultValue);
                else if (elementType.Equals(MVABAccountBookElement.ELEMENTTYPE_SalesRegion) && setValue)
                    vc.SetVAB_SalesRegionState_ID(defaultValue);
                else if (elementType.Equals(MVABAccountBookElement.ELEMENTTYPE_UserList1) && setValue)
                    vc.SetUser1_ID(defaultValue);
                else if (elementType.Equals(MVABAccountBookElement.ELEMENTTYPE_UserList2) && setValue)
                    vc.SetUser2_ID(defaultValue);
                else if (elementType.Equals(MVABAccountBookElement.ELEMENTTYPE_UserElement1) && setValue)
                    vc.SetUserElement1_ID(defaultValue);
                else if (elementType.Equals(MVABAccountBookElement.ELEMENTTYPE_UserElement2) && setValue)
                    vc.SetUserElement2_ID(defaultValue);
            }
            _log.Fine("Client_ID=" + vc.GetVAF_Client_ID() + ", Org_ID=" + vc.GetVAF_Org_ID()
                + " - AcctSchema_ID=" + vc.GetVAB_AccountBook_ID() + ", Account_ID=" + vc.GetAccount_ID());
            return vc;
        }

        /// <summary>
        /// Get Account
        /// </summary>
        /// <param name="ctx"> context</param>
        /// <param name="VAB_Acct_ValidParameter_ID">combination</param>
        /// <returns>Account</returns>
        public static MVABAccount Get(Ctx ctx, int VAB_Acct_ValidParameter_ID)
        {
            //	Maybe later cache
            return new MVABAccount(ctx, VAB_Acct_ValidParameter_ID, null);
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
            String sql = "SELECT * FROM VAB_Acct_ValidParameter";
            if (where != null && where.Length > 0)
                sql += " WHERE " + where;
            sql += " ORDER BY VAB_Acct_ValidParameter_ID";
            int count = 0;
            int errors = 0;
            DataSet ds = new DataSet();
            try
            {
                ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    MVABAccount account = new MVABAccount(ctx, rs, trxName);
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
        /// <param name="VAB_Acct_ValidParameter_ID"></param>
        /// <param name="trxName"></param>
        public MVABAccount(Ctx ctx, int VAB_Acct_ValidParameter_ID, Trx trxName)
            : base(ctx, VAB_Acct_ValidParameter_ID, trxName)
        {

            if (VAB_Acct_ValidParameter_ID == 0)
            {
                //	setAccount_ID (0);
                //	setVAB_AccountBook_ID (0);
                SetIsFullyQualified(false);
            }
        }

        /// <summary>
        /// Load constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rs"></param>
        /// <param name="trxName"></param>
        public MVABAccount(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="as1">as account schema</param>
        public MVABAccount(MVABAccountBook as1)
            : this(as1.GetCtx(), 0, as1.Get_TrxName())
        {
            SetClientOrg(as1);
            SetVAB_AccountBook_ID(as1.GetVAB_AccountBook_ID());
        }

        /// <summary>
        /// Return String representation
        /// </summary>
        /// <returns>string</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVABAccount=[");
            sb.Append(GetVAB_Acct_ValidParameter_ID());
            if (GetCombination() != null)
                sb.Append(",").Append(GetCombination());
            else
            {
                //	.Append(",Client=").Append(getVAF_Client_ID())
                sb.Append(",Schema=").Append(GetVAB_AccountBook_ID())
                    .Append(",Org=").Append(GetVAF_Org_ID())
                    .Append(",Acct=").Append(GetAccount_ID())
                    .Append(" ");
                if (GetVAB_SubAcct_ID() != 0)
                    sb.Append(",VAB_SubAcct_ID=").Append(GetVAB_SubAcct_ID());
                if (GetVAM_Product_ID() != 0)
                    sb.Append(",VAM_Product_ID=").Append(GetVAM_Product_ID());
                if (GetVAB_BusinessPartner_ID() != 0)
                    sb.Append(",VAB_BusinessPartner_ID=").Append(GetVAB_BusinessPartner_ID());
                if (GetVAF_OrgTrx_ID() != 0)
                    sb.Append(",VAF_OrgTrx_ID=").Append(GetVAF_OrgTrx_ID());
                if (GetC_LocFrom_ID() != 0)
                    sb.Append(",C_LocFrom_ID=").Append(GetC_LocFrom_ID());
                if (GetC_LocTo_ID() != 0)
                    sb.Append(",C_LocTo_ID=").Append(GetC_LocTo_ID());
                if (GetVAB_SalesRegionState_ID() != 0)
                    sb.Append(",VAB_SalesRegionState_ID=").Append(GetVAB_SalesRegionState_ID());
                if (GetVAB_Project_ID() != 0)
                    sb.Append(",VAB_Project_ID=").Append(GetVAB_Project_ID());
                if (GetVAB_Promotion_ID() != 0)
                    sb.Append(",VAB_Promotion_ID=").Append(GetVAB_Promotion_ID());
                if (GetVAB_BillingCode_ID() != 0)
                    sb.Append(",VAB_BillingCode_ID=").Append(GetVAB_BillingCode_ID());
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
        public MVABAcctElement GetAccount()
        {
            if (_accountEV == null)
            {
                if (GetAccount_ID() != 0)
                    _accountEV = new MVABAcctElement(GetCtx(), GetAccount_ID(), Get_TrxName());
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
            return (MVABAcctElement.ACCOUNTTYPE_Asset.Equals(accountType)
                || MVABAcctElement.ACCOUNTTYPE_Liability.Equals(accountType)
                || MVABAcctElement.ACCOUNTTYPE_OwnerSEquity.Equals(accountType));
        }

        /// <summary>
        /// Is this an Activa Account
        /// </summary>
        /// <returns>bool</returns>
        public bool IsActiva()
        {
            return MVABAcctElement.ACCOUNTTYPE_Asset.Equals(GetAccountType());
        }

        /// <summary>
        /// Is this a Passiva Account
        /// </summary>
        /// <returns>bool</returns>
        public bool IsPassiva()
        {
            String accountType = GetAccountType();
            return (MVABAcctElement.ACCOUNTTYPE_Liability.Equals(accountType)
                || MVABAcctElement.ACCOUNTTYPE_OwnerSEquity.Equals(accountType));
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
            MVABAccountBook as1 = new MVABAccountBook(GetCtx(), GetVAB_AccountBook_ID(), Get_TrxName());	//	In Trx!
            MVABAccountBookElement[] elements = MVABAccountBookElement.GetAcctSchemaElements(as1);
            for (int i = 0; i < elements.Length; i++)
            {
                if (i > 0)
                {
                    combi.Append(as1.GetSeparator());
                    descr.Append(as1.GetSeparator());
                }
                MVABAccountBookElement element = elements[i];
                String combiStr = "_";		//	not defined
                String descrStr = "_";

                if (MVABAccountBookElement.ELEMENTTYPE_Organization.Equals(element.GetElementType()))
                {
                    if (GetVAF_Org_ID() != 0)
                    {
                        MVAFOrg org = new MVAFOrg(GetCtx(), GetVAF_Org_ID(), Get_TrxName());	//	in Trx!
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
                else if (MVABAccountBookElement.ELEMENTTYPE_Account.Equals(element.GetElementType()))
                {
                    if (GetAccount_ID() != 0)
                    {
                        if (_accountEV == null)
                            _accountEV = new MVABAcctElement(GetCtx(), GetAccount_ID(), Get_TrxName());
                        combiStr = _accountEV.GetValue();
                        descrStr = _accountEV.GetName();
                    }
                    else if (element.IsMandatory())
                    {
                        //log.warning("Mandatory Element missing: Account");
                        fullyQualified = false;
                    }
                }
                else if (MVABAccountBookElement.ELEMENTTYPE_SubAccount.Equals(element.GetElementType()))
                {
                    if (GetVAB_SubAcct_ID() != 0)
                    {
                        X_VAB_SubAcct sa = new X_VAB_SubAcct(GetCtx(), GetVAB_SubAcct_ID(), Get_TrxName());
                        combiStr = sa.GetValue();
                        descrStr = sa.GetName();
                    }
                }
                else if (MVABAccountBookElement.ELEMENTTYPE_Product.Equals(element.GetElementType()))
                {
                    if (GetVAM_Product_ID() != 0)
                    {
                        X_VAM_Product product = new X_VAM_Product(GetCtx(), GetVAM_Product_ID(), Get_TrxName());
                        combiStr = product.GetValue();
                        descrStr = product.GetName();
                    }
                    else if (element.IsMandatory())
                    {
                        //log.warning("Mandatory Element missing: Product");
                        fullyQualified = false;
                    }
                }
                else if (MVABAccountBookElement.ELEMENTTYPE_BPartner.Equals(element.GetElementType()))
                {
                    if (GetVAB_BusinessPartner_ID() != 0)
                    {
                        X_VAB_BusinessPartner partner = new X_VAB_BusinessPartner(GetCtx(), GetVAB_BusinessPartner_ID(), Get_TrxName());
                        combiStr = partner.GetValue();
                        descrStr = partner.GetName();
                    }
                    else if (element.IsMandatory())
                    {
                        //log.warning("Mandatory Element missing: Business Partner");
                        fullyQualified = false;
                    }
                }
                else if (MVABAccountBookElement.ELEMENTTYPE_OrgTrx.Equals(element.GetElementType()))
                {
                    if (GetVAF_OrgTrx_ID() != 0)
                    {
                        MVAFOrg org = new MVAFOrg(GetCtx(), GetVAF_OrgTrx_ID(), Get_TrxName());	// in Trx!
                        combiStr = org.GetValue();
                        descrStr = org.GetName();
                    }
                    else if (element.IsMandatory())
                    {
                        //log.warning("Mandatory Element missing: Trx Org");
                        fullyQualified = false;
                    }
                }
                else if (MVABAccountBookElement.ELEMENTTYPE_LocationFrom.Equals(element.GetElementType()))
                {
                    if (GetC_LocFrom_ID() != 0)
                    {
                        MVABAddress loc = new MVABAddress(GetCtx(), GetC_LocFrom_ID(), Get_TrxName());	//	in Trx!
                        combiStr = loc.GetPostal();
                        descrStr = loc.GetCity();
                    }
                    else if (element.IsMandatory())
                    {
                        //log.warning("Mandatory Element missing: Location From");
                        fullyQualified = false;
                    }
                }
                else if (MVABAccountBookElement.ELEMENTTYPE_LocationTo.Equals(element.GetElementType()))
                {
                    if (GetC_LocTo_ID() != 0)
                    {
                        MVABAddress loc = new MVABAddress(GetCtx(), GetC_LocFrom_ID(), Get_TrxName());	//	in Trx!
                        combiStr = loc.GetPostal();
                        descrStr = loc.GetCity();
                    }
                    else if (element.IsMandatory())
                    {
                        //log.warning("Mandatory Element missing: Location To");
                        fullyQualified = false;
                    }
                }
                else if (MVABAccountBookElement.ELEMENTTYPE_SalesRegion.Equals(element.GetElementType()))
                {
                    if (GetVAB_SalesRegionState_ID() != 0)
                    {
                        MVABSalesRegionState loc = new MVABSalesRegionState(GetCtx(), GetVAB_SalesRegionState_ID(), Get_TrxName());
                        combiStr = loc.GetValue();
                        descrStr = loc.GetName();
                    }
                    else if (element.IsMandatory())
                    {
                        //log.warning("Mandatory Element missing: SalesRegion");
                        fullyQualified = false;
                    }
                }
                else if (MVABAccountBookElement.ELEMENTTYPE_Project.Equals(element.GetElementType()))
                {
                    if (GetVAB_Project_ID() != 0)
                    {
                        X_VAB_Project project = new X_VAB_Project(GetCtx(), GetVAB_Project_ID(), Get_TrxName());
                        combiStr = project.GetValue();
                        descrStr = project.GetName();
                    }
                    else if (element.IsMandatory())
                    {
                        //log.warning("Mandatory Element missing: Project");
                        fullyQualified = false;
                    }
                }
                else if (MVABAccountBookElement.ELEMENTTYPE_Campaign.Equals(element.GetElementType()))
                {
                    if (GetVAB_Promotion_ID() != 0)
                    {
                        X_VAB_Promotion campaign = new X_VAB_Promotion(GetCtx(), GetVAB_Promotion_ID(), Get_TrxName());
                        combiStr = campaign.GetValue();
                        descrStr = campaign.GetName();
                    }
                    else if (element.IsMandatory())
                    {
                        //log.warning("Mandatory Element missing: Campaign");
                        fullyQualified = false;
                    }
                }
                else if (MVABAccountBookElement.ELEMENTTYPE_Activity.Equals(element.GetElementType()))
                {
                    if (GetVAB_BillingCode_ID() != 0)
                    {
                        X_VAB_BillingCode act = new X_VAB_BillingCode(GetCtx(), GetVAB_BillingCode_ID(), Get_TrxName());
                        combiStr = act.GetValue();
                        descrStr = act.GetName();
                    }
                    else if (element.IsMandatory())
                    {
                        //log.warning("Mandatory Element missing: Campaign");
                        fullyQualified = false;
                    }
                }
                else if (MVABAccountBookElement.ELEMENTTYPE_UserList1.Equals(element.GetElementType()))
                {
                    if (GetUser1_ID() != 0)
                    {
                        MVABAcctElement ev = new MVABAcctElement(GetCtx(), GetUser1_ID(), Get_TrxName());
                        combiStr = ev.GetValue();
                        descrStr = ev.GetName();
                    }
                }
                else if (MVABAccountBookElement.ELEMENTTYPE_UserList2.Equals(element.GetElementType()))
                {
                    if (GetUser2_ID() != 0)
                    {
                        MVABAcctElement ev = new MVABAcctElement(GetCtx(), GetUser2_ID(), Get_TrxName());
                        combiStr = ev.GetValue();
                        descrStr = ev.GetName();
                    }
                }
                else if (MVABAccountBookElement.ELEMENTTYPE_UserElement1.Equals(element.GetElementType()))
                {
                    if (GetUserElement1_ID() != 0 && element.GetVAF_Column_ID() > 0)
                    {
                        string qry = "SELECT tab.TableName, tab.VAF_TableView_ID, col.ColumnName FROM VAF_Column col INNER JOIN VAF_TableView tab ON col.VAF_TableView_ID = tab.VAF_TableView_ID WHERE VAF_Column_ID = " + element.GetVAF_Column_ID();
                        DataSet ds = DB.ExecuteDataset(qry);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            string tableName = Util.GetValueOfString(ds.Tables[0].Rows[0]["TableName"]);
                            int tableID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAF_TableView_ID"]);
                            string columnName = Util.GetValueOfString(ds.Tables[0].Rows[0]["ColumnName"]);
                            qry = "SELECT Count(*) FROM VAF_Column WHERE VAF_TableView_ID =" + tableID + " AND ColumnName='Value'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Value FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement1_ID();
                                combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                combiStr = Util.GetValueOfString(GetUserElement1_ID());
                            }
                            qry = "SELECT Count(*) FROM VAF_Column WHERE VAF_TableView_ID =" + tableID + " AND ColumnName='Name'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Name FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement1_ID();
                                descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                descrStr = Util.GetValueOfString(GetUserElement1_ID());
                            }
                        }
                    }
                }
                else if (MVABAccountBookElement.ELEMENTTYPE_UserElement2.Equals(element.GetElementType()))
                {
                    if (GetUserElement2_ID() != 0 && element.GetVAF_Column_ID() > 0)
                    {
                        string qry = "SELECT tab.TableName, tab.VAF_TableView_ID, col.ColumnName FROM VAF_Column col INNER JOIN VAF_TableView tab ON col.VAF_TableView_ID = tab.VAF_TableView_ID WHERE VAF_Column_ID = " + element.GetVAF_Column_ID();
                        DataSet ds = DB.ExecuteDataset(qry);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            string tableName = Util.GetValueOfString(ds.Tables[0].Rows[0]["TableName"]);
                            int tableID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAF_TableView_ID"]);
                            string columnName = Util.GetValueOfString(ds.Tables[0].Rows[0]["ColumnName"]);
                            qry = "SELECT Count(*) FROM VAF_Column WHERE VAF_TableView_ID =" + tableID + " AND ColumnName='Value'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Value FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement2_ID();
                                combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                combiStr = Util.GetValueOfString(GetUserElement2_ID());
                            }
                            qry = "SELECT Count(*) FROM VAF_Column WHERE VAF_TableView_ID =" + tableID + " AND ColumnName='Name'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Name FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement2_ID();
                                descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                descrStr = Util.GetValueOfString(GetUserElement2_ID());
                            }
                        }
                    }
                }
                else if (MVABAccountBookElement.ELEMENTTYPE_UserElement3.Equals(element.GetElementType()))
                {
                    if (GetUserElement3_ID() != 0 && element.GetVAF_Column_ID() > 0)
                    {
                        string qry = "SELECT tab.TableName, tab.VAF_TableView_ID, col.ColumnName FROM VAF_Column col INNER JOIN VAF_TableView tab ON col.VAF_TableView_ID = tab.VAF_TableView_ID WHERE VAF_Column_ID = " + element.GetVAF_Column_ID();
                        DataSet ds = DB.ExecuteDataset(qry);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            string tableName = Util.GetValueOfString(ds.Tables[0].Rows[0]["TableName"]);
                            int tableID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAF_TableView_ID"]);
                            string columnName = Util.GetValueOfString(ds.Tables[0].Rows[0]["ColumnName"]);
                            qry = "SELECT Count(*) FROM VAF_Column WHERE VAF_TableView_ID =" + tableID + " AND ColumnName='Value'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Value FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement3_ID();
                                combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                combiStr = Util.GetValueOfString(GetUserElement3_ID());
                            }
                            qry = "SELECT Count(*) FROM VAF_Column WHERE VAF_TableView_ID =" + tableID + " AND ColumnName='Name'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Name FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement3_ID();
                                descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                descrStr = Util.GetValueOfString(GetUserElement3_ID());
                            }
                        }
                    }
                }
                else if (MVABAccountBookElement.ELEMENTTYPE_UserElement4.Equals(element.GetElementType()))
                {
                    if (GetUserElement4_ID() != 0 && element.GetVAF_Column_ID() > 0)
                    {
                        string qry = "SELECT tab.TableName, tab.VAF_TableView_ID, col.ColumnName FROM VAF_Column col INNER JOIN VAF_TableView tab ON col.VAF_TableView_ID = tab.VAF_TableView_ID WHERE VAF_Column_ID = " + element.GetVAF_Column_ID();
                        DataSet ds = DB.ExecuteDataset(qry);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            string tableName = Util.GetValueOfString(ds.Tables[0].Rows[0]["TableName"]);
                            int tableID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAF_TableView_ID"]);
                            string columnName = Util.GetValueOfString(ds.Tables[0].Rows[0]["ColumnName"]);
                            qry = "SELECT Count(*) FROM VAF_Column WHERE VAF_TableView_ID =" + tableID + " AND ColumnName='Value'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Value FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement4_ID();
                                combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                combiStr = Util.GetValueOfString(GetUserElement4_ID());
                            }
                            qry = "SELECT Count(*) FROM VAF_Column WHERE VAF_TableView_ID =" + tableID + " AND ColumnName='Name'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Name FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement4_ID();
                                descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                descrStr = Util.GetValueOfString(GetUserElement4_ID());
                            }
                        }
                    }
                }
                else if (MVABAccountBookElement.ELEMENTTYPE_UserElement5.Equals(element.GetElementType()))
                {
                    if (GetUserElement5_ID() != 0 && element.GetVAF_Column_ID() > 0)
                    {
                        string qry = "SELECT tab.TableName, tab.VAF_TableView_ID, col.ColumnName FROM VAF_Column col INNER JOIN VAF_TableView tab ON col.VAF_TableView_ID = tab.VAF_TableView_ID WHERE VAF_Column_ID = " + element.GetVAF_Column_ID();
                        DataSet ds = DB.ExecuteDataset(qry);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            string tableName = Util.GetValueOfString(ds.Tables[0].Rows[0]["TableName"]);
                            int tableID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAF_TableView_ID"]);
                            string columnName = Util.GetValueOfString(ds.Tables[0].Rows[0]["ColumnName"]);
                            qry = "SELECT Count(*) FROM VAF_Column WHERE VAF_TableView_ID =" + tableID + " AND ColumnName='Value'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Value FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement5_ID();
                                combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                combiStr = Util.GetValueOfString(GetUserElement5_ID());
                            }
                            qry = "SELECT Count(*) FROM VAF_Column WHERE VAF_TableView_ID =" + tableID + " AND ColumnName='Name'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Name FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement5_ID();
                                descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                descrStr = Util.GetValueOfString(GetUserElement5_ID());
                            }
                        }
                    }
                }
                else if (MVABAccountBookElement.ELEMENTTYPE_UserElement6.Equals(element.GetElementType()))
                {
                    if (GetUserElement6_ID() != 0 && element.GetVAF_Column_ID() > 0)
                    {
                        string qry = "SELECT tab.TableName, tab.VAF_TableView_ID, col.ColumnName FROM VAF_Column col INNER JOIN VAF_TableView tab ON col.VAF_TableView_ID = tab.VAF_TableView_ID WHERE VAF_Column_ID = " + element.GetVAF_Column_ID();
                        DataSet ds = DB.ExecuteDataset(qry);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            string tableName = Util.GetValueOfString(ds.Tables[0].Rows[0]["TableName"]);
                            int tableID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAF_TableView_ID"]);
                            string columnName = Util.GetValueOfString(ds.Tables[0].Rows[0]["ColumnName"]);
                            qry = "SELECT Count(*) FROM VAF_Column WHERE VAF_TableView_ID =" + tableID + " AND ColumnName='Value'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Value FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement6_ID();
                                combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                combiStr = Util.GetValueOfString(GetUserElement6_ID());
                            }
                            qry = "SELECT Count(*) FROM VAF_Column WHERE VAF_TableView_ID =" + tableID + " AND ColumnName='Name'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Name FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement6_ID();
                                descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                descrStr = Util.GetValueOfString(GetUserElement6_ID());
                            }
                        }
                    }
                }
                else if (MVABAccountBookElement.ELEMENTTYPE_UserElement7.Equals(element.GetElementType()))
                {
                    if (GetUserElement7_ID() != 0 && element.GetVAF_Column_ID() > 0)
                    {
                        string qry = "SELECT tab.TableName, tab.VAF_TableView_ID, col.ColumnName FROM VAF_Column col INNER JOIN VAF_TableView tab ON col.VAF_TableView_ID = tab.VAF_TableView_ID WHERE VAF_Column_ID = " + element.GetVAF_Column_ID();
                        DataSet ds = DB.ExecuteDataset(qry);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            string tableName = Util.GetValueOfString(ds.Tables[0].Rows[0]["TableName"]);
                            int tableID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAF_TableView_ID"]);
                            string columnName = Util.GetValueOfString(ds.Tables[0].Rows[0]["ColumnName"]);
                            qry = "SELECT Count(*) FROM VAF_Column WHERE VAF_TableView_ID =" + tableID + " AND ColumnName='Value'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Value FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement7_ID();
                                combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                combiStr = Util.GetValueOfString(GetUserElement7_ID());
                            }
                            qry = "SELECT Count(*) FROM VAF_Column WHERE VAF_TableView_ID =" + tableID + " AND ColumnName='Name'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Name FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement7_ID();
                                descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                descrStr = Util.GetValueOfString(GetUserElement7_ID());
                            }
                        }
                    }
                }
                else if (MVABAccountBookElement.ELEMENTTYPE_UserElement8.Equals(element.GetElementType()))
                {
                    if (GetUserElement8_ID() != 0 && element.GetVAF_Column_ID() > 0)
                    {
                        string qry = "SELECT tab.TableName, tab.VAF_TableView_ID, col.ColumnName FROM VAF_Column col INNER JOIN VAF_TableView tab ON col.VAF_TableView_ID = tab.VAF_TableView_ID WHERE VAF_Column_ID = " + element.GetVAF_Column_ID();
                        DataSet ds = DB.ExecuteDataset(qry);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            string tableName = Util.GetValueOfString(ds.Tables[0].Rows[0]["TableName"]);
                            int tableID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAF_TableView_ID"]);
                            string columnName = Util.GetValueOfString(ds.Tables[0].Rows[0]["ColumnName"]);
                            qry = "SELECT Count(*) FROM VAF_Column WHERE VAF_TableView_ID =" + tableID + " AND ColumnName='Value'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Value FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement8_ID();
                                combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                combiStr = Util.GetValueOfString(GetUserElement8_ID());
                            }
                            qry = "SELECT Count(*) FROM VAF_Column WHERE VAF_TableView_ID =" + tableID + " AND ColumnName='Name'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Name FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement8_ID();
                                descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                descrStr = Util.GetValueOfString(GetUserElement8_ID());
                            }
                        }
                    }
                }
                else if (MVABAccountBookElement.ELEMENTTYPE_UserElement9.Equals(element.GetElementType()))
                {
                    if (GetUserElement9_ID() != 0 && element.GetVAF_Column_ID() > 0)
                    {
                        string qry = "SELECT tab.TableName, tab.VAF_TableView_ID, col.ColumnName FROM VAF_Column col INNER JOIN VAF_TableView tab ON col.VAF_TableView_ID = tab.VAF_TableView_ID WHERE VAF_Column_ID = " + element.GetVAF_Column_ID();
                        DataSet ds = DB.ExecuteDataset(qry);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            string tableName = Util.GetValueOfString(ds.Tables[0].Rows[0]["TableName"]);
                            int tableID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAF_TableView_ID"]);
                            string columnName = Util.GetValueOfString(ds.Tables[0].Rows[0]["ColumnName"]);
                            qry = "SELECT Count(*) FROM VAF_Column WHERE VAF_TableView_ID =" + tableID + " AND ColumnName='Value'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Value FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement9_ID();
                                combiStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                combiStr = Util.GetValueOfString(GetUserElement9_ID());
                            }
                            qry = "SELECT Count(*) FROM VAF_Column WHERE VAF_TableView_ID =" + tableID + " AND ColumnName='Name'";
                            if (Util.GetValueOfInt(DB.ExecuteScalar(qry)) > 0)
                            {
                                qry = "SELECT Name FROM " + tableName + " WHERE " + tableName + "_ID =" + GetUserElement9_ID();
                                descrStr = Util.GetValueOfString(DB.ExecuteScalar(qry));
                            }
                            else
                            {
                                descrStr = Util.GetValueOfString(GetUserElement9_ID());
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
            if (GetVAB_SubAcct_ID() != 0)
            {
                X_VAB_SubAcct sa = new X_VAB_SubAcct(GetCtx(), GetVAB_SubAcct_ID(), Get_TrxName());
                if (sa.GetVAB_Acct_Element_ID() != GetAccount_ID())
                {
                    log.SaveError("Error", "VAB_SubAcct.VAB_Acct_Element_ID=" + sa.GetVAB_Acct_Element_ID()
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
