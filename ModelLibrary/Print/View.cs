using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.Drawing.Design;
using System.Drawing.Text;
using System.Drawing;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Classes;
using VAdvantage.Common;
using System.Threading;
using VAdvantage.Utility;
using System.Windows.Forms;
using System.Reflection;

namespace VAdvantage.Print
{
    /// <summary>
    /// View Panel
    /// </summary>
    public class View : System.Windows.Forms.Panel
    {
        /// <summary>
        /// Print Preview
        /// </summary>
        /// <param name="layout"></param>
        public View(LayoutEngine layout)
        {
            printDoc.EndPrint += new PrintEventHandler(printDoc_EndPrint);
            printDoc.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDoc_PrintPage);
            printDoc.QueryPageSettings += new QueryPageSettingsEventHandler(this.printDoc_QueryPageSettings);
            printDoc.BeginPrint += new PrintEventHandler(printDoc_BeginPrint);

            this.DoubleBuffered = true;
            //SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            m_layout = layout;
        }

        /**	Layout to be Printed		*/
        private LayoutEngine m_layout;


        /**	Zoom Level						*/
        private int m_zoomLevel = 0;
        /** Zoom Options					*/
        public static String[] ZOOM_OPTIONS = new String[] { "100%", "75%", "50%" };
        /**	Margin around paper				*/
        public static int MARGIN = 5;
        /** Margin Background Color			*/
        private static Color COLOR_BACKGROUND = Color.LightGray;

        /**	Logger			*/
        private static VLogger log = VLogger.GetVLogger(typeof(View).FullName);

        /*************************************************************************/

        /// <summary>
        /// Minimum Size
        /// </summary>
        /// <returns>Max Page Size</returns>
        public java.awt.Dimension GetMinimumSize()
        {
            return GetMaximumSize();
        }	//	getMinimumSize

        /// <summary>
        /// Maximize Size
        /// </summary>
        /// <returns>Max Page Size</returns>
        public java.awt.Dimension GetMaximumSize()
        {
            return new java.awt.Dimension(GetPaperWidth() + (2 * MARGIN),
                (GetPaperHeight() + MARGIN) * GetPageCount() + MARGIN);
        }	//	getMaximumSize

        /// <summary>
        /// Preferred Size
        /// </summary>
        /// <returns>Max Page Size</returns>
        public java.awt.Dimension GetPreferredSize()
        {
            return GetMaximumSize();
        }	//	getPreferredSize

        protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        /// <summary>
        /// Paint the report
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
            PdfSharp.Drawing.XGraphics xg;

