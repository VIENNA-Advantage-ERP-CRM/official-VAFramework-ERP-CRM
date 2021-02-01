/********************************************************
 * Module Name    :  Email Test
 * Purpose        : Request Mail Template model.
 *	                 Cannot be cached as it holds PO/BPartner/User to parse
 * Class Used     : X_VAR_MailTemplate
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
    public class MMailText : X_VAR_MailTemplate
    {
        /**	Parse User			*/
        private MVAFUserContact _user = null;
        /** Parse BPartner		*/
        private X_VAB_BusinessPartner _bpartner = null;
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
        /// <param name="VAR_MailTemplate_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MMailText(Ctx ctx, int VAR_MailTemplate_ID, Trx trxName)
            : base(ctx, VAR_MailTemplate_ID, trxName)
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
        /// <param name="VAF_Language">language</param>
        /// <returns>trl</returns>
        private MMailTextTrl GetTranslation(String VAF_Language)
        {
            MMailTextTrl trl = null;
            String sql = "SELECT * FROM VAR_MailTemplate_TL WHERE VAR_MailTemplate_ID=@textid AND VAF_Language=@lang";
            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@textid", GetVAR_MailTemplate_ID());
                param[1] = new SqlParameter("@lang", VAF_Language);
                dr = CoreLibrary.DataBase.DB.ExecuteReader(sql, param);
                if (dr.Read())
                {
                    trl = new MMailTextTrl();
                    trl.VAF_Language = dr["VAF_Language"].ToString();
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
                    int id = po.GetVAF_Client_ID();
                    outStr.Append(DB.ExecuteScalar("Select Name FROM VAF_Client WHERE VAF_Client_ID=" + id));
                }
                else if (token == "Org")
                {
                    int id = po.GetVAF_Org_ID();
                    outStr.Append(DB.ExecuteScalar("Select Name FROM VAF_ORG WHERE VAF_ORG_ID=" + id));
                }
                else if (token == "BPName")
                {
                    if (po.Get_TableName() == "VAB_BusinessPartner")
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

            MVAFColumn column = (new MVAFTableView(GetCtx(), po.Get_Table_ID(), Get_Trx())).GetColumn(variable);
            if (column.GetVAF_Control_Ref_ID() == DisplayType.Location)
            {
                StringBuilder sb = new StringBuilder();
                DataSet ds = DB.ExecuteDataset(@"SELECT l.address1,
                                                          l.address2,
                                                          l.address3,
                                                          l.address4,
                                                          l.city,
                                                          CASE
                                                            WHEN l.VAB_City_ID IS NOT NULL
                                                            THEN
                                                              ( SELECT NAME FROM VAB_City ct WHERE ct.VAB_City_ID=l.VAB_City_ID
                                                              )
                                                            ELSE NULL
                                                          END CityName,
                                                          (SELECT NAME FROM VAB_Country c WHERE c.VAB_Country_ID=l.VAB_Country_ID
                                                          ) AS CountryName
                                                        FROM VAB_Address l WHERE l.VAB_Address_ID="+value);
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
            if (_poInfo != null && _poInfo.getVAF_TableView_ID() == po.Get_Table_ID() && _poInfo.IsColumnLookup(index) && value != null)
            {
                VLookUpInfo lookup = Common.Common.GetColumnLookupInfo(GetCtx(),_poInfo.GetColumnInfo(index)); //create lookup info for column
                DataSet ds = DB.ExecuteDataset(lookup.queryDirect.Replace("@key", DB.TO_STRING(value.ToString())), null); //Get Name from data

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    value = ds.Tables[0].Rows[0][2]; //Name Value
                }
            }

            

            if (column.GetVAF_Control_Ref_ID() == DisplayType.Date)
            {
                return Util.GetValueOfDateTime(value).Value.Date.ToShortDateString();
            }
            return value.ToString();
        }

        public void SetBPartner(int VAB_BusinessPartner_ID)
        {
            _bpartner = new X_VAB_BusinessPartner(GetCtx(), VAB_BusinessPartner_ID, Get_TrxName());
        }

        public void SetBPartner(X_VAB_BusinessPartner bpartner)
        {
            _bpartner = bpartner;
        }

        public void SetBPartner(PO bpartner)
        {
            int index = bpartner.Get_ColumnIndex("VAB_BusinessPartner_ID");
            if (index > 0)
            {
                Object oo = bpartner.Get_Value(index);
                if (oo != null && oo.GetType() == typeof(int))
                {
                    int VAB_BusinessPartner_ID = int.Parse(oo.ToString());
                    SetBPartner(VAB_BusinessPartner_ID);
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
                int index = po.Get_ColumnIndex("VAB_BusinessPartner_ID");
                if (index > 0)
                {
                    Object oo = po.Get_Value(index);
                    if (oo != null && oo.GetType() == typeof(int))
                    {
                        int VAB_BusinessPartner_ID = int.Parse(oo.ToString());
                        SetBPartner(VAB_BusinessPartner_ID);
                    }
                }
                index = po.Get_ColumnIndex("VAF_UserContact_ID");
                if (index > 0)
                {
                    Object oo = po.Get_Value(index);
                    if (oo != null && oo.GetType() == typeof(int))
                    {
                        int VAF_UserContact_ID = int.Parse(oo.ToString());
                        SetUser(VAF_UserContact_ID);
                    }
                }
            }
            _poInfo = POInfo.GetPOInfo(p_ctx, po.Get_Table_ID());
        }

        /// <summary>
        /// Set User for parse
        /// </summary>
        /// <param name="VAF_UserContact_ID">user</param>
        public void SetUser(int VAF_UserContact_ID)
        {
            _user = MVAFUserContact.Get(GetCtx(), VAF_UserContact_ID);
        }

        /// <summary>
        /// Set User for parse
        /// </summary>
        /// <param name="user">user</param>
        public void SetUser(MVAFUserContact user)
        {
            _user = user;
        }

        /// <summary>
        /// Translate to BPartner Language
        /// </summary>
        private void Translate()
        {
            string language = "";
            if (_bpartner != null && _bpartner.GetVAF_Language() != null)
            {
                language = _bpartner.GetVAF_Language();
            }
            else if (!Env.IsBaseLanguage(GetCtx().GetVAF_Language(), ""))
            {
                language = GetCtx().GetVAF_Language();
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
        public String VAF_Language = null;
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
