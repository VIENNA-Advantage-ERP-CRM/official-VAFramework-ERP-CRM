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
        public List<SurveyAssignmentsDetails> GetSurveyAssignments(int AD_Window_ID, int AD_Tab_ID)
        {
            SurveyAssignmentsDetails lst = new SurveyAssignmentsDetails();
            List<SurveyAssignmentsDetails> LsDetails = new List<SurveyAssignmentsDetails>();
            StringBuilder sql = new StringBuilder(@"SELECT sa.AD_Window_ID, sa.AD_Survey_ID, sa.C_DocType_ID, sa.SurveyListFor,
                                                  sa.DocAction, sa.ShowAllQuestions, sa.AD_SurveyAssignment_ID, s.surveytype,
                                                  s.ismandatory, s.name FROM ad_surveyassignment sa INNER join AD_Survey s on 
                                                  s.ad_survey_ID= sa.ad_survey_id where sa.IsActive='Y' AND sa.ad_window_id= " + AD_Window_ID);
            DataSet _dsDetails = DB.ExecuteDataset(sql.ToString(), null);
            if (_dsDetails != null && _dsDetails.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dt in _dsDetails.Tables[0].Rows)
                {
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
                        IsDocActionActive = checkDocActionColumn(AD_Tab_ID)
                    }); ;
                }
            }
            return LsDetails;
        }

        public bool checkDocActionColumn(int AD_Tab_ID)
        {
            bool IsDocActionExists = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(*) FROM AD_Field f 
            INNER JOIN AD_Column c ON f.AD_Column_ID = c.AD_Column_ID WHERE f.AD_Tab_ID = " + AD_Tab_ID + @" AND 
            UPPER(c.ColumnName)='DOCACTION' and f.IsActive='Y'")) > 0;
            return IsDocActionExists;
        }

        public List<ListSurveyItemValues> GetSurveyItems(int AD_Survey_ID)
        {
            List<ListSurveyItemValues> ListItemValues = new List<ListSurveyItemValues>();
            ListSurveyItemValues SurveyItemandValue = null;
            SurveyItem item = null;
            StringBuilder sql = new StringBuilder(@"SELECT AD_SurveyItem_ID,AD_Survey_ID,Question,Line,
            AnswerType,IsMandatory,IsActive FROM AD_SurveyItem WHERE IsActive='Y' AND 
            AD_Survey_ID = " + AD_Survey_ID + "  ORDER BY Line ASC");
            DataSet _dsDetails = DB.ExecuteDataset(sql.ToString(), null);
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
                    SurveyItemandValue.Item = item;
                    SurveyItemandValue.Values = GetSurveyItemValues(item.AD_SurveyItem_ID);
                    ListItemValues.Add(SurveyItemandValue);
                }
            }


            return ListItemValues;
        }

        public List<SurveyItemValue> GetSurveyItemValues(int AD_SurveyItem_ID)
        {
            SurveyItemValue lst = new SurveyItemValue();
            List<SurveyItemValue> LsDetails = new List<SurveyItemValue>();
            StringBuilder sql = new StringBuilder(@"SELECT AD_SurveyItem_ID,AD_SurveyValue_ID,Line,
                              Answer, IsActive FROM AD_SurveyValue WHERE IsActive='Y' AND 
            AD_SurveyItem_ID = " + AD_SurveyItem_ID + "  ORDER BY Line ASC");
            DataSet _dsDetails = DB.ExecuteDataset(sql.ToString(), null);
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
        public bool IsDocActionActive { get; set; }
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
    }
    public class SurveyItemValue
    {
        public int AD_SurveyItem_ID { get; set; }
        public int AD_SurveyValue_ID { get; set; }
        public string Answer { get; set; }
        public int LineNo { get; set; }
        public string IsActive { get; set; }
    }

}