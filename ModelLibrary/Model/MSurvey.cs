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
    /// <summary>
    /// Survey Model
    /// VIS0228 Date-07-Sep-2022
    /// </summary>
   public class MSurvey: X_AD_Survey
    {
        /// <summary>
        /// Load constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trx"></param>
        public MSurvey(Ctx ctx, DataRow dr, Trx trx)
           : base(ctx, dr, trx)
        {

            // TODO Auto-generated constructor stub
        }
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="MSurvey_ID">MSurvey_ID</param>
        /// <param name="trx">trx</param>
        public MSurvey(Ctx ctx, int MSurvey_ID, Trx trx)
            : base(ctx, MSurvey_ID, trx)
        {
        }

        /// <summary>
        /// Before Save Logic
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns></returns>
        protected override bool BeforeSave(bool newRecord)
        {
            // checking answer exist if update Questionnaire  to Check list
            if (!newRecord && Is_ValueChanged("SurveyType") && GetSurveyType()== "CL")
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
