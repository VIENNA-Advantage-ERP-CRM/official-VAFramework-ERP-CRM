using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Login;
using VAdvantage.Model;
using VAdvantage.Utility;


namespace VIS.Helpers
{
    /// <summary>
    /// Construct menu after login
    /// </summary>
    public class MenuHelper
    {
        /* context */
        private Ctx _ctx = null;
        public bool _onDemandTree = false;


        /// <summary>
        /// std contructor
        /// </summary>
        /// <param name="ctx"></param>
        public MenuHelper(Ctx ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// retrun Model objet of tree
        /// --  
        /// </summary>
        /// <returns>MTree object</returns>
        public MVAFTreeInfo GetMenuTree()
        {

            int VAF_TreeInfo_ID = DB.GetSQLValue(null,
          "SELECT COALESCE(r.VAF_TreeInfo_Menu_ID, ci.VAF_TreeInfo_Menu_ID)"
                          + "FROM VAF_ClientDetail ci"
                          + " INNER JOIN VAF_Role r ON (ci.VAF_Client_ID=r.VAF_Client_ID) "
                          + "WHERE VAF_Role_ID=" + _ctx.GetVAF_Role_ID());

            MVAFRole.GetDefault(_ctx, true); // init MRole


            if (VAF_TreeInfo_ID <= 0)
            {
                VAF_TreeInfo_ID = 10;	//Default Tree
            }

            return GetMenuTree(VAF_TreeInfo_ID, false);
            //MTree vTree = new MTree(_ctx, VAF_TreeInfo_ID, false, true, null);
            //return vTree;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="VAF_TreeInfo_ID"></param>
        /// <param name="editable"></param>
        /// <returns></returns>

        public MVAFTreeInfo GetMenuTree(int VAF_TreeInfo_ID, bool editable)
        {
            MVAFTreeInfo vTree = new MVAFTreeInfo(_ctx, VAF_TreeInfo_ID, editable, true, null);
            return vTree;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="VAF_TreeInfo_ID"></param>
        /// <param name="editable"></param>
        /// <returns></returns>

        public MVAFTreeInfo GetMenuTree(int VAF_TreeInfo_ID, bool editable, bool onDemandTree, int nodeID,int VAF_Tab_ID,int winNo)
        {
            _onDemandTree = onDemandTree;
            MVAFTreeInfo vTree = null;
            if (onDemandTree)
            {
                vTree = new MVAFTreeInfo(_ctx, VAF_TreeInfo_ID, editable, true, null, true, VAF_Tab_ID, winNo);
            }
            else
            {
                vTree = new MVAFTreeInfo(_ctx, VAF_TreeInfo_ID, editable, true, null, nodeID, VAF_Tab_ID);
            }
            return vTree;
        }


        /// <summary>
        /// get Menu Tree html String 
        /// </summary>
        /// <param name="root">Root of tree</param>
        /// <param name="baseUrl">application url</param>
        /// <returns>html string</returns>
        public string GetMenuTreeUI(VTreeNode root, string baseUrl, string windowNo = "", string tableName = "table")
        {
            StringBuilder sb = new StringBuilder("");
            if (windowNo != "")
            {
                sb.Append("<ul data-tableName='" + tableName + "'>");
                sb.Append("<li data-value='" + root.Node_ID + "'>").Append(GetRootItem(root.Node_ID, root.SetName, windowNo));
            }

            sb.Append("<ul>");

            sb.Append(CreateTree(root.Nodes, baseUrl, windowNo));

            sb.Append("</ul>");

            sb.Append("</li></ul>");

            return sb.ToString();
        }

        /// <summary>
        /// Create Tree 
        /// </summary>
        /// <param name="treeNodeCollection"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        private string CreateTree(System.Windows.Forms.TreeNodeCollection treeNodeCollection, string baseUrl, string windowNo = "")
        {
            StringBuilder sb = new StringBuilder();

            if (_onDemandTree)
            {
                foreach (var item in treeNodeCollection)
                {
                    VTreeNode vt = (VTreeNode)item;
                    if (vt.IsSummary)
                    {
                        if (vt.IsSummary)
                        {
                            if (((System.Windows.Forms.TreeNode)vt).Nodes.Count > 0)
                            {
                                sb.Append(GetSummaryItemStart(vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName) + " (" + ((System.Windows.Forms.TreeNode)vt).Nodes.Count + ")", windowNo));
                            }
                            else
                            {
                                sb.Append(GetSummaryItemStart(vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), windowNo));
                            }
                            sb.Append(CreateTree(((System.Windows.Forms.TreeNode)vt).Nodes, baseUrl, windowNo));
                            sb.Append(GetSummaryItemEnd());
                        }
                        else
                        {
                            sb.Append(GetTreeItem(vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), vt.ImageKey, vt.GetAction(), vt.GetActionID(), baseUrl, windowNo, vt.OnBar));
                        }
                    }
                }
            }
            else
            {

                foreach (var item in treeNodeCollection)
                {
                    VTreeNode vt = (VTreeNode)item;
                    if (vt.IsSummary)
                    {
                        sb.Append(GetSummaryItemStart(vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), windowNo));
                        sb.Append(CreateTree(((System.Windows.Forms.TreeNode)vt).Nodes, baseUrl, windowNo));
                        sb.Append(GetSummaryItemEnd());
                    }
                    else
                    {
                        sb.Append(GetTreeItem(vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), vt.ImageKey, vt.GetAction(), vt.GetActionID(), baseUrl, windowNo, vt.OnBar));
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Return Root item 
        /// </summary>
        /// <param name="id">id of node</param>
        /// <param name="text">text to display</param>
        /// <returns>root item hmnl string</returns>
        private string GetRootItem(int id, string text, string windowNo = "")
        {
            var h = "<input type='checkbox' data-value='" + id + "'  id='" + windowNo + id + "' checked='checked' /><label for='" + windowNo + id + "'>" + text + "</label>";
            if (windowNo != "")
            {
                h += "<span></span>";
            }
            return h;
        }

        /// <summary>
        /// get leaf node html string
        /// </summary>
        /// <param name="id">id of node</param>
        /// <param name="text">text to display</param>
        /// <param name="img">img to display gainst node</param>
        /// <param name="action">action of node (window , form etc)</param>
        /// <param name="aid">data attribute id</param>
        /// <param name="baseUrl">app url</param>
        /// <returns>html string </returns>
        private string GetTreeItem(int id, string text, string img, string action, int aid, string baseUrl, string windowNo = "", bool onBar = false)
        {
            if (action.Trim() == "") { action = "W"; img = "W"; }
            var h = "";
            if (windowNo != "")
            {
                h += "<li  data-value='" + id + "' data-summary='N'>" +
                    "<a href='javascript:void(0)' data-value='" + id + "' data-action='" + action + "' data-actionid =" + aid + "> ";

                h += "<span " + GetSpanClass(img);

                if (_ctx.GetIsRightToLeft())
                {
                    h += " style='float:right;margin:1px 0px 0px 10px;' ";
                }

                h += " ></span>" + text + "</a>";
                h += "<span class='vis-treewindow-span'>";
                h +=  "<span class='vis-css-treewindow-arrow-up'></span></span>";
                h += "</li>";


            }
            else
            {
                h += "<li style='min-height: 40px;overflow: auto;' data-value='" + id + "' data-summary='N'>" +
                     "<a class='vis-menuitm-with-favItm' href='javascript:void(0)'  data-value='" + id + "' data-action='" + action + "' data-actionid ='" + aid + "'>" +
                     "<span " + GetSpanClass(img);
                if (_ctx.GetIsRightToLeft())
                {
                    h += " style='float:right;margin:1px 0px 0px 10px;' ";
                }

                h += " ></span>" + text + "</a>";

                if (onBar)
                {
                    h += "<a data-isfavbtn='yes' data-value='" + id + "' data-isfav='yes' data-action='" + action + "' data-actionid ='" + aid + "' data-name ='" + text + "'   class='vis-menufavitm vis-favitmchecked'></a>";
                }
                else
                {
                    h += "<a data-isfavbtn='yes' data-value='" + id + "' data-isfav='no' data-action='" + action + "' data-actionid ='" + aid + "'  data-name ='" + text + "' class='vis-menufavitm vis-favitmunchecked'></a>";
                }
            }
            return h;
        }

        private string GetSpanClass(string img)
        {
            switch (img)
            {
                case "W":
                    return "class = 'fa fa-window-maximize'";
                case "R":
                    return "class = 'vis vis-report'";
                case "P":
                    return "class = 'fa fa-cog'";
                case "T":
                    return "class = 'fa fa-cog'";
                case "F":
                    return "class = 'fa fa-clone'";
                case "B":
                    return "class = 'fa fa-clone'";
                case "X":
                    return "class = 'fa fa-list-alt'";
                case "V":
                    return "class = 'fa fa-clone'";
                case "D":
                    return "class = 'fa fa-clone'";
                default:
                    return "class = 'fa fa-clone'";
            }

            //public static String ACTION_UserWorkbench = "B";
            ///** User Choice = C */
            //public static String ACTION_UserChoice = "C";
            ///** Document Action = D */
            //public static String ACTION_DocumentAction = "D";
            ///** Sub Workflow = F */
            //public static String ACTION_SubWorkflow = "F";
            ///** EMail = M */
            //public static String ACTION_EMail = "M";
            ///** Apps Process = P */
            //public static String ACTION_AppsProcess = "P";
            ///** Apps Report = R */
            //public static String ACTION_AppsReport = "R";
            ///** Apps Task = T */
            //public static String ACTION_AppsTask = "T";
            ///** Set Variable = V */
            //public static String ACTION_SetVariable = "V";
            ///** User Window = W */
            //public static String ACTION_UserWindow = "W";
            ///** User Form = X */
            //public static String ACTION_UserForm = "X";
            ///** Wait (Sleep) = Z */
            //public static String ACTION_WaitSleep = "Z";

        }

        /// <summary>
        /// summary node start html string 
        /// </summary>
        /// <param name="id">id of node</param>
        /// <param name="text">text display</param>
        /// <returns>html string</returns>
        private string GetSummaryItemStart(int id, string text, string windowNo = "")
        {
            var h = "";
            if (windowNo != "")
            {
                h += "<li data-value='" + id + "' data-summary='Y' class='vis-hasSubMenu'><input type='checkbox'  id='" + windowNo + id + "' /><label for='" + windowNo + id + "'>" + text + "</label>";
                h += "<span class='vis-treewindow-span'><span class='vis-css-treewindow-arrow-up'></span></span>";
                h += "<ul>";
            }
            else
            {
                h += "<li  data-value='" + id + "' data-summary='Y' class='vis-hasSubMenu'> " +
                     "<input type='checkbox'  id='" + windowNo + id + "' />" +
                     "<label data-target='#ul_" + id + "' data-toggle='collapse' for='" + windowNo + id + "'><i class='fa fa-folder-o vis-folder-open-ico'></i>" +
                      text + 
                      " </label>";
                h += "<ul class='collapse'  id='ul_" + id + "'>";
            }

            return h;
        }

        public int updateTree(Ctx ctx, string nodeID, int oldParentID, int newParentID, int VAF_TreeInfo_ID)
        {
            MVAFTreeInfo trr = new MVAFTreeInfo(ctx, VAF_TreeInfo_ID, null);
            string tableName = trr.GetNodeTableName();


            string[] selectedID = nodeID.Split(',');


            string sql = "Update " + tableName + " SET SeqNo=Seqno+" + selectedID.Length + ", updated=SYSDATE WHERE VAF_TreeInfo_ID=" + VAF_TreeInfo_ID + " AND Parent_ID=" + newParentID;
            DB.ExecuteQuery(sql);

            for (int i = 0; i < selectedID.Length; i++)
            {
                DB.ExecuteQuery("UPDATE " + tableName + " SET Parent_ID=" + newParentID + ", seqNo=0, updated=SYSDATE WHERE VAF_TreeInfo_ID=" + VAF_TreeInfo_ID + " AND Node_ID=" + selectedID[i]);
            }

            return 1;

        }


        /// <summary>
        /// summary node end html string 
        /// </summary>
        /// <returns></returns>
        private string GetSummaryItemEnd()
        {
            return "</ul></li>";
        }

        /// <summary>
        /// return image url according node action
        /// </summary>
        /// <param name="initial">action of node</param>
        /// <param name="baseUrl">app url</param>
        /// <returns>uri string</returns>
        private static string GetImageURI(string initial, string baseUrl)
        {
            var url = baseUrl + "Areas/VIS/Images/login/";
            switch (initial)
            {
                case "W":
                    return url + "mWindow.png";
                case "R":
                    return url + "mReport.png";
                case "P":
                    return url + "mProcess.png";
                case "F":
                    return url + "mWorkflow.png";
                case "B":
                    return url + "mWorkbench.png";
                case "X":
                    return url + "mWindow.png";
                case "V":
                    return url + "mWindow.png";
                case "D":
                    return url + "mDocAction.png";
                default:
                    return url + "mWindow.png";
            }
        }


        /// <summary>
        /// clean up
        /// </summary>
        internal void dispose()
        {
            _ctx = null;
        }

        internal bool GetIsBasicDB()
        {
            bool isBase = false;
            try
            {
                string str = "SELECT Count(1) FROM VAF_TABLEVIEW WHERE upper(TableName) = 'VAB_BUSINESSPARTNER' ";
                int count = Convert.ToInt32(DB.ExecuteScalar(str, null, null));
                if (count <= 0)
                    isBase = true;
            }
            catch
            {
                isBase = true;
            }
            return isBase;
        }
    }
}