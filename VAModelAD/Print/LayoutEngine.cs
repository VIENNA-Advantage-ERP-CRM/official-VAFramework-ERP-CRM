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

using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using System.IO;
using System.Web;


namespace VAdvantage.Print
{
    /// <summary>
    /// Layout Engine for Reporting. Fixes the Layout
    /// </summary>
    public class LayoutEngine : IPrintable
    {
        public LayoutEngine(MPrintFormat format, PrintData data, Query query)
        {
            log.Info(format + " - " + data + " - " + query);
            SetPrintFormat(format, false);
            SetPrintData(data, query, false);
            Layout();
        }

        public LayoutEngine(MPrintFormat format, PrintData data, Query query, int AD_Org_ID)
        {
            _ad_org_id = AD_Org_ID;
            log.Info(format + " - " + data + " - " + query);
            SetPrintFormat(format, false);
            SetPrintData(data, query, false);
            Layout();
        }

        /*************************************************************************/

        /**	Logger						*/
        private static VLogger log = VLogger.GetVLogger(typeof(LayoutEngine).FullName);
        /** Existing Layout				*/
        private bool m_hasLayout = false;
        /**	The Format					*/
        private MPrintFormat m_format;
        /**	Print Context				*/
        private Ctx m_printCtx;
        /** The Data					*/
        private PrintData m_data;
        /** The Query (parameter		*/
        private Query m_query;
        /**	Default Color				*/
        private MPrintColor m_printColor;
        /**	Default Font				*/
        private MPrintFont m_printFont;
        /**	Printed Column Count		*/
        private int m_columnCount = -1;

        //AMenu
        /**	Paper - default: standard portrait		*/
        private CPaper m_paper;
        /**	Header Area Height (1/4")				*/
        private int m_headerHeight = 18;		//	1/4" => 72/4
        /** Footer Area Height (1/4")				*/
        private int m_footerHeight = 18;


        /**	Current Page Number			*/
        private int m_pageNo = 0;
        /** Current Page				*/
        private Page m_currPage;
        /** Pages						*/
        private List<Page> m_pages = new List<Page>();
        /**	Header&Footer for all pages	*/
        private HeaderFooter m_headerFooter;


        /**	Header Coordinates			*/
        private Rectangle m_header = new Rectangle();
        /** Content Coordinates			*/
        private Rectangle m_content = new Rectangle();
        /** Footer Coordinates			*/
        private Rectangle m_footer = new Rectangle();
        /** Temporary NL Position		*/
        private int m_tempNLPositon = 0;

        /** Header Area					*/
        public static int AREA_HEADER = 0;
        /** Content Area				*/
        public static int AREA_CONTENT = 1;
        /** Footer Area					*/
        public static int AREA_FOOTER = 2;
        /** Area Pointer				*/
        private int m_area = AREA_CONTENT;

        /** Current Position in 1/72 inch	*/
        private PointF[] m_position = new PointF[] { new Point(0, 0), new Point(0, 0), new Point(0, 0) };
        /** Max Height Since New Line		*/
        private float[] m_maxHeightSinceNewLine = new float[] { 0f, 0f, 0f };

        /**	Primary Table Element for Page XY Info	*/
        private TableElement m_tableElement = null;

        /**	Last Height	by area				*/
        private float[] m_lastHeight = new float[] { 0f, 0f, 0f };
        /** Last Width by area				*/
        private float[] m_lastWidth = new float[] { 0f, 0f, 0f };

        /**	Draw using attributed String vs. Text Layout where possible */
        public static bool s_FASTDRAW = true;
        /** Print Copy (print interface)	*/
        private bool m_isCopy = false;

        /** Image Size				*/
        public static java.awt.Dimension IMAGE_SIZE = new java.awt.Dimension(10, 10);

        /** True Image				*/
        public static Image IMAGE_TRUE = CoreLibrary.Properties.Resource.tick1;
        /** False Image				*/
        public static Image IMAGE_FALSE = CoreLibrary.Properties.Resource.cross;

        /*************************************************************************/


        private int _ad_org_id = 0;

        /// <summary>
        /// Set Print Format
        /// <para>
        /// ptionally re-calculate layout
        /// </para>
        /// </summary>
        /// <param name="format">if layout exists, redo it</param>
        /// <param name="doLayout">print Format</param>
        public void SetPrintFormat(MPrintFormat format, bool doLayout)
        {
            m_format = format;
            //	Initial & Default Settings
            //m_printCtx = format.GetCtx();
            m_printCtx = new Ctx(format.GetCtx().GetMap());
            //	Set Paper
            bool tempHasLayout = m_hasLayout;
            m_hasLayout = false;	//	do not start re-calculation
            MPrintPaper mPaper = MPrintPaper.Get(format.GetAD_PrintPaper_ID());
            if (m_format.IsStandardHeaderFooter())
                SetPaper(mPaper.GetCPaper());
            else
                SetPaper(mPaper.GetCPaper(),
                    m_format.GetHeaderMargin(), m_format.GetFooterMargin());
            m_hasLayout = tempHasLayout;
            //
            m_printColor = MPrintColor.Get(GetCtx(), format.GetAD_PrintColor_ID());
            m_printFont = MPrintFont.Get(format.GetAD_PrintFont_ID());

            //	Print Context
            m_printCtx.SetContext(Page.CONTEXT_REPORTNAME, m_format.GetName());
            m_printCtx.SetContext(Page.CONTEXT_HEADER, Env.GetHeader(GetCtx(), 0));
            m_printCtx.SetContext(Env.LANGUAGE, m_format.GetLanguage().GetAD_Language());

            if (m_hasLayout && doLayout)
                Layout();			//	re-calculate
        }	//	setPrintFormat

        /// <summary>
        /// Calculate Page size based on Paper and header/footerHeight.
        /// <para>
        /// Paper: 8.5x11.0" Portrait x=32.0,y=32.0 w=548.0,h=728.0
        /// <para>+------------------------ Paper   612x792</para>
        /// <para>|    non-imageable space          32x32</para>
        /// <para>|  +--------------------- Header = printable area start</para>
        /// <para>|  | headerHeight=32      =>  [x=32,y=32,width=548,height=32]</para>
        /// <para>|  +--------------------- Content</para>
        /// <para>|  |                      =>  [x=32,y=64,width=548,height=664]</para>
        /// <para>|  |</para>
        /// <para>|  |</para>
        /// <para>|  |</para>
        /// <para>|  +--------------------- Footer</para>
        /// <para>|  | footerHeight=32      =>  [x=32,y=728,width=548,height=32]</para>
        /// <para>|  +--------------------- Footer end = printable area end</para>
        /// <para>|   non-imageable space</para>
        /// <para>+------------------------</para>
        /// </para>
        /// </summary>
        private void CalculatePageSize()
        {
            int x = (int)m_paper.GetImageableX(true);
            int w = (int)m_paper.GetImageableWidth(true);
            //
            int y = (int)m_paper.GetImageableY(true);
            int h = (int)m_paper.GetImageableHeight(true);

            int height = m_headerHeight;
            m_header.X = x;
            m_header.Y = y;
            m_header.Width = w;
            m_header.Height = height;
            //
            y += height;
            height = h - m_headerHeight - m_footerHeight;
            m_content.X = x;
            m_content.Y = y;
            m_content.Width = w;
            m_content.Height = height;
            //
            y += height;
            height = m_footerHeight;
            m_footer.X = x;
            m_footer.Y = y;
            m_footer.Width = w;
            m_footer.Height = height;

            log.Fine("Paper=" + m_paper + ",HeaderHeight=" + m_headerHeight + ",FooterHeight=" + m_footerHeight
                + " => Header=" + m_header + ",Contents=" + m_content + ",Footer=" + m_footer);
        }	//	calculatePageSize

        /// <summary>
        /// Set PrintData.
        /// Optionally re-calculate layout
        /// </summary>
        /// <param name="data"></param>
        /// <param name="query"></param>
        /// <param name="doLayout"></param>
        public void SetPrintData(PrintData data, Query query, bool doLayout)
        {
            m_data = data;
            m_query = query;
            if (m_hasLayout && doLayout)
            {
                Layout();			//	re-calculate

            }
        }	//	setPrintData

