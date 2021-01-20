using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Data;

namespace VAdvantage.ProcessEngine
{
    public class MCrystalInstance : X_VAF_CrystalInstance
    {
        /// <summary>
        /// 	Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_JInstance_ID">instance or 0</param>
        /// <param name="ignored">no transaction support</param>
        public MCrystalInstance(Ctx ctx, int AD_CrytalInstance_ID, string ignored)
            : base(ctx, AD_CrytalInstance_ID, null)
        {
            if (AD_CrytalInstance_ID == 0)
            {
                int VAF_Role_ID = ctx.GetVAF_Role_ID();
                if (VAF_Role_ID != 0)
                    SetVAF_Role_ID(VAF_Role_ID);
                SetIsProcessing(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">dataRow</param>
        /// <param name="ignored">no transaction support</param>
        public MCrystalInstance(Ctx ctx, DataRow dr, String ignored)
            : base(ctx, dr, null)
        {
        }


        ///// <summary>
        ///// New Constructor
        ///// </summary>
        ///// <param name="ctx">context</param>
        ///// <param name="VAF_Job_ID">Process ID</param>
        ///// <param name="Record_ID">record</param>
        public MCrystalInstance(Ctx ctx, int VAF_Page_ID, int Record_ID)
            : this(ctx, 0, null)
        {
            //SetVAF_Job_ID(VAF_Job_ID);
            SetVAF_Page_ID(VAF_Page_ID);
            SetRecord_ID(Record_ID);
            //SetAD_User_ID(ctx.GetAD_User_ID());
            SetIsProcessing(false);
        }

        /// <summary>
        /// Set VAF_Job_ID.
        /// Check Role if process can be performed
        /// </summary>
        /// <param name="VAF_Job_ID">process</param>
        public  void SetVAF_Job_ID(int AD_CrytalProcess_ID)
        {
            if (AD_CrytalProcess_ID <= 0)
                return;
            Console.WriteLine(AD_CrytalProcess_ID.ToString());
            int VAF_Role_ID = Utility.Env.GetContext().GetVAF_Role_ID();
            if (VAF_Role_ID != 0)
            {
                MRole role = MRole.Get(GetCtx(), VAF_Role_ID);
                //bool? access = role.GetProcessAccess(AD_CrytalProcess_ID);
                //if (access == null)
                //    throw new Exception("Cannot access Process " + AD_CrytalProcess_ID
                //        + " with Role: " + role.Get_Value("Name"));
            }
            base.SetVAF_CrystalInstance_ID(AD_CrytalProcess_ID);
        }

        /// <summary>
        /// Set Record ID.
        /// direct internal record ID
        /// </summary>
        /// <param name="Record_ID">record</param>
        public void SetRecord_ID(int Record_ID)
        {
            if (Record_ID < 0)
            {
                Record_ID = 0;
            }
            Set_ValueNoCheck("Record_ID", (int)Record_ID);
        }
    }
}
