﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VAdvantage.Model;

namespace VIS.Helpers
{
    public class RoleHelper
    {

        //private OrgAccess[] _orgAccess = null;
        public static Role GetRole(MRole role)
        {
            Role r = new Role();
            r.IsAccessAllOrgs = role.IsAccessAllOrgs();
            r.IsAdministrator = role.IsAdministrator();
            r.VAF_Client_ID = role.GetVAF_Client_ID();
            r.VAF_UserContact_ID = role.GetVAF_UserContact_ID();
            r.UserLevel = role.GetUserLevel();
            r.IsCanExport = role.IsCanExport();
            r.IsCanReport = role.IsCanReport();
            r.IsUseBPRestrictions = role.IsUseBPRestrictions();
            r.IsPersonalAccess = role.IsPersonalAccess();
            r.Name = role.GetName();
            r.IsShowPreference = role.IsShowPreference();
            r.PreferenceType = role.GetPreferenceType();

            r.IsDisplayClient = role.IsDisplayClient();
            r.IsDisplayOrg = role.IsDisplayOrg();
            r.IsPersonalLock = role.IsPersonalLock();
            r.IsOverwritePriceLimit = role.IsOverwritePriceLimit();
            r.IsOverrideReturnPolicy = role.IsOverrideReturnPolicy();

            r.tableAccessLevel = role.GetTableAccessLevel();
            r.tableName = role.GetTableNames();
            r.orgAccess = role.GetOrgAccess();
            r.windowAccess = role.GetWindowAccess();
            r.formAccess = role.GetFormAccess();
            r.processAccess = role.GetProcessAccess();

            r.tableAccess = TableAccess.Get(role.GetTableAccess());
            r.columnAccess = ColumnAccess.Get(role.GetColumnAccess());
            r.recordAccess = RecordAccess.Get(role.GetRecordAccess());
            r.recordDependentAccess = RecordAccess.Get(role.GetRecordDependentAccess());

            r.MaxQueryRecords = role.GetMaxQueryRecords();
            r.IsShowAcct = role.IsShowAcct();
            r.IsDisableMenu = role.IsDisableMenu();
            r.HomePage = role.GetHomePage_ID();

            /* Prepare Pre Setting of Table*/

            r.tableData = TableData.Get();


            return r;
        }

    }

    public class Role
    {
        public List<TableAccess> tableAccess = null;
        public List<ColumnAccess> columnAccess = null;
        public List<RecordAccess> recordAccess = null;
        public List<RecordAccess> recordDependentAccess = null;
        public VAdvantage.Model.MRole.OrgAccess[] orgAccess = null;
        public Dictionary<int, string> tableAccessLevel = null;
        public Dictionary<string, int> tableName = null;

        public Dictionary<int, bool> windowAccess = null;
        public Dictionary<int, bool> formAccess = null;
        public Dictionary<int, bool> processAccess = null;

        /*Properties*/
        public bool IsAccessAllOrgs;
        public bool IsAdministrator;
        public int VAF_Client_ID;
        public int VAF_UserContact_ID;
        public string UserLevel;
        public bool IsCanExport;
        public bool IsCanReport;
        public bool IsUseBPRestrictions;
        public bool IsPersonalAccess;

        public bool IsDisplayClient;
        public bool IsDisplayOrg;
        public bool IsPersonalLock;
        public bool IsOverwritePriceLimit;
        public bool IsOverrideReturnPolicy;


        public bool IsShowPreference;
        public String Name;
        public string PreferenceType;

        public int MaxQueryRecords;
        public bool IsShowAcct;
        public bool IsDisableMenu;
        public int HomePage;

        public Dictionary<int, TableData> tableData = null;

    }

    public class TableData
    {
        public bool IsView;
        public bool HasKey;

        internal static Dictionary<int, TableData> Get()
        {
            Dictionary<int, TableData> dataList = new Dictionary<int, TableData>();

            String sql = "SELECT t.VAF_TableView_ID , (SELECT COUNT(VAF_Column_ID) FROM VAF_Column "
                        + " WHERE UPPER(ColumnName) = UPPER(t.TableName)||'_ID' AND VAF_TableView_ID=t.VAF_TableView_ID ) as hasKey, "
                        + " Isview from VAF_TableView t";


            IDataReader dr = VAdvantage.DataBase.DB.ExecuteReader(sql);
            TableData data = null;
            try
            {
                while (dr.Read())
                {
                    data = new TableData();
                    data.HasKey = Convert.ToInt32(dr[1])>0;// dr.GetInt32(1) > 0;
                    data.IsView = dr[2].ToString() == "Y";
                    dataList[dr.GetInt32(0)] = data;
                }
                dr.Close();
            }
            catch
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
            }
            return dataList;
        }
    }


    public class TableAccess
    {
        public bool IsExclude;
        public string AccessTypeRule;
        public int VAF_TableView_ID;
        public bool IsCanReport;
        public bool IsCanExport;
        public bool IsReadOnly;

        public static List<TableAccess> Get(MTableAccess[] tblAccess)
        {
            List<TableAccess> tl = new List<TableAccess>();
            TableAccess ta = null;
            foreach (MTableAccess t in tblAccess)
            {
                ta = new TableAccess();
                ta.IsExclude = t.IsExclude();
                ta.AccessTypeRule = t.GetAccessTypeRule();
                ta.IsCanReport = t.IsCanReport();
                ta.IsCanExport = t.IsCanExport();
                ta.IsReadOnly = t.IsReadOnly();
                ta.VAF_TableView_ID = t.GetVAF_TableView_ID();
                tl.Add(ta);
            }
            return tl;
        }


    }

    public class ColumnAccess
    {
        public bool IsExclude;
        public int VAF_TableView_ID;
        public int VAF_Column_ID;
        public bool IsReadOnly;
        public static List<ColumnAccess> Get(MColumnAccess[] colAccess)
        {
            List<ColumnAccess> cl = new List<ColumnAccess>();
            ColumnAccess ca = null;
            foreach (MColumnAccess c in colAccess)
            {
                ca = new ColumnAccess();
                ca.IsExclude = c.IsExclude();
                ca.VAF_Column_ID = c.GetVAF_Column_ID();
                ca.VAF_TableView_ID = c.GetVAF_TableView_ID();
                ca.IsReadOnly = c.IsReadOnly();
                cl.Add(ca);
            }
            return cl;
        }



    }

    public class RecordAccess
    {
        public int VAF_TableView_ID;
        public int Record_ID;
        public bool IsReadOnly;
        public bool IsExclude;
        public string KeyColumnName;

        public static List<RecordAccess> Get(MRecordAccess[] recAccess)
        {
            List<RecordAccess> rl = new List<RecordAccess>();
            RecordAccess ra = null;
            foreach (MRecordAccess r in recAccess)
            {
                ra = new RecordAccess();
                ra.IsExclude = r.IsExclude();
                ra.IsReadOnly = r.IsReadOnly();
                ra.VAF_TableView_ID = r.GetVAF_TableView_ID();
                ra.Record_ID = r.GetRecord_ID();
                ra.KeyColumnName = r.GetKeyColumnName();
                rl.Add(ra);
            }
            return rl;
        }

    }
}