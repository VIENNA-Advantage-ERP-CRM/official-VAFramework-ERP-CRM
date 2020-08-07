using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using System.Drawing;
using VAdvantage.Classes;
using VAdvantage.Utility;

namespace VAdvantage.Print
{
    public class LocationElement : GridElement
    {
        public LocationElement(Ctx ctx, int C_Location_ID, Font font, Color color) : base(10, 1)		//	max
        {
            SetGap(0, 0);
            MLocation ml = MLocation.Get(ctx, C_Location_ID, null);
            //	log.fine("C_Location_ID=" + C_Location_ID);
            if (ml != null)
            {
                int index = 0;
                if (ml.IsAddressLinesReverse())
                {
                    SetData(index++, 0, ml.GetCountry(true), font, color);
                    //String[] lines = java.util.regex.Pattern.compile("$", java.util.regex.Pattern.MULTILINE).split(ml.GetCityRegionPostal());
                    String[] lines = System.Text.RegularExpressions.Regex.Split(ml.GetCityRegionPostal(), "$", System.Text.RegularExpressions.RegexOptions.Multiline);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (string.IsNullOrEmpty(lines[i]))
                        {
                            continue;
                        }
                        SetData(index++, 0, lines[i], font, color);
                    }
                    if (ml.GetAddress4() != null && ml.GetAddress4().Length > 0)
                        SetData(index++, 0, ml.GetAddress4(), font, color);
                    if (ml.GetAddress3() != null && ml.GetAddress3().Length > 0)
                        SetData(index++, 0, ml.GetAddress3(), font, color);
                    if (ml.GetAddress2() != null && ml.GetAddress2().Length > 0)
                        SetData(index++, 0, ml.GetAddress2(), font, color);
                    if (ml.GetAddress1() != null && ml.GetAddress1().Length > 0)
                        SetData(index++, 0, ml.GetAddress1(), font, color);
                }
                else
                {
                    if (ml.GetAddress1() != null && ml.GetAddress1().Length > 0)
                        SetData(index++, 0, ml.GetAddress1(), font, color);
                    if (ml.GetAddress2() != null && ml.GetAddress2().Length > 0)
                        SetData(index++, 0, ml.GetAddress2(), font, color);
                    if (ml.GetAddress3() != null && ml.GetAddress3().Length > 0)
                        SetData(index++, 0, ml.GetAddress3(), font, color);
                    if (ml.GetAddress4() != null && ml.GetAddress4().Length > 0)
                        SetData(index++, 0, ml.GetAddress4(), font, color);
                    //String[] lines = java.util.regex.Pattern.compile("$", java.util.regex.Pattern.MULTILINE).split(ml.GetCityRegionPostal());
                    String[] lines = System.Text.RegularExpressions.Regex.Split(ml.GetCityRegionPostal(), "$", System.Text.RegularExpressions.RegexOptions.Multiline);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (string.IsNullOrEmpty(lines[i]))
                        {
                            continue;
                        }
                        SetData(index++, 0, lines[i], font, color);
                    }
                    SetData(index++, 0, ml.GetCountry(true), font, color);
                }
            }
        }	//	LocationElement

        
    }
}
