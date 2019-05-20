using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Print
{
    [Serializable]
    public class Size2DSyntax : ICloneable
    {

        /// <summary>
        /// X dimension in units of micrometers (&#181;m).
        /// </summary>
        private int x;

        /// <summary>
        /// Y dimension in units of micrometers (&#181;m).
        /// </summary>
        private int y;

        /// <summary>
        /// Value to indicate units of inches (in). It is actually the conversion factor by which to multiply inches to yield &#181;m (25400). 
        /// </summary>
        public const int INCH = 25400;

        /// <summary>
        /// Value to indicate units of millimeters (mm). It is actually the conversion factor by which to multiply mm to yield &#181;m (1000). 
        /// </summary>
        public const int MM = 1000;

        /// <summary>
        /// Construct a new two-dimensional size attribute from the given floating-point values. 
        /// </summary>
        /// <param name="x">X dimension.</param>
        /// <param name="y">Y dimension.</param>
        /// <param name="units">units</param>
        protected Size2DSyntax(float x, float y, int units)
        {
            if (x < 0.0f)
            {
                throw new ArgumentException("x < 0");
            }
            if (y < 0.0f)
            {
                throw new ArgumentException("y < 0");
            }
            if (units < 1)
            {
                throw new ArgumentException("units < 1");
            }
            this.x = (int)(x * units + 0.5f);
            this.y = (int)(y * units + 0.5f);
        }

        /// <summary>
        /// Construct a new two-dimensional size attribute from the given integer values. 
        /// </summary>
        /// <param name="x">X dimension.</param>
        /// <param name="y">Y dimension.</param>
        /// <param name="units">units</param>
        protected Size2DSyntax(int x, int y, int units)
        {
            if (x < 0)
            {
                throw new ArgumentException("x < 0");
            }
            if (y < 0)
            {
                throw new ArgumentException("y < 0");
            }
            if (units < 1)
            {
                throw new ArgumentException("units < 1");
            }
            this.x = x * units;
            this.y = y * units;
        }

        /// <summary>
        /// Convert a value from micrometers to some other units. The result is returned as a floating-point number. 
        /// </summary>
        /// <param name="x">Value (micrometers) to convert.</param>
        /// <param name="units">units</param>
        /// <returns>The value of x converted to the desired units.</returns>
        private static float ConvertFromMicrometers(int x, int units)
        {
            if (units < 1)
            {
                throw new ArgumentException("units is < 1");
            }
            return ((float)x) / ((float)units);
        }

        /// <summary>
        /// Get this two-dimensional size attribute's dimensions in the given units as floating-point values. 
        /// </summary>
        /// <param name="units">units</param>
        /// <returns>A two-element array with the X dimension at index 0 and the Y dimension at index 1. </returns>
        public float[] GetSize(int units)
        {
            return new float[] { GetX(units), GetY(units) };
        }

        /// <summary>
        /// Returns this two-dimensional size attribute's X dimension in the given units as a floating-point value. 
        /// </summary>
        /// <param name="units">units</param>
        /// <returns>X dimension. </returns>
        public float GetX(int units)
        {
            return ConvertFromMicrometers(x, units);
        }

        /// <summary>
        /// Returns this two-dimensional size attribute's Y dimension in the given units as a floating-point value. 
        /// </summary>
        /// <param name="units">units</param>
        /// <returns>Y dimension. </returns>
        public float GetY(int units)
        {
            return ConvertFromMicrometers(y, units);
        }

        /// <summary>
        /// Returns a string version of this two-dimensional size attribute in the given units.
        /// </summary>
        /// <param name="units">Unit conversion factor, e.g. {@link #INCH <CODE>INCH</CODE>} or  {@link #MM <CODE>MM</CODE>}</param>
        /// <param name="unitsName">Units name string, e.g. <CODE>"in"</CODE> or <CODE>"mm"</CODE>. If null, no units name is appended to the result. </param>
        /// <returns>String version of this two-dimensional size attribute.</returns>
        public String ToString(int units, String unitsName)
        {
            StringBuilder result = new StringBuilder();
            result.Append(GetX(units));
            result.Append('x');
            result.Append(GetY(units));
            if (unitsName != null)
            {
                result.Append(' ');
                result.Append(unitsName);
            }
            return result.ToString();
        }

        /// <summary>
        /// Returns whether this two-dimensional size attribute is equivalent to the passed in object. To be equivalent, all of the following conditions must be true;
        /// <para>object is not null.</para>
        /// <para>object is an instance of class Size2DSyntax.</para>
        /// <para>This attribute's X dimension is equal to object's X dimension.</para>
        /// <para> This attribute's Y dimension is equal to object's Y dimension.</para>
        /// </summary>
        /// <param name="obj">Object to compare to.</param>
        /// <returns>True/False</returns>
        public override bool Equals(Object obj)
        {
            return (obj != null && obj is Size2DSyntax && this.x == ((Size2DSyntax)obj).x && this.y == ((Size2DSyntax)obj).y);
        }

        /// <summary>
        /// Returns a string version of this two-dimensional size attribute. The string takes the form <CODE>"<I>X</I>x<I>Y</I> um"</CODE>, where
        /// <I>X</I> is the X dimension and <I>Y</I> is the Y dimension.
        /// The values are reported in the internal units of micrometers.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append(x);
            result.Append('x');
            result.Append(y);
            result.Append(" um");
            return result.ToString();
        }

        /// <summary>
        /// Returns this two-dimensional size attribute's X dimension in units of micrometers (&#181;m). (For use in a subclass.)
        /// </summary>
        /// <returns>X dimension (&#181;m).s</returns>
        protected int GetXMicrometers()
        {
            return x;
        }

        /// <summary>
        /// Returns this two-dimensional size attribute's Y dimension in units of micrometers (&#181;m). (For use in a subclass.)
        /// </summary>
        /// <returns>Y dimension (&#181;m).</returns>
        protected int GetYMicrometers()
        {
            return y;
        }

        /// <summary>
        /// Implement the clone method
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this;
        }

        /// <summary>
        /// Returns a hash code value for this two-dimensional size attribute.
        /// </summary>
        /// <returns>hashcode int</returns>
        public override int GetHashCode()
        {
            return (((x & 0x0000FFFF)) | ((y & 0x0000FFFF) << 16));
        }
    }
}
