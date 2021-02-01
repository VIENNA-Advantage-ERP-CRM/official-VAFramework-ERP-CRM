/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MReportTree
 * Purpose        : Report Tree Model
 * Class Used     : 
 * Chronological    Development
 * Deepak           23-Nov-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Windows.Forms;
namespace VAdvantage.Report
{
    public class MReportTree : VTreeNode
    {
        /// <summary>
        /// Get Report Tree (cached)
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAPA_FinancialReportingOrder_ID">optional hierarchy</param>
        /// <param name="ElementType">Account Schema Element Type</param>
        /// <returns>tree</returns>
        public static MReportTree Get(Ctx ctx, int VAPA_FinancialReportingOrder_ID, String ElementType)
        {
            String key = VAPA_FinancialReportingOrder_ID + ElementType;
            MReportTree tree = (MReportTree)s_trees[key];//.get(key);
            if (tree == null)
            {
                tree = new MReportTree(ctx, VAPA_FinancialReportingOrder_ID, ElementType);
                //s_trees.put(key, tree);
                s_trees.Add(key, tree);

            }
            return tree;
        }	//	get

        /// <summary>
        /// Get Where Clause
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAPA_FinancialReportingOrder_ID">optional hierarchy</param>
        /// <param name="ElementType">Account Schema Element Type</param>
        /// <param name="ID">leaf element id</param>
        /// <returns>where clause</returns>
        public static String GetWhereClause(Ctx ctx,
            int VAPA_FinancialReportingOrder_ID, String ElementType, int ID)
        {
            MReportTree tree = Get(ctx, VAPA_FinancialReportingOrder_ID, ElementType);
            return tree.GetWhereClause(ID);
        }	//	getWhereClause

        /// <summary>
        /// Get Child IDs
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAPA_FinancialReportingOrder_ID">optional hierarchie</param>
        /// <param name="ElementType">Account Schema Element Type</param>
        /// <param name="ID">id</param>
        /// <returns>array of IDs</returns>
        public static int?[] GetChildIDs(Ctx ctx,
            int VAPA_FinancialReportingOrder_ID, String ElementType, int ID)
        {
            MReportTree tree = Get(ctx, VAPA_FinancialReportingOrder_ID, ElementType);
            return tree.GetChildIDs(ID);
        }	//	getChildIDs

        /**	Map with Tree				*/
        private static CCache<String, MReportTree> s_trees
            = new CCache<String, MReportTree>("MReportTree", 20);


        /// <summary>
        /// Report Tree
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAPA_FinancialReportingOrder_ID">optional hierarchy</param>
        /// <param name="ElementType">Account Schema Element Type</param>
        public MReportTree(Ctx ctx, int VAPA_FinancialReportingOrder_ID, String ElementType)
        {
            log = VLogger.GetVLogger(this.GetType().FullName);
            _ElementType = ElementType;
            _TreeType = _ElementType;
            if (MAcctSchemaElement.ELEMENTTYPE_Account.Equals(_ElementType)
                || MAcctSchemaElement.ELEMENTTYPE_UserList1.Equals(_ElementType)
                || MAcctSchemaElement.ELEMENTTYPE_UserList2.Equals(_ElementType))
                _TreeType = MVAFTreeInfo.TREETYPE_ElementValue;
            _VAPA_FinancialReportingOrder_ID = VAPA_FinancialReportingOrder_ID;
            _ctx = ctx;
            //
            int VAF_TreeInfo_ID = GetVAF_TreeInfo_ID();
            //	Not found
            if (VAF_TreeInfo_ID == 0)
            {
                throw new ArgumentException("No VAF_TreeInfo_ID for TreeType=" + _TreeType
                    + ", VAPA_FinancialReportingOrder_ID=" + VAPA_FinancialReportingOrder_ID);
            }
            //
            Boolean clientTree = true;
            _tree = new MVAFTreeInfo(ctx, VAF_TreeInfo_ID, true, clientTree, null);
        }	//	MReportTree

        /** Optional Hierarchy		*/
        private int _VAPA_FinancialReportingOrder_ID = 0;
        /**	Element Type			*/
        private String _ElementType = null;
        /** Context					*/
        private Ctx _ctx = null;
        /** Tree Type				*/
        private String _TreeType = null;
        /**	The Tree				*/
        private MVAFTreeInfo _tree = null;
        /**	Logger					*/
        private VLogger log = null;



        /// <summary>
        /// Get VAF_TreeInfo_ID 
        /// </summary>
        /// <returns>tree</returns>
        protected int GetVAF_TreeInfo_ID()
        {
            if (_VAPA_FinancialReportingOrder_ID == 0 || _VAPA_FinancialReportingOrder_ID == -1)
            {
                return GetDefaultVAF_TreeInfo_ID();
            }

            MHierarchy hierarchy = MHierarchy.Get(_ctx, _VAPA_FinancialReportingOrder_ID);
            int VAF_TreeInfo_ID = hierarchy.GetVAF_TreeInfo_ID(_TreeType);

            if (VAF_TreeInfo_ID == 0)
            {
                return GetDefaultVAF_TreeInfo_ID();
            }

            return VAF_TreeInfo_ID;
        }	//	getVAF_TreeInfo_ID

