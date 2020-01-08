using CrystalDecisions.CrystalReports.Engine;
using System;

using VAdvantage.CrystalReport;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VIS.WebPages
{
    public partial class CrystalReprotViewer : System.Web.UI.Page
    {
        private static VLogger log = VLogger.GetVLogger(typeof(CrystalReprotViewer).FullName);
        ReportDocument rd = null;
        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    //try
        //    //{
        //    //    //if (IsPostBack == false)
        //    //    //{
        //    //        Ctx ctx = Session["ctx"] as Ctx;
        //    //        string title = Convert.ToString(Request.QueryString["title"]);
        //    //        int AD_Process_ID = Convert.ToInt32(Request.QueryString["pid"]);
        //    //        int AD_Table_ID = Convert.ToInt32(Request.QueryString["tid"]);
        //    //        int Record_ID = Convert.ToInt32(Request.QueryString["rid"]);
        //    //        int AD_PInstance_ID = Convert.ToInt32(Request.QueryString["aid"]);
        //    //        VAdvantage.ProcessEngine.ProcessInfo pi = new VAdvantage.ProcessEngine.ProcessInfo(title, AD_Process_ID, AD_Table_ID, Record_ID);
        //    //        pi.SetAD_PInstance_ID(AD_PInstance_ID);
        //    //        CrystalReportEngine cr = new CrystalReportEngine(ctx, pi);
        //    //        rd = cr.GetReportDocument();
        //    //        CrystalReportViewer1.ReportSource = rd;
        //    //    //}
        //    //    //else {
        //    //    //    CrystalReportViewer1.ReportSource = (ReportDocument)Session["Repr"];
        //    //    //}
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    log.SaveError("", ex.Message);
        //    //}

        //}

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                //if (IsPostBack == false)
                //{
                Ctx ctx = Session["ctx"] as Ctx;
                string title = Convert.ToString(Request.QueryString["title"]);
                int AD_Process_ID = Convert.ToInt32(Request.QueryString["pid"]);
                int AD_Table_ID = Convert.ToInt32(Request.QueryString["tid"]);
                int Record_ID = Convert.ToInt32(Request.QueryString["rid"]);
                int AD_PInstance_ID = Convert.ToInt32(Request.QueryString["aid"]);
                VAdvantage.ProcessEngine.ProcessInfo pi = new VAdvantage.ProcessEngine.ProcessInfo(title, AD_Process_ID, AD_Table_ID, Record_ID);
                pi.SetAD_PInstance_ID(AD_PInstance_ID);
                CrystalReportEngine cr = new CrystalReportEngine(ctx, pi);
                rd = cr.GetReportDocument();
                CrystalReportViewer1.ReportSource = rd;
                //}
                //else {
                //    CrystalReportViewer1.ReportSource = (ReportDocument)Session["Repr"];
                //}
            }
            catch (Exception ex)
            {
                log.SaveError("", ex.Message);
            }
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            try
            {
                rd.Close();
                rd.Dispose();
                GC.Collect();
            }
            catch { }
        }


    }
}