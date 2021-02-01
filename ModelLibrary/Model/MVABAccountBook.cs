/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MLocation
 * Purpose        : used for VAB_AccountBook
 * Class Used     : X_VAB_AccountBook
 * Chronological    Development
 * Raghunandan     06-Jun-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using System.Data.SqlClient;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.Process;
using VAdvantage.Utility;
using System.Threading;
using System.Drawing;
using System.IO;

namespace VAdvantage.Model
{
    public class MVABAccountBook : X_VAB_AccountBook
    {
        /** Costing Currency Precision		*/
        private int _costPrecision = -1;
        /** Accounting Currency Precision	*/
        private int _stdPrecision = -1;
        /** Element List       */
        private MVABAccountBookElement[] _elements = null;
        // Default Info		
        private MVABAccountBookDefault _default = null;

        //Only Post Org Childs			
        private int?[] _onlyOrgs = null;
        //Only Post Org				
        private MVAFOrg _onlyOrg = null;

        //GL Info				
        private MVABAccountBookGL _gl = null;
        private MAccount _SuspenseError_Acct = null;
        private MAccount _DueTo_Acct = null;
        private MAccount _DueFrom_Acct = null;
        private MAccount _CurrencyBalancing_Acct = null;

        private MAccount _realizedGain_Acct = null;
        private MAccount _realizedLoss_Acct = null;



        /// <summary>
        /// Get AccountSchema of Client
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_AccountBook_ID">schema id</param>
        /// <returns>Accounting schema</returns>
        public static MVABAccountBook Get(Ctx ctx, int VAB_AccountBook_ID)
        {
            return Get(ctx, VAB_AccountBook_ID, null);
        }	//	get

        /// <summary>
        /// Get AcctSchema Element
        /// </summary>
        /// <param name="elementType">elementType segment type - AcctSchemaElement.ELEMENTTYPE_</param>
        /// <returns>AcctSchemaElement</returns>
        public MVABAccountBookElement GetAcctSchemaElement(String elementType)
        {
            if (_elements == null)
            {
                GetAcctSchemaElements();
            }
            for (int i = 0; i < _elements.Length; i++)
            {
                MVABAccountBookElement ase = _elements[i];
                if (ase.GetElementType().Equals(elementType))
                {
                    return ase;
                }
            }
            return null;
        }

        /// <summary>
        /// Get AccountSchema of Client
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_AccountBook_ID">schema id</param>
        /// <param name="trxName">optional trx</param>
        /// <returns>Accounting schema</returns>
        public static MVABAccountBook Get(Ctx ctx, int VAB_AccountBook_ID, Trx trxName)
        {
            //  Check Cache
            int key = VAB_AccountBook_ID;
            MVABAccountBook retValue = (MVABAccountBook)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MVABAccountBook(ctx, VAB_AccountBook_ID, trxName);
            if (trxName == null)
                _cache.Add(key, retValue);
            return retValue;
        }	//	get

        /// <summary>
        /// Get AccountSchema of Client
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Client_ID">client or 0 for all</param>
        /// <returns>Array of AcctSchema of Client</returns>
        public static MVABAccountBook[] GetClientAcctSchema(Ctx ctx, int VAF_Client_ID)
        {
            return GetClientAcctSchema(ctx, VAF_Client_ID, null);
        }	//	getClientAcctSchema

