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
        private List<int> c_acctschemas_ids = new List<int>();

        /// <summary>
        /// get value from proper accounting schema
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="ad_client_id"></param>
        /// <param name="ad_org_id"></param>
        /// <returns></returns>
        public AcctViewerDataGetClientAcctSch GetClientAcctSchema(Ctx ctx, int ad_client_id, int ad_org_id)
        {
            AcctViewerDataGetClientAcctSch obj = new AcctViewerDataGetClientAcctSch();
            obj.AcctSchemas = GetAcctSchemas(ad_client_id, ad_org_id);
            obj.OtherAcctSchemas = OtherAcctSchemas(ad_client_id);
            return obj;
        }

        /// <summary>
        /// Get Accounting Schema
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="ad_client_id"></param>
        /// <param name="ad_org_id"></param>
        /// <returns></returns>
        private List<AcctViewerDatakeysParam> GetAcctSchemas(int ad_client_id, int ad_org_id)
        {
            List<AcctViewerDatakeysParam> obj = new List<AcctViewerDatakeysParam>();
            string sql =
            sql = "SELECT C_ACCTSCHEMA_ID,NAME FROM C_AcctSchema WHERE ISACTIVE='Y' AND C_ACCTSCHEMA_ID IN( " +
        "SELECT C_ACCTSCHEMA_ID FROM FRPT_AssignedOrg WHERE ISACTIVE='Y' AND AD_CLIENT_ID=" + ad_client_id + " AND AD_ORG_ID=" + ad_org_id + ")" +
        //Get default Accounting schema selected on tenant
        " OR C_ACCTSCHEMA_ID IN (SELECT C_ACCTSCHEMA1_ID  FROM AD_ClientInfo where  AD_Client_ID=" + ad_client_id + ")";

            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    AcctViewerDatakeysParam kp = new AcctViewerDatakeysParam();
                    kp.Key = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_ACCTSCHEMA_ID"]);
                    kp.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["NAME"]);
                    obj.Add(kp);
                    c_acctschemas_ids.Add(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_ACCTSCHEMA_ID"]));
                }
            }
            return obj;
        }

        /// <summary>
        /// get accounting schema according to C_AcctSchema_Gl
        /// </summary>
        /// <param name="ad_client_id"></param>
        /// <returns></returns>
        private List<AcctViewerDatakeysParam> OtherAcctSchemas(int ad_client_id)
        {
            List<AcctViewerDatakeysParam> obj = new List<AcctViewerDatakeysParam>();
            string sql = "SELECT c_acctschema_id,name FROM C_AcctSchema acs "
                + "WHERE IsActive='Y'"
                + " AND EXISTS (SELECT * FROM C_AcctSchema_GL gl WHERE acs.C_AcctSchema_ID=gl.C_AcctSchema_ID)";
            if (Env.IsModuleInstalled("FRPT_"))
            {
                sql += " AND EXISTS (SELECT * FROM FRPT_AcctSchema_Default d WHERE acs.C_AcctSchema_ID=d.C_AcctSchema_ID)";
            }
            else
            {
                sql += " AND EXISTS (SELECT * FROM C_AcctSchema_Default d WHERE acs.C_AcctSchema_ID=d.C_AcctSchema_ID)";
            }
            if (ad_client_id != 0)
            {
                sql += " AND AD_Client_ID=" + ad_client_id;
            }

            sql += " ORDER BY C_AcctSchema_ID";

            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                var id = Util.GetValueOfInt(ds.Tables[0].Rows[0]["c_acctschema_id"]);

                if (!c_acctschemas_ids.Contains(id))	//	already in _elements
                {
                    sql = "SELECT c_acctschema_id,name from C_AcctSchema WHERE C_AcctSchema_ID=" + id;
                    var drSch = DB.ExecuteDataset(sql);
                    if (drSch != null)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            AcctViewerDatakeysParam kp = new AcctViewerDatakeysParam();
                            kp.Key = Util.GetValueOfInt(ds.Tables[0].Rows[i]["c_acctschema_id"]);
                            kp.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["name"]);
                            obj.Add(kp);
                        }
                    }
                }

            }
            return obj;
        }



        /// <summary>
        /// Get Table Data from Ad_table
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public List<AcctViewerDataTabel> AcctViewerGetTabelData(Ctx ctx)
        {
            List<AcctViewerDataTabel> obj = new List<AcctViewerDataTabel>();
            var sql = "SELECT AD_Table_ID, TableName FROM AD_Table t "
                        + "WHERE EXISTS (SELECT * FROM AD_Column c"
                        + " WHERE t.AD_Table_ID=c.AD_Table_ID AND c.ColumnName='Posted')"
                        + " AND IsView='N'";
            DataSet ds = DB.ExecuteDataset(sql);

            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    AcctViewerDataTabel objt = new AcctViewerDataTabel();
                    objt.AD_Table_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Table_ID"]);
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
        /// <param name="ad_client_id"></param>
        /// <returns></returns>
        public List<AcctViewerDataOrg> AcctViewerGetOrgData(Ctx ctx, int ad_client_id)
        {
            List<AcctViewerDataOrg> obj = new List<AcctViewerDataOrg>();

            var sql = "SELECT AD_Org_ID, Name FROM AD_Org WHERE AD_Client_ID=" + ad_client_id;
            // check applied for checking if organization unit window is available on the target DB or not.
            MOrg Org = new MOrg(ctx, ctx.GetAD_Org_ID(), null);
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
                    objt.AD_Org_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Org_ID"]);
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
        /// <param name="ad_reference_id"></param>
        /// <returns></returns>
        public List<AcctViewerDataPosting> AcctViewerGetPostingType(Ctx ctx, int ad_reference_id)
        {
            List<AcctViewerDataPosting> obj = new List<AcctViewerDataPosting>();
            var sql = " SELECT Value, Name FROM AD_Ref_List "
            + "WHERE AD_Reference_ID=" + ad_reference_id + " AND IsActive='Y' ORDER BY 1";
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
            var sql = "SELECT c_acctschema_element_id,name,elementtype,c_elementvalue_id,seqno," +
                         "'AcctSchemaElement['||c_acctschema_element_id||'-'||name||'('||elementtype||')='||c_elementvalue_id||',Pos='||seqno||']' as detail,c_element_id FROM C_AcctSchema_Element "
            + "WHERE C_AcctSchema_ID=" + key + " AND IsActive='Y' ORDER BY SeqNo";
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    AcctViewerDataAcctSchElements objt = new AcctViewerDataAcctSchElements();
                    objt.C_AcctSchema_Element_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["c_acctschema_element_id"]);
                    objt.ElementName = Util.GetValueOfString(ds.Tables[0].Rows[i]["name"]);
                    objt.ElementType = Util.GetValueOfString(ds.Tables[0].Rows[i]["elementtype"]);
                    objt.C_ElementValue_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["c_elementvalue_id"]);
                    objt.SeqNo = Util.GetValueOfInt(ds.Tables[0].Rows[i]["seqno"]);
                    objt.Detail = Util.GetValueOfString(ds.Tables[0].Rows[i]["detail"]);
                    objt.C_Element_ID = Util.GetValueOfString(ds.Tables[0].Rows[i]["c_element_id"]);
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

            string invoiceID = "(SELECT ca.c_invoice_id FROM c_allocationline ca" +
                   " inner join c_invoice ci on ci.c_invoice_id= ca.c_invoice_id" +
                   " WHERE ci.issotrx='Y' and ca.c_allocationhdr_id=" + dataRecID;


            string postValue = "SELECT (SELECT SUM(al.amount) FROM c_allocationline al INNER JOIN" +
                " c_allocationhdr alh ON al.c_allocationhdr_id=alh.c_allocationhdr_id  WHERE " +
                " alh.posted   ='Y' and c_invoice_id=" + invoiceID + ")) as aloc  ," +
                "(SELECT SUM(cl.linenetamt)  FROM c_invoiceline cl WHERE " +
                " c_invoice_id     =" + invoiceID + ")) as adj  from dual";


            IDataReader dr = null;
            try
            {
                dr = DB.ExecuteReader(postValue);
                if (dr.Read())
                {
                    // check if value is null
                    if (!(dr[0] is DBNull && dr[1] is DBNull))
                    {
                        if (dr.GetInt32(0) - dr.GetInt32(1) == 0)
                        {
                            //reposting
                            var sql = "update c_allocationhdr alh set alh.posted ='N' where alh.c_allocationhdr_id in (select c_allocationhdr_id from c_allocationline where c_invoice_id=" + invoiceID + "))";
                            DB.ExecuteQuery(sql);
                        }
                    }
                }
                dr.Close();
                dr = null;
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                VAdvantage.Logging.Logger.global.Severe("AcctViewerRePost" + e.Message);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
            }
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
            MOrg Org = new MOrg(ctx, ctx.GetAD_Org_ID(), null);
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