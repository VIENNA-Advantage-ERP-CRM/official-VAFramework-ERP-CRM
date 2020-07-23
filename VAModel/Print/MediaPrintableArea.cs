using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Print
{
    public class MediaPrintableArea
    {
        private int x, y, w, h;
        //private int units;

        /// <summary>
        /// Value to indicate units of inches (in). It is actually the conversion factor by which to multiply inches to yield &#181;m (25400).
        /// </summary>
        public static int INCH = 25400;

        /// <summary>
        /// Value to indicate units of millimeters (mm). It is actually the conversion factor by which to multiply mm to yield &#181;m (1000).
        /// </summary>
        public static int MM = 1000;
        
        /// <summary>
        /// Constructs a MediaPrintableArea object from floating point values.
        /// </summary>
        /// <param name="x">printable x</param>
        /// <param name="y">printable y</param>
        /// <param name="w">printable width</param>
        /// <param name="h">printable height</param>
        /// <param name="units">in which the values are expressed.</param>
        public MediaPrintableArea(float x, float y, float w, float h, int units)
        {
            if ((x < 0.0) || (y < 0.0) || (w <= 0.0) || (h <= 0.0) ||
                (units < 1))
            {
                throw new ArgumentException("0 or negative value argument");
            }

            this.x = (int)(x * units + 0.5f);
            this.y = (int)(y * units + 0.5f);
            this.w = (int)(w * units + 0.5f);
            this.h = (int)(h * units + 0.5f);
        }

        /// <summary>
        /// Constructs a MediaPrintableArea object from integer values.
        /// </summary>
        /// <param name="x">printable x</param>
        /// <param name="y">printable y</param>
        /// <param name="w">printable width</param>
        /// <param name="h">printable height</param>
        /// <param name="units">in which the values are expressed.</param>
        public MediaPrintableArea(int x, int y, int w, int h, int units)
        {
            if ((x < 0) || (y < 0) || (w <= 0) || (h <= 0) ||
                (units < 1))
            {
                throw new ArgumentException("0 or negative value argument");
            }
            this.x = x * units;
            this.y = y * units;
            this.w = w * units;
            this.h = h * units;
        }

        /// <summary>
        /// Get the printable area as an array of 4 values in the order x, y, w, h. 
        /// The values returned are in the given units.
        /// </summary>
        /// <param name="units">units</param>
        /// <returns>printable area as array of x, y, w, h in the specified units.</returns>
        public float[] GetPrintableArea(int units)
        {
            return new float[] { GetX(units), GetY(units),
                             GetWidth(units), GetHeight(units) };
        }

        /// <summary>
        /// Get the X location of the origin of the printable area in the specified unit
        /// </summary>
        /// <param name="units">units</param>
        /// <returns>X location of the origin of the printable area in the specified unit</returns>
        public float GetX(int units)
        {
            return ConvertFromMicrometers(x, units);
        }

        /// <summary>
        /// Get the y location of the origin of the printable area in the specified unit
        /// </summary>
        /// <param name="units">units</param>
        /// <returns>y location of the origin of the printable area in the specified unit</returns>
        public float GetY(int units)
        {
            return ConvertFromMicrometers(y, units);
        }

        /// <summary>
        /// Get the width of the printable area in the specified units.
        /// </summary>
        /// <param name="units">units</param>
        /// <returns>width of the printable area in the specified units.</returns>
        public float GetWidth(int units)
        {
            return ConvertFromMicrometers(w, units);
        }

        /// <summary>
        /// Get the height of the printable area in the specified units.
        /// </summary>
        /// <param name="units">units</param>
        /// <returns>height of the printable area in the specified units.</returns>
        public float GetHeight(int units)
        {
            return ConvertFromMicrometers(h, units);
        }

        public override bool Equals(Object ob)
        {
            bool ret = false;
            if (ob is MediaPrintableArea)
            {
                MediaPrintableArea mm = (MediaPrintableArea)ob;
                if (x == mm.x && y == mm.y && w == mm.w && h == mm.h)
                {
                    ret = true;
                }
            }
            return ret;
        }

        /// <summary>
        /// Get the name of the category of which this attribute value is an instance
        /// </summary>
        /// <returns></returns>
        public String GetName()
        {
            return "media-printable-area";
        }

        /// <summary>
        /// Returns a string version of this rectangular size attribute in the given units
        /// </summary>
        /// <param name="units">units</param>
        /// <param name="unitsName">unitsName</param>
        /// <returns></returns>
        public String ToString(int units, String unitsName)
        {
            if (unitsName == null)
            {
                unitsName = "";
            }
            float[] vals = GetPrintableArea(units);
            String str = "(" + vals[0] + "," + vals[1] + ")->(" + vals[2] + "," + vals[3] + ")";
            return str + unitsName;
        }

        /// <summary>
        /// Returns a string version of this rectangular size attribute in mm.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return (ToString(MM, "mm"));
        }

        /// <summary>
        /// Returns a hash code value for this attribute.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return x + 37 * y + 43 * w + 47 * h;
        }

        /// <summary>
        /// Convert From Micrometers
        /// </summary>
        /// <param name="x"></param>
        /// <param name="units"></param>
        /// <returns></returns>
        private static float ConvertFromMicrometers(int x, int units)
        {
            if (units < 1)
            {
                throw new ArgumentException("units is < 1");
            }
            return ((float)x) / ((float)units);
        }

    }
}
