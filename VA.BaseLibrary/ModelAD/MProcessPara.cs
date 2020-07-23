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


using VAdvantage.ProcessEngine;
namespace VAdvantage.Model
{
   public class MProcessPara : X_AD_Process_Para
    {
        //private static final long serialVersionUID = 1L;

        private static CCache<int, MProcessPara> s_cache = new CCache<int, MProcessPara>("AD_Process_Para", 20);

        public static MProcessPara Get(Ctx ctx, int AD_Process_Para_ID)
        {
            int key = AD_Process_Para_ID;
            MProcessPara retValue = (MProcessPara)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MProcessPara(ctx, AD_Process_Para_ID, null);
            if (retValue.Get_ID() != 0)
                s_cache.Add(key, retValue);
            return retValue;
        }	//	get

        public MProcessPara(Ctx ctx, int AD_Process_Para_ID, Trx trxName)
            : base(ctx, AD_Process_Para_ID, trxName)
        {
            if (AD_Process_Para_ID == 0)
            {
                SetFieldLength(0);
                SetSeqNo(0);
                //SetAD_Reference_ID(0);
                SetIsCentrallyMaintained(true);
                SetIsRange(false);
                SetIsMandatory(false);
                SetEntityType(ENTITYTYPE_UserMaintained);
            }
        }	//	MProcessPara


        public MProcessPara(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }	//	MProcessPara


    }
}
