/********************************************************
 * Module Name    : Tree
 * Purpose        : 
 * Class Used     : X_VAF_TreeInfo
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
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Windows.Forms;
//using VAdvantage.StartMenu;


namespace VAdvantage.Model
{
    public class MTree : X_VAF_TreeInfo
    {
        #region "Mtree TreeNode

        /**	Cache						*/
        private static CCache<int, MTree> s_cache = new CCache<int, MTree>("VAF_TreeInfo", 10);

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
        private int VAF_Tab_ID = 0;
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
		    X_VAB_BillingCode.Table_ID,
		    X_M_BOM.Table_ID,
		    X_VAB_BusinessPartner.Table_ID,
		    X_CM_Container.Table_ID,
		    X_CM_Media.Table_ID,
		    X_CM_CStage.Table_ID,
		    X_VACM_Layout.Table_ID,
		    X_VAB_Acct_Element.Table_ID,
		    X_VAB_Promotion.Table_ID,
		    X_VAF_MenuConfig.Table_ID,
		    X_VAF_Org.Table_ID,
		    X_M_Product_Category.Table_ID,
		    X_VAB_Project.Table_ID,
		    X_M_Product.Table_ID,
		    X_VAB_SalesRegionState.Table_ID,
		    0,0,0,0,0
	    };

        /// <summary>
        /// Default Constructor. Need to call loadNodes explicitly
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_TreeInfo_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MTree(Ctx ctx, int VAF_TreeInfo_ID, Trx trxName)
            : base(ctx, VAF_TreeInfo_ID, trxName)
        {
            if (VAF_TreeInfo_ID == 0)
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
        /// <param name="VAF_TreeInfo_ID">The tree to build</param>
        /// <param name="editable">True, if tree can be modified - includes inactive and empty summary nodes</param>
        /// <param name="clientTree">the tree is displayed on the java client (not on web)</param>
        /// <param name="trxName">transaction</param>
        public MTree(Ctx ctx, int VAF_TreeInfo_ID, bool editable, bool clientTree, Trx trxName, bool onDemand, int VAF_Tab_ID, int windowNO)
            : this(ctx, VAF_TreeInfo_ID, trxName)
        {
            this.VAF_Tab_ID = VAF_Tab_ID;
            this.onDemand = onDemand;
            this.windowNo = windowNO;
            m_editable = editable;
            int VAF_UserContact_ID = ctx.GetVAF_UserContact_ID();
            m_clientTree = clientTree;
            log.Info("VAF_TreeInfo_ID=" + VAF_TreeInfo_ID + ", VAF_UserContact_ID=" + VAF_UserContact_ID
                + ", Editable=" + editable + ", OnClient=" + clientTree);
            //
            LoadNodes(VAF_UserContact_ID);
        }

        /// <summary>
        /// Construct & Load Tree
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_TreeInfo_ID">The tree to build</param>
        /// <param name="editable">True, if tree can be modified - includes inactive and empty summary nodes</param>
        /// <param name="clientTree">the tree is displayed on the java client (not on web)</param>
        /// <param name="trxName">transaction</param>
        public MTree(Ctx ctx, int VAF_TreeInfo_ID, bool editable, bool clientTree, Trx trxName, bool onDemand)
            : this(ctx, VAF_TreeInfo_ID, trxName)
        {
            this.VAF_Tab_ID = VAF_Tab_ID;
            this.onDemand = onDemand;
            m_editable = editable;
            int VAF_UserContact_ID = ctx.GetVAF_UserContact_ID();
            m_clientTree = clientTree;
            log.Info("VAF_TreeInfo_ID=" + VAF_TreeInfo_ID + ", VAF_UserContact_ID=" + VAF_UserContact_ID
                + ", Editable=" + editable + ", OnClient=" + clientTree);
            //
            LoadNodes(VAF_UserContact_ID);
        }


        /// <summary>
        /// Construct & Load Tree
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_TreeInfo_ID">The tree to build</param>
        /// <param name="editable">True, if tree can be modified - includes inactive and empty summary nodes</param>
        /// <param name="clientTree">the tree is displayed on the java client (not on web)</param>
        /// <param name="trxName">transaction</param>
        public MTree(Ctx ctx, int VAF_TreeInfo_ID, bool editable, bool clientTree, Trx trxName, int nodeid)
            : this(ctx, VAF_TreeInfo_ID, trxName)
        {
            this.VAF_Tab_ID = VAF_Tab_ID;
            m_editable = editable;
            currentNodeID = nodeid;
            int VAF_UserContact_ID = ctx.GetVAF_UserContact_ID();
            m_clientTree = clientTree;
            log.Info("VAF_TreeInfo_ID=" + VAF_TreeInfo_ID + ", VAF_UserContact_ID=" + VAF_UserContact_ID
                + ", Editable=" + editable + ", OnClient=" + clientTree);
            //
            LoadNodes(VAF_UserContact_ID);
        }

        /// <summary>
        /// Construct & Load Tree
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_TreeInfo_ID">The tree to build</param>
        /// <param name="editable">True, if tree can be modified - includes inactive and empty summary nodes</param>
        /// <param name="clientTree">the tree is displayed on the java client (not on web)</param>
        /// <param name="trxName">transaction</param>
        public MTree(Ctx ctx, int VAF_TreeInfo_ID, bool editable, bool clientTree, Trx trxName, int nodeid, int VAF_Tab_ID)
            : this(ctx, VAF_TreeInfo_ID, trxName)
        {
            this.VAF_Tab_ID = VAF_Tab_ID;
            m_editable = editable;
            currentNodeID = nodeid;
            int VAF_UserContact_ID = ctx.GetVAF_UserContact_ID();
            m_clientTree = clientTree;
            log.Info("VAF_TreeInfo_ID=" + VAF_TreeInfo_ID + ", VAF_UserContact_ID=" + VAF_UserContact_ID
                + ", Editable=" + editable + ", OnClient=" + clientTree);
            //
            LoadNodes(VAF_UserContact_ID);
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
            SetVAF_TableView_ID();
            SetIsAllNodes(true);	//	complete tree
            SetIsDefault(false);
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="client">client</param>
        /// <param name="name">name</param>
        /// <param name="treeType">tree type</param>
        public MTree(X_VAF_Client client, String name, String treeType)
            : this(client.GetCtx(), 0, client.Get_TrxName())
        {
            SetClientOrg(client);
            SetName(name);
            SetTreeType(treeType);
            SetVAF_TableView_ID();
        }

        /// <summary>
        /// Construct & Load Tree
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_TreeInfo_ID">The tree to build</param>
        /// <param name="editable">True, if tree can be modified - includes inactive and empty summary nodes</param>
        /// <param name="clientTree">the tree is displayed on the java client (not on web)</param>
        /// <param name="trxName">transaction</param>
        public MTree(Ctx ctx, int VAF_TreeInfo_ID, bool editable, bool clientTree, Trx trxName)
            : this(ctx, VAF_TreeInfo_ID, trxName)
        {
            m_editable = editable;
            int VAF_UserContact_ID = ctx.GetVAF_UserContact_ID();
            m_clientTree = clientTree;
            log.Info("VAF_TreeInfo_ID=" + VAF_TreeInfo_ID + ", VAF_UserContact_ID=" + VAF_UserContact_ID
                + ", Editable=" + editable + ", OnClient=" + clientTree);
            //
            LoadNodes(VAF_UserContact_ID);
        }

        /// <summary>
        /// Get MTree_Base from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_TreeInfo_ID">id</param>
        /// <param name="trxName">transaction</param>
        /// <returns>MTree_Base</returns>
        public static MTree Get(Ctx ctx, int VAF_TreeInfo_ID, Trx trxName)
        {
            int key = VAF_TreeInfo_ID;
            MTree retValue = (MTree)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MTree(ctx, VAF_TreeInfo_ID, trxName);
            if (retValue.Get_ID() != 0)
                s_cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Get default (oldest) complete VAF_TreeInfo_ID for KeyColumn.
        /// </summary>
        /// <param name="VAF_Client_ID">client id</param>
        /// <param name="VAF_TableView_ID">table id</param>
        /// <returns>VAF_TreeInfo_ID or 0</returns>
        public static int GetDefaultVAF_TreeInfo_ID(int VAF_Client_ID, int VAF_TableView_ID)
        {
            _log.Finer("VAF_TableView_ID=" + VAF_TableView_ID);
            if (VAF_TableView_ID == 0)
                return 0;
            int VAF_TreeInfo_ID = 0;
            String sql = "SELECT VAF_TreeInfo_ID, Name FROM VAF_TreeInfo "
                + "WHERE VAF_Client_ID=" + VAF_Client_ID + " AND VAF_TableView_ID=" + VAF_TableView_ID + " AND IsActive='Y' AND IsAllNodes='Y' "
                + "ORDER BY IsDefault DESC, VAF_TreeInfo_ID";

            IDataReader dr = null;
            try
            {
                dr = CoreLibrary.DataBase.DB.ExecuteReader(sql, null, null);
                if (dr.Read())
                {
                    if (dr["VAF_TreeInfo_ID"] != null && dr["VAF_TreeInfo_ID"].ToString() != "")
                        VAF_TreeInfo_ID = Utility.Util.GetValueOfInt(dr["VAF_TreeInfo_ID"].ToString());
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
            return VAF_TreeInfo_ID;
        }

        /// <summary>
        /// Get default (oldest) complete VAF_TreeInfo_ID for KeyColumn.
        /// </summary>
        /// <param name="VAF_Client_ID">client id</param>
        /// <param name="tableName">table name</param>
        /// <returns>VAF_TreeInfo_ID or 0</returns>
        public static int GetDefaultVAF_TreeInfo_ID(int VAF_Client_ID, String tableName)
        {
            _log.Finer("TableName=" + tableName);
            if (tableName == null)
                return 0;
            int VAF_TreeInfo_ID = 0;
            String sql = "SELECT tr.VAF_TreeInfo_ID, tr.Name "
                + "FROM VAF_TreeInfo tr INNER JOIN VAF_TableView tb ON (tr.VAF_TableView_ID=tb.VAF_TableView_ID) "
                + "WHERE tr.VAF_Client_ID=@clientid AND tb.TableName=@tablename AND tr.IsActive='Y' AND tr.IsAllNodes='Y' "
                + "ORDER BY tr.IsDefault DESC, tr.VAF_TreeInfo_ID";

            SqlParameter[] param = new SqlParameter[2];
            param[0] = new SqlParameter("@clientid", VAF_Client_ID);
            param[1] = new SqlParameter("@tablename", tableName);
            IDataReader dr = null;
            try
            {
                dr = ExecuteQuery.ExecuteReader(sql, param);
                if (dr.Read())
                {
                    if (dr["VAF_TreeInfo_ID"] != null && dr["VAF_TreeInfo_ID"].ToString() != "")
                        VAF_TreeInfo_ID = Utility.Util.GetValueOfInt(dr["VAF_TreeInfo_ID"].ToString());
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
            return VAF_TreeInfo_ID;
        }

        /// <summary>
        /// Get Node TableName
        /// </summary>
        /// <param name="treeType">tree type</param>
        /// <returns>node table name, e.g. VAF_TreeInfoChild</returns>
        static String GetNodeTableName(String treeType)
        {
            String nodeTableName = "VAF_TreeInfoChild";
            if (TREETYPE_Menu.Equals(treeType))
                nodeTableName += "Menu";
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
        /// <param name="VAF_TableView_ID">table id</param>
        /// <returns>e name, e.g. VAF_TreeInfoChild</returns>
        static public String GetNodeTableName(int VAF_TableView_ID, Ctx ctx)
        {
            String nodeTableName = "VAF_TreeInfoChild";
            if (X_VAF_MenuConfig.Table_ID == VAF_TableView_ID)
                nodeTableName += "MM";
            else if (X_VAB_BusinessPartner.Table_ID == VAF_TableView_ID)
                nodeTableName += "BP";
            else if (X_M_Product.Table_ID == VAF_TableView_ID)
                nodeTableName += "PR";
            //
            else if (X_CM_Container.Table_ID == VAF_TableView_ID)
                nodeTableName += "CMC";
            else if (X_CM_CStage.Table_ID == VAF_TableView_ID)
                nodeTableName += "CMS";
            else if (X_CM_Media.Table_ID == VAF_TableView_ID)
                nodeTableName += "CMM";
            else if (X_VACM_Layout.Table_ID == VAF_TableView_ID)
                nodeTableName += "CMT";
            else
            {
                if (s_TableIDs == null)
                    FillUserTables(null, ctx);
                int ii = VAF_TableView_ID;
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
        /// <param name="VAF_TableView_ID">table id</param>
        /// <returns>true if table has tree</returns>
        public static bool HasTree(int VAF_TableView_ID, Ctx ctx)
        {
            if (s_TableIDs == null)
                FillUserTables(null, ctx);
            return s_TableIDs.Contains(VAF_TableView_ID);
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
            String sql = "SELECT DISTINCT TreeType, VAF_TableView_ID FROM VAF_TreeInfo";

            IDataReader dr = null;
            try
            {
                dr = CoreLibrary.DataBase.DB.ExecuteReader(sql, null, trxName);
                while (dr.Read())
                {
                    String TreeType = dr["TreeType"].ToString();//rs.getString(1);
                    int VAF_TableView_ID = 0;
                    if (dr["VAF_TableView_ID"] != null && dr["VAF_TableView_ID"].ToString() != "")
                    {
                        VAF_TableView_ID = Utility.Util.GetValueOfInt(dr["VAF_TableView_ID"].ToString());//rs.getInt(2);
                    }
                    if (VAF_TableView_ID == 0)
                        continue;
                    s_TableIDs.Add(VAF_TableView_ID);		//	all
                    if (TreeType.Equals("U1"))
                        s_TableIDs_U1.Add(VAF_TableView_ID);
                    else if (TreeType.Equals("U2"))
                        s_TableIDs_U2.Add(VAF_TableView_ID);
                    else if (TreeType.Equals("U3"))
                        s_TableIDs_U3.Add(VAF_TableView_ID);
                    else if (TreeType.Equals("U4"))
                        s_TableIDs_U4.Add(VAF_TableView_ID);
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
            SetVAF_TableView_ID();
            for (int i = 0; i < TREETYPES.Length; i++)
                UpdateTrees(TREETYPES[i], TABLEIDS[i]);
        }

        /// <summary>
        /// Update Trees
        /// </summary>
        /// <param name="treeType">tree type</param>
        /// <param name="VAF_TableView_ID">table id</param>
        private void UpdateTrees(String treeType, int VAF_TableView_ID)
        {
            if (VAF_TableView_ID == 0)
                return;
            StringBuilder sb = new StringBuilder("UPDATE VAF_TreeInfo SET VAF_TableView_ID=")
                .Append(VAF_TableView_ID)
                .Append(" WHERE TreeType='").Append(treeType).Append("' AND VAF_TableView_ID IS NULL");
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
        private int SetVAF_TableView_ID()
        {
            int VAF_TableView_ID = 0;
            String type = GetTreeType();
            if (type == null
                || type.StartsWith("U")	//	User
                || type.Equals(TREETYPE_Other))
                return 0;
            for (int i = 0; i < TREETYPES.Length; i++)
            {
                if (type.Equals(TREETYPES[i]))
                {
                    VAF_TableView_ID = TABLEIDS[i];
                    break;
                }
            }
            if (VAF_TableView_ID != 0)
                SetVAF_TableView_ID(VAF_TableView_ID);
            if (VAF_TableView_ID == 0)
            {
                log.Warning("Did not find Table for TreeType=" + type);
            }
            return VAF_TableView_ID;
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
            if (GetVAF_TableView_ID(true) == 0)
            {
                if (newRecord)
                    SetVAF_TableView_ID();
                else
                    UpdateTrees();
                //
                if (GetVAF_TableView_ID(true) == 0)
                {
                    log.Warning("No Table for " + ToString());
                    return false;
                }
            }
            return Validate();
        }

        /// <summary>
        /// Get VAF_TableView_ID
        /// </summary>
        /// <returns>table id</returns>
        public new int GetVAF_TableView_ID()
        {
            int VAF_TableView_ID = base.GetVAF_TableView_ID();
            if (VAF_TableView_ID == 0)
                VAF_TableView_ID = SetVAF_TableView_ID();
            return VAF_TableView_ID;
        }

        /// <summary>
        /// Get VAF_TableView_ID
        /// </summary>
        /// <param name="isBase">base</param>
        /// <returns>table id</returns>
        public int GetVAF_TableView_ID(bool isBase)
        {
            if (isBase)
                return base.GetVAF_TableView_ID();
            return GetVAF_TableView_ID();
        }

        /// <summary>
        /// Validate TreeType and VAF_TableView_ID
        /// </summary>
        /// <returns>true if Tree Type compatible with VAF_TableView_ID</returns>
        private bool Validate()
        {
            String type = GetTreeType();
            if (type != null
                    && (type.StartsWith("U") || type.Equals(TREETYPE_Other)))
                return true;
            //
            int VAF_TableView_ID = GetVAF_TableView_ID(true);
            for (int i = 0; i < TREETYPES.Length; i++)
            {
                if (type == null)
                {
                    if (VAF_TableView_ID == TABLEIDS[i])
                    {
                        SetTreeType(TREETYPES[i]);
                        return true;
                    }
                }
                else if (VAF_TableView_ID == TABLEIDS[i])
                {
                    if (type.Equals(TREETYPES[i]))
                        return true;
                    else
                    {
                        SetTreeType(TREETYPES[i]);
                        return true;
                    }
                }
                else if (VAF_TableView_ID == 0 && type.Equals(TREETYPES[i]))
                {
                    SetVAF_TableView_ID(TABLEIDS[i]);
                    return true;
                }
            }
            //	None found
            if (type == null)
            {
                SetTreeType(TREETYPE_Other);
                return true;
            }
            log.Warning("TreeType=" + type + " <> VAF_TableView_ID=" + VAF_TableView_ID);
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
                sourceTable = "VAF_MenuConfig";
                if (baseLang)
                    sqlNode.Append("SELECT VAF_MenuConfig.VAF_MenuConfig_ID, VAF_MenuConfig.Name,VAF_MenuConfig.Description,VAF_MenuConfig.IsSummary,VAF_MenuConfig.Action, "
                        + "VAF_MenuConfig.VAF_Screen_ID, VAF_MenuConfig.VAF_Job_ID, VAF_MenuConfig.VAF_Page_ID, VAF_MenuConfig.VAF_Workflow_ID, VAF_MenuConfig.VAF_Task_ID, VAF_MenuConfig.AD_Workbench_ID "
                        + "FROM VAF_MenuConfig VAF_MenuConfig");
                else
                    sqlNode.Append("SELECT VAF_MenuConfig.VAF_MenuConfig_ID,  t.Name,t.Description,VAF_MenuConfig.IsSummary,VAF_MenuConfig.Action, "
                        + "VAF_MenuConfig.VAF_Screen_ID, VAF_MenuConfig.VAF_Job_ID, VAF_MenuConfig.VAF_Page_ID, VAF_MenuConfig.VAF_Workflow_ID, VAF_MenuConfig.VAF_Task_ID, VAF_MenuConfig.AD_Workbench_ID "
                        + "FROM VAF_MenuConfig VAF_MenuConfig JOIN  VAF_MenuConfig_TL t ON VAF_MenuConfig.VAF_MenuConfig_ID=t.VAF_MenuConfig_ID ");
                if (!baseLang)
                {
                    sqlNode.Append(" JOIN " + GetNodeTableName() + " pr on pr.NODE_ID=VAF_MenuConfig." + columnNameX + "_ID ");

                    sqlNode.Append(" WHERE VAF_MenuConfig.VAF_MenuConfig_ID=t.VAF_MenuConfig_ID AND t.VAF_Language='")
                        .Append(Utility.Env.GetVAF_Language(GetCtx())).Append("'");

                    if (onDemand)
                    {

                        //  sqlNode.Append(" OR ( m." + columnNameX + "_ID IN (SELECT NODE_ID FROM " + GetNodeTableName() + " WHERE Parent_ID=0 AND VAF_TreeInfo_ID=" + GetVAF_TreeInfo_ID() + " AND m.IsSummary='N' AND IsActive='Y')) ");

                        sqlNode.Append(" AND pr.VAF_TreeInfo_ID=" + GetVAF_TreeInfo_ID() + "  AND (IsSummary='Y')");

                    }
                }
                else
                {
                    if (onDemand)
                    {
                        sqlNode.Append(" JOIN " + GetNodeTableName() + " pr on pr.NODE_ID=VAF_MenuConfig." + columnNameX + "_ID ");
                        //  sqlNode.Append(" OR ( m." + columnNameX + "_ID IN (SELECT NODE_ID FROM " + GetNodeTableName() + " WHERE Parent_ID=0 AND VAF_TreeInfo_ID=" + GetVAF_TreeInfo_ID() + " AND m.IsSummary='N' AND IsActive='Y')) ");
                        sqlNode.Append("AND pr.VAF_TreeInfo_ID=" + GetVAF_TreeInfo_ID() + "  AND (IsSummary='Y')");

                    }
                }

                if (!m_editable)
                {
                    bool hasWhere = sqlNode.ToString().IndexOf(" WHERE ") != -1;
                    sqlNode.Append(hasWhere ? " AND " : " WHERE ").Append("VAF_MenuConfig.IsActive='Y' ");
                }
                //	Do not show Beta
                if (!GetCtx().GetIsUseBetaFunctions())
                {
                    bool hasWhere = sqlNode.ToString().IndexOf(" WHERE ") != -1;
                    sqlNode.Append(hasWhere ? " AND " : " WHERE ");
                    sqlNode.Append("(VAF_MenuConfig.VAF_Screen_ID IS NULL OR EXISTS (SELECT * FROM VAF_Screen w WHERE VAF_MenuConfig.VAF_Screen_ID=w.VAF_Screen_ID AND w.IsBetaFunctionality='N'))")
                        .Append(" AND (VAF_MenuConfig.VAF_Job_ID IS NULL OR EXISTS (SELECT * FROM VAF_Job p WHERE VAF_MenuConfig.VAF_Job_ID=p.VAF_Job_ID AND p.IsBetaFunctionality='N'))")
                        .Append(" AND (VAF_MenuConfig.VAF_Page_ID IS NULL OR EXISTS (SELECT * FROM VAF_Page f WHERE VAF_MenuConfig.VAF_Page_ID=f.VAF_Page_ID AND f.IsBetaFunctionality='N'))");
                }
                //	In R/O Menu - Show only defined Forms
                if (!m_editable)
                {
                    bool hasWhere = sqlNode.ToString().IndexOf(" WHERE ") != -1;
                    sqlNode.Append(hasWhere ? " AND " : " WHERE ");
                    sqlNode.Append("(VAF_MenuConfig.VAF_Page_ID IS NULL OR EXISTS (SELECT * FROM VAF_Page f WHERE VAF_MenuConfig.VAF_Page_ID=f.VAF_Page_ID AND ");
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
                        sqlNode.Append(" AND pr.VAF_TreeInfo_ID=" + GetVAF_TreeInfo_ID() + "  AND (IsSummary='Y' )");
                        //  sqlNode.Append(" AND t.IsSummary='Y'");
                        //sqlNode.Append(" OR ( t." + columnNameX + "_ID IN (SELECT NODE_ID FROM " + GetNodeTableName() + " WHERE Parent_ID=0 AND VAF_TreeInfo_ID=" + GetVAF_TreeInfo_ID() + " AND t.IsSummary='N' AND IsActive='Y')) ");
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
                        //sqlNode.Append(" OR ( t." + columnNameX + "_ID IN (SELECT NODE_ID FROM " + GetNodeTableName() + " WHERE Parent_ID=0 AND VAF_TreeInfo_ID=" + GetVAF_TreeInfo_ID() + " AND t.IsSummary='N' AND IsActive='Y')) ");

                        sqlNode.Append(" JOIN " + GetNodeTableName() + " pr on pr.NODE_ID=" + columnNameX + "." + columnNameX + "_ID ");
                        sqlNode.Append(" WHERE " + columnNameX + ".IsActive='Y'");
                        sqlNode.Append(" AND pr.VAF_TreeInfo_ID=" + GetVAF_TreeInfo_ID() + "  AND (IsSummary='Y')");

                    }
                }


            }

            String sql = sqlNode.ToString();

            if (sql.ToLower().IndexOf("where") > -1)
            {
                if (VAF_Tab_ID > 0)
                {
                    MTab tab = new MTab(p_ctx, VAF_Tab_ID, null);
                    if (!String.IsNullOrEmpty(tab.GetWhereClause()))
                    {
                        sql += " AND " + tab.GetWhereClause();
                    }
                }
            }
            else
            {
                if (VAF_Tab_ID > 0)
                {
                    MTab tab = new MTab(p_ctx, VAF_Tab_ID, null);
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

                drTree = CoreLibrary.DataBase.DB.ExecuteReader(sql, null, null);
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
            LoadNodes(ctx.GetVAF_UserContact_ID(), orderClause);
        }



        /// <summary>
        /// Load Nodes and Bar
        /// </summary>
        /// <param name="VAF_UserContact_ID">user for tree bar</param>
        private void LoadNodes(int VAF_UserContact_ID,string orderClause="")
        {
            ////  SQL for TreeNodes
            StringBuilder sql = new StringBuilder("SELECT "
                + "tn.Node_ID,COALESCE(tn.Parent_ID, -1) AS Parent_ID,tn.SeqNo,tb.IsActive "
                + "FROM ").Append(GetNodeTableName()).Append(" tn"
                + " LEFT OUTER JOIN VAF_TreeInfoBar tb ON (tn.VAF_TreeInfo_ID=tb.VAF_TreeInfo_ID"
                + " AND tn.Node_ID=tb.Node_ID AND tb.VAF_UserContact_ID=@userid) ");	//	#1

            string tblName = MTable.GetTableName(p_ctx, GetVAF_TableView_ID());
            //on (mp.M_Product_ID= tn.Node_ID)

            //if (onDemand || VAF_Tab_ID > 0)
            //{
            sql.Append(" JOIN ").Append(tblName + " " + tblName + " ON (" + tblName + "." + tblName + "_ID = tn.Node_ID)");
            //}

            if (onDemand)
            {


                sql.Append("WHERE tn.VAF_TreeInfo_ID=@treeId");								//	#2
                if (!m_editable)
                    sql.Append(" AND tn.IsActive='Y'");

                sql.Append(" AND (" + tblName + "." + "IsSummary='Y')");
            }
            else
            {
                sql.Append("WHERE tn.VAF_TreeInfo_ID=@treeId");								//	#2
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
            if (VAF_Tab_ID > 0)
            {
                MTab tab = new MTab(p_ctx, VAF_Tab_ID, null);
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
                param[0] = new SqlParameter("@userid", VAF_UserContact_ID);
                param[1] = new SqlParameter("@treeId", GetVAF_TreeInfo_ID());
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
            int VAF_Screen_ID = 0;
            int VAF_Job_ID = 0;
            int VAF_Page_ID = 0;
            int VAF_Workflow_ID = 0;
            int VAF_Task_ID = 0;
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
                        VAF_Screen_ID = Utility.Util.GetValueOfInt(dr[index]);
                        index++;
                        VAF_Job_ID = Utility.Util.GetValueOfInt(dr[index]);
                        index++;
                        VAF_Page_ID = Utility.Util.GetValueOfInt(dr[index]);
                        index++;
                        VAF_Workflow_ID = Utility.Util.GetValueOfInt(dr[index]);
                        index++;
                        VAF_Task_ID = Utility.Util.GetValueOfInt(dr[index]);
                        index++;
                        AD_Workbench_ID = Utility.Util.GetValueOfInt(dr[index]);
                        index++;
                        //
                        MRole role = MRole.GetDefault(GetCtx(), false);
                        bool? blnAccess = false;


                        if (X_VAF_MenuConfig.ACTION_Window.Equals(actionColor))
                            blnAccess = role.GetWindowAccess(VAF_Screen_ID);
                        else if (X_VAF_MenuConfig.ACTION_Process.Equals(actionColor)
                        || X_VAF_MenuConfig.ACTION_Report.Equals(actionColor))
                            blnAccess = role.GetProcessAccess(VAF_Job_ID);
                        else if (X_VAF_MenuConfig.ACTION_Form.Equals(actionColor))
                            blnAccess = role.GetFormAccess(VAF_Page_ID);
                        else if (X_VAF_MenuConfig.ACTION_WorkFlow.Equals(actionColor))
                            blnAccess = role.GetWorkflowAccess(VAF_Workflow_ID);
                        else if (X_VAF_MenuConfig.ACTION_Task.Equals(actionColor))
                            blnAccess = role.GetTaskAccess(VAF_Task_ID);

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
        /// <returns>source table name, e.g. VAF_Org or null</returns>
        public String GetSourceTableName(bool tableNameOnly)
        {
            int VAF_TableView_ID = GetVAF_TableView_ID();
            String tableName = MTable.GetTableName(GetCtx(), VAF_TableView_ID);
            //
            if (tableNameOnly)
                return tableName;
            if ("M_Product".Equals(tableName))
                return "M_Product M_Product INNER JOIN M_Product_Category x ON (M_Product.M_Product_Category_ID=x.M_Product_Category_ID)";
            if ("VAB_BusinessPartner".Equals(tableName))
                return "VAB_BusinessPartner VAB_BusinessPartner INNER JOIN VAB_BPart_Category x ON (VAB_BusinessPartner.VAB_BPart_Category_ID=x.VAB_BPart_Category_ID)";
            if ("VAF_Org".Equals(tableName))
                return "VAF_Org VAF_Org INNER JOIN VAF_OrgDetail i ON (VAF_Org.VAF_Org_ID=i.VAF_Org_ID) "
                    + "LEFT OUTER JOIN VAF_OrgCategory x ON (i.VAF_OrgCategory_ID=x.VAF_OrgCategory_ID)";
            if ("VAB_Promotion".Equals(tableName))
                return "VAB_Promotion VAB_Promotion LEFT OUTER JOIN VAB_MarketingChannel x ON (VAB_Promotion.VAB_MarketingChannel_ID=x.VAB_MarketingChannel_ID)";
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
            int VAF_TableView_ID = GetVAF_TableView_ID();
            String tableName = MTable.GetTableName(GetCtx(), VAF_TableView_ID);
            //
            if ("VAF_MenuConfig".Equals(tableName))
                return "t.Action";
            if ("M_Product".Equals(tableName) || "VAB_BusinessPartner".Equals(tableName)
                || "VAF_Org".Equals(tableName) || "VAB_Promotion".Equals(tableName))
                return "x.VAF_Print_Rpt_Colour_ID";
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
        /// <param name="VAF_TreeInfo_ID">The tree to build</param>
        /// <param name="editable">True, if tree can be modified - includes inactive and empty summary nodes</param>
        /// <param name="clientTree">the tree is displayed on the java client (not on web)</param>
        /// <param name="trxName">transaction</param>

        //public MTree(Ctx ctx, int VAF_TreeInfo_ID, bool editable, bool clientTree, Trx trxName, System.Windows.Forms.ImageList imgList, StartMenu.IToolStripItem source)
        //    : this(ctx, VAF_TreeInfo_ID, trxName)
        //{
        //    _imgList = imgList;
        //    _itemClickHandler = source;
        //    m_editable = editable;
        //    int VAF_UserContact_ID = ctx.GetVAF_UserContact_ID();
        //    m_clientTree = clientTree;
        //    log.Info("VAF_TreeInfo_ID=" + VAF_TreeInfo_ID + ", VAF_UserContact_ID=" + VAF_UserContact_ID
        //        + ", Editable=" + editable + ", OnClient=" + clientTree);
        //    //
        //    LoadStartMenu(VAF_UserContact_ID);
        //}

        /// <summary>
        /// Load Nodes and Bar
        /// </summary>
        /// <param name="VAF_UserContact_ID">user for tree bar</param>
        //private void LoadStartMenu(int VAF_UserContact_ID)
        //{
        //    ////  SQL for TreeNodes
        //    StringBuilder sql = new StringBuilder("SELECT "
        //        + "tn.Node_ID,tn.Parent_ID,tn.SeqNo,tb.IsActive "
        //        + "FROM ").Append(GetNodeTableName()).Append(" tn"
        //        + " LEFT OUTER JOIN VAF_TreeInfoBar tb ON (tn.VAF_TreeInfo_ID=tb.VAF_TreeInfo_ID"
        //        + " AND tn.Node_ID=tb.Node_ID AND tb.VAF_UserContact_ID=@userid) "	//	#1
        //        + "WHERE tn.VAF_TreeInfo_ID=@treeId");								//	#2
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
        //        param[0] = new SqlParameter("@userid", VAF_UserContact_ID);
        //        param[1] = new SqlParameter("@treeId", GetVAF_TreeInfo_ID());
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
        //    int VAF_Screen_ID = 0;
        //    int VAF_Job_ID = 0;
        //    int VAF_Page_ID = 0;
        //    int VAF_Workflow_ID = 0;
        //    int VAF_Task_ID = 0;
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
        //                VAF_Screen_ID = Utility.Util.GetValueOfInt(dr[index]);
        //                index++;
        //                VAF_Job_ID = Utility.Util.GetValueOfInt(dr[index]);
        //                index++;
        //                VAF_Page_ID = Utility.Util.GetValueOfInt(dr[index]);
        //                index++;
        //                VAF_Workflow_ID = Utility.Util.GetValueOfInt(dr[index]);
        //                index++;
        //                VAF_Task_ID = Utility.Util.GetValueOfInt(dr[index]);
        //                index++;
        //                AD_Workbench_ID = Utility.Util.GetValueOfInt(dr[index]);
        //                index++;
        //                //
        //                MRole role = MRole.GetDefault(GetCtx(), false);
        //                bool? blnAccess = false;


        //                if (X_VAF_MenuConfig.ACTION_Window.Equals(actionColor))
        //                    blnAccess = role.GetWindowAccess(VAF_Screen_ID);
        //                else if (X_VAF_MenuConfig.ACTION_Process.Equals(actionColor)
        //                || X_VAF_MenuConfig.ACTION_Report.Equals(actionColor))
        //                    blnAccess = role.GetProcessAccess(VAF_Job_ID);
        //                else if (X_VAF_MenuConfig.ACTION_Form.Equals(actionColor))
        //                    blnAccess = role.GetFormAccess(VAF_Page_ID);
        //                else if (X_VAF_MenuConfig.ACTION_WorkFlow.Equals(actionColor))
        //                    blnAccess = role.GetWorkflowAccess(VAF_Workflow_ID);
        //                else if (X_VAF_MenuConfig.ACTION_Task.Equals(actionColor))
        //                    blnAccess = role.GetTaskAccess(VAF_Task_ID);

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
        //        retValue.VAF_Screen_ID = VAF_Screen_ID;
        //        retValue.VAF_Job_ID = VAF_Job_ID;
        //        retValue.VAF_Page_ID = VAF_Page_ID;
        //        retValue.VAF_Workflow_ID = VAF_Workflow_ID;
        //        retValue.VAF_Task_ID = VAF_Task_ID;
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
