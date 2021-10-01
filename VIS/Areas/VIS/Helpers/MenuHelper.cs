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
        private StringBuilder leftMenuHTML;
        private StringBuilder mainContainerHTML;
        private StringBuilder menu1HTML;
        private StringBuilder menu2HTML;
        private StringBuilder settingsHTML;
        List<string> lstMenuSections;
        private int itemNo = 1;
        private int lastParentID = 0;
        private int rootParentID = 0;
        private bool isSubItem = false;
        private string lastParent = "";

        private bool isSettingItem = false;
        private Dictionary<string, string> menuIcons = new Dictionary<string, string>() {
            { "W","fa fa-window-maximize"},
            { "R","vis vis-report"},
            { "P","fa fa-cog"},
            { "X","fa fa-list-alt"},
            { "S","fa fa-folder-o"}

        };


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
        public MTree GetMenuTree()
        {

            int AD_Tree_ID = DB.GetSQLValue(null,
          "SELECT COALESCE(r.AD_Tree_Menu_ID, ci.AD_Tree_Menu_ID)"
                          + "FROM AD_ClientInfo ci"
                          + " INNER JOIN AD_Role r ON (ci.AD_Client_ID=r.AD_Client_ID) "
                          + "WHERE AD_Role_ID=" + _ctx.GetAD_Role_ID());

            MRole.GetDefault(_ctx, true); // init MRole


            if (AD_Tree_ID <= 0)
            {
                AD_Tree_ID = 10;	//Default Tree
            }

            return GetMenuTree(AD_Tree_ID, false);
            //MTree vTree = new MTree(_ctx, AD_Tree_ID, false, true, null);
            //return vTree;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="AD_Tree_ID"></param>
        /// <param name="editable"></param>
        /// <returns></returns>

        public MTree GetMenuTree(int AD_Tree_ID, bool editable)
        {
            MTree vTree = new MTree(_ctx, AD_Tree_ID, editable, true, null);
            return vTree;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AD_Tree_ID"></param>
        /// <param name="editable"></param>
        /// <returns></returns>

        public MTree GetMenuTree(int AD_Tree_ID, bool editable, bool onDemandTree, int nodeID, int AD_Tab_ID, int winNo)
        {
            _onDemandTree = onDemandTree;
            MTree vTree = null;
            if (onDemandTree)
            {
                vTree = new MTree(_ctx, AD_Tree_ID, editable, true, null, true, AD_Tab_ID, winNo);
            }
            else
            {
                vTree = new MTree(_ctx, AD_Tree_ID, editable, true, null, nodeID, AD_Tab_ID);
            }
            return vTree;
        }

        /// <summary>
        /// Start creating New UI For Web Menu
        /// </summary>
        /// <param name="root"></param>
        /// <param name="baseUrl"></param>
        /// <param name="windowNo"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string GetNewMenuTreeUI(VTreeNode root, string baseUrl, string windowNo = "", string tableName = "table")
        {
            mainContainerHTML = new StringBuilder("<div class='d-flex vis-navContentWrap'>");

            leftMenuHTML = new StringBuilder();

            menu1HTML = new StringBuilder();
            menu2HTML = new StringBuilder();
            settingsHTML = new StringBuilder();
            lstMenuSections = new List<string>();
            leftMenuHTML.Append("<div class='vis-navLeftWrap'><ul class='list-unstyled'>");
            CreateNewTree(root.Nodes, baseUrl, windowNo);

            mainContainerHTML.Append(leftMenuHTML.ToString() + "</ul></div><div class='d-flex vis-navMainContent'>"
                + String.Join(" ", lstMenuSections.ToArray()) + "</div></div>");

            return mainContainerHTML.ToString();

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
        /// Start creatint menu for Mobile
        /// </summary>
        /// <param name="root"></param>
        /// <param name="baseUrl"></param>
        /// <param name="windowNo"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string GetMobileMenuTreeUI(VTreeNode root, string baseUrl, string windowNo = "", string tableName = "table")
        {
            StringBuilder sb = new StringBuilder("");
            if (windowNo != "")
            {
                sb.Append("<ul data-tableName='" + tableName + "'>");
                sb.Append("<li data-value='" + root.Node_ID + "'>").Append(GetRootItem(root.Node_ID, root.SetName, windowNo));
            }
            menu1HTML = new StringBuilder();
            menu2HTML = new StringBuilder();

            menu1HTML.Append("<ul class='vismenu-parent'>");

            sb.Append(CreateMobileTree(root.Nodes, baseUrl, windowNo));

            menu2HTML.Append("</ul>");

            menu1HTML.Append("</li></ul>");

            return menu1HTML.ToString() + menu2HTML.ToString();
        }

        /// <summary>
        /// REcursive function to add summary and non summary items in menu
        /// </summary>
        /// <param name="treeNodeCollection"></param>
        /// <param name="baseUrl"></param>
        /// <param name="windowNo"></param>
        /// <returns></returns>
        private string CreateMobileTree(System.Windows.Forms.TreeNodeCollection treeNodeCollection, string baseUrl, string windowNo = "")
        {
            StringBuilder sb = new StringBuilder();
            bool addbackButton = true;

            foreach (var item in treeNodeCollection)
            {
                VTreeNode vt = (VTreeNode)item;
                if (vt.Parent_ID < 0)
                    continue;
                // Add back buttons for first subfolder which will use to navigate to main menu
                if (vt.Parent_ID != 0 && rootParentID == lastParentID && addbackButton)
                {
                    GetSummaryBackBtn(vt.Node_ID, System.Net.WebUtility.HtmlEncode(lastParent), windowNo, lastParentID);
                    addbackButton = false;
                }
                if (vt.IsSummary)
                {
                    lastParent = vt.SetName;
                    lastParentID = vt.Node_ID;
                    //
                    if (vt.Parent_ID == 0)
                    {
                        rootParentID = vt.Node_ID;
                        sb.Append(GetMobileSummaryItemStart(vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), windowNo, vt.Parent_ID));
                    }
                    else
                    {
                        if (vt.Parent_ID == rootParentID)
                        {

                            GetMobileSummaryItem(vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), rootParentID, false, vt.Image);

                        }
                        else
                        {
                            isSubItem = true;
                            GetMobileSummaryItem(vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), rootParentID, true, vt.Image);

                        }

                    }
                    sb.Append(CreateMobileTree(((System.Windows.Forms.TreeNode)vt).Nodes, baseUrl, windowNo));
                    GetMobileSummaryItemEnd(vt.Parent_ID == 0);
                    if (vt.Parent_ID != rootParentID)
                    {
                        isSubItem = false;
                    }
                }
                else
                {

                    sb.Append(GetMobileTreeItem(vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), vt.ImageKey, vt.GetAction(), vt.GetActionID(), baseUrl, windowNo, vt.OnBar, vt.Parent_ID));
                }
            }
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
                        sb.Append(GetSummaryItemStart(vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), windowNo, vt.Parent_ID));
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
        /// Recursive method to add items in Menu
        /// </summary>
        /// <param name="treeNodeCollection"></param>
        /// <param name="baseUrl"></param>
        /// <param name="windowNo"></param>
        private void CreateNewTree(System.Windows.Forms.TreeNodeCollection treeNodeCollection, string baseUrl, string windowNo = "")
        {
            ////bool isOutContainerDiv = false;
            foreach (var item in treeNodeCollection)
            {
                VTreeNode vt = (VTreeNode)item;
                if (vt.IsSummary)
                {
                    // If parent is 0, that means this is mian item of menu and will be added to left side.
                    //Reset all variables for subitems
                    if (vt.Parent_ID == 0)
                    {
                        rootParentID = vt.Node_ID;
                        GetMainMenuSummaryItem(vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), windowNo);
                        itemNo = 0;
                        menu1HTML.Clear();
                        menu2HTML.Clear();
                        settingsHTML.Clear();
                        lastParentID = 0;
                        isSettingItem = false;
                        menu1HTML.Append("<div class='vis-navColWrap'>");
                        menu2HTML.Append("<div class='vis-navColWrap'>");
                        settingsHTML.Append("<div class='vis-navColWrap'>");
                    }
                    else
                    {
                        //Add item to Settings section of menu , if sequence no is 9999
                        if (vt.IsSetting)
                        {
                            isSettingItem = true;
                            if (vt.Parent_ID == rootParentID)
                            {
                                GetNewSummaryItemStart(vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), windowNo, vt.SeqNo, vt.Image, 0, vt.IsSetting);
                            }
                            else
                            {
                                GetNewSummaryItemStart(vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), windowNo, vt.SeqNo, vt.Image, vt.Parent_ID, vt.IsSetting);
                            }

                            lastParentID = vt.Node_ID;
                        }
                        // If previous node has parent ID 0 or preivios node's parent is same as current
                        else if (lastParentID == 0 || lastParentID == vt.Parent_ID)
                        {
                            //if preivios node's parent is same as current, then treat it as sibling and show in section 1 or 2.
                            if (vt.Parent_ID == rootParentID)
                            {
                                itemNo++;
                                GetNewSummaryItemStart(vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), windowNo, 0, vt.Image, 0);
                            }
                            //Otherwise treat it as child and add as subitem
                            else
                            {
                                GetNewSummaryItemStart(vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), windowNo, 0, vt.Image, vt.Parent_ID);
                            }
                            lastParentID = vt.Node_ID;


                        }
                        else
                        {
                            // Close previous parent and start new one.
                            if (vt.IsSetting || isSettingItem)
                                settingsHTML.Append("</div>");
                            else
                            {
                                if (itemNo > 0 && itemNo % 2 == 0)
                                    menu2HTML.Append("</div>");
                                else
                                    menu1HTML.Append("</div>");
                                itemNo++;
                            }
                            GetNewSummaryItemStart(vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), windowNo, 0, vt.Image, 0);
                        }

                    }

                    CreateNewTree(((System.Windows.Forms.TreeNode)vt).Nodes, baseUrl, windowNo);

                    // here if parent ID is 0, then close this main menu item
                    if (vt.Parent_ID == 0)
                    {
                       
                        if (menu2HTML.Length < 40 && settingsHTML.Length > 40)
                        {
                            menu2HTML.Clear();
                            menu2HTML.Append("<div class='vis-navColWrap' style='display:none'>");
                            menu1HTML.Replace("vis-navColWrap'", "vis-navColWrap' style='min-width:250px'");
                            settingsHTML.Replace("vis-navColWrap'", "vis-navColWrap' style='min-width:250px'");
                        }
                        else if (menu2HTML.Length < 40 && settingsHTML.Length < 40)
                        {
                            menu2HTML.Clear();
                            settingsHTML.Clear();
                            menu2HTML.Append("<div class='vis-navColWrap' style='display:none'>");
                            settingsHTML.Append("<div class='vis-navColWrap' style='display:none'>");
                            menu1HTML.Replace("vis-navColWrap'", "vis-navColWrap' style='min-width:250px'");
                        }
                        else if (menu2HTML.Length > 40 && settingsHTML.Length < 40)
                        {
                            settingsHTML.Clear();
                            settingsHTML.Append("<div class='vis-navColWrap' style='display:none'>");
                            menu1HTML.Replace("vis-navColWrap'", "vis-navColWrap' style='min-width:250px'");
                            menu2HTML.Replace("vis-navColWrap'", "vis-navColWrap' style='min-width:250px'");
                        }

                        menu2HTML.Append("</div>");
                        menu1HTML.Append("</div>");

                        settingsHTML.Append("</div>");

                        isSettingItem = false;



                        lstMenuSections.Add("<div class='vis-navmenuItems-Container' style='display:none' id='Menu" + vt.Node_ID + "'><h6 class='vismenu-hidden-header' style='display:none'><div class='vis-nav-AllItems-Header'>" + vt.SetName + "</div></h6><div style='display:flex'>" + menu1HTML.ToString() + menu2HTML.ToString() + settingsHTML.ToString() + "</div></div>");
                    }
                    // Until parent ID is not 0, keep adding items as child of cuerrnt main item 
                    else
                    {
                        string endDiv = "</div>";
                        if (vt.IsSetting || isSettingItem)
                            settingsHTML.Append(endDiv);
                        else if (itemNo > 0 && itemNo % 2 == 0)
                            menu2HTML.Append(endDiv);
                        else
                            menu1HTML.Append(endDiv);

                        if (vt.IsSetting)
                        {
                            isSettingItem = false;
                        }

                        lastParentID = vt.Parent_ID;
                    }
                }
                else
                {
                    GetNewTreeItem(vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), vt.ImageKey, vt.GetAction(), vt.GetActionID(), baseUrl, windowNo, vt.OnBar, vt.SeqNo);
                }
            }

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
        /// Add non summary items to menu.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text"></param>
        /// <param name="img"></param>
        /// <param name="action"></param>
        /// <param name="aid"></param>
        /// <param name="baseUrl"></param>
        /// <param name="windowNo"></param>
        /// <param name="onBar"></param>
        /// <param name="seqNo"></param>
        private void GetNewTreeItem(int id, string text, string img, string action, int aid, string baseUrl, string windowNo = "", bool onBar = false, int seqNo = 0)
        {
            if (action.Trim() == "") { action = "W"; img = "W"; }

            string contextMenu = "<div class=''>";
            if (onBar)
            {
                contextMenu += " <span data-isfavbtn='yes' data-value='" + id + "' data-isfav='yes' data-action='" + action + "' data-actionid ='" + aid + "' data-name ='" + text + "'><span class='fa fa-star'></span>";
            }
            else
            {
                contextMenu += " <span data-isfavbtn='yes' data-value='" + id + "' data-isfav='no' data-action='" + action + "' data-actionid ='" + aid + "' data-name ='" + text + "'>";
            }

            contextMenu += "  <i class='fa fa-ellipsis-v'></i>" +
           "  </span> </div>";




            if (isSettingItem)
                settingsHTML.Append("<li class='vis-navList'  data-summary='N'><a href='javascript:void(0)'  data-sqeno='" + seqNo + "'  data-value='" + id + "' data-action='" + action + "' data-actionid ='" + aid + "'><i class='" + menuIcons[action] + "'></i>" + text + "</a>" + contextMenu + "</li>");
            else if (itemNo > 0 && itemNo % 2 == 0)
            {
                menu2HTML.Append("<li class='vis-navList'  data-summary='N'><a href='javascript:void(0)'  data-sqeno='" + seqNo + "'  data-value='" + id + "' data-action='" + action + "' data-actionid ='" + aid + "'><i class='" + menuIcons[action] + "'></i>" + text + "</a>" + contextMenu + "</li>");
            }
            else
            {
                menu1HTML.Append("<li class='vis-navList'  data-summary='N'><a href='javascript:void(0)'  data-sqeno='" + seqNo + "'  data-value='" + id + "' data-action='" + action + "' data-actionid ='" + aid + "'><i class='" + menuIcons[action] + "'></i>" + text + "</a>" + contextMenu + "</li>");
            }

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
                h += "<span class='vis-css-treewindow-arrow-up'></span></span>";
                h += "</li>";


            }
            else
            {
                menu2HTML.Append("<li style='min-height: 40px;overflow: auto;' data-value='" + id + "' data-summary='N'>");
                menu2HTML.Append("<a class='vis-menuitm-with-favItm' href='javascript:void(0)'  data-value='" + id + "' data-action='" + action + "' data-actionid ='" + aid + "'>");
                menu2HTML.Append("<span " + GetSpanClass(img));
                if (_ctx.GetIsRightToLeft())
                {
                    menu2HTML.Append(" style='float:right;margin:1px 0px 0px 10px;' ");
                }

                menu2HTML.Append(" ></span>" + text + "</a>");

                if (onBar)
                {
                    menu2HTML.Append("<a data-isfavbtn='yes' data-value='" + id + "' data-isfav='yes' data-action='" + action + "' data-actionid ='" + aid + "' data-name ='" + text + "'   class='vis-menufavitm vis-favitmchecked'></a>");
                }
                else
                {
                    menu2HTML.Append("<a data-isfavbtn='yes' data-value='" + id + "' data-isfav='no' data-action='" + action + "' data-actionid ='" + aid + "'  data-name ='" + text + "' class='vis-menufavitm vis-favitmunchecked'></a>");
                }
            }
            return h;
        }

        /// <summary>
        /// Add Non Summary Nodes to tree
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text"></param>
        /// <param name="img"></param>
        /// <param name="action"></param>
        /// <param name="aid"></param>
        /// <param name="baseUrl"></param>
        /// <param name="windowNo"></param>
        /// <param name="onBar"></param>
        /// <param name="parentID"></param>
        /// <returns></returns>
        private string GetMobileTreeItem(int id, string text, string img, string action, int aid, string baseUrl, string windowNo = "", bool onBar = false, int parentID = 0)
        {
            if (action.Trim() == "") { action = "W"; img = "W"; }
            var h = "";

            string contextMenu = "<div class=''>";
            if (onBar)
            {
                contextMenu += " <span data-isfavbtn='yes' data-value='" + id + "' data-isfav='yes' data-action='" + action + "' data-actionid ='" + aid + "' data-name ='" + text + "'><span class='fa fa-star'></span>";
            }
            else
            {
                contextMenu += " <span data-isfavbtn='yes' data-value='" + id + "' data-isfav='no' data-action='" + action + "' data-actionid ='" + aid + "' data-name ='" + text + "'>";
            }

            contextMenu += "  <i class='fa fa-ellipsis-v'></i>" +
           "  </span> </div>";
            if (isSubItem)
                menu2HTML.Append("<li class='vis-menuItem vis-menusubItem' style='overflow: auto;display:none' data-ulid='" + parentID + "' data-val='" + id + "' data-summary='N' >");
            else
                menu2HTML.Append("<li class='vis-menuItem' style='overflow: auto' data-ulid='" + parentID + "' data-val='" + id + "' data-summary='N' >");
            menu2HTML.Append("<a class='vis-menuitm-with-favItm' href='javascript:void(0)'  data-val='" + id + "' data-action='" + action + "' data-actionid ='" + aid + "'>");
            menu2HTML.Append("<span " + GetSpanClass(img));

            menu2HTML.Append(" ></span>" + text + "</a>");
            menu2HTML.Append(contextMenu);
            menu2HTML.Append("</li>");
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
        private string GetSummaryItemStart(int id, string text, string windowNo = "", int parentID = 0)
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
                if (parentID > 0)
                    menu1HTML.Append("<li  data-con='Y' data-value='" + id + "' data-summary='Y' class='vis-hasSubMenu' style='display:none'> ");
                else
                    menu1HTML.Append("<li  data-con='Y' data-value='" + id + "' data-summary='Y' class='vis-hasSubMenu'> ");
                menu1HTML.Append("<input type='checkbox'  id='" + windowNo + id + "' />");
                menu1HTML.Append("<label data-target='#ul_" + id + "' data-toggle='collapse' for='" + windowNo + id + "'><i class='fa fa-folder-o vis-folder-open-ico'></i>" + text + " </label>");

                if (parentID > 0)
                    menu2HTML.Append("<ul  data-con='Y' id='ul_" + id + "' style='display:none'>");
                else
                    menu2HTML.Append("<ul data-con='Y' id='ul_" + id + "' style='display:none'>");

                menu2HTML.Append("<li style='min-height: 40px;overflow: auto;' data-value='" + id + "' data-summary='N' style='display:none'>");
                menu2HTML.Append("<a class='vis-menuitm-with-favItm vis-menuitm-backbtn' href='javascript:void(0)'  data-value='" + id + "' data-action='N' data-actionid ='" + id + "'>");
                menu2HTML.Append("<span class='' style='float:right;margin:1px 0px 0px 10px;'></span>" + text + "</a></li>");
            }



            return h;
        }

        private string GetMobileSummaryItemStart(int id, string text, string windowNo = "", int parentID = 0)
        {
            var h = "";
            if (windowNo != "")
            {
                h += "<li data-val='" + id + "' data-summary='Y' class='vis-hasSubMenu'><input type='checkbox'  id='" + windowNo + id + "' /><label for='" + windowNo + id + "'>" + text + "</label>";
                h += "<span class='vis-treewindow-span'><span class='vis-css-treewindow-arrow-up'></span></span>";
                h += "<ul>";
            }
            else
            {
                if (parentID > 0)
                    menu1HTML.Append("<li  data-con='Y' data-val='" + id + "' data-summary='Y' class='vis-hasSubMenu' style='display:none'> ");
                else
                    menu1HTML.Append("<li  data-con='Y' data-val='" + id + "' data-summary='Y' class='vis-hasSubMenu'> ");
                menu1HTML.Append("<input type='checkbox'  id='" + windowNo + id + "' />");
                menu1HTML.Append("<label data-target='" + id + "' data-toggle='collapse' for='" + windowNo + id + "'>" + text + " </label>");

                if (parentID > 0)
                    menu2HTML.Append("<ul  data-con='Y' data-ulid='" + id + "' style='display:none'>");
                else
                    menu2HTML.Append("<ul data-con='Y' data-ulid='" + id + "' style='display:none'>");


            }

            return h;
        }

        // Add back buttons for first subfolder which will use to navigate to main menu
        private void GetSummaryBackBtn(int id, string text, string windowNo = "", int parentID = 0)
        {
            menu2HTML.Append("<ul  data-ulid='" + parentID + "'><li class='vis-subNavFirstElement' style='min-height: 40px;overflow: auto;display:none' data-con='Y' data-ulid='" + parentID + "' data-val='" + id + "' data-summary='N'>");
            menu2HTML.Append("<label class='vis-menuitm-with-favItm vis-menuitm-backbtn' href='javascript:void(0)'  data-val='" + id + "' data-action='N' data-actionid ='" + id + "'>");
            menu2HTML.Append("<i class='fa fa-arrow-left'></i><span class='' style='float:right;margin:1px 0px 0px 10px;'></span>" + text + "</label></li>");
        }


        private string GetMobileSummaryItem(int id, string text, int parentID = 0, bool addPlusIcon = false, string Image = null)
        {
            var h = "";
            string icon = "<i class='" + menuIcons["S"] + "'></i>";

            // if any image is linked with summary item, then show that one
            if (!string.IsNullOrEmpty(Image))
            {
                if (Image.IndexOf("Images/") > -1)
                {
                    icon = Image.Substring(Image.IndexOf("Images/") + 7);
                    icon = "<img src='" + _ctx.GetContextUrl() + "Images/Thumb32x32/" + icon + "'/>";
                }
                else
                {
                    icon = "<i class='" + Image + "'></i>";
                }
            }


            menu2HTML.Append("<ul class='vis-menu-foldderUL' data-con='Y' data-ulid='" + parentID + "' style='display:none'>");

            menu2HTML.Append("<li class='vis-menu-innerFolders' data-con='Y' style='overflow: auto;' data-val='" + id + "' data-summary='N'>");
            menu2HTML.Append(icon).Append("<span class='vis-menuitm-with-favItm' href='javascript:void(0)'  data-val='" + id + "' data-action='N' data-actionid ='" + id + "'>");
            if (addPlusIcon)
                menu2HTML.Append(text + "</span><i class='fa fa-plus vis-navAccordian' data-pID='" + id + "'></i></li>");
            else
                menu2HTML.Append(text + "</span></li>");

            return h;
        }


        /// <summary>
        /// Create Folder in menu. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text"></param>
        /// <param name="windowNo"></param>
        /// <param name="seqNo"></param>
        /// <param name="Image"></param>
        /// <param name="parent_ID">if not 0, that means this usmmary item is child of another one. So add plus icon for that</param>
        private void GetNewSummaryItemStart(int id, string text, string windowNo = "", int seqNo = 0, string Image = "", int parent_ID = 0, bool setting = false)
        {
            string icon = "<i class='" + menuIcons["S"] + "'></i>";

            // if any image is linked with summary item, then show that one
            if (!string.IsNullOrEmpty(Image))
            {
                if (Image.IndexOf("Images/") > -1)
                {
                    icon = Image.Substring(Image.IndexOf("Images/") + 7);
                    icon = "<img src='" + _ctx.GetContextUrl() + "Images/Thumb32x32/" + icon + "'/>";
                }
                else
                {
                    icon = "<i class='" + Image + "'></i>";
                }
            }

            string expandIcon = "";
            var ulUnStyle = "<ul class='list-unstyled'>";
            string subSummaryNodeClasss = "";
            if (parent_ID > 0)
            {
                subSummaryNodeClasss = "vismenu-subSummaryNode";
                expandIcon = "<i class='fa fa-plus vis-navAccordian' data-pID='" + id + "'></i>";
                ulUnStyle = "<ul class='list-unstyled vismenu-subSummaryNode' style='Display:none' data-ulID='" + id + "'>";
            }
            //if (container)
            //{
            //    continerAttribute = "data-isContainer='Y'";
            //    parentDiv = "<div class='vismenu-hidden-header'><h6 class='vismenu-hidden-headerh5' style='display:none'>" + rootItem+"</h6>";
            //}


            if (setting || isSettingItem)
            {
                settingsHTML.Append("<div  data-sqeno='" + seqNo + "' data-value='" + id + "' data-summary='Y'  class='vis-navSubMenu'><h5 class='vis-navDataHead'>" + icon +
        "<span>" + text + "</span>" + expandIcon + "</h5>" + ulUnStyle);
            }
            else if (itemNo > 0 && itemNo % 2 == 0)
            {
                menu2HTML.Append("<div data-sqeno='" + seqNo + "'  data-value='" + id + "' data-summary='Y'  class='vis-navSubMenu'><h5 class='vis-navDataHead'>" + icon +
    "<span>" + text + "</span>" + expandIcon + "</h5>" + ulUnStyle);
            }
            else
            {
                menu1HTML.Append("<div data-sqeno='" + seqNo + "' data-value='" + id + "' data-summary='Y'  class='vis-navSubMenu'><h5 class='vis-navDataHead'>" + icon +
"<span>" + text + "</span>" + expandIcon + "</h5>" + ulUnStyle);
            }


        }

        /// <summary>
        /// Add new summary item to left section of menu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text"></param>
        /// <param name="windowNo"></param>
        private void GetMainMenuSummaryItem(int id, string text, string windowNo = "")
        {
            leftMenuHTML.Append("<li class='vis-navList'><a  data-value='" + id + "' data-summary='Y' href='javascript:void(0)'>" + text + "<i class='vis vis-arrow-right'></i></a></li>");
        }

        public int updateTree(Ctx ctx, string nodeID, int oldParentID, int newParentID, int AD_Tree_ID)
        {
            MTree trr = new MTree(ctx, AD_Tree_ID, null);
            string tableName = trr.GetNodeTableName();


            string[] selectedID = nodeID.Split(',');


            string sql = "Update " + tableName + " SET SeqNo=Seqno+" + selectedID.Length + ", updated=SYSDATE WHERE AD_Tree_ID=" + AD_Tree_ID + " AND Parent_ID=" + newParentID;
            DB.ExecuteQuery(sql);

            for (int i = 0; i < selectedID.Length; i++)
            {
                DB.ExecuteQuery("UPDATE " + tableName + " SET Parent_ID=" + newParentID + ", seqNo=0, updated=SYSDATE WHERE AD_Tree_ID=" + AD_Tree_ID + " AND Node_ID=" + selectedID[i]);
            }

            return 1;

        }


        /// <summary>
        /// summary node end html string 
        /// </summary>
        /// <returns></returns>
        private string GetSummaryItemEnd()
        {
            //menu2HTML.Append("</ul>");
            //menu1HTML.Append("</li>");
            //return "";
            return "</ul></li>";
        }

        //private string GetNewSummaryItemEnd()
        //{
        //    menu2HTML.Append("</ul>");
        //    menu1HTML.Append("</li>");
        //    return "";
        //    //  return "</ul></li>";
        //}


        private string GetMobileSummaryItemEnd(bool hasbackButton)
        {
            if (hasbackButton)
                menu2HTML.Append("</ul>");
            menu2HTML.Append("</ul>");
            menu1HTML.Append("</li>");
            return "";
            //  return "</ul></li>";
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
                string str = "SELECT Count(1) FROM AD_TABLE WHERE upper(TableName) = 'C_BPARTNER' ";
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