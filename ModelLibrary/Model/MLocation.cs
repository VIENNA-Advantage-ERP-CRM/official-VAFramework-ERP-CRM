/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MLocation
 * Purpose        : Location (Address)
 * Class Used     :  X_C_Location, IComparer<PO>
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
using System.Xml;
using System.Web;

namespace VAdvantage.Model
{
    public class MLocation : X_C_Location, IEquatable<Object> //Comparator<PO>
    {


        #region Private Variable
        //	Cache						
        private static CCache<int, MLocation> s_cache = new CCache<int, MLocation>("C_Location", 100, 30);
        private MCountry _country = null;
        private MRegion _region = null;
        //	Static Logger				
        private static VLogger _log = VLogger.GetVLogger(typeof(MLocation).FullName);
        #endregion

        /// <summary>
        /// Get Location from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Location_ID">id</param>
        /// <param name="trxName">transaction</param>
        /// <returns>MLocation</returns>
        public static MLocation Get(Ctx ctx, int C_Location_ID, Trx trxName)
        {
            //	New
            if (C_Location_ID == 0)
                return new MLocation(ctx, C_Location_ID, trxName);
            //
            int key = (int)C_Location_ID;
            MLocation retValue = (MLocation)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MLocation(ctx, C_Location_ID, trxName);
            if (retValue.Get_ID() != 0)		//	found
            {
                s_cache.Add(key, retValue);
                return retValue;
            }
            return null;					//	not found
        }

        // VIS0008 function to get location from cache based on datarow
        /// <summary>
        /// Get Location from Cache
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        /// <returns></returns>
        public static MLocation Get(Ctx ctx, DataRow dr, Trx trxName)
        {
            //	New
            int key = Util.GetValueOfInt(dr["C_Location_ID"]);
            if (key == 0)
                return new MLocation(ctx, dr, trxName);
            //
            //int key = (int)C_Location_ID;
            MLocation retValue = (MLocation)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MLocation(ctx, dr, trxName);
            if (retValue.Get_ID() != 0)		//	found
            {
                s_cache.Add(key, retValue);
                return retValue;
            }
            return null;					//	not found
        }

