/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MWindowAccess
 * Purpose        : 
 * Class Used     : MWindowAccess inherits X_VAF_Screen_Rights
 * Chronological    Development
 * Raghunandan      05-May-2009 
  ******************************************************/
using System;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Common;
using System.Data;
using VAdvantage.Utility;
using VAdvantage.DataBase;
namespace VAdvantage.Model
{
 public class MWindowAccess : X_VAF_Screen_Rights
    {
        /// <summary>
        ///Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ignored">ignored -</param>
        /// <param name="trxName">transaction</param>
        public MWindowAccess(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, 0, trxName)
        {
            if (ignored != 0)
                throw new ArgumentException("Multi-Key");
            else
            {
                //	setVAF_Role_ID (0);
                //	setVAF_Screen_ID (0);
                SetIsReadWrite(true);
            }
        }

        /// <summary>
        ///Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="VAF_Role_ID">role id</param>
        public MWindowAccess(MWindow parent, int VAF_Role_ID)
            : base(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetVAF_Screen_ID(parent.GetVAF_Screen_ID());
            SetVAF_Role_ID(VAF_Role_ID);
        }

        /// <summary>
        /// MWindowAccess
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">transaction</param>
        public MWindowAccess(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

        /// <summary>
        /// String Info
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MWindowAccess[");
            sb.Append("VAF_Screen_ID=").Append(GetVAF_Screen_ID())
                .Append(",VAF_Role_ID=").Append(GetVAF_Role_ID())
                .Append("]");
            return sb.ToString();
        }
    }
}
