
/********************************************************
 * Module Name    : lookup
 * Purpose        : Address Loaction Lookup model.
 * Class Used     : Ctx.cs,
 * Chronological Development
 * Harwinder        26-june-2009
 ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.Model;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MLocationLookup : Lookup
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="WindowNo">WindowNo window no (to derive AD_Client/Org for new records)</param>
        public MLocationLookup(Ctx ctx, int windowNo)
            : base(ctx, windowNo, DisplayType.TableDir)
        {
        }

        /// <summary>
        ///Get Display for Value (not cached)
        /// </summary>
        /// <param name="value">Location_ID</param>
        /// <returns>display Value</returns>
        public override String GetDisplay(Object value)
        {
            if (value == null || value == DBNull.Value)
                return null;
            MLocation loc = GetLocation(value, null);
            if (loc == null)
                return "<" + value.ToString() + ">";
            return loc.ToString();
        }

        /// <summary>
        ///Get Object of Key Value
        /// </summary>
        /// <param name="value">value</param>
        /// <returns></returns>
        public override NamePair Get(Object value)
        {
            if (value == null)
                return null;
            MLocation loc = GetLocation(value, null);
            if (loc == null)
                return null;
            return new KeyNamePair(loc.GetC_Location_ID(), loc.ToString());
        }

        /// <summary>
        /// The Lookup contains the key 
        /// <param name="key">Location_ID</param>
        /// <returns>true if key known</returns>
        public bool ContainsKey(Object key)
        {
            return GetLocation(key, null) == null;
        }   //  containsKey

        /// <summary>
        ///Get Location
        /// </summary>
        /// <param name="key">ID as string or integer</param>
        /// <param name="trxName">transaction</param>
        /// <returns>Location</returns>
        public MLocation GetLocation(Object key, Trx trxName)
        {
            if (key == null)
                return null;
            int C_Location_ID = 0;
            if (key is int)
                C_Location_ID = (int)key;
            else if (key != null)
                C_Location_ID = int.Parse(key.ToString());
            //
            return GetLocation(C_Location_ID, trxName);
        }

        /// <summary>
        ///Get Location
        /// </summary>
        /// <param name="C_Location_ID">C_Location_ID id</param>
        /// <param name="trxName">transaction</param>
        /// <returns></returns>
        public MLocation GetLocation(int C_Location_ID, Trx trxName)
        {
            return MLocation.Get((Context)GetCtx(), C_Location_ID, trxName);
        }


        /// <summary>
        ///Get underlying fully qualified Table.Column Name.
        ///	Used for VLookup.actionButton (Zoom)
        /// </summary>
        /// <returns>Column</returns>
        public override String GetColumnName()
        {
            return "C_Location_ID";
        }

        /// <summary>
        ///Return data as sorted Array - not implemented
        /// </summary>
        /// <param name="mandatory">mandatory</param>
        /// <param name="onlyValidated">only validated</param>
        /// <param name="onlyActive">only active</param>
        /// <param name="temporary">force load for temporary display</param>
        /// <returns></returns>
        public override List<NamePair> GetData(bool mandatory, bool onlyValidated, bool onlyActive, bool temporary)
        {
            List<NamePair> list = new List<NamePair>();
            if (!mandatory)
                list.Add(new KeyNamePair(-1, ""));
            //
            StringBuilder sql = new StringBuilder(
                    "SELECT C_Location_ID from C_Location WHERE AD_Client_ID = @ClientId AND (AD_Org_ID = 0 OR @parameter = 0)");
            if (onlyActive)
                sql.Append(" AND IsActive='Y'");
            sql.Append(" ORDER BY 1");
            System.Data.IDataReader dr = null;
            System.Data.SqlClient.SqlParameter[] param= null;
            try
            {
                param = new System.Data.SqlClient.SqlParameter[2];
                param[0] = new System.Data.SqlClient.SqlParameter("@ClientId", GetCtx().GetAD_Client_ID(_WindowNo));
                param[1] = new System.Data.SqlClient.SqlParameter("@parameter", GetCtx().GetAD_Org_ID(_WindowNo));

                dr = DataBase.DB.ExecuteReader(sql.ToString(), param);
                while (dr.Read())
                {
                    list.Add(Get(Utility.Util.GetValueOfInt(dr[0])));
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
    }
}
