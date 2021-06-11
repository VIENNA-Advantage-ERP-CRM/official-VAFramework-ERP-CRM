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

        public static List<GridField> GetParametersList(Ctx ctx,int AD_Process_ID,int windowNo)
        {
            List<GridField> fields = new List<GridField>();

            string strSql = "";
            SqlParameter[] param;
            if (Utility.Env.IsBaseLanguage(ctx, ""))//    GlobalVariable.IsBaseLanguage())
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@processid", AD_Process_ID);
                //strSql = "SELECT p.Name, p.Description, p.Help, "
                //    + "p.AD_Reference_ID, p.AD_Process_Para_ID, "
                //    + "p.FieldLength, p.IsMandatory IsMandatoryUI, p.IsRange, p.ColumnName, "
                //    + "p.DefaultValue, p.DefaultValue2, p.VFormat, p.ValueMin, p.ValueMax, "
                //    + "p.SeqNo, p.AD_Reference_Value_ID, vr.Code AS ValidationCode "
                //    + "FROM AD_Process_Para p"
                //    + " LEFT OUTER JOIN AD_Val_Rule vr ON (p.AD_Val_Rule_ID=vr.AD_Val_Rule_ID) "
                //    + "WHERE p.AD_Process_ID=@processid"		//	1
                //    + " AND p.IsActive='Y' "
                //    + "ORDER BY SeqNo";

                strSql = @"SELECT p.Name,
                                  p.Description,
                                  p.Help,
                                  p.AD_Reference_ID,
                                  p.AD_Process_Para_ID,
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
                                  p.AD_Reference_Value_ID,
                                  vr.Code AS ValidationCode,
                                  p.AD_InfoWindow_ID,
                                  p.LoadRecursiveData,
                                  p.ShowChildOfSelected,
                                  p.IsEncrypted,
                                  p.ZoomWindow_ID
                                FROM AD_Process_Para p
                                LEFT OUTER JOIN AD_Val_Rule vr
                                ON (p.AD_Val_Rule_ID =vr.AD_Val_Rule_ID)
                                WHERE p.AD_Process_ID=@processid
                                AND p.IsActive       ='Y'
                                ORDER BY SeqNo";

            }
            else
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@processid", AD_Process_ID);
                //strSql = "SELECT t.Name, t.Description, t.Help, "
                //    + "p.AD_Reference_ID, p.AD_Process_Para_ID, "
                //    + "p.FieldLength, p.IsMandatory IsMandatoryUI, p.IsRange, p.ColumnName, "
                //    + "p.DefaultValue, p.DefaultValue2, p.VFormat, p.ValueMin, p.ValueMax, "
                //    + "p.SeqNo, p.AD_Reference_Value_ID, vr.Code AS ValidationCode "
                //    + "FROM AD_Process_Para p"
                //    + " INNER JOIN AD_Process_Para_Trl t ON (p.AD_Process_Para_ID=t.AD_Process_Para_ID)"
                //    + " LEFT OUTER JOIN AD_Val_Rule vr ON (p.AD_Val_Rule_ID=vr.AD_Val_Rule_ID) "
                //    + "WHERE p.AD_Process_ID=@processid"		//	1
                //    + " AND t.AD_Language='" + Utility.Env.GetAD_Language(ctx) + "'"
                //    + " AND p.IsActive='Y' "
                //    + "ORDER BY SeqNo";

                strSql = @"SELECT t.Name,
                                  t.Description,
                                  t.Help,
                                  p.AD_Reference_ID,
                                  p.AD_Process_Para_ID,
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
                                  p.AD_Reference_Value_ID,
                                  vr.Code AS ValidationCode,
                                  p.ad_infowindow_id,
                                  p.LoadRecursiveData,
                                  p.ShowChildOfSelected,
                                  p.IsEncrypted,
                                  p.ZoomWindow_ID
                                FROM AD_Process_Para p
                                INNER JOIN AD_Process_Para_Trl t
                                ON (p.AD_Process_Para_ID=t.AD_Process_Para_ID)
                                LEFT OUTER JOIN AD_Val_Rule vr
                                ON (p.AD_Val_Rule_ID =vr.AD_Val_Rule_ID)
                                WHERE p.AD_Process_ID=@processid
                                AND t.AD_Language    ='" + Utility.Env.GetAD_Language(ctx) + @"'
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
