/********************************************************
 * Module Name    :  Email Test
 * Purpose        : Request Mail Template model.
 *	                 Cannot be cached as it holds PO/BPartner/User to parse
 * Class Used     : X_R_MailText
 * Chronological Development
 * raghunandan    26-March-2010
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Common;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    /// <summary>
    /// Request Mail Template model.
    ///	Cannot be cached as it holds PO/BPartner/User to parse
    /// </summary>
    public class MMailText : X_R_MailText
    {
        /**	Parse User			*/
        private MUser _user = null;
        /** Parse BPartner		*/
        private X_C_BPartner _bpartner = null;
        /** Parse PO			*/
        /**parse  Lookup column **/
        private POInfo _poInfo = null;

        private PO _po = null;
        /** Translated Header	*/
        private String _mailHeader = null;
        /** Translated Text		*/
        private String _mailText = null;
        /** Translated Text 2	*/
        private String _mailText2 = null;
        /** Translated Text 3	*/
        private String _mailText3 = null;
        /** Translation Cache	*/
        private static CCache<String, MMailTextTrl> cacheTrl = new CCache<String, MMailTextTrl>("", 20);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="R_MailText_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MMailText(Ctx ctx, int R_MailText_ID, Trx trxName)
            : base(ctx, R_MailText_ID, trxName)
        {
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MMailText(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// Get parsed/translated Mail Header
        /// </summary>
        /// <returns>parsed/translated text</returns>
        public new String GetMailHeader()
        {
            if (_mailHeader == null)
                Translate();
            return Parse(_mailHeader);
        }

        /// <summary>
        /// Get parsed/translated Mail Text
        /// </summary>
        /// <returns>parsed/translated text</returns>
        public new String GetMailText()
        {
            if (_mailText == null)
                Translate();
            return Parse(_mailText);
        }

        /// <summary>
        /// Get parsed/translated Mail Text
        /// </summary>
        /// <param name="all">concatinate all</param>
        /// <returns>parsed/translated text</returns>
        public String GetMailText(bool all)
        {
            if (_mailText == null)
                Translate();
            if (!all)
                return Parse(_mailText);
            //
            StringBuilder sb = new StringBuilder();
            sb.Append(_mailText);
            String s = _mailText2;
            if (s != null && s.Length > 0)
                sb.Append("\n").Append(s);
            s = _mailText3;
            if (s != null && s.Length > 0)
                sb.Append("\n").Append(s);
            //
            return Parse(sb.ToString());
        }

        /// <summary>
        /// Get parsed/translated Mail Text 2
        /// </summary>
        /// <returns>parsed/translated text</returns>
        public new String GetMailText2()
        {
            if (_mailText == null)
                Translate();
            return Parse(_mailText2);
        }

        /// <summary>
        /// Get parsed/translated Mail Text 3
        /// </summary>
        /// <returns>parsed/translated text</returns>
        public new String GetMailText3()
        {
            if (_mailText == null)
                Translate();
            return Parse(_mailText3);
        }

        /// <summary>
        /// Get Translation
        /// </summary>
        /// <param name="AD_Language">language</param>
        /// <returns>trl</returns>
        private MMailTextTrl GetTranslation(String AD_Language)
        {
            MMailTextTrl trl = null;
            String sql = "SELECT * FROM R_MailText_Trl WHERE R_MailText_ID=@textid AND AD_Language=@lang";
            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@textid", GetR_MailText_ID());
                param[1] = new SqlParameter("@lang", AD_Language);
                dr = DataBase.DB.ExecuteReader(sql, param);
                if (dr.Read())
                {
                    trl = new MMailTextTrl();
                    trl.AD_Language = dr["AD_Language"].ToString();
                    trl.mailHeader = dr["MailHeader"].ToString();
                    trl.mailText = dr["MailText"].ToString();
                    trl.mailText2 = dr["MailText2"].ToString();
                    trl.mailText3 = dr["MailText3"].ToString();
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            return trl;
        }

        /// <summary>
        /// Parse Text
        /// </summary>
        /// <param name="text">text</param>
        /// <returns>parsed text</returns>
        private String Parse(String text)
        {
            if (text == null || text.IndexOf("@") == -1)
                return text;
            //	Parse User
            // text = Parse(text, _user);
            //	Parse BP
            // text = Parse(text, _bpartner);
            //	Parse PO
            text = Parse(text, _po);
            //
            return text;
        }

        /// <summary>
        /// Parse text
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="po">po object</param>
        /// <returns>parsed text</returns>
        private String Parse(String text, PO po)
        {
            if (po == null || text.IndexOf("@") == -1)
                return text;

            String inStr = text;
            String token;
            StringBuilder outStr = new StringBuilder();

            int i = inStr.IndexOf("@");
            while (i != -1)
            {
                outStr.Append(inStr.Substring(0, i));			// up to @
                inStr = inStr.Substring(i + 1); ///from first @

                int j = inStr.IndexOf("@");						// next @
                if (j < 0)										// no second tag
                {
                    inStr = "@" + inStr;
                    break;
                }

                token = inStr.Substring(0, j);
                if (token == "Tenant")
                {
                    int id = po.GetAD_Client_ID();
                    outStr.Append(DB.ExecuteScalar("Select Name FROM AD_Client WHERE AD_Client_ID=" + id));
                }
                else if (token == "Org")
                {
                    int id = po.GetAD_Org_ID();
                    outStr.Append(DB.ExecuteScalar("Select Name FROM AD_ORG WHERE AD_ORG_ID=" + id));
                }
                else if (token == "BPName")
                {
                    if (po.Get_TableName() == "C_BPartner")
                    {
                        outStr.Append(ParseVariable("Name", po));
                    }
                    else
                    {
                        outStr.Append("@" + token + "@");
                    }
                }
                else
                {
                    outStr.Append(ParseVariable(token, po));		// replace context
                }
                inStr = inStr.Substring(j + 1);
                // from second @
                i = inStr.IndexOf("@");
            }

            outStr.Append(inStr);           					//	add remainder
            return outStr.ToString();
        }

        /// <summary>
        /// Parse Variable
        /// </summary>
        /// <param name="variable">variable</param>
        /// <param name="po">po object</param>
        /// <returns>translated variable or if not found the original tag</returns>
        private String ParseVariable(String variable, PO po)
        {
            int index = po.Get_ColumnIndex(variable);
            if (index == -1)
                return "@" + variable + "@";	//	keep for next
            //
            Object value = po.Get_Value(index);
            if (value == null)
                return "";

            MColumn column = (new MTable(GetCtx(), po.Get_Table_ID(), Get_Trx())).GetColumn(variable);
            if (column.GetAD_Reference_ID() == DisplayType.Location)
            {
                StringBuilder sb = new StringBuilder();
                DataSet ds = DB.ExecuteDataset(@"SELECT l.address1,
                                                          l.address2,
                                                          l.address3,
                                                          l.address4,
                                                          l.city,
                                                          CASE
                                                            WHEN l.C_City_ID IS NOT NULL
                                                            THEN
                                                              ( SELECT NAME FROM C_City ct WHERE ct.C_City_ID=l.C_City_ID
                                                              )
                                                            ELSE NULL
                                                          END CityName,
                                                          (SELECT NAME FROM C_Country c WHERE c.C_Country_ID=l.C_Country_ID
                                                          ) AS CountryName
                                                        FROM C_Location l WHERE l.C_Location_ID="+value);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0]["address1"] != null && ds.Tables[0].Rows[0]["address1"] != DBNull.Value)
                    {
                        sb.Append(ds.Tables[0].Rows[0]["address1"]).Append(",");
                    }
                    if (ds.Tables[0].Rows[0]["address2"] != null && ds.Tables[0].Rows[0]["address2"] != DBNull.Value)
                    {
                        sb.Append(ds.Tables[0].Rows[0]["address2"]).Append(",");
                    }
                    if (ds.Tables[0].Rows[0]["address3"] != null && ds.Tables[0].Rows[0]["address3"] != DBNull.Value)
                    {
                        sb.Append(ds.Tables[0].Rows[0]["address3"]).Append(",");
                    }
                    if (ds.Tables[0].Rows[0]["address4"] != null && ds.Tables[0].Rows[0]["address4"] != DBNull.Value)
                    {
                        sb.Append(ds.Tables[0].Rows[0]["address4"]).Append(",");
                    }
                    if (ds.Tables[0].Rows[0]["city"] != null && ds.Tables[0].Rows[0]["city"] != DBNull.Value)
                    {
                        sb.Append(ds.Tables[0].Rows[0]["city"]).Append(",");
                    }
                    if (ds.Tables[0].Rows[0]["CityName"] != null && ds.Tables[0].Rows[0]["CityName"] != DBNull.Value)
                    {
                        sb.Append(ds.Tables[0].Rows[0]["CityName"]).Append(",");
                    }
                    if (ds.Tables[0].Rows[0]["CountryName"] != null && ds.Tables[0].Rows[0]["CountryName"] != DBNull.Value)
                    {
                        sb.Append(ds.Tables[0].Rows[0]["CountryName"]).Append(",");
                    }
                    return sb.ToString().TrimEnd(',');

                }
                else
                {
                    return "";
                }

            }
            
            //Get lookup display column name for ID 
            if (_poInfo != null && _poInfo.getAD_Table_ID() == po.Get_Table_ID() && _poInfo.IsColumnLookup(index) && value != null)
            {
                //VLookUpInfo lookup = _poInfo.GetColumnLookupInfo(index); //create lookup info for column
                VLookUpInfo lookup = Common.Common.GetColumnLookupInfo(GetCtx(), _poInfo.GetColumnInfo(index)); //create lookup info for column
                DataSet ds = DB.ExecuteDataset(lookup.queryDirect.Replace("@key", DB.TO_STRING(value.ToString())), null); //Get Name from data

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    value = ds.Tables[0].Rows[0][2]; //Name Value
                }
            }

            

            if (column.GetAD_Reference_ID() == DisplayType.Date)
            {
                return Util.GetValueOfDateTime(value).Value.Date.ToShortDateString();
            }
            return value.ToString();
        }

        public void SetBPartner(int C_BPartner_ID)
        {
            _bpartner = new X_C_BPartner(GetCtx(), C_BPartner_ID, Get_TrxName());
        }

        public void SetBPartner(X_C_BPartner bpartner)
        {
            _bpartner = bpartner;
        }

        public void SetBPartner(PO bpartner)
        {
            int index = bpartner.Get_ColumnIndex("C_BPartner_ID");
            if (index > 0)
            {
                Object oo = bpartner.Get_Value(index);
                if (oo != null && oo.GetType() == typeof(int))
                {
                    int C_BPartner_ID = int.Parse(oo.ToString());
                    SetBPartner(C_BPartner_ID);
                }
            }
        }
        /// <summary>
        /// Set PO for parse
        /// </summary>
        /// <param name="po">po</param>
        public void SetPO(PO po)
        {
            _po = po;
            _poInfo = POInfo.GetPOInfo(p_ctx, po.Get_Table_ID());
        }

        /// <summary>
        /// Set PO for parse
        /// </summary>
        /// <param name="po">po</param>
        /// <param name="analyse">analyse if set to true, search for BPartner/User</param>
        public void SetPO(PO po, bool analyse)
        {
            _po = po;
            if (analyse)
            {
                int index = po.Get_ColumnIndex("C_BPartner_ID");
                if (index > 0)
                {
                    Object oo = po.Get_Value(index);
                    if (oo != null && oo.GetType() == typeof(int))
                    {
                        int C_BPartner_ID = int.Parse(oo.ToString());
                        SetBPartner(C_BPartner_ID);
                    }
                }
                index = po.Get_ColumnIndex("AD_User_ID");
                if (index > 0)
                {
                    Object oo = po.Get_Value(index);
                    if (oo != null && oo.GetType() == typeof(int))
                    {
                        int AD_User_ID = int.Parse(oo.ToString());
                        SetUser(AD_User_ID);
                    }
                }
            }
            _poInfo = POInfo.GetPOInfo(p_ctx, po.Get_Table_ID());
        }

        /// <summary>
        /// Set User for parse
        /// </summary>
        /// <param name="AD_User_ID">user</param>
        public void SetUser(int AD_User_ID)
        {
            _user = MUser.Get(GetCtx(), AD_User_ID);
        }

        /// <summary>
        /// Set User for parse
        /// </summary>
        /// <param name="user">user</param>
        public void SetUser(MUser user)
        {
            _user = user;
        }

        /// <summary>
        /// Translate to BPartner Language
        /// </summary>
        private void Translate()
        {
            string language = "";
            if (_bpartner != null && _bpartner.GetAD_Language() != null)
            {
                language = _bpartner.GetAD_Language();
            }
            else if (!Env.IsBaseLanguage(GetCtx().GetAD_Language(), ""))
            {
                language = GetCtx().GetAD_Language();
            }

            if (!string.IsNullOrEmpty(language) && language.Length > 0)
            {
                String key = language + Get_ID();
                MMailTextTrl trl = cacheTrl[key];
                if (trl == null)
                {
                    trl = GetTranslation(language);
                    if (trl != null)
                        cacheTrl.Add(key, trl);
                }
                if (trl != null)
                {
                    _mailHeader = trl.mailHeader;
                    _mailText = trl.mailText;
                    _mailText2 = trl.mailText2;
                    _mailText3 = trl.mailText3;
                    return;
                }
            }

            //	No Translation
            _mailHeader = base.GetMailHeader();
            _mailText = base.GetMailText();
            _mailText2 = base.GetMailText2();
            _mailText3 = base.GetMailText3();
        }

    }

    class MMailTextTrl
    {
        /** Language			*/
        public String AD_Language = null;
        /** Translated Header	*/
        public String mailHeader = null;
        /** Translated Text		*/
        public String mailText = null;
        /** Translated Text 2	*/
        public String mailText2 = null;
        /** Translated Text 3	*/
        public String mailText3 = null;
    }
}
