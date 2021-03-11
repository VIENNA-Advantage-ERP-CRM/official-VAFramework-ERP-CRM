using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Xml.Linq;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;

namespace VAdvantage.Utility
{
    public class ExternalWebMethod
    {
        public const string GRANT_TYPE_PASSWORD = "password";
        public const string GRANT_TYPE_ACESSKEY = "accesskey";

        public const string AUTHTOKEN_PASSWORD = "viennapwd:viennapwd";
        public const string AUTHTOKEN_ACCESSKEY = "VIENNA:accesskey";

        public const string SCOPE_VADMS = "vadms openid";

        public const string WEB_USRENAME = "IdeasIncAdmin";
        public const string WEB_PASSWORD = "IdeasIncAdmin@123";

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

        /// <summary>
        /// Call Api Async with save document ref
        /// </summary>
        /// <param name="URL">URL of Api</param>
        /// <param name="AuthToken">Token for api</param>
        /// <param name="Values">Values for data</param>
        /// <param name="DocumentData">document byte data</param>
        /// <param name="ctx">Current context</param>
        /// <param name="trx">Current transaction</param>
        /// <param name="AD_AttachmentLine_ID">Ad attachment line id</param>
        /// <param name="log">log object</param>
        /// <returns>bool, true false of work done</returns>
        public async System.Threading.Tasks.Task<bool> SaveFileApiAsync(string URL, string AuthToken, Dictionary<string, string> Values, string DocumentData,
            Ctx ctx, Trx trx, int AD_AttachmentLine_ID, VLogger log)
        {
            // Call IDP server for token
            using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) })
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", AuthToken);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

                var builder = new UriBuilder(new Uri(URL));

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, builder.Uri);
                //request.Content = new StringContent(myContent, Encoding.UTF8, "application/x-www-form-urlencoded");//CONTENT-TYPE header
                request.Content = new FormUrlEncodedContent(Values);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                string result = await response.Content.ReadAsStringAsync();

                APITokenResponse apires = JsonConvert.DeserializeObject<APITokenResponse>(result);

                // Call Handler with token

                MAttachmentReference attRef = new MAttachmentReference(ctx, 0, trx);

                attRef.SetAD_AttachmentLine_ID(AD_AttachmentLine_ID);
                attRef.SetAD_AttachmentRef("IDP");
                if (!attRef.Save(trx))
                {
                    log.Severe("MAttachmentReference not saved " + VLogger.RetrieveError().Name);
                    return false;
                }
                return true;
            };


            //// Use this Async call for calling this function
            //System.Threading.Tasks.Task<bool> result = new ExternalWebMethod().SaveFileApiAsync(
            //    cInfo.GetAD_WebServiceURL(), 
            //    cInfo.GetAD_WebServiceToken(), 
            //    apiValues, 
            //    Newtonsoft.Json.JsonConvert.SerializeObject(DocumentData),
            //    GetCtx(), Get_Trx(), AD_AttachmentLine_ID, log);
            //return result.Result;
        }

        /// <summary>
        /// Save file using API
        /// </summary>
        /// <param name="ctx">Current Context</param>
        /// <param name="IDPServerURL">IDP server url</param>
        /// <param name="IDPServerClient">IDP server grant type</param>
        /// <param name="WebServiceURL">Web service url</param>
        /// <param name="WebServiceToken">Web service token</param>
        /// <param name="DocumentData">Document data for posting</param>
        /// <returns>String, URI or result</returns>
        public string SaveFileApi(Ctx ctx, string IDPServerURL, string IDPServerClient, string WebServiceURL, string WebServiceToken, ExtApiDocumentData DocumentData)
        {
            // Call IDP server for token

            APITokenResponse apires = GenerateBasicToken(ctx, IDPServerURL, IDPServerClient, WebServiceURL, WebServiceToken);
            APIFileResponse apifileres = new APIFileResponse();

            //// Test Call external web service methods and file info to it (Internal Testing)
            //ExtWebServiceData ewsData = new ExtWebServiceData();
            //ewsData.Token = WebServiceToken;
            //ewsData.DocumentData = new ExtDocumentData()
            //{
            //    DocumentName = DocumentData.filename,
            //    Size = DocumentData.fileBytes.Length,
            //    DocumentBytes = DocumentData.fileBytes
            //};
            //return CallWebService(WebServiceURL + "/StoreDocument", ewsData);


            // Call Handler with apires.access_token and DocumentData after searlization using Newtonsoft.Json.JsonConvert.SerializeObject(DocumentData)

            byte[] bytevalue = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(DocumentData));

            WebRequest request = WebRequest.Create(WebServiceURL);
            request.Method = "POST"; // or "GET", "PUT", "PATCH", "DELETE", etc.
            request.Headers.Add("Authorization", "Bearer " + apires.access_token);
            //request.Headers.Add("Accept", "*/*");
            request.ContentType = "application/json";
            request.ContentLength = bytevalue.Length;

            // Create request stream
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(bytevalue, 0, bytevalue.Length);

                // Get http web response
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // Read response stream
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        // Get a reader capable of reading the response stream
                        using (StreamReader myStreamReader = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            // Read stream content as string
                            string responseJSON = myStreamReader.ReadToEnd();

                            // Assuming the response is in JSON format, deserialize it, creating an instance of APIFileResponse type (generic type declared before).
                            apifileres = JsonConvert.DeserializeObject<APIFileResponse>(responseJSON);
                        }
                    }
                }
            }

            return apifileres.data;
        }

        /// <summary>
        /// Get file using API
        /// </summary>
        /// <param name="ctx">Current Context</param>
        /// <param name="IDPServerURL">IDP server url</param>
        /// <param name="IDPServerClient">IDP server grant type</param>
        /// <param name="WebServiceURL">Web service url</param>
        /// <param name="WebServiceToken">Web service token</param>
        /// <param name="DocumentData">Document data for posting</param>
        /// <returns>String, Document bytes in string or result</returns>
        public string GetFileApi(Ctx ctx, string IDPServerURL, string IDPServerClient, string WebServiceURL, string WebServiceToken, ExtApiDocumentData DocumentData)
        {
            // Call IDP server for token
            APITokenResponse apires = GenerateBasicToken(ctx, IDPServerURL, IDPServerClient, WebServiceURL, WebServiceToken);
            APIFileResponse apifileres = new APIFileResponse();

            //// Test Call external web service methods and file info to it (Internal Testing)
            //ExtWebServiceData ewsData = new ExtWebServiceData();
            //ewsData.Token = WebServiceToken;
            //ewsData.DocumentData = new ExtDocumentData()
            //{
            //    DocumentName = DocumentData.filename,
            //    Size = 0,
            //    DocumentBytes = null
            //};
            //return CallWebService(WebServiceURL + "/GetDocument", ewsData);

            // Call Handler with apires.access_token and Document URI

            byte[] bytevalue = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(DocumentData));

            WebRequest request = WebRequest.Create(WebServiceURL.TrimEnd('/') + "/" + DocumentData.documentUri + "/");
            request.Method = "GET"; // or "GET", "PUT", "PATCH", "DELETE", etc.
            request.Headers.Add("Authorization", "Bearer " + apires.access_token);
            //request.Headers.Add("Accept", "*/*");
            request.ContentType = "application/json";
            //request.ContentLength = bytevalue.Length;

            // Create request stream
            //using (Stream stream = request.GetRequestStream())
            //{
            //    stream.Write(bytevalue, 0, bytevalue.Length);

            // Get http web response
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                // Read response stream
                using (Stream responseStream = response.GetResponseStream())
                {
                    // Get a reader capable of reading the response stream
                    using (StreamReader myStreamReader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        // Read stream content as string
                        string responseJSON = myStreamReader.ReadToEnd();

                        // Assuming the response is in JSON format, deserialize it, creating an instance of APIFileResponse type (generic type declared before).
                        apifileres = JsonConvert.DeserializeObject<APIFileResponse>(responseJSON);
                    }
                }
            }
            //}

            return apifileres.data;
        }

        /// <summary>
        /// Get JWT token
        /// </summary>
        /// <param name="ctx">Current Context</param>
        /// <param name="IDPServerURL">IDP server url</param>
        /// <param name="IDPServerClient">IDP server grant type</param>
        /// <param name="WebServiceURL">Web service url</param>
        /// <param name="WebServiceToken">Web service token</param>
        /// <returns>Object, response from IDP server</returns>
        public APITokenResponse GenerateBasicToken(Ctx ctx, string IDPServerURL, string IDPServerClient, string WebServiceURL, string WebServiceToken)
        {
            APITokenResponse apires = new APITokenResponse();

            Dictionary<string, string> apiValues = new Dictionary<string, string>();
            string authToken = "";

            if (IDPServerClient == X_AD_ClientInfo.AD_IDPSERVERCLIENT_UserNamePassword)
            {
                apiValues = new Dictionary<string, string>
                {
                    { "grant_type", ExternalWebMethod.GRANT_TYPE_PASSWORD },
                    { "username", ExternalWebMethod.WEB_USRENAME },
                    { "password", ExternalWebMethod.WEB_PASSWORD },
                    { "scope", ExternalWebMethod.SCOPE_VADMS },
                    { "webservicetoken", WebServiceToken },
                    { "AD_Client_ID", ctx.GetAD_Client_ID().ToString() },
                    { "AD_Role_ID", ctx.GetAD_Client_ID().ToString() },
                    { "AD_Org_ID", ctx.GetAD_Org_ID().ToString() },
                    { "AD_User_ID", ctx.GetAD_User_ID().ToString() }
                };

                authToken = ExternalWebMethod.AUTHTOKEN_PASSWORD;
            }

            if (IDPServerClient == X_AD_ClientInfo.AD_IDPSERVERCLIENT_AccessKey)
            {
                apiValues = new Dictionary<string, string>
                {
                    { "grant_type", ExternalWebMethod.GRANT_TYPE_ACESSKEY },
                    { "accesskey", SecureEngine.Encrypt(System.Web.Configuration.WebConfigurationManager.AppSettings["accesskey"].ToString()) },
                    { "scope", ExternalWebMethod.SCOPE_VADMS },
                    { "webservicetoken", WebServiceToken },
                    { "AD_Client_ID", ctx.GetAD_Client_ID().ToString() },
                    { "AD_Role_ID", ctx.GetAD_Client_ID().ToString() },
                    { "AD_Org_ID", ctx.GetAD_Org_ID().ToString() },
                    { "AD_User_ID", ctx.GetAD_User_ID().ToString() }
                };

                authToken = ExternalWebMethod.AUTHTOKEN_ACCESSKEY;
            }

            string g = String.Join("&", apiValues.Select(x => HttpUtility.UrlEncode(x.Key) + "=" + HttpUtility.UrlEncode(x.Value)));
            byte[] bytevalue = Encoding.UTF8.GetBytes(g);

            WebRequest request = WebRequest.Create(IDPServerURL.TrimEnd('/') + "/auth/connect/token");
            request.Method = "POST"; // or "GET", "PUT", "PATCH", "DELETE", etc.
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(authToken)));
            //request.Headers.Add("Accept", "*/*");
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytevalue.Length;

            // Create request stream
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(bytevalue, 0, bytevalue.Length);

                // Get http web response
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // Read response stream
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        // Get a reader capable of reading the response stream
                        using (StreamReader myStreamReader = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            // Read stream content as string
                            string responseJSON = myStreamReader.ReadToEnd();

                            // Assuming the response is in JSON format, deserialize it, creating an instance of APITokenResponse type (generic type declared before).
                            apires = JsonConvert.DeserializeObject<APITokenResponse>(responseJSON);
                        }
                    }
                }
            }

            return apires;
        }

        /// <summary>
        /// Get JWT token (Do not use for now)
        /// </summary>
        /// <param name="IDPURL">IDP server url</param>
        /// <param name="AuthToken">Authorization token</param>
        /// <param name="ApiValues">Values to include in post</param>
        /// <returns>Object, response from IDP server</returns>
        public APITokenResponse GenerateBasicTokenold(string IDPURL, string AuthToken, Dictionary<string, string> ApiValues)
        {
            APITokenResponse apires = new APITokenResponse();

            string g = String.Join("&", ApiValues.Select(x => HttpUtility.UrlEncode(x.Key) + "=" + HttpUtility.UrlEncode(x.Value)));
            byte[] bytevalue = Encoding.UTF8.GetBytes(g);

            WebRequest request = WebRequest.Create(Path.Combine(IDPURL, "/auth/connect/token"));
            request.Method = "POST"; // or "GET", "PUT", "PATCH", "DELETE", etc.
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(AuthToken)));
            //request.Headers.Add("Accept", "*/*");
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytevalue.Length;

            // Create request stream
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(bytevalue, 0, bytevalue.Length);

                // Get http web response
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // Read response stream
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        // Get a reader capable of reading the response stream
                        using (StreamReader myStreamReader = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            // Read stream content as string
                            string responseJSON = myStreamReader.ReadToEnd();

                            // Assuming the response is in JSON format, deserialize it, creating an instance of APITokenResponse type (generic type declared before).
                            apires = JsonConvert.DeserializeObject<APITokenResponse>(responseJSON);
                        }
                    }
                }
            }

            return apires;
        }

    }

    // For web service (soap)
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

    // For API (rest)
    public class ExtApiDocumentData
    {
        public string filename { get; set; }
        public string fileBytes { get; set; }
        public string fileExtension { get; set; }
        public string documentUri { get; set; }
    }
    public class APITokenResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
    }
    public class APIFileResponse
    {
        public string data { get; set; }
        public string[] errors { get; set; }
        public bool success { get; set; }
        public string[] validations { get; set; }
    }
}