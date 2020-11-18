/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MCashBook
 * Purpose        : Cash Book Model
 * Class Used     : X_C_CashBook
 * Chronological    Development
 * Raghunandan     23-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.ProcessEngine;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MCashBook : X_C_CashBook
    {

        //	Cache						
        private static CCache<int, MCashBook> _cache = new CCache<int, MCashBook>("", 20);
        //	Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MCashBook).FullName);
        /**
         * 	Get MCashBook from Cache
         *	@param ctx context
         *	@param C_CashBook_ID id
         *	@return MCashBook
         */
        public static MCashBook Get(Ctx ctx, int C_CashBook_ID)
        {
            int key = C_CashBook_ID;
            MCashBook retValue = (MCashBook)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MCashBook(ctx, C_CashBook_ID, null);
            if (retValue.Get_ID() != 0)
            {
                _cache.Add(key, retValue);
            }
            return retValue;
        }

        /**
         * 	Get CashBook for Org and Currency
         *	@param ctx context
         *	@param AD_Org_ID org
         *	@param C_Currency_ID currency
         *	@return cash book or null
         */
        public static MCashBook Get(Ctx ctx, int AD_Org_ID, int C_Currency_ID)
        {
            //	Try from cache
            //Iterator it = _cache.values().iterator();
            IEnumerator it = _cache.Values.GetEnumerator();
            while (it.MoveNext())
            {
                MCashBook cb = (MCashBook)it.Current;
                if (cb.GetAD_Org_ID() == AD_Org_ID && cb.GetC_Currency_ID() == C_Currency_ID)
                    return cb;
            }

            //	Get from DB
            //SI_0648_1 : if there are multiple "defaults / no default" with in same org and currency, system will create object of highest ID.
            MCashBook retValue = null;
            String sql = "SELECT * FROM C_CashBook "
                + " WHERE AD_Org_ID=" + AD_Org_ID + " AND C_Currency_ID=" + C_Currency_ID
                + " ORDER BY IsDefault DESC,  C_CashBook_id DESC";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MCashBook(ctx, dr, null);
                    int key = retValue.GetC_CashBook_ID();
                    _cache.Add(key, retValue);
                    break;
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, "get", e);
            }
            finally { dt = null; }
            return retValue;
        }

        /***
         * 	Standard Constructor
         *	@param ctx context
         *	@param C_CashBook_ID id
         *	@param trxName transaction
         */
        public MCashBook(Ctx ctx, int C_CashBook_ID, Trx trxName)
            : base(ctx, C_CashBook_ID, trxName)
        {
            if (C_CashBook_ID == 0)
                SetIsDefault(false);
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MCashBook(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return success
         */
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            int _client_ID = 0;
            StringBuilder _sql = new StringBuilder();
            //_sql.Append("Select count(*) from  ad_table where tablename like 'FRPT_Cashbook_Acct'");
            //_sql.Append("SELECT count(*) FROM all_objects WHERE object_type IN ('TABLE') AND (object_name)  = UPPER('FRPT_Cashbook_Acct')  AND OWNER LIKE '" + DB.GetSchema() + "'");
            _sql.Append(DBFunctionCollection.CheckTableExistence(DB.GetSchema(), "FRPT_Cashbook_Acct"));
            int count = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
            if (count > 0)
            {
                _sql.Clear();
                _sql.Append("SELECT L.VALUE FROM AD_REF_LIST L inner join AD_Reference r on R.AD_REFERENCE_ID=L.AD_REFERENCE_ID where r.name='FRPT_RelatedTo' and l.name='CashBook'");
                var relatedtoCashbook = Convert.ToString(DB.ExecuteScalar(_sql.ToString()));

                PO cshbkact = null;
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
                                // DataSet ds2 = new DataSet();
                                string _relatedTo = ds.Tables[0].Rows[i]["Frpt_Relatedto"].ToString();
                                if (_relatedTo != "")
                                {

                                    if (_relatedTo == relatedtoCashbook)
                                    {
                                        _sql.Clear();
                                        _sql.Append("Select COUNT(*) From C_Cashbook Bp Left Join FRPT_Cashbook_Acct ca On Bp.C_CashBook_ID=ca.C_CashBook_ID And ca.Frpt_Acctdefault_Id=" + ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"] + " WHERE Bp.IsActive='Y' AND Bp.AD_Client_ID=" + _client_ID + " AND ca.C_Validcombination_Id = " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Validcombination_Id"]) + " AND Bp.C_CashBook_ID = " + GetC_CashBook_ID());
                                        int recordFound = Convert.ToInt32(DB.ExecuteScalar(_sql.ToString(), null, Get_Trx()));
                                        //ds2 = DB.ExecuteDataset(_sql.ToString(), null);
                                        //if (ds2 != null && ds2.Tables[0].Rows.Count > 0)
                                        //{
                                        //    for (int j = 0; j < ds2.Tables[0].Rows.Count; j++)
                                        //    {
                                        //        int value = Util.GetValueOfInt(ds2.Tables[0].Rows[j]["Frpt_Acctdefault_Id"]);
                                        //        if (value == 0)
                                        //        {
                                        //cshbkact = new X_FRPT_Cashbook_Acct(GetCtx(), 0, null);
                                        if (recordFound == 0)
                                        {
                                            cshbkact = MTable.GetPO(GetCtx(), "FRPT_Cashbook_Acct", 0, null);
                                            cshbkact.Set_ValueNoCheck("AD_Org_ID", 0);
                                            cshbkact.Set_ValueNoCheck("C_CashBook_ID", Util.GetValueOfInt(GetC_CashBook_ID()));
                                            cshbkact.Set_ValueNoCheck("FRPT_AcctDefault_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));
                                            cshbkact.Set_ValueNoCheck("C_ValidCombination_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Validcombination_Id"]));
                                            cshbkact.Set_ValueNoCheck("C_AcctSchema_ID", _AcctSchema_ID);

                                            if (!cshbkact.Save())
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
                    success = Insert_Accounting("C_CashBook_Acct", "C_AcctSchema_Default", null);

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
        protected override bool BeforeDelete()
        {
            return Delete_Accounting("C_Cashbook_Acct");
        }

        /**
       * 	Before Save
       *	@param newRecord new
       *	@return true if valid
       */
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            //Issue ID- SI_0614 in Google Sheet Standard Issues.. On Cashbook currency should not allow to change if any transcation made against Cashbook.
            if (!newRecord && Is_ValueChanged("C_Currency_ID"))
            {
                string sql = "SELECT SUM(total) AS TOTAL FROM  (SELECT COUNT(*) AS TOTAL FROM C_CashBook_Acct WHERE AD_Client_ID= " + GetAD_Client_ID() + " AND C_CashBook_ID =" + GetC_CashBook_ID()
                               + " UNION ALL SELECT COUNT(*) AS TOTAL FROM C_CashLine WHERE AD_Client_ID= " + GetAD_Client_ID() + " AND  C_CashBook_ID = " + GetC_CashBook_ID()
                               + " UNION ALL  SELECT COUNT(*) AS TOTAL FROM C_Cash WHERE AD_Client_ID= " + GetAD_Client_ID() + " AND  C_CashBook_ID =" + GetC_CashBook_ID()
                               + " UNION ALL  SELECT COUNT(*) AS TOTAL FROM C_POS WHERE AD_Client_ID= " + GetAD_Client_ID() + " AND  C_CashBook_ID =" + GetC_CashBook_ID()
                               + " UNION ALL  SELECT COUNT(*) as TOTAL from C_CASHBOOKLINE WHERE AD_Client_ID= " + GetAD_Client_ID() + " AND C_CASHBOOK_ID =" + GetC_CashBook_ID() + ")";

                if (Util.GetValueOfInt(DB.ExecuteScalar(sql)) > 0)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "CouldNotChnageCurrency"));
                    return false;
                }
            }
            //End

            // JID_1582: system will update the Opening balance value in Completed Balance field
            if (newRecord && Get_ColumnIndex("OpenBalance") > 0)
            {
                SetCompletedBalance(Util.GetValueOfDecimal(Get_Value("OpenBalance")));
            }
            return true;
        }

    }
}