        /// <summary>
        /// Get AccountSchema of Client
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Client_ID">client or 0 for all</param>
        /// <param name="trxName">optional trx </param>
        /// <returns>Array of AcctSchema of Client</returns>
        public static MVABAccountBook[] GetClientAcctSchema(Ctx ctx, int VAF_Client_ID, Trx trxName)
        {
            //  Check Cache
            int key = VAF_Client_ID;
            if (_schema.ContainsKey(key))
                return (MVABAccountBook[])_schema[key];

            //  Create New
            List<MVABAccountBook> list = new List<MVABAccountBook>();
            MVAFClientDetail info = MVAFClientDetail.Get(ctx, VAF_Client_ID, trxName);
            MVABAccountBook ass = MVABAccountBook.Get(ctx, info.GetVAB_AccountBook1_ID(), trxName);
            if (ass.Get_ID() != 0 && trxName == null)
                list.Add(ass);

            //	Other
            String sql = "SELECT VAB_AccountBook_ID FROM VAB_AccountBook acs "
                + "WHERE IsActive='Y'"
                + " AND EXISTS (SELECT * FROM VAB_AccountBook_GL gl WHERE acs.VAB_AccountBook_ID=gl.VAB_AccountBook_ID)"
                + " AND EXISTS (SELECT * FROM VAB_AccountBook_Default d WHERE acs.VAB_AccountBook_ID=d.VAB_AccountBook_ID)";
            if (VAF_Client_ID != 0)
            {
                sql += " AND VAF_Client_ID=" + VAF_Client_ID;
            }
            sql += " ORDER BY VAB_AccountBook_ID";

            IDataReader dr = null;
            try
            {
                dr = CoreLibrary.DataBase.DB.ExecuteReader(sql, null, trxName);
                while (dr.Read())
                {
                    int id = Utility.Util.GetValueOfInt(dr[0].ToString());
                    if (id != info.GetVAB_AccountBook1_ID())	//	already in list
                    {
                        ass = MVABAccountBook.Get(ctx, id, trxName);
                        if (ass.Get_ID() != 0 && trxName == null)
                            list.Add(ass);
                    }
                }
                dr.Close();
                dr = null;
            }
            catch (System.Data.Common.DbException e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
            }
            //  Save
            MVABAccountBook[] retValue = new MVABAccountBook[list.Count];
            retValue = list.ToArray();
            if (trxName == null)
                _schema.Add(key, retValue);
            return retValue;
        }   //  getClientAcctSchema

        // by amit 23-12-2015
        public static MVABAccountBook[] GetClientAcctSchemas(Ctx ctx, int VAF_Client_ID, Trx trxName)
        {
            //  Check Cache
            int key = VAF_Client_ID;
            if (_schema.ContainsKey(key))
                return (MVABAccountBook[])_schema[key];

            //  Create New
            List<MVABAccountBook> list = new List<MVABAccountBook>();
            MVAFClientDetail info = MVAFClientDetail.Get(ctx, VAF_Client_ID, trxName);
            MVABAccountBook ass = MVABAccountBook.Get(ctx, info.GetVAB_AccountBook1_ID(), trxName);
            if (ass.Get_ID() != 0 && trxName == null)
                list.Add(ass);

            //	Other
            String sql = "SELECT VAB_AccountBook_ID FROM VAB_AccountBook acs "
                + "WHERE IsActive='Y'";
            if (VAF_Client_ID != 0)
            {
                sql += " AND VAF_Client_ID=" + VAF_Client_ID;
            }
            sql += " ORDER BY VAB_AccountBook_ID";

            IDataReader dr = null;
            try
            {
                dr = CoreLibrary.DataBase.DB.ExecuteReader(sql, null, trxName);
                while (dr.Read())
                {
                    int id = Utility.Util.GetValueOfInt(dr[0].ToString());
                    if (id != info.GetVAB_AccountBook1_ID())	//	already in list
                    {
                        ass = MVABAccountBook.Get(ctx, id, trxName);
                        if (ass.Get_ID() != 0 && trxName == null)
                            list.Add(ass);
                    }
                }
                dr.Close();
                dr = null;
            }
            catch (System.Data.Common.DbException e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
            }
            //  Save
            MVABAccountBook[] retValue = new MVABAccountBook[list.Count];
            retValue = list.ToArray();
            if (trxName == null)
                _schema.Add(key, retValue);
            return retValue;
        }   //  getClientAcctSchema


        /** Cache of Client AcctSchema Arrays		**/
        private static CCache<int, MVABAccountBook[]> _schema = new CCache<int, MVABAccountBook[]>("VAF_ClientDetail", 3);	//  3 clients
        /**	Cache of AcctSchemas 					**/
        private static CCache<int, MVABAccountBook> _cache = new CCache<int, MVABAccountBook>("VAB_AccountBook", 3);	//  3 accounting schemas
        /**	Logger			*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MVABAccountBook).FullName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="currency"></param>
        public MVABAccountBook(MVAFClient client, KeyNamePair currency)
            : this(client.GetCtx(), 0, client.Get_TrxName())
        {
            SetClientOrg(client);
            SetVAB_Currency_ID(currency.GetKey());
            SetName(client.GetName() + " " + GetGAAP() + "/" + Get_ColumnCount() + " " + currency.GetName());
        }	//	MAcctSchema

        /// <summary>
        ///Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_AccountBook_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVABAccountBook(Ctx ctx, int VAB_AccountBook_ID, Trx trxName)
            : base(ctx, VAB_AccountBook_ID, trxName)
        {
            //super (ctx, VAB_AccountBook_ID, trxName);
            if (VAB_AccountBook_ID == 0)
            {
                //	setVAB_Currency_ID (0);
                //	setName (null);
                SetAutoPeriodControl(true);
                SetPeriod_OpenFuture(2);
                SetPeriod_OpenHistory(2);
                SetCostingMethod(COSTINGMETHOD_StandardCosting);
                SetCostingLevel(COSTINGLEVEL_Client);
                SetIsAdjustCOGS(false);
                SetGAAP(GAAP_InternationalGAAP);
                SetHasAlias(true);
                SetHasCombination(false);
                SetIsAccrual(true);	// Y
                SetCommitmentType(COMMITMENTTYPE_None);
                SetIsDiscountCorrectsTax(false);
                SetTaxCorrectionType(TAXCORRECTIONTYPE_None);
                SetIsTradeDiscountPosted(false);
                SetIsPostServices(false);
                SetIsExplicitCostAdjustment(false);
                SetSeparator("-");	// -
            }
        }

        /**
	 * 	Get Costing Precision of accounting Currency
	 *	@return precision
	 */
        public int GetCostingPrecision()
        {
            if (_costPrecision < 0)
                GetStdPrecision();
            return _costPrecision;
        }