        public void SetPaper(CPaper paper)
        {
            SetPaper(paper, m_headerHeight, m_footerHeight);
        }	//	setPaper

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paper"></param>
        /// <param name="headerHeight"></param>
        /// <param name="footerHeight"></param>
        public void SetPaper(CPaper paper, int headerHeight, int footerHeight)
        {
            if (paper == null)
                return;
            //
            bool paperChange = headerHeight != m_headerHeight || footerHeight != m_footerHeight;
            if (!paperChange)
                paperChange = !paper.Equals(m_paper);
            //
            log.Fine(paper + " - Header=" + headerHeight + ", Footer=" + footerHeight);
            m_paper = paper;
            m_headerHeight = headerHeight;
            m_footerHeight = footerHeight;
            CalculatePageSize();
            //
            if (m_hasLayout && paperChange)
                Layout();			//	re-calculate
        }	//	setPaper

        protected void SetPageFormat(PageFormat pf)
        {
            if (pf != null)
                SetPaper(new CPaper(pf));
            else
                SetPaper(null);
        }	//	setPageFormat

        public PageFormat GetPageFormat()
        {
            return m_paper.GetPageFormat();
        }	//	getPageFormat

        public CPaper GetPaper()
        {
            //MClient
            return m_paper;
        }	//	getPaper

        private PrintElement LayoutParameter()
        {
            if (m_query == null || !m_query.IsActive())
                return null;
            //

            ParameterElement pe = new ParameterElement(m_query, m_printCtx, m_format.GetTableFormat(), out paraListHtml);
            pe.Layout(0, 0, false, null);
            return pe;
        }	//	layoutParameter

        private void Layout()
        {
            finalHtml = new StringBuilder("<div class='vis-attach-main-wrap' style='background: white;'>");
            //	Header/Footer
            m_headerFooter = new HeaderFooter(m_printCtx);
            //m_pdfheaderFooter = new PdfHeaderFooter(m_printCtx);
            if (!m_format.IsForm() && m_format.IsStandardHeaderFooter())
            {
                CreateStandardHeaderFooter();
            }

            m_pageNo = 0;
            m_pages.Clear();
            m_tableElement = null;
            NewPage(true, false);	//	initialize

            //
            if (m_format.IsForm())
            {
                LayoutForm();
            }
            else
            {
                //	Parameter
                
                PrintElement element = LayoutParameter();
                if (element != null)
                {
                   
                    m_currPage.AddElement(element);
                    element.SetLocation(m_position[AREA_CONTENT]);
                    m_position[AREA_CONTENT].Y += (int)element.GetHeight() + 9;	//	GAP
                }
                //	Table
                if (m_data != null)
                {
                    if (element == null)
                        m_position[AREA_CONTENT].Y += 9;	//	GAP in case there are no parameter list

                    element = LayoutTable(m_format, m_data, 0);
                    element.SetLocation(m_content.Location);
                    element = (TableElement)element;
                    for (int p = 1; p <= ((TableElement)element).GetPageCount(); p++)
                    {
                        if (p != 1)
                        {
                            NewPage(true, false);                          
                        }
                        
                        m_currPage.AddElement(element);
                    }
                    for (int i = 0; i < dynamicPageHtml.Length; i++)
                    {
                        finalHtml.Append(newPageHtml + headerHtml );
                        ReplaceFirstOccurrence("@*Page@", (i+1).ToString());
                        ReplaceFirstOccurrence("@*PageCount@", dynamicPageHtml.Length.ToString());
                        if (i == 0)
                        {
                            finalHtml.Append(paraListHtml);
                        }
                        finalHtml.Append(dynamicPageHtml[i]);
                        finalHtml.Append(footerHtml + endPageHtml);
                    }
                }
            }
            //
            String pageInfo = (m_pages.Count() + GetPageInfo(m_pages.Count())).ToString();
            m_printCtx.SetContext(Page.CONTEXT_PAGECOUNT, pageInfo);
            DateTime now = DateTime.Now;
            m_printCtx.SetContext(Page.CONTEXT_DATE, DisplayType.GetDateFormat(DisplayType.Date).Format(now));
            m_printCtx.SetContext(Page.CONTEXT_TIME,
                DisplayType.GetDateFormat(DisplayType.DateTime).Format(now));
            //	Update Page Info
            int pages = m_pages.Count();
            for (int i = 0; i < pages; i++)
            {
                //StringElement
                Page page = (Page)m_pages[i];                
                int pageNo = page.GetPageNo();
                pageInfo = (pageNo + GetPageInfo(pageNo)).ToString();
                page.SetPageInfo(pageInfo);
                page.SetPageCount(pages);
               
            }

            finalHtml.Append("</div>");
            finalHtml.Replace("@*ReportName@", this.m_format.GetName());
            //finalHtml.Replace("@*PageCount@ ", pages.ToString());
            m_hasLayout = true;
        }	//	layout

