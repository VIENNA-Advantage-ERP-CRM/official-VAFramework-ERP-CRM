﻿/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAM_Warehouse
 * Chronological Development
 * Veena Pandey     
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MWarehouse : X_VAM_Warehouse
    {
        //	Cache
        private static CCache<int, MWarehouse> cache = new CCache<int, MWarehouse>("MWarehouse", 5);
        //	Warehouse Locators
        private MVAMLocator[] locators = null;
        //	Default Locator
        private int VAM_Locator_ID = -1;
        //	Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MWarehouse).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_Warehouse_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MWarehouse(Ctx ctx, int VAM_Warehouse_ID, Trx trxName)
            : base(ctx, VAM_Warehouse_ID, trxName)
        {
            if (VAM_Warehouse_ID == 0)
            {
                //SetValue(null);
                //SetName(null);
                //SetVAB_Address_ID(0);
                SetSeparator("*");	// *
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MWarehouse(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Organization Constructor
        /// </summary>
        /// <param name="org">org</param>
        public MWarehouse(MVAFOrg org)
            : this(org.GetCtx(), 0, org.Get_TrxName())
        {
            SetClientOrg(org);
            SetValue(org.GetValue());
            SetName(org.GetName());
            if (org.GetInfo() != null)
                SetVAB_Address_ID(org.GetInfo().GetVAB_Address_ID());
        }

        /// <summary>
        /// Get from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_Warehouse_ID">id</param>
        /// <returns>warehouse</returns>
        public static MWarehouse Get(Ctx ctx, int VAM_Warehouse_ID)
        {
            int key = VAM_Warehouse_ID;
            MWarehouse retValue = null;
            if (cache.ContainsKey(key))
            {
                retValue = (MWarehouse)cache[key];
            }
            if (retValue != null)
                return retValue;
            //
            retValue = new MWarehouse(ctx, VAM_Warehouse_ID, null);
            cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Get Warehouses for Org
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Org_ID">id</param>
        /// <returns>warehouse</returns>
        public static MWarehouse[] GetForOrg(Ctx ctx, int VAF_Org_ID)
        {
            List<MWarehouse> list = new List<MWarehouse>();
            String sql = "SELECT * FROM VAM_Warehouse WHERE VAF_Org_ID=" + VAF_Org_ID + " ORDER BY Created";
            try
            {
                DataSet ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, null);
                if (ds.Tables.Count > 0)
                {
                    DataRow dr = null;
                    int totCount = ds.Tables[0].Rows.Count;
                    for (int i = 0; i < totCount; i++)
                    {
                        dr = ds.Tables[0].Rows[i];
                        list.Add(new MWarehouse(ctx, dr, null));
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            MWarehouse[] retValue = new MWarehouse[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get Default Locator
        /// </summary>
        /// <returns>(first) default locator</returns>
        public MVAMLocator GetDefaultLocator()
        {
            MVAMLocator[] locators = GetLocators(false);	//	ordered by x,y,z
            MVAMLocator loc1 = null;
            for (int i = 0; i < locators.Length; i++)
            {
                MVAMLocator locIn = locators[i];
                if (locIn.IsDefault() && locIn.IsActive())
                    return locIn;
                if (loc1 == null || loc1.GetPriorityNo() > locIn.GetPriorityNo())
                    loc1 = locIn;
            }
            //	No Default - return highest priority
            if (locators.Length > 0)
            {
                log.Warning("No default Locator for " + GetName());
                return loc1;
            }
            //	No Locator - create one
            MVAMLocator loc = new MVAMLocator(this, "Standard");
            loc.SetIsDefault(true);
            loc.Save();
            log.Info("Created default locator for " + GetName());
            return loc;
        }

        /// <summary>
        /// Get Default VAM_Locator_ID
        /// </summary>
        /// <returns>id</returns>
        public int GetDefaultVAM_Locator_ID()
        {
            if (VAM_Locator_ID <= 0)
                VAM_Locator_ID = GetDefaultLocator().GetVAM_Locator_ID();
            return VAM_Locator_ID;
        }

        /// <summary>
        /// Get Locators
        /// </summary>
        /// <param name="reload">if true reload</param>
        /// <returns>array of locators</returns>
        public MVAMLocator[] GetLocators(bool reload)
        {
            if (!reload && locators != null)
                return locators;
            //
            String sql = "SELECT * FROM VAM_Locator WHERE VAM_Warehouse_ID=" + GetVAM_Warehouse_ID() + " ORDER BY X,Y,Z";
            List<MVAMLocator> list = new List<MVAMLocator>();
            try
            {
                DataSet ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, null);
                if (ds.Tables.Count > 0)
                {
                    DataRow dr = null;
                    int totCount = ds.Tables[0].Rows.Count;
                    for (int i = 0; i < totCount; i++)
                    {
                        dr = ds.Tables[0].Rows[i];
                        list.Add(new MVAMLocator(GetCtx(), dr, null));
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            //
            locators = new MVAMLocator[list.Count];
            locators = list.ToArray();
            return locators;
        }

        /// <summary>
        /// Before Delete
        /// </summary>
        /// <returns>true</returns>
        protected override bool BeforeDelete()
        {
            return Delete_Accounting("VAM_Warehouse_Acct");
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            if (!success)
            {
                return success;
            }

            PO wrhus = null;
            int _client_ID = 0;
            StringBuilder _sql = new StringBuilder();
            //_sql.Append("Select count(*) from  vaf_tableview where tablename like 'FRPT_Warehouse_Acct'");
            //_sql.Append("SELECT count(*) FROM all_objects WHERE object_type IN ('TABLE') AND (object_name)  = UPPER('FRPT_Warehouse_Acct')  AND OWNER LIKE '" + DB.GetSchema() + "'");
            _sql.Append(DBFunctionCollection.CheckTableExistence(DB.GetSchema(), "FRPT_Warehouse_Acct"));
            int count = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
            if (count > 0)
            {
                _sql.Clear();
                _sql.Append("Select L.Value From VAF_CtrlRef_List L inner join VAF_Control_Ref r on R.VAF_CONTROL_REF_ID=L.VAF_CONTROL_REF_ID where r.name='FRPT_RelatedTo' and l.name='Warehouse'");
                var relatedtoWarehouse = Convert.ToString(DB.ExecuteScalar(_sql.ToString()));
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

                                    if (_relatedTo == relatedtoWarehouse)
                                    {
                                        _sql.Clear();
                                        _sql.Append("Select COUNT(*) From VAM_Warehouse Bp Left Join Frpt_warehouse_Acct ca On Bp.VAM_Warehouse_ID=ca.VAM_Warehouse_ID And ca.Frpt_Acctdefault_Id=" + ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"] + " WHERE Bp.IsActive='Y' AND Bp.VAF_Client_ID=" + _client_ID + " AND ca.VAB_Acct_ValidParameter_Id = " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Acct_ValidParameter_Id"]) + " AND Bp.VAM_Warehouse_ID = " + GetVAM_Warehouse_ID());
                                        int recordFound = Convert.ToInt32(DB.ExecuteScalar(_sql.ToString(), null, Get_Trx()));
                                        //ds2 = DB.ExecuteDataset(_sql.ToString(), null);
                                        //if (ds2 != null && ds2.Tables[0].Rows.Count > 0)
                                        //{
                                        //    for (int j = 0; j < ds2.Tables[0].Rows.Count; j++)
                                        //    {
                                        //        int value = Util.GetValueOfInt(ds2.Tables[0].Rows[j]["Frpt_Acctdefault_Id"]);
                                        //        if (value == 0)
                                        //        {
                                        //wrhus = new X_FRPT_Warehouse_Acct(GetCtx(), 0, null);
                                        if (recordFound == 0)
                                        {
                                            wrhus = MVAFTableView.GetPO(GetCtx(), "FRPT_Warehouse_Acct", 0, null);
                                            wrhus.Set_ValueNoCheck("VAF_Org_ID", 0);
                                            wrhus.Set_ValueNoCheck("VAM_Warehouse_ID", Util.GetValueOfInt(GetVAM_Warehouse_ID()));
                                            wrhus.Set_ValueNoCheck("FRPT_AcctDefault_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));
                                            wrhus.Set_ValueNoCheck("VAB_Acct_ValidParameter_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Acct_ValidParameter_Id"]));
                                            wrhus.Set_ValueNoCheck("VAB_AccountBook_ID", _AcctSchema_ID);

                                            if (!wrhus.Save())
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
                    bool sucs = Insert_Accounting("VAM_Warehouse_Acct", "VAB_AccountBook_Default", null);
                    //Karan. work done to show message if data not saved in accounting tab. but will save data in current tab.
                    // Before this, data was being saved but giving message "record not saved".
                    if (!sucs)
                    {
                        log.SaveWarning("AcctNotSaved", "");
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns></returns>
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            /* Disallow Negative Inventory cannot be checked if there are storage records 
            with negative onhand. */
            if (Is_ValueChanged("IsDisallowNegativeInv") && IsDisallowNegativeInv())
            {
                //String sql = "SELECT VAM_Product_ID FROM VAM_StorageDetail s " +
                //             "WHERE s.QtyType = 'H' AND s.VAM_Locator_ID IN (SELECT VAM_Locator_ID FROM VAM_Locator l " +
                //                            "WHERE VAM_Warehouse_ID=" + GetVAM_Warehouse_ID() + " )" +
                //             " GROUP BY VAM_Product_ID, VAM_Locator_ID " +
                //             " HAVING SUM(s.Qty) < 0 ";
                String sql = "SELECT VAM_Product_ID FROM VAM_Storage s " +
                            "WHERE s.VAM_Locator_ID IN (SELECT VAM_Locator_ID FROM VAM_Locator l " +
                                           "WHERE VAM_Warehouse_ID=" + GetVAM_Warehouse_ID() + " )" +
                            " GROUP BY VAM_Product_ID, VAM_Locator_ID " +
                            " HAVING SUM(s.Qty) < 0 ";

                IDataReader idr = null;
                Boolean negInvExists = false;
                try
                {
                    idr = DB.ExecuteReader(sql, null, Get_TrxName());
                    if (idr.Read())
                    {
                        negInvExists = true;
                    }
                }
                catch (Exception e)
                {
                    log.Log(Level.SEVERE, sql, e);
                }
                finally
                {
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                }

                if (negInvExists)
                {
                    log.SaveError("Error", Msg.Translate(GetCtx(), "NegativeOnhandExists"));
                    return false;
                }
            }

            // Added by Vivek on 21/09/2017 suggested by Ravikant
            // Check if Drop ship warehouse already exist for the same organization
            // Check Column exist in DB -- Vivek on 17/10/2017 by Mukesh sir
            MVAFTableView tab = MVAFTableView.Get(GetCtx(), "VAM_Warehouse");
            if (tab.Get_ColumnIndex("IsDropShip") > 0)
            {
                if (Util.GetValueOfInt(DB.ExecuteScalar("Select Count(*) From VAM_Warehouse Where VAF_Org_ID=" + GetVAF_Org_ID() + " AND IsDropShip='Y' AND IsActive='Y'")) > 0)
                {
                    log.SaveError("Error", Msg.Translate(GetCtx(), "DropShipWarehouse"));
                    return false;
                }
            }
            if (GetVAF_Org_ID() == 0)
            {
                int context_VAF_Org_ID = GetCtx().GetVAF_Org_ID();
                if (context_VAF_Org_ID != 0)
                {
                    SetVAF_Org_ID(context_VAF_Org_ID);
                    log.Warning("Changed Org to Context=" + context_VAF_Org_ID);
                }
                else
                {
                    log.SaveError("Error", Msg.Translate(GetCtx(), "Org0NotAllowed"));
                    return false;
                }
            }

            if ((newRecord || Is_ValueChanged("IsWMSEnabled") || Is_ValueChanged("IsDisallowNegativeInv"))
                    && IsWMSEnabled() && !IsDisallowNegativeInv())
            {
                log.SaveError("Error", Msg.Translate(GetCtx(), "NegativeInventoryDisallowedWMS"));
                return false;
            }

            if (newRecord || Is_ValueChanged("Separator"))
            {
                if (GetSeparator() == null || GetSeparator().Length <= 0)
                {
                    SetSeparator("*");
                }
            }

            // JID_1888 Checks for the duplicate search key

            int count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(Value) FROM VAM_Warehouse WHERE Value= '" + GetValue() + "' AND VAM_Warehouse_ID !=" + GetVAM_Warehouse_ID() + " AND VAF_Org_ID = " + GetVAF_Org_ID()));
            if (count > 0)
            {
                log.SaveError("", Msg.GetMsg(GetCtx(), "SearchKeyUnique"));
                return false;
            }
            //JID_1888 checks for the duplicate name

            int countName = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(Name) FROM VAM_Warehouse WHERE Name= '" + GetName() + "' AND VAM_Warehouse_ID !=" + GetVAM_Warehouse_ID() + " AND VAF_Org_ID = " + GetVAF_Org_ID()));
            if (countName > 0)
            {
                log.SaveError("", Msg.GetMsg(GetCtx(), "RequiredUniqueName"));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if locator is in warehouse 
        /// </summary>
        /// <param name="p_VAM_Warehouse_ID"></param>
        /// <param name="p_VAM_Locator_ID"></param>
        /// <returns>true if locator is in the warehouse</returns>
        public static Boolean IsLocatorInWarehouse(int p_VAM_Warehouse_ID, int p_VAM_Locator_ID)
        {
            int VAM_Warehouse_ID = DB.GetSQLValue(null,
                    "SELECT VAM_Warehouse_ID FROM VAM_Locator WHERE VAM_Locator_ID=@param1", p_VAM_Locator_ID);
            if (p_VAM_Warehouse_ID == VAM_Warehouse_ID)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MWarehouse[");
            sb.Append(Get_ID()).Append("-").Append(GetName()).Append("]");
            return sb.ToString();
        }
    }
}
