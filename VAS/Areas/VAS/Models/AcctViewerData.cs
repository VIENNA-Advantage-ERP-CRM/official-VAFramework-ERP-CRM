using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VIS.Classes;
using VAdvantage.Model;

namespace VIS.Models
{
    public class AcctViewerData
    {
        private List<int> VAB_AccountBooks_ids = new List<int>();

        /// <summary>
        /// get value from proper accounting schema
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="vaf_client_id"></param>
        /// <param name="vaf_org_id"></param>
        /// <returns></returns>
        public AcctViewerDataGetClientAcctSch GetClientAcctSchema(Ctx ctx, int vaf_client_id, int vaf_org_id)
        {
            AcctViewerDataGetClientAcctSch obj = new AcctViewerDataGetClientAcctSch();
            obj.AcctSchemas = GetAcctSchemas(vaf_client_id, vaf_org_id);
            obj.OtherAcctSchemas = OtherAcctSchemas(vaf_client_id);
            return obj;
        }

        /// <summary>
        /// Get Accounting Schema
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="vaf_client_id"></param>
        /// <param name="vaf_org_id"></param>
        /// <returns></returns>
        private List<AcctViewerDatakeysParam> GetAcctSchemas(int vaf_client_id, int vaf_org_id)
        {
            List<AcctViewerDatakeysParam> obj = new List<AcctViewerDatakeysParam>();
            string sql =
            sql = "SELECT VAB_ACCOUNTBOOK_ID,NAME FROM VAB_AccountBook WHERE ISACTIVE='Y' AND VAB_ACCOUNTBOOK_ID IN( " +
        "SELECT VAB_ACCOUNTBOOK_ID FROM FRPT_AssignedOrg WHERE ISACTIVE='Y' AND VAF_CLIENT_ID=" + vaf_client_id + " AND VAF_ORG_ID=" + vaf_org_id + ")" +
        //Get default Accounting schema selected on tenant
        " OR VAB_ACCOUNTBOOK_ID IN (SELECT VAB_ACCOUNTBOOK1_ID  FROM VAF_ClientDetail where  VAF_Client_ID=" + vaf_client_id + ")";

            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    AcctViewerDatakeysParam kp = new AcctViewerDatakeysParam();
                    kp.Key = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_ACCOUNTBOOK_ID"]);
                    kp.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["NAME"]);
                    obj.Add(kp);
                    VAB_AccountBooks_ids.Add(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_ACCOUNTBOOK_ID"]));
                }
            }
            return obj;
        }

        /// <summary>
        /// get accounting schema according to VAB_AccountBook_Gl
        /// </summary>
        /// <param name="vaf_client_id"></param>
        /// <returns></returns>
        private List<AcctViewerDatakeysParam> OtherAcctSchemas(int vaf_client_id)
        {
            List<AcctViewerDatakeysParam> obj = new List<AcctViewerDatakeysParam>();
            string sql = "SELECT VAB_AccountBook_id,name FROM VAB_AccountBook acs "
                + "WHERE IsActive='Y'"
                + " AND EXISTS (SELECT * FROM VAB_AccountBook_GL gl WHERE acs.VAB_AccountBook_ID=gl.VAB_AccountBook_ID)";
            if (Env.IsModuleInstalled("FRPT_"))
            {
                sql += " AND EXISTS (SELECT * FROM FRPT_AcctSchema_Default d WHERE acs.VAB_AccountBook_ID=d.VAB_AccountBook_ID)";
            }
            else
            {
                sql += " AND EXISTS (SELECT * FROM VAB_AccountBook_Default d WHERE acs.VAB_AccountBook_ID=d.VAB_AccountBook_ID)";
            }
            if (vaf_client_id != 0)
            {
                sql += " AND VAF_Client_ID=" + vaf_client_id;
            }

            sql += " ORDER BY VAB_AccountBook_ID";

            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                var id = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAB_AccountBook_id"]);

                if (!VAB_AccountBooks_ids.Contains(id))	//	already in _elements
                {
                    sql = "SELECT VAB_AccountBook_id,name from VAB_AccountBook WHERE VAB_AccountBook_ID=" + id;
                    var drSch = DB.ExecuteDataset(sql);
                    if (drSch != null)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            AcctViewerDatakeysParam kp = new AcctViewerDatakeysParam();
                            kp.Key = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_AccountBook_id"]);
                            kp.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["name"]);
                            obj.Add(kp);
                        }
                    }
                }

            }
            return obj;
        }



        /// <summary>
        /// Get Table Data from vaf_tableview
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public List<AcctViewerDataTabel> AcctViewerGetTabelData(Ctx ctx)
        {
            List<AcctViewerDataTabel> obj = new List<AcctViewerDataTabel>();
            var sql = "SELECT VAF_TableView_ID, TableName FROM VAF_TableView t "
                        + "WHERE EXISTS (SELECT * FROM VAF_Column c"
                        + " WHERE t.VAF_TableView_ID=c.VAF_TableView_ID AND c.ColumnName='Posted')"
                        + " AND IsView='N'";
            DataSet ds = DB.ExecuteDataset(sql);

            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    AcctViewerDataTabel objt = new AcctViewerDataTabel();
                    objt.VAF_TableView_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_TableView_ID"]);
                    objt.TableName = Util.GetValueOfString(ds.Tables[0].Rows[i]["TableName"]);
                    obj.Add(objt);
                }
            }
            return obj;
        }

        /// <summary>
        /// get organization data.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="vaf_client_id"></param>
        /// <returns></returns>
        public List<AcctViewerDataOrg> AcctViewerGetOrgData(Ctx ctx, int vaf_client_id)
        {
            List<AcctViewerDataOrg> obj = new List<AcctViewerDataOrg>();

            var sql = "SELECT VAF_Org_ID, Name FROM VAF_Org WHERE VAF_Client_ID=" + vaf_client_id;
            // check applied for checking if organization unit window is available on the target DB or not.
            MVAFOrg Org = new MVAFOrg(ctx, ctx.GetVAF_Org_ID(), null);
            if (Org.Get_ColumnIndex("IsOrgUnit") > -1)
            {
                sql += " AND IsActive='Y' AND IsCostCenter='N' AND IsProfitCenter='N' AND IsSummary='N' ORDER BY Value ";
            }
            else
            {
                sql += " AND IsActive='Y'  AND IsSummary='N' ORDER BY Value ";
            }
            DataSet ds = DB.ExecuteDataset(sql);

            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    AcctViewerDataOrg objt = new AcctViewerDataOrg();
                    objt.VAF_Org_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_Org_ID"]);
                    objt.OrgName = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    obj.Add(objt);
                }
            }
            return obj;
        }

        /// <summary>
        /// Get posting data
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAF_Control_Ref_id"></param>
        /// <returns></returns>
        public List<AcctViewerDataPosting> AcctViewerGetPostingType(Ctx ctx, int VAF_Control_Ref_id)
        {
            List<AcctViewerDataPosting> obj = new List<AcctViewerDataPosting>();
            var sql = " SELECT Value, Name FROM VAF_CtrlRef_List "
            + "WHERE VAF_Control_Ref_ID=" + VAF_Control_Ref_id + " AND IsActive='Y' ORDER BY 1";
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    AcctViewerDataPosting objt = new AcctViewerDataPosting();
                    objt.PostingValue = Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]);
                    objt.PostingName = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    obj.Add(objt);
                }
            }
            return obj;
        }

        /// <summary>
        /// get account schema elements
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<AcctViewerDataAcctSchElements> AcctViewerGetAcctSchElements(Ctx ctx, int key)
        {
            List<AcctViewerDataAcctSchElements> obj = new List<AcctViewerDataAcctSchElements>();
            var sql = "SELECT VAB_AccountBook_element_id,name,elementtype,VAB_Acct_Element_id,seqno," +
                         "'AcctSchemaElement['||VAB_AccountBook_element_id||'-'||name||'('||elementtype||')='||VAB_Acct_Element_id||',Pos='||seqno||']' as detail,VAB_Element_id FROM VAB_AccountBook_Element "
            + "WHERE VAB_AccountBook_ID=" + key + " AND IsActive='Y' ORDER BY SeqNo";
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    AcctViewerDataAcctSchElements objt = new AcctViewerDataAcctSchElements();
                    objt.VAB_AccountBook_Element_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_AccountBook_element_id"]);
                    objt.ElementName = Util.GetValueOfString(ds.Tables[0].Rows[i]["name"]);
                    objt.ElementType = Util.GetValueOfString(ds.Tables[0].Rows[i]["elementtype"]);
                    objt.VAB_Acct_Element_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Acct_Element_id"]);
                    objt.SeqNo = Util.GetValueOfInt(ds.Tables[0].Rows[i]["seqno"]);
                    objt.Detail = Util.GetValueOfString(ds.Tables[0].Rows[i]["detail"]);
                    objt.VAB_Element_ID = Util.GetValueOfString(ds.Tables[0].Rows[i]["VAB_Element_id"]);
                    obj.Add(objt);
                }
            }
            return obj;
        }

        /// <summary>
        /// Repost the data
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dataRecID"></param>
        /// <returns></returns>
        public bool AcctViewerRePost(Ctx ctx, int dataRecID)
        {
            bool repostval = true;

            string invoiceID = "(SELECT ca.VAB_Invoice_id FROM VAB_DocAllocationLine ca" +
                   " inner join VAB_Invoice ci on ci.VAB_Invoice_id= ca.VAB_Invoice_id" +
                   " WHERE ci.issotrx='Y' and ca.VAB_DocAllocation_id=" + dataRecID;


            string postValue = "SELECT (SELECT SUM(al.amount) FROM VAB_DocAllocationLine al INNER JOIN" +
                " VAB_DocAllocation alh ON al.VAB_DocAllocation_id=alh.VAB_DocAllocation_id  WHERE " +
                " alh.posted   ='Y' and VAB_Invoice_id=" + invoiceID + ")) as aloc  ," +
                "(SELECT SUM(cl.linenetamt)  FROM VAB_InvoiceLine cl WHERE " +
                " VAB_Invoice_id     =" + invoiceID + ")) as adj  from dual";


            var dr = DB.ExecuteReader(postValue);
            if (dr.Read())
            {
                // check if value is null
                if (!(dr[0] is DBNull && dr[1] is DBNull))
                {
                    if (dr.GetInt32(0) - dr.GetInt32(1) == 0)
                    {
                        //reposting
                        var sql = "update VAB_DocAllocation alh set alh.posted ='N' where alh.VAB_DocAllocation_id in (select VAB_DocAllocation_id from VAB_DocAllocationLine where VAB_Invoice_id=" + invoiceID + "))";
                        DB.ExecuteQuery(sql);
                    }
                }
            }
            dr.Close();
            dr = null;

            return repostval;
        }



        /// <summary>
        /// get data according to selected documnet type from the info window
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="lookupDirEmbeded"></param>
        /// <param name="tName"></param>
        /// <param name="wheres"></param>
        /// <param name="selectSQLs"></param>
        /// <returns></returns>
        public string AcctViewerGetButtonText(Ctx ctx, string lookupDirEmbeded, string tName, string wheres, string selectSQLs)
        {
            string sqlQry = "SELECT (" + lookupDirEmbeded + ") FROM " + tName + " avd WHERE avd." + selectSQLs;

            try
            {
                string dex = DB.ExecuteScalar(sqlQry).ToString();
                if (dex != null)
                {
                    return dex;
                }
            }
            catch (Exception)
            {
                return "";
            }
            return "";
        }

        /// <summary>
        /// Method to check if organization unit window is available or not.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public bool HasOrganizationUnit(Ctx ctx)
        {
            MVAFOrg Org = new MVAFOrg(ctx, ctx.GetVAF_Org_ID(), null);
            if (Org.Get_ColumnIndex("IsOrgUnit") > -1)
            {
                return true;
            }
            return false;
        }

    }

    public class AcctViewerDataGetClientAcctSch
    {
        public List<AcctViewerDatakeysParam> AcctSchemas { get; set; }
        public List<AcctViewerDatakeysParam> OtherAcctSchemas { get; set; }
    }
}