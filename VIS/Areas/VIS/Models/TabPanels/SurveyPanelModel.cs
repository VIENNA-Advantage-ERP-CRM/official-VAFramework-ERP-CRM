using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class SurveyPanelModel
    {        
        public List<SurveyAssignmentsDetails> GetSurveyAssignments(Ctx ctx, int AD_Window_ID, int AD_Tab_ID, int AD_Table_ID,int AD_Record_ID)
        {
            SurveyAssignmentsDetails lst = new SurveyAssignmentsDetails();
            List<SurveyAssignmentsDetails> LsDetails = new List<SurveyAssignmentsDetails>();
            StringBuilder sql = new StringBuilder(@"SELECT sa.AD_Window_ID, sa.AD_Survey_ID, sa.C_DocType_ID, sa.SurveyListFor,
                                                  sa.DocAction, sa.ShowAllQuestions, sa.AD_SurveyAssignment_ID, s.surveytype,sa.AD_ShowEverytime,
                                                  s.ismandatory, s.name,sa.QuestionsPerPage FROM ad_surveyassignment sa INNER JOIN AD_Survey s ON 
                                                  s.ad_survey_ID= sa.ad_survey_id WHERE sa.IsActive='Y' AND sa.ad_tab_id=" + AD_Tab_ID + " AND sa.ad_window_id= " + AD_Window_ID );
            DataSet _dsDetails = DB.ExecuteDataset(MRole.GetDefault(ctx).AddAccessSQL(sql.ToString(), "sa", true, false), null);
            if (_dsDetails != null && _dsDetails.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dt in _dsDetails.Tables[0].Rows)
                {
                    if (Util.GetValueOfString(dt["AD_ShowEverytime"])=="N")
                    {
                        bool isvalidate = checkConditions(ctx, AD_Window_ID, AD_Tab_ID, AD_Table_ID, AD_Record_ID);
                        if (!isvalidate)
                        {
                            break;
                        }
                    }
                  
                    LsDetails.Add(new SurveyAssignmentsDetails
                    {
                        Window_ID = Util.GetValueOfInt(dt["AD_Window_ID"]),
                        Survey_ID = Util.GetValueOfInt(dt["AD_Survey_ID"]),
                        DocType_ID = Util.GetValueOfInt(dt["C_DocType_ID"]),
                        SurveyListFor = Util.GetValueOfString(dt["SurveyListFor"]),
                        DocAction = Util.GetValueOfString(dt["DocAction"]),
                        ShowAllQuestion = Util.GetValueOfBool(Util.GetValueOfString(dt["ShowAllQuestions"]).Equals("Y")),
                        SurveyAssignment_ID = Util.GetValueOfInt(dt["AD_SurveyAssignment_ID"]),
                        SurveyType = Util.GetValueOfString(dt["surveytype"]),
                        IsMandatory = Util.GetValueOfBool(Util.GetValueOfString(dt["ismandatory"]).Equals("Y")),
                        SurveyName = Util.GetValueOfString(dt["name"]),
                        QuestionsPerPage = Util.GetValueOfInt(dt["QuestionsPerPage"]),
                        IsDocActionActive = checkDocActionColumn(AD_Tab_ID),
                        ShowEverytime= Util.GetValueOfBool(Util.GetValueOfString(dt["AD_ShowEverytime"]).Equals("Y"))
                    }); 
                }
            }
            return LsDetails;
        }
        /// <summary>
        /// check any condition apply for show survey List
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Window_ID"></param>
        /// <param name="AD_Tab_ID"></param>
        /// <param name="AD_Table_ID"></param>
        /// <param name="AD_Record_ID"></param>
        /// <returns></returns>
        public bool checkConditions(Ctx ctx, int AD_Window_ID, int AD_Tab_ID, int AD_Table_ID, int AD_Record_ID)
        {
            bool isExist = true;
            string sql = @"SELECT AD_Field.AD_column_ID,
                            ad_surveyshowcondition.seqno,AD_Column.ColumnName,ad_surveyshowcondition.operation,ad_surveyshowcondition.ad_equalto,ad_surveyshowcondition.Value2,
                            ad_surveyshowcondition.andor,AD_Column.AD_Reference_ID
                            FROM AD_Field INNER JOIN AD_Column
                            ON AD_Field.AD_Column_ID=AD_Column.AD_Column_ID
                            INNER JOIN ad_surveyshowcondition ON AD_Field.AD_column_ID=ad_surveyshowcondition.AD_column_ID
                            WHERE AD_Field.AD_Tab_ID=" + AD_Tab_ID + " AND AD_Column.AD_Table_ID=" + AD_Table_ID + @"
                            ORDER BY ad_surveyshowcondition.seqno";
            DataSet _dsDetails = DB.ExecuteDataset(MRole.GetDefault(ctx).AddAccessSQL(sql, "ad_surveyshowcondition", true, false), null);

            //prepare where condition for filter
            if (_dsDetails != null && _dsDetails.Tables[0].Rows.Count > 0)
            {
                string WhereCondition = "";

                foreach (DataRow dt in _dsDetails.Tables[0].Rows)
                {
                    string type = "";
                    string value = Util.GetValueOfString(dt["ad_equalto"]);
                    string columnName = Util.GetValueOfString(dt["ColumnName"]);
                    int displayType = Util.GetValueOfInt(dt["AD_Reference_ID"]);

                    //Checking data type of column
                    if (columnName.Equals("AD_Language") || columnName.Equals("EntityType") || columnName.Equals("DocBaseType"))
                    {
                        type = typeof(System.String).Name;
                    }
                    else if (columnName.Equals("Posted") || columnName.Equals("Processed") || columnName.Equals("Processing"))
                    {
                        type = typeof(System.Boolean).Name;
                    }
                    else if (columnName.Equals("Record_ID"))
                    {

                        type = typeof(System.Int32).Name;
                    }
                    else
                    {
                        type = VAdvantage.Classes.DisplayType.GetClass(displayType, true).Name;
                    }

                    //Rearange operater
                    string oprtr = Util.GetValueOfString(dt["operation"]);
                    if (oprtr == "==")
                    {
                        oprtr = "=";
                    }
                    else if (oprtr == "<<")
                    {
                        oprtr = "<";
                    }
                    else if (oprtr == ">>")
                    {
                        oprtr = ">";
                    }
                    else if (oprtr == "~~")
                    {
                        oprtr = " BETWEEN ";
                        value = "%" + value + "%";
                    }
                    else if (oprtr == "AB")
                    {
                        oprtr = ">";
                    }

                    if (Util.GetValueOfInt(dt["seqno"]) == 10)
                    {
                        WhereCondition += columnName + " " + oprtr;
                    }
                    else
                    {
                        string andOR = " AND ";
                        if (Util.GetValueOfString(dt["operation"]) == "O")
                        {
                            andOR = " OR ";
                        }
                        WhereCondition += andOR;
                        WhereCondition += columnName + " " + oprtr;
                    }

                    if (type == "Int32" || type == "Decimal" || type == "Boolean")
                    {
                        WhereCondition += value;
                        if (Util.GetValueOfString(dt["operation"]) == "AB")
                        {
                            WhereCondition += " AND " + columnName + " <" + Util.GetValueOfString(dt["Value2"]);
                        }
                    }
                    else if (type == "String")
                    {
                        WhereCondition += "'" + value + "'";
                        if (Util.GetValueOfString(dt["operation"]) == "AB")
                        {
                            WhereCondition += " AND " + columnName + " <'" + Util.GetValueOfString(dt["Value2"]) + "'";
                        }
                    }
                    else if (type == "DateTime")
                    {
                        WhereCondition += "TO_DATE(" + value + ")";
                        if (Util.GetValueOfString(dt["operation"]) == "AB")
                        {
                            WhereCondition += " AND " + columnName + " < TO_DATE(" + Util.GetValueOfString(dt["Value2"]) + ")";
                        }
                    }
                }

                string tableName = Util.GetValueOfString(DB.ExecuteScalar("SELECT TableName FROM AD_Table WHERE AD_Table_ID=" + AD_Table_ID));

                sql = "SELECT COUNT(" + tableName + "_ID) FROM " + tableName + " WHERE " + tableName + "_ID=" + AD_Record_ID + " AND " + WhereCondition;
                int count = Util.GetValueOfInt(DB.ExecuteScalar(MRole.GetDefault(ctx).AddAccessSQL(sql, tableName, true, false)));
                if (count > 0)
                {
                    isExist = true;
                }
                else
                {
                    isExist = false;
                }
            }
            return isExist;


        }

        public bool checkDocActionColumn(int AD_Tab_ID)
        {
            bool IsDocActionExists = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(*) FROM AD_Field f 
            INNER JOIN AD_Column c ON f.AD_Column_ID = c.AD_Column_ID WHERE f.AD_Tab_ID = " + AD_Tab_ID + @" AND 
            UPPER(c.ColumnName)='DOCACTION' AND f.IsActive='Y'")) > 0;
            return IsDocActionExists;
        }

        /// <summary>
        /// Get Survey Items
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Survey_ID"></param>
        /// <returns></returns>
        public List<ListSurveyItemValues> GetSurveyItems(Ctx ctx, int AD_Survey_ID)
        {
            List<ListSurveyItemValues> ListItemValues = new List<ListSurveyItemValues>();
            ListSurveyItemValues SurveyItemandValue = null;
            SurveyItem item = null;
            StringBuilder sql = new StringBuilder(@"SELECT AD_SurveyItem_ID,AD_Survey_ID,Question,Line,
            AnswerType,IsMandatory,IsActive,AnswerSelection FROM AD_SurveyItem WHERE IsActive='Y' AND 
            AD_Survey_ID = " + AD_Survey_ID + "  ORDER BY Line ASC");
            DataSet _dsDetails = DB.ExecuteDataset(MRole.GetDefault(ctx).AddAccessSQL(sql.ToString(), "AD_SurveyItem", true, false), null);
            if (_dsDetails != null && _dsDetails.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dt in _dsDetails.Tables[0].Rows)
                {
                    SurveyItemandValue = new ListSurveyItemValues();
                    item = new SurveyItem();
                    item.AD_Survey_ID = Util.GetValueOfInt(dt["AD_Survey_ID"]);
                    item.AD_SurveyItem_ID = Util.GetValueOfInt(dt["AD_SurveyItem_ID"]);
                    item.Question = Util.GetValueOfString(dt["Question"]);
                    item.LineNo = Util.GetValueOfInt(dt["Line"]);
                    item.AnswerType = Util.GetValueOfString(dt["AnswerType"]);
                    item.IsMandatory = Util.GetValueOfString(dt["IsMandatory"]);
                    item.IsActive = Util.GetValueOfString(dt["IsActive"]);
                    item.AnswerSelection = Util.GetValueOfString(dt["AnswerSelection"]);
                    SurveyItemandValue.Item = item;
                    SurveyItemandValue.Values = GetSurveyItemValues(ctx, item.AD_SurveyItem_ID);
                    ListItemValues.Add(SurveyItemandValue);
                }
            }


            return ListItemValues;
        }

        /// <summary>
        /// Get survey item Values
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_SurveyItem_ID"></param>
        /// <returns></returns>
        public List<SurveyItemValue> GetSurveyItemValues(Ctx ctx, int AD_SurveyItem_ID)
        {
            SurveyItemValue lst = new SurveyItemValue();
            List<SurveyItemValue> LsDetails = new List<SurveyItemValue>();
            StringBuilder sql = new StringBuilder(@"SELECT AD_SurveyItem_ID,AD_SurveyValue_ID,Line,
                              Answer, IsActive FROM AD_SurveyValue WHERE IsActive='Y' AND 
            AD_SurveyItem_ID = " + AD_SurveyItem_ID + "  ORDER BY Line ASC");
            DataSet _dsDetails = DB.ExecuteDataset(MRole.GetDefault(ctx).AddAccessSQL(sql.ToString(), "AD_SurveyValue", true, false), null);
            if (_dsDetails != null && _dsDetails.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dt in _dsDetails.Tables[0].Rows)
                {
                    LsDetails.Add(new SurveyItemValue
                    {
                        AD_SurveyItem_ID = Util.GetValueOfInt(dt["AD_SurveyItem_ID"]),
                        AD_SurveyValue_ID = Util.GetValueOfInt(dt["AD_SurveyValue_ID"]),
                        LineNo = Util.GetValueOfInt(dt["Line"]),
                        Answer = Util.GetValueOfString(dt["Answer"]),
                        IsActive = Util.GetValueOfString(dt["IsActive"])
                    });
                }
            }
            return LsDetails;
        }
        /// <summary>
        /// Save Survey Response Value
        /// </summary>
        /// <param name="surveyResponseValue"></param>
        public int SaveSurveyResponse(Ctx ctx, List<SurveyResponseValue> surveyResponseValue, int AD_Window_ID, int AD_Survey_ID, int Record_ID, int AD_Table_ID)
        {
            MSurveyResponse SR = new MSurveyResponse(ctx, 0, null);
            SR.SetAD_Window_ID(AD_Window_ID);
            SR.SetAD_Survey_ID(AD_Survey_ID);
            SR.SetRecord_ID(Record_ID);
            SR.SetAD_Table_ID(AD_Table_ID);
            SR.SetAD_User_ID(ctx.GetAD_User_ID());
            if (SR.Save())
            {
                for (var i = 0; i < surveyResponseValue.Count; i++)
                {

                    MSurveyResponseLine SRL = new MSurveyResponseLine(ctx, 0, null);
                    SRL.SetAD_SurveyResponse_ID(SR.GetAD_SurveyResponse_ID());
                    SRL.SetAD_SurveyValue_ID(surveyResponseValue[i].AD_SurveyValue_ID);
                    SRL.SetQuestion(surveyResponseValue[i].Question);
                    SRL.SetAnswer(surveyResponseValue[i].Answer);
                    SRL.SetAD_SurveyItem_ID(surveyResponseValue[i].AD_SurveyItem_ID);
                    if (SRL.Save()) { 
                    
                    }
                }
            }

            return SR.GetAD_SurveyResponse_ID();
        }

        /// <summary>
        /// Check any record exist in checklist response for docAction
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Window_ID"></param>
        /// <param name="AD_Tab_ID"></param>
        /// <param name="Record_ID"></param>
        /// <param name="DocAction"></param>
        /// <returns></returns>
        public bool CheckDocActionCheckListResponse(Ctx ctx, int AD_Window_ID, int AD_Tab_ID, int Record_ID,string DocAction,int AD_Table_ID) {

            string sql = "SELECT AD_ShowEverytime FROM  ad_surveyassignment WHERE IsActive='Y' AND ad_tab_id=" + AD_Tab_ID + " AND ad_window_id= " + AD_Window_ID;

            string ShowEverytime = Util.GetValueOfString(DB.ExecuteScalar(sql));
            if (ShowEverytime=="N")
            {
                bool isvalidate = checkConditions(ctx, AD_Window_ID, AD_Tab_ID,  AD_Table_ID, Record_ID);
                if (!isvalidate) {
                    return true;
                }
            }

            sql = "SELECT AD_Survey_ID FROM AD_SurveyAssignment WHERE ad_window_id=" + AD_Window_ID + " AND AD_TAb_ID=" + AD_Tab_ID;
            if (DocAction != "RE") {
                sql += " AND docaction='" + DocAction + "'";
            }
            int AD_Survey_ID =Util.GetValueOfInt(DB.ExecuteScalar(sql));
            if (AD_Survey_ID > 0)
            {
                if (DocAction != "RE")
                {
                    sql = "SELECT count(AD_SurveyResponse_id) FROM AD_SurveyResponse WHERE ad_window_id=" + AD_Window_ID + " AND AD_Survey_ID=" + AD_Survey_ID + " AND record_ID=" + Record_ID + " AND IsActive='Y'";
                    int count = Util.GetValueOfInt(DB.ExecuteScalar(sql));
                    if (count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    sql = "UPDATE AD_SurveyResponse SET IsActive='N'  WHERE ad_window_id=" + AD_Window_ID + " AND AD_Survey_ID=" + AD_Survey_ID + " AND record_ID=" + Record_ID + " AND IsActive='Y'";
                    DB.ExecuteQuery(sql);
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        public bool CheckDocActionInTable(int AD_Table_ID)
        {
            string sql = "SELECT Count(AD_Column_ID) FROM AD_Column WHERE IsActive='Y' AND ad_table_id=" + AD_Table_ID + " AND AD_Reference_Value_ID= " + 135;
            int count = Util.GetValueOfInt(DB.ExecuteScalar(sql));
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
       
    }

    public class SurveyAssignmentsDetails
    {
        public int Window_ID { get; set; }
        public int Survey_ID { get; set; }
        public int DocType_ID { get; set; }
        public string SurveyListFor { get; set; }
        public string DocAction { get; set; }
        public bool ShowAllQuestion { get; set; }
        public int SurveyAssignment_ID { get; set; }
        public string SurveyType { get; set; }
        public bool IsMandatory { get; set; }
        public string SurveyName { get; set; }
        public int QuestionsPerPage { get; set; }
        public bool IsDocActionActive { get; set; }
        public bool ShowEverytime { get; set; }
    }

    public class ListSurveyItemValues
    {
        public SurveyItem Item { get; set; }
        public List<SurveyItemValue> Values { get; set; }
    }
    public class SurveyItem
    {
        public int AD_SurveyItem_ID { get; set; }
        public int AD_Survey_ID { get; set; }
        public string Question { get; set; }
        public int LineNo { get; set; }
        public string AnswerType { get; set; }
        public string IsMandatory { get; set; }
        public string IsActive { get; set; }
        public string AnswerSelection { get; set; }
    }
    public class SurveyItemValue
    {
        public int AD_SurveyItem_ID { get; set; }
        public int AD_SurveyValue_ID { get; set; }
        public string Answer { get; set; }
        public int LineNo { get; set; }
        public string IsActive { get; set; }
        public string AnswerSelection { get; set; }
    }
    public class SurveyResponseValue
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public int AD_Survey_ID { get; set; }
        public int AD_SurveyItem_ID { get; set; }
        public string AD_SurveyValue_ID { get; set; }
        
    }

}