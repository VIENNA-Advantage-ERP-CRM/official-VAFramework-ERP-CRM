using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using VAdvantage.Utility;
using PdfSharp.Drawing;

namespace VAdvantage.Print
{
    public class BoxElement : PrintElement
    {
        public BoxElement(MPrintFormatItem item, Color color)
            : base()
        {
            if (item != null && item.IsTypeBox())
            {
                m_item = item;
                m_color = color;
                p_info = item.ToString();
            }
        }	//	BoxElement

        /** The Item					*/
        private MPrintFormatItem m_item = null;
        private Color m_color = Color.Black;

        protected override bool CalculateSize()
        {
            p_width = 0;
            p_height = 0;
            if (m_item == null)
                return true;
            return true;
        }	//	calculateSize

        public override void Paint(Graphics g2D, int pageNo, PointF pageStart, Ctx ctx, bool isView)
        {
            if (m_item == null)
                return;
            //
            System.Drawing.SolidBrush g2Dpen = new System.Drawing.SolidBrush(m_color);
            Pen pen = new Pen(g2Dpen);
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            //
            PointF location = GetAbsoluteLocation(pageStart);
            int x = (int)location.X;
            int y = (int)location.Y;

            int width = m_item.GetMaxWidth();
            int height = m_item.GetMaxHeight();

            if (m_item.GetPrintFormatType().Equals(MPrintFormatItem.PRINTFORMATTYPE_Line))
                g2D.DrawLine(pen, x, y, x + width, y + height);
            else
            {
                String type = m_item.GetShapeType();
                if (type == null)
                    type = "";
                if (m_item.IsFilledRectangle())
                {
                    if (type.Equals(MPrintFormatItem.SHAPETYPE_3DRectangle))
                        g2D.FillRectangle(g2Dpen, x, y, width, height);
                    else if (type.Equals(MPrintFormatItem.SHAPETYPE_Oval))
                        g2D.FillEllipse(g2Dpen, x, y, width, height);
                    else if (type.Equals(MPrintFormatItem.SHAPETYPE_RoundRectangle))
                    {
                        g2D.FillRectangle(g2Dpen, x, y, width, height); //, m_item.getArcDiameter(), m_item.getArcDiameter());
                    }
                    else
                        g2D.FillRectangle(g2Dpen, x, y, width, height);
                }
                else
                {
                    if (type.Equals(MPrintFormatItem.SHAPETYPE_3DRectangle))
                        g2D.DrawRectangle(pen, x, y, width, height);
                    else if (type.Equals(MPrintFormatItem.SHAPETYPE_Oval))
                        g2D.DrawEllipse(pen, x, y, width, height);
                    else if (type.Equals(MPrintFormatItem.SHAPETYPE_RoundRectangle))
                    {
                        g2D.DrawRectangle(pen, x, y, width, height); //, m_item.getArcDiameter(), m_item.getArcDiameter());
                    }
                    else
                        g2D.DrawRectangle(pen, x, y, width, height);

                    //PrintElement
                }
            }
        }


        public override void PaintPdf(PdfSharp.Drawing.XGraphics g2D, int pageNo, PointF pageStart, Ctx ctx, bool isView)
        {
            if (m_item == null)
                return;
            //
            //System.Drawing.SolidBrush g2Dpen = new System.Drawing.SolidBrush(m_color);
            XSolidBrush g2Dpen = new XSolidBrush(XColor.FromArgb(m_color));
            XPen pen = new XPen(XColor.FromArgb(m_color));
            pen.DashStyle = PdfSharp.Drawing.XDashStyle.Solid;
            //
            PointF location = GetAbsoluteLocation(pageStart);
            int x = (int)location.X;
            int y = (int)location.Y;

            int width = m_item.GetMaxWidth();
            int height = m_item.GetMaxHeight();

            if (m_item.GetPrintFormatType().Equals(MPrintFormatItem.PRINTFORMATTYPE_Line))
                g2D.DrawLine(pen, (double)x, (double)y, (double)x + width, (double)y + height);
            else
            {
                String type = m_item.GetShapeType();
                if (type == null)
                    type = "";
                if (m_item.IsFilledRectangle())
                {
                    if (type.Equals(MPrintFormatItem.SHAPETYPE_3DRectangle))
                        g2D.DrawRectangle(g2Dpen, (double)x, (double)y, (double)width, (double)height);
                    else if (type.Equals(MPrintFormatItem.SHAPETYPE_Oval))
                        g2D.DrawRectangle(g2Dpen, (double)x, (double)y, (double)width, (double)height);
                    else if (type.Equals(MPrintFormatItem.SHAPETYPE_RoundRectangle))
                    {
                        g2D.DrawRectangle(g2Dpen, (double)x, (double)y, (double)width, (double)height); //, m_item.getArcDiameter(), m_item.getArcDiameter());
                    }
                    else
                        g2D.DrawRectangle(g2Dpen, (double)x, (double)y, (double)width, (double)height);
                }
                else
                {
                    if (type.Equals(MPrintFormatItem.SHAPETYPE_3DRectangle))
                        g2D.DrawRectangle(pen, (double)x, (double)y, (double)width, (double)height);
                    else if (type.Equals(MPrintFormatItem.SHAPETYPE_Oval))
                        g2D.DrawEllipse(pen, (double)x, (double)y, (double)width, (double)height);
                    else if (type.Equals(MPrintFormatItem.SHAPETYPE_RoundRectangle))
                    {
                        g2D.DrawRectangle(pen, (double)x, (double)y, (double)width, (double)height); //, m_item.getArcDiameter(), m_item.getArcDiameter());
                    }
                    else
                        g2D.DrawRectangle(pen, (double)x, (double)y, (double)width, (double)height);

                    //PrintElement
                }
            }
        }

        public override VAdvantage.Classes.Query GetDrillDown(Point relativePoint, int pageNo)
        {
            return null;
        }

        public override VAdvantage.Classes.Query GetDrillAcross(Point relativePoint, int pageNo)
        {
            return null;
        }
    }
}
