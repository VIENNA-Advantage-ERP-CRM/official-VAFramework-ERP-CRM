/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MColor
 * Purpose        : Color Persistent Object Model
 * Class Used     : X_AD_Color
 * Chronological    Development
 * Deepak           12-Feb-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Drawing;

namespace VAdvantage.Model
{
    public class MColor : X_AD_Color
    { 
        /// <summary>
        /// Color Model
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Color_ID"></param>
        /// <param name="trxName"></param>
        public MColor(Ctx ctx, int AD_Color_ID, Trx trxName)
            : base(ctx, AD_Color_ID, trxName)
        {
            if (AD_Color_ID == 0)
            {
                SetName("-/-");
            }
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>string</returns>
        public override String ToString()
        {
            return "MColor[ID=" + Get_ID() + " - " + GetName() + "]";
        }

        /// <summary>
        /// Load Special data (images, ..).
        /// To be extended by sub-classes
        /// </summary>
        /// <param name="idr"></param>
        /// <param name="index">zero based index</param>
        /// <returns>value</returns>
        protected Object LoadSpecial(IDataReader idr, int index)
        {
            log.Config(p_info.GetColumnName(index));
            if (index == Get_ColumnIndex("ColorType"))
            {
                return Utility.Util.GetValueOfString(idr[index]);//.getString(index + 1);
            }
            return null;
        }


        /// <summary>
        /// Save Special Data.
        /// AD_Image_ID (Background)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns>SQL code for INSERT VALUES clause</returns>
        protected new String SaveNewSpecial(Object value, int index)
        {
            String colName = p_info.GetColumnName(index);
            String colValue = value == null ? "null" : value.GetType().FullName.ToString();//getClass().toString();
            log.Fine(colName + "=" + colValue);
            if (value == null)
            {
                return "NULL";
            }
            return value.ToString();
        }


       

        /// <summary>
        ///  Get Color
        /// </summary>
        /// <param name="primary">true if primary false if secondary</param>
        /// <returns>Color</returns>
        private Color GetColor(bool primary)
        {
            int red = primary ? GetRed() : GetRed_1();
            int green = primary ? GetGreen() : GetGreen_1();
            int blue = primary ? GetBlue() : GetBlue_1();
            //
            //return new Color(red, green, blue);
            return Color.FromArgb(red, green, blue);
        }


        /// <summary>
        ///  Get URL from Image
        /// </summary>
        /// <param name="AD_Image_ID">image</param>
        /// <returns>URL as String or null</returns>
        private String GetURL(int AD_Image_ID)
        {
            if (AD_Image_ID == 0)
            {
                return null;
            }
            //
            String retValue = null;
            String sql = "SELECT ImageURL FROM AD_Image WHERE AD_Image_ID=" + AD_Image_ID;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                if (idr.Read())
                {
                    retValue = Utility.Util.GetValueOfString(idr[0]);//.getString(1);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                log.Log(Level.SEVERE, sql, e);
            }
            return retValue;
        }

    }
}
