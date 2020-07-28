using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using VAdvantage.Classes;
using VAdvantage.Utility;

namespace VAdvantage.Print
{
    public class BarcodeElement : PrintElement
    {
        public BarcodeElement(String code, MPrintFormatItem item)
            : base()
        {
            if (code == null || code.Length == 0
                || item == null
                || item.GetBarcodeType() == null || item.GetBarcodeType().Length == 0)
                m_valid = false;
            m_barcode = new BarcodeGenerator(item);
            if (m_barcode == null)
                m_valid = false;
        }	//	BarcodeElement

        /**	Valid					*/
        private bool m_valid = true;
        /**	 Barcode				*/
        private BarcodeGenerator m_barcode = null;

        private void CreateBarcode(String code, MPrintFormatItem item)
        {
            String type = item.GetBarcodeType();
            try
            {
                if (type.Equals(MPrintFormatItem.BARCODETYPE_Codabar2Of7Linear))
                { }
                else if (type.Equals(MPrintFormatItem.BARCODETYPE_CodabarMonarchLinear))
                { }
                else if (type.Equals(MPrintFormatItem.BARCODETYPE_CodabarNW_7Linear))
                { }
                else if (type.Equals(MPrintFormatItem.BARCODETYPE_CodabarUSD_4Linear))
                { }
                else if (type.Equals(MPrintFormatItem.BARCODETYPE_Code128ACharacterSet))
                { }
                else if (type.Equals(MPrintFormatItem.BARCODETYPE_Code128BCharacterSet))
                { }
                else if (type.Equals(MPrintFormatItem.BARCODETYPE_Code128CCharacterSet))
                { }
                else if (type.Equals(MPrintFormatItem.BARCODETYPE_Code128DynamicallySwitching))
                { }
                else if (type.Equals(MPrintFormatItem.BARCODETYPE_Code393Of9LinearWithChecksum))
                { }
                else if (type.Equals(MPrintFormatItem.BARCODETYPE_Code393Of9LinearWOChecksum))
                { }
                else if (type.Equals(MPrintFormatItem.BARCODETYPE_Code39LinearWithChecksum))
                { }
                else if (type.Equals(MPrintFormatItem.BARCODETYPE_Code39LinearWOChecksum))
                { }
                else if (type.Equals(MPrintFormatItem.BARCODETYPE_Code39USD3WithChecksum))
                { }
                else if (type.Equals(MPrintFormatItem.BARCODETYPE_Code39USD3WOChecksum))
                { }
                else if (type.Equals(MPrintFormatItem.BARCODETYPE_CodeabarLinear))
                { }

                //	http://www.idautomation.com/code128faq.html
                else if (type.Equals(MPrintFormatItem.BARCODETYPE_EAN128))
                { }
                else if (type.Equals(MPrintFormatItem.BARCODETYPE_GlobalTradeItemNoGTINUCCEAN128))
                { }
                else if (type.Equals(MPrintFormatItem.BARCODETYPE_PDF417TwoDimensional))
                { }
                else if (type.Equals(MPrintFormatItem.BARCODETYPE_SCC_14ShippingCodeUCCEAN128))
                { }
                else if (type.Equals(MPrintFormatItem.BARCODETYPE_ShipmentIDNumberUCCEAN128))
                { }
                else if (type.Equals(MPrintFormatItem.BARCODETYPE_SSCC_18NumberUCCEAN128))
                { }
                else if (type.Equals(MPrintFormatItem.BARCODETYPE_UCC128))
                { }

                //	http://www.usps.com/cpim/ftp/pubs/pub97/97apxs_006.html#_Toc481397331
                else if (type.Equals(MPrintFormatItem.BARCODETYPE_USPostalServiceUCCEAN128))
                {
                    //m_barcode = BarcodeFactory.createUSPS(code);
                    //m_barcode.setDrawingText(false);
                }
                else
                    log.Warning("Invalid Type" + type);
            }
            catch (Exception e)
            {
                log.Warning(code + " - " + e.ToString());
                m_valid = false;
            }

            //if (m_valid && m_barcode != null)
            //{
            //    p_info = "BarCodeType=" + type;
            //    if (item.GetAD_PrintFont_ID() != 0)
            //    {
            //        MPrintFont mFont = MPrintFont.Get(item.GetAD_PrintFont_ID());
            //        if (mFont != null)
            //        {
            //            m_barcode.FontName = mFont.GetFont().Name;
            //            //m_barcode.FontName = mFont.GetFont().Size;
            //        }
            //    }
            //    if (item.GetMaxWidth() > 0)
            //        m_barcode.setBarWidth(item.getMaxWidth());
            //    if (item.getMaxHeight() > 0)
            //        m_barcode.setBarHeight(item.getMaxHeight());
            //    //	m_barcode.setResolution(72);
            //    //
            //    p_width = m_barcode.getWidth();
            //    p_height = m_barcode.getHeight();
            //    log.fine(type + " height=" + p_height + ", width=" + p_width);
            //}
        }	//	createBarcode

        public BarcodeGenerator GetBarcode()
        {
            return m_barcode;
        }	//	getBarcode

        public bool IsValid()
        {
            return m_valid;
        }	//	isValid

        protected override bool CalculateSize()
        {
            return true;
        }	//	calculateSize


        public override void Paint(Graphics g2D, int pageNo, PointF pageStart, Ctx ctx, bool isView)
        {
            if (!m_valid || m_barcode == null)
                return;

            //	Position
            PointF location = GetAbsoluteLocation(pageStart);
            int x = (int)location.X;
            if (MPrintFormatItem.FIELDALIGNMENTTYPE_TrailingRight.Equals(p_FieldAlignmentType))
                x += (int)p_maxWidth - (int)p_width;
            else if (MPrintFormatItem.FIELDALIGNMENTTYPE_Center.Equals(p_FieldAlignmentType))
                x += ((int)p_maxWidth - (int)p_width) / 2;
            int y = (int)location.Y;

            //m_barcode.draw(g2D, x, y);
        }	//	paint

        public override void PaintPdf(PdfSharp.Drawing.XGraphics g2D, int pageNo, PointF pageStart, Ctx ctx, bool isView)
        {
            if (!m_valid || m_barcode == null)
                return;

            //	Position
            PointF location = GetAbsoluteLocation(pageStart);
            int x = (int)location.X;
            if (MPrintFormatItem.FIELDALIGNMENTTYPE_TrailingRight.Equals(p_FieldAlignmentType))
                x += (int)p_maxWidth - (int)p_width;
            else if (MPrintFormatItem.FIELDALIGNMENTTYPE_Center.Equals(p_FieldAlignmentType))
                x += ((int)p_maxWidth - (int)p_width) / 2;
            int y = (int)location.Y;

            //m_barcode.draw(g2D, x, y);
        }	//	paint

        public override String ToString()
        {
            if (m_barcode == null)
                return base.ToString();
            return base.ToString() + " ";
        }	//	toString

        public override Query GetDrillDown(Point relativePoint, int pageNo)
        {
            return null;
        }

        public override Query GetDrillAcross(Point relativePoint, int pageNo)
        {
            return null;
        }
    }
}
