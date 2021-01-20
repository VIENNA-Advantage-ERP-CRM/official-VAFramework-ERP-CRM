using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using VAdvantage.Controller;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VAdvantage.ProcessEngine
{
    public class ProcessParameter
    {

        public static List<GridField> GetParametersList(Ctx ctx,int VAF_Job_ID,int windowNo)
        {
            List<GridField> fields = new List<GridField>();

            string strSql = "";
            SqlParameter[] param;
            if (Utility.Env.IsBaseLanguage(ctx, ""))//    GlobalVariable.IsBaseLanguage())
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@processid", VAF_Job_ID);
                //strSql = "SELECT p.Name, p.Description, p.Help, "
                //    + "p.VAF_Control_Ref_ID, p.VAF_Job_Para_ID, "
                //    + "p.FieldLength, p.IsMandatory IsMandatoryUI, p.IsRange, p.ColumnName, "
                //    + "p.DefaultValue, p.DefaultValue2, p.VFormat, p.ValueMin, p.ValueMax, "
                //    + "p.SeqNo, p.VAF_Control_Ref_Value_ID, vr.Code AS ValidationCode "
                //    + "FROM VAF_Job_Para p"
                //    + " LEFT OUTER JOIN VAF_DataVal_Rule vr ON (p.VAF_DataVal_Rule_ID=vr.VAF_DataVal_Rule_ID) "
                //    + "WHERE p.VAF_Job_ID=@processid"		//	1
                //    + " AND p.IsActive='Y' "
                //    + "ORDER BY SeqNo";

                strSql = @"SELECT p.Name,
                                  p.Description,
                                  p.Help,
                                  p.VAF_Control_Ref_ID,
                                  p.VAF_Job_Para_ID,
                                  p.FieldLength,
                                  p.IsMandatory IsMandatoryUI,
                                  p.IsRange,
                                  p.ColumnName,
                                  p.DefaultValue,
                                  p.DefaultValue2,
                                  p.VFormat,
                                  p.ValueMin,
                                  p.ValueMax,
                                  p.SeqNo,
                                  p.VAF_Control_Ref_Value_ID,
                                  vr.Code AS ValidationCode,
                                  p.VAF_QuickSearchWindow_ID,
                                  p.LoadRecursiveData,
p.ShowChildOfSelected,
p.IsEncrypted
                                FROM VAF_Job_Para p
                                LEFT OUTER JOIN VAF_DataVal_Rule vr
                                ON (p.VAF_DataVal_Rule_ID =vr.VAF_DataVal_Rule_ID)
                                WHERE p.VAF_Job_ID=@processid
                                AND p.IsActive       ='Y'
                                ORDER BY SeqNo";

            }
            else
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@processid", VAF_Job_ID);
                //strSql = "SELECT t.Name, t.Description, t.Help, "
                //    + "p.VAF_Control_Ref_ID, p.VAF_Job_Para_ID, "
                //    + "p.FieldLength, p.IsMandatory IsMandatoryUI, p.IsRange, p.ColumnName, "
                //    + "p.DefaultValue, p.DefaultValue2, p.VFormat, p.ValueMin, p.ValueMax, "
                //    + "p.SeqNo, p.VAF_Control_Ref_Value_ID, vr.Code AS ValidationCode "
                //    + "FROM VAF_Job_Para p"
                //    + " INNER JOIN VAF_Job_Para_TL t ON (p.VAF_Job_Para_ID=t.VAF_Job_Para_ID)"
                //    + " LEFT OUTER JOIN VAF_DataVal_Rule vr ON (p.VAF_DataVal_Rule_ID=vr.VAF_DataVal_Rule_ID) "
                //    + "WHERE p.VAF_Job_ID=@processid"		//	1
                //    + " AND t.VAF_Language='" + Utility.Env.GetVAF_Language(ctx) + "'"
                //    + " AND p.IsActive='Y' "
                //    + "ORDER BY SeqNo";

                strSql = @"SELECT t.Name,
                                  t.Description,
                                  t.Help,
                                  p.VAF_Control_Ref_ID,
                                  p.VAF_Job_Para_ID,
                                  p.FieldLength,
                                  p.IsMandatory IsMandatoryUI,
                                  p.IsRange,
                                  p.ColumnName,
                                  p.DefaultValue,
                                  p.DefaultValue2,
                                  p.VFormat,
                                  p.ValueMin,
                                  p.ValueMax,
                                  p.SeqNo,
                                  p.VAF_Control_Ref_Value_ID,
                                  vr.Code AS ValidationCode,
                                  p.VAF_QuickSearchWindow_id,
                                  p.LoadRecursiveData
                                    , p.ShowChildOfSelected,
p.IsEncrypted
                                FROM VAF_Job_Para p
                                INNER JOIN VAF_Job_Para_TL t
                                ON (p.VAF_Job_Para_ID=t.VAF_Job_Para_ID)
                                LEFT OUTER JOIN VAF_DataVal_Rule vr
                                ON (p.VAF_DataVal_Rule_ID =vr.VAF_DataVal_Rule_ID)
                                WHERE p.VAF_Job_ID=@processid
                                AND t.VAF_Language    ='" + Utility.Env.GetVAF_Language(ctx) + @"'
                                AND p.IsActive       ='Y'
                                ORDER BY SeqNo";
            }
            DataSet ds = null;
            try
            {
                ds = DB.ExecuteDataset(strSql, param, null);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    int count = ds.Tables[0].Rows.Count;
                    for (int i = 0; i < count; i++)
                    {
                        GridFieldVO fvo = GridFieldVO.CreateParameter(ctx, windowNo, ds.Tables[0].Rows[i]);
                        GridField fo = new GridField(fvo);
                        fields.Add(fo);
                    }
                }
            }
            catch
            {
                //Log
            }
            return fields;
        }
    }
}
