using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MAmtDimension : Lookup
    {
        private static KeyNamePair NO_INSTANCE = new KeyNamePair(0, "");


          /// <summary>
        ///	Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="WindowNo"></param>
        public MAmtDimension(Ctx ctx, int windowNo)
            : base(ctx, windowNo, DisplayType.AmtDimension)
        {

        }

        /// <summary>
        ///	Dispose
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        public override List<NamePair> GetData(bool mandatory, bool onlyValidated, bool onlyActive, bool temporary)
        {
            List<NamePair> list = new List<NamePair>();
            if (!mandatory)
                list.Add(new KeyNamePair(-1, ""));
            //
            StringBuilder sql = new StringBuilder(
                    "SELECT C_DimAmt_ID,Amount from C_DimAmt WHERE AD_Client_ID = @ClientId AND (AD_Org_ID = 0 OR @parameter = 0)");
            if (onlyActive)
                sql.Append(" AND IsActive='Y'");
            sql.Append(" ORDER BY 1");
            System.Data.IDataReader dr = null;
            System.Data.SqlClient.SqlParameter[] param = null;
            try
            {
                param = new System.Data.SqlClient.SqlParameter[2];
                param[0] = new System.Data.SqlClient.SqlParameter("@ClientId", GetCtx().GetAD_Client_ID(_WindowNo));
                param[1] = new System.Data.SqlClient.SqlParameter("@parameter", GetCtx().GetAD_Org_ID(_WindowNo));

                dr = DataBase.DB.ExecuteReader(sql.ToString(), param);
                while (dr.Read())
                {
                    int key = Utility.Util.GetValueOfInt(dr[0]);//.getInt(1);
                    String desc = Utility.Util.GetValueOfString(dr[1]);//.getString(2);
                    if (desc == "")
                        desc = "{" + key + "}";
                    list.Add(new KeyNamePair(key, desc));
                }
                dr.Close();
                dr = null;
                param = null;
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                    param = null;
                }
                log.Log(Level.SEVERE, sql.ToString(), e);
            }
            // Sort & return
            return list;
        }

        public override string GetColumnName()
        {
            return "C_DimAmt_ID";
        }

        public override NamePair Get(object value)
        {
            if (value == null)
                return null;
            int C_DimAmt_ID = 0;
            if (value is int)
            {
                C_DimAmt_ID = (int)value;
            }
            else
            {
                try
                {
                    C_DimAmt_ID = int.Parse(value.ToString());
                }
                catch (Exception e)
                {
                    log.Log(Level.SEVERE, "Value=" + value, e);
                }
            }
            if (C_DimAmt_ID == 0)
            {
                return NO_INSTANCE;
            }

            String description = null;
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader("SELECT Amount "
                    + "FROM C_DimAmt "
                    + "WHERE C_DimAmt_ID=" + C_DimAmt_ID);

                if (dr.Read())
                {
                    description = dr[0].ToString();
                    if (description == null || description.Length == 0)
                    {
                        if (VLogMgt.IsLevelFinest())
                        {
                            description = "{" + C_DimAmt_ID.ToString() + "}";
                        }
                        else
                        {
                            description = "";
                        }
                    }
                }
                dr.Close();
                dr = null;
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                log.Log(Level.SEVERE, "get", e);
            }
            if (description == null)
            {
                return null;
            }
            return new KeyNamePair(C_DimAmt_ID, description);

        }

        public override string GetDisplay(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return "";
            }
            NamePair pp = Get(value);
            if (pp == null)
            {
                return "<" + value.ToString() + ">";
            }
            return pp.GetName();
        }
    }
}
