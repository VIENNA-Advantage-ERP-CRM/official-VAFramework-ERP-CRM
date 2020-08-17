/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MPAttributeLookup
 * Purpose        : 
 * Chronological    Development
 * Raghunandan     26-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    [Serializable]
    public class MPAttributeLookup : Lookup
    {
        //	Statement					
        //private DataSet ds = null;
        //	No Instance Value			
        private static KeyNamePair NO_INSTANCE = new KeyNamePair(0, "");

        /// <summary>
        ///	Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="WindowNo"></param>
        public MPAttributeLookup(Ctx ctx, int windowNo)
            : base(ctx, windowNo, DisplayType.TableDir)
        {

        }

        /// <summary>
        ///Get Display for Value (not cached)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override String GetDisplay(Object value)
        {
            if (value == null || value ==DBNull.Value)
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

        /// <summary>
        /// The Lookup contains the key (not cached)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(Object key)
        {
            return Get(key) != null;
        }

        /// <summary>
        ///Get Object of Key Value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override NamePair Get(Object value)
        {
            if (value == null)
                return null;
            int M_AttributeSetInstance_ID = 0;
            if (value is int)
            {
                M_AttributeSetInstance_ID = (int)value;
            }
            else
            {
                try
                {
                    M_AttributeSetInstance_ID = int.Parse(value.ToString());
                }
                catch (Exception e)
                {
                    log.Log(Level.SEVERE, "Value=" + value, e);
                }
            }
            if (M_AttributeSetInstance_ID == 0)
            {
                return NO_INSTANCE;
            }
            //
            //	Statement
            // if (ds == null)


            //
            String description = null;
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader("SELECT Description "
                    + "FROM M_AttributeSetInstance "
                    + "WHERE M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID);

                if (dr.Read())
                {
                    description = dr[0].ToString();
                    if (description == null || description.Length == 0)
                    {
                        if (VLogMgt.IsLevelFinest())
                        {
                            description = "{" + M_AttributeSetInstance_ID.ToString() + "}";
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
            return new KeyNamePair(M_AttributeSetInstance_ID, description);
        }

        /// <summary>
        ///	Dispose
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        /// <summary>
        /// Return data as sorted Array - not implemented
        /// </summary>
        /// <param name="mandatory"></param>
        /// <param name="onlyValidated"></param>
        /// <param name="onlyActive"></param>
        /// <param name="temporary"></param>
        /// <returns></returns>
        public override List<NamePair> GetData(bool mandatory, bool onlyValidated, bool onlyActive, bool temporary)
        {

            List<NamePair> list = new List<NamePair>();
            if (!mandatory)
                list.Add(new KeyNamePair(-1, ""));
            //
            StringBuilder sql = new StringBuilder(
                    "SELECT ASI.M_AttributeSetInstance_ID, ASI.Description from M_AttributeSetInstance ASI, M_Product P WHERE ASI.M_AttributeSet_ID = P.M_AttributeSet_ID AND P.M_Product_ID = " + GetCtx().GetContextAsInt(_WindowNo, "M_Product_ID"));
            if (onlyActive)
                sql.Append(" AND ASI.IsActive='Y'");
            sql.Append(" ORDER BY 2");
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql.ToString(), null, null);

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
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Close();
                }
                log.Log(Level.SEVERE, sql.ToString(), e);
            }

            //  Sort & return
            return list;
        }

        /// <summary>
        ///	Get underlying fully qualified Table.Column Name.
        /// 	Used for VLookup.actionButton (Zoom)
        //   @return column name
        /// </summary>
        /// <returns></returns>
        public override String GetColumnName()
        {
            return "M_AttributeSetInstance_ID";
        }
    }
}
