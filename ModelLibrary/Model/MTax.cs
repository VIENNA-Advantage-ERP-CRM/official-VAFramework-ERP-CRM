/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MTax
 * Purpose        : for workflow
 * Chronological    Development
 * Raghunandan     05-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using java.math;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MTax : X_C_Tax
    {
        #region Private Variables
        //	Cache			
        private static CCache<int, MTax> _cache = new CCache<int, MTax>("C_Tax", 5);
        //	Cache of Client	
        private static CCache<int, MTax[]> _cacheAll = new CCache<int, MTax[]>("C_Tax", 5);
        //	Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MTax).FullName);
        //	100				
        private const Decimal ONEHUNDRED = 100M;
        //	Child Taxes		
        private MTax[] _childTaxes = null;
        // Postal Codes		
        private MTaxPostal[] _postals = null;

        #endregion

        /// <summary>
        /// Get All
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>array list</returns>
        public static MTax[] GetAll(Ctx ctx)
        {
            int AD_Client_ID = ctx.GetAD_Client_ID();
            int key = AD_Client_ID;
            MTax[] retValue = (MTax[])_cacheAll[key];
            if (retValue != null)
                return retValue;

            //	Create it
            String sql = "SELECT * FROM C_Tax WHERE AD_Client_ID=@AD_Client_ID"
                + " ORDER BY C_Country_ID, C_Region_ID, To_Country_ID, To_Region_ID";
            List<MTax> list = new List<MTax>();
            //PreparedStatement pstmt = null;
            DataSet ds;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@AD_Client_ID", AD_Client_ID);
                ds = new DataSet();
                ds = DataBase.DB.ExecuteDataset(sql, param);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    MTax tax = new MTax(ctx, dr, null);
                    _cache.Add(tax.GetC_Tax_ID(), tax);
                    list.Add(tax);
                }
                ds = null;
                //pstmt.close ();
                //pstmt = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                ds = null;
            }

            //	Create Array
            retValue = new MTax[list.Count];
            retValue = list.ToArray();
            //
            _cacheAll.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Get Tax from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Tax_ID">id</param>
        /// <returns>MTax</returns>
        public static MTax Get(Ctx ctx, int C_Tax_ID)
        {
            int key = C_Tax_ID;
            MTax retValue = (MTax)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MTax(ctx, C_Tax_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Tax_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MTax(Ctx ctx, int C_Tax_ID, Trx trxName) :
            base(ctx, C_Tax_ID, trxName)
        {

            if (C_Tax_ID == 0)
            {
                //	setC_Tax_ID (0);		PK
                SetIsDefault(false);
                SetIsDocumentLevel(true);
                SetIsSummary(false);
                SetIsTaxExempt(false);
                //	setName (null);
                SetRate(Env.ZERO);
                SetRequiresTaxCertificate(false);
                //	setC_TaxCategory_ID (0);	//	FK
                SetSOPOType(SOPOTYPE_Both);
                SetValidFrom(TimeUtil.GetDay(1990, 1, 1));
                SetIsSalesTax(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">transaction</param>
        public MTax(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// New Constructor
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="Name">Name</param>
        /// <param name="Rate"></param>
        /// <param name="C_TaxCategory_ID"></param>
        /// <param name="trxName">transaction</param>
        public MTax(Ctx ctx, String Name, Decimal Rate, int C_TaxCategory_ID, Trx trxName)
            : this(ctx, 0, trxName)
        {

            SetName(Name);
            SetRate(Rate);
            SetC_TaxCategory_ID(C_TaxCategory_ID);	//	FK
        }

        /// <summary>
        /// Get Child Taxes
        /// </summary>
        /// <param name="requery">reload</param>
        /// <returns>array of taxes or null</returns>
        public MTax[] GetChildTaxes(Boolean requery)
        {
            if (!IsSummary())
                return null;
            if (_childTaxes != null && !requery)
                return _childTaxes;
            //
            String sql = "SELECT * FROM C_Tax WHERE Parent_Tax_ID=" + GetC_Tax_ID();
            List<MTax> list = new List<MTax>();
            DataSet ds;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MTax(GetCtx(), dr, Get_TrxName()));

                }
                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                ds = null;
            }

            _childTaxes = new MTax[list.Count];
            _childTaxes = list.ToArray();
            return _childTaxes;
        }

        /// <summary>
        /// Get Postal Qualifiers
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns> array of postal codes</returns>
        public MTaxPostal[] GetPostals(Boolean requery)
        {
            if (_postals != null && !requery)
                return _postals;

            String sql = "SELECT * FROM C_TaxPostal WHERE C_Tax_ID=" + GetC_Tax_ID() + " ORDER BY Postal, Postal_To";
            List<MTaxPostal> list = new List<MTaxPostal>();
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                while (dr.Read())
                {
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }

            _postals = new MTaxPostal[list.Count];
            _postals = list.ToArray();
            return _postals;
        }

        /// <summary>
        /// Do we have Postal Codes
        /// </summary>
        /// <returns>true if postal codes exist</returns>
        public Boolean IsPostal()
        {
            return GetPostals(false).Length > 0;
        }

        /// <summary>
        /// Is Zero Tax
        /// </summary>
        /// <returns>true if tax rate is 0</returns>
        public Boolean IsZeroTax()
        {
            return Env.ZERO.CompareTo(GetRate()) == 0;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MTax[");
            sb.Append(Get_ID()).Append(",").Append(GetName())
                .Append(", SO/PO=").Append(GetSOPOType())
                .Append(",Rate=").Append(GetRate())
                .Append(",C_TaxCategory_ID=").Append(GetC_TaxCategory_ID())
                .Append(",Summary=").Append(IsSummary())
                .Append(",Parent=").Append(GetParent_Tax_ID())
                .Append(",Country=").Append(GetC_Country_ID()).Append("|").Append(GetTo_Country_ID())
                .Append(",Region=").Append(GetC_Region_ID()).Append("|").Append(GetTo_Region_ID())
                .Append(",From=").Append(GetValidFrom())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Calculate Tax - no rounding
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="taxIncluded">if true tax is calculated from gross otherwise from net </param>
        /// <param name="scale"></param>
        /// <returns>tax amount</returns>
        public Decimal CalculateTax(Decimal amount, Boolean taxIncluded, int scale)
        {
            //	Null Tax
            if (IsZeroTax())
                return Env.ZERO;

            // VIS0060: Work done to Round off Tax Amount based on setting taken on Tenant.
            MClient client = MClient.Get(GetCtx());
            if (client.Get_ColumnIndex("IsRoundLineTaxAmt") >= 0 && !client.IsRoundLineTaxAmt())
            {
                scale = 7;      //SET 7 By VIS0228 As discussed with Mukesh Sir
            }
            Decimal multiplier = Decimal.Round(Decimal.Divide(GetRate(), ONEHUNDRED), 12, MidpointRounding.AwayFromZero);
            //BigDecimal multiplier = getRate().divide(ONEHUNDRED, 12, BigDecimal.ROUND_HALF_UP);		
            Decimal? tax = null;
            if (!taxIncluded)	//	$100 * 6 / 100 == $6 == $100 * 0.06
            {
                tax = Decimal.Multiply(amount, multiplier);
            }
            else			//	$106 - ($106 / (100+6)/100) == $6 == $106 - ($106/1.06)
            {
                multiplier = Decimal.Add(multiplier, Env.ONE);
                //BigDecimal bbase = amount.divide(multiplier, 12, BigDecimal.ROUND_HALF_UP); 
                Decimal bbase = Decimal.Divide(amount, multiplier);
                bbase = Decimal.Round(bbase, 12, MidpointRounding.AwayFromZero);


                tax = Decimal.Subtract(amount, bbase);
            }
            Decimal finalTax = Decimal.Round((Decimal)tax, scale, MidpointRounding.AwayFromZero);
            //BigDecimal finalTax = tax.setScale(scale, BigDecimal.ROUND_HALF_UP);
            log.Fine("calculateTax " + amount
                + " (incl=" + taxIncluded + ",mult=" + multiplier + ",scale=" + scale
                + ") = " + finalTax + " [" + tax + "]");
            return finalTax;
        }

        /// <summary>
        /// Calculate Tax and Surcharge
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="taxIncluded">if true tax is calculated from gross otherwise from net </param>
        /// <param name="scale"></param>
        /// <returns>tax amount</returns>
        public Decimal CalculateSurcharge(Decimal amount, Boolean taxIncluded, int scale, out Decimal SurchargeAmt)
        {
            //	Null Tax
            SurchargeAmt = Env.ZERO;
            Decimal TaxAmt = Env.ZERO;

            if (IsZeroTax())
            {
                return Env.ZERO;
            }

            Decimal multiplier = Env.ZERO;
            Decimal taxBase = amount;
            Decimal taxRate = GetRate();
            MTax surTax = MTax.Get(GetCtx(), GetSurcharge_Tax_ID());
            Decimal surRate = surTax.GetRate();

            // for Surcharge Calculation type - Line Amount + Tax Amount
            if (GetSurchargeType() == MTax.SURCHARGETYPE_LineAmountPlusTax)
            {

                if (!taxIncluded)   //	$100 * 6 / 100 == $6 == $100 * 0.06
                {
                    // Calculate Tax Amount on Line Amount
                    multiplier = Decimal.Round(Decimal.Divide(taxRate, 100), 12, MidpointRounding.AwayFromZero);
                    TaxAmt = Decimal.Multiply(amount, multiplier);

                    // Calculate Surcharge Tax Amount on Line Amount + Tax Amount
                    amount = amount + TaxAmt;
                    multiplier = Decimal.Round(Decimal.Divide(surRate, 100), 12, MidpointRounding.AwayFromZero);
                    SurchargeAmt = Decimal.Multiply(amount, multiplier);

                    TaxAmt = Decimal.Round(TaxAmt, scale, MidpointRounding.AwayFromZero);
                    SurchargeAmt = Decimal.Round(SurchargeAmt, scale, MidpointRounding.AwayFromZero);
                }
                else            //	$106 - ($106 / (100+6)/100) == $6 == $106 - ($106/1.06)
                {
                    multiplier = Decimal.Round(Decimal.Divide(surRate, 100), 12, MidpointRounding.AwayFromZero);
                    multiplier = Decimal.Add(multiplier, Env.ONE);

                    taxBase = Decimal.Divide(amount, multiplier);
                    taxBase = Decimal.Round(taxBase, 12, MidpointRounding.AwayFromZero);
                    amount = taxBase;

                    multiplier = Decimal.Round(Decimal.Divide(taxRate, 100), 12, MidpointRounding.AwayFromZero);
                    multiplier = Decimal.Add(multiplier, Env.ONE);

                    taxBase = Decimal.Divide(amount, multiplier);

                    TaxAmt = CalculateTax(taxBase, false, scale);
                    SurchargeAmt = surTax.CalculateTax(Decimal.Add(taxBase, TaxAmt), false, scale);
                }
            }
            // for Surcharge Calculation type - Line Amount 
            else if (GetSurchargeType() == MTax.SURCHARGETYPE_LineAmount)
            {
                if (!taxIncluded)   //	$100 * 6 / 100 == $6 == $100 * 0.06
                {
                    // Calculate Tax Amount on Line Amount
                    multiplier = Decimal.Round(Decimal.Divide(taxRate, 100), 12, MidpointRounding.AwayFromZero);
                    TaxAmt = Decimal.Multiply(amount, multiplier);
                    TaxAmt = Decimal.Round(TaxAmt, scale, MidpointRounding.AwayFromZero);

                    // Calculate Surcharge Tax Amount on Line Amount                
                    multiplier = Decimal.Round(Decimal.Divide(surRate, 100), 12, MidpointRounding.AwayFromZero);
                    SurchargeAmt = Decimal.Multiply(amount, multiplier);
                    SurchargeAmt = Decimal.Round(SurchargeAmt, scale, MidpointRounding.AwayFromZero);
                }
                else            //	$106 - ($106 / (100+6)/100) == $6 == $106 - ($106/1.06)
                {
                    multiplier = Decimal.Round(Decimal.Divide(Decimal.Add(taxRate, surRate), 100), 12, MidpointRounding.AwayFromZero);
                    multiplier = Decimal.Add(multiplier, Env.ONE);

                    taxBase = Decimal.Divide(amount, multiplier);
                    taxBase = Decimal.Round(taxBase, 12, MidpointRounding.AwayFromZero);

                    TaxAmt = CalculateTax(taxBase, false, scale);
                    SurchargeAmt = surTax.CalculateTax(taxBase, false, scale);
                }
            }
            // for Surcharge Calculation type - Tax Amount
            else
            {
                if (!taxIncluded)   //	$100 * 6 / 100 == $6 == $100 * 0.06
                {
                    // Calculate Tax Amount on Line Amount
                    multiplier = Decimal.Round(Decimal.Divide(taxRate, 100), 12, MidpointRounding.AwayFromZero);
                    TaxAmt = Decimal.Multiply(amount, multiplier);

                    // Calculate Surcharge Tax Amount on Line Amount + Tax Amount                    
                    multiplier = Decimal.Round(Decimal.Divide(surRate, 100), 12, MidpointRounding.AwayFromZero);
                    SurchargeAmt = Decimal.Multiply(TaxAmt, multiplier);

                    TaxAmt = Decimal.Round(TaxAmt, scale, MidpointRounding.AwayFromZero);
                    SurchargeAmt = Decimal.Round(SurchargeAmt, scale, MidpointRounding.AwayFromZero);
                }
                else            //	$106 - ($106 / (100+6)/100) == $6 == $106 - ($106/1.06)
                {
                    multiplier = Decimal.Round(Decimal.Divide(surRate, 100), 12, MidpointRounding.AwayFromZero);
                    multiplier = Decimal.Multiply(taxRate, multiplier);
                    multiplier = Decimal.Add(taxRate, multiplier);
                    multiplier = Decimal.Round(Decimal.Divide(multiplier, 100), 12, MidpointRounding.AwayFromZero);
                    multiplier = Decimal.Add(multiplier, Env.ONE);

                    taxBase = Decimal.Divide(amount, multiplier);
                    taxBase = Decimal.Round(taxBase, 12, MidpointRounding.AwayFromZero);

                    TaxAmt = CalculateTax(taxBase, false, scale);
                    SurchargeAmt = surTax.CalculateTax(TaxAmt, false, scale);
                }
            }
            return TaxAmt;
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <param name="success"></param>
        /// <returns>success</returns>
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            PO tax = null;
            int _client_ID = 0;
            StringBuilder _sql = new StringBuilder();
            //_sql.Append("Select count(*) from  ad_table where tablename like 'FRPT_TaxRate_Acct'");
            //_sql.Append("SELECT count(*) FROM all_objects WHERE object_type IN ('TABLE') AND (object_name)  = UPPER('FRPT_TaxRate_Acct')  AND OWNER LIKE '" + DB.GetSchema() + "'");
            _sql.Append(DBFunctionCollection.CheckTableExistence(DB.GetSchema(), "FRPT_TaxRate_Acct"));
            int count = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
            if (count > 0)
            {
                _sql.Clear();
                _sql.Append("Select L.Value From Ad_Ref_List L inner join AD_Reference r on R.AD_REFERENCE_ID=L.AD_REFERENCE_ID where r.name='FRPT_RelatedTo' and l.name='Tax Rate'");
                var relatedtoTax = Convert.ToString(DB.ExecuteScalar(_sql.ToString()));
                _client_ID = GetAD_Client_ID();
                _sql.Clear();
                _sql.Append("select C_AcctSchema_ID from C_AcctSchema where AD_CLIENT_ID=" + _client_ID);
                DataSet ds3 = new DataSet();
                ds3 = DB.ExecuteDataset(_sql.ToString(), null);
                if (ds3 != null && ds3.Tables[0].Rows.Count > 0)
                {
                    for (int k = 0; k < ds3.Tables[0].Rows.Count; k++)
                    {
                        int _AcctSchema_ID = Util.GetValueOfInt(ds3.Tables[0].Rows[k]["C_AcctSchema_ID"]);
                        _sql.Clear();
                        _sql.Append("Select Frpt_Acctdefault_Id,C_Validcombination_Id,Frpt_Relatedto From Frpt_Acctschema_Default Where ISACTIVE='Y' AND AD_CLIENT_ID=" + _client_ID + "AND C_Acctschema_Id=" + _AcctSchema_ID);
                        DataSet ds = DB.ExecuteDataset(_sql.ToString(), null);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                string _relatedTo = ds.Tables[0].Rows[i]["Frpt_Relatedto"].ToString();
                                if (_relatedTo != "" && (_relatedTo == relatedtoTax))
                                {
                                    _sql.Clear();
                                    _sql.Append("Select COUNT(*) From C_Tax Bp Left Join FRPT_TaxRate_Acct ca On Bp.C_Tax_ID=ca.C_Tax_ID And ca.Frpt_Acctdefault_Id=" + ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"] + " WHERE Bp.IsActive='Y' AND Bp.AD_Client_ID=" + _client_ID + " AND ca.C_Validcombination_Id = " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Validcombination_Id"]) + " AND Bp.C_Tax_ID = " + GetC_Tax_ID());
                                    int recordFound = Convert.ToInt32(DB.ExecuteScalar(_sql.ToString(), null, Get_Trx()));
                                    if (recordFound == 0)
                                    {
                                        tax = MTable.GetPO(GetCtx(), "FRPT_TaxRate_Acct", 0, null);
                                        tax.Set_ValueNoCheck("AD_Org_ID", 0);
                                        tax.Set_ValueNoCheck("C_Tax_ID", Util.GetValueOfInt(GetC_Tax_ID()));
                                        tax.Set_ValueNoCheck("FRPT_AcctDefault_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));
                                        tax.Set_ValueNoCheck("C_ValidCombination_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Validcombination_Id"]));
                                        tax.Set_ValueNoCheck("C_AcctSchema_ID", _AcctSchema_ID);
                                        if (!tax.Save())
                                        {

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (newRecord & success && (String.IsNullOrEmpty(GetCtx().GetContext("#DEFAULT_ACCOUNTING_APPLICABLE")) || Util.GetValueOfString(GetCtx().GetContext("#DEFAULT_ACCOUNTING_APPLICABLE")) == "Y"))
            {
                bool sucs = Insert_Accounting("C_Tax_Acct", "C_AcctSchema_Default", null);
                //Karan. work done to show message if data not saved in accounting tab. but will save data in current tab.
                // Before this, data was being saved but giving message "record not saved".
                if (!sucs)
                {
                    log.SaveWarning("AcctNotSaved", "");
                }
            }

            return success;
        }

        /// <summary>
        /// Before Delete
        /// </summary>
        /// <returns>true</returns>
        protected override Boolean BeforeDelete()
        {
            return Delete_Accounting("C_Tax_Acct");
        }
    }
}
