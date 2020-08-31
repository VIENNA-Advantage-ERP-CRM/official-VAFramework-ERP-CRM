using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Data;
using MarketSvc.MService;
using System.Data.OracleClient;
using System.ServiceModel.Activation;
using MarketSvc.Classes;
using System.Net.NetworkInformation;
using System.IO;

namespace MarketSvc
{
    [ServiceContract(Namespace = "")]
    public interface IMarketServiceBasic
    {
        [OperationContract]
        InitLoginInfo InitLogin(InitLoginInfo InfoIn,string authority);

        [OperationContract]
        MInfo GetModules(string sql, int pageIndex, int pageSize, string modType, out CustomException customError);

        [OperationContract]
        MService.ModuleContainer GetMarketModules(MService.ModuleContainer mcIn, out CustomException customError);

        [OperationContract]
        string UpdateSubscription(int AD_ModuleInfo_ID, int M_Product_ID, string key, bool isFree, string versionNo);

        [OperationContract]
        string InitKeyEdition(string url);

        [OperationContract]
        string DownloadAndExtractModuleFiles(int AD_ModuleInfo_ID);

        [OperationContract]
        MService.MarketModuleInfo GetModuleInfo(int AD_moduleInfo_ID, string tokenKey);

    }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class MarketServiceBasic : IMarketServiceBasic
    {
        private string connectionString = System.Configuration.ConfigurationManager.AppSettings["oracleConnectionString"];
        #region Key Validation

        /// <summary>
        /// Valiadte login key
        /// </summary>
        /// <param name="infoIn"></param>
        /// <returns></returns>
        public InitLoginInfo InitLogin(InitLoginInfo infoIn, string authority)
        {
            InitLoginInfo info = InitLoginInfo.InitLogin(infoIn, authority);
            return info;
        }

        #endregion

        #region Market

        /// <summary>
        /// Modules
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="isLocal"></param>
        /// <param name="customError"></param>
        /// <returns></returns>
        public MInfo GetModules(string sql, int pageIndex, int pageSize, string modType, out CustomException customError)
        {
            MInfo mInfo = new MInfo();
            mInfo.IsProfessionalEdition = EvaluationCheck.IsProfessionalEdition();
            mInfo.TokenKey = EvaluationCheck.GetTokenKey();
            mInfo.IsRegistered = !String.IsNullOrEmpty(mInfo.TokenKey);
            mInfo.IsKeyExpired = EvaluationCheck.IsTokenKeyExpired();
            customError = null;
            // if (!isMy)
            // {
            var client = ServiceEndPoint.GetMServiceClient();
            if (client != null)
            {
                string error = null;
                System.Net.ServicePointManager.Expect100Continue = false;
                mInfo.lstMarketModuleInfo = client.GetModules(sql, pageIndex, pageSize, modType, mInfo.TokenKey, out error);
                client.Close();
                if (error != null)
                {
                    customError = new CustomException(new Exception(error));
                    return null;
                }
            }
            return mInfo;
            //}
            //else //Get From Local
            //{
            //    //return null;
            //    // Get List of Bought module
            //    //list1

            //    mInfo.lstMarketModuleInfo = GetLocalModules(sql, pageIndex, pageSize, out customError);
            //    return mInfo;
            //}
        }

        private List<MService.MarketModuleInfo> GetLocalModules(string sql, int stratIndex, int pageSize, out CustomException customError)
        {
            DataSet ds = null;
            List<MarketModuleInfo> lstModuleInfo = null;
            customError = null;
#pragma warning disable 612, 618
            OracleConnection Connection = new OracleConnection(connectionString);
            try
            {
                string sqlScalar = "SELECT COUNT(*) " + sql.Substring(sql.IndexOf("FROM"));
                sql = GetSelectSql(sql);
                int totalcount = Convert.ToInt32(VAdvantage.DataBase.DB.ExecuteScalar(sqlScalar));
                OracleDataAdapter adapter = new OracleDataAdapter();
                adapter.SelectCommand = new OracleCommand(sql);
                Connection.Open();
                adapter.SelectCommand.Connection = Connection;
                ds = new DataSet();
                adapter.Fill(ds, stratIndex, pageSize, "Table");
                DataTable dataTable = null;
                if (ds.Tables.Count < 1)
                {
                    return null;
                }
                else
                {
                    dataTable = ds.Tables[0];
                }
                lstModuleInfo = new List<MarketModuleInfo>(dataTable.Rows.Count);
                MarketModuleInfo mInfo = null;
                foreach (DataRow dr in dataTable.Rows)
                {
                    mInfo = new MarketModuleInfo();
                    mInfo.TotalCount = totalcount;
                    //int col = 0;
                    mInfo.ModuleEdition = "N";

                    mInfo.AD_Module_ID = Convert.ToInt32(dr["ID"]);
                    mInfo.Name = dr["Name"].ToString();
                    mInfo.Prefix = dr["Prefix"].ToString();
                    mInfo.VersionID = VAdvantage.Utility.Util.GetValueOfInt(dr["VersionID"]);
                    mInfo.Author = dr["Author"].ToString();
                    mInfo.Image = dr.IsNull("BinaryData") ? null : (byte[])dr["BinaryData"];
                    mInfo.Rating = dr.IsNull("Rating") ? 6 : System.Convert.ToInt32(dr["Rating"]);
                    mInfo.Description = dr["Description"].ToString();
                    mInfo.ColNameValues = new Dictionary<string, object>();
                    for (int i = 7; i < dataTable.Columns.Count; i++)
                    {
                        mInfo.ColNameValues[dataTable.Columns[i].ColumnName.ToUpper()] = dr[i].ToString();
                    }

                    mInfo.Images = new List<object>();
                    mInfo.Videos = new List<object>();

                    //Get Video And Images

                    lstModuleInfo.Add(mInfo);
                }
            }
            catch (Exception err)
            {
                customError = new CustomException(err);
            }
            finally
            {
                Connection.Close();
            }
#pragma warning restore 612, 618
            return lstModuleInfo;

        }

        private string GetSelectSql(string sql)
        {
            String afterSelect = sql.Substring(sql.IndexOf("FROM"));

            string sqlSelect = " SELECT AD_ModuleInfo_ID as ID,Name as Name,Prefix as Prefix,VersionId as VersionID, Author as Author," +
                               " BinaryData as BinaryData,Rating as Rating, " + //FIx Mandatory Column
                               " Description as Description,Feature as Feature,VersionNo as VersionNo,InstallationInstruction as InstallationInstruction, " +
                               " DependencyInfo as DependencyInfo,UpgradeInfo as UpgradeInfo,URL as Url "; //Other Columns 
            return sqlSelect + afterSelect + " Order By Rating Desc ";
        }

        #endregion


        /// <summary>
        /// Get Module form cloud Server
        /// </summary>
        /// <remarks>Used for backward compatibility [till 1.0.1.14]</remarks>
        /// <param name="mInfoIn"></param>
        /// <param name="customError"></param>
        /// <returns>/// <returns>MInfo Class has module list and its properties </returns></returns>
        public MInfo GetModules(MInfo mInfoIn, out CustomException customError)
        {
            MInfo mInfoOut = new MInfo();
            mInfoOut.IsProfessionalEdition = EvaluationCheck.IsProfessionalEdition();
            mInfoOut.TokenKey = EvaluationCheck.GetTokenKey();
            mInfoOut.IsRegistered = !String.IsNullOrEmpty(mInfoOut.TokenKey);
            mInfoOut.IsKeyExpired = EvaluationCheck.IsTokenKeyExpired();
            customError = null;


            var client = ServiceEndPoint.GetMServiceClient();
            if (client != null)
            {
                string error = null;
                System.Net.ServicePointManager.Expect100Continue = false;
                mInfoOut.lstMarketModuleInfo = client.GetModules(mInfoIn.SqlString, mInfoIn.PageIndex, mInfoIn.PageSize, mInfoIn.ModType, mInfoOut.TokenKey, out error);
                client.Close();
                if (error != null)
                {
                    customError = new CustomException(new Exception(error));
                    return null;
                }
            }
            return mInfoOut;

        }

        /// <summary>
        /// Get Module from Cloud Service [works for both (cloud or local host) ] 
        /// </summary>
        /// <param name="mcIn">input criteria</param>
        /// <param name="customError">error if any</param>
        /// <returns>container object conating modules </returns>
        public MService.ModuleContainer GetMarketModules(MService.ModuleContainer mcIn, out CustomException customError)
        {
            customError = null;

            customError = null;
            bool notify = false;
            if (mcIn != null && mcIn.SqlString != null)
            {
                notify = " AND Upper(Prefix) In ('MARKET_', 'VIS_') ".Equals(mcIn.SqlString);
            }

            if (notify && !ServiceEndPoint.CheckServerConnection())
            {
                customError = new CustomException(new Exception("Connection[Ping] failed[400]"));
                return null;
            }

            if (notify &&  !ServiceEndPoint.CheckServerConnection())
            {
                customError = new CustomException(new Exception("Connection[Ping] failed"));
                return null;
            }

            var client = ServiceEndPoint.GetMServiceClient();

            if (!mcIn.IsCloudMarket && (mcIn.ModType != "SPL" || !mcIn.IsProfessionalEdition)) //Local Server
            {
                mcIn.TokenKey = EvaluationCheck.GetTokenKey();
            }

            if (client != null)
            {
                //string error = null;


                //Requested by surya 
                //Live market also show unreleased module for testing purpose against the setting in web config file
                mcIn.IsRegistered = Utility.ShowUnReleasedModule();
                //IsRegistred is reused for reusability
                MService.ModuleContainer mcOut = null;
                try
                {
                    System.Net.ServicePointManager.Expect100Continue = false;
                    mcOut = client.GetModuleFromMarket(mcIn);

                }
                catch (Exception ex)
                {
                    var err = ex.Message;
                    if (mcOut != null)
                    {
                        err = mcOut.ErrorMsg;
                    }
                    customError = new CustomException(new Exception(err));
                    client.Close();
                    return null;
                }
                client.Close();

                if (mcOut.ErrorMsg != null)
                {
                    customError = new CustomException(new Exception(mcOut.ErrorMsg));
                    return null;
                }

                if (!mcIn.IsCloudMarket && (mcIn.ModType != "SPL" || !mcIn.IsProfessionalEdition)) //Local Server
                {
                    mcOut.IsProfessionalEdition = EvaluationCheck.IsProfessionalEdition();

                    mcOut.IsRegistered = !String.IsNullOrEmpty(mcOut.TokenKey);
                    mcOut.IsKeyExpired = EvaluationCheck.IsTokenKeyExpired();
                    mcOut.IsMarketExpired  = EvaluationCheck.IsMarketExpired();
                    
                }
                return mcOut;
            }
            return null;
        }


        /// <summary>
        /// Update Module Subscrption table on cloud Service
        /// </summary>
        /// <param name="AD_ModuleInfo_ID">module id</param>
        /// <param name="M_Product_ID">product id</param>
        /// <param name="key">token key</param>
        /// <param name="isFree">is free module</param>
        /// <param name="versionNo">version of module</param>
        /// <returns> succusfull or failure opration message </returns>
        public string UpdateSubscription(int AD_ModuleInfo_ID, int M_Product_ID, string key, bool isFree, string versionNo)
        {
            var client = ServiceEndPoint.GetMServiceClient();
            string msg = null;
            if (client != null)
            {
                if (string.IsNullOrEmpty(key))
                {
                    key = EvaluationCheck.GetTokenKey();
                }
                try
                {
                    msg = client.UpdateSubscription(AD_ModuleInfo_ID, M_Product_ID, key, isFree, versionNo, MarketSvc.Classes.Utility.GetAccesskey());
                }
                catch
                {

                }
                client.Close();
            }
            return msg;
        }

        public string InitKeyEdition(string url)
        {
            return EvaluationCheck.GetKeyEdition(url);
        }


        public string DownloadAndExtractModuleFiles(int AD_ModuleInfo_ID)
        {
            /* Market Service */

            StringBuilder sb = new StringBuilder();

            var client = ServiceEndPoint.GetMServiceClient();

            /* Call market service to get info data against module */
            //MarketSvc.MService.ModuleFolderInfo data = client.GetModuleData(AD_ModuleInfo_ID);
            System.Net.ServicePointManager.Expect100Continue = false;
            MarketSvc.MService.ModuleFolderInfo data = null;

            try
            {

                data = client.GetModuleData(AD_ModuleInfo_ID, MarketSvc.Classes.Utility.GetAccesskey());

                if (data.HasMoreChunk)
                {
                    MemoryStream stream = new MemoryStream();
                    //List<byte[]> fullData = new List<byte[]>();
                    //fullData.Add(data.Data);
                    int currentChunk = 0;
                    stream.Write(data.Data, 0, data.Data.Length);
                    for (int i = 0; i < data.TotalChunks; i++)
                    {
                        currentChunk++;
                        byte[] tempData = client.DownloadFileChunk(data.CacheKey, currentChunk, data.TotalChunks);
                        if (tempData == null)
                        {
                            sb.Clear();
                            sb.Append("Error=> While Downloading Module Files.");
                            break;
                        }
                        stream.Write(tempData, 0, tempData.Length);
                    }
                    data.Data = stream.ToArray();
                }

            }
            catch(Exception exx)
            {
                sb.Clear();
                sb.Append("Error=> "+ exx.Message);
            }
            if (client != null)
            {
                client.Close();
            }
            if (data == null)
            {
                sb.Clear();
                sb.Append("Error=> Operation aborted, Please try after some time");
            }
            else if (data.ISError)
            {
                sb.Clear();
                sb.Append("Error=>" + data.ErrorMessage);
            }
            else
            {

                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string fileTempPath = basePath + data.Prefix + DateTime.Now.Ticks;
                string filePath = basePath + data.Prefix;
                try
                {
                    System.IO.File.WriteAllBytes(fileTempPath, data.Data);
                    ICSharpCode.SharpZipLib.Zip.FastZip z = new ICSharpCode.SharpZipLib.Zip.FastZip();
                    z.ExtractZip(fileTempPath, basePath + data.Prefix, "");
                    System.IO.File.Delete(fileTempPath);

                    sb.Append("Folder extracted at " + filePath);

                    string moduleLocalPath = data.Prefix + "\\" + data.Name.Trim() + "_" + data.VersionNo;

                    sb.Append("$ModulePath$").Append(moduleLocalPath);
                }
                catch (Exception e)
                {
                    //callback.QueryExecuted(new CallBackDetail() { Status = "Error while extracting folder/data", Action = "Done" });
                    sb.Clear();
                    sb.Append("Error=> while extracting folder/data, " + e.Message);
                }
            }
            return sb.ToString();
        }


        public MService.MarketModuleInfo GetModuleInfo(int AD_ModuleInfo_ID, string tokenKey)
        {
            var client = ServiceEndPoint.GetMServiceClient();

            /* Call market service to get info data against module */
            //MarketSvc.MService.ModuleFolderInfo data = client.GetModuleData(AD_ModuleInfo_ID);
            System.Net.ServicePointManager.Expect100Continue = false;
            MarketSvc.MService.MarketModuleInfo data = null;
            try
            {
                data = client.GetMarketModuleInfoHTML(AD_ModuleInfo_ID, tokenKey, MarketSvc.Classes.Utility.GetAccesskey(),Classes.Utility.ShowUnReleasedModule());
                client.Close();
            }
            catch
            {
                if (client != null)
                    client.Close();
            }
            
            return data;
        }

        public List<MService.ModuleVersionInfo> GetModuleversionHostory(int AD_Module_ID, int pageNo)
        {
            var client = ServiceEndPoint.GetMServiceClient();
            List<MService.ModuleVersionInfo> lst = null;
            try
            {
                System.Net.ServicePointManager.Expect100Continue = false;
                lst=client.GetModuleVersionHostory(AD_Module_ID, pageNo, MarketSvc.Classes.Utility.GetAccesskey());
                     
            }
            catch (Exception ex)
            {
                var err = ex.Message;
                return null;
            }
            client.Close();
            return lst;
        }
    }
}
