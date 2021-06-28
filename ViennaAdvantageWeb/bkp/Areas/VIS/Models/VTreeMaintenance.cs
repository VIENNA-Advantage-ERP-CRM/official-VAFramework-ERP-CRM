/********************************************************
 * Module Name    : Tree
 * Purpose        : To add and delete and update sequence of nodes in tree
 * Created By     : Kiran Sangwan
 * Class Used     : VTreeMaintenance.cs
 * Chronological Development
 * Kiran Sangwan     15-Jan-2008
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Common;
using VAdvantage.DataBase;
using System.Data.SqlClient;
using VAdvantage.SqlExec;
using System.Data;

namespace VAdvantage.Classes
{
    class VTreeMaintenance
    {
        Context ctx = Utility.Env.GetContext();
        private int _AD_Tree_Id;
        private string _TreeType;
        private string nodeId;


        public int AD_Tree_Id
        {
            set { _AD_Tree_Id = value; }
            get { return _AD_Tree_Id; }

        }
        public string TreeType
        {
            set { _TreeType= value; }
            get { return _TreeType; }

        }
        public string Node_Id
        {
            set { nodeId = value; }
            get { return nodeId; }

        }


        /// <summary>
        /// Get login client trees with type MM and OO
        /// </summary>   
        /// <returns>DataSet</returns>
        public DataSet GetClientTrees()
        {
            //string sqlCmd = "select Name,AD_Tree_Id +'|'+treetype+'|'+IsAllNodes as AD_Tree_Id from Ad_Tree where AD_Client_Id="+ctx.GetAD_Client_ID()+" and TreeType in('MM','OO') order by Name";
            string sqlCmd = "select Name,AD_Tree_Id ||'|'||treetype || '|'||IsAllNodes as AD_Tree_Id from Ad_Tree where AD_Client_Id=" + ctx.GetAD_Client_ID() + " and TreeType in('MM','OO') order by Name";
            return ExecuteQuery.ExecuteDataset(sqlCmd);
        }

        /// <summary>
        /// Get menu list of selected tree
        /// </summary>   
        /// <param name="TreeType">Tree Type</param>
        /// <returns>DataSet</returns>
        public DataSet GetTreeList(string TreeType)
        {
            string sqlCmd ="";
            //if tree is of menu type
            if(TreeType=="MM")
            {
                //sqlCmd = "select ad_menu.Name +' ('+ad_menu.Description +')' as Name,ad_menu.ad_menu_id as pkid from ad_menu where " +
                  //            "ad_client_id in(0," + ctx.GetAD_Client_ID() + ") order by ad_menu.Name" ;
                sqlCmd = "select ad_menu.Name ||' ('|| ad_menu.Description ||')' as Name,ad_menu.ad_menu_id as pkid from ad_menu where " +
                              "ad_client_id in(0," + ctx.GetAD_Client_ID() + ") order by ad_menu.Name";
            }
            else if (TreeType == "OO")
            {
                //sqlCmd = "select ad_org.Name +' ('+ ad_org.Description +')' as Name,ad_org.ad_org_id as pkid from ad_org where " +
                //              "ad_client_id in(0," + ctx.GetAD_Client_ID() + ") order by ad_org.Name";
                sqlCmd = "select ad_org.Name ||' ('|| ad_org.Description ||')' as Name,ad_org.ad_org_id as pkid from ad_org where " +
                              "ad_client_id in(0," + ctx.GetAD_Client_ID() + ") order by ad_org.Name";

            }
            return ExecuteQuery.ExecuteDataset(sqlCmd);


        }

        /// <summary>
        /// Add nodes in tree 
        /// </summary>   
        /// <returns></returns>
        public void ActionAdd()
        {
            int iCount=0;
            StringBuilder sqlCmd = new StringBuilder();

            string tableName="AD_TREENODEMM";
            //check tree type
             if (TreeType == "OO")
            {
                tableName= "AD_TREENODE";
            }
           
                 sqlCmd = new StringBuilder("insert into " + tableName + " (AD_CLIENT_ID,AD_ORG_ID,AD_TREE_ID,CREATEDBY," +
                             "NODE_ID,SEQNO,PARENT_ID,UPDATEDBY) select " + ctx.GetAD_Client_ID() + ",0," + _AD_Tree_Id + ",100,");

            //if tree type MM then select from table AD_Menu else from AD_Org
                     if (TreeType == "MM")
                     {
                         sqlCmd.Append("AD_Menu_ID,9999,0,100 from AD_Menu where AD_CLIENT_ID in(0," + ctx.GetAD_Client_ID() + ") and  AD_Menu_Id");
                     }
                     else if (TreeType == "OO")
                     {
                         sqlCmd.Append("AD_Org_ID,9999,0,100 from AD_Org where AD_CLIENT_ID in(0," + ctx.GetAD_Client_ID() + ") and  AD_Org_ID");
                     }
            //.....................................................
            //add which are not in tree
                     sqlCmd.Append(" not in(select NODE_ID from " +
                           "" + tableName + " where AD_TREE_ID=" + _AD_Tree_Id + ")");
                    
            //if nodeId is not blank i.e request is to add particular row
                     if (nodeId != "" && nodeId != null)
                     {
                         if (TreeType == "MM")
                         {
                             sqlCmd.Append(" and ad_menu_id=" + nodeId);
                         }
                         else if (TreeType == "OO")
                         {
                             sqlCmd.Append(" and ad_Org_id=" + nodeId);
                         }
                     }
              
                 ExecuteQuery.ExecuteNonQuery(sqlCmd.ToString());
           
        }

        /// <summary>
        /// delete nodes in tree 
        /// </summary>   
        /// <returns></returns>
        public void ActionDelete(bool IsDeleteAll)
        {
            StringBuilder sqlCmd = new StringBuilder("delete from");
            
            //check tree type
            if (_TreeType == "MM")
            {
                sqlCmd.Append(" AD_TREENODEMM ");
            }
            else if (_TreeType == "OO")
            {
                sqlCmd.Append(" AD_TREENODE ");
            }

            sqlCmd.Append("where Ad_Tree_Id=" + _AD_Tree_Id + "");
            
            //if tree type is MM then dont delete TreeMaintainence  node
            if (_TreeType == "MM")
            {
                sqlCmd.Append(" and Node_Id not in " +
                    "(select AD_Menu_Id from AD_Menu where AD_Form_Id in " +
                    "(select AD_Form_Id from AD_Form where ClassName='VAdvantage.Apps.Form.VTreeMaintenance'))");
            }
            //if request is to delete a particular row
            if (!IsDeleteAll)
            {
                sqlCmd.Append(" and Node_Id=" + nodeId + "");
            }
            
            ExecuteQuery.ExecuteNonQuery(sqlCmd.ToString());
        }

        /// <summary>
        /// check if selected node is TreeMaintenance node
        /// </summary>   
        /// <param name="Node_Id">selected node id</param>
        /// <returns></returns>
        public int CheckMaintenenceNode(int Node_Id)
        {
            string sqlCmd="select count(AD_Menu_Id) from AD_Menu where AD_Form_Id in " +
                    "(select AD_Form_Id from AD_Form where ClassName='VAdvantage.Apps.Form.VTreeMaintenance') and AD_Menu_id="+Node_Id;
            return int.Parse(ExecuteQuery.ExecuteScalar(sqlCmd));

        }

        /// <summary>
        /// get node name
        /// </summary>   
        /// <returns></returns>
        public string GetNodeName()
        {
            string sqlCmd = "Select Name from ";
            if (_TreeType == "MM")
            {
                sqlCmd += "AD_Menu where AD_Menu_Id=" + nodeId;
            }
            else if (_TreeType == "OO")
            {
                sqlCmd += "AD_Org where AD_Org_Id=" + nodeId;
            }
            return ExecuteQuery.ExecuteScalar(sqlCmd).ToString();
        }
    }

}
