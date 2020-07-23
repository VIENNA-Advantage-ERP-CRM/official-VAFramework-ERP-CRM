using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MModuleDBSchema:X_AD_Module_DB_Schema
    {
       

        public MModuleDBSchema(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        public MModuleDBSchema(Ctx ctx, int id, Trx trxName)
            : base(ctx, id, trxName)
        {
       
        }
    }
}
