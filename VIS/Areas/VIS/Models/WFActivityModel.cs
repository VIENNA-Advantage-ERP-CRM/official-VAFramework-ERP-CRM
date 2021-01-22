using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.WF;

namespace VIS.Models
{
    public class WFActivityModel
    {

        private string _synonym = "A";
        private string whereClause = "";
        private string fromClause = "";


        /// <summary>
        /// Fetch all the workflow activities which are open
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="VAF_UserContact_ID">Login User ID</param>
        /// <param name="VAF_Client_ID"> Login Client ID </param>
        /// <param name="pageNo"> Current Page Number </param>
        /// <param name="pageSize"> Sepcify number of records per page</param>
        /// <param name="refresh"> Refresh Data? </param>
        /// <param name="searchText"> search Activities based on summary </param>
        /// <param name="VAF_Screen_ID"> Window ID based on which serach Activities </param>
        /// <param name="dateFrom">Activities Start From</param>
        /// <param name="dateTo"> Activities Date To </param>
        /// <param name="AD_Node_ID">Window ID based on which serach Activities</param>
        /// <returns></returns>
        public WFInfo GetActivities(Ctx ctx, int VAF_UserContact_ID, int VAF_Client_ID, int pageNo, int pageSize, bool refresh, string searchText, int VAF_Screen_ID, DateTime? dateFrom, DateTime? dateTo, int AD_Node_ID)
        {
            string sql = "";
            List<MTable> mtable = new List<MTable>();
            int count = 0;

            // If window is selected or search text is available..
            if (VAF_Screen_ID > 0 || (!string.IsNullOrEmpty(searchText) && searchText.Length > 0))
            {
                sql = @"SELECT DISTINCT tabl.vaf_tableview_ID,Tab.VAF_Tab_ID FROM Ad_Window Wind Join vaf_tab Tab 
                    ON Wind.Ad_Window_Id=Tab.Ad_Window_Id JOIN  vaf_tableview Tabl On Tab.vaf_tableview_Id=Tabl.vaf_tableview_Id 
                    WHERE Tab.IsActive    ='Y'";

                sql += " AND wind.VAF_Screen_ID=" + VAF_Screen_ID + " and tab.vaf_tableview_ID IN (Select Distinct VAF_TableView_ID FROM VAF_WFlow_Task where VAF_Screen_ID=" + VAF_Screen_ID + ") ORDER BY Tab.VAF_Tab_ID Asc";

                //if window is selected then search for tables associated with window.
                DataSet ds = DB.ExecuteDataset(sql);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    //for each table , create where clause based on columns such as Name, DocumentNo,C_Bpatner_ID, VAF_UserContact_ID exist in that table or not.
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        count++;
                        MTable table = new MTable(ctx, Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_TableView_ID"]), null);

                        GetFromClause(ctx, table.GetTableName(), "", "", "");
                        GetWhereClause(ctx, table, searchText, VAF_Screen_ID, AD_Node_ID);
                        SynonymNext();
                    }
                }
                sql = "";
            }

            if (count == 0)
            {
                GetWhereClause(ctx, null, searchText, VAF_Screen_ID, AD_Node_ID);
            }

            GetDateWiseWhereClause(dateFrom, dateTo, searchText);



            // if (VAF_Screen_ID > 0 || (!string.IsNullOrEmpty(searchText) && searchText.Length > 0))
            if (whereClause.Length > 7)
            {
                sql = @"SELECT mytable.* FROM (";
            }

            string dmsCheck = string.Empty;

            if (Env.IsModuleInstalled("VADMS_"))
            {
                // Check if table id and record id in current activity has any document binded in VADMS_WindowDocLink table, or current table is VADMS_MetaData
                dmsCheck = @",
(SELECT 
Name || VADMS_FileType || '_' || Value 
FROM VADMS_Document 
WHERE VADMS_Document_ID = 
(SELECT VADMS_Document_ID FROM VADMS_MetaData WHERE VADMS_MetaData_ID = a.Record_ID AND 
(
(SELECT VADMS_WindowDocLink_ID FROM VADMS_WindowDocLink WHERE VAF_TableView_ID = a.VAF_TableView_ID AND Record_ID = a.Record_ID) > 0 
OR
(SELECT TableName FROM VAF_TableView WHERE VAF_TableView_ID = a.VAF_TableView_ID) = 'VADMS_MetaData'
))
) AS DocumentNameValue
";
            }


            sql += @" SELECT a.*
" + dmsCheck + @" 
                            FROM VAF_WFlow_Task a
                            WHERE a.Processed  ='N'
                            AND a.WFState      ='OS'
                            AND a.VAF_Client_ID =" + VAF_Client_ID + @" 
                            AND ( (a.VAF_UserContact_ID=" + VAF_UserContact_ID + @" 
                            OR a.VAF_UserContact_ID   IN
                              (SELECT VAF_UserContact_ID
                              FROM VAF_UserContact_Standby
                              WHERE IsActive   ='Y'
                              AND Substitute_ID=" + VAF_UserContact_ID + @" 
                              AND (validfrom  <=sysdate)
                              AND (sysdate    <=validto )
                              ))
                            OR EXISTS
                              (SELECT *
                              FROM VAF_WFlow_Incharge r
                              WHERE a.VAF_WFlow_Incharge_ID=r.VAF_WFlow_Incharge_ID
                              AND COALESCE(r.VAF_UserContact_ID,0)=0
                              AND (a.VAF_UserContact_ID           =" + VAF_UserContact_ID + @" 
                              OR a.VAF_UserContact_ID            IS NULL
                              OR a.VAF_UserContact_ID            IN
                                (SELECT VAF_UserContact_ID
                                FROM VAF_UserContact_Standby
                                WHERE IsActive   ='Y'
                                AND Substitute_ID=" + VAF_UserContact_ID + @" 
                                AND (validfrom  <=sysdate)
                                AND (sysdate    <=validto )
                                ))
                              )
                            OR EXISTS
                              (SELECT *
                              FROM VAF_WFlow_Incharge r
                              WHERE a.VAF_WFlow_Incharge_ID=r.VAF_WFlow_Incharge_ID
                              AND (r.VAF_UserContact_ID           =" + VAF_UserContact_ID + @" 
                              OR a.VAF_UserContact_ID            IN
                                (SELECT VAF_UserContact_ID
                                FROM VAF_UserContact_Standby
                                WHERE IsActive   ='Y'
                                AND Substitute_ID=" + VAF_UserContact_ID + @" 
                                AND (validfrom  <=sysdate)
                                AND (sysdate    <=validto )
                                ))
                              )
                            OR EXISTS
                              (SELECT *
                              FROM VAF_WFlow_Incharge r
                              INNER JOIN VAF_UserContact_Roles ur
                              ON (r.VAF_Role_ID            =ur.VAF_Role_ID)
                              WHERE a.VAF_WFlow_Incharge_ID=r.VAF_WFlow_Incharge_ID
                              AND ur.IsActive = 'Y'
                              AND (ur.VAF_UserContact_ID          =" + VAF_UserContact_ID + @" 
                              OR a.VAF_UserContact_ID            IN
                                (SELECT VAF_UserContact_ID
                                FROM VAF_UserContact_Standby
                                WHERE IsActive   ='Y'
                                AND Substitute_ID=" + VAF_UserContact_ID + @" 
                                AND (validfrom  <=sysdate)
                                AND (sysdate    <=validto )
                                ))
                              AND r.responsibletype !='H' AND r.responsibletype !='C'
                              ) ) ";

