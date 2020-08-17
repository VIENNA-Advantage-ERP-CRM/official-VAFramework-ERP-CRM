/*
 * Date : 1-July-2009
 * Author: Jagmohan Bhatt
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Model;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Logging;
using System.Drawing;

namespace VAdvantage.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class MPrintColor : X_AD_PrintColor
    {
        //	Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MPrintColor).FullName);
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">DataRow</param>
        /// <param name="trxName">Transaction</param>
        public MPrintColor(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }	//	MPrintColor

        static MPrintColor Create(Color color, String name)
        {
            MPrintColor pc = new MPrintColor(Env.GetContext(), 0, null);
            pc.SetName(name);
            pc.SetColor(color);
            pc.Save();
            return pc;
        }	//	create



        /** Dark Green			*/
        public static Color darkGreen = Color.FromArgb(0, 128, 0);
        /** Black Green			*/
        public static Color blackGreen = Color.FromArgb(0, 64, 0);
        /** Dark Blue			*/
        public static Color darkBlue = Color.FromArgb(0, 0, 128);
        /** Black Blue			*/
        public static Color blackBlue = Color.FromArgb(0, 0, 64);
        /** White Gray			*/
        public static Color whiteGray = Color.FromArgb(224, 224, 224);
        /** Brown				*/
        public static Color brown = Color.FromArgb(153, 102, 51);
        /** Dark Brown			*/
        public static Color darkBrown = Color.FromArgb(102, 51, 0);

        static private CCache<int, MPrintColor> _colors = new CCache<int, MPrintColor>("AD_PrintColor", 20);


        static public MPrintColor Get(Ctx ctx, int AD_PrintColor_ID)
        {
            //	if (AD_PrintColor_ID == 0)
            //		return new MPrintColor (ctx, 0);
            //Info
            int key = AD_PrintColor_ID;
            MPrintColor pc = (MPrintColor)_colors[key];
            if (pc == null)
            {
                pc = new MPrintColor(ctx, AD_PrintColor_ID, null);
                _colors.Add(key, pc);
            }
            return pc;
        }	//	get

        static public MPrintColor Get(Ctx ctx, String AD_PrintColor_ID)
        {
            if (AD_PrintColor_ID == null || AD_PrintColor_ID.Length == 0)
                return null;
            try
            {
                int id = int.Parse(AD_PrintColor_ID);
                return Get(ctx, id);
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, "AD_PrintColor_ID=" + AD_PrintColor_ID
                + " - " + e.ToString());
            }
            return null;
        }


        public MPrintColor(Ctx ctx, int AD_PrintColor_ID, Trx trxName)
            : base(ctx, AD_PrintColor_ID, trxName)
        {
            if (AD_PrintColor_ID == 0)
                SetIsDefault(false);
        }	//	MPrintColor

        private Color? _cacheColor = null;

        public Color GetColor()
        {
            if (_cacheColor != null)
            {
                return (Color)_cacheColor;
            }
            String code = GetCode();
            if (code == null || code.Equals("."))
            {
                _cacheColor = Color.Black;
            }
            try
            {
                if (code != null && !code.Equals("."))
                {
                    int rgba = int.Parse(code);
                    _cacheColor = Color.FromArgb(rgba);
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "MPrintColor.getColor", e);
            }
            if (code == null)
            {
                _cacheColor = Color.Black;
            }
            //	log.fine( "MPrintColor.getColor " + code, m_cacheColor);
            if (_cacheColor != null)
            {
                return (Color)_cacheColor;
            }
            else
            {
                return Color.Empty;
            }
        }

        /// <summary>
        /// Get Color
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(Color color)
        {
            int rgba = color.ToArgb();
            base.SetCode(rgba.ToString());
        }

    }
}