        public StringBuilder GetRptHtml()
        {
            return finalHtml;
        }
        public void ReplaceFirstOccurrence(string oldValue, string newValue)
        {
            int i = finalHtml.ToString().IndexOf(oldValue);
            if (i < 0)
            {
                return;
            }
            finalHtml = new StringBuilder(finalHtml.ToString().Remove(i, oldValue.Length).Insert(i, newValue));
        }
        private void LayoutForm()
        {
            //	log.info("layoutForm");
            m_columnCount = 0;
            if (m_data == null)
                return;
            //	for every row
            for (int row = 0; row < m_data.GetRowCount(); row++)
            {
                log.Info("Row=" + row);
                m_data.SetRowIndex(row);
                bool somethingPrinted = true;	//	prevent NL of nothing printed and supress null
                //	for every item

                for (int i = 0; i < m_format.GetItemCount(); i++)
                {
                    MPrintFormatItem item = m_format.GetItem(i);

                    //	log.fine("layoutForm - Row=" + row + " - #" + i + " - " + item);
                    if (!item.IsPrinted())
                        continue;
                    //	log.fine("layoutForm - Row=" + row + " - #" + i + " - " + item);
                    m_columnCount++;
                    //	Read Header/Footer just once
                    if (row > 0 && (item.IsHeader() || item.IsFooter()))
                        continue;
                    //	Position
                    if (item.IsHeader())			//	Area
                        SetArea(AREA_HEADER);
                    else if (item.IsFooter())
                        SetArea(AREA_FOOTER);
                    else
                        SetArea(AREA_CONTENT);
                    //
                    if (item.IsSetNLPosition() && item.IsRelativePosition())
                        m_tempNLPositon = 0;
                    //	New Page/Line
                    if (item.IsNextPage())			//	item.isPageBreak()			//	new page
                        NewPage(false, false);
                    else if (item.IsNextLine() && somethingPrinted)		//	new line
                    {
                        NewLine();
                        somethingPrinted = false;
                    }
                    else
                        AddX(m_lastWidth[m_area]);
                    //	Relative Position space
                    if (item.IsRelativePosition())
                    {
                        AddX(item.GetXSpace());
                        AddY(item.GetYSpace());
                    }
                    else	//	Absolute relative position
                        SetRelativePosition((int)item.GetXPosition(), (int)item.GetYPosition());
                    //	Temporary NL Position when absolute positioned
                    if (item.IsSetNLPosition() && !item.IsRelativePosition())
                        m_tempNLPositon = (int)GetPosition().X;

                    //	line alignment
                    String alignment = item.GetFieldAlignmentType();
                    int maxWidth = item.GetMaxWidth();
                    bool lineAligned = false;
                    if (item.IsRelativePosition())
                    {
                        if (item.IsLineAlignLeading())
                        {
                            alignment = MPrintFormatItem.FIELDALIGNMENTTYPE_LeadingLeft;
                            maxWidth = GetAreaBounds().Width;
                            lineAligned = true;
                        }
                        else if (item.IsLineAlignCenter())
                        {
                            alignment = MPrintFormatItem.FIELDALIGNMENTTYPE_Center;
                            maxWidth = GetAreaBounds().Width;
                            lineAligned = true;
                        }
                        else if (item.IsLineAlignTrailing())
                        {
                            alignment = MPrintFormatItem.FIELDALIGNMENTTYPE_TrailingRight;
                            maxWidth = GetAreaBounds().Width;
                            lineAligned = true;
                        }
                    }

                    //	Type
                    PrintElement element = null;
                    if (item.IsTypePrintFormat())		//** included PrintFormat
                    {
                        element = IncludeFormat(item, m_data);
                    }
                    else if (item.IsBarcode())
                    {
                        //element = createBarcodeElement(item);
                        //element.layout(maxWidth, item.getMaxHeight(), false, alignment);
                    }
                    else if (item.IsTypeImage())		//**	Image
                    {

                        if (!item.IsOrgLogo())
                        {
                            if (item.IsImageField())
                                element = CreateImageElement(item);
                            else if (item.IsImageIsAttached())
                                element = ImageElement.Get(item.Get_ID());
                            else
                            {
                                element = ImageElement.Get(CoreLibrary.Properties.Resource.vienna);
                                element.Layout(maxWidth, item.GetMaxHeight(), false, alignment);
                            }
                        }
                        else
                        {
                            ///////////change To pickImage on the Bases of Organisations////////
                            //object logo = DB.ExecuteScalar("SELECT LOGO FROM AD_OrgInfo WHERE ISACTIVE='Y' AND AD_Org_ID=" + GetCtx().GetAD_Org_ID());
                            if (_ad_org_id > -1)
                            {
                                object logo = DB.ExecuteScalar("SELECT LOGO FROM AD_OrgInfo WHERE ISACTIVE='Y' AND AD_Org_ID=" + _ad_org_id);
                                if (logo != null && logo != DBNull.Value)
                                {
                                    MemoryStream ms = new MemoryStream((Byte[])logo);
                                    Image img = Image.FromStream(ms);
                                    element = ImageElement.Get(img);
                                }
                                /////////////////
                                else
                                {
                                    element = ImageElement.Get(CoreLibrary.Properties.Resource.vienna);
                                }
                            }
                            else
                            {
                                element = ImageElement.Get(CoreLibrary.Properties.Resource.vienna);
                            }
                            element.Layout(maxWidth, item.GetMaxHeight(), false, alignment);

                        }

                    }
                    else if (item.IsTypeField())		//**	Field
                    {
                        if (maxWidth == 0 && item.IsFieldAlignBlock())
                            maxWidth = GetAreaBounds().Width;
                        element = CreateFieldElement(item, maxWidth, alignment, m_format.IsForm());
                    }
                    else if (item.IsTypeBox())			//**	Line/Box
                    {
                        if (m_format.IsForm())
                            element = CreateBoxElement(item);
                    }
                    else	//	(item.isTypeText())		//**	Text
                    {
                        if (maxWidth == 0 && item.IsFieldAlignBlock())
                            maxWidth = GetAreaBounds().Width;
                        element = CreateStringElement(item.GetPrintName(m_format.GetLanguage()),
                            item.GetAD_PrintColor_ID(), item.GetAD_PrintFont_ID(),
                            maxWidth, item.GetMaxHeight(), item.IsHeightOneLine(), alignment, true);
                    }

                    //	Printed - set last width/height
                    if (element != null)
                    {
                        somethingPrinted = true;
                        if (!lineAligned)
                            m_lastWidth[m_area] = element.GetWidth();
                        m_lastHeight[m_area] = element.GetHeight();
                    }
                    else
                    {
                        somethingPrinted = false;
                        m_lastWidth[m_area] = 0f;
                        m_lastHeight[m_area] = 0f;
                    }

                    //	Does it fit?
                    if (item.IsRelativePosition() && !lineAligned)
                    {
                        if (!IsXspaceFor(m_lastWidth[m_area]))
                        {
                            log.Finest("Not enough X space for "
                                    + m_lastWidth[m_area] + " - remaining " + GetXspace() + " - Area=" + m_area);
                            NewLine();
                        }
                        if (m_area == AREA_CONTENT && !IsYspaceFor(m_lastHeight[m_area]))
                        {
                            log.Finest("Not enough Y space "
                                    + m_lastHeight[m_area] + " - remaining " + GetYspace() + " - Area=" + m_area);
                            NewPage(true, true);
                        }
                    }
                    //	We know Position and Size
                    //	log.fine( "LayoutEngine.layoutForm",
                    //		"Page=" + m_pageNo + " [" + m_area + "] " + m_position[m_area].x + "/" + m_position[m_area].y
                    //		+ " w=" + lastWidth[m_area] + ",h=" + lastHeight[m_area] + " " + item);
                    if (element != null)
                        element.SetLocation(m_position[m_area]);
                    //	Add to Area
                    if (m_area == AREA_CONTENT)
                        m_currPage.AddElement(element);
                    else
                        m_headerFooter.AddElement(element);
                    //
                    if (m_lastHeight[m_area] > m_maxHeightSinceNewLine[m_area])
                        m_maxHeightSinceNewLine[m_area] = m_lastHeight[m_area];

                }	//	for every item



            }	//	for every row
        }	//	layoutForm

        private PrintElement CreateStringElement(String content, int AD_PrintColor_ID, int AD_PrintFont_ID,
    int maxWidth, int maxHeight, bool isHeightOneLine, String FieldAlignmentType, bool isTranslated)
        {
            if (content == null || content.Length == 0)
                return null;
            //	Color / Font
            Color color = GetColor();	//	default
            if (AD_PrintColor_ID != 0 && m_printColor.Get_ID() != AD_PrintColor_ID)
            {
                MPrintColor c = MPrintColor.Get(GetCtx(), AD_PrintColor_ID);
                if (c.GetColor() != null)
                    color = c.GetColor();
            }
            Font font = m_printFont.GetFont();		//	default
            if (AD_PrintFont_ID != 0 && m_printFont.Get_ID() != AD_PrintFont_ID)
            {
                MPrintFont f = MPrintFont.Get(AD_PrintFont_ID);
                if (f.GetFont() != null)
                    font = f.GetFont();
            }
            PrintElement e = new StringElement(content, font, color, null, isTranslated);
            e.Layout(maxWidth, maxHeight, isHeightOneLine, FieldAlignmentType);
            return e;
        }	//	createStringElement

        /// <summary>
        /// Create Image Element from item
        /// </summary>
        /// <param name="item">item</param>
        /// <returns>image element</returns>
        private PrintElement CreateImageElement(MPrintFormatItem item)
        {
            Object obj = m_data.GetNode(item.GetAD_Column_ID());
            if (obj == null)
                return null;
            else if (obj is PrintDataElement)
            { }
            else
            {
                log.Log(Level.SEVERE, "Element not PrintDataElement " + obj.GetType().FullName);
                return null;
            }

            PrintDataElement data = (PrintDataElement)obj;
            if (data.IsNull() && item.IsSuppressNull())
                return null;
            String url = data.GetValueDisplay(m_format.GetLanguage());
            if ((url == null || url.Length == 0))
            {
                if (item.IsSuppressNull())
                    return null;
                else	//	should create an empty area
                    return null;
            }
            ImageElement element = ImageElement.Get(url);
            return element;
        }	//	createImageElement

        /// <summary>
        /// Create Box/Line Element
        /// </summary>
        /// <param name="item">item</param>
        /// <returns>BoxElement</returns>
        private PrintElement CreateBoxElement(MPrintFormatItem item)
        {
            Color color = GetColor();	//	default
            if (item.GetAD_PrintColor_ID() != 0
                && m_printColor.Get_ID() != item.GetAD_PrintColor_ID())
            {
                MPrintColor c = MPrintColor.Get(GetCtx(), item.GetAD_PrintColor_ID());
                if (c.GetColor() != null)
                    color = c.GetColor();
            }
            return new BoxElement(item, color);
        }	//	createBoxElement

        public Color GetColor()
        {
            if (m_printColor == null)
                return Color.Black;
            return m_printColor.GetColor();
        }	//	getColor

