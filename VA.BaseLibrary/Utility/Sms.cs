using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.Logging;

namespace VAdvantage.Utility
{

    /// <summary>
    /// Class to send email
    /// </summary>
    [Serializable]
    public class Sms
    {
        #region Private Variables
        string _msgRespons = "";
        string _phoneNumbers = "";
        string _messageText = "";
        StringBuilder strRep = new StringBuilder("");
        private static VLogger _log = VLogger.GetVLogger(typeof(Sms).FullName);

        #endregion

        /// <summary>
        /// Base constructor for sms
        /// </summary>
        /// <param name="mobileno"></param>
        /// <param name="smsText"></param>
        public Sms(string mobileno, string smsText)
        {
            _phoneNumbers = mobileno;
            _messageText = smsText;
        }

        /// <summary>
        /// Send Sms
        /// </summary>
        /// <returns>OK or error message</returns>
        public string Send()
        {
            string strUrl = "";
            //get value from Sms config table
            IDataReader idr = null;
            idr = DataBase.DB.ExecuteReader("select * from ad_smsconfiguration WHERE isactive='Y'");
            //DataTable dt = new DataTable();
            //dt.Load(idr);
            //idr.Close();

            //if (dt.Rows.Count > 0)
            if (idr.Read())
            {
                try
                {
                    #region static massege

                    strRep = new StringBuilder(Replace(_messageText));
                    strUrl = idr["url"].ToString() + "?" + idr["userKeyword"].ToString() + "=" +
                      idr["username"].ToString() + "&" + idr["PasswordKeyword"].ToString() + "=" +
                      idr["password"].ToString() + "&" +
                      idr["senderKeyword"].ToString() + "=" + idr["sender"].ToString() + "&" +
                      idr["messageKeyword"].ToString() + "=" +
                      strRep + "&" + idr["MobilenumberKeyword"].ToString() + "=";

                    strUrl += _phoneNumbers + "&";// phoneNumbers + "&";

                    strUrl += idr["priorityKeyword"].ToString() + "=" + idr["priorityValue"].ToString();
                    if (idr["unicodeValue"].ToString().Length > 0 || idr["dndValue"].ToString().Length > 0)
                    {
                        strUrl += "&" + idr["dndKeyword"].ToString() + "=" + idr["dndValue"].ToString() +
                            "&" + idr["unicodeKeyword"].ToString() + "=" + idr["unicodeValue"].ToString();
                    }
                    try
                    {
                        HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(strUrl);
                        HttpWebResponse resp = (HttpWebResponse)webReq.GetResponse();//send sms
                        StreamReader responseReader = new StreamReader(resp.GetResponseStream());//read the response 
                        String resultmsg = responseReader.ReadToEnd();//get result
                        _msgRespons = resultmsg;
                        responseReader.Close();
                        _log.SaveInfo(resultmsg.ToString(), strUrl.ToString());
                        resp.Close();
                    }
                    catch (Exception ex)
                    {
                        _msgRespons = "Error";
                        _log.SaveWarning(ex.ToString(), "");

                    }

                    #endregion

                    #region static massege

                    //strRep = new StringBuilder(Replace(_messageText));
                    //strUrl = dt.Rows[0]["url"].ToString() + "?" + dt.Rows[0]["userKeyword"].ToString() + "=" +
                    //  dt.Rows[0]["username"].ToString() + "&" + dt.Rows[0]["PasswordKeyword"].ToString() + "=" +
                    //  dt.Rows[0]["password"].ToString() + "&" +
                    //  dt.Rows[0]["senderKeyword"].ToString() + "=" + dt.Rows[0]["sender"].ToString() + "&" +
                    //  dt.Rows[0]["messageKeyword"].ToString() + "=" +
                    //  strRep + "&" + dt.Rows[0]["MobilenumberKeyword"].ToString() + "=";

                    //strUrl += _phoneNumbers + "&";// phoneNumbers + "&";

                    //strUrl += dt.Rows[0]["priorityKeyword"].ToString() + "=" + dt.Rows[0]["priorityValue"].ToString();
                    //if (dt.Rows[0]["unicodeValue"].ToString().Length > 0 || dt.Rows[0]["dndValue"].ToString().Length > 0)
                    //{
                    //    strUrl += "&" + dt.Rows[0]["dndKeyword"].ToString() + "=" + dt.Rows[0]["dndValue"].ToString() +
                    //        "&" + dt.Rows[0]["unicodeKeyword"].ToString() + "=" + dt.Rows[0]["unicodeValue"].ToString();
                    //}
                    //try
                    //{
                    //    HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(strUrl);
                    //    HttpWebResponse resp = (HttpWebResponse)webReq.GetResponse();//send sms
                    //    StreamReader responseReader = new StreamReader(resp.GetResponseStream());//read the response 
                    //    String resultmsg = responseReader.ReadToEnd();//get result
                    //    _msgRespons = resultmsg;
                    //    responseReader.Close();
                    //    _log.SaveInfo(resultmsg.ToString(), strUrl.ToString());
                    //    resp.Close();
                    //}
                    //catch (Exception ex)
                    //{
                    //    _msgRespons = "Error";
                    //    _log.SaveWarning(ex.ToString(), "");

                    //}

                    #endregion
                }
                catch (Exception ex)
                {
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                    _log.SaveWarning(ex.ToString(), "");
                    return "Errror";
                }

            }
            if (idr != null)
            {
                idr.Close();
                idr = null;
            }

            return _msgRespons;
        }

        /// <summary>
        /// Replace Special charactor from URL
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string Replace(string str)
        {
            StringBuilder replaceSpecilChar = new StringBuilder();
            replaceSpecilChar.Append(str);
            // replaceSpecilChar.Replace("%", "%25");
            replaceSpecilChar.Replace("#", "%23");
            replaceSpecilChar.Replace("$", "%24");
            replaceSpecilChar.Replace("&", "%26");
            replaceSpecilChar.Replace("+", "%2B");
            replaceSpecilChar.Replace(",", "%2C");
            replaceSpecilChar.Replace(":", "%3A");
            replaceSpecilChar.Replace(";", "%3B");
            replaceSpecilChar.Replace("=", "%3D");
            replaceSpecilChar.Replace("?", "%3F");
            replaceSpecilChar.Replace("@", "%40");
            replaceSpecilChar.Replace("<", "%3C");
            replaceSpecilChar.Replace(">", "%3E");
            replaceSpecilChar.Replace("{", "%7B");
            replaceSpecilChar.Replace("}", "%7D");
            replaceSpecilChar.Replace("|", "%7C");
            replaceSpecilChar.Replace("\\", "%5C");
            replaceSpecilChar.Replace("^", "%5E");
            replaceSpecilChar.Replace("~", "%7E");
            replaceSpecilChar.Replace("[", "%5B");
            replaceSpecilChar.Replace("]", "%5D");
            replaceSpecilChar.Replace("`", "%60");

            return replaceSpecilChar.ToString();
        }
    }
}
