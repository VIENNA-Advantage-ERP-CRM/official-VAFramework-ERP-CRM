/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MTreeNodeCMS
 * Purpose        : (Disk) Tree Node Model CM Stage
 * Class Used     : X_AD_TreeNodeCMS
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
    public class MTreeNodeCMS : X_AD_TreeNodeCMS
    {
        /**	Static Logger	*/
	private static VLogger	_log	= VLogger.GetVLogger (typeof(MTreeNodeCMS).FullName);

   /// <summary>
   /// Get Tree
   /// </summary>
   /// <param name="ctx">context</param>
   /// <param name="AD_Tree_ID">tree</param>
   /// <param name="trxName">transaction</param>
   /// <returns>array of nodes</returns>
	public static MTreeNodeCMS[] GetTree(Ctx ctx, int AD_Tree_ID, Trx trxName)
	{
		List<MTreeNodeCMS> list = new List<MTreeNodeCMS>();
		String sql = "SELECT * FROM AD_TreeNodeCMS WHERE AD_Tree_ID=@Param ORDER BY Node_ID";
		SqlParameter[] Param=new SqlParameter[1];
        IDataReader idr=null;
        DataTable dt=null;
		try
		{
            Param[0]=new SqlParameter("@Param",AD_Tree_ID);
			//pstmt = DataBase.prepareStatement (sql, trxName);
            idr=DataBase.DB.ExecuteReader(sql,Param,trxName);
            dt=new DataTable();
            dt.Load(idr);
            idr.Close();
			//pstmt.setInt (1, AD_Tree_ID);
			//ResultSet rs = pstmt.executeQuery ();
			foreach(DataRow dr in dt.Rows)
			{
				list.Add (new MTreeNodeCMS (ctx,dr, trxName));
			}
            dt=null;
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
			_log.Log (Level.SEVERE, sql, e);
		}
		
		MTreeNodeCMS[] retValue = new MTreeNodeCMS[list.Count];
		retValue=list.ToArray();
		return retValue;
	}	//	getTree

	/// <summary>
	/// Get Tree Node
	/// </summary>
	/// <param name="tree">tree</param>
	/// <param name="Node_ID">node</param>
	/// <returns>node or null</returns>
	public static MTreeNodeCMS Get(MTree tree, int Node_ID)
	{
		MTreeNodeCMS retValue = null;
		String sql = "SELECT * FROM AD_TreeNodeCMS WHERE AD_Tree_ID=@Param1 AND Node_ID=@Param2";
		SqlParameter[] Param=new SqlParameter[2];
        IDataReader idr=null;
        DataTable dt=null;
		try
		{
			//pstmt = DataBase.prepareStatement (sql, tree.get_TrxName());
			//pstmt.setInt (1, tree.getAD_Tree_ID());
            Param[0]=new SqlParameter("@Param1",tree.GetAD_Tree_ID());
			//pstmt.setInt (2, Node_ID);
            Param[1]=new SqlParameter("@Param2",Node_ID);
            idr=DataBase.DB.ExecuteReader(sql,Param,tree.Get_TrxName());
			//ResultSet rs = pstmt.executeQuery ();
			foreach(DataRow dr in dt.Rows)
            {
				retValue = new MTreeNodeCMS (tree.GetCtx(), dr, tree.Get_TrxName());
            }
			
		}
		catch (Exception e)
		{
			_log.Log(Level.SEVERE, sql, e);
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
	/// <param name="trxName"></param>
	public MTreeNodeCMS (Ctx ctx,DataRow dr, Trx trxName):base(ctx,dr, trxName)
	{
		//super(ctx, rs, trxName);
	}	//	MTreeNodeCMS

	/// <summary>
	/// Full Constructor
	/// </summary>
	/// <param name="tree"> tree</param>
	/// <param name="Node_ID">node</param>
	public MTreeNodeCMS (MTree tree, int Node_ID):base(tree.GetCtx(), 0, tree.Get_TrxName())
	{
		//super (tree.GetCtx(), 0, tree.get_TrxName());
		SetClientOrg(tree);
		SetAD_Tree_ID (tree.GetAD_Tree_ID());
		SetNode_ID(Node_ID);
		//	Add to root
		SetParent_ID(0);
		SetSeqNo (0);
	}	//	MTreeNodeCMS


        //Manish 8/6/2016

    /// <summary>
    /// Full Constructor
    /// </summary>
    /// <param name="tree"> tree</param>
    /// <param name="Node_ID">node</param>
    public MTreeNodeCMS(MTree tree, int Node_ID, int setSeqManually)
        : base(tree.GetCtx(), 0, tree.Get_TrxName())
    {
        //super (tree.GetCtx(), 0, tree.get_TrxName());
        SetClientOrg(tree);
        SetAD_Tree_ID(tree.GetAD_Tree_ID());
        SetNode_ID(Node_ID);
        //	Add to root
        SetParent_ID(0);
        SetSeqNo(setSeqManually);
    }	//	MTreeNodeCMS

        //End


	
}	//	MTreeNodeCMS

}