            Graphics g2D = e.Graphics;
            Bitmap drawing = null;
            drawing = new Bitmap(this.Width, this.Height, e.Graphics);
            g2D = Graphics.FromImage(drawing);
            g2D.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);
            RectangleF bounds = g2D.VisibleClipBounds;
            SolidBrush brush = new SolidBrush(COLOR_BACKGROUND);    //background color filling
            g2D.FillRectangle(brush, bounds.X, bounds.Y, bounds.Width, bounds.Height);
            for (int page = 0; page < m_layout.GetPages().Count(); page++)
            {
                Rectangle pageRectangle = GetRectangleOfPage(page + 1);
                if (bounds.IntersectsWith(pageRectangle))
                {
                    PdfSharp.Pdf.PdfPage pdfpage = document.AddPage();
                    pdfpage.Height = pageRectangle.Height;
                    pdfpage.Width = pageRectangle.Width;
                    xg = PdfSharp.Drawing.XGraphics.FromPdfPage(pdfpage);

                    Page p = (Page)m_layout.GetPages()[page];
                    p.PaintPdf(xg, pageRectangle, true, false);		//	sets context
                    m_layout.GetHeaderFooter().PaintPdf(xg, pageRectangle, true);
                    //p.Paint(g2D, pageRectangle, true, false);		//	sets context
                    //m_layout.GetHeaderFooter().Paint(g2D, pageRectangle, true);
                }
            }

            document.Save("D:\\Saved.pdf");
            document.Close();

            e.Graphics.DrawImageUnscaled(drawing, 0, 0);
            brush.Dispose();
            g2D.Dispose();
            this.AutoScrollMinSize = new System.Drawing.Size(this.GetPaperWidth(), this.GetPaperHeight() * m_layout.GetPages().Count() + (m_layout.GetPages().Count() * 5) + 10);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            //this.Invalidate();
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
        }
        //To handle Scroll of report************************************
        protected override void OnMouseEnter(EventArgs e)
        {
            //this.Focus();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            this.Focus();
            base.OnMouseClick(e);
        }


        protected override void OnMouseWheel(MouseEventArgs e)
        {
        }

        //To handle Scroll of report************************************

        /// <summary>
        /// Set Zoom Level
        /// </summary>
        /// <param name="level"></param>
        public void SetZoomLevel(int level)
        {
            m_zoomLevel = level;
        }	//	setZoomLevel

        /// <summary>
        /// Set Zoom Level
        /// </summary>
        /// <param name="levelString">zoom level string</param>
        public void SetZoomLevel(String levelString)
        {
            for (int i = 0; i < ZOOM_OPTIONS.Length; i++)
            {
                if (ZOOM_OPTIONS[i].Equals(levelString))
                {
                    m_zoomLevel = i;
                    break;
                }
            }
        }	//	setZoomLevel

        /// <summary>
        /// Get Zoom Level
        /// </summary>
        /// <returns>Zoom Level in Int</returns>
        public int GetZoomLevel()
        {
            return m_zoomLevel;
        }	//	getZoomLevel

        /// <summary>
        /// Get Rectange of Page
        /// </summary>
        /// <param name="pageNo">page no</param>
        /// <returns>rectangle</returns>
        public Rectangle GetRectangleOfPage(int pageNo)
        {
            int y = MARGIN + ((GetPaperHeight() + MARGIN) * (pageNo - 1));
            return new Rectangle(MARGIN, y, GetPaperWidth(), GetPaperHeight());
        }	//	getRectangleOfPage

        /// <summary>
        /// Get Page at Point
        /// </summary>
        /// <param name="p">Point</param>
        /// <returns>page as float to determine also position on page</returns>
        public float GetPageNoAt(Point p)
        {
            float y = p.Y;
            float pageHeight = GetPaperHeight() + MARGIN;
            return 1f + (y / pageHeight);
        }	//	getPageAt

        /// <summary>
        /// Counts the Pages
        /// </summary>
        /// <returns>Number of Pages in int</returns>
        public int GetPageCount()
        {
            return m_layout.GetPages().Count();
        }	//	getPageCount

        /// <summary>
        /// Get Page Info for Multi-Page tables
        /// </summary>
        /// <param name="pageNo">page</param>
        /// <returns>info e.g. (1,1)</returns>
        public String GetPageInfo(int pageNo)
        {
            return m_layout.GetPageInfo(pageNo);
        }	//	getPageInfo

        public String GetPageInfoMax()
        {
            return m_layout.GetPageInfoMax();
        }	//	getPageInfo

        public CPaper GetPaper()
        {
            return m_layout.GetPaper();
        }	//	getPaper

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

        public Query GetDrillDown(Point absolutePoint)
        {
            int pageNo = (int)GetPageNoAt(absolutePoint);
            Rectangle pageRectangle = GetRectangleOfPage(pageNo);
            Point relativePoint = new Point(absolutePoint.X - pageRectangle.X,
                absolutePoint.Y - pageRectangle.Y);
            Page page = (Page)m_layout.GetPages()[pageNo - 1];
            //
            log.Config("Relative=" + relativePoint + ", " + page);
            //	log.config("AbsolutePoint=" + absolutePoint + ", PageNo=" + pageNo + ", pageRectangle=" + pageRectangle);
            Query retValue = page.GetDrillDown(relativePoint);
            if (retValue == null)
                retValue = m_layout.GetHeaderFooter().GetDrillDown(relativePoint);
            return retValue;
        }	//	getDrillDown

        public Query GetDrillAcross(Point absolutePoint)
        {
            int pageNo = (int)GetPageNoAt(absolutePoint);
            Rectangle pageRectangle = GetRectangleOfPage(pageNo);
            Point relativePoint = new Point(absolutePoint.X - pageRectangle.X,
                absolutePoint.Y - pageRectangle.Y);
            Page page = (Page)m_layout.GetPages()[pageNo - 1];
            //
            log.Config("Relative=" + relativePoint + ", " + page);
            //	log.config("AbsolutePoint=" + absolutePoint + ", PageNo=" + pageNo + ", pageRectangle=" + pageRectangle);
            return page.GetDrillAcross(relativePoint);
        }	//	getDrillAcross

        public void RefreshMe()
        {
            this.Invalidate(true);
        }

        /* Printing the page *************/

        private System.Drawing.Printing.PrintDocument printDoc = new System.Drawing.Printing.PrintDocument();
        private System.Windows.Forms.PrintDialog printPreview = new System.Windows.Forms.PrintDialog();
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialog = new System.Windows.Forms.PrintPreviewDialog();

        private void printDoc_QueryPageSettings(object sender, QueryPageSettingsEventArgs e)
        {
            Rectangle rect = GetRectangleOfPage(m_PageNum);
            //e.PageSettings.PrintableArea = new RectangleF(MARGIN, y, GetPaperWidth(), GetPaperHeight());
            e.PageSettings.PaperSize = new PaperSize("Custom", rect.Width, rect.Height);

            //e.PageSettings.PrinterSettings.DefaultPageSettings.PrintableArea = rect;


        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, DeviceCapsIndex index);

        enum DeviceCapsIndex
        {
            PhysicalOffSetX = 112,
            PhysicalOffSetY = 113
        }

        private void printDoc_EndPrint(object sender, PrintEventArgs e)
        {
            m_PageNum = 0;
        }

        private Rectangle GetRealPageBound(PrintPageEventArgs e, bool preview)
        {
            if (preview)
                return e.PageBounds;

            int cx = 0;
            int cy = 0;

            IntPtr hdc = e.Graphics.GetHdc();
            try
            {
                cx = GetDeviceCaps(hdc, DeviceCapsIndex.PhysicalOffSetX);
                cy = GetDeviceCaps(hdc, DeviceCapsIndex.PhysicalOffSetY);
            }
            finally
            {
                e.Graphics.ReleaseHdc(hdc);
            }

            Rectangle marginBounds = e.MarginBounds;
            int dipX = (int)e.Graphics.DpiX;
            int dipY = (int)e.Graphics.DpiY;

            marginBounds.Offset(-cx * 100 / dipX, -cy * 100 / dipY);
            return marginBounds;

        }

        private void printDoc_BeginPrint(object sender, PrintEventArgs e)
        {
            m_PageNum = 0;
        }

        ReportEngine_N m_reportEngine = null;
        public void InitPrint(ReportEngine_N s_reportEngine)
        {
            try
            {
                //this.printDoc.DefaultPageSettings.Margins.Top = 1;
                //this.printDoc.DefaultPageSettings.Margins.Left = 1;
                //this.printDoc.DefaultPageSettings.Margins.Right = 1;
                //this.printDoc.DefaultPageSettings.Margins.Bottom = 1;
                m_reportEngine = s_reportEngine;
                m_reportEngine.CreatePDF(Application.StartupPath + "\\temp.pdf", true);
                //this.printDoc.Print();      //print the page
            }
            catch
            {
            }
        }

        public void InitPrintDialog()
        {
            //page setup
            this.printPreview.Document = this.printDoc;
            this.printPreview.AllowCurrentPage = true;
            this.printPreview.AllowSomePages = true;
            this.printPreview.UseEXDialog = true;
            if (printPreview.ShowDialog() == DialogResult.OK)
            {
                printDoc.Print();
            }
        }

        public void InitPrintPreview()
        {
            printDoc.EndPrint += new PrintEventHandler(printDoc_EndPrint);
            printDoc.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDoc_PrintPage);
            printDoc.QueryPageSettings += new QueryPageSettingsEventHandler(this.printDoc_QueryPageSettings);
            printDoc.BeginPrint += new PrintEventHandler(printDoc_BeginPrint);


            //this.printPreviewDialog.AutoScrollMargin = new System.Drawing.Size(0, 0);
            //this.printPreviewDialog.AutoScrollMinSize = new System.Drawing.Size(0, 0);

            //this.printPreviewDialog.ClientSize = new System.Drawing.Size(1400, 1300);
            this.printPreviewDialog.Document = this.printDoc;
            this.printPreviewDialog.Enabled = true;
            this.printPreviewDialog.Name = "printPreviewDialog1";
            (this.printPreviewDialog as Form).WindowState = FormWindowState.Maximized;
            (this.printPreviewDialog as Form).AutoScroll = true;
            //this.printDoc.Print();
            printPreviewDialog.ShowDialog();
        }

        int m_PageNum = 0;
        private void printDoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics g2D = e.Graphics;
            float dpiX = e.PageSettings.PrinterResolution.X;
            float dpiY = e.PageSettings.PrinterResolution.Y;


            MediaPrintableArea area = m_layout.GetPaper().GetMediaPrintableArea();
            RectangleF rcfFrame = new RectangleF(dpiX * e.MarginBounds.Left / 100.0f, dpiY * e.MarginBounds.Top / 100.0f,
            dpiX * e.MarginBounds.Width / 100.0f - 1,
            dpiY * e.MarginBounds.Height / 100.0f - 1);

            //if(e.Graphics.PageUnit != GraphicsUnit.Inch)
            //e.Graphics.PageUnit = GraphicsUnit.Pixel;

            //g2D.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);
            try
            {
                //RectangleF bounds = e.PageBounds;
                //Rectangle bound = GetRealPageBound(e, true);
                SolidBrush brush = new SolidBrush(COLOR_BACKGROUND);    //background color filling
                //for (int page = 0; page < m_layout.GetPages().Count(); page++)
                {
                    Rectangle pageRectangle = GetRectangleOfPage(m_PageNum + 1);
                    pageRectangle.Y = 5;
                    //if (bound.IntersectsWith(pageRectangle))
                    {
                        Page p = (Page)m_layout.GetPages()[m_PageNum];
                        p.Paint(g2D, pageRectangle, true, false);		//	sets context
                        m_layout.GetHeaderFooter().Paint(g2D, pageRectangle, true);
                        ++m_PageNum;
                        e.HasMorePages = (m_PageNum < m_layout.GetPages().Count());
                    }
                    //    //e.HasMorePages = true;
                }
                //e.HasMorePages = false;
                //e.Graphics.DrawImageUnscaled(drawing, 0, 0);
                brush.Dispose();
            }
            catch
            {
            }

            g2D.Dispose();

            //this.AutoScrollMinSize = new System.Drawing.Size(this.GetPaperWidth(), this.GetPaperHeight() * m_layout.GetPages().Count() + (m_layout.GetPages().Count() * 5) + 10);

        }
    }
}
