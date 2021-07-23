/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MLocation
 * Purpose        : used for C_AcctSchema
 * Class Used     : X_C_AcctSchema
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
    public class MAcctSchema : X_C_AcctSchema
    {
        /** Costing Currency Precision		*/
        private int _costPrecision = -1;
        /** Accounting Currency Precision	*/
        private int _stdPrecision = -1;
        /** Element List       */
        private MAcctSchemaElement[] _elements = null;
        // Default Info		
        private MAcctSchemaDefault _default = null;

        //Only Post Org Childs			
        private int?[] _onlyOrgs = null;
        //Only Post Org				
        private MOrg _onlyOrg = null;

        //GL Info				
        private MAcctSchemaGL _gl = null;
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
        /// <param name="C_AcctSchema_ID">schema id</param>
        /// <returns>Accounting schema</returns>
        public static MAcctSchema Get(Ctx ctx, int C_AcctSchema_ID)
        {
            return Get(ctx, C_AcctSchema_ID, null);
        }	//	get

        /// <summary>
        /// Get AcctSchema Element
        /// </summary>
        /// <param name="elementType">elementType segment type - AcctSchemaElement.ELEMENTTYPE_</param>
        /// <returns>AcctSchemaElement</returns>
        public MAcctSchemaElement GetAcctSchemaElement(String elementType)
        {
            if (_elements == null)
            {
                GetAcctSchemaElements();
            }
            for (int i = 0; i < _elements.Length; i++)
            {
                MAcctSchemaElement ase = _elements[i];
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
        /// <param name="C_AcctSchema_ID">schema id</param>
        /// <param name="trxName">optional trx</param>
        /// <returns>Accounting schema</returns>
        public static MAcctSchema Get(Ctx ctx, int C_AcctSchema_ID, Trx trxName)
        {
            //  Check Cache
            int key = C_AcctSchema_ID;
            MAcctSchema retValue = (MAcctSchema)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MAcctSchema(ctx, C_AcctSchema_ID, trxName);
            if (trxName == null)
                _cache.Add(key, retValue);
            return retValue;
        }	//	get

        /// <summary>
        /// Get AccountSchema of Client
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Client_ID">client or 0 for all</param>
        /// <returns>Array of AcctSchema of Client</returns>
        public static MAcctSchema[] GetClientAcctSchema(Ctx ctx, int AD_Client_ID)
        {
            return GetClientAcctSchema(ctx, AD_Client_ID, null);
        }	//	getClientAcctSchema

        /// <summary>
        /// Get AccountSchema of Client
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Client_ID">client or 0 for all</param>
        /// <param name="trxName">optional trx </param>
        /// <returns>Array of AcctSchema of Client</returns>
        public static MAcctSchema[] GetClientAcctSchema(Ctx ctx, int AD_Client_ID, Trx trxName)
        {
            //  Check Cache
            int key = AD_Client_ID;
            if (_schema.ContainsKey(key))
                return (MAcctSchema[])_schema[key];

            //  Create New
            List<MAcctSchema> list = new List<MAcctSchema>();
            MClientInfo info = MClientInfo.Get(ctx, AD_Client_ID, trxName);
            MAcctSchema ass = MAcctSchema.Get(ctx, info.GetC_AcctSchema1_ID(), trxName);
            if (ass.Get_ID() != 0 && trxName == null)
                list.Add(ass);

            //	Other
            String sql = "SELECT C_AcctSchema_ID FROM C_AcctSchema acs "
                + "WHERE IsActive='Y'"
                + " AND EXISTS (SELECT * FROM C_AcctSchema_GL gl WHERE acs.C_AcctSchema_ID=gl.C_AcctSchema_ID)"
                + " AND EXISTS (SELECT * FROM C_AcctSchema_Default d WHERE acs.C_AcctSchema_ID=d.C_AcctSchema_ID)";
            if (AD_Client_ID != 0)
            {
                sql += " AND AD_Client_ID=" + AD_Client_ID;
            }
            sql += " ORDER BY C_AcctSchema_ID";

            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql, null, trxName);
                while (dr.Read())
                {
                    int id = Utility.Util.GetValueOfInt(dr[0].ToString());
                    if (id != info.GetC_AcctSchema1_ID())	//	already in list
                    {
                        ass = MAcctSchema.Get(ctx, id, trxName);
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
            MAcctSchema[] retValue = new MAcctSchema[list.Count];
            retValue = list.ToArray();
            if (trxName == null)
                _schema.Add(key, retValue);
            return retValue;
        }   //  getClientAcctSchema

        // by amit 23-12-2015
        public static MAcctSchema[] GetClientAcctSchemas(Ctx ctx, int AD_Client_ID, Trx trxName)
        {
            //  Check Cache
            int key = AD_Client_ID;
            if (_schema.ContainsKey(key))
                return (MAcctSchema[])_schema[key];

            //  Create New
            List<MAcctSchema> list = new List<MAcctSchema>();
            MClientInfo info = MClientInfo.Get(ctx, AD_Client_ID, trxName);
            MAcctSchema ass = MAcctSchema.Get(ctx, info.GetC_AcctSchema1_ID(), trxName);
            if (ass.Get_ID() != 0 && trxName == null)
                list.Add(ass);

            //	Other
            String sql = "SELECT C_AcctSchema_ID FROM C_AcctSchema acs "
                + "WHERE IsActive='Y'";
            if (AD_Client_ID != 0)
            {
                sql += " AND AD_Client_ID=" + AD_Client_ID;
            }
            sql += " ORDER BY C_AcctSchema_ID";

            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql, null, trxName);
                while (dr.Read())
                {
                    int id = Utility.Util.GetValueOfInt(dr[0].ToString());
                    if (id != info.GetC_AcctSchema1_ID())	//	already in list
                    {
                        ass = MAcctSchema.Get(ctx, id, trxName);
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
            MAcctSchema[] retValue = new MAcctSchema[list.Count];
            retValue = list.ToArray();
            if (trxName == null)
                _schema.Add(key, retValue);
            return retValue;
        }   //  getClientAcctSchema


        /** Cache of Client AcctSchema Arrays		**/
        private static CCache<int, MAcctSchema[]> _schema = new CCache<int, MAcctSchema[]>("AD_ClientInfo", 3);	//  3 clients
        /**	Cache of AcctSchemas 					**/
        private static CCache<int, MAcctSchema> _cache = new CCache<int, MAcctSchema>("C_AcctSchema", 3);	//  3 accounting schemas
        /**	Logger			*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MAcctSchema).FullName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="currency"></param>
        public MAcctSchema(MClient client, KeyNamePair currency)
            : this(client.GetCtx(), 0, client.Get_TrxName())
        {
            SetClientOrg(client);
            SetC_Currency_ID(currency.GetKey());
            SetName(client.GetName() + " " + GetGAAP() + "/" + Get_ColumnCount() + " " + currency.GetName());
        }	//	MAcctSchema

        /// <summary>
        ///Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_AcctSchema_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MAcctSchema(Ctx ctx, int C_AcctSchema_ID, Trx trxName)
            : base(ctx, C_AcctSchema_ID, trxName)
        {
            //super (ctx, C_AcctSchema_ID, trxName);
            if (C_AcctSchema_ID == 0)
            {
                //	setC_Currency_ID (0);
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
                MCurrency cur = MCurrency.Get(GetCtx(), GetC_Currency_ID());
                _stdPrecision = cur.GetStdPrecision();
                _costPrecision = cur.GetCostingPrecision();
            }
            return _stdPrecision;
        }

        /**
        *  AcctSchema Elements
        *  @return ArrayList of AcctSchemaElement
        */
        public MAcctSchemaElement[] GetAcctSchemaElements()
        {
            if (_elements == null)
                _elements = MAcctSchemaElement.GetAcctSchemaElements(this);
            return _elements;
        }

        protected override bool BeforeSave(bool newRecord)
        {
            if (GetAD_Org_ID() != 0)
                SetAD_Org_ID(0);
            if (base.GetTaxCorrectionType() == null)
                SetTaxCorrectionType(IsDiscountCorrectsTax() ? TAXCORRECTIONTYPE_Write_OffAndDiscount : TAXCORRECTIONTYPE_None);

            // Applied check for Table to enable support with Framework database.
            if (PO.Get_Table_ID("M_CostType") > 0)
            {
                CheckCosting();
            }
            //	Check Primary
            if (GetAD_OrgOnly_ID() != 0)
            {
                MClientInfo info = MClientInfo.Get(GetCtx(), GetAD_Client_ID());
                if (info.GetC_AcctSchema1_ID() == GetC_AcctSchema_ID())
                    SetAD_OrgOnly_ID(0);
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
                ct.SetClientOrg(GetAD_Client_ID(), 0);
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
        /// <param name="AD_Org_ID"></param>
        /// <returns>true if to skip</returns>
        public bool IsSkipOrg(int AD_Org_ID)
        {
            if (GetAD_OrgOnly_ID() == 0)
            {
                return false;
            }
            //	Only Organization
            if (GetAD_OrgOnly_ID() == AD_Org_ID)
            {
                return false;
            }
            if (_onlyOrg == null)
                _onlyOrg = MOrg.Get(GetCtx(), GetAD_OrgOnly_ID());
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
                if (AD_Org_ID == Utility.Util.GetValueOfInt(_onlyOrgs[i]))
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
        public MAcctSchemaGL GetAcctSchemaGL()
        {
            if (_gl == null)
            {
                _gl = MAcctSchemaGL.Get(GetCtx(), GetC_AcctSchema_ID());
            }
            if (_gl == null)
            {
                throw new Exception("No GL Definition for C_AcctSchema_ID=" + GetC_AcctSchema_ID());
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
            int C_ValidCombination_ID = _gl.GetSuspenseBalancing_Acct();
            _SuspenseError_Acct = MAccount.Get(GetCtx(), C_ValidCombination_ID);
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
            int C_ValidCombination_ID = _gl.GetIntercompanyDueTo_Acct();
            _DueTo_Acct = MAccount.Get(GetCtx(), C_ValidCombination_ID);
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
            int C_ValidCombination_ID = _gl.GetIntercompanyDueFrom_Acct();
            _DueFrom_Acct = MAccount.Get(GetCtx(), C_ValidCombination_ID);
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

            int C_ValidCombination_ID = _gl.GetFRPT_RealizedGain_Acct();
            if (C_ValidCombination_ID > 0)
                _realizedGain_Acct = MAccount.Get(GetCtx(), C_ValidCombination_ID);
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
            int C_ValidCombination_ID = _gl.GetFRPT_RealizedLoss_Acct();
            if (C_ValidCombination_ID > 0)
                _realizedLoss_Acct = MAccount.Get(GetCtx(), C_ValidCombination_ID);
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
            int C_ValidCombination_ID = _gl.GetCurrencyBalancing_Acct();
            _CurrencyBalancing_Acct = MAccount.Get(GetCtx(), C_ValidCombination_ID);
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
        public MAcctSchemaDefault GetAcctSchemaDefault()
        {
            if (_default == null)
            {
                _default = MAcctSchemaDefault.Get(GetCtx(), GetC_AcctSchema_ID());
            }
            if (_default == null)
            {
                throw new Exception("No Default Definition for C_AcctSchema_ID=" + GetC_AcctSchema_ID());
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
