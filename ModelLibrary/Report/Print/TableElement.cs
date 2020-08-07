using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Model;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Logging;
using System.Drawing;
using System.Drawing.Printing;
using PdfSharp.Drawing;

namespace VAdvantage.Print
{
    /// <summary>
    /// Table Print Element.
    /// Maintains a logical cross page table, which is "broken up" when printing
    /// <para>
    /// The table is 3 pages wide, 2 pages high
    ///<para>	+-----+-----+-----+</para>
    ///<para>	| 1.1 | 1.2 | 1.3 |</para>
    ///<para>	+-----+-----+-----+</para>
    ///<para>	| 2.1 | 2.2 | 2.3 |</para>
    ///<para>	+-----+-----+-----+</para>
    ///<para>	Printed</para>
    ///<para>	+-----+-----+-----+</para>
    ///<para>	|  1  |  2  |  3  |</para>
    ///<para>	+-----+-----+-----+</para>
    ///<para>	|  4  |  5  |  6  |</para>
    ///<para>	+-----+-----+-----+</para>
    /// </para>
    /// </summary>
    public class TableElement : PrintElement
    {
        public TableElement(ValueNamePair[] columnHeader,
    int[] columnMaxWidth, int[] columnMaxHeight, String[] columnJustification,
    bool[] fixedWidth, List<int> functionRows, bool multiLineHeader,
    Object[,] data, KeyNamePair[] pk, String pkColumnName,
    int pageNoStart, Rectangle firstPage, Rectangle nextPages, int repeatedColumns, HashMap<int, int> additionalLines,
    HashMap<Point, Font> rowColFont, HashMap<Point, Color> rowColColor, HashMap<Point, Color> rowColBackground,
    MPrintTableFormat tFormat, List<int> pageBreak)
            : base()
        {
            p_info = "Table:R=" + data.Length + ",C=" + columnHeader.Length;
            log.Fine(p_info);
            m_columnHeader = columnHeader;
            m_columnMaxWidth = columnMaxWidth;
            m_columnMaxHeight = columnMaxHeight;
            m_columnJustification = columnJustification;
            m_functionRows = functionRows;
            m_fixedWidth = fixedWidth;
            //
            m_multiLineHeader = multiLineHeader;
            m_data = data;
            m_pk = pk;
            m_pkColumnName = pkColumnName;
            //
            m_pageNoStart = pageNoStart;
            m_firstPage = firstPage;
            m_nextPages = nextPages;
            m_repeatedColumns = repeatedColumns;
            m_additionalLines = additionalLines;
            //	Used Fonts,Colots
            Point pAll = new Point(ALL, ALL);
            m_rowColFont = rowColFont;
            m_baseFont = (Font)m_rowColFont.Get(pAll);
            if (m_baseFont == null)
                m_baseFont = new Font(FontFamily.GenericSansSerif, 9f, FontStyle.Regular, GraphicsUnit.World);
            m_rowColColor = rowColColor;
            m_baseColor = (Color)m_rowColColor[pAll];
            if (m_baseColor.IsEmpty)
                m_baseColor = Color.Black;
            m_rowColBackground = rowColBackground;
            m_baseBackground = (Color)m_rowColBackground.Get(pAll);
            if (m_baseBackground.IsEmpty)
                m_baseBackground = Color.White;
            m_tFormat = tFormat;

            //	Page Break - not two after each other
            m_pageBreak = pageBreak;
            for (int i = 0; i < m_pageBreak.Count(); i++)
            {
                int row = (int)m_pageBreak[i];
                while ((i + 1) < m_pageBreak.Count())
                {
                    int nextRow = (int)m_pageBreak[i + 1];
                    if ((row + 1) == nextRow)
                    {
                        log.Fine("- removing PageBreak row=" + row);
                        m_pageBreak.Remove(i);
                        row = nextRow;
                    }
                    else
                        break;
                }
            }	//	for all page breaks

            //	Load Image
            WaitForLoad(LayoutEngine.IMAGE_TRUE);
            WaitForLoad(LayoutEngine.IMAGE_FALSE);
        }

        /**	Column Headers				*/
        private ValueNamePair[] m_columnHeader;
        /** Max column widths			*/
        private int[] m_columnMaxWidth;
        /** Max row height per column	*/
        private int[] m_columnMaxHeight;
        /** Field Justification for Column	*/
        private String[] m_columnJustification;
        /** True if column fixed length		*/
        private bool[] m_fixedWidth;
        /** Create multiple header lines if required	*/
        private bool m_multiLineHeader;
        /** List of Function Rows			*/
        private List<int> m_functionRows;
        /** The Data					*/
        private Object[,] m_data;
        /** Primary Keys				*/
        private KeyNamePair[] m_pk;
        /** Primary Key Column Name		*/
        private String m_pkColumnName;
        /** Starting page Number		*/
        private int m_pageNoStart;
        /** Bounds of first Page		*/
        private Rectangle m_firstPage;
        /** Bounds of next Pages		*/
        private Rectangle m_nextPages;

        /** repeat first x columns on - X Axis follow pages	*/
        private int m_repeatedColumns;

        /** base font for table			*/
        private Font m_baseFont;
        /** Dictionary with Point as key with Font overwrite	*/
        private HashMap<Point, Font> m_rowColFont;
        /** base foreground color for table		*/
        private Color m_baseColor;
        /** Dictionary with Point as key with foreground Color overwrite	*/
        private HashMap<Point, Color> m_rowColColor;
        /** base color for table		*/
        private Color m_baseBackground;
        /** Dictionary with Point as key with background Color overwrite	*/
        private HashMap<Point, Color> m_rowColBackground;
        /**	Format of Table				*/
        private MPrintTableFormat m_tFormat;
        /**	Page Break Rows				*/
        private List<int> m_pageBreak;


        /** width of columns (float)		*/
        private List<float> m_columnWidths = new List<float>();
        /** height of rows (float)			*/
        private List<float> m_rowHeights = new List<float>();
        /** height of header				*/
        private int m_headerHeight = 0;

        /** first data row number per page	*/
        private List<int> m_firstRowOnPage = new List<int>();
        /** first column number per -> page	*/
        private List<int> m_firstColumnOnPage = new List<int>();
        /** Height of page					*/
        private List<float> m_pageHeight = new List<float>();

        /**	Key: Point(row,col) - Value: NamePair	*/
        private HashMap<Point, NamePair> m_rowColDrillDown = new HashMap<Point, NamePair>();
        /**	Key: int (original Column) - Value: int (below column)	*/
        private HashMap<int, int> m_additionalLines;
        /** Print Data				*/
        private List<List<List<Object>>> m_printRows
            = new List<List<List<Object>>>();

        /*************************************************************************/

        /**	Header Row Indicator			*/
        public static int HEADER_ROW = -2;
        /**	Header Row Indicator			*/
        public static int ALL = -1;

        /**	Horizontal - GAP between text & line	*/
        private static int H_GAP = 2;
        /**	Vertical | GAP between text & line		*/
        private static int V_GAP = 2;

        /** Debug Print Paint						*/
        private static bool DEBUG_PRINT = false;


