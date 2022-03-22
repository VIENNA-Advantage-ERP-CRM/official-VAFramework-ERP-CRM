/********************************************************
 * Module Name    : Tree
 * Purpose        : 
 * Class Used     : X_AD_Tree
 * Chronological Development
 * Veena Pandey     05-May-2009
 * Harwinder    :   13 june 2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.SqlExec;
using VAdvantage.Classes;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAModelAD.Model;
//using VAdvantage.StartMenu;


namespace VAdvantage.Model
{
    public class MTree : X_AD_Tree
    {
        #region "Mtree TreeNode

        /**	Cache						*/
        private static CCache<int, MTree> s_cache = new CCache<int, MTree>("AD_Tree", 10);

        /** All Table Names with tree		*/
        private static List<String> s_TableNames = null;
        /** All Table IDs with tree			*/
        private static List<int> s_TableIDs = null;
        /** U1 Table IDs					*/
        private static List<int> s_TableIDs_U1 = null;
        /** U2 Table IDs					*/
        private static List<int> s_TableIDs_U2 = null;
        /** U3 Table IDs					*/
        private static List<int> s_TableIDs_U3 = null;
        /** U4 Table IDs					*/
        private static List<int> s_TableIDs_U4 = null;
        /** Is Tree editable    	*/
        private bool m_editable = false;
        /** Root Node                   */
        private VTreeNode m_root = null;
        /** Buffer while loading tree   */
        private List<VTreeNode> _buffer = new List<VTreeNode>();
        /** Prepared Statement for Node Details */
        //private RowSet m_nodeRowSet;
        private DataTable dt;

        /** The tree is displayed on the Java Client (i.e. not web)	*/
        private bool m_clientTree = true;
        private List<VTreeNode> mnuNodes = new List<VTreeNode>(); //menu nodes
        private List<VTreeNode> barNodes = new List<VTreeNode>(); //favourite nodes
        int maxLevel = 0;
        int currentNodeID = 0;
        private bool onDemand = false;
        private int AD_Tab_ID = 0;
        private int windowNo = 0;

        //	Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MTree).FullName);

        /** Tree Type Array		*/
        private static String[] TREETYPES = new String[] {
            TREETYPE_Activity,
            TREETYPE_BoM,
            TREETYPE_BPartner,
            TREETYPE_CMContainer,
            TREETYPE_CMMedia,
            TREETYPE_CMContainerStage,
            TREETYPE_CMTemplate,
            TREETYPE_ElementValue,
            TREETYPE_Campaign,
            TREETYPE_Menu,
            TREETYPE_Organization,
            TREETYPE_ProductCategory,
            TREETYPE_Project,
            TREETYPE_Product,
            TREETYPE_SalesRegion,
            TREETYPE_User1,
            TREETYPE_User2,
            TREETYPE_User3,
            TREETYPE_User4,
            TREETYPE_Other
        };
        /** Table ID Array				*/
        private static int[] TABLEIDS = new int[] {
            X_C_Activity.Table_ID,
            X_M_BOM.Table_ID,
            X_C_BPartner.Table_ID,
            X_CM_Container.Table_ID,
            X_CM_Media.Table_ID,
            X_CM_CStage.Table_ID,
            X_CM_Template.Table_ID,
            X_C_ElementValue.Table_ID,
            X_C_Campaign.Table_ID,
            X_AD_Menu.Table_ID,
            X_AD_Org.Table_ID,
            X_M_Product_Category.Table_ID,
            X_C_Project.Table_ID,
            X_M_Product.Table_ID,
            X_C_SalesRegion.Table_ID,
            0,0,0,0,0
        };

        /// <summary>
        /// Default Constructor. Need to call loadNodes explicitly
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Tree_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MTree(Ctx ctx, int AD_Tree_ID, Trx trxName)
            : base(ctx, AD_Tree_ID, trxName)
        {
            if (AD_Tree_ID == 0)
            {
                //	setName (null);
                //	setTreeType (null);
                SetIsAllNodes(true);	//	complete tree
                SetIsDefault(false);
            }
        }

        /// <summary>
        /// Construct & Load Tree
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Tree_ID">The tree to build</param>
        /// <param name="editable">True, if tree can be modified - includes inactive and empty summary nodes</param>
        /// <param name="clientTree">the tree is displayed on the java client (not on web)</param>
        /// <param name="trxName">transaction</param>
        public MTree(Ctx ctx, int AD_Tree_ID, bool editable, bool clientTree, Trx trxName, bool onDemand, int AD_Tab_ID, int windowNO)
            : this(ctx, AD_Tree_ID, trxName)
        {
            this.AD_Tab_ID = AD_Tab_ID;
            this.onDemand = onDemand;
            this.windowNo = windowNO;
            m_editable = editable;
            int AD_User_ID = ctx.GetAD_User_ID();
            m_clientTree = clientTree;
            log.Info("AD_Tree_ID=" + AD_Tree_ID + ", AD_User_ID=" + AD_User_ID
                + ", Editable=" + editable + ", OnClient=" + clientTree);
            //
            LoadNodes(AD_User_ID);
        }

        /// <summary>
        /// Construct & Load Tree
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Tree_ID">The tree to build</param>
        /// <param name="editable">True, if tree can be modified - includes inactive and empty summary nodes</param>
        /// <param name="clientTree">the tree is displayed on the java client (not on web)</param>
        /// <param name="trxName">transaction</param>
        public MTree(Ctx ctx, int AD_Tree_ID, bool editable, bool clientTree, Trx trxName, bool onDemand)
            : this(ctx, AD_Tree_ID, trxName)
        {
            this.AD_Tab_ID = AD_Tab_ID;
            this.onDemand = onDemand;
            m_editable = editable;
            int AD_User_ID = ctx.GetAD_User_ID();
            m_clientTree = clientTree;
            log.Info("AD_Tree_ID=" + AD_Tree_ID + ", AD_User_ID=" + AD_User_ID
                + ", Editable=" + editable + ", OnClient=" + clientTree);
            //
            LoadNodes(AD_User_ID);
        }


        /// <summary>
        /// Construct & Load Tree
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Tree_ID">The tree to build</param>
        /// <param name="editable">True, if tree can be modified - includes inactive and empty summary nodes</param>
        /// <param name="clientTree">the tree is displayed on the java client (not on web)</param>
        /// <param name="trxName">transaction</param>
        public MTree(Ctx ctx, int AD_Tree_ID, bool editable, bool clientTree, Trx trxName, int nodeid)
            : this(ctx, AD_Tree_ID, trxName)
        {
            this.AD_Tab_ID = AD_Tab_ID;
            m_editable = editable;
            currentNodeID = nodeid;
            int AD_User_ID = ctx.GetAD_User_ID();
            m_clientTree = clientTree;
            log.Info("AD_Tree_ID=" + AD_Tree_ID + ", AD_User_ID=" + AD_User_ID
                + ", Editable=" + editable + ", OnClient=" + clientTree);
            //
            LoadNodes(AD_User_ID);
        }

        /// <summary>
        /// Construct & Load Tree
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Tree_ID">The tree to build</param>
        /// <param name="editable">True, if tree can be modified - includes inactive and empty summary nodes</param>
        /// <param name="clientTree">the tree is displayed on the java client (not on web)</param>
        /// <param name="trxName">transaction</param>
        public MTree(Ctx ctx, int AD_Tree_ID, bool editable, bool clientTree, Trx trxName, int nodeid, int AD_Tab_ID)
            : this(ctx, AD_Tree_ID, trxName)
        {
            this.AD_Tab_ID = AD_Tab_ID;
            m_editable = editable;
            currentNodeID = nodeid;
            int AD_User_ID = ctx.GetAD_User_ID();
            m_clientTree = clientTree;
            log.Info("AD_Tree_ID=" + AD_Tree_ID + ", AD_User_ID=" + AD_User_ID
                + ", Editable=" + editable + ", OnClient=" + clientTree);
            //
            LoadNodes(AD_User_ID);
        }



        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MTree(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// Full Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="Name">name</param>
        /// <param name="TreeType">tree type</param>
        /// <param name="trxName">transaction</param>
        public MTree(Ctx ctx, String Name, String TreeType, Trx trxName)
            : this(ctx, 0, trxName)
        {
            SetName(Name);
            SetTreeType(TreeType);
            SetAD_Table_ID();
            SetIsAllNodes(true);	//	complete tree
            SetIsDefault(false);
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="client">client</param>
        /// <param name="name">name</param>
        /// <param name="treeType">tree type</param>
        public MTree(X_AD_Client client, String name, String treeType)
            : this(client.GetCtx(), 0, client.Get_TrxName())
        {
            SetClientOrg(client);
            SetName(name);
            SetTreeType(treeType);
            SetAD_Table_ID();
        }

        /// <summary>
        /// Construct & Load Tree
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Tree_ID">The tree to build</param>
        /// <param name="editable">True, if tree can be modified - includes inactive and empty summary nodes</param>
        /// <param name="clientTree">the tree is displayed on the java client (not on web)</param>
        /// <param name="trxName">transaction</param>
        public MTree(Ctx ctx, int AD_Tree_ID, bool editable, bool clientTree, Trx trxName)
            : this(ctx, AD_Tree_ID, trxName)
        {
            m_editable = editable;
            int AD_User_ID = ctx.GetAD_User_ID();
            m_clientTree = clientTree;
            log.Info("AD_Tree_ID=" + AD_Tree_ID + ", AD_User_ID=" + AD_User_ID
                + ", Editable=" + editable + ", OnClient=" + clientTree);
            //
            LoadNodes(AD_User_ID);
        }

        /// <summary>
        /// Get MTree_Base from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Tree_ID">id</param>
        /// <param name="trxName">transaction</param>
        /// <returns>MTree_Base</returns>
        public static MTree Get(Ctx ctx, int AD_Tree_ID, Trx trxName)
        {
            int key = AD_Tree_ID;
            MTree retValue = (MTree)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MTree(ctx, AD_Tree_ID, trxName);
            if (retValue.Get_ID() != 0)
                s_cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Get default (oldest) complete AD_Tree_ID for KeyColumn.
        /// </summary>
        /// <param name="AD_Client_ID">client id</param>
        /// <param name="AD_Table_ID">table id</param>
        /// <returns>AD_Tree_ID or 0</returns>
        public static int GetDefaultAD_Tree_ID(int AD_Client_ID, int AD_Table_ID)
        {
            _log.Finer("AD_Table_ID=" + AD_Table_ID);
            if (AD_Table_ID == 0)
                return 0;
            int AD_Tree_ID = 0;
            String sql = "SELECT AD_Tree_ID, Name FROM AD_Tree "
                + "WHERE AD_Client_ID=" + AD_Client_ID + " AND AD_Table_ID=" + AD_Table_ID + " AND IsActive='Y' AND IsAllNodes='Y' "
                + "ORDER BY IsDefault DESC, AD_Tree_ID";

            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql, null, null);
                if (dr.Read())
                {
                    if (dr["AD_Tree_ID"] != null && dr["AD_Tree_ID"].ToString() != "")
                        AD_Tree_ID = Utility.Util.GetValueOfInt(dr["AD_Tree_ID"].ToString());
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
                _log.Log(Level.SEVERE, sql, e);
            }
            return AD_Tree_ID;
        }

        /// <summary>
        /// Get default (oldest) complete AD_Tree_ID for KeyColumn.
        /// </summary>
        /// <param name="AD_Client_ID">client id</param>
        /// <param name="tableName">table name</param>
        /// <returns>AD_Tree_ID or 0</returns>
        public static int GetDefaultAD_Tree_ID(int AD_Client_ID, String tableName)
        {
            _log.Finer("TableName=" + tableName);
            if (tableName == null)
                return 0;
            int AD_Tree_ID = 0;
            String sql = "SELECT tr.AD_Tree_ID, tr.Name "
                + "FROM AD_Tree tr INNER JOIN AD_Table tb ON (tr.AD_Table_ID=tb.AD_Table_ID) "
                + "WHERE tr.AD_Client_ID=@clientid AND tb.TableName=@tablename AND tr.IsActive='Y' AND tr.IsAllNodes='Y' "
                + "ORDER BY tr.IsDefault DESC, tr.AD_Tree_ID";

            SqlParameter[] param = new SqlParameter[2];
            param[0] = new SqlParameter("@clientid", AD_Client_ID);
            param[1] = new SqlParameter("@tablename", tableName);
            IDataReader dr = null;
            try
            {
                dr = ExecuteQuery.ExecuteReader(sql, param);
                if (dr.Read())
                {
                    if (dr["AD_Tree_ID"] != null && dr["AD_Tree_ID"].ToString() != "")
                        AD_Tree_ID = Utility.Util.GetValueOfInt(dr["AD_Tree_ID"].ToString());
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
                _log.Log(Level.SEVERE, sql, e);
            }
            return AD_Tree_ID;
        }

        /// <summary>
        /// Get Node TableName
        /// </summary>
        /// <param name="treeType">tree type</param>
        /// <returns>node table name, e.g. AD_TreeNode</returns>
        static String GetNodeTableName(String treeType)
        {
            String nodeTableName = "AD_TreeNode";
            if (TREETYPE_Menu.Equals(treeType))
                nodeTableName += "MM";
            else if (TREETYPE_BPartner.Equals(treeType))
                nodeTableName += "BP";
            else if (TREETYPE_Product.Equals(treeType))
                nodeTableName += "PR";
            //
            else if (TREETYPE_CMContainer.Equals(treeType))
                nodeTableName += "CMC";
            else if (TREETYPE_CMContainerStage.Equals(treeType))
                nodeTableName += "CMS";
            else if (TREETYPE_CMMedia.Equals(treeType))
                nodeTableName += "CMM";
            else if (TREETYPE_CMTemplate.Equals(treeType))
                nodeTableName += "CMT";
            //
            else if (TREETYPE_User1.Equals(treeType))
                nodeTableName += "U1";
            else if (TREETYPE_User2.Equals(treeType))
                nodeTableName += "U2";
            else if (TREETYPE_User3.Equals(treeType))
                nodeTableName += "U3";
            else if (TREETYPE_User4.Equals(treeType))
                nodeTableName += "U4";
            return nodeTableName;
        }

        /// <summary>
        /// Get Node TableName
        /// </summary>
        /// <param name="AD_Table_ID">table id</param>
        /// <returns>e name, e.g. AD_TreeNode</returns>
        static public String GetNodeTableName(int AD_Table_ID, Ctx ctx)
        {
            String nodeTableName = "AD_TreeNode";
            if (X_AD_Menu.Table_ID == AD_Table_ID)
                nodeTableName += "MM";
            else if (X_C_BPartner.Table_ID == AD_Table_ID)
                nodeTableName += "BP";
            else if (X_M_Product.Table_ID == AD_Table_ID)
                nodeTableName += "PR";
            //
            else if (X_CM_Container.Table_ID == AD_Table_ID)
                nodeTableName += "CMC";
            else if (X_CM_CStage.Table_ID == AD_Table_ID)
                nodeTableName += "CMS";
            else if (X_CM_Media.Table_ID == AD_Table_ID)
                nodeTableName += "CMM";
            else if (X_CM_Template.Table_ID == AD_Table_ID)
                nodeTableName += "CMT";
            else
            {
                if (s_TableIDs == null)
                    FillUserTables(null, ctx);
                int ii = AD_Table_ID;
                if (s_TableIDs.Contains(ii))
                {
                    if (s_TableIDs_U1.Contains(ii))
                        nodeTableName += "U1";
                    else if (s_TableIDs_U2.Contains(ii))
                        nodeTableName += "U2";
                    else if (s_TableIDs_U3.Contains(ii))
                        nodeTableName += "U3";
                    else if (s_TableIDs_U4.Contains(ii))
                        nodeTableName += "U4";
                }
                else	//	no tree
                    return null;
            }
            return nodeTableName;
        }

        /// <summary>
        /// Table has Tree
        /// </summary>
        /// <param name="AD_Table_ID">table id</param>
        /// <returns>true if table has tree</returns>
        public static bool HasTree(int AD_Table_ID, Ctx ctx)
        {
            if (s_TableIDs == null)
                FillUserTables(null, ctx);
            return s_TableIDs.Contains(AD_Table_ID);
        }

        /// <summary>
        /// Table has Tree
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <returns>true if table has tree</returns>
        public static bool HasTree(String tableName, Ctx ctx)
        {
            if (s_TableNames == null)
                FillUserTables(null, ctx);
            //return s_TableIDs.Contains(tableName);
            return s_TableNames.Contains(tableName);
        }

        /// <summary>
        /// Fill User Tables
        /// </summary>
        /// <param name="trxName">transaction</param>
        static void FillUserTables(Trx trxName, Ctx ctx)
        {
            s_TableNames = new List<String>();
            s_TableIDs = new List<int>();
            s_TableIDs_U1 = new List<int>();
            s_TableIDs_U2 = new List<int>();
            s_TableIDs_U3 = new List<int>();
            s_TableIDs_U4 = new List<int>();
            //
            String sql = "SELECT DISTINCT TreeType, AD_Table_ID FROM AD_Tree";

            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql, null, trxName);
                while (dr.Read())
                {
                    String TreeType = dr["TreeType"].ToString();//rs.getString(1);
                    int AD_Table_ID = 0;
                    if (dr["AD_Table_ID"] != null && dr["AD_Table_ID"].ToString() != "")
                    {
                        AD_Table_ID = Utility.Util.GetValueOfInt(dr["AD_Table_ID"].ToString());//rs.getInt(2);
                    }
                    if (AD_Table_ID == 0)
                        continue;
                    s_TableIDs.Add(AD_Table_ID);		//	all
                    if (TreeType.Equals("U1"))
                        s_TableIDs_U1.Add(AD_Table_ID);
                    else if (TreeType.Equals("U2"))
                        s_TableIDs_U2.Add(AD_Table_ID);
                    else if (TreeType.Equals("U3"))
                        s_TableIDs_U3.Add(AD_Table_ID);
                    else if (TreeType.Equals("U4"))
                        s_TableIDs_U4.Add(AD_Table_ID);
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
                _log.Log(Level.SEVERE, sql, e);
            }
            //	Not updated
            if (s_TableIDs.Count < 3)
            {
                MTree xx = Get(ctx, 10, trxName);
                xx.UpdateTrees();
                FillUserTables(null, ctx);
            }
        }

        /// <summary>
        /// Update all Trees with Table_ID
        /// </summary>
        public void UpdateTrees()
        {
            SetAD_Table_ID();
            for (int i = 0; i < TREETYPES.Length; i++)
                UpdateTrees(TREETYPES[i], TABLEIDS[i]);
        }

        /// <summary>
        /// Update Trees
        /// </summary>
        /// <param name="treeType">tree type</param>
        /// <param name="AD_Table_ID">table id</param>
        private void UpdateTrees(String treeType, int AD_Table_ID)
        {
            if (AD_Table_ID == 0)
                return;
            StringBuilder sb = new StringBuilder("UPDATE AD_Tree SET AD_Table_ID=")
                .Append(AD_Table_ID)
                .Append(" WHERE TreeType='").Append(treeType).Append("' AND AD_Table_ID IS NULL");
            int no = DB.ExecuteQuery(sb.ToString(), null, Get_Trx());
            //int no = DataBase.executeUpdate(sb.ToString(), Get_TrxName());
            log.Fine(treeType + " #" + no);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public String GetNodeTableName()
        {
            return GetNodeTableName(GetTreeType());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int SetAD_Table_ID()
        {
            int AD_Table_ID = 0;
            String type = GetTreeType();
            if (type == null
                || type.StartsWith("U")	//	User
                || type.Equals(TREETYPE_Other))
                return 0;
            for (int i = 0; i < TREETYPES.Length; i++)
            {
                if (type.Equals(TREETYPES[i]))
                {
                    AD_Table_ID = TABLEIDS[i];
                    break;
                }
            }
            if (AD_Table_ID != 0)
                SetAD_Table_ID(AD_Table_ID);
            if (AD_Table_ID == 0)
            {
                log.Warning("Did not find Table for TreeType=" + type);
            }
            return AD_Table_ID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newRecord"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            String treeType = GetTreeType();
            /**
            if (newRecord)	//	Base Node
            {
                if (TREETYPE_BPartner.equals(treeType))
                {
                    MTreeNodeBP ndBP = new MTreeNodeBP(this, 0);
                    ndBP.save();
                }
                else if (TREETYPE_Menu.equals(treeType))
                {
                    MTreeNodeMM ndMM = new MTreeNodeMM(this, 0);
                    ndMM.save();
                }
                else if (TREETYPE_Product.equals(treeType))
                {
                    MTreeNodePR ndPR = new MTreeNodePR(this, 0);
                    ndPR.save();
                }
                else
                {
                    MTreeNode nd = new MTreeNode(this, 0);
                    nd.save();
                }
            }
            **/
            if (treeType.StartsWith("U"))
                FillUserTables(Get_Trx(), GetCtx());
            //
            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns></returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (!IsActive() || !IsAllNodes())
                if (IsDefault())
                    SetIsDefault(false);
            //	Table
            if (GetAD_Table_ID(true) == 0)
            {
                if (newRecord)
                    SetAD_Table_ID();
                else
                    UpdateTrees();
                //
                if (GetAD_Table_ID(true) == 0)
                {
                    log.Warning("No Table for " + ToString());
                    return false;
                }
            }
            return Validate();
        }

        /// <summary>
        /// Get AD_Table_ID
        /// </summary>
        /// <returns>table id</returns>
        public new int GetAD_Table_ID()
        {
            int AD_Table_ID = base.GetAD_Table_ID();
            if (AD_Table_ID == 0)
                AD_Table_ID = SetAD_Table_ID();
            return AD_Table_ID;
        }

        /// <summary>
        /// Get AD_Table_ID
        /// </summary>
        /// <param name="isBase">base</param>
        /// <returns>table id</returns>
        public int GetAD_Table_ID(bool isBase)
        {
            if (isBase)
                return base.GetAD_Table_ID();
            return GetAD_Table_ID();
        }

        /// <summary>
        /// Validate TreeType and AD_Table_ID
        /// </summary>
        /// <returns>true if Tree Type compatible with AD_Table_ID</returns>
        private bool Validate()
        {
            String type = GetTreeType();
            if (type != null
                    && (type.StartsWith("U") || type.Equals(TREETYPE_Other)))
                return true;
            //
            int AD_Table_ID = GetAD_Table_ID(true);
            for (int i = 0; i < TREETYPES.Length; i++)
            {
                if (type == null)
                {
                    if (AD_Table_ID == TABLEIDS[i])
                    {
                        SetTreeType(TREETYPES[i]);
                        return true;
                    }
                }
                else if (AD_Table_ID == TABLEIDS[i])
                {
                    if (type.Equals(TREETYPES[i]))
                        return true;
                    else
                    {
                        SetTreeType(TREETYPES[i]);
                        return true;
                    }
                }
                else if (AD_Table_ID == 0 && type.Equals(TREETYPES[i]))
                {
                    SetAD_Table_ID(TABLEIDS[i]);
                    return true;
                }
            }
            //	None found
            if (type == null)
            {
                SetTreeType(TREETYPE_Other);
                return true;
            }
            log.Warning("TreeType=" + type + " <> AD_Table_ID=" + AD_Table_ID);
            SetTreeType(TREETYPE_Other);
            return false;
        }

        /// <summary>
        /// Loads data into datatable dt 
        /// </summary>
        protected void GetNodeDetails()
        {
            StringBuilder sqlNode = new StringBuilder();
            String sourceTable = "t";
            String fromClause = GetSourceTableName(false);	//	fully qualified
            String columnNameX = GetSourceTableName(true);
            String color = GetActionColorName();


            if (GetTreeType().Equals(TREETYPE_Menu))
            {
                bool baseLang = Utility.Env.IsBaseLanguage(GetCtx(), "");// DataBase.GlobalVariable.IsBaseLanguage();
                sourceTable = "AD_Menu";
                if (baseLang)
                    sqlNode.Append("SELECT AD_Menu.AD_Menu_ID, AD_Menu.Name,AD_Menu.Description,AD_Menu.IsSummary,AD_Menu.Action, "
                        + "AD_Menu.AD_Window_ID, AD_Menu.AD_Process_ID, AD_Menu.AD_Form_ID, AD_Menu.AD_Workflow_ID, AD_Menu.AD_Task_ID, AD_Menu.AD_Workbench_ID, "

                        + " NVL(img.FontName,img.ImageURL) as Image, AD_Menu.IsSetting FROM AD_Menu AD_Menu");
                else
                    sqlNode.Append("SELECT AD_Menu.AD_Menu_ID,  t.Name,t.Description,AD_Menu.IsSummary,AD_Menu.Action, "
                        + "AD_Menu.AD_Window_ID, AD_Menu.AD_Process_ID, AD_Menu.AD_Form_ID, AD_Menu.AD_Workflow_ID, AD_Menu.AD_Task_ID, AD_Menu.AD_Workbench_ID, "
                        + " NVL(img.FontName,img.ImageURL) as Image, AD_Menu.IsSetting FROM AD_Menu AD_Menu JOIN  AD_Menu_Trl t ON AD_Menu.AD_Menu_ID=t.AD_Menu_ID ");

                if (!baseLang)
                {
                    sqlNode.Append(" JOIN " + GetNodeTableName() + " pr on pr.NODE_ID=AD_Menu." + columnNameX + "_ID ");
                    sqlNode.Append(" LEFT OUTER JOIN AD_Image img on AD_Menu.AD_Image_ID=img.AD_Image_ID ");

                    sqlNode.Append(" WHERE AD_Menu.AD_Menu_ID=t.AD_Menu_ID AND t.AD_Language='")
                        .Append(Utility.Env.GetAD_Language(GetCtx())).Append("'");

                    if (onDemand)
                    {

                        //  sqlNode.Append(" OR ( m." + columnNameX + "_ID IN (SELECT NODE_ID FROM " + GetNodeTableName() + " WHERE Parent_ID=0 AND AD_Tree_ID=" + GetAD_Tree_ID() + " AND m.IsSummary='N' AND IsActive='Y')) ");

                        sqlNode.Append(" AND pr.AD_Tree_ID=" + GetAD_Tree_ID() + "  AND (IsSummary='Y')");

                    }
                }
                else
                {
                    sqlNode.Append(" LEFT OUTER JOIN AD_Image img on AD_Menu.AD_Image_ID=img.AD_Image_ID ");
                    if (onDemand)
                    {
                        sqlNode.Append(" JOIN " + GetNodeTableName() + " pr on pr.NODE_ID=AD_Menu." + columnNameX + "_ID ");
                        sqlNode.Append(" LEFT OUTER JOIN AD_Image img on AD_Menu.AD_Image_ID=img.AD_Image_ID ");
                        //  sqlNode.Append(" OR ( m." + columnNameX + "_ID IN (SELECT NODE_ID FROM " + GetNodeTableName() + " WHERE Parent_ID=0 AND AD_Tree_ID=" + GetAD_Tree_ID() + " AND m.IsSummary='N' AND IsActive='Y')) ");
                        sqlNode.Append("AND pr.AD_Tree_ID=" + GetAD_Tree_ID() + "  AND (IsSummary='Y')");

                    }
                }

                if (!m_editable)
                {
                    bool hasWhere = sqlNode.ToString().IndexOf(" WHERE ") != -1;
                    sqlNode.Append(hasWhere ? " AND " : " WHERE ").Append("AD_Menu.IsActive='Y' ");
                }
                //	Do not show Beta
                if (!MClient.Get(GetCtx()).IsUseBetaFunctions())
                {
                    bool hasWhere = sqlNode.ToString().IndexOf(" WHERE ") != -1;
                    sqlNode.Append(hasWhere ? " AND " : " WHERE ");
                    sqlNode.Append("(AD_Menu.AD_Window_ID IS NULL OR EXISTS (SELECT * FROM AD_Window w WHERE AD_Menu.AD_Window_ID=w.AD_Window_ID AND w.IsBetaFunctionality='N'))")
                        .Append(" AND (AD_Menu.AD_Process_ID IS NULL OR EXISTS (SELECT * FROM AD_Process p WHERE AD_Menu.AD_Process_ID=p.AD_Process_ID AND p.IsBetaFunctionality='N'))")
                        .Append(" AND (AD_Menu.AD_Form_ID IS NULL OR EXISTS (SELECT * FROM AD_Form f WHERE AD_Menu.AD_Form_ID=f.AD_Form_ID AND f.IsBetaFunctionality='N'))");
                }
                //	In R/O Menu - Show only defined Forms
                if (!m_editable)
                {
                    bool hasWhere = sqlNode.ToString().IndexOf(" WHERE ") != -1;
                    sqlNode.Append(hasWhere ? " AND " : " WHERE ");
                    sqlNode.Append("(AD_Menu.AD_Form_ID IS NULL OR EXISTS (SELECT * FROM AD_Form f WHERE AD_Menu.AD_Form_ID=f.AD_Form_ID AND ");
                    if (m_clientTree)
                        sqlNode.Append("f.Classname");
                    else
                        sqlNode.Append("f.JSPURL");
                    sqlNode.Append(" IS NOT NULL))");
                }

                // sqlNode.Append("   ORDER BY m.Name");

            }
            else
            {

                sourceTable = columnNameX;
                if (columnNameX == null)
                    throw new ArgumentException("Unknown TreeType=" + GetTreeType());
                sqlNode.Append("SELECT " + columnNameX + ".").Append(columnNameX)
                    .Append("_ID," + columnNameX + ".Name," + columnNameX + ".Description," + columnNameX + ".IsSummary,").Append(color)
                    .Append(" FROM ").Append(fromClause);
                if (!m_editable)
                {


                    if (onDemand)
                    {
                        sqlNode.Append(" JOIN " + GetNodeTableName() + " pr on pr.NODE_ID=" + columnNameX + "." + columnNameX + "_ID ");
                        sqlNode.Append(" WHERE " + columnNameX + ".IsActive='Y'");
                        sqlNode.Append(" AND pr.AD_Tree_ID=" + GetAD_Tree_ID() + "  AND (IsSummary='Y' )");
                        //  sqlNode.Append(" AND t.IsSummary='Y'");
                        //sqlNode.Append(" OR ( t." + columnNameX + "_ID IN (SELECT NODE_ID FROM " + GetNodeTableName() + " WHERE Parent_ID=0 AND AD_Tree_ID=" + GetAD_Tree_ID() + " AND t.IsSummary='N' AND IsActive='Y')) ");
                    }
                    else
                    {
                        sqlNode.Append(" WHERE " + columnNameX + ".IsActive='Y'");
                    }
                }
                else
                {
                    if (onDemand)
                    {
                        // sqlNode.Append(" WHERE t.IsActive='Y'");
                        // sqlNode.Append(" AND t.IsSummary='Y'");
                        //sqlNode.Append(" OR ( t." + columnNameX + "_ID IN (SELECT NODE_ID FROM " + GetNodeTableName() + " WHERE Parent_ID=0 AND AD_Tree_ID=" + GetAD_Tree_ID() + " AND t.IsSummary='N' AND IsActive='Y')) ");

                        sqlNode.Append(" JOIN " + GetNodeTableName() + " pr on pr.NODE_ID=" + columnNameX + "." + columnNameX + "_ID ");
                        sqlNode.Append(" WHERE " + columnNameX + ".IsActive='Y'");
                        sqlNode.Append(" AND pr.AD_Tree_ID=" + GetAD_Tree_ID() + "  AND (IsSummary='Y')");

                    }
                }


            }

            String sql = sqlNode.ToString();

            if (sql.ToLower().IndexOf("where") > -1)
            {
                if (AD_Tab_ID > 0)
                {
                    MTab tab = new MTab(p_ctx, AD_Tab_ID, null);
                    if (!String.IsNullOrEmpty(tab.GetWhereClause()))
                    {
                        sql += " AND " + tab.GetWhereClause();
                    }
                }
            }
            else
            {
                if (AD_Tab_ID > 0)
                {
                    MTab tab = new MTab(p_ctx, AD_Tab_ID, null);
                    if (!String.IsNullOrEmpty(tab.GetWhereClause()))
                    {
                        sql += " WHERE " + tab.GetWhereClause();
                    }
                }
            }


            if (!m_editable)	//	editable = menu/etc. window
                sql = MRole.GetDefault(GetCtx(), false).AddAccessSQL(sql,
                    sourceTable, MRole.SQL_FULLYQUALIFIED, m_editable);
            log.Fine(sql);

            if (DB.IsPostgreSQL())
            {
                sql = "SELECT Distinct * FROM (" + sqlNode.ToString() + ") as foo ORDER BY Name";
            }
            else
            {
                sql = "SELECT Distinct * FROM (" + sqlNode.ToString() + ") ORDER BY Name";
            }


            IDataReader drTree = null;
            try
            {

                drTree = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(drTree);
                drTree.Close();

            }
            catch
            {
                if (drTree != null)
                    drTree.Close();
            }
        }

        /// <summary>
        /// Method to load nodes in case order by some value is required.
        /// </summary>
        /// <param name="ctx">current Context</param>
        /// <param name="orderClause">Order By clause</param>
        public void GetTreeNodes(Ctx ctx, string orderClause)
        {
            LoadNodes(ctx.GetAD_User_ID(), orderClause);
        }



        /// <summary>
        /// Load Nodes and Bar
        /// </summary>
        /// <param name="AD_User_ID">user for tree bar</param>
        private void LoadNodes(int AD_User_ID, string orderClause = "")
        {
            ////  SQL for TreeNodes
            StringBuilder sql = new StringBuilder("SELECT "
                + "tn.Node_ID,COALESCE(tn.Parent_ID, -1) AS Parent_ID,tn.SeqNo,tb.IsActive "
                + "FROM ").Append(GetNodeTableName()).Append(" tn"
                + " LEFT OUTER JOIN AD_TreeBar tb ON (tn.AD_Tree_ID=tb.AD_Tree_ID"
                + " AND tn.Node_ID=tb.Node_ID AND tb.AD_User_ID=@userid) ");	//	#1

            string tblName = MTable.GetTableName(p_ctx, GetAD_Table_ID());
            //on (mp.M_Product_ID= tn.Node_ID)

            //if (onDemand || AD_Tab_ID > 0)
            //{
            sql.Append(" JOIN ").Append(tblName + " " + tblName + " ON (" + tblName + "." + tblName + "_ID = tn.Node_ID)");
            //}

            if (onDemand)
            {


                sql.Append("WHERE tn.AD_Tree_ID=@treeId");								//	#2
                if (!m_editable)
                    sql.Append(" AND tn.IsActive='Y'");

                sql.Append(" AND (" + tblName + "." + "IsSummary='Y')");
            }
            else
            {
                sql.Append("WHERE tn.AD_Tree_ID=@treeId");								//	#2
                if (!m_editable)
                    sql.Append(" AND tn.IsActive='Y'");
            }




            if (currentNodeID > 0)
            {
                GetChildNodesID(currentNodeID);
                if (str.Length > 0)
                {
                    sql.Append(" AND tn." + GetNodeTableName() + "_ID IN (").Append(str).Append(")"); ;
                }
            }

            string sqls = sql.ToString();
            if (AD_Tab_ID > 0)
            {
                MTab tab = new MTab(p_ctx, AD_Tab_ID, null);
                string _whareCaluse = tab.GetWhereClause();

                _whareCaluse = Env.ParseContext(p_ctx, windowNo, _whareCaluse, false);

                if (sqls.ToLower().IndexOf("where") > -1)
                {
                    if (!String.IsNullOrEmpty(_whareCaluse))
                    {
                        sqls += " AND " + _whareCaluse;
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(_whareCaluse))
                    {
                        sqls += " WHERE " + _whareCaluse;
                    }
                }
            }

            //sql.Append(" ORDER BY COALESCE(tn.Parent_ID, -1), tn.SeqNo");
            if (orderClause == "")
            {

                sqls += " ORDER BY COALESCE(tn.Parent_ID, -1), tn.SeqNo, Upper(" + tblName + ".Name)";
            }
            else
            {
                sqls += " ORDER BY " + orderClause;
            }

            //sqls += " ORDER BY COALESCE(tn.Parent_ID, -1), tn.SeqNo";

            log.Finest(sqls.ToString());

            //  The Node Loop
            IDataReader dr = null;
            try
            {
                // load Node details - addToTree -> getNodeDetail
                GetNodeDetails();
                //
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@userid", AD_User_ID);
                param[1] = new SqlParameter("@treeId", GetAD_Tree_ID());
                //PreparedStatement pstmt = DataBase.prepareStatement(sql.toString(), get_TrxName());

                //    //	Get Tree & Bar

                dr = ExecuteQuery.ExecuteReader(sqls.ToString(), param);

                //DataSet dsTable = DB.ExecuteDataset(sql.ToString(),param);

                //DataTable dtTable = new DataTable();

                ////dtTable.Load(dr);
                ////dtTable.DefaultView.Sort = "Parent_ID, SeqNo";
                ////dtTable = dtTable.DefaultView.ToTable();

                //dsTable.Tables[0].DefaultView.Sort = "Parent_ID, SeqNo";
                //dtTable = dsTable.Tables[0].DefaultView.ToTable();



                m_root = new VTreeNode(0, 0, GetName(), GetDescription(), 0, true, null, false);//, null);
                while (dr.Read())
                {
                    int node_ID = Utility.Util.GetValueOfInt(dr[0]);
                    int parent_ID = 0;
                    if (dr["Parent_ID"].ToString() != "")
                        parent_ID = Utility.Util.GetValueOfInt(dr[1]);
                    int seqNo = Utility.Util.GetValueOfInt(dr[2]);
                    bool onBar = dr[3].ToString() != "";

                    if (node_ID == 0 && parent_ID == 0)
                    { }
                    else
                        AddToTree(node_ID, parent_ID, seqNo, onBar);	//	calls getNodeDetail
                }


                //for (int x = 0; x < dtTable.Rows.Count; x++)
                //{
                //    int node_ID = Utility.Util.GetValueOfInt(dtTable.Rows[x][0]);
                //    int parent_ID = 0;
                //    if (dtTable.Rows[x]["Parent_ID"].ToString() != "")
                //        parent_ID = Utility.Util.GetValueOfInt(dtTable.Rows[x][1]);
                //    int seqNo = Utility.Util.GetValueOfInt(dtTable.Rows[x][2]);
                //    bool onBar = dtTable.Rows[x][3].ToString() != "";

                //    if (node_ID == 0 && parent_ID == 0)
                //    { }
                //    else
                //        AddToTree(node_ID, parent_ID, seqNo, onBar);	//	calls getNodeDetail
                //}


                dr.Close();
                //dtTable.Clear();
                //dtTable = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sqls.ToString(), e);
                if (dr != null)
                {
                    dr.Close();

                }
                if (dt != null)
                {
                    dt.Clear();
                    dt = null;
                }
            }
            ////  Done with loading - add remainder from buffer
            // Get From list
            log.Finest("clearing buffer - Adding to****************: " + m_root);
            for (int i = 0; i < mnuNodes.Count; i++)
            {
                //log.Finest("clearing buffer - Adding to: " + m_root);
                VTreeNode node = mnuNodes[i];
                VTreeNode parent = FindTreeNode(m_root.Nodes, node.Parent_ID.ToString());
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
                log.Severe("Nodes w/o parent - adding to root - " + _buffer);
                for (int i = 0; i < mnuNodes.Count; i++)
                {
                    VTreeNode node = mnuNodes[i];
                    m_root.Nodes.Add(node);
                    mnuNodes.RemoveAt(i);
                    i = -1;
                }
                if (_buffer.Count != 0)
                {
                    log.Severe("Still nodes in Buffer - " + _buffer);
                }
            }
            //  clean up
            if (!m_editable && m_root.Nodes.Count > 0)
                TrimTree(m_root);
            ////		diagPrintTree();
            if (VLogMgt.IsLevelFinest() || m_root.Nodes.Count == 0)
                log.Fine("ChildCount=" + m_root.Nodes.Count);
        }


        StringBuilder str = new StringBuilder();
        private void GetChildNodesID(int currentnode)
        {
            if (str.Length == 0)
            {
                str.Append(currentnode);
            }
            else
            {
                str.Append(currentnode).Append(",");
            }
            string sql = "SELECT node_ID FROM " + GetNodeTableName() + " WHERE Parent_ID=" + currentnode;
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds == null || ds.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    GetChildNodesID(Convert.ToInt32(ds.Tables[0].Rows[j]["node_ID"]));
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
            //VTreeNode parent = null;
            if (onBar == true) // if in favorite (short -cut)
            {
                VTreeNode childBar = GetNodeDetail(Node_ID, Parent_ID, seqNo, onBar);
                barNodes.Insert(barNodes.Count, childBar);
            }

            if (!child.IsSummary)
            {
                SearchItem childLeaf = new SearchItem(child);
                _leafNodesList.Insert(_leafNodesList.Count, childLeaf);
            }


            if (m_root != null)
            {
                if (child.Parent_ID == 0 || child.Parent_ID == -1)
                {
                    m_root.Nodes.Add(child);
                }
                else
                {
                    if (mnuNodes.Count > 0)
                    {
                        for (int i = 0; i < mnuNodes.Count; i++)
                        {
                            VTreeNode node = mnuNodes[i];// FindTreeNode(mnuNodes[i].no
                            if (child.Parent_ID == node.Node_ID)
                            {
                                node.Nodes.Add(child);
                                return;
                            }
                            if (node.Nodes.Count > 0)
                            {
                                VTreeNode parentnode = FindTreeNode(node.Nodes, child.Parent_ID.ToString());
                                if (parentnode != null)
                                {
                                    parentnode.Nodes.Add(child);
                                    return;
                                }
                            }
                        }
                    }

                    mnuNodes.Add(child);
                }   //  addToTree
            }
        }


        private List<SearchItem> _leafNodesList = new List<SearchItem>(20);

        public List<SearchItem> GetLeafNodes()
        {
            return _leafNodesList;
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
            int AD_Window_ID = 0;
            int AD_Process_ID = 0;
            int AD_Form_ID = 0;
            int AD_Workflow_ID = 0;
            int AD_Task_ID = 0;
            int AD_Workbench_ID = 0;

            VTreeNode retValue = null;

            // Serch For Node details
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int node1 = int.Parse(dt.Rows[i][0].ToString());
                    if (Node_ID != node1)	//	search for correct one
                    {
                        continue;
                    }
                    DataRow dr = dt.Rows[i];

                    int node = int.Parse(dr[0].ToString());
                    int index = 1;


                    string name = dr[index++].ToString();

                    string description = dr[index++].ToString();
                    bool isSummary = "Y".Equals(dr[index++].ToString());
                    string actionColor = dr[index++].ToString().Trim();
                    //	Menu only
                    if (GetTreeType().Equals(TREETYPE_Menu) && !isSummary)
                    {
                        AD_Window_ID = Utility.Util.GetValueOfInt(dr[index]);
                        index++;
                        AD_Process_ID = Utility.Util.GetValueOfInt(dr[index]);
                        index++;
                        AD_Form_ID = Utility.Util.GetValueOfInt(dr[index]);
                        index++;
                        AD_Workflow_ID = Utility.Util.GetValueOfInt(dr[index]);
                        index++;
                        AD_Task_ID = Utility.Util.GetValueOfInt(dr[index]);
                        index++;
                        AD_Workbench_ID = Utility.Util.GetValueOfInt(dr[index]);
                        index++;
                        //
                        MRole role = MRole.GetDefault(GetCtx(), false);
                        bool? blnAccess = false;


                        if (X_AD_Menu.ACTION_Window.Equals(actionColor))
                            blnAccess = role.GetWindowAccess(AD_Window_ID);
                        else if (X_AD_Menu.ACTION_Process.Equals(actionColor)
                        || X_AD_Menu.ACTION_Report.Equals(actionColor))
                            blnAccess = role.GetProcessAccess(AD_Process_ID);
                        else if (X_AD_Menu.ACTION_Form.Equals(actionColor))
                            blnAccess = role.GetFormAccess(AD_Form_ID);
                        else if (X_AD_Menu.ACTION_WorkFlow.Equals(actionColor))
                            blnAccess = role.GetWorkflowAccess(AD_Workflow_ID);
                        else if (X_AD_Menu.ACTION_Task.Equals(actionColor))
                            blnAccess = role.GetTaskAccess(AD_Task_ID);

                        if (blnAccess != null || m_editable)		//	rw or ro for Role 
                        {
                            retValue = new VTreeNode(Node_ID, seqNo,
                                name, description, Parent_ID, isSummary,
                                actionColor, onBar);	//	menu has no color
                        }
                    }
                    else
                    {
                        //Color color = null;	//	action
                        //if (actionColor != null && !getTreeType().equals(TREETYPE_Menu))
                        //{
                        //    MPrintColor printColor = MPrintColor.get(getCtx(), actionColor);
                        //    if (printColor != null)
                        //        color = printColor.getColor();
                        //}

                        retValue = new VTreeNode(Node_ID, seqNo,
                                name, description, Parent_ID, isSummary,
                                actionColor, onBar);
                        retValue.Image = Utility.Util.GetValueOfString(dr["Image"]);

                        retValue.IsSetting = Utility.Util.GetValueOfString(dr["IsSetting"]) == "Y";

                    }
                    break;
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "", e);
            }
            if (retValue != null)
            {
                // set VTreeNode ID's
                retValue.AD_Window_ID = AD_Window_ID;
                retValue.AD_Process_ID = AD_Process_ID;
                retValue.AD_Form_ID = AD_Form_ID;
                retValue.AD_Workflow_ID = AD_Workflow_ID;
                retValue.AD_Task_ID = AD_Task_ID;
                retValue.AD_Workbench_ID = AD_Workbench_ID;
            }
            return retValue;
        }

        /// <summary>
        /// remove summery node having no children
        /// </summary>
        /// <param name="tn">tree node</param>
        private void TrimTree(System.Windows.Forms.TreeNode tn)
        {
            System.Collections.IEnumerator en = ((VTreeNode)tn).preorderEnumeration();

            while (en.MoveNext())
            {
                VTreeNode node = (VTreeNode)en.Current;

                if (node.IsSummary && node.Nodes.Count < 1)
                {
                    if (tn == node)
                        continue;
                    tn.Nodes.Remove(node);

                    if (barNodes.Contains(node))
                    {
                        barNodes.Remove(node);
                    }

                    en = ((VTreeNode)tn).preorderEnumeration();
                }
            }



            //for (int i = 0; i < tn.Nodes.Count; i++)
            //{
            //    VTreeNode node = (VTreeNode)tn.Nodes[i];
            //    if (node.IsSummary && node.Nodes.Count < 1)
            //    {
            //        //m_root.Nodes.Remove(node);

            //        //if (barNodes.Contains(node))
            //        //{
            //        //    barNodes.Remove(node);

            //        //}
            //        i = -1;
            //    }
            //    else
            //    {
            //        if (node.Nodes.Count > 0)
            //        {
            //            TrimTree(node);

            //        }
            //    }
            //}
        }

        /// <summary>
        /// Get Source TableName (i.e. where to get the name and color)
        /// </summary>
        /// <param name="tableNameOnly">if false return From clause (alias = t)</param>
        /// <returns>source table name, e.g. AD_Org or null</returns>
        public String GetSourceTableName(bool tableNameOnly)
        {
            int AD_Table_ID = GetAD_Table_ID();
            String tableName = MTable.GetTableName(GetCtx(), AD_Table_ID);
            //
            if (tableNameOnly)
                return tableName;
            if ("M_Product".Equals(tableName))
                return "M_Product M_Product INNER JOIN M_Product_Category x ON (M_Product.M_Product_Category_ID=x.M_Product_Category_ID)";
            if ("C_BPartner".Equals(tableName))
                return "C_BPartner C_BPartner INNER JOIN C_BP_Group x ON (C_BPartner.C_BP_Group_ID=x.C_BP_Group_ID)";
            if ("AD_Org".Equals(tableName))
                return "AD_Org AD_Org INNER JOIN AD_OrgInfo i ON (AD_Org.AD_Org_ID=i.AD_Org_ID) "
                    + "LEFT OUTER JOIN AD_OrgType x ON (i.AD_OrgType_ID=x.AD_OrgType_ID)";
            if ("C_Campaign".Equals(tableName))
                return "C_Campaign C_Campaign LEFT OUTER JOIN C_Channel x ON (C_Campaign.C_Channel_ID=x.C_Channel_ID)";
            if (tableName != null)
                tableName += (" " + tableName);
            return tableName;
        }




        /// <summary>
        /// Is Business Partner Tree
        /// </summary>
        /// <returns>true if partner</returns>
        public bool IsBPartner()
        {
            return TREETYPE_BPartner.Equals(GetTreeType());
        }

        /// <summary>
        /// true if product
        /// </summary>
        /// <returns>true if menu</returns>
        public bool IsMenu()
        {
            return TREETYPE_Menu.Equals(GetTreeType());
        }

        /// <summary>
        /// Is Product Tree
        /// </summary>
        /// <returns>true if product</returns>
        public bool IsProduct()
        {
            return TREETYPE_Product.Equals(GetTreeType());
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
        /// Get fully qualified Name of Action/Color Column
        /// </summary>
        /// <returns>NULL or Action or Color</returns>
        public String GetActionColorName()
        {
            int AD_Table_ID = GetAD_Table_ID();
            String tableName = MTable.GetTableName(GetCtx(), AD_Table_ID);
            //
            if ("AD_Menu".Equals(tableName))
                return "t.Action";
            if ("M_Product".Equals(tableName) || "C_BPartner".Equals(tableName)
                || "AD_Org".Equals(tableName) || "C_Campaign".Equals(tableName))
                return "x.AD_PrintColor_ID";
            return "NULL";
        }

        /// <summary>
        /// Get Root Node of tree
        /// </summary>
        /// <returns>root node</returns>
        public VTreeNode GetRootNode()
        {
            return m_root;
        }


        //public CTreeNode GetRootNode1()
        //{
        //    return m_root;
        //}
        /// <summary>
        /// Get Bar Nodes
        /// </summary>
        /// <returns>nodes in favorite tree</returns>
        public List<VTreeNode> GetBarNodes()
        {
            return barNodes;
        }

        #endregion

        #region "StartMenu"
        #region "Declaration"

        //private List<VToolStripMenuItem> startMenuCache = new List<VToolStripMenuItem>(); //menu nodes
        //private List<VToolStripMenuItem> startFavoriteCache = new List<VToolStripMenuItem>(); //favourite nodes

        //private List<VToolStripMenuItem> startLeafnodeCache = new List<VToolStripMenuItem>();

        #endregion




        //public VToolStripMenuItem StartMenuRoot
        //{
        //    get;
        //    set;
        //}

        //private StartMenu.IToolStripItem _itemClickHandler;

        //public List<VToolStripMenuItem> StartMenuItems
        //{
        //    get;
        //    set;
        //}

        // private System.Windows.Forms.ImageList _imgList;

        /// <summary>
        /// Construct & Load Tree
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Tree_ID">The tree to build</param>
        /// <param name="editable">True, if tree can be modified - includes inactive and empty summary nodes</param>
        /// <param name="clientTree">the tree is displayed on the java client (not on web)</param>
        /// <param name="trxName">transaction</param>

        //public MTree(Ctx ctx, int AD_Tree_ID, bool editable, bool clientTree, Trx trxName, System.Windows.Forms.ImageList imgList, StartMenu.IToolStripItem source)
        //    : this(ctx, AD_Tree_ID, trxName)
        //{
        //    _imgList = imgList;
        //    _itemClickHandler = source;
        //    m_editable = editable;
        //    int AD_User_ID = ctx.GetAD_User_ID();
        //    m_clientTree = clientTree;
        //    log.Info("AD_Tree_ID=" + AD_Tree_ID + ", AD_User_ID=" + AD_User_ID
        //        + ", Editable=" + editable + ", OnClient=" + clientTree);
        //    //
        //    LoadStartMenu(AD_User_ID);
        //}

        /// <summary>
        /// Load Nodes and Bar
        /// </summary>
        /// <param name="AD_User_ID">user for tree bar</param>
        //private void LoadStartMenu(int AD_User_ID)
        //{
        //    ////  SQL for TreeNodes
        //    StringBuilder sql = new StringBuilder("SELECT "
        //        + "tn.Node_ID,tn.Parent_ID,tn.SeqNo,tb.IsActive "
        //        + "FROM ").Append(GetNodeTableName()).Append(" tn"
        //        + " LEFT OUTER JOIN AD_TreeBar tb ON (tn.AD_Tree_ID=tb.AD_Tree_ID"
        //        + " AND tn.Node_ID=tb.Node_ID AND tb.AD_User_ID=@userid) "	//	#1
        //        + "WHERE tn.AD_Tree_ID=@treeId");								//	#2
        //    if (!m_editable)
        //        sql.Append(" AND tn.IsActive='Y'");
        //    sql.Append(" ORDER BY COALESCE(tn.Parent_ID, -1), tn.SeqNo");
        //    log.Finest(sql.ToString());

        //    //  The Node Loop
        //    IDataReader dr = null;
        //    try
        //    {
        //        // load Node details - addToTree -> getNodeDetail
        //        GetNodeDetails();
        //        //
        //        SqlParameter[] param = new SqlParameter[2];
        //        param[0] = new SqlParameter("@userid", AD_User_ID);
        //        param[1] = new SqlParameter("@treeId", GetAD_Tree_ID());
        //        //PreparedStatement pstmt = DataBase.prepareStatement(sql.toString(), get_TrxName());

        //        //    //	Get Tree & Bar

        //        dr = ExecuteQuery.ExecuteReader(sql.ToString(), param);
        //        // m_root = new VTreeNode(0, 0, GetName(), GetDescription(), 0, true, null, false);//, null);
        //        StartMenuRoot = new VToolStripMenuItem(0, 0, GetName(), GetDescription(), 0, true, null, false, GetImage("C", true), null);

        //        while (dr.Read())
        //        {
        //            int node_ID = Utility.Util.GetValueOfInt(dr[0]);
        //            int parent_ID = 0;
        //            if (dr["Parent_ID"].ToString() != "")
        //                parent_ID = Utility.Util.GetValueOfInt(dr[1]);
        //            int seqNo = Utility.Util.GetValueOfInt(dr[2]);
        //            bool onBar = dr[3].ToString() != "";

        //            if (node_ID == 0 && parent_ID == 0)
        //            { }
        //            else
        //                AddToStartMenu(node_ID, parent_ID, seqNo, onBar);	//	calls getNodeDetail
        //        }
        //        dr.Close();

        //    }
        //    catch (Exception e)
        //    {
        //        log.Log(Level.SEVERE, sql.ToString(), e);
        //        if (dr != null)
        //        {
        //            dr.Close();

        //        }
        //        if (dt != null)
        //        {
        //            dt.Clear();
        //            dt = null;
        //        }
        //    }
        //    ////  Done with loading - add remainder from buffer
        //    // Get From list
        //    log.Finest("clearing buffer - Adding to****************: " + m_root);
        //    for (int i = 0; i < startMenuCache.Count; i++)
        //    {
        //        //log.Finest("clearing buffer - Adding to: " + m_root);
        //        VToolStripMenuItem node = startMenuCache[i];
        //        VToolStripMenuItem parent = FindMenuItem(StartMenuRoot.DropDown.Items, node.Parent_ID.ToString());
        //        if (parent != null)
        //        {

        //            parent.DropDown.Items.Add(node);
        //            //if (parent.FirstNode.Level > maxLevel)
        //            // {
        //            //    maxLevel = parent.FirstNode.Level;
        //            // }
        //            //CheckList(node);
        //            startMenuCache.RemoveAt(i);
        //            i = -1;		//	start again with i=0
        //        }
        //    }

        //    //	Nodes w/o parent
        //    if (startMenuCache.Count != 0)
        //    {
        //        log.Severe("Nodes w/o parent - adding to root - " + _buffer);
        //        for (int i = 0; i < startMenuCache.Count; i++)
        //        {
        //            VToolStripMenuItem node = startMenuCache[i];
        //            StartMenuRoot.DropDown.Items.Add(node);
        //            startMenuCache.RemoveAt(i);
        //            i = -1;
        //        }
        //        if (_buffer.Count != 0)
        //        {
        //            log.Severe("Still nodes in Buffer - " + _buffer);
        //        }
        //    }
        //    //  clean up
        //    if (!m_editable && StartMenuRoot.DropDown.Items.Count > 0)
        //        TrimStartMenu(StartMenuRoot);
        //    ////		diagPrintTree();
        //    if (VLogMgt.IsLevelFinest() || StartMenuRoot.DropDown.Items.Count == 0)
        //        log.Fine("ChildCount=" + StartMenuRoot.DropDown.Items.Count);
        //}



        private System.Drawing.Image GetImage(string imgKey, bool isSummary)
        {
            if (imgKey == null)
            {
                imgKey = "";
            }
            if (isSummary)
            {
                imgKey = "C";
            }
            return null; //_imgList.Images[imgKey];
        }

        /// <summary>
        /// Add Node to root node  and tree bar(favorite) list 
        /// </summary>
        /// <param name="Node_ID">node key id</param>
        /// <param name="Parent_ID">parent node id </param>
        /// <param name="seqNo"> seq no</param>
        /// <param name="onBar">node is in favorite tree or not</param>
        //private void AddToStartMenu(int Node_ID, int Parent_ID, int seqNo, bool onBar)
        //{
        //    //  Create new Node
        //    VToolStripMenuItem child = GetStartMenuDetail(Node_ID, Parent_ID, seqNo, onBar);

        //    if (child == null)
        //        return;

        //    //  Add to Tree
        //    VToolStripMenuItem parent = null;
        //    if (onBar == true) // if in favorite (short -cut)
        //    {
        //        VToolStripMenuItem childBar = GetStartMenuDetail(Node_ID, Parent_ID, seqNo, onBar);
        //        startFavoriteCache.Insert(startFavoriteCache.Count, childBar);
        //    }

        //    if (StartMenuRoot != null)
        //    {
        //        if (child.Parent_ID == 0)
        //        {
        //            StartMenuRoot.DropDown.Items.Add(child);
        //        }
        //        else
        //        {
        //            if (startMenuCache.Count > 0)
        //            {
        //                for (int i = 0; i < startMenuCache.Count; i++)
        //                {
        //                    VToolStripMenuItem node = startMenuCache[i];// FindTreeNode(mnuNodes[i].no
        //                    if (child.Parent_ID == node.Node_ID)
        //                    {
        //                        node.DropDown.Items.Add(child);
        //                        return;
        //                    }
        //                    if (node.DropDown.Items.Count > 0)
        //                    {
        //                        VToolStripMenuItem parentnode = FindMenuItem(node.DropDown.Items, child.Parent_ID.ToString());
        //                        if (parentnode != null)
        //                        {
        //                            parentnode.DropDown.Items.Add(child);
        //                            return;
        //                        }
        //                    }
        //                }
        //            }

        //            startMenuCache.Add(child);
        //        }   //  addToTree
        //    }
        //}

        //public List<VToolStripMenuItem> GetFavoriteNodes()
        //{
        //    return startFavoriteCache;
        //}

        /// <summary>
        /// set node properties
        /// </summary>
        /// <param name="Node_ID">node key id</param>
        /// <param name="Parent_ID">parent node id</param>
        /// <param name="seqNo">seq no</param>
        /// <param name="onBar">node in favorite tree</param>
        /// <returns>VTreeNode </returns>
        //private VToolStripMenuItem GetStartMenuDetail(int Node_ID, int Parent_ID, int seqNo, bool onBar)
        //{
        //    int AD_Window_ID = 0;
        //    int AD_Process_ID = 0;
        //    int AD_Form_ID = 0;
        //    int AD_Workflow_ID = 0;
        //    int AD_Task_ID = 0;
        //    int AD_Workbench_ID = 0;

        //    VToolStripMenuItem retValue = null;

        //    // Serch For Node details
        //    try
        //    {
        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            int node1 = int.Parse(dt.Rows[i][0].ToString());
        //            if (Node_ID != node1)	//	search for correct one
        //            {
        //                continue;
        //            }
        //            DataRow dr = dt.Rows[i];

        //            int node = int.Parse(dr[0].ToString());
        //            int index = 1;


        //            string name = dr[index++].ToString();

        //            string description = dr[index++].ToString();
        //            bool isSummary = "Y".Equals(dr[index++].ToString());
        //            string actionColor = dr[index++].ToString().Trim();
        //            //	Menu only
        //            if (GetTreeType().Equals(TREETYPE_Menu) && !isSummary)
        //            {
        //                AD_Window_ID = Utility.Util.GetValueOfInt(dr[index]);
        //                index++;
        //                AD_Process_ID = Utility.Util.GetValueOfInt(dr[index]);
        //                index++;
        //                AD_Form_ID = Utility.Util.GetValueOfInt(dr[index]);
        //                index++;
        //                AD_Workflow_ID = Utility.Util.GetValueOfInt(dr[index]);
        //                index++;
        //                AD_Task_ID = Utility.Util.GetValueOfInt(dr[index]);
        //                index++;
        //                AD_Workbench_ID = Utility.Util.GetValueOfInt(dr[index]);
        //                index++;
        //                //
        //                MRole role = MRole.GetDefault(GetCtx(), false);
        //                bool? blnAccess = false;


        //                if (X_AD_Menu.ACTION_Window.Equals(actionColor))
        //                    blnAccess = role.GetWindowAccess(AD_Window_ID);
        //                else if (X_AD_Menu.ACTION_Process.Equals(actionColor)
        //                || X_AD_Menu.ACTION_Report.Equals(actionColor))
        //                    blnAccess = role.GetProcessAccess(AD_Process_ID);
        //                else if (X_AD_Menu.ACTION_Form.Equals(actionColor))
        //                    blnAccess = role.GetFormAccess(AD_Form_ID);
        //                else if (X_AD_Menu.ACTION_WorkFlow.Equals(actionColor))
        //                    blnAccess = role.GetWorkflowAccess(AD_Workflow_ID);
        //                else if (X_AD_Menu.ACTION_Task.Equals(actionColor))
        //                    blnAccess = role.GetTaskAccess(AD_Task_ID);

        //                if (blnAccess != null || m_editable)		//	rw or ro for Role 
        //                {
        //                    retValue = new VToolStripMenuItem(Node_ID, seqNo,
        //                        name, description, Parent_ID, isSummary,
        //                        actionColor, onBar, GetImage(actionColor, isSummary), _itemClickHandler);	//	menu has no color
        //                }
        //            }
        //            else
        //            {
        //                //Color color = null;	//	action
        //                //if (actionColor != null && !getTreeType().equals(TREETYPE_Menu))
        //                //{
        //                //    MPrintColor printColor = MPrintColor.get(getCtx(), actionColor);
        //                //    if (printColor != null)
        //                //        color = printColor.getColor();
        //                //}

        //                retValue = new VToolStripMenuItem(Node_ID, seqNo,
        //                        name, description, Parent_ID, isSummary,
        //                        actionColor, onBar, GetImage(actionColor, isSummary), _itemClickHandler);
        //            }
        //            break;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        log.Log(Level.SEVERE, "", e);
        //    }
        //    if (retValue != null)
        //    {
        //        // set VTreeNode ID's
        //        retValue.AD_Window_ID = AD_Window_ID;
        //        retValue.AD_Process_ID = AD_Process_ID;
        //        retValue.AD_Form_ID = AD_Form_ID;
        //        retValue.AD_Workflow_ID = AD_Workflow_ID;
        //        retValue.AD_Task_ID = AD_Task_ID;
        //        retValue.AD_Workbench_ID = AD_Workbench_ID;
        //    }
        //    return retValue;
        //}


        ///// <summary>
        ///// Seach for tree node
        ///// </summary>
        ///// <param name="tNodes">TreeNode Collection</param>
        ///// <param name="strFind">node name</param>
        ///// <returns>VTreeNode</returns>
        //private VToolStripMenuItem FindMenuItem(ToolStripItemCollection tItems, string strFind)
        //{
        //    foreach (ToolStripItem tn in tItems)
        //    {
        //        if (tn.Name == strFind)
        //        {
        //            return (VToolStripMenuItem)tn;
        //        }
        //        //if has Child Nodes
        //        if (((VToolStripMenuItem)tn).DropDown.Items.Count > 0)
        //        {
        //            //Call recursive
        //            VToolStripMenuItem match = FindMenuItem(((VToolStripMenuItem)tn).DropDown.Items, strFind);
        //            if (match != null)
        //            {
        //                return (VToolStripMenuItem)match;
        //            }
        //        }
        //    }
        //    return null;
        //}
        ///// <summary>
        /// remove summery node having no children
        /// </summary>
        /// <param name="tn">tree node</param>
        //private void TrimStartMenu(System.Windows.Forms.ToolStripMenuItem tn)
        //{

        //    System.Collections.IEnumerator en = ((VToolStripMenuItem)tn).preorderMenuEnumeration();

        //    while (en.MoveNext())
        //    {
        //        Object current = en.Current;
        //        if (current == null)
        //        {
        //            continue;
        //        }
        //        VToolStripMenuItem node = (VToolStripMenuItem)current;
        //        if (node.IsSummary && node.DropDown.Items.Count < 1)
        //        {
        //            //if (node.OwnerItem != null)
        //            //{
        //            ToolStripMenuItem parent = (ToolStripMenuItem)node.OwnerItem;
        //            if (parent != null)
        //                parent.DropDown.Items.Remove(node);
        //            //}

        //            if (startFavoriteCache.Contains(node))
        //            {
        //                startFavoriteCache.Remove(node);
        //            }
        //        }
        //        else if (!node.IsSummary)
        //        {

        //            startLeafnodeCache.Add(node.Clone());
        //        }
        //    }

        //    //for (int i = 0; i < tn.Nodes.Count; i++)
        //    //{
        //    //    VTreeNode node = (VTreeNode)tn.Nodes[i];
        //    //    if (node.IsSummary && node.Nodes.Count < 1)
        //    //    {
        //    //        //m_root.Nodes.Remove(node);

        //    //        //if (barNodes.Contains(node))
        //    //        //{
        //    //        //    barNodes.Remove(node);

        //    //        //}
        //    //        i = -1;
        //    //    }
        //    //    else
        //    //    {
        //    //        if (node.Nodes.Count > 0)
        //    //        {
        //    //            TrimTree(node);

        //    //        }
        //    //    }
        //    //}
        //}

        #endregion

        //public List<VToolStripMenuItem> LeafNodes
        //{
        //    get { return startLeafnodeCache; }
        //}




    }


    public class SearchItem
    {
        public String Name
        {
            get;
            set;
        }

        public String ActionIndicator
        {
            get;
            set;
        }


        public string Description
        {
            get;
            set;
        }

        //public BitmapImage ImageSource
        //{
        //    get;
        //    set;
        //}

        public int ID
        {
            get;
            set;
        }

        public int ActionID
        {
            get;
            set;
        }

        public SearchItem(VAdvantage.Classes.VTreeNode node)
        {
            Name = node.SetName;
            Description = node.GetDescription;
            // ImageSource = Utility.Envs.LoadImageSource(node.GetImageName(node.GetImageIndicator));
            ID = node.GetNode_ID();
            ActionIndicator = node.GetAction();
            ActionID = node.GetActionID();
        }


    }

}
