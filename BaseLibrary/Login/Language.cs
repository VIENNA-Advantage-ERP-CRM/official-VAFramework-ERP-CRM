
using System;
using System.Collections.Generic;

using VAdvantage.Logging;

using VAdvantage.Print;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Classes;

using System.Runtime.CompilerServices;
using VAdvantage.DataBase;

namespace VAdvantage.Login
{

    /// <summary>
    /// Language Management
    /// </summary>
    [Serializable]
    public class Language
    {
        /**	Logger			*/
        private static Logger log = Logger.GetLogger(typeof(Language).FullName);

        public static String AD_Language_en_US = "en_US"; //base language
        private static String AD_Language_en_GB = "en_GB";//y
        private static String AD_Language_en_AU = "en_AU";//y
        private static String AD_Language_ca_ES = "ca_ES";//y
        private static String AD_Language_hr_HR = "hr_HR";//y
        private static String AD_Language_de_DE = "de_DE";//y
        private static String AD_Language_it_IT = "it_IT";//y
        private static String AD_Language_es_ES = "es_ES";//y
        private static String AD_Language_es_MX = "es_MX";//y
        private static String AD_Language_fr_FR = "fr_FR";//y
        private static String AD_Language_bg_BG = "bg_BG";//y
        private static String AD_Language_th_TH = "th_TH";//n
        private static String AD_Language_pl_PL = "pl_PL";//y
        private static String AD_Language_zh_TW = "zh_TW";//y
        private static String AD_Language_nl_NL = "nl_NL";//y
        private static String AD_Language_no_NO = "nb_NO"; //not supported (no is supported instead)
        private static String AD_Language_pt_BR = "pt_BR";//y
        private static String AD_Language_ru_RU = "ru_RU";//y
        private static String AD_Language_sl_SI = "sl_SI";//y
        private static String AD_Language_sv_SE = "sv_SE";//y
        private static String AD_Language_vi_VN = "vi_VN";//y
        private static String AD_Language_zh_CN = "zh_CN";//y
        private static String AD_Language_da_DK = "da_DK";//y
        private static String AD_Language_ml_ML = "ms-BN";
        private static String AD_Language_fa_IR = "fa_IR";//y
        private static String AD_Language_fi_FI = "fi_FI";//y
        private static String AD_Language_ro_RO = "ro_RO";//y
        private static String AD_Language_ja_JP = "ja_JP";//y
        private static String AD_Language_in_ID = "id_ID"; //y
        private static String AD_Language_ar_TN = "ar_TN"; //y
        private static String AD_Language_ar_IQ = "ar_IQ"; //y
        private static String AD_Language_ar_KU = "ar_KU"; //y
        private static String AD_Language_zh_HK = "zh_HK";//y
        private static String AD_Language_es_UY = "es_UY"; //y
        private static String AD_Language_ar_SA = "ar_SA"; //y
        private static String AD_Language_en_IN = "en_IN"; //y
        private static String AD_Language_sq_AL = "sq_AL"; //y


        static private CCache<int, Language> _languages = new CCache<int, Language>("loginlang", 10);
        static List<ValueNamePair> langList = new List<ValueNamePair>();