        protected override bool CalculateSize()
        {
            try
            {
                if (p_sizeCalculated)
                    return true;

                p_width = 0;
                m_printRows = new List<List<List<Object>>>(m_data.GetLength(0));	//	reset

                //	Max Column Width = 50% of available width (used if maxWidth not set)
                float dynMxColumnWidth = m_firstPage.Width / 2;
                //	Width caolculation
                int rows = m_data.GetLength(0);
                int cols = m_columnHeader.Length;

                //	Data Sizes and Header Sizes
                Dimension2DImpl[,] dataSizes = new Dimension2DImpl[rows, cols];
                Dimension2DImpl[] headerSizes = new Dimension2DImpl[cols];

                for (int dataCol = 0; dataCol < cols; dataCol++)
                {
                    int col = dataCol;
                    //	Print below existing column
                    if (m_additionalLines.ContainsKey(dataCol))
                    {
                        col = ((int)m_additionalLines[dataCol]);
                        log.Finest("DataColumn=" + dataCol + ", BelowColumn=" + col);
                    }
                    float colWidth = 0;
                    for (int row = 0; row < rows; row++)
                    {
                        Object dataItem = m_data[row, dataCol];
                        try
                        {
                            if (dataItem == null)
                            {
                                dataSizes[row, dataCol] = new Dimension2DImpl();
                                continue;

                            }
                            else if (dataItem.GetType().Name == "String")
                            {
                                if (string.IsNullOrEmpty(dataItem.ToString()))
                                {
                                    dataSizes[row, dataCol] = new Dimension2DImpl();
                                    continue;
                                }

                            }
                        }
                        catch 
                        {

                        }
                        String str = dataItem.ToString();
                        if (str.Length == 0)
                        {
                            dataSizes[row, dataCol] = new Dimension2DImpl();
                            continue;
                        }
                        Font font = GetFont(row, dataCol);
                        //	Print below existing column = (col != dataCol)
                        AddPrintLines(row, col, dataItem);
                        dataSizes[row, dataCol] = new Dimension2DImpl();		//	don't print

                        if (dataItem is Boolean)
                        {
                            dataSizes[row, col].AddBelow(LayoutEngine.IMAGE_SIZE);
                            continue;
                        }
                        else if (dataItem is ImageElement)
                        {
                            dataSizes[row, col].AddBelow(new java.awt.Dimension((int)((ImageElement)dataItem).GetWidth(), (int)((ImageElement)dataItem).GetHeight()));
                            continue;
                        }
                        //	No Width Limitations
                        if (m_columnMaxWidth[col] == 0 || m_columnMaxWidth[col] == -1)
                        {
                            //	if (HTMLElement.isHTML(string))
                            //		log.finest( "HTML (no) r=" + row + ",c=" + dataCol); 
                            TextLayout layout = new TextLayout(str, font);
                            float width = layout.GetAdvance() + 2;	//	buffer
                            float height = layout.GetHeight();
                            if (width > dynMxColumnWidth)
                                m_columnMaxWidth[col] = (int)Math.Ceiling(dynMxColumnWidth);
                            else if (colWidth < width)
                                colWidth = width;
                            if (dataSizes[row, col] == null)
                            {
                                dataSizes[row, col] = new Dimension2DImpl();
                                log.Log(Level.WARNING, "No Size for r=" + row + ",c=" + col);
                            }
                            dataSizes[row, col].AddBelow(width, height);
                        }
                        //	Width limitations
                        if (m_columnMaxWidth[col] != 0 && m_columnMaxWidth[col] != -1)
                        {
                            float height = 0;
                            if (HTMLElement.IsHTML(str))
                            {
                                //HTMLElement
                            }
                            else
                            {
                               // String[] lines = java.util.regex.Pattern.compile("$", java.util.regex.Pattern.MULTILINE).split(str);
                                String[] lines = System.Text.RegularExpressions.Regex.Split(str, "$", System.Text.RegularExpressions.RegexOptions.Multiline);
                                for (int lineNo = 0; lineNo < lines.Length; lineNo++)
                                {
                                    if (string.IsNullOrEmpty(lines[lineNo]))
                                    {
                                        continue;
                                    }
                                    AttributedString aString = new AttributedString(lines[lineNo]);
                                    aString.AddAttribute(TextAttribute.FONT, font);
                                    AttributedCharacterIterator iter = aString.GetIterator();
                                    LineBreakMeasurer measurer = new LineBreakMeasurer(iter);
                                    while (measurer.GetPosition() < iter.GetEndIndex())
                                    {
                                        TextLayout layout = measurer.NextLayout(Math.Abs(m_columnMaxWidth[col]));
                                        float width = layout.GetAdvance();
                                        if (colWidth < width)
                                            colWidth = width;
                                        float lineHeight = layout.GetHeight();
                                        if (m_columnMaxHeight[col] == -1)		//	one line only
                                        {
                                            height = lineHeight;
                                            break;
                                        }
                                        else if (m_columnMaxHeight[col] == 0 || (height + lineHeight) <= m_columnMaxHeight[col])
                                            height += lineHeight;
                                    }
                                }
                            }
                            if (m_fixedWidth[col])
                                colWidth = Math.Abs(m_columnMaxWidth[col]);
                            dataSizes[row, col].AddBelow(colWidth, height);
                        }
                        dataSizes[row, col].RoundUp();
                        if (dataItem is NamePair)
                            m_rowColDrillDown.Put(new Point(row, col), (NamePair)dataItem);
                        //	

                        //log.Finest("Col=" + col + ", row=" + row
                        //    + " => " + dataSizes[row, col] + " - ColWidth=" + colWidth);

                    }
                    //	Column Width  for Header
                    String _str = "";
                    if (!string.IsNullOrEmpty(m_columnHeader[dataCol].GetValue()))
                        _str = m_columnHeader[dataCol].ToString();

                    //	Print below existing column
                    if (col != dataCol)
                        headerSizes[dataCol] = new Dimension2DImpl();
                    else if (colWidth == 0 && m_columnMaxWidth[dataCol] < 0		//	suppress Null
                            || _str.Length == 0)
                        headerSizes[dataCol] = new Dimension2DImpl();
                    else
                    {
                        Font font = GetFont(HEADER_ROW, dataCol);
                        if (!font.Bold)
                        {
                            font = new Font(font.Name, font.Size, FontStyle.Bold, GraphicsUnit.World);
                        }
                        //	No Width Limitations
                        if (m_columnMaxWidth[dataCol] == 0 || m_columnMaxWidth[dataCol] == -1 || !m_multiLineHeader)
                        {
                            TextLayout layout = new TextLayout(_str, font);
                            float width = layout.GetAdvance() + 3;	//	buffer
                            float height = layout.GetHeight();
                            if (width > dynMxColumnWidth)
                                m_columnMaxWidth[dataCol] = (int)Math.Ceiling(dynMxColumnWidth);
                            else if (colWidth < width)
                                colWidth = width;
                            headerSizes[dataCol] = new Dimension2DImpl(width, height);
                        }
                        //	Width limitations
                        if (m_columnMaxWidth[dataCol] != 0 && m_columnMaxWidth[dataCol] != -1)
                        {
                            float height = 0;
                            //
                            //String[] lines = java.util.regex.Pattern.compile("$", java.util.regex.Pattern.MULTILINE).split(_str);
                            String[] lines = System.Text.RegularExpressions.Regex.Split(_str, "$", System.Text.RegularExpressions.RegexOptions.Multiline);
                            for (int lineNo = 0; lineNo < lines.Length; lineNo++)
                            {
                                if (string.IsNullOrEmpty(lines[lineNo]))
                                {
                                    continue;
                                }
                                AttributedString aString = new AttributedString(lines[lineNo]);
                                aString.AddAttribute(TextAttribute.FONT, font);
                                AttributedCharacterIterator iter = aString.GetIterator();
                                LineBreakMeasurer measurer = new LineBreakMeasurer(iter);
                                colWidth = Math.Abs(m_columnMaxWidth[dataCol]);
                                while (measurer.GetPosition() < iter.GetEndIndex())
                                {
                                    TextLayout layout = measurer.NextLayout(colWidth);
                                    float lineHeight = layout.GetHeight();
                                    if (!m_multiLineHeader)			//	one line only
                                    {
                                        height = lineHeight;
                                        break;
                                    }
                                    else if (m_columnMaxHeight[dataCol] == 0
                                        || (height + lineHeight) <= m_columnMaxHeight[dataCol])
                                        height += lineHeight;
                                }
                            }	//	for all header lines
                            headerSizes[dataCol] = new Dimension2DImpl(colWidth, height);
                        }
                    } //header size

                    headerSizes[dataCol].RoundUp();
                    colWidth = (float)Math.Ceiling(colWidth);
                    //	System.out.println("Col=" + dataCol + " => " + headerSizes[dataCol]);

                    //	Round Column Width
                    if (dataCol == 0)
                        colWidth += (float)m_tFormat.GetVLineStroke();
                    if (colWidth != 0)
                        colWidth += (2 * H_GAP) + (float)m_tFormat.GetVLineStroke();


                    //	Print below existing column
                    if (col != dataCol)
                    {
                        m_columnWidths.Add(0.0f);		//	for the data column
                        float? origWidth = (float)m_columnWidths[col];
                        if (origWidth == null)
                            log.Log(Level.SEVERE, "Column " + dataCol + " below " + col + " - no value for orig width");
                        else
                        {
                            if (((float)origWidth).CompareTo(colWidth) >= 0)
                            {
                                log.Finest("Same Width - Col=" + col + " - OrigWidth=" + origWidth + " - Width=" + colWidth + " - Total=" + p_width);
                            }
                            else
                            {
                                m_columnWidths[col] = colWidth;
                                p_width += (colWidth - (float)origWidth);
                                log.Finest("New Width - Col=" + col + " - OrigWidth=" + (float)origWidth + " - Width=" + colWidth + " - Total=" + p_width);
                            }
                        }
                    }
                    //	Add new Column
                    else
                    {
                        m_columnWidths.Add(colWidth);
                        p_width += colWidth;
                        log.Finest("Width - Col=" + dataCol + " - Width=" + colWidth + " - Total=" + p_width);
                    }
                }   //	for all columns
                //	Height	**********
                p_height = 0;
                for (int row = 0; row < rows; row++)
                {
                    float rowHeight = 0f;
                    for (int col = 0; col < cols; col++)
                    {
                        if (dataSizes[row, col].height > rowHeight)	//	max
                            rowHeight = (float)dataSizes[row, col].height;
                    }	//	for all columns
                    rowHeight += (float)m_tFormat.GetLineStroke() + (2 * V_GAP);
                    m_rowHeights.Add(rowHeight);
                    p_height += rowHeight;
                }	//	for all rows
                //Header Rows

                m_headerHeight = 0;
                for (int col = 0; col < cols; col++)
                {
                    if (headerSizes[col].height > m_headerHeight)
                        m_headerHeight = (int)headerSizes[col].height;
                }	//	for all columns
                m_headerHeight += (4 * (int)m_tFormat.GetLineStroke()) + (2 * V_GAP);	//	Thick lines
                p_height += m_headerHeight;


                //	Page Layout	*******************************************************

                log.Fine("FirstPage=" + m_firstPage + ", NextPages=" + m_nextPages);
                //	One Page on Y | Axis
                if (m_firstPage.Height >= p_height && m_pageBreak.Count() == 0)
                {
                    log.Finest("Page Y=1 - PageHeight=" + m_firstPage.Height + " - TableHeight=" + p_height);
                    m_firstRowOnPage.Add(0);	//	Y
                    m_pageHeight.Add(p_height);	//	Y index only
                }
                //	multiple pages on Y | Axis
                else
                {
                    float availableHeight = 0f;
                    float usedHeight = 0f;
                    bool firstPage = true;
                    int addlRows = 0;
                    //	for all rows
                    for (int dataRow = 0; dataRow < m_rowHeights.Count(); dataRow++)
                    {
                        float rowHeight = ((float)m_rowHeights[dataRow]);
                        //	Y page break before
                        bool pageBreak = IsPageBreak(dataRow);
                        if (!pageBreak && availableHeight < rowHeight)
                        {
                            if (availableHeight > 40 && rowHeight > 40)
                            {
                                log.Finest("- Split (leave on current) Row=" + dataRow + " - Available=" + availableHeight + ", RowHeight=" + rowHeight);
                                //	if (splitRow (dataRow))
                                //		addlRows += 1;
                            }
                            //	else
                            pageBreak = true;
                        }
                        if (pageBreak)
                        {
                            availableHeight = firstPage ? m_firstPage.Height : m_nextPages.Height;
                            m_firstRowOnPage.Add(dataRow + addlRows);	//	Y
                            if (!firstPage)
                            {
                                m_pageHeight.Add(usedHeight);	//	Y index only
                                log.Finest("Page Y=" + m_pageHeight.Count() + " - PageHeight=" + usedHeight);
                            }
                            log.Finest("Page Y=" + m_firstRowOnPage.Count() + " - Row=" + dataRow + " - force=" + IsPageBreak(dataRow));
                            firstPage = false;
                            //
                            availableHeight -= m_headerHeight;
                            usedHeight += m_headerHeight;
                        }
                        availableHeight -= rowHeight;
                        usedHeight += rowHeight;
                        if (availableHeight < 0)
                        {
                            log.Finest("- Split (move to next) Row=" + dataRow
                                + " - Available=" + availableHeight + ", RowHeight=" + rowHeight);

                        }
                        log.Finest("Page Y=" + m_pageHeight.Count()
                            + ", Row=" + dataRow + ",AddlRows=" + addlRows + ", Height=" + rowHeight
                            + " - Available=" + availableHeight + ", Used=" + usedHeight);
                    }	//	for all rows
                    m_pageHeight.Add(usedHeight);			//	Y index only
                    log.Finest("Page Y=" + m_pageHeight.Count() + " - PageHeight=" + usedHeight);
                }	//	multiple Y | pages

                //	One page on - X Axis
                if (m_firstPage.Width >= p_width)
                {
                    log.Finest("Page X=1 - PageWidth=" + m_firstPage.Width
                            + " - TableWidth=" + p_width);
                    m_firstColumnOnPage.Add(0);	//	X
                    //
                    DistributeColumns(m_firstPage.Width - (int)p_width, 0, m_columnWidths.Count());
                }
                //	multiple pages on - X Axis
                else
                {
                    int availableWidth = 0;
                    int lastStart = 0;
                    for (int col = 0; col < m_columnWidths.Count(); col++)
                    {
                        int columnWidth = (int)m_columnWidths[col];
                        //	X page preak
                        if (availableWidth < columnWidth)
                        {
                            if (col != 0)
                                DistributeColumns(availableWidth, lastStart, col);
                            //
                            m_firstColumnOnPage.Add(col);	//	X
                            log.Finest("Page X=" + m_firstColumnOnPage.Count() + " - Col=" + col);
                            lastStart = col;
                            availableWidth = m_firstPage.Width; 		//	Width is the same on all pages
                            //
                            for (int repCol = 0; repCol < m_repeatedColumns && col > repCol; repCol++)
                            {
                                float repColumnWidth = ((float)m_columnWidths[repCol]);
                                //	leave 50% of space available for non repeated columns
                                if (availableWidth < m_firstPage.Width * 0.5)
                                    break;
                                availableWidth -= (int)repColumnWidth;
                            }
                        }	//	pageBreak
                        availableWidth -= columnWidth;
                    }	//	for acc columns
                }	//	multiple - X pages

                //	Last row Lines
                p_height += (float)m_tFormat.GetLineStroke();			//	last fat line

                log.Fine("Pages=" + GetPageCount() + " X=" + m_firstColumnOnPage.Count() + "/Y=" + m_firstRowOnPage.Count() + " - Width=" + p_width + ", Height=" + p_height);
            }
            catch
            {
            }
            return true;
        }

