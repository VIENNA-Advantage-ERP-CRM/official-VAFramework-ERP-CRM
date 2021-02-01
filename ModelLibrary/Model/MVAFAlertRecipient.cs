using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MVAFAlertRecipient : X_VAF_AlertRecipient
    {
        public MVAFAlertRecipient(Ctx ctx, int VAF_AlertRecipient_ID, Trx trx)
            : base(ctx, VAF_AlertRecipient_ID, trx)
        {
            
        }	//	MAlertRecipient

        public MVAFAlertRecipient(Ctx ctx, DataRow rs, Trx trx)
            : base(ctx, rs, trx)
        {
            
        }	//	MAlertRecipient


        public new int GetVAF_UserContact_ID()
        {
            int ii = Util.GetValueOfInt(Get_Value("VAF_UserContact_ID"));
            //if (ii == null)
            //    return -1;
            return ii;
        }	//	getVAF_UserContact_ID

        public new int GetVAF_Role_ID()
        {
            int ii = Util.GetValueOfInt(Get_Value("VAF_Role_ID"));
            //if (ii == null)
            //    return -1;
            return ii;
        }	//	getVAF_Role_ID

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("MAlertRecipient[");
            sb.Append(Get_ID())
                .Append(",VAF_UserContact_ID=").Append(GetVAF_UserContact_ID())
                .Append(",VAF_Role_ID=").Append(GetVAF_Role_ID())
                .Append("]");
            return sb.ToString();
            
        }
    }
}
