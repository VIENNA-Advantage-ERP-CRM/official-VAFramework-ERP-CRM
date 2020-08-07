/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MTreeNodeCMT
 * Purpose        : (Disk) Tree Node Model CM Media
 * Class Used     : X_AD_TreeNodeCMT
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
     public class MTreeNodeCMT : X_AD_TreeNodeCMT
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
	public static MTreeNodeCMT[] GetTree(Ctx ctx, int AD_Tree_ID, Trx trxName)
	{
		List<MTreeNodeCMT> list = new List<MTreeNodeCMT>();
		String sql = "SELECT * FROM AD_TreeNodeCMT WHERE AD_Tree_ID=@Param1 ORDER BY Node_ID";
		SqlParameter[] Param=new SqlParameter[1];
        IDataReader idr=null;
        DataTable dt=null;
		try
		{
			//pstmt = DataBase.prepareStatement (sql, trxName);
            Param[0]=new SqlParameter("@Param1",AD_Tree_ID);
            idr=DataBase.DB.ExecuteReader(sql,Param,trxName);
            dt=new DataTable();
            dt.Load(idr);
			//pstmt.setInt (1, AD_Tree_ID);
			//ResultSet rs = pstmt.executeQuery ();
			foreach(DataRow dr in dt.Rows)
			{
				list.Add (new MTreeNodeCMT (ctx,dr, trxName));
			}
		}
		catch (Exception e)
		{
			_log.Log (Level.SEVERE, sql, e);
		}
		finally
        {
            idr.Close();
            dt=null;
        }
		MTreeNodeCMT[] retValue = new MTreeNodeCMT[list.Count];
		retValue=list.ToArray();
		return retValue;
	}	//	getTree

	/// <summary>
	/// Get Tree Node
	/// </summary>
	/// <param name="tree">tree</param>
	/// <param name="Node_ID">node</param>
	/// <returns>node or null</returns>
	public static MTreeNodeCMT Get(MTree tree, int Node_ID)
	{
		MTreeNodeCMT retValue = null;
		String sql = "SELECT * FROM AD_TreeNodeCMT WHERE AD_Tree_ID=@Param1 AND Node_ID=@Param2";
		SqlParameter[] Param=new SqlParameter[2];
        IDataReader idr=null;
        DataTable dt=null;
		try
		{
            //pstmt = DataBase.prepareStatement (sql, tree.get_TrxName());
            //pstmt.setInt (1, tree.getAD_Tree_ID());
            Param[0]=new SqlParameter("@Param1",tree.GetAD_Tree_ID());
            Param[1]=new SqlParameter("@Param2",Node_ID);
            //pstmt.setInt (2, Node_ID);
            //ResultSet rs = pstmt.executeQuery ();
            idr=DataBase.DB.ExecuteReader(sql,Param,tree.Get_TrxName());
            dt=new DataTable();
            dt.Load(idr);
			foreach(DataRow dr in dt.Rows)
            {
				retValue = new MTreeNodeCMT (tree.GetCtx(),dr, tree.Get_TrxName());
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
	/// <param name="rs"></param>
    /// <param name="trxName">transaction</param>
	public MTreeNodeCMT (Ctx ctx, DataRow dr, Trx trxName):base(ctx,dr, trxName)
	{
		//super(ctx, rs, trxName);
	}	//	MTreeNodeCMS

	/// <summary>
	/// Full Constructor
	 /// </summary>
	/// <param name="tree">tree</param>
	/// <param name="Node_ID">node</param>
	public MTreeNodeCMT (MTree tree, int Node_ID):base(tree.GetCtx(), 0, tree.Get_TrxName())
	{
		//super (tree.getCtx(), 0, tree.get_TrxName());
		SetClientOrg(tree);
		SetAD_Tree_ID (tree.GetAD_Tree_ID());
		SetNode_ID(Node_ID);
		//	Add to root
		SetParent_ID(0);
		SetSeqNo (0);
	}	//	MTreeNodeCMT



         //Manish 8/6/2016

    /// <summary>
    /// Full Constructor
    /// </summary>
    /// <param name="tree">tree</param>
    /// <param name="Node_ID">node</param>
    public MTreeNodeCMT(MTree tree, int Node_ID, int setSeqManually)
        : base(tree.GetCtx(), 0, tree.Get_TrxName())
    {
        //super (tree.getCtx(), 0, tree.get_TrxName());
        SetClientOrg(tree);
        SetAD_Tree_ID(tree.GetAD_Tree_ID());
        SetNode_ID(Node_ID);
        //	Add to root
        SetParent_ID(0);
        SetSeqNo(setSeqManually);
    }	//	MTreeNodeCMT


         //End



	
}	//	MTreeNodeCMT

}
