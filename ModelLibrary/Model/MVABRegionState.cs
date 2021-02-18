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
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;


namespace VAdvantage.Model
{
    [Serializable]
    public class MVABRegionState : X_VAB_RegionState, IComparer<PO>
    {
        /* 	Load Regions (cached)
         *	@param ctx context
         */
        private static void LoadAllRegions(Ctx ctx)
        {
            s_regions = new CCache<String, MVABRegionState>("VAB_RegionState", 100);
            String sql = "SELECT * FROM VAB_RegionState WHERE IsActive='Y'";
            try
            {
                DataSet stmt = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < stmt.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = stmt.Tables[0].Rows[i];
                    MVABRegionState r = new MVABRegionState(ctx, rs, null);
                    s_regions.Add(r.GetVAB_RegionState_ID().ToString(), r);
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
         *	@param VAB_RegionState_ID ID
         *	@return Country
         */
        public static MVABRegionState Get(Ctx ctx, int VAB_RegionState_ID)
        {
            if (s_regions == null || s_regions.Count == 0)
                LoadAllRegions(ctx);
            String key = VAB_RegionState_ID.ToString();
            MVABRegionState r = (MVABRegionState)s_regions[key];
            if (r != null)
                return r;
            r = new MVABRegionState(ctx, VAB_RegionState_ID, null);
            if (r.GetVAB_RegionState_ID() == VAB_RegionState_ID)
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
        public static MVABRegionState GetDefault(Ctx ctx)
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
        public static MVABRegionState[] GetRegions(Ctx ctx)
        {
            if (s_regions == null || s_regions.Count == 0)
                LoadAllRegions(ctx);
            MVABRegionState[] retValue = new MVABRegionState[s_regions.Count];
            retValue = s_regions.Values.ToArray();
            Array.Sort(retValue, new MVABRegionState(ctx, 0, null));
            return retValue;
        }

        /**
         *	Return Array of Regions of Country
         * 	@param ctx context
         *  @param VAB_Country_ID country
         *  @return MRegion Array
         */
        //@SuppressWarnings("unchecked")
        public static MVABRegionState[] GetRegions(Ctx ctx, int VAB_Country_ID)
        {
            if (s_regions == null || s_regions.Count == 0)
                LoadAllRegions(ctx);
            List<MVABRegionState> list = new List<MVABRegionState>();
            //iterator it = s_regions.Values.iterator();
            IEnumerator it = s_regions.Values.GetEnumerator();
            while (it.MoveNext())
            {
                MVABRegionState r = (MVABRegionState)it.Current;
                if (r.GetVAB_Country_ID() == VAB_Country_ID)
                    list.Add(r);
            }
            //  Sort it
            MVABRegionState[] retValue = new MVABRegionState[list.Count];
            retValue = list.ToArray();
            Array.Sort(retValue, new MVABRegionState(ctx, 0, null));
            return retValue;
        }

        /**	Region Cache				*/
        private static CCache<String, MVABRegionState> s_regions = null;
        /** Default Region				*/
        private static MVABRegionState s_default = null;
        //	Static Logger				
        private static VLogger _log = VLogger.GetVLogger(typeof(MVABRegionState).FullName);


        /**************************************************************************
         *	Create empty Region
         * 	@param ctx context
         * 	@param VAB_RegionState_ID id
         *	@param trxName transaction
         */
        public MVABRegionState(Ctx ctx, int VAB_RegionState_ID, Trx trxName)
            : base(ctx, VAB_RegionState_ID, trxName)
        {

            if (VAB_RegionState_ID == 0)
            {
            }
        }

        /**
         *	Create Region from current row in ResultSet
         * 	@param ctx context
         *  @param rs result set
         *	@param trxName transaction
         */
        public MVABRegionState(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

        /**
         * 	Parent Constructor
         *	@param country country
         *	@param regionName Region Name
         */
        public MVABRegionState(MVABCountry country, String regionName)
            : base(country.GetCtx(), 0, country.Get_TrxName())
        {
            SetVAB_Country_ID(country.GetVAB_Country_ID());
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