        //{
        //        new Language 
        //            ("English", AD_Language_en_US, new System.Globalization.CultureInfo("en-US") ,  null, "", MediaSize.NA.LETTER),
        //        new Language
        //            ("\uFE94\uFEF4\uFE91\uFEAE\uFECC\uFEDF\uFE8D (AR)", AD_Language_ar_TN,new System.Globalization.CultureInfo("ar-TN") ,   true, "dd.MM.yyyy", MediaSize.ISO.A4),
        //        new Language
        //            ("\uFE94\uFEF4\uFE91\uFEAE\uFECC\uFEDF\uFE8D (AR-Iraq)", AD_Language_ar_IQ,new System.Globalization.CultureInfo("ar-IQ") ,   true, "dd.MM.yyyy", MediaSize.ISO.A4),
        //  new Language 
        //            ("\u0411\u044A\u043B\u0433\u0430\u0440\u0441\u043A\u0438 (BG)", AD_Language_bg_BG, new System.Globalization.CultureInfo("bg-BG") ,  false, "dd/MM/yyyy", MediaSize.ISO.A4),
        //  new Language 
        //            ("Catal\u00e0", AD_Language_ca_ES,new System.Globalization.CultureInfo("ca-ES") ,   null, "dd/MM/yyyy", MediaSize.ISO.A4),
        //        new Language
        //            ("Deutsch", AD_Language_de_DE, new System.Globalization.CultureInfo("de-DE") ,  null, "dd.MM.yyyy", MediaSize.ISO.A4),
        //		new Language 
        //            ("Dansk", AD_Language_da_DK,  new System.Globalization.CultureInfo("da-DK") ,   false, "dd-MM-yyyy", MediaSize.ISO.A4),
        //  new Language 
        //            ("English (AU)", AD_Language_en_AU,  new System.Globalization.CultureInfo("en-AU") ,  null, "dd/MM/yyyy", MediaSize.ISO.A4),
        //  new Language 
        //            ("English (UK)", AD_Language_en_GB,  new System.Globalization.CultureInfo("en-GB") ,    null, "dd/MM/yyyy", MediaSize.ISO.A4),
        //  new Language 
        //            ("Espa\u00f1ol", AD_Language_es_ES,new System.Globalization.CultureInfo("es-ES") ,  false, "dd/MM/yyyy", MediaSize.ISO.A4),
        //  new Language 
        //            ("Espa\u00f1ol (MX)", AD_Language_es_MX,  new System.Globalization.CultureInfo("es-MX") ,  false, "dd/MM/yyyy", MediaSize.NA.LETTER),
        //        new Language 
        //            ("Espa\u00f1ol (UY)", AD_Language_es_UY, new System.Globalization.CultureInfo("es-UY") ,    false, "dd/MM/yyyy", MediaSize.ISO.A4),
        //  new Language 
        //            ("Farsi", AD_Language_fa_IR, new System.Globalization.CultureInfo("fa-IR") ,    false, "dd-MM-yyyy", MediaSize.ISO.A4),
        //  new Language 
        //            ("Finnish", AD_Language_fi_FI,  new System.Globalization.CultureInfo("fi-FI") ,   true, "dd.MM.yyyy", MediaSize.ISO.A4),
        //  new Language 
        //            ("Fran\u00e7ais", AD_Language_fr_FR, new System.Globalization.CultureInfo("fr-FR") ,    null, "", MediaSize.ISO.A4),
        //  new Language 
        //            ("Hrvatski", AD_Language_hr_HR, new System.Globalization.CultureInfo("hr-HR") ,  null, "dd.MM.yyyy", MediaSize.ISO.A4),
        //  new Language 
        //            ("Indonesia Bahasa", AD_Language_in_ID, new System.Globalization.CultureInfo("id-ID") ,  false, "dd-MM-yyyy", MediaSize.ISO.A4),
        // 	new Language 
        //            ("Italiano", AD_Language_it_IT, new System.Globalization.CultureInfo("it-IT") ,    null, "", MediaSize.ISO.A4),
        //  new Language 
        //            ("\u65e5\u672c\u8a9e (JP)", AD_Language_ja_JP, new System.Globalization.CultureInfo("ja-JP") ,  null, "", MediaSize.ISO.A4),
        //  new Language 
        //            ("Malay", AD_Language_ml_ML,new System.Globalization.CultureInfo("ms-BN") ,  false, "dd-MM-yyyy", MediaSize.ISO.A4),
        //  new Language 
        //            ("Nederlands", AD_Language_nl_NL, new System.Globalization.CultureInfo("nl-NL") ,  false, "dd-MM-yyyy", MediaSize.ISO.A4),
        //  new Language 
        //            ("Norsk", AD_Language_no_NO, new System.Globalization.CultureInfo("nb-NO") ,  false, "dd/MM/yyyy", MediaSize.ISO.A4),
        //  new Language 
        //            ("Polski", AD_Language_pl_PL, new System.Globalization.CultureInfo("pl-PL") ,  false, "dd-MM-yyyy", MediaSize.ISO.A4),
        //  new Language 
        //            ("Portuguese (BR)", AD_Language_pt_BR, new System.Globalization.CultureInfo("pt-BR") ,  false, "dd/MM/yyyy", MediaSize.ISO.A4),
        //  new Language 
        //            ("Rom\u00e2n\u0103", AD_Language_ro_RO,new System.Globalization.CultureInfo("ro-RO") ,   false, "dd.MM.yyyy", MediaSize.ISO.A4),
        //  new Language 
        //            ("\u0420\u0443\u0441\u0441\u043a\u0438\u0439 (Russian)", AD_Language_ru_RU, new System.Globalization.CultureInfo("ru-RU") ,  false, "dd-MM-yyyy", MediaSize.ISO.A4),
        //  new Language 
        //            ("Slovenski", AD_Language_sl_SI, new System.Globalization.CultureInfo("sl-SI") ,  null, "dd.MM.yyyy", MediaSize.ISO.A4),
        //  new Language 
        //            ("Svenska", AD_Language_sv_SE, new System.Globalization.CultureInfo("sv-SE") ,    false, "dd.MM.yyyy", MediaSize.ISO.A4),
        //  new Language 
        //            ("\u0e44\u0e17\u0e22 (TH)", AD_Language_th_TH, new System.Globalization.CultureInfo("th-TH") ,   false, "dd/MM/yyyy", MediaSize.ISO.A4),
        //  new Language 
        //            ("Vi\u1EC7t Nam", AD_Language_vi_VN, new System.Globalization.CultureInfo("vi-VN") ,  false, "dd-MM-yyyy", MediaSize.ISO.A4),
        //		new Language 
        //            ("\u7b80\u4f53\u4e2d\u6587 (CN)", AD_Language_zh_CN, new System.Globalization.CultureInfo("zh-CN") ,   null, "yyyy-MM-dd", MediaSize.ISO.A4),
        //  new Language 
        //            ("\u7e41\u9ad4\u4e2d\u6587 (TW)", AD_Language_zh_TW, new System.Globalization.CultureInfo("zh-TW") ,  null, null, MediaSize.ISO.A4),                

