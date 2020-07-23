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
using PdfSharp.Drawing;

namespace VAdvantage.Print
{
    public abstract class GridElement : PrintElement
    {
        public GridElement(int rows, int cols)
        {
            m_rows = rows;
            m_cols = cols;
            m_textLayout = new TextLayout[rows, cols];
            m_iterator = new AttributedCharacterIterator[rows, cols];
            m_rowHeight = new int[rows];
            m_colWidth = new int[cols];
            //	explicit init
            for (int r = 0; r < m_rows; r++)
            {
                m_rowHeight[r] = 0;
                for (int c = 0; c < m_cols; c++)
                {
                    m_textLayout[r, c] = null;
                    m_iterator[r, c] = null;
                }
            }
            for (int c = 0; c < m_cols; c++)
                m_colWidth[c] = 0;
            p_info = "Grid:R=" + rows + ",C=" + cols;
        }	//	GridElement

        
        /**	Gap between Rows		*/
        private int m_rowGap = 3;
        /**	Gap between Columns		*/
        private int m_colGap = 5;

        /** Rows				*/
        private int m_rows;
        /**	Columns				*/
        private int m_cols;
        /**	The Layout Data			*/
        private TextLayout[,] m_textLayout = null;
        /** Character Iterator		*/
        private AttributedCharacterIterator[,] m_iterator = null;

        /**	Row Height			*/
        private int[] m_rowHeight = null;
        /**	Column Width		*/
        private int[] m_colWidth = null;




        public void SetGap(int rowGap, int colGap)
        {
            m_rowGap = rowGap;
            m_colGap = colGap;
        }	//	setGap

        protected override bool CalculateSize()
        {
            p_height = 0;
            for (int r = 0; r < m_rows; r++)
            {
                p_height += m_rowHeight[r];
                if (m_rowHeight[r] > 0)
                    p_height += m_rowGap;
            }
            p_height -= m_rowGap;	//	remove last
            p_width = 0;
            for (int c = 0; c < m_cols; c++)
            {
                p_width += m_colWidth[c];
                if (m_colWidth[c] > 0)
                    p_width += m_colGap;
            }
            p_width -= m_colGap;		//	remove last
            return true;
        }	//	calculateSize


        public override void Paint(Graphics g2D, int pageNo, PointF pageStart, Ctx ctx, bool isView)
        {
            PointF location = GetAbsoluteLocation(pageStart);
            float y = (float)location.Y;
            //
            for (int row = 0; row < m_rows; row++)
            {
                float x = (float)location.X;
                for (int col = 0; col < m_cols; col++)
                {
                    if (m_textLayout[row, col] != null)
                    {
                        float yy = y + m_textLayout[row, col].GetAscent();
                        //	if (m_iterator[row][col] != null)
                        //		g2D.drawString(m_iterator[row][col], x, yy);
                        //	else
                        m_textLayout[row, col].Draw(g2D, x, yy - 7);
                        //g2D.DrawString(m_textLayout[row, col].ToString(), x, yy);
                    }
                    x += m_colWidth[col];
                    if (m_colWidth[col] > 0)
                        x += m_colGap;
                }
                y += m_rowHeight[row];
                if (m_rowHeight[row] > 0)
                    y += m_rowGap;
            }
        }

        private void SetData(int row, int col, TextLayout layout, AttributedCharacterIterator iter)
        {
            if (layout == null)
                return;
            if (p_sizeCalculated)
                throw new Exception("Size already calculated");
            if (row < 0 || row >= m_rows)
                throw new IndexOutOfRangeException("Row Index=" + row + " Rows=" + m_rows);
            if (col < 0 || col >= m_cols)
                throw new IndexOutOfRangeException("Column Index=" + col + " Cols=" + m_cols);
            //
            m_textLayout[row, col] = layout;
            m_iterator[row, col] = iter;
            //	Set Size
            int height = layout.GetFont().Height;
            int width = (int)layout.GetAdvance() + 1;
            if (m_rowHeight[row] < height)
                m_rowHeight[row] = height;
            if (m_colWidth[col] < width)
                m_colWidth[col] = width;
        }	//	setData

        public void SetData(int row, int col, String stringData, Font font, Color foreground)
        {
            if (string.IsNullOrEmpty(stringData))
                return;

            TextLayout layout;
            AttributedString aString = new AttributedString(stringData);
            aString.AddAttribute(TextAttribute.FONT, font);
            aString.AddAttribute(TextAttribute.FOREGROUND, foreground);
            AttributedCharacterIterator iter = aString.GetIterator();

            //HashMap<TextAttribute, object> map = new HashMap<TextAttribute, object>();
            //map.Put(TextAttribute.FONT, font);
            //map.Put(TextAttribute.FOREGROUND, foreground);
            layout = new TextLayout(iter);
            SetData(row, col, layout, iter);
        }

        public override void PaintPdf(XGraphics g2D, int pageNo, PointF pageStart, Ctx ctx, bool isView)
        {
            PointF location = GetAbsoluteLocation(pageStart);
            float y = (float)location.Y;
            //
            for (int row = 0; row < m_rows; row++)
            {
                float x = (float)location.X;
                for (int col = 0; col < m_cols; col++)
                {
                    if (m_textLayout[row, col] != null)
                    {
                        float yy = y + m_textLayout[row, col].GetAscent();
                        //	if (m_iterator[row][col] != null)
                        //		g2D.drawString(m_iterator[row][col], x, yy);
                        //	else
                        m_textLayout[row, col].Draw(g2D, (double)x, (double)yy);
                        //g2D.DrawString(m_textLayout[row, col].ToString(), x, yy);
                    }
                    x += m_colWidth[col];
                    if (m_colWidth[col] > 0)
                        x += m_colGap;
                }
                y += m_rowHeight[row];
                if (m_rowHeight[row] > 0)
                    y += m_rowGap;
            }
        }

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
