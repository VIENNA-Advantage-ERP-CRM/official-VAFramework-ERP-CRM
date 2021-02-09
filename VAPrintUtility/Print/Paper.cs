using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Drawing.Text;

namespace VAdvantage.Print
{
    /// <summary>
    /// The Paper class describes the physical characteristics of a piece of paper.
    /// <para>
    /// When creating a Paper object, it is the application's 
    /// responsibility to ensure that the paper size and the imageable are
    /// are compatible.  For example, if the paper size is changed from
    /// 11 x 17 to 8.5 x 11, the application might need to reduce the 
    /// imageable area so that whatever is printed fits on the page.
    /// </para>
    /// </summary>
    public class Paper : ICloneable
    {
        /* Private Class Variables */

        private static int INCH = 72;
        private static float LETTER_WIDTH = (float)8.5 * INCH;
        private static float LETTER_HEIGHT = 11 * INCH;


        /// <summary>
        /// The height of the physical page in 1/72nds of an inch. The number is stored as a floating
        /// point value rather than as an integer
        /// to facilitate the conversion from metric
        /// units to 1/72nds of an inch and then back.
        /// </summary>
        private float mHeight;

        /// <summary>
        /// The width of the physical page in 1/72nds of an inch.
        /// </summary>
        private float mWidth;

        /// <summary>
        /// The area of the page on which drawing will
        /// be visable. The area outside of this
        /// rectangle but on the Page generally
        /// reflects the printer's hardware margins.
        /// The origin of the physical page is at (0, 0) with this rectangle provided
        /// in that coordinate system.
        /// </summary>
        private RectangleF mImageableArea;

        /// <summary>
        /// Creates a letter sized piece of paper with one inch margins.
        /// </summary>
        public Paper()
        {
            mHeight = LETTER_HEIGHT;
            mWidth = LETTER_WIDTH;
            mImageableArea = new RectangleF(INCH, INCH, mWidth - 2 * INCH, mHeight - 2 * INCH);
        }

        /// <summary>
        /// Returns the height of the page in 1/72nds of an inch.
        /// </summary>
        /// <returns>The height of the page described by this</returns>
        public float GetHeight()
        {
            return mHeight;
        }

        /// <summary>
        /// Sets the width and height of this Paper object, which represents 
        /// the properties of the page onto which printing occurs.
        /// The dimensions are supplied in 1/72nds of and inch.
        /// </summary>
        /// <param name="width">The width value to which to set this Paper</param>
        /// <param name="height">The height value to which to set this Paper</param>
        public void SetSize(float width, float height)
        {
            mWidth = width;
            mHeight = height;
        }

        /// <summary> 
        /// Returns the width of the page in 1/72nds of an inch.
        /// </summary>
        /// <returns>width of the page described by this Paper</returns>
        public float GetWidth()
        {
            return mWidth;
        }

        /// <summary>
        /// Sets the imageable area of this Paper. The
        /// imageable area is the area on the page in which printing
        /// occurs.
        /// </summary>
        /// <param name="x">the coordinates to which to set the upper-left corner of the imageable area of this Paper</param>
        /// <param name="y">the coordinates to which to set the upper-left corner of the imageable area of this Paper</param>
        /// <param name="width">value to which to set the width of the imageable area of this Paper</param>
        /// <param name="height">value to which to set the height of the imageable area of this Paper</param>
        public void SetImageableArea(float x, float y, float width, float height)
        {
            mImageableArea = new RectangleF(x, y, width, height);
        }

        /// <summary>
        /// Returns the x coordinate of the upper-left corner of this
        /// Paper object's imageable area.
        /// </summary>
        /// <returns>The x coordinate of the imageable area.</returns>
        public float GetImageableX()
        {
            return mImageableArea.X;
        }

        /// <summary>
        /// Returns the y coordinate of the upper-left corner of this Paper object's imageable area.
        /// </summary>
        /// <returns>The y coordinate of the imageable area.</returns>
        public float GetImageableY()
        {
            return mImageableArea.Y;
        }

        /// <summary>
        /// Returns the width of this Paper object's imageable area.
        /// </summary>
        /// <returns>the width of the imageable area.</returns>
        public float GetImageableWidth()
        {
            return mImageableArea.Width;
        }

        /// <summary>
        /// Returns the height of this Paper object's imageable area.
        /// </summary>
        /// <returns>The height of the imageable area.</returns>
        public float GetImageableHeight()
        {
            return mImageableArea.Height;
        }

        /// <summary>
        /// Creates a copy of this Paper with the same contents as this Paper.
        /// </summary>
        /// <returns>a copy of this Paper.</returns>
        public Object Clone()
        {
            Paper newPaper;
            try
            {
                /* It's okay to copy the reference to the imageable
                 * area into the clone since we always return a copy
                 * of the imageable area when asked for it.
                 */
                newPaper = (Paper)base.MemberwiseClone();
            }
            catch
            {
                newPaper = null;
            }

            return newPaper;
        }
    }
}