        public new int GetPageCount()
        {
            return m_firstRowOnPage.Count() * m_firstColumnOnPage.Count();
        }	//	getPageCount

        public new float GetHeight(int pageNo)
        {
            int pageIndex = GetPageIndex(pageNo);
            int pageYindex = GetPageYIndex(pageIndex);
            log.Fine("Page=" + pageNo + " - PageIndex=" + pageIndex
                + ", PageYindex=" + pageYindex);
            float pageHeight = ((float)m_pageHeight[pageYindex]);
            float pageHeightPrevious = 0F;
            if (pageYindex > 0)
                pageHeightPrevious = ((float)m_pageHeight[pageYindex - 1]);
            float retValue = pageHeight - pageHeightPrevious;
            log.Fine("Page=" + pageNo + " - PageIndex=" + pageIndex + ", PageYindex=" + pageYindex + ", Height=" + retValue.ToString());
            return retValue;
        }	//	getHeight

        public float GetWidth(int pageNo)
        {
            int pageIndex = GetPageIndex(pageNo);
            if (pageIndex == 0)
                return m_firstPage.Width;
            return m_nextPages.Width;
        }	//	getHeight


        private Color GetBackground(int row, int col)
        {
            //	First specific position
            Color color = m_rowColBackground.Get(new Point(row, col));
            if(!color.IsEmpty)
                return color;
            //	Row Next
            color = m_rowColBackground.Get(new Point(row, ALL));
            if (!color.IsEmpty)
                return color;
            //	Column then
            color = m_rowColBackground.Get(new Point(ALL, col));
            if (!color.IsEmpty)
                return color;
            //	default
            return m_baseBackground;
        }	//	getFont

        private Color GetColor(int row, int col)
        {
            //	First specific position
            Color color = m_rowColColor.Get(new Point(row, col));
            if (!color.IsEmpty)
                return color;
            //	Row Next
            color = m_rowColColor.Get(new Point(row, ALL));
            if (!color.IsEmpty)
                return color;
            //	Column then
            color = m_rowColColor.Get(new Point(ALL, col));
            if (!color.IsEmpty)
                return color;
            //	default
            return m_baseColor;
        }	//	getFont


