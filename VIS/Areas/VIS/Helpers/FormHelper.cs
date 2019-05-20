using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.DataContracts;

namespace VIS.Helpers
{
    public class FormHelper
    {
        public static FormDataOut GetFormInfo(int AD_Form_ID, Ctx ctx)
        {
            FormDataOut outt = new FormDataOut();

            String className = null;

            String sql = "SELECT Name, Description, Classname, Help , IsReport, DisplayName FROM AD_Form WHERE AD_Form_ID=" + AD_Form_ID;  //jz ClassName, AD?
            bool trl = !Env.IsBaseLanguage(ctx, "AD_Form");
            if (trl)
            {
                sql = " SELECT f.Name, t.Description, f.Classname, t.Help ,f.IsReport,t.Name " //jz
                    + " FROM AD_Form f INNER JOIN AD_Form_Trl t"
                    + " ON (f.AD_Form_ID=t.AD_Form_ID AND AD_Language='" + Env.GetAD_Language(ctx) + "')"
                    + " WHERE f.AD_Form_ID=" + AD_Form_ID;
            }

            IDataReader idr = null;
            DataTable dt = null;

            try
            {
                idr = DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    outt.Name = Util.GetValueOfString(dr[0]);
                    outt.Description = Util.GetValueOfString(dr[1]);
                    if (dr["IsReport"].ToString().Equals("Y", StringComparison.OrdinalIgnoreCase))
                    {
                        className = "VAdvantage.Report.ReportLoader";
                        outt.IsReport = true;
                    }
                    else
                    {
                        className = dr[2].ToString();
                        string prefix = "";
                        string nSpace = "";

                        try
                        {
                            //Tuple<String, String> aInfo = null;
                            if (Env.GetModulePrefix(outt.Name, out prefix, out nSpace))
                            {
                                className = className.Replace(nSpace, prefix.Substring(0, prefix.Length - 1));

                            }
                            //else if (!string.IsNullOrEmpty(className) && Env.GetModulePrefix(className.Substring(, out prefix,out nSpace))
                            //{

                            //}
                            else
                            {
                                if (prefix.Length == 0)
                                {
                                    prefix = "VIS_";
                                }
                                if (className.Contains("org.compiere.apps.form"))
                                {
                                    className = className.Replace("org.compiere.apps.form", "VAdvantage.Apps.AForms");
                                }
                                className = className.Replace("org.compiere.install", "VAdvantage.Install");

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
                        outt.Prefix = "";
                    }


                    outt.ClassName = className;
                    //className = Util.GetValueOfString(dr[2]);
                    outt.Help = Util.GetValueOfString(dr[3]);
                    // if (!trl)
                    // {
                    outt.DisplayName = Util.GetValueOfString(dr[5]);
                    // }
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                outt.IsError = true;
                outt.Message = e.Message;
            }
            if (className == null)
            {
                //return false;
            }
            else
            {
                className = className.Replace("org.compiere.apps.form", "VAdvantage.Apps.AForms");
                className = className.Replace("org.compiere.install", "VAdvantage.Install");
                // bool success = OpenForm(AD_Form_ID, className, name);
                //	Log
                MSession session = MSession.Get(ctx, true);
                session.WindowLog(ctx.GetAD_Client_ID(), ctx.GetAD_Org_ID(), 0, AD_Form_ID);
            }

            return outt;
        }
    }
}