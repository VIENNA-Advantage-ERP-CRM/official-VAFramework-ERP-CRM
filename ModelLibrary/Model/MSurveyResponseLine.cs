using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
   public class MSurveyResponseLine:X_AD_SurveyResponseLine
    {
        public MSurveyResponseLine(Ctx ctx, int AD_SurveyResponseLine_ID, Trx trxName)
                : base(ctx, AD_SurveyResponseLine_ID, trxName)
        {
        }
    }
}