        /// <summary>
        /// Creates Field Element
        /// </summary>
        /// <param name="item"></param>
        /// <param name="maxWidth"></param>
        /// <param name="FieldAlignmentType"></param>
        /// <param name="isForm"></param>
        /// <returns></returns>
        private PrintElement CreateFieldElement(MPrintFormatItem item, int maxWidth,
            String FieldAlignmentType, bool isForm)
        {
            //	Get Data
            Object obj = m_data.GetNode(item.GetAD_Column_ID(), false);
            if (obj == null)
                return null;
            else if (obj is PrintDataElement)
            { }
            else
            {
                log.Log(Level.SEVERE, "Element not PrintDataElement " + obj.GetType().FullName);
                return null;
            }

            //	Convert DataElement to String
            PrintDataElement data = (PrintDataElement)obj;
            if (data.IsNull() && item.IsSuppressNull())
                return null;
            String stringContent = data.GetValueDisplay(m_format.GetLanguage());
            if ((stringContent == null || stringContent.Length == 0) && item.IsSuppressNull())
                return null;
            //	non-string
            Object content = stringContent;
            if (data.GetValue() is bool)
                content = data.GetValue();

            //	Convert AmtInWords Content to alpha
            if (item.GetColumnName().Equals("AmtInWords"))
            {
                log.Fine("AmtInWords: " + stringContent);
                stringContent = Msg.GetAmtInWords(m_format.GetLanguage(), stringContent);
                content = stringContent;
            }
            //	Label
            String label = item.GetPrintName(m_format.GetLanguage());
            String labelSuffix = item.GetPrintNameSuffix(m_format.GetLanguage());

            //	ID Type
            NamePair ID = null;
            if (data.IsID())
            {	//	Record_ID/ColumnName
                Object value = data.GetValue();
                if (value is KeyNamePair)
                    ID = new KeyNamePair(((KeyNamePair)value).GetKey(), item.GetColumnName());
                else if (value is ValueNamePair)
                    ID = new ValueNamePair(((ValueNamePair)value).GetValue(), item.GetColumnName());
            }
            else if (X_AD_PrintFormatItem.FIELDALIGNMENTTYPE_Default.Equals(FieldAlignmentType))
            {
                if (data.IsNumeric())
                    FieldAlignmentType = X_AD_PrintFormatItem.FIELDALIGNMENTTYPE_TrailingRight;
                else
                    FieldAlignmentType = X_AD_PrintFormatItem.FIELDALIGNMENTTYPE_LeadingLeft;
            }

            //	Get Color/ Font
            Color color = GetColor();	//	default
            if (ID != null && !isForm)
            { }								//	link color/underline handeled in PrintElement classes
            else if (item.GetAD_PrintColor_ID() != 0 && m_printColor.Get_ID() != item.GetAD_PrintColor_ID())
            {
                MPrintColor c = MPrintColor.Get(GetCtx(), item.GetAD_PrintColor_ID());
                if (c.GetColor() != null)
                    color = c.GetColor();
            }

            Font font = m_printFont.GetFont();		//	default
            if (item.GetAD_PrintFont_ID() != 0 && m_printFont.Get_ID() != item.GetAD_PrintFont_ID())
            {
                MPrintFont f = MPrintFont.Get(item.GetAD_PrintFont_ID());
                if (f.GetFont() != null)
                    font = f.GetFont();
            }

            //	Create String, HTML or Location
            PrintElement e = null;
            if (data.GetDisplayType() == DisplayType.Location)
            {
                e = new LocationElement(m_printCtx, ((KeyNamePair)ID).GetKey(), font, color);
                e.Layout(maxWidth, item.GetMaxHeight(), item.IsHeightOneLine(), FieldAlignmentType);
            }
            else
            {
                if (HTMLElement.IsHTML(stringContent))
                    e = new HTMLElement(stringContent);
                else
                    e = new StringElement(content, font, color, isForm ? null : ID, label, labelSuffix);
                e.Layout(maxWidth, item.GetMaxHeight(), item.IsHeightOneLine(), FieldAlignmentType);
            }
            return e;
        }	//	createFieldElement

        /// <summary>
        /// Include Table Format
        /// </summary>
        /// <param name="item">print format item</param>
        /// <param name="data">print data</param>
        /// <returns>Print Element</returns>
        private PrintElement IncludeFormat(MPrintFormatItem item, PrintData data)
        {
            NewLine();
            PrintElement element = null;
            //
            MPrintFormat format = MPrintFormat.Get(GetCtx(), item.GetAD_PrintFormatChild_ID(), false);
            format.SetLanguage(m_format.GetLanguage());
            if (m_format.IsTranslationView())
                format.SetTranslationLanguage(m_format.GetLanguage());
            int AD_Column_ID = item.GetAD_Column_ID();
            log.Info(format + " - Item=" + item.GetName() + " (" + AD_Column_ID + ")");
            //
            Object obj = data.GetNode(AD_Column_ID, false);
            //	Object obj = data.getNode(item.getColumnName());	//	slower
            if (obj == null)
            {
                data.DumpHeader();
                data.DumpCurrentRow();
                log.Log(Level.SEVERE, "No Node - AD_Column_ID=" + AD_Column_ID + " - " + item + " - " + data);
                return null;
            }
            PrintDataElement dataElement = (PrintDataElement)obj;
            String recordString = dataElement.GetValueKey();
            if (recordString == null || recordString.Length == 0)
            {
                data.DumpHeader();
                data.DumpCurrentRow();
                log.Log(Level.SEVERE, "No Record Key - " + dataElement
                    + " - AD_Column_ID=" + AD_Column_ID + " - " + item);
                return null;
            }
            int Record_ID = 0;
            try
            {
                Record_ID = int.Parse(recordString);
            }
            catch (Exception e)
            {
                data.DumpCurrentRow();
                log.Log(Level.SEVERE, "Invalid Record Key - " + recordString
                    + " (" + e.Message
                    + ") - AD_Column_ID=" + AD_Column_ID + " - " + item);
                return null;
            }
            Query query = new Query(format.GetAD_Table_ID());
            query.AddRestriction(item.GetColumnName(), Query.EQUAL, Record_ID);
            format.SetTranslationViewQuery(query);
            log.Fine(query.ToString());
            //
            DataEngine de = new DataEngine(format.GetLanguage());
            PrintData includedData = de.GetPrintData(data.GetCtx(), format, query);
            log.Fine(includedData.ToString());
            if (includedData == null)
                return null;
            //
            element = LayoutTable(format, includedData, item.GetXSpace());
            //	handle multi page tables
            if (((TableElement)element).GetPageCount() > 1)
            {
                PointF loc = m_position[m_area];
                element.SetLocation(loc);
                for (int p = 1; p < ((TableElement)element).GetPageCount(); p++)	//	don't add last one
                {
                    m_currPage.AddElement(element);
                    NewPage(true, false);
                }
                //m_position[m_area] = loc; //commented by jagmohan on 12-jul-2011. loc is not a reference type in .net
                ((TableElement)element).SetHeightToLastPage();
            }

            m_lastWidth[m_area] = element.GetWidth();
            m_lastHeight[m_area] = element.GetHeight();

            if (!IsXspaceFor(m_lastWidth[m_area]))
            {
                log.Finest("Not enough X space for "
                        + m_lastWidth[m_area] + " - remaining " + GetXspace() + " - Area=" + m_area);
                NewLine();
            }
            if (m_area == AREA_CONTENT && !IsYspaceFor(m_lastHeight[m_area]))
            {
                log.Finest("Not enough Y space "
                        + m_lastHeight[m_area] + " - remaining " + GetYspace() + " - Area=" + m_area);
                NewPage(true, false);
            }
            //
            return element;
        }	//	includeFormat


        public int GetColumnCount()
        {
            return m_columnCount;
        }	//	getColumnCount

        protected void SetArea(int area)
        {
            if (m_area == area)
                return;
            if (area < 0 || area > 2)
                throw new IndexOutOfRangeException(area.ToString());
            m_area = area;
        }	//	setArea

        public int GetArea()
        {
            return m_area;
        }	//	getArea

        /**
         * 	Return bounds of current Area
         * 	@return rectangle with bounds
         */
        public Rectangle GetAreaBounds()
        {
            Rectangle part = m_content;
            if (m_area == AREA_HEADER)
                part = m_header;
            else if (m_area == AREA_FOOTER)
                part = m_footer;
            //
            return part;
        }	//	getAreaBounds


