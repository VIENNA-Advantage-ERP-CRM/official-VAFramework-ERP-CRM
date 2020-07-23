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
using PdfSharp.Drawing;

namespace VAdvantage.Print
{
    public class Page
    {
        public Page(Ctx ctx, int pageNo)
        {
            m_ctx = ctx;
            m_pageNo = pageNo;
            if (m_pageInfo == null || m_pageInfo.Length == 0)
                m_pageInfo = m_pageNo.ToString();
        }	//	Page

        /**	Current Page No	(set here)				*/
        public static String CONTEXT_PAGE = "*Page";
        /** Page Count (set in Layout Engine		*/
        public static String CONTEXT_PAGECOUNT = "*PageCount";
        /** Multi Page Info (set here)				*/
        public static String CONTEXT_MULTIPAGE = "*MultiPageInfo";
        /** Copy Info (set here)				*/
        public static String CONTEXT_COPY = "*CopyInfo";

        /** Report Name (set in Layout Engine)		*/
        public static String CONTEXT_REPORTNAME = "*ReportName";
        /** Report Header (set in Layout Engine)	*/
        public static String CONTEXT_HEADER = "*Header";
        /**	Current Date (set in Layout Engine)		*/
        public static String CONTEXT_DATE = "*CurrentDate";
        /**	Current Time (set in Layout Engine)		*/
        public static String CONTEXT_TIME = "*CurrentDateTime";

        /**	Page Number					*/
        private int m_pageNo;
        /**	Page Number					*/
        private int m_pageCount = 1;
        /** Page Count					*/
        private String m_pageInfo;
        /**	Context						*/
        private Ctx m_ctx;
        /** Page content				*/
        private List<PrintElement> m_elements = new List<PrintElement>();

        /// <summary>
        /// Get Page Number
        /// </summary>
        /// <returns>Page Number</returns>
        public int GetPageNo()
        {
            return m_pageNo;
        }	//	getPageNo

        /// <summary>
        /// Get Page Info
        /// </summary>
        /// <returns>Page Info</returns>
        public String GetPageInfo()
        {
            return m_pageInfo;
        }	//	getPageInfo

        /// <summary>
        /// Set Page Info.
        /// Enhanced pagae no, e.g.,  7(2,3)
        /// </summary>
        /// <param name="pageInfo">page info</param>
        public void SetPageInfo(String pageInfo)
        {
            if (m_pageInfo == null || m_pageInfo.Length == 0)
                m_pageInfo = m_pageNo.ToString();
            m_pageInfo = pageInfo;
        }	//	getPageInfo

        /// <summary>
        /// Set Page Count
        /// </summary>
        /// <param name="pageCount">Page count</param>
        public void SetPageCount(int pageCount)
        {
            m_pageCount = pageCount;
        }	//	setPageCount

        /// <summary>
        /// Add Print element to page
        /// </summary>
        /// <param name="element">print element</param>
        public void AddElement(PrintElement element)
        {
            if (element != null)
                m_elements.Add(element);
        }	//	addElement

        /// <summary>
        /// Paint Page on Graphics in Bounds
        /// </summary>
        /// <param name="g2D">graphics</param>
        /// <param name="bounds">page bounds</param>
        /// <param name="isView">true if online view (IDs are links)</param>
        /// <param name="isCopy">this print is a copy</param>
        public void Paint(Graphics g2D, Rectangle bounds, bool isView, bool isCopy)
        {
            m_ctx.Put(CONTEXT_PAGE, m_pageInfo);
            //	log.finest( "PrintContext", CONTEXT_PAGE + "=" + m_pageInfo);
            //
            StringBuilder sb = new StringBuilder();
            if (m_pageCount != 1)		//	set to "Page 1 of 2"
                sb.Append(Msg.GetMessageText(m_ctx, "Page")).Append(" ")
                    .Append(m_pageNo)
                    .Append(" ").Append(Msg.GetMessageText(m_ctx,"of")).Append(" ")
                    .Append(m_pageCount);
            sb.Append(" ");
            m_ctx.Put(CONTEXT_MULTIPAGE, sb.ToString());
            //	log.finest( "PrintContext", CONTEXT_MULTIPAGE + "=" + sb.toString());
            //
            sb = new StringBuilder();
            if (isCopy)					//	set to "(Copy)"
                sb.Append("(")
                    .Append(Msg.GetMessageText(m_ctx, "DocumentCopy"))
                    .Append(")");
            sb.Append(" ");
            m_ctx.Put(CONTEXT_COPY, sb.ToString());
            //	log.finest( "PrintContext copy=" + isCopy, CONTEXT_COPY + "=" + sb.toString());

            //	Paint Background
            // Create solid brush.
            SolidBrush whiteBrush = new SolidBrush(Color.White);

            g2D.FillRectangle(whiteBrush, bounds.X, bounds.Y, bounds.Width, bounds.Height);
            //
            Point pageStart = new Point(bounds.X, bounds.Y);
            for (int i = 0; i < m_elements.Count(); i++)
            {
                //
                PrintElement e = m_elements[i];
                e.Paint(g2D, m_pageNo, pageStart, m_ctx, isView);
            }
        }	//	paint

