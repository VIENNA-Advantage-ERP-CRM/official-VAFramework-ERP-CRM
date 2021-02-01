using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MVAFGroupWindow : X_VAF_Group_Window
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_GroupWindow_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAFGroupWindow(Ctx ctx, int VAF_GroupWindow_ID, Trx trxName)
            : base(ctx, VAF_GroupWindow_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">transaction</param>
        public MVAFGroupWindow(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }
        public MVAFGroupWindow(Ctx ctx, IDataReader idr, Trx trxName)
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
                DB.ExecuteQuery(@"UPDATE VAF_Screen_Rights
                                    SET IsActive      ='Y',IsReadWrite='Y'
                                    WHERE ad_window_id=" + GetVAF_Screen_ID() + @"
                                    AND VAF_Role_ID   IN
                                      ( SELECT VAF_Role_ID FROM VAF_Role_Group WHERE VAF_Groupinfo_id=" + GetVAF_GroupInfo_ID() + ")");
            }
            else
            {
                DB.ExecuteQuery(@"UPDATE VAF_Screen_Rights
                                    SET IsActive      ='N',IsReadWrite='N'
                                    WHERE ad_window_id=" + GetVAF_Screen_ID() + @"
                                    AND VAF_Role_ID   IN
                                      ( SELECT VAF_Role_ID FROM VAF_Role_Group WHERE VAF_Groupinfo_id=" + GetVAF_GroupInfo_ID() + ")");
            }
            return true;
        }

        private void InsertNewRecordInRole()
        {
            DataSet ds = DB.ExecuteDataset("SELECT VAF_Role_ID FROM VAF_Role_Group WHERE VAF_Groupinfo_id=" + GetVAF_GroupInfo_ID() );
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    MVAFScreenRights access = new MVAFScreenRights(GetCtx(), 0, null);
                    access.SetVAF_Screen_ID(GetVAF_Screen_ID());
                    access.SetVAF_Role_ID(Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Role_ID"]));
                    access.SetIsReadWrite(true);
                    access.Save();
                }
            }
        }
    }
}
