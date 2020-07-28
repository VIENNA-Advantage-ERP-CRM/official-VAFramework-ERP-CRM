using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Utility;

namespace VAdvantage.Print
{
    public class ParameterElement : GridElement
    {
        public ParameterElement(Query query, Ctx ctx, MPrintTableFormat tFormat)
            : base(query.GetRestrictionCount(), 4)
        {
            
            SetData(0, 0, Msg.GetMsg(ctx, "Parameter") + ":", tFormat.GetPageHeader_Font(), tFormat.GetPageHeaderFG_Color());
           // html = "<div><span ></span>";
            for (int r = 0; r < query.GetRestrictionCount(); r++)
            {
                SetData(r, 1, query.GetInfoName(r), tFormat.GetParameter_Font(), tFormat.GetParameter_Color());
                SetData(r, 2, query.GetInfoOperator(r), tFormat.GetParameter_Font(), tFormat.GetParameter_Color());
                SetData(r, 3, query.GetInfoDisplayAll(r), tFormat.GetParameter_Font(), tFormat.GetParameter_Color());
            }
        }	//	ParameterElement

        public ParameterElement(Query query, Ctx ctx, MPrintTableFormat tFormat,out string html)
            : base(query.GetRestrictionCount(), 4)
        {
            
            SetData(0, 0, Msg.GetMsg(ctx, "Parameter") + ":", tFormat.GetPageHeader_Font(), tFormat.GetPageHeaderFG_Color());
            StringBuilder sb = new StringBuilder("<div class='vis-report-content-head'><div>" + Msg.GetMsg(ctx, "Parameter") + ":" + " </div><div>");

            sb.Append("<table style='white-space: nowrap;'>");        
            for (int r = 0; r < query.GetRestrictionCount(); r++)
            {
                
                sb.Append("<tr><td>" + query.GetInfoName(r) + "</td>");
                sb.Append("<td>" + query.GetInfoOperator(r) + "</td>");
                sb.Append("<td>" + query.GetInfoDisplayAll(r) + "</td></tr>");

                SetData(r, 1, query.GetInfoName(r), tFormat.GetParameter_Font(), tFormat.GetParameter_Color());
                SetData(r, 2, query.GetInfoOperator(r), tFormat.GetParameter_Font(), tFormat.GetParameter_Color());
                SetData(r, 3, query.GetInfoDisplayAll(r), tFormat.GetParameter_Font(), tFormat.GetParameter_Color());
            }
            sb.Append("</table></div></div>");        
            html = sb.ToString();
            sb = null;
        }	
    }
}
