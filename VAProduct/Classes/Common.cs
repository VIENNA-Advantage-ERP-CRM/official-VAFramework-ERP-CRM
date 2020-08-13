using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAModelAD.Model;

namespace VAModelAD.Classes
{
    public class Common
    {

        public static VLookUpInfo GetColumnLookupInfo(Ctx ctx,POInfoColumn colInfo)
        {
            
                if (colInfo == null)
                    return null;
            //
            int WindowNo = 0;
            //  List, Table, TableDir
            VLookUpInfo lookup = null;
            try
            {
                lookup = VLookUpFactory.GetLookUpInfo(ctx, WindowNo, colInfo.DisplayType,colInfo.AD_Column_ID, Env.GetLanguage(ctx),
                   colInfo.ColumnName,colInfo.AD_Reference_Value_ID,
                   colInfo.IsParent,colInfo.ValidationCode);
            }
            catch
            {


                lookup = null;          //  cannot create Lookup
            }
            return lookup;


        }

        public static bool IsMultiLingualDocument(Ctx ctx)
        {
            return MClient.Get((Ctx)ctx).IsMultiLingualDocument();//
            //MClient.get(ctx).isMultiLingualDocument();
        }

        internal static string GetLanguageCode()
        {
            return "en_US";
        }
    }
}
