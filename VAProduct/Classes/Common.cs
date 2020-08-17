using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAModelAD.Model;

namespace VAModelAD.Classes
{
    public class Common
    {
        static public List<string> lstTableName = null;
        static public bool ISTENATRUNNINGFORERP = false;
        public static VLookUpInfo GetColumnLookupInfo(Ctx ctx, POInfoColumn colInfo)
        {
            if (colInfo == null)
                return null;
            int WindowNo = 0;
            //  List, Table, TableDir
            VLookUpInfo lookup = null;
            try
            {
                lookup = VLookUpFactory.GetLookUpInfo(ctx, WindowNo, colInfo.DisplayType, colInfo.AD_Column_ID, Env.GetLanguage(ctx),
                   colInfo.ColumnName, colInfo.AD_Reference_Value_ID,
                   colInfo.IsParent, colInfo.ValidationCode);
            }
            catch
            {
                lookup = null;          //  cannot create Lookup
            }
            return lookup;
        }

        public static Lookup GetColumnLookup(Ctx ctx, POInfoColumn colInfo)
        {
            //
            int WindowNo = 0;
            //  List, Table, TableDir
            Lookup lookup = null;
            try
            {
                lookup = VLookUpFactory.Get(ctx, WindowNo, colInfo.AD_Column_ID,
                    colInfo.DisplayType,
                    colInfo.ColumnName,
                    colInfo.AD_Reference_Value_ID,
                    colInfo.IsParent, colInfo.ValidationCode);
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


        public static void GetAllTable()
        {
            if (lstTableName.Count == 0)
            {
                lstTableName = new List<string>();
                DataSet ds = DB.ExecuteDataset("select tablename from ad_table where isactive='Y'");

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 350)
                    {
                        ISTENATRUNNINGFORERP = true;
                    }
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        lstTableName.Add(Convert.ToString(ds.Tables[0].Rows[i]["TABLENAME"]));
                    }

                }
            }
           
        }

        
    }
}