        //            new Language
        //            ("Khurdi", AD_Language_ar_KU,new System.Globalization.CultureInfo("ar-IQ") ,   true, "dd.MM.yyyy", MediaSize.ISO.A4)
        //          ,
        //		new Language 
        //            ("\u7b80\u4f53\u4e2d\u6587 (Hongkong)", AD_Language_zh_HK, new System.Globalization.CultureInfo("zh-HK") ,   null, "yyyy-MM-dd", MediaSize.ISO.A4),
        //            new Language 
        //            ("\uFE94\uFEF4\uFE91\uFEAE\uFECC\uFEDF\uFE8D (AR-SA)", AD_Language_ar_SA, new System.Globalization.CultureInfo("ar-SA") ,   null, "dd.MM.yyyy", MediaSize.ISO.A4),
        //};

        // [MethodImpl(MethodImplOptions.Synchronized)]
        public static DataTable GetSystemLanguage()
        {
            DataSet ds = DB.ExecuteDataset("SELECT AD_Language,Name,Name AS DisplayName FROM AD_Language WHERE IsSystemLanguage = 'Y' AND IsActive='Y' Order BY  Name asc");
            if (ds != null)
                return ds.Tables[0];
            return null;
        }

        static object _lock = new object();
        public static void FillLanguage()
        {
            if (_languages.Count > 0)
            {
                return;
            }

            lock (_lock)
            {
                if (_languages.Count > 0)
                {
                    return;
                }

                langList.Clear();
                DataTable dt = GetSystemLanguage();
                _languages.Add(0, new Language("English", AD_Language_en_US, new System.Globalization.CultureInfo("en-US"), null, "", MediaSize.NA.LETTER));
                langList.Add(new ValueNamePair("en_US", "English"));
                _loginLanguage = _languages[0];
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        try
                        {
                            var cul = new System.Globalization.CultureInfo(row["AD_Language"].ToString().Replace("_", "-"));
                            _languages.Add(_languages.Count, new Language
                            (row["Name"].ToString(), row["AD_Language"].ToString(),
                            cul, cul.NumberFormat.NumberDecimalSeparator == ".", cul.DateTimeFormat.ShortDatePattern, MediaSize.ISO.A4));

                            langList.Add(new ValueNamePair(row["AD_Language"].ToString(), row["DisplayName"].ToString()));
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }
        }



        //static Language()
        //{
        //    FillLanguage();

        //}

        //Default Language            
        private static Language _loginLanguage = null;
        private MediaSize m_mediaSize = null;// MediaSize.ISO.A4;

        /// <summary>
        /// Get Number of Languages
        /// </summary>
        /// <returns>Language count</returns>
        //public static int GetLanguageCount()
        //{
        //    return _languages.Length;
        //}   //  getLanguageCount


        /// <summary>
        /// Get Language
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>Language</returns>
        //public static Language GetLanguage(int index)
        //{
        //    if (index < 0 || index >= _languages.Length)
        //        return _loginLanguage;
        //    return _languages[index];
        //}   //  getLanguage



        /// <summary>
        /// Get Language.
        /// </summary>
        /// <param name="langInfo">either language (en) or locale (en-US) or display name</param>
        /// <returns> Name (e.g. Deutsch)</returns>
        public static Language GetLanguage(String langInfo)
        {
            String lang = langInfo;
            FillLanguage();

            //	Search existing Languages
            for (int i = 0; i < _languages.Count; i++)
            {
                if (lang.Equals(_languages[i].GetName())
                    || lang.Equals(_languages[i].GetLanguageCode())
                    || lang.Equals(_languages[i].GetAD_Language()))
                    return _languages[i];
            }

            return _loginLanguage;
        }

        public static List<ValueNamePair> GetLanguages()
        {
            FillLanguage();
            return langList;
        }

        /// <summary>
        /// Is This the Base Language
        /// </summary>
        /// <param name="langInfo">language info</param>
        /// <returns>true if base language</returns>
        public static bool IsBaseLanguage(String langInfo)
        {
            var lang = GetBaseLanguage();
            if (langInfo == null || langInfo.Length == 0
                || langInfo.Equals(lang.GetName())
                || langInfo.Equals(lang.GetLanguageCode())
                || langInfo.Equals(lang.GetAD_Language()))
                return true;
            return false;
        }   //  isBaseLanguage



        /// <summary>
        /// Get Base Language
        /// </summary>
        /// <returns>Base Language</returns>
        public static Language GetBaseLanguage()
        {
            FillLanguage();
            return _languages[0];
        }   //  getBase

        /// <summary>
        /// Get Base Language code. (e.g. en-US)
        /// </summary>
        /// <returns>Base Language</returns>
        public static String GetBaseAD_Language()
        {
            var lang = GetBaseLanguage();
            return lang.GetAD_Language();
        }   //  getBase


        /// <summary>
        /// Get Supported Culture
        /// </summary>
        /// <param name="langInfo">either language (en) or locale (en-US) or display name</param>
        /// <returns>Supported Culture</returns>
        public System.Globalization.CultureInfo GetCulture(string langInfo)
        {
            return GetLanguage(langInfo).GetCulture();
        }


        /// <summary>
        ///  Get Application Dictionary Language (system supported).
        ///  <example>e.g - en-US</example>
        /// </summary>
        /// <returns>AD_Language</returns>
        //public String GetAD_Language(string langInfo)
        //{
        //    return GetLanguage(langInfo).GetAD_Language();
        //}   //  getAD_Language


        /// <summary>
        /// Get Supported Language
        /// </summary>
        /// <param name="culture">Locale</param>
        /// <returns>AD_Language</returns>
        //public static String GetAD_Language(System.Globalization.CultureInfo culture)
        //{
        //    if (culture != null)
        //    {
        //        for (int i = 0; i < _languages.Length; i++)
        //        {
        //            if (culture.Equals(_languages[i].GetCulture()))
        //                return _languages[i].GetAD_Language();
        //        }
        //    }
        //    return _loginLanguage.GetAD_Language();
        //}   //  getLocale



        /// <summary>
        /// Get Display names of supported languages
        /// </summary>
        /// <returns>Array of Language names</returns>
        //public static String[] GetNames()
        //{
        //    String[] retValue = new String[_languages.Length];
        //    for (int i = 0; i < _languages.Length; i++)
        //        retValue[i] = _languages[i].GetName();
        //    return retValue;
        //}   //  getNames


        /// <summary>
        /// use same function with parameter Ctx instead
        /// Get Login Language
        /// </summary>
        ///
        [ObsoleteAttribute("This method will soon be deprecated. Use same with paramter instead.")]
        public static Language GetLoginLanguage()
        {

            return _loginLanguage;
        }   //  setLanguage

        public static Language GetLoginLanguage(Ctx ctx)
        {
            return Env.GetLoginLanguage(ctx);
        }

        /// <summary>
        /// Set Default Login Language
        /// </summary>
        /// <param name="language">language</param>
        public static void SetLoginLanguage(Language language)
        {
            if (language != null)
            {
                _loginLanguage = language;
                log.Config(_loginLanguage.ToString());
            }
        }   //  setLanguage



        /// <summary>
        /// Define Language with A4 and default decimal point and date format
        /// </summary>
        /// <param name="name">displayed value, e.g. English</param>
        /// <param name="AD_Language">
        /// the code of system supported langauge, e.g. en_US
        /// <para>
        /// (might be different than Locale - i.e. if the system does not support the language)
        /// </para>
        /// </param>
        /// <param name="locale">the Locale, e.g. Locale.US</param>
        /// <param name="decimalPoint">true if Decimal Point - if null, derived from Locale</param>
        /// <param name="appDatePattern">Java date pattern as not all locales are defined - if null, derived from Locale</param>
        /// <param name="mediaSize">default media size</param>
        //public Language(String name, String AD_Language, System.Globalization.CultureInfo culture, Boolean? decimalPoint, String appDatePattern)
        //{
        //    if (name == null || AD_Language == null)
        //        throw new ArgumentException("Language - parameter is null");
        //    _name = name;
        //    _AD_Language = AD_Language;
        //    _culture = culture;

        //    //
        //    _decimalPoint = decimalPoint;   //set the decimal point
        //    SetDateFormat(appDatePattern);
        //}

        public Language(String name, String AD_Language, System.Globalization.CultureInfo culture, Boolean? decimalPoint, String appDatePattern, MediaSize mediaSize)
        {
            if (name == null || AD_Language == null)
                throw new ArgumentException("Language - parameter is null");
            _name = name;
            _AD_Language = AD_Language;
            _culture = culture;

            //
            _decimalPoint = decimalPoint;   //set the decimal point
            SetDateFormat(appDatePattern);
            SetMediaSize(mediaSize);
        }

        /// <summary>
        /// Set the Media Size
        /// </summary>
        /// <param name="size">MediaSize</param>
        public void SetMediaSize(MediaSize size)
        {
            if (size != null)
                m_mediaSize = size;
        }	//	setMediaSize

        /// <summary>
        /// Get the Media Size
        /// </summary>
        /// <returns></returns>
        public MediaSize GetMediaSize()
        {
            return m_mediaSize;
        }	//	getMediaSize

        /// <summary>
        /// Define Language with A4 and default decimal point and date format
        /// </summary>
        /// <param name="name">displayed value, e.g. English</param>
        /// <param name="AD_Language">
        /// the code of system supported langauge, e.g. en_US
        /// <para>
        /// (might be different than Locale - i.e. if the system does not support the language)
        /// </para>
        /// </param>
        /// <param name="locale">the Locale, e.g. Locale.US</param>
        //public Language(String name, String AD_Language, System.Globalization.CultureInfo culture)
        //    : this(name, AD_Language, culture, false, "")
        //{
        //}	//	Language


        //Name					
        private String _name = "";
        //Language (key)			
        private String _AD_Language;

        private System.Globalization.CultureInfo _culture;

        private Boolean? _decimalPoint;
        private Boolean? _leftToRight;

        System.Globalization.DateTimeFormatInfo _dateFormatInfo;
        System.Globalization.NumberFormatInfo _numberFormatInfo;


        /// <summary>
        /// Get Language Name.
        /// e.g. English
        /// </summary>
        /// <returns>name</returns>
        public String GetName()
        {
            return _name;
        }   //  getName


        /// <summary>
        /// Get Application Dictionary Language (system supported).
        /// e.g. en-US
        /// </summary>
        /// <returns>AD_Language</returns>
        public String GetAD_Language()
        {
            return _AD_Language;
        }   //  getAD_Language


        /// <summary>
        /// Set Application Dictionary Language (system supported).
        /// </summary>
        /// <param name="AD_Language">AD_Language e.g. en-US</param>
        public void SetAD_Language(String AD_Language)
        {
            if (AD_Language != null)
            {
                _AD_Language = AD_Language;
                log.Config(ToString());
            }
        }   //  getAD_Language


        /// <summary>
        /// Get Culture
        /// </summary>
        /// <returns>culture</returns>
        public System.Globalization.CultureInfo GetCulture()
        {
            return _culture;
        }


        /// <summary>
        /// Set the culture in system thread with existing thread
        /// </summary>
        /// <param name="culture">name of the current culture</param>
        //private void SetCulture(System.Globalization.CultureInfo culture)
        //{
        //    if (culture == null)
        //        return;

        //    _culture = culture;
        //    _decimalPoint = null;
        //}


        /// <summary>
        /// Get Language Code
        /// </summary>
        /// <returns>Language code</returns>
        public String GetLanguageCode()
        {
            return _culture.TwoLetterISOLanguageName;
        }   //  getLanguageCode


        /// <summary>
        /// Component orientation is Left To Right
        /// </summary>
        /// <returns>true if left-to-right</returns>
        public bool IsLeftToRight()
        {
            if (_leftToRight == null)
                //  returns true if language not iw, ar, fa, ur
                _leftToRight = _culture.TextInfo.IsRightToLeft;
            return (bool)_leftToRight;
        }   //  isLeftToRight


        /// <summary>
        /// Get Language Name
        /// </summary>
        /// <param name="langInfo">either language (en) or locale (en-US) or display name</param>
        /// <returns>Langauge Name (e.g. English)</returns>
        public static String GetName(String langInfo)
        {
            return GetLanguage(langInfo).GetName();
        }   //  getAD_Language


        /// <summary>
        /// Returns true if Decimal Point (not comma)
        /// </summary>
        /// <returns>use of decimal point</returns>
        public bool IsDecimalPoint()
        {
            if (_decimalPoint == null)
            {
                _numberFormatInfo = _culture.NumberFormat;
                _decimalPoint = _numberFormatInfo.CurrencyDecimalSeparator.Equals(".");
            }
            return (bool)_decimalPoint;
        }   //  isDecimalPoint


        /// <summary>
        /// Is This the Base Language
        /// </summary>
        /// <returns>true if base Language</returns>
        public bool IsBaseLanguage()
        {
            return this.Equals(GetBaseLanguage());
        }	//	isBaseLanguage


        /// <summary>
        /// Set Date Pattern.
        /// <para>The date format is not checked for correctness</para>
        /// </summary>
        /// <param name="appDatePattern">appDatePattern for details see java.text.SimpleDateFormat, 
        /// format must be able to be converted to database date format by
        /// using the upper case function.
        /// <para>It also must have leading zero for day and month.</para>
        /// </param>
        public void SetDateFormat(String appDatePattern)
        {
            if (string.IsNullOrEmpty(appDatePattern))
            {
                _dateFormatInfo = _culture.DateTimeFormat;
                return;
            }
            _dateFormatInfo = _culture.DateTimeFormat;
            try
            {
                _dateFormatInfo.ShortDatePattern = appDatePattern;  //apply the custom date format
            }
            catch (Exception e)
            {
                log.Severe(appDatePattern + " - " + e.Message);
                _dateFormatInfo = null;  //if apply fails, set the default culture pattern
            }
        }   //  setDateFormat


        /// <summary>
        /// Get (Short) Date Format.
        /// The date format must parseable by DateTimeFormatInfo
        /// i.e. leading zero for date and month
        /// </summary>
        /// <returns>date format MM/dd/yyyy - dd.MM.yyyy</returns>
        //public System.Globalization.DateTimeFormatInfo GetDateFormat()
        //{
        //    if (_dateFormatInfo == null)
        //    {
        //        _dateFormatInfo = _culture.DateTimeFormat;
        //        String sFormat = _dateFormatInfo.ShortDatePattern;
        //        //	some short formats have only one M and d (e.g. ths US)
        //        if (sFormat.IndexOf("MM") == -1 && sFormat.IndexOf("dd") == -1)
        //        {
        //            String nFormat = "";
        //            for (int i = 0; i < sFormat.Length; i++)
        //            {
        //                if (sFormat[i] == 'M')
        //                    nFormat += "MM";
        //                else if (sFormat[i] == 'd')
        //                    nFormat += "dd";
        //                else
        //                    nFormat += sFormat[i];
        //            }
        //            //	log.finer(sFormat + " => " + nFormat);
        //            _dateFormatInfo.ShortDatePattern = nFormat;
        //        }
        //        //	Unknown short format => use JDBC
        //        if (_dateFormatInfo.ShortDatePattern.Length != 8)
        //            _dateFormatInfo.ShortDatePattern = "yyyy-MM-dd";

        //        //	4 digit year
        //        if (_dateFormatInfo.ShortDatePattern.IndexOf("yyyy") == -1)
        //        {
        //            sFormat = _dateFormatInfo.ShortDatePattern;
        //            String nFormat = "";
        //            for (int i = 0; i < sFormat.Length; i++)
        //            {
        //                if (sFormat[i] == 'y')
        //                    nFormat += "yy";
        //                else
        //                    nFormat += sFormat[i];
        //            }
        //            _dateFormatInfo.ShortDatePattern = nFormat;
        //        }
        //    }
        //    return _dateFormatInfo;
        //}   //  getDateFormat



        /// <summary>
        /// Get Date Time Format.
        /// Used for Display only
        /// </summary>
        /// <returns>Date Time format MMM d, yyyy h:mm:ss a z -or- dd.MM.yyyy HH:mm:ss z -or- j nnn aaaa, H' ?????? 'm' ????'</returns>
        //public System.Globalization.DateTimeFormatInfo GetDateTimeFormat()
        //{
        //    System.Globalization.DateTimeFormatInfo retValue = _culture.DateTimeFormat;
        //    return retValue;
        //}	//	getDateTimeFormat


        /// <summary>
        /// Get Time Format.
        /// Used for Display only
        /// </summary>
        /// <returns>Time format h:mm:ss z or HH:mm:ss z</returns>
        //public System.Globalization.DateTimeFormatInfo GetTimeFormat()
        //{
        //    return _culture.DateTimeFormat;
        //}	//	getTimeFormat


        /// <summary>
        /// Get Database Date Pattern.
        /// Derive from date pattern (make upper case)
        /// </summary>
        /// <returns>date pattern</returns>
        //public String GetDBdatePattern()
        //{
        //    return GetDateFormat().ShortDatePattern.ToUpper(_culture);
        //}   //  getDBdatePattern

        /// <summary>
        /// Get time format info (new)
        /// </summary>
        /// <returns></returns>
        //public string GetTimeFormatString()
        //{
        //    return _culture.DateTimeFormat.ShortTimePattern;
        //}

        /// <summary>
        /// Hashcode
        /// </summary>
        ///// <returns>hashcode</returns>
        //public int HashCode()
        //{
        //    return _AD_Language.GetHashCode();
        //}	//	hashcode

        /// <summary>
        /// Get hashcode
        /// </summary>
        /// <returns>hashcode</returns>
        //public override int GetHashCode()
        //{
        //    return _AD_Language.GetHashCode();
        //}

        /// <summary>
        /// Equals.
        /// Two languages are equal, if they have the same AD_Language
        /// </summary>
        /// <param name="obj">compare</param>
        /// <returns>true if AD_Language is the same</returns>
        public override bool Equals(Object obj)
        {
            if (obj.GetType() == typeof(Language))
            {
                Language cmp = (Language)obj;
                if (cmp.GetAD_Language().Equals(_AD_Language))
                    return true;
            }
            return false;
        }	//	equals


        const string RESOURCE_LOCATION = "VAdvantage.Login.ALoginRes";

        /// <summary>
        /// Gets the resource for a particualr key
        /// </summary>
        /// <param name="key">key of the resource</param>
        /// <returns>Languge translation</returns>
        //public string GetResource(string key)
        //{

        //    string resText = key;

        //    string languageName = RESOURCE_LOCATION + "_" + _culture.TwoLetterISOLanguageName;   //get the current culure name (e.g en-US)
        //    if (IsBaseLanguage())
        //        languageName = RESOURCE_LOCATION;
        //    Type typeLang = Type.GetType(languageName);
        //    if (typeLang == null)   //try again with full name
        //        typeLang = Type.GetType(RESOURCE_LOCATION + "_" + GetAD_Language());
        //    if (typeLang != null)
        //    {
        //        if (typeLang.IsClass)
        //        {
        //            LanguageCall lang = (LanguageCall)Activator.CreateInstance(typeLang);
        //            resText = lang.GetResource(key);
        //        }
        //        else
        //        {
        //            //Get key text from Dictionary
        //            resText = key;
        //        }
        //    }

        //    return resText;

        //}

        //private PaperSize m_mediaSize;

        //public PaperSize GetMediaSize()
        //{
        //    return m_mediaSize;
        //}	//	getMediaSize

        //public void SetMediaSize(PaperKind size)
        //{
        //    if (size != null)
        //        m_mediaSize.Kind = size;
        //}	//	setMediaSize

        public override String ToString()
        {
            //StringBuilder sb = new StringBuilder("Language=[");
            //sb.Append(_name).Append(",Culture=").Append(_culture.ToString())
            //    .Append(",AD_Language=").Append(_AD_Language)
            //    .Append(",DatePattern=").Append(GetDBdatePattern())
            //    .Append(",DecimalPoint=").Append(IsDecimalPoint())
            //    .Append("]");
            //return sb.ToString();
            return "";
        }   //  toString
    }
}
