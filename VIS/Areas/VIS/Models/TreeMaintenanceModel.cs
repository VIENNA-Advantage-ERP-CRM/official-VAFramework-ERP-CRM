using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Model;
using System.Text;
using VAdvantage.Classes;
using VIS.Areas.VIS.Models;
using VAdvantage.Logging;


namespace VIS.Models
{
    public class TreeMaintenanceModel
    {
        Ctx _ctx = null;
        bool _editable;
        VTreeNode root = null;
        DataTable dt;
        VAdvantage.Classes.VTree.TreeType Vtreetype;
        MTree objVTree = null;
        List<VTreeNode> barNodes = new List<VTreeNode>();
        List<VTreeNode> mnuNodes = new List<VTreeNode>();
        string bindornot = "true";
        string menuArrays = "";

        public TreeMaintenanceModel(Ctx ctx)
        {
            _ctx = ctx;
        }

        public List<TreeDataForCombo> TreeDataForCombo()
        {
            List<TreeDataForCombo> obj = new List<TreeDataForCombo>();
            //string sql = "SELECT Name,description,treetype,isallnodes,AD_Tree_ID,AD_Table_ID FROM AD_Tree WHERE TreeType NOT IN ('BB','PC') ORDER BY AD_Tree_ID DESC";
            string sql = "SELECT Name,description,treetype,isallnodes,AD_Tree_ID,AD_Table_ID FROM AD_Tree WHERE TreeType NOT IN ('BB','PC' ,'U1','U2','U3','U4','AY','CC','CT','CM','CS') AND IsActive='Y' ORDER BY AD_Tree_ID DESC";

            //            sql = MRole.GetDefault(_ctx).AddAccessSQL(sql, "AD_Tree", true, true);

            sql = MRole.GetDefault(_ctx).AddAccessSQL(sql, "AD_Tree", true, true);
            //            sql = MRole.GetDefault(_ctx).AddAccessSQL(sql, "AD_Tree", true, false);

            DataSet ds = DB.ExecuteDataset(sql, null, null);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj.Add(new TreeDataForCombo()
                    {
                        Name = System.Net.WebUtility.HtmlDecode(Convert.ToString(ds.Tables[0].Rows[i]["Name"])),
                        ID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Tree_ID"]),
                        TreeType = Convert.ToString(ds.Tables[0].Rows[i]["treetype"]),
                        IsAllNodes = Convert.ToString(ds.Tables[0].Rows[i]["isallnodes"]),
                    });
                }
            }
            return obj;
        }

        public List<AllMenuData> LoadMenuData(Ctx _ctx, int pageLength, int pageNo, int AD_Tree_ID, string searchtext, string demandsMenu)
        {
            List<AllMenuData> obj = new List<AllMenuData>();
            List<int> getNodeID = new List<int>();

            MTree tree = new MTree(_ctx, AD_Tree_ID, null);
            int AD_Table_ID = tree.GetAD_Table_ID();
            string TableName = MTable.GetTableName(_ctx, AD_Table_ID);
            string type = tree.GetTreeType();

            string sql = "";

            string qryPaging = "";

            searchtext = System.Net.WebUtility.HtmlDecode(searchtext);



            if (demandsMenu == "All")
            {

                if (searchtext != "")
                {
                    qryPaging = @"SELECT * FROM 
                     
                              ( SELECT " + TableName + @"_ID, description,issummary,Name,ROW_NUMBER () OVER (ORDER BY " + TableName + @"_ID) row_num  FROM " + TableName + @"  WHERE UPPER(Name) Like '%" + searchtext.ToUpper() + @"%' AND IsActive='Y' ORDER BY upper(name) ) 
                            
                           x WHERE row_num BETWEEN (((" + pageNo + @"-1) * " + pageLength + @") + 1)

                        AND ((" + pageNo + @" * " + pageLength + @") + 1 ) ";

                }
                else
                {
                    qryPaging = @"SELECT * FROM 
                     
                       ( SELECT " +TableName + @"_ID, description,issummary,Name,ROW_NUMBER () OVER (ORDER BY " + TableName + @"_ID) row_num FROM " + TableName + @" WHERE IsActive='Y' ORDER BY upper(name) ) 
                            
                           x WHERE row_num BETWEEN (((" + pageNo + @"-1) * " + pageLength + @") + 1)  

                     AND  ((" + pageNo + @" * " + pageLength + @") + 1 )";
                }

                DataSet ds = new DataSet();
                qryPaging = MRole.GetDefault(_ctx).AddAccessSQL(qryPaging, TableName, true, false);
                ds = DB.ExecuteDataset(qryPaging, null, null);

                string qry = "";
                DataSet qryds = new DataSet();

                qry = "SELECT node_id FROM " + tree.GetNodeTableName() + " WHERE IsActive='Y' AND AD_Tree_ID=" + AD_Tree_ID;
                qry = MRole.GetDefault(_ctx).AddAccessSQL(qry, tree.GetNodeTableName(), true, false);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    qry += " AND node_id IN (";

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (i > 0)
                        {
                            qry += ",";
                        }
                        qry += ds.Tables[0].Rows[i][TableName + "_ID"].ToString();
                    }
                    qry += ")";
                }

                qryds = DB.ExecuteDataset(qry, null, null);

                if (qryds != null && qryds.Tables[0].Rows.Count > 0)
                {
                    for (int k = 0; k < qryds.Tables[0].Rows.Count; k++)
                    {
                        getNodeID.Add(Convert.ToInt32(qryds.Tables[0].Rows[k]["node_id"]));
                    }
                }


                if (demandsMenu == "All")
                {

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            var id = TableName + "_ID";
                            int getindex = getNodeID.IndexOf(Convert.ToInt32(ds.Tables[0].Rows[i][id]));
                            if (getindex > -1)
                            {
                                obj.Add(new AllMenuData()
                                {
                                    ID = Convert.ToInt32(ds.Tables[0].Rows[i][id]),
                                    Name = System.Net.WebUtility.HtmlDecode(Convert.ToString(ds.Tables[0].Rows[i]["Name"])),
                                    description = System.Net.WebUtility.HtmlDecode(Convert.ToString(ds.Tables[0].Rows[i]["description"])),
                                    issummary = Convert.ToString(ds.Tables[0].Rows[i]["issummary"]),
                                    color = "Black",
                                    disabled = true,
                                    classopacity = "vis-tm-opacity",
                                    tbname = tree.GetNodeTableName(),
                                    //unlinkClass = "glyphicon glyphicon-link",
                                    unlinkClass = "VIS-Tm-glyphiconglyphicon-link",
                                });
                            }
                            else if (qryds != null)
                            {
                                obj.Add(new AllMenuData()
                                {
                                    ID = Convert.ToInt32(ds.Tables[0].Rows[i][id]),
                                    Name = System.Net.WebUtility.HtmlDecode(Convert.ToString(ds.Tables[0].Rows[i]["Name"])),
                                    description = System.Net.WebUtility.HtmlDecode(Convert.ToString(ds.Tables[0].Rows[i]["description"])),
                                    issummary = Convert.ToString(ds.Tables[0].Rows[i]["issummary"]),
                                    color = "Black",
                                    disabled = false,
                                    tbname = tree.GetNodeTableName(),
                                });
                            }
                            else
                            {
                                obj.Add(new AllMenuData()
                                {
                                    ID = Convert.ToInt32(ds.Tables[0].Rows[i][id]),
                                    Name = System.Net.WebUtility.HtmlDecode(Convert.ToString(ds.Tables[0].Rows[i]["Name"])),
                                    description = System.Net.WebUtility.HtmlDecode(Convert.ToString(ds.Tables[0].Rows[i]["description"])),
                                    issummary = Convert.ToString(ds.Tables[0].Rows[i]["issummary"]),
                                    color = "Black",
                                    disabled = true,
                                    classopacity = "vis-tm-opacity",
                                    tbname = tree.GetNodeTableName(),
                                    unlinkClass = "VIS-Tm-glyphiconglyphicon-link",
                                });
                            }

                        }
                    }
                }

            }


            // //string unlinkdata = "select * from M_Product where IsActive='Y' AND M_Pr_ID NOT IN (select NOde_ID FROM AD_TreeNodePR where AD_Tree_ID=" + AD_Tree_ID + ")";
            if (demandsMenu == "Unlinked")
            {
                string tbname = tree.GetNodeTableName();
                string unlinkdata = "";
                string executeqry = "";

                if (searchtext != "")
                {
                    unlinkdata = "SELECT " + TableName + @"_ID, description,issummary,Name, ROW_NUMBER () OVER (ORDER BY " + TableName + @"_ID) row_num FROM " + TableName + " WHERE  UPPER(Name) Like '%" + searchtext.ToUpper() + @"%' AND  IsActive='Y' AND " + TableName + "_ID NOT IN (SELECT Node_ID FROM " + tbname + " where AD_Tree_ID=" + AD_Tree_ID + ") ORDER BY upper(name))";

                    // unlinkdata = "SELECT * FROM " + TableName + " WHERE  UPPER(Name) Like '%" + searchtext.ToUpper() + @"%' AND  IsActive='Y' AND " + TableName + "_ID NOT IN (SELECT Node_ID FROM " + tbname + " ) ORDER BY upper(name))";
                    executeqry = @"SELECT * FROM (
                     
                              ( " + unlinkdata + @" )
                            
                           x WHERE row_num BETWEEN (((" + pageNo + @"-1) * " + pageLength + @") + 1)
                        
                        AND ((" + pageNo + @" * " + pageLength + @") + 1 )";
                }
                else
                {
                    unlinkdata = "SELECT  " + TableName + @"_ID, description,issummary,Name,ROW_NUMBER () OVER (ORDER BY " + TableName + @"_ID) row_num FROM " + TableName + " WHERE IsActive='Y' AND " + TableName + "_ID NOT IN (SELECT Node_ID FROM " + tbname + " where AD_Tree_ID=" + AD_Tree_ID + ") ORDER BY upper(name)";


                    //unlinkdata = "SELECT * FROM " + TableName + " WHERE IsActive='Y' AND " + TableName + "_ID NOT IN (SELECT Node_ID FROM " + tbname + " ) ORDER BY upper(name)";
                    executeqry = @"SELECT * FROM 
                                      ( " + unlinkdata + @"  )
                            
                         x   WHERE row_num BETWEEN (((" + pageNo + @"-1) * " + pageLength + @") + 1)            
                        
                        AND  ((" + pageNo + @" * " + pageLength + @") + 1 )";
                }
                DataSet dst = new DataSet();
                executeqry = MRole.GetDefault(_ctx).AddAccessSQL(executeqry, TableName, true, false);
                 dst = DB.ExecuteDataset(executeqry, null, null);


                if (dst != null && dst.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                    {
                        var id = TableName + "_ID";
                        int getindex = getNodeID.IndexOf(Convert.ToInt32(dst.Tables[0].Rows[i][id]));
                        // if (getindex == -1)
                        // {
                        obj.Add(new AllMenuData()
                        {
                            ID = Convert.ToInt32(dst.Tables[0].Rows[i][id]),
                            Name = System.Net.WebUtility.HtmlDecode(Convert.ToString(dst.Tables[0].Rows[i]["Name"])),
                            description = System.Net.WebUtility.HtmlDecode(Convert.ToString(dst.Tables[0].Rows[i]["description"])),
                            issummary = Convert.ToString(dst.Tables[0].Rows[i]["issummary"]),
                            color = "Black",
                            disabled = false,
                            tbname = tree.GetNodeTableName(),
                        });
                        // }
                    }
                }
            }


            else if (demandsMenu == "Linked")
            {
                string tbname = tree.GetNodeTableName();
                string unlinkdata = "";
                string executeqry = "";

                if (searchtext != "")
                {
                    unlinkdata = "SELECT " + TableName + @"_ID, description,issummary,Name,ROW_NUMBER () OVER (ORDER BY " + TableName + @"_ID) row_num FROM " + TableName + " WHERE  UPPER(Name) Like '%" + searchtext.ToUpper() + @"%' AND  IsActive='Y' AND " + TableName + "_ID IN (SELECT Node_ID FROM " + tbname + " where AD_Tree_ID=" + AD_Tree_ID + ") ORDER BY upper(name))";
                    executeqry = @"SELECT * FROM 
                     
                              ( " + unlinkdata + @" )
                            
                           x WHERE row_num BETWEEN (((" + pageNo + @"-1) * " + pageLength + @") + 1)

                        AND ((" + pageNo + @" * " + pageLength + @") + 1 ) ";

                }
                else
                {
                    unlinkdata = "SELECT  " + TableName + @"_ID, description,issummary,Name,ROW_NUMBER () OVER (ORDER BY " + TableName + @"_ID) row_num FROM " + TableName + " WHERE IsActive='Y' AND " + TableName + "_ID  IN (SELECT Node_ID FROM " + tbname + " where AD_Tree_ID=" + AD_Tree_ID + ") ORDER BY upper(name)";

                    executeqry = @"SELECT * FROM 
                                      ( " + unlinkdata + @"  )
                            
                           x WHERE row_num BETWEEN (((" + pageNo + @"-1) * " + pageLength + @") + 1)            
                        
                        AND  ((" + pageNo + @" * " + pageLength + @") + 1 )";
                }

                executeqry = MRole.GetDefault(_ctx).AddAccessSQL(executeqry, TableName, true, false);
                DataSet dst = DB.ExecuteDataset(executeqry, null, null);


                if (dst != null && dst.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                    {
                        var id = TableName + "_ID";
                        obj.Add(new AllMenuData()
                        {
                            ID = Convert.ToInt32(dst.Tables[0].Rows[i][id]),
                            Name = System.Net.WebUtility.HtmlDecode(Convert.ToString(dst.Tables[0].Rows[i]["Name"])),
                            description = System.Net.WebUtility.HtmlDecode(Convert.ToString(dst.Tables[0].Rows[i]["description"])),
                            issummary = Convert.ToString(dst.Tables[0].Rows[i]["issummary"]),
                            color = "Black",
                            disabled = true,
                            classopacity = "vis-tm-opacity",
                            tbname = tree.GetNodeTableName(),
                            unlinkClass = "VIS-Tm-glyphiconglyphicon-link",
                        });
                    }
                }
            }
            return obj;
        }

        public Tree BindTree(Ctx _ctx, string treeType, int AD_Tree_ID, string isAllNodes, bool isSummary)
        {
            objVTree = new MTree(_ctx, AD_Tree_ID, true, false, null, isSummary);
            List<SetTree> setttreeobj = new List<SetTree>();

            SetTree trees = new SetTree();
            trees.AD_Tree_ID = AD_Tree_ID;
            setttreeobj.Add(trees);

            Tree obj1 = new Tree();
            //obj1.reeDataForCombo = obj;            
            obj1.settree = GetMenuTreeUI(trees, objVTree.GetRootNode(), objVTree.GetNodeTableName());

            return obj1;
        }

        //public List<SetTree> GetMenuTreeUI(SetTree trees, VTreeNode vObject, string tbname, List<TreeDataForCombo> obj)
        public List<SetTree> GetMenuTreeUI(SetTree trees, VTreeNode vObject, string tbname)
        {
            List<SetTree> obj2 = new List<SetTree>();


            SetTree treees = new SetTree()
            {
                text = System.Net.WebUtility.HtmlEncode(vObject.SetName),
                TableName = tbname,
                IsSummary = true,
                NodeID = vObject.Node_ID,
                IsActive = true,
                ImageSource = "Areas/VIS/Images/folder.png",
                ParentID = vObject.Parent_ID,
                TreeParentID = vObject.Parent_ID,
                imageIndegator = vObject.ImageKey,
                cmbname = "cmbname",
            };

            obj2.Add(treees);
            CreateTree(treees, vObject.Nodes);
            return obj2;
        }

        private string CreateTree(SetTree trees, System.Windows.Forms.TreeNodeCollection treeNodeCollection)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in treeNodeCollection)
            {
                VTreeNode vt = (VTreeNode)item;
                if (vt.IsSummary)
                {
                    SetTree newTrees = new SetTree();
                    if (trees.items == null)
                    {
                        trees.ImageSource = "Areas/VIS/Images/folder.png";
                        //trees.AD_Tree_ID = trees.AD_Tree_ID;
                        trees.id = trees.id;
                        trees.IsSummary = trees.IsSummary;
                        trees.NodeID = trees.NodeID;
                        trees.ParentID = trees.ParentID;
                        trees.TreeParentID = trees.TreeParentID;
                        trees.TableName = trees.TableName;
                        trees.getparentnode = "getparentnode";
                        trees.items = new List<SetTree>();
                    }
                    trees.items.Add(newTrees);

                    sb.Append(GetSummaryItemStart(newTrees, vt.Parent_ID, vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName)));
                    sb.Append(CreateTree(newTrees, ((System.Windows.Forms.TreeNode)vt).Nodes));
                }
                else
                {

                    sb.Append(GetTreeItem(trees, vt.Parent_ID, vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), vt.ImageKey, vt.GetAction(), vt.GetActionID(), vt.OnBar));
                }
            }

            return sb.ToString();
        }

        private string GetSummaryItemStart(SetTree newTrees, int parentID, int id, string text, string windowNo = "")
        {
            var h = "";
            //h += " { text: '" + text + "', issummary: true , nodeid:" + id + ",items:[";

            //            newTrees.text = System.Net.WebUtility.HtmlEncode(text);
            newTrees.text = text;
            newTrees.IsSummary = true;
            newTrees.NodeID = id;
            newTrees.ParentID = parentID;
            newTrees.TreeParentID = parentID;
            newTrees.ImageSource = "Areas/VIS/Images/folder.png";
            newTrees.TableName = newTrees.TableName;

            return h;
        }

        private string GetTreeItem(SetTree newTrees, int parent_ID, int id, string text, string img, string action, int aid, bool onBar = false)
        {
            if (action.Trim() == "") { action = "W"; img = "W"; }
            var h = "";
            if (newTrees.items == null)
            {
                newTrees.items = new List<SetTree>();
            }
            SetTree nTree = new SetTree();
            //nTree.text = System.Net.WebUtility.HtmlEncode(text);
            nTree.text = text;
            nTree.ParentID = parent_ID;
            nTree.TreeParentID = parent_ID;
            nTree.IsSummary = false;
            nTree.TableName = newTrees.TableName;


            nTree.NodeID = id;


            //if (action == "R")
            //{
            //    nTree.ImageSource = "Areas/VIS/Images/mReport.png";
            //}
            //else if (action == "P")
            //{
            //    nTree.ImageSource = "Areas/VIS/Images/mProcess.png";
            //}
            //else if (action == "F")
            //{
            //    nTree.ImageSource = "Areas/VIS/Images/login/mWorkFlow.png";
            //}
            //else if (action == "B")
            //{
            //    nTree.ImageSource = "Areas/VIS/Images/login/mWorkbench.png";
            //}
            //else
            //{
            nTree.ImageSource = "Areas/VIS/Images/mWindow.png";
            // }

            if (id != 0)
            {
                newTrees.items.Add(nTree);
            }

            return h;
        }

        /// <summary>
        /// Save Data with Right and Mid Drag
        /// </summary>
        /// <param name="_ctx"></param>
        /// <param name="summaryid"></param>
        /// <param name="nodid"></param>
        /// <param name="treeID"></param>
        /// <param name="dragMenuNodeID"></param>
        /// <param name="checkMorRdragable"></param>
        /// <returns></returns>
        public string SaveDataOnDrop(Ctx _ctx, int summaryid, int nodid, int treeID, string dragMenuNodeID, bool checkMorRdragable, string IsExistItem)
        {
            MTree tree = new MTree(_ctx, treeID, null);
            int AD_Table_ID = tree.GetAD_Table_ID();
            string TableName = MTable.GetTableName(_ctx, AD_Table_ID);
            string type = tree.GetTreeType();
            string[] stringArray = dragMenuNodeID.Split(',');
            string[] existitem = IsExistItem.Split(',');

            //string sqlqq = "SELECT Max(seqNo) FROM " + tree.GetNodeTableName() + " WHERE parent_ID=" + summaryid + " AND AD_Tree_ID=" + treeID;
            //object maxID = DB.ExecuteScalar(sqlqq);
            //int j = 0;
            //if (maxID != null && maxID != DBNull.Value)
            //{
            //    j = Convert.ToInt32(maxID) + 1;
            //}

            List<int> matchwithchild = new List<int>();


            string getchild = "SELECT node_id FROM " + tree.GetNodeTableName() + " WHERE parent_id=" + summaryid + " AND ad_tree_id=" + treeID;

            getchild = MRole.GetDefault(_ctx).AddAccessSQL(getchild, tree.GetNodeTableName(), true, false);
            DataSet dsgetparent = DB.ExecuteDataset(getchild, null, null);


            if (dsgetparent != null && dsgetparent.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsgetparent.Tables[0].Rows.Count; i++)
                {
                    matchwithchild.Add(Convert.ToInt32(dsgetparent.Tables[0].Rows[i]["node_id"]));
                }
            }


            if (summaryid == 0)
            {
                DB.ExecuteQuery("UPDATE " + tree.GetNodeTableName() + "  SET Updated=Sysdate, seqNo=seqNo+" + (stringArray.Length + 1) + " WHERE (Parent_ID=" + summaryid + " OR Parent_ID IS NULL) AND AD_Tree_ID=" + treeID);
            }
            else
            {
                DB.ExecuteQuery("UPDATE " + tree.GetNodeTableName() + "  SET Updated=Sysdate, seqNo=seqNo+" + (stringArray.Length + 1) + " WHERE Parent_ID=" + summaryid + " AND AD_Tree_ID=" + treeID);
            }




            for (int i = 0; i < stringArray.Length; i++)
            {
                if (Convert.ToInt32(stringArray[i]) == summaryid)
                {
                    continue;
                }

                int getindex = matchwithchild.IndexOf(Convert.ToInt32(stringArray[i]));
                if (getindex > -1)
                {
                    continue;
                }



                if (type == "PR")
                {
                    if (checkMorRdragable)
                    {
                        MTreeNodePR obj = MTreeNodePR.Get(tree, Convert.ToInt32(stringArray[i]));
                        obj.SetParent_ID(summaryid);
                        obj.SetSeqNo(i);
                        if (!obj.Save())
                        {
                            return Msg.GetMsg(_ctx, "VIS_DataNotSave");
                            //return "DataNotSave";
                        }
                    }
                    else
                    {
                        if (existitem[i] == "new")
                        {
                            MTreeNodePR obj = new MTreeNodePR(tree, Convert.ToInt32(stringArray[i]));
                            obj.SetParent_ID(summaryid);
                            obj.SetSeqNo(i);
                            if (!obj.Save())
                            {
                                return Msg.GetMsg(_ctx, "VIS_DataNotSave");
                            }
                        }
                        else
                        {
                            MTreeNodePR obj = MTreeNodePR.Get(tree, Convert.ToInt32(stringArray[i]));
                            obj.SetParent_ID(summaryid);
                            obj.SetSeqNo(i);
                            if (!obj.Save())
                            {
                                return Msg.GetMsg(_ctx, "VIS_DataNotSave");
                            }
                        }
                    }
                }
                else if (type == "MM")
                {
                    if (checkMorRdragable)
                    {
                        MTreeNodeMM obj = MTreeNodeMM.Get(tree, Convert.ToInt32(stringArray[i]));
                        obj.SetParent_ID(summaryid);
                        obj.SetSeqNo(i);
                        if (!obj.Save())
                        {
                            //return "DataNotSave";
                            return Msg.GetMsg(_ctx, "VIS_DataNotSave");
                        }
                    }
                    else
                    {
                        if (existitem[i] == "new")
                        {
                            MTreeNodeMM obj = new MTreeNodeMM(tree, Convert.ToInt32(stringArray[i]));
                            obj.SetParent_ID(summaryid);
                            obj.SetSeqNo(i);
                            if (!obj.Save())
                            {
                                //return "DataNotSave";
                                return Msg.GetMsg(_ctx, "VIS_DataNotSave");
                            }
                        }
                        else
                        {
                            MTreeNodeMM obj = MTreeNodeMM.Get(tree, Convert.ToInt32(stringArray[i]));
                            obj.SetParent_ID(summaryid);
                            obj.SetSeqNo(i);
                            if (!obj.Save())
                            {
                                //return "DataNotSave";
                                return Msg.GetMsg(_ctx, "VIS_DataNotSave");
                            }
                        }
                    }
                }
                else if (type == "BP")
                {
                    if (checkMorRdragable)
                    {
                        MTreeNodeBP obj = MTreeNodeBP.Get(tree, Convert.ToInt32(stringArray[i]));
                        obj.SetParent_ID(summaryid);
                        obj.SetSeqNo(i);
                        if (!obj.Save())
                        {
                            return Msg.GetMsg(_ctx, "VIS_DataNotSave");
                            //   return "DataNotSave";
                        }
                    }
                    else
                    {
                        if (existitem[i] == "new")
                        {
                            MTreeNodeBP obj = new MTreeNodeBP(tree, Convert.ToInt32(stringArray[i]));
                            obj.SetParent_ID(summaryid);
                            obj.SetSeqNo(i);
                            if (!obj.Save())
                            {
                                //return "DataNotSave";
                                return Msg.GetMsg(_ctx, "VIS_DataNotSave");
                            }
                        }
                        else
                        {
                            MTreeNodeBP obj = MTreeNodeBP.Get(tree, Convert.ToInt32(stringArray[i]));
                            obj.SetParent_ID(summaryid);
                            obj.SetSeqNo(i);
                            if (!obj.Save())
                            {
                                //return "DataNotSave";
                                return Msg.GetMsg(_ctx, "VIS_DataNotSave");
                            }
                        }
                    }
                }
                else
                {
                    if (checkMorRdragable)
                    {
                        MTreeNode obj = MTreeNode.Get(tree, Convert.ToInt32(stringArray[i]));
                        obj.SetParent_ID(summaryid);
                        obj.SetSeqNo(i);
                        if (!obj.Save())
                        {
                            //return "DataNotSave";
                            return Msg.GetMsg(_ctx, "VIS_DataNotSave");
                        }
                    }
                    else
                    {

                        if (existitem[i] == "new")
                        {
                            MTreeNode obj = new MTreeNode(tree, Convert.ToInt32(stringArray[i]));
                            obj.SetParent_ID(summaryid);
                            obj.SetSeqNo(i);
                            if (!obj.Save())
                            {
                                //return "DataNotSave";
                                return Msg.GetMsg(_ctx, "VIS_DataNotSave");
                            }
                        }
                        else
                        {
                            MTreeNode obj = MTreeNode.Get(tree, Convert.ToInt32(stringArray[i]));
                            obj.SetParent_ID(summaryid);
                            obj.SetSeqNo(i);
                            if (!obj.Save())
                            {
                                //return "DataNotSave";
                                return Msg.GetMsg(_ctx, "VIS_DataNotSave");
                            }
                        }
                    }
                }
            }
            return Msg.GetMsg(_ctx, "VIS_DataSave");
            //return "DataSave"; 
        }

        //        public List<GetDataTreeNodeSelect> GetDataTreeNodeSelect(Ctx _ctx, int nodeID, int treeID, int pageNo, int pageLength, string searchChildNode, string getTreeNodeChkValue)
        //        {
        //            List<GetDataTreeNodeSelect> obj = new List<GetDataTreeNodeSelect>();
        //            MTree tree = new MTree(_ctx, treeID, null);
        //            int AD_Table_ID = tree.GetAD_Table_ID();
        //            string TableName = MTable.GetTableName(_ctx, AD_Table_ID);
        //            string type = tree.GetTreeType();
        //            string qry = "";


        //            //            if (parentID == 0)
        //            //            {
        //            //                qry = @"SELECT mp.name,mp.issummary, mp." + TableName + @"_ID FROM " + TableName + @" mp 
        //            //                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @" 
        //            //                           ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" ORDER BY COALESCE(" + tree.GetNodeTableName() + @".parent_id," + -1 + @"), " + tree.GetNodeTableName() + ".seqno";                
        //            //            }
        //            //            else
        //            //            {
        //            //                qry = @"SELECT mp.name, mp.issummary,mp." + TableName + @"_ID FROM " + TableName + @" mp 
        //            //                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"                            
        //            //                             ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + parentID + "  ORDER BY COALESCE(" + tree.GetNodeTableName() + @".parent_id," + -1 + @"), " + tree.GetNodeTableName() + ".seqno";              
        //            //            }

        //            string qryPaging = "";

        //            string clientIDProp = MRole.GetDefault(_ctx).GetClientWhere(true);

        //            string orgwhere = MRole.GetDefault(_ctx).GetOrgWhere(true);
        //            clientIDProp = clientIDProp + " AND " + orgwhere;

        //            int orgIDss = _ctx.GetAD_Org_ID();

        //            if (getTreeNodeChkValue == "true")
        //            {
        //                if (searchChildNode != "")
        //                {
        //                    if (nodeID == 0)
        //                    {
        //                        int clientID = _ctx.GetAD_Client_ID();

        //                        // WHERE mp.Isactive='Y' AND mp.issummary='N' AND  (" + tree.GetNodeTableName() + ".parent_id    =0   OR " + tree.GetNodeTableName() + ".parent_id IS NULL) AND " + tree.GetNodeTableName() + @".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + @".AD_Client_ID IN (0," + clientID + @")  AND Upper(mp.name) LIKE upper('%" + searchChildNode + @"%') 

        //                        // WHERE mp.Isactive='Y' AND mp.issummary='N' AND  " + tree.GetNodeTableName() + ".parent_id    =0  AND " + tree.GetNodeTableName() + @".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + @".AD_Client_ID IN (0," + clientID + @")  AND Upper(mp.name) LIKE upper('%" + searchChildNode + @"%') 
        //                        qryPaging = @"  SELECT * FROM
        //                            
        //(SELECT *  FROM
        //    (SELECT name, issummary," + TableName + @"_ID, parent_id, seqno, rownum r__
        //    FROM
        //      (SELECT mp.name,mp.issummary, mp." + TableName + @"_ID, " + tree.GetNodeTableName() + @".parent_id, 
        //        " + tree.GetNodeTableName() + @".seqno 
        //        FROM " + TableName + @" mp
        //      INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"
        //        ON mp." + TableName + @"_ID = " + tree.GetNodeTableName() + @".node_id
        //      WHERE mp.Isactive='Y' AND mp.issummary='N' AND  (" + tree.GetNodeTableName() + ".parent_id    =0   OR " + tree.GetNodeTableName() + ".parent_id IS NULL) AND " + tree.GetNodeTableName() + @".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + @".AD_Client_ID IN (0," + clientID + @") AND " + orgwhere + @"  AND Upper(mp.name) LIKE upper('%" + searchChildNode + @"%') 
        // ORDER BY  COALESCE(" + tree.GetNodeTableName() + ".Parent_ID, -1),   seqno " + @"
        //)
        //    WHERE rownum < ((" + pageNo + @" * " + pageLength + @") + 1 )
        //    )
        //  WHERE r__ >= (((" + pageNo + @"-1) * " + pageLength + @") + 1)
        //  )
        //ORDER BY   seqno";
        //                        //ORDER BY COALESCE(parent_id,-1),  seqno";
        //                        //  WHERE mp.Isactive='Y' AND  " + tree.GetNodeTableName() + ".parent_id=0 AND " + tree.GetNodeTableName() + @".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + @".AD_Client_ID IN (0," + clientID + @")  AND Upper(mp.name) LIKE upper('%" + searchChildNode + @"%') 
        //                        qry = qryPaging;
        //                    }
        //                    else
        //                    {//ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE  mp.Isactive='Y' AND " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + nodeID + " AND Upper(mp.name) LIKE upper('%" + searchChildNode + "%')  ORDER BY COALESCE(" + tree.GetNodeTableName() + @".parent_id," + -1 + @"), " + tree.GetNodeTableName() + ".seqno";
        //                        qry = @"SELECT mp.name, mp.issummary,mp." + TableName + @"_ID FROM " + TableName + @" mp 
        //                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"                            
        //                             ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE  mp.Isactive='Y' AND mp.issummary='N' AND " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + nodeID + " AND Upper(mp.name) LIKE upper('%" + searchChildNode + "%') ORDER BY  COALESCE(" + tree.GetNodeTableName() + ".Parent_ID, -1),   seqno ";


        //                        qryPaging = @"SELECT * FROM (
        //                     
        //                         SELECT name, issummary, " + TableName + @"_ID, rownum r__
        //                         FROM
        //                              ( " + qry + @" ) 
        //                            
        //                            WHERE rownum < ((" + pageNo + @" * " + pageLength + @") + 1 )
        //                        )
        //                        WHERE r__ >= (((" + pageNo + @"-1) * " + pageLength + @") + 1)";
        //                        qry = MRole.GetDefault(_ctx).AddAccessSQL(qryPaging, tree.GetNodeTableName(), true, false);
        //                    }

        //                }
        //                else
        //                {

        //                    if (nodeID == 0)
        //                    {
        //                        //                qry = @"SELECT mp.name,mp.issummary, mp." + TableName + @"_ID FROM " + TableName + @" mp 
        //                        //                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @" 
        //                        //                           ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" ORDER BY COALESCE(" + tree.GetNodeTableName() + @".parent_id," + -1 + @"), " + tree.GetNodeTableName() + ".seqno";
        //                        int clientID = _ctx.GetAD_Client_ID();
        //                        //WHERE mp.Isactive='Y' AND mp.issummary='N' AND  (" + tree.GetNodeTableName() + ".parent_id    =0   OR " + tree.GetNodeTableName() + ".parent_id IS NULL)  AND  " + tree.GetNodeTableName() + @".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + @".AD_Client_ID IN (0," + clientID + @")
        //                        //WHERE mp.Isactive='Y' AND mp.issummary='N' AND  " + tree.GetNodeTableName() + ".parent_id    =0     AND  " + tree.GetNodeTableName() + @".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + @".AD_Client_ID IN (0," + clientID + @")
        //                        qryPaging = @"  SELECT * FROM
        //                            
        //(SELECT *  FROM
        //    (SELECT name, issummary," + TableName + @"_ID, parent_id, seqno, rownum r__
        //    FROM
        //      (SELECT mp.name,mp.issummary, mp." + TableName + @"_ID, " + tree.GetNodeTableName() + @".parent_id, 
        //        " + tree.GetNodeTableName() + @".seqno 
        //        FROM " + TableName + @" mp
        //      INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"
        //        ON mp." + TableName + @"_ID = " + tree.GetNodeTableName() + @".node_id
        //     WHERE mp.Isactive='Y' AND mp.issummary='N' AND  (" + tree.GetNodeTableName() + ".parent_id    =0   OR " + tree.GetNodeTableName() + ".parent_id IS NULL)  AND  " + tree.GetNodeTableName() + @".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + @".AD_Client_ID IN (0," + clientID + @") AND " + orgwhere + @"
        // ORDER BY COALESCE(" + tree.GetNodeTableName() + ".Parent_ID, -1),   seqno " + @"
        //)
        //    WHERE rownum < ((" + pageNo + @" * " + pageLength + @") + 1 )
        //    )
        //  WHERE r__ >= (((" + pageNo + @"-1) * " + pageLength + @") + 1)
        //  )
        //ORDER BY   seqno";
        //                        //ORDER BY COALESCE(parent_id,-1),  seqno";
        //                        qry = qryPaging;
        //                    }
        //                    else
        //                    {//ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE mp.Isactive='Y' AND  " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + nodeID + "  ORDER BY COALESCE(" + tree.GetNodeTableName() + @".parent_id," + -1 + @"), " + tree.GetNodeTableName() + ".seqno";
        //                        qry = @"SELECT mp.name, mp.issummary,mp." + TableName + @"_ID FROM " + TableName + @" mp 
        //                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"                            
        //                             ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE mp.Isactive='Y' AND mp.issummary='N' AND  " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + nodeID + "  ORDER BY  COALESCE(" + tree.GetNodeTableName() + ".Parent_ID, -1),   seqno ";


        //                        qryPaging = @"SELECT * FROM (
        //                     
        //                         SELECT name, issummary, " + TableName + @"_ID, rownum r__
        //                         FROM
        //                              ( " + qry + @" ) 
        //                            
        //                            WHERE rownum < ((" + pageNo + @" * " + pageLength + @") + 1 )
        //                        )
        //                        WHERE r__ >= (((" + pageNo + @"-1) * " + pageLength + @") + 1)";
        //                        qry = MRole.GetDefault(_ctx).AddAccessSQL(qryPaging, tree.GetNodeTableName(), true, false);
        //                    }

        //                }
        //            }
        //            else
        //            {

        //                if (searchChildNode != "")
        //                {

        //                    if (nodeID == 0)
        //                    {
        //                        //                qry = @"SELECT mp.name,mp.issummary, mp." + TableName + @"_ID FROM " + TableName + @" mp 
        //                        //                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @" 
        //                        //                           ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" ORDER BY COALESCE(" + tree.GetNodeTableName() + @".parent_id," + -1 + @"), " + tree.GetNodeTableName() + ".seqno";
        //                        int clientID = _ctx.GetAD_Client_ID();
        //                        // WHERE mp.Isactive='Y' AND mp.issummary='N' AND Upper(mp.name) LIKE upper('%" + searchChildNode + @"%')   AND  (" + tree.GetNodeTableName() + ".parent_id    =0   OR " + tree.GetNodeTableName() + ".parent_id IS NULL) AND  " + tree.GetNodeTableName() + @".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + @".AD_Client_ID IN (0," + clientID + @")
        //                        //  WHERE mp.Isactive='Y' AND mp.issummary='N' AND Upper(mp.name) LIKE upper('%" + searchChildNode + @"%')   AND  " + tree.GetNodeTableName() + ".parent_id    =0    AND  " + tree.GetNodeTableName() + @".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + @".AD_Client_ID IN (0," + clientID + @")
        //                        qryPaging = @"  SELECT * FROM
        //                            
        //(SELECT *  FROM
        //    (SELECT name, issummary," + TableName + @"_ID, parent_id, seqno, rownum r__
        //    FROM
        //      (SELECT mp.name,mp.issummary, mp." + TableName + @"_ID, " + tree.GetNodeTableName() + @".parent_id, 
        //        " + tree.GetNodeTableName() + @".seqno 
        //        FROM " + TableName + @" mp
        //      INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"
        //        ON mp." + TableName + @"_ID = " + tree.GetNodeTableName() + @".node_id
        //    WHERE mp.Isactive='Y' AND mp.issummary='N' AND Upper(mp.name) LIKE upper('%" + searchChildNode + @"%')   AND  (" + tree.GetNodeTableName() + ".parent_id    =0   OR " + tree.GetNodeTableName() + ".parent_id IS NULL) AND  " + tree.GetNodeTableName() + @".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + @".AD_Client_ID IN (0," + clientID + @") AND " + orgwhere + @"
        // ORDER BY   COALESCE(" + tree.GetNodeTableName() + ".Parent_ID, -1),   seqno " + @"
        //)
        //    WHERE rownum < ((" + pageNo + @" * " + pageLength + @") + 1 )
        //    )
        //  WHERE r__ >= (((" + pageNo + @"-1) * " + pageLength + @") + 1)
        //  )
        //ORDER BY  seqno";
        //                        //ORDER BY COALESCE(parent_id,-1),  seqno";
        //                        qry = qryPaging;
        //                    }
        //                    else
        //                    {//ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE  mp.Isactive='Y' AND " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + nodeID + "  ORDER BY COALESCE(" + tree.GetNodeTableName() + @".parent_id," + -1 + @"), " + tree.GetNodeTableName() + ".seqno";
        //                        qry = @"SELECT mp.name, mp.issummary,mp." + TableName + @"_ID FROM " + TableName + @" mp 
        //                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"                            
        //                             ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE  mp.Isactive='Y' AND mp.issummary='N'  AND Upper(mp.name) LIKE upper('%" + searchChildNode + @"%')     AND " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + nodeID + "  ORDER BY COALESCE(" + tree.GetNodeTableName() + ".Parent_ID, -1),   seqno ";


        //                        qryPaging = @"SELECT * FROM (
        //                     
        //                         SELECT name, issummary, " + TableName + @"_ID, rownum r__
        //                         FROM
        //                              ( " + qry + @" ) 
        //                            
        //                            WHERE rownum < ((" + pageNo + @" * " + pageLength + @") + 1 )
        //                        )
        //                        WHERE r__ >= (((" + pageNo + @"-1) * " + pageLength + @") + 1)";
        //                        qry = MRole.GetDefault(_ctx).AddAccessSQL(qryPaging, tree.GetNodeTableName(), true, false);
        //                    }
        //                }
        //                else
        //                {
        //                    if (nodeID == 0)
        //                    {
        //                        //                qry = @"SELECT mp.name,mp.issummary, mp." + TableName + @"_ID FROM " + TableName + @" mp 
        //                        //                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @" 
        //                        //                           ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" ORDER BY COALESCE(" + tree.GetNodeTableName() + @".parent_id," + -1 + @"), " + tree.GetNodeTableName() + ".seqno";
        //                        int clientID = _ctx.GetAD_Client_ID();
        //                        //WHERE mp.Isactive='Y' AND mp.issummary='N' AND  (" + tree.GetNodeTableName() + ".parent_id    =0   OR " + tree.GetNodeTableName() + ".parent_id IS NULL) AND  " + tree.GetNodeTableName() + @".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + @".AD_Client_ID IN (0," + clientID + @")
        //                        // WHERE mp.Isactive='Y' AND mp.issummary='N' AND  " + tree.GetNodeTableName() + ".parent_id    =0    AND  " + tree.GetNodeTableName() + @".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + @".AD_Client_ID IN (0," + clientID + @")
        //                        qryPaging = @"  SELECT * FROM
        //                            
        //(SELECT *  FROM
        //    (SELECT name, issummary," + TableName + @"_ID, parent_id, seqno, rownum r__
        //    FROM
        //      (SELECT mp.name,mp.issummary, mp." + TableName + @"_ID, " + tree.GetNodeTableName() + @".parent_id, 
        //        " + tree.GetNodeTableName() + @".seqno 
        //        FROM " + TableName + @" mp
        //      INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"
        //        ON mp." + TableName + @"_ID = " + tree.GetNodeTableName() + @".node_id
        //    WHERE mp.Isactive='Y' AND mp.issummary='N' AND  (" + tree.GetNodeTableName() + ".parent_id    =0   OR " + tree.GetNodeTableName() + ".parent_id IS NULL) AND  " + tree.GetNodeTableName() + @".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + @".AD_Client_ID IN (0," + clientID + @") AND " + orgwhere + @"
        // ORDER BY  COALESCE(" + tree.GetNodeTableName() + ".Parent_ID, -1),   seqno " + @"
        //)
        //    WHERE rownum < ((" + pageNo + @" * " + pageLength + @") + 1 )
        //    )
        //  WHERE r__ >= (((" + pageNo + @"-1) * " + pageLength + @") + 1)
        //  )
        //ORDER BY  seqno";
        //                        //ORDER BY COALESCE(parent_id,-1),  seqno";
        //                        qry = qryPaging;
        //                    }
        //                    else
        //                    {//ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE  mp.Isactive='Y' AND " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + nodeID + "  ORDER BY COALESCE(" + tree.GetNodeTableName() + @".parent_id," + -1 + @"), " + tree.GetNodeTableName() + ".seqno";
        //                        qry = @"SELECT mp.name, mp.issummary,mp." + TableName + @"_ID FROM " + TableName + @" mp 
        //                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"                            
        //                             ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE  mp.Isactive='Y' AND mp.issummary='N' AND " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + nodeID + "  ORDER BY COALESCE(" + tree.GetNodeTableName() + ".Parent_ID, -1),   seqno ";


        //                        qryPaging = @"SELECT * FROM (
        //                     
        //                         SELECT name, issummary, " + TableName + @"_ID, rownum r__
        //                         FROM
        //                              ( " + qry + @" ) 
        //                            
        //                            WHERE rownum < ((" + pageNo + @" * " + pageLength + @") + 1 )
        //                        )
        //                        WHERE r__ >= (((" + pageNo + @"-1) * " + pageLength + @") + 1)";
        //                        qry = MRole.GetDefault(_ctx).AddAccessSQL(qryPaging, tree.GetNodeTableName(), true, false);
        //                    }
        //                }
        //            }

        //            //qry = MRole.GetDefault(_ctx).AddAccessSQL(qryPaging, tree.GetNodeTableName(), true, false);


        //            //   DB.ExecuteReader(qry.ToString());




        //            DataSet ds = DB.ExecuteDataset(qry, null, null);
        //            if (ds != null && ds.Tables[0].Rows.Count > 0)
        //            {
        //                var id = TableName + "_ID";
        //                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //                {
        //                    obj.Add(new GetDataTreeNodeSelect()
        //                    {
        //                        parentID = Convert.ToInt32(ds.Tables[0].Rows[i][id]),
        //                        name = System.Net.WebUtility.HtmlDecode(Convert.ToString(ds.Tables[0].Rows[i]["name"])),
        //                        issummary = Convert.ToString(ds.Tables[0].Rows[i]["issummary"]),
        //                    });
        //                }
        //            }

        //            return obj;
        //        }



        //-----------------------------------
        public List<GetDataTreeNodeSelect> FillSequenceDailog(Ctx _ctx, int nodeID, int treeID, int pageNo, int pageLength, string searchChildNode, string getTreeNodeChkValue)
        {

            List<GetDataTreeNodeSelect> obj = new List<GetDataTreeNodeSelect>();
            MTree tree = new MTree(_ctx, treeID, null);
            int AD_Table_ID = tree.GetAD_Table_ID();
            string TableName = MTable.GetTableName(_ctx, AD_Table_ID);
            string type = tree.GetTreeType();
            string qry = "";

            string qryPaging = "";


            //            if (nodeID == 0)
            //            {
            //                int clientID = _ctx.GetAD_Client_ID();
            //                qryPaging = @"  SELECT * FROM
            //                            
            //(SELECT *  FROM
            //    (SELECT name, issummary," + TableName + @"_ID, parent_id, seqno, rownum r__
            //    FROM
            //      (
            //
            //SELECT mp.name,mp.issummary, mp." + TableName + @"_ID, " + tree.GetNodeTableName() + @".parent_id, 
            //        " + tree.GetNodeTableName() + @".seqno 
            //        FROM " + TableName + @" mp
            //      INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"
            //        ON mp." + TableName + @"_ID = " + tree.GetNodeTableName() + @".node_id
            //    WHERE mp.Isactive='Y'  AND  (" + tree.GetNodeTableName() + ".parent_id    =0   OR " + tree.GetNodeTableName() + ".parent_id IS NULL) AND  " + tree.GetNodeTableName() + @".ad_tree_id=" + treeID + @" 
            // ORDER BY  COALESCE(" + tree.GetNodeTableName() + ".Parent_ID, -1),   seqno " + @"
            //
            //)
            //    WHERE rownum < ((" + pageNo + @" * " + pageLength + @") + 1 )
            //    )
            //  WHERE r__ >= (((" + pageNo + @"-1) * " + pageLength + @") + 1)
            //  )
            //ORDER BY  seqno";

            //                qry = qryPaging;
            //                qry = MRole.GetDefault(_ctx).AddAccessSQL(qry, tree.GetNodeTableName(), true, false);
            //            }
            //            else
            //            {
            //                qry = @"SELECT mp.name, mp.issummary,mp." + TableName + @"_ID FROM " + TableName + @" mp 
            //                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"                            
            //                             ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE  mp.Isactive='Y'  AND " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + nodeID + "  ORDER BY COALESCE(" + tree.GetNodeTableName() + ".Parent_ID, -1),   seqno ";


            //                qryPaging = @"SELECT * FROM (
            //                     
            //                         SELECT name, issummary, " + TableName + @"_ID, rownum r__
            //                         FROM
            //                              ( " + qry + @" ) 
            //                            
            //                            WHERE rownum < ((" + pageNo + @" * " + pageLength + @") + 1 )
            //                        )
            //                        WHERE r__ >= (((" + pageNo + @"-1) * " + pageLength + @") + 1)";
            //                qry = MRole.GetDefault(_ctx).AddAccessSQL(qryPaging, tree.GetNodeTableName(), true, false);
            //            }


            if (nodeID == 0)
            {
                int clientID = _ctx.GetAD_Client_ID();
                qryPaging = @"SELECT mp.name,mp.issummary, mp." + TableName + @"_ID, " + tree.GetNodeTableName() + @".parent_id, 
        " + tree.GetNodeTableName() + @".seqno 
        FROM " + TableName + @" mp
      INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"
        ON mp." + TableName + @"_ID = " + tree.GetNodeTableName() + @".node_id
      WHERE mp.Isactive='Y'  AND  (" + tree.GetNodeTableName() + ".parent_id    =0 OR " + tree.GetNodeTableName() + ".parent_id IS NULL)   AND  " + tree.GetNodeTableName() + @".ad_tree_id=" + treeID + @" 
 ORDER BY  COALESCE(" + tree.GetNodeTableName() + ".Parent_ID, -1),   seqno,Upper(mp.Name) ";





                qry = qryPaging;
                qry = MRole.GetDefault(_ctx).AddAccessSQL(qry, tree.GetNodeTableName(), true, false);
            }
            else
            {
                qry = @"SELECT mp.name, mp.issummary,mp." + TableName + @"_ID FROM " + TableName + @" mp 
                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"                            
                             ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE  mp.Isactive='Y'  AND " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + nodeID + "  ORDER BY COALESCE(" + tree.GetNodeTableName() + ".Parent_ID, -1),   seqno,Upper(mp.Name) ";


                qry = MRole.GetDefault(_ctx).AddAccessSQL(qry, tree.GetNodeTableName(), true, false);
            }


            DataSet ds = VIS.DBase.DB.ExecuteDatasetPaging(qry, pageNo, pageLength);
            ds = VAdvantage.DataBase.DB.SetUtcDateTime(ds);

            // DataSet ds = DB.ExecuteDataset(qry, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                var id = TableName + "_ID";
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj.Add(new GetDataTreeNodeSelect()
                    {
                        parentID = Convert.ToInt32(ds.Tables[0].Rows[i][id]),
                        name = System.Net.WebUtility.HtmlDecode(Convert.ToString(ds.Tables[0].Rows[i]["name"])),
                        issummary = Convert.ToString(ds.Tables[0].Rows[i]["issummary"]),
                    });
                }
            }

            return obj;
        }
        //-------------------------------------------

        public List<GetDataTreeNodeSelect> GetDataTreeNodeSelect(Ctx _ctx, int nodeID, int treeID, int pageNo, int pageLength, string searchChildNode, string getTreeNodeChkValue)
        {
            List<GetDataTreeNodeSelect> obj = new List<GetDataTreeNodeSelect>();
            MTree tree = new MTree(_ctx, treeID, null);
            int AD_Table_ID = tree.GetAD_Table_ID();
            string TableName = MTable.GetTableName(_ctx, AD_Table_ID);
            string type = tree.GetTreeType();
            string qry = "";
            string qryPaging = "";

            string clientIDProp = MRole.GetDefault(_ctx).GetClientWhere(true);

            string orgwhere = MRole.GetDefault(_ctx).GetOrgWhere(true);
            clientIDProp = clientIDProp + " AND " + orgwhere;


            if (getTreeNodeChkValue == "true")
            {
                if (searchChildNode != "")
                {
                    if (nodeID == 0)
                    {
                        int clientID = _ctx.GetAD_Client_ID();

                        qryPaging = @"SELECT mp.name,mp.issummary, mp." + TableName + @"_ID, " + tree.GetNodeTableName() + @".parent_id, 
        " + tree.GetNodeTableName() + @".seqno 
        FROM " + TableName + @" mp
      INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"
        ON mp." + TableName + @"_ID = " + tree.GetNodeTableName() + @".node_id
      WHERE mp.Isactive='Y' AND mp.issummary='N' AND  (" + tree.GetNodeTableName() + ".parent_id    =0 OR " + tree.GetNodeTableName() + ".parent_id IS NULL)  AND " + tree.GetNodeTableName() + @".ad_tree_id=" + treeID + @"  AND Upper(mp.name) LIKE upper('%" + searchChildNode + @"%') 
 ORDER BY  COALESCE(" + tree.GetNodeTableName() + ".Parent_ID, -1),   seqno,Upper(mp.Name) ";

                        qry = qryPaging;

                        qry = MRole.GetDefault(_ctx).AddAccessSQL(qry, tree.GetNodeTableName(), true, false);
                    }
                    else
                    {
                        qry = @"SELECT mp.name, mp.issummary,mp." + TableName + @"_ID FROM " + TableName + @" mp 
                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"                            
                             ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE  mp.Isactive='Y' AND mp.issummary='N' AND " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + nodeID + " AND Upper(mp.name) LIKE upper('%" + searchChildNode + "%') ORDER BY  COALESCE(" + tree.GetNodeTableName() + ".Parent_ID, -1),   seqno,Upper(mp.Name) ";

                        qry = MRole.GetDefault(_ctx).AddAccessSQL(qry, tree.GetNodeTableName(), true, false);
                    }
                }
                else
                {

                    if (nodeID == 0)
                    {
                        int clientID = _ctx.GetAD_Client_ID();
                        qryPaging = @"SELECT mp.name,mp.issummary, mp." + TableName + @"_ID, " + tree.GetNodeTableName() + @".parent_id, 
        " + tree.GetNodeTableName() + @".seqno 
        FROM " + TableName + @" mp
      INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"
        ON mp." + TableName + @"_ID = " + tree.GetNodeTableName() + @".node_id
      WHERE mp.Isactive='Y' AND mp.issummary='N' AND  (" + tree.GetNodeTableName() + ".parent_id    =0 OR " + tree.GetNodeTableName() + ".parent_id IS NULL)     AND  " + tree.GetNodeTableName() + @".ad_tree_id=" + treeID + @" 
 ORDER BY COALESCE(" + tree.GetNodeTableName() + ".Parent_ID, -1),   seqno,Upper(mp.Name) ";

                        qry = qryPaging;
                        qry = MRole.GetDefault(_ctx).AddAccessSQL(qry, tree.GetNodeTableName(), true, false);
                    }
                    else
                    {
                        qry = @"SELECT mp.name, mp.issummary,mp." + TableName + @"_ID FROM " + TableName + @" mp 
                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"                            
                             ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE mp.Isactive='Y' AND mp.issummary='N' AND  " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + nodeID + "  ORDER BY  COALESCE(" + tree.GetNodeTableName() + ".Parent_ID, -1),   seqno,Upper(mp.Name) ";

                        qry = MRole.GetDefault(_ctx).AddAccessSQL(qry, tree.GetNodeTableName(), true, false);
                    }

                }
            }
            else
            {

                if (searchChildNode != "")
                {

                    if (nodeID == 0)
                    {
                        int clientID = _ctx.GetAD_Client_ID();
                        qryPaging = @"SELECT mp.name,mp.issummary, mp." + TableName + @"_ID, " + tree.GetNodeTableName() + @".parent_id, 
        " + tree.GetNodeTableName() + @".seqno 
        FROM " + TableName + @" mp
      INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"
        ON mp." + TableName + @"_ID = " + tree.GetNodeTableName() + @".node_id
      WHERE mp.Isactive='Y' AND mp.issummary='N' AND Upper(mp.name) LIKE upper('%" + searchChildNode + @"%')   AND  (" + tree.GetNodeTableName() + ".parent_id    =0 OR " + tree.GetNodeTableName() + ".parent_id IS NULL)    AND  " + tree.GetNodeTableName() + @".ad_tree_id=" + treeID + @" 
 ORDER BY   COALESCE(" + tree.GetNodeTableName() + ".Parent_ID, -1),   seqno,Upper(mp.Name) ";


                        qry = qryPaging;
                        qry = MRole.GetDefault(_ctx).AddAccessSQL(qry, tree.GetNodeTableName(), true, false);
                    }
                    else
                    {
                        qry = @"SELECT mp.name, mp.issummary,mp." + TableName + @"_ID FROM " + TableName + @" mp 
                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"                            
                             ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE  mp.Isactive='Y' AND mp.issummary='N'  AND Upper(mp.name) LIKE upper('%" + searchChildNode + @"%')     AND " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + nodeID + "  ORDER BY COALESCE(" + tree.GetNodeTableName() + ".Parent_ID, -1),   seqno,Upper(mp.Name) ";



                        qry = MRole.GetDefault(_ctx).AddAccessSQL(qry, tree.GetNodeTableName(), true, false);
                    }
                }
                else
                {
                    if (nodeID == 0)
                    {
                        int clientID = _ctx.GetAD_Client_ID();
                        qryPaging = @"SELECT mp.name,mp.issummary, mp." + TableName + @"_ID, " + tree.GetNodeTableName() + @".parent_id, 
        " + tree.GetNodeTableName() + @".seqno 
        FROM " + TableName + @" mp
      INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"
        ON mp." + TableName + @"_ID = " + tree.GetNodeTableName() + @".node_id
      WHERE mp.Isactive='Y' AND mp.issummary='N' AND  (" + tree.GetNodeTableName() + ".parent_id    =0 OR " + tree.GetNodeTableName() + ".parent_id IS NULL)   AND  " + tree.GetNodeTableName() + @".ad_tree_id=" + treeID + @" 
 ORDER BY  COALESCE(" + tree.GetNodeTableName() + ".Parent_ID, -1),   seqno,Upper(mp.Name) ";




                        qry = qryPaging;
                        qry = MRole.GetDefault(_ctx).AddAccessSQL(qry, tree.GetNodeTableName(), true, false);
                    }
                    else
                    {
                        qry = @"SELECT mp.name, mp.issummary,mp." + TableName + @"_ID FROM " + TableName + @" mp 
                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"                            
                             ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE  mp.Isactive='Y' AND mp.issummary='N' AND " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + nodeID + "  ORDER BY COALESCE(" + tree.GetNodeTableName() + ".Parent_ID, -1),   seqno,Upper(mp.Name) ";


                        qry = MRole.GetDefault(_ctx).AddAccessSQL(qry, tree.GetNodeTableName(), true, false);
                    }
                }
            }



            DataSet ds = VIS.DBase.DB.ExecuteDatasetPaging(qry, pageNo, pageLength);
            ds = VAdvantage.DataBase.DB.SetUtcDateTime(ds);


            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                var id = TableName + "_ID";
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj.Add(new GetDataTreeNodeSelect()
                    {
                        parentID = Convert.ToInt32(ds.Tables[0].Rows[i][id]),
                        name = System.Net.WebUtility.HtmlDecode(Convert.ToString(ds.Tables[0].Rows[i]["name"])),
                        issummary = Convert.ToString(ds.Tables[0].Rows[i]["issummary"]),
                    });
                }
            }

            return obj;
        }







        public string GetNodePath(Ctx ctx, int node_ID, int TreeID)
        {
            MTree tree = new MTree(ctx, TreeID, null);

            object otput = "";

            string tableName = MTable.GetTableName(ctx, tree.GetAD_Table_ID());
            string sql = "select gettreenodepaths(" + node_ID + ",'" + ctx.GetAD_Language() + "','" + tree.GetNodeTableName() + "','" + tableName + "','" + tableName + "_ID', " + TreeID + ") from dual";

            otput = DB.ExecuteScalar(sql, null, null);


            if (otput.ToString().IndexOf("\\") > 0)
            {
                otput = otput.ToString().Substring(0, otput.ToString().LastIndexOf("\\"));
            }

            if (otput.ToString().IndexOf("\\") > 0)
            {
                otput = otput.ToString().Substring(0, otput.ToString().LastIndexOf("\\"));
                otput = tree.GetName() + " \\ " + otput;
            }
            else
            {
                otput = tree.GetName();
            }

            return System.Net.WebUtility.HtmlDecode(otput.ToString());

        }

        /// <summary>
        /// When Tree Drag Drop Properties True
        /// </summary>
        /// <param name="_ctx"></param>
        /// <param name="treeID"></param>
        /// <param name="NodeID"></param>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public string SaveTreeDragDrop(Ctx _ctx, int treeID, int NodeID, int ParentID)
        {
            MTree tree = new MTree(_ctx, treeID, null);
            int AD_Table_ID = tree.GetAD_Table_ID();
            string TableName = MTable.GetTableName(_ctx, AD_Table_ID);
            string type = tree.GetTreeType();
            tree.GetNodeTableName();

            if (NodeID == ParentID)
            {
                return Msg.GetMsg(_ctx, "VIS_DataNotSave");
            }


            DB.ExecuteQuery("UPDATE " + tree.GetNodeTableName() + "  SET Updated=Sysdate, seqNo=seqNo+1 WHERE Parent_ID=" + ParentID + " AND AD_Tree_ID=" + treeID);

            if (type == "PR")
            {
                MTreeNodePR obj = MTreeNodePR.Get(tree, NodeID);
                obj.SetParent_ID(ParentID);
                obj.SetSeqNo(0);
                if (!obj.Save())
                {
                    return Msg.GetMsg(_ctx, "VIS_DataNotSave");
                    //return "DataNotSave";
                }
            }
            else if (type == "MM")
            {
                MTreeNodeMM obj = MTreeNodeMM.Get(tree, NodeID);
                obj.SetParent_ID(ParentID);
                obj.SetSeqNo(0);
                if (!obj.Save())
                {
                    return Msg.GetMsg(_ctx, "VIS_DataNotSave");
                    //return "DataNotSave";
                }
            }
            else if (type == "BP")
            {
                MTreeNodeBP obj = MTreeNodeBP.Get(tree, NodeID);
                obj.SetParent_ID(ParentID);
                obj.SetSeqNo(0);
                if (!obj.Save())
                {
                    //return "DataNotSave";
                    return Msg.GetMsg(_ctx, "VIS_DataNotSave");
                }
            }
            else
            {
                MTreeNode obj = MTreeNode.Get(tree, NodeID);
                obj.SetParent_ID(ParentID);
                obj.SetSeqNo(0);
                if (!obj.Save())
                {
                    //return "DataNotSave";
                    return Msg.GetMsg(_ctx, "VIS_DataNotSave");
                }
            }

            return Msg.GetMsg(_ctx, "VIS_DataSave");
            //return "DataSave";
        }


        public String DeleteNodeFromTree(Ctx _ctx, int nodeid, int treeID, string unlinkchild, string menuArray)
        {
            menuArrays = menuArray;
            MTree tree = new MTree(_ctx, treeID, null);
            int AD_Table_ID = tree.GetAD_Table_ID();
            string TableName = MTable.GetTableName(_ctx, AD_Table_ID);
            string type = tree.GetTreeType();
            string nodeTreeTableName = tree.GetNodeTableName();
            string _result = "";
            
             bindornot = "true";            
            if (TableName == "AD_Menu")
            {
                string rolCheck = @"SELECT count(*) FROM AD_Role WHERE ad_tree_menu_id=" + treeID;
                int checkCount = Convert.ToInt32(DB.ExecuteScalar(rolCheck));
                if (checkCount > 0)
                {
                    bindornot = "false";                   
                }
                else
                {
                    string tenantCheck = @"SELECT count(*) FROM AD_ClientInfo WHERE ad_tree_menu_id=" + treeID;
                    int checktenant = Convert.ToInt32(DB.ExecuteScalar(tenantCheck));
                    if (checktenant > 0)
                    {
                        bindornot = "false";                       
                    }
                }
            }












            if (unlinkchild == "false")
            {
                string getfirstParent = "select parent_ID  from " + nodeTreeTableName + " WHERE NODE_ID=" + nodeid + " AND AD_Tree_ID=" + treeID + " AND IsActive='Y'";
                DataSet ds = DB.ExecuteDataset(getfirstParent, null, null);
                if (type == "PR")
                {
                    MTreeNodePR obj = MTreeNodePR.Get(tree, nodeid);
                    if (!obj.Delete(true))
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        _result = pp.ToString();
                    }
                }
                else if (type == "MM")
                {
                    MTreeNodeMM obj = MTreeNodeMM.Get(tree, nodeid);
                    if (!obj.Delete(true))
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        _result = pp.ToString();
                    }

                }
                else if (type == "BP")
                {
                    MTreeNodeBP obj = MTreeNodeBP.Get(tree, nodeid);
                    if (!obj.Delete(true))
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        _result = pp.ToString();
                    }
                }
                else
                {
                    MTreeNode obj = MTreeNode.Get(tree, nodeid);
                    if (!obj.Delete(true))
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        _result = pp.ToString();
                    }
                }



                string sqls = "UPDATE " + nodeTreeTableName + " SET Parent_ID=" + Convert.ToString(ds.Tables[0].Rows[0]["parent_ID"]) + " WHERE Parent_ID=" + nodeid + " AND AD_Tree_ID=" + treeID;

                //string sqls = "UPDATE " + nodeTreeTableName + " SET Parent_ID=0 WHERE Parent_ID=" + nodeid + " AND AD_Tree_ID=" + treeID;
                DB.ExecuteQuery(sqls);
            }
            else
            {
                //                string qry = @"SELECT " + tree.GetNodeTableName() + @".node_id AS node_id ,mp.issummary FROM " + TableName + @" mp 
                //                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"                            
                //                             ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + nodeid + "  ORDER BY COALESCE(" + tree.GetNodeTableName() + @".parent_id," + -1 + @"), " + tree.GetNodeTableName() + ".seqno";

                //                qry = MRole.GetDefault(_ctx).AddAccessSQL(qry, tree.GetNodeTableName(), true, false);
                //                DataSet ds = DB.ExecuteDataset(qry, null, null);

                //                string arrString = "";
                //                if (ds != null && ds.Tables[0].Rows.Count > 0)
                //                {
                //                    var id = TableName + "_ID";
                //                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                //                    {
                //                        arrString += Convert.ToString(ds.Tables[0].Rows[i]["node_id"]);
                //                        arrString += ",";
                //                    }
                //                    arrString.LastIndexOf(",");
                //                    arrString = arrString.Substring(0, arrString.LastIndexOf(","));
                //                }

                //                string[] stringArray = arrString.Split(',');

                //                if (stringArray.Length > 1)
                //                {

                //                    for (int i = 0; i < stringArray.Length; i++)
                //                    {
                //                        if (type == "PR")
                //                        {
                //                            MTreeNodePR obj = MTreeNodePR.Get(tree, Convert.ToInt32(stringArray[i]));
                //                            if (!obj.Delete(true))
                //                            {
                //                                ValueNamePair pp = VLogger.RetrieveError();
                //                                _result = pp.ToString();
                //                            }
                //                        }
                //                        else if (type == "MM")
                //                        {
                //                            MTreeNodeMM obj = MTreeNodeMM.Get(tree, Convert.ToInt32(stringArray[i]));
                //                            if (!obj.Delete(true))
                //                            {
                //                                ValueNamePair pp = VLogger.RetrieveError();
                //                                _result = pp.ToString();
                //                            }

                //                        }
                //                        else if (type == "BP")
                //                        {
                //                            MTreeNodeBP obj = MTreeNodeBP.Get(tree, Convert.ToInt32(stringArray[i]));
                //                            if (!obj.Delete(true))
                //                            {
                //                                ValueNamePair pp = VLogger.RetrieveError();
                //                                _result = pp.ToString();
                //                            }

                //                        }
                //                        else
                //                        {
                //                            MTreeNode obj = MTreeNode.Get(tree, Convert.ToInt32(stringArray[i]));
                //                            if (!obj.Delete(true))
                //                            {
                //                                ValueNamePair pp = VLogger.RetrieveError();
                //                                _result = pp.ToString();
                //                            }
                //                        }
                //                    }
                //                }

                deleteChildNodes(tree, TableName, treeID, nodeid);

                if (type == "PR")
                {
                    MTreeNodePR obj = MTreeNodePR.Get(tree, nodeid);
                    if (obj != null)
                    {
                        if (!obj.Delete(true))
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            _result = pp.ToString();
                        }
                    }
                }
                else if (type == "MM")
                {
                    MTreeNodeMM obj = MTreeNodeMM.Get(tree, nodeid);
                    if (obj != null)
                    {
                        if (!obj.Delete(true))
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            _result = pp.ToString();
                        }
                    }
                }
                else if (type == "BP")
                {
                    MTreeNodeBP obj = MTreeNodeBP.Get(tree, nodeid);
                    if (obj != null)
                    {
                        if (!obj.Delete(true))
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            _result = pp.ToString();
                        }
                    }
                }
                else
                {
                    MTreeNode obj = MTreeNode.Get(tree, nodeid);
                    if (obj != null)
                    {
                        if (!obj.Delete(true))
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            _result = pp.ToString();
                        }
                    }
                }
            }

            return _result;
        }

        private void deleteChildNodes(MTree tree, string TableName, int treeID, int nodeID)        
        {
            // bindornot is false because menu bind with rol window or tenant window;
            if (bindornot == "false")
            {
                string qrys = "DELETE FROM " + tree.GetNodeTableName() + " WHERE AD_Tree_ID=" + treeID + " AND Parent_ID=" + nodeID + @" AND NODE_ID IN (SELECT " + tree.GetNodeTableName() + @".node_id AS node_id  FROM " + TableName + @" mp 
                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"                            
                             ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + nodeID + " AND mp.issummary='N') AND NODE_ID NOT IN (" + menuArrays + ")";

                DB.ExecuteQuery(qrys);

                qrys = @"SELECT " + tree.GetNodeTableName() + @".node_id AS node_id  FROM " + TableName + @" mp 
                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"                            
                             ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + nodeID + " AND mp.issummary='Y'";

                qrys = MRole.GetDefault(_ctx).AddAccessSQL(qrys, tree.GetNodeTableName(), true, false);
                DataSet ds = DB.ExecuteDataset(qrys, null, null);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        deleteChildNodes(tree, TableName, treeID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["node_ID"]));
                        if (i == ds.Tables[0].Rows.Count - 1)
                        {
                            qrys = "DELETE FROM " + tree.GetNodeTableName() + " WHERE AD_Tree_ID=" + treeID + " AND Parent_ID=" + nodeID + @" AND NODE_ID IN (SELECT " + tree.GetNodeTableName() + @".node_id AS node_id  FROM " + TableName + @" mp 
                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"                            
                             ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + nodeID + " AND mp.issummary='Y') AND NODE_ID NOT IN (" + menuArrays + ")";
                            DB.ExecuteQuery(qrys);
                        }
                    }
                }
            }

            //----------------------------------
            else if (bindornot == "true")
            {
                string qry = "DELETE FROM " + tree.GetNodeTableName() + " WHERE AD_Tree_ID=" + treeID + " AND Parent_ID=" + nodeID + @" AND NODE_ID IN (SELECT " + tree.GetNodeTableName() + @".node_id AS node_id  FROM " + TableName + @" mp 
                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"                            
                             ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + nodeID + " AND mp.issummary='N')";

                DB.ExecuteQuery(qry);

                qry = @"SELECT " + tree.GetNodeTableName() + @".node_id AS node_id  FROM " + TableName + @" mp 
                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"                            
                             ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + nodeID + " AND mp.issummary='Y'";

                qry = MRole.GetDefault(_ctx).AddAccessSQL(qry, tree.GetNodeTableName(), true, false);
                DataSet ds = DB.ExecuteDataset(qry, null, null);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        deleteChildNodes(tree, TableName, treeID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["node_ID"]));
                        if (i == ds.Tables[0].Rows.Count - 1)
                        {
                            qry = "DELETE FROM " + tree.GetNodeTableName() + " WHERE AD_Tree_ID=" + treeID + " AND Parent_ID=" + nodeID + @" AND NODE_ID IN (SELECT " + tree.GetNodeTableName() + @".node_id AS node_id  FROM " + TableName + @" mp 
                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"                            
                             ON mp." + TableName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + nodeID + " AND mp.issummary='Y')";
                            DB.ExecuteQuery(qry);
                        }
                    }
                }
            }
        }


        //if (ds != null && ds.Tables[0].Rows.Count > 0)
        //{
        //    //var id = TableName + "_ID";
        //    //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //    //{
        //    //    arrString += Convert.ToString(ds.Tables[0].Rows[i]["node_id"]);
        //    //    arrString += ",";
        //    //}
        //    //arrString.LastIndexOf(",");
        //    //arrString = arrString.Substring(0, arrString.LastIndexOf(","));
        //}

        // string[] stringArray = arrString.Split(',');

        string[] strArray;
        StringBuilder arrStringss = new StringBuilder();

        public string SelectAllChildNodes(Ctx _ctx, string TableName, int treeID, string nodeID)
        {
            string[] stringArray = nodeID.Split(',');
            for (int i = 0; i < stringArray.Length; i++)
            {
                SelectAllChildNodes(_ctx, TableName, treeID, Convert.ToInt32(stringArray[i]));
            }

            return arrStringss.ToString();
        }

        private string SelectAllChildNodes(Ctx _ctx, string TableName, int treeID, int nodeID)
        {
            MTree tree = new MTree(_ctx, treeID, null);
            int AD_Table_ID = tree.GetAD_Table_ID();
            string tbName = MTable.GetTableName(_ctx, AD_Table_ID);

            DataSet dst = new DataSet();
            DataSet dsts = new DataSet();

            string qry = "SELECT Node_ID FROM " + tree.GetNodeTableName() + " WHERE AD_Tree_ID=" + treeID + " AND Parent_ID=" + nodeID + @" AND NODE_ID IN (SELECT " + tree.GetNodeTableName() + @".node_id AS node_id  FROM " + tbName + @" mp 
                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"                            
                             ON mp." + tbName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + nodeID + " AND mp.issummary='N')";

            dst = DB.ExecuteDataset(qry, null, null);

            if (arrStringss.Length > 0 && !arrStringss.ToString().EndsWith(","))
            {
                arrStringss.Append(",");
            }
            arrStringss.Append(nodeID);

            if (dst != null && dst.Tables[0].Rows.Count > 0)
            {
                if (arrStringss.Length > 0 && !arrStringss.ToString().EndsWith(","))
                {
                    arrStringss.Append(",");
                }
                for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                {
                    arrStringss.Append(Convert.ToString(dst.Tables[0].Rows[i]["Node_ID"]));
                    if (i < dst.Tables[0].Rows.Count - 1)
                    {
                        arrStringss.Append(",");
                    }

                }

            }




            qry = @"SELECT " + tree.GetNodeTableName() + @".node_id AS node_id  FROM " + tbName + @" mp 
                           INNER JOIN " + tree.GetNodeTableName() + " " + tree.GetNodeTableName() + @"                            
                             ON mp." + tbName + @"_ID=" + tree.GetNodeTableName() + ".node_id WHERE " + tree.GetNodeTableName() + ".ad_tree_id=" + treeID + @" AND " + tree.GetNodeTableName() + ".parent_id=" + nodeID + " AND mp.issummary='Y'";

            qry = MRole.GetDefault(_ctx).AddAccessSQL(qry, tree.GetNodeTableName(), true, false);
            DataSet ds = DB.ExecuteDataset(qry, null, null);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                if (arrStringss.Length > 0 && !arrStringss.ToString().EndsWith(","))
                {
                    arrStringss.Append(",");
                }
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //arrStringss.Append(Convert.ToString(ds.Tables[0].Rows[i]["node_ID"]));
                    //if (i < dst.Tables[0].Rows.Count - 1)
                    //{
                    //    arrStringss.Append(",");
                    //}
                    SelectAllChildNodes(_ctx, TableName, treeID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["node_ID"]));
                }
            }

            return arrStringss.ToString();
        }






        public String DeleteNodeFromBottom(Ctx _ctx, string nodeid, int treeID, string menuArray)
        {
            menuArrays = menuArray;
            MTree tree = new MTree(_ctx, treeID, null);
            int AD_Table_ID = tree.GetAD_Table_ID();
            string TableName = MTable.GetTableName(_ctx, AD_Table_ID);
            string type = tree.GetTreeType();
            tree.GetNodeTableName();

            string[] stringArray = nodeid.Split(',');
            string[] menuArrayID = menuArray.Split(',');



             bindornot = "true";
            string menu_id = "";
            if (TableName == "AD_Menu")
            {
                string rolCheck = @"SELECT count(*) FROM AD_Role WHERE ad_tree_menu_id=" + treeID;
                int checkCount = Convert.ToInt32(DB.ExecuteScalar(rolCheck));
                if (checkCount > 0)
                {
                    bindornot = "false";                    
                }
                else
                {
                    string tenantCheck = @"SELECT count(*) FROM AD_ClientInfo WHERE ad_tree_menu_id=" + treeID;
                    int checktenant = Convert.ToInt32(DB.ExecuteScalar(tenantCheck));
                    if (checktenant > 0)
                    {
                        bindornot = "false";                        
                    }
                }
            }




            //deleteChildNodes(tree, TableName, treeID, Convert.ToInt32(nodeid));

            string _result = "";
            for (int i = 0; i < stringArray.Length; i++)
            {
                if (bindornot == "false")
                {
                    for (int p = 0; p < menuArrayID.Length; p++)
                    {
                        if (Convert.ToInt32(stringArray[i]) == Convert.ToInt32(menuArrayID[p]))
                        {
                            continue;
                        }
                    }
                }
                if (type == "PR")
                {
                    deleteChildNodes(tree, TableName, treeID, Convert.ToInt32(stringArray[i]));

                    MTreeNodePR obj = MTreeNodePR.Get(tree, Convert.ToInt32(stringArray[i]));
                    if (!obj.Delete(true))
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        _result = pp.ToString();
                    }
                }
                else if (type == "MM")
                {
                    deleteChildNodes(tree, TableName, treeID, Convert.ToInt32(stringArray[i]));
                    MTreeNodeMM obj = MTreeNodeMM.Get(tree, Convert.ToInt32(stringArray[i]));
                    if (!obj.Delete(true))
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        _result = pp.ToString();
                    }

                }
                else if (type == "BP")
                {
                   deleteChildNodes(tree, TableName, treeID, Convert.ToInt32(stringArray[i]));

                    MTreeNodeBP obj = MTreeNodeBP.Get(tree, Convert.ToInt32(stringArray[i]));
                    if (!obj.Delete(true))
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        _result = pp.ToString();
                    }

                }
                else
                {
                    deleteChildNodes(tree, TableName, treeID, Convert.ToInt32(stringArray[i]));

                    MTreeNode obj = MTreeNode.Get(tree, Convert.ToInt32(stringArray[i]));
                    if (!obj.Delete(true))
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        _result = pp.ToString();
                    }
                }




            }
            return _result;
        }


        public String TreeTableName(Ctx _ctx, int treeID)
        {
            MTree tree = new MTree(_ctx, treeID, null);
            int AD_Table_ID = tree.GetAD_Table_ID();
            string TableName = MTable.GetTableName(_ctx, AD_Table_ID);
            string type = tree.GetTreeType();
            string tblName = tree.GetNodeTableName();
            return tblName;
        }

        public string UpdateItemSeqNo(Ctx _ctx, int treeID, string itemsid, int ParentID)
        {
            MTree tree = new MTree(_ctx, treeID, null);
            int AD_Table_ID = tree.GetAD_Table_ID();
            string TableName = MTable.GetTableName(_ctx, AD_Table_ID);
            string type = tree.GetTreeType();
            string treeTblName = tree.GetNodeTableName();
            string[] stringArray = itemsid.Split(',');


            //if (ParentID == 0)
            //{
            //    DB.ExecuteQuery("UPDATE " + tree.GetNodeTableName() + "  SET Updated=Sysdate, seqNo=seqNo+" + (stringArray.Length + 1) + " WHERE (Parent_ID=" + ParentID + " OR Parent_ID IS NULL) AND AD_Tree_ID=" + treeID);
            //}
            //else
            //{
            //    DB.ExecuteQuery("UPDATE " + tree.GetNodeTableName() + "  SET Updated=Sysdate, seqNo=seqNo+" + (stringArray.Length + 1) + " WHERE Parent_ID=" + ParentID + " AND AD_Tree_ID=" + treeID);
            //}



            for (int i = 0; i < stringArray.Length; i++)
            {
                string parentzero = "";
                string sql = "UPDATE ";

                if (ParentID == 0)
                {
                    //parentzero=  " WHERE AD_Tree_ID=" + treeID + " AND (Parent_ID=" + ParentID + " or Parent_ID is null)  AND Node_ID=" + Convert.ToInt32(stringArray[i]);

                    parentzero = " WHERE AD_Tree_ID=" + treeID + " AND (Parent_ID=" + ParentID + " or Parent_ID is null)  AND Node_ID=" + Convert.ToInt32(stringArray[i]);

                    sql += treeTblName + " SET Parent_ID=0, SeqNo=" + i + ", Updated=SysDate" + parentzero;
                }
                else
                {
                    parentzero = " WHERE AD_Tree_ID=" + treeID + " AND Parent_ID=" + ParentID + "  AND Node_ID=" + Convert.ToInt32(stringArray[i]);

                    sql += treeTblName + " SET  SeqNo=" + i + ", Updated=SysDate" + parentzero;
                }


                DB.ExecuteQuery(sql);
            }
            return Msg.GetMsg(_ctx, "ItemsUpdate");
        }

        ////rolCheck = MRole.GetDefault(_ctx).AddAccessSQL(rolCheck, "AD_Role", true, false);
        public String RemoveLinkedItemFromTree(Ctx _ctx, int treeID, string menuId)
        {
            MTree tree = new MTree(_ctx, treeID, null);
            int AD_Table_ID = tree.GetAD_Table_ID();
            string TableName = MTable.GetTableName(_ctx, AD_Table_ID);
            string treeTblName = tree.GetNodeTableName();

             bindornot = "true";

            string menu_id = "";


            if (TableName == "AD_Menu")
            {
                string rolCheck = @"SELECT count(*) FROM AD_Role WHERE ad_tree_menu_id=" + treeID;
                int checkCount = Convert.ToInt32(DB.ExecuteScalar(rolCheck));
                if (checkCount > 0)
                {
                    bindornot = "false";
                    //return "menubind";
                }
                else
                {
                    string tenantCheck = @"SELECT count(*) FROM AD_ClientInfo WHERE ad_tree_menu_id=" + treeID;
                    int checktenant = Convert.ToInt32(DB.ExecuteScalar(tenantCheck));
                    if (checktenant > 0)
                    {
                        bindornot = "false";
                        //  return "menubind";
                    }
                }
            }

            string getvalue = @"SELECT count(*) as count FROM " + treeTblName + @"
                                WHERE AD_Tree_ID =" + treeID;
            getvalue = MRole.GetDefault(_ctx).AddAccessSQL(getvalue, tree.GetNodeTableName(), true, false);
            int counts = Convert.ToInt32(DB.ExecuteScalar(getvalue));



            if (bindornot == "false")
            {
                string delquery = @"DELETE FROM " + treeTblName + @"
                                WHERE AD_Tree_ID =" + treeID + " AND node_id NOT IN (" + menuId + ")";

                delquery = MRole.GetDefault(_ctx).AddAccessSQL(delquery, tree.GetNodeTableName(), true, false);

                DB.ExecuteQuery(delquery);

      string maxSeq = "SELECT MAX(seqno) FROM " + treeTblName + " WHERE AD_Tree_ID=" + treeID;
      int seq = Convert.ToInt32(DB.ExecuteScalar(maxSeq));         
      seq += 1;

      for (int a = 0; a < menuId.Split(',').Length; a++)
      {
          seq += a;
          string increaseSqe = "update " + treeTblName + " set seqno=" + seq + ",Updated=Sysdate,parent_ID=0 WHERE AD_Tree_ID=" + treeID + " AND node_id IN (" + menuId[a] + ")";
          DB.ExecuteQuery(increaseSqe, null, null);
      }

      

            }
            else
            {
                string delquery = @"DELETE FROM " + treeTblName + @"
                                WHERE AD_Tree_ID =" + treeID;
                delquery = MRole.GetDefault(_ctx).AddAccessSQL(delquery, tree.GetNodeTableName(), true, false);

                DB.ExecuteQuery(delquery);
            }

            if (counts > 0)
            {
                return "count";
            }

            return "";
        }



    }

    public class AllMenuData
    {
        public int ID { get; set; }
        public string Action { get; set; }
        public string Name { get; set; }
        public string description { get; set; }
        public string issummary { get; set; }
        public int NodeID { get; set; }
        public string color { get; set; }
        public bool disabled { get; set; }
        public string classopacity { get; set; }
        public string tbname { get; set; }
        public string unlinkClass { get; set; }
        //        public string IsSummary { get; set; }

    }

    public class TreeDataForCombo
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string TreeType { get; set; }
        public string IsAllNodes { get; set; }
        public string tbname { get; set; }
    }

    public class SetTree
    {
        public int id { get; set; }
        public string text { get; set; }
        public int AD_Tree_ID { get; set; }
        public string TableName { get; set; }
        public bool IsSummary { get; set; }
        public int NodeID { get; set; }
        public bool IsActive { get; set; }
        public string ImageSource { get; set; }
        public string imageIndegator { get; set; }
        public int ParentID { get; set; }
        public int TreeParentID { get; set; }
        public string cmbname { get; set; }
        public string getparentnode { get; set; }
        public List<SetTree> items { get; set; }
    }
    public class Tree
    {
        public List<SetTree> settree { get; set; }
        public List<TreeDataForCombo> reeDataForCombo { get; set; }
    }

    public class GetDataTreeNodeSelect
    {
        public int parentID { get; set; }
        public string name { get; set; }
        public string issummary { get; set; }
    }

}