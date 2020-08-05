using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using VAdvantage.DataBase;
using VAdvantage.Model;

namespace VAdvantage.Utility
{
    public class ExternalWebMethod
    {
        /// <summary>
        /// Call external web service
        /// </summary>
        /// <param name="url">URL of the web service</param>
        /// <param name="ewsData">External web service data</param>
        /// <returns>String, response</returns>
        public string CallWebService(string url, ExtWebServiceData ewsData)
        {
            string result;
            
            try
            {
                // Create a byte array of the data we want to send
                byte[] byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ewsData));

                // Create the web request
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Timeout = 200000;
                request.ContentLength = byteData.Length;
                request.ContentType = "application/x-www-form-urlencoded";
                
                // Write data to request
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(byteData, 0, byteData.Length);
                }

                // Get response and return it
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    result = reader.ReadToEnd();
                    reader.Close();

                    XDocument xDoc = XDocument.Parse(result);
                    result = xDoc.Root.Value;
                }
            }
            catch (Exception e)
            {
                result = e.InnerException.Message;
            }
            return result;
        }


        // Do not delete
        //public string CallWebService(MClientInfo cInfo, string method, byte[] byteData)
        //{
        //    string result = "";

        //    string URL_ADDRESS = Path.Combine(cInfo.GetAD_WebServiceURL(), method);
        //    //string finalData = "Token=" + cInfo.GetAD_WebServiceToken() + "&Data=" + data;

        //    // Get response and return it
        //    try
        //    {
        //        // Create the web request
        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL_ADDRESS);
        //        request.Method = "POST";
        //        request.Timeout = 200000;
        //        //request.ContentLength = byteData.Length;
        //        request.ContentType = "application/x-www-form-urlencoded";

        //        // Write data to request
        //        using (Stream postStream = request.GetRequestStream())
        //        {
        //            postStream.Write(byteData, 0, byteData.Length);
        //        }

        //        using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
        //        {
        //            StreamReader reader = new StreamReader(response.GetResponseStream());
        //            result = reader.ReadToEnd();
        //            reader.Close();

        //            XDocument xDoc = XDocument.Parse(result);
        //            return xDoc.Root.Value;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        result = e.InnerException.Message;
        //    }
        //    return result;
        //}

        // Do not delete
        //public static void HttpUploadFile(string url, string file, string paramName, string contentType, NameValueCollection nvc)
        //{
        //    string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
        //    byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

        //    HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
        //    wr.ContentType = "multipart/form-data; boundary=" + boundary;
        //    wr.Method = "POST";
        //    wr.KeepAlive = true;
        //    wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

        //    Stream rs = wr.GetRequestStream();

        //    string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
        //    foreach (string key in nvc.Keys)
        //    {
        //        rs.Write(boundarybytes, 0, boundarybytes.Length);
        //        string formitem = string.Format(formdataTemplate, key, nvc[key]);
        //        byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
        //        rs.Write(formitembytes, 0, formitembytes.Length);
        //    }
        //    rs.Write(boundarybytes, 0, boundarybytes.Length);

        //    string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
        //    string header = string.Format(headerTemplate, paramName, file, contentType);
        //    byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
        //    rs.Write(headerbytes, 0, headerbytes.Length);

        //    FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
        //    byte[] buffer = new byte[4096];
        //    int bytesRead = 0;
        //    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
        //    {
        //        rs.Write(buffer, 0, bytesRead);
        //    }
        //    fileStream.Close();

        //    byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
        //    rs.Write(trailer, 0, trailer.Length);
        //    rs.Close();
        //    rs = null;

        //    WebResponse wresp = null;
        //    try
        //    {
        //        wresp = wr.GetResponse();
        //        Stream stream2 = wresp.GetResponseStream();
        //        StreamReader reader2 = new StreamReader(stream2);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (wresp != null)
        //        {
        //            wresp.Close();
        //            wresp = null;
        //        }
        //        wr = null;
        //    }
        //}
    }

    public class ExtWebServiceData
    {
        public string Token { get; set; }
        public ExtDocumentData DocumentData { get; set; }
        public ExtDocumentMetaData MetaData { get; set; }
        public ExtDocumentFolder FolderData { get; set; }
    }
    public class ExtDocumentData
    {
        public string DocumentName { get; set; }
        public long Size { get; set; }
        public string DocumentBytes { get; set; }
        public string URI { get; set; }
    }
    public class ExtDocumentMetaData
    {
        public string KeyWord { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }

        public int DocumentCategoryID { get; set; }
        public string DocumentCategoryName { get; set; }

        public int AttributeSetID { get; set; }
        public int AttributeSetInstanceID { get; set; }
        public List<KeyNamePair> MetaData { get; set; }
    }
    public class ExtDocumentFolder
    {
        public int FolderID { get; set; }
        public int ParentFolderID { get; set; }
        public string FolderPath { get; set; }
    }
}