        private void PrintColumn(Graphics g2D, int col, int origX, int origY, bool leftVline, int firstRow, int nextPageRow, bool isView)
        {
            int curX = origX;
            int curY = origY;	//	start from top

            float colWidth = m_columnWidths[col];		//	includes 2*Gaps+Line
            float netWidth = colWidth - (2 * H_GAP) - (float)m_tFormat.GetVLineStroke();

            if (leftVline)
                netWidth -= (float)m_tFormat.GetVLineStroke();
            int rowHeight = m_headerHeight;
            float netHeight = rowHeight - (4 * (float)m_tFormat.GetLineStroke()) + (2 * V_GAP);

            if (DEBUG_PRINT)
                log.Finer("#" + col + " - x=" + curX + ", y=" + curY + ", width=" + colWidth + "/" + netWidth + ", HeaderHeight=" + rowHeight + "/" + netHeight);
            String alignment = m_columnJustification[col];

            Pen color = new Pen(Color.White);
            SolidBrush paint = new SolidBrush(Color.White);
            //	paint header	***************************************************
            if (leftVline)			//	draw left | line
            {
                color.Color = m_tFormat.GetVLine_Color();
                color.DashStyle = m_tFormat.GetVLine_Stroke();
                color.Width = (float)m_tFormat.GetLineStroke() / 2;

                if (m_tFormat.IsPaintBoundaryLines())				//	 -> | (left)
                {
                    g2D.DrawLine(color, origX, (int)(origY + (float)m_tFormat.GetLineStroke()), origX, (int)(origY + rowHeight - (4 * (float)m_tFormat.GetLineStroke())));
                }
                curX += (int)m_tFormat.GetVLineStroke();
            }
            //	X - start line
            if (m_tFormat.IsPaintHeaderLines())
            {
                color.Color = m_tFormat.GetHeaderLine_Color();
                color.DashStyle = m_tFormat.GetHeader_Stroke();
                color.Width = (float)m_tFormat.GetHdrStroke();
                //	 -> - (top) 
                g2D.DrawLine(color, origX, origY, (int)(origX + colWidth - (float)m_tFormat.GetVLineStroke() + 1), origY);
            }
            curY += (2 * (int)m_tFormat.GetLineStroke());	//	thick
            //	Background
            System.Drawing.Color bg = GetBackground(HEADER_ROW, col);
            if (!bg.Equals(Color.White))
            {
                paint.Color = bg;
                g2D.FillRectangle(paint, curX, (int)(curY - (float)m_tFormat.GetLineStroke()), (int)(colWidth - (float)m_tFormat.GetVLineStroke() + 1), (int)((rowHeight) - (4 * (float)m_tFormat.GetLineStroke())));
            }
            curX += H_GAP;		//	upper left gap
            curY += V_GAP;
            //	Header
            AttributedString aString = null;
            AttributedCharacterIterator iter = null;
            LineBreakMeasurer measurer = null;
            float usedHeight = 0;
            if (m_columnHeader[col].ToString().Length > 0)
            {
                aString = new AttributedString(m_columnHeader[col].ToString());
                aString.AddAttribute(TextAttribute.FONT, GetFont(HEADER_ROW, col));
                aString.AddAttribute(TextAttribute.FOREGROUND, GetColor(HEADER_ROW, col));

                bool fastDraw = LayoutEngine.s_FASTDRAW;
                if (fastDraw && !isView && !Utility.Util.Is8Bit(m_columnHeader[col].ToString()))
                    fastDraw = false;
                iter = aString.GetIterator();
                measurer = new LineBreakMeasurer(iter);

                while (measurer.GetPosition() < iter.GetEndIndex())		//	print header
                {
                    TextLayout layout = measurer.NextLayout(netWidth + 2);
                    if (iter.GetEndIndex() != measurer.GetPosition())
                        fastDraw = false;
                    float lineHeight = layout.GetHeight();
                    if (m_columnMaxHeight[col] <= 0		//	-1 = FirstLineOnly  
                        || (usedHeight + lineHeight) <= m_columnMaxHeight[col])
                    {
                        if (alignment.Equals(MPrintFormatItem.FIELDALIGNMENTTYPE_Block))
                        {
                            //layout = layout.getJustifiedLayout(netWidth + 2);
                            fastDraw = false;
                        }
                        curY += (int)layout.GetAscent();
                        float penX = curX;
                        if (alignment.Equals(MPrintFormatItem.FIELDALIGNMENTTYPE_Center))
                            penX += (netWidth - layout.GetAdvance()) / 2;
                        else if ((alignment.Equals(MPrintFormatItem.FIELDALIGNMENTTYPE_TrailingRight) && layout.IsLeftToRight())
                            || (alignment.Equals(MPrintFormatItem.FIELDALIGNMENTTYPE_LeadingLeft) && !layout.IsLeftToRight()))
                            penX += netWidth - layout.GetAdvance();
                        //
                        if (fastDraw)
                        {	//	Bug - set Font/Color explicitly 
                            Font drwFont = GetFont(HEADER_ROW, col);
                            paint.Color = GetColor(HEADER_ROW, col);
                            g2D.DrawString(iter.GetText(), drwFont, paint, penX, curY - 12); // Y Axis is not same as Y Asix of Java. We have to recalculate it. (minus some figure from Y asix)
                        }
                        else
                            layout.Draw(g2D, penX, curY - 12);										//	-> text
                        curY += (int)layout.GetDescent() + (int)layout.GetLeading();
                        usedHeight += lineHeight;
                    }
                    if (!m_multiLineHeader)			//	one line only
                        break;
                }
            }
            curX += (int)netWidth + H_GAP;
            curY += V_GAP;
            //	Y end line
            //g2D.setPaint(m_tFormat.GetVLine_Color());
            color.Color = m_tFormat.GetVLine_Color();
            color.DashStyle = m_tFormat.GetVLine_Stroke();
            color.Width = (float)m_tFormat.GetLineStroke() / 2;
            if (m_tFormat.IsPaintVLines())					//	 -> | (right)
            {
                g2D.DrawLine(color, curX, (int)(origY + m_tFormat.GetLineStroke()), curX, (int)(origY + rowHeight - (4 * m_tFormat.GetLineStroke())));
            }
            curX += (int)m_tFormat.GetVLineStroke();
            //	X end line
            if (m_tFormat.IsPaintHeaderLines())     //Bottom line of header row
            {
                color.Color = m_tFormat.GetHeaderLine_Color();
                color.DashStyle = m_tFormat.GetHeader_Stroke();
                color.Width = (float)m_tFormat.GetHdrStroke();
                //	 -> - (button)
                g2D.DrawLine(color, origX, curY , (int)((origX) + colWidth - (int)m_tFormat.GetVLineStroke() +1), curY);
            }
            curY += (2 * (int)m_tFormat.GetLineStroke());	//	thick

            //	paint Data		***************************************************
            for (int row = firstRow; row < nextPageRow; row++)
            {
                rowHeight = ((int)m_rowHeights[row]);	//	includes 2*Gaps+Line
                netHeight = (float)rowHeight - (2 * V_GAP) - (float)m_tFormat.GetLineStroke();
                int rowYstart = curY;

                curX = origX;
                if (leftVline)			//	draw left | line
                {
                    color.Color = m_tFormat.GetVLine_Color();
                    color.DashStyle = m_tFormat.GetVLine_Stroke();
                    color.Width = (float)m_tFormat.GetLineStroke() / 2;
                    if (m_tFormat.IsPaintBoundaryLines())    				//	 -> | (left)
                    {
                        g2D.DrawLine(color, curX, rowYstart, curX, (int)(rowYstart + rowHeight - m_tFormat.GetLineStroke()));
                    }
                    curX += (int)m_tFormat.GetVLineStroke();
                }
                //	Background
                bg = GetBackground(row, col);
                if (!bg.Equals(Color.White))
                {
                    paint.Color = bg;
                    g2D.FillRectangle(paint, curX, curY, (int)(colWidth - (float)m_tFormat.GetVLineStroke()), (int)(rowHeight - m_tFormat.GetLineStroke()));
                }
                curX += H_GAP;		//	upper left gap
                curY += V_GAP;

                //	actual data
                Object[] printItems = GetPrintItems(row, col);
                float penY = curY;
                for (int index = 0; index < printItems.Length; index++)
                {
                    if (printItems[index] == null)
                    { }
                    else if (printItems[index] is ImageElement)
                    {
                        g2D.DrawImage(((ImageElement)printItems[index]).GetImage(), curX, (int)penY);
                    }
                    else if (printItems[index] is Boolean)
                    {
                        int penX = curX + (int)((netWidth - LayoutEngine.IMAGE_SIZE.width) / 2);	//	center
                        if (((Boolean)printItems[index]))
                        {
                            g2D.DrawImage(LayoutEngine.IMAGE_TRUE, penX, (int)penY);
                        }
                        else
                        {
                            g2D.DrawImage(LayoutEngine.IMAGE_FALSE, penX, (int)penY);
                        }
                        penY += LayoutEngine.IMAGE_SIZE.height;
                    }
                    else
                    {
                        String str = printItems[index].ToString();
                        if (DEBUG_PRINT)
                            log.Fine("row=" + row + ",col=" + col + " - " + str + " 8Bit=" + Utility.Util.Is8Bit(str));
                        if (str.Length > 0)
                        {
                            usedHeight = 0;
                           // String[] lines = java.util.regex.Pattern.compile("$", java.util.regex.Pattern.MULTILINE).split(str);
                            String[] lines = System.Text.RegularExpressions.Regex.Split(str, "$", System.Text.RegularExpressions.RegexOptions.Multiline);
                            for (int lineNo = 0; lineNo < lines.Length; lineNo++)
                            {
                                if (string.IsNullOrEmpty(lines[lineNo]))
                                {
                                    continue;
                                }
                                aString = new AttributedString(lines[lineNo]);
                                aString.AddAttribute(TextAttribute.FONT, GetFont(row, col));
                                if (isView && printItems[index] is NamePair)	//	ID
                                {
                                    aString.AddAttribute(TextAttribute.FOREGROUND, LINK_COLOR);
                                    aString.AddAttribute(TextAttribute.UNDERLINE, TextAttribute.UNDERLINE_LOW_ONE_PIXEL, 0, str.Length);
                                }
                                else
                                    aString.AddAttribute(TextAttribute.FOREGROUND, GetColor(row, col));
                                //
                                iter = aString.GetIterator();
                                bool fastDraw = LayoutEngine.s_FASTDRAW;
                                if (fastDraw && !isView && !Utility.Util.Is8Bit(lines[lineNo]))
                                    fastDraw = false;
                                measurer = new LineBreakMeasurer(iter);
                                while (measurer.GetPosition() < iter.GetEndIndex())		//	print element
                                {
                                    TextLayout layout = measurer.NextLayout(netWidth + 2);
                                    if (iter.GetEndIndex() != measurer.GetPosition())
                                        fastDraw = false;
                                    float lineHeight = layout.GetAscent() + layout.GetDescent() + layout.GetLeading();
                                    if ((m_columnMaxHeight[col] <= 0
                                            || (usedHeight + lineHeight) <= m_columnMaxHeight[col])
                                        && (usedHeight + lineHeight) <= netHeight)
                                    {
                                        if (alignment.Equals(MPrintFormatItem.FIELDALIGNMENTTYPE_Block) && measurer.GetPosition() < iter.GetEndIndex())
                                        {
                                            //layout = layout.getJustifiedLayout(netWidth + 2);
                                            fastDraw = false;
                                        }
                                        penY += layout.GetAscent();
                                        float penX = curX;
                                        if (alignment.Equals(MPrintFormatItem.FIELDALIGNMENTTYPE_Center))
                                            penX += (netWidth - layout.GetAdvance()) / 2;
                                        else if ((alignment.Equals(MPrintFormatItem.FIELDALIGNMENTTYPE_TrailingRight) && layout.IsLeftToRight())
                                            || (alignment.Equals(MPrintFormatItem.FIELDALIGNMENTTYPE_LeadingLeft) && !layout.IsLeftToRight()))
                                            penX += netWidth - layout.GetAdvance();
                                        //

                                        if (fastDraw)
                                        {	//	Bug - set Font/Color explicitly
                                            //g2D.setFont(getFont(row, col));
                                            Font drwFont = GetFont(row, col);
                                            if (isView && printItems[index] is NamePair)	//	ID
                                            {
                                                paint.Color = LINK_COLOR;
                                                //	TextAttribute.UNDERLINE
                                            }
                                            else
                                                paint.Color = GetColor(row, col);

                                            g2D.DrawString(iter.GetText(), drwFont, paint, penX, penY - 9);
                                        }
                                        else
                                            layout.Draw(g2D, penX, penY - 9);										//	-> text
                                        if (DEBUG_PRINT)
                                            log.Fine("row=" + row + ",col=" + col + " - " + str + " - x=" + penX + ",y=" + penY);
                                        penY += layout.GetDescent() + layout.GetLeading();
                                        usedHeight += lineHeight;
                                        //
                                        if (m_columnMaxHeight[col] == -1)	//	FirstLineOny
                                            break;
                                    }
                                }	//	print element
                            }	//	for all lines
                        }	//	length > 0
                    }
                }   //for all print items

                curY += (int)netHeight + V_GAP;
                curX += (int)netWidth + H_GAP;
                //	Y end line
                color.Color = m_tFormat.GetVLine_Color();
                color.DashStyle = m_tFormat.GetVLine_Stroke();
                color.Width = (float)m_tFormat.GetLineStroke() / 2;
                if (m_tFormat.IsPaintVLines())
                {
                    //	 -> | (right)
                    g2D.DrawLine(color, curX, rowYstart, curX, (int)(rowYstart + rowHeight - m_tFormat.GetLineStroke())); curX += (int)m_tFormat.GetVLineStroke();
                }

                //	X end line
                if (row == m_data.GetLength(0) - 1)			//	last Line
                {
                    color.Color = m_tFormat.GetHeaderLine_Color();
                    color.DashStyle = m_tFormat.GetHeader_Stroke();
                    color.Width = (float)m_tFormat.GetHdrStroke();
                    //	 -> - (last line) 
                    g2D.DrawLine(color, origX, curY , (int)(origX + (int)colWidth - (int)m_tFormat.GetVLineStroke() + 1), curY );
                    curY += (2 * (int)m_tFormat.GetLineStroke());	//	thick
                }
                else
                {
                    //	next line is a funcion column -> underline this
                    bool nextIsFunction = m_functionRows.Contains(row + 1);
                    if (nextIsFunction && m_functionRows.Contains(row))
                        nextIsFunction = false;		//	this is a function line too
                    if (nextIsFunction)
                    {
                        color.Color = m_tFormat.GetFunctFG_Color();
                        color.DashStyle = m_tFormat.GetHLine_Stroke();
                        color.Width = (float)m_tFormat.GetLineStroke() / 2;
                        //	 -> - (bottom)
                        g2D.DrawLine(color, origX, curY, (int)(origX + (int)colWidth - (int)m_tFormat.GetVLineStroke()), curY);
                    }
                    else if (m_tFormat.IsPaintHLines())
                    {
                        color.Color = m_tFormat.GetHLine_Color();
                        color.DashStyle = m_tFormat.GetHLine_Stroke();
                        color.Width = (float)m_tFormat.GetLineStroke() / 2;
                        //	 -> - (bottom)
                        g2D.DrawLine(color, origX, curY, (int)(origX + (int)colWidth - (int)m_tFormat.GetVLineStroke()), curY);
                    }
                    curY += (int)m_tFormat.GetLineStroke();
                }
            }
            color.Dispose();
            paint.Dispose();
        }

