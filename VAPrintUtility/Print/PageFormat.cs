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
    /// The PageFormat class describes the size and orientation of a page to be printed.
    /// </summary>
    public class PageFormat : ICloneable
    {
        /// <summary>
        /// The origin is at the bottom left of the paper with x running bottom to top and y running left to right.
        /// </summary>
        public const int LANDSCAPE = 0;

        /// <summary>
        /// The origin is at the top left of the paper with x running to the right and y running down the paper
        /// </summary>
        public const int PORTRAIT = 1;

        /// <summary>
        /// The origin is at the top right of the paper with x running top to bottom and y running right to left.
        /// </summary>
        public const int REVERSE_LANDSCAPE = 2;

        /// <summary>
        /// A description of the physical piece of paper.
        /// </summary>
        private Paper mPaper;

        /// <summary>
        /// The orientation of the current page. This will be one of the constants: PORTRIAT, LANDSCAPE, or REVERSE_LANDSCAPE
        /// </summary>
        private int mOrientation = PORTRAIT;

        /// <summary>
        /// Creates a default, portrait-oriented PageFormat
        /// </summary>
        public PageFormat()
        {
            mPaper = new Paper();
        }

        /// <summary>
        /// Makes a copy of this PageFormat with the same contents as this PageFormat.
        /// </summary>
        /// <returns>copy of this PageFormat</returns>
        public Object Clone()
        {
            PageFormat newPage;

            try
            {
                newPage = (PageFormat)base.MemberwiseClone();
                newPage.mPaper = (Paper)mPaper.Clone();

            }
            catch
            {
                newPage = null;	// should never happen.
            }

            return newPage;
        }

        /// <summary>
        /// Returns the width, in 1/72nds of an inch, of the page.
        /// This method takes into account the orientation of the page when determining the width.
        /// </summary>
        /// <returns>width of the page.</returns>
        public float GetWidth()
        {
            float width;
            int orientation = GetOrientation();

            if (orientation == PORTRAIT)
            {
                width = mPaper.GetWidth();
            }
            else
            {
                width = mPaper.GetHeight();
            }

            return width;
        }

        /// <summary>
        /// Returns the height, in 1/72nds of an inch, of the page.
        /// This method takes into account the orientation of the page when determining the height.
        /// </summary>
        /// <returns>height of the page.</returns>
        public float GetHeight()
        {
            float height;
            int orientation = GetOrientation();

            if (orientation == PORTRAIT)
            {
                height = mPaper.GetHeight();
            }
            else
            {
                height = mPaper.GetWidth();
            }

            return height;
        }

        /// <summary>
        /// Returns the x coordinate of the upper left point of the imageable area of the Paper object 
        /// associated with this PageFormat
        /// </summary>
        /// <returns>the x coordinate of the upper left point of the imageable area of the paper</returns>
        public float GetImageableX()
        {
            float x;

            switch (GetOrientation())
            {

                case LANDSCAPE:
                    x = mPaper.GetHeight()
                    - (mPaper.GetImageableY() + mPaper.GetImageableHeight());
                    break;

                case PORTRAIT:
                    x = mPaper.GetImageableX();
                    break;

                case REVERSE_LANDSCAPE:
                    x = mPaper.GetImageableY();
                    break;

                default:
                    /* This should never happen since it signifies that the
                     * PageFormat is in an invalid orientation.
                     */
                    throw new Exception("unrecognized orientation");

            }

            return x;
        }

        /// <summary>
        /// Returns the y coordinate of the upper left point of the imageable area of the Paper object 
        /// </summary>
        /// <returns>the y coordinate of the upper left point of the imageable area of the paper</returns>
        public float GetImageableY()
        {
            float y;

            switch (GetOrientation())
            {

                case LANDSCAPE:
                    y = mPaper.GetImageableX();
                    break;

                case PORTRAIT:
                    y = mPaper.GetImageableY();
                    break;

                case REVERSE_LANDSCAPE:
                    y = mPaper.GetWidth()
                    - (mPaper.GetImageableX() + mPaper.GetImageableWidth());
                    break;

                default:
                    /* This should never happen since it signifies that the
                     * PageFormat is in an invalid orientation.
                     */
                    throw new Exception("unrecognized orientation");

            }

            return y;
        }

        /// <summary>
        /// Returns the width, in 1/72nds of an inch, of the imageable 
        /// area of the page. This method takes into account the orientation of the page.
        /// </summary>
        /// <returns>width of the page.</returns>
        public float GetImageableWidth()
        {
            float width;

            if (GetOrientation() == PORTRAIT)
            {
                width = mPaper.GetImageableWidth();
            }
            else
            {
                width = mPaper.GetImageableHeight();
            }

            return width;
        }

        /// <summary>
        /// Return the height, in 1/72nds of an inch, of the imageable
        /// area of the page. This method takes into account the orientation
        /// </summary>
        /// <returns>height of the page.</returns>
        public float GetImageableHeight()
        {
            float height;

            if (GetOrientation() == PORTRAIT)
            {
                height = mPaper.GetImageableHeight();
            }
            else
            {
                height = mPaper.GetImageableWidth();
            }

            return height;
        }

        /// <summary>
        /// Returns a copy of the Paper object associated with this PageFormat. 
        /// Changes made to the Paper object returned from this method do not affect 
        /// the Paper object of this PageFormat. 
        /// To update the Paper object of this PageFormat, create a new Paper object 
        /// and set it into this PageFormat by using the setPaper(Paper) method.
        /// </summary>
        /// <returns></returns>
        public Paper GetPaper()
        {
            return (Paper)mPaper.Clone();
        }

        /// <summary>
        /// Sets the Paper object for this PageFormat
        /// </summary>
        /// <param name="paper">The Paper object to which to set the Paper object for this PageFormat.</param>
        public void SetPaper(Paper paper)
        {
            mPaper = (Paper)paper.Clone();
        }

        /// <summary>
        /// Sets the page orientation. orientation must be one of the constants: PORTRAIT, LANDSCAPE, or REVERSE_LANDSCAPE.
        /// </summary>
        /// <param name="orientation">the new orientation for the page</param>
        public void SetOrientation(int orientation)
        {
            if (0 <= orientation && orientation <= REVERSE_LANDSCAPE)
            {
                mOrientation = orientation;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Returns the orientation of this PageFormat.
        /// </summary>
        /// <returns>PageFormat object's orientation.</returns>
        public int GetOrientation()
        {
            return mOrientation;
        }

        /// <summary>
        /// Returns a transformation matrix that translates user space rendering to 
        /// the requested orientation of the page. 
        /// The values are placed into the array as { m00, m10, m01, m11, m02, m12} 
        /// in the form required by the AffineTransform constructor.
        /// </summary>
        /// <returns>matrix used to translate user space rendering to the orientation of the page.</returns>
        public float[] GetMatrix()
        {
            float[] matrix = new float[6];

            switch (mOrientation)
            {

                case LANDSCAPE:
                    matrix[0] = 0; matrix[1] = -1;
                    matrix[2] = 1; matrix[3] = 0;
                    matrix[4] = 0; matrix[5] = mPaper.GetHeight();
                    break;

                case PORTRAIT:
                    matrix[0] = 1; matrix[1] = 0;
                    matrix[2] = 0; matrix[3] = 1;
                    matrix[4] = 0; matrix[5] = 0;
                    break;

                case REVERSE_LANDSCAPE:
                    matrix[0] = 0; matrix[1] = 1;
                    matrix[2] = -1; matrix[3] = 0;
                    matrix[4] = mPaper.GetWidth(); matrix[5] = 0;
                    break;

                default:
                    throw new ArgumentException();
            }

            return matrix;
        }
    }
}
