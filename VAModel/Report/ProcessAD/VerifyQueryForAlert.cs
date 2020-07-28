using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ProcessEngine;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.Model;
using VAdvantage.DataBase;

namespace VAdvantage.Process
{
    public class VerifyQueryForAlert : ProcessEngine.SvrProcess
    {
        protected override string DoIt()
        {
            string msg = "";
            X_AD_AlertRuleCondition AlertCondition = new X_AD_AlertRuleCondition(GetCtx(), GetRecord_ID(), null);
            try
            {
                if (AlertCondition.GetSqlQuery().ToLower().Trim().StartsWith("select"))
                {
                    if (AlertCondition.GetReturnValueType() == X_AD_AlertRuleCondition.RETURNVALUETYPE_Number)
                    {

                        Convert.ToDecimal(DB.ExecuteScalar(AlertCondition.GetSqlQuery()));
                        msg = "Success";
                    }
                    else if (AlertCondition.GetReturnValueType() == X_AD_AlertRuleCondition.RETURNVALUETYPE_String)
                    {
                        Convert.ToString(DB.ExecuteScalar(AlertCondition.GetSqlQuery()));
                        msg = "Success";
                    }
                    else
                    {
                        Convert.ToDateTime(DB.ExecuteScalar(AlertCondition.GetSqlQuery()));
                        msg = "Success";
                    }
                }
                else
                {
                    msg = "Error : Only Execute Select Query";
                }
            }
            catch (Exception e)
            {
                msg = "Error :" + e.Message;
            }
            return msg;
        }
        protected override void Prepare()
        {
            
        }

    }
}
