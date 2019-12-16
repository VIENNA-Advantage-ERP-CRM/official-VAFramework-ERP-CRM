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
using System.Drawing.Printing;
using VAdvantage.Print;

namespace VAdvantage.Model
{
    public class MPrintPaper : X_AD_PrintPaper
    {
        /// <summary>
        /// Get Paper
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_PrintPaper_ID"AD_PrintPaper_ID></param>
        /// <returns>Paper</returns>
        static public MPrintPaper Get(int AD_PrintPaper_ID)
        {
            int key = AD_PrintPaper_ID;            
            MPrintPaper pp = null;
            if(s_papers.ContainsKey(key))
                pp = s_papers[key];
            if (pp == null)
            {
                pp = new MPrintPaper(Env.GetContext(), AD_PrintPaper_ID, null);
                if(s_papers.ContainsKey(key))
                    s_papers[key] = pp;
                else
                    s_papers.Add(key, pp);
            }
            else
                s_log.Config("AD_PrintPaper_ID=" + AD_PrintPaper_ID);
            return pp;
        }	//	get

        /**	Logger				*/
        private static VLogger s_log = VLogger.GetVLogger(typeof(MPrintPaper).FullName);
        /** Cached Fonts						*/
        private static CCache<int, MPrintPaper> s_papers = new CCache<int, MPrintPaper>("AD_PrintPaper", 5);


        /// <summary>
        /// Create Paper and save
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="landscape">landscape</param>
        /// <returns>Paper</returns>
        static MPrintPaper Create(String name, bool landscape)
        {
            MPrintPaper pp = new MPrintPaper(Env.GetContext(), 0, null);
            pp.SetName(name);
            pp.SetIsLandscape(landscape);
            pp.Save();
            return pp;
        }	//	create

        public MPrintPaper(Context ctx, int AD_PrintPaper_ID, Trx trx)
            : base(ctx, AD_PrintPaper_ID, trx)
        {

            if (AD_PrintPaper_ID == 0)
            {
                SetIsDefault(false);
                SetIsLandscape(true);
                SetCode("iso-a4");
                SetMarginTop(36);
                SetMarginBottom(36);
                SetMarginLeft(36);
                SetMarginRight(36);
            }
        }	//	MPrintPaper


        public MPrintPaper(Ctx ctx, int AD_PrintPaper_ID, Trx trx)
            : base(ctx, AD_PrintPaper_ID, trx)
        {

            if (AD_PrintPaper_ID == 0)
            {
                SetIsDefault(false);
                SetIsLandscape(true);
                SetCode("iso-a4");
                SetMarginTop(36);
                SetMarginBottom(36);
                SetMarginLeft(36);
                SetMarginRight(36);
            }
        }	//	MPrintPaper


        public MPrintPaper(Context ctx, DataRow rs, Trx trx)
            : base(ctx, rs, trx)
        {
        }	//	MPrintPaper

        public MPrintPaper(Ctx ctx, DataRow rs, Trx trx)
            : base(ctx, rs, trx)
        {
        }


        /** Media Size			*/
        private MediaSize m_mediaSize = null;


        public MediaSize GetMediaSize()
        {
            if (m_mediaSize != null)
                return m_mediaSize;

            String nameCode = GetCode();
            if (nameCode != null)
            {
                //	Get Name
                MediaSizeName nameMedia = null;
                CMediaSizeName msn = new CMediaSizeName(4);
                String[] names = msn.GetStringTable();
                for (int i = 0; i < names.Length; i++)
                {
                    String name = names[i];
                    if (name.Equals(nameCode, StringComparison.InvariantCultureIgnoreCase))
                    {
                        nameMedia = (MediaSizeName)msn.GetEnumValueTable()[i];
                        log.Finer("Name=" + nameMedia);
                        break;
                    }
                }
                if (nameMedia != null)
                {
                    m_mediaSize = MediaSize.GetMediaSizeForName(nameMedia);
                    log.Fine("Name->Size=" + m_mediaSize);
                }
            }
            //	Create New Media Size
            if (m_mediaSize == null)
            {
                float x = (float)GetSizeX();
                float y = (float)GetSizeY();
                if (x > 0 && y > 0)
                {
                    m_mediaSize = new MediaSize(x, y, GetUnitsInt(), MediaSizeName.A);
                    log.Fine("Size=" + m_mediaSize);
                }
                //	Fallback
                if (m_mediaSize == null)
                    m_mediaSize = GetMediaSizeDefault();
                return m_mediaSize;
            }
            return m_mediaSize;
        }

        public MediaSize GetMediaSizeDefault()
        {
            m_mediaSize = VAdvantage.Login.Language.GetLoginLanguage().GetMediaSize();
            if (m_mediaSize == null)
                m_mediaSize = MediaSize.ISO.A4;
            log.Fine("Size=" + m_mediaSize);
            return m_mediaSize;
        }	//	getMediaSizeDefault

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetUnitsInt()
        {
            String du = GetDimensionUnits();
            if (du == null || DIMENSIONUNITS_MM.Equals(du))
                return Size2DSyntax.MM;
            else
                return Size2DSyntax.INCH;
        }	//	getUnits

        public CPaper GetCPaper()
        {
            CPaper retValue = new CPaper(GetMediaSize(), IsLandscape(), GetMarginLeft(), GetMarginTop(), GetMarginRight(), GetMarginBottom());
            return retValue;
        }	//	getCPaper

        class CMediaSizeName : MediaSizeName
        {
            public CMediaSizeName(int code)
                : base(code)
            {
                
            }	//	CMediaSizeName

            //public String[] GetStringTable()
            //{
            //    return base.GetStringTable();
            //}

            public new EnumSyntax[] GetEnumValueTable()
            {
                return (EnumSyntax[])base.GetEnumValueTable();
            }
        }

    }
}
