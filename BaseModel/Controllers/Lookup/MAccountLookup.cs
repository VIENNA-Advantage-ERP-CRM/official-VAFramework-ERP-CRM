/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MPAttributeLookup
 * Purpose        : used for Lookup account and C_ValidCombination table
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
    public class MAccountLookup : Lookup
    {
        //Account_ID			
        public int C_ValidCombination_ID;
        private String Combination;
        private String Description;

       /// <summary>
       /// Constructor
       /// </summary>
       /// <param name="ctx"></param>
       /// <param name="WindowNo"></param>
        public MAccountLookup(Ctx ctx, int windowNo)
            : base(ctx, windowNo, DisplayType.TableDir)
        {

        }

       /// <summary>
       /// Get Display for Value
       /// </summary>
       /// <param name="value"></param>
       /// <returns></returns>
        public override String GetDisplay(Object value)
        {
            if (!ContainsKey(value))
                return "<" + value.ToString() + ">";
            return ToString();
        }

       /// <summary>
       /// Get Object of Key Value
       /// </summary>
       /// <param name="value"></param>
       /// <returns></returns>
        public override NamePair Get(Object value)
        {
            if (value == null)
                return null;
            if (!ContainsKey(value))
                return null;
            return new KeyNamePair(C_ValidCombination_ID, ToString());
        }
        /// <summary>
        /// The Lookup contains the key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(Object key)
        {
            int intValue = 0;
            if (key is int)
                intValue = (int)key;
            else if (key != null && key != DBNull.Value )
                intValue = int.Parse(key.ToString());
            //
            return Load(intValue);
        }

        /// <summary>
        /// Get Description
        /// </summary>
        /// <returns></returns>
        public String GetDescription()
        {
            return Description;
        }

        /// <summary>
        ///Return String representation
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            if (C_ValidCombination_ID == 0)
                return "";
            return Combination;
        }

        /// <summary>
        ///Load C_ValidCombination with ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool Load(int ID)
        {
            if (ID == 0)						//	new
            {
                C_ValidCombination_ID = 0;
                Combination = "";
                Description = "";
                return true;
            }
            if (ID == C_ValidCombination_ID)	//	already loaded
                return true;

            String sql = "SELECT C_ValidCombination_ID, Combination, Description "
                + "FROM C_ValidCombination WHERE C_ValidCombination_ID=" + ID;
            IDataReader dr = null;
            try
            {
                //	Prepare Statement
                dr = DataBase.DB.ExecuteReader(sql, null, null);
                if (dr.Read())
                {
                    C_ValidCombination_ID = Utility.Util.GetValueOfInt(dr[0]);
                    Combination = dr[1].ToString();//.getString(2);
                    Description = dr[2].ToString();//.getString(3);

                    dr.Close();
                    dr = null;
                    return true;
                }
                else
                {
                    dr.Close();
                    dr = null;
                    return false;
                }
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                log.Severe(e.ToString());
                return false;
            }
           // return true;
        }

        /// <summary>
        ///Get underlying fully qualified Table.Column Name
        /// </summary>
        /// <returns></returns>
        public override String GetColumnName()
        {
            return "";
        }

       /// <summary>
       /// Return data as sorted Array.
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
            StringBuilder sql = new StringBuilder("SELECT C_ValidCombination_ID, Combination, Description "
                + "FROM C_ValidCombination WHERE AD_Client_ID=" + GetCtx().GetAD_Client_ID());
            if (onlyActive)
                sql.Append(" AND IsActive='Y'");
            sql.Append(" ORDER BY 2");

            IDataReader dr=null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql.ToString(), null, null);
                while (dr.Read())
                {
                    //list.Add(new KeyNamePair(dr.getInt(1), dr.getString(2) + " - " + dr.getString(3)));
                    list.Add(new KeyNamePair(Utility.Util.GetValueOfInt(dr[0]), dr[1].ToString() + " - " + dr[2].ToString()));
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
                log.Log(Level.SEVERE, sql.ToString(), e);
            }
            

            //  Sort & return
            return list;
        }
    }
}
