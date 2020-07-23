using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
   public class MGroupWorkflow:X_AD_Group_Workflow
    {
            /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_GroupWorkflow_ID">id</param>
        /// <param name="trxName">transaction</param>
       public MGroupWorkflow(Ctx ctx, int AD_GroupWorkflow_ID, Trx trxName)
           : base(ctx, AD_GroupWorkflow_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">transaction</param>
        public MGroupWorkflow(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }
        public MGroupWorkflow(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        { }

        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (newRecord)
            {
                InsertNewRecordInRole();
            }
            else
            {
                UpdateRole(IsActive());
            }
            return success;
        }

        protected override bool AfterDelete(bool success)
        {
            UpdateRole(false);
            return success;
        }

        private bool UpdateRole(bool isActive)
        {
            if (isActive)
            {
                DB.ExecuteQuery(@"UPDATE ad_workflow_access
                                    SET IsActive      ='Y',IsReadWrite='Y'
                                    WHERE ad_workflow_id=" + GetAD_Workflow_ID() + @"
                                    AND AD_Role_ID   IN
                                      ( SELECT AD_Role_ID FROM AD_Role_Group WHERE ad_groupinfo_id=" + GetAD_GroupInfo_ID() + ")");
            }
            else
            {
                DB.ExecuteQuery(@"UPDATE ad_workflow_access
                                    SET IsActive      ='N',IsReadWrite='N'
                                    WHERE ad_workflow_id=" + GetAD_Workflow_ID() + @"
                                    AND AD_Role_ID   IN
                                      ( SELECT AD_Role_ID FROM AD_Role_Group WHERE ad_groupinfo_id=" + GetAD_GroupInfo_ID() + ")");
            }
            return true;
        }

        private void InsertNewRecordInRole()
        {
            DataSet ds = DB.ExecuteDataset("SELECT AD_Role_ID FROM AD_Role_Group WHERE ad_groupinfo_id=" + GetAD_GroupInfo_ID() );
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    X_AD_Workflow_Access access = new X_AD_Workflow_Access(GetCtx(), 0, null);
                    access.SetAD_Workflow_ID(GetAD_Workflow_ID());
                    access.SetAD_Role_ID(Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Role_ID"]));
                    access.SetIsReadWrite(true);
                    access.Save();
                }
            }
        }

    }
}
