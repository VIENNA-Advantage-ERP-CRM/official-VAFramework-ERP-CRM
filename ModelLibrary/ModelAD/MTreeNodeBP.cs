/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MTreeNodeBP
 * Purpose        : (Disk) Tree Node Model BPartner
 * Class Used     : X_AD_TreeNodeBP
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
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Data.SqlClient;

namespace VAdvantage.Model
{
    public class MTreeNodeBP : X_AD_TreeNodeBP
    {
        /**	Static Logger	*/
	private static VLogger	_log	= VLogger.GetVLogger (typeof(MTreeNodeBP).FullName);
    /// <summary>
    /// Get Tree Node
	/// </summary>
    /// <param name="tree">tree</param>
    /// <param name="Node_ID">node</param>
    /// <returns>node or null</returns>
	public static MTreeNodeBP Get(MTree tree, int Node_ID)
	{
		MTreeNodeBP retValue = null;
		String sql = "SELECT * FROM AD_TreeNodeBP WHERE AD_Tree_ID=@Param1 AND Node_ID=@Param2";
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
            dt = new DataTable();
            idr=DataBase.DB.ExecuteReader(sql,Param, tree.Get_TrxName());
            dt.Load(idr);
            idr.Close();
			//if (rs.next ())
            foreach(DataRow dr in dt.Rows)
            {
				retValue = new MTreeNodeBP (tree.GetCtx(),dr, tree.Get_TrxName());
            }
		}
		catch (Exception e)
		{
			_log.Log(Level.SEVERE, "get", e);
		}
		finally
        {
            
            dt=null;
            idr.Close();
        }
		return retValue;
	}	//	get

	

	/// <summary>
	/// Load Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="dr">datarow</param>
	/// <param name="trxName">transaction</param>
	public MTreeNodeBP(Ctx ctx,DataRow dr, Trx trxName):base(ctx, dr, trxName)
	{
		//super(ctx, rs, trxName);
	}	//	MTreeNodeBP

	/// <summary>
	/// Full Constructor
	/// </summary>
	/// <param name="tree">tree</param>
    /// <param name="Node_ID">node</param>
	public MTreeNodeBP (MTree tree, int Node_ID):base(tree.GetCtx(), 0, tree.Get_TrxName())
	{
		//super (tree.getCtx(), 0, tree.get_TrxName());
		SetClientOrg(tree);
		SetAD_Tree_ID (tree.GetAD_Tree_ID());
		SetNode_ID(Node_ID);
		//	Add to root
		SetParent_ID(0);
		SetSeqNo (9999);
	}	//	MTreeNodeBP


        //Manish 8/6/2016

    /// <summary>
    /// Full Constructor
    /// </summary>
    /// <param name="tree">tree</param>
    /// <param name="Node_ID">node</param>
    public MTreeNodeBP(MTree tree, int Node_ID, int setSeqManually)
        : base(tree.GetCtx(), 0, tree.Get_TrxName())
    {
        //super (tree.getCtx(), 0, tree.get_TrxName());
        SetClientOrg(tree);
        SetAD_Tree_ID(tree.GetAD_Tree_ID());
        SetNode_ID(Node_ID);
        //	Add to root
        SetParent_ID(0);
        SetSeqNo(setSeqManually);
    }	//	MTreeNodeBP


        //end







	/// <summary>
	///	String Info
	/// </summary>
    /// <returns>info</returns>
	public String toString()
	{
		StringBuilder sb = new StringBuilder("MTreeNodeBP[");
		sb.Append("AD_Tree_ID=").Append(GetAD_Tree_ID())
			.Append(",C_BPartner_ID=").Append(GetNode_ID())
			.Append(",Parent_ID=").Append(GetParent_ID())
			.Append("]");
		return sb.ToString();
	}	//	toString

}	//	MTreeNodeBP

}
