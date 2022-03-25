using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.Utility;
using VIS.DBase;
using VIS.Models;

namespace VIS.Helpers
{
    public class ShortcutHelper
    {

        public static List<ShortcutItemModel> GetShortcutItems(Ctx ctx)
        {

            List<ShortcutItemModel> lst = new List<ShortcutItemModel>();


            bool isBaseLang = Env.IsBaseLanguage(ctx, "AD_Shortcut");


            int AD_Client_ID = ctx.GetAD_Client_ID();
            StringBuilder sql = new StringBuilder(@" SELECT o.Name AS Name, 
                                    o.AD_Image_ID AS AD_Image_ID,
                                  o.Classname AS ClassName  ,
                                   o.AD_ShortCut_ID,
                                  o.Action  AS Action   ,
                                  (
                                  CASE
                                    WHEN o.Action = 'W'
                                    THEN o.AD_Window_ID
                                    WHEN o.Action='P'
                                    OR o.Action  ='R'
                                    THEN o.AD_Process_ID
                                    WHEN o.Action = 'B'
                                    THEN o.AD_Workbench_ID
                                    WHEN o.Action = 'T'
                                    THEN o.AD_Task_ID
                                    WHEN o.Action = 'X'
                                    THEN o.AD_Form_ID
                                    WHEN o.Action ='F'
                                    THEN o.AD_Workflow_ID
                                    ELSE 0
                                  END ) AS ActionID,
                                
                                  (
                                  CASE
                                    WHEN
                                     (SELECT COUNT(*)
                                       FROM AD_Shortcut i
                                      WHERE i.Parent_ID = o.AD_Shortcut_ID and IsChild = 'Y') >0
                                    THEN 'Y'
                                    ELSE 'N'
                                  END) AS HasChild, o.AD_Shortcut_ID as ID,  o.Url as Url,
                                  
                                  (SELECT COUNT(*) FROM AD_ShortcutParameter WHERE AD_ShortCut_ID=o.AD_ShortCut_ID) as hasPara,");

            if (isBaseLang)
            {
                sql.Append(" COALESCE(o.DisplayName,o.Name) as Name2");
            }
            else
            {
                sql.Append(" COALESCE(trl.Name,o.Name) as Name2");
            }
            sql.Append(" FROM AD_Shortcut o ");
            if (!isBaseLang)
            {
                sql.Append(" INNER JOIN AD_Shortcut_Trl trl ON o.AD_Shortcut_ID = trl.AD_Shortcut_ID AND trl.AD_Language =")
                .Append("'").Append(Env.GetAD_Language(ctx)).Append("'");
            }

            sql.Append(" WHERE o.AD_Client_ID = 0 AND o.IsActive ='Y' AND o.IsChild = 'N' ");

            sql.Append(@"AND (o.AD_Window_ID IS NULL OR EXISTS (SELECT * FROM Ad_Window_Access w WHERE w.AD_Window_ID=o.AD_Window_ID AND w.IsReadWrite='Y' and AD_Role_ID=" + ctx.GetAD_Role_ID() + @"))
                        AND (o.AD_Form_ID IS NULL OR EXISTS (SELECT * FROM ad_Form_access f WHERE f.ad_form_id=o.AD_Form_ID AND f.isreadwrite='Y' and AD_Role_ID=" + ctx.GetAD_Role_ID() + @"))
                        AND (o.AD_Process_ID IS NULL OR EXISTS (SELECT * FROM ad_process_access p WHERE p.ad_process_id=o.AD_Process_ID AND p.isreadwrite='Y' and AD_Role_ID=" + ctx.GetAD_Role_ID() + @"))");
            sql.Append("  ORDER BY SeqNo");

            IDataReader dr = null;
            try
            {
                dr = DB.ExecuteReader(sql.ToString(), null);
            }
            catch
            {
                if (dr != null)
                    dr.Close();
                return lst;
            }

            CreateShortcut(dr, lst, ctx);

            return lst;
        }

        private static void CreateShortcut(IDataReader dr, List<ShortcutItemModel> lst, Ctx ctx, bool isSetting = false)
        {
            while (dr.Read())
            {


                ShortcutItemModel itm = new ShortcutItemModel();

                itm.ShortcutName = Util.GetValueOfString(dr["Name2"]);
                itm.Action = Util.GetValueOfString(dr["Action"]);
                itm.ActionID = Util.GetValueOfInt(dr["ActionID"]);
                itm.SpecialAction = Util.GetValueOfString(dr["ClassName"]);
                itm.ActionName = Util.GetValueOfString(dr["Name"]);
                if (!isSetting)
                    itm.HasChild = "Y".Equals(Util.GetValueOfString(dr["HasChild"]));

                if (!string.IsNullOrEmpty(itm.SpecialAction))
                {
                    string className = itm.SpecialAction;
                    string prefix = "";
                    string nSpace = "";

                    try
                    {
                      //  Tuple<String, String> aInfo = null;
                        if (Env.GetModulePrefix(itm.ActionName, out prefix, out nSpace))
                        {
                            className = className.Replace(nSpace, prefix.Substring(0, prefix.Length - 1));

                        }
                        else
                        {
                            if (prefix.Length == 0)
                            {
                                prefix = "VIS_";
                            }
                            

                            nSpace = "VAdvantage";
                            if (className.Contains(nSpace))
                            {
                                className = className.Replace(nSpace, prefix.Substring(0, prefix.Length - 1));
                            }
                            nSpace = "ViennaAdvantage";
                            if (className.Contains(nSpace))
                            {
                                className = className.Replace(nSpace, prefix.Substring(0, prefix.Length - 1));
                            }
                        }
                    }
                    catch
                    {
                        // blank
                    }
                    itm.SpecialAction = className;
                }



                StringBuilder builder = new StringBuilder();

                if (Util.GetValueOfInt(dr["HASPARA"]) > 0)
                {
                    string strSql = "SELECT parametername, parametervalue,ISENCRYPTED FROM AD_ShortCutParameter WHERE IsActive='Y' AND AD_ShortCut_ID=" + Util.GetValueOfInt(dr["AD_SHORTCUT_ID"]);
                    IDataReader drPara = null;
                    try
                    {
                        drPara = DB.ExecuteReader(strSql, null);
                        while (drPara.Read())
                        {
                            if (drPara["PARAMETERVALUE"] != null && drPara["PARAMETERVALUE"].ToString() != "")
                            {
                                string variableName = drPara["PARAMETERVALUE"].ToString();
                                String columnName = string.Empty;
                                string env = string.Empty;
                                if (variableName.Contains("@"))
                                {
                                    int index = variableName.IndexOf("@");
                                    columnName = variableName.Substring(index + 1);
                                    index = columnName.IndexOf("@");
                                    if (index == -1)
                                    {
                                        break;
                                    }
                                    columnName = columnName.Substring(0, index);
                                    env = ctx.GetContext(columnName);
                                }
                                else
                                {
                                    if (drPara["PARAMETERNAME"] != null && drPara["PARAMETERNAME"].ToString() != "")
                                    {
                                        columnName = drPara["PARAMETERNAME"].ToString();
                                    }
                                    env = variableName;
                                }

                                if (env.Length == 0)
                                {
                                    break;
                                }

                                if (drPara["ISENCRYPTED"].ToString().Equals("Y", StringComparison.OrdinalIgnoreCase))
                                {
                                    env = SecureEngine.Encrypt(env);
                                }
                                if (columnName.StartsWith("#"))
                                {
                                    while (columnName.StartsWith("#"))
                                    {
                                        columnName = columnName.Substring(1);
                                    }
                                }
                                builder.Append(columnName).Append("=").Append(env).Append('&');
                            }
                        }
                        builder.ToString().TrimEnd('&');
                        if (drPara != null)
                        {
                            drPara.Close();
                            drPara = null;
                        }
                    }
                    catch
                    {
                        if (drPara != null)
                        {
                            drPara.Close();
                            drPara = null;
                        }
                    
                    }
                }

                if ((builder.ToString().Length > 0))
                {
                    itm.Url = Util.GetValueOfString(dr["Url"]) + builder.ToString();
                }
                else
                {
                    itm.Url = Util.GetValueOfString(dr["Url"]);
                }

                itm.KeyID = Util.GetValueOfInt(dr["ID"]);
                int AD_Image_ID = Util.GetValueOfInt(dr["AD_Image_ID"]);
                if (AD_Image_ID > 0)
                {
                    var img = new VAdvantage.Model.MImage(ctx, AD_Image_ID, null);

                    if (img.GetFontName() != null && img.GetFontName().Length > 0)
                    {
                        itm.HasImage = true;
                        itm.IsImageByteArray = false;
                        itm.IconUrl = img.GetFontName();
                    }
                    else if (img.GetImageURL() != null && img.GetImageURL().Length > 0)
                    {
                        itm.HasImage = true;
                        itm.IsImageByteArray = false;
                        itm.IconUrl = img.GetImageURL();
                    }
                    else if (img.GetBinaryData() != null)
                    {
                        itm.HasImage = true;
                        itm.IsImageByteArray = true;
                        itm.IconBytes = img.GetBinaryData();
                    }
                }
                lst.Add(itm);
            }
            dr.Close();
        }

        public static List<ShortcutItemModel> GetSettingItems(Ctx ctx, int AD_Shortcut_ID)
        {

            List<ShortcutItemModel> lst = new List<ShortcutItemModel>();

            bool isBaseLang = Env.IsBaseLanguage(ctx, "AD_Shortcut");


            string sql = @"SELECT o.Name AS Name,
                                  o.AD_Image_ID AS AD_Image_ID,
                                  o.Classname AS ClassName  ,
                                  o.Action  AS Action   ,
                                  (
                                  CASE
                                    WHEN o.action = 'W'
                                    THEN o.ad_window_id
                                    WHEN o.action='P'
                                    OR o.action  ='R'
                                    THEN o.ad_process_id
                                    WHEN o.action = 'B'
                                    THEN o.ad_workbench_id
                                    WHEN o.action = 'T'
                                    THEN o.ad_task_id
                                    WHEN o.action = 'X'
                                    THEN o.ad_form_id
                                    WHEN o.action ='F'
                                    THEN o.ad_workflow_id
                                    ELSE 0
                                  END ) AS ActionID, o.ad_shortcut_id as ID,  o.Url as Url, 
                                (SELECT COUNT(*) FROM AD_ShortcutParameter WHERE AD_ShortCut_ID=o.AD_ShortCut_ID) as hasPara,";

            if (isBaseLang)
            {
                sql += " o.DisplayName as Name2 FROM AD_Shortcut o ";
            }
            else
            {
                sql += " trl.Name as Name2 FROM AD_Shortcut o INNER JOIN AD_Shortcut_Trl trl ON o.AD_Shortcut_ID = trl.AD_Shortcut_ID "
                         + " AND trl.AD_Language =  '" + Env.GetAD_Language(ctx) + "' ";
            }
            sql += @" WHERE o.AD_Client_ID = 0
                                  AND o.IsActive         ='Y'
                                  AND o.IsChild          = 'Y'
                                 AND o.Parent_ID =  " + AD_Shortcut_ID + @"
                        AND (o.AD_Window_ID IS NULL OR EXISTS (SELECT * FROM AD_Window_Access w WHERE w.AD_Window_ID=o.AD_Window_ID AND w.IsReadWrite='Y' and AD_Role_ID=" + ctx.GetAD_Role_ID() + @"))
                        AND (o.AD_Form_ID IS NULL OR EXISTS (SELECT * FROM AD_Form_Access f WHERE f.AD_Form_ID=o.AD_Form_ID AND f.IsReadWrite='Y' and AD_Role_ID=" + ctx.GetAD_Role_ID() + @"))
                        AND (o.AD_Process_ID IS NULL OR EXISTS (SELECT * FROM AD_Process_Access p WHERE p.AD_Process_ID=o.AD_Process_ID AND p.IsReadWrite='Y' and AD_Role_ID=" + ctx.GetAD_Role_ID() + @"))
                        ORDER BY SeqNo";

            IDataReader dr = null;

            try
            {
                dr = DB.ExecuteReader(sql, null);
            }
            catch
            {
                if (dr != null)
                    dr.Close();
                return lst;
            }

            CreateShortcut(dr, lst, ctx,true);
            return lst;
        }
    }
}