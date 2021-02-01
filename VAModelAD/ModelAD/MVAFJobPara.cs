using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Model;
using VAdvantage.Utility;


namespace VAdvantage.Model
{
   public class MVAFJobPara : X_VAF_Job_Para
    {
        //private static final long serialVersionUID = 1L;

        private static CCache<int, MVAFJobPara> s_cache = new CCache<int, MVAFJobPara>("VAF_Job_Para", 20);

        public static MVAFJobPara Get(Ctx ctx, int VAF_Job_Para_ID)
        {
            int key = VAF_Job_Para_ID;
            MVAFJobPara retValue = (MVAFJobPara)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MVAFJobPara(ctx, VAF_Job_Para_ID, null);
            if (retValue.Get_ID() != 0)
                s_cache.Add(key, retValue);
            return retValue;
        }	//	get

        public MVAFJobPara(Ctx ctx, int VAF_Job_Para_ID, Trx trxName)
            : base(ctx, VAF_Job_Para_ID, trxName)
        {
            if (VAF_Job_Para_ID == 0)
            {
                SetFieldLength(0);
                SetSeqNo(0);
                //SetVAF_Control_Ref_ID(0);
                SetIsCentrallyMaintained(true);
                SetIsRange(false);
                SetIsMandatory(false);
                SetEntityType(ENTITYTYPE_UserMaintained);
            }
        }	//	MProcessPara


        public MVAFJobPara(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }	//	MProcessPara


    }
}
