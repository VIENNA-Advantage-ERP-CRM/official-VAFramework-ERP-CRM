using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class LocationModel
    {
        public int VAB_ADDRESS_ID { get; set; }
        public int VAB_City_ID { get; set; }
        public int VAB_Country_ID { get; set; }
        public int VAB_RegionState_ID { get; set; }

        [Display(Name = "Search")]
        public List<SearchAddress> Addresslist { get; set; }
        public Dictionary<int, string> countryList { get; set; }

        [Display(Name = "test")]
        public string Address1 { get; set; }
        [Display(Name = "test")]
        public string Address2 { get; set; }
        [Display(Name = "test")]
        public string Address3 { get; set; }
        [Display(Name = "test")]
        public string Address4 { get; set; }
        [Display(Name = "test")]
        public string Country { get; set; }
        [Display(Name = "test")]
        public string City { get; set; }
        [Display(Name = "test")]
        public string RegionName { get; set; }
        [Display(Name = "test")]
        public string Postal_Add { get; set; }
        [Display(Name = "test")]
        public string Postal { get; set; }
        [Display(Name = "test")]
        public string ZipCode { get; set; }

        /// <summary>
        /// addres get from database
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public LocationModel GetAddressfromDataBase(string locationId,Ctx ctx)
        {
            LocationModel obj = new LocationModel();            
            string sql="";

                // Check applied by mohit - asked by mukesh sir - to check if login langauge is base language - then pick non translated data.
            if (Env.IsBaseLanguage(ctx, ""))
            {
                sql = "SELECT L.ADDRESS1,L.ADDRESS2,L.ADDRESS3,L.ADDRESS4,L.CITY,L.REGIONNAME ,L.POSTAL,L.POSTAL_ADD,CN.NAME AS COUNTRY,L.VAF_CLIENT_ID,L.VAF_ORG_ID,L.VAB_CITY_ID," +
                           " L.VAB_COUNTRY_ID,L.VAB_ADDRESS_ID,L.VAB_REGIONSTATE_ID FROM VAB_ADDRESS L" +
                           " LEFT JOIN VAB_COUNTRY CN ON CN.VAB_COUNTRY_ID=L.VAB_COUNTRY_ID WHERE L.IsActive='Y'";
            }
            else
            {
                // Check applied by mohit - Picked data from translation tab - if base language
                sql = "SELECT L.ADDRESS1,L.ADDRESS2,L.ADDRESS3,L.ADDRESS4,L.CITY,L.REGIONNAME ,L.POSTAL,L.POSTAL_ADD,CNTRL.NAME AS COUNTRY,L.VAF_CLIENT_ID,L.VAF_ORG_ID,L.VAB_CITY_ID," +
                           " L.VAB_COUNTRY_ID,L.VAB_ADDRESS_ID,L.VAB_REGIONSTATE_ID FROM VAB_ADDRESS L" +
                           " LEFT JOIN VAB_COUNTRY CN ON CN.VAB_COUNTRY_ID=L.VAB_COUNTRY_ID INNER JOIN VAB_Country_TL CNTRL ON (CN.VAB_COUNTRY_ID  =CNTRL.VAB_COUNTRY_ID)" +
                           " WHERE L.IsActive='Y' AND CNTRL.VAF_Language='" + ctx.GetVAF_Language() + "' ";
            }

            sql += " AND L.VAB_Address_id=" + locationId;

            var ds = new DataSet();
            try
            {
                ds = DB.ExecuteDataset(sql);

                if (ds != null)
                {
                    //DataRow[] dr = ds.Tables[0].Select("VAB_Address_id=" + locationId);
                    DataRowCollection dr = ds.Tables[0].Rows;
                    //if contain saved records
                    if (dr.Count > 0)
                    {
                        if (dr[0].ItemArray.Length > 0)
                        {
                            obj.Address1 = Convert.ToString(dr[0]["Address1"] == DBNull.Value ? "" : dr[0]["Address1"]);
                            obj.Address2 = Convert.ToString(dr[0]["Address2"] == DBNull.Value ? "" : dr[0]["Address2"]);
                            obj.Address3 = Convert.ToString(dr[0]["Address3"] == DBNull.Value ? "" : dr[0]["Address3"]);
                            obj.Address4 = Convert.ToString(dr[0]["Address4"] == DBNull.Value ? "" : dr[0]["Address4"]);

                            obj.VAB_City_ID = Convert.ToInt32(dr[0]["VAB_City_ID"] == DBNull.Value ? 0 : dr[0]["VAB_City_ID"]);
                            obj.VAB_Country_ID = Convert.ToInt32(dr[0]["VAB_Country_ID"] == DBNull.Value ? 0 : dr[0]["VAB_Country_ID"]);
                            obj.VAB_RegionState_ID = Convert.ToInt32(dr[0]["VAB_RegionState_ID"] == DBNull.Value ? 0 : dr[0]["VAB_RegionState_ID"]);

                            obj.City = Convert.ToString(dr[0]["City"] == DBNull.Value ? "" : dr[0]["City"]);
                            obj.RegionName = Convert.ToString(dr[0]["RegionName"] == DBNull.Value ? "" : dr[0]["RegionName"]);
                            obj.Country = Convert.ToString(dr[0]["Country"] == DBNull.Value ? "" : dr[0]["Country"]);

                            obj.ZipCode = Convert.ToString(dr[0]["POSTAL"] == DBNull.Value ? "" : dr[0]["POSTAL"]);
                        }
                    }

                    ////Auto fill search control
                    //obj.Addresslist = new List<SearchAddress>();
                    //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    //{
                    //    SearchAddress searchObj = new SearchAddress();
                    //    if (i == 0)
                    //    {
                    //        obj.Addresslist.Add(searchObj);
                    //        searchObj = new SearchAddress();
                    //    }
                    //    searchObj.Name = Convert.ToString(ds.Tables[0].Rows[i]["Country"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["Country"]) +
                    //        "," + Convert.ToString(ds.Tables[0].Rows[i]["Address1"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["Address1"])
                    //        + "," + Convert.ToString(ds.Tables[0].Rows[i]["Address2"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["Address2"])
                    //        + "," + Convert.ToString(ds.Tables[0].Rows[i]["Address3"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["Address3"])
                    //        + "," + Convert.ToString(ds.Tables[0].Rows[i]["Address4"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["Address4"])
                    //        + "," + Convert.ToString(ds.Tables[0].Rows[i]["City"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["City"])
                    //        + "," + Convert.ToString(ds.Tables[0].Rows[i]["RegionName"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["RegionName"])
                    //        + "," + Convert.ToString(ds.Tables[0].Rows[i]["POSTAL"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["POSTAL"]);

                    //    searchObj.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAB_Address_id"] == DBNull.Value ? 0 : ds.Tables[0].Rows[i]["VAB_Address_id"]);
                    //    searchObj.CityId = Convert.ToInt32(ds.Tables[0].Rows[i]["VAB_City_ID"] == DBNull.Value ? 0 : ds.Tables[0].Rows[i]["VAB_City_ID"]);
                    //    searchObj.StateId = Convert.ToInt32(ds.Tables[0].Rows[i]["VAB_RegionState_ID"] == DBNull.Value ? 0 : ds.Tables[0].Rows[i]["VAB_RegionState_ID"]);
                    //    searchObj.CountryId = Convert.ToInt32(ds.Tables[0].Rows[i]["VAB_Country_ID"] == DBNull.Value ? 0 : ds.Tables[0].Rows[i]["VAB_Country_ID"]);

                    //    obj.Addresslist.Add(searchObj);
                    //}
                }
            }
            catch (Exception ev)
            {

            }
            ds = null;
            return obj;
        }

        /// <summary>
        /// save location
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="pref"></param>
        /// <returns></returns>
        public MVABAddress LocationSave(Ctx ctx, Dictionary<string, object> pref)
        {
            MVABAddress _location = null;

            var VAB_Address_Id = Convert.ToInt32(pref["clocationId"]);

            string sql = "SELECT COUNT(*) FROM VAB_Address WHERE VAB_Address_ID=" + VAB_Address_Id;
            object count = DB.ExecuteScalar(sql);
            if (count == null || count == DBNull.Value || Util.GetValueOfInt(count) == 0)
            {
                VAB_Address_Id = 0;
            }

            _location = new MVABAddress(ctx, VAB_Address_Id, null);
            _location.SetAddress1(Convert.ToString(pref["addvalue1"]));
            _location.SetAddress2(Convert.ToString(pref["addvalue2"]));
            _location.SetAddress3(Convert.ToString(pref["addvalue3"]));
            _location.SetAddress4(Convert.ToString(pref["addvalue4"]));
            _location.SetCity(Convert.ToString(pref["cityValue"]));
            _location.SetPostal(Convert.ToString(pref["zipValue"]));
            //  Country/Region
            _location.SetVAB_Country_ID(Convert.ToInt32(pref["countryId"]));

            if (_location.GetCountry().IsHasRegion())
            {
                _location.SetVAB_RegionState_ID(Convert.ToInt32(pref["stateId"]));
            }
            else
            {
                _location.SetVAB_RegionState_ID(0);
            }
            _location.SetRegionName(Convert.ToString(pref["stateValue"]));
            _location.SetVAB_City_ID(Convert.ToInt32(pref["cityId"]));

            _location.Save();
            return _location;
        }

        /// <summary>
        /// Country search
        /// </summary>
        /// <param name="name_startsWith"></param>
        /// <returns></returns>
        public List<KeyNamePair> GetCountryByText(string name_startsWith,Ctx ctx)
        {
            List<KeyNamePair> obj = new List<KeyNamePair>();
            string sqlquery = "";
            // Check applied by mohit - asked by mukesh sir - to check if login langauge is base language - then pick non translated data.
            if (Env.IsBaseLanguage(ctx, ""))
            {
                sqlquery = " select VAB_COUNTRY_ID,Name from c_country where IsActive='Y' AND LOWER(name) like LOWER('" + name_startsWith + "%')";
            }
            else
            {
                // Check applied by mohit - Picked data from translation tab - if base language
                sqlquery = " SELECT cn.VAB_COUNTRY_ID,CNTRL.Name FROM c_country cn INNER JOIN VAB_Country_TL CNTRL ON (cn.VAB_Country_ID=CNTRL.VAB_Country_ID) " +
                    " WHERE cn.IsActive='Y' AND LOWER(CNTRL.name) like LOWER('" + name_startsWith + "%') AND CNTRL.VAF_Language='" + ctx.GetVAF_Language() + "'";
            }
            var ds = new DataSet();
            ds = DB.ExecuteDataset(sqlquery);
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj.Add(new KeyNamePair(Convert.ToInt32(ds.Tables[0].Rows[i]["VAB_COUNTRY_ID"]), Convert.ToString(ds.Tables[0].Rows[i]["Name"])));
                }
            }
            ds = null;
            return obj;
        }

        /// <summary>
        /// State search
        /// </summary>
        /// <param name="name_startsWith"></param>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public List<KeyNamePair> GetStatesByText(string name_startsWith, string countryId)
        {
            List<KeyNamePair> obj = new List<KeyNamePair>();
            string sqlquery = " select VAB_REGIONSTATE_id,Name from VAB_REGIONSTATE where IsActive='Y' AND VAB_COUNTRY_ID=" + countryId + " and LOWER(name) like LOWER('" + name_startsWith + "%')";
            var ds = new DataSet();
            ds = DB.ExecuteDataset(sqlquery);
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj.Add(new KeyNamePair(Convert.ToInt32(ds.Tables[0].Rows[i]["VAB_REGIONSTATE_id"]), Convert.ToString(ds.Tables[0].Rows[i]["Name"])));
                }
            }
            ds = null;
            return obj;
        }

        /// <summary>
        /// Addres search
        /// </summary>
        /// <param name="name_startsWith"></param>
        /// <returns></returns>
        public List<LocationAddress> GetAddressesSearch(Ctx ctx, string name_startsWith)
        {
            List<LocationAddress> obj = new List<LocationAddress>();            
            string sqlquery = "";
            #region Commentedny Mohit
            //if (Env.IsBaseLanguage(ctx, ""))
            //{
            //    sqlquery = " SELECT * FROM (SELECT (NVL(cn.Name,'')||' '|| NVL(l.ADDRESS1,'') ||' '|| NVL(l.ADDRESS2,'') ||' '|| NVL(l.ADDRESS3,'') ||' '|| NVL(l.ADDRESS4,'') ||' '|| NVL(l.CITY,'') ||' '|| NVL(l.REGIONNAME,'') ||' '|| NVL(l.POSTAL,'') ||' '|| NVL(l.POSTAL_ADD,'')) as address," +
            //                        "cn.Name, l.ADDRESS1 , l.ADDRESS2 , l.ADDRESS3 , l.ADDRESS4 , l.CITY , l.REGIONNAME , l.POSTAL , l.POSTAL_ADD," +
            //                        " l.VAF_CLIENT_ID,l.VAF_ORG_ID,l.VAB_CITY_ID,l.VAB_COUNTRY_ID,l.VAB_ADDRESS_ID,l.VAB_REGIONSTATE_ID FROM VAB_Address l" +
            //                        " LEFT JOIN VAB_Country cn on cn.VAB_COUNTRY_ID=l.VAB_COUNTRY_ID WHERE l.ISACTIVE='Y') qb1";
            //}
            //else
            //{
            //    // Check applied by mohit - Picked data from translation tab - if base language
            //    sqlquery = " SELECT * FROM (SELECT (NVL(CNTRL.Name,'')||' '|| NVL(l.ADDRESS1,'') ||' '|| NVL(l.ADDRESS2,'') ||' '|| NVL(l.ADDRESS3,'') ||' '|| NVL(l.ADDRESS4,'') ||' '|| NVL(l.CITY,'') ||' '|| NVL(l.REGIONNAME,'') ||' '|| NVL(l.POSTAL,'') ||' '|| NVL(l.POSTAL_ADD,'')) as address," +
            //                        " cnTRL.Name, l.ADDRESS1 , l.ADDRESS2 , l.ADDRESS3 , l.ADDRESS4 , l.CITY , l.REGIONNAME , l.POSTAL , l.POSTAL_ADD," +
            //                        " l.VAF_CLIENT_ID,l.VAF_ORG_ID,l.VAB_CITY_ID,l.VAB_COUNTRY_ID,l.VAB_ADDRESS_ID,l.VAB_REGIONSTATE_ID FROM VAB_Address l" +
            //                        " LEFT JOIN VAB_Country cn on cn.VAB_COUNTRY_ID=l.VAB_COUNTRY_ID INNER JOIN VAB_Country_TL CNTRL     ON (cn.VAB_Country_ID=CNTRL.VAB_Country_ID) WHERE l.ISACTIVE='Y' AND cnTRL.VAF_Language='" + ctx.GetVAF_Language() + "') qb1";
            //}
            #endregion
            // Check applied by mohit - asked by mukesh sir - to check if login langauge is base language - then pick non translated data.
            if (Env.IsBaseLanguage(ctx, ""))
            {
                sqlquery = " SELECT (NVL(cn.Name,'')||' '|| NVL(VAB_Address.ADDRESS1,'') ||' '|| NVL(VAB_Address.ADDRESS2,'') ||' '|| NVL(VAB_Address.ADDRESS3,'') ||' '|| NVL(VAB_Address.ADDRESS4,'') ||' '|| NVL(VAB_Address.CITY,'') ||' '|| NVL(VAB_Address.REGIONNAME,'') ||' '|| NVL(VAB_Address.POSTAL,'') ||' '|| NVL(VAB_Address.POSTAL_ADD,'')) as address," +
                                    "cn.Name, VAB_Address.ADDRESS1 , VAB_Address.ADDRESS2 , VAB_Address.ADDRESS3 , VAB_Address.ADDRESS4 , VAB_Address.CITY , VAB_Address.REGIONNAME , VAB_Address.POSTAL , VAB_Address.POSTAL_ADD," +
                                    " VAB_Address.VAF_CLIENT_ID,VAB_Address.VAF_ORG_ID,VAB_Address.VAB_CITY_ID,VAB_Address.VAB_COUNTRY_ID,VAB_Address.VAB_ADDRESS_ID,VAB_Address.VAB_REGIONSTATE_ID FROM VAB_Address VAB_Address" +
                                    " LEFT JOIN VAB_Country cn on cn.VAB_COUNTRY_ID=VAB_Address.VAB_COUNTRY_ID WHERE VAB_Address.ISACTIVE='Y'" +
                                    " AND Lower( (NVL(cn.Name,'') ||' '  || NVL(VAB_Address.ADDRESS1,'')  ||' ' || NVL(VAB_Address.ADDRESS2,'') ||' ' || NVL(VAB_Address.ADDRESS3,'')  ||' ' || NVL(VAB_Address.ADDRESS4,'') ||' ' || NVL(VAB_Address.CITY,'')" +
                                   " ||' '  || NVL(VAB_Address.REGIONNAME,'') ||' ' || NVL(VAB_Address.POSTAL,'')  ||' '   || NVL(VAB_Address.POSTAL_ADD,''))) like LOWER ('%" + name_startsWith + "%') AND rownum < 500";
            }
            else
            {
                // Check applied by mohit - Picked data from translation tab - if base language
                sqlquery = " SELECT (NVL(CNTRL.Name,'')||' '|| NVL(VAB_Address.ADDRESS1,'') ||' '|| NVL(VAB_Address.ADDRESS2,'') ||' '|| NVL(VAB_Address.ADDRESS3,'') ||' '|| NVL(VAB_Address.ADDRESS4,'') ||' '|| NVL(VAB_Address.CITY,'') ||' '|| NVL(VAB_Address.REGIONNAME,'') ||' '|| NVL(VAB_Address.POSTAL,'') ||' '|| NVL(VAB_Address.POSTAL_ADD,'')) as address," +
                                    " cnTRL.Name, VAB_Address.ADDRESS1 , VAB_Address.ADDRESS2 , VAB_Address.ADDRESS3 , VAB_Address.ADDRESS4 , VAB_Address.CITY , VAB_Address.REGIONNAME , VAB_Address.POSTAL , VAB_Address.POSTAL_ADD," +
                                    " VAB_Address.VAF_CLIENT_ID,VAB_Address.VAF_ORG_ID,VAB_Address.VAB_CITY_ID,VAB_Address.VAB_COUNTRY_ID,VAB_Address.VAB_ADDRESS_ID,VAB_Address.VAB_REGIONSTATE_ID FROM VAB_Address VAB_Address" +
                                    " LEFT JOIN VAB_Country cn on cn.VAB_COUNTRY_ID=VAB_Address.VAB_COUNTRY_ID INNER JOIN VAB_Country_TL CNTRL     ON (cn.VAB_Country_ID=CNTRL.VAB_Country_ID) WHERE VAB_Address.ISACTIVE='Y' AND cnTRL.VAF_Language='" + ctx.GetVAF_Language() + "'" +
                                    " AND Lower((NVL(CNTRL.Name,'')   ||' '  || NVL(VAB_Address.ADDRESS1,'')  ||' '  || NVL(VAB_Address.ADDRESS2,'')  ||' '  || NVL(VAB_Address.ADDRESS3,'')  ||' '  || NVL(VAB_Address.ADDRESS4,'')  ||' ' " +
                                    " || NVL(VAB_Address.CITY,'')  ||' '  || NVL(VAB_Address.REGIONNAME,'')  ||' '  || NVL(VAB_Address.POSTAL,'')  ||' '  || NVL(VAB_Address.POSTAL_ADD,''))) like Lower('%" + name_startsWith + "%')  AND rownum <500";
            }
            sqlquery = MVAFRole.GetDefault(ctx).AddAccessSQL(sqlquery, "VAB_Address", MVAFRole.SQL_FULLYQUALIFIED, MVAFRole.SQL_RO);
            //sqlquery = "SELECT * FROM (" + sqlquery + " ) fltr WHERE LOWER(fltr.address) LIKE LOWER('" + name_startsWith + "%') or LOWER(fltr.address) LIKE LOWER('%" + name_startsWith + "%') or LOWER(fltr.address) LIKE LOWER('%" + name_startsWith + "')";



            var ds = new DataSet();
            ds = DB.ExecuteDataset(sqlquery);
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    LocationAddress ladd = new LocationAddress
                    {
                        ADDRESS = Convert.ToString(ds.Tables[0].Rows[i]["address"]),
                        COUNTRYNAME = Convert.ToString(ds.Tables[0].Rows[i]["Name"]),
                        ADDRESS1 = Convert.ToString(ds.Tables[0].Rows[i]["ADDRESS1"]),
                        ADDRESS2 = Convert.ToString(ds.Tables[0].Rows[i]["ADDRESS2"]),
                        ADDRESS3 = Convert.ToString(ds.Tables[0].Rows[i]["ADDRESS3"]),
                        ADDRESS4 = Convert.ToString(ds.Tables[0].Rows[i]["ADDRESS4"]),

                        CITYNAME = Convert.ToString(ds.Tables[0].Rows[i]["CITY"]),
                        STATENAME = Convert.ToString(ds.Tables[0].Rows[i]["REGIONNAME"]),
                        ZIPCODE = Convert.ToString(ds.Tables[0].Rows[i]["POSTAL"]),

                        VAF_CLIENT_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_CLIENT_ID"]),
                        VAF_ORG_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_ORG_ID"]),
                        // VAB_CITY_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAB_CITY_ID"]),
                        VAB_COUNTRY_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAB_COUNTRY_ID"] == DBNull.Value ? null : ds.Tables[0].Rows[i]["VAB_COUNTRY_ID"]),
                        VAB_ADDRESS_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAB_ADDRESS_ID"]),
                        VAB_REGIONSTATE_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAB_REGIONSTATE_ID"] == DBNull.Value ? null : ds.Tables[0].Rows[i]["VAB_REGIONSTATE_ID"])
                    };
                    obj.Add(ladd);
                }
            }
            ds = null;
            return obj;
        }

        /// <summary>
        /// Get Location List
        /// </summary>
        /// <param name="ctx">client context</param>
        /// <returns>keyname pair dictionary</returns>
        internal static LocationData GetAllLocations(VAdvantage.Utility.Ctx ctx)
        {
            IDataReader dr = null;
            LocationData locData = new LocationData();


            int MAX_ROWS = 10000;
            int count = 0;
            Dictionary<int, KeyNamePair> locs = new Dictionary<int, KeyNamePair>();
            Dictionary<int, LatLng> lstLatLng = new Dictionary<int, LatLng>();
            try
            {
                dr = VAdvantage.DataBase.DB.ExecuteReader("SELECT * FROM VAB_Address");
                while (dr.Read())
                {
                    if (count > MAX_ROWS)
                        break;
                    MVABAddress loc = MVABAddress.Get(ctx, Convert.ToInt32(dr["VAB_Address_ID"]), null);
                    locs[loc.Get_ID()] = new KeyNamePair(loc.Get_ID(), loc.ToString());

                    lstLatLng[loc.Get_ID()] = new LatLng() { Latitude = loc.GetLatitude(), Longitude = loc.GetLongitude() };
                    count++;

                }
                dr.Close();
            }
            catch
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }

            }
            locData.LocLookup = locs;
            locData.LocLatLng = lstLatLng;
            return locData;
        }
        // Change By Mohit - To get country from login langauge on location form.
        public DefaultCountry GetCountryName(string VAF_Language,Ctx ctx)
        {
            DefaultCountry obj = new DefaultCountry();
            DataSet _ds = null;
            try
            {
                // load the country from login orgnization's organization info loaction.
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAB_Address_ID FROM VAF_OrgDetail WHERE IsActive='Y' AND VAF_Org_ID=" + ctx.GetVAF_Org_ID() + " ", null, null)) > 0)
                {
                    if (Env.IsBaseLanguage(ctx, ""))
                    {
                        _ds = DB.ExecuteDataset("SELECT cnt.VAB_Country_ID,  cnt.Name FROM VAB_Country cnt INNER JOIN VAB_Address loc ON(loc.VAB_Country_ID = cnt.VAB_Country_ID) "
                                         + " INNER JOIN VAF_OrgDetail oi ON(loc.VAB_Address_ID = oi.VAB_Address_ID) WHERE oi.VAF_Org_ID =  " + ctx.GetVAF_Org_ID());
                    }
                    else
                    {
                        _ds = DB.ExecuteDataset(@"SELECT cnt.VAB_Country_ID,  cntrl.Name FROM VAB_Country cnt INNER JOIN VAB_Country_TL cntrl ON(cnt.c_country_ID = cntrl.c_country_id)
                                                INNER JOIN VAB_Address loc ON(loc.VAB_Country_ID = cnt.VAB_Country_ID) INNER JOIN VAF_OrgDetail oi ON(loc.VAB_Address_ID = oi.VAB_Address_ID) WHERE 
                                                oi.VAF_Org_ID = " + ctx.GetVAF_Org_ID() + "  AND CNTRL.VAF_Language = '" + VAF_Language + "'");
                    }
                }
                else
                {

                    // Check applied by mohit - asked by mukesh sir - to check if login langauge is base language - then pick non translated data.
                    if (Env.IsBaseLanguage(ctx, ""))
                    {
                        _ds = DB.ExecuteDataset("SELECT Name , VAB_Country_ID FROM VAB_Country WHERE IsActive='Y' AND CountryCode=(SELECT CountryCode FROM VAF_Language WHERE IsActive='Y' AND VAF_Language='" + VAF_Language + "')");
                    }
                    else
                    {
                        // Check applied by mohit - Picked data from translation tab - if base language
                        _ds = DB.ExecuteDataset("SELECT CNTRL.Name , CN.VAB_Country_ID FROM VAB_Country CN INNER JOIN VAB_Country_TL CNTRL ON (CN.VAB_Country_ID=CNTRL.VAB_Country_ID)" +
                            " WHERE CN.IsActive='Y' AND CN.CountryCode=(SELECT CountryCode FROM VAF_Language WHERE IsActive='Y' AND VAF_Language='" + VAF_Language + "') AND CNTRL.VAF_Language='" + VAF_Language + "'");
                    }
                }
                if (_ds != null && _ds.Tables[0].Rows.Count > 0)
                {
                    obj.CountryName = _ds.Tables[0].Rows[0]["Name"].ToString();
                    obj.CountryID = Util.GetValueOfInt(_ds.Tables[0].Rows[0]["VAB_Country_ID"]);
                }
                _ds = null;

            }
            catch (Exception ev)
            {
                _ds = null;
            }
            return obj;
        }
    }

    public class SearchAddress
    {
        public String Name
        {
            get;
            set;
        }

        public int ID
        {
            get;
            set;
        }
        public int CityId
        {
            get;
            set;
        }
        public int StateId
        {
            get;
            set;
        }
        public int CountryId
        {
            get;
            set;
        }
    }

    public class LocationAddress
    {
        public string ADDRESS { get; set; }

        public string ADDRESS1 { get; set; }
        public string ADDRESS2 { get; set; }
        public string ADDRESS3 { get; set; }
        public string ADDRESS4 { get; set; }

        public string COUNTRYNAME { get; set; }
        public string CITYNAME { get; set; }
        public string STATENAME { get; set; }
        public string ZIPCODE { get; set; }

        public int VAF_CLIENT_ID { get; set; }
        public int VAF_ORG_ID { get; set; }
        // public int? VAB_CITY_ID { get; set; }
        public int? VAB_COUNTRY_ID { get; set; }
        public int VAB_ADDRESS_ID { get; set; }
        public int? VAB_REGIONSTATE_ID { get; set; }
    }

    public class LocationData
    {
        public Dictionary<int, KeyNamePair> LocLookup { get; set; }
        public Dictionary<int, LatLng> LocLatLng { get; set; }
    }

    public class LocationRecord
    {
        public KeyNamePair LocItem { get; set; }
        public LatLng LocLatLng { get; set; }
    }

    public class LatLng
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
    public class DefaultCountry
    {
        public string CountryName { get; set; }
        public int CountryID { get; set; }
    }
}