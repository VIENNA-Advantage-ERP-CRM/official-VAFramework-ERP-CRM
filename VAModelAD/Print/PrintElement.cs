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
    public abstract class PrintElement
    {
        protected PrintElement()
        {
            
            log = VLogger.GetVLogger(this.GetType().FullName);
        }   //  PrintElement

	    /**	Link Color					*/
        public static Color LINK_COLOR = Color.Blue;

        /**	Calculated Size of Element	*/
        protected float p_width = 0f;
        protected float p_height = 0f;
        protected bool p_sizeCalculated = false;

        /**	Max Size of Element			*/
        protected float p_maxWidth = 0f;
        protected float p_maxHeight = 0f;

        /** Field Align Type			*/
        protected String p_FieldAlignmentType;

        /**	Location on Page			*/
        protected PointF p_pageLocation;
        /** Loading Flag				*/
        private bool m_imageNotLoaded = true;


        /** Image Info			*/
        protected String p_info = "";

        /**	Logger			*/
        protected VLogger log = null;


        /// <summary>
        /// Get Calculated Width
        /// </summary>
        /// <returns>Width</returns>
        public float GetWidth()
        {
            if (!p_sizeCalculated)
                p_sizeCalculated = CalculateSize();
            return p_width;
        }	//	getWidth

        /// <summary>
        /// Get Calculated Height
        /// </summary>
        /// <returns>Height</returns>
        public float GetHeight()
        {
            if (!p_sizeCalculated)
                p_sizeCalculated = CalculateSize();
            return p_height;
        }	//	getHeight

        /// <summary>
        /// Get Calculated Height on page
        /// </summary>
        /// <param name="pageNo">Page Number</param>
        /// <returns>Height</returns>
        public float GetHeight(int pageNo)
        {
            return GetHeight();
        }	//	getHeight

        /// <summary>
        /// Get number of pages
        /// </summary>
        /// <returns>Page count(1)</returns>
        public int GetPageCount()
        {
            return 1;
        }	//	getPageCount

        /// <summary>
        /// Layout and Calculate Size
        /// Set p_width & p_height
        /// </summary>
        /// <returns>true if calculated</returns>
        protected abstract bool CalculateSize();

        /// <summary>
        /// Layout Element
        /// </summary>
        /// <param name="maxWidth">max width</param>
        /// <param name="maxHeight">max height</param>
        /// <param name="isHeightOneLine">just one line</param>
        /// <param name="FieldAlignmentType">alignment type (MPrintFormatItem.FIELD_ALIGN_*)</param>
        public void Layout(float maxWidth, float maxHeight, bool isHeightOneLine, String FieldAlignmentType)
        {
            if (isHeightOneLine)
                p_maxHeight = -1f;
            else if (maxHeight > 0f)
                p_maxHeight = maxHeight;

            p_maxWidth = maxWidth;
            p_FieldAlignmentType = FieldAlignmentType;

            if ((string.IsNullOrEmpty(p_FieldAlignmentType)) || (p_FieldAlignmentType == X_AD_PrintFormatItem.FIELDALIGNMENTTYPE_Default))
                p_FieldAlignmentType = X_AD_PrintFormatItem.FIELDALIGNMENTTYPE_LeadingLeft;

            p_sizeCalculated = CalculateSize();
        } //Layout

        /// <summary>
        /// Set Maximum Height
        /// </summary>
        /// <param name="maxHeight">maximum height (0) is no limit</param>
        public void SetMaxHeight(float maxHeight)
        {
            p_maxHeight = maxHeight;
        }	//	setMaxHeight

        /// <summary>
        /// Set Maximum Width
        /// </summary>
        /// <param name="maxWidth">maximum width (0) is no limit</param>
        public void SetMaxWidth(float maxWidth)
        {
            p_maxWidth = maxWidth;
        }	//	setMaxWidth

        /// <summary>
        /// Set Location within page.
        /// Called from LayoutEngine.layoutForm(), lauout(), createStandardFooterHeader()
        /// </summary>
        /// <param name="pageLocation">location within page</param>
        public void SetLocation(PointF pageLocation)
        {
            p_pageLocation = new PointF(pageLocation.X, pageLocation.Y);
        }	//	setLocation

        /// <summary>
        /// Get Location within page
        /// </summary>
        /// <returns>location within page</returns>
        public PointF GetLocation()
        {
            return p_pageLocation;
        }	//	getLocation

        /// <summary>
        /// Return Absolute Position
        /// </summary>
        /// <param name="pageStart">start of page</param>
        /// <returns>absolite position</returns>
        protected PointF GetAbsoluteLocation(PointF pageStart)
        {
            PointF retValue = new PointF(p_pageLocation.X + pageStart.X, p_pageLocation.Y + pageStart.Y);
            //	log.finest( "PrintElement.getAbsoluteLocation", "PageStart=" + pageStart.getX() + "/" + pageStart.getY()
            //		+ ",PageLocaton=" + p_pageLocation.x + "/" + p_pageLocation.y + " => " + retValue.x + "/" + retValue.y);
            return retValue;
        }	//	getAbsoluteLocation


        /// <summary>
        /// Get relative Bounds of Element
        /// </summary>
        /// <returns>relative position on page</returns>
        public Rectangle GetBounds()
        {
            if (p_pageLocation == null)
                return new Rectangle(0, 0, (int)p_width, (int)p_height);
            return new Rectangle((int)p_pageLocation.X, (int)p_pageLocation.Y, (int)p_width, (int)p_height);
        }	//	getBounds


        /// <summary>
        /// Get Drill Down value
        /// </summary>
        /// <param name="relativePoint">relative Point</param>
        /// <param name="pageNo">page number</param>
        /// <returns>(subclasses overwrite)</returns>
        public abstract Query GetDrillDown(Point relativePoint, int pageNo);
        //{
        //    return null;
        //}	//	getDrillDown

        /// <summary>
        /// Get Drill Across value
        /// </summary>
        /// <param name="relativePoint">relative Point</param>
        /// <param name="pageNo">page number</param>
        /// <returns>null (subclasses overwrite)</returns>
        public abstract Query GetDrillAcross(Point relativePoint, int pageNo);
        //{
        //    return null;
        //}	//	getDrillAcross

        /// <summary>
        /// Translate Context if required.
        /// If content is translated, the element needs to stay in the bounds
        /// of the originally calculated size and need to align the field.
        /// </summary>
        /// <param name="ctx">context</param>
        public void Translate(Ctx ctx)
        {
            //	noop
        }	//	translate

        /// <summary>
        /// Content is translated
        /// </summary>
        /// <returns>false</returns>
        public bool IsTranslated()
        {
            return false;
        }	//	translate

        /// <summary>
        /// Paint/Print.
        /// </summary>
        /// <param name="g2D">Graphics</param>
        /// <param name="pageNo">page number for multi page support (0 = header/footer)</param>
        /// <param name="pageStart">top left Location of page</param>
        /// <param name="ctx">context</param>
        /// <param name="isView">true if online view (IDs are links)</param>
        public abstract void Paint(Graphics g2D, int pageNo, PointF pageStart, Ctx ctx, bool isView);

        public abstract void PaintPdf(XGraphics g2D, int pageNo, PointF pageStart, Ctx ctx, bool isView);

        /// <summary>
        /// Image Observer
        /// </summary>
        /// <param name="img"></param>
        /// <param name="infoflags"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public bool ImageUpdate(Image img, int infoflags, int x, int y, int width, int height)
        {
            //	copied from java.awt.component
            //m_imageNotLoaded = (infoflags & (ALLBITS | ABORT)) == 0;
            if (VLogMgt.IsLevelFinest())
                log.Finest("Flags=" + infoflags
                    + ", x=" + x + ", y=" + y + ", width=" + width + ", height=" + height
                    + " - NotLoaded=" + m_imageNotLoaded);
            return m_imageNotLoaded;
        }	//	imageUpdate

        /// <summary>
        /// Wait until Image is loaded.
        /// </summary>
        /// <param name="image">image</param>
        /// <returns>true if loaded</returns>
        public bool WaitForLoad(Image image)
        {
            long start = CommonFunctions.CurrentTimeMillis();
            //Thread.Sleep(10);
            return true;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>Info</returns>
        public override String ToString()
        {
            String cn = this.GetType().FullName;;
            StringBuilder sb = new StringBuilder();
            sb.Append(cn.Substring(cn.LastIndexOf('.') + 1)).Append("[");
            if (!Utility.Util.IsEmpty(p_info))
                sb.Append(p_info).Append(",");
            sb.Append("Bounds=").Append(GetBounds())
                .Append(",Height=").Append(p_height).Append("(").Append(p_maxHeight)
                .Append("),Width=").Append(p_width).Append("(").Append(p_maxHeight)
                .Append("),PageLocation=").Append(p_pageLocation);
            sb.Append("]");
            return sb.ToString();
        }	//	toString

        /// <summary>
        /// Is Image Loaded ?
        /// </summary>
        /// <returns>true or false</returns>
        public bool IsImageNotLoaded()
        {
            return m_imageNotLoaded;
        }

    }

}