        /// <summary>
        ///	Get Default VAF_TreeInfo_ID ,see MTree.getDefaultVAF_TreeInfo_ID
        /// </summary>
        /// <returns>tree</returns>
        protected int GetDefaultVAF_TreeInfo_ID()
        {
            int VAF_TreeInfo_ID = 0;
            int VAF_Client_ID = _ctx.GetVAF_Client_ID();

            String sql = "SELECT VAF_TreeInfo_ID, Name FROM VAF_TreeInfo "
                + "WHERE VAF_Client_ID=@param1 AND TreeType=@param2 AND IsActive='Y' AND IsAllNodes='Y' "
                + "ORDER BY IsDefault DESC, VAF_TreeInfo_ID";	//	assumes first is primary tree
            SqlParameter[] param = new SqlParameter[2];
            IDataReader idr = null;
            try
            {
                //PreparedStatement pstmt = DataBase.prepareStatement(sql, null);
                param[0] = new SqlParameter("@param1", VAF_Client_ID);
                //pstmt.setInt(1, VAF_Client_ID);
                // pstmt.setString(2, _TreeType);
                param[1] = new SqlParameter("@param2", _TreeType);
                //ResultSet rs = pstmt.executeQuery();
                idr = DataBase.DB.ExecuteReader(sql, param, null);
                if (idr.Read())
                {
                    VAF_TreeInfo_ID = Utility.Util.GetValueOfInt(idr[0]);// rs.getInt(1);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                 log.Log(VAdvantage.Logging.Level.SEVERE, sql, e);
            }

            return VAF_TreeInfo_ID;
        }	//	getDefaultVAF_TreeInfo_ID

        /// <summary>
        ///	Get Account Schema Element Type
        /// </summary>
        /// <returns>element Type</returns>
        public String GetElementType()
        {
            return _ElementType;
        }	//	getElementType

        /// <summary>
        ///	Get Tree Type
        /// </summary>
        /// <returns>tree type</returns>
        public String GetTreeType()
        {
            return _TreeType;
        }	//	getTreeType

        /// <summary>
        /// Get Tree
        /// </summary>
        /// <returns>tree</returns>
        public MVAFTreeInfo GetTree()
        {
            return _tree;
        }	//	getTreeType

        /// <summary>
        /// Get Where Clause
        /// </summary>
        /// <param name="ID">start node</param>
        /// <returns>ColumnName = 1 or ColumnName IN (1,2,3)</returns>
        public String GetWhereClause(int ID)
	{
		log.Fine("(" + _ElementType + ") ID=" + ID);
		String ColumnName = MAcctSchemaElement.GetColumnName(_ElementType);
		if (ID == 0)	//	All
        {
			return ColumnName + " IS NOT NULL";
        }
		VTreeNode nod = _tree.GetRootNode().FindNode(ID);

		log.Finest("Root=" + nod);
		//
		StringBuilder result = null;
        if(nod!=null && nod.IsSummary)
		{ 
			StringBuilder sb = new StringBuilder();
               System.Collections.IEnumerator en = nod.preorderEnumeration();         
               
            while (en.MoveNext())
            { 
                VTreeNode nn = (VTreeNode)en.Current;
                if (!nn.IsSummary)
                { 
                    if (sb.Length > 0)
                    {
                        sb.Append(",");
                    }
                    sb.Append(nn.GetNode_ID());
                    log.Finest("- " + nn);
                }
                else
                {
                    log.Finest("- skipped parent (" + nn + ")");
                }
            }
           
			result = new StringBuilder (ColumnName).Append(" IN (").Append(sb).Append(")");
		}
		else	//	not found or not summary 
			result = new StringBuilder (ColumnName).Append("=").Append(ID);
		//
		log.Finest(result.ToString());
		return result.ToString();
	}	//	getWhereClause

        /// <summary>
        /// Get Child IDs
        /// </summary>
        /// <param name="ID">start node</param>
        /// <returns>array if IDs</returns>
        public int?[] GetChildIDs(int ID)
        {
           	log.Fine("(" + _ElementType + ") ID=" + ID);
		List<int?> list = new List<int?>(); 
		//
		VTreeNode node = _tree.GetRootNode().FindNode(ID);
		log.Finest("Root=" + node);
		//
        if (node != null && node.IsSummary)
        {

            System.Collections.IEnumerator en = node.preorderEnumeration();
            while (en.MoveNext())
            {
                VTreeNode nn = (VTreeNode)en.Current;
                if (!nn.IsSummary)
                {
                    list.Add(Utility.Util.GetValueOfInt(nn.GetNode_ID()));
                    log.Finest("- " + nn);
                }
                else
                {
                    log.Finest("- skipped parent (" + nn + ")");
                }
            }
        }
        else	//	not found or not summary 
        {
            list.Add(Utility.Util.GetValueOfInt(ID));
        }
		//
		int?[] retValue = new int?[list.Count];
		retValue= list.ToArray();
		return retValue;
        }	//	getWhereClause


        /// <summary>
        ///  String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MReportTree[ElementType=");
            sb.Append(_ElementType).Append(",TreeType=").Append(_TreeType)
                .Append(",").Append(_tree)
                .Append("]");
            return sb.ToString();
        }	//	toString

    }	//	MReportTree

}
