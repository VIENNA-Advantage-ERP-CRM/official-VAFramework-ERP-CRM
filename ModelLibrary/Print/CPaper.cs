using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Login;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Print
{
    public class CPaper : Paper
    {
        /// <summary>
        /// Constructor
        /// Derive Paper from PageForamt
        /// </summary>
        /// <param name="pf">PageFormat</param>
        public CPaper(PageFormat pf)
            : base()
        {
            m_landscape = pf.GetOrientation() != PageFormat.PORTRAIT;
            //	try to find MediaSize
            float x = (float)pf.GetWidth();
            float y = (float)pf.GetHeight();
            MediaSizeName msn = MediaSize.FindMedia(x / 72, y / 72, MediaSize.INCH);
            MediaSize ms = null;
            if (msn == null)
                msn = MediaSize.FindMedia(y / 72, x / 72, MediaSize.INCH);	//	flip it
            if (msn != null)
                ms = MediaSize.GetMediaSizeForName(msn);
            SetMediaSize(ms, m_landscape);
            //	set size directly
            SetSize(pf.GetWidth(), pf.GetHeight());
            SetImageableArea(pf.GetImageableX(), pf.GetImageableY(), pf.GetImageableWidth(), pf.GetImageableHeight());
        }	//	CPaper

        /// <summary>
        /// Constructor
        /// Get Media Size from Default Language
        /// </summary>
        /// <param name="landscape">true if landscape, false if portrait</param>
        public CPaper(bool landscape)
            : this(Language.GetLoginLanguage(), landscape)
        {
            
        }	//	CPaper

        /// <summary>
        /// Detail Constructor 1/2 inch on all sides
        /// </summary>
        /// <param name="mediaSize"> media size</param>
        /// <param name="landscape">true if landscape, false if portrait</param>
        private CPaper(MediaSize mediaSize, bool landscape)
            : this(mediaSize, landscape, 36, 36, 36, 36)
        {
            
        }	//	CPaper

        /// <summary>
        /// Detail Constructor
        /// </summary>
        /// <param name="mediaSize">media size</param>
        /// <param name="landscape"></param>
        /// <param name="left">x in 1/72 inch</param>
        /// <param name="top">y in 1/72 inch</param>
        /// <param name="right">right x in 1/72</param>
        /// <param name="bottom"> bottom y in 1/72</param>
        public CPaper(MediaSize mediaSize, bool landscape, float left, float top, float right, float bottom)
            : base()
        {
            SetMediaSize(mediaSize, landscape);
            SetImageableArea(left, top, GetWidth() - left - right, GetHeight() - top - bottom);
        }	//	CPaper

        /// <summary>
        /// Constructor
        /// Get Media Size from Language,
        /// </summary>
        /// <param name="language">language to derive media size</param>
        /// <param name="landscape">true if landscape, false if portrait</param>
        private CPaper(Language language, bool landscape)
            : this(language.GetMediaSize(), landscape)
        {
            
        }	//	CPaper

        /**	Media size						*/
        private MediaSize m_mediaSize;
        /** Landscape flag					*/
        private bool m_landscape = false;
        /**	Logger			*/
        private static VLogger log = VLogger.GetVLogger(typeof(CPaper).FullName);


        /// <summary>
        /// Set Media Size
        /// </summary>
        /// <param name="mediaSize">Set Media Size</param>
        /// <param name="landscape">true if landscape, false if portrait</param>
        public void SetMediaSize(MediaSize mediaSize, bool landscape)
        {
            if (mediaSize == null)
                throw new ArgumentException("MediaSize is null");
            m_mediaSize = mediaSize;
            m_landscape = landscape;

            //	Get Sise in Inch * 72
            float width = m_mediaSize.GetX(MediaSize.INCH) * 72;
            float height = m_mediaSize.GetY(MediaSize.INCH) * 72;
            //	Set Size
            SetSize(width, height);
            log.Fine(mediaSize.GetMediaSizeName() + ": " + m_mediaSize + " - Landscape=" + m_landscape);
        }	//	setMediaSize

        /// <summary>
        /// Get Media Size
        /// </summary>
        /// <returns>media size</returns>
        public MediaSizeName GetMediaSizeName()
        {
            return m_mediaSize.GetMediaSizeName();
        }	//	getMediaSizeName

        /// <summary>
        /// Get the Page Format for the Papaer
        /// </summary>
        /// <returns>Page Format</returns>
        public PageFormat GetPageFormat()
        {
            PageFormat pf = new PageFormat();
            pf.SetPaper(this);
            int orient = PageFormat.PORTRAIT;
            if (m_landscape)
                orient = PageFormat.LANDSCAPE;
            pf.SetOrientation(orient);
            return pf;
        }	//	getPageFormat

        /// <summary>
        /// Get String Representation
        /// </summary>
        /// <returns>Info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("CPaper[");
            sb.Append(GetWidth() / 72).Append("x").Append(GetHeight() / 72).Append('"')
                .Append(m_landscape ? " Landscape " : " Portrait ")
                .Append("x=").Append(GetImageableX())
                .Append(",y=").Append(GetImageableY())
                .Append(" w=").Append(GetImageableWidth())
                .Append(",h=").Append(GetImageableHeight())
                .Append("]");
            return sb.ToString();
        }	//	toString

        /// <summary>
        /// Get Printable Media Area
        /// </summary>
        /// <returns>Printable Area</returns>
        public MediaPrintableArea GetMediaPrintableArea()
        {
            MediaPrintableArea area = new MediaPrintableArea((float)GetImageableX() / 72, (float)GetImageableY() / 72,
                (float)GetImageableWidth() / 72, (float)GetImageableHeight() / 72, MediaPrintableArea.INCH);
            //	log.fine( "CPaper.getMediaPrintableArea", area.toString(MediaPrintableArea.INCH, "\""));
            return area;
        }	//	getMediaPrintableArea

        /// <summary>
        /// Get Printable Media Area
        /// </summary>
        /// <param name="area">Printable Area</param>
        public void SetMediaPrintableArea(MediaPrintableArea area)
        {
            int inch = MediaPrintableArea.INCH;
            log.Fine(area.ToString(inch, "\""));
            SetImageableArea(area.GetX(inch) * 72, area.GetY(inch) * 72,
                area.GetWidth(inch) * 72, area.GetHeight(inch) * 72);
        }	//	setMediaPrintableArea

        /// <summary>
        /// Is Landscape
        /// </summary>
        /// <returns>true if landscape</returns>
        public bool IsLandscape()
        {
            return m_landscape;
        }	//	isLandscape


        public double GetWidth(bool orientationCorrected)
        {
            if (orientationCorrected && m_landscape)
                return base.GetHeight();
            return base.GetWidth();
        }


        public double GetHeight(bool orientationCorrected)
        {
            if (orientationCorrected && m_landscape)
                return base.GetWidth();
            return base.GetHeight();
        }

        public double GetImageableX(bool orientationCorrected)
        {
            if (orientationCorrected && m_landscape)
                return base.GetImageableY();
            return base.GetImageableX();
        }

        public double GetImageableY(bool orientationCorrected)
        {
            if (orientationCorrected && m_landscape)
                return base.GetImageableX();
            return base.GetImageableY();
        }

        public double GetImageableHeight(bool orientationCorrected)
        {
            if (orientationCorrected && m_landscape)
                return base.GetImageableWidth();
            return base.GetImageableHeight();
        }

        public double GetImageableWidth(bool orientationCorrected)
        {
            if (orientationCorrected && m_landscape)
                return base.GetImageableHeight();
            return base.GetImageableWidth();
        }


        public String ToString(Ctx ctx)
        {
            StringBuilder sb = new StringBuilder();
            //	Print Media size
            sb.Append(m_mediaSize.GetMediaName());
            //	Print dimension
            String name = m_mediaSize.GetMediaName().ToString();
            if (!name.StartsWith("iso"))
                sb.Append(" - ").Append(m_mediaSize.ToString(MediaSize.INCH, "\""))
                    .Append(" (").Append(GetMediaPrintableArea().ToString(MediaPrintableArea.INCH, "\""));
            if (!name.StartsWith("na"))
                sb.Append(" - ").Append(m_mediaSize.ToString(MediaSize.MM, "mm"))
                    .Append(" (").Append(GetMediaPrintableArea().ToString(MediaPrintableArea.MM, "mm"));
            //	Print Orientation
            sb.Append(") - ")
                .Append(Msg.GetMsg(ctx, m_landscape ? "Landscape" : "Portrait"));
            return sb.ToString();
        }	//	toString
    }
}
