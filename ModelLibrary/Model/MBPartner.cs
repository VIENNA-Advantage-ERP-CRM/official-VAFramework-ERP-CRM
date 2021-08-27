/********************************************************
 * Module Name    : VFramwork
 * Purpose        : Business Partner Model
 * Class Used     : X_C_BPartner
 * Chronological Development
 * Veena Pandey     02-June-2009
 * Raghunandan      24-june-2009
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
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    /// <summary>
    /// Business Partner Model
    /// </summary>
    public class MBPartner : X_C_BPartner
    {
        #region private Variables
        // Users						
        private MUser[] _contacts = null;
        //Addressed						
        private MBPartnerLocation[] _locations = null;
        // BP Bank Accounts				
        private MBPBankAccount[] _accounts = null;
        // Prim Address					
        private int? _primaryC_BPartner_Location_ID = null;
        // Prim User						
        private int? _primaryAD_User_ID = null;
        // Credit Limit recently calcualted		
        private bool _TotalOpenBalanceSet = false;
        // BP Group						
        private MBPGroup _group = null;
        private static VLogger _log = VLogger.GetVLogger(typeof(MBPartner).FullName);
        #endregion

        /// <summary>
        /// Get Empty Template Business Partner
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Client_ID">client</param>
        /// <returns>Template Business Partner or null</returns>
        public static MBPartner GetTemplate(Ctx ctx, int AD_Client_ID)
        {
            MBPartner template = GetBPartnerCashTrx(ctx, AD_Client_ID);
            if (template == null)
                template = new MBPartner(ctx, 0, null);
            //	Reset
            if (template != null)
            {
                template.Set_ValueNoCheck("C_BPartner_ID", 0);
                template.SetValue("");
                template.SetName("");
                template.SetName2(null);
                template.SetDUNS("");
                template.SetFirstSale(null);
                //
                template.SetSO_CreditLimit(Env.ZERO);
                template.SetSO_CreditUsed(Env.ZERO);
                template.SetTotalOpenBalance(Env.ZERO);
                //	s_template.setRating(null);
                //
                template.SetActualLifeTimeValue(Env.ZERO);
                template.SetPotentialLifeTimeValue(Env.ZERO);
                template.SetAcqusitionCost(Env.ZERO);
                template.SetShareOfCustomer(0);
                template.SetSalesVolume(0);
            }
            return template;
        }

        /// <summary>
        /// Get Cash Trx Business Partner
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Client_ID">client</param>
        /// <returns>Cash Trx Business Partner or null</returns>
        public static MBPartner GetBPartnerCashTrx(Ctx ctx, int AD_Client_ID)
        {
            MBPartner retValue = null;
            String sql = "SELECT * FROM C_BPartner "
                + " WHERE C_BPartner_ID IN (SELECT C_BPartnerCashTrx_ID FROM AD_ClientInfo" +
                " WHERE AD_Client_ID=" + AD_Client_ID + ")";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MBPartner(ctx, dr, null);
                }
                if (dt == null)
                {
                    _log.Log(Level.SEVERE, "Not found for AD_Client_ID=" + AD_Client_ID);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }
            return retValue;
        }

        /// <summary>
        /// Get BPartner with Value
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="Value">value</param>
        /// <returns>BPartner or null</returns>
        public static MBPartner Get(Ctx ctx, String Value)
        {
            if (Value == null || Value.Length == 0)
                return null;
            MBPartner retValue = null;
            int AD_Client_ID = ctx.GetAD_Client_ID();
            String sql = "SELECT * FROM C_BPartner WHERE Value=@Value"
                + " AND AD_Client_ID=" + AD_Client_ID;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@Value", Value);
                idr = DataBase.DB.ExecuteReader(sql, param, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MBPartner(ctx, dr, null);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }
            return retValue;
        }

        /// <summary>
        /// Get BPartner with Value
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_BPartner_ID">id</param>
        /// <returns>BPartner or null</returns>
        public static MBPartner Get(Ctx ctx, int C_BPartner_ID)
        {
            MBPartner retValue = null;
            int AD_Client_ID = ctx.GetAD_Client_ID();
            String sql = "SELECT * FROM C_BPartner WHERE C_BPartner_ID=" + C_BPartner_ID;
            //    + " AND  AD_Client_ID=" + AD_Client_ID;
            DataSet ds = new DataSet();
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    retValue = new MBPartner(ctx, dr, null);
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            return retValue;
        }

        /// <summary>
        /// Get Not Invoiced Shipment Value
        /// </summary>
        /// <param name="C_BPartner_ID">partner</param>
        /// <returns>value in accounting currency</returns>
        public static Decimal GetNotInvoicedAmt(int C_BPartner_ID)
        {
            Decimal retValue = new decimal();
            String sql = "SELECT SUM(COALESCE("
                + "CURRENCYBASEWITHCONVERSIONTYPE((ol.QtyDelivered-ol.QtyInvoiced)*ol.PriceActual,o.C_Currency_ID,o.DateOrdered, o.AD_Client_ID,o.AD_Org_ID, o.C_CONVERSIONTYPE_ID) ,0)) "
                + " FROM C_OrderLine ol"
                + " INNER JOIN C_Order o ON (ol.C_Order_ID=o.C_Order_ID) "
                + " WHERE o.IsSOTrx='Y' AND Bill_BPartner_ID=" + C_BPartner_ID;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                if (idr.Read())
                {
                    retValue = Utility.Util.GetValueOfDecimal(idr[0]);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            return retValue;
        }

        /// <summary>
        /// Constructor for new BPartner from Template
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="trxName">transaction</param>
        public MBPartner(Ctx ctx, Trx trxName)
            : this(ctx, -1, trxName)
        {

        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MBPartner(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_BPartner_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MBPartner(Ctx ctx, int C_BPartner_ID, Trx trxName)
            : base(ctx, C_BPartner_ID, trxName)
        {
            //
            if (C_BPartner_ID == -1)
            {
                InitTemplate(ctx.GetContextAsInt("AD_Client_ID"));
                C_BPartner_ID = 0;
            }
            if (C_BPartner_ID == 0)
            {
                //	setValue ("");
                //	setName ("");
                //	setName2 (null);
                //	setDUNS("");
                //
                SetIsCustomer(true);
                SetIsProspect(true);
                //
                SetSendEMail(false);
                SetIsOneTime(false);
                SetIsVendor(false);
                SetIsSummary(false);
                SetIsEmployee(false);
                SetIsSalesRep(false);
                SetIsTaxExempt(false);
                SetIsDiscountPrinted(false);
                //
                SetSO_CreditLimit(Env.ZERO);
                SetSO_CreditUsed(Env.ZERO);
                SetTotalOpenBalance(Env.ZERO);
                SetSOCreditStatus(SOCREDITSTATUS_NoCreditCheck);
                //
                SetFirstSale(null);
                SetActualLifeTimeValue(Env.ZERO);
                SetPotentialLifeTimeValue(Env.ZERO);
                SetAcqusitionCost(Env.ZERO);
                SetShareOfCustomer(0);
                SetSalesVolume(0);
            }
            log.Fine(ToString());
        }

        /// <summary>
        /// Import Contstructor
        /// </summary>
        /// <param name="impBP">import</param>
        public MBPartner(X_I_BPartner impBP)
            : this(impBP.GetCtx(), 0, impBP.Get_TrxName())
        {

            SetClientOrg(impBP);
            SetUpdatedBy(impBP.GetUpdatedBy());
            //
            String value = impBP.GetValue();
            if (value == null || value.Length == 0)
                value = impBP.GetEMail();
            if (value == null || value.Length == 0)
                value = impBP.GetContactName();
            SetValue(value);
            String name = impBP.GetName();
            if (name == null || name.Length == 0)
                name = impBP.GetContactName();
            if (name == null || name.Length == 0)
                name = impBP.GetEMail();
            SetName(name);
            SetName2(impBP.GetName2());
            SetDescription(impBP.GetDescription());
            //	setHelp(impBP.getHelp());
            SetDUNS(impBP.GetDUNS());
            SetTaxID(impBP.GetTaxID());
            SetNAICS(impBP.GetNAICS());
            SetC_BP_Group_ID(impBP.GetC_BP_Group_ID());
        }

        /// <summary>
        /// Load Default BPartner
        /// </summary>
        /// <param name="AD_Client_ID">client id</param>
        /// <returns>true if loaded</returns>
        private bool InitTemplate(int AD_Client_ID)
        {
            if (AD_Client_ID == 0)
                throw new ArgumentException("Client_ID=0");

            bool success = true;
            String sql = "SELECT * FROM C_BPartner "
                + " WHERE C_BPartner_ID=(SELECT C_BPartnerCashTrx_ID FROM AD_ClientInfo" +
                " WHERE AD_Client_ID=" + AD_Client_ID + ")";
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                if (ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                {
                    success = Load(ds.Tables[0].Rows[0]);
                }
                else
                {
                    Load(0, (Trx)null);
                    success = false;
                    log.Severe("None found");
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            SetStandardDefaults();
            //	Reset
            Set_ValueNoCheck("C_BPartner_ID", I_ZERO);
            SetValue("");
            SetName("");
            SetName2(null);
            return success;
        }

        /// <summary>
        /// Get All Contacts
        /// </summary>
        /// <param name="reload">if true users will be requeried</param>
        /// <returns>contacts</returns>
        public MUser[] GetContacts(bool reload)
        {
            if (reload || _contacts == null || _contacts.Length == 0)
            {
                ;
            }
            else
                return _contacts;
            //
            List<MUser> list = new List<MUser>();
            String sql = "SELECT * FROM AD_User WHERE C_BPartner_ID=" + GetC_BPartner_ID();
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MUser(GetCtx(), dr, null));
                }
                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            _contacts = new MUser[list.Count];
            _contacts = list.ToArray();
            return _contacts;
        }

        /// <summary>
        /// Get specified or first Contact
        /// </summary>
        /// <param name="AD_User_ID">optional user</param>
        /// <returns>contact or null</returns>
        public MUser GetContact(int AD_User_ID)
        {
            MUser[] users = GetContacts(false);
            if (users.Length == 0)
                return null;
            for (int i = 0; AD_User_ID != 0 && i < users.Length; i++)
            {
                if (users[i].GetAD_User_ID() == AD_User_ID)
                    return users[i];
            }
            return users[0];
        }

        /// <summary>
        /// Get All Locations
        /// </summary>
        /// <param name="reload">if true locations will be requeried</param>
        /// <returns>locations</returns>
        public MBPartnerLocation[] GetLocations(bool reload)
        {
            if (reload || _locations == null || _locations.Length == 0)
            {
                ;
            }
            else
            {
                return _locations;
            }
            //
            List<MBPartnerLocation> list = new List<MBPartnerLocation>();
            String sql = "SELECT * FROM C_BPartner_Location WHERE C_BPartner_ID=" + GetC_BPartner_ID();
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MBPartnerLocation(GetCtx(), dr, Get_TrxName()));
                }
                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            _locations = new MBPartnerLocation[list.Count];
            _locations = list.ToArray();
            return _locations;
        }

        /// <summary>
        /// Get explicit or first bill Location
        /// </summary>
        /// <param name="C_BPartner_Location_ID">optional explicit location</param>
        /// <returns>location or null</returns>
        public MBPartnerLocation GetLocation(int C_BPartner_Location_ID)
        {
            MBPartnerLocation[] locations = GetLocations(false);
            if (locations.Length == 0)
                return null;
            MBPartnerLocation retValue = null;
            for (int i = 0; i < locations.Length; i++)
            {
                if (locations[i].GetC_BPartner_Location_ID() == C_BPartner_Location_ID)
                    return locations[i];
                if (retValue == null && locations[i].IsBillTo())
                    retValue = locations[i];
            }
            if (retValue == null)
                return locations[0];
            return retValue;
        }

        /// <summary>
        /// Get Bank Accounts
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>Bank Accounts</returns>
        public MBPBankAccount[] GetBankAccounts(bool requery)
        {
            if (_accounts != null && _accounts.Length >= 0 && !requery)	//	re-load
                return _accounts;
            //
            List<MBPBankAccount> list = new List<MBPBankAccount>();
            String sql = "SELECT * FROM C_BP_BankAccount WHERE C_BPartner_ID=" + GetC_BPartner_ID()
                + " AND IsActive='Y'";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MBPBankAccount(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }

            _accounts = new MBPBankAccount[list.Count];
            _accounts = list.ToArray();
            return _accounts;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MBPartner[ID=")
                .Append(Get_ID())
                .Append(",Value=").Append(GetValue())
                .Append(",Name=").Append(GetName())
                .Append(",OpenBalance=").Append(GetTotalOpenBalance())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Set Client/Org
        /// </summary>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="AD_Org_ID">org</param>
        //public void SetClientOrg(int AD_Client_ID, int AD_Org_ID)
        //{
        //    base.SetClientOrg(AD_Client_ID, AD_Org_ID);
        //}

        /// <summary>
        /// Set Linked Organization.
        /// </summary>
        /// <param name="AD_OrgBP_ID">id</param>
        public void SetAD_OrgBP_ID(int AD_OrgBP_ID)
        {
            if (AD_OrgBP_ID == 0)
                base.SetAD_OrgBP_ID(null);
            else
                base.SetAD_OrgBP_ID(AD_OrgBP_ID.ToString());
        }

        /// <summary>
        /// Get Linked Organization.
        ///	(is Button)
        /// The Business Partner is another Organization 
        ///	for explicit Inter-Org transactions 
        /// </summary>
        /// <returns>AD_OrgBP_ID if BP</returns>
        public int GetAD_OrgBP_ID_Int()
        {
            String org = base.GetAD_OrgBP_ID();
            if (org == null)
                return 0;
            int AD_OrgBP_ID = 0;
            try
            {
                AD_OrgBP_ID = int.Parse(org);
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, org, ex);
            }
            return AD_OrgBP_ID;
        }

        /// <summary>
        /// Get Primary C_BPartner_Location_ID
        /// </summary>
        /// <returns>C_BPartner_Location_ID</returns>
        public int GetPrimaryC_BPartner_Location_ID()
        {
            if (_primaryC_BPartner_Location_ID == null)
            {
                MBPartnerLocation[] locs = GetLocations(false);
                for (int i = 0; _primaryC_BPartner_Location_ID == null && i < locs.Length; i++)
                {
                    if (locs[i].IsBillTo())
                    {
                        SetPrimaryC_BPartner_Location_ID(locs[i].GetC_BPartner_Location_ID());
                        break;
                    }
                }
                //	get first
                if (_primaryC_BPartner_Location_ID == null && locs.Length > 0)
                    SetPrimaryC_BPartner_Location_ID(locs[0].GetC_BPartner_Location_ID());
            }
            if (_primaryC_BPartner_Location_ID == null)
                return 0;
            return (int)_primaryC_BPartner_Location_ID;
        }

        /// <summary>
        /// Get Primary C_BPartner_Location
        /// </summary>
        /// <returns>C_BPartner_Location</returns>
        public MBPartnerLocation GetPrimaryC_BPartner_Location()
        {
            if (_primaryC_BPartner_Location_ID == null)
            {
                _primaryC_BPartner_Location_ID = GetPrimaryC_BPartner_Location_ID();
            }
            if (_primaryC_BPartner_Location_ID == null)
                return null;
            return new MBPartnerLocation(GetCtx(), (int)_primaryC_BPartner_Location_ID, null);
        }

        /// <summary>
        /// Get Primary AD_User_ID
        /// </summary>
        /// <returns>AD_User_ID</returns>
        public int GetPrimaryAD_User_ID()
        {
            if (_primaryAD_User_ID == null)
            {
                MUser[] users = GetContacts(false);
                //	for (int i = 0; i < users.Length; i++)
                //	{
                //	}
                if (_primaryAD_User_ID == null && users.Length > 0)
                    SetPrimaryAD_User_ID(users[0].GetAD_User_ID());
            }
            if (_primaryAD_User_ID == null)
                return 0;
            return (int)_primaryAD_User_ID;
        }

        /// <summary>
        /// Set Primary C_BPartner_Location_ID
        /// </summary>
        /// <param name="C_BPartner_Location_ID">id</param>
        public void SetPrimaryC_BPartner_Location_ID(int C_BPartner_Location_ID)
        {
            _primaryC_BPartner_Location_ID = C_BPartner_Location_ID;
        }

        /// <summary>
        /// Set Primary AD_User_ID
        /// </summary>
        /// <param name="AD_User_ID">id</param>
        public void SetPrimaryAD_User_ID(int AD_User_ID)
        {
            _primaryAD_User_ID = AD_User_ID;
        }

        /// <summary>
        /// Calculate Total Open Balance and SO_CreditUsed.
        /// (includes drafted invoices)
        /// </summary>
        public void SetTotalOpenBalance()
        {
            Decimal? SO_CreditUsed = null;
            Decimal? TotalOpenBalance = null;
            String sql = "SELECT "
                //	SO Credit Used	= SO Invoices
                + "COALESCE((SELECT SUM(CURRENCYBASEWITHCONVERSIONTYPE(i.GrandTotal,i.C_Currency_ID,i.DateOrdered, i.AD_Client_ID,i.AD_Org_ID, i.C_CONVERSIONTYPE_ID)) "
                    + " FROM C_Invoice_v i "
                    + " WHERE i.C_BPartner_ID=bp.C_BPartner_ID"
                    + " AND i.IsSOTrx='Y' AND i.DocStatus IN('CO','CL')),0) "
                //					- All SO Allocations
                + "-COALESCE((SELECT SUM(CURRENCYBASEWITHCONVERSIONTYPE(a.Amount+a.DiscountAmt+a.WriteoffAmt,i.C_Currency_ID,i.DateOrdered,a.AD_Client_ID,a.AD_Org_ID, i.C_CONVERSIONTYPE_ID)) "
                    + " FROM C_AllocationLine a INNER JOIN C_Invoice i ON (a.C_Invoice_ID=i.C_Invoice_ID) "
                    + " INNER JOIN C_AllocationHdr h ON (a.C_AllocationHdr_ID = h.C_AllocationHdr_ID) "
                    + " WHERE a.C_BPartner_ID=bp.C_BPartner_ID AND a.IsActive='Y'"
                    + " AND i.isSoTrx='Y' AND h.DocStatus IN('CO','CL')),0) "
                //					- Unallocated Receipts	= (All Receipts
                + "-(SELECT COALESCE(SUM(CURRENCYBASEWITHCONVERSIONTYPE(p.PayAmt+p.DiscountAmt+p.WriteoffAmt,p.C_Currency_ID,p.DateTrx,p.AD_Client_ID,p.AD_Org_ID, p.C_CONVERSIONTYPE_ID)),0) "
                    + " FROM C_Payment_v p "
                    + " WHERE p.C_BPartner_ID=bp.C_BPartner_ID"
                    + " AND p.IsReceipt='Y' AND p.DocStatus IN('CO','CL')"
                    + " AND p.C_Charge_ID IS NULL)"
                // JID_1224: Consider Cash Journal Transaction also to Get Total Open Balance of Business Partner
                //                  - Unallocated Cash Receipts
                + "-(SELECT COALESCE(SUM(CURRENCYBASEWITHCONVERSIONTYPE(cl.Amount,cl.C_Currency_ID,c.StatementDate, cl.AD_Client_ID,cl.AD_Org_ID, cl.C_CONVERSIONTYPE_ID)),0)"
                    + " FROM C_CASHLINE cl INNER JOIN C_Cash C ON C.C_Cash_ID = cl.C_Cash_ID WHERE cl.C_BPartner_ID=bp.C_BPartner_ID"
                    + " AND cl.VSS_PaymentType ='R' AND c.DocStatus IN('CO','CL') AND cl.C_Charge_ID IS NULL)"
                //											- All Receipt Allocations
                + "+(SELECT COALESCE(SUM(CURRENCYBASEWITHCONVERSIONTYPE(a.Amount+a.DiscountAmt+a.WriteoffAmt,i.C_Currency_ID,i.DateOrdered,a.AD_Client_ID,a.AD_Org_ID, i.C_CONVERSIONTYPE_ID)),0) "
                    + " FROM C_AllocationLine a INNER JOIN C_Invoice i ON (a.C_Invoice_ID=i.C_Invoice_ID) "
                    + " INNER JOIN C_AllocationHdr h ON (a.C_AllocationHdr_ID = h.C_AllocationHdr_ID) "
                    + " WHERE a.C_BPartner_ID=bp.C_BPartner_ID"
                    + " AND a.IsActive='Y' AND a.C_Payment_ID IS NOT NULL"
                    + " AND i.isSoTrx='Y' AND h.DocStatus IN('CO','CL')) "
                //                                          - All Cash Receipt Allocatioons
                + "+(SELECT COALESCE(SUM(CURRENCYBASEWITHCONVERSIONTYPE(a.Amount+a.DiscountAmt+a.WriteoffAmt,i.C_Currency_ID,i.DateOrdered,a.AD_Client_ID,a.AD_Org_ID, i.C_CONVERSIONTYPE_ID)),0)"
                    + " FROM C_AllocationLine a INNER JOIN C_Invoice i ON (a.C_Invoice_ID=i.C_Invoice_ID) INNER JOIN C_AllocationHdr h ON (a.C_AllocationHdr_ID = h.C_AllocationHdr_ID)"
                    + " WHERE a.C_BPartner_ID=bp.C_BPartner_ID AND A.IsActive ='Y' AND a.C_CashLine_ID IS NOT NULL AND i.isSoTrx ='Y' AND h.DocStatus IN('CO','CL')), "

                //	Balance			= All Invoices
                + "COALESCE((SELECT SUM(CURRENCYBASEWITHCONVERSIONTYPE(i.GrandTotal*MultiplierAP,i.C_Currency_ID,i.DateOrdered, i.AD_Client_ID,i.AD_Org_ID, i.C_CONVERSIONTYPE_ID)) "
                    + " FROM C_Invoice_v i "
                    + " WHERE i.C_BPartner_ID=bp.C_BPartner_ID"
                    + " AND i.DocStatus IN('CO','CL')),0) "
                //					- All Allocations
                + "-COALESCE((SELECT SUM(CURRENCYBASEWITHCONVERSIONTYPE(a.Amount+a.DiscountAmt+a.WriteoffAmt,i.C_Currency_ID,i.DateOrdered,a.AD_Client_ID,a.AD_Org_ID, i.C_CONVERSIONTYPE_ID)) "
                    + " FROM C_AllocationLine a INNER JOIN C_Invoice i ON (a.C_Invoice_ID=i.C_Invoice_ID) "
                    + " INNER JOIN C_AllocationHdr h ON (a.C_AllocationHdr_ID = h.C_AllocationHdr_ID) "
                    + " WHERE a.C_BPartner_ID=bp.C_BPartner_ID AND a.IsActive='Y' AND h.DocStatus IN('CO','CL')),0) "
                //					- Unallocated Receipts	= (All Receipts
                + "-(SELECT COALESCE(SUM(CURRENCYBASEWITHCONVERSIONTYPE(p.PayAmt+p.DiscountAmt+p.WriteoffAmt,p.C_Currency_ID,p.DateTrx,p.AD_Client_ID,p.AD_Org_ID, p.C_CONVERSIONTYPE_ID)),0) "
                    + " FROM C_Payment_v p "
                    + " WHERE p.C_BPartner_ID=bp.C_BPartner_ID"
                    + " AND p.DocStatus IN('CO','CL')"
                    + " AND p.C_Charge_ID IS NULL)"
                // JID_1224: Consider Cash Journal Transaction also to Get Total Open Balance of Business Partner
                //                  - Unallocated Cash Receipts
                + "-(SELECT COALESCE(SUM(CURRENCYBASEWITHCONVERSIONTYPE(cl.Amount,cl.C_Currency_ID,c.StatementDate, cl.AD_Client_ID,cl.AD_Org_ID, cl.C_CONVERSIONTYPE_ID)),0)"
                    + " FROM C_CASHLINE cl INNER JOIN C_Cash C ON C.C_Cash_ID = cl.C_Cash_ID WHERE cl.C_BPartner_ID=bp.C_BPartner_ID"
                    + " AND c.DocStatus IN('CO','CL') AND cl.C_Charge_ID IS NULL)"
                //											- All Allocations
                + "+(SELECT COALESCE(SUM(CURRENCYBASEWITHCONVERSIONTYPE(a.Amount+a.DiscountAmt+a.WriteoffAmt,i.C_Currency_ID,i.DateOrdered,a.AD_Client_ID,a.AD_Org_ID, i.C_CONVERSIONTYPE_ID)),0) "
                    + " FROM C_AllocationLine a INNER JOIN C_Invoice i ON (a.C_Invoice_ID=i.C_Invoice_ID) "
                    + " INNER JOIN C_AllocationHdr h ON (a.C_AllocationHdr_ID = h.C_AllocationHdr_ID) "
                    + " WHERE a.C_BPartner_ID=bp.C_BPartner_ID"
                    + " AND a.IsActive='Y' AND a.C_Payment_ID IS NOT NULL AND h.DocStatus IN('CO','CL')) "
                //											- All Cash Allocations
                + "+(SELECT COALESCE(SUM(CURRENCYBASEWITHCONVERSIONTYPE(a.Amount+a.DiscountAmt+a.WriteoffAmt,i.C_Currency_ID,i.DateOrdered,a.AD_Client_ID,a.AD_Org_ID, i.C_CONVERSIONTYPE_ID)),0) "
                    + " FROM C_AllocationLine a INNER JOIN C_Invoice i ON (a.C_Invoice_ID=i.C_Invoice_ID) "
                    + " INNER JOIN C_AllocationHdr h ON (a.C_AllocationHdr_ID = h.C_AllocationHdr_ID) "
                    + " WHERE a.C_BPartner_ID=bp.C_BPartner_ID"
                    + " AND a.IsActive='Y' AND a.C_CashLine_ID IS NOT NULL AND h.DocStatus IN('CO','CL')) "
                //
                + " FROM C_BPartner bp "
                + " WHERE C_BPartner_ID=" + GetC_BPartner_ID();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    SO_CreditUsed = Utility.Util.GetValueOfDecimal(dr[0]);
                    TotalOpenBalance = Utility.Util.GetValueOfDecimal(dr[1]);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }

            _TotalOpenBalanceSet = true;
            String info = null;
            if (SO_CreditUsed != null)
            {
                info = "SO_CreditUsed=" + SO_CreditUsed;
                base.SetSO_CreditUsed(Convert.ToDecimal(SO_CreditUsed));
            }

            if (TotalOpenBalance != null)
            {
                if (info != null)
                    info += ", ";
                info += "TotalOpenBalance=" + TotalOpenBalance;
                base.SetTotalOpenBalance(Convert.ToDecimal(TotalOpenBalance));
            }
            log.Fine(info);
            SetSOCreditStatus();
        }

        /// <summary>
        /// Set Actual Life Time Value from DB
        /// </summary>
        public void SetActualLifeTimeValue()
        {
            Decimal? ActualLifeTimeValue = null;
            String sql = "SELECT "
                + "COALESCE ((SELECT SUM(CURRENCYBASEWITHCONVERSIONTYPE(i.GrandTotal,i.C_Currency_ID,i.DateOrdered,"
                + " i.AD_Client_ID,i.AD_Org_ID, i.C_CONVERSIONTYPE_ID)) "
                    + " FROM C_Invoice_v i WHERE i.C_BPartner_ID=bp.C_BPartner_ID AND i.IsSOTrx='Y'"
            + " AND i.DocStatus IN('CO','CL')),0) FROM C_BPartner bp "
                + " WHERE C_BPartner_ID=" + GetC_BPartner_ID();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    ActualLifeTimeValue = Utility.Util.GetValueOfDecimal(dr[0]);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }

            if (ActualLifeTimeValue != null)
                base.SetActualLifeTimeValue(Convert.ToDecimal(ActualLifeTimeValue));
        }

        /// <summary>
        /// Get Total Open Balance
        /// </summary>
        /// <param name="calculate">if null calculate it</param>
        /// <returns>open balance</returns>
        public Decimal GetTotalOpenBalance(bool calculate)
        {
            if (Env.Signum(GetTotalOpenBalance()) == 0 && calculate)
                SetTotalOpenBalance();
            return base.GetTotalOpenBalance();
        }

        /// <summary>
        /// Set Credit Status
        /// </summary>
        public void SetSOCreditStatus()
        {
            Decimal creditLimit = GetSO_CreditLimit();
            //	Nothing to do
            if (SOCREDITSTATUS_NoCreditCheck.Equals(GetSOCreditStatus())
                || SOCREDITSTATUS_CreditStop.Equals(GetSOCreditStatus())
                || Env.ZERO.CompareTo(creditLimit) == 0)
                return;

            //	Above Credit Limit
            // changed this function for fetching open balance because in case of void it calculates again and fetches wrong value of open balance // Lokesh Chauhan
            //if (creditLimit.CompareTo(GetTotalOpenBalance(!_TotalOpenBalanceSet)) < 0)
            if (creditLimit.CompareTo(GetTotalOpenBalance()) <= 0)
                SetSOCreditStatus(SOCREDITSTATUS_CreditHold);
            else
            {
                //	Above Watch Limit
                //Peference check if credit watch per is zero on header then gets the value from bpgroup 
                Decimal watchAmt;
                if (Get_ColumnIndex("CreditWatchPercent") > 0)
                {
                    if (GetCreditWatchPercent() == 0)
                    {
                        watchAmt = Decimal.Multiply(creditLimit, GetCreditWatchRatio());

                    }
                    else
                        watchAmt = Decimal.Multiply(creditLimit, GetCustomerCreditWatch());
                }
                else
                {
                    watchAmt = Decimal.Multiply(creditLimit, GetCreditWatchRatio());
                }

                if (watchAmt.CompareTo(GetTotalOpenBalance()) < 0)
                    SetSOCreditStatus(SOCREDITSTATUS_CreditWatch);
                else	//	is OK
                    SetSOCreditStatus(SOCREDITSTATUS_CreditOK);
            }
            log.Fine("SOCreditStatus=" + GetSOCreditStatus());
        }

        /// <summary>
        /// Get SO CreditStatus with additional amount
        /// </summary>
        /// <param name="additionalAmt">additional amount in Accounting Currency</param>
        /// <returns>sinulated credit status</returns>
        public String GetSOCreditStatus(Decimal? additionalAmt)
        {
            if (additionalAmt == null || Env.Signum((Decimal)additionalAmt) == 0)
                return GetSOCreditStatus();
            //
            Decimal creditLimit = GetSO_CreditLimit();
            //	Nothing to do
            if (SOCREDITSTATUS_NoCreditCheck.Equals(GetSOCreditStatus())
                || SOCREDITSTATUS_CreditStop.Equals(GetSOCreditStatus())
                || Env.ZERO.CompareTo(creditLimit) == 0)
                return GetSOCreditStatus();
            //	Above (reduced) Credit Limit
            creditLimit = Decimal.Subtract(creditLimit, (Decimal)additionalAmt);
            if (creditLimit.CompareTo(GetTotalOpenBalance(!_TotalOpenBalanceSet)) < 0)
                return SOCREDITSTATUS_CreditHold;

            //	Above Watch Limit
            Decimal watchAmt;
            if (Get_ColumnIndex("CreditWatchPercent") > 0)
            {
                if (GetCreditWatchPercent() == 0)
                {
                    watchAmt = Decimal.Multiply(creditLimit, GetCreditWatchRatio());

                }
                else
                    watchAmt = Decimal.Multiply(creditLimit, GetCustomerCreditWatch());
            }
            else
            {
                watchAmt = Decimal.Multiply(creditLimit, GetCreditWatchRatio());
            }

            if (watchAmt.CompareTo(GetTotalOpenBalance()) < 0)
                return SOCREDITSTATUS_CreditWatch;
            //	is OK
            return SOCREDITSTATUS_CreditOK;
        }
        /// <summary>
        /// Get Credit Watch Ratio on group
        /// </summary>
        /// <returns>credit watch ratio</returns>
        public Decimal GetCustomerCreditWatch()
        {
            Object bd = GetCreditWatchPercent();
            return Decimal.Round(Decimal.Divide(Util.GetValueOfDecimal(bd), Env.ONEHUNDRED), 2, MidpointRounding.AwayFromZero);

        }
        /// <summary>
        /// Get Credit Watch Ratio
        /// </summary>
        /// <returns>BP Group ratio or 0.9</returns>
        public Decimal GetCreditWatchRatio()
        {
            return GetBPGroup().GetCreditWatchRatio();
        }

        /// <summary>
        /// Credit Status is Stop or Hold.
        /// </summary>
        /// <returns>true if Stop/Hold</returns>
        public bool IsCreditStopHold()
        {
            String status = GetSOCreditStatus();
            return SOCREDITSTATUS_CreditStop.Equals(status)
                || SOCREDITSTATUS_CreditHold.Equals(status);
        }

        /// <summary>
        /// Set Total Open Balance
        /// </summary>
        /// <param name="TotalOpenBalance">Total Open Balance</param>
        public void SetTotalOpenBalance(Decimal TotalOpenBalance)
        {
            _TotalOpenBalanceSet = false;
            base.SetTotalOpenBalance(TotalOpenBalance);
        }

        /// <summary>
        /// Get BP Group
        /// </summary>
        /// <returns>group</returns>
        public MBPGroup GetBPGroup()
        {
            if (_group == null)
            {
                if (GetC_BP_Group_ID() == 0)
                    _group = MBPGroup.GetDefault(GetCtx());
                else
                    _group = MBPGroup.Get(GetCtx(), GetC_BP_Group_ID());
            }
            return _group;
        }

        /// <summary>
        /// Get BP Group
        /// </summary>
        /// <param name="group">group</param>
        public void SetBPGroup(MBPGroup group)
        {
            _group = group;
            if (_group == null)
                return;
            SetC_BP_Group_ID(_group.GetC_BP_Group_ID());
            if (_group.GetC_Dunning_ID() != 0)
                SetC_Dunning_ID(_group.GetC_Dunning_ID());
            // if pricelist already selected from UI then skip..
            if (base.GetM_PriceList_ID() > 0) { }
            else
            {
                if (_group.GetM_PriceList_ID() != 0)
                    SetM_PriceList_ID(_group.GetM_PriceList_ID());
            }
            if (_group.GetPO_PriceList_ID() != 0)
                SetPO_PriceList_ID(_group.GetPO_PriceList_ID());
            if (_group.GetM_DiscountSchema_ID() != 0)
                SetM_DiscountSchema_ID(_group.GetM_DiscountSchema_ID());
            if (_group.GetPO_DiscountSchema_ID() != 0)
                SetPO_DiscountSchema_ID(_group.GetPO_DiscountSchema_ID());
        }

        /// <summary>
        /// Get PriceList
        /// </summary>
        /// <returns>price list</returns>
        public new int GetM_PriceList_ID()
        {
            int ii = base.GetM_PriceList_ID();
            if (ii == 0)
                ii = GetBPGroup().GetM_PriceList_ID();
            return ii;
        }

        /// <summary>
        /// Get PO PriceList
        /// </summary>
        /// <returns>price list</returns>
        public new int GetPO_PriceList_ID()
        {
            int ii = base.GetPO_PriceList_ID();
            if (ii == 0)
                ii = GetBPGroup().GetPO_PriceList_ID();
            return ii;
        }

        /// <summary>
        /// Get DiscountSchema
        /// </summary>
        /// <returns>Discount Schema</returns>
        public new int GetM_DiscountSchema_ID()
        {
            int ii = base.GetM_DiscountSchema_ID();
            if (ii == 0)
                ii = GetBPGroup().GetM_DiscountSchema_ID();
            return ii;
        }

        /// <summary>
        /// Get PO DiscountSchema
        /// </summary>
        /// <returns>PO Discount</returns>
        public new int GetPO_DiscountSchema_ID()
        {
            int ii = base.GetPO_DiscountSchema_ID();
            if (ii == 0)
                ii = GetBPGroup().GetPO_DiscountSchema_ID();
            return ii;
        }

        /// <summary>
        /// Get ReturnPolicy
        /// </summary>
        /// <returns>Return Policy</returns>
        public new int GetM_ReturnPolicy_ID()
        {
            int ii = base.GetM_ReturnPolicy_ID();
            if (ii == 0)
                ii = GetBPGroup().GetM_ReturnPolicy_ID();
            if (ii == 0)
                ii = MReturnPolicy.GetDefault(GetCtx());
            return ii;
        }

        /// <summary>
        /// Get Vendor ReturnPolicy
        /// </summary>
        /// <returns>Return Policy</returns>
        public new int GetPO_ReturnPolicy_ID()
        {
            int ii = base.GetPO_ReturnPolicy_ID();
            if (ii == 0)
                ii = GetBPGroup().GetPO_ReturnPolicy_ID();
            if (ii == 0)
                ii = MReturnPolicy.GetDefault(GetCtx());
            return ii;
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (newRecord || Is_ValueChanged("C_BP_Group_ID"))
            {
                MBPGroup grp = GetBPGroup();
                if (grp == null)
                {
                    log.SaveWarning("Error", Msg.ParseTranslation(GetCtx(), "@NotFound@:  @C_BP_Group_ID@"));
                    return false;
                }
                SetBPGroup(grp);	//	setDefaults
                // Set Search Key from Serial No defined on Business Partner Group
                if (grp.Get_ColumnIndex("M_SerNoCtl_ID") >= 0 && grp.GetM_SerNoCtl_ID() > 0)
                {
                    string name = "";
                    MSerNoCtl ctl = new MSerNoCtl(GetCtx(), grp.GetM_SerNoCtl_ID(), Get_TrxName());

                    // if Organization level check box is true on Serila No Control, then Get Current next from Serila No tab.
                    if (ctl.Get_ColumnIndex("IsOrgLevelSequence") >= 0)
                    {
                        name = ctl.CreateDefiniteSerNo(this);
                    }
                    else
                    {
                        name = ctl.CreateSerNo();
                    }
                    SetValue(name);
                }
            }

            //when we select payment method then need to update  payment rule accordingly
            if (Get_ColumnIndex("VA009_PaymentMethod_ID") >= 0)
            {
                if ((GetVA009_PaymentMethod_ID() > 0) || (GetVA009_PO_PaymentMethod_ID() > 0))
                {

                    if (IsCustomer())
                    {
                        string paymentBaseType = Util.GetValueOfString(DB.ExecuteScalar(@"select VA009_PAYMENTBASETYPE from VA009_PAYMENTMETHOD where VA009_PAYMENTMETHOD_ID=" + GetVA009_PaymentMethod_ID()));
                        //if (paymentBaseType == "C")  // Cash + Card
                        //{

                        //}
                        //else if (paymentBaseType == "W") // Wire Transfer
                        //{

                        //}
                        //else
                        SetPaymentRule(paymentBaseType);
                    }
                    if (IsVendor())
                    {
                        string paymentBaseType = Util.GetValueOfString(DB.ExecuteScalar(@"select VA009_PAYMENTBASETYPE from VA009_PAYMENTMETHOD where VA009_PAYMENTMETHOD_ID=" + GetVA009_PO_PaymentMethod_ID()));
                        //if (paymentBaseType == "C") // Cash + Card
                        //{

                        //}
                        //else if (paymentBaseType == "W") // Wire Transfer
                        //{

                        //}
                        //else
                        SetPaymentRulePO(paymentBaseType);
                    }
                }
                else
                {
                    SetPaymentRulePO(null);
                    SetPaymentRule(null);
                }
            }

            // change done by mohit to handle the free seats and filled seats on creation and deletion of employee from employee master window.- asked by ravikant.- 22/01/2018
            if (IsEmployee())
            {
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM AD_ModuleInfo WHERE Prefix='VAHRUAE_' AND isactive='Y'")) > 0)
                {
                    if (newRecord)
                    {
                        X_C_Job job = new X_C_Job(GetCtx(), GetC_Job_ID(), null);
                        if (Util.GetValueOfInt(job.Get_Value("VAHRUAE_FreeSeats")) > 0)
                        {
                            string sql = "SELECT SUM(VAHRUAE_ApprovedVacancy) FROM VAHRUAE_Vacancy vac WHERE vac.DocStatus='CO' and vac.C_Job_ID = " + GetC_Job_ID() +
                         " AND AD_Client_ID = " + GetAD_Client_ID();
                            int ApprVac = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                            sql = "SELECT SUM(VAHRUAE_ApprovableVacancy) FROM VAHRUAE_Vacancy vac WHERE vac.DocStatus='IP' and vac.C_Job_ID=" + GetC_Job_ID() +
                                  " AND AD_Client_ID = " + GetAD_Client_ID();
                            int InApprVac = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                            if ((Util.GetValueOfInt(job.Get_Value("VAHRUAE_FreeSeats")) - (ApprVac + InApprVac)) <= 0)
                            {
                                SetC_Job_ID(0);
                                log.SaveError("Error", Msg.GetMsg(GetCtx(), "VAHRUAE_NoFreeSeat"));
                                return false;
                            }
                        }
                        else
                        {
                            log.SaveError("Error", Msg.GetMsg(GetCtx(), "VAHRUAE_NoFreeSeat"));
                            return false;
                        }
                    }
                    else
                    {
                        if (Is_ValueChanged("C_Job_ID"))
                        {
                            X_C_Job job1 = new X_C_Job(GetCtx(), GetC_Job_ID(), null);
                            if (Util.GetValueOfInt(job1.Get_Value("VAHRUAE_FreeSeats")) > 0)
                            {
                                string sql = "SELECT SUM(VAHRUAE_ApprovedVacancy) FROM VAHRUAE_Vacancy vac WHERE vac.DocStatus='CO' and vac.C_Job_ID = " + GetC_Job_ID() +
                         " AND AD_Client_ID = " + GetAD_Client_ID();
                                int ApprVac = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                                sql = "SELECT SUM(VAHRUAE_ApprovableVacancy) FROM VAHRUAE_Vacancy vac WHERE vac.DocStatus='IP' and vac.C_Job_ID=" + GetC_Job_ID() +
                                      " AND AD_Client_ID = " + GetAD_Client_ID();
                                int InApprVac = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                                if ((Util.GetValueOfInt(job1.Get_Value("VAHRUAE_FreeSeats")) - (ApprVac + InApprVac)) <= 0)
                                {
                                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "VAHRUAE_NoFreeSeat"));
                                    return false;
                                }
                            }
                            else
                            {
                                log.SaveError("Error", Msg.GetMsg(GetCtx(), "VAHRUAE_NoFreeSeat"));
                                return false;
                            }
                            X_C_Job job = new X_C_Job(GetCtx(), Util.GetValueOfInt(Get_ValueOld("C_Job_ID")), null);
                            job.Set_Value("VAHRUAE_FreeSeats", Util.GetValueOfInt(job.Get_Value("VAHRUAE_FreeSeats")) + 1);
                            job.Set_Value("VAHRUAE_FilledSeats", Util.GetValueOfInt(job.Get_Value("VAHRUAE_FilledSeats")) - 1);
                            job.Save();
                        }
                    }
                }
            }

            // If NextStepBy value not provided or neither NextStepBy nor C_Followupdate has changed, no task will be created.
            if (Env.IsModuleInstalled("VA061_"))
            {
                int wf_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Workflow_ID FROM AD_Workflow WHERE WorkflowType='V' AND IsActive='Y' AND AD_Table_ID="
                    + Get_Table_ID(), null, Get_Trx()));
                if (wf_ID > 0 && Get_Value("VA061_NextStepBy") != null && (Is_ValueChanged("VA061_NextStepBy") || Is_ValueChanged("C_Followupdate")))
                {
                    // Ensure that VA061_NextStep has value
                    if (string.IsNullOrEmpty(Util.GetValueOfString(Get_Value("VA061_NextStep"))))
                    {
                        log.SaveWarning("VA061_NextStepMustHaveValue", "");
                        return false;
                    }

                    // Ensure that FollowUp On has value
                    if (string.IsNullOrEmpty(Util.GetValueOfString(Get_Value("C_Followupdate"))))
                    {
                        log.SaveWarning("VA061_FollowupdateMustHaveValue", "");
                        return false;
                    }
                    // Set Value in this field to true, for task generation on workflow process
                    Set_Value("VA061_IsCreateTask", true);
                }
                else
                {
                    Set_Value("VA061_IsCreateTask", false);
                }
            }
            return true;
        }

        /// <summary>
        /// After save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return false;

            StringBuilder _sql = new StringBuilder("");

            //_sql.Append("Select count(*) from  ad_table where tablename like 'FRPT_BP_Customer_Acct'");
            //_sql.Append("SELECT count(*) FROM all_objects WHERE object_type IN ('TABLE') AND (object_name)  = UPPER('FRPT_BP_Customer_Acct')  AND OWNER LIKE '" + DB.GetSchema() + "'");
            _sql.Append(DBFunctionCollection.CheckTableExistence(DB.GetSchema(), "FRPT_BP_Customer_Acct"));
            int countC = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
            if (countC > 0)
            {
                if (IsCustomer())
                {
                    PO obj = null;
                    //MFRPTBPCustomerAcct obj = null;
                    int Customer_ID = GetC_BPartner_ID();
                    int C_BP_Group_ID = GetC_BP_Group_ID();
                    string sql = "SELECT L.VALUE FROM AD_REF_LIST L inner join AD_Reference r on R.AD_REFERENCE_ID=L.AD_REFERENCE_ID where   r.name='FRPT_RelatedTo' and l.name='Customer'";
                    //string sql = "select VALUE from AD_Ref_List where name='Customer'";
                    string _RelatedToCustmer = Convert.ToString(DB.ExecuteScalar(sql));

                    _sql.Clear();
                    _sql.Append("Select Count(*) From FRPT_BP_Customer_Acct   where C_BPartner_ID=" + Customer_ID + " AND IsActive = 'Y' AND AD_Client_ID = " + GetAD_Client_ID());
                    int value = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
                    if (value < 1)
                    {

                        _sql.Clear();
                        _sql.Append("Select  BPG.c_acctschema_id, BPG.c_validcombination_id, BPG.frpt_acctdefault_id From FRPT_BP_Group_Acct  BPG inner join frpt_acctdefault ACC ON ACC.frpt_acctdefault_id= BPG.frpt_acctdefault_id where BPG.C_BP_Group_ID=" + C_BP_Group_ID + " and ACC.frpt_relatedto='" + _RelatedToCustmer + "' AND BPG.IsActive = 'Y' AND BPG.AD_Client_ID = " + GetAD_Client_ID());
                        DataSet ds = DB.ExecuteDataset(_sql.ToString());
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                //obj = new MFRPTBPCustomerAcct(GetCtx(), 0, null);
                                obj = MTable.GetPO(GetCtx(), "FRPT_BP_Customer_Acct", 0, null);
                                obj.Set_ValueNoCheck("C_BPartner_ID", Customer_ID);
                                obj.Set_ValueNoCheck("AD_Org_ID", 0);
                                obj.Set_ValueNoCheck("C_AcctSchema_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_AcctSchema_ID"]));
                                obj.Set_ValueNoCheck("C_ValidCombination_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_ValidCombination_ID"]));
                                obj.Set_ValueNoCheck("FRPT_AcctDefault_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));
                                if (!obj.Save())
                                {
                                }
                            }
                        }
                    }
                }
            }
            _sql.Clear();
            //_sql.Append("Select count(*) from  ad_table where tablename like 'FRPT_BP_Vendor_Acct'");
            //_sql.Append("SELECT count(*) FROM all_objects WHERE object_type IN ('TABLE') AND (object_name)  = UPPER('FRPT_BP_Vendor_Acct')  AND OWNER LIKE '" + DB.GetSchema() + "'");
            _sql.Append(DBFunctionCollection.CheckTableExistence(DB.GetSchema(), "FRPT_BP_Vendor_Acct"));
            int countV = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
            if (countV > 0)
            {
                if (IsVendor())
                {
                    PO obj = null;
                    //MFRPTBPVendorAcct obj = null;
                    int Vendor_ID = GetC_BPartner_ID();
                    int C_BP_Group_ID = GetC_BP_Group_ID();
                    string sql = "SELECT L.VALUE FROM AD_REF_LIST L inner join AD_Reference r on R.AD_REFERENCE_ID=L.AD_REFERENCE_ID where   r.name='FRPT_RelatedTo' and l.name='Vendor'";
                    //string sql = "select VALUE from AD_Ref_List where name='Vendor'";
                    string _RelatedToVendor = Convert.ToString(DB.ExecuteScalar(sql));
                    //string _RelatedToVendor = X_FRPT_AcctDefault.FRPT_RELATEDTO_Vendor.ToString();


                    _sql.Clear();
                    _sql.Append("Select Count(*) From FRPT_BP_Vendor_Acct   where C_BPartner_ID=" + Vendor_ID + " AND IsActive = 'Y' AND AD_Client_ID = " + GetAD_Client_ID());
                    int value = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
                    if (value < 1)
                    {
                        _sql.Clear();
                        _sql.Append("Select  BPG.c_acctschema_id, BPG.c_validcombination_id, BPG.frpt_acctdefault_id From FRPT_BP_Group_Acct  BPG inner join frpt_acctdefault ACC ON ACC.frpt_acctdefault_id= BPG.frpt_acctdefault_id where BPG.C_BP_Group_ID=" + C_BP_Group_ID + " and ACC.frpt_relatedto='" + _RelatedToVendor + "' AND BPG.IsActive = 'Y' AND BPG.AD_Client_ID = " + GetAD_Client_ID());
                        DataSet ds = DB.ExecuteDataset(_sql.ToString());
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                //obj = new MFRPTBPVendorAcct(GetCtx(), 0, null);
                                obj = MTable.GetPO(GetCtx(), "FRPT_BP_Vendor_Acct", 0, null);
                                obj.Set_ValueNoCheck("C_BPartner_ID", Vendor_ID);
                                obj.Set_ValueNoCheck("AD_Org_ID", 0);
                                obj.Set_ValueNoCheck("C_AcctSchema_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_AcctSchema_ID"]));
                                obj.Set_ValueNoCheck("C_ValidCombination_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_ValidCombination_ID"]));
                                obj.Set_ValueNoCheck("FRPT_AcctDefault_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));

                                if (!obj.Save())
                                {
                                }
                            }
                        }
                    }
                }
            }
            _sql.Clear();
            //_sql.Append("Select count(*) from  ad_table where tablename like 'FRPT_BP_Employee_Acct'");
            //_sql.Append("SELECT count(*) FROM all_objects WHERE object_type IN ('TABLE') AND (object_name)  = UPPER('FRPT_BP_Employee_Acct')  AND OWNER LIKE '" + DB.GetSchema() + "'");
            _sql.Append(DBFunctionCollection.CheckTableExistence(DB.GetSchema(), "FRPT_BP_Employee_Acct"));
            int countE = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
            if (countE > 0)
            {

                if (IsEmployee())
                {
                    // MFRPTBPEmployeeAcct obj = null;
                    int Employee_ID = GetC_BPartner_ID();
                    int C_BP_Group_ID = GetC_BP_Group_ID();
                    string sql = "SELECT L.VALUE FROM AD_REF_LIST L inner join AD_Reference r on R.AD_REFERENCE_ID=L.AD_REFERENCE_ID where   r.name='FRPT_RelatedTo' and l.name='Employee'";
                    //string sql = "select VALUE from AD_Ref_List where name='Employee'";
                    string _RelatedToEmployee = Convert.ToString(DB.ExecuteScalar(sql));
                    //string _RelatedToEmployee = X_FRPT_AcctDefault.FRPT_RELATEDTO_Employee.ToString();


                    _sql.Clear();
                    _sql.Append("Select Count(*) From FRPT_BP_Employee_Acct  where C_BPartner_ID=" + Employee_ID + " AND IsActive = 'Y' AND AD_Client_ID = " + GetAD_Client_ID());
                    int value = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
                    if (value < 1)
                    {
                        _sql.Clear();
                        _sql.Append("Select  BPG.c_acctschema_id, BPG.c_validcombination_id, BPG.frpt_acctdefault_id From FRPT_BP_Group_Acct  BPG inner join frpt_acctdefault ACC ON ACC.frpt_acctdefault_id= BPG.frpt_acctdefault_id where BPG.C_BP_Group_ID=" + C_BP_Group_ID + " and ACC.frpt_relatedto='" + _RelatedToEmployee + "' AND BPG.IsActive = 'Y' AND BPG.AD_Client_ID = " + GetAD_Client_ID());
                        DataSet ds = DB.ExecuteDataset(_sql.ToString());
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                //obj = new MFRPTBPEmployeeAcct(GetCtx(), 0, null);
                                var obj = MTable.GetPO(GetCtx(), "FRPT_BP_Employee_Acct", 0, null);
                                obj.Set_ValueNoCheck("C_BPartner_ID", Employee_ID);
                                obj.Set_ValueNoCheck("AD_Org_ID", 0);
                                obj.Set_ValueNoCheck("C_AcctSchema_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_AcctSchema_ID"]));
                                obj.Set_ValueNoCheck("C_ValidCombination_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_ValidCombination_ID"]));
                                obj.Set_ValueNoCheck("FRPT_AcctDefault_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));
                                if (!obj.Save())
                                {
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // work done by Amit -- skip inserting default accounting on save of new record
                if (newRecord & success && (String.IsNullOrEmpty(GetCtx().GetContext("#DEFAULT_ACCOUNTING_APPLICABLE")) || Util.GetValueOfString(GetCtx().GetContext("#DEFAULT_ACCOUNTING_APPLICABLE")) == "Y"))
                {
                    //	Accounting
                    bool sucs = Insert_Accounting("C_BP_Customer_Acct", "C_BP_Group_Acct",
                           "p.C_BP_Group_ID=" + GetC_BP_Group_ID());
                    //Karan. work done to show message if data not saved in accounting tab. but will save data in current tab.
                    // Before this, data was being saved but giving message "record not saved".
                    if (!sucs)
                    {
                        log.SaveWarning("AcctNotSaved", "");
                    }

                    sucs = Insert_Accounting("C_BP_Vendor_Acct", "C_BP_Group_Acct",
                           "p.C_BP_Group_ID=" + GetC_BP_Group_ID());

                    //Karan. work done to show message if data not saved in accounting tab. but will save data in current tab.
                    // Before this, data was being saved but giving message "record not saved".
                    if (!sucs)
                    {
                        log.SaveWarning("AcctNotSaved", "");
                    }


                    sucs = Insert_Accounting("C_BP_Employee_Acct", "C_AcctSchema_Default", null);

                    //Karan. work done to show message if data not saved in accounting tab. but will save data in current tab.
                    // Before this, data was being saved but giving message "record not saved".
                    if (!sucs)
                    {
                        log.SaveWarning("AcctNotSaved", "");
                    }
                }

                //	Value/Name change
                if (success && !newRecord
                    && (Is_ValueChanged("Value") || Is_ValueChanged("Name")))
                    MAccount.UpdateValueDescription(GetCtx(), "C_BPartner_ID=" +
                        GetC_BPartner_ID(), Get_TrxName());
            }
            //Added by Neha Thakur--05 Jan 2018--Set "Report To" (from Header tab) as Supervisor in Login User tab--Asked by Ravikant
            if (IsEmployee())
            {
                int ModuleId = Util.GetValueOfInt(DB.ExecuteScalar("select ad_moduleinfo_id from ad_moduleinfo where prefix='VAHRUAE_' and isactive='Y'"));
                if (ModuleId > 0)
                {
                    int _Emp_ID = Util.GetValueOfInt(Get_Value("VAHRUAE_HR_Employee"));
                    if (_Emp_ID > 0)
                    {
                        _sql.Clear();
                        _sql.Append(@"SELECT AD_USER_ID FROM AD_USER WHERE C_BPARTNER_ID=" + _Emp_ID + " AND AD_CLIENT_ID =" + GetAD_Client_ID());
                        int AD_User_ID = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString(), null, null));
                        _sql.Clear();
                        if (AD_User_ID > 0)
                        {
                            _sql.Append(@"UPDATE AD_USER SET SUPERVISOR_ID=" + AD_User_ID + " WHERE C_BPARTNER_ID=" + GetC_BPartner_ID());

                        }
                        else
                        {
                            _sql.Append(@"UPDATE AD_USER SET SUPERVISOR_ID=null WHERE C_BPARTNER_ID=" + GetC_BPartner_ID());
                        }
                        int _count = Util.GetValueOfInt(DB.ExecuteQuery(_sql.ToString(), null, null));
                        _sql.Clear();
                    }
                    // change done by mohit to handle the free seats and filled seats on creation and deletion of employee from employee master window.- asked by ravikant.- 22/01/2018
                    if (newRecord)
                    {
                        X_C_Job job = new X_C_Job(GetCtx(), GetC_Job_ID(), null);
                        job.Set_Value("VAHRUAE_FreeSeats", Util.GetValueOfInt(job.Get_Value("VAHRUAE_FreeSeats")) - 1);
                        job.Set_Value("VAHRUAE_FilledSeats", Util.GetValueOfInt(job.Get_Value("VAHRUAE_FilledSeats")) + 1);
                        job.Save();
                    }
                    else
                    {
                        if (Is_ValueChanged("C_Job_ID"))
                        {
                            X_C_Job job = new X_C_Job(GetCtx(), GetC_Job_ID(), null);
                            job.Set_Value("VAHRUAE_FreeSeats", Util.GetValueOfInt(job.Get_Value("VAHRUAE_FreeSeats")) - 1);
                            job.Set_Value("VAHRUAE_FilledSeats", Util.GetValueOfInt(job.Get_Value("VAHRUAE_FilledSeats")) + 1);
                            job.Save();
                        }
                    }
                }

            }

            int count = DB.ExecuteQuery("UPDATE C_BPartner_Location SET CreditStatusSettingOn = '" + GetCreditStatusSettingOn() + "' WHERE C_BPartner_ID = " + GetC_BPartner_ID(), null, Get_Trx());

            //---------End----------------------------------------------------
            return success;
        }

        /// <summary>
        /// Check whether credit Validation matches against the Credit Validation,
        /// set either on Business Partner header Or Locaion based on the settings
        /// </summary>
        /// <param name="TrxType">Credit Validation type to match</param>
        /// <param name="C_BPartner_Location_ID">Business Partner Locaion ID</param>
        /// <returns>True/False</returns>
        public bool ValidateCreditValidation(string TrxType, int C_BPartner_Location_ID)
        {
            if (GetCreditStatusSettingOn() == X_C_BPartner.CREDITSTATUSSETTINGON_CustomerHeader)
            {
                if (TrxType.Contains(GetCreditValidation()))
                    return true;
            }
            else if (C_BPartner_Location_ID > 0)
            {
                MBPartnerLocation loc = new MBPartnerLocation(GetCtx(), C_BPartner_Location_ID, Get_Trx());
                if (TrxType.Contains(loc.GetCreditValidation()))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// function to check credit limit and other validations for Business partner,
        /// based on the settings on Business partner 
        /// </summary>
        /// <param name="C_BPartner_Location_ID">Business partner location ID</param>
        /// <param name="Amt">Amount</param>
        /// <param name="retMsg">Return Message</param>
        /// <returns>Status True/False (whether credit allowed or not)</returns>
        public bool IsCreditAllowed(int C_BPartner_Location_ID, Decimal? Amt, out string retMsg)
        {
            Decimal? totOpnBal = 0;
            Decimal? crdLmt = 0;
            retMsg = "";
            string creditStatus = GetSOCreditStatus();
            if (GetCreditStatusSettingOn() == X_C_BPartner.CREDITSTATUSSETTINGON_CustomerHeader)
            {
                //Credit Limit by Vivek on 30/09/2016
                if (X_C_BPartner.SOCREDITSTATUS_CreditStop.Equals(GetSOCreditStatus()))
                {
                    retMsg = Msg.GetMsg(GetCtx(), "BPartnerCreditStop") + " - " + Msg.Translate(GetCtx(), "TotalOpenBalance") + " = "
                        + GetTotalOpenBalance()
                        + ", " + Msg.Translate(GetCtx(), "SO_CreditLimit") + " = " + GetSO_CreditLimit();
                    return false;
                }
                if (X_C_BPartner.SOCREDITSTATUS_CreditHold.Equals(GetSOCreditStatus()))
                {
                    retMsg = Msg.GetMsg(GetCtx(), "BPartnerCreditHold") + " - " + Msg.Translate(GetCtx(), "TotalOpenBalance") + " = "
                        + GetTotalOpenBalance()
                        + ", " + Msg.Translate(GetCtx(), "SO_CreditLimit") + " = " + GetSO_CreditLimit();
                    return false;
                }
                totOpnBal = GetTotalOpenBalance();
                crdLmt = GetSO_CreditLimit();
            }
            else if (C_BPartner_Location_ID > 0 && GetCreditStatusSettingOn() == X_C_BPartner.CREDITSTATUSSETTINGON_CustomerLocation)
            {
                MBPartnerLocation bploc = new MBPartnerLocation(GetCtx(), C_BPartner_Location_ID, Get_Trx());
                creditStatus = bploc.GetSOCreditStatus();
                if (X_C_BPartner.SOCREDITSTATUS_CreditStop.Equals(bploc.GetSOCreditStatus()))
                {
                    retMsg = Msg.GetMsg(GetCtx(), "BPartnerCreditStop") + " - " + Msg.Translate(GetCtx(), "TotalOpenBalance") + " = "
                        + bploc.GetTotalOpenBalance()
                        + ", " + Msg.Translate(GetCtx(), "SO_CreditLimit") + " = " + bploc.GetSO_CreditLimit();
                    return false;
                }
                if (X_C_BPartner.SOCREDITSTATUS_CreditHold.Equals(bploc.GetSOCreditStatus()))
                {
                    retMsg = Msg.GetMsg(GetCtx(), "BPartnerCreditHold") + " - " + Msg.Translate(GetCtx(), "TotalOpenBalance") + " = "
                        + bploc.GetTotalOpenBalance()
                        + ", " + Msg.Translate(GetCtx(), "SO_CreditLimit") + " = " + bploc.GetSO_CreditLimit();
                    return false;
                }
                totOpnBal = bploc.GetTotalOpenBalance();
                crdLmt = bploc.GetSO_CreditLimit();
            }
            // check for payment if Total Open Balance + payment Amount exceeds Credit limit do not allow to complete transaction
            if ((creditStatus != X_C_BPartner.SOCREDITSTATUS_NoCreditCheck) && (crdLmt > 0) && (crdLmt < (totOpnBal + Amt)))
            {
                retMsg = Msg.GetMsg(GetCtx(), "VIS_CreditLimitExceed") + " - " + Msg.Translate(GetCtx(), "TotalOpenBalance") + " = "
                        + totOpnBal + ", " + Msg.GetMsg(GetCtx(), "BPartnerCreditStop") + " = " + Amt
                        + ", " + Msg.Translate(GetCtx(), "SO_CreditLimit") + " = " + crdLmt;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check whether Business partner credit stauts is on Credit Watch
        /// </summary>
        /// <param name="C_BPartner_Location_ID">Business Partner Location ID</param>
        /// <returns></returns>
        public bool IsCreditWatch(int C_BPartner_Location_ID)
        {
            if (GetCreditStatusSettingOn() == X_C_BPartner.CREDITSTATUSSETTINGON_CustomerHeader)
            {
                if (GetSOCreditStatus() == X_C_BPartner.SOCREDITSTATUS_CreditWatch)
                    return true;
            }
            else if (GetCreditStatusSettingOn() == X_C_BPartner.CREDITSTATUSSETTINGON_CustomerLocation)
            {
                MBPartnerLocation bploc = new MBPartnerLocation(GetCtx(), C_BPartner_Location_ID, Get_Trx());
                if (bploc.GetSOCreditStatus() == X_C_BPartner.SOCREDITSTATUS_CreditWatch)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Before Delete
        /// </summary>
        /// <returns>true</returns>
        protected override bool BeforeDelete()
        {
            return Delete_Accounting("C_BP_Customer_Acct")
                && Delete_Accounting("C_BP_Vendor_Acct")
                && Delete_Accounting("C_BP_Employee_Acct");
        }
        // change done by mohit to handle the free seats and filled seats on creation and deletion of employee from employee master window.- asked by ravikant.- 22/01/2018
        protected override bool AfterDelete(bool success)
        {
            if (success)
            {
                X_C_Job job = new X_C_Job(GetCtx(), GetC_Job_ID(), null);
                job.Set_Value("VAHRUAE_FreeSeats", Util.GetValueOfInt(job.Get_Value("VAHRUAE_FreeSeats")) + 1);
                job.Set_Value("VAHRUAE_FilledSeats", Util.GetValueOfInt(job.Get_Value("VAHRUAE_FilledSeats")) - 1);
                job.Save();
            }
            return success;
        }
    }
}