        protected int NewPage(bool force, bool preserveXPos)
        {
            //	We are on a new page
            if (!force
                && m_position[AREA_CONTENT].X == m_content.X
                && m_position[AREA_CONTENT].Y == m_content.Y)
            {
                log.Fine("skipped");
                return m_pageNo;
            }

            m_pageNo++;
            m_currPage = new Page(m_printCtx, m_pageNo);
            m_pages.Add(m_currPage);
            //
            m_position[AREA_HEADER].X = m_header.X;
            m_position[AREA_HEADER].Y = m_header.Y;

            if (preserveXPos)
            {
                m_position[AREA_CONTENT].X = m_position[AREA_CONTENT].X;
                m_position[AREA_CONTENT].Y = m_content.Y;
            }
            else
            {
                m_position[AREA_CONTENT].X = m_content.X;
                m_position[AREA_CONTENT].Y = m_content.Y;
            }
            m_position[AREA_FOOTER].X = m_footer.X;
            m_position[AREA_FOOTER].Y = m_footer.Y;

            m_maxHeightSinceNewLine = new float[] { 0f, 0f, 0f };
            log.Finer("Page=" + m_pageNo);
            return m_pageNo;
        }	//	newPage

        protected void NewLine()
        {
            Rectangle part = m_content;
            if (m_area == AREA_HEADER)
                part = m_header;
            else if (m_area == AREA_FOOTER)
                part = m_footer;

            //	Temporary NL Position
            int xPos = part.X;
            if (m_tempNLPositon != 0)
                xPos = m_tempNLPositon;

            if (IsYspaceFor(m_maxHeightSinceNewLine[m_area]))
            {
                m_position[m_area].X = xPos;
                m_position[m_area].Y = m_position[m_area].Y + (int)m_maxHeightSinceNewLine[m_area];
                log.Finest("Page=" + m_pageNo + " [" + m_area + "] " + m_position[m_area].X + "/" + m_position[m_area].Y);
            }
            else if (m_area == AREA_CONTENT)
            {
                log.Finest("Not enough Y space "
                    + m_lastHeight[m_area] + " - remaining " + GetYspace() + " - Area=" + m_area);
                NewPage(true, false);
                log.Finest("Page=" + m_pageNo + " [" + m_area + "] " + m_position[m_area].X + "/" + m_position[m_area].Y);
            }
            else	//	footer/header
            {
                m_position[m_area].X = part.X;
                m_position[m_area].Y = m_position[m_area].Y + (int)m_maxHeightSinceNewLine[m_area];
                log.Log(Level.SEVERE, "Outside of Area(" + m_area + "): " + m_position[m_area]);
            }
            m_maxHeightSinceNewLine[m_area] = 0f;
        }	//	newLine

        public bool IsYspaceFor(float height)
        {
            return (GetYspace() - height) > 0f;
        }	//	isYspaceFor

        public float GetYspace()
        {
            Rectangle part = m_content;
            if (m_area == AREA_HEADER)
                part = m_header;
            else if (m_area == AREA_FOOTER)
                part = m_footer;
            //
            return (float)(part.Y + part.Height - m_position[m_area].Y);
        }	//	getYspace

        public int GetPageNo()
        {
            return m_pageNo;
        }	//	getPageNo


        public Page GetPage(int pageNo)
        {
            if (pageNo <= 0 || pageNo > m_pages.Count())
            {
                log.Log(Level.SEVERE, "No page #" + pageNo);
                return null;
            }
            Page retValue = (Page)m_pages[pageNo - 1];
            return retValue;
        }	//	getPage


        public List<Page> GetPages()
        {
            return m_pages;
        }	//	getPages


        public HeaderFooter GetHeaderFooter()
        {
            return m_headerFooter;
        }	//	getPages

        protected void SetPage(int pageNo)
        {
            if (pageNo <= 0 || pageNo > m_pages.Count())
            {
                log.Log(Level.SEVERE, "No page #" + pageNo);
                return;
            }
            Page retValue = (Page)m_pages[pageNo - 1];
            m_currPage = retValue;
        }	//	setPage

        public String GetPageInfo(int pageNo)
        {
            if (m_tableElement == null || m_tableElement.GetPageXCount() == 1)
                return "";
            int pi = m_tableElement.GetPageIndex(pageNo);
            StringBuilder sb = new StringBuilder("(");
            sb.Append(m_tableElement.GetPageYIndex(pi) + 1).Append(",")
                .Append(m_tableElement.GetPageXIndex(pi) + 1).Append(")");
            return sb.ToString();
        }	//	getPageInfo


        public String GetPageInfoMax()
        {
            if (m_tableElement == null || m_tableElement.GetPageXCount() == 1)
                return "";
            StringBuilder sb = new StringBuilder("(");
            sb.Append(m_tableElement.GetPageYCount()).Append(",")
                .Append(m_tableElement.GetPageXCount()).Append(")");
            return sb.ToString();
        }	//	getPageInfoMax


        public MPrintFormat GetFormat()
        {
            return m_format;
        }	//	getFormat

        protected void SetRelativePosition(PointF p)
        {
            if (p == null)
                return;
            Rectangle part = m_content;
            if (m_area == AREA_HEADER)
                part = m_header;
            else if (m_area == AREA_FOOTER)
                part = m_footer;
            m_position[m_area].X = part.X + p.X;
            m_position[m_area].Y = part.Y + p.Y;
            log.Finest("Page=" + m_pageNo + " [" + m_area + "] " + m_position[m_area].X + "/" + m_position[m_area].Y);
        }	//	setPosition

        /**
         * 	Set Position  on current page (no check)
         * 	@param x x position in 1/72 inch
         *  @param y y position in 1/72 inch
         */
        protected void SetRelativePosition(float x, float y)
        {
            SetRelativePosition(new PointF(x, y));
        }	//	setPosition

        /**
         * 	Get the current position on current page
         * 	@return current position
         */
        public PointF GetPosition()
        {
            return m_position[m_area];
        }	//	getPosition

        /// <summary>
        /// Set X Position on current page
        /// </summary>
        /// <param name="x"></param>
        protected void SetX(float x)
        {
            m_position[m_area].X = x;
            log.Finest("Page=" + m_pageNo + " [" + m_area + "] " + m_position[m_area].X + "/" + m_position[m_area].Y);
        }	//	setX


        /// <summary>
        /// Add to X Position on current page
        /// </summary>
        /// <param name="xOffset">add offset to x position in 1/72 inch</param>
        protected void AddX(float xOffset)
        {
            if (xOffset == 0f)
                return;
            m_position[m_area].X += xOffset;
            log.Finest("Page=" + m_pageNo + " [" + m_area + "] " + m_position[m_area].X + "/" + m_position[m_area].Y);
        }	//	addX

        /**
         * 	Get X Position on current page
         * 	@return x position in 1/72 inch
         */
        public float GetX()
        {
            return (float)m_position[m_area].X;
        }	//	getX

        /**
         * 	Set Y Position on current page
         * 	@param y y position in 1/72 inch
         */
        protected void SetY(int y)
        {
            m_position[m_area].Y = y;
            log.Finest("Page=" + m_pageNo + " [" + m_area + "] " + m_position[m_area].X + "/" + m_position[m_area].Y);
        }	//	setY

        /**
         * 	Add to Y Position - may cause New Page
         * 	@param yOffset add offset to y position in 1/72 inch
         */
        protected void AddY(int yOffset)
        {
            if (yOffset == 0f)
                return;
            if (IsYspaceFor(yOffset))
            {
                m_position[m_area].Y += yOffset;
                log.Finest("Page=" + m_pageNo + " [" + m_area + "] " + m_position[m_area].X + "/" + m_position[m_area].Y);
            }
            else if (m_area == AREA_CONTENT)
            {
                log.Finest("Not enough Y space "
                    + m_lastHeight[m_area] + " - remaining " + GetYspace() + " - Area=" + m_area);
                NewPage(true, true);
                log.Finest("Page=" + m_pageNo + " [" + m_area + "] " + m_position[m_area].X + "/" + m_position[m_area].Y);
            }
            else
            {
                m_position[m_area].Y += yOffset;
                log.Log(Level.SEVERE, "Outside of Area: " + m_position);
            }
        }	//	addY

