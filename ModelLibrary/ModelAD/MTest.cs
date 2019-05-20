using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
   public  class MTest:X_Test
    {
       public MTest(Ctx ctx, DataRow dr, Trx trxName)
           : base(ctx, dr, trxName)
       {


       }
       public MTest(Ctx ctx, int Id, Trx trxName)
           : base(ctx, Id, trxName)
       {


       }

       protected override bool BeforeDelete()
       {
           return base.BeforeDelete();
       }

       protected override bool BeforeSave(bool newRecord)
       {
           //VAdvantage.Classes.//ShowMessage.Info("From Dll ",null,null,"");
           return base.BeforeSave(newRecord);
       }
    }
}