        private void DistributeColumns(int availableWidth, int fromCol, int toCol)
        {
            log.Finest("Available=" + availableWidth + ", Columns " + fromCol + "->" + toCol);
            int start = fromCol;
            if (fromCol == 0 && m_repeatedColumns > 0)
                start = m_repeatedColumns;
            //	calculate total Width
            int totalWidth = availableWidth;
            for (int col = start; col < toCol; col++)
                totalWidth += (int)((float)m_columnWidths[col]);
            int remainingWidth = availableWidth;
            //	distribute proportionally (does not increase zero width columns)
            for (int x = 0; remainingWidth > 0 && x < 5; x++)	//	max 4 iterations
            {
                log.Finest("TotalWidth=" + totalWidth + ", Remaining=" + remainingWidth);
                for (int col = start; col < toCol && remainingWidth != 0; col++)
                {
                    int columnWidth = (int)((float)m_columnWidths[col]);
                    if (columnWidth != 0)
                    {
                        int additionalPart = columnWidth * availableWidth / totalWidth;
                        if (remainingWidth < additionalPart)
                        {
                            m_columnWidths[col] = columnWidth + remainingWidth;
                            remainingWidth = 0;
                        }
                        else
                        {
                            m_columnWidths[col] = columnWidth + additionalPart;
                            remainingWidth -= additionalPart;
                        }
                        log.Finest("  col=" + col + " - From " + columnWidth + " to " + m_columnWidths[col]);
                    }
                }
            }
            //	add remainder to last non 0 width column
            for (int c = toCol - 1; remainingWidth != 0 && c >= 0; c--)
            {
                int columnWidth = (int)((float)m_columnWidths[c]);
                if (columnWidth > 0)
                {
                    m_columnWidths[c] =  columnWidth + remainingWidth;
                    log.Finest("Final col=" + c + " - From " + columnWidth + " to " + m_columnWidths[c]);
                    remainingWidth = 0;
                }
            }
        }	//	distribute Columns

        private bool IsPageBreak(int row)
        {
            for (int i = 0; i < m_pageBreak.Count(); i++)
            {
                int rr = (int)m_pageBreak[i];
                if (rr + 1 == row)
                    return true;
                else if (rr > row)
                    return false;
            }
            return false;
        }	//	isPageBreak

        public void SetHeightToLastPage()
        {
            int lastLayoutPage = GetPageCount() + m_pageNoStart - 1;
            log.Fine("PageCount - Table=" + GetPageCount()
                + "(Start=" + m_pageNoStart
                + ") Layout=" + lastLayoutPage
                + " - Old Height=" + p_height);
            p_height = GetHeight(lastLayoutPage);
            log.Fine("New Height=" + p_height);
        }	//	setHeightToLastPage

        private Font GetFont(int row, int col)
        {
            //	First specific position

            Font font = m_rowColFont.Get(new Point(row, col));
            if (font != null)
                return font;
            //	Row Next
            font = m_rowColFont.Get(new Point(row, ALL));
            if (font != null)
                return font;
            //	Column then
            font = m_rowColFont.Get(new Point(ALL, col));
            if (font != null)
                return font;
            //	default
            return m_baseFont;
        }	//	getFont


