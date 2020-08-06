using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;

namespace VAdvantage.Print
{
    public class HTMLElement : PrintElement
    {
        public HTMLElement(string elem)
        {
        }

        public override void Paint(System.Drawing.Graphics g2D, int pageNo, System.Drawing.PointF pageStart, Ctx ctx, bool isView)
        {
            throw new NotImplementedException();
        }

        public override void PaintPdf(PdfSharp.Drawing.XGraphics g2D, int pageNo, System.Drawing.PointF pageStart, Ctx ctx, bool isView)
        {
            throw new NotImplementedException();
        }

        protected override bool CalculateSize()
        {
            throw new NotImplementedException();
        }

        public static bool IsHTML(Object content)
        {
            if (content == null)
                return false;
            String s = content.ToString();
            if (s.Length < 20)	//	assumption
                return false;
            s = s.Trim().ToUpper();
            if (s.StartsWith("<HTML>"))
                return true;
            return false;
        }	//	isHTML

        public override VAdvantage.Classes.Query GetDrillDown(System.Drawing.Point relativePoint, int pageNo)
        {
            return null;
        }

        public override VAdvantage.Classes.Query GetDrillAcross(System.Drawing.Point relativePoint, int pageNo)
        {
            return null;
        }
    }
}