        /**
	 * 	Get Std Precision of accounting Currency
	 *	@return precision
	 */
        public int GetStdPrecision()
        {
            if (_stdPrecision < 0)
            {
                MVABCurrency cur = MVABCurrency.Get(GetCtx(), GetVAB_Currency_ID());
                _stdPrecision = cur.GetStdPrecision();
                _costPrecision = cur.GetCostingPrecision();
            }
            return _stdPrecision;
        }

        /**
        *  AcctSchema Elements
        *  @return ArrayList of AcctSchemaElement
        */
        public MVABAccountBookElement[] GetAcctSchemaElements()
        {
            if (_elements == null)
                _elements = MVABAccountBookElement.GetAcctSchemaElements(this);
            return _elements;
        }

        protected override bool BeforeSave(bool newRecord)
        {
            if (GetVAF_Org_ID() != 0)
                SetVAF_Org_ID(0);
            if (base.GetTaxCorrectionType() == null)
                SetTaxCorrectionType(IsDiscountCorrectsTax() ? TAXCORRECTIONTYPE_Write_OffAndDiscount : TAXCORRECTIONTYPE_None);
            CheckCosting();
            //	Check Primary
            if (GetVAF_OrgOnly_ID() != 0)
            {
                MVAFClientDetail info = MVAFClientDetail.Get(GetCtx(), GetVAF_Client_ID());
                if (info.GetVAB_AccountBook1_ID() == GetVAB_AccountBook_ID())
                    SetVAF_OrgOnly_ID(0);
            }
            return true;
        }

        public void CheckCosting()
        {
            log.Info(ToString());
            //	Create Cost Type
            if (GetM_CostType_ID() == 0)
            {
                MCostType ct = new MCostType(GetCtx(), 0, Get_TrxName());
                ct.SetClientOrg(GetVAF_Client_ID(), 0);
                ct.SetName(GetName());
                ct.Save();
                SetM_CostType_ID(ct.GetM_CostType_ID());
            }

            //	Create Cost Elements
            MCostElement.GetMaterialCostElement(this, GetCostingMethod());

            //	Default Costing Level
            if (GetCostingLevel() == null)
                SetCostingLevel(COSTINGLEVEL_Client);
            if (GetCostingMethod() == null)
                SetCostingMethod(COSTINGMETHOD_StandardCosting);
            if (GetGAAP() == null)
                SetGAAP(GAAP_InternationalGAAP);
        }	//	checkCosting

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("AcctSchema[");
            sb.Append(Get_ID()).Append("-").Append(GetName())
                .Append("]");
            return sb.ToString();
        }
        /// <summary>
        /// Set Only Org Childs
        /// </summary>
        /// <returns>orgs</returns>
        public int?[] GetOnlyOrgs()
        {
            return _onlyOrgs;
        }

        /// <summary>
        /// 	Set Only Org Childs
        /// </summary>
        /// <param name="orgs">orgs</param>
        public void SetOnlyOrgs(int?[] orgs)
        {
            _onlyOrgs = orgs;
        }


