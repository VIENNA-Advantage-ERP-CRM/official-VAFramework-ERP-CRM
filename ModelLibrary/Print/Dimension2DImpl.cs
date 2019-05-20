using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using java.awt.geom;
using java.awt;

namespace VAdvantage.Print
{
    /// <summary>
    /// 2D Dimesnion Implementation
    /// </summary>
    public class Dimension2DImpl : Dimension2D
    {
        //public override double Height
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //public override double Width
        //{
        //    get { throw new NotImplementedException(); }
        //}

        /// <summary>
        /// Constructor
        /// </summary>
        public Dimension2DImpl()
        {
        }	//	Dimension2DImpl

        public Dimension2DImpl(Dimension dim)
        {
            SetSize(dim);
        }	//	Dimension2DImpl
        public Dimension2DImpl(double Width, double Height)
        {
            SetSize(Width, Height);
        }	//	Dimension2DImpl
        /**	Width			*/
        public double width = 0;
        /**	Height			*/
        public double height = 0;

        public void SetSize(double Width, double Height)
        {
            this.width = Width;
            this.height = Height;
        }	//	setSize

        public void SetSize(Dimension dim)
        {
            this.width = dim.width;
            this.height = dim.height;
        }	//	setSize

        public void AddBelow(double dWidth, double dHeight)
        {
            if (this.width < dWidth)
                this.width = dWidth;
            this.height += dHeight;
        }	//	addBelow

        public void AddBelow(Dimension dim)
        {
            AddBelow(dim.width, dim.height);
        }	//	addBelow

        public void RoundUp()
        {
            width = Math.Ceiling(width);
            height = Math.Ceiling(height);
        }	//	roundUp

        public double GetWidth()
        {
            return width;
        }	//	getWidth

        public double GetHeight()
        {
            return height;
        }	//	getHeight


        /// <summary>
        /// Hashcode
        /// </summary>
        /// <returns></returns>
        public new  int GetHashCode()
        {
            return base.GetHashCode();
        }
 
        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns></returns>
        public new  bool Equals(object obj)
        {
            if (obj != null && obj is Dimension2D)
            {
                Dimension2D d = (Dimension2D)obj;
                if (d.getWidth() == width && d.getHeight() == height)
                    return true;
            }
            return false;

        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>Info</returns>
        public new string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Dimension2D[w=").Append(width).Append(",h=").Append(height).Append("]");
            return sb.ToString();
        }

        public override double getHeight()
        {
            throw new NotImplementedException();
        }

        public override double getWidth()
        {
            throw new NotImplementedException();
        }

        public override void setSize(double d1, double d2)
        {
            this.width = d1;
            this.height = d2;
        }
    }
}
