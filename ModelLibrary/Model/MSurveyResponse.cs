using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
  public class MSurveyResponse:X_AD_SurveyResponse
    {
        public MSurveyResponse(Ctx ctx, int AD_SurveyResponse_ID, Trx trxName)
            : base(ctx, AD_SurveyResponse_ID, trxName)
        {
        }
    }
    
}
