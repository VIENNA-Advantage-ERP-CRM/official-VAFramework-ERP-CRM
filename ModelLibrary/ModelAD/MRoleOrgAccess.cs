using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using VAdvantage.Utility;
using VAdvantage.Logging;
using System.Data.SqlClient;

namespace VAdvantage.Model
{
    public class MRoleOrgAccess : X_AD_Role_OrgAccess
    {
        private String _clientName;
        private String _orgName;
        //Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MRoleOrgAccess).FullName);

        /// <summary>
        /// Get Organizational Access of Role
        /// </summary>
        /// <param name="ctx"> context</param>
        /// <param name="AD_Role_ID">role</param>
        /// <returns>array of Role Org Access</returns>
        public static MRoleOrgAccess[] GetOfRole(Ctx ctx, int AD_Role_ID)
        {
            return Get(ctx, "SELECT * FROM AD_Role_OrgAccess WHERE AD_Role_ID=@param1", AD_Role_ID);
        }

        /// <summary>
        /// Get Organizational Access of Client
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Client_ID">client</param>
        /// <returns>array of Role Org Access</returns>
        public static MRoleOrgAccess[] GetOfClient(Ctx ctx, int AD_Client_ID)
        {
            return Get(ctx, "SELECT * FROM AD_Role_OrgAccess WHERE AD_Client_ID=" + AD_Client_ID, AD_Client_ID);
        }

        /// <summary>
        /// Get Organizational Access of Org
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Org_ID"></param>
        /// <returns>array of Role Org Access</returns>
        public static MRoleOrgAccess[] GetOfOrg(Ctx ctx, int AD_Org_ID)
        {
            return Get(ctx, "SELECT * FROM AD_Role_OrgAccess WHERE AD_Org_ID=@param1", AD_Org_ID);
        }

        /// <summary>
        /// Get Organizational Info
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="sql">sql command</param>
        /// <param name="id">id</param>
        /// <returns>array of Role Org Access</returns>
        private static MRoleOrgAccess[] Get(Ctx ctx, String sql, int id)
        {
            List<MRoleOrgAccess> list = new List<MRoleOrgAccess>();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@param1", id);
                idr = DataBase.DB.ExecuteReader(sql, param, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MRoleOrgAccess(ctx, dr, null));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, "get", e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;

            }

            MRoleOrgAccess[] retValue = new MRoleOrgAccess[list.Count];
            retValue = list.ToArray();
            return retValue;
        }


        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">transaction</param>
        public MRoleOrgAccess(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Persistency Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ignored">ignored</param>
        /// <param name="trxName">transaction</param>
        public MRoleOrgAccess(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, 0, trxName)
        {
            if (ignored != 0)
                throw new ArgumentException("Multi-Key");
            SetIsReadOnly(false);
        }

        /// <summary>
        /// Organization Constructor
        /// </summary>
        /// <param name="org">org</param>
        /// <param name="AD_Role_ID">role id</param>
        public MRoleOrgAccess(MOrg org, int AD_Role_ID)
            : this(org.GetCtx(), 0, org.Get_TrxName())
        {
            SetClientOrg(org);
            SetAD_Role_ID(AD_Role_ID);
        }

        /// <summary>
        /// Role Constructor
        /// </summary>
        /// <param name="role">role</param>
        /// <param name="AD_Org_ID">org</param>
        public MRoleOrgAccess(MRole role, int AD_Org_ID)
            : this(role.GetCtx(), 0, role.Get_TrxName())
        {
            SetClientOrg(role.GetAD_Client_ID(), AD_Org_ID);
            SetAD_Role_ID(role.GetAD_Role_ID());
        }


        /// <summary>
        /// Create Organizational Access for all Automatic Roles
        /// </summary>
        /// <param name="org">org</param>
        /// <returns>true if created</returns>
        public static bool CreateForOrg(MOrg org)
        {
            int counter = 0;
            MRole[] roles = MRole.GetOfClient(org.GetCtx());
            for (int i = 0; i < roles.Length; i++)
            {
                if (!roles[i].IsManual())
                {
                    MRoleOrgAccess orgAccess = new MRoleOrgAccess(org, roles[i].GetAD_Role_ID());
                    if (orgAccess.Save())
                        counter++;
                }
            }
            _log.Info(org + " - created #" + counter);
            return counter != 0;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MRoleOrgAccess[");
            sb.Append("AD_Role_ID=").Append(GetAD_Role_ID())
                .Append(",AD_Client_ID=").Append(GetAD_Client_ID())
                .Append(",AD_Org_ID=").Append(GetAD_Org_ID())
                .Append(",RO=").Append(IsReadOnly());
            sb.Append("]");
            return sb.ToString();
        }


        /// <summary>
        /// Extended String Representation
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>extended info</returns>
        public String ToStringX(Ctx ctx)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Msg.Translate(ctx, "AD_Client_ID")).Append("=").Append(GetClientName()).Append(" - ")
                .Append(Msg.Translate(ctx, "AD_Org_ID")).Append("=").Append(GetOrgName());
            return sb.ToString();
        }

