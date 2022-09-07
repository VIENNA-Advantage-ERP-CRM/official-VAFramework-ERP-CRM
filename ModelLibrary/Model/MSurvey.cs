using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using ViennaAdvantage.Model;

namespace VAdvantage.Model
{
   public class MSurvey: X_AD_Survey
    {
        public MSurvey(Ctx ctx, DataRow dr, Trx trx)
           : base(ctx, dr, trx)
        {

            // TODO Auto-generated constructor stub
        }
        public MSurvey(Ctx ctx, int MSurvey_ID, Trx trx)
            : base(ctx, MSurvey_ID, trx)
        {
        }
        protected override bool BeforeSave(bool newRecord)
        {
            if(!newRecord && Is_ValueChanged("SurveyType") && GetSurveyType()== "CL")
            {
                string sql = @"SELECT count(AD_SurveyValue.AD_SurveyValue_ID) FROM AD_SurveyItem 
                                INNER JOIN AD_SurveyValue ON AD_SurveyValue.AD_SurveyItem_ID=AD_SurveyItem.AD_SurveyItem_ID
                                WHERE ad_surveyitem.AD_Survey_ID="+GetAD_Survey_ID();
                int count =Convert.ToInt32(DB.ExecuteScalar(sql));
                if (count > 0)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "FailedToUpdateSurveyType"));
                    return false;
                }

            }
            return true;
        }
    }
}
