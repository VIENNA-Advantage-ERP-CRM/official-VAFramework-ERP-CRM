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
    public class StringElement : PrintElement
    {
        /**	Actual Elements	- Viewer		*/
        private AttributedString[] m_string_view = null;
        /** Actual Elements - Printer		*/
        private AttributedString[] m_string_paper = null;
        /**	To be translated String			*/
        private String m_originalString = null;
        /** Font used						*/
        private Font m_font = null;
        /** Paint used						*/
        private Color m_paint;
        /**	Optional ID of String				*/
        private NamePair m_ID = null;
        /** Optional CheckBox					*/
        private Boolean? m_check = null;

        public static char DONE = '\uFFFF';

        private SolidBrush m_solidbrush;
        public override void Paint(Graphics g2D, int pageNo, PointF pageStart, Ctx ctx, bool isView)
        {
            m_solidbrush = new SolidBrush(Color.Black);
            PointF location = GetAbsoluteLocation(pageStart);
            //
            if (!string.IsNullOrEmpty(m_originalString))
                Translate(ctx);

            AttributedString aString = null;
            AttributedCharacterIterator iter = null;
            //	AttributedCharacterIterator iter2 = null;
            float xPos = (float)location.X;
            float yPos = (float)location.Y;
            float yPen = 0f;
            float height = 0f;
            float width = 0f;
            //	for all lines
            for (int i = 0; i < m_string_paper.Length; i++)
            {
                //	Get Text
                if (isView)
                {
                    if (m_string_view[i] == null)
                        continue;
                    aString = m_string_view[i];
                }
                else
                {
                    if (m_string_paper[i] == null)
                        continue;
                    aString = m_string_paper[i];
                }
                iter = aString.GetIterator();
                //	Zero Length
                if (iter.GetBeginIndex() == iter.GetEndIndex())
                    continue;


                //	Check for Tab (just first) and 16 bit characters
                int tabPos = -1;
                bool is8Bit = true;
                for (char c = iter.First(); c != DONE; c = iter.Next())
                {
                    if (c == '\t' && tabPos == -1)
                        tabPos = iter.GetIndex();
                    if (c > 255)
                        is8Bit = false;
                }

                TextLayout layout = null;
                float xPen = xPos;

                //	No Limit
                if (p_maxWidth == 0f)
                {
                    if (tabPos == -1)
                    {
                        layout = new TextLayout(iter);
                        yPen = yPos + layout.GetAscent();
                        //layout.Draw(g2D, xPen, yPen);
                        //m_font);
                        //g2D.setPaint(m_paint);
                        g2D.DrawString(iter.GetText(), m_font, m_solidbrush, xPen, yPen - 7);
                        //
                        yPos += layout.GetAscent() + layout.GetDescent() + layout.GetLeading();
                        if (width < layout.GetAdvance())
                            width = layout.GetAdvance();
                    }
                    else	//	we have a tab
                    {
                        LineBreakMeasurer measurer = new LineBreakMeasurer(iter);
                        layout = measurer.NextLayout(9999, tabPos);
                        float lineHeight_1 = layout.GetAscent() + layout.GetDescent() + layout.GetLeading();
                        yPen = yPos + layout.GetAscent();
                        g2D.DrawString(iter.GetText(), m_font, m_solidbrush, xPen, yPen - 7);		//	first part before tab
                        xPen = GetTabPos(xPos, layout.GetAdvance());
                        float lineWidth = xPen - xPos;
                        layout = measurer.NextLayout(9999);//, iter.getEndIndex(), true);
                        float lineHeight_2 = layout.GetAscent() + layout.GetDescent() + layout.GetLeading();
                        //layout.draw(g2D, xPen, yPen);		//	second part after tab
                        //
                        yPos += Math.Max(lineHeight_1, lineHeight_2);
                        lineWidth += layout.GetAdvance();
                        if (width < lineWidth)
                            width = lineWidth;
                    }
                    //	log.finest( "StringElement.paint - No Limit - " + location.x + "/" + yPos
                    //		+ " w=" + layout.getAdvance() + ",h=" + lineHeight + ", Bounds=" + layout.getBounds());
                }
                //	Size Limits
                else
                {
                    bool fastDraw = LayoutEngine.s_FASTDRAW;
                    if (fastDraw && !isView && !is8Bit)
                        fastDraw = false;
                    LineBreakMeasurer measurer = new LineBreakMeasurer(iter);
                    while (measurer.GetPosition() < iter.GetEndIndex())
                    {
                        if (tabPos == -1)
                        {
                            layout = measurer.NextLayout(p_maxWidth);
                            // use fastDraw if the string fits in one line
                            if (fastDraw && iter.GetEndIndex() != measurer.GetPosition())
                                fastDraw = false;
                        }
                        else	//	tab
                        {
                            fastDraw = false;
                            layout = measurer.NextLayout(p_maxWidth, tabPos);
                        }
                        //	Line Height
                        float lineHeight = layout.GetAscent() + layout.GetDescent() + layout.GetLeading();
                        if (p_maxHeight == -1f && i == 0)		//	one line only
                            p_maxHeight = lineHeight;
                        //	If we have hight left over
                        if (p_maxHeight == 0f || (height + lineHeight) <= p_maxHeight)
                        {
                            yPen = (float)location.Y + height + layout.GetAscent();
                            //	Tab in Text
                            if (tabPos != -1)
                            {
                                layout.Draw(g2D, xPen, yPen);	//	first part before tab
                                xPen = GetTabPos(xPos, layout.GetAdvance());
                                layout = measurer.NextLayout(p_width, iter.GetEndIndex());
                                tabPos = -1;	//	reset (just one tab)
                            }
                            else if ((X_AD_PrintFormatItem.FIELDALIGNMENTTYPE_TrailingRight.Equals(p_FieldAlignmentType) && layout.IsLeftToRight())
                                || (X_AD_PrintFormatItem.FIELDALIGNMENTTYPE_LeadingLeft.Equals(p_FieldAlignmentType)) && !layout.IsLeftToRight())
                                xPen += p_maxWidth - layout.GetAdvance();
                            else if (X_AD_PrintFormatItem.FIELDALIGNMENTTYPE_Center.Equals(p_FieldAlignmentType))
                                xPen += (p_maxWidth - layout.GetAdvance()) / 2;
                            else if (X_AD_PrintFormatItem.FIELDALIGNMENTTYPE_Block.Equals(p_FieldAlignmentType) && measurer.GetPosition() < iter.GetEndIndex())
                            {
                                //layout = layout.getJustifiedLayout(p_maxWidth);
                                fastDraw = false;
                            }
                            if (fastDraw)
                            {
                                //g2D.setFont(m_font);
                                //g2D.setPaint(m_paint);
                                g2D.DrawString(iter.GetText(), m_font, m_solidbrush, xPen, yPen - 7);
                            }
                            else
                            {
                                layout.Draw(g2D, xPen, yPen - 7);
                            }
                            height += lineHeight;
                            //	log.finest( "StringElement.paint - Limit - " + xPen + "/" + yPen
                            //		+ " w=" + layout.getAdvance() + ",h=" + lineHeight + ", Align=" + p_FieldAlignmentType + ", Max w=" + p_maxWidth + ",h=" + p_maxHeight + ", Bounds=" + layout.getBounds());
                        }
                    }
                    width = p_maxWidth;
                }	//	size limits
            }	//	for all strings
            if (m_check != null)
            {
                int x = (int)(location.X + width + 1);
                int y = (int)(location.Y);
                g2D.DrawImage((bool)m_check ? LayoutEngine.IMAGE_TRUE : LayoutEngine.IMAGE_FALSE, x, y);
            }
        }

        private float GetTabPos(float xPos, float length)
        {
            float retValue = xPos + length;
            int iLength = (int)Math.Ceiling(length);
            int tabSpace = iLength % 30;
            retValue += (30 - tabSpace);
            return retValue;
        }	//	getTabPos

        protected override bool CalculateSize()
        {
            try
            {
                if (p_sizeCalculated)
                    return true;
                //
                //FontRenderContext frc = new FontRenderContext(null, true, true);
                TextLayout layout = null;
                p_height = 0f;
                p_width = 0f;

                //	No Limit
                if (p_maxWidth == 0f && p_maxHeight == 0f)
                {
                    foreach (AttributedString element in m_string_paper)
                    {
                        AttributedCharacterIterator iter = element.GetIterator();
                        if (iter.GetBeginIndex() == iter.GetEndIndex())
                            continue;

                        //	Check for Tab (just first)
                        int tabPos = -1;
                        for (char c = iter.First(); c != DONE && tabPos == -1; c = iter.Next())
                        {
                            if (c == '\t')
                                tabPos = iter.GetIndex();
                        }

                        if (tabPos == -1)
                        {
                            layout = new TextLayout(iter);
                            p_height += layout.GetAscent() + layout.GetDescent() + layout.GetLeading();
                            if (p_width < layout.GetAdvance())
                                p_width = layout.GetAdvance();
                        }
                        else	//	with tab
                        {
                            LineBreakMeasurer measurer = new LineBreakMeasurer(iter);
                            layout = measurer.NextLayout(9999, tabPos);
                            p_height += layout.GetAscent() + layout.GetDescent() + layout.GetLeading();
                            float width = GetTabPos(0, layout.GetAdvance());
                            layout = measurer.NextLayout(9999, iter.GetEndIndex());
                            width += layout.GetAdvance();
                            if (p_width < width)
                                p_width = width;
                        }
                    }	//	 for all strings

                    //	Add CheckBox Size
                    if (m_check != null)
                    {
                        p_width += LayoutEngine.IMAGE_SIZE.width;
                        if (p_height < LayoutEngine.IMAGE_SIZE.height)
                            p_height = LayoutEngine.IMAGE_SIZE.height;
                    }
                }
                //	Size Limits
                else
                {
                    p_width = p_maxWidth;
                    for (int i = 0; i < m_string_paper.Length; i++)
                    {
                        AttributedCharacterIterator iter = m_string_paper[i].GetIterator();
                        if (iter.GetBeginIndex() == iter.GetEndIndex())
                            continue;

                        LineBreakMeasurer measurer = new LineBreakMeasurer(iter);
                        //	System.out.println("StringLength=" + m_originalString.length() + " MaxWidth=" + p_maxWidth + " MaxHeight=" + p_maxHeight);
                        while (measurer.GetPosition() < iter.GetEndIndex())
                        {
                            //	no need to expand tab space for limited space
                            layout = measurer.NextLayout(p_maxWidth);
                            float lineHeight = layout.GetAscent() + layout.GetDescent() + layout.GetLeading();
                            //	System.out.println("  LineWidth=" + layout.getAdvance() + "  LineHeight=" + lineHeight);
                            if (p_maxHeight == -1f && i == 0)		//	one line only
                                p_maxHeight = lineHeight;
                            if (p_maxHeight == 0f || (p_height + lineHeight) <= p_maxHeight)
                                p_height += lineHeight;
                        }
                    }	//	 for all strings

                    //	Add CheckBox Size
                    if (m_check != null)
                    {
                        //	p_width += LayoutEngine.IMAGE_SIZE.width;
                        if (p_height < LayoutEngine.IMAGE_SIZE.height)
                            p_height = LayoutEngine.IMAGE_SIZE.height;
                    }
                    //	System.out.println("  Width=" + p_width + "  Height=" + p_height);
                }
                //	System.out.println("StringElement.calculate size - Width="
                //		+ p_width + "(" + p_maxWidth + ") - Height=" + p_height + "(" + p_maxHeight + ")");

                //	Enlarge Size when aligned and max size is given
                if (p_FieldAlignmentType != null)
                {
                    bool changed = false;
                    if (p_height < p_maxHeight)
                    {
                        p_height = p_maxHeight;
                        changed = true;
                    }
                    if (p_width < p_maxWidth)
                    {
                        p_width = p_maxWidth;
                        changed = true;
                    }
                    if (changed)
                        log.Finest("Width=" + p_width + "(" + p_maxWidth + ") - Height="
                            + p_height + "(" + p_maxHeight + ")");
                }
            }
            catch
            {
            }
            return true;
        }	//	calculateSize
        //throw new NotImplementedException();

        public StringElement(String inText, Font font, Color paint, NamePair ID, bool translateText)
            : base()
        {

            log.Finest("Text=" + inText + ", ID=" + ID + ", Translate=" + translateText);
            p_info = inText;
            m_font = font;
            m_paint = paint;
            if (translateText)
            {
                int count = Utility.Util.GetCount(inText, '@');

                if (count > 0 && count % 2 == 0)
                {
                    m_originalString = inText;
                    //	Translate it to get rough space (not correct context) = may be too small
                    inText = Msg.ParseTranslation(Env.GetCtx(), m_originalString);
                }
            }
            m_ID = ID;
           // String[] lines = java.util.regex.Pattern.compile("$", java.util.regex.Pattern.MULTILINE).split(inText);
            String[] lines = System.Text.RegularExpressions.Regex.Split(inText, "$", System.Text.RegularExpressions.RegexOptions.Multiline);
                            
            m_string_paper = new AttributedString[lines.Length];
            m_string_view = new AttributedString[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                {
                    continue;
                }
                String line = Utility.Util.RemoveCRLF(lines[i]);
                m_string_paper[i] = new AttributedString(line);
                if (line.Length == 0)
                    continue;
                log.Finest(" - line=" + i + " - " + line);
                m_string_paper[i].AddAttribute(TextAttribute.FONT, font);
                m_string_paper[i].AddAttribute(TextAttribute.FOREGROUND, paint);
                if (m_ID != null && i == 0)		//	first line only - create special Attributed String
                {
                    m_string_view[i] = new AttributedString(line);
                    m_string_view[i].AddAttribute(TextAttribute.FONT, font);
                    int endIndex = line.Length;
                    m_string_view[i].AddAttribute(TextAttribute.FOREGROUND, LINK_COLOR);
                    m_string_view[i].AddAttribute(TextAttribute.UNDERLINE, TextAttribute.UNDERLINE_LOW_ONE_PIXEL, 0, endIndex);
                }
                else
                    m_string_view[i] = m_string_paper[i];
            }
            //	Load Image
            WaitForLoad(LayoutEngine.IMAGE_TRUE);
            WaitForLoad(LayoutEngine.IMAGE_FALSE);
        }


        public StringElement(AttributedString str)
        {
            m_string_paper = new AttributedString[] { str };
            m_string_view = m_string_paper;
        }	//	StringElement 


        public StringElement(Object content, Font font, Color paint, NamePair ID, String label, String labelSuffix)
        {
            log.Finest("Label=" + label + "|" + labelSuffix
                    + ", Content=" + content + ", ID=" + ID);
            m_font = font;
            m_paint = paint;
            int startIndex = 0;
            int endOffset = 0;

            StringBuilder text = new StringBuilder();
            //text.Append("\u202D");

            if (label != null && label.Length > 0 && (content != null && content.ToString() !=""))
            {
                text.Append(label).Append(" ");
                startIndex = label.Length + 1;
            }
            
            if (content is Boolean)
                m_check = (Boolean)content;

            else if (content is DateTime)
            {
                if (content.ToString().Length > 10)
                {
                    text.Append(content.ToString().Substring(0, 10));
                }
                else
                {
                    text.Append(content.ToString());
                }
            }
            else
            {
                text.Append(content);
            }

            if (labelSuffix != null && labelSuffix.Length > 0)
            {
                text.Append(labelSuffix);
                endOffset = labelSuffix.Length;
            }
            //text.Append("\u202C");
            m_ID = ID;
            //String[] lines = java.util.regex.Pattern.compile("$", java.util.regex.Pattern.MULTILINE).split(text.ToString());
            String[] lines = System.Text.RegularExpressions.Regex.Split(text.ToString(), "$", System.Text.RegularExpressions.RegexOptions.Multiline);
            m_string_paper = new AttributedString[lines.Length];
            m_string_view = new AttributedString[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                {
                    continue;
                }
                String line = Utility.Util.RemoveCRLF(lines[i]);
                m_string_paper[i] = new AttributedString(line);
                if (line.Length == 0)
                    continue;
                log.Finest(" - line=" + i + " - " + line);
                m_string_paper[i].AddAttribute(TextAttribute.FONT, font);
                m_string_paper[i].AddAttribute(TextAttribute.FOREGROUND, paint);
                if (m_ID != null && i == 0)		//	first line only - create special Attributed String
                {
                    m_string_view[i] = new AttributedString(line);
                    m_string_view[i].AddAttribute(TextAttribute.FONT, font);
                    m_string_view[i].AddAttribute(TextAttribute.FOREGROUND, paint);
                    int endIndex = line.Length - endOffset;
                    if (endIndex > startIndex)
                    {
                        m_string_view[i].AddAttribute(TextAttribute.FOREGROUND, LINK_COLOR, startIndex, endIndex);
                        m_string_view[i].AddAttribute(TextAttribute.UNDERLINE, TextAttribute.UNDERLINE_LOW_ONE_PIXEL, startIndex, endIndex);
                    }
                }
                else
                    m_string_view[i] = m_string_paper[i];
            }
        }	//	StringElement

        public NamePair GetID()
        {
            return m_ID;
        }	//	getID

        public String GetOriginalString()
        {
            return m_originalString;
        }	//	getOrig

        public new void Translate(Ctx ctx)
        {
            if (m_originalString == null)
                return;
            String inText = Msg.ParseTranslation(ctx, m_originalString);
            //	log.fine( "StringElement.translate", inText);
            //String[] lines = java.util.regex.Pattern.compile("$", java.util.regex.Pattern.MULTILINE).split(inText);
            String[] lines = System.Text.RegularExpressions.Regex.Split(inText, "$", System.Text.RegularExpressions.RegexOptions.Multiline);
            m_string_paper = new AttributedString[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                {
                    continue;
                }
                String line = Utility.Util.RemoveCRLF(lines[i]);
                m_string_paper[i] = new AttributedString(line);
                if (line.Length > 0)
                {
                    m_string_paper[i].AddAttribute(TextAttribute.FONT, m_font);
                    m_string_paper[i].AddAttribute(TextAttribute.FOREGROUND, m_paint);
                }
            }
            m_string_view = m_string_paper;
        }	//	translate


        public override void PaintPdf(XGraphics g2D, int pageNo, PointF pageStart, Ctx ctx, bool isView)
        {
            m_solidbrush = new SolidBrush(Color.Black);
            PointF location = GetAbsoluteLocation(pageStart);
            //
            if (m_originalString != null)
                Translate(ctx);

            AttributedString aString = null;
            AttributedCharacterIterator iter = null;
            //	AttributedCharacterIterator iter2 = null;
            float xPos = (float)location.X;
            float yPos = (float)location.Y;
            float yPen = 0f;
            float height = 0f;
            float width = 0f;
            //	for all lines
            for (int i = 0; i < m_string_paper.Length; i++)
            {
                //	Get Text
                if (isView)
                {
                    if (m_string_view[i] == null)
                        continue;
                    aString = m_string_view[i];
                }
                else
                {
                    if (m_string_paper[i] == null)
                        continue;
                    aString = m_string_paper[i];
                }
                iter = aString.GetIterator();
                //	Zero Length
                if (iter.GetBeginIndex() == iter.GetEndIndex())
                    continue;


                //	Check for Tab (just first) and 16 bit characters
                int tabPos = -1;
                bool is8Bit = true;
                for (char c = iter.First(); c != DONE; c = iter.Next())
                {
                    if (c == '\t' && tabPos == -1)
                        tabPos = iter.GetIndex();
                    if (c > 255)
                        is8Bit = false;
                }

                TextLayout layout = null;
                float xPen = xPos;

                //	No Limit
                if (p_maxWidth == 0f)
                {
                    if (tabPos == -1)
                    {
                        layout = new TextLayout(iter);
                        yPen = yPos + layout.GetAscent();
                        //	layout.draw(g2D, xPen, yPen);
                        //m_font);
                        //g2D.setPaint(m_paint);
                        //XFont d = new XFont(m_font, new XPdfFontOptions(PdfSharp.Pdf.PdfFontEncoding.Unicode, PdfSharp.Pdf.PdfFontEmbedding.Automatic));

                        //StringBuilder txt = new StringBuilder();
                        //txt.Append("\u202D")
                        //.Append(iter.GetText())
                        //.Append("\u202C");
                        //g2D.DrawString(

                        XPdfFontOptions options = new XPdfFontOptions(PdfSharp.Pdf.PdfFontEncoding.Unicode, PdfSharp.Pdf.PdfFontEmbedding.Always);

                        PdfSharp.Drawing.Layout.XTextFormatter tf = new PdfSharp.Drawing.Layout.XTextFormatter(g2D);
                        
                        //XStringFormat sf = new XStringFormat();
                        //sf.Alignment = XStringAlignment.Near;
                        
                        //tf.Alignment = PdfSharp.Drawing.Layout.XParagraphAlignment.Left;

                        //tf.DrawString(iter.GetText(), new XFont(m_font, options), new XSolidBrush(XColors.Black), (double)xPen, (double)yPen);
                        g2D.DrawString(iter.GetText(), new XFont(m_font, options), new XSolidBrush(XColors.Black), (double)xPen, (double)yPen, new XStringFormat() { Alignment = XStringAlignment.Near });
                        //
                        yPos += layout.GetAscent() + layout.GetDescent() + layout.GetLeading();
                        if (width < layout.GetAdvance())
                            width = layout.GetAdvance();
                    }
                    else	//	we have a tab
                    {
                        LineBreakMeasurer measurer = new LineBreakMeasurer(iter);
                        layout = measurer.NextLayout(9999, tabPos);
                        float lineHeight_1 = layout.GetAscent() + layout.GetDescent() + layout.GetLeading();
                        yPen = yPos + layout.GetAscent();
                        g2D.DrawString(iter.GetText(), new XFont(m_font, new XPdfFontOptions(PdfSharp.Pdf.PdfFontEncoding.Unicode,PdfSharp.Pdf.PdfFontEmbedding.Always)), new XSolidBrush(XColors.Black), (double)xPen, (double)yPen);		//	first part before tab
                        xPen = GetTabPos(xPos, layout.GetAdvance());
                        float lineWidth = xPen - xPos;
                        layout = measurer.NextLayout(9999);//, iter.getEndIndex(), true);
                        float lineHeight_2 = layout.GetAscent() + layout.GetDescent() + layout.GetLeading();
                        //layout.draw(g2D, xPen, yPen);		//	second part after tab
                        //
                        yPos += Math.Max(lineHeight_1, lineHeight_2);
                        lineWidth += layout.GetAdvance();
                        if (width < lineWidth)
                            width = lineWidth;
                    }
                    //	log.finest( "StringElement.paint - No Limit - " + location.x + "/" + yPos
                    //		+ " w=" + layout.getAdvance() + ",h=" + lineHeight + ", Bounds=" + layout.getBounds());
                }
                //	Size Limits
                else
                {
                    bool fastDraw = LayoutEngine.s_FASTDRAW;
                    if (fastDraw && !isView && !is8Bit)
                        fastDraw = false;
                    LineBreakMeasurer measurer = new LineBreakMeasurer(iter);
                    while (measurer.GetPosition() < iter.GetEndIndex())
                    {
                        if (tabPos == -1)
                        {
                            layout = measurer.NextLayout(p_maxWidth);
                            // use fastDraw if the string fits in one line
                            if (fastDraw && iter.GetEndIndex() != measurer.GetPosition())
                                fastDraw = false;
                        }
                        else	//	tab
                        {
                            fastDraw = false;
                            layout = measurer.NextLayout(p_maxWidth, tabPos);
                        }
                        //	Line Height
                        float lineHeight = layout.GetAscent() + layout.GetDescent() + layout.GetLeading();
                        if (p_maxHeight == -1f && i == 0)		//	one line only
                            p_maxHeight = lineHeight;
                        //	If we have hight left over
                        if (p_maxHeight == 0f || (height + lineHeight) <= p_maxHeight)
                        {
                            yPen = (float)location.Y + height + layout.GetAscent();
                            //	Tab in Text
                            if (tabPos != -1)
                            {
                                layout.Draw(g2D, (double)xPen, (double)yPen);	//	first part before tab
                                xPen = GetTabPos(xPos, layout.GetAdvance());
                                layout = measurer.NextLayout(p_width, iter.GetEndIndex());
                                tabPos = -1;	//	reset (just one tab)
                            }
                            else if ((X_AD_PrintFormatItem.FIELDALIGNMENTTYPE_TrailingRight.Equals(p_FieldAlignmentType) && layout.IsLeftToRight())
                                || (X_AD_PrintFormatItem.FIELDALIGNMENTTYPE_LeadingLeft.Equals(p_FieldAlignmentType)) && !layout.IsLeftToRight())
                                xPen += p_maxWidth - layout.GetAdvance();
                            else if (X_AD_PrintFormatItem.FIELDALIGNMENTTYPE_Center.Equals(p_FieldAlignmentType))
                                xPen += (p_maxWidth - layout.GetAdvance()) / 2;
                            else if (X_AD_PrintFormatItem.FIELDALIGNMENTTYPE_Block.Equals(p_FieldAlignmentType) && measurer.GetPosition() < iter.GetEndIndex())
                            {
                                //layout = layout.getJustifiedLayout(p_maxWidth);
                                fastDraw = false;
                            }
                            if (fastDraw)
                            {
                                //g2D.setFont(m_font);
                                //g2D.setPaint(m_paint);
                                g2D.DrawString(iter.GetText(), new XFont(m_font, new XPdfFontOptions(PdfSharp.Pdf.PdfFontEncoding.Unicode,PdfSharp.Pdf.PdfFontEmbedding.Always)), new XSolidBrush(XColors.Black), (double)xPen, (double)yPen - 7);
                            }
                            else
                            {
                                layout.Draw(g2D, (double)xPen, (double)yPen);
                            }
                            height += lineHeight;
                            //	log.finest( "StringElement.paint - Limit - " + xPen + "/" + yPen
                            //		+ " w=" + layout.getAdvance() + ",h=" + lineHeight + ", Align=" + p_FieldAlignmentType + ", Max w=" + p_maxWidth + ",h=" + p_maxHeight + ", Bounds=" + layout.getBounds());
                        }
                    }
                    width = p_maxWidth;
                }	//	size limits
            }	//	for all strings
            if (m_check != null)
            {
                int x = (int)(location.X + width + 1);
                int y = (int)(location.Y);
                //g2D.DrawImage((bool)m_check ? LayoutEngine.IMAGE_TRUE : LayoutEngine.IMAGE_FALSE, x, y);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("StringElement[");
            sb.Append("Bounds=").Append(GetBounds())
                .Append(",Height=").Append(p_height).Append("(").Append(p_maxHeight)
                .Append("),Width=").Append(p_width).Append("(").Append(p_maxHeight)
                .Append("),PageLocation=").Append(p_pageLocation).Append(" - ");
            for (int i = 0; i < m_string_paper.Length; i++)
            {
                if (m_string_paper.Length > 1)
                    sb.Append(Env.NL).Append(i).Append(":");
                AttributedCharacterIterator iter = m_string_paper[i].GetIterator();
                for (char c = iter.First(); c != DONE; c = iter.Next())
                    sb.Append(c);
            }
            if (m_ID != null)
                sb.Append(",ID=(").Append(m_ID.ToStringX()).Append(")");
            sb.Append("]");
            return sb.ToString();
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