        /// <summary>
        /// Load Location with ID if Business Partner Location
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_BPartner_Location_ID">Business Partner Location</param>
        /// <param name="trxName">transaction</param>
        /// <returns>location or null</returns>
        public static MLocation GetBPLocation(Ctx ctx, int C_BPartner_Location_ID, Trx trxName)
        {
            if (C_BPartner_Location_ID == 0)					//	load default
                return null;

            MLocation loc = null;
            String sql = "SELECT * FROM C_Location l "
                + "WHERE C_Location_ID IN (SELECT C_Location_ID FROM C_BPartner_Location WHERE C_BPartner_Location_ID=" + C_BPartner_Location_ID + ")";
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    loc = new MLocation(ctx, dr, trxName);
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql + " - " + C_BPartner_Location_ID, e);
                loc = null;
            }
            return loc;
        }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Location_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MLocation(Ctx ctx, int C_Location_ID, Trx trxName)
            : base(ctx, C_Location_ID, trxName)
        {
            if (C_Location_ID == 0)
            {
                MCountry defaultCountry = MCountry.GetDefault(GetCtx());
                SetCountry(defaultCountry);
                MRegion defaultRegion = MRegion.GetDefault(GetCtx());
                if (defaultRegion != null
                    && defaultRegion.GetC_Country_ID() == defaultCountry.GetC_Country_ID())
                    SetRegion(defaultRegion);
            }
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="country">mandatory country</param>
        /// <param name="region">optional region</param>
        public MLocation(MCountry country, MRegion region)
            : base(country.GetCtx(), 0, country.Get_TrxName())
        {
            SetCountry(country);
            SetRegion(region);
        }

        /// <summary>
        /// Full Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Country_ID">country</param>
        /// <param name="C_Region_ID">region</param>
        /// <param name="city">city</param>
        /// <param name="trxName">transaction</param>
        public MLocation(Ctx ctx, int C_Country_ID, int C_Region_ID, String city, Trx trxName)
            : this(ctx, 0, trxName)
        {
            if (C_Country_ID != 0)
                SetC_Country_ID(C_Country_ID);
            if (C_Region_ID != 0)
                SetC_Region_ID(C_Region_ID);
            SetCity(city);
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transaction</param>
        public MLocation(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Set Country
        /// </summary>
        /// <param name="country">country</param>
        public void SetCountry(MCountry country)
        {
            if (country != null)
            {
                _country = country;
            }
            else
            {
                _country = MCountry.GetDefault(GetCtx());
            }
            base.SetC_Country_ID(_country.GetC_Country_ID());
        }

        /// <summary>
        /// Set C_Country_ID
        /// </summary>
        /// <param name="C_Country_ID"></param>
        public new void SetC_Country_ID(int C_Country_ID)
        {
            if (GetC_Country_ID() != C_Country_ID)
            {
                SetRegion(null);
            }
            SetCountry(MCountry.Get(GetCtx(), C_Country_ID));
        }

        /// <summary>
        /// Get Country
        /// </summary>
        public MCountry GetCountry()
        {
            if (_country == null)
            {
                if (GetC_Country_ID() != 0)
                    _country = MCountry.Get(GetCtx(), GetC_Country_ID());
                else
                    _country = MCountry.GetDefault(GetCtx());
            }
            return _country;
        }

        /**
         * 	Get Country Name
         *	@return	Country Name
         */
        public String GetCountryName()
        {
            return GetCountry().GetName();
        }

        /**
         * 	Get Country Line
         * 	@param local if true only foreign country is returned
         * 	@return country or null
         */
        public String GetCountry(bool local)
        {
            if (local && GetC_Country_ID() == MCountry.GetDefault(GetCtx()).GetC_Country_ID())
                return null;
            return GetCountryName();
        }

        /**
         * 	Set Region
         *	@param region
         */
        public void SetRegion(MRegion region)
        {
            _region = region;
            if (region == null)
                base.SetC_Region_ID(0);
            else
            {
                base.SetC_Region_ID(_region.GetC_Region_ID());
                if (_region.GetC_Country_ID() != GetC_Country_ID())
                {
                    log.Info("Region(" + region + ") C_Country_ID=" + region.GetC_Country_ID()
                  + " - From  C_Country_ID=" + GetC_Country_ID());
                    SetC_Country_ID(region.GetC_Country_ID());
                }
            }
        }

        /**
         * 	Set C_Region_ID
         *	@param C_Region_ID region
         */
        public new void SetC_Region_ID(int C_Region_ID)
        {
            if (C_Region_ID == 0)
                SetRegion(null);
            //	Country defined
            else if (GetC_Country_ID() != 0)
            {
                MCountry cc = GetCountry();
                if (cc.IsValidRegion(C_Region_ID))
                    base.SetC_Region_ID(C_Region_ID);
                else
                    SetRegion(null);
            }
            else
                SetRegion(MRegion.Get(GetCtx(), C_Region_ID));
        }

        /**
         * 	Get Region
         *	@return region
         */
        public MRegion GetRegion()
        {
            if (_region == null && GetC_Region_ID() != 0)
                _region = MRegion.Get(GetCtx(), GetC_Region_ID());
            return _region;
        }

        /**
         * 	Get (local) Region Name
         *	@return	region Name or ""
         */
        public new String GetRegionName()
        {
            return GetRegionName(false);
        }

        /**
         * 	Get Region Name
         * 	@param getFromRegion get from region (not locally)
         *	@return	region Name or ""
         */
        public String GetRegionName(bool getFromRegion)
        {
            if (getFromRegion && GetCountry().IsHasRegion()
                && GetRegion() != null)
            {
                base.SetRegionName("");	//	avoid duplicates
                return GetRegion().GetName();
            }
            //
            String regionName = base.GetRegionName();
            if (regionName == null)
                regionName = "";
            return regionName;
        }

        /**
         * 	Compares to current record
         *	@param C_Country_ID if 0 ignored
         *	@param C_Region_ID if 0 ignored
         *	@param Postal match postal
         *	@param Postal_Add match postal add
         *	@param City match city
         *	@param Address1 match address 1
         *	@param Address2 match addtess 2
         *	@return true if equals
         */
        public bool Equals(int C_Country_ID, int C_Region_ID,
            String Postal, String Postal_Add, String City, String Address1, String Address2)
        {
            if (C_Country_ID != 0 && GetC_Country_ID() != C_Country_ID)
                return false;
            if (C_Region_ID != 0 && GetC_Region_ID() != C_Region_ID)
                return false;
            //	must match
            if (!EqualsNull(Postal, GetPostal()))
                return false;
            if (!EqualsNull(Postal_Add, GetPostal_Add()))
                return false;
            if (!EqualsNull(City, GetCity()))
                return false;
            if (!EqualsNull(Address1, GetAddress1()))
                return false;
            if (!EqualsNull(Address2, GetAddress2()))
                return false;
            return true;
        }

        /// <summary>
        /// Equals if "" or Null
        /// </summary>
        /// <param name="c1">c1</param>
        /// <param name="c2">c2</param>
        /// <returns>true if equal (ignore case)</returns>
        private bool EqualsNull(String c1, String c2)
        {
            if (c1 == null)
            {
                c1 = "";
            }
            if (c2 == null)
            {
                c2 = "";
            }
            return c1.Equals(c2);
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="cmp">comperator</param>
        /// <returns></returns>
        public override bool Equals(Object cmp)
        {
            MessageBox.Show("Note:--Check interface value for \n 'IEquatable<Object> //Comparator<PO> interface'");
            if (cmp == null)
            {
                return false;
            }
            //if (cmp.getClass().equals(this.getClass()))
            if (cmp.GetType().Equals(this.GetType()))
            {
                return ((PO)cmp).Get_ID() == Get_ID();
            }
            return Equals(cmp);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        /**
         * 	Print Address Reverse Order
         *	@return true if reverse depending on country
         */
        public bool IsAddressLinesReverse()
        {
            //	Local
            if (GetC_Country_ID() == MCountry.GetDefault(GetCtx()).GetC_Country_ID())
                return GetCountry().IsAddressLinesLocalReverse();
            return GetCountry().IsAddressLinesReverse();
        }

        /**
         * 	Get formatted City Region Postal line
         * 	@return City, Region Postal
         */
        public String GetCityRegionPostal()
        {
            return ParseCRP(GetCountry());
        }

        /**
         *	Parse according Ctiy/Postal/Region according to displaySequence.
         *	@C@ - City		@R@ - Region	@P@ - Postal  @A@ - PostalAdd
         *  @param c country
         *  @return parsed String
         */
        private String ParseCRP(MCountry c)
        {
            if (c == null)
            {
                return "CountryNotFound";
            }
            bool local = GetC_Country_ID() == MCountry.GetDefault(GetCtx()).GetC_Country_ID();
            String inStr = local ? c.GetDisplaySequenceLocal() : c.GetDisplaySequence();
            StringBuilder outStr = new StringBuilder();

            String token;
            int i = inStr.IndexOf("@");
            while (i != -1)
            {
                outStr.Append(inStr.Substring(0, i));			// up to @
                inStr = inStr.Substring(i + 1, inStr.Length - (i + 1));	// from first @

                int j = inStr.IndexOf("@");						// next @
                if (j < 0)
                {
                    token = "";									//	no second tag
                    j = i + 1;
                }
                else
                    token = inStr.Substring(0, j);
                //	Tokens
                if (token.Equals("C"))
                {
                    if (GetCity() != null)
                        outStr.Append(GetCity());
                }
                else if (token.Equals("R"))
                {
                    if (GetRegion() != null)					//	we have a region
                        outStr.Append(GetRegion().GetName());
                    else if (base.GetRegionName() != null && base.GetRegionName().Length > 0)
                        outStr.Append(base.GetRegionName());	//	local region name
                }
                else if (token.Equals("P"))
                {
                    if (GetPostal() != null)
                        outStr.Append(GetPostal());
                }
                else if (token.Equals("A"))
                {
                    String add = GetPostal_Add();
                    if (add != null && add.Length > 0)
                        outStr.Append("-").Append(add);
                }
                else
                    outStr.Append("@").Append(token).Append("@");

                inStr = inStr.Substring(j + 1, inStr.Length - (j + 1));	// from second @
                i = inStr.IndexOf("@");
            }
            outStr.Append(inStr);						// add the rest of the string

            //	Print Region Name if entered and not part of pattern
            if (c.GetDisplaySequence().IndexOf("@R@") == -1
                && base.GetRegionName() != null && base.GetRegionName().Length > 0)
                outStr.Append(" ").Append(base.GetRegionName());

            String retValue = Utility.Util.Replace(outStr.ToString(), "\\n", "\n");
            log.Finest("parseCRP - " + c.GetDisplaySequence() + " -> " + retValue);
            return retValue;
        }

        /// <summary>
        /// Return printable String representation
        /// </summary>
        /// <returns>String</returns>
        public override String ToString()
        {
            StringBuilder retStr = new StringBuilder();
            if (IsAddressLinesReverse())
            {
                //	City, Region, Postal
                retStr.Append(ParseCRP(GetCountry()));
                if (GetAddress4() != null && GetAddress4().Length > 0)
                {
                    retStr.Append(", ").Append(GetAddress4());
                }
                if (GetAddress3() != null && GetAddress3().Length > 0)
                {
                    retStr.Append(", ").Append(GetAddress3());
                }
                if (GetAddress2() != null && GetAddress2().Length > 0)
                {
                    retStr.Append(", ").Append(GetAddress2());
                }
                if (GetAddress1() != null && GetAddress1().Length > 0)
                {
                    retStr.Append(", ").Append(GetAddress1());
                }
            }
            else
            {
                if (GetAddress1() != null && GetAddress1().Trim().Length > 0)
                    retStr.Append(GetAddress1()).Append(", ");
                if (GetAddress2() != null && GetAddress2().Trim().Length > 0)
                    retStr.Append(GetAddress2()).Append(", ");
                if (GetAddress3() != null && GetAddress3().Trim().Length > 0)
                    retStr.Append(GetAddress3()).Append(", ");
                if (GetAddress4() != null && GetAddress4().Trim().Length > 0)
                    retStr.Append(GetAddress4()).Append(", ");
                //	City, Region, Postal
                if (ParseCRP(GetCountry()).Trim() != ",")
                {                    
                        retStr.Append(ParseCRP(GetCountry()));
                }
                //	Add Country would come here
            }
            return retStr.ToString().Replace(", ,", ",");
        }

        /**
         *	Return String representation with CR at line end
         *  @return String
         */
        public String ToStringCR()
        {
            StringBuilder retStr = new StringBuilder();
            if (IsAddressLinesReverse())
            {
                //	City, Region, Postal
                retStr.Append(ParseCRP(GetCountry()));
                if (GetAddress4() != null && GetAddress4().Length > 0)
                    retStr.Append("\n").Append(GetAddress4());
                if (GetAddress3() != null && GetAddress3().Length > 0)
                    retStr.Append("\n").Append(GetAddress3());
                if (GetAddress2() != null && GetAddress2().Length > 0)
                    retStr.Append("\n").Append(GetAddress2());
                if (GetAddress1() != null)
                    retStr.Append("\n").Append(GetAddress1());
            }
            else
            {
                if (GetAddress1() != null)
                    retStr.Append(GetAddress1());
                if (GetAddress2() != null && GetAddress2().Length > 0)
                    retStr.Append("\n").Append(GetAddress2());
                if (GetAddress3() != null && GetAddress3().Length > 0)
                    retStr.Append("\n").Append(GetAddress3());
                if (GetAddress4() != null && GetAddress4().Length > 0)
                    retStr.Append("\n").Append(GetAddress4());
                //	City, Region, Postal
                retStr.Append("\n").Append(ParseCRP(GetCountry()));
                //	Add Country would come here
            }
            return retStr.ToString();
        }

        /**
         *	Return detailed String representation
         *  @return String
         */
        public String ToStringX()
        {
            StringBuilder sb = new StringBuilder("MLocation=[");
            sb.Append(Get_ID())
                .Append(",C_Country_ID=").Append(GetC_Country_ID())
                .Append(",C_Region_ID=").Append(GetC_Region_ID())
                .Append(",Postal=").Append(GetPostal())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetAD_Org_ID() != 0)
                SetAD_Org_ID(0);
            //	Region Check
            if (GetC_Region_ID() != 0)
            {
                if (_country == null || _country.GetC_Country_ID() != GetC_Country_ID())
                    GetCountry();
                if (!_country.IsHasRegion())
                    SetC_Region_ID(0);
            }

            var LngLat = GetLongitudeAndLatitude(ToString(), "false");
            SetLongitude(LngLat[1]);
            SetLatitude(LngLat[0]);

            return true;
        }

        public string[] GetLongitudeAndLatitude(string address, string sensor)
        {
            string urlAddress = "http://maps.googleapis.com/maps/api/geocode/xml?address=" + HttpUtility.UrlEncode(address) + "&sensor=" + sensor;
            string[] returnValue = new string[2];
            try
            {
                XmlDocument objXmlDocument = new XmlDocument();
                objXmlDocument.Load(urlAddress);
                XmlNodeList objXmlNodeList = objXmlDocument.SelectNodes("/GeocodeResponse/result/geometry/location");
                foreach (XmlNode objXmlNode in objXmlNodeList)
                {
                    // GET LATITUDE 
                    returnValue[0] = objXmlNode.ChildNodes.Item(0).InnerText;

                    // GET Longitude 
                    returnValue[1] = objXmlNode.ChildNodes.Item(1).InnerText;
                }
            }
            catch
            {
                // Process an error action here if needed  
            }
            return returnValue;
        }

        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return success
         */
        protected override bool AfterSave(bool newRecord, bool success)
        {
            //	Value/Name change in Account
            if (!newRecord
                && ("Y".Equals(GetCtx().GetContext("$Element_LF"))
                    || "Y".Equals(GetCtx().GetContext("$Element_LT")))
                && (Is_ValueChanged("Postal") || Is_ValueChanged("City"))
                )
            {
                MAccount.UpdateValueDescription(GetCtx(),
                    "(C_LocFrom_ID=" + GetC_Location_ID()
                    + " OR C_LocTo_ID=" + GetC_Location_ID() + ")", Get_TrxName());
            }
            return success;
        }



    }
}
