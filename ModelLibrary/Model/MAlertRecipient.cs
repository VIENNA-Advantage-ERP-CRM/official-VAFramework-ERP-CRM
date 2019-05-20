using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MAlertRecipient : X_AD_AlertRecipient
    {
        public MAlertRecipient(Ctx ctx, int AD_AlertRecipient_ID, Trx trx)
            : base(ctx, AD_AlertRecipient_ID, trx)
        {
            
        }	//	MAlertRecipient

        public MAlertRecipient(Ctx ctx, DataRow rs, Trx trx)
            : base(ctx, rs, trx)
        {
            
        }	//	MAlertRecipient


        public new int GetAD_User_ID()
        {
            int ii = Util.GetValueOfInt(Get_Value("AD_User_ID"));
            //if (ii == null)
            //    return -1;
            return ii;
        }	//	getAD_User_ID

        public new int GetAD_Role_ID()
        {
            int ii = Util.GetValueOfInt(Get_Value("AD_Role_ID"));
            //if (ii == null)
            //    return -1;
            return ii;
        }	//	getAD_Role_ID

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("MAlertRecipient[");
            sb.Append(Get_ID())
                .Append(",AD_User_ID=").Append(GetAD_User_ID())
                .Append(",AD_Role_ID=").Append(GetAD_Role_ID())
                .Append("]");
            return sb.ToString();
            
        }
    }
}