        /**
         * 	Get Y Position on current page
         * 	@return y position in 1/72 inch
         */
        public float GetY()
        {
            return (float)m_position[m_area].Y;
        }	//	getY


        public float GetXspace()
        {
            Rectangle part = m_content;
            if (m_area == AREA_HEADER)
                part = m_header;
            else if (m_area == AREA_FOOTER)
                part = m_footer;
            //
            return (float)(part.X + part.Width - m_position[m_area].X);
        }	//	getXspace


        public bool IsXspaceFor(float width)
        {
            return (GetXspace() - width) > 0f;
        }	//	isXspaceFor


        /** Vienna is a wordwide registered Trademark
 *  - Don't modify this - Program will someday fail unexpectedly	*/
        static public String VIENNA_R = "Vienna Solutions\u00AE";
        StringBuilder finalHtml = null;
        string headerHtml = null;
        string footerHtml = null;
        //string contentHtml = null;
        string paraListHtml = null;
        string newPageHtml = "<div style='position:relative;border-style:outset;padding: 10px;padding-top: 20px;overflow:auto;bottom:6px'>";
        string endPageHtml = "</div>";
        private void CreateStandardHeaderFooter()
        {

            /** Removing/modifying the Vienna logo/trademark/copyright is a violation of the license	*/

            /////////change To pickImage on the Bases of Organisations////////
            //object logo = DB.ExecuteScalar("SELECT LOGO FROM AD_OrgInfo WHERE ISACTIVE='Y' AND AD_Org_ID=" + GetCtx().GetAD_Org_ID());
            //Image img = null;
            //if (logo != null)
            //{
            //    MemoryStream ms = new MemoryStream((Byte[])logo);
            //     img = Image.FromStream(ms);
            //    //element = ImageElement.Get(img);
            //}
            ///////////////////
            //else
            //{
            //    img = VAdvantage.DataBase.DB.GetImageLogoSmall(true);
            //}


            PrintElement element = new ImageElement(CoreLibrary.DataBase.DB.GetImageLogoSmall(true), "Vienna Small Log");	//	48x15
            //PrintElement element = new ImageElement(img, "Vienna Small Log");	//	48x15
            //element = new ImageElement(org.Vienna.Vienna.getImageLogo());	//	100x30
            element.Layout(48, 15, false, MPrintFormatItem.FIELDALIGNMENTTYPE_LeadingLeft);
            element.SetLocation(m_header.Location);
            //m_headerFooter.AddElement(element);

            MPrintTableFormat tf = m_format.GetTableFormat();
            Font font = tf.GetPageHeader_Font();
            Color color = tf.GetPageHeaderFG_Color();

            element = new StringElement("@*ReportName@", font, color, null, true);
            element.Layout(m_header.Width, 0, true, MPrintFormatItem.FIELDALIGNMENTTYPE_Center);
            element.SetLocation(m_header.Location);
            m_headerFooter.AddElement(element);

            element = new StringElement("@Page@ @*Page@ @of@ @*PageCount@", font, color, null, true);
            element.Layout(m_header.Width, 0, true, MPrintFormatItem.FIELDALIGNMENTTYPE_TrailingRight);
            element.SetLocation(m_header.Location);
            m_headerFooter.AddElement(element);

            //html for header
            //headerHtml = "<div style='overflow: auto;'>" +
            //                  "<span style='font-family: Microsoft Sans Serif,serif;font-size: 16px;color: rgb(0,0,0);font-weight: " +
            //                               "bold;font-style: normal;text-decoration: none;width: 80%;float: left;text-align:center;'>@*ReportName@" +
            //                  "</span>" +
            //                  "<span style='font-family: Microsoft Sans Serif,serif;color: rgb(0,0,0);font-weight: bold;text-decoration: " +
            //                               "none;float: right;font-size: 16px;'>" + Msg.GetMsg(GetCtx(), "Page") +
            //                               "@*Page@ " + Msg.GetMsg(GetCtx(), "of")
            //                               + " @*PageCount@ " +
            //                  "</span>" +
            //               "</div>";

            headerHtml = "<div class='vis-report-content'>" +
                "<h4 style='text-align:"+(GetCtx().GetIsRightToLeft()?"right":"left")+";'>@*ReportName@</h4>" +
                          "</div>";

            ////	Footer
            font = tf.GetPageFooter_Font();
            color = tf.GetPageFooterFG_Color();
            ////

            element = new StringElement(VIENNA_R, font, color, null, true);
            element.Layout(m_footer.Width, 0, true, MPrintFormatItem.FIELDALIGNMENTTYPE_LeadingLeft);
            PointF ft = m_footer.Location;
            ft.Y += m_footer.Height - element.GetHeight() - 2;	//	2pt above min
            element.SetLocation(ft);
            // m_headerFooter.AddElement(element);

            element = new StringElement("@*Header@", font, color, null, true);
            element.Layout(m_footer.Width, 0, true, MPrintFormatItem.FIELDALIGNMENTTYPE_Center);
            element.SetLocation(ft);
            //m_headerFooter.AddElement(element);

            element = new StringElement(DateTime.UtcNow.Date.ToString("d"), font, color, null, true);
            element.Layout(m_footer.Width, 0, true, MPrintFormatItem.FIELDALIGNMENTTYPE_TrailingRight);
            element.SetLocation(ft);
            m_headerFooter.AddElement(element);

            //html for footer
            footerHtml = @"<div style='bottom: 5px; position: absolute;width:99%'>
                               <span style='font-size: 12px;  font-family: sans-serif;  color: black;font-weight: bold;float: right;'>
                                " + DateTime.UtcNow.Date.ToString("d") + @"
                               </span>
                           </div>";
            //PdfDocument document = new PdfDocument();
            //PdfPage page = document.AddPage();
            //XGraphics xg = XGraphics.FromPdfPage(page);


            //PdfPrintElement pdfelement = new PdfStringElement("@Page@ @*Page@ @of@ @*PageCount@", new XFont("arial",10)  , XColors.Black , null, true);
            //pdfelement.Layout(m_header.Width, 0, true, MPrintFormatItem.FIELDALIGNMENTTYPE_TrailingRight);
            //pdfelement.SetLocation(m_header.Location);
            //m_pdfheaderFooter.AddElement(pdfelement);

            //document.Save("C:\\pdfexport.pdf");
        }	//	createStandardHeaderFooter


        private void CreateStandardHeaderFooterForPdf()
        {
            /** Removing/modifying the Vienna logo/trademark/copyright is a violation of the license	*/
            PrintElement element = new ImageElement(CoreLibrary.DataBase.DB.GetImageLogoSmall(true), "Vienna Small Log");	//	48x15
            //	element = new ImageElement(org.Vienna.Vienna.getImageLogo());	//	100x30
            element.Layout(48, 15, false, MPrintFormatItem.FIELDALIGNMENTTYPE_LeadingLeft);
            element.SetLocation(m_header.Location);

            m_headerFooter.AddElement(element);
            //
            MPrintTableFormat tf = m_format.GetTableFormat();
            Font font = tf.GetPageHeader_Font();
            Color color = tf.GetPageHeaderFG_Color();
            ////
            element = new StringElement("@*ReportName@", font, color, null, true);
            element.Layout(m_header.Width, 0, true, MPrintFormatItem.FIELDALIGNMENTTYPE_Center);
            element.SetLocation(m_header.Location);
            m_headerFooter.AddElement(element);
            //
            //
            element = new StringElement("@Page@ @*Page@ @of@ @*PageCount@", font, color, null, true);
            element.Layout(m_header.Width, 0, true, MPrintFormatItem.FIELDALIGNMENTTYPE_TrailingRight);
            element.SetLocation(m_header.Location);
            m_headerFooter.AddElement(element);

            //	Footer
            font = tf.GetPageFooter_Font();
            color = tf.GetPageFooterFG_Color();
            //
            /** Removing/modifying the Vienna logo/trademark/copyright is a violation of the license	*/
            //element = new StringElement(Vienna.Vienna_R, font, color, null, true);
            /** If you have a valid Vienna support contract, you can use the following	*/
            element = new StringElement(VIENNA_R, font, color, null, true);
            element.Layout(m_footer.Width, 0, true, MPrintFormatItem.FIELDALIGNMENTTYPE_LeadingLeft);
            PointF ft = m_footer.Location;
            ft.Y += m_footer.Height - element.GetHeight() - 2;	//	2pt above min
            element.SetLocation(ft);
            m_headerFooter.AddElement(element);
            //
            element = new StringElement("@*Header@", font, color, null, true);
            element.Layout(m_footer.Width, 0, true, MPrintFormatItem.FIELDALIGNMENTTYPE_Center);
            element.SetLocation(ft);
            m_headerFooter.AddElement(element);
            //
            element = new StringElement("@*CurrentDateTime@", font, color, null, true);
            element.Layout(m_footer.Width, 0, true, MPrintFormatItem.FIELDALIGNMENTTYPE_TrailingRight);
            element.SetLocation(ft);
            m_headerFooter.AddElement(element);

            //PdfDocument document = new PdfDocument();
            //PdfPage page = document.AddPage();
            //XGraphics xg = XGraphics.FromPdfPage(page);


            //PdfPrintElement pdfelement = new PdfStringElement("@Page@ @*Page@ @of@ @*PageCount@", new XFont("arial",10)  , XColors.Black , null, true);
            //pdfelement.Layout(m_header.Width, 0, true, MPrintFormatItem.FIELDALIGNMENTTYPE_TrailingRight);
            //pdfelement.SetLocation(m_header.Location);
            //m_pdfheaderFooter.AddElement(pdfelement);

            //document.Save("C:\\pdfexport.pdf");
        }	//	createStandardHeaderFooter


