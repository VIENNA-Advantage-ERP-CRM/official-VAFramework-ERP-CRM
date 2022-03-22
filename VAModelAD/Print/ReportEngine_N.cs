/********************************************************
 * Module Name    :     Report
 * Purpose        :     Generate Reports
 * Author         :     Jagmohan Bhatt
 * Date           :     13-July-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Model;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using System.IO;
using VAdvantage.Process;
using VAdvantage.Login;
using VAdvantage.Logging;

using System.Web.UI;
using System.Drawing;
using Exl = Excel;
using System.Reflection;

using System.Drawing.Printing;
using System.Windows.Forms;
using VAdvantage.ProcessEngine;
using VAModelAD.Model;

namespace VAdvantage.Print
{

    public class ReportEngine_N:IReportEngine,IReportView
    {
        public ReportEngine_N(Ctx ctx, MPrintFormat pf, Query query, PrintInfo info)
        {
            string tableName = "";

            if (info != null)
            {
                try
                {
                    string sqlTable = "select tablename from ad_table where ad_table_id = " + info.GetAD_Table_ID();
                    tableName = Util.GetValueOfString(DB.ExecuteScalar(sqlTable, null, null));
                    if (tableName != null || tableName != "")
                    {
                        string orgSql = "select ad_org_id from " + tableName + " where  " + tableName + "_ID = " + info.GetRecord_ID();
                        _ad_org_id = Util.GetValueOfInt(DB.ExecuteScalar(orgSql, null, null));
                    }
                }
                catch
                {
                    _ad_org_id = -1;
                }
            }


            if (pf == null)
                throw new ArgumentException("ReportEngine - no PrintFormat");
            log.Info(pf + " -- " + query);
            m_ctx = ctx;
            m_printerName = m_ctx.GetPrinterName();
            //
            m_printFormat = pf;
            m_info = info;



            SetQuery(query);		//	loads Data
        }	//	ReportEngine

        /**	Static Logger	*/
        private static VLogger log = VLogger.GetVLogger(typeof(ReportEngine_N).FullName);

        /**	Context					*/
        private Ctx m_ctx;

        /**	Print Format			*/
        private MPrintFormat m_printFormat;
        /** Print Info				*/
        private PrintInfo m_info;
        /**	Query					*/
        private Query m_query;
        /**	Query Data				*/
        private PrintData m_printData;
        /** Layout					*/
        private LayoutEngine m_layout = null;
        /**	Printer					*/
        private String m_printerName = null;
        /**	View					*/
        private View m_view = null;

        //******************//
        private int _ad_org_id = -1;

        public void SetPrintFormat(MPrintFormat pf)
        {
            m_printFormat = pf;
            if (m_layout != null)
            {
                SetPrintData();
                m_layout.SetPrintFormat(pf, false);
                m_layout.SetPrintData(m_printData, m_query, true);	//	format changes data
            }
            if (m_view != null)
                m_view.Invalidate();
        }	//	setPrintFormat

        public void SetQuery(Query query)
        {
            m_query = query;
            if (query == null)
                return;
            //
            SetPrintData();
            if (m_layout != null)
                m_layout.SetPrintData(m_printData, m_query, true);
            if (m_view != null)
                m_view.Refresh();
        }	//	setQuery

        public Query GetQuery()
        {
            return m_query;
        }	//	getQuery

        private void SetPrintData()
        {
            if (m_query == null)
                return;
            DataEngine de = new DataEngine(m_printFormat.GetLanguage());
            de.SetPInfo(m_info);
            SetPrintData(de.GetPrintData(m_ctx, m_printFormat, m_query));
            //	m_printData.dump();
        }	//	setPrintData

        private void SetPrintData(Query query)
        {
            if (query == null)
                return;
            DataEngine de = new DataEngine(m_printFormat.GetLanguage());
            de.SetPInfo(m_info);
            SetPrintData(de.GetPrintData(m_ctx, m_printFormat, query));
            //	m_printData.dump();
        }	//	setPrintData

        public PrintData GetPrintData()
        {
            return m_printData;
        }	//	getPrintData

        public void SetPrintData(PrintData printData)
        {
            if (printData == null)
                return;
            m_printData = printData;
        }	//	setPrintData


        StringBuilder html = null;

        private void Layout()
        {
            if (m_printFormat == null)
                throw new Exception("No print format");
            if (m_printData == null)
                throw new Exception("No print data (Delete Print Format and restart)");

            //actaull calling for the reports happens here
           // m_layout = new LayoutEngine(m_printFormat, m_printData, m_query);
            m_layout = new LayoutEngine(m_printFormat, m_printData, m_query,_ad_org_id);
            html = m_layout.GetRptHtml();
            //	Printer
            String printerName = m_printFormat.GetPrinterName();
            if (printerName == null && m_info != null)
                printerName = m_info.GetPrinterName();
            //setPrinterName(printerName);
        }	//	layout

        public StringBuilder GetRptHtml()
        {
            return html;
        }

        protected LayoutEngine GetLayout()
        {
            if (m_layout == null)
                Layout();
            return m_layout;
        }	//	getLayout

        public String GetName()
        {
            return m_printFormat.GetName();
        }	//	getName

        public MPrintFormat GetPrintFormat()
        {
            return m_printFormat;
        }	//	getPrintFormat

        public PrintInfo GetPrintInfo()
        {
            return m_info;
        }	//	getPrintInfo

        public Ctx GetCtx()
        {
            return m_layout.GetCtx();
        }	//	getCtx

        public int GetRowCount()
        {
            return m_printData.GetRowCount();
        }	//	getRowCount

        public int GetColumnCount()
        {
            if (m_layout != null)
                return m_layout.GetColumnCount();
            return 0;
        }	//	getColumnCount

        public View GetView()
        {
            if (m_layout == null)
                Layout();
            if (m_view == null)
                m_view = new View(m_layout);
            return m_view;
        }	//	getView


        public void Print()
        {
            log.Info(m_info.ToString());
            if (m_layout == null)
                Layout();

            PrintDocument pd = new PrintDocument();
            pd.DefaultPageSettings.Landscape = m_layout.GetPaper().IsLandscape();

        }

        #region Archive

        public byte[] CreateCSV(Ctx ctx)
        {
            //added by jagmohan
            FILE_PATH = GlobalVariable.PhysicalPath + "TempDownload";

            if (!Directory.Exists(FILE_PATH))
                Directory.CreateDirectory(FILE_PATH);

            string filePath = FILE_PATH + "\\temp_" + CommonFunctions.CurrentTimeMillis() + ".csv";
            System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write);
            CreateCSV(fs, ',', Env.GetLanguage(ctx));

            var ms = new MemoryStream();
            var buff = new byte[64000];
            using (var sr = new FileStream(filePath, FileMode.Open))
            {
                for (; ; )
                {
                    int read = sr.Read(buff, 0, buff.Length);
                    ms.Write(buff, 0, read);
                    if (read == 0) break;
                }
            }

            return ms.ToArray();
        }



        public string GetCSVPath(Ctx ctx)
        {
            //added by jagmohan
            FILE_PATH = GlobalVariable.PhysicalPath + "TempDownload";

            if (!Directory.Exists(FILE_PATH))
                Directory.CreateDirectory(FILE_PATH);

            string filePath = FILE_PATH + "\\temp_" + CommonFunctions.CurrentTimeMillis() + Guid.NewGuid().ToString("N").Substring(0, 5) + ".csv";
            System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write);
            CreateCSV(fs, ',', Env.GetLanguage(ctx));

            return filePath.Substring(filePath.IndexOf("TempDownload"));
        }


        public string FILE_PATH = "D:\\TempDownload";

        public byte[] GetReportBytes()
        {
            return CreatePDF();
        }

        public string GetReportString()
        {
            return null;
        }

        public String GetReportFilePath(bool fetchBytes, out byte[] bArry)
        {
            bArry = null;
            return CreatePDF(fetchBytes, bArry);
        }

        public bool StartReport(Ctx ctx, ProcessInfo pi, Trx trx)
        {
            return true;
        }

        public byte[] CreatePDF()
        {
            //added by jagmohan
            FILE_PATH = GlobalVariable.PhysicalPath + "TempDownload";

            if (!Directory.Exists(FILE_PATH))
                Directory.CreateDirectory(FILE_PATH);

            string filePath = FILE_PATH + "\\temp_" + CommonFunctions.CurrentTimeMillis() + ".pdf";

            var ms = new MemoryStream();
           
            try
            {
                //string fileName = "C:\\" + GetName().Replace(" ", "_") + "_" + VAdvantage.Classes.CommonFunctions.CurrentTimeMillis() + ".pdf";
                PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();


                
                PdfSharp.Drawing.XGraphics xg;
                for (int page = 0; page < m_layout.GetPages().Count(); page++)
                {
                    Rectangle pageRectangle = GetRectangleOfPage(page + 1);
                    PdfSharp.Pdf.PdfPage pdfpage = document.AddPage();
                    pdfpage.Height = pageRectangle.Height;
                    pdfpage.Width = pageRectangle.Width;
                    xg = PdfSharp.Drawing.XGraphics.FromPdfPage(pdfpage);
                    Page p = (Page)m_layout.GetPages()[page];
                    p.PaintPdf(xg, pageRectangle, true, false);		//	sets context
                    m_layout.GetHeaderFooter().PaintPdf(xg, pageRectangle, true);
                }
                //xg.Dispose();


                document.Save(filePath);
                document.Close();

                // load file into stream

                var buff = new byte[64000];
                using (var sr = new FileStream(filePath, FileMode.Open))
                {
                    for (; ; )
                    {
                        int read = sr.Read(buff, 0, buff.Length);
                        ms.Write(buff, 0, read);
                        if (read == 0) break;
                    }
                }

                //bytes = StreamFile(Application.StartupPath + "\\temp.pdf");

                //File.Delete(filePath);
            }
            catch
            {
                return null;
            }

            return ms.ToArray();
        }

        public string CreatePDF(bool fetchBytes, byte[] bytes)
        {
            //added by jagmohan
            FILE_PATH = GlobalVariable.PhysicalPath + "TempDownload";
            if (!Directory.Exists(FILE_PATH))
                Directory.CreateDirectory(FILE_PATH);

            string filePath = FILE_PATH + "\\temp_" + CommonFunctions.CurrentTimeMillis() + Guid.NewGuid().ToString("N").Substring(0, 5) + ".pdf";

            var ms = new MemoryStream();
            //byte[] bytes = null;
            try
            {
                //string fileName = "C:\\" + GetName().Replace(" ", "_") + "_" + VAdvantage.Classes.CommonFunctions.CurrentTimeMillis() + ".pdf";
                PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();



                PdfSharp.Drawing.XGraphics xg;
                for (int page = 0; page < m_layout.GetPages().Count(); page++)
                {
                    Rectangle pageRectangle = GetRectangleOfPage(page + 1);
                    PdfSharp.Pdf.PdfPage pdfpage = document.AddPage();
                    pdfpage.Height = pageRectangle.Height;
                    pdfpage.Width = pageRectangle.Width;
                    xg = PdfSharp.Drawing.XGraphics.FromPdfPage(pdfpage);
                    Page p = (Page)m_layout.GetPages()[page];
                    p.PaintPdf(xg, pageRectangle, true, false);		//	sets context
                    m_layout.GetHeaderFooter().PaintPdf(xg, pageRectangle, true);
                }
                //xg.Dispose();


                document.Save(filePath);
                document.Close();

                // load file into stream
                if (fetchBytes)
                {
                    var buff = new byte[64000];
                    using (var sr = new FileStream(filePath, FileMode.Open))
                    {
                        for (; ; )
                        {
                            int read = sr.Read(buff, 0, buff.Length);
                            ms.Write(buff, 0, read);
                            if (read == 0) break;
                        }
                    }
                    bytes = ms.ToArray();
                }
            }
            catch(Exception e)
            {
                log.Severe("ReportEngine_N_CreatePDF_" + e.ToString());
                return null;
            }

            return filePath.Substring(filePath.IndexOf("TempDownload"));
        }

        private byte[] StreamFile(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);

            // Create a byte array of file stream length
            byte[] ImageData = new byte[fs.Length];

            //Read block of bytes from stream into the byte array
            fs.Read(ImageData, 0, System.Convert.ToInt32(fs.Length));

            //Close the File Stream
            fs.Close();
            return ImageData; //return the byte data
        }
        #endregion

        public bool CreatePDF(string fileName, bool isPrint)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            bool b = CreatePDF(fileName);
            if (isPrint && b)
            {
                try
                {
                    System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
                    myProcess.StartInfo.FileName = fileName;
                    myProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    myProcess.StartInfo.CreateNoWindow = true;
                    myProcess.StartInfo.Verb = "print";
                    //myProcess.StartInfo.Arguments = cboTSPrinter.SelectedItem;
                    myProcess.StartInfo.UseShellExecute = true;

                    myProcess.Start();
                    myProcess.WaitForExit(2000);

                    myProcess.Close();
                }
                catch (Exception ex)
                {
                    log.Info(ex.Message);
                }
            }

            return true;
        }

        public bool CreatePDF(string fileName)
        {
            try
            {
                //First check for diretory if not exits then Craete
                if (!Directory.Exists(Path.Combine(GlobalVariable.PhysicalPath, "TempDownload")))
                {
                    Directory.CreateDirectory(Path.Combine(GlobalVariable.PhysicalPath, "TempDownload"));
                }


                //string fileName = "C:\\" + GetName().Replace(" ", "_") + "_" + VAdvantage.Classes.CommonFunctions.CurrentTimeMillis() + ".pdf";
                PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
                PdfSharp.Drawing.XGraphics xg;
                for (int page = 0; page < m_layout.GetPages().Count(); page++)
                {
                    Rectangle pageRectangle = GetRectangleOfPage(page + 1);
                    PdfSharp.Pdf.PdfPage pdfpage = document.AddPage();
                    pdfpage.Height = pageRectangle.Height;
                    pdfpage.Width = pageRectangle.Width;
                    xg = PdfSharp.Drawing.XGraphics.FromPdfPage(pdfpage);
                    Page p = (Page)m_layout.GetPages()[page];
                    p.PaintPdf(xg, pageRectangle, true, false);		//	sets context
                    m_layout.GetHeaderFooter().PaintPdf(xg, pageRectangle, true);
                }
                //FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                //StreamWriter sw = new StreamWriter(fs);

                //xg.Dispose();
                document.Save(fileName);
                document.Close();
            }
            catch
            {
                return false;
            }
            //System.Diagnostics.Process.Start(fileName);
            return true;
        }

        public byte[] CreatePDF(string table_ID, string Record_ID)
        {
            string path = table_ID + "_" + Record_ID;
            FILE_PATH = GlobalVariable.PhysicalPath + "TempDownload";

            if (!Directory.Exists(FILE_PATH))
                Directory.CreateDirectory(FILE_PATH);

            string filePath = FILE_PATH + "\\Inv_" + path + ".pdf";

            var ms = new MemoryStream();
            
            try
            {
                //string fileName = "C:\\" + GetName().Replace(" ", "_") + "_" + VAdvantage.Classes.CommonFunctions.CurrentTimeMillis() + ".pdf";
                PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
                PdfSharp.Drawing.XGraphics xg;
                for (int page = 0; page < m_layout.GetPages().Count(); page++)
                {
                    Rectangle pageRectangle = GetRectangleOfPage(page + 1);
                    PdfSharp.Pdf.PdfPage pdfpage = document.AddPage();
                    pdfpage.Height = pageRectangle.Height;
                    pdfpage.Width = pageRectangle.Width;
                    xg = PdfSharp.Drawing.XGraphics.FromPdfPage(pdfpage);
                    Page p = (Page)m_layout.GetPages()[page];
                    p.PaintPdf(xg, pageRectangle, true, false);		//	sets context
                    m_layout.GetHeaderFooter().PaintPdf(xg, pageRectangle, true);
                }
                //xg.Dispose();


                document.Save(filePath);
                document.Close();

                // load file into stream

                var buff = new byte[64000];
                using (var sr = new FileStream(filePath, FileMode.Open))
                {
                    for (; ; )
                    {
                        int read = sr.Read(buff, 0, buff.Length);
                        ms.Write(buff, 0, read);
                        if (read == 0) break;
                    }
                }

                //bytes = StreamFile(Application.StartupPath + "\\temp.pdf");

                //File.Delete(filePath);
            }
            catch
            {
                return null;
            }

            return ms.ToArray();
        }

        /**	Margin around paper				*/
        public static int MARGIN = 5;

        public int GetPaperHeight()
        {
            //return 0;
            return (int)m_layout.GetPaper().GetHeight(true);
        }	//	getPaperHeight

        public int GetPaperWidth()
        {
            //return 0;
            return (int)m_layout.GetPaper().GetWidth(true);
        }	//	getPaperHeight

        public Rectangle GetRectangleOfPage(int pageNo)
        {
            int y = MARGIN + ((pageNo - 1) * (GetPaperHeight() + MARGIN));
            return new Rectangle(MARGIN, 5, GetPaperWidth(), GetPaperHeight());
        }

        /** Order = 0				*/
        public static int ORDER = 0;
        /** Shipment = 1				*/
        public static int SHIPMENT = 1;
        /** Invoice = 2				*/
        public static int INVOICE = 2;
        /** Project = 3				*/
        public static int PROJECT = 3;
        /** RfQ = 4					*/
        public static int RFQ = 4;
        /** Remittance = 5			*/
        public static int REMITTANCE = 5;
        /** Check = 6				*/
        public static int CHECK = 6;
        /** Dunning = 7				*/
        public static int DUNNING = 7;
        /** Movement = 8            */
        public static int MOVEMENT = 8;
        /** Inventory = 9            */
        public static int INVENTORY = 9;
        /******************Manufacturing**************/
        /** WorkOrder = 10            */
        public static int WORKORDER = 10;
        /** TaskList = 11            */
        public static int TASKLIST = 11;
        /** WorkOrderTxn = 12        */
        public static int WORKORDERTXN = 12;
        /** StandardOperation = 13   */
        public static int STANDARDOPERATION = 13;
        /** Routing = 14             */
        public static int ROUTING = 14;
        /******************Manufacturing**************/

        private static String[] DOC_TABLES = new String[] {
		"C_Order_Header_v", "M_InOut_Header_v", "C_Invoice_Header_v", "C_Project_Header_v",
		"C_RfQResponse_v",
		"C_PaySelection_Check_v", "C_PaySelection_Check_v",  
		"C_DunningRunEntry_v", "M_Movement", "M_Inventory" ,
        /******************Manufacturing**************/
        "M_WorkOrder_Header_v", "M_TaskList",
		"M_WorkOrderTxn_Header_V", "M_StandardOperation_Header_v", "M_Routing_Header_v"
        /******************Manufacturing**************/
        };
        private static String[] DOC_BASETABLES = new String[] {
		"C_Order", "M_InOut", "C_Invoice", "C_Project",
		"C_RfQResponse",
		"C_PaySelectionCheck", "C_PaySelectionCheck", 
		"C_DunningRunEntry", "M_Movement", "M_Inventory" ,
        /******************Manufacturing**************/
         "M_WorkOrder", "M_TaskList",
		"M_WorkOrderTransaction", "M_StandardOperation", "M_Routing"
        /******************Manufacturing**************/
        
        };
        private static String[] DOC_IDS = new String[] {
		"C_Order_ID", "M_InOut_ID", "C_Invoice_ID", "C_Project_ID",
		"C_RfQResponse_ID",
		"C_PaySelectionCheck_ID", "C_PaySelectionCheck_ID", 
		"C_DunningRunEntry_ID", "M_Movement_ID",  "M_Inventory_ID" ,
         /******************Manufacturing**************/
          "M_WorkOrder_ID", "M_TaskList_ID",
		"M_WorkOrderTransaction_ID", "M_StandardOperation_ID", "M_Routing_ID"
          /******************Manufacturing**************/
        
        };
        private static int[] DOC_TABLE_ID = new int[] {
		X_C_Order.Table_ID, X_M_InOut.Table_ID, X_C_Invoice.Table_ID, X_C_Project.Table_ID,
		X_C_RfQResponse.Table_ID,
		X_C_PaySelectionCheck.Table_ID, X_C_PaySelectionCheck.Table_ID, 
		X_C_DunningRunEntry.Table_ID, X_M_Movement.Table_ID, X_M_Inventory.Table_ID ,
        /******************Manufacturing**************/
         X_M_WorkOrder.Table_ID, X_M_TaskList.Table_ID,
		X_M_WorkOrderTransaction.Table_ID, X_M_StandardOperation.Table_ID, X_M_Routing.Table_ID
        /******************Manufacturing**************/
        
        };

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">current context</param>
        /// <param name="pi">process info</param>
        /// <returns>same class object</returns>
        static public ReportEngine_N Get(Ctx ctx, ProcessInfo pi)
        {
            int AD_Client_ID = (int)pi.GetAD_Client_ID();
            //
            int AD_Table_ID = 0;
            int AD_ReportView_ID = 0;
            String TableName = null;
            String whereClause = "";
            String orderbyClause = "";
            int AD_PrintFormat_ID = 0;
            bool IsForm = false;
            int Client_ID = -1;

            //	Get AD_Table_ID and TableName
            String sql = "SELECT rv.AD_ReportView_ID,rv.WhereClause, "
                + " t.AD_Table_ID,t.TableName, pf.AD_PrintFormat_ID, pf.IsForm, pf.AD_Client_ID, rv.OrderByClause "
                + "FROM AD_PInstance pi"
                + " INNER JOIN AD_Process p ON (pi.AD_Process_ID=p.AD_Process_ID)"
                + " INNER JOIN AD_ReportView rv ON (p.AD_ReportView_ID=rv.AD_ReportView_ID)"
                + " INNER JOIN AD_Table t ON (rv.AD_Table_ID=t.AD_Table_ID)"
                + " LEFT OUTER JOIN AD_PrintFormat pf ON (p.AD_ReportView_ID=pf.AD_ReportView_ID AND pf.AD_Client_ID IN (0,'" + AD_Client_ID + "')) "
                + "WHERE pi.AD_PInstance_ID='" + pi.GetAD_PInstance_ID() + "' "		//	#2
                + "ORDER BY pf.AD_Client_ID DESC, pf.IsDefault DESC";	//	own first
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql);
                //	Just get first 
                if (dr.Read())
                {
                    AD_ReportView_ID = Utility.Util.GetValueOfInt(dr[0].ToString());		//	required
                    whereClause = dr[1].ToString();
                    orderbyClause = dr["OrderByClause"].ToString();
                    if (string.IsNullOrEmpty(whereClause))
                        whereClause = "";
                    //
                    AD_Table_ID = Utility.Util.GetValueOfInt(dr[2].ToString());
                    TableName = dr[3].ToString();			//	required for query
                    AD_PrintFormat_ID = Utility.Util.GetValueOfInt(dr[4].ToString());		//	required
                    IsForm = "Y".Equals(dr[5].ToString());	//	required
                    Client_ID = Utility.Util.GetValueOfInt(dr[6].ToString());
                }
                dr.Close();
            }
            catch (Exception e1)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Log(Level.SEVERE, "(1) - " + sql, e1);
            }
            //	Nothing found
            if (AD_ReportView_ID == 0)
            {
                //	Check Print format in Report Directly
                sql = "SELECT t.AD_Table_ID,t.TableName, pf.AD_PrintFormat_ID, pf.IsForm "
                    + "FROM AD_PInstance pi"
                    + " INNER JOIN AD_Process p ON (pi.AD_Process_ID=p.AD_Process_ID)"
                    + " INNER JOIN AD_PrintFormat pf ON (p.AD_PrintFormat_ID=pf.AD_PrintFormat_ID)"
                    + " INNER JOIN AD_Table t ON (pf.AD_Table_ID=t.AD_Table_ID) "
                    + "WHERE pi.AD_PInstance_ID='" + pi.GetAD_PInstance_ID() + "'";
                IDataReader idr = null;
                try
                {
                    idr = DataBase.DB.ExecuteReader(sql);
                    while (idr.Read())
                    {
                        whereClause = "";
                        AD_Table_ID = Utility.Util.GetValueOfInt(idr[0].ToString());
                        TableName = idr[1].ToString();			//	required for query
                        AD_PrintFormat_ID = Utility.Util.GetValueOfInt(idr[2].ToString());		//	required
                        IsForm = "Y".Equals(idr[3].ToString());	//	required
                        Client_ID = AD_Client_ID;
                    }
                    idr.Close();
                }
                catch (Exception e)
                {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                    log.Severe(e.ToString());
                }
                if (AD_PrintFormat_ID == 0)
                {
                    return null;
                }
            }

            //  Create Query from Parameters
            Query query = null;
            if (IsForm && pi.GetRecord_ID() != 0)	//	Form = one record
                query = Query.GetEqualQuery(TableName + "_ID", pi.GetRecord_ID());
            else
                query = Query.Get(ctx, pi.GetAD_PInstance_ID(), TableName);

            //  Add to static where clause from ReportView
            if (whereClause.Length != 0)
                query.AddRestriction(whereClause);

            //Added by Jagmohan Bhatt on 3-Feb-2010 for order by 
            //if (orderbyClause.Length != 0)
            //    query.AddRestriction(" Order By " + orderbyClause);


            //	Get Print Format
            MPrintFormat format = null;
            //Object so = pi.getSerializableObject();
            //if (so instanceof MPrintFormat)
            //	format = (MPrintFormat)so;
            if (format == null && AD_PrintFormat_ID != 0)
            {
                //	We have a PrintFormat with the correct Client
                if (Client_ID == AD_Client_ID)
                    format = MPrintFormat.Get(ctx, AD_PrintFormat_ID, false);
                else
                    format = MPrintFormat.CopyToClient(ctx, AD_PrintFormat_ID, AD_Client_ID);
            }
            if (format != null)
            {
                format.SetName(Env.TrimModulePrefix(format.GetName()));
            }

            if (format != null && format.GetItemCount() == 0)
            {

                format.Delete(true);
                format = null;
            }
            //	Create Format
            if (format == null && AD_ReportView_ID != 0)
                format = MPrintFormat.CreateFromReportView(ctx, AD_ReportView_ID, pi.GetTitle());
            if (format == null)
                return null;
            //
            PrintInfo info = new PrintInfo(pi);
            info.SetAD_Table_ID(AD_Table_ID);

            if (AD_ReportView_ID > 0)
            {
                format.IsGridReport = true;
                format.PageNo = 1;
            }


            return new ReportEngine_N(ctx, format, query, info);
        }
        /// <summary>
        /// Gets the document according to order id
        /// </summary>
        /// <param name="C_Order_ID">order id</param>
        /// <returns>array of int</returns>
        private static int[] GetDocumentWhat(int C_Order_ID)
        {
            int[] what = new int[2];
            what[0] = ORDER;
            what[1] = C_Order_ID;
            //
            String sql = "SELECT dt.DocSubTypeSO "
                + "FROM C_DocType dt, C_Order o "
                + "WHERE o.C_DocType_ID=dt.C_DocType_ID"
                + " AND o.C_Order_ID='" + C_Order_ID + "'";
            String DocSubTypeSO = null;
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql);
                while (dr.Read())
                {
                    DocSubTypeSO = dr[0].ToString();
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Severe(e.ToString());
                return null;		//	error
            }

            if (DocSubTypeSO == null)
                DocSubTypeSO = "";
            //	WalkIn Receipt, WalkIn Invoice,
            if (DocSubTypeSO.Equals("WR") || DocSubTypeSO.Equals("WI"))
                what[0] = INVOICE;
            //	WalkIn Pickup,
            else if (DocSubTypeSO.Equals("WP"))
                what[0] = SHIPMENT;
            //	Offer Binding, Offer Nonbinding, Standard Order
            else
                return what;

            //	Get Record_ID of Invoice/Receipt
            if (what[0] == INVOICE)
                sql = "SELECT C_Invoice_ID REC FROM C_Invoice WHERE C_Order_ID='" + C_Order_ID + "'"	//	1
                    + " ORDER BY C_Invoice_ID DESC";
            else
                sql = "SELECT M_InOut_ID REC FROM M_InOut WHERE C_Order_ID='" + C_Order_ID + "'" 	//	1
                    + " ORDER BY M_InOut_ID DESC";
            IDataReader idr = null;
            try
            {

                idr = DataBase.DB.ExecuteReader(sql);
               
                if (idr.Read())
                {
                    //bl = true;
                    //	if (i == 1 &&`1      ADialog.ask(0, null, what[0] == INVOICE ? "PrintOnlyRecentInvoice?" : "PrintOnlyRecentShipment?")) break;
                    what[1] = Utility.Util.GetValueOfInt(idr[0].ToString());
                }
                else
                {
                    //if (bl == true)//	No Document Found
                    what[0] = ORDER;
                }

                idr.Close();

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Severe(e.ToString());
                return null;
            }
            return what;
        }

        /// <summary>
        /// Get Document Print Engine for Document Type.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="type"></param>
        /// <param name="Record_ID"></param>
        /// <returns>Report Engine or null</returns>
        public static ReportEngine_N Get(Ctx ctx, int type, int Record_ID)
        {
            if (Record_ID < 1)
            {
                log.Log(Level.WARNING, "No PrintFormat for Record_ID=" + Record_ID
                        + ", Type=" + type);
                return null;
            }
            //	Order - Print Shipment or Invoice
            if (type == ORDER)
            {
                int[] what = GetDocumentWhat(Record_ID);
                if (what != null)
                {
                    type = what[0];
                    Record_ID = what[1];
                }
            }	//	Order
            //
            //	String JobName = DOC_BASETABLES[type] + "_Print";
            int AD_PrintFormat_ID = 0;
            int C_BPartner_ID = 0;
            String DocumentNo = null;
            int copies = 1;

            //	Language
            MClient client = MClient.Get(ctx);
            Language language = client.GetLanguage();
            // Set Report Language -VIS0228
            if (!string.IsNullOrEmpty(ctx.GetContext("Report_Lang"))) {
                language = Language.GetLanguage(ctx.GetContext("Report_Lang"));
            }
                //	Get Document Info
                String sql = null;
            if (type == CHECK)
                sql = "SELECT bad.Check_PrintFormat_ID,"								//	1
                    + "	c.IsMultiLingualDocument,bp.AD_Language,bp.C_BPartner_ID,d.DocumentNo "		//	2..5
                    + "FROM C_PaySelectionCheck d"
                    + " INNER JOIN C_PaySelection ps ON (d.C_PaySelection_ID=ps.C_PaySelection_ID)"
                    + " INNER JOIN C_BankAccountDoc bad ON (ps.C_BankAccount_ID=bad.C_BankAccount_ID AND d.PaymentRule=bad.PaymentRule)"
                    + " INNER JOIN AD_Client c ON (d.AD_Client_ID=c.AD_Client_ID)"
                    + " INNER JOIN C_BPartner bp ON (d.C_BPartner_ID=bp.C_BPartner_ID) "
                    + "WHERE d.C_PaySelectionCheck_ID=@recordid";		//	info from BankAccount
            else if (type == DUNNING)
                sql = "SELECT dl.Dunning_PrintFormat_ID,"
                    + " c.IsMultiLingualDocument,bp.AD_Language,bp.C_BPartner_ID,dr.DunningDate "
                    + "FROM C_DunningRunEntry d"
                    + " INNER JOIN AD_Client c ON (d.AD_Client_ID=c.AD_Client_ID)"
                    + " INNER JOIN C_BPartner bp ON (d.C_BPartner_ID=bp.C_BPartner_ID)"
                    + " INNER JOIN C_DunningRun dr ON (d.C_DunningRun_ID=dr.C_DunningRun_ID)"
                    + " INNER JOIN C_DunningLevel dl ON (dl.C_DunningLevel_ID=dr.C_DunningLevel_ID) "
                    + "WHERE d.C_DunningRunEntry_ID=@recordid";			//	info from Dunning
            else if (type == REMITTANCE)
                sql = "SELECT pf.Remittance_PrintFormat_ID,"
                    + " c.IsMultiLingualDocument,bp.AD_Language,bp.C_BPartner_ID,d.DocumentNo "
                    + "FROM C_PaySelectionCheck d"
                    + " INNER JOIN AD_Client c ON (d.AD_Client_ID=c.AD_Client_ID)"
                    + " INNER JOIN AD_PrintForm pf ON (c.AD_Client_ID=pf.AD_Client_ID)"
                    + " INNER JOIN C_BPartner bp ON (d.C_BPartner_ID=bp.C_BPartner_ID) "
                    + "WHERE d.C_PaySelectionCheck_ID=@recordid"		//	info from PrintForm
                    + " AND pf.AD_Org_ID IN (0,d.AD_Org_ID) ORDER BY pf.AD_Org_ID DESC";
            else if (type == PROJECT)
                sql = "SELECT pf.Project_PrintFormat_ID,"
                    + " c.IsMultiLingualDocument,bp.AD_Language,bp.C_BPartner_ID,d.Value "
                    + "FROM C_Project d"
                    + " INNER JOIN AD_Client c ON (d.AD_Client_ID=c.AD_Client_ID)"
                    + " INNER JOIN AD_PrintForm pf ON (c.AD_Client_ID=pf.AD_Client_ID)"
                    + " LEFT OUTER JOIN C_BPartner bp ON (d.C_BPartner_ID=bp.C_BPartner_ID) "
                    + "WHERE d.C_Project_ID=@recordid"					//	info from PrintForm
                    + " AND pf.AD_Org_ID IN (0,d.AD_Org_ID) ORDER BY pf.AD_Org_ID DESC";
            else if (type == RFQ)
                sql = "SELECT COALESCE(t.AD_PrintFormat_ID, pf.AD_PrintFormat_ID),"
                    + " c.IsMultiLingualDocument,bp.AD_Language,bp.C_BPartner_ID,rr.Name "
                    + "FROM C_RfQResponse rr"
                    + " INNER JOIN C_RfQ r ON (rr.C_RfQ_ID=r.C_RfQ_ID)"
                    + " INNER JOIN C_RfQ_Topic t ON (r.C_RfQ_Topic_ID=t.C_RfQ_Topic_ID)"
                    + " INNER JOIN AD_Client c ON (rr.AD_Client_ID=c.AD_Client_ID)"
                    + " INNER JOIN C_BPartner bp ON (rr.C_BPartner_ID=bp.C_BPartner_ID),"
                    + " AD_PrintFormat pf "
                    + "WHERE pf.AD_Client_ID IN (0,rr.AD_Client_ID)"
                    + " AND pf.AD_Table_ID=725 AND pf.IsTableBased='N'"	//	from RfQ PrintFormat
                    + " AND rr.C_RfQResponse_ID=@recordid "				//	Info from RfQTopic
                    + "ORDER BY t.AD_PrintFormat_ID, pf.AD_Client_ID DESC, pf.AD_Org_ID DESC";
            else if (type == MOVEMENT)
                sql = "SELECT pf.Movement_PrintFormat_ID,"
                    + " c.IsMultiLingualDocument, COALESCE(dt.DocumentCopies,0) "
                    + "FROM M_Movement d"
                    + " INNER JOIN AD_Client c ON (d.AD_Client_ID=c.AD_Client_ID)"
                    + " INNER JOIN AD_PrintForm pf ON (d.AD_Client_ID=pf.AD_Client_ID OR pf.AD_Client_ID=0)"
                    + " LEFT OUTER JOIN C_DocType dt ON (d.C_DocType_ID=dt.C_DocType_ID) "
                    + "WHERE d.M_Movement_ID=@recordid"                 //  info from PrintForm
                    + " AND pf.AD_Org_ID IN (0,d.AD_Org_ID) AND pf.Movement_PrintFormat_ID IS NOT NULL "
                    + "ORDER BY pf.AD_Client_ID DESC, pf.AD_Org_ID DESC";
            else if (type == INVENTORY)
                sql = "SELECT pf.Inventory_PrintFormat_ID,"
                    + " c.IsMultiLingualDocument, COALESCE(dt.DocumentCopies,0) "
                    + "FROM M_Inventory d"
                    + " INNER JOIN AD_Client c ON (d.AD_Client_ID=c.AD_Client_ID)"
                    + " INNER JOIN AD_PrintForm pf ON (d.AD_Client_ID=pf.AD_Client_ID OR pf.AD_Client_ID=0)"
                    + " LEFT OUTER JOIN C_DocType dt ON (d.C_DocType_ID=dt.C_DocType_ID) "
                    + "WHERE d.M_Inventory_ID=@recordid"                 //  info from PrintForm
                    + " AND pf.AD_Org_ID IN (0,d.AD_Org_ID) AND pf.Inventory_PrintFormat_ID IS NOT NULL "
                    + "ORDER BY pf.AD_Client_ID DESC,  pf.AD_Org_ID DESC";
            /****************Manfacturing***********************/
            else if (type == WORKORDER)
                sql = "SELECT COALESCE(dt.AD_PrintFormat_ID,pf.WorkOrder_PrintFormat_ID), "
                    + " c.IsMultiLingualDocument, COALESCE(dt.DocumentCopies,0), "
                    + " dt.AD_PrintFormat_ID "
                    + "FROM M_WorkOrder d"
                    + " INNER JOIN AD_Client c ON (d.AD_Client_ID=c.AD_Client_ID)"
                    + " INNER JOIN AD_PrintForm pf ON (d.AD_Client_ID=pf.AD_Client_ID OR pf.AD_Client_ID=0)"
                    + " LEFT OUTER JOIN C_DocType dt ON (d.C_DocType_ID=dt.C_DocType_ID) "
                    + "WHERE d.M_WorkOrder_ID=@recordid"                 //  info from PrintForm
                    + " AND pf.AD_Org_ID IN (0,d.AD_Org_ID) "
                    + "ORDER BY pf.AD_Client_ID DESC,  pf.AD_Org_ID DESC";
            else if (type == WORKORDERTXN)
                sql = "SELECT COALESCE(dt.AD_PrintFormat_ID,pf.WorkOrderTxn_PrintFormat_ID), "
                    + " c.IsMultiLingualDocument, COALESCE(dt.DocumentCopies,0), "
                    + " dt.AD_PrintFormat_ID "
                    + "FROM M_WorkOrderTransaction d"
                    + " INNER JOIN AD_Client c ON (d.AD_Client_ID=c.AD_Client_ID)"
                    + " INNER JOIN AD_PrintForm pf ON (d.AD_Client_ID=pf.AD_Client_ID OR pf.AD_Client_ID=0)"
                    + " LEFT OUTER JOIN C_DocType dt ON (d.C_DocType_ID=dt.C_DocType_ID) "
                    + "WHERE d.M_WorkOrderTransaction_ID=@recordid"                 //  info from PrintForm
                    + " AND pf.AD_Org_ID IN (0,d.AD_Org_ID) "
                    + "ORDER BY pf.AD_Client_ID DESC,  pf.AD_Org_ID DESC";
            else if (type == STANDARDOPERATION)
                sql = "SELECT pf.StdOperation_PrintFormat_ID, "
                    + " c.IsMultiLingualDocument"
                    + " FROM M_StandardOperation d"
                    + " INNER JOIN AD_Client c ON (d.AD_Client_ID=c.AD_Client_ID)"
                    + " INNER JOIN AD_PrintForm pf ON (d.AD_Client_ID=pf.AD_Client_ID OR pf.AD_Client_ID=0)"
                    + " INNER JOIN M_Operation op ON (d.M_Operation_ID=op.M_Operation_ID) "
                    + " WHERE d.M_StandardOperation_ID=@recordid" // info from PrintForm
                    + " AND pf.AD_Org_ID IN (0,d.AD_Org_ID) "
                    + "ORDER BY pf.AD_Client_ID DESC, pf.AD_Org_ID DESC";
            else if (type == ROUTING)
                sql = "SELECT pf.Routing_PrintFormat_ID, "
                    + " c.IsMultiLingualDocument"
                    + " FROM M_Routing d"
                    + " INNER JOIN AD_Client c ON (d.AD_Client_ID=c.AD_Client_ID)"
                    + " INNER JOIN AD_PrintForm pf ON (d.AD_Client_ID=pf.AD_Client_ID OR pf.AD_Client_ID=0)"
                    + " LEFT OUTER JOIN M_RoutingOperation ro ON (d.M_Routing_ID=ro.M_Routing_ID) "
                    + " WHERE d.M_Routing_ID=@recordid" // info from PrintForm
                    + " AND pf.AD_Org_ID IN (0,d.AD_Org_ID) "
                    + "ORDER BY pf.AD_Client_ID DESC, pf.AD_Org_ID DESC";
            else if (type == TASKLIST)
                sql = " SELECT dt.DocBaseType, pf.RPL_TList_PrintFormat_ID, " 			//1..2
                    + " pf.PUT_TList_PrintFormat_ID, pf.PCK_CluTList_PrintFormat_ID, "	//3..4
                    + " pf.PCK_OrdTList_PrintFormat_ID, M.PickMethod, "					//5..6
                    + " c.IsMultiLingualDocument, COALESCE(dt.DocumentCopies,0), "		//7..8
                    + " dt.AD_PrintFormat_ID"											//9
                    + " FROM M_TaskList M "
                    + " INNER JOIN AD_Client c ON (M.AD_Client_ID=c.AD_Client_ID)"
                    + " INNER JOIN AD_PrintForm pf ON (M.AD_Client_ID=pf.AD_Client_ID OR pf.AD_Client_ID=0)"
                    + " LEFT OUTER JOIN C_DocType dt ON (M.C_DocType_ID=dt.C_DocType_ID) "
                    + " WHERE M.M_TaskList_ID=@recordid"
                    + " AND pf.AD_Org_ID IN (0,M.AD_Org_ID) "
                    + " ORDER BY pf.AD_Client_ID DESC,  pf.AD_Org_ID DESC";
            /****************Manfacturing***********************/
            else	//	Get PrintFormat from Org or 0 of document client
            {
                sql = "SELECT pf.Order_PrintFormat_ID,pf.Shipment_PrintFormat_ID,"		//	1..2
                    //	Prio: 1. BPartner 2. DocType, 3. PrintFormat (Org)	//	see InvoicePrint
                    + " COALESCE (bp.Invoice_PrintFormat_ID,dt.AD_PrintFormat_ID,pf.Invoice_PrintFormat_ID)," // 3
                    + " pf.Project_PrintFormat_ID, pf.Remittance_PrintFormat_ID,"		//	4..5
                    + " c.IsMultiLingualDocument, bp.AD_Language,"						//	6..7
                    + " COALESCE(dt.DocumentCopies,0)+COALESCE(bp.DocumentCopies,1), " 	// 	8
                    + " dt.AD_PrintFormat_ID,bp.C_BPartner_ID,d.DocumentNo "			//	9..11
                    + "FROM " + DOC_BASETABLES[type] + " d"
                    + " INNER JOIN AD_Client c ON (d.AD_Client_ID=c.AD_Client_ID)"
                    + " INNER JOIN AD_PrintForm pf ON (c.AD_Client_ID=pf.AD_Client_ID)"
                    + " INNER JOIN C_BPartner bp ON (d.C_BPartner_ID=bp.C_BPartner_ID)"
                    + " LEFT OUTER JOIN C_DocType dt ON (d.C_DocType_ID=dt.C_DocType_ID) "
                    + "WHERE d." + DOC_IDS[type] + "=@recordid"			//	info from PrintForm
                    + " AND pf.AD_Org_ID IN (0,d.AD_Org_ID) "
                    + "ORDER BY pf.AD_Org_ID DESC";
            }
            //
            IDataReader dr=null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@recordid", Record_ID);
                dr = DataBase.DB.ExecuteReader(sql, param);
                if (dr.Read())
                {
                    if (type == CHECK || type == DUNNING || type == REMITTANCE
                        || type == PROJECT || type == RFQ)
                    {
                        AD_PrintFormat_ID = Utility.Util.GetValueOfInt(dr[0]);// rs.getInt(1);
                        copies = 1;
                        //	Set Language when enabled
                        String AD_Language = Utility.Util.GetValueOfString(dr[2]);// rs.getString(3);
                        if (AD_Language != null)// && "Y".equals(rs.getString(2)))	//	IsMultiLingualDocument
                            language = Language.GetLanguage(AD_Language);
                        C_BPartner_ID = Utility.Util.GetValueOfInt(dr[3]);// rs.getInt(4);
                        if (type == DUNNING)
                        {
                            DateTime? ts = Utility.Util.GetValueOfDateTime(dr[4]);// rs.getTimestamp(5);
                            DocumentNo = ts.ToString();
                        }
                        else
                            DocumentNo = Utility.Util.GetValueOfString(dr[4]);// rs.getString(5);
                    }
                    else if (type == MOVEMENT || type == INVENTORY)
                    {
                        AD_PrintFormat_ID = Utility.Util.GetValueOfInt(dr[0]);// rs.getInt(1);
                        copies = Utility.Util.GetValueOfInt(dr[2]);// rs.getInt(3);
                        if (copies == 0)
                            copies = 1;
                    }
                    /******************Manufacturing**************/
                    else if (type == WORKORDER || type == WORKORDERTXN)
                    {
                        int pfAD_PrintFormat_ID = Utility.Util.GetValueOfInt(dr[0]);
                        AD_PrintFormat_ID = Utility.Util.GetValueOfInt(dr[3]);
                        if (AD_PrintFormat_ID == 0)
                            AD_PrintFormat_ID = pfAD_PrintFormat_ID;

                        copies = Utility.Util.GetValueOfInt(dr[2]);
                        if (copies == 0)
                            copies = 1;
                        String AD_Language = Utility.Util.GetValueOfString(dr[1]);
                        if (AD_Language != null) // && "Y".equals(rs.getString(6)))	//	IsMultiLingualDocument
                            language = Language.GetLanguage(AD_Language);
                    }
                    else if (type == STANDARDOPERATION || type == ROUTING)
                    {
                        AD_PrintFormat_ID = Utility.Util.GetValueOfInt(dr[0]);
                        copies = 1;
                        /*String AD_Language = rs.getString(2);
                        if (AD_Language != null) // && "Y".equals(rs.getString(6))) // IsMultiLingualDocument
                        language = Language.getLanguage(AD_Language);*/
                    }
                    else if (type == TASKLIST)
                    {
                        String docBaseType = Utility.Util.GetValueOfString(dr[0]);
                        int replFormatID = Utility.Util.GetValueOfInt(dr[1]);
                        int putFormatID = Utility.Util.GetValueOfInt(dr[2]);
                        int cpickFormatID = Utility.Util.GetValueOfInt(dr[3]);
                        int opickFormatID = Utility.Util.GetValueOfInt(dr[4]);
                        String pmethod = Utility.Util.GetValueOfString(dr[5]);
                        copies = Utility.Util.GetValueOfInt(dr[7]);
                        if (copies == 0)
                            copies = 1;
                        AD_PrintFormat_ID = Utility.Util.GetValueOfInt(dr[8]);
                        if (AD_PrintFormat_ID == 0)
                        {
                            if (docBaseType.ToUpper().Equals("RPL"))
                            {
                                AD_PrintFormat_ID = replFormatID;
                            }
                            else if (docBaseType.ToUpper().Equals("PUT"))
                            {
                                AD_PrintFormat_ID = putFormatID;
                            }
                            else if (docBaseType.ToUpper().Equals("PCK"))
                            {
                                if (pmethod.ToUpper().Equals("C"))
                                {
                                    AD_PrintFormat_ID = cpickFormatID;
                                }
                                else
                                {
                                    AD_PrintFormat_ID = opickFormatID;
                                }
                            }
                        }

                    }
                    /******************Manufacturing**************/
                    else
                    {
                        //	Set PrintFormat
                        AD_PrintFormat_ID = Utility.Util.GetValueOfInt(dr[type]);// rs.getInt(type + 1);
                        if (Utility.Util.GetValueOfInt(dr[8].ToString()) != 0)		//	C_DocType.AD_PrintFormat_ID
                            AD_PrintFormat_ID = Utility.Util.GetValueOfInt(dr[8].ToString());// rs.getInt(9);
                        copies = Utility.Util.GetValueOfInt(dr[7].ToString());// rs.getInt(8);
                        //	Set Language when enabled
                        String AD_Language = Utility.Util.GetValueOfString(dr[6].ToString());// rs.getString(7);
                        if (AD_Language != null) // && "Y".equals(rs.getString(6)))	//	IsMultiLingualDocument
                            language = Language.GetLanguage(AD_Language);
                        C_BPartner_ID = Utility.Util.GetValueOfInt(dr[9]);// rs.getInt(10);
                        DocumentNo = Utility.Util.GetValueOfString(dr[10]);// rs.getString(11);
                    }
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Log(Level.SEVERE, "Record_ID=" + Record_ID + ", SQL=" + sql, e);
            }
            if (AD_PrintFormat_ID == 0)
            {
                log.Log(Level.SEVERE, "No PrintFormat found for Type=" + type + ", Record_ID=" + Record_ID);
                return null;
            }

            //	Get Format & Data
            MPrintFormat format = MPrintFormat.Get(ctx, AD_PrintFormat_ID, false);
            format.SetLanguage(language);		//	BP Language if Multi-Lingual
            //	if (!Env.isBaseLanguage(language, DOC_TABLES[type]))
            format.SetTranslationLanguage(language);


            /*   Set Culture according to BPartner Language */

            string lan = language.GetAD_Language().Replace('_', '-');
            // Set Report Language -VIS0228
            if (!string.IsNullOrEmpty(ctx.GetContext("Report_Lang")))
            {
                lan = ctx.GetContext("Report_Lang").Replace('_', '-');
            }

                System.Globalization.CultureInfo cInfo = new System.Globalization.CultureInfo(lan);

            

            System.Threading.Thread.CurrentThread.CurrentCulture = cInfo;
            System.Threading.Thread.CurrentThread.CurrentUICulture = cInfo;

            /*** END  *******/



            //	query
            Query query = new Query(DOC_TABLES[type]);
            query.AddRestriction(DOC_IDS[type], Query.EQUAL, Utility.Util.GetValueOfInt(Record_ID));
            //	log.config( "ReportCtrl.startDocumentPrint - " + format, query + " - " + language.getAD_Language());
            //
            if (DocumentNo == null || DocumentNo.Length == 0)
                DocumentNo = "DocPrint";
            PrintInfo info = new PrintInfo(
                DocumentNo,
                DOC_TABLE_ID[type],
                Record_ID,
                C_BPartner_ID);
            info.SetCopies(copies);
            info.SetDocumentCopy(false);		//	true prints "Copy" on second

            //	Engine
            ReportEngine_N re = new ReportEngine_N(ctx, format, query, info);

            //cInfo = new System.Globalization.CultureInfo("en-US");
            //System.Threading.Thread.CurrentThread.CurrentCulture = cInfo;
            //System.Threading.Thread.CurrentThread.CurrentUICulture = cInfo;

            return re;
        }


        public static void PrintConfirm(int type, int Record_ID)
        {
            StringBuilder sql = new StringBuilder();
            if (type == ORDER || type == SHIPMENT || type == INVOICE)
                sql.Append("UPDATE ").Append(DOC_BASETABLES[type])
                    .Append(" SET DatePrinted=SysDate, IsPrinted='Y' WHERE ")
                    .Append(DOC_IDS[type]).Append("=").Append(Record_ID);
            //
            if (sql.Length > 0)
            {
                int no = DataBase.DB.ExecuteQuery(sql.ToString(), null, null);
                if (no != 1)
                    log.Log(Level.SEVERE, "Updated records=" + no + " - should be just one");
            }
        }	//	printConfirm

        #region "Create CSV File"

        public bool CreateCSV(FileStream fs, char delimiter, Language language)
        {
            return CreateCSV(new StreamWriter(fs,Encoding.UTF8), delimiter, language);
        }

        public bool CreateCSV(StreamWriter writer, char delimiter, Language language)
        {
            if (delimiter == 0)
                delimiter = '\t';

            try
            {
                //Check if any of the ID has a child print format

                //changes made by Jagmohan Bhatt :- Date: 13-july-2010
                for (int col = 0; col < m_printFormat.GetItemCount(); col++)
                {
                    MPrintFormatItem item = m_printFormat.GetItem(col);
                    if (item.IsTypePrintFormat())
                    {
                        //isAnyHasChild = true;
                        int AD_Column_ID = item.GetAD_Column_ID();
                        m_printFormat = MPrintFormat.Get(GetCtx(), item.GetAD_PrintFormatChild_ID(), true);
                        Object obj = m_printData.GetNode(AD_Column_ID, false);

                        PrintDataElement dataElement = (PrintDataElement)obj;
                        String recordString = dataElement.GetValueKey();

                        Query query = new Query(m_printFormat.GetAD_Table_ID());
                        query.AddRestriction(item.GetColumnName(), Query.EQUAL, int.Parse(recordString));
                        m_printFormat.SetTranslationViewQuery(query);

                        SetPrintData(query);
                    }
                }
                //changes made by Jagmohan Bhatt :- Date: 13-july-2010
                StringBuilder sb = new StringBuilder();
                for (int row = -1; row < m_printData.GetRowCount(); row++)
                {
                   // StringBuilder sb = new StringBuilder();
                    if (row != -1)
                        m_printData.SetRowIndex(row);

                    //	for all columns
                    bool first = true;	//	first column to print
                    for (int col = 0; col < m_printFormat.GetItemCount(); col++)
                    {
                        MPrintFormatItem item = m_printFormat.GetItem(col);
                        if (item.IsPrinted())
                        {
                            if (first)
                                first = false;
                            else
                                sb.Append(delimiter);
                            if (row == -1)
                                CreateCSVvalue(sb, delimiter, m_printFormat.GetItem(col).GetPrintName(language));
                            else
                            {
                                Object obj = m_printData.GetNode(item.GetAD_Column_ID(), false);
                                String data = "";
                                if (obj == null)
                                {
                                }
                                else if (obj.GetType() == typeof(PrintDataElement))
                                {
                                    PrintDataElement pde = (PrintDataElement)obj;
                                    if (pde.IsPKey())
                                        data = pde.GetValueAsString();
                                    else
                                        data = pde.GetValueDisplay(language);	//	formatted
                                }
                                else if (obj.GetType() == typeof(PrintData))
                                {
                                }
                                else
                                {
                                }
                                CreateCSVvalue(sb, delimiter, data);
                            }
                        }
                    }
                    //writer.Write(sb.ToString());
                    //writer.Write(Env.NL);
                    sb.Append('\n');
                }
                writer.Write(sb.ToString());
                writer.Write(Env.NL);
                writer.Flush();
                writer.Close();
                return true;
            }
            catch (Exception e)
            {
                log.Severe("ReportEngine_N_CreateCSV_" + e.ToString());
            }

            return true;

        }

        /// <summary>
        /// Add Content to CSV string.
        /// Encapsulate/mask content in " if required
        /// </summary>
        /// <param name="sb">StringBuffer to add to</param>
        /// <param name="delimiter">delimiter</param>
        /// <param name="content">value</param>
        private void CreateCSVvalue(StringBuilder sb, char delimiter, String content)
        {
            //	nothing to add
            if (content == null || content.Length == 0)
                return;
            //
            bool needMask = false;
            StringBuilder buff = new StringBuilder();
            char[] chars = content.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                if (c == '"')
                {
                    needMask = true;
                    buff.Append(c);		//	repeat twice
                }	//	mask if any control character
                else if (!needMask && (c == delimiter || !Char.IsLetterOrDigit(c)))
                    needMask = true;
                buff.Append(c);
            }

            //	Optionally mask value
            if (needMask)
                sb.Append('"').Append(buff).Append('"');
            else
                sb.Append(buff);
        }	//	addCSVColumnValue
        #endregion

        #region "Create HTML File"
        public bool CreateHTML(FileStream fs, bool onlyTable, Language language)
        {
            CreateHTML(new StreamWriter(fs), onlyTable, language);
            return false;
        }

        /// <summary>
        /// Create the HTML Report
        /// </summary>
        /// <param name="writer">StreamWrite Object</param>
        /// <param name="onlyTable">does not in operation rightn now</param>
        /// <param name="language">current language</param>
        /// <returns>HTML Content</returns>
        public bool CreateHTML(StreamWriter writer, bool onlyTable, Language language)
        {
            MPrintTableFormat tf = m_printFormat.GetTableFormat();
            MPrintFont printFont = MPrintFont.Get(m_printFormat.GetAD_PrintFont_ID());
            tf.SetStandard_Font(printFont.GetFont());
            StringBuilder sb = new StringBuilder(@"<html><head><title>Report : " + m_printData.GetTableName() + "</title></head><body>");

            sb.Append(@"<table cellpadding='2' cellspacing='0' width='100%'>");
            for (int row = -1; row < m_printData.GetRowCount(); row++)
            {
                if (row != -1)
                    m_printData.SetRowIndex(row);

                int i = 0;
                for (int col = 0; col < m_printFormat.GetItemCount(); col++)
                {
                    MPrintFormatItem item = m_printFormat.GetItem(col);
                    if (item.GetAD_PrintFont_ID() != 0)
                    {
                        MPrintFont font = MPrintFont.Get(item.GetAD_PrintFont_ID());
                    }
                    if (item.IsPrinted())
                    {
                        i = i + 1;
                        if (row == -1)
                        {
                            if (i == 1)
                                sb.Append(CreateHeaderRow(tf, item, true));

                            sb.Append("<td" + CreateHeaderRow(tf, item, false) + ">");
                            sb.Append(Utility.Util.MaskHTML(item.GetPrintName(language)));
                            sb.Append("</td>");

                        }
                        else
                        {
                            if ((i == 1) && (row != -1))
                                sb.Append("</tr>").Append(CreateDataRow(tf, item, true));

                            sb.Append("<td" + CreateDataRow(tf, item, false) + ">");
                            Object obj = m_printData.GetNode(item.GetAD_Column_ID(), false);
                            if (obj == null)
                                sb.Append(@"&nbsp;");
                            else if (obj.GetType() == typeof(PrintDataElement))
                            {
                                String value = ((PrintDataElement)obj).GetValueDisplay(language);	//	formatted
                                sb.Append(Utility.Util.MaskHTML(value));
                            }
                            else if (obj.GetType() == typeof(PrintData))
                            {
                                //ignore contained data
                            }
                            sb.Append("</td>");
                        }
                    }
                }
                sb.Append("</tr>");
            }
            sb.Append("</table></body></html>");

            writer.Write(sb.ToString());
            writer.Flush();
            writer.Close();

            return true;
        }

        private string CreateHeaderRow(MPrintTableFormat tbf, MPrintFormatItem item, bool row)
        {
            StringBuilder sb = new StringBuilder("");
            if (row)
            {
                sb.Append("<tr");
                sb.Append(@" style='height:28px;background-color:" + System.Drawing.ColorTranslator.ToHtml(tbf.GetHeaderBG_Color()));
                sb.Append(";font-family:" + tbf.GetHeader_Font().Name);
                sb.Append(";color:" + System.Drawing.ColorTranslator.ToHtml(tbf.GetHeaderFG_Color()));
                sb.Append(";font-size:" + tbf.GetHeader_Font().Size);
                if (tbf.GetHeader_Font().Style == System.Drawing.FontStyle.Bold)
                    sb.Append(";font-weight:bold");
                if (tbf.GetHeader_Font().Style == System.Drawing.FontStyle.Italic)
                    sb.Append(";font-style:italic");

                sb.Append("'>");
            }
            else
            {
                if (tbf.IsPaintHeaderLines())
                {
                    sb.Append(" style='");
                    sb.Append("border-color:" + ColorTranslator.ToHtml(tbf.GetHeaderLine_Color()));
                    sb.Append(";border-width:" + tbf.GetHdrStroke() + "px");
                    sb.Append(";border-top-style:" + tbf.GetHeader_Stroke());
                    sb.Append(";border-bottom-style:" + tbf.GetHeader_Stroke());

                    sb.Append("'");
                }
            }
            //sb.Append(">");
            return sb.ToString();
        }


        private string CreateDataRow(MPrintTableFormat tbf, MPrintFormatItem item, bool row)
        {

            StringBuilder sb = new StringBuilder("");
            if (row)
            {
                sb.Append("<tr ");

                if (m_printData.IsFunctionRow())
                {
                    sb.Append(@" style='height:25px;background-color:" + System.Drawing.ColorTranslator.ToHtml(tbf.GetFunctBG_Color()));
                    sb.Append(";font-family:" + tbf.GetFunct_Font().Name);
                    sb.Append(";color:" + System.Drawing.ColorTranslator.ToHtml(tbf.GetFunctFG_Color()));
                    sb.Append(";font-size:" + tbf.GetFunct_Font().Size);
                    if (tbf.GetFunct_Font().Style == System.Drawing.FontStyle.Bold)
                        sb.Append(";font-weight:bold");
                    if (tbf.GetFunct_Font().Style == System.Drawing.FontStyle.Italic)
                        sb.Append(";font-style:italic");

                    sb.Append("'");
                }

                sb.Append(">");
            }
            else    //data rows
            {
                sb.Append(" style='");
                if (m_printData.IsFunctionRow())
                {
                    sb.Append("border-color:" + ColorTranslator.ToHtml(tbf.GetHeaderLine_Color()));
                    sb.Append(";border-width:" + tbf.GetHdrStroke() + "px");
                    sb.Append(";border-bottom-style:" + tbf.GetHeader_Stroke());

                }
                else
                {

                    sb.Append("font-family:" + tbf.GetStandard_Font().Name);
                    sb.Append(";font-size:" + tbf.GetStandard_Font().Size);
                    sb.Append(";width:" + item.GetMaxWidth() + "px");
                    if (tbf.GetStandard_Font().Style == FontStyle.Bold)
                        sb.Append(";font-weight:bold");
                    if (tbf.GetStandard_Font().Style == FontStyle.Italic)
                        sb.Append(";font-style:italic");

                }
                sb.Append("'");
            }
            return sb.ToString();
        }

        #endregion

        public string GetCsvReportFilePath(string data)
        {
            return null;
        }
        public string GetRtfReportFilePath(string data)
        {
            return null;
        }
        
    }
}
