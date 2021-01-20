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
        //    //        int VAF_Job_ID = Convert.ToInt32(Request.QueryString["pid"]);
        //    //        int VAF_TableView_ID = Convert.ToInt32(Request.QueryString["tid"]);
        //    //        int Record_ID = Convert.ToInt32(Request.QueryString["rid"]);
        //    //        int VAF_JInstance_ID = Convert.ToInt32(Request.QueryString["aid"]);
        //    //        VAdvantage.ProcessEngine.ProcessInfo pi = new VAdvantage.ProcessEngine.ProcessInfo(title, VAF_Job_ID, VAF_TableView_ID, Record_ID);
        //    //        pi.SetVAF_JInstance_ID(VAF_JInstance_ID);
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
                int VAF_Job_ID = Convert.ToInt32(Request.QueryString["pid"]);
                int VAF_TableView_ID = Convert.ToInt32(Request.QueryString["tid"]);
                int Record_ID = Convert.ToInt32(Request.QueryString["rid"]);
                int VAF_JInstance_ID = Convert.ToInt32(Request.QueryString["aid"]);
                VAdvantage.ProcessEngine.ProcessInfo pi = new VAdvantage.ProcessEngine.ProcessInfo(title, VAF_Job_ID, VAF_TableView_ID, Record_ID);
                pi.SetVAF_JInstance_ID(VAF_JInstance_ID);
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