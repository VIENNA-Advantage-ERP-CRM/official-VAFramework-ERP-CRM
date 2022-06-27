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
        /// <param name="AD_User_ID">Login User ID</param>
        /// <param name="AD_Client_ID"> Login Client ID </param>
        /// <param name="pageNo"> Current Page Number </param>
        /// <param name="pageSize"> Sepcify number of records per page</param>
        /// <param name="refresh"> Refresh Data? </param>
        /// <param name="searchText"> search Activities based on summary </param>
        /// <param name="AD_Window_ID"> Window ID based on which serach Activities </param>
        /// <param name="dateFrom">Activities Start From</param>
        /// <param name="dateTo"> Activities Date To </param>
        /// <param name="AD_Node_ID">Window ID based on which serach Activities</param>
        /// <returns></returns>
        public WFInfo GetActivities(Ctx ctx, int AD_User_ID, int AD_Client_ID, int pageNo, int pageSize, bool refresh, string searchText, int AD_Window_ID, DateTime? dateFrom, DateTime? dateTo, int AD_Node_ID)
        {
            string sql = "";
            List<MTable> mtable = new List<MTable>();
            int count = 0;

            // If window is selected or search text is available..
            if (AD_Window_ID > 0 || (!string.IsNullOrEmpty(searchText) && searchText.Length > 0))
            {
                sql = @"SELECT DISTINCT tabl.AD_table_ID,Tab.AD_Tab_ID FROM AD_Window Wind Join AD_Tab Tab 
                    ON (Wind.AD_Window_Id=Tab.AD_Window_Id) JOIN  AD_Table Tabl On (Tab.AD_Table_Id=Tabl.AD_Table_Id) 
                    WHERE Tab.IsActive    ='Y'";

                sql += " AND wind.AD_Window_ID=" + AD_Window_ID + " AND tab.AD_table_ID IN (Select Distinct AD_Table_ID FROM AD_WF_Activity where AD_Window_ID=" + AD_Window_ID + ") ORDER BY Tab.AD_Tab_ID Asc";

                //if window is selected then search for tables associated with window.
                DataSet ds = DB.ExecuteDataset(sql);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    //for each table , create where clause based on columns such as Name, DocumentNo,C_Bpatner_ID, AD_User_ID exist in that table or not.
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        count++;
                        MTable table = new MTable(ctx, Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Table_ID"]), null);

                        GetFromClause(ctx, table.GetTableName(), "", "", "");
                        GetWhereClause(ctx, table, searchText, AD_Window_ID, AD_Node_ID);
                        SynonymNext();
                    }
                }
                sql = "";
            }

            if (count == 0)
            {
                GetWhereClause(ctx, null, searchText, AD_Window_ID, AD_Node_ID);
            }

            GetDateWiseWhereClause(dateFrom, dateTo, searchText);



            // if (AD_Window_ID > 0 || (!string.IsNullOrEmpty(searchText) && searchText.Length > 0))
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
(SELECT COUNT(VADMS_WindowDocLink_ID) FROM VADMS_WindowDocLink WHERE AD_Table_ID = a.AD_Table_ID AND Record_ID = a.Record_ID AND (SELECT TableName FROM AD_Table WHERE AD_Table_ID = a.AD_Table_ID) = 'VADMS_MetaData') > 0 
OR
(SELECT TableName FROM AD_Table WHERE AD_Table_ID = a.AD_Table_ID) = 'VADMS_MetaData'
))
) AS DocumentNameValue
";
            }


            sql += @" SELECT a.*
