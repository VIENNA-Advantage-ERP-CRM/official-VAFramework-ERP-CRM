using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MAlert : X_AD_Alert
    {
        public MAlert(Ctx ctx, int AD_Alert_ID, Trx trx)
            : base(ctx, AD_Alert_ID, trx)
        {
            if (AD_Alert_ID == 0)
            {
                //	setAD_AlertProcessor_ID (0);
                //	setName (null);
                //	setAlertMessage (null);
                //	setAlertSubject (null);
                SetEnforceClientSecurity(true);	// Y
                SetEnforceRoleSecurity(true);	// Y
                SetIsValid(true);	// Y
            }
        }	//	MAlert

        public MAlert(Ctx ctx, DataRow rs, Trx trx)
            : base(ctx, rs, trx)
        {
        }	//	MAlert

        /**	The Rules						*/
        private MAlertRule[] m_rules = null;
        /**	The Recipients					*/
        private MAlertRecipient[] m_recipients = null;


        public MAlertRule[] GetRules(bool reload)
        {
            if (m_rules != null && !reload)
                return m_rules;
            String sql = "SELECT * FROM AD_AlertRule "
                + "WHERE isactive='Y' AND AD_Alert_ID=" + GetAD_Alert_ID();
            List<MAlertRule> list = new List<MAlertRule>();
            
            try
            {
                DataSet ds = DB.ExecuteDataset(sql);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    MAlertRule mAlertRule = new MAlertRule(GetCtx(), dr, null);
                    ValidateAlertRuleCondition(mAlertRule);
                    list.Add(mAlertRule);
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            //
            m_rules = new MAlertRule[list.Count()];
            m_rules = list.ToArray();
            return m_rules;
        }	//	getRules


        public MAlertRecipient[] GetRecipients(bool reload)
        {
            if (m_recipients != null && !reload)
                return m_recipients;
            String sql = "SELECT * FROM AD_AlertRecipient "
                + "WHERE AD_Alert_ID=" + GetAD_Alert_ID();
            List<MAlertRecipient> list = new List<MAlertRecipient>();
            try
            {
                DataSet ds = DB.ExecuteDataset(sql);
                foreach (DataRow dr in ds.Tables[0].Rows)
                    list.Add(new MAlertRecipient(GetCtx(), dr, null));
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            //
            m_recipients = new MAlertRecipient[list.Count()];
            m_recipients = list.ToArray();
            return m_recipients;
        }	//	getRecipients


        public int GetFirstAD_Role_ID()
        {
            GetRecipients(false);
            foreach (MAlertRecipient element in m_recipients)
            {
                if (element.GetAD_Role_ID() != -1)
                    return element.GetAD_Role_ID();
            }
            return -1;
        }	//	getForstAD_Role_ID


        public int GetFirstUserAD_Role_ID()
        {
            GetRecipients(false);
            int AD_User_ID = GetFirstAD_User_ID();
            if (AD_User_ID != -1)
            {
                MUserRoles[] urs = MUserRoles.GetOfUser(GetCtx(), AD_User_ID);
                foreach (MUserRoles element in urs)
                {
                    if (element.IsActive())
                        return element.GetAD_Role_ID();
                }
            }
            return -1;
        }	//	getFirstUserAD_Role_ID


        public int GetFirstAD_User_ID()
        {
            GetRecipients(false);
            foreach (MAlertRecipient element in m_recipients)
            {
                if (element.GetAD_User_ID() != -1)
                    return element.GetAD_User_ID();
            }
            return -1;
        }	//	getFirstAD_User_ID


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("MAlert[");
            sb.Append(Get_ID())
                .Append("-").Append(GetName())
                .Append(",Valid=").Append(IsValid());
            if (m_rules != null)
                sb.Append(",Rules=").Append(m_rules.Length);
            if (m_recipients != null)
                sb.Append(",Recipients=").Append(m_recipients.Length);
            sb.Append("]");
            return sb.ToString();
        }
        /**
        * 	Alert Condition 
        *	@param alert alert
        *	@return true if processed
        */
        public bool ValidateAlertRuleCondition(MAlertRule AlertRule)
        {
            bool returnConditionValue = true;
            int errorType = 0;
            // VIS0008
            // Change to check table in database, bug fixed in case of PostgreSQL
            //string Sql = "SELECT object_name FROM all_objects WHERE object_type IN ('TABLE','VIEW') AND (object_name)  = UPPER('AD_ALERTRULECONDITION') AND OWNER LIKE '" + DB.GetSchema() + "'";
            //string ObjectName = Convert.ToString(DB.ExecuteScalar(Sql));
            if (DBFunctionCollection.IsTableExists(DB.GetSchema(), "AD_AlertRuleCondition"))
            {
                //Fetch All Alert Condition Against AlertID.............
                DataSet dsAlertCondition = DB.ExecuteDataset("SELECT AD_AlertRuleCondition_ID FROM AD_AlertRuleCondition WHERE AD_AlertRule_ID=" + AlertRule.GetAD_AlertRule_ID() + " AND IsActive='Y' ORDER BY Sequence,AD_AlertRuleCondition_ID");
                //IF No Alert Condition Find then return true otherwise Follow further Condition............
                if (dsAlertCondition != null && dsAlertCondition.Tables[0].Rows.Count > 0)
                {

                    for (int i = 0; i < dsAlertCondition.Tables[0].Rows.Count; i++)
                    {
                        decimal numericValue = 0;
                        string stringValue = "";
                        DateTime dateValue = new DateTime();
                        bool validateResult = false;
                        errorType = 0;
                        int alertConditionID = Convert.ToInt32(dsAlertCondition.Tables[0].Rows[i]["ad_alertRulecondition_id"]);

                        X_AD_AlertRuleCondition alertCondition = new X_AD_AlertRuleCondition(AlertRule.GetCtx(), alertConditionID, null);
                        string sqlQuery = alertCondition.GetSqlQuery();

                        try
                        {
                            if (alertCondition.GetSqlQuery().ToLower().Trim().StartsWith("select"))
                            {
                                //Check What is the return type of Query. if Query retrun more than one record than throw error............
                                if (alertCondition.GetReturnValueType() == X_AD_AlertRuleCondition.RETURNVALUETYPE_Number) //Numeric Value
                                {
                                    errorType = 1;//if error occured in following query than used in catch 
                                    if (DB.ExecuteScalar(sqlQuery) == DBNull.Value || DB.ExecuteScalar(sqlQuery) == null)
                                    {
                                        numericValue =Convert.ToDecimal(0);
                                    }
                                    else
                                    {
                                        numericValue = Convert.ToDecimal(DB.ExecuteScalar(sqlQuery));
                                    }
                                    errorType = 2;//if error occured in following comparison then used in catch
                                    //This function Match condition on Query Return Value and User's enterd Value based on users Selected Operator...........
                                    validateResult = EvaluateNumaricLogic(numericValue, Convert.ToDecimal(alertCondition.GetAlphaNumValue()), alertCondition.GetOperator());
                                }
                                else if (alertCondition.GetReturnValueType() == X_AD_AlertRuleCondition.RETURNVALUETYPE_String)//String Value
                                {
                                    errorType = 1;//if error occured in following query than used in catch 
                                    if (DB.ExecuteScalar(sqlQuery) == DBNull.Value || DB.ExecuteScalar(sqlQuery) == null)
                                    {
                                        stringValue = "";
                                    }
                                    else
                                    {
                                        stringValue = Convert.ToString(DB.ExecuteScalar(sqlQuery));
                                    }
                                    errorType = 2;//if error occured in following comparison then used in catch
                                    //This function Match condition on Query Return Value and User's enterd Value based on users Selected Operator...........
                                    validateResult = EvaluateStringLogic(stringValue, alertCondition.GetAlphaNumValue(), alertCondition.GetOperator());
                                }
                                else if (alertCondition.GetReturnValueType() == X_AD_AlertRuleCondition.RETURNVALUETYPE_Date)// Date Value
                                {
                                    // this Date Section is not implemented in Alert Return Value Type List.......................

                                    errorType = 1;//if error occured in following query than used in catch 
                                    dateValue = Convert.ToDateTime(DB.ExecuteScalar(sqlQuery));
                                    errorType = 2;//if error occured in following comparison then used in catch
                                    //This function Match condition on Query Return Value and User's enterd Value based on users Selected Operator...........
                                    validateResult = EvaluateDateLogic(dateValue, Convert.ToDateTime(alertCondition.GetDateValue()), alertCondition.GetOperator(), alertCondition.IsDynamic(), alertCondition.GetDateOperation(), alertCondition.GetDay(), alertCondition.GetMONTH(), alertCondition.GetYEAR());
                                }
                                //if we Find multiple condition against same alert then we have to find on the basis of And OR
                                if (i != 0)
                                {
                                    if (X_AD_AlertRuleCondition.ANDOR_Or.Equals(alertCondition.GetAndOr()))
                                    {
                                        returnConditionValue = returnConditionValue || validateResult;
                                    }
                                    else
                                    {
                                        returnConditionValue = returnConditionValue && validateResult;
                                    }
                                }
                                else
                                {
                                    returnConditionValue = validateResult;
                                }
                            }
                            else
                            {
                                returnConditionValue = false;
                                AlertRule.SetErrorMsg("Conditional Sequence Number " +alertCondition.GetSequence()+ " Error= Only Execute Select Query");
                                AlertRule.SetIsValid(false);
                                AlertRule.Save();
                                return false;
                            }
                        }
                        catch (Exception e)
                        {
                            returnConditionValue = false;
                            if (errorType == 1)
                            {
                                AlertRule.SetErrorMsg("Conditional Sequence Number " + alertCondition.GetSequence() + " Select Error=" + e.Message);
                               
                            }
                            else
                            {
                                AlertRule.SetErrorMsg("Conditional Sequence Number " + alertCondition.GetSequence() + " Comparison Error=" + e.Message);
                                
                            }
                            AlertRule.SetIsValid(false);
                            AlertRule.Save();
                            return false;
                        }
                    }
                }
                else
                {
                    returnConditionValue = true;
                }
            }
            else
            {
                returnConditionValue = true;
            }
            if (AlertRule.GetErrorMsg() == null || AlertRule.GetErrorMsg() == string.Empty)
            {
                AlertRule.SetIsValid(returnConditionValue);
                AlertRule.Save();
            }
            return returnConditionValue;
        }
        private bool EvaluateNumaricLogic(decimal numericValue, decimal compareValue, string operation)
        {
            if (X_AD_AlertRuleCondition.OPERATOR_Eq.Equals(operation))
                return numericValue.CompareTo(compareValue) == 0;
            else if (X_AD_AlertRuleCondition.OPERATOR_Gt.Equals(operation))
                return numericValue.CompareTo(compareValue) > 0;
            else if (X_AD_AlertRuleCondition.OPERATOR_GtEq.Equals(operation))
                return numericValue.CompareTo(compareValue) >= 0;
            else if (X_AD_AlertRuleCondition.OPERATOR_Le.Equals(operation))
                return numericValue.CompareTo(compareValue) < 0;
            else if (X_AD_AlertRuleCondition.OPERATOR_LeEq.Equals(operation))
                return numericValue.CompareTo(compareValue) <= 0;
            else if (X_AD_AlertRuleCondition.OPERATOR_NotEq.Equals(operation))
                return numericValue.CompareTo(compareValue) != 0;
            else if (X_AD_AlertRuleCondition.OPERATOR_Like.Equals(operation))
                return numericValue.CompareTo(compareValue) == 0;
            else
                return false;
        }

        private bool EvaluateStringLogic(string Value, string compareValue, string operation)
        {
            if (X_AD_AlertRuleCondition.OPERATOR_Eq.Equals(operation))
            {
                return Value.Equals(compareValue, StringComparison.OrdinalIgnoreCase);
            }
            //else if (X_AD_AlertCondition.OPERATOR_Like.Equals(operation))
            //{

            //    return Value.ToLower().Contains(compareValue.ToLower());
            //}
            else if (X_AD_AlertRuleCondition.OPERATOR_NotEq.Equals(operation))
            {
                return Value.ToLower() != compareValue.ToLower();
            }
            else
            {
                return false;
            }
        }
        // This Date Comparison function not properly implemented for Alert...........
        #region
        private bool EvaluateDateLogic(DateTime value, DateTime compareValue, string operation, bool isDynamic, string dynamicValue, int day, int month, int year)
        {
            value = value.Date;
            compareValue = compareValue.Date;
            if (X_AD_AlertRuleCondition.OPERATOR_Eq.Equals(operation))
            {
                if (isDynamic == true)
                {
                    compareValue = DynamicDateLogic(dynamicValue, day, month, year);
                }
                return value.CompareTo(compareValue) == 0;
            }
            else if (X_AD_AlertRuleCondition.OPERATOR_Gt.Equals(operation))
            {
                if (isDynamic == true)
                {
                    compareValue = DynamicDateLogic(dynamicValue, day, month, year);
                }
                return value.CompareTo(compareValue) > 0;
            }
            else if (X_AD_AlertRuleCondition.OPERATOR_GtEq.Equals(operation))
            {
                if (isDynamic == true)
                {
                    compareValue = DynamicDateLogic(dynamicValue, day, month, year);
                }
                return value.CompareTo(compareValue) >= 0;
            }
            else if (X_AD_AlertRuleCondition.OPERATOR_Le.Equals(operation))
            {
                if (isDynamic == true)
                {
                    compareValue = DynamicDateLogic(dynamicValue, day, month, year);
                }
                return value.CompareTo(compareValue) < 0;
            }
            else if (X_AD_AlertRuleCondition.OPERATOR_LeEq.Equals(operation))
            {
                if (isDynamic == true)
                {
                    compareValue = DynamicDateLogic(dynamicValue, day, month, year);
                }
                return value.CompareTo(compareValue) <= 0;
            }
            else if (X_AD_AlertRuleCondition.OPERATOR_NotEq.Equals(operation))
            {
                if (isDynamic == true)
                {
                    compareValue = DynamicDateLogic(dynamicValue, day, month, year);
                }
                return value.CompareTo(compareValue) != 0;
            }
            else
                return false;
        }
        private DateTime DynamicDateLogic(string DynamicValue, int day, int month, int year)
        {

            if (X_AD_AlertRuleCondition.DATEOPERATION_Today.Equals(DynamicValue))
            {
                return System.DateTime.Now.Date;
            }
            else if (X_AD_AlertRuleCondition.DATEOPERATION_Now.Equals(DynamicValue))
            {
                return System.DateTime.Now;
            }
            else if (X_AD_AlertRuleCondition.DATEOPERATION_LastxDays.Equals(DynamicValue))
            {
                return System.DateTime.Now.Date.AddDays(-day);
            }
            else if (X_AD_AlertRuleCondition.DATEOPERATION_LastxMonth.Equals(DynamicValue))
            {
                int tempDay = (month * 31) + day;
                return System.DateTime.Now.Date.AddDays(-tempDay);
            }
            else if (X_AD_AlertRuleCondition.DATEOPERATION_LastxYear.Equals(DynamicValue))
            {
                int tempDay = (year * 365) + (month * 31) + day;
                return System.DateTime.Now.Date.AddDays(-tempDay);
            }
            else
            {
                return System.DateTime.Now.Date;
            }

        }
        #endregion
    }
}
