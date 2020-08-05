/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : RoleAccessUpdate
 * Purpose        : Update Role Access 
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           30-Nov-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;


using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class RoleAccessUpdate : ProcessEngine.SvrProcess
    {
        //	Update Role
        private int _AD_Role_ID = 0;
        //	Update Roles of Client	
        private int _AD_Client_ID = 0;
        /// <summary>
        ///	Prepare
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
                else if (name.Equals("AD_Role_ID"))
                {
                    _AD_Role_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("AD_Client_ID"))
                {
                    _AD_Client_ID = para[i].GetParameterAsInt();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }	//	prepare

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {
            int lenth = 0;
            log.Info("AD_Client_ID=" + _AD_Client_ID + ", AD_Role_ID=" + _AD_Role_ID);
            //
            if (_AD_Role_ID != 0 && _AD_Role_ID != -1)
            {
                UpdateRole(new MRole(GetCtx(), _AD_Role_ID, Get_TrxName()));

            }
            else
            {
                String sql = "SELECT * FROM AD_Role ";
                if (_AD_Client_ID != 0)
                {
                    lenth = 1;
                    sql += "WHERE AD_Client_ID=@Param1 ";
                }
                sql += "ORDER BY AD_Client_ID, Name";
                SqlParameter[] Param = new SqlParameter[lenth];
                IDataReader idr = null;
                DataTable dt = null;
                //PreparedStatement pstmt = null;
                try
                {
                    //pstmt = DataBase.prepareStatement(sql, get_TrxName());

                    if (_AD_Client_ID != 0)
                    {
                        Param[0] = new SqlParameter("@Param1", _AD_Client_ID);
                    }
                    idr = DataBase.DB.ExecuteReader(sql, Param, Get_TrxName());
                    //pstmt.setInt(1, _AD_Client_ID);
                    //ResultSet rs = pstmt.executeQuery();

                    dt = new DataTable();
                    dt.Load(idr);
                    idr.Close();
                    //while (rs.next())
                    foreach (DataRow dr in dt.Rows)
                    {
                        UpdateRole(new MRole(GetCtx(), dr, Get_TrxName()));
                    }

                }
                catch (Exception e)
                {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                    log.Log(Level.SEVERE, sql, e);
                }
                finally
                {
                    dt = null;
                    if (idr != null)
                    {
                        idr.Close();
                    }
                }

            }

            return "";
        }	//	doIt

        /// <summary>
        ///	Update Role
        /// </summary>
        /// <param name="role">role</param>
        private void UpdateRole(MRole role)
        {
            AddLog(0, null, null, role.GetName() + ": "
                + role.UpdateAccessRecords());
        }	//	updateRole

    }	//	RoleAccessUpdate

}