        /// <summary>
        /// Skip creating postings for this Org.
        /// Requires setOnlyOrgs (MReportTree requires MTree in Basis)
        /// </summary>
        /// <param name="VAF_Org_ID"></param>
        /// <returns>true if to skip</returns>
        public bool IsSkipOrg(int VAF_Org_ID)
        {
            if (GetVAF_OrgOnly_ID() == 0)
            {
                return false;
            }
            //	Only Organization
            if (GetVAF_OrgOnly_ID() == VAF_Org_ID)
            {
                return false;
            }
            if (_onlyOrg == null)
                _onlyOrg = MVAFOrg.Get(GetCtx(), GetVAF_OrgOnly_ID());
            //	Not Summary Only - i.e. skip it
            if (!_onlyOrg.IsSummary())
            {
                return true;
            }
            if (_onlyOrgs == null)
            {
                return false;
            }
            for (int i = 0; i < _onlyOrgs.Length; i++)
            {
                if (VAF_Org_ID == Utility.Util.GetValueOfInt(_onlyOrgs[i]))
                {
                    return false;

                }
            }
            return true;
        }

        /// <summary>
        /// Is Suspense Balancing active
        /// </summary>
        /// <returns>suspense balancing</returns>
        public bool IsSuspenseBalancing()
        {
            if (_gl == null)
            {
                GetAcctSchemaGL();
            }
            return _gl.IsUseSuspenseBalancing() && _gl.GetSuspenseBalancing_Acct() != 0;
        }

        /// <summary>
        /// Get AcctSchema GL info
        /// </summary>
        /// <returns>GL info</returns>
        public MVABAccountBookGL GetAcctSchemaGL()
        {
            if (_gl == null)
            {
                _gl = MVABAccountBookGL.Get(GetCtx(), GetVAB_AccountBook_ID());
            }
            if (_gl == null)
            {
                throw new Exception("No GL Definition for VAB_AccountBook_ID=" + GetVAB_AccountBook_ID());
            }
            return _gl;
        }


        /// <summary>
        /// Get Suspense Error Account
        /// </summary>
        /// <returns>suspense error account</returns>
        public MAccount GetSuspenseBalancing_Acct()
        {
            if (_SuspenseError_Acct != null)
            {
                return _SuspenseError_Acct;
            }
            if (_gl == null)
            {
                GetAcctSchemaGL();
            }
            int VAB_Acct_ValidParameter_ID = _gl.GetSuspenseBalancing_Acct();
            _SuspenseError_Acct = MAccount.Get(GetCtx(), VAB_Acct_ValidParameter_ID);
            return _SuspenseError_Acct;
        }

        /// <summary>
        /// Get Due To Account for Segment
        /// </summary>
        /// <param name="segment">ignored</param>
        /// <returns>Account</returns>
        public MAccount GetDueTo_Acct(String segment)
        {
            if (_DueTo_Acct != null)
            {
                return _DueTo_Acct;
            }
            if (_gl == null)
            {
                GetAcctSchemaGL();
            }
            int VAB_Acct_ValidParameter_ID = _gl.GetIntercompanyDueTo_Acct();
            _DueTo_Acct = MAccount.Get(GetCtx(), VAB_Acct_ValidParameter_ID);
            return _DueTo_Acct;

        }

      /// <summary>
      /// gain
      /// </summary>
      /// <param name="segment"></param>
      /// <returns></returns>
        public MAccount GetDueFrom_Acct(String segment)
        {
            if (_DueFrom_Acct != null)
            {
                return _DueFrom_Acct;
            }
            if (_gl == null)
            {
                GetAcctSchemaGL();
            }
            int VAB_Acct_ValidParameter_ID = _gl.GetIntercompanyDueFrom_Acct();
            _DueFrom_Acct = MAccount.Get(GetCtx(), VAB_Acct_ValidParameter_ID);
            return _DueFrom_Acct;
        }

        /// <summary>
        /// Get Due From Account for Segment
        /// </summary>
        /// <param name="segment">ignored</param>
        /// <returns>Account</returns>
        public MAccount GetFRPT_RealizedGain_Acct()
        {
            if (_realizedGain_Acct != null)
            {
                return _realizedGain_Acct;
            }
            if (_gl == null)
            {
                GetAcctSchemaGL();
            }

            int VAB_Acct_ValidParameter_ID = _gl.GetFRPT_RealizedGain_Acct();
            if (VAB_Acct_ValidParameter_ID > 0)
                _realizedGain_Acct = MAccount.Get(GetCtx(), VAB_Acct_ValidParameter_ID);
            return _realizedGain_Acct;
        }