        public override void Paint(Graphics g2D, int pageNo, PointF pageStart, Ctx ctx, bool isView)
        {
            
            int pageIndex = GetPageIndex(pageNo);
            int pageXindex = GetPageXIndex(pageIndex);
            int pageYindex = GetPageYIndex(pageIndex);
            if (DEBUG_PRINT)
                log.Config("Page=" + pageNo + " [x=" + pageXindex + ", y=" + pageYindex + "]");
            //
            int firstColumn = ((int)m_firstColumnOnPage[pageXindex]);
            int nextPageColumn = m_columnHeader.Length;		// no of cols
            if (pageXindex + 1 < m_firstColumnOnPage.Count())
                nextPageColumn = ((int)m_firstColumnOnPage[pageXindex + 1]);
            //
            int firstRow = ((int)m_firstRowOnPage[pageYindex]);
            int nextPageRow = m_data.GetLength(0);				//	no of rows
            if (pageYindex + 1 < m_firstRowOnPage.Count())
                nextPageRow = ((int)m_firstRowOnPage[pageYindex + 1]);
            if (DEBUG_PRINT)
                log.Finest("Col=" + firstColumn + "-" + (nextPageColumn - 1) + ", Row=" + firstRow + "-" + (nextPageRow - 1));

            //	Top Left
            int startX = (int)pageStart.X;
            int startY = (int)pageStart.Y;
            //	Table Start
            startX += pageXindex == 0 ? m_firstPage.X : m_nextPages.X;
            startY += pageYindex == 0 ? m_firstPage.Y : m_nextPages.Y;
            if (DEBUG_PRINT)
                log.Finest("PageStart=" + pageStart + ", StartTable x=" + startX + ", y=" + startY);

            //	paint first fixed volumns
            bool firstColumnPrint = true;
            int regularColumnStart = firstColumn;
            for (int col = 0; col < m_repeatedColumns && col < m_columnWidths.Count(); col++)
            {
                int colWidth = (int)((float)m_columnWidths[col]);		//	includes 2*Gaps+Line
                if (colWidth != 0)
                {
                    PrintColumn(g2D, col, startX, startY, firstColumnPrint, firstRow, nextPageRow, isView);
                    startX += colWidth;
                    firstColumnPrint = false;
                }
                if (regularColumnStart == col)
                    regularColumnStart++;
            }

            //	paint columns
            for (int col = regularColumnStart; col < nextPageColumn; col++)
            {
                int colWidth = (int)((float)m_columnWidths[col]);		//	includes 2*Gaps+Line
                if (colWidth != 0)
                {
                    PrintColumn(g2D, col, startX, startY, firstColumnPrint, firstRow, nextPageRow, isView);
                    startX += colWidth;
                    firstColumnPrint = false;
                }
            }	//	for all columns

        }	//	paint


        public int GetPageIndex(int pageNo)
        {
            int index = pageNo - m_pageNoStart;
            if (index < 0)
                log.Log(Level.SEVERE, "index=" + index, new Exception());
            return index;
        }	//	getPageIndex


        public int GetPageNo(int pageIndex)
        {
            return pageIndex + m_pageNoStart;
        }	//	getPageNo

        public int GetPageXIndex(int pageIndex)
        {
            int noXpages = m_firstColumnOnPage.Count();
            //	int noYpages = m_firstRowOnPage.size();
            int x = pageIndex % noXpages;
            return x;
        }	//	getPageXIndex

        public int GetPageXCount()
        {
            return m_firstColumnOnPage.Count();
        }	//	getPageXCount

        public int GetPageYIndex(int pageIndex)
        {
            int noXpages = m_firstColumnOnPage.Count();
            //	int noYpages = m_firstRowOnPage.size();
            int y = (pageIndex - (pageIndex % noXpages)) / noXpages;
            return y;
        }	//	getPageYIndex

        public int GetPageYCount()
        {
            return m_firstRowOnPage.Count();
        }	//	getPageYCount

        public int GetRow(int yPos, int pageNo)
        {
            int pageIndex = GetPageIndex(pageNo);
            int pageYindex = GetPageYIndex(pageIndex);
            //
            int curY = (pageYindex == 0 ? m_firstPage.Y : m_nextPages.Y) + m_headerHeight;
            if (yPos < curY)
                return -1;		//	above
            //
            int firstRow = ((int)m_firstRowOnPage[pageYindex]);
            int nextPageRow = m_data.Length;				//	no of rows
            if (pageYindex + 1 < m_firstRowOnPage.Count())
                nextPageRow = ((int)m_firstRowOnPage[pageYindex + 1]);
            //
            for (int row = firstRow; row < nextPageRow; row++)
            {
                try
                {
                    int rowHeight = (int)((float)m_rowHeights[row]);	//	includes 2*Gaps+Line
                    if (yPos >= curY && yPos < (curY + rowHeight))
                        return row;
                    curY += rowHeight;
                }
                catch
                {
                }
            }
            //	below
            return -1;
        }	//	getRow

        public override Query GetDrillAcross(Point relativePoint, int pageNo)
        {
            if (!GetBounds(pageNo).Contains(relativePoint))
                return null;
            int row = GetRow(relativePoint.Y, pageNo);
            if (row == -1)
                return null;
            log.Fine("Row=" + row + ", PageNo=" + pageNo);
            //
            if (m_pk[row] == null)	//	FunctionRows
                return null;
            return Query.GetEqualQuery(m_pkColumnName, m_pk[row].GetKey());
        }	//	getDrillAcross

        public Rectangle GetBounds(int pageNo)
        {
            int pageIndex = GetPageIndex(pageNo);
            int pageYindex = GetPageYIndex(pageIndex);
            if (pageYindex == 0)
                return m_firstPage;
            else
                return m_nextPages;
        }	//	getBounds

        private int GetCol(int xPos, int pageNo)
        {
            int pageIndex = GetPageIndex(pageNo);
            int pageXindex = GetPageXIndex(pageIndex);
            //
            int curX = pageXindex == 0 ? m_firstPage.X : m_nextPages.X;
            if (xPos < curX)
                return -1;		//	too left

            int firstColumn = ((int)m_firstColumnOnPage[pageXindex]);
            int nextPageColumn = m_columnHeader.Length;		// no of cols
            if (pageXindex + 1 < m_firstColumnOnPage.Count())
                nextPageColumn = ((int)m_firstColumnOnPage[pageXindex + 1]);

            //	fixed volumns
            int regularColumnStart = firstColumn;
            for (int col = 0; col < m_repeatedColumns; col++)
            {
                int colWidth = (int)((float)m_columnWidths[col]);		//	includes 2*Gaps+Line
                if (xPos >= curX && xPos < (curX + colWidth))
                    return col;
                curX += colWidth;
                if (regularColumnStart == col)
                    regularColumnStart++;
            }
            //	regular columns
            for (int col = regularColumnStart; col < nextPageColumn; col++)
            {
                int colWidth = (int)((float)m_columnWidths[col]);		//	includes 2*Gaps+Line
                if (xPos >= curX && xPos < (curX + colWidth))
                    return col;
                curX += colWidth;
            }	//	for all columns
            //	too right
            return -1;
        }	//	getCol


        public override Query GetDrillDown(Point relativePoint, int pageNo)
        {
            if (m_rowColDrillDown.Count() == 0)
                return null;
            if (!GetBounds(pageNo).Contains(relativePoint))
                return null;
            int row = GetRow(relativePoint.Y, pageNo);
            if (row == -1)
                return null;
            int col = GetCol(relativePoint.X, pageNo);
            if (col == -1)
                return null;
            log.Fine("Row=" + row + ", Col=" + col + ", PageNo=" + pageNo);
            //
            NamePair pp = (NamePair)m_rowColDrillDown.Get(new Point(row, col));
            if (pp == null)
                return null;
            String columnName = Query.GetZoomColumnName(m_columnHeader[col].GetID());
            String tableName = Query.GetZoomTableName(columnName);
            Object code = pp.GetID();
            if (pp is KeyNamePair)
                code = ((KeyNamePair)pp).GetKey();
            //
            Query query = new Query(tableName);
            query.AddRestriction(columnName, Query.EQUAL, code, null, pp.ToString());
            return query;
        }	//	getDrillDown


        private void AddPrintLines(int row, int col, Object data)
        {
            while (m_printRows.Count() <= row)
                m_printRows.Add(null);
            List<List<Object>> columns = m_printRows[row];
            if (columns == null)
                columns = new List<List<Object>>(m_columnHeader.Length);
            while (columns.Count() <= col)
                columns.Add(null);
            //
            List<Object> coordinate = columns[col];
            if (coordinate == null)
                coordinate = new List<Object>();
            coordinate.Add(data);
            //
            columns[col] = coordinate;
            m_printRows[row] = columns;
            log.Finest("row=" + row + ", col=" + col + " - Rows=" + m_printRows.Count() + ", Cols=" + columns.Count() + " - " + data);
        }	//	addAdditionalLines


        private Object[] GetPrintItems(int row, int col)
        {
            List<List<Object>> columns = null;
            if (m_printRows.Count() > row)
                columns = m_printRows[row];
            if (columns == null)
                return new Object[] { };
            List<Object> coordinate = null;
            if (columns.Count() > col)
                coordinate = columns[col];
            if (coordinate == null)
                return new Object[] { };
            //
            return coordinate.ToArray();
        }	//	getPrintItems

