/********************************************************
 * Module Name    : Show Tree(menu and favorite) 
 * Purpose        : used to Get Tree info (eg menu , org etc) 
 * Class Used     : VTreeNode.cs, GlobalVariable.cs
 * Created By     : Harwinder 
 * Date           : 24 nov 2008
**********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Model;

namespace VAdvantage.Classes
{
    public class VTree
    {
        /// <summary>
        /// Define Tree Types
        /// </summary>
        public enum TreeType
        {
            MM,
            BM,
            OO
        };
     #region declaration 
       
        private  Context ctx = Utility.Env.GetContext(); // Get Context object
        private int _VAF_TreeInfo_ID;
        private bool _editable;
        private bool _clientTree;
        private TreeType treeType;
        private DataTable dt; //temp data strorage
        private VTreeNode root = null; //root node
        private List<VTreeNode> mnuNodes = new List<VTreeNode>(); //menu nodes
        private List<VTreeNode> barNodes = new List<VTreeNode>(); //favourite nodes
        int maxLevel = 0;
        
        #endregion
        
       /// <summary>
       /// Constructror initilize variable
       /// </summary>
       /// <param name="VAF_TreeInfo_ID">tree id</param>
       /// <param name="_editable">_editable or not</param>
       /// <param name="_clientTree"></param>
       /// <param name="menuType">tree type</param>
        public VTree(int VAF_TreeInfo_ID, bool editable, bool clientTree,TreeType menuType)
        {
            _VAF_TreeInfo_ID = VAF_TreeInfo_ID;
            _editable = editable;
             treeType = menuType;
        }

        /// <summary>
        /// Initilize tree loading process
        /// </summary>
        /// <param name="VAF_TreeInfo_ID">tree key id</param>
        public void InitTree(int VAF_TreeInfo_ID)
        {
            //Get Root node info
            string str = "Select name,description from VAF_TreeInfo where VAF_TreeInfo_id =" + VAF_TreeInfo_ID;
            IDataReader drRoot = DataBase.DB.ExecuteReader(str);
            string strRootName, strDesc;
            drRoot.Read();
            strRootName = drRoot[0].ToString();
            strDesc = drRoot[1].ToString();
            drRoot.Close();
            //
            StringBuilder sql = new StringBuilder("SELECT ")
            .Append(" tn.Node_ID,tn.Parent_ID,tn.SeqNo,tb.IsActive ")
            .Append(" FROM " + GetTableName() + "  tn")
            .Append(" LEFT OUTER JOIN VAF_TreeInfoBar tb ON (tn.VAF_TreeInfo_ID=tb.VAF_TreeInfo_ID")
            .Append(" AND tn.Node_ID=tb.Node_ID AND tb.VAF_UserContact_ID=" + ctx.GetVAF_UserContact_ID() + ") ")	//	#1
            .Append(" WHERE tn.VAF_TreeInfo_ID=" + VAF_TreeInfo_ID);
            
            if (!_editable)
            {
                sql.Append(" AND tn.IsActive='Y' ");
            }
                sql.Append(" ORDER BY COALESCE(tn.Parent_ID, -1), tn.SeqNo");
            
            //Get All Nodes Info (eg. name description etc);
            GetNodeDetail();

            IDataReader dr = DataBase.DB.ExecuteReader(sql.ToString());
            // Create Root node
            root = new VTreeNode(0, 0, strRootName, strDesc, 0, false, "O", false);

            while (dr.Read())
            {
                int Node_ID = Utility.Util.GetValueOfInt(dr[0].ToString());
                int Parent_ID = 0;
                try
                {
                    Parent_ID = Utility.Util.GetValueOfInt(dr[1].ToString());
                }
                catch
                {
                    Parent_ID = 0;
                }
                int seqNo = Utility.Util.GetValueOfInt(dr[2].ToString());
                //Node in Favorite menu or not
                bool onBar = (dr[3] != null) ? ((dr[3].ToString() == "Y") ? true : false) : false;
                if (Node_ID == 0 && Parent_ID == 0)
                { }
                else
                {
                    AddToTree(Node_ID, Parent_ID, seqNo, onBar);	//	get node detail and add to root node
                }
            }
            dr.Close();

            // Get From list
                for (int i = 0; i < mnuNodes.Count; i++)
                {
                    VTreeNode node = mnuNodes[i];
                    VTreeNode parent = FindTreeNode(root.Nodes, node.Parent_ID.ToString());
                    if (parent != null)
                    {
                        parent.Nodes.Add(node);
                        if (parent.FirstNode.Level > maxLevel)
                        {
                            maxLevel = parent.FirstNode.Level;
                        }
                        //CheckList(node);
                        mnuNodes.RemoveAt(i);
                        i = -1;		//	start again with i=0
                    }
                }


            //	Nodes w/o parent
                if (mnuNodes.Count != 0)
                {
                    //log.severe ("Nodes w/o parent - adding to root - " + m_buffer);
                    for (int i = 0; i < mnuNodes.Count; i++)
                    {
                        VTreeNode node = mnuNodes[i];
                        root.Nodes.Add(node);
                        mnuNodes.RemoveAt(i);
                        i = -1;
                    }
                }

                //remove summary node, not having  child node
            if (!_editable)
            {
                TrimTree(root);
            } 
        }

        /// <summary>
        /// remove summery node having no children
        /// </summary>
        /// <param name="tn">tree node</param>
        private void TrimTree(TreeNode tn)
        {
            for (int i = 0; i < tn.Nodes.Count; i++)
            {
                VTreeNode node = (VTreeNode)tn.Nodes[i];
                if (node.IsSummary && node.Nodes.Count < 1)
                {
                    root.Nodes.Remove(node);
                   
                    if (barNodes.Contains(node))
                    {
                        barNodes.Remove(node);

                    }
                    i=-1;
                }
                else
                {
                    if (node.Nodes.Count > 0)
                    {
                        TrimTree(node);
                       
                    }
                }
            }
        }

        /// <summary>
        /// Add Node to root node  and tree bar(favorite) list 
        /// </summary>
        /// <param name="Node_ID">node key id</param>
        /// <param name="Parent_ID">parent node id </param>
        /// <param name="seqNo"> seq no</param>
        /// <param name="onBar">node is in favorite tree or not</param>
        private void AddToTree(int Node_ID, int Parent_ID, int seqNo, bool onBar)
        {
            //  Create new Node
            VTreeNode child = GetNodeDetail(Node_ID, Parent_ID, seqNo, onBar);
            if (child == null)
                return;

            //  Add to Tree
            VTreeNode parent = null;
            if (onBar == true) // if in favorite (short -cut)
            {
                VTreeNode childBar = GetNodeDetail(Node_ID, Parent_ID, seqNo, onBar);
                barNodes.Insert(barNodes.Count, childBar);
            }

            if (root != null)
            {
                if (child.Parent_ID == 0)
                {
                    root.Nodes.Add(child);
                }
                else
                {
                    mnuNodes.Add(child);
                }   //  addToTree
            }
        }


        /// <summary>
        /// set node properties
        /// </summary>
        /// <param name="Node_ID">node key id</param>
        /// <param name="Parent_ID">parent node id</param>
        /// <param name="seqNo">seq no</param>
        /// <param name="onBar">node in favorite tree</param>
        /// <returns>VTreeNode </returns>

        private VTreeNode GetNodeDetail(int Node_ID, int Parent_ID, int seqNo, bool onBar)
        {
            int VAF_Screen_ID = 0;
            int VAF_Job_ID = 0;
            int VAF_Page_ID = 0;
            int VAF_Workflow_ID = 0;
            int VAF_Task_ID = 0;
            int AD_Workbench_ID = 0;

            VTreeNode retValue = null;
            
            string strColumnName = "";
            if (treeType == TreeType.MM)
            {
                strColumnName = "VAF_MenuConfig_Id";
            }
            else
            {
                strColumnName = "vaf_org_ID";
            }
            
            // Serch For Node details
            for (int i=0;i<dt.Rows.Count;i++)
            {
               int node1 = Utility.Util.GetValueOfInt(dt.Rows[i][0].ToString());
               if (Node_ID != node1)	//	search for correct one
               {
                   continue;
               }
             DataRow dr = dt.Rows[i];

                int node = Utility.Util.GetValueOfInt(dr[0].ToString());
                int index = 1;

                string name = dr[index++].ToString();
                string description = dr[index++].ToString();
                bool isSummary = "Y".Equals(dr[index++].ToString());
                string actionColor = "";
                //	Menu only
                //if (getTreeType().equals(TREETYPE_Menu) && !isSummary)
                if (!isSummary)
                {

                    bool? blnAccess = null;
                    if (treeType == TreeType.MM)
                    {
                        actionColor = dr[index++].ToString();
                        VAF_Screen_ID = (dr[index].ToString().Trim() == "") ? 0 : Utility.Util.GetValueOfInt(dr[index].ToString());
                        index++;
                        VAF_Job_ID = (dr[index].ToString().Trim() == "") ? 0 : Utility.Util.GetValueOfInt(dr[index].ToString());
                        index++;
                        VAF_Page_ID = (dr[index].ToString().Trim() == "") ? 0 : Utility.Util.GetValueOfInt(dr[index].ToString());
                        index++;
                        VAF_Workflow_ID = (dr[index].ToString().Trim() == "") ? 0 : Utility.Util.GetValueOfInt(dr[index].ToString());
                        index++;
                        VAF_Task_ID = (dr[index].ToString().Trim() == "") ? 0 : Utility.Util.GetValueOfInt(dr[index].ToString());
                        index++;
                        AD_Workbench_ID = (dr[index].ToString().Trim() == "") ? 0 : Utility.Util.GetValueOfInt(dr[index].ToString());
                        index++;
                        MRole role = MRole.GetDefault(ctx);
                        if (VTreeNode.ACTION_WINDOW.Equals(actionColor))
                        {
                            blnAccess = role.GetWindowAccess(VAF_Screen_ID);
                        }
                        else if (VTreeNode.ACTION_PROCESS.Equals(actionColor)
                                || VTreeNode.ACTION_REPORT.Equals(actionColor))
                        {
                            blnAccess = role.GetProcessAccess(VAF_Job_ID);
                        }
                        else if (VTreeNode.ACTION_FORM.Equals(actionColor))
                        {
                            blnAccess = role.GetFormAccess(VAF_Page_ID);
                        }
                        else if (VTreeNode.ACTION_WORKFLOW.Equals(actionColor))
                        {
                            blnAccess = role.GetWorkflowAccess(VAF_Workflow_ID);
                        }
                        else if (VTreeNode.ACTION_TASK.Equals(actionColor))
                        {
                            blnAccess = role.GetTaskAccess(VAF_Task_ID);
                        }
                    }
                    if (blnAccess != null || _editable)		//	rw or ro for Role 
                    {
                        retValue = new VTreeNode(Node_ID, seqNo,
                            name, description, Parent_ID, isSummary,
                            actionColor, onBar);	//	menu has no color
                    }
                }
                else
                {
                    retValue = new VTreeNode(Node_ID, seqNo,
                            name, description, Parent_ID, isSummary,
                            actionColor, onBar);
                }
                break;

            }

            if (retValue != null && treeType == TreeType.MM)
            {
                // set VTreeNode ID's
                retValue.VAF_Screen_ID = VAF_Screen_ID;
                retValue.VAF_Job_ID = VAF_Job_ID;
                retValue.VAF_Page_ID = VAF_Page_ID;
                retValue.VAF_Workflow_ID = VAF_Workflow_ID;
                retValue.VAF_Task_ID = VAF_Task_ID;
                retValue.AD_Workbench_ID = AD_Workbench_ID;
            }
            return retValue;
        }

        /// <summary>
        /// Seach for tree node
        /// </summary>
        /// <param name="tNodes">TreeNode Collection</param>
        /// <param name="strFind">node name</param>
        /// <returns>VTreeNode</returns>
        private VTreeNode FindTreeNode(TreeNodeCollection tNodes, string strFind)
        {
            foreach (TreeNode tn in tNodes)
            {
                if (tn.Name == strFind)
                {
                    return (VTreeNode)tn;
                }
                //if has Child Nodes
                if (tn.Nodes.Count > 0)
                {
                    //Call recursive
                    TreeNode match = FindTreeNode(tn.Nodes, strFind);
                    if (match != null)
                    {
                        return (VTreeNode)match;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// get menu items details and store temp in memory(datatable)
        /// </summary>
        protected void GetNodeDetail()
        {
            string strSql = "";

             if (treeType == TreeType.OO)
            {
                strSql = "SELECT o.VAF_ORG_ID,o.Name,o.Description,o.ISSUMMARY,o.VAF_CLIENT_ID,c.NAME as Tenant,o.VALUE as SearchKey,"
                    + "(case o.ISACTIVE when 'Y' then 'True' else 'False' end) as Active,"
                    + "(case o.ISSUMMARY when 'Y' then 'True' else 'False' end) as Summary "
                    + "FROM VAF_Role r, VAF_Client c"
                    + " INNER JOIN VAF_Org o ON (c.VAF_Client_ID=o.VAF_Client_ID) "
                    + "WHERE "
                    //r.VAF_Role_ID=" + DataBase.GlobalVariable.VAF_ROLE_ID
                    //+ " AND 
                    +"(c.VAF_Client_ID=" + ctx.GetVAF_Client_ID() + " OR c.vaf_client_id=0)"
                    //+ " AND (r.IsAccessAllOrgs='Y' "
                    //    + "OR (r.IsUseUserOrgAccess='N' AND o.VAF_Org_ID IN (SELECT VAF_Org_ID FROM VAF_Role_OrgRights ra "
                    //        + "WHERE ra.VAF_Role_ID=r.VAF_Role_ID AND ra.IsActive='Y')) "
                    //    + "OR (r.IsUseUserOrgAccess='Y' AND o.VAF_Org_ID IN (SELECT VAF_Org_ID FROM VAF_UserContact_OrgRights ua "
                    //        + "WHERE ua.VAF_UserContact_ID=" + ctx.GetVAF_UserContact_ID + " AND ua.IsActive='Y'))"
                    //    + ") "
                    + "ORDER BY o.VAF_ORG_ID";
            }
             else if (treeType == TreeType.MM)
             {
                 //Check Base langugae
                 bool isBase = Utility.Env.IsBaseLanguage(ctx, "VAF_MENUCONFIG");
                 if (isBase)
                 {
                     strSql = @"SELECT m.VAF_MenuConfig_ID, m.Name,m.Description,m.IsSummary,m.Action, m.VAF_Screen_ID,
                       m.VAF_Job_ID, m.VAF_Page_ID, m.VAF_Workflow_ID, m.VAF_Task_ID, m.AD_Workbench_ID 
                       FROM VAF_MenuConfig m WHERE";
                 }
                 else
                 {
                     strSql = @"SELECT m.VAF_MenuConfig_ID, t.Name,t.Description,m.IsSummary,m.Action, m.VAF_Screen_ID,
                        m.VAF_Job_ID, m.VAF_Page_ID, m.VAF_Workflow_ID, m.VAF_Task_ID, m.AD_Workbench_ID 
                      FROM VAF_MenuConfig m, VAF_MenuConfig_TL t WHERE m.VAF_MenuConfig_ID=t.VAF_MenuConfig_ID AND t.VAF_Language='" + Utility.Env.GetVAF_Language(ctx) + "' And";
                 }
                 if (!_editable)
                 {
                     strSql += " m.IsActive='Y' AND ";
                 }
                 strSql += @" 
                       (m.VAF_Screen_ID IS NULL OR EXISTS (SELECT * FROM VAF_Screen w WHERE m.VAF_Screen_ID=w.VAF_Screen_ID AND w.IsBetaFunctionality='N'))
                       AND 
                       (m.VAF_Job_ID IS NULL OR EXISTS (SELECT * FROM VAF_Job p WHERE m.VAF_Job_ID=p.VAF_Job_ID AND p.IsBetaFunctionality='N')) 
                       AND 
                       (m.VAF_Page_ID IS NULL OR EXISTS (SELECT * FROM VAF_Page f WHERE m.VAF_Page_ID=f.VAF_Page_ID AND f.IsBetaFunctionality='N')) ";
                 if (!_editable)
                 {
                     strSql += @" AND 
                       (m.VAF_Page_ID IS NULL OR EXISTS (SELECT * FROM VAF_Page f WHERE m.VAF_Page_ID=f.VAF_Page_ID AND f.Classname IS NOT NULL)) ";
                 }
                 if (!_editable)
                 {
                     strSql += @" And  m.VAF_Client_ID in ( " + GetClientId() + ")  AND m.VAF_Org_ID in( " + GetOrgID() + ")";
                 }
             }
            IDataReader drTree = DataBase.DB.ExecuteReader(strSql);
            dt = new DataTable();
            dt.Load(drTree);
            drTree.Close();
        }

        /// <summary>
        /// Get Client Ids
        /// </summary>
        /// <returns>clents ids</returns>
        public string GetClientId()
        {
                return "0," + ctx.GetVAF_Client_ID();
        }

        /// <summary>
        /// Get Organization ids
        /// </summary>
        /// <returns>org ids</returns>
        public string GetOrgID()
        {
            if (ctx.GetVAF_Role_ID().ToString() != "" && ctx.GetVAF_Role_ID().ToString() != "0")
            {
                return "0," + ctx.GetVAF_Role_ID().ToString();
            }
            else
            {
                string strResult="";
                string strQuery = "select VAF_Org_Id from VAF_Org where VAF_CLIENT_ID="+ctx.GetVAF_Client_ID()
                    +" or VAF_Org_Id=0";
                IDataReader objIDataReader = DataBase.DB.ExecuteReader(strQuery);
                while (objIDataReader.Read())
                {
                    if (strResult == "")
                    {
                        strResult = objIDataReader["VAF_Org_Id"].ToString();
                    }
                    else
                    {
                        strResult = strResult + "," + objIDataReader["VAF_Org_Id"].ToString();
                    }
                }
                objIDataReader.Close();
                return strResult;
            }
        }

        /// <summary>
        /// Get Table Name agianst tree type
        /// </summary>
        /// <returns></returns>
        public string GetTableName()
        {
            string strReturn = "";
            if (treeType == TreeType.OO)
            {
                strReturn= "VAF_TreeInfoChild";
            }
            else
            {
                strReturn = "VAF_TreeInfoChildMenu";
            }
            return strReturn;
        }
        /// <summary>
        /// Get Root Node of tree
        /// </summary>
        /// <returns>root node</returns>
        public VTreeNode GetRootNode()
        {
            return root;
        }


        /// <summary>
        /// Get Depth of tree
        /// </summary>
        /// <returns>depth of tree</returns>
        public int GetMaxLevel()
        {
            return maxLevel - 1;
        }

        /// <summary>
        /// Get Bar Nodes
        /// </summary>
        /// <returns>nodes in favorite tree</returns>
        public List<VTreeNode> GetBarNodes()
        {
            return barNodes;
        }

        /// <summary>
        /// Add Node to bar(favorite tree)
        /// </summary>
        /// <param name="iNodeId"></param>
        /// <returns>true if sucussfull else false</returns>
        public bool SaveNodeToBar(int iNodeId)
        {
            bool blnReturn = true;
            string strSql = @"Insert into VAF_TreeInfoBar (VAF_CLIENT_ID, VAF_ORG_ID, VAF_TreeInfo_ID, VAF_USERCONTACT_ID, 
                             CREATEDBY, ISACTIVE, NODE_ID,UPDATEDBY) 
                             values (" + ctx.GetVAF_Client_ID() + "," + ctx.GetVAF_Org_ID() + @",
                                     " + _VAF_TreeInfo_ID + "," + ctx.GetVAF_UserContact_ID() + "," + ctx.GetVAF_UserContact_ID() + @",
                                    " + "'Y'," + iNodeId.ToString() + "," + ctx.GetVAF_UserContact_ID() + ")";
            try
            {

                SqlExec.ExecuteQuery.ExecuteNonQuery(strSql);
            }
            catch 
            {
                // log file e
                blnReturn = false;
            }
            return blnReturn;
        }

        /// <summary>
        /// Get Action Of Node (eg = W for Winow etc)
        /// </summary>
        /// <param name="Node_Id"></param>
        /// <returns>return action of node(eg - "W" for window)</returns>
        public string GetAction(int Node_ID)
        {
            string strAction = "";
            //Get row Info From Table Using LINQ agianst Node_ID
            var rowInfo = from en in dt.AsEnumerable()
                         where en.Field<decimal>("VAF_MenuConfig_ID") == Node_ID
                         select en;
            
            foreach (DataRow dr in rowInfo)
            {
                strAction = dr["Action"].ToString();
                   
            }
            return strAction;
        }

        /// <summary>
        /// Get Summary Property of node
        /// </summary>
        /// <param name="Node_Id"></param>
        /// <param name="TreeType"></param>
        /// <returns>node is summary or not</returns>
        public string GetSummary(int Node_ID, string treeType)
        {
            string strColumnId = "";
            if (treeType == "MM")
            {
                strColumnId = "VAF_MenuConfig_Id";
            }
            else
            {
                strColumnId = "VAF_Org_Id";
            }

            string strSummary = "";

            if (VAdvantage.DataBase.DatabaseType.IsPostgre)
            {
                //Get row Info From Table Using LINQ agianst Node_ID
                var  rowInfo = from en in dt.AsEnumerable()
                             where en.Field<Int32>(strColumnId) == Node_ID
                             select en;
                foreach (DataRow dr in rowInfo)
                {
                    strSummary = dr["IsSummary"].ToString();
                }
            }
            else //(case for oracle database)
            {
                var rowInfo = from en in dt.AsEnumerable()
                             where en.Field<decimal>(strColumnId) == Node_ID
                             select en;
                foreach (DataRow dr in rowInfo)
                {
                    strSummary = dr["IsSummary"].ToString();
                }
            }

            return strSummary;
        }

        /// <summary>
        /// Remove Node from Favorite Tree (also from database tree bar table)
        /// </summary>
        /// <param name="strNodeId"></param>
        /// <returns>true if success or vice versa</returns>
        public bool RemoveNodeFromBar(string strNodeId)
        {
            bool blnReturn = true;
            string strSql = @"Delete from VAF_TreeInfoBar Where " +
                             " VAF_TreeInfo_ID = " + _VAF_TreeInfo_ID + " and VAF_USERCONTACT_ID=" + ctx.GetVAF_UserContact_ID() + " and  NODE_ID=" + strNodeId;
            try
            {
                SqlExec.ExecuteQuery.ExecuteNonQuery(strSql);
            }
            catch
            {
                //log file e
                blnReturn = false;
            }
            return blnReturn;
        }

    }

    #region "Comment"
    //for (int i = 0; i < dt.Rows.Count; i++)
    //       {
    //         int node = int.Parse(dt.Rows[i][0].ToString());
    //        if (node_ID != node)	//	search for correct one
    //               continue;
    //           //	ID,Name,Description,IsSummary,Action/Color
    //           int index = 1;
    //           string name =  dt.Rows[i][index++].ToString();
    //           string description = dt.Rows[i][index++].ToString();
    //           bool isSummary = "Y".Equals(dt.Rows[i][index++].ToString());
    //           string actionColor = dt.Rows[i][index++].ToString();
    //           //	Menu only
    //           //if (getTreeType().equals(TREETYPE_Menu) && !isSummary)
    //           //{

    //           VAF_Screen_ID = (dt.Rows[i][index].ToString().Trim() == "") ? 0 : int.Parse(dt.Rows[i][index].ToString());
    //           index++;
    //           VAF_Job_ID = (dt.Rows[i][index].ToString().Trim() == "") ? 0 : Utility.Util.GetValueOfInt(dt.Rows[i][index].ToString());
    //           index++;
    //           VAF_Page_ID = (dt.Rows[i][index].ToString().Trim() == "") ? 0 : Utility.Util.GetValueOfInt(dt.Rows[i][index].ToString());
    //           index++;
    //           VAF_Workflow_ID = (dt.Rows[i][index].ToString().Trim() == "") ? 0 : Utility.Util.GetValueOfInt(dt.Rows[i][index].ToString());
    //           index++;
    //           VAF_Task_ID = (dt.Rows[i][index].ToString().Trim() == "") ? 0 : Utility.Util.GetValueOfInt(dt.Rows[i][index].ToString());
    //           index++;
    //           AD_Workbench_ID = (dt.Rows[i][index].ToString().Trim() == "") ? 0 : Utility.Util.GetValueOfInt(dt.Rows[i][index].ToString());
    //           index++;

    //           if (!isSummary)
    //           {
    //               bool? blnAccess = null;

    //               if (VTreeNode.ACTION_WINDOW.Equals(actionColor))
    //               {
    //                   blnAccess = VAF_ROLE.GetWindowAccess(VAF_Screen_ID);
    //               }
    //               else if (VTreeNode.ACTION_PROCESS.Equals(actionColor)
    //                       || VTreeNode.ACTION_REPORT.Equals(actionColor))
    //               {
    //                   blnAccess = VAF_ROLE.GetProcessAccess(VAF_Job_ID);
    //               }
    //               else if (VTreeNode.ACTION_FORM.Equals(actionColor))
    //               {
    //                   blnAccess = VAF_ROLE.GetFormAccess(VAF_Page_ID);
    //               }
    //               else if (VTreeNode.ACTION_WORKFLOW.Equals(actionColor))
    //               {
    //                   blnAccess = VAF_ROLE.GetWorkflowAccess(VAF_Workflow_ID);
    //               }
    //               else if (VTreeNode.ACTION_TASK.Equals(actionColor))
    //               {
    //                   blnAccess = VAF_ROLE.GetTaskAccess(VAF_Task_ID);
    //               }

    //               if (blnAccess != null)		//	rw or ro for Role 
    //               {
    //                   retValue = new VTreeNode(node_ID, seqNo,
    //                       name, description, Parent_ID, isSummary,
    //                       actionColor, onBar);	//	menu has no color
    //               }
    //           }
    //           else
    //           {
    //               retValue = new VTreeNode(node_ID, seqNo,
    //                       name, description, Parent_ID, isSummary,
    //                       actionColor, onBar);
    //           }
    //           break;
    //       }

    #endregion
}