       /// <summary>
       /// loass
       /// </summary>
       /// <param name="segment"></param>
       /// <returns></returns>
        public MAccount GetFRPT_RealizedLoss_Acct()
        {
            if (_realizedLoss_Acct != null)
            {
                return _realizedLoss_Acct;
            }
            if (_gl == null)
            {
                GetAcctSchemaGL();
            }
            int VAB_Acct_ValidParameter_ID = _gl.GetFRPT_RealizedLoss_Acct();
            if (VAB_Acct_ValidParameter_ID > 0)
                _realizedLoss_Acct = MAccount.Get(GetCtx(), VAB_Acct_ValidParameter_ID);
            return _realizedLoss_Acct;
        }



        /// <summary>
        /// Is Currency Balancing active
        /// </summary>
        /// <returns>suspense balancing</returns>
        public bool IsCurrencyBalancing()
        {
            if (_gl == null)
            {
                GetAcctSchemaGL();
            }
            return _gl.IsUseCurrencyBalancing();
        }

        /// <summary>
        /// Get Currency Balancing Account
        /// </summary>
        /// <returns>currency balancing account</returns>
        public MAccount GetCurrencyBalancing_Acct()
        {
            if (_CurrencyBalancing_Acct != null)
            {
                return _CurrencyBalancing_Acct;
            }
            if (_gl == null)
            {
                GetAcctSchemaGL();
            }
            int VAB_Acct_ValidParameter_ID = _gl.GetCurrencyBalancing_Acct();
            _CurrencyBalancing_Acct = MAccount.Get(GetCtx(), VAB_Acct_ValidParameter_ID);
            return _CurrencyBalancing_Acct;
        }

        /// <summary>
        /// Create Commitment Accounting
        /// </summary>
        /// <returns>true if creaet commitments</returns>
        public bool IsCreateCommitment()
        {
            String s = GetCommitmentType();
            if (s == null)
            {
                return false;
            }
            return COMMITMENTTYPE_CommitmentOnly.Equals(s)
                || COMMITMENTTYPE_CommitmentReservation.Equals(s);
        }

        /// <summary>
        /// Create Commitment/Reservation Accounting
        /// </summary>
        /// <returns>true if create reservations</returns>
        public bool IsCreateReservation()
        {
            String s = GetCommitmentType();
            if (s == null)
            {
                return false;
            }
            return COMMITMENTTYPE_CommitmentReservation.Equals(s);
        }

        /// <summary>
        /// Tax Correction Doc_AllocationTax
        /// </summary>
        /// <returns> true if not none</returns>
        public bool IsTaxCorrection()
        {
            return !GetTaxCorrectionType().Equals(TAXCORRECTIONTYPE_None);
        }

        /// <summary>
        /// Tax Correction for Discount
        /// </summary>
        /// <returns>true if tax is corrected for Discount </returns>
        public bool IsTaxCorrectionDiscount()
        {
            return GetTaxCorrectionType().Equals(TAXCORRECTIONTYPE_DiscountOnly)
                || GetTaxCorrectionType().Equals(TAXCORRECTIONTYPE_Write_OffAndDiscount);
        }

        /// <summary>
        /// Tax Correction for WriteOff
        /// </summary>
        /// <returns>true if tax is corrected for WriteOff </returns>
        public bool IsTaxCorrectionWriteOff()
        {
            return GetTaxCorrectionType().Equals(TAXCORRECTIONTYPE_Write_OffOnly)
                || GetTaxCorrectionType().Equals(TAXCORRECTIONTYPE_Write_OffAndDiscount);
        }

        /// <summary>
        /// Get AcctSchema Defaults
        /// </summary>
        /// <returns>defaults</returns>
        public MVABAccountBookDefault GetAcctSchemaDefault()
        {
            if (_default == null)
            {
                _default = MVABAccountBookDefault.Get(GetCtx(), GetVAB_AccountBook_ID());
            }
            if (_default == null)
            {
                throw new Exception("No Default Definition for VAB_AccountBook_ID=" + GetVAB_AccountBook_ID());
            }
            return _default;
        }

        /// <summary>
        /// Does the dateAcct fall in the range
        /// </summary>
        /// <param name="dateAcct"></param>
        /// <returns>true if falls within range	</returns>
        /// <date>07-march-2011</date>
        public Boolean IsAutoPeriodControlOpen(DateTime? dateAcct)
        {
            DateTime today = DateTime.Now.Date;
            DateTime first = TimeUtil.AddDays(today, -GetPeriod_OpenHistory());
            DateTime last = TimeUtil.AddDays(today, GetPeriod_OpenFuture());
            if (dateAcct < first)
            {
                log.Warning(dateAcct + " before first day - " + first);
                return false;
            }
            if (dateAcct > last)
            {
                log.Warning(dateAcct + " after last day - " + first);
                return false;
            }
            return true;
        }
    }


}
