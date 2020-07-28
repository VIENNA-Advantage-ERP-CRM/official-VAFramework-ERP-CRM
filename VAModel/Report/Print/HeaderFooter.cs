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

namespace VAdvantage.Print
{
    public class HeaderFooter
    {
        public HeaderFooter(Ctx ctx)
        {
            m_ctx = ctx;
        }	//	HeaderFooter

        /**	Ctx						*/
        private Ctx m_ctx;

        /**	Header/Footer content			*/
        private List<PrintElement> m_elements = new List<PrintElement>();
        /** Header/Footer content as Array	*/
        private PrintElement[] m_pe = null;

        public void AddElement(PrintElement element)
        {
            if (element != null)
                m_elements.Add(element);
            m_pe = null;
        }	//	addElement

        public PrintElement[] GetElements()
        {
            if (m_pe == null)
            {
                m_pe = new PrintElement[m_elements.Count()];
                m_pe = m_elements.ToArray();
            }
            return m_pe;
        }	//	getElements

        public void Paint(Graphics g2D, Rectangle bounds, bool isView)
        {
            Point pageStart = new Point(bounds.X, bounds.Y);
            GetElements();
            for (int i = 0; i < m_pe.Length; i++)
                m_pe[i].Paint(g2D, 0, pageStart, m_ctx, isView);
        }	//	paint

        public void PaintPdf(PdfSharp.Drawing.XGraphics g2D, Rectangle bounds, bool isView)
        {
            Point pageStart = new Point(bounds.X, bounds.Y);
            GetElements();
            for (int i = 0; i < m_pe.Length; i++)
                m_pe[i].PaintPdf(g2D, 0, pageStart, m_ctx, isView);
        }	//	paint


        public Query GetDrillDown(Point relativePoint)
        {
            Query retValue = null;
            for (int i = 0; i < m_elements.Count() && retValue == null; i++)
            {
                PrintElement element = (PrintElement)m_elements[i];
                retValue = element.GetDrillDown(relativePoint, 1);
            }
            return retValue;
        }	//	getDrillDown
    }
}
