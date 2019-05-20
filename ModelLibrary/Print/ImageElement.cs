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
using System.IO;
using PdfSharp.Drawing;
using VAdvantage.Utility;

namespace VAdvantage.Print
{
    public class ImageElement : PrintElement
    {
        public static ImageElement Get(String imageURLString)
        {
            Object key = imageURLString;
            ImageElement image = null;
            if(s_cache.ContainsKey(key))
                image = (ImageElement)s_cache[key];

            if (image == null)
            {
                image = new ImageElement(imageURLString);
                s_cache[key] =  image;
            }
            return new ImageElement(image.GetImage(), imageURLString);
        }	//	get


        public static ImageElement Get(int AD_PrintFormatItem_ID)
        {
            Object key = AD_PrintFormatItem_ID;
            ImageElement image = null;
            if (s_cache.ContainsKey(key))
                image = (ImageElement)s_cache[key];
            if (image == null)
            {
                image = new ImageElement(AD_PrintFormatItem_ID);
                s_cache[key] = image;
            }
            return new ImageElement(image.GetImage(), "AD_PrintFormatItem_ID=" + AD_PrintFormatItem_ID);
        }	//	get


        /// <summary>
        /// Get the Image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static ImageElement Get(Image image)
        {
            return new ImageElement(image, "Custom Image");
        }

        public ImageElement(Image image, String info)
        {
            m_image = image;
            p_info = info;
            if (m_image != null)
                log.Fine("Image=" + info + " - " + image);
            else
                log.Log(Level.WARNING, "Image is NULL");
        }	//	ImageElement

        private ImageElement(int AD_PrintFormatItem_ID)
        {
            LoadAttachment(AD_PrintFormatItem_ID);
        }	//	ImageElement

        private ImageElement(String imageURLstring)
        {
            p_info = imageURLstring;
            string imageURL = GetURL(imageURLstring);
            if (imageURL != null)
            {
                m_image = Image.FromFile(imageURL);
                if (m_image != null)
                    log.Fine("URL=" + imageURL);
                else
                    log.Log(Level.WARNING, "Not loaded - URL=" + imageURL);
            }
            else
                log.Log(Level.WARNING, "Invalid URL=" + imageURLstring);
        }	//	ImageElement

        private string GetURL(string imageURLstring)
        {
            return imageURLstring;
        }



        /**	The Image			*/
        private Image m_image = null;
        /** Scale				*/
        private float m_scaleFactor = 1;

        /**	60 minute Cache						*/
        private static CCache<Object, ImageElement> s_cache = new CCache<Object, ImageElement>("ImageElement", 10, 60);

        /// <summary>
        /// Image
        /// </summary>
        /// <returns></returns>
        public Image GetImage()
        {
            return m_image;
        }	//	getImage


        private void LoadAttachment(int AD_PrintFormatItem_ID)
        {
            MAttachment attachment = MAttachment.Get(VAdvantage.Utility.Env.GetCtx(), MPrintFormatItem.Table_ID, AD_PrintFormatItem_ID);
            if (attachment == null)
            {
                log.Log(Level.WARNING, "No Attachment - AD_PrintFormatItem_ID=" + AD_PrintFormatItem_ID);
                return;
            }
            if (attachment.GetEntryCount() != 1)
            {
                log.Log(Level.WARNING, "Need just 1 Attachment Entry = " + attachment.GetEntryCount());
                return;
            }
            p_info = attachment.GetEntryName(0);
            byte[] imageData = attachment.GetEntryData(0);
            if (imageData != null)
            {
                MemoryStream ms = new MemoryStream(imageData);
                m_image = Image.FromStream(ms);
            }
            if (m_image != null)
                log.Fine(attachment.GetEntryName(0) + " - Size=" + imageData.Length);
            else
                log.Log(Level.WARNING, attachment.GetEntryName(0) + " - not loaded (must be gif or jpg) - AD_PrintFormatItem_ID=" + AD_PrintFormatItem_ID);
        }	//	loadAttachment

        protected override bool CalculateSize()
        {
            p_width = 0;
            p_height = 0;
            if (m_image == null)
                return true;
            //	we have an image
            WaitForLoad(m_image);
            if (m_image != null)
            {
                p_width = m_image.Width;
                p_height = m_image.Height;

                if (p_width * p_height == 0)
                    return true;	//	don't bother scaling and prevent div by 0

                // 0 = unlimited so scale to fit restricted dimension
                if (p_maxWidth * p_maxHeight != 0)	// scale to maintain aspect ratio
                {
                    if (p_width / p_height > p_maxWidth / p_maxHeight)
                        // image "fatter" than available area
                        m_scaleFactor = p_maxWidth / p_width;
                    else
                        m_scaleFactor = p_maxHeight / p_height;
                }
                p_width = (float)m_scaleFactor * p_width;
                p_height = (float)m_scaleFactor * p_height;
            }
            return true;
        }	//	calculateSize

        public override void Paint(Graphics g2D, int pageNo, PointF pageStart, Ctx ctx, bool isView)
        {
            if (m_image == null)
                return;

            //	Position
            PointF location = GetAbsoluteLocation(pageStart);
            float x = location.X;
            if (MPrintFormatItem.FIELDALIGNMENTTYPE_TrailingRight.Equals(p_FieldAlignmentType))
                x += p_maxWidth - p_width;
            else if (MPrintFormatItem.FIELDALIGNMENTTYPE_Center.Equals(p_FieldAlignmentType))
                x += (p_maxWidth - p_width) / 2;
            float y = location.Y;

            // 	map a scaled and shifted version of the image to device space
            //g2D.ScaleTransform(m_scaleFactor, m_scaleFactor);
            g2D.DrawImage(ResizeImage(m_image,(int)p_width,(int)p_height), x, y);
            //g2D.ResetTransform();
        }	//	paint


        public static System.Drawing.Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            //a holder for the result
            Bitmap result = new Bitmap(width, height);
            //use a graphics object to draw the resized image into the bitmap
            using (Graphics graphics = Graphics.FromImage(result))
            {
                //set the resize quality modes to high quality
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //draw the image into the target bitmap
                graphics.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            //return the resulting bitmap
            return result;
        }

        public override void PaintPdf(XGraphics g2D, int pageNo, PointF pageStart, Ctx ctx, bool isView)
        {
            if (m_image == null)
                return;

            //	Position
            PointF location = GetAbsoluteLocation(pageStart);
            float x = location.X;
            if (MPrintFormatItem.FIELDALIGNMENTTYPE_TrailingRight.Equals(p_FieldAlignmentType))
                x += p_maxWidth - p_width;
            else if (MPrintFormatItem.FIELDALIGNMENTTYPE_Center.Equals(p_FieldAlignmentType))
                x += (p_maxWidth - p_width) / 2;
            float y = location.Y;

            // 	map a scaled and shifted version of the image to device space
            //g2D.ScaleTransform(m_scaleFactor, m_scaleFactor);

            g2D.DrawImage(ResizeImage(m_image, (int)p_width, (int)p_height), (double)x, (double)y);
            //g2D.ResetTransform();
        }	//	paint


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
