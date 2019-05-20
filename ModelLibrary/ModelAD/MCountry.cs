/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MCountry
 * Purpose        : Contries notifications/display
 * Class Used     : X_C_Country
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
    public class MCountry : X_C_Country, IComparer<PO>
    {
        #region Variables
        //	Translated Name			
        private String _trlName = null;
        //	Display Language		
        private static String _AD_Language = null;
        //	Country Cache			
        //private static CCache<string, MCountry> s_countries = new CCache<string, MCountry>();
        private static CCache<String, MCountry> s_countries = null;
        //	Default Country 		
        private static MCountry _default = null;
        //	Static Logger			
        private static VLogger _log = VLogger.GetVLogger(typeof(MCountry).FullName);
        //	Default DisplaySequence	
        private static String DISPLAYSEQUENCE = "@C@, @P@";
        #endregion

        /// <summary>
        /// Get Country (cached)
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Country_ID">ID</param>
        /// <returns>Country</returns>
        public static MCountry Get(Ctx ctx, int C_Country_ID)
        {
            if (s_countries == null || s_countries.Count == 0)
            {
                LoadAllCountries(ctx);
            }
            String key = C_Country_ID.ToString();
            MCountry c = (MCountry)s_countries[key];
            if (c != null)
                return c;
            c = new MCountry(ctx, C_Country_ID, null);
            if (c.GetC_Country_ID() == C_Country_ID)
            {
                s_countries.Add(key, c);
                return c;
            }
            return null;
        }

        /// <summary>
        /// Get Default Country
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>Country</returns>
        public static MCountry GetDefault(Ctx ctx)
        {
            if (s_countries == null || s_countries.Count == 0)
            {
                LoadAllCountries(ctx);
            }
            return _default;
        }

        /// <summary>
        /// Return Countries as Array
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>MCountry Array</returns>
        ///@SuppressWarnings("unchecked") 
        public static MCountry[] GetCountries(Ctx ctx)
        {
            if (s_countries == null || s_countries.Count == 0)
            {
                LoadAllCountries(ctx);
            }
            MCountry[] retValue = new MCountry[s_countries.Count];
            retValue = s_countries.Values.ToArray();
            Array.Sort(retValue, new MCountry(ctx, 0, null));
            return retValue;
        }

        /// <summary>
        /// Load active Countries (no summary).	
        /// Set Default Language to Client Language
        /// </summary>
        /// <param name="ctx">Ctx</param>
        private static void LoadAllCountries(Ctx ctx)
        {
            MClient client = MClient.Get(ctx);
            MLanguage lang = MLanguage.Get(ctx, client.GetAD_Language());
            MCountry usa = null;
            //

            int countryID = Util.GetValueOfInt(ctx.Get("P|C_Country_ID"));

            s_countries = new CCache<String, MCountry>("C_Country", 250);
            String sql = "SELECT * FROM C_Country WHERE IsActive='Y' AND IsSummary='N'";
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    MCountry c = new MCountry(ctx, dr, null);
                    s_countries.Add(c.GetC_Country_ID().ToString(), c);
                    //	Country code of Client Language
                    if (lang != null && lang.GetCountryCode().Equals(c.GetCountryCode()) && _default == null)
                    {
                        _default = c;
                    }
                    else if (countryID == c.GetC_Country_ID())
                    {
                        _default = c;
                    }
                    if (c.GetC_Country_ID() == 100)		//	USA
                    {
                        usa = c;
                    }

                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            if (_default == null)
            {
                _default = usa;
            }
            _log.Fine("#" + s_countries.Size()
                + " - Default=" + _default);
        }

        /// <summary>
        /// Set the Language for Display (toString)
        /// </summary>
        /// <param name="AD_Language">language or null</param>
        public static void SetDisplayLanguage(String AD_Language)
        {
            _AD_Language = AD_Language;
            //if (Language.isBaseLanguage(AD_Language))
            if (GlobalVariable.AD_BASE_LANGUAGE == AD_Language)
            {
                _AD_Language = null;
            }
        }

        /// <summary>
        /// Create empty Country
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_Country_ID"></param>
        /// <param name="trxName"></param>
        public MCountry(Ctx ctx, int C_Country_ID, Trx trxName)
            : base(ctx, C_Country_ID, trxName)
        {
            if (C_Country_ID == 0)
            {
                //	setName (null);
                //	setCountryCode (null);
                SetDisplaySequence(DISPLAYSEQUENCE);
                SetHasRegion(false);
                SetHasPostal_Add(false);
                SetIsAddressLinesLocalReverse(false);
                SetIsAddressLinesReverse(false);
            }
        }

        /// <summary>
        /// Create Country from current row in ResultSet
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MCountry(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Return Name - translated if DisplayLanguage is set.
        /// </summary>
        /// <returns>name</returns>
        public override String ToString()
        {
            if (_AD_Language != null)
            {
                String nn = GetTrlName();
                if (nn != null)
                {
                    return nn;
                }
            }
            return GetName();
        }

        /// <summary>
        /// Get Translated Name
        /// </summary>
        /// <returns>name</returns>
        public String GetTrlName()
        {
            if (_trlName != null && _AD_Language != null)
            {
                _trlName = Get_Translation("Name", _AD_Language);
                if (_trlName == null)
                {
                    _AD_Language = null;	//	assume that there is no translation
                }
            }
            return _trlName;
        }

        /// <summary>
        /// Get Display Sequence
        /// </summary>
        /// <returns>display sequence</returns>
        public new String GetDisplaySequence()
        {
            String ds = base.GetDisplaySequence();
            if (ds == null || ds.Length == 0)
            {
                ds = DISPLAYSEQUENCE;
            }
            return ds;
        }

        /// <summary>
        /// Get Local Display Sequence.
        /// If not defined get Display Sequence
        /// </summary>
        /// <returns>local display sequence</returns>
        public new String GetDisplaySequenceLocal()
        {
            String ds = base.GetDisplaySequenceLocal();
            if (ds == null || ds.Length == 0)
            {
                ds = GetDisplaySequence();
            }
            return ds;
        }

        /// <summary>
        /// Compare based on Name
        /// Work when Arrsy.Sort() call on using IComparer<PO>
        /// </summary>
        /// <param name="o1">o1 object 1</param>
        /// <param name="o2">o2 object 2</param>
        /// <returns>-1,0, 1</returns>
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

        /// <summary>
        /// Is the region valid in the country
        /// </summary>
        /// <param name="C_Region_ID">region</param>
        /// <returns>true if valid</returns>
        public bool IsValidRegion(int C_Region_ID)
        {
            if (C_Region_ID == 0
                || GetC_Country_ID() == 0
                || !IsHasRegion())
            {
                return false;
            }
            MRegion[] regions = MRegion.GetRegions(GetCtx(), GetC_Country_ID());
            for (int i = 0; i < regions.Length; i++)
            {
                if (C_Region_ID == regions[i].GetC_Region_ID())
                {
                    return true;
                }
            }
            return false;
        }

        /*	Insert Countries
        * 	@param args none
        */
        //public static void main (String[] args)
        //{
        //    /**	Migration before
        //    UPDATE C_Country SET AD_Client_ID=0, AD_Org_ID=0 WHERE AD_Client_ID<>0 OR AD_Org_ID<>0;
        //    UPDATE C_Region SET AD_Client_ID=0, AD_Org_ID=0 WHERE AD_Client_ID<>0 OR AD_Org_ID<>0;
        //    IDs migration for C_Location, C_City, C_Tax (C_Country, C_Region)
        //    **
        //    //	from http://www.iso.org/iso/en/prods-services/iso3166ma/02iso-3166-code-lists/list-en1-semic.txt
        //    String countries = "AFGHANISTAN;AF, ALBANIA;AL, ALGERIA;DZ, AMERICAN SAMOA;AS, ANDORRA;AD, ANGOLA;AO, ANGUILLA;AI, ANTARCTICA;AQ, ANTIGUA AND BARBUDA;AG, ARGENTINA;AR,"
        //        + "ARMENIA;AM, ARUBA;AW, AUSTRALIA;AU, AUSTRIA;AT, AZERBAIJAN;AZ, BAHAMAS;BS, BAHRAIN;BH, BANGLADESH;BD, BARBADOS;BB, BELARUS;BY, BELGIUM;BE, BELIZE;BZ,"
        //        + "BENIN;BJ, BERMUDA;BM, BHUTAN;BT, BOLIVIA;BO, BOSNIA AND HERZEGOVINA;BA, BOTSWANA;BW, BOUVET ISLAND;BV, BRAZIL;BR, BRITISH INDIAN OCEAN TERRITORY;IO, BRUNEI DARUSSALAM;BN,"
        //        + "BULGARIA;BG, BURKINA FASO;BF, BURUNDI;BI, CAMBODIA;KH, CAMEROON;CM, CANADA;CA, CAPE VERDE;CV, CAYMAN ISLANDS;KY, CENTRAL AFRICAN REPUBLIC;CF, CHAD;TD, CHILE;CL,"
        //        + "CHINA;CN, CHRISTMAS ISLAND;CX, COCOS (KEELING) ISLANDS;CC, COLOMBIA;CO, COMOROS;KM, CONGO;CG, CONGO THE DEMOCRATIC REPUBLIC OF THE;CD, COOK ISLANDS;CK,"
        //        + "COSTA RICA;CR, COTE D'IVOIRE;CI, CROATIA;HR, CUBA;CU, CYPRUS;CY, CZECH REPUBLIC;CZ, DENMARK;DK, DJIBOUTI;DJ, DOMINICA;DM, DOMINICAN REPUBLIC;DO, ECUADOR;EC,"
        //        + "EGYPT;EG, EL SALVADOR;SV, EQUATORIAL GUINEA;GQ, ERITREA;ER, ESTONIA;EE, ETHIOPIA;ET, FALKLAND ISLANDS (MALVINAS);FK, FAROE ISLANDS;FO, FIJI;FJ,"
        //        + "FINLAND;FI, FRANCE;FR, FRENCH GUIANA;GF, FRENCH POLYNESIA;PF, FRENCH SOUTHERN TERRITORIES;TF, GABON;GA, GAMBIA;GM, GEORGIA;GE, GERMANY;DE, GHANA;GH,"
        //        + "GIBRALTAR;GI, GREECE;GR, GREENLAND;GL, GRENADA;GD, GUADELOUPE;GP, GUAM;GU, GUATEMALA;GT, GUINEA;GN, GUINEA-BISSAU;GW, GUYANA;GY, HAITI;HT,"
        //        + "HEARD ISLAND AND MCDONALD ISLANDS;HM, HOLY SEE (VATICAN CITY STATE);VA, HONDURAS;HN, HONG KONG;HK, HUNGARY;HU, ICELAND;IS, INDIA;IN, INDONESIA;ID,"
        //        + "IRAN ISLAMIC REPUBLIC OF;IR, IRAQ;IQ, IRELAND;IE, ISRAEL;IL, ITALY;IT, JAMAICA;JM, JAPAN;JP, JORDAN;JO, KAZAKHSTAN;KZ, KENYA;KE, KIRIBATI;KI, KOREA DEMOCRATIC PEOPLE'S REPUBLIC OF;KP,"
        //        + "KOREA REPUBLIC OF;KR, KUWAIT;KW, KYRGYZSTAN;KG, LAO PEOPLE'S DEMOCRATIC REPUBLIC;LA, LATVIA;LV, LEBANON;LB, LESOTHO;LS, LIBERIA;LR, LIBYAN ARAB JAMAHIRIYA;LY,"
        //        + "LIECHTENSTEIN;LI, LITHUANIA;LT, LUXEMBOURG;LU, MACAO;MO, MACEDONIA FORMER YUGOSLAV REPUBLIC OF;MK, MADAGASCAR;MG, MALAWI;MW, MALAYSIA;MY, MALDIVES;MV, "
        //        + "MALI;ML, MALTA;MT, MARSHALL ISLANDS;MH, MARTINIQUE;MQ, MAURITANIA;MR, MAURITIUS;MU, MAYOTTE;YT, MEXICO;MX, MICRONESIA FEDERATED STATES OF;FM,"
        //        + "MOLDOVA REPUBLIC OF;MD, MONACO;MC, MONGOLIA;MN, MONTSERRAT;MS, MOROCCO;MA, MOZAMBIQUE;MZ, MYANMAR;MM, NAMIBIA;NA, NAURU;NR, NEPAL;NP,"
        //        + "NETHERLANDS;NL, NETHERLANDS ANTILLES;AN, NEW CALEDONIA;NC, NEW ZEALAND;NZ, NICARAGUA;NI, NIGER;NE, NIGERIA;NG, NIUE;NU, NORFOLK ISLAND;NF,"
        //        + "NORTHERN MARIANA ISLANDS;MP, NORWAY;NO, OMAN;OM, PAKISTAN;PK, PALAU;PW, PALESTINIAN TERRITORY OCCUPIED;PS, PANAMA;PA, PAPUA NEW GUINEA;PG,"
        //        + "PARAGUAY;PY, PERU;PE, PHILIPPINES;PH, PITCAIRN;PN, POLAND;PL, PORTUGAL;PT, PUERTO RICO;PR, QATAR;QA, REUNION;RE, ROMANIA;RO, RUSSIAN FEDERATION;RU,"
        //        + "RWANDA;RW, SAINT HELENA;SH, SAINT KITTS AND NEVIS;KN, SAINT LUCIA;LC, SAINT PIERRE AND MIQUELON;PM, SAINT VINCENT AND THE GRENADINES;VC,"
        //        + "SAMOA;WS, SAN MARINO;SM, SAO TOME AND PRINCIPE;ST, SAUDI ARABIA;SA, SENEGAL;SN, SEYCHELLES;SC, SIERRA LEONE;SL, SINGAPORE;SG, SLOVAKIA;SK,"
        //        + "SLOVENIA;SI, SOLOMON ISLANDS;SB, SOMALIA;SO, SOUTH AFRICA;ZA, SOUTH GEORGIA AND THE SOUTH SANDWICH ISLANDS;GS, SPAIN;ES, SRI LANKA;LK,"
        //        + "SUDAN;SD, SURINAME;SR, SVALBARD AND JAN MAYEN;SJ, SWAZILAND;SZ, SWEDEN;SE, SWITZERLAND;CH, SYRIAN ARAB REPUBLIC;SY, TAIWAN;TW,"
        //        + "TAJIKISTAN;TJ, TANZANIA UNITED REPUBLIC OF;TZ, THAILAND;TH, TIMOR-LESTE;TL, TOGO;TG, TOKELAU;TK, TONGA;TO, TRINIDAD AND TOBAGO;TT,"
        //        + "TUNISIA;TN, TURKEY;TR, TURKMENISTAN;TM, TURKS AND CAICOS ISLANDS;TC, TUVALU;TV, UGANDA;UG, UKRAINE;UA, UNITED ARAB EMIRATES;AE, UNITED KINGDOM;GB,"
        //        + "UNITED STATES;US, UNITED STATES MINOR OUTLYING ISLANDS;UM, URUGUAY;UY, UZBEKISTAN;UZ, VANUATU;VU, VENEZUELA;VE, VIET NAM;VN, VIRGIN ISLANDS BRITISH;VG,"
        //        + "VIRGIN ISLANDS U.S.;VI, WALLIS AND FUTUNA;WF, WESTERN SAHARA;EH, YEMEN;YE, YUGOSLAVIA;YU, ZAMBIA;ZM, ZIMBABWE;ZW";
        //    //
        
        //    StringTokenizer st = new StringTokenizer(countries, ",", false);
        //    while (st.hasMoreTokens())
        //    {
        //        String s = st.nextToken().trim();
        //        int pos = s.indexOf(";");
        //        String name = Utility.initCap(s.substring(0,pos));
        //        String cc = s.substring(pos+1);
        //        System.out.println(cc + " - " + name);
        //        //
        //        MCountry mc = new MCountry(Env.getCtx(), 0);
        //        mc.setCountryCode(cc);
        //        mc.setName(name);
        //        mc.setDescription(name);
        //        mc.save();
        //    }
        //    **/
        //}


    }
}