        public Ctx GetCtx()
        {
            return m_printCtx;
        }


        private PrintElement LayoutTable(MPrintFormat format, PrintData printData, int xOffset)
        {
            log.Info(format.GetName() + " - " + printData.GetName());
            MPrintTableFormat tf = format.GetTableFormat();
            //	Initial Values
            HashMap<Point, Font> rowColFont = new HashMap<Point, Font>();
            MPrintFont printFont = MPrintFont.Get(format.GetAD_PrintFont_ID());
            rowColFont.Put(new Point(TableElement.ALL, TableElement.ALL), printFont.GetFont());
            tf.SetStandard_Font(printFont.GetFont());
            rowColFont.Put(new Point(TableElement.HEADER_ROW, TableElement.ALL), tf.GetHeader_Font());

            //
            HashMap<Point, Color> rowColColor = new HashMap<Point, Color>();
            MPrintColor printColor = MPrintColor.Get(GetCtx(), format.GetAD_PrintColor_ID());
            rowColColor.Put(new Point(TableElement.ALL, TableElement.ALL), printColor.GetColor());
            rowColColor.Put(new Point(TableElement.HEADER_ROW, TableElement.ALL), tf.GetHeaderFG_Color());
            //
            HashMap<Point, Color> rowColBackground = new HashMap<Point, Color>();
            rowColBackground.Put(new Point(TableElement.HEADER_ROW, TableElement.ALL), tf.GetHeaderBG_Color());
            //	Sizes
            bool multiLineHeader = false;
            int pageNoStart = m_pageNo;
            int repeatedColumns = 1;
            Rectangle firstPage = new Rectangle();
            firstPage = m_content;
            firstPage.X += xOffset;
            firstPage.Width -= xOffset;
            int yOffset = (int)m_position[AREA_CONTENT].Y - m_content.Y;
            firstPage.Y += yOffset;
            firstPage.Height -= yOffset;
            Rectangle nextPages = new Rectangle();
            nextPages = m_content;

            nextPages.X += xOffset;
            nextPages.Width -= xOffset;
            //	Column count
            int columnCount = 0;
            for (int c = 0; c < format.GetItemCount(); c++)
            {
                if (format.GetItem(c).IsPrinted())
                    columnCount++;
            }
            //	System.out.println("Cols=" + cols);

            //	Header & Column Setup
            ValueNamePair[] columnHeader = new ValueNamePair[columnCount];
            int[] columnMaxWidth = new int[columnCount];
            int[] columnMaxHeight = new int[columnCount];
            bool[] fixedWidth = new bool[columnCount];
            String[] columnJustification = new String[columnCount];
            HashMap<int, int> additionalLines = new HashMap<int, int>();

            int col = 0;
            for (int c = 0; c < format.GetItemCount(); c++)
            {
                MPrintFormatItem item = format.GetItem(c);
                if (item.IsPrinted())
                {
                    if (item.IsNextLine() && item.GetBelowColumn() != 0)
                    {
                        additionalLines.Put(col, item.GetBelowColumn() - 1);
                        if (!item.IsSuppressNull())
                        {
                            item.SetIsSuppressNull(true);	//	display size will be set to 0 in TableElement
                            item.Save();
                        }
                    }
                    columnHeader[col] = new ValueNamePair(item.GetColumnName(),
                        item.GetPrintName(format.GetLanguage()));
                    columnMaxWidth[col] = item.GetMaxWidth();
                    fixedWidth[col] = (columnMaxWidth[col] != 0 && item.IsFixedWidth());
                    if (item.IsSuppressNull())
                    {
                        if (columnMaxWidth[col] == 0)
                            columnMaxWidth[col] = -1;		//	indication suppress if Null
                        else
                            columnMaxWidth[col] *= -1;
                    }
                    columnMaxHeight[col] = item.GetMaxHeight();
                    if (item.IsHeightOneLine())
                        columnMaxHeight[col] = -1;
                    columnJustification[col] = item.GetFieldAlignmentType();
                    if (columnJustification[col] == null || columnJustification[col].Equals(MPrintFormatItem.FIELDALIGNMENTTYPE_Default))
                        columnJustification[col] = MPrintFormatItem.FIELDALIGNMENTTYPE_LeadingLeft;	//	when generated sets correct alignment
                    //	Column Fonts
                    if (item.GetAD_PrintFont_ID() != 0 && item.GetAD_PrintFont_ID() != format.GetAD_PrintFont_ID())
                    {
                        MPrintFont font = MPrintFont.Get(item.GetAD_PrintFont_ID());
                        rowColFont.Put(new Point(TableElement.ALL, col), font.GetFont());
                    }
                    if (item.GetAD_PrintColor_ID() != 0 && item.GetAD_PrintColor_ID() != format.GetAD_PrintColor_ID())
                    {
                        //QueryRestriction
                        MPrintColor color = MPrintColor.Get(GetCtx(), item.GetAD_PrintColor_ID());
                        rowColColor.Put(new Point(TableElement.ALL, col), color.GetColor());
                    }
                    //
                    col++;
                }
            }

            //	The Data
            int rows = printData.GetRowCount();
            //	System.out.println("Rows=" + rows);
            Object[,] data = new Object[rows, columnCount];
            KeyNamePair[] pk = new KeyNamePair[rows];
            String pkColumnName = null;
            List<int> functionRows = new List<int>();
            List<int> pageBreak = new List<int>();
            List<string> fieldAlignment = new List<string>();


            //	for all rows
            for (int row = 0; row < rows; row++)
            {
                //	System.out.println("row=" + row);
                printData.SetRowIndex(row);
                if (printData.IsFunctionRow())
                {
                    functionRows.Add(row);
                    rowColFont.Put(new Point(row, TableElement.ALL), tf.GetFunct_Font());
                    rowColColor.Put(new Point(row, TableElement.ALL), tf.GetFunctFG_Color());
                    rowColBackground.Put(new Point(row, TableElement.ALL), tf.GetFunctBG_Color());
                    if (printData.IsPageBreak())
                    {
                        pageBreak.Add(row);
                        log.Finer("PageBreak row=" + row);
                    }
                }
                //	Summary/Line Levels for Finanial Reports
                else
                {
                    int levelNo = printData.GetLineLevelNo();
                    if (levelNo != 0)
                    {
                        if (levelNo < 0)
                            levelNo = -levelNo;
                        Font basef = printFont.GetFont();
                        if (levelNo == 1)
                            rowColFont.Put(new Point(row, TableElement.ALL), new Font(basef.Name, basef.Size - levelNo, FontStyle.Italic, GraphicsUnit.World));
                        else if (levelNo == 2)
                            rowColFont.Put(new Point(row, TableElement.ALL), new Font(basef.Name, basef.Size - levelNo, FontStyle.Regular, GraphicsUnit.World));
                    }
                }
                //	for all columns
                col = 0;
                for (int c = 0; c < format.GetItemCount(); c++)
                {
                    MPrintFormatItem item = format.GetItem(c);
                    Object dataElement = null;
                    if (item.IsPrinted() && item.GetAD_Column_ID() > 0)	//	Text Columns
                    {
                        if (item.IsTypePrintFormat())
                        {
                            log.Warning("Unsupported: PrintFormat in Table: " + item);
                        }
                        else if (item.IsTypeImage())
                        {
                            if (item.IsImageField())
                            {
                                Object obj = printData.GetNode(item.GetAD_Column_ID());
                                if (obj == null)
                                {
                                }
                                else if (obj is PrintDataElement)
                                {
                                    PrintDataElement pde = (PrintDataElement)obj;
                                    data[row, col] = ImageElement.Get((String)pde.GetValue());
                                }
                            }
                            else if (item.IsImageIsAttached())
                                data[row, col] = ImageElement.Get(item.Get_ID());
                            else
                                data[row, col] = ImageElement.Get(item.GetImageURL());
                        }
                        else if (item.IsBarcode())
                        {
                            Object obj = printData.GetNode(item.GetAD_Column_ID());
                            if (obj == null)
                            { }
                            else if (obj is PrintDataElement)
                            {
                                PrintDataElement pde = (PrintDataElement)obj;
                                BarcodeElement element = new BarcodeElement((String)pde.GetValue(), item);
                                if (element.IsValid())
                                    data[row, col] = element;
                            }
                        }
                        else
                        {
                            Object obj = printData.GetNode(item.GetAD_Column_ID(), false);
                            if (obj == null)
                            { }
                            else if (obj is PrintDataElement)
                            {
                                PrintDataElement pde = (PrintDataElement)obj;
                                if (pde.IsID() || pde.IsYesNo())
                                    dataElement = pde.GetValue();
                                else
                                    dataElement = pde.GetValueDisplay(format.GetLanguage());
                            }
                            else
                                log.Log(Level.SEVERE, "Element not PrintDataElement " + obj.GetType().FullName);
                            //	System.out.println("  row=" + row + ",col=" + col + " - " + item.getAD_Column_ID() + " => " + dataElement);
                            data[row, col] = dataElement;
                        }
                        if (item.GetFieldAlignmentType().Equals(X_AD_PrintFormatItem.FIELDALIGNMENTTYPE_LeadingLeft))
                        {
                            fieldAlignment.Add("Left");
                        }
                        else if (item.GetFieldAlignmentType().Equals(X_AD_PrintFormatItem.FIELDALIGNMENTTYPE_TrailingRight))
                        {
                            fieldAlignment.Add("right");
                        }
                        else if (item.GetFieldAlignmentType().Equals(X_AD_PrintFormatItem.FIELDALIGNMENTTYPE_Center))
                        {
                            fieldAlignment.Add("center");
                        }
                        else if (item.GetFieldAlignmentType().Equals(X_AD_PrintFormatItem.FIELDALIGNMENTTYPE_Block))
                        {
                            fieldAlignment.Add("justify");
                        }
                        else if (item.GetFieldAlignmentType().Equals(X_AD_PrintFormatItem.FIELDALIGNMENTTYPE_Default))
                        {
                            fieldAlignment.Add("");
                        }
                        //fieldAlignment.Add(item.GetFieldAlignmentType());

                        col++;
                    }	//	printed
                }   //	for all columns

                
                PrintDataElement pdel = printData.GetPKey();
                if (pdel != null)	//	for FunctionRows
                {
                    if (pdel.GetValue() is KeyNamePair)
                        pk[row] = (KeyNamePair)pdel.GetValue();
                    if (pkColumnName == null)
                        pkColumnName = pdel.GetColumnName();
                }
                else
                    log.Log(Level.INFO, "No PK " + printData);
            }	//	for all rows



            StringBuilder htmlTable = new StringBuilder("<div class='vis-report-data' style='margin-bottom: 20px;'><table class='vis-reptabledetect' style='white-space: nowrap;'>");

            StringBuilder colHeaderHtml = new StringBuilder("<thead><tr class='vis-report-table-head'>");
            for (int colh = 0; colh < columnHeader.Length; colh++)
            {
                //colHeaderHtml.Append("<th style=\'text-align:" +( GetCtx().GetIsRightToLeft()?"right":"left") + ";\'>" + columnHeader[colh] + "</th>");
                colHeaderHtml.Append("<th style=\'text-align:" + fieldAlignment[colh] + ";\'>" + columnHeader[colh] + "</th>");
            }
            colHeaderHtml.Append("</tr></thead>");

            htmlTable.Append(colHeaderHtml);


            for (int row = 0; row < rows; row++)
            {

                if (row > 0 && row % 21 == 0)
                {
                    htmlTable.Append("</table></div>●<div class='vis-report-data' style='margin-bottom: 20px;'><table class='vis-reptabledetect' style='white-space: nowrap;'>");
                    htmlTable.Append(colHeaderHtml);
                }
                if(row>0 && row % 2>0)
                {
                    htmlTable.Append("<tr class='vis-report-table-row vis-report-gray'>");
                }
                else{
                    htmlTable.Append("<tr class='vis-report-table-row'>");
                }
                

                for (int c = 0; c < col; c++)
                {
                    if (data[row, c] == null)
                    {
                        htmlTable.Append("<td>" + data[row, c] +"</td>");
                    }
                    else
                    {
                        htmlTable.Append("<td>" + HttpUtility.HtmlEncode(data[row, c].ToString()) + "</td>");
                    }
                }
                htmlTable.Append("</tr>");
                if (row == rows - 1)
                {
                    htmlTable.Append("</table></div>");
                }
            }
            htmlTable.Append("</table>");
            
            //
            
            TableElement table = new TableElement(columnHeader,
                columnMaxWidth, columnMaxHeight, columnJustification,
                fixedWidth, functionRows, multiLineHeader,
                data, pk, pkColumnName,
                pageNoStart, firstPage, nextPages, repeatedColumns, additionalLines,
                rowColFont, rowColColor, rowColBackground,
                tf, pageBreak);
            table.Layout(0, 0, false, MPrintFormatItem.FIELDALIGNMENTTYPE_LeadingLeft);
            if (m_tableElement == null)
                m_tableElement = table;
            dynamicPageHtml = htmlTable.ToString().Split('●');
            return table;
        }	//	layoutTable

