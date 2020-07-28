/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MWindowAccess
 * Purpose        : 
 * Class Used     : MWindowAccess inherits X_AD_Window_Access
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
 public class MWindowAccess : X_AD_Window_Access
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
                //	setAD_Role_ID (0);
                //	setAD_Window_ID (0);
                SetIsReadWrite(true);
            }
        }

        /// <summary>
        ///Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="AD_Role_ID">role id</param>
        public MWindowAccess(MWindow parent, int AD_Role_ID)
            : base(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetAD_Window_ID(parent.GetAD_Window_ID());
            SetAD_Role_ID(AD_Role_ID);
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
            sb.Append("AD_Window_ID=").Append(GetAD_Window_ID())
                .Append(",AD_Role_ID=").Append(GetAD_Role_ID())
                .Append("]");
            return sb.ToString();
        }
    }
}
