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
        private int _VAF_TreeInfo_Id;
        private string _TreeType;
        private string nodeId;


        public int VAF_TreeInfo_Id
        {
            set { _VAF_TreeInfo_Id = value; }
            get { return _VAF_TreeInfo_Id; }

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
            //string sqlCmd = "select Name,VAF_TreeInfo_Id +'|'+treetype+'|'+IsAllNodes as VAF_TreeInfo_Id from VAF_TreeInfo where VAF_Client_Id="+ctx.GetVAF_Client_ID()+" and TreeType in('MM','OO') order by Name";
            string sqlCmd = "select Name,VAF_TreeInfo_Id ||'|'||treetype || '|'||IsAllNodes as VAF_TreeInfo_Id from VAF_TreeInfo where VAF_Client_Id=" + ctx.GetVAF_Client_ID() + " and TreeType in('MM','OO') order by Name";
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
                //sqlCmd = "select VAF_MenuConfig.Name +' ('+VAF_MenuConfig.Description +')' as Name,VAF_MenuConfig.VAF_MenuConfig_id as pkid from VAF_MenuConfig where " +
                  //            "vaf_client_id in(0," + ctx.GetVAF_Client_ID() + ") order by VAF_MenuConfig.Name" ;
                sqlCmd = "select VAF_MenuConfig.Name ||' ('|| VAF_MenuConfig.Description ||')' as Name,VAF_MenuConfig.VAF_MenuConfig_id as pkid from VAF_MenuConfig where " +
                              "vaf_client_id in(0," + ctx.GetVAF_Client_ID() + ") order by VAF_MenuConfig.Name";
            }
            else if (TreeType == "OO")
            {
                //sqlCmd = "select vaf_org.Name +' ('+ vaf_org.Description +')' as Name,vaf_org.vaf_org_id as pkid from vaf_org where " +
                //              "vaf_client_id in(0," + ctx.GetVAF_Client_ID() + ") order by vaf_org.Name";
                sqlCmd = "select vaf_org.Name ||' ('|| vaf_org.Description ||')' as Name,vaf_org.vaf_org_id as pkid from vaf_org where " +
                              "vaf_client_id in(0," + ctx.GetVAF_Client_ID() + ") order by vaf_org.Name";

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

            string tableName="VAF_TreeInfoChildMenu";
            //check tree type
             if (TreeType == "OO")
            {
                tableName= "VAF_TreeInfoChild";
            }
           
                 sqlCmd = new StringBuilder("insert into " + tableName + " (VAF_CLIENT_ID,VAF_ORG_ID,VAF_TreeInfo_ID,CREATEDBY," +
                             "NODE_ID,SEQNO,PARENT_ID,UPDATEDBY) select " + ctx.GetVAF_Client_ID() + ",0," + _VAF_TreeInfo_Id + ",100,");

            //if tree type MM then select from table VAF_MenuConfig else from VAF_Org
                     if (TreeType == "MM")
                     {
                         sqlCmd.Append("VAF_MenuConfig_ID,9999,0,100 from VAF_MenuConfig where VAF_CLIENT_ID in(0," + ctx.GetVAF_Client_ID() + ") and  VAF_MenuConfig_Id");
                     }
                     else if (TreeType == "OO")
                     {
                         sqlCmd.Append("VAF_Org_ID,9999,0,100 from VAF_Org where VAF_CLIENT_ID in(0," + ctx.GetVAF_Client_ID() + ") and  VAF_Org_ID");
                     }
            //.....................................................
            //add which are not in tree
                     sqlCmd.Append(" not in(select NODE_ID from " +
                           "" + tableName + " where VAF_TreeInfo_ID=" + _VAF_TreeInfo_Id + ")");
                    
            //if nodeId is not blank i.e request is to add particular row
                     if (nodeId != "" && nodeId != null)
                     {
                         if (TreeType == "MM")
                         {
                             sqlCmd.Append(" and VAF_MenuConfig_id=" + nodeId);
                         }
                         else if (TreeType == "OO")
                         {
                             sqlCmd.Append(" and vaf_org_id=" + nodeId);
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
                sqlCmd.Append(" VAF_TreeInfoChildMenu ");
            }
            else if (_TreeType == "OO")
            {
                sqlCmd.Append(" VAF_TreeInfoChild ");
            }

            sqlCmd.Append("where VAF_TreeInfo_Id=" + _VAF_TreeInfo_Id + "");
            
            //if tree type is MM then dont delete TreeMaintainence  node
            if (_TreeType == "MM")
            {
                sqlCmd.Append(" and Node_Id not in " +
                    "(select VAF_MenuConfig_Id from VAF_MenuConfig where VAF_Page_Id in " +
                    "(select VAF_Page_Id from VAF_Page where ClassName='VAdvantage.Apps.Form.VTreeMaintenance'))");
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
            string sqlCmd="select count(VAF_MenuConfig_Id) from VAF_MenuConfig where VAF_Page_Id in " +
                    "(select VAF_Page_Id from VAF_Page where ClassName='VAdvantage.Apps.Form.VTreeMaintenance') and VAF_MenuConfig_id="+Node_Id;
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
                sqlCmd += "VAF_MenuConfig where VAF_MenuConfig_Id=" + nodeId;
            }
            else if (_TreeType == "OO")
            {
                sqlCmd += "VAF_Org where VAF_Org_Id=" + nodeId;
            }
            return ExecuteQuery.ExecuteScalar(sqlCmd).ToString();
        }
    }

}
