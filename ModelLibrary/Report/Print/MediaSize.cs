using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace VAdvantage.Print
{
    /// <summary>
    /// Class MediaSize is a two-dimensional size valued printing attribute class 
    /// that indicates the dimensions of the medium in a portrait orientation, with
    /// the X dimension running along the bottom edge and the Y dimension running
    /// along the left edge.
    /// </summary>
    public class MediaSize : Size2DSyntax
    {
        private MediaSizeName mediaName;

        private static Hashtable  mediaMap = new Hashtable(100);

        private static ArrayList sizeVector = new ArrayList(100);


        /// <summary>
        /// Construct a new media size attribute from the given floating-point values
        /// </summary>
        /// <param name="x">X dimension.</param>
        /// <param name="y">Y dimension.</param>
        /// <param name="units">Unit conversion factor, e.g. Size2DSyntax.INCH or Size2DSyntax.MM</param>
        public MediaSize(float x, float y, int units) : base(x, y, units)
        {
            if (x > y)
            {
                throw new ArgumentException("X dimension > Y dimension");
            }
            sizeVector.Add(this);
        }

        /// <summary>
        /// Construct a new media size attribute from the given integer values. 
        /// </summary>
        /// <param name="x">X dimension.</param>
        /// <param name="y">Y dimension.</param>
        /// <param name="units">Unit conversion factor, e.g. Size2DSyntax.INCH or Size2DSyntax.MM</param>
        public MediaSize(int x, int y, int units)
            : base(x, y, units)
        {

            if (x > y)
            {
                throw new ArgumentException("X dimension > Y dimension");
            }
            sizeVector.Add(this);
        }

        /// <summary>
        /// Construct a new media size attribute from the given floating-point values
        /// </summary>
        /// <param name="x">X dimension.</param>
        /// <param name="y">Y dimension.</param>
        /// <param name="units">Unit conversion factor, e.g. Size2DSyntax.INCH or Size2DSyntax.MM</param>
        /// <param name="media">a media name to associate with this MediaSize</param>
        public MediaSize(float x, float y, int units, MediaSizeName media)
            : base(x, y, units)
        {
            
            if (x > y)
            {
                throw new ArgumentException("X dimension > Y dimension");
            }
            mediaName = media;
            if (mediaMap.ContainsKey(mediaName))
            {
                mediaMap[mediaName] = this;
            }
            else
            {
                mediaMap.Add(mediaName, this);
            }
            sizeVector.Add(this);
        }

        /// <summary>
        /// Construct a new media size attribute from the given int values
        /// </summary>
        /// <param name="x">X dimension.</param>
        /// <param name="y">Y dimension.</param>
        /// <param name="units">Unit conversion factor, e.g. Size2DSyntax.INCH or Size2DSyntax.MM</param>
        /// <param name="media">a media name to associate with this MediaSize</param>
        public MediaSize(int x, int y, int units, MediaSizeName media)
            : base(x, y, units)
        {
            
            if (x > y)
            {
                throw new ArgumentException("X dimension > Y dimension");
            }
            mediaName = media;
            if (mediaMap.ContainsKey(mediaName))
            {
                mediaMap[mediaName] = this;
            }
            else
            {
                mediaMap.Add(mediaName, this);
            }
            sizeVector.Add(this);
        }

        /// <summary>
        /// The specified dimensions are used to locate a matching MediaSize 
        /// instance from amongst all the standard MediaSize instances.
        /// If there is no exact match, the closest match is used.
        /// </summary>
        /// <param name="x">X dimension</param>
        /// <param name="y">Y dimension</param>
        /// <param name="units">Unit conversion factor, e.g. Size2DSyntax.INCH or Size2DSyntax.MM</param>
        /// <returns>matching these dimensions, or null.</returns>
        public static MediaSizeName FindMedia(float x, float y, int units)
        {

            MediaSize match = MediaSize.ISO.A4;

            if (x <= 0.0f || y <= 0.0f || units < 1)
            {
                throw new ArgumentException("args must be +ve values");
            }

            double ls = x * x + y * y;
            double tmp_ls;
            float[] dim;
            float diffx = x;
            float diffy = y;

            for (int i = 0; i < sizeVector.Count; i++)
            {
                MediaSize mediaSize = (MediaSize)sizeVector[i];
                dim = mediaSize.GetSize(units);
                if (x == dim[0] && y == dim[1])
                {
                    match = mediaSize;
                    break;
                }
                else
                {
                    diffx = x - dim[0];
                    diffy = y - dim[1];
                    tmp_ls = diffx * diffx + diffy * diffy;
                    if (tmp_ls < ls)
                    {
                        ls = tmp_ls;
                        match = mediaSize;
                    }
                }
            }
            return match.GetMediaSizeName();
        }

        /// <summary>
        /// Get the MediaSize for the specified named media.
        /// </summary>
        /// <param name="media">the name of the media for which the size is sought</param>
        /// <returns>size of the media, or null if this media is not associated with any size</returns>
        public static MediaSize GetMediaSizeForName(MediaSizeName media)
        {
            return (MediaSize)mediaMap[media];
        }

        /// <summary>
        /// Get the media name, if any, for this size.
        /// </summary>
        /// <returns>the name for this media size, or null if no name was associated with this size (an anonymous size).</returns>
        public string GetMediaName()
        {
            return mediaName.GetStringTable()[mediaName.GetValue()].ToString();
        }

        /// <summary>
        /// Get the media name, if any, for this size.
        /// </summary>
        /// <returns>the name for this media size, or null if no name was associated with this size (an anonymous size).</returns>
        public MediaSizeName GetMediaSizeName()
        {
            return mediaName;
        }

        /// <summary>
        /// Returns whether this media size attribute is equivalent to the passed  in object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(Object obj)
        {
            return (base.Equals(obj) && obj is MediaSize);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    
        /// <summary>
        /// Get the name of the category of which this attribute value is instance
        /// </summary>
        /// <returns>Category Name</returns>
        public String GetName()
        {
            return "media-size";
        }
        /// <summary>
        /// Class MediaSize.ISO includes {@link MediaSize MediaSize} values for ISO media. 
        /// </summary>

        #region ISO
        public static class ISO
        {
            /**
             * Specifies the ISO A0 size, 841 mm by 1189 mm. 
             */
            public static MediaSize
                A0 = new MediaSize(841, 1189, Size2DSyntax.MM, MediaSizeName.ISO_A0);
            /**
             * Specifies the ISO A1 size, 594 mm by 841 mm. 
             */
            public static MediaSize
                A1 = new MediaSize(594, 841, Size2DSyntax.MM, MediaSizeName.ISO_A1);
            /**
             * Specifies the ISO A2 size, 420 mm by 594 mm. 
             */
            public static MediaSize
                A2 = new MediaSize(420, 594, Size2DSyntax.MM, MediaSizeName.ISO_A2);
            /**
             * Specifies the ISO A3 size, 297 mm by 420 mm. 
             */
            public static MediaSize
                A3 = new MediaSize(297, 420, Size2DSyntax.MM, MediaSizeName.ISO_A3);
            /**
             * Specifies the ISO A4 size, 210 mm by 297 mm. 
             */
            public static MediaSize
                A4 = new MediaSize(210, 297, Size2DSyntax.MM, MediaSizeName.ISO_A4);
            /**
             * Specifies the ISO A5 size, 148 mm by 210 mm. 
             */
            public static MediaSize
                A5 = new MediaSize(148, 210, Size2DSyntax.MM, MediaSizeName.ISO_A5);
            /**
             * Specifies the ISO A6 size, 105 mm by 148 mm. 
             */
            public static MediaSize
                A6 = new MediaSize(105, 148, Size2DSyntax.MM, MediaSizeName.ISO_A6);
            /**
             * Specifies the ISO A7 size, 74 mm by 105 mm. 
             */
            public static MediaSize
                A7 = new MediaSize(74, 105, Size2DSyntax.MM, MediaSizeName.ISO_A7);
            /**
             * Specifies the ISO A8 size, 52 mm by 74 mm. 
             */
            public static MediaSize
                A8 = new MediaSize(52, 74, Size2DSyntax.MM, MediaSizeName.ISO_A8);
            /**
             * Specifies the ISO A9 size, 37 mm by 52 mm. 
             */
            public static MediaSize
                A9 = new MediaSize(37, 52, Size2DSyntax.MM, MediaSizeName.ISO_A9);
            /**
             * Specifies the ISO A10 size, 26 mm by 37 mm. 
             */
            public static MediaSize
                A10 = new MediaSize(26, 37, Size2DSyntax.MM, MediaSizeName.ISO_A10);
            /**
             * Specifies the ISO B0 size, 1000 mm by 1414 mm. 
             */
            public static MediaSize
                B0 = new MediaSize(1000, 1414, Size2DSyntax.MM, MediaSizeName.ISO_B0);
            /**
             * Specifies the ISO B1 size, 707 mm by 1000 mm. 
             */
            public static MediaSize
                B1 = new MediaSize(707, 1000, Size2DSyntax.MM, MediaSizeName.ISO_B1);
            /**
             * Specifies the ISO B2 size, 500 mm by 707 mm. 
             */
            public static MediaSize
                B2 = new MediaSize(500, 707, Size2DSyntax.MM, MediaSizeName.ISO_B2);
            /**
             * Specifies the ISO B3 size, 353 mm by 500 mm. 
             */
            public static MediaSize
                B3 = new MediaSize(353, 500, Size2DSyntax.MM, MediaSizeName.ISO_B3);
            /**
             * Specifies the ISO B4 size, 250 mm by 353 mm. 
             */
            public static MediaSize
                B4 = new MediaSize(250, 353, Size2DSyntax.MM, MediaSizeName.ISO_B4);
            /**
             * Specifies the ISO B5 size, 176 mm by 250 mm. 
             */
            public static MediaSize
                B5 = new MediaSize(176, 250, Size2DSyntax.MM, MediaSizeName.ISO_B5);
            /**
             * Specifies the ISO B6 size, 125 mm by 176 mm. 
             */
            public static MediaSize
                B6 = new MediaSize(125, 176, Size2DSyntax.MM, MediaSizeName.ISO_B6);
            /**
             * Specifies the ISO B7 size, 88 mm by 125 mm. 
             */
            public static MediaSize
                B7 = new MediaSize(88, 125, Size2DSyntax.MM, MediaSizeName.ISO_B7);
            /**
             * Specifies the ISO B8 size, 62 mm by 88 mm. 
             */
            public static MediaSize
                B8 = new MediaSize(62, 88, Size2DSyntax.MM, MediaSizeName.ISO_B8);
            /**
             * Specifies the ISO B9 size, 44 mm by 62 mm. 
             */
            public static MediaSize
                B9 = new MediaSize(44, 62, Size2DSyntax.MM, MediaSizeName.ISO_B9);
            /**
             * Specifies the ISO B10 size, 31 mm by 44 mm. 
             */
            public static MediaSize
                B10 = new MediaSize(31, 44, Size2DSyntax.MM, MediaSizeName.ISO_B10);
            /**
             * Specifies the ISO C3 size, 324 mm by 458 mm. 
             */
            public static MediaSize
                C3 = new MediaSize(324, 458, Size2DSyntax.MM, MediaSizeName.ISO_C3);
            /**
             * Specifies the ISO C4 size, 229 mm by 324 mm. 
             */
            public static MediaSize
                C4 = new MediaSize(229, 324, Size2DSyntax.MM, MediaSizeName.ISO_C4);
            /**
             * Specifies the ISO C5 size, 162 mm by 229 mm. 
             */
            public static MediaSize
                C5 = new MediaSize(162, 229, Size2DSyntax.MM, MediaSizeName.ISO_C5);
            /**
             * Specifies the ISO C6 size, 114 mm by 162 mm. 
             */
            public static MediaSize
                C6 = new MediaSize(114, 162, Size2DSyntax.MM, MediaSizeName.ISO_C6);
            /**
             * Specifies the ISO Designated Long size, 110 mm by 220 mm. 
             */
            public static MediaSize
                DESIGNATED_LONG = new MediaSize(110, 220, Size2DSyntax.MM,
                                MediaSizeName.ISO_DESIGNATED_LONG);

        }
        #endregion

        #region Other
        public static class Other
        {
            /**
             * Specifies the executive size, 7.25 inches by 10.5 inches.
             */
            public static MediaSize
                EXECUTIVE = new MediaSize(7.25f, 10.5f, Size2DSyntax.INCH,
                              MediaSizeName.EXECUTIVE);
            /**
             * Specifies the ledger size, 11 inches by 17 inches.
             */
            public static MediaSize
                LEDGER = new MediaSize(11.0f, 17.0f, Size2DSyntax.INCH,
                           MediaSizeName.LEDGER);

            /**
             * Specifies the tabloid size, 11 inches by 17 inches.
             */
            public static MediaSize
                TABLOID = new MediaSize(11.0f, 17.0f, Size2DSyntax.INCH,
                           MediaSizeName.TABLOID);

            /**
             * Specifies the invoice size, 5.5 inches by 8.5 inches.
             */
            public static MediaSize
                INVOICE = new MediaSize(5.5f, 8.5f, Size2DSyntax.INCH,
                          MediaSizeName.INVOICE);
            /**
             * Specifies the folio size, 8.5 inches by 13 inches.
             */
            public static MediaSize
                FOLIO = new MediaSize(8.5f, 13.0f, Size2DSyntax.INCH,
                          MediaSizeName.FOLIO);
            /**
             * Specifies the quarto size, 8.5 inches by 10.83 inches.
             */
            public static MediaSize
                QUARTO = new MediaSize(8.5f, 10.83f, Size2DSyntax.INCH,
                           MediaSizeName.QUARTO);
            /**
             * Specifies the Italy envelope size, 110 mm by 230 mm. 
             */
            public static MediaSize
            ITALY_ENVELOPE = new MediaSize(110, 230, Size2DSyntax.MM,
                               MediaSizeName.ITALY_ENVELOPE);
            /**
             * Specifies the Monarch envelope size, 3.87 inch by 7.5 inch. 
             */
            public static MediaSize
            MONARCH_ENVELOPE = new MediaSize(3.87f, 7.5f, Size2DSyntax.INCH,
                             MediaSizeName.MONARCH_ENVELOPE);
            /**
             * Specifies the Personal envelope size, 3.625 inch by 6.5 inch. 
             */
            public static MediaSize
            PERSONAL_ENVELOPE = new MediaSize(3.625f, 6.5f, Size2DSyntax.INCH,
                             MediaSizeName.PERSONAL_ENVELOPE);
            /**
             * Specifies the Japanese postcard size, 100 mm by 148 mm. 
             */
            public static MediaSize
                JAPANESE_POSTCARD = new MediaSize(100, 148, Size2DSyntax.MM,
                                  MediaSizeName.JAPANESE_POSTCARD);
            /**
             * Specifies the Japanese Double postcard size, 148 mm by 200 mm. 
             */
            public static MediaSize
                JAPANESE_DOUBLE_POSTCARD = new MediaSize(148, 200, Size2DSyntax.MM,
                             MediaSizeName.JAPANESE_DOUBLE_POSTCARD);
        }
        #endregion

        #region NA
           /**
     * Class MediaSize.NA includes {@link MediaSize MediaSize} values for North 
     * American media. 
     */
        public static class NA
        {

            /**
             * Specifies the North American letter size, 8.5 inches by 11 inches.
             */
            public static MediaSize
                LETTER = new MediaSize(8.5f, 11.0f, Size2DSyntax.INCH,
                                        MediaSizeName.NA_LETTER);
            /**
             * Specifies the North American legal size, 8.5 inches by 14 inches.
             */
            public static MediaSize
                LEGAL = new MediaSize(8.5f, 14.0f, Size2DSyntax.INCH,
                                       MediaSizeName.NA_LEGAL);
            /**
             * Specifies the North American 5 inch by 7 inch paper.
             */
            public static MediaSize
                NA_5X7 = new MediaSize(5, 7, Size2DSyntax.INCH,
                           MediaSizeName.NA_5X7);
            /**
             * Specifies the North American 8 inch by 10 inch paper.
             */
            public static MediaSize
                NA_8X10 = new MediaSize(8, 10, Size2DSyntax.INCH,
                           MediaSizeName.NA_8X10);
            /**
             * Specifies the North American Number 9 business envelope size,
             * 3.875 inches by 8.875 inches. 
             */
            public static MediaSize
                NA_NUMBER_9_ENVELOPE =
                new MediaSize(3.875f, 8.875f, Size2DSyntax.INCH,
                      MediaSizeName.NA_NUMBER_9_ENVELOPE);
            /**
             * Specifies the North American Number 10 business envelope size,
             * 4.125 inches by 9.5 inches. 
             */
            public static MediaSize
                NA_NUMBER_10_ENVELOPE =
                new MediaSize(4.125f, 9.5f, Size2DSyntax.INCH,
                      MediaSizeName.NA_NUMBER_10_ENVELOPE);
            /**
             * Specifies the North American Number 11 business envelope size,
             * 4.5 inches by 10.375 inches. 
             */
            public static MediaSize
                NA_NUMBER_11_ENVELOPE =
                new MediaSize(4.5f, 10.375f, Size2DSyntax.INCH,
                      MediaSizeName.NA_NUMBER_11_ENVELOPE);
            /**
             * Specifies the North American Number 12 business envelope size,
             * 4.75 inches by 11 inches. 
             */
            public static MediaSize
                NA_NUMBER_12_ENVELOPE =
                new MediaSize(4.75f, 11.0f, Size2DSyntax.INCH,
                      MediaSizeName.NA_NUMBER_12_ENVELOPE);
            /**
             * Specifies the North American Number 14 business envelope size,
             * 5 inches by 11.5 inches. 
             */
            public static MediaSize
                NA_NUMBER_14_ENVELOPE =
                new MediaSize(5.0f, 11.5f, Size2DSyntax.INCH,
                      MediaSizeName.NA_NUMBER_14_ENVELOPE);

            /**
             * Specifies the North American 6 inch by 9 inch envelope size.
             */
            public static MediaSize
                NA_6X9_ENVELOPE = new MediaSize(6.0f, 9.0f, Size2DSyntax.INCH,
                                MediaSizeName.NA_6X9_ENVELOPE);
            /**
             * Specifies the North American 7 inch by 9 inch envelope size.
             */
            public static MediaSize
                NA_7X9_ENVELOPE = new MediaSize(7.0f, 9.0f, Size2DSyntax.INCH,
                                MediaSizeName.NA_7X9_ENVELOPE);
            /**
             * Specifies the North American 9 inch by 11 inch envelope size.
             */
            public static MediaSize
                NA_9x11_ENVELOPE = new MediaSize(9.0f, 11.0f, Size2DSyntax.INCH,
                                 MediaSizeName.NA_9X11_ENVELOPE);
            /**
             * Specifies the North American 9 inch by 12 inch envelope size.
             */
            public static MediaSize
                NA_9x12_ENVELOPE = new MediaSize(9.0f, 12.0f, Size2DSyntax.INCH,
                                 MediaSizeName.NA_9X12_ENVELOPE);
            /**
             * Specifies the North American 10 inch by 13 inch envelope size.
             */
            public static MediaSize
                NA_10x13_ENVELOPE = new MediaSize(10.0f, 13.0f, Size2DSyntax.INCH,
                                  MediaSizeName.NA_10X13_ENVELOPE);
            /**
             * Specifies the North American 10 inch by 14 inch envelope size.
             */
            public static MediaSize
                NA_10x14_ENVELOPE = new MediaSize(10.0f, 14.0f, Size2DSyntax.INCH,
                                  MediaSizeName.NA_10X14_ENVELOPE);
            /**
             * Specifies the North American 10 inch by 15 inch envelope size.
             */
            public static MediaSize
                NA_10X15_ENVELOPE = new MediaSize(10.0f, 15.0f, Size2DSyntax.INCH,
                                  MediaSizeName.NA_10X15_ENVELOPE);

        }
        #endregion

        #region Engineer

        #endregion
    }
}
