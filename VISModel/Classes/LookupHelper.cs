using System;

using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Classes
{
    public class LookupHelper
    {
        public static Lookup GetLookup(Ctx ctx, int windowNo, int Column_ID, int VAF_Control_Ref_ID,
                 String columnName, int VAF_Control_Ref_Value_ID,
                bool IsParent, String ValidationCode)
        {
            return VLookUpFactory.Get(ctx, windowNo, Column_ID, VAF_Control_Ref_ID, columnName, VAF_Control_Ref_Value_ID, IsParent, ValidationCode);
        }

        public static string [] GetKeyColumns(int VAF_TableView_ID,Ctx ctx)
        {
            //return new MVAFTableView(ctx, VAF_TableView_ID, null).GetKeyColumns();
            return MVAFTableView.Get(ctx, VAF_TableView_ID).GetKeyColumns();
        }
    }

}