        public override void PaintPdf(XGraphics g2D, int pageNo, PointF pageStart, Ctx ctx, bool isView)
        {

            int pageIndex = GetPageIndex(pageNo);
            int pageXindex = GetPageXIndex(pageIndex);
            int pageYindex = GetPageYIndex(pageIndex);
            if (DEBUG_PRINT)
                log.Config("Page=" + pageNo + " [x=" + pageXindex + ", y=" + pageYindex + "]");
            //
            int firstColumn = ((int)m_firstColumnOnPage[pageXindex]);
            int nextPageColumn = m_columnHeader.Length;		// no of cols
            if (pageXindex + 1 < m_firstColumnOnPage.Count())
                nextPageColumn = ((int)m_firstColumnOnPage[pageXindex + 1]);
            //
            int firstRow = ((int)m_firstRowOnPage[pageYindex]);
            int nextPageRow = m_data.GetLength(0);				//	no of rows
            if (pageYindex + 1 < m_firstRowOnPage.Count())
                nextPageRow = ((int)m_firstRowOnPage[pageYindex + 1]);
            if (DEBUG_PRINT)
                log.Finest("Col=" + firstColumn + "-" + (nextPageColumn - 1) + ", Row=" + firstRow + "-" + (nextPageRow - 1));

            //	Top Left
            int startX = (int)pageStart.X;
            int startY = (int)pageStart.Y;
            //	Table Start
            startX += pageXindex == 0 ? m_firstPage.X : m_nextPages.X;
            startY += pageYindex == 0 ? m_firstPage.Y : m_nextPages.Y;
            if (DEBUG_PRINT)
                log.Finest("PageStart=" + pageStart + ", StartTable x=" + startX + ", y=" + startY);

            //	paint first fixed volumns
            bool firstColumnPrint = true;
            int regularColumnStart = firstColumn;
            for (int col = 0; col < m_repeatedColumns && col < m_columnWidths.Count(); col++)
            {
                int colWidth = (int)((float)m_columnWidths[col]);		//	includes 2*Gaps+Line
                if (colWidth != 0)
                {
                    PrintColumnForPDF(g2D, col, startX, startY, firstColumnPrint, firstRow, nextPageRow, isView);
                    startX += colWidth;
                    firstColumnPrint = false;
                }
                if (regularColumnStart == col)
                    regularColumnStart++;
            }

            //	paint columns
            for (int col = regularColumnStart; col < nextPageColumn; col++)
            {
                int colWidth = (int)((float)m_columnWidths[col]);		//	includes 2*Gaps+Line
                if (colWidth != 0)
                {
                    PrintColumnForPDF(g2D, col, startX, startY, firstColumnPrint, firstRow, nextPageRow, isView);
                    startX += colWidth;
                    firstColumnPrint = false;
                }
            }	//	for all columns

        }	//	paint