            // if (VAF_Screen_ID > 0 || (!string.IsNullOrEmpty(searchText) && searchText.Length > 0))
            if (whereClause.Length > 7)
            {
                // Applied Role access on workflow Activities
                sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "a", true, true) + @" )  MyTable ";

                sql += fromClause;
                sql += whereClause;
                sql += "  ORDER BY myTable.Priority DESC, myTable.Created DESC";

                //LEFT OUTER Join 
                //                           VAF_WFlow_Task Wf On Abc.VAF_WFlow_Task_Id=Wf.VAF_WFlow_Task_Id
                //                           LEFT OUTER Join VAF_WFlow_Node Wfn On Wfn.VAF_WFlow_Node_Id=Wf.VAF_WFlow_Node_Id
                //                           WHERE upper(wfn.value) like Upper('%" + searchText + "%') OR upper(wfn.Name) like Upper('%" + searchText + @"%')
                //                           ORDER BY Abc.Priority DESC, Abc.Created";
            }
            else
            {
                sql += "  ORDER BY Priority DESC, Created DESC";
                // Applied Role access on workflow Activities
                sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "a", true, true);
            }

            //temp ORDER BY Created desc,a.Priority DESC
            //final  ORDER BY a.Priority DESC,Created
            //int VAF_UserContact_ID = Envs.GetContext().GetVAF_UserContact_ID();
            try
            {
                //SqlParameter[] param = new SqlParameter[2];
                //param[0] = new SqlParameter("@clientid", VAF_Client_ID);
                //param[1] = new SqlParameter("@userid", VAF_UserContact_ID);
                VLogger.Get().Log(Level.SEVERE, sql);
                DataSet ds = VIS.DBase.DB.ExecuteDatasetPaging(sql, pageNo, pageSize);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return null;
                }
                List<WFActivityInfo> lstInfo = new List<WFActivityInfo>();
                WFActivityInfo itm = null;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    itm = new WFActivityInfo();

                    itm.VAF_TableView_ID = Util.GetValueOfInt(dr["VAF_TableView_ID"]);
                    itm.VAF_UserContact_ID = Util.GetValueOfInt(dr["VAF_UserContact_ID"]);
                    itm.VAF_WFlow_Task_ID = Util.GetValueOfInt(dr["VAF_WFlow_Task_ID"]);

                    itm.AD_Node_ID = Util.GetValueOfInt(dr["VAF_WFlow_Node_ID"]);
                    itm.VAF_WFlow_Handler_ID = Util.GetValueOfInt(dr["VAF_WFlow_Handler_ID"]);
                    itm.VAF_WFlow_Incharge_ID = Util.GetValueOfInt(dr["VAF_WFlow_Incharge_ID"]);
                    itm.AD_Workflow_ID = Util.GetValueOfInt(dr["AD_Workflow_ID"]);
                    itm.CreatedBy = Util.GetValueOfInt(dr["CreatedBy"]);
                    itm.DynPriorityStart = Util.GetValueOfInt(dr["DynPriorityStart"]);
                    itm.Record_ID = Util.GetValueOfInt(dr["Record_ID"]);

                    itm.DocumentNameValue = "";

                    if (Env.IsModuleInstalled("VADMS_"))
                    {
                        itm.DocumentNameValue = Util.GetValueOfString(dr["DocumentNameValue"]);
                    }

                    itm.TxtMsg = Util.GetValueOfString(dr["TextMsg"]);
                    itm.WfState = Util.GetValueOfString(dr["WfState"]);
                    itm.EndWaitTime = Util.GetValueOfDateTime(dr["EndWaitTime"]);
                    itm.Created = Util.GetValueOfString(dr["Created"]);
                    MWFActivity act = new MWFActivity(ctx, itm.VAF_WFlow_Task_ID, null);
                    itm.NodeName = act.GetNodeName();
                    itm.Summary = act.GetSummary();
                    itm.Description = act.GetNodeDescription();
                    itm.Help = act.GetNodeHelp();
                    itm.History = act.GetHistoryHTML();
                    itm.Priority = Util.GetValueOfInt(dr["Priority"]);
                    itm.VAF_Screen_ID = Util.GetValueOfInt(dr["VAF_Screen_ID"]);
                    lstInfo.Add(itm);

                }

                WFInfo info = new WFInfo();
                info.LstInfo = lstInfo;
                //return lstInfo;

                if (refresh)
                {
                    sql = "";
                    //if (VAF_Screen_ID > 0 || (!string.IsNullOrEmpty(searchText) && searchText.Length > 0))
                    //if (whereClause.Length > 7)
                    //{
                    sql = @"SELECT count(*) FROM (";
                    //}
                    sql += @" SELECT a.*
                            FROM VAF_WFlow_Task a
                            WHERE a.Processed  ='N'
                            AND a.WFState      ='OS'
                            AND a.VAF_Client_ID =" + ctx.GetVAF_Client_ID() + @"
                            AND ( (a.VAF_UserContact_ID=" + ctx.GetVAF_UserContact_ID() + @"
                            OR a.VAF_UserContact_ID   IN
                              (SELECT VAF_UserContact_ID
                              FROM VAF_UserContact_Standby
                              WHERE IsActive   ='Y'
                              AND Substitute_ID=" + ctx.GetVAF_UserContact_ID() + @"
                              AND (validfrom  <=sysdate)
                              AND (sysdate    <=validto )
                              ))
                            OR EXISTS
                              (SELECT *
                              FROM VAF_WFlow_Incharge r
                              WHERE a.VAF_WFlow_Incharge_ID=r.VAF_WFlow_Incharge_ID
                              AND COALESCE(r.VAF_UserContact_ID,0)=0
                              AND (a.VAF_UserContact_ID           =" + ctx.GetVAF_UserContact_ID() + @"
                              OR a.VAF_UserContact_ID            IS NULL
                              OR a.VAF_UserContact_ID            IN
                                (SELECT VAF_UserContact_ID
                                FROM VAF_UserContact_Standby
                                WHERE IsActive   ='Y'
                                AND Substitute_ID=" + ctx.GetVAF_UserContact_ID() + @"
                                AND (validfrom  <=sysdate)
                                AND (sysdate    <=validto )
                                ))
                              )
                            OR EXISTS
                              (SELECT *
                              FROM VAF_WFlow_Incharge r
                              WHERE a.VAF_WFlow_Incharge_ID=r.VAF_WFlow_Incharge_ID
                              AND (r.VAF_UserContact_ID           =" + ctx.GetVAF_UserContact_ID() + @"
                              OR a.VAF_UserContact_ID            IN
                                (SELECT VAF_UserContact_ID
                                FROM VAF_UserContact_Standby
                                WHERE IsActive   ='Y'
                                AND Substitute_ID=" + ctx.GetVAF_UserContact_ID() + @"
                                AND (validfrom  <=sysdate)
                                AND (sysdate    <=validto )
                                ))
                              )
                            OR EXISTS
                              (SELECT *
                              FROM VAF_WFlow_Incharge r
                              INNER JOIN VAF_UserContact_Roles ur
                              ON (r.VAF_Role_ID            =ur.VAF_Role_ID)
                              WHERE a.VAF_WFlow_Incharge_ID=r.VAF_WFlow_Incharge_ID
                              AND (ur.VAF_UserContact_ID          =" + ctx.GetVAF_UserContact_ID() + @"
                              OR a.VAF_UserContact_ID            IN
                                (SELECT VAF_UserContact_ID
                                FROM VAF_UserContact_Standby
                                WHERE IsActive   ='Y'
                                AND Substitute_ID=" + ctx.GetVAF_UserContact_ID() + @"
                                AND (validfrom  <=sysdate)
                                AND (sysdate    <=validto )
                                ))
                              AND r.responsibletype !='H' AND r.responsibletype !='C'
                              ) )";
                    if (whereClause.Length > 7)
                    {
                        // Applied Role access on workflow Activities
                        sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "a", true, true) + @" )  MyTable ";

                        sql += fromClause;
                        sql += whereClause;

                        //                        sql += @" )  Abc LEFT OUTER Join 
                        //                           VAF_WFlow_Task Wf On Abc.VAF_WFlow_Task_Id=Wf.VAF_WFlow_Task_Id
                        //                           LEFT OUTER Join VAF_WFlow_Node Wfn On Wfn.VAF_WFlow_Node_Id=Wf.VAF_WFlow_Node_Id
                        //                           Where Upper(Wfn.Value) Like Upper('%" + searchText + "%') Or Upper(Wfn.Name) Like Upper('%" + searchText + @"%')
                        //                           ORDER BY Abc.Priority DESC,Abc.Created";
                    }
                    else
                    {
                        // Applied Role access on workflow Activities
                        sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "a", true, true) + "  ) MyTable";
                    }

                    info.count = Util.GetValueOfInt(DB.ExecuteScalar(sql));
                }
                return info;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// Get the next synonym
        /// </summary>
        private void SynonymNext()
        {
            int Length = _synonym.Length;
            char cc = _synonym[0];
            if (cc == 'Z')
            {
                cc = 'A';
                Length++;
            }
            else
                cc++;
            //
            _synonym = cc.ToString();
            if (Length == 1)
                return;
            _synonym += cc.ToString();
            if (Length == 2)
                return;
            _synonym += cc.ToString();
        }	//	synonymNext


        /// <summary>
        /// Where condition based on search creteria entered by user.
        /// Search from tables, textmessage and Summary column.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="table"></param>
        /// <param name="searchText"></param>
        /// <param name="VAF_Screen_ID"></param>
        private void GetWhereClause(Ctx ctx, MTable table, string searchText, int VAF_Screen_ID, int AD_Node_ID)
        {
            if (whereClause.Length > 7 && searchText.Length > 0)
            {
                whereClause += " OR ";
            }
            else
            {
                whereClause = " WHERE ";
                if (VAF_Screen_ID > 0)
                {
                    whereClause += " MyTable.VAF_Screen_ID=" + VAF_Screen_ID + " AND MyTable.VAF_WFLOW_NODE_ID=" + AD_Node_ID;
                }
            }

            if (searchText.Length > 0 && table != null)
            {
                StringBuilder sb = new StringBuilder();
                if (VAF_Screen_ID > 0)
                {
                    sb.Append(" AND (");
                }
                else
                {
                    sb.Append(" AND ");
                }

                PO _po = table.GetPO(ctx, 0, null);

                //StringBuilder sb = new StringBuilder(" AND UPPER( ' '");
                sb.Append(" UPPER( ' '");
                int index = _po.Get_ColumnIndex("DocumentNo");
                if (index != -1)
                {
                    sb.Append(" || ' '  || ").Append(_synonym).Append(".").Append("DocumentNo");
                }
                index = _po.Get_ColumnIndex("Value");
                if (index != -1)
                {
                    sb.Append(" || ' '  || ").Append(_synonym).Append(".").Append("Value");
                }
                index = _po.Get_ColumnIndex("Name");
                if (index != -1)
                {
                    sb.Append(" || ' '  || ").Append(_synonym).Append(".").Append("Name");
                }
                index = _po.Get_ColumnIndex("SalesRep_ID");

                string localSynonmus = _synonym;

                //int? sr = null;
                if (index != -1)
                {
                    GetFromClause(ctx, "VAF_UserContact", table.GetTableName(), localSynonmus, "SalesRep_ID");
                }
                //sr = (int?)_po.Get_Value(index);
                else
                {
                    index = _po.Get_ColumnIndex("VAF_UserContact_ID");
                    //if (index != -1)
                    //    sr = (int?)_po.Get_Value(index);
                }
                if (index != -1)
                {
                    //MUser user = MUser.Get(ctx, sr.Value);
                    //if (user != null)
                    //    sb.Append(user.GetName()).Append(" ");

                    SynonymNext();

                    GetFromClause(ctx, "VAF_UserContact", table.GetTableName(), localSynonmus, "VAF_UserContact_ID");

                    sb.Append(" || ' '  || ").Append(_synonym).Append(".").Append("Name");

                }
                //
                index = _po.Get_ColumnIndex("C_BPartner_ID");
                if (index != -1)
                {
                    // int? bp = (int?)_po.Get_Value(index);
                    //if (bp != null)
                    {
                        //MBPartner partner = MBPartner.Get(ctx, bp.Value);
                        //if (partner != null)
                        //    sb.Append(partner.GetName()).Append(" ");

                        SynonymNext();

                        GetFromClause(ctx, "C_BPartner", table.GetTableName(), localSynonmus, "");

                        sb.Append(" || ' ' || ").Append(_synonym).Append(".").Append("Name");

                    }
                }



                sb.Append(")");

                whereClause += sb + " like upper('%" + searchText + "%')";
                if (VAF_Screen_ID > 0)
                {
                    whereClause += " OR Upper(myTable.TextMsg) like Upper('%" + searchText + "%') OR Upper(myTable.Summary) like Upper('%" + searchText + "%'))";
                }
                else
                {
                    if (whereClause.Length > 7)
                    {
                        whereClause += " AND (Upper(myTable.TextMsg) like Upper('%" + searchText + "%') OR Upper(myTable.Summary) like Upper('%" + searchText + "%') )";
                    }
                    else
                    {
                        whereClause += " (Upper(myTable.TextMsg) like Upper('%" + searchText + "%') OR Upper(myTable.Summary) like Upper('%" + searchText + "%'))";
                    }
                }

            }
            else if (searchText.Length > 0)
            {
                if (whereClause.Length > 7)
                {
                    whereClause += " AND ( Upper(myTable.TextMsg) like Upper('%" + searchText + "%') OR Upper(myTable.Summary) like Upper('%" + searchText + "%'))";
                }
                else
                {
                    whereClause += " (Upper(myTable.TextMsg) like Upper('%" + searchText + "%') OR Upper(myTable.Summary) like Upper('%" + searchText + "%'))";
                }
            }



        }


        /// <summary>
        /// Where clasuse based on date entered.
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="searchText"></param>
        private void GetDateWiseWhereClause(DateTime? dateFrom, DateTime? dateTo, string searchText)
        {
            if (dateFrom != null || dateTo != null)
            {
                if (whereClause.Length == 0)
                {
                    whereClause += " WHERE ";
                }
                else if (whereClause.Length > 7)
                {
                    whereClause += " AND ";
                }
            }
            if (dateFrom != null && dateTo != null)
            {
                whereClause += " myTable.created Between " + GlobalVariable.TO_DATE(dateFrom, false) + " AND " + GlobalVariable.TO_DATE(Convert.ToDateTime(dateTo).AddDays(1), false);
            }
            else if (dateFrom != null)
            {
                whereClause += " myTable.created >= " + GlobalVariable.TO_DATE(dateFrom, false);
            }
            else if (dateTo != null)
            {
                whereClause += " myTable.created <= " + GlobalVariable.TO_DATE(Convert.ToDateTime(dateTo).AddDays(1), false);
            }



        }

        /// <summary>
        /// Create From Clause
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="tablename"></param>
        /// <param name="joinTable"></param>
        /// <param name="localSynonumns"></param>
        /// <param name="columnName"></param>
        private void GetFromClause(Ctx ctx, string tablename, string joinTable, string localSynonumns, string columnName)
        {
            fromClause += " LEFT JOIN " + tablename + " " + _synonym + " ON " + _synonym + "." + tablename + "_ID = ";
            if (!string.IsNullOrEmpty(joinTable))
            {
                if (!string.IsNullOrEmpty(columnName))
                {
                    fromClause += localSynonumns + "." + columnName;
                }
                else
                {
                    fromClause += localSynonumns + "." + tablename + "_ID";
                }
            }
            else
            {
                fromClause += "MyTable.Record_ID";
            }
        }

        public ActivityInfo GetActivityInfo(int activityID, int nodeID, int wfProcessID, Ctx ctx)
        {
            ActivityInfo info = new ActivityInfo();
            try
            {
                MWFNode node = new MWFNode(ctx, nodeID, null);
                info.NodeAction = node.GetAction();
                info.NodeName = node.GetName();
                if (MWFNode.ACTION_UserChoice.Equals(node.GetAction()))
                {
                    MColumn col = node.GetColumn();
                    info.ColID = col.GetVAF_Column_ID();
                    info.ColReference = col.GetVAF_Control_Ref_ID();
                    info.ColReferenceValue = col.GetVAF_Control_Ref_Value_ID();
                    info.ColName = col.GetColumnName();
                }
                else if (MWFNode.ACTION_UserWindow.Equals(node.GetAction()))
                {
                    info.VAF_Screen_ID = node.GetVAF_Screen_ID();
                    MWFActivity activity = new MWFActivity(ctx, activityID, null);
                    info.KeyCol = activity.GetPO().Get_TableName() + "_ID";
                }
                else if (MWFNode.ACTION_UserForm.Equals(node.GetAction()))
                {
                    info.VAF_Page_ID = node.GetVAF_Page_ID();
                }



                string sql = @"SELECT node.VAF_WFlow_Node_ID,
                                  node.Name AS NodeName,
                                  usr.Name AS UserName,
                                  wfea.wfstate,
                                  wfea.TextMsg
                              FROM VAF_WFlow_EventLog wfea
                                INNER JOIN VAF_WFlow_Node node
                                ON (node.VAF_WFlow_Node_ID=wfea.VAF_WFlow_Node_id)
                                INNER JOIN VAF_UserContact usr
                                ON (usr.VAF_UserContact_ID         =wfea.VAF_UserContact_ID)
                              WHERE wfea.VAF_WFlow_Handler_ID=" + wfProcessID + @"
                              Order By wfea.VAF_WFlow_EventLog_id desc";
                DataSet ds = DB.ExecuteDataset(sql);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    List<NodeInfo> nodeInfo = new List<NodeInfo>();
                    List<int> nodes = new List<int>();
                    NodeInfo ni = null;
                    NodeHistory nh = null;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (!nodes.Contains(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_WFlow_Node_ID"])))
                        {
                            ni = new NodeInfo();
                            ni.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["NodeName"]);
                            nh = new NodeHistory();
                            nh.State = Util.GetValueOfString(ds.Tables[0].Rows[i]["WFState"]);
                            nh.ApprovedBy = Util.GetValueOfString(ds.Tables[0].Rows[i]["UserName"]);
                            ni.History = new List<NodeHistory>();

                            if (ds.Tables[0].Rows[i]["TextMsg"] == null || ds.Tables[0].Rows[i]["TextMsg"] == DBNull.Value)
                            {
                                nh.TextMsg = string.Empty;
                            }
                            else
                            {
                                nh.TextMsg = ds.Tables[0].Rows[i]["TextMsg"].ToString();
                            }
                            ni.History.Add(nh);
                            nodes.Add(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_WFlow_Node_ID"]));
                            nodeInfo.Add(ni);
                        }
                        else
                        {
                            int index = nodes.IndexOf(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_WFlow_Node_ID"]));
                            nh = new NodeHistory();
                            nh.State = Util.GetValueOfString(ds.Tables[0].Rows[i]["WFState"]);
                            nh.ApprovedBy = Util.GetValueOfString(ds.Tables[0].Rows[i]["UserName"]);
                            if (ds.Tables[0].Rows[i]["TextMsg"] == null || ds.Tables[0].Rows[i]["TextMsg"] == DBNull.Value)
                            {
                                nh.TextMsg = string.Empty;
                            }
                            else
                            {
                                nh.TextMsg = ds.Tables[0].Rows[i]["TextMsg"].ToString();
                            }
                            nodeInfo[index].History.Add(nh);
                        }
                    }
                    info.Node = nodeInfo;

                }

                return info;

            }
            catch
            {
                return info;
            }
        }
        /// <summary>
        /// Approve Activities
        /// </summary>
        /// <param name="nodeID"> Noda ID </param>
        /// <param name="activityID"> string of activites separated by comma </param>
        /// <param name="textMsg">Message used while approving or forwarding activiy</param>
        /// <param name="forward">VAF_UserContact_ID to whom activity is forwarded</param>
        /// <param name="answer"> Message</param>
        /// <param name="ctx">Context</param>
        /// <param name="VAF_Screen_ID">VAF_Screen_ID</param>
        /// <returns></returns>
        public string ApproveIt(int nodeID, string activityID, string textMsg, object forward, object answer, Ctx ctx, int VAF_Screen_ID)
        {
            if (!string.IsNullOrEmpty(activityID) && activityID.Length > 0)
            {
                List<int> activitiesID = activityID.Split(',').Select(int.Parse).ToList();
                activitiesID.Sort();
                for (int i = 0; i < activitiesID.Count; i++)
                {
                    MWFActivity activity = new MWFActivity(ctx, Convert.ToInt32(activitiesID[i]), null);
                    MWFNode node = activity.GetNode();
                    // Change done to set zoom window from Node
                    if (node.GetZoomWindow_ID() > 0)
                        activity.SetVAF_Screen_ID(node.GetZoomWindow_ID());
                    int approvalLevel = node.GetApprovalLeval();
                    int VAF_UserContact_ID = ctx.GetVAF_UserContact_ID();
                    MColumn column = node.GetColumn();

                    if (forward != null) // Prefer Forward 
                    {
                        int fw = int.Parse(forward.ToString());
                        if (fw == VAF_UserContact_ID || fw == 0)
                        {
                            return "";
                        }
                        if (!activity.ForwardTo(fw, textMsg, true, true))
                        {
                            return "CannotForward";
                        }
                    }
                    //	User Choice - Answer
                    else if (MWFNode.ACTION_UserChoice.Equals(node.GetAction()))
                    {
                        if (column == null)
                            column = node.GetColumn();
                        //	Do we have an answer?
                        int dt = column.GetVAF_Control_Ref_ID();
                        String value = null;
                        value = answer != null ? answer.ToString() : null;
                        //if (dt == DisplayType.YesNo || dt == DisplayType.List || dt == DisplayType.TableDir)
                        if (!node.IsMultiApproval() &&
                            (dt == DisplayType.YesNo || dt == DisplayType.List || dt == DisplayType.TableDir))
                        {
                            if (value == null || value.Length == 0)
                            {
                                return "FillMandatory";
                            }
                            //
                            string res = SetUserChoice(VAF_UserContact_ID, value, dt, textMsg, activity, node, VAF_Screen_ID);
                            if (res != "OK")
                            {
                                return res;
                            }
                        }
                        //Genral Attribute Instance
                        //else if (column.GetColumnName().ToUpper().Equals("C_GENATTRIBUTESETINSTANCE_ID"))
                        //{
                        //    if (attrib == null)
                        //    {
                        //        Dispatcher.BeginInvoke(delegate
                        //        {
                        //            SetBusy(false);
                        //            ShowMessage.Error("FillMandatory", true, Msg.GetMsg(Envs.GetContext(), "Answer", true));
                        //            //log.Config("Answer=" + value + " - " + textMsg);
                        //            return;
                        //        });
                        //        return;
                        //    }

                        //    SetUserChoice(VAF_UserContact_ID, attrib.GetAttributeSetInstance().ToString(), 0, textMsg, activity, node);
                        //}

                        else if (forward == null && node.IsMultiApproval() && approvalLevel > 0 && answer.ToString().Equals("Y"))
                        {

                            int eventCount = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(WFE.VAF_WFlow_EventLog_ID) FROM VAF_WFlow_EventLog WFE
                                                                                INNER JOIN VAF_WFlow_Handler WFP ON (WFP.VAF_WFlow_Handler_ID=WFE.VAF_WFlow_Handler_ID)
                                                                                INNER JOIN VAF_WFlow_Task WFA ON (WFA.VAF_WFlow_Handler_ID=WFP.VAF_WFlow_Handler_ID)
                                                                                WHERE WFE.VAF_WFlow_Node_ID=" + node.GetVAF_WFlow_Node_ID() + " AND WFA.VAF_WFlow_Task_ID=" + activity.GetVAF_WFlow_Task_ID()));
                            if (eventCount < approvalLevel) //Forward Activity
                            {
                                int VAF_WFlow_Incharge_ID = 0;
                                if (node.GetVAF_WFlow_Incharge_ID() > 0)
                                    VAF_WFlow_Incharge_ID = node.GetVAF_WFlow_Incharge_ID();
                                else if (node.GetWorkflow().GetVAF_WFlow_Incharge_ID() > 0)
                                    VAF_WFlow_Incharge_ID = node.GetWorkflow().GetVAF_WFlow_Incharge_ID();
                                MWFResponsible resp = new MWFResponsible(ctx, VAF_WFlow_Incharge_ID, null);
                                int superVisiorID = 0;
                                bool setRespOrg = false;
                                int parentOrg_ID = -1;
                                if (resp.IsOrganization())
                                {
                                    parentOrg_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Parent_Org_Id FROM VAF_OrgDetail WHERE VAF_Org_ID = " + activity.GetResponsibleOrg_ID()));
                                    superVisiorID = (new MOrgInfo(ctx, parentOrg_ID, null).GetSupervisor_ID());
                                    if (superVisiorID > 0)
                                        setRespOrg = true;
                                }
                                else
                                    superVisiorID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Supervisor_ID FROM VAF_UserContact WHERE IsActive='Y' AND VAF_UserContact_ID=" + activity.GetVAF_UserContact_ID()));
                                if (setRespOrg)
                                    activity.SetResponsibleOrg_ID(parentOrg_ID);
                                if (superVisiorID == 0)//Approve
                                {
                                    //SetUserConfirmation(VAF_UserContact_ID, textMsg, activity, node);

                                    string res = SetUserChoice(VAF_UserContact_ID, value, dt, textMsg, activity, node, VAF_Screen_ID);
                                    if (res != "OK")
                                    {
                                        return res;
                                    }
                                }
                                else //forward
                                {

                                    if (!activity.ForwardTo(superVisiorID, textMsg, true))
                                    {
                                        //Dispatcher.BeginInvoke(delegate
                                        //{
                                        //    SetBusy(false);
                                        //    ShowMessage.Error("CannotForward", true);
                                        //    return;
                                        //});
                                        return "CannotForward";
                                    }
                                }
                            }
                            else //Approve
                            {

                                //SetUserConfirmation(VAF_UserContact_ID, textMsg, activity, node);

                                string res = SetUserChoice(VAF_UserContact_ID, value, dt, textMsg, activity, node, VAF_Screen_ID);
                                if (res != "OK")
                                {
                                    return res;
                                }
                            }
                        }
                        else
                        {

                            string res = SetUserChoice(VAF_UserContact_ID, value, dt, textMsg, activity, node, VAF_Screen_ID);
                            if (res != "OK")
                            {
                                return res;
                            }
                        }


                    }
                    //	User Action
                    else
                    {
                        //   log.Config("Action=" + node.GetAction() + " - " + textMsg);
                        //try
                        //{
                        //    activity.SetUserConfirmation(VAF_UserContact_ID, textMsg);
                        //}
                        //catch (Exception exx)
                        //{
                        //    Dispatcher.BeginInvoke(delegate
                        //            {
                        //                SetBusy(false);
                        //                log.Log(Level.SEVERE, node.GetName(), exx);
                        //                ShowMessage.Error("Error", true, exx.ToString());
                        //                return;
                        //            });
                        //    return;
                        //}
                        activity.SetUserConfirmation(VAF_UserContact_ID, textMsg);

                    }
                }
            }

            return "";
        }

        /// <summary>
        /// function to check for non 0 user id and to check whether user is active or not
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="User_ID"></param>
        /// <returns>string message if user not found or user not active</returns>
        private string CheckUser(Ctx ctx, int User_ID)
        {
            if (User_ID == 0)
                return Msg.GetMsg(ctx, "ApproverNotFound");
            else
            {
                bool chkActiveUser = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsActive FROM VAF_UserContact WHERE VAF_UserContact_ID = " + User_ID)).ToLower() == "y";
                if (!chkActiveUser)
                    return Msg.GetMsg(ctx, "ApproverNotActive"); ;
            }
            return "";
        }

        private string SetUserChoice(int VAF_UserContact_ID, string value, int dt, string textMsg, MWFActivity _activity, MWFNode _node, int VAF_Screen_ID)
        {
            try
            {
                // Passed VAF_Screen_ID to process to set windowID at all levels of current activity
                _activity.SetUserChoice(VAF_UserContact_ID, value, dt, textMsg,VAF_Screen_ID);
                return "OK";
            }
            catch (Exception ex)
            {
                //Dispatcher.BeginInvoke(delegate
                //{
                //    SetBusy(false);
                //    log.Log(Level.SEVERE, _node.GetName(), ex);
                //    ShowMessage.Error("Error", true, ex.ToString());
                //    return;
                //});
                return "Error" + ex.Message;
            }
        }

        public AttributeInfo GetRelativeData(Ctx ctx, int activityID)
        {
            try
            {
                AttributeInfo aInfo = new AttributeInfo();
                DataSet ds = DB.ExecuteDataset(@"SELECT 
                                                            WFP.VAF_TableView_ID,
                                                            WFP.Record_ID
                                                            FROM VAF_WFlow_Handler WFP
                                                            INNER JOIN VAF_WFlow_Task WFA
                                                            ON (WFA.VAF_WFlow_Handler_ID=WFP.VAF_WFlow_Handler_ID)
                                                            WHERE WFA.VAF_WFlow_Task_ID=" + activityID, null);
                PO doc = GetPO(Util.GetValueOfInt(ds.Tables[0].Rows[0][0]), Util.GetValueOfInt(ds.Tables[0].Rows[0][1]), ctx);
                ds = null;
                aInfo.GenAttributeSetID = Util.GetValueOfInt(doc.Get_Value("C_GenAttributeSet_ID"));
                aInfo.GenAttributeSetInstanceID = Util.GetValueOfInt(doc.Get_Value("C_GenAttributeSetInstance_ID"));
                doc = null;
                doc = GetPO("C_GenAttributeSetInstance", aInfo.GenAttributeSetInstanceID, ctx);
                aInfo.Description = Util.GetValueOfString(doc.Get_Value("Description"));
                return aInfo;
            }
            catch
            {
                return null;
            }

        }

        private PO GetPO(int tableID, int recordID, Ctx ctx)
        {
            MTable table = MTable.Get(ctx, tableID);
            return table.GetPO(ctx, recordID, null);

        }

        private PO GetPO(string tableName, int recordID, Ctx ctx)
        {
            //throw new NotImplementedException();
            MTable table = MTable.Get(ctx, Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAF_TableView_ID FROM VAF_TableView WHERE TableName='" + tableName + "'")));
            return table.GetPO(ctx, recordID, null);
        }

        /// <summary>
        /// Load all windows for which workflow activies are open
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public List<WorkflowWindowList> GetWorkflowWindows(Ctx ctx)
        {
            List<WorkflowWindowList> list = new List<WorkflowWindowList>();
            bool baseLang = Env.IsBaseLanguage(ctx, "");
            string sql = "";
            if (baseLang)
            {
                sql = @"SELECT DISTINCT VAF_Screen.AD_window_ID,  VAF_Screen.DisplayName  || ' (' || VAF_WFlow_Node.Name || ')' As Name,VAF_WFLOW_NODEVAF_WFlow_Node_Para
.VAF_WFLOW_NODE_ID FROM VAF_WFlow_Task VAF_WFlow_Task
                            JOIN VAF_Screen VAF_Screen ON VAF_WFlow_Task.Ad_Window_Id = VAF_Screen.Ad_Window_Id
                            JOIN VAF_WFLOW_NODE VAF_WFLOW_NODE ON VAF_WFLOW_NODE.VAF_WFLOW_NODE_ID=VAF_WFlow_Task.VAF_WFLOW_NODE_ID
                            WHERE VAF_Screen.IsActive ='Y'  AND VAF_WFlow_Task.Processed = 'N'  AND VAF_WFlow_Task.WFState      ='OS' ";
                sql += " AND VAF_WFlow_Task.VAF_Client_ID =" + ctx.GetVAF_Client_ID() + @" 
                            AND  ((VAF_WFlow_Task.VAF_UserContact_ID=" + ctx.GetVAF_UserContact_ID() + @" 
                            OR VAF_WFlow_Task.VAF_UserContact_ID   IN
                              (SELECT VAF_UserContact_ID
                              FROM VAF_UserContact_Standby
                              WHERE IsActive   ='Y'
                              AND Substitute_ID=" + ctx.GetVAF_UserContact_ID() + @" 
                              AND (validfrom  <=sysdate)
                              AND (sysdate    <=validto )
                              ))
                                OR EXISTS
                                  (SELECT *
                                  FROM VAF_WFlow_Incharge r
                                  WHERE VAF_WFlow_Task.VAF_WFlow_Incharge_ID=r.VAF_WFlow_Incharge_ID
                                  AND COALESCE(r.VAF_UserContact_ID,0)=0
                                  AND (VAF_WFlow_Task.VAF_UserContact_ID           =" + ctx.GetVAF_UserContact_ID() + @"
                                  OR VAF_WFlow_Task.VAF_UserContact_ID            IS NULL
                                  OR VAF_WFlow_Task.VAF_UserContact_ID            IN
                                    (SELECT VAF_UserContact_ID
                                    FROM VAF_UserContact_Standby
                                    WHERE IsActive   ='Y'
                                    AND Substitute_ID=" + ctx.GetVAF_UserContact_ID() + @"
                                    AND (validfrom  <=sysdate)
                                    AND (sysdate    <=validto )
                                    ))
                                  )
                                OR EXISTS
                                  (SELECT *
                                  FROM VAF_WFlow_Incharge r
                                  WHERE VAF_WFlow_Task.VAF_WFlow_Incharge_ID=r.VAF_WFlow_Incharge_ID
                                  AND (r.VAF_UserContact_ID           =" + ctx.GetVAF_UserContact_ID() + @"
                                  OR VAF_WFlow_Task.VAF_UserContact_ID            IN
                                    (SELECT VAF_UserContact_ID
                                    FROM VAF_UserContact_Standby
                                    WHERE IsActive   ='Y'
                                    AND Substitute_ID=" + ctx.GetVAF_UserContact_ID() + @"
                                    AND (validfrom  <=sysdate)
                                    AND (sysdate    <=validto )
                                    ))
                                  )
                                OR EXISTS
                                  (SELECT *
                                  FROM VAF_WFlow_Incharge r
                                  INNER JOIN VAF_UserContact_Roles ur
                                  ON (r.VAF_Role_ID            =ur.VAF_Role_ID)
                                  WHERE VAF_WFlow_Task.VAF_WFlow_Incharge_ID=r.VAF_WFlow_Incharge_ID
                                  AND (ur.VAF_UserContact_ID          =" + ctx.GetVAF_UserContact_ID() + @"
                                  OR VAF_WFlow_Task.VAF_UserContact_ID            IN
                                    (SELECT VAF_UserContact_ID
                                    FROM VAF_UserContact_Standby
                                    WHERE IsActive   ='Y'
                                    AND Substitute_ID=" + ctx.GetVAF_UserContact_ID() + @"
                                    AND (validfrom  <=sysdate)
                                    AND (sysdate    <=validto )
                                    ))
                                  And R.Responsibletype !='H'
                                  ))";
            }
            else
            {
                sql = @"SELECT DISTINCT Ad_Window.Ad_Window_Id,
                        VAF_Screen_TL.Name || ' (' || VAF_WFlow_Node.Name || ')' As Name,VAF_WFLOW_NODE.VAF_WFLOW_NODE_ID
                        FROM VAF_WFlow_Task VAF_WFlow_Task
                        JOIN VAF_Screen VAF_Screen
                        ON VAF_WFlow_Task.Ad_Window_Id = Ad_Window.Ad_Window_Id
                        JOIN VAF_Screen_TL VAF_Screen_TL
                        ON VAF_Screen_TL.VAF_Screen_ID=AD_window.AD_window_ID
                        JOIN VAF_WFLOW_NODE VAF_WFLOW_NODE ON VAF_WFLOW_NODE.VAF_WFLOW_NODE_ID=VAF_WFlow_Task.VAF_WFLOW_NODE_ID
                        WHERE VAF_Screen.IsActive     ='Y'
                        AND VAF_WFlow_Task.Processed = 'N'
                        AND VAF_WFlow_Task.WFState   ='OS' AND VAF_Language='" + Env.GetVAF_Language(ctx) + "'";
                sql += " AND VAF_WFlow_Task.VAF_Client_ID =" + ctx.GetVAF_Client_ID() + @" 
                            AND  ((VAF_WFlow_Task.VAF_UserContact_ID=" + ctx.GetVAF_UserContact_ID() + @" 
                            OR VAF_WFlow_Task.VAF_UserContact_ID   IN
                              (SELECT VAF_UserContact_ID
                              FROM VAF_UserContact_Standby
                              WHERE IsActive   ='Y'
                              AND Substitute_ID=" + ctx.GetVAF_UserContact_ID() + @" 
                              AND (validfrom  <=sysdate)
                              AND (sysdate    <=validto )
                              ))
                                  OR EXISTS
                                  (SELECT *
                                  FROM VAF_WFlow_Incharge r
                                  WHERE VAF_WFlow_Task.VAF_WFlow_Incharge_ID=r.VAF_WFlow_Incharge_ID
                                  AND COALESCE(r.VAF_UserContact_ID,0)=0
                                  AND (VAF_WFlow_Task.VAF_UserContact_ID           =" + ctx.GetVAF_UserContact_ID() + @"
                                  OR VAF_WFlow_Task.VAF_UserContact_ID            IS NULL
                                  OR VAF_WFlow_Task.VAF_UserContact_ID            IN
                                    (SELECT VAF_UserContact_ID
                                    FROM VAF_UserContact_Standby
                                    WHERE IsActive   ='Y'
                                    AND Substitute_ID=" + ctx.GetVAF_UserContact_ID() + @"
                                    AND (validfrom  <=sysdate)
                                    AND (sysdate    <=validto )
                                    ))
                                  )
                                OR EXISTS
                                  (SELECT *
                                  FROM VAF_WFlow_Incharge r
                                  WHERE VAF_WFlow_Task.VAF_WFlow_Incharge_ID=r.VAF_WFlow_Incharge_ID
                                  AND (r.VAF_UserContact_ID           =" + ctx.GetVAF_UserContact_ID() + @"
                                  OR VAF_WFlow_Task.VAF_UserContact_ID            IN
                                    (SELECT VAF_UserContact_ID
                                    FROM VAF_UserContact_Standby
                                    WHERE IsActive   ='Y'
                                    AND Substitute_ID=" + ctx.GetVAF_UserContact_ID() + @"
                                    AND (validfrom  <=sysdate)
                                    AND (sysdate    <=validto )
                                    ))
                                  )
                                OR EXISTS
                                  (SELECT *
                                  FROM VAF_WFlow_Incharge r
                                  INNER JOIN VAF_UserContact_Roles ur
                                  ON (r.VAF_Role_ID            =ur.VAF_Role_ID)
                                  WHERE VAF_WFlow_Task.VAF_WFlow_Incharge_ID=r.VAF_WFlow_Incharge_ID
                                  AND (ur.VAF_UserContact_ID          =" + ctx.GetVAF_UserContact_ID() + @"
                                  OR VAF_WFlow_Task.VAF_UserContact_ID            IN
                                    (SELECT VAF_UserContact_ID
                                    FROM VAF_UserContact_Standby
                                    WHERE IsActive   ='Y'
                                    AND Substitute_ID=" + ctx.GetVAF_UserContact_ID() + @"
                                    AND (validfrom  <=sysdate)
                                    AND (sysdate    <=validto )
                                    ))
                                  And R.Responsibletype !='H'
                                  ))";
            }



            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "VAF_WFlow_Task", true, true);
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    WorkflowWindowList wwl = new WorkflowWindowList();
                    wwl.WindowName = Convert.ToString(ds.Tables[0].Rows[i]["Name"]);
                    wwl.VAF_Screen_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Screen_ID"]);
                    wwl.AD_Node_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_WFLOW_NODE_ID"]);
                    list.Add(wwl);
                }
            }
            return list;
        }
    }

    public class WorkflowWindowList
    {
        public int VAF_Screen_ID { get; set; }
        public string WindowName { get; set; }
        public int AD_Node_ID { get; set; }
    }

    public class WFInfo
    {
        public List<WFActivityInfo> LstInfo
        {
            get;
            set;
        }
        public int count
        {
            get;
            set;
        }

    }

    public class WFActivityInfo
    {

        public int VAF_TableView_ID
        {
            get;
            set;
        }
        public int VAF_UserContact_ID
        {
            get;
            set;
        }
        public int VAF_WFlow_Task_ID
        {
            get;
            set;
        }
        public int AD_Node_ID
        {
            get;
            set;
        }
        public int VAF_WFlow_Handler_ID
        {
            get;
            set;
        }
        public int VAF_WFlow_Incharge_ID
        {
            get;
            set;
        }
        public int AD_Workflow_ID
        {
            get;
            set;
        }

        public int CreatedBy
        {
            get;
            set;
        }
        public int DynPriorityStart
        {
            get;
            set;
        }
        public DateTime? EndWaitTime
        {
            get;
            set;
        }
        public int Record_ID
        {
            get;
            set;
        }
        public string TxtMsg
        {
            get;
            set;
        }
        public string WfState
        {
            get;
            set;
        }
        public string NodeName
        {
            get;
            set;
        }

        public string Summary
        {
            get;
            set;
        }
        public string Created
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public string Help
        {
            get;
            set;
        }
        public string History
        {
            get;
            set;
        }
        public int Priority
        {
            get;
            set;
        }

        public int VAF_Screen_ID
        {
            get;
            set;
        }

        public string DocumentNameValue
        {
            get;
            set;
        }
    }

    public class ActivityInfo
    {
        public string NodeAction
        {
            get;
            set;
        }
        public string NodeName
        {
            get;
            set;
        }
        public string KeyCol
        {
            get;
            set;
        }
        public int ColID
        {
            get;
            set;
        }
        public int ColReference
        {
            get;
            set;
        }
        public string ColName
        {
            get;
            set;
        }
        public int ColReferenceValue
        {
            get;
            set;
        }
        public int VAF_Screen_ID
        {
            get;
            set;
        }
        public int VAF_Page_ID
        {
            get;
            set;
        }
        public List<NodeInfo> Node
        {
            get;
            set;
        }

    }

    public class NodeInfo
    {
        public string Name
        {
            get;
            set;
        }
        public List<NodeHistory> History
        {
            get;
            set;
        }

    }

    public class NodeHistory
    {
        public string State
        {
            get;
            set;
        }
        public string ApprovedBy
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string TextMsg
        {
            get;
            set;
        }
    }

    public class AttributeInfo
    {
        public int GenAttributeSetID
        {
            get;
            set;
        }
        public int GenAttributeSetInstanceID
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }

    }
}