        public void PaintPdf(XGraphics g2D, Rectangle bounds, bool isView, bool isCopy)
        {
            m_ctx.Put(CONTEXT_PAGE, m_pageInfo);
            //	log.finest( "PrintContext", CONTEXT_PAGE + "=" + m_pageInfo);
            //
            StringBuilder sb = new StringBuilder();
            if (m_pageCount != 1)		//	set to "Page 1 of 2"
                sb.Append(Msg.GetMsg(m_ctx, "Page")).Append(" ")
                    .Append(m_pageNo)
                    .Append(" ").Append(Msg.GetMsg(m_ctx, "of")).Append(" ")
                    .Append(m_pageCount);
            sb.Append(" ");
            m_ctx.Put(CONTEXT_MULTIPAGE, sb.ToString());
            //	log.finest( "PrintContext", CONTEXT_MULTIPAGE + "=" + sb.toString());
            //
            sb = new StringBuilder();
            if (isCopy)					//	set to "(Copy)"
                sb.Append("(")
                    .Append( Msg.GetMsg(m_ctx, "DocumentCopy"))
                    .Append(")");
            sb.Append(" ");
            m_ctx.Put(CONTEXT_COPY, sb.ToString());
            //	log.finest( "PrintContext copy=" + isCopy, CONTEXT_COPY + "=" + sb.toString());

            //	Paint Background
            // Create solid brush.
            //XSolidBrush whiteBrush = new XSolidBrush(XColors.White);

            //g2D.DrawRectangle(whiteBrush, bounds.X, bounds.Y, bounds.Width, bounds.Height);
            //
            Point pageStart = new Point(bounds.X, bounds.Y);
            for (int i = 0; i < m_elements.Count(); i++)
            {
                //
                PrintElement e = m_elements[i];
                e.PaintPdf(g2D, m_pageNo, pageStart, m_ctx, isView);
            }
        }	//	paint

        /// <summary>
        /// Get DrillDown value
        /// </summary>
        /// <param name="relativePoint">relative Point</param>
        /// <returns>if found NamePait or null</returns>
        public Query GetDrillDown(Point relativePoint)
        {
            Query retValue = null;
            for (int i = 0; i < m_elements.Count() && retValue == null; i++)
            {
                PrintElement element = m_elements[i];
                retValue = element.GetDrillDown(relativePoint, m_pageNo);
            }
            return retValue;
        }	//	getDrillDown

        /// <summary>
        /// Get DrillAcross value
        /// </summary>
        /// <param name="relativePoint">relative Point</param>
        /// <returns>if found Query or null</returns>
        public Query GetDrillAcross(Point relativePoint)
        {
            Query retValue = null;
            for (int i = 0; i < m_elements.Count() && retValue == null; i++)
            {
                PrintElement element = m_elements[i];
                retValue = element.GetDrillAcross(relativePoint, m_pageNo);
            }
            return retValue;
        }	//	getDrillAcross

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>Info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("Page[");
            sb.Append(m_pageNo).Append(",Elements=").Append(m_elements.Count());
            sb.Append("]");
            return sb.ToString();
        }	//	toString

        public int GetPageCount()
        {
            return m_pageCount;
        }
    }
}
