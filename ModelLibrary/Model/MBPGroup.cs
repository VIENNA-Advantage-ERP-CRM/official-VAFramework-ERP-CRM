/********************************************************
 * Module Name    : Vframwork
 * Purpose        : Business Partner Group Model 
 * Class Used     : X_VAB_BPart_Category
 * Chronological Development
 * Raghunandan    24-June-2009
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
    public class MBPGroup : X_VAB_BPart_Category
    {
        /**
	     * 	Get MBPGroup from Cache
	     *	@param ctx context
	     *	@param VAB_BPart_Category_ID id
	     *	@return MBPGroup
	     */
        public static MBPGroup Get(Ctx ctx, int VAB_BPart_Category_ID)
        {
            int key = VAB_BPart_Category_ID;
            MBPGroup retValue = (MBPGroup)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MBPGroup(ctx, VAB_BPart_Category_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }

        /**
         * 	Get Default MBPGroup
         *	@param ctx context
         *	@return MBPGroup
         */
        public static MBPGroup GetDefault(Ctx ctx)
        {
            int VAF_Client_ID = ctx.GetVAF_Client_ID();
            int key = VAF_Client_ID;
            MBPGroup retValue = (MBPGroup)_cacheDefault[key];
            if (retValue != null)
                return retValue;

            DataTable dt = null;
            String sql = "SELECT * FROM VAB_BPart_Category g "
                + "WHERE IsDefault='Y' AND VAF_Client_ID= " + VAF_Client_ID
                + " ORDER BY IsActive DESC";
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MBPGroup(ctx, dr, null);
                    if (retValue.Get_ID() != 0)
                        _cacheDefault.Add(key, retValue);
                    break;
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

            if (retValue == null)
            {
                _log.Warning("No Default BP Group for VAF_Client_ID=" + VAF_Client_ID);
            }
            return retValue;
        }

        /**
         * 	Get MBPGroup from Business Partner
         *	@param ctx context
         *	@param VAB_BusinessPartner_ID business partner id
         *	@return MBPGroup
         */
        public static MBPGroup GetOfBPartner(Ctx ctx, int VAB_BusinessPartner_ID)
        {
            MBPGroup retValue = null;
            DataTable dt = null;
            String sql = "SELECT * FROM VAB_BPart_Category g "
                + "WHERE EXISTS (SELECT * FROM VAB_BusinessPartner p "
                    + "WHERE p.VAB_BusinessPartner_ID=" + VAB_BusinessPartner_ID + " AND p.VAB_BPart_Category_ID=g.VAB_BPart_Category_ID)";
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MBPGroup(ctx, dr, null);
                    int key = retValue.GetVAB_BPart_Category_ID();
                    if (retValue.Get_ID() != 0)
                        _cache.Add(key, retValue);
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

        //	Cache						
        private static CCache<int, MBPGroup> _cache
            = new CCache<int, MBPGroup>("BP_Group", 10);
        //	Default Cache					
        private static CCache<int, MBPGroup> _cacheDefault
            = new CCache<int, MBPGroup>("BP_Group", 5);
        //Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MBPGroup).FullName);

        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param VAB_BPart_Category_ID id
         *	@param trxName transaction
         */
        public MBPGroup(Ctx ctx, int VAB_BPart_Category_ID, Trx trxName) :
            base(ctx, VAB_BPart_Category_ID, trxName)
        {
            if (VAB_BPart_Category_ID == 0)
            {
                //	SetValue (null);
                //	SetName (null);
                SetIsConfidentialInfo(false);	// N
                SetIsDefault(false);
                SetPriorityBase(PRIORITYBASE_Same);
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result Set
         *	@param trxName transaction
         */
        public MBPGroup(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }

        /**
         * 	Get Credit Watch Percent
         *	@return 90 or defined percent
         */
        public new Decimal GetCreditWatchPercent()
        {
            Object bd = Get_Value("CreditWatchPercent");
            if (bd != null)
            {
                return Convert.ToDecimal(bd);
            }
            return new Decimal(90);
        }
        /**
         * 	Get Credit Watch Ratio
         *	@return 0.90 or defined percent
         */
        public Decimal GetCreditWatchRatio()
        {
            //Decimal bd = base.GetCreditWatchPercent();
            Object bd = Get_Value("CreditWatchPercent");
            if (bd != null)
                return Decimal.Round(Decimal.Divide(Convert.ToDecimal(bd), Env.ONEHUNDRED), 2, MidpointRounding.AwayFromZero);
            return new Decimal(0.90);
        }

        protected override Boolean BeforeSave(Boolean newRecord)
        {
            // TODO Auto-generated method stub
            return true;
        }

        /**
        * 	After Save
        *	@param newRecord new record
        *	@param success success
        *	@return success
        */
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            int _client_ID = 0;
            StringBuilder _sql = new StringBuilder();
            //_sql.Append("Select count(*) from  vaf_tableview where tablename like 'FRPT_BP_Group_Acct'");
            //_sql.Append("SELECT count(*) FROM all_objects WHERE object_type IN ('TABLE') AND (object_name)  = UPPER('FRPT_BP_Group_Acct')  AND OWNER LIKE '" + DB.GetSchema() + "'");
            _sql.Append(DBFunctionCollection.CheckTableExistence(DB.GetSchema(), "FRPT_BP_Group_Acct"));
            int count = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
            if (count > 0)
            {
                _sql.Clear();
                _sql.Append("SELECT L.VALUE FROM VAF_CTRLREF_LIST L inner join VAF_Control_Ref r on R.VAF_CONTROL_REF_ID=L.VAF_CONTROL_REF_ID where r.name='FRPT_RelatedTo' and l.name='Customer'");
                var relatedtoCustomer = Convert.ToString(DB.ExecuteScalar(_sql.ToString()));
                _sql.Clear();
                _sql.Append("Select L.Value From VAF_CtrlRef_List L inner join VAF_Control_Ref r on R.VAF_CONTROL_REF_ID=L.VAF_CONTROL_REF_ID where r.name='FRPT_RelatedTo' and l.name='Vendor'");
                var relatedtoVendor = Convert.ToString(DB.ExecuteScalar(_sql.ToString()));
                _sql.Clear();
                _sql.Append("Select L.Value From VAF_CtrlRef_List L inner join VAF_Control_Ref r on R.VAF_CONTROL_REF_ID=L.VAF_CONTROL_REF_ID where r.name='FRPT_RelatedTo' and l.name='Employee'");
                var relatedtoEmployee = Convert.ToString(DB.ExecuteScalar(_sql.ToString()));

                PO gpact = null;
                _client_ID = GetVAF_Client_ID();
                _sql.Clear();
                _sql.Append("select VAB_AccountBook_ID from VAB_AccountBook where VAF_CLIENT_ID=" + _client_ID);
                DataSet ds3 = new DataSet();
                ds3 = DB.ExecuteDataset(_sql.ToString(), null);
                if (ds3 != null && ds3.Tables[0].Rows.Count > 0)
                {
                    for (int k = 0; k < ds3.Tables[0].Rows.Count; k++)
                    {
                        int _AcctSchema_ID = Util.GetValueOfInt(ds3.Tables[0].Rows[k]["VAB_AccountBook_ID"]);
                        _sql.Clear();
                        _sql.Append("Select Frpt_Acctdefault_Id,VAB_Acct_ValidParameter_Id,Frpt_Relatedto From Frpt_Acctschema_Default Where ISACTIVE='Y' AND VAF_CLIENT_ID=" + _client_ID + "AND VAB_AccountBook_Id=" + _AcctSchema_ID);
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

                                    if (_relatedTo == relatedtoCustomer || _relatedTo == relatedtoEmployee || _relatedTo == relatedtoVendor)
                                    {
                                        _sql.Clear();
                                        //_sql.Append("Select Bp.VAB_BPart_Category_ID,ca.Frpt_Acctdefault_Id From VAB_BPart_Category Bp Left Join FRPT_BP_Group_Acct ca On Bp.VAB_BPart_Category_ID=ca.VAB_BPart_Category_ID And ca.Frpt_Acctdefault_Id=" + ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"] + " WHERE Bp.IsActive='Y' AND Bp.VAF_Client_ID=" + _client_ID);
                                        _sql.Append("Select COUNT(*) From VAB_BPart_Category Bp Left Join FRPT_BP_Group_Acct ca On Bp.VAB_BPart_Category_ID=ca.VAB_BPart_Category_ID And ca.Frpt_Acctdefault_Id=" + ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"] + " WHERE Bp.IsActive='Y' AND Bp.VAF_Client_ID=" + _client_ID + " AND ca.VAB_Acct_ValidParameter_Id = " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Acct_ValidParameter_Id"]) + " AND Bp.VAB_BPart_Category_ID = " + GetVAB_BPart_Category_ID());
                                        int recordFound = Convert.ToInt32(DB.ExecuteScalar(_sql.ToString(), null, Get_Trx()));
                                        // ds2 = DB.ExecuteDataset(_sql.ToString(), null);
                                        //if (ds2 != null && ds2.Tables[0].Rows.Count > 0)
                                        //{
                                        //    for (int j = 0; j < ds2.Tables[0].Rows.Count; j++)
                                        //    {
                                        //        int value = Util.GetValueOfInt(ds2.Tables[0].Rows[j]["Frpt_Acctdefault_Id"]);
                                        //        if (value == 0)
                                        //        {
                                        //gpact = new X_FRPT_BP_Group_Acct(GetCtx(), 0, null);
                                        if (recordFound == 0)
                                        {
                                            gpact = MVAFTableView.GetPO(GetCtx(), "FRPT_BP_Group_Acct", 0, null);
                                            //gpact.Set_ValueNoCheck("VAB_BPart_Category_ID", Util.GetValueOfInt(ds2.Tables[0].Rows[j]["VAB_BPart_Category_ID"]));
                                            gpact.Set_ValueNoCheck("VAB_BPart_Category_ID", Util.GetValueOfInt(GetVAB_BPart_Category_ID()));
                                            gpact.Set_ValueNoCheck("VAF_Org_ID", 0);
                                            gpact.Set_ValueNoCheck("FRPT_AcctDefault_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));
                                            gpact.Set_ValueNoCheck("VAB_Acct_ValidParameter_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Acct_ValidParameter_Id"]));
                                            gpact.Set_ValueNoCheck("VAB_AccountBook_ID", _AcctSchema_ID);
                                            if (!gpact.Save())
                                            {

                                            }
                                        }
                                        //}
                                        //}
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
                object table = DB.ExecuteScalar("SELECT count(*) from VAF_TableView WHERE TableName='VAB_BPart_Category_Acct'");
                if (table == null || table == DBNull.Value || table == "" || Convert.ToInt16(table) == 0)
                {
                    return success;
                }

                if (newRecord & success && (String.IsNullOrEmpty(GetCtx().GetContext("#DEFAULT_ACCOUNTING_APPLICABLE")) || Util.GetValueOfString(GetCtx().GetContext("#DEFAULT_ACCOUNTING_APPLICABLE")) == "Y"))
                {
                    success = Insert_Accounting("VAB_BPart_Category_Acct", "VAB_AccountBook_Default", null);

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
        protected override Boolean BeforeDelete()
        {
            return Delete_Accounting("VAB_BPart_Category_Acct");
        }
    }
}
