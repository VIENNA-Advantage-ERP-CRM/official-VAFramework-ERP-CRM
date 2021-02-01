using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MVAFModuleDBSchema:X_VAF_Module_DB_Schema
    {
       

        public MVAFModuleDBSchema(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        public MVAFModuleDBSchema(Ctx ctx, int id, Trx trxName)
            : base(ctx, id, trxName)
        {
       
        }
    }
}
