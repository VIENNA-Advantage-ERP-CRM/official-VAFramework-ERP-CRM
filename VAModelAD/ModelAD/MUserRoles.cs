using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using VAdvantage.Process;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Classes;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MUserRoles : X_VAF_UserContact_Roles
    {
        Ctx role_ctx = null;
        /// <summary>
        /// Get User Roles Of Role
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Role_ID">role id</param>
        /// <returns>Array of user roles</returns>
        public static MUserRoles[] GetOfRole(Ctx ctx, int VAF_Role_ID)
        {
            String sql = "SELECT * FROM VAF_UserContact_Roles WHERE VAF_Role_ID=@roleid";
            List<MUserRoles> list = new List<MUserRoles>();
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@roleid", VAF_Role_ID);

                DataSet ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql, param);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MUserRoles(ctx, dr, null));
                }
                ds = null;
            }
            catch (Exception e)
            {

                s_log.Log(Level.SEVERE, "getOfRole", e);
            }

            MUserRoles[] retValue = new MUserRoles[list.Count()];
            retValue = list.ToArray();
            return retValue;
        }	//	getOfRole


        /// <summary>
        /// Get User Roles Of User
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_UserContact_ID">ad user id</param>
        /// <returns>array of user roles</returns>
        public static MUserRoles[] GetOfUser(Ctx ctx, int VAF_UserContact_ID)
        {
            String sql = "SELECT * FROM VAF_UserContact_Roles WHERE VAF_UserContact_ID=@userid";
            List<MUserRoles> list = new List<MUserRoles>();
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@userid", VAF_UserContact_ID);

                DataSet ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql, param);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MUserRoles(ctx, dr, null));
                }
                ds = null;
            }
            catch (Exception e)
            {

                s_log.Log(Level.SEVERE, "getOfUser", e);
            }

            MUserRoles[] retValue = new MUserRoles[list.Count()];
            retValue = list.ToArray();
            return retValue;
        }	//	getOfUser

        /**	Static Logger	*/
        private static VLogger s_log = VLogger.GetVLogger(typeof(MUserRoles).FullName);


        /// <summary>
        /// Persistence Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ignored">ignored</param>
        /// <param name="trxName">optional transaction</param>
        public MUserRoles(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, ignored, trxName)
        {
            role_ctx = ctx;
            if (ignored != 0)
                throw new Exception("Multi-Key");
        }	//	MUserRoles


        /// <summary>
        /// Load constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">resultset</param>
        /// <param name="trxName">optional transaction name</param>
        public MUserRoles(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
            role_ctx = ctx;
        }	//	MUserRoles


        /// <summary>
        /// Full Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_UserContact_ID">user id</param>
        /// <param name="VAF_Role_ID">role id</param>
        /// <param name="trxName">optional transaction name</param>
        public MUserRoles(Ctx ctx, int VAF_UserContact_ID, int VAF_Role_ID, Trx trxName)
            : this(ctx, 0, trxName)
        {
            role_ctx = ctx;
            SetVAF_UserContact_ID(VAF_UserContact_ID);
            SetVAF_Role_ID(VAF_Role_ID);
        }	//	MUserRoles

        /// <summary>
        /// Set User/Contact.
        /// User within the system - Internal or Business Partner Contact
        /// </summary>
        /// <param name="VAF_UserContact_ID">user</param>
        public new void SetVAF_UserContact_ID(int VAF_UserContact_ID)
        {
            Set_ValueNoCheck("VAF_UserContact_ID", VAF_UserContact_ID);
        }	//	setVAF_UserContact_ID

        /// <summary>
        /// Set Role
        /// Responsiblity Role
        /// </summary>
        /// <param name="VAF_Role_ID">Role ID</param>
        public new void SetVAF_Role_ID(int VAF_Role_ID)
        {
            Set_ValueNoCheck("VAF_Role_ID", VAF_Role_ID);
        }	//	setVAF_Role_ID

        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (success)
            {
                if (!IsActive())
                {
                    deleteRoleForLogin();
                }
                //Update Role on Japser User..............
                MUser obj = new MUser(role_ctx, GetVAF_UserContact_ID(), null);
                if(obj.IsVA039_IsJasperUser())
                {
                    obj.createJasperUser();
                }
            }
            return success;
        }

        protected override bool AfterDelete(bool success)
        {
            if (success)
            {
                deleteRoleForLogin();
                MUser obj = new MUser(role_ctx, GetVAF_UserContact_ID(), null);
                if (obj.IsVA039_IsJasperUser())
                {
                    obj.createJasperUser();
                }
            }
            return success;
        }

        private void deleteRoleForLogin()
        {
            DB.ExecuteQuery("DELETE FROM VAF_Loginsetting WHERE VAF_UserContact_ID=" + GetVAF_UserContact_ID() + " AND VAF_Role_ID = " + GetVAF_Role_ID());
        }
    }
}