        string[] dynamicPageHtml = null;
      


        public IPrintable GetPrintable(int pageIndex)
        {
            if (!HavePage(pageIndex))
                throw new IndexOutOfRangeException("No page index=" + pageIndex);
            return this;
        }	//	getPrintable

        /// <summary>
        /// Print Page (Printable Interface)
        /// </summary>
        /// <param name="element"></param>
        public int Print(Graphics graphics, PageFormat pageFormat, int pageIndex)
        {
            if (!HavePage(pageIndex))
                return 1;
            //
            Rectangle r = new Rectangle(0, 0, (int)GetPaper().GetWidth(true), (int)GetPaper().GetHeight(true));
            Page page = GetPage(pageIndex + 1);
            //
            //	log.fine("#" + m_id, "PageIndex=" + pageIndex + ", Copy=" + m_isCopy);
            page.Paint((Graphics)graphics, r, false, m_isCopy);	//	sets context
            GetHeaderFooter().Paint((Graphics)graphics, r, false);
            //
            return 0;
        }

        private bool HavePage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= GetNumberOfPages())
                return false;
            return true;
        }	//	havePage

        public int GetNumberOfPages()
        {
            return m_pages.Count();
        }	//	getNumberOfPages

        public PageFormat GetPageFormat(int pageIndex)
        {
            if (!HavePage(pageIndex))
                throw new IndexOutOfRangeException("No page index=" + pageIndex);
            return GetPageFormat();
        }	//	getPageFormat

        public bool IsCopy()
        {
            return m_isCopy;
        }	//	isCopy

        public void SetCopy(bool isCopy)
        {
            m_isCopy = isCopy;
        }	//	setCopy
    }
}