        /// <summary>
        /// /Get Client Name
        /// </summary>
        /// <returns>name</returns>
        public String GetClientName()
        {
            if (_clientName == null)
            {
                String sql = "SELECT c.Name, o.Name "
                    + "FROM AD_Client c INNER JOIN AD_Org o ON (c.AD_Client_ID=o.AD_Client_ID) "
                    + "WHERE o.AD_Org_ID=" + GetAD_Org_ID();
                DataTable dt = null;
                IDataReader idr = null;
                try
                {
                    idr = DataBase.DB.ExecuteReader(sql, null, null);
                    dt = new DataTable();
                    dt.Load(idr);
                    idr.Close();
                    foreach (DataRow dr in dt.Rows)
                    {
                        _clientName = dr[0].ToString();//.getString(1);
                        _orgName = dr[1].ToString();//  rs.getString(2);
                    }
                }
                catch (Exception e)
                {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                    log.Log(Level.SEVERE, "getClientName", e);
                }
            }
            return _clientName;
        }

        /// <summary>
        /// Get Client Name
        /// </summary>
        /// <returns>name</returns>
        public String GetOrgName()
        {
            if (_orgName == null)
            {
                GetClientName();
            }
            return _orgName;
        }


        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (success)
            {
                if (!IsActive())
                {
                    UpdateLoginSettings();
                }
            }
            return success;
        }

        protected override bool AfterDelete(bool success)
        {
            if (success)
            {
                UpdateLoginSettings();
            }
            return success;
        }

        private void UpdateLoginSettings()
        {
            MRole role = new MRole(GetCtx(), GetAD_Role_ID(), null);
            if (!role.IsUseUserOrgAccess())
            {
                DB.ExecuteQuery("DELETE FROM ad_loginsetting WHERE AD_Org_ID=" + GetAD_Org_ID() + " AND AD_Role_ID=" + GetAD_Role_ID());
            }
            else
            {

                DataSet ds = DB.ExecuteDataset("SELECT AD_User_ID FROM ad_user_orgaccess WHERE AD_Org_ID=" + GetAD_Org_ID());
                List<int> UIDs = new List<int>();
                if (ds != null || ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        UIDs.Add(Convert.ToInt32(ds.Tables[0].Rows[i]["AD_User_ID"]));
                    }

                }

                string sql = "SELECT AD_User_ID FROM ad_loginsetting   WHERE AD_Org_ID=" + GetAD_Org_ID() + " AND AD_Role_ID=" + GetAD_Role_ID();
                ds = DB.ExecuteDataset(sql);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i <= ds.Tables[0].Rows.Count; i++)
                    {
                        if (UIDs.IndexOf(Convert.ToInt32(ds.Tables[0].Rows[i]["AD_User_ID"])) == -1)
                        {
                            DB.ExecuteQuery("DELETE FROM ad_loginsetting WHERE AD_User_ID=" + ds.Tables[0].Rows[i]["AD_User_ID"].ToString());
                        }
                    }
                }
                else
                {
                    DB.ExecuteQuery("DELETE FROM ad_loginsetting WHERE AD_Org_ID=" + GetAD_Org_ID() + " AND AD_Role_ID=" + GetAD_Role_ID());
                }
            }
        }


    }
}
