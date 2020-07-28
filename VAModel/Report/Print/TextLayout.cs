using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace VAdvantage.Print
{
    public class TextLayout
    {
        private float advance;
        private float ascent;
        private float descent;
        private float leading;
        private float height;
        private string fullText;
        private string trimText;

        private FontFamily layout;
        private Font font;
        private Color stringColor;

        public TextLayout()
        {

        }

        public TextLayout(string sb, int start, int limit, Font stringFont, Color _stringColor)
        {
            font = stringFont;
            stringColor = _stringColor;
            layout = font.FontFamily;
            fullText = sb;
            if(fullText.Length > start)
                trimText = sb.Substring(start, limit - start);
      

            GetGraphicLayout(sb, font);
        }

        public static char DONE = '\uFFFF';

        public TextLayout(AttributedCharacterIterator text)
        {
            fullText = text.GetText();
            trimText = fullText;
            font = text.GetFont();
            layout = font.FontFamily;
            stringColor = text.GetColor();

            int start = text.GetBeginIndex();
            int limit = text.GetEndIndex();
            if (start == limit)
            {
                throw new ArgumentException("Zero length iterator passed to TextLayout constructor.");
            }

            int len = limit - start;
            text.First();
            char[] chars = new char[len];
            int n = 0;
            for (char c = text.First(); c != DONE; c = text.Next())
            {
                chars[n++] = c;
            }
            GetGraphicLayout(fullText, text.GetFont());
        }

        public TextLayout(string stringData, Font font)
        {
            GetGraphicLayout(stringData, font);
        }

        private void GetGraphicLayout(string stringData, Font font)
        {
            SizeF size = TextRenderer.MeasureText(stringData, font);
            advance = size.Width;

            //FontFamily layout = new FontFamily(System.Drawing.Text.GenericFontFamilies.SansSerif);
            float lineSpace = font.GetHeight(96);
            int cellSpace = font.FontFamily.GetLineSpacing(font.Style);
            int cellAscent = font.FontFamily.GetCellAscent(font.Style);
            int cellDescent = font.FontFamily.GetCellDescent(font.Style);
            int cellLeading = cellSpace - cellAscent - cellDescent;

            // Get effective ascent
            ascent = lineSpace * cellAscent / cellSpace;
            // Get effective descent
            descent = lineSpace * cellDescent / cellSpace;
            // Get effective leading
            if (cellLeading > 0)
                leading = lineSpace * cellLeading / cellSpace;
            //leading = lineSpacingInDU * (font.Size / emSizeInDU);
            height = size.Height + leading;
        }

        public void Draw(Graphics g2, float x, float y)
        {
            if (g2 == null)
            {
                throw new ArgumentException("Null Graphics2D passed to TextLayout.draw()");
            }
            g2.DrawString(trimText, GetFont(), new SolidBrush(stringColor), x, y);
        }


        public void Draw(PdfSharp.Drawing.XGraphics g2, double x, double y)
        {
            if (g2 == null)
            {
                throw new ArgumentException("Null Graphics2D passed to TextLayout.draw()");
            }
            PdfSharp.Drawing.XFont xfont = new PdfSharp.Drawing.XFont(GetFont(), new PdfSharp.Drawing.XPdfFontOptions(PdfSharp.Pdf.PdfFontEncoding.Unicode, PdfSharp.Pdf.PdfFontEmbedding.Always));

            g2.DrawString(trimText, new PdfSharp.Drawing.XFont(GetFont(), new PdfSharp.Drawing.XPdfFontOptions(PdfSharp.Pdf.PdfFontEncoding.Unicode, PdfSharp.Pdf.PdfFontEmbedding.Always)), new PdfSharp.Drawing.XSolidBrush(PdfSharp.Drawing.XColor.FromArgb(stringColor)), x, y);
        }
        public Font GetFont()
        {
            return font;
        }

        public float GetAscent()
        {
            return ascent;
        }

        public float GetDescent()
        {
            return descent;
        }

        public float GetLeading()
        {
            return leading;
        }

        public float GetAdvance()
        {
            return advance;
        }

        public float GetHeight()
        {
            return height;
        }

        public bool IsLeftToRight()
        {
            return true;
        }
    }
}
