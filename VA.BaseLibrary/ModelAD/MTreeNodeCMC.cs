/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MTreeNodeCMC
 * Purpose        : (Disk) Tree Node Model CM Container
 * Class Used     : X_AD_TreeNodeCMC
 * Chronological    Development
 * Deepak           27-Nov-2009 
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

namespace VAdvantage.Model
{
    public class MTreeNodeCMC : X_AD_TreeNodeCMC
    {

        /**	Static Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MTreeNodeCMC).FullName);

      /// <summary>
      /// Get Tree
	  /// </summary>
      /// <param name="ctx">context</param>
      /// <param name="AD_Tree_ID">tree</param>
      /// <param name="trxName">transaction</param>
      /// <returns>array of nodes</returns>
	public static MTreeNodeCMC[] GetTree(Ctx ctx, int AD_Tree_ID, Trx trxName)
	{  
		//ArrayList<MTreeNodeCMC> list = new ArrayList<MTreeNodeCMC>();
        List<MTreeNodeCMC> list = new List<MTreeNodeCMC>();
		String sql = "SELECT * FROM AD_TreeNodeCMC WHERE AD_Tree_ID=@Param1 ORDER BY Node_ID";
        SqlParameter[] Param = new SqlParameter[1];
        IDataReader idr = null;
        DataTable dt = null;
        try
        {
            //pstmt = DataBase.prepareStatement (sql, trxName);
            //pstmt.setInt (1, AD_Tree_ID);
            Param[0] = new SqlParameter("@Param1", AD_Tree_ID);
            idr = DataBase.DB.ExecuteReader(sql, Param, trxName);
            dt = new DataTable();
            dt.Load(idr);
            idr.Close();
            //ResultSet rs = pstmt.executeQuery ();
            //while (rs.next ())
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(new MTreeNodeCMC(ctx, dr, trxName));
            }
            dt = null;
        }
        catch (Exception e)
        {
            if (idr != null)
            {
                idr.Close();
            }
            if (dt != null)
            {
                dt = null;
            }
            _log.Log(Level.SEVERE, sql, e);
        }
        
		MTreeNodeCMC[] retValue = new MTreeNodeCMC[list.Count];
		//list.toArray (retValue);
        retValue = list.ToArray();
		return retValue;
	}	//	getTree
	
	
	/// <summary>
	/// Get Tree Node
	/// </summary>
	/// <param name="tree">tree</param>
	/// <param name="Node_ID">node</param>
    /// <returns>node or null</returns>
	public static MTreeNodeCMC Get(MTree tree, int Node_ID)
	{
		MTreeNodeCMC retValue = null;
		String sql = "SELECT * FROM AD_TreeNodeCMC WHERE AD_Tree_ID=@Param1 AND Node_ID=@Param2";
        SqlParameter[] Param = new SqlParameter[2];
        IDataReader idr = null;
        DataTable dt = null;

        try
        {
            //pstmt = DataBase.prepareStatement (sql, tree.get_TrxName());
            Param[0] = new SqlParameter("@Param1", tree.GetAD_Tree_ID());
            //pstmt.setInt (1, tree.getAD_Tree_ID());
            Param[1] = new SqlParameter("@Param1", Node_ID);
            //pstmt.setInt (2, Node_ID);
            //ResultSet rs = pstmt.executeQuery ();
            idr = DataBase.DB.ExecuteReader(sql, Param, tree.Get_TrxName());
            dt = new DataTable();
            dt.Load(idr);
            //if (rs.next ())
            foreach (DataRow dr in dt.Rows)
            {
                retValue = new MTreeNodeCMC(tree.GetCtx(), dr, tree.Get_TrxName());
            }
        }
        catch (Exception e)
        {
            _log.Log(Level.SEVERE, "get", e);
        }
        finally
        {
            idr.Close();
            dt=null;
        }
		return retValue;
	}	//	get
	/// <summary>
	/// Load Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="dr">datarow</param>
    /// <param name="trxName">transaction</param>
    public MTreeNodeCMC(Ctx ctx,DataRow dr, Trx trxName):base(ctx, dr, trxName)
	{
		//super(ctx, rs, trxName);
    }	//	MTreeNodeCMC

	/// <summary>
	/// Full Constructor
	/// </summary>
	/// <param name="tree">tree</param>
    /// <param name="Node_ID">node</param>
	public MTreeNodeCMC (MTree tree, int Node_ID):base(tree.GetCtx(), 0, tree.Get_TrxName())
	{
		//super (tree.getCtx(), 0, tree.get_TrxName());
		SetClientOrg(tree);
		SetAD_Tree_ID (tree.GetAD_Tree_ID());
		SetNode_ID(Node_ID);
		//	Add to root
		setParent_ID(0);
		SetSeqNo(0);
	}	//	MTreeNodeCMC



        //Manish 8/6/2016

    /// <summary>
    /// Full Constructor
    /// </summary>
    /// <param name="tree">tree</param>
    /// <param name="Node_ID">node</param>
    public MTreeNodeCMC(MTree tree, int Node_ID, int setSeqManually)
        : base(tree.GetCtx(), 0, tree.Get_TrxName())
    {
        //super (tree.getCtx(), 0, tree.get_TrxName());
        SetClientOrg(tree);
        SetAD_Tree_ID(tree.GetAD_Tree_ID());
        SetNode_ID(Node_ID);
        //	Add to root
        setParent_ID(0);
        SetSeqNo(setSeqManually);
    }	//	MTreeNodeCMC

        //END





	
	/// <summary>
	/// setParent_ID overwrite as Tree's need to allow 0 parents
	/// </summary>
	/// <param name="Parent_ID">Parent_ID</param>
	public void setParent_ID (int Parent_ID)
	{
		//Set_Value ("Parent_ID", new Integer(Parent_ID));
        Set_Value("Parent_ID",Utility.Util.GetValueOfInt(Parent_ID));
	}
}	//	MTreeNodeCMC
}