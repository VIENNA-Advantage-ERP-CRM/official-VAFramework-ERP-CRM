/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MCharge
 * Purpose        : Charge Modle
 * Class Used     : X_C_Charge
 * Chronological    Development
 * Raghunandan     23-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
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
using java.math;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MCharge : X_C_Charge
    {
        /**
         * 
         */
        private const long serialVedrionUID = 1L;


        /**
         *  Get Charge Account
         *  @param C_Charge_ID charge
         *  @param as account schema
         *  @param amount amount for expense(+)/revenue(-)
         *  @return Charge Account or null
         */
        public static MAccount GetAccount(int C_Charge_ID, MAcctSchema aSchema, Decimal amount)
        {
            if (C_Charge_ID == 0 || aSchema == null)
                return null;

            int acct_index = 1;     //  Expense (positive amt)
            if (amount < 0)
            {
                acct_index = 2;     //  Revenue (negative amt) 
            }

            String sql = "SELECT CH_Expense_Acct, CH_Revenue_Acct FROM C_Charge_Acct WHERE C_Charge_ID=" + C_Charge_ID + " AND C_AcctSchema_ID=" + aSchema.GetC_AcctSchema_ID();
            int Account_ID = 0;

            IDataReader dr = null;
            try
            {
                //	PreparedStatement pstmt = DataBase.prepareStatement(sql, null);
                //	pstmt.setInt (1, C_Charge_ID);
                //	pstmt.setInt (2, aSchema.getC_AcctSchema_ID());
                //	ResultSet dr = pstmt.executeQuery();
                dr = DataBase.DB.ExecuteReader(sql, null, null);

                if (dr.Read())
                    Account_ID = Utility.Util.GetValueOfInt(dr[acct_index - 1].ToString());
                dr.Close();
                //pstmt.close();
            }
            catch (SqlException e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
                return null;
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
            }

            //	No account
            if (Account_ID == 0)
            {
                _log.Severe("NO account for C_Charge_ID=" + C_Charge_ID);
                return null;
            }

            //	Return Account
            MAccount acct = MAccount.Get(aSchema.GetCtx(), Account_ID);
            return acct;


        }   //  getAccount

        /**
         * 	Get MCharge from Cache
         *	@param ctx context
         *	@param C_Charge_ID id
         *	@return MCharge
         */
        public static MCharge Get(Ctx ctx, int C_Charge_ID)
        {
            int key = C_Charge_ID;
            MCharge retValue = _cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MCharge(ctx, C_Charge_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }	//	get

        /**	Cache						*/
        private static CCache<int, MCharge> _cache = new CCache<int, MCharge>("C_Charge", 10);

        //	Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MCharge).FullName);


        /**************************************************************************
         * 	Standard Constructor
         *	@param ctx context
         *	@param C_Charge_ID id
         *	@param trxName transaction
         */
        public MCharge(Ctx ctx, int C_Charge_ID, Trx trxName) :
            base(ctx, C_Charge_ID, trxName)
        {
            //super (ctx, C_Charge_ID, null);
            if (C_Charge_ID == 0)
            {
                SetChargeAmt(Env.ZERO);
                SetIsSameCurrency(false);
                SetIsSameTax(false);
                SetIsTaxIncluded(false);	// N
                //	setName (null);
                //	setC_TaxCategory_ID (0);
            }
        }	//	MCharge

        /**
         * 	Load Constructor
         *	@param ctx ctx
         *	@param dr result set
         *	@param trxName transaction
         */
        public MCharge(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
            //super(ctx, dr, trxName);
        }	//	MCharge

        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return success
         */
        //@Override
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            int _client_ID = 0;
            StringBuilder _sql = new StringBuilder();
            //_sql.Append("Select count(*) from  ad_table where tablename like 'FRPT_Charge_Acct'");
            //_sql.Append("SELECT count(*) FROM all_objects WHERE object_type IN ('TABLE') AND (object_name)  = UPPER('FRPT_Charge_Acct')  AND OWNER LIKE '" + DB.GetSchema() + "'");
            _sql.Append(DBFunctionCollection.CheckTableExistence(DB.GetSchema(), "FRPT_Charge_Acct"));
            int count = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
            if (count > 0)
            {
                _sql.Clear();
                _sql.Append("Select L.Value From Ad_Ref_List L inner join AD_Reference r on R.AD_REFERENCE_ID=L.AD_REFERENCE_ID where r.name='FRPT_RelatedTo' and l.name='Charge'");
                var relatedtoChrge = Convert.ToString(DB.ExecuteScalar(_sql.ToString()));

                PO chrgact = null;
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
                        DataSet ds = new DataSet();
                        ds = DB.ExecuteDataset(_sql.ToString(), null);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                //DataSet ds2 = new DataSet();
                                string _relatedTo = ds.Tables[0].Rows[i]["Frpt_Relatedto"].ToString();
                                if (_relatedTo != "")
                                {
                                   
                                    if (_relatedTo == relatedtoChrge)
                                    {
                                        _sql.Clear();
                                        _sql.Append("Select COUNT(*) From C_Charge Bp Left Join Frpt_Charge_Acct ca On Bp.C_Charge_ID=ca.C_Charge_ID And ca.Frpt_Acctdefault_Id=" + ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"] + " WHERE Bp.IsActive='Y' AND Bp.AD_Client_ID=" + _client_ID + " AND ca.C_Validcombination_Id = " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Validcombination_Id"]) + " AND Bp.C_Charge_ID = " + GetC_Charge_ID());
                                        int recordFound = Convert.ToInt32(DB.ExecuteScalar(_sql.ToString(), null, Get_Trx()));
                                        //ds2 = DB.ExecuteDataset(_sql.ToString(), null);
                                        //if (ds2 != null && ds2.Tables[0].Rows.Count > 0)
                                        //{
                                        //    for (int j = 0; j < ds2.Tables[0].Rows.Count; j++)
                                        //    {
                                        //        int value = Util.GetValueOfInt(ds2.Tables[0].Rows[j]["Frpt_Acctdefault_Id"]);
                                        //        if (value == 0)
                                        //        {
                                                    //chrgact = new X_FRPT_Charge_Acct(GetCtx(), 0, null);
                                        if (recordFound == 0)
                                        {
                                            chrgact = MTable.GetPO(GetCtx(), "FRPT_Charge_Acct", 0, null);
                                            chrgact.Set_ValueNoCheck("C_Charge_ID", Util.GetValueOfInt(GetC_Charge_ID()));
                                            chrgact.Set_ValueNoCheck("AD_Org_ID", 0);
                                            chrgact.Set_ValueNoCheck("FRPT_AcctDefault_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));
                                            chrgact.Set_ValueNoCheck("C_ValidCombination_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Validcombination_Id"]));
                                            chrgact.Set_ValueNoCheck("C_AcctSchema_ID", _AcctSchema_ID);
                                            if (!chrgact.Save())
                                            {

                                            }
                                        }
                                        //        }
                                        //    }
                                        //}
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (newRecord & success && (String.IsNullOrEmpty(GetCtx().GetContext("#DEFAULT_ACCOUNTING_APPLICABLE")) || Util.GetValueOfString(GetCtx().GetContext("#DEFAULT_ACCOUNTING_APPLICABLE")) == "Y"))
                {
                    success = Insert_Accounting("C_Charge_Acct", "C_AcctSchema_Default", null);

                    //Karan. work done to show message if data not saved in accounting tab. but will save data in current tab.
                    // Before this, data was being saved but giving message "record not saved".
                    if (!success)
                    {
                        log.SaveWarning("AcctNotSaved", "");
                        return true;
                    }
                }
            }
            return success;
        }


        /**
         * 	Before Delete
         *	@return true
         */
        //	@Override
        protected override Boolean BeforeDelete()
        {
            return Delete_Accounting("C_Charge_Acct");
        }	//	beforeDelete

    }
}
