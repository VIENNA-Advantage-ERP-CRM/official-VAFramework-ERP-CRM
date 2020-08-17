/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MTreeNodeMM
 * Purpose        : (Disk) Tree Node Model Menu
 * Class Used     : X_AD_TreeNodeMM
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
     public class MTreeNodeMM : X_AD_TreeNodeMM
    {

         
	/**	Static Logger	*/
	private static VLogger	_log	= VLogger.GetVLogger (typeof(MTreeNodeMM).FullName);
    /// <summary>
    ///	Get Tree Node
	/// </summary>
    /// <param name="tree">tree</param>
    /// <param name="Node_ID">node</param>
    /// <returns>node or null</returns>
	public static MTreeNodeMM Get(MTree tree, int Node_ID)
	{
		MTreeNodeMM retValue = null;
		String sql = "SELECT * FROM AD_TreeNodeMM WHERE AD_Tree_ID=@Param1 AND Node_ID=@Param2";
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
            //ResultSet rs = pstmt.executeQuery ();
            idr=DataBase.DB.ExecuteReader(sql,Param,tree.Get_TrxName());
            dt=new DataTable();
            dt.Load(idr);
			foreach(DataRow dr in dt.Rows)
            {
				retValue = new MTreeNodeMM (tree.GetCtx(),dr, tree.Get_TrxName());
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
    /// <param name="trxName">transaction</param>
	public MTreeNodeMM (Ctx ctx,DataRow dr, Trx trxName):base(ctx, dr, trxName)
	{
		//super(ctx, rs, trxName);
	}	//	MTreeNodeMM

	/// <summary>
	/// Full Constructor
	/// </summary>
	/// <param name="tree"> tree</param>
    /// <param name="Node_ID">node</param>
	public MTreeNodeMM (MTree tree, int Node_ID):base(tree.GetCtx(), 0, tree.Get_TrxName())
	{
		//super (tree.getCtx(), 0, tree.get_TrxName());
		SetClientOrg(tree);
		SetAD_Tree_ID (tree.GetAD_Tree_ID());
		SetNode_ID(Node_ID);
		//	Add to root
		SetParent_ID(0);
		SetSeqNo (9999);
	}	//	MTreeNodeMM


    //Manish 8/6/2016
    /// <summary>
    /// Full Constructor
    /// </summary>
    /// <param name="tree"> tree</param>
    /// <param name="Node_ID">node</param>
    public MTreeNodeMM(MTree tree, int Node_ID, int setSeqManually) : base(tree.GetCtx(), 0, tree.Get_TrxName())
    {        
        SetClientOrg(tree);
        SetAD_Tree_ID(tree.GetAD_Tree_ID());
        SetNode_ID(Node_ID);
        //	Add to root
        SetParent_ID(0);
        SetSeqNo(setSeqManually);
    }	
         //end






	
	/// <summary>
	///	String Info
	/// </summary>
    /// <returns>info</returns>
	public String toString()
	{
		StringBuilder sb = new StringBuilder("MTreeNodeMM[");
		sb.Append("AD_Tree_ID=").Append(GetAD_Tree_ID())
			.Append(",AD_Menu_ID=").Append(GetNode_ID())
			.Append(",Parent_ID=").Append(GetParent_ID())
			.Append("]");
		return sb.ToString();
	}	//	toString

}	//	MTreeNodeMM

}