        private void PrintColumnForPDF(XGraphics g2D, int col, int origX, int origY, bool leftVline, int firstRow, int nextPageRow, bool isView)
        {
            int curX = origX;
            int curY = origY;	//	start from top

            float colWidth = m_columnWidths[col];		//	includes 2*Gaps+Line
            float netWidth = colWidth - (2 * H_GAP) - (float)m_tFormat.GetVLineStroke();

            if (leftVline)
                netWidth -= (float)m_tFormat.GetVLineStroke();
            int rowHeight = m_headerHeight;
            float netHeight = rowHeight - (4 * (float)m_tFormat.GetLineStroke()) + (2 * V_GAP);

            if (DEBUG_PRINT)
                log.Finer("#" + col + " - x=" + curX + ", y=" + curY + ", width=" + colWidth + "/" + netWidth + ", HeaderHeight=" + rowHeight + "/" + netHeight);
            String alignment = m_columnJustification[col];


            XPen color = new XPen(XColors.White);
            XSolidBrush paint = new XSolidBrush(XColors.White);
            //	paint header	***************************************************
            if (leftVline)			//	draw left | line
            {
                color.Color = XColor.FromArgb(m_tFormat.GetVLine_Color());
                color.DashStyle = m_tFormat.GetVLine_StrokeForPdf();
                color.Width = (float)m_tFormat.GetLineStroke() / 2;

                if (m_tFormat.IsPaintBoundaryLines())				//	 -> | (left)
                    g2D.DrawLine(color, origX, (int)(origY + (float)m_tFormat.GetLineStroke()),
                        origX, (int)(origY + rowHeight - (4 * (float)m_tFormat.GetLineStroke())));
                curX += (int)m_tFormat.GetVLineStroke();
            }
            //	X - start line
            if (m_tFormat.IsPaintHeaderLines())
            {
                color.Color = XColor.FromArgb(m_tFormat.GetHeaderLine_Color());
                color.DashStyle = m_tFormat.GetHeader_StrokePdf();
                color.Width = (float)m_tFormat.GetHdrStroke();
                g2D.DrawLine(color, origX, origY, 							//	 -> - (top) 
                    (int)(origX + colWidth - (float)m_tFormat.GetVLineStroke() + 1), origY);
            }
            curY += (2 * (int)m_tFormat.GetLineStroke());	//	thick
            //	Background
            XColor bg = XColor.FromArgb(GetBackground(HEADER_ROW, col));
            if (!bg.Equals(XColors.White))
            {
                paint.Color = bg;
                g2D.DrawRectangle(paint, curX,
                    (int)(curY - (float)m_tFormat.GetLineStroke()),
                    (int)(colWidth - (float)m_tFormat.GetVLineStroke() + 1),
                    (int)((rowHeight - 1) - (4 * (float)m_tFormat.GetLineStroke())));
            }
            curX += H_GAP;		//	upper left gap
            curY += V_GAP;
            //	Header
            AttributedString aString = null;
            AttributedCharacterIterator iter = null;
            LineBreakMeasurer measurer = null;
            float usedHeight = 0;
            if (m_columnHeader[col].ToString().Length > 0)
            {
                aString = new AttributedString(m_columnHeader[col].ToString());
                aString.AddAttribute(TextAttribute.FONT, GetFont(HEADER_ROW, col));
                aString.AddAttribute(TextAttribute.FOREGROUND, GetColor(HEADER_ROW, col));

                bool fastDraw = LayoutEngine.s_FASTDRAW;
                if (fastDraw && !isView && !Utility.Util.Is8Bit(m_columnHeader[col].ToString()))
                    fastDraw = false;
                iter = aString.GetIterator();
                measurer = new LineBreakMeasurer(iter);

                while (measurer.GetPosition() < iter.GetEndIndex())		//	print header
                {
                    TextLayout layout = measurer.NextLayout(netWidth + 2);
                    if (iter.GetEndIndex() != measurer.GetPosition())
                        fastDraw = false;
                    float lineHeight = layout.GetHeight();
                    if (m_columnMaxHeight[col] <= 0		//	-1 = FirstLineOnly  
                        || (usedHeight + lineHeight) <= m_columnMaxHeight[col])
                    {
                        if (alignment.Equals(MPrintFormatItem.FIELDALIGNMENTTYPE_Block))
                        {
                            //layout = layout.getJustifiedLayout(netWidth + 2);
                            fastDraw = false;
                        }
                        curY += (int)layout.GetAscent();
                        float penX = curX;
                        if (alignment.Equals(MPrintFormatItem.FIELDALIGNMENTTYPE_Center))
                            penX += (netWidth - layout.GetAdvance()) / 2;
                        else if ((alignment.Equals(MPrintFormatItem.FIELDALIGNMENTTYPE_TrailingRight) && layout.IsLeftToRight())
                            || (alignment.Equals(MPrintFormatItem.FIELDALIGNMENTTYPE_LeadingLeft) && !layout.IsLeftToRight()))
                            penX += netWidth - layout.GetAdvance();
                        //
                        if (fastDraw)
                        {	//	Bug - set Font/Color explicitly 
                            //g2D.setFont(GetFont(HEADER_ROW, col));
                            //XFont drwFont = new XFont(GetFont(HEADER_ROW, col), new XPdfFontOptions(true)); // change to draw special char
                            XFont drwFont = new XFont(GetFont(HEADER_ROW, col), new XPdfFontOptions(PdfSharp.Pdf.PdfFontEncoding.Unicode, PdfSharp.Pdf.PdfFontEmbedding.Always));
                            
                            paint.Color = XColor.FromArgb(GetColor(HEADER_ROW, col)); 
                            //g2D.setColor(GetColor(HEADER_ROW, col));
                            g2D.DrawString(iter.GetText(), drwFont, paint, penX, curY);
                        }
                        else
                            layout.Draw(g2D, (double)penX, (double)curY);										//	-> text
                        curY += (int)layout.GetDescent() + (int)layout.GetLeading();
                        usedHeight += lineHeight;
                    }
                    if (!m_multiLineHeader)			//	one line only
                        break;
                }
            }
            curX += (int)netWidth + H_GAP;
            curY += V_GAP;
            //	Y end line
            //g2D.setPaint(m_tFormat.GetVLine_Color());
            color.Color = m_tFormat.GetVLine_Color();
            color.DashStyle = m_tFormat.GetVLine_StrokeForPdf();
            //g2D.setStroke(m_tFormat.GetVLine_Stroke());
            if (m_tFormat.IsPaintVLines())					//	 -> | (right)
                g2D.DrawLine(color, curX, (int)(origY + m_tFormat.GetLineStroke()),
                    curX, (int)(origY + rowHeight - (4 * m_tFormat.GetLineStroke())));
            curX += (int)m_tFormat.GetVLineStroke();
            //	X end line
            if (m_tFormat.IsPaintHeaderLines())
            {
                color.Color = m_tFormat.GetHeaderLine_Color();
                //color.DashStyle = m_tFormat.GetHeader_Stroke();
                color.Width = (float)m_tFormat.GetHdrStroke();
                g2D.DrawLine(color, origX, curY + 5, 					//	 -> - (button)
                    (int)((origX) + colWidth - (int)m_tFormat.GetVLineStroke() + 1), curY + 5);
            }
            curY += (2 * (int)m_tFormat.GetLineStroke());	//	thick

            //	paint Data		***************************************************
            for (int row = firstRow; row < nextPageRow; row++)
            {
                rowHeight = ((int)m_rowHeights[row]);	//	includes 2*Gaps+Line
                netHeight = (float)rowHeight - (2 * V_GAP) - (float)m_tFormat.GetLineStroke();
                int rowYstart = curY;

                curX = origX;
                if (leftVline)			//	draw left | line
                {
                    color.Color = m_tFormat.GetVLine_Color();
                    color.DashStyle = m_tFormat.GetVLine_StrokeForPdf();
                    //if (m_tFormat.IsPaintBoundaryLines())
                    //    g2D.DrawLine(color, curX, rowYstart, 				//	 -> | (left)
                    //        curX, (int)(rowYstart + rowHeight - m_tFormat.GetLineStroke()));
                    curX += (int)m_tFormat.GetVLineStroke();
                }
                //	Background
                bg = GetBackground(row, col);
                if (!bg.Equals(Color.White))
                {
                    paint.Color = bg;
                    g2D.DrawRectangle(paint, curX, curY, (int)(colWidth - (float)m_tFormat.GetVLineStroke()), (int)(rowHeight - m_tFormat.GetLineStroke()));
                }
                curX += H_GAP;		//	upper left gap
                curY += V_GAP;

                //	actual data
                Object[] printItems = GetPrintItems(row, col);
                float penY = curY;
                for (int index = 0; index < printItems.Length; index++)
                {
                    if (printItems[index] == null)
                    { }
                    else if (printItems[index] is ImageElement)
                    {
                        g2D.DrawImage(((ImageElement)printItems[index]).GetImage(), curX, (int)penY);
                    }
                    else if (printItems[index] is Boolean)
                    {
                        int penX = curX + (int)((netWidth - LayoutEngine.IMAGE_SIZE.width) / 2);	//	center
                        if (((Boolean)printItems[index]))
                            g2D.DrawImage(LayoutEngine.IMAGE_TRUE, penX, (int)penY);
                        else
                            g2D.DrawImage(LayoutEngine.IMAGE_FALSE, penX, (int)penY);
                        penY += LayoutEngine.IMAGE_SIZE.height;
                    }
                    else
                    {
                        String str = printItems[index].ToString();
                        if (DEBUG_PRINT)
                            log.Fine("row=" + row + ",col=" + col + " - " + str + " 8Bit=" + Utility.Util.Is8Bit(str));
                        if (str.Length > 0)
                        {
                            usedHeight = 0;
                           // String[] lines = java.util.regex.Pattern.compile("$", java.util.regex.Pattern.MULTILINE).split(str);
                            String[] lines = System.Text.RegularExpressions.Regex.Split(str, "$", System.Text.RegularExpressions.RegexOptions.Multiline);
                            for (int lineNo = 0; lineNo < lines.Length; lineNo++)
                            {
                                if (string.IsNullOrEmpty(lines[lineNo]))
                                {
                                    continue;
                                }
                                aString = new AttributedString(lines[lineNo]);
                                aString.AddAttribute(TextAttribute.FONT, GetFont(row, col));
                                if (isView && printItems[index] is NamePair)	//	ID
                                {
                                    aString.AddAttribute(TextAttribute.FOREGROUND, LINK_COLOR);
                                    aString.AddAttribute(TextAttribute.UNDERLINE, TextAttribute.UNDERLINE_LOW_ONE_PIXEL, 0, lines[lineNo].Length);
                                }
                                else
                                    aString.AddAttribute(TextAttribute.FOREGROUND, GetColor(row, col));
                                //
                                iter = aString.GetIterator();
                                bool fastDraw = LayoutEngine.s_FASTDRAW;
                                if (fastDraw && !isView && !Utility.Util.Is8Bit(lines[lineNo]))
                                    fastDraw = false;
                                measurer = new LineBreakMeasurer(iter);
                                while (measurer.GetPosition() < iter.GetEndIndex())		//	print element
                                {
                                    TextLayout layout = measurer.NextLayout(netWidth + 2);
                                    if (iter.GetEndIndex() != measurer.GetPosition())
                                        fastDraw = false;
                                    float lineHeight = layout.GetAscent() + layout.GetDescent() + layout.GetLeading();
                                    if ((m_columnMaxHeight[col] <= 0
                                            || (usedHeight + lineHeight) <= m_columnMaxHeight[col])
                                        && (usedHeight + lineHeight) <= netHeight)
                                    {
                                        if (alignment.Equals(MPrintFormatItem.FIELDALIGNMENTTYPE_Block) && measurer.GetPosition() < iter.GetEndIndex())
                                        {
                                            //layout = layout.getJustifiedLayout(netWidth + 2);
                                            fastDraw = false;
                                        }
                                        penY += layout.GetAscent();
                                        float penX = curX;
                                        if (alignment.Equals(MPrintFormatItem.FIELDALIGNMENTTYPE_Center))
                                            penX += (netWidth - layout.GetAdvance()) / 2;
                                        else if ((alignment.Equals(MPrintFormatItem.FIELDALIGNMENTTYPE_TrailingRight) && layout.IsLeftToRight())
                                            || (alignment.Equals(MPrintFormatItem.FIELDALIGNMENTTYPE_LeadingLeft) && !layout.IsLeftToRight()))
                                            penX += netWidth - layout.GetAdvance();
                                        //

                                        if (fastDraw)
                                        {	//	Bug - set Font/Color explicitly
                                            //g2D.setFont(getFont(row, col));
                                            //XFont drwFont = new XFont(GetFont(row, col), new XPdfFontOptions(true); // change to draw special char
                                            XFont drwFont = new XFont(GetFont(row, col), new XPdfFontOptions(PdfSharp.Pdf.PdfFontEncoding.Unicode,PdfSharp.Pdf.PdfFontEmbedding.Always));
                                            
                                            if (isView && printItems[index] is NamePair)	//	ID
                                            {
                                                paint.Color = XColor.FromArgb(LINK_COLOR);
                                                //	TextAttribute.UNDERLINE
                                            }
                                            else
                                                paint.Color = XColor.FromArgb(GetColor(row, col));

                                            g2D.DrawString(iter.GetText(), drwFont, paint, penX, penY );
                                        }
                                        else
                                            layout.Draw(g2D, (double)penX, (double)penY);										//	-> text
                                        if (DEBUG_PRINT)
                                            log.Fine("row=" + row + ",col=" + col + " - " + str + " - x=" + penX + ",y=" + penY);
                                        penY += layout.GetDescent() + layout.GetLeading();
                                        usedHeight += lineHeight;
                                        //
                                        if (m_columnMaxHeight[col] == -1)	//	FirstLineOny
                                            break;
                                    }
                                }	//	print element
                            }	//	for all lines
                        }	//	length > 0
                    }
                }   //for all print items

                curY += (int)netHeight + V_GAP;
                curX += (int)netWidth + H_GAP;
                //	Y end line
                color.Color = XColor.FromArgb(m_tFormat.GetVLine_Color());
                color.DashStyle = m_tFormat.GetVLine_StrokeForPdf();
                if (m_tFormat.IsPaintVLines())
                    g2D.DrawLine(color, curX, rowYstart, 				//	 -> | (right)
                        curX, (int)(rowYstart + rowHeight - m_tFormat.GetLineStroke()));
                curX += (int)m_tFormat.GetVLineStroke();

                //	X end line
                if (row == m_data.GetLength(0) - 1)			//	last Line
                {
                    color.Color = XColor.FromArgb(m_tFormat.GetHeaderLine_Color());
                    color.DashStyle = m_tFormat.GetHeader_StrokePdf();
                    color.Width = (float)m_tFormat.GetHdrStroke();
                    g2D.DrawLine(color, origX, curY,					//	 -> - (last line) 
                        (int)(origX + (int)colWidth - (int)m_tFormat.GetVLineStroke() + 1), curY);
                    curY += (2 * (int)m_tFormat.GetLineStroke());	//	thick
                }
                else
                {
                    //	next line is a funcion column -> underline this
                    bool nextIsFunction = m_functionRows.Contains(row + 1);
                    if (nextIsFunction && m_functionRows.Contains(row))
                        nextIsFunction = false;		//	this is a function line too
                    if (nextIsFunction)
                    {
                        color.Color = XColor.FromArgb(m_tFormat.GetFunctFG_Color());
                        color.DashStyle = m_tFormat.GetHLine_StrokePdf();
                        g2D.DrawLine(color, origX, curY, 				//	 -> - (bottom)
                            (int)(origX + (int)colWidth - (int)m_tFormat.GetVLineStroke()), curY);
                    }
                    else if (m_tFormat.IsPaintHLines())
                    {
                        color.Color = XColor.FromArgb(m_tFormat.GetHLine_Color());
                        color.DashStyle = m_tFormat.GetHLine_StrokePdf();

                        g2D.DrawLine(color, origX, curY, 				//	 -> - (bottom)
                            (int)(origX + (int)colWidth - (int)m_tFormat.GetVLineStroke()), curY);
                    }
                    curY += (int)m_tFormat.GetLineStroke();
                }
            }
            //color.Dispose();
            //paint.Dispose();
        }

    }
}
