/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : TreeMaintenance
 * Purpose        : Tree Maintenance
 * Class Used     : ProcessEngine.SvrProcess
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

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class TreeMaintenance:ProcessEngine.SvrProcess
    {
    /**	Tree				*/
	private int		m_VAF_TreeInfo_ID;
	
	/// <summary>
	/// Prepare - e.g., get Parameters.
	/// </summary>
	protected override void Prepare()
	{
		ProcessInfoParameter[] para = GetParameter();
		for (int i = 0; i < para.Length; i++)
		{
			String name = para[i].GetParameterName();
			if (para[i].GetParameter() == null)
            {
				;
            }
			else
            {
				log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
		}
		m_VAF_TreeInfo_ID = GetRecord_ID();		//	from Window
	}	//	prepare

	/// <summary>
	/// Perform Process.
	/// </summary>
	/// <returns>Message (clear text)</returns>
	protected override String DoIt()
	{
		log.Info("VAF_TreeInfo_ID=" + m_VAF_TreeInfo_ID);
        if (m_VAF_TreeInfo_ID == 0)
        {
            throw new ArgumentException("Tree_ID = 0");
        }
		MVAFTreeInfo tree = new MVAFTreeInfo (GetCtx(), m_VAF_TreeInfo_ID, Get_Trx());
        if (tree == null || tree.GetVAF_TreeInfo_ID() == 0)
        {
            throw new ArgumentException("No Tree -" + tree);
        }
		//
        if (MVAFTreeInfo.TREETYPE_BoM.Equals(tree.GetTreeType()))
        {
            return "BOM Trees not implemented";
        }
		return VerifyTree(tree);
	}	//	doIt

	/// <summary>
	/// Verify Tree
	/// </summary>
    /// <param name="tree">tree</param>
	/// <returns>message</returns>
	private String VerifyTree (MVAFTreeInfo tree)
	{
        if (tree.GetVAF_TableView_ID(true) == 0)
        {
            tree.UpdateTrees();
        }
		String nodeTableName = tree.GetNodeTableName();
		String sourceTableName = tree.GetSourceTableName(true);
		String sourceTableKey = sourceTableName + "_ID";
		int VAF_Client_ID = tree.GetVAF_Client_ID();
		int VAB_Element_ID = 0;
		if (MVAFTreeInfo.TREETYPE_ElementValue.Equals(tree.GetTreeType()))
		{
			String sql = "SELECT VAB_Element_ID FROM VAB_Element "
				+ "WHERE VAF_TreeInfo_ID=" + tree.GetVAF_TreeInfo_ID();
			VAB_Element_ID = DataBase.DB.GetSQLValue(null, sql);
            if (VAB_Element_ID <= 0)
            {
                throw new Exception("No Account Element found");
            }
		}
		
		//	Delete unused
		StringBuilder sql1 = new StringBuilder();
		sql1.Append("DELETE FROM ").Append(nodeTableName)
			.Append(" WHERE VAF_TreeInfo_ID=").Append(tree.GetVAF_TreeInfo_ID())
			.Append(" AND Node_ID NOT IN (SELECT ").Append(sourceTableKey)
			.Append(" FROM ").Append(sourceTableName)
			.Append(" WHERE VAF_Client_ID IN (0,").Append(VAF_Client_ID).Append(")");
        if (VAB_Element_ID > 0)
        {
            sql1.Append(" AND VAB_Element_ID=").Append(VAB_Element_ID);
        }
		sql1.Append(")");
		log.Finer(sql1.ToString());
		//
		int deletes = DataBase.DB.ExecuteQuery(sql1.ToString(),null, Get_Trx());
		AddLog(0,null, new Decimal(deletes), tree.GetName()+ " Deleted");
        if (!tree.IsAllNodes())
        {
            return tree.GetName() + " OK";
        }
		
		//	Insert new
		int inserts = 0;
		StringBuilder sql2 = new StringBuilder();
		sql2.Append("SELECT ").Append(sourceTableKey)
			.Append(" FROM ").Append(sourceTableName)
			.Append(" WHERE VAF_Client_ID IN (0,").Append(VAF_Client_ID).Append(")");
        if (VAB_Element_ID > 0)
        {
            sql2.Append(" AND VAB_Element_ID=").Append(VAB_Element_ID);
        }
        sql2.Append(" AND ").Append(sourceTableKey)
            .Append("  NOT IN (SELECT Node_ID FROM ").Append(nodeTableName)
            .Append(" WHERE VAF_TreeInfo_ID=").Append(tree.GetVAF_TreeInfo_ID()).Append(")")
            .Append(" ORDER BY Upper(name)");
		log.Finer(sql2.ToString());
		//
		Boolean ok = true;
        IDataReader idr = null;
		//PreparedStatement pstmt = null;
        try
        {
            //pstmt = DataBase.prepareStatement(sql.toString(), Get_Trx());
            //ResultSet rs = pstmt.executeQuery();
            idr = DataBase.DB.ExecuteReader(sql2.ToString(), null, Get_Trx());

            //Manish 8/06/2016

            int setSeqManually = -1;

            //End

            while (idr.Read())
            {
                int Node_ID = Utility.Util.GetValueOfInt(idr[0]);// rs.getInt(1);
                PO node = null;
                if (nodeTableName.Equals("VAF_TreeInfoChild"))
                {
                  // node = new MVAFTreeInfoChild(tree, Node_ID);

                   //Manish
                   setSeqManually += 1;
                   node = new MVAFTreeInfoChild(tree, Node_ID, setSeqManually);
                    //end

                }
                else if (nodeTableName.Equals("VAF_TreeInfoChildBPart"))
                {
                   //node = new MTreeNodeBP(tree, Node_ID);

                   //Manish
                   setSeqManually += 1;
                   node = new MVAFTreeInfoChildBPart(tree, Node_ID, setSeqManually);
                    //end

                }
                else if (nodeTableName.Equals("VAF_TreeInfoChildProd"))
                {
                  //node = new MTreeNodePR(tree, Node_ID);

                  //Manish
                  setSeqManually += 1;
                  node = new MVAFTreeInfoChildProd(tree, Node_ID, setSeqManually);
                    //end


                }
                else if (nodeTableName.Equals("VAF_TreeInfoChildCMC"))
                {
                   //node = new MVAFTreeInfoChildCMC(tree, Node_ID);

                   //Manish
                   setSeqManually += 1;
                   node = new MVAFTreeInfoChildCMC(tree, Node_ID, setSeqManually);
                    //end


                }
                else if (nodeTableName.Equals("VAF_TreeInfoChildCMM"))
                {
                  //node = new MTreeNodeCMM(tree, Node_ID);

                  //Manish
                  setSeqManually += 1;
                  node = new MVAFTreeInfoChildCMM(tree, Node_ID, setSeqManually);
                    //end


                }
                else if (nodeTableName.Equals("VAF_TreeInfoChildCMS"))
                {
                  // node = new MTreeNodeCMS(tree, Node_ID);

                   //Manish
                   setSeqManually += 1;
                   node = new MVAFTreeInfoChildCMS(tree, Node_ID, setSeqManually);
                    //end

                }
                else if (nodeTableName.Equals("VAF_TreeInfoChildCMT"))
                {
                   //node = new MVAFTreeInfoChildCMT(tree, Node_ID);

                   //Manish
                   setSeqManually += 1;
                   node = new MVAFTreeInfoChildCMT(tree, Node_ID, setSeqManually);
                    //end

                }
                else if (nodeTableName.Equals("VAF_TreeInfoChildMenu"))
                {
                   // node = new MVAFTreeInfoChildMenu(tree, Node_ID);
                    
                    //Manish
                    setSeqManually += 1;
                    node = new MVAFTreeInfoChildMenu(tree, Node_ID, setSeqManually);
                    //end
                   
                  
                }
                			
                if (node == null)
                    log.Log(Level.SEVERE, "No Model for " + nodeTableName);
                else
                {
                    if (node.Save())
                    {
                        inserts++;
                    }
                    else
                    {
                        log.Log(Level.SEVERE, "Could not add to " + tree + " Node_ID=" + Node_ID);
                    }
                }
            }

        }
        catch (Exception e)
        {
            log.Log(Level.SEVERE, sql2.ToString(), e);
            ok = false;
        }
        finally
        {
            idr.Close();
        }
		AddLog(0,null, new Decimal(inserts), tree.GetName()+ " Inserted");
		return tree.GetName() + (ok ? " OK" : " Error");
	}	//	verifyTree

}	//	TreeMaintenence

}