" + dmsCheck + @" 
                            FROM AD_WF_Activity a
                            WHERE a.Processed  ='N'
                            AND a.WFState      ='OS'
                            AND a.AD_Client_ID =" + AD_Client_ID + @" 
                            AND ((a.AD_User_ID=" + AD_User_ID + @" 
                            OR a.AD_User_ID   IN
                              (SELECT AD_User_ID
                              FROM AD_User_Substitute
                              WHERE IsActive   ='Y'
                              AND Substitute_ID=" + AD_User_ID + @" 
                              AND (validfrom  <=sysdate)
                              AND (sysdate    <=validto )
                              ))
                            OR EXISTS
                              (SELECT *
                              FROM AD_WF_Responsible r
                              WHERE a.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID
                              AND COALESCE(r.AD_User_ID,0)=0
                              AND (a.AD_User_ID           =" + AD_User_ID + @" 
                              OR a.AD_User_ID            IS NULL
                              OR a.AD_User_ID            IN
                                (SELECT AD_User_ID
                                FROM AD_User_Substitute
                                WHERE IsActive   ='Y'
                                AND Substitute_ID=" + AD_User_ID + @" 
                                AND (validfrom  <=sysdate)
                                AND (sysdate    <=validto )
                                ))
                              )
                            OR EXISTS
                              (SELECT *
                              FROM AD_WF_Responsible r
                              WHERE a.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID
                              AND (r.AD_User_ID           =" + AD_User_ID + @" 
                              OR a.AD_User_ID            IN
                                (SELECT AD_User_ID
                                FROM AD_User_Substitute
                                WHERE IsActive   ='Y'
                                AND Substitute_ID=" + AD_User_ID + @" 
                                AND (validfrom  <=sysdate)
                                AND (sysdate    <=validto )
                                ))
                              )
                            OR EXISTS
                              (SELECT *
                              FROM AD_WF_Responsible r
                              INNER JOIN AD_User_Roles ur
                              ON (r.AD_Role_ID            =ur.AD_Role_ID)
                              WHERE a.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID
                              AND ur.IsActive = 'Y'
                              AND (ur.AD_User_ID          =" + AD_User_ID + @" 
                              OR a.AD_User_ID            IN
                                (SELECT AD_User_ID
                                FROM AD_User_Substitute
                                WHERE IsActive   ='Y'
                                AND Substitute_ID=" + AD_User_ID + @" 
                                AND (validfrom  <=sysdate)
                                AND (sysdate    <=validto )
                                ))
                              AND r.responsibletype !='H' AND r.responsibletype !='C'
                              )) ";

            // if (AD_Window_ID > 0 || (!string.IsNullOrEmpty(searchText) && searchText.Length > 0))
            if (whereClause.Length > 7)
            {
                // Applied Role access on workflow Activities
                sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "a", true, true) + @" )  MyTable ";

                sql += fromClause;
                sql += whereClause;
                sql += "  ORDER BY myTable.Priority DESC, myTable.Created DESC";

                //LEFT OUTER Join 
                //                           AD_Wf_Activity Wf On Abc.AD_Wf_Activity_Id=Wf.AD_Wf_Activity_Id
                //                           LEFT OUTER Join AD_WF_Node Wfn On Wfn.AD_WF_Node_Id=Wf.AD_WF_Node_Id
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
            //int AD_User_ID = Envs.GetContext().GetAD_User_ID();
            try
            {
                //SqlParameter[] param = new SqlParameter[2];
                //param[0] = new SqlParameter("@clientid", AD_Client_ID);
                //param[1] = new SqlParameter("@userid", AD_User_ID);
                //VLogger.Get().Log(Level.SEVERE, sql);
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

                    itm.AD_Table_ID = Util.GetValueOfInt(dr["AD_Table_ID"]);
                    itm.AD_User_ID = Util.GetValueOfInt(dr["AD_User_ID"]);
                    itm.AD_WF_Activity_ID = Util.GetValueOfInt(dr["AD_WF_Activity_ID"]);

                    itm.AD_Node_ID = Util.GetValueOfInt(dr["AD_WF_Node_ID"]);
                    itm.AD_WF_Process_ID = Util.GetValueOfInt(dr["AD_WF_Process_ID"]);
                    itm.AD_WF_Responsible_ID = Util.GetValueOfInt(dr["AD_WF_Responsible_ID"]);
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
                    MWFActivity act = new MWFActivity(ctx, itm.AD_WF_Activity_ID, null);
                    itm.NodeName = act.GetNodeName();
                    itm.Summary = act.GetSummary();
                    itm.Description = act.GetNodeDescription();
                    itm.Help = act.GetNodeHelp();
                    itm.History = act.GetHistoryHTML();
                    itm.Priority = Util.GetValueOfInt(dr["Priority"]);
                    itm.AD_Window_ID = Util.GetValueOfInt(dr["AD_Window_ID"]);
                    lstInfo.Add(itm);

                }

                WFInfo info = new WFInfo();
                info.LstInfo = lstInfo;
                //return lstInfo;

                if (refresh)
                {
                    sql = "";
                    //if (AD_Window_ID > 0 || (!string.IsNullOrEmpty(searchText) && searchText.Length > 0))
                    //if (whereClause.Length > 7)
                    //{
                    sql = @"SELECT COUNT(*) FROM (";
                    //}
                    sql += @" SELECT a.* 
                            FROM AD_WF_Activity a
                            WHERE a.Processed  ='N'
                            AND a.WFState      ='OS'
                            AND a.AD_Client_ID =" + ctx.GetAD_Client_ID() + @"
                            AND ((a.AD_User_ID=" + ctx.GetAD_User_ID() + @"
                            OR a.AD_User_ID   IN
                              (SELECT AD_User_ID
                              FROM AD_User_Substitute
                              WHERE IsActive   ='Y'
                              AND Substitute_ID=" + ctx.GetAD_User_ID() + @"
                              AND (validfrom  <=sysdate)
                              AND (sysdate    <=validto )
                              ))
                            OR EXISTS
                              (SELECT *
                              FROM AD_WF_Responsible r
                              WHERE a.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID
                              AND COALESCE(r.AD_User_ID,0)=0
                              AND (a.AD_User_ID           =" + ctx.GetAD_User_ID() + @"
                              OR a.AD_User_ID            IS NULL
                              OR a.AD_User_ID            IN
                                (SELECT AD_User_ID
                                FROM AD_User_Substitute
                                WHERE IsActive   ='Y'
                                AND Substitute_ID=" + ctx.GetAD_User_ID() + @"
                                AND (validfrom  <=sysdate)
                                AND (sysdate    <=validto )
                                ))
                              )
                            OR EXISTS
                              (SELECT *
                              FROM AD_WF_Responsible r
                              WHERE a.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID
                              AND (r.AD_User_ID           =" + ctx.GetAD_User_ID() + @"
                              OR a.AD_User_ID            IN
                                (SELECT AD_User_ID
                                FROM AD_User_Substitute
                                WHERE IsActive   ='Y'
                                AND Substitute_ID=" + ctx.GetAD_User_ID() + @"
                                AND (validfrom  <=sysdate)
                                AND (sysdate    <=validto )
                                ))
                              )
                            OR EXISTS
                              (SELECT *
                              FROM AD_WF_Responsible r
                              INNER JOIN AD_User_Roles ur
                              ON (r.AD_Role_ID            =ur.AD_Role_ID)
                              WHERE a.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID
                              AND (ur.AD_User_ID          =" + ctx.GetAD_User_ID() + @"
                              OR a.AD_User_ID            IN
                                (SELECT AD_User_ID
                                FROM AD_User_Substitute
                                WHERE IsActive   ='Y'
                                AND Substitute_ID=" + ctx.GetAD_User_ID() + @"
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
                        //                           AD_Wf_Activity Wf On Abc.AD_Wf_Activity_Id=Wf.AD_Wf_Activity_Id
                        //                           LEFT OUTER Join AD_WF_Node Wfn On Wfn.AD_WF_Node_Id=Wf.AD_WF_Node_Id
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
        /// <param name="AD_Window_ID"></param>
        private void GetWhereClause(Ctx ctx, MTable table, string searchText, int AD_Window_ID, int AD_Node_ID)
        {
            if (whereClause.Length > 7 && searchText.Length > 0)
            {
                whereClause += " OR ";
            }
            else
            {
                whereClause = " WHERE ";
                if (AD_Window_ID > 0)
                {
                    whereClause += " MyTable.AD_Window_ID=" + AD_Window_ID + " AND MyTable.AD_WF_NODE_ID=" + AD_Node_ID;
                }
            }

            if (searchText.Length > 0 && table != null)
            {
                StringBuilder sb = new StringBuilder();
                if (AD_Window_ID > 0)
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
                    GetFromClause(ctx, "AD_User", table.GetTableName(), localSynonmus, "SalesRep_ID");
                }
                //sr = (int?)_po.Get_Value(index);
                else
                {
                    index = _po.Get_ColumnIndex("AD_User_ID");
                    //if (index != -1)
                    //    sr = (int?)_po.Get_Value(index);
                }
                if (index != -1)
                {
                    //MUser user = MUser.Get(ctx, sr.Value);
                    //if (user != null)
                    //    sb.Append(user.GetName()).Append(" ");

                    SynonymNext();

                    GetFromClause(ctx, "AD_User", table.GetTableName(), localSynonmus, "AD_User_ID");

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
                if (AD_Window_ID > 0)
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
                    info.ColID = col.GetAD_Column_ID();
                    info.ColReference = col.GetAD_Reference_ID();
                    info.ColReferenceValue = col.GetAD_Reference_Value_ID();
                    info.ColName = col.GetColumnName();
                }
                else if (MWFNode.ACTION_UserWindow.Equals(node.GetAction()))
                {
                    info.AD_Window_ID = node.GetAD_Window_ID();
                    MWFActivity activity = new MWFActivity(ctx, activityID, null);
                    info.KeyCol = activity.GetPO().Get_TableName() + "_ID";
                }
                else if (MWFNode.ACTION_UserForm.Equals(node.GetAction()))
                {
                    info.AD_Form_ID = node.GetAD_Form_ID();
                }



                string sql = @"SELECT node.ad_wf_node_ID,
                                  node.Name AS NodeName,
                                  usr.Name AS UserName,
                                  wfea.wfstate,
                                  wfea.TextMsg
                              FROM ad_wf_eventaudit wfea
                                INNER JOIN AD_WF_Node node
                                ON (node.AD_Wf_node_ID=wfea.AD_Wf_Node_id)
                                INNER JOIN AD_User usr
                                ON (usr.AD_User_ID         =wfea.ad_User_ID)
                              WHERE wfea.AD_WF_Process_ID=" + wfProcessID + @"
                              Order By wfea.ad_wf_eventaudit_id desc";
                DataSet ds = DB.ExecuteDataset(sql);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    List<NodeInfo> nodeInfo = new List<NodeInfo>();
                    List<int> nodes = new List<int>();
                    NodeInfo ni = null;
                    NodeHistory nh = null;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (!nodes.Contains(Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_WF_Node_ID"])))
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
                            nodes.Add(Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_WF_Node_ID"]));
                            nodeInfo.Add(ni);
                        }
                        else
                        {
                            int index = nodes.IndexOf(Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_WF_Node_ID"]));
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
        /// <param name="forward">AD_User_ID to whom activity is forwarded</param>
        /// <param name="answer"> Message</param>
        /// <param name="ctx">Context</param>
        /// <param name="AD_Window_ID">AD_Window_ID</param>
        /// <returns></returns>
        public string ApproveIt(int nodeID, string activityID, string textMsg, object forward, object answer, Ctx ctx, int AD_Window_ID)
        {
            if (!string.IsNullOrEmpty(activityID) && activityID.Length > 0)
            {
                List<int> activitiesID = activityID.Split(',').Select(int.Parse).ToList();
                activitiesID.Sort();
                string chkUserTxt = "";
                for (int i = 0; i < activitiesID.Count; i++)
                {
                    chkUserTxt = "";
                    MWFActivity activity = new MWFActivity(ctx, Convert.ToInt32(activitiesID[i]), null);
                    MWFNode node = activity.GetNode();
                    // Change done to set zoom window from Node
                    if (node.GetZoomWindow_ID() > 0)
                        activity.SetAD_Window_ID(node.GetZoomWindow_ID());
                    int approvalLevel = node.GetApprovalLeval();
                    int AD_User_ID = ctx.GetAD_User_ID();
                    MColumn column = node.GetColumn();

                    if (forward != null) // Prefer Forward 
                    {
                        int fw = int.Parse(forward.ToString());
                        if (fw == AD_User_ID || fw == 0)
                        {
                            return "";
                        }
                        //chkUserTxt = CheckUser(fw);
                        //if (chkUserTxt != "")
                        //    return chkUserTxt;
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
                        int dt = column.GetAD_Reference_ID();
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
                            string res = SetUserChoice(AD_User_ID, value, dt, textMsg, activity, node, AD_Window_ID);
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

                        //    SetUserChoice(AD_User_ID, attrib.GetAttributeSetInstance().ToString(), 0, textMsg, activity, node);
                        //}

                        else if (forward == null && node.IsMultiApproval() && approvalLevel > 0 && answer.ToString().Equals("Y"))
                        {

                            int eventCount = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(WFE.AD_WF_EventAudit_ID) FROM AD_WF_EventAudit WFE
                                                                                INNER JOIN AD_WF_Process WFP ON (WFP.AD_WF_Process_ID=WFE.AD_WF_Process_ID)
                                                                                INNER JOIN AD_WF_Activity WFA ON (WFA.AD_WF_Process_ID=WFP.AD_WF_Process_ID)
                                                                                WHERE WFE.AD_WF_Node_ID=" + node.GetAD_WF_Node_ID() + " AND WFA.AD_WF_Activity_ID=" + activity.GetAD_WF_Activity_ID()));
                            if (eventCount < approvalLevel) //Forward Activity
                            {
                                int AD_WF_Responsible_ID = 0;
                                if (node.GetAD_WF_Responsible_ID() > 0)
                                    AD_WF_Responsible_ID = node.GetAD_WF_Responsible_ID();
                                else if (node.GetWorkflow().GetAD_WF_Responsible_ID() > 0)
                                    AD_WF_Responsible_ID = node.GetWorkflow().GetAD_WF_Responsible_ID();
                                MWFResponsible resp = new MWFResponsible(ctx, AD_WF_Responsible_ID, null);
                                int superVisiorID = 0;
                                bool setRespOrg = false;
                                int parentOrg_ID = -1;
                                if (resp.IsOrganization())
                                {
                                    parentOrg_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Parent_Org_Id FROM ad_OrgInfo WHERE AD_Org_ID = " + activity.GetResponsibleOrg_ID()));
                                    superVisiorID = (new MOrgInfo(ctx, parentOrg_ID, null).GetSupervisor_ID());
                                    if (superVisiorID > 0)
                                        setRespOrg = true;
                                }
                                else
                                    superVisiorID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Supervisor_ID FROM AD_User WHERE IsActive='Y' AND AD_User_ID=" + activity.GetAD_User_ID()));
                                //chkUserTxt = CheckUser(superVisiorID);
                                //if (chkUserTxt != "")
                                //    return chkUserTxt;
                                if (setRespOrg)
                                    activity.SetResponsibleOrg_ID(parentOrg_ID);
                                if (superVisiorID == 0)//Approve
                                {
                                    string res = SetUserChoice(AD_User_ID, value, dt, textMsg, activity, node, AD_Window_ID);
                                    if (res != "OK")
                                    {
                                        return res;
                                    }
                                }
                                else //forward
                                {
                                    if (!activity.ForwardTo(superVisiorID, textMsg, true))
                                    {
                                        return "CannotForward";
                                    }
                                }
                            }
                            else //Approve
                            {
                                string res = SetUserChoice(AD_User_ID, value, dt, textMsg, activity, node, AD_Window_ID);
                                if (res != "OK")
                                {
                                    return res;
                                }
                            }
                        }
                        else
                        {

                            string res = SetUserChoice(AD_User_ID, value, dt, textMsg, activity, node, AD_Window_ID);
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
                        //    activity.SetUserConfirmation(AD_User_ID, textMsg);
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
                        activity.SetUserConfirmation(AD_User_ID, textMsg);

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
                bool chkActiveUser = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsActive FROM AD_User WHERE AD_User_ID = " + User_ID)).ToLower() == "y";
                if (!chkActiveUser)
                    return Msg.GetMsg(ctx, "ApproverNotActive"); ;
            }
            return "";
        }

        private string SetUserChoice(int AD_User_ID, string value, int dt, string textMsg, MWFActivity _activity, MWFNode _node, int AD_Window_ID)
        {
            try
            {
                // Passed AD_Window_ID to process to set windowID at all levels of current activity
                _activity.SetUserChoice(AD_User_ID, value, dt, textMsg, AD_Window_ID);
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
                                                            WFP.AD_Table_ID,
                                                            WFP.Record_ID
                                                            FROM AD_WF_Process WFP
                                                            INNER JOIN AD_WF_Activity WFA
                                                            ON (WFA.AD_WF_Process_ID=WFP.AD_WF_Process_ID)
                                                            WHERE WFA.AD_WF_Activity_ID=" + activityID, null);
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
            MTable table = MTable.Get(ctx, Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Table_ID FROM AD_Table WHERE TableName='" + tableName + "'")));
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
                sql = @"SELECT DISTINCT AD_Window.AD_window_ID,  AD_Window.DisplayName  || ' (' || AD_WF_Node.Name || ')' As Name,AD_WF_Node.AD_WF_Node_ID FROM AD_WF_Activity AD_WF_Activity
                            INNER JOIN AD_Window AD_Window ON (AD_WF_Activity.AD_Window_ID = AD_Window.AD_Window_ID)
                            INNER JOIN AD_WF_Node AD_WF_Node ON (AD_WF_Node.AD_WF_Node_ID=AD_Wf_Activity.AD_WF_Node_ID)
                            WHERE AD_Window.IsActive ='Y'  AND AD_Wf_Activity.Processed = 'N'  AND AD_Wf_Activity.WFState      ='OS' ";
                sql += " AND AD_Wf_Activity.AD_Client_ID =" + ctx.GetAD_Client_ID() + @" 
                            AND  ((AD_Wf_Activity.AD_User_ID=" + ctx.GetAD_User_ID() + @" 
                            OR AD_Wf_Activity.AD_User_ID   IN
                              (SELECT AD_User_ID
                              FROM AD_User_Substitute
                              WHERE IsActive   ='Y'
                              AND Substitute_ID=" + ctx.GetAD_User_ID() + @" 
                              AND (validfrom  <=sysdate)
                              AND (sysdate    <=validto )
                              ))
                                OR EXISTS
                                  (SELECT *
                                  FROM AD_WF_Responsible r
                                  WHERE AD_WF_Activity.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID
                                  AND COALESCE(r.AD_User_ID,0)=0
                                  AND (AD_WF_Activity.AD_User_ID           =" + ctx.GetAD_User_ID() + @"
                                  OR AD_WF_Activity.AD_User_ID            IS NULL
                                  OR AD_WF_Activity.AD_User_ID            IN
                                    (SELECT AD_User_ID
                                    FROM AD_User_Substitute
                                    WHERE IsActive   ='Y'
                                    AND Substitute_ID=" + ctx.GetAD_User_ID() + @"
                                    AND (validfrom  <=sysdate)
                                    AND (sysdate    <=validto )
                                    ))
                                  )
                                OR EXISTS
                                  (SELECT *
                                  FROM AD_WF_Responsible r
                                  WHERE AD_WF_Activity.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID
                                  AND (r.AD_User_ID           =" + ctx.GetAD_User_ID() + @"
                                  OR AD_WF_Activity.AD_User_ID            IN
                                    (SELECT AD_User_ID
                                    FROM AD_User_Substitute
                                    WHERE IsActive   ='Y'
                                    AND Substitute_ID=" + ctx.GetAD_User_ID() + @"
                                    AND (validfrom  <=sysdate)
                                    AND (sysdate    <=validto )
                                    ))
                                  )
                                OR EXISTS
                                  (SELECT *
                                  FROM AD_WF_Responsible r
                                  INNER JOIN AD_User_Roles ur
                                  ON (r.AD_Role_ID            =ur.AD_Role_ID)
                                  WHERE AD_WF_Activity.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID
                                  AND (ur.AD_User_ID          =" + ctx.GetAD_User_ID() + @"
                                  OR AD_WF_Activity.AD_User_ID            IN
                                    (SELECT AD_User_ID
                                    FROM AD_User_Substitute
                                    WHERE IsActive   ='Y'
                                    AND Substitute_ID=" + ctx.GetAD_User_ID() + @"
                                    AND (validfrom  <=sysdate)
                                    AND (sysdate    <=validto )
                                    ))
                                  And R.Responsibletype !='H'
                                  ))";
            }
            else
            {
                sql = @"SELECT DISTINCT AD_Window.AD_Window_Id,
                        AD_Window_Trl.Name || ' (' || AD_WF_Node.Name || ')' As Name,AD_WF_Node.AD_WF_Node_ID
                        FROM AD_Wf_Activity AD_Wf_Activity
                        INNER JOIN AD_Window AD_Window
                        ON (AD_Wf_Activity.AD_Window_Id = AD_Window.AD_Window_Id)
                        INNER JOIN AD_window_Trl AD_window_Trl
                        ON (AD_window_Trl.AD_Window_ID=AD_window.AD_window_ID)
                        INNER JOIN AD_WF_Node AD_WF_Node ON (AD_WF_Node.AD_WF_Node_ID=AD_Wf_Activity.AD_WF_Node_ID)
                        WHERE AD_Window.IsActive     ='Y'
                        AND AD_Wf_Activity.Processed = 'N'
                        AND AD_Wf_Activity.WFState   ='OS' AND AD_Language='" + Env.GetAD_Language(ctx) + "'";
                sql += " AND AD_Wf_Activity.AD_Client_ID =" + ctx.GetAD_Client_ID() + @" 
                            AND  ((AD_Wf_Activity.AD_User_ID=" + ctx.GetAD_User_ID() + @" 
                            OR AD_Wf_Activity.AD_User_ID   IN
                              (SELECT AD_User_ID
                              FROM AD_User_Substitute
                              WHERE IsActive   ='Y'
                              AND Substitute_ID=" + ctx.GetAD_User_ID() + @" 
                              AND (validfrom  <=sysdate)
                              AND (sysdate    <=validto )
                              ))
                                  OR EXISTS
                                  (SELECT *
                                  FROM AD_WF_Responsible r
                                  WHERE AD_WF_Activity.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID
                                  AND COALESCE(r.AD_User_ID,0)=0
                                  AND (AD_WF_Activity.AD_User_ID           =" + ctx.GetAD_User_ID() + @"
                                  OR AD_WF_Activity.AD_User_ID            IS NULL
                                  OR AD_WF_Activity.AD_User_ID            IN
                                    (SELECT AD_User_ID
                                    FROM AD_User_Substitute
                                    WHERE IsActive   ='Y'
                                    AND Substitute_ID=" + ctx.GetAD_User_ID() + @"
                                    AND (validfrom  <=sysdate)
                                    AND (sysdate    <=validto )
                                    ))
                                  )
                                OR EXISTS
                                  (SELECT *
                                  FROM AD_WF_Responsible r
                                  WHERE AD_WF_Activity.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID
                                  AND (r.AD_User_ID           =" + ctx.GetAD_User_ID() + @"
                                  OR AD_WF_Activity.AD_User_ID            IN
                                    (SELECT AD_User_ID
                                    FROM AD_User_Substitute
                                    WHERE IsActive   ='Y'
                                    AND Substitute_ID=" + ctx.GetAD_User_ID() + @"
                                    AND (validfrom  <=sysdate)
                                    AND (sysdate    <=validto )
                                    ))
                                  )
                                OR EXISTS
                                  (SELECT *
                                  FROM AD_WF_Responsible r
                                  INNER JOIN AD_User_Roles ur
                                  ON (r.AD_Role_ID            =ur.AD_Role_ID)
                                  WHERE AD_WF_Activity.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID
                                  AND (ur.AD_User_ID          =" + ctx.GetAD_User_ID() + @"
                                  OR AD_WF_Activity.AD_User_ID            IN
                                    (SELECT AD_User_ID
                                    FROM AD_User_Substitute
                                    WHERE IsActive   ='Y'
                                    AND Substitute_ID=" + ctx.GetAD_User_ID() + @"
                                    AND (validfrom  <=sysdate)
                                    AND (sysdate    <=validto )
                                    ))
                                  And R.Responsibletype !='H'
                                  ))";
            }



            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "AD_WF_Activity", true, false);
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    WorkflowWindowList wwl = new WorkflowWindowList();
                    wwl.WindowName = Convert.ToString(ds.Tables[0].Rows[i]["Name"]);
                    wwl.AD_Window_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Window_ID"]);
                    wwl.AD_Node_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_WF_Node_ID"]);
                    list.Add(wwl);
                }
            }
            return list;
        }
    }

    public class WorkflowWindowList
    {
        public int AD_Window_ID { get; set; }
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

        public int AD_Table_ID
        {
            get;
            set;
        }
        public int AD_User_ID
        {
            get;
            set;
        }
        public int AD_WF_Activity_ID
        {
            get;
            set;
        }
        public int AD_Node_ID
        {
            get;
            set;
        }
        public int AD_WF_Process_ID
        {
            get;
            set;
        }
        public int AD_WF_Responsible_ID
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

        public int AD_Window_ID
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
        public int AD_Window_ID
        {
            get;
            set;
        }
        public int AD_Form_ID
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