/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRegion
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     05-Jun-2009
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
    public class MRegion : X_C_Region, IComparer<PO>
    {
        /* 	Load Regions (cached)
         *	@param ctx context
         */
        private static void LoadAllRegions(Ctx ctx)
        {
            s_regions = new CCache<String, MRegion>("C_Region", 100);
            String sql = "SELECT * FROM C_Region WHERE IsActive='Y'";
            try
            {
                DataSet stmt = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < stmt.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = stmt.Tables[0].Rows[i];
                    MRegion r = new MRegion(ctx, rs, null);
                    s_regions.Add(r.GetC_Region_ID().ToString(), r);
                    if (r.IsDefault())
                        s_default = r;
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            _log.Fine(s_regions.Count + " - default=" + s_default);
        }

        /**
         * 	Get Country (cached)
         * 	@param ctx context
         *	@param C_Region_ID ID
         *	@return Country
         */
        public static MRegion Get(Ctx ctx, int C_Region_ID)
        {
            if (s_regions == null || s_regions.Count == 0)
                LoadAllRegions(ctx);
            String key = C_Region_ID.ToString();
            MRegion r = (MRegion)s_regions[key];
            if (r != null)
                return r;
            r = new MRegion(ctx, C_Region_ID, null);
            if (r.GetC_Region_ID() == C_Region_ID)
            {
                s_regions.Add(key, r);
                return r;
            }
            return null;
        }

        /**
         * 	Get Default Region
         * 	@param ctx context
         *	@return Region or null
         */
        public static MRegion GetDefault(Ctx ctx)
        {
            if (s_regions == null || s_regions.Count == 0)
                LoadAllRegions(ctx);
            return s_default;
        }

        /**
         *	Return Regions as Array
         * 	@param ctx context
         *  @return MCountry Array
         */
        //@SuppressWarnings("unchecked")
        public static MRegion[] GetRegions(Ctx ctx)
        {
            if (s_regions == null || s_regions.Count == 0)
                LoadAllRegions(ctx);
            MRegion[] retValue = new MRegion[s_regions.Count];
            retValue = s_regions.Values.ToArray();
            Array.Sort(retValue, new MRegion(ctx, 0, null));
            return retValue;
        }

        /**
         *	Return Array of Regions of Country
         * 	@param ctx context
         *  @param C_Country_ID country
         *  @return MRegion Array
         */
        //@SuppressWarnings("unchecked")
        public static MRegion[] GetRegions(Ctx ctx, int C_Country_ID)
        {
            if (s_regions == null || s_regions.Count == 0)
                LoadAllRegions(ctx);
            List<MRegion> list = new List<MRegion>();
            //iterator it = s_regions.Values.iterator();
            IEnumerator it = s_regions.Values.GetEnumerator();
            while (it.MoveNext())
            {
                MRegion r = (MRegion)it.Current;
                if (r.GetC_Country_ID() == C_Country_ID)
                    list.Add(r);
            }
            //  Sort it
            MRegion[] retValue = new MRegion[list.Count];
            retValue = list.ToArray();
            Array.Sort(retValue, new MRegion(ctx, 0, null));
            return retValue;
        }

        /**	Region Cache				*/
        private static CCache<String, MRegion> s_regions = null;
        /** Default Region				*/
        private static MRegion s_default = null;
        //	Static Logger				
        private static VLogger _log = VLogger.GetVLogger(typeof(MRegion).FullName);


        /**************************************************************************
         *	Create empty Region
         * 	@param ctx context
         * 	@param C_Region_ID id
         *	@param trxName transaction
         */
        public MRegion(Ctx ctx, int C_Region_ID, Trx trxName)
            : base(ctx, C_Region_ID, trxName)
        {

            if (C_Region_ID == 0)
            {
            }
        }

        /**
         *	Create Region from current row in ResultSet
         * 	@param ctx context
         *  @param rs result set
         *	@param trxName transaction
         */
        public MRegion(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

        /**
         * 	Parent Constructor
         *	@param country country
         *	@param regionName Region Name
         */
        public MRegion(MCountry country, String regionName)
            : base(country.GetCtx(), 0, country.Get_TrxName())
        {
            SetC_Country_ID(country.GetC_Country_ID());
            SetName(regionName);
        }

        /**
         *	Return Name
         *  @return Name
         */
        public  override String ToString()
        {
            return GetName();
        }

        /**
         *  Compare
         *  @param o1 object 1
         *  @param o2 object 2
         *  @return -1,0, 1
         */
        public new int Compare(PO o1, PO o2)
        {
            String s1 = o1.ToString();
            if (s1 == null)
                s1 = "";
            String s2 = o2.ToString();
            if (s2 == null)
                s2 = "";
            return s1.CompareTo(s2);
        }
    }
}