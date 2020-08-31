using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.ComponentModel;
using System.ServiceModel;
using System.Data;
using System.IO;
using System.Management;
using VAdvantage.Logging;

namespace MarketSvc
{

    internal sealed class EvaluationCheck
    {
        private static VLogger log = VLogger.GetVLogger(typeof(EvaluationCheck).FullName);
        private static string keyName = "VAdvantageDemo";
        //private static string keyNameOff = "VAdvantageDemoOff";

        private const int DEFAULT_USERS = 1;
        public static InitLoginInfo DemoCheck(InitLoginInfo key)
        {
            EvaluationCheck ev = new EvaluationCheck();

            return ev.CheckLicenceValues(key);
        }

        // vinay bhatt - check login key on cloud server and onffline for end date. Also optional offline 50 attempts, one month check work
        //check if a connection to the Internet can be established 
        public static bool IsConnectionAvailable()
        {
            try
            {
                System.Net.NetworkInformation.PingReply pingStatus = new System.Net.NetworkInformation.Ping().Send(System.Net.IPAddress.Parse("8.8.8.8"), 1000);

                if (pingStatus.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        private InitLoginInfo CheckLicenceValues(InitLoginInfo info)
        {
            // String keyName = info.Url;
            // String keyName = "VAdvantageDemo";
            InitLoginInfo aHelper = new InitLoginInfo();
            string regValue = "";
            //string regValueOff = "";

            try
            {
                regValue = SecureEngineE.Encrypt(keyName);
                //regValueOff = SecureEngineE.Encrypt(keyNameOff);
            }
            catch (Exception ee)
            {
                aHelper.IsValid = false;
                aHelper.Message = "Error-> null class" + ee.Message;
                return aHelper;
            }
            String[] subKeys = Microsoft.Win32.Registry.CurrentUser.GetSubKeyNames();
            if (subKeys.Contains(regValue))
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regValue);
                //Microsoft.Win32.RegistryKey keyoff = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regValueOff);

                string tokenKey = (string)key.GetValue(SecureEngineE.Encrypt("TK"));

                if (IsConnectionAvailable()) //System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()
                {
                    try
                    {
                        Ctx ctx = System.Web.HttpContext.Current.Session["ctx"] as Ctx;
                        aHelper.Url = ctx.GetApplicationUrl();
                        aHelper.Client_ID = ctx.GetAD_Client_ID();
                        aHelper.UserName = ctx.GetAD_User_Name();
                        aHelper.RoleName = ctx.GetAD_Role_Name();
                        aHelper.ClientName = ctx.GetAD_Client_Name();
                        aHelper.Token = tokenKey;
                        aHelper.IsDemoCheck = info.IsDemoCheck;
                        aHelper.IsEntCheck = false;
                        aHelper = ProcessUpdateLicense(aHelper);
                        if (!aHelper.IsValid)
                        {
                            if (aHelper.Message == "EVAEXP")
                            {
                                aHelper = CheckLicenceValuesOffline(aHelper, key);
                            }
                            else
                            {
                                aHelper.IsAllowWork = true;
                            }
                        }
                        aHelper.IsRegistered = true;
                    }
                    catch (Exception ex)
                    {
                        log.SaveError("KeyEvaluationError", ex);
                    }
                }
                else
                {
                    aHelper = CheckLicenceValuesOffline(aHelper, key);
                }
            }
            else
            {
                aHelper.IsRegistered = true;
                aHelper.IsAllowWork = true;
                aHelper.Message = "EnterKey";
                aHelper.IsValid = false;

                //DateTime dt = new DateTime();
                //if (!subKeys.Contains(regValueOff))
                //{
                //    Microsoft.Win32.RegistryKey keyoff = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regValueOff);
                //    keyoff.SetValue(SecureEngineE.Encrypt("ED"), SecureEngineE.Encrypt(dt.Date.Day.ToString() + "-" + dt.Date.Month.ToString() + "-" + dt.Date.Year.ToString()));
                //}
                //else
                //{
                //    Microsoft.Win32.RegistryKey keyoff = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regValueOff);
                //    object finalDate = keyoff.GetValue(SecureEngineE.Encrypt("ED"));
                //    if (finalDate != null)
                //    {
                //        finalDate = SecureEngineE.Decrypt(finalDate.ToString());
                //        string[] fDate = finalDate.ToString().Split('-');
                //        DateTime EndDate = new DateTime(Convert.ToInt32(fDate.GetValue(2)), Convert.ToInt32(fDate.GetValue(1)), Convert.ToInt32(fDate.GetValue(0)));
                //        if (DateTime.Now > EndDate)
                //        {
                //            aHelper.IsRegistered = true;
                //            aHelper.IsAllowWork = true;

                //            aHelper.IsValid = false;
                //            aHelper.Message = "Market_KeyExpired";
                //            aHelper.IsExpired = true;
                //        }
                //    }
                //}
            }
            return aHelper;

            # region Old data

            //try
            //{
            //    int count = GetUserCount();
            //    string regValue = "";
            //    try
            //    {
            //        regValue = SecureEngineE.Encrypt(keyName);
            //    }
            //    catch (Exception ee)
            //    {
            //        aHelper.IsValid = false;
            //        aHelper.Message = "Error-> null class" + ee.Message;
            //        return aHelper;
            //    }
            //    String[] subKeys = Microsoft.Win32.Registry.CurrentUser.GetSubKeyNames();
            //    if (subKeys.Contains(regValue))
            //    {
            //        Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regValue);

            //        object vd = key.GetValue(SecureEngineE.Encrypt("LD"));
            //        object vm = key.GetValue(SecureEngineE.Encrypt("LM"));
            //        object vy = key.GetValue(SecureEngineE.Encrypt("LY"));
            //        DateTime lastAccessedDate = new DateTime(int.Parse(SecureEngineE.Decrypt(vy.ToString())), int.Parse(SecureEngineE.Decrypt(vm.ToString())), int.Parse(SecureEngineE.Decrypt(vd.ToString())));

            //        aHelper.IsRegistered = true;
            //        //Check For WorkSpace
            //        //Get From Key
            //        string subscribrWork = SecureEngineE.Decrypt(key.GetValue(SecureEngineE.Encrypt("IP")).ToString());
            //        if ("Y".Equals(subscribrWork))
            //        {
            //            aHelper.IsAllowWork = true;
            //        }
            //        else
            //        {
            //            //CheckIsPaid(lastAccessedDate, aHelper, key);

            //        }

            //        // 2.
            //        //First Check For User Count 

            //        int userLimit = int.Parse(SecureEngineE.Decrypt(key.GetValue(SecureEngineE.Encrypt("UL")).ToString()));
            //        //if (count <= DEFAULT_USERS)
            //        //{
            //        //    aHelper.IsValid = true;
            //        //    //aHelper.Message = "UserLimitExceeded";
            //        //    //aHelper.IsULExceeded = true;
            //        //}
            //        if (count > userLimit)
            //        {
            //            aHelper.IsValid = false;
            //            aHelper.Message = "UserLimitExceeded";
            //            aHelper.IsULExceeded = true;
            //        }

            //        else
            //        {
            //            object finalDate = key.GetValue(SecureEngineE.Encrypt("ED"));
            //            if (finalDate == null)
            //            {
            //                if (count <= userLimit)
            //                {
            //                    aHelper.IsValid = true;
            //                }
            //                else
            //                {
            //                    aHelper.IsValid = false;
            //                }
            //                //return aHelper;
            //            }
            //            else
            //            {
            //                if (DateTime.Now < lastAccessedDate)
            //                {
            //                    aHelper.IsValid = false;
            //                    aHelper.Message = "Market_KeyExpired";
            //                    aHelper.IsExpired = true;

            //                }
            //                else
            //                {
            //                    finalDate = SecureEngineE.Decrypt(finalDate.ToString());

            //                    string[] fDate = finalDate.ToString().Split('-');

            //                    //DateTime EndDate = DateTime.Parse(SecureEngineE.Decrypt(finalDate.ToString()));
            //                    DateTime EndDate = new DateTime(Convert.ToInt32(fDate.GetValue(2)), Convert.ToInt32(fDate.GetValue(1)), Convert.ToInt32(fDate.GetValue(0)));
            //                    if (lastAccessedDate > EndDate)
            //                    {
            //                        aHelper.IsValid = false;
            //                        aHelper.Message = "Market_KeyExpired";
            //                        //aHelper.IsULExceeded = true; // Special case 
            //                        aHelper.IsExpired = true;

            //                    }
            //                    else
            //                    {
            //                        aHelper.IsValid = true;
            //                    }
            //                }
            //            }
            //        }
            //        key.SetValue(SecureEngineE.Encrypt("LD"), SecureEngineE.Encrypt(DateTime.Now.Day.ToString()));
            //        key.SetValue(SecureEngineE.Encrypt("LM"), SecureEngineE.Encrypt(DateTime.Now.Month.ToString()));
            //        key.SetValue(SecureEngineE.Encrypt("LY"), SecureEngineE.Encrypt(DateTime.Now.Year.ToString()));
            //        return aHelper;
            //    }
            //    else //First Time only
            //    {
            //        try
            //        {
            //            //Microsoft.Win32.RegistryKey key1 = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regValue);
            //            //key1.SetValue(SecureEngineE.Encrypt("FA"), SecureEngineE.Encrypt(DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString()));
            //            //key1.SetValue(SecureEngineE.Encrypt("UL"), SecureEngineE.Encrypt(DEFAULT_USERS.ToString()));
            //            //key1.SetValue(SecureEngineE.Encrypt("IP"), SecureEngineE.Encrypt("N"));
            //            //key1.SetValue(SecureEngineE.Encrypt("LD"), SecureEngineE.Encrypt(DateTime.Now.Day.ToString()));
            //            //key1.SetValue(SecureEngineE.Encrypt("LM"), SecureEngineE.Encrypt(DateTime.Now.Month.ToString()));
            //            //key1.SetValue(SecureEngineE.Encrypt("LY"), SecureEngineE.Encrypt(DateTime.Now.Year.ToString()));
            //            //aHelper.IsAllowWork = true;
            //            aHelper.IsValid = false;

            //            //if (count > DEFAULT_USERS)
            //            //{
            //            //aHelper.IsValid = false;
            //            //aHelper.Message = "UserLimitExceeded";
            //            //aHelper.IsULExceeded = true;
            //            // }
            //        }
            //        catch (System.UnauthorizedAccessException)
            //        {
            //            aHelper.IsValid = false;
            //            aHelper.Message = "RNP";
            //        }

            //        catch
            //        {
            //            aHelper.IsValid = false;
            //            aHelper.Message = "IntenalError";
            //        }
            //    }
            //}
            //catch (System.UnauthorizedAccessException)
            //{
            //    aHelper.IsValid = false;
            //    aHelper.Message = "RNP";
            //}
            //catch (Exception e)
            //{
            //    aHelper.IsValid = false;
            //    aHelper.Message = "Error-> " + " Inrenal Error " + e.Message;
            //}
            //return aHelper;
            #endregion
        }

        // To check key on local registry
        private InitLoginInfo CheckLicenceValuesOffline(InitLoginInfo aHelper, Microsoft.Win32.RegistryKey key)
        {
            try
            {
                aHelper.IsRegistered = true;

                //Check For WorkSpace , Get From Key
                string subscribeWork = SecureEngineE.Decrypt(key.GetValue(SecureEngineE.Encrypt("IP")).ToString());
                if ("Y".Equals(subscribeWork))
                {
                    aHelper.IsAllowWork = true;
                }

                if (key.GetValue(SecureEngineE.Encrypt("UI")) == null)
                {
                    throw new Exception("Market_UpdateKey");
                }

                if (SecureEngineE.Decrypt(key.GetValue(SecureEngineE.Encrypt("UI")).ToString()) != GetUniqueID("C"))
                {
                    aHelper.IsValid = false;
                    aHelper.Message = "Market_TokenKeyInUse";
                }
                else
                {

                    //int offwork = Convert.ToInt32(SecureEngineE.Decrypt(keyoff.GetValue(SecureEngineE.Encrypt("OFF")).ToString()));
                    //int offworka = Convert.ToInt32(SecureEngineE.Decrypt(keyoff.GetValue(SecureEngineE.Encrypt("OFFA")).ToString()));
                    //if (offwork <= offworka)
                    //{

                    //    aHelper.IsValid = false;
                    //    aHelper.Message = "Info-> Offline attempts finished";
                    //    return aHelper;
                    //}
                    //keyoff.SetValue(SecureEngineE.Encrypt("OFFA"), SecureEngineE.Encrypt((offworka + 1).ToString()));

                    int count = GetUserCount();
                    object vd = key.GetValue(SecureEngineE.Encrypt("LD"));
                    object vm = key.GetValue(SecureEngineE.Encrypt("LM"));
                    object vy = key.GetValue(SecureEngineE.Encrypt("LY"));
                    DateTime lastAccessedDate = new DateTime(int.Parse(SecureEngineE.Decrypt(vy.ToString())), int.Parse(SecureEngineE.Decrypt(vm.ToString())), int.Parse(SecureEngineE.Decrypt(vd.ToString())));

                    //First Check For User Count 
                    int userLimit = int.Parse(SecureEngineE.Decrypt(key.GetValue(SecureEngineE.Encrypt("UL")).ToString()));
                    if (count > userLimit && subscribeWork == "Y")
                    {
                        aHelper.IsValid = false;
                        aHelper.Message = "Market_UserLimitExceeded";
                        aHelper.IsULExceeded = true;
                    }
                    else
                    {
                        object finalDate = key.GetValue(SecureEngineE.Encrypt("ED"));
                        if (finalDate == null)
                        {
                            if (count <= userLimit)
                            {
                                aHelper.IsValid = true;
                            }
                            else
                            {
                                aHelper.IsValid = false;
                            }
                        }
                        else
                        {
                            if (DateTime.Now < lastAccessedDate)
                            {
                                aHelper.IsValid = false;
                                aHelper.Message = "Market_KeyExpired";
                                aHelper.IsExpired = true;
                            }
                            else
                            {
                                finalDate = SecureEngineE.Decrypt(finalDate.ToString());
                                string[] fDate = finalDate.ToString().Split('-');
                                DateTime EndDate = new DateTime(Convert.ToInt32(fDate.GetValue(2)), Convert.ToInt32(fDate.GetValue(1)), Convert.ToInt32(fDate.GetValue(0)));
                                if (lastAccessedDate > EndDate)
                                {
                                    aHelper.IsValid = false;
                                    aHelper.Message = "Market_KeyExpired";
                                    aHelper.IsExpired = true;
                                }
                                else
                                {
                                    aHelper.IsValid = true;
                                }
                            }
                        }
                    }
                }
                key.SetValue(SecureEngineE.Encrypt("LD"), SecureEngineE.Encrypt(DateTime.Now.Day.ToString()));
                key.SetValue(SecureEngineE.Encrypt("LM"), SecureEngineE.Encrypt(DateTime.Now.Month.ToString()));
                key.SetValue(SecureEngineE.Encrypt("LY"), SecureEngineE.Encrypt(DateTime.Now.Year.ToString()));
            }
            catch (System.UnauthorizedAccessException)
            {
                aHelper.IsValid = false;
                aHelper.Message = "RNP";
            }
            catch (Exception e)
            {
                aHelper.IsValid = false;
                aHelper.Message = "Error -> " + " Intrenal Error -> " + e.Message;
            }
            return aHelper;
        }


        private static void CheckIsPaid(DateTime? lastAccessedDate1, InitLoginInfo aHelper, Microsoft.Win32.RegistryKey key)
        {
            DateTime lastAccessedDate;
            if (lastAccessedDate1 == null)
            {
                object vd = key.GetValue(SecureEngineE.Encrypt("LD"));
                object vm = key.GetValue(SecureEngineE.Encrypt("LM"));
                object vy = key.GetValue(SecureEngineE.Encrypt("LY"));
                lastAccessedDate = new DateTime(int.Parse(SecureEngineE.Decrypt(vy.ToString())), int.Parse(SecureEngineE.Decrypt(vm.ToString())), int.Parse(SecureEngineE.Decrypt(vd.ToString())));
            }
            else
            {
                lastAccessedDate = lastAccessedDate1.Value;
            }
            string aDate = SecureEngineE.Decrypt(key.GetValue(SecureEngineE.Encrypt("FA").ToString())).ToString();

            string[] aDates = aDate.ToString().Split('-');

            //DateTime EndDate = DateTime.Parse(SecureEngineE.Decrypt(finalDate.ToString()));
            DateTime FirstAccess = new DateTime(Convert.ToInt32(aDates.GetValue(2)), Convert.ToInt32(aDates.GetValue(1)), Convert.ToInt32(aDates.GetValue(0)));

            if (DateTime.Now < lastAccessedDate)
            {
                aHelper.IsAllowWork = false;
            }

            else if (DateTime.Now.Date < FirstAccess.Date)
            {
                aHelper.IsAllowWork = false;
            }
            else if (DateTime.Now.Date > (FirstAccess.Date.AddDays(30)).Date)
            {
                aHelper.IsAllowWork = false;
            }
            else
            {
                aHelper.IsAllowWork = true;
            }
        }


        private static int GetUserCount()
        {

            int count = DEFAULT_USERS;
            try
            {
                //count = Convert.ToInt32(VAdvantage.DataBase.DB.ExecuteScalar(@"SELECT Count(AD_User_ID) FROM AD_USER WHERE
                //IsLoginUser='Y' AND AD_User_ID > 100 AND AD_Client_ID >0 "));
                //Lakhwinder
                count = Convert.ToInt32(VAdvantage.DataBase.DB.ExecuteScalar(@"SELECT Count(AD_User_ID) FROM AD_USER WHERE
                                                                     IsLoginUser='Y' AND AD_Client_ID NOT IN (0,11) "));
            }
            catch
            {

            }
            return count;

        }

        // Vinay bhatt - Updated for key checking for login 
        public static InitLoginInfo ProcessUpdateLicense(InitLoginInfo infoIn)
        {
            string urlKeyName = keyName;
            //string urlKeyNameOff = keyNameOff;

            string token = infoIn.Token;
            InitLoginInfo aHelper = new InitLoginInfo();

            string svcpath = ServiceEndPoint.GetDNSNameForTokenKey() + "AService.svc";

            EndpointAddress endPointAddress = new EndpointAddress(new Uri(svcpath));
            BasicHttpBinding bind = new BasicHttpBinding();
            bind.MaxReceivedMessageSize = int.MaxValue;
            bind.CloseTimeout = new TimeSpan(00, 10, 00);
            bind.SendTimeout = new TimeSpan(00, 10, 00);
            bind.ReceiveTimeout = new TimeSpan(00, 10, 00);
            bind.MaxBufferPoolSize = int.MaxValue;
            bind.MaxBufferSize = int.MaxValue;
            //bind.MessageEncoding = WSMessageEncoding.Mtom;
            ChannelFactory<IAService> channel = null;
            try
            {
                //if (path.EndsWith("asmx")) throw new Exception();
                channel = new ChannelFactory<IAService>(bind, endPointAddress);
                IAService proxy = channel.CreateChannel();
                ActivationInfo aInfo = new ActivationInfo();
                try
                {
                    aInfo.TokenKey = infoIn.Token;
                    aInfo.UserName = infoIn.UserName;
                    aInfo.ClientName = infoIn.ClientName;
                    aInfo.Machine_ID = GetUniqueID("C");
                    aInfo.IsEntKey = infoIn.IsEntCheck;
                    aInfo.EvaluationOnly = infoIn.IsDemoCheck;

                    System.Net.ServicePointManager.Expect100Continue = false;

                    aInfo = proxy.UpdateMarketLicence(aInfo, GetAccesskey());
                }
                catch (Exception)
                {
                    aHelper.Message = "AInfoEXP";
                    return aHelper;
                }

                if (aInfo == null || aInfo.EndDate == null || aInfo.IsActivated == false)
                {
                    aHelper.IsValid = false;
                    if (aInfo.Machine_ID == "InUse")
                    {
                        aHelper.IsAllowWork = aInfo.IsPaid;
                        aHelper.Message = "Market_TokenKeyInUse";
                    }
                    else if (aInfo.Machine_ID == "PaidFound")
                    {
                        aHelper.Message = "Market_LowerVersionKey";
                    }
                    else
                    {
                        aHelper.Message = "Market_InvalidTokenKey";
                    }
                }
                else
                {
                    DateTime dt = (DateTime)aInfo.EndDate;
                    aHelper.IsAllowWork = aInfo.IsPaid;

                    if (IsProfessionalEdition())
                    {
                        if (aInfo.IsPaid != true)
                        {
                            aHelper.IsValid = false;
                            aHelper.Message = "Market_LowerVersionKey";
                            return aHelper;
                        }
                    }
                    if (dt < DateTime.Now)
                    {
                        aHelper.IsValid = false;
                        aHelper.Message = "Market_ExpiredToken";
                    }
                    else
                    {
                        //int clientInfoID = int.Parse(ds.Tables[0].Rows[0]["AD_EXPORTCLIENTINFO_ID"].ToString());
                        // bool b = false;
                        // if (proxy != null)
                        //    b = proxy.UpdateLicence(token, clientInfoID, GetUniqueID("C"), (DateTime)dt, GetAccesskey());
                        //else if (service != null)
                        //    b = service.UpdateLicence(txtToken.Text, clientInfoID, GetUniqueID("C"), (DateTime)endDate);
                        //else
                        //    b = UpdateLicence(txtToken.Text, clientInfoID, GetUniqueID("C"), (DateTime)endDate);

                        if (aInfo.IsActivated)
                        {
                            try
                            {
                                DateTime dtm = DateTime.Now;
                                if (aInfo.MarketEndDate != null)
                                {
                                    dtm = aInfo.MarketEndDate.Value;
                                }
                                if (dtm < DateTime.Now)
                                {
                                    aHelper.IsMarketExpired = true;
                                    aHelper.Message = "Market_MarketExpired";
                                    //return aHelper;
                                }

                                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(SecureEngineE.Encrypt(urlKeyName));
                                key.SetValue(SecureEngineE.Encrypt("ED"), SecureEngineE.Encrypt(dt.Date.Day.ToString() + "-" + dt.Date.Month.ToString() + "-" + dt.Date.Year.ToString()));
                                key.SetValue(SecureEngineE.Encrypt("LD"), SecureEngineE.Encrypt(DateTime.Now.Day.ToString()));
                                key.SetValue(SecureEngineE.Encrypt("LM"), SecureEngineE.Encrypt(DateTime.Now.Month.ToString()));
                                key.SetValue(SecureEngineE.Encrypt("LY"), SecureEngineE.Encrypt(DateTime.Now.Year.ToString()));

                                key.SetValue(SecureEngineE.Encrypt("MED"), SecureEngineE.Encrypt(dtm.Date.Day.ToString() + "-" + dtm.Date.Month.ToString() + "-" + dtm.Date.Year.ToString()));
                                key.SetValue(SecureEngineE.Encrypt("MLD"), SecureEngineE.Encrypt(DateTime.Now.Day.ToString()));
                                key.SetValue(SecureEngineE.Encrypt("MLM"), SecureEngineE.Encrypt(DateTime.Now.Month.ToString()));
                                key.SetValue(SecureEngineE.Encrypt("MLY"), SecureEngineE.Encrypt(DateTime.Now.Year.ToString()));


                                key.SetValue(SecureEngineE.Encrypt("UL"), SecureEngineE.Encrypt(aInfo.UserCount.ToString()));
                                key.SetValue(SecureEngineE.Encrypt("IP"), SecureEngineE.Encrypt(aInfo.IsPaid ? "Y" : "N"));
                                key.SetValue(SecureEngineE.Encrypt("TK"), aInfo.TokenKey);
                                key.SetValue(SecureEngineE.Encrypt("UI"), SecureEngineE.Encrypt(aInfo.Machine_ID));

                                //key.SetValue(SecureEngineE.Encrypt("OFF"), SecureEngineE.Encrypt("5"));
                                //key.SetValue(SecureEngineE.Encrypt("OFFA"), SecureEngineE.Encrypt("0"));

                                aHelper.IsValid = true;

                                //Microsoft.Win32.RegistryKey keyoff = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(SecureEngineE.Encrypt(urlKeyNameOff));
                                //keyoff.SetValue(SecureEngineE.Encrypt("OFF"), SecureEngineE.Encrypt("5"));
                                //keyoff.SetValue(SecureEngineE.Encrypt("OFFA"), SecureEngineE.Encrypt("0"));

                                //aHelper.IsAllowWork = aInfo.IsPaid;

                                //if (!aHelper.IsAllowWork)
                                //{
                                //    CheckIsPaid(null, aHelper, key);
                                //}

                                int count = GetUserCount();
                                if (aInfo.UserCount < count && count > DEFAULT_USERS && aHelper.IsAllowWork)
                                {
                                    aHelper.IsValid = false;
                                    aHelper.IsULExceeded = true;
                                    aHelper.Message = "Market_UserLimitExceeded";
                                }
                                else
                                {
                                    aHelper.Message = "Market_KeyValidated";
                                    aHelper.Token = infoIn.Token;
                                }

                                aHelper.IsRegistered = true;
                            }
                            catch (System.UnauthorizedAccessException e)
                            {
                                aHelper.IsValid = false;
                                aHelper.Message = e.Message;
                                aHelper.Message = "RNP";
                            }
                            catch (Exception e)
                            {
                                aHelper.Message = "GR EXP";
                            }
                        }
                        else
                        {
                            aHelper.IsValid = false;
                            aHelper.Message = "Market_TokenNotValidated";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                aHelper.Message = "EVAEXP";
                //aHelper.IsValid = false;

                if (ex.InnerException != null && ex.InnerException.Message != null)
                    aHelper.Message = ex.InnerException.Message;
                    
                aHelper.Message += "_" + ex.Message;

                //proxy = null;
                //bg.ReportProgress(0, "Activating License through alternate service");
                //VService service = new VService(path);
                //ServicePointManager.Expect100Continue = false;
                //DateTime? endDate = null;
                //try
                //{
                //    DataSet ds = service.CheckLicence(txtToken.Text, out endDate);
                //    if (ds == null)
                //    {
                //        e.Result = false;
                //        return;
                //    }
                //    bg.ReportProgress(0, "Data fetched from Vienna Server");
                //    bool result = ProcessLicense(ds, (DateTime)endDate, bg, service);
                //    e.Result = result;

                //}
                //catch (Exception ex1)
                //{
                //    e.Result = false;
                //    bg.ReportProgress(0, "Error ocurred while activating");
                //    //ShowMessage.Error("Exception Occured: " + ex1.Message, false);
                //    //try
                //    //{
                //    //    bg.ReportProgress(0, "Activating License through Direct connection");
                //    //    //DateTime? endDate = null;
                //    //    DataSet ds = CheckLicence(txtToken.Text, out endDate);

                //    //    if (ds == null)
                //    //    {
                //    //        e.Result = false;
                //    //        return;

                //    //    }
                //    //    bg.ReportProgress(0, "Data fetched from Vienna Server");
                //    //    bool result = ProcessLicense(ds, (DateTime)endDate, bg, null);
                //    //    e.Result = result;

                //    //}
                //    //catch (Exception ex1)
                //    //{
                //    //    ShowMessage.Error("Exception Occured: " + ex1.Message, false);
                //    //}
                //}

                //MessageBox.Show(ex.Message);
            }
            finally
            {
                if (channel != null)
                    channel.Close();
            }
            return aHelper;
        }

        private static string GetUniqueID(string drive)
        {
            if (drive == string.Empty)
            {
                //Find first drive
                foreach (DriveInfo compDrive in DriveInfo.GetDrives())
                {
                    if (compDrive.IsReady)
                    {
                        drive = compDrive.RootDirectory.ToString();
                        break;
                    }
                }
            }

            if (drive.EndsWith(":\\"))
            {
                //C:\ -> C
                drive = drive.Substring(0, drive.Length - 2);
            }

            string volumeSerial = GetVolumeSerial(drive);
            string cpuID = GetCPUID();

            //Mix them up and remove some useless 0's
            return cpuID.Substring(13) + cpuID.Substring(1, 4) + volumeSerial + cpuID.Substring(4, 4);
        }

        private static string GetVolumeSerial(string drive)
        {
            ManagementObject disk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + drive + @":""");
            disk.Get();

            string volumeSerial = disk["VolumeSerialNumber"].ToString();
            disk.Dispose();

            return volumeSerial;
        }

        private static string GetCPUID()
        {
            string cpuInfo = "";
            ManagementClass managClass = new ManagementClass("win32_processor");
            ManagementObjectCollection managCollec = managClass.GetInstances();

            foreach (ManagementObject managObj in managCollec)
            {
                if (cpuInfo == "")
                {
                    //Get only the first CPU's ID
                    cpuInfo = managObj.Properties["processorID"].Value.ToString();
                    break;
                }
            }

            return cpuInfo;
        }

        private static string GetAccesskey()
        {
            //string url = "";
            //try
            //{
            //    url = SecureEngine.Encrypt(System.Configuration.ConfigurationManager.AppSettings["accesskey"].ToString());

            //}
            //catch { }
            //return url;
            return "caff4eb4fbd6273e37e8a325e19f0991";
        }

        internal static string GetKey()
        {
            return SecureEngineE.Encrypt(keyName);
        }

        internal static string GetTokenKey()
        {
            string regValue = SecureEngineE.Encrypt(keyName);
            String[] subKeys = Microsoft.Win32.Registry.CurrentUser.GetSubKeyNames();
            if (subKeys.Contains(regValue))
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regValue);
                //Get From Key
                string tokenKey = (string)key.GetValue(SecureEngineE.Encrypt("TK"));

                key.SetValue(SecureEngineE.Encrypt("LD"), SecureEngineE.Encrypt(DateTime.Now.Day.ToString()));
                key.SetValue(SecureEngineE.Encrypt("LM"), SecureEngineE.Encrypt(DateTime.Now.Month.ToString()));
                key.SetValue(SecureEngineE.Encrypt("LY"), SecureEngineE.Encrypt(DateTime.Now.Year.ToString()));

                return tokenKey;
            }
            return "";
        }

        internal static bool IsTokenKeyExpired()
        {
            bool isExpired = true;

            string regValue = SecureEngineE.Encrypt(keyName);
            String[] subKeys = Microsoft.Win32.Registry.CurrentUser.GetSubKeyNames();
            if (subKeys.Contains(regValue))
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regValue);
                //Get From Key
                string tokenKey = (string)key.GetValue(SecureEngineE.Encrypt("TK"));





                object vd = key.GetValue(SecureEngineE.Encrypt("LD"));
                object vm = key.GetValue(SecureEngineE.Encrypt("LM"));
                object vy = key.GetValue(SecureEngineE.Encrypt("LY"));
                DateTime lastAccessedDate = new DateTime(int.Parse(SecureEngineE.Decrypt(vy.ToString())), int.Parse(SecureEngineE.Decrypt(vm.ToString())), int.Parse(SecureEngineE.Decrypt(vd.ToString())));

                if (DateTime.Now < lastAccessedDate)
                {
                    isExpired = true;

                }
                else
                {
                    // vinay bhatt - check market key on cloud server
                    try
                    {
                        Ctx ctx = System.Web.HttpContext.Current.Session["ctx"] as Ctx;
                        InitLoginInfo lInfo = new InitLoginInfo();
                        lInfo.Url = ctx.GetApplicationUrl();
                        lInfo.Client_ID = ctx.GetAD_Client_ID();
                        lInfo.UserName = ctx.GetAD_User_Name();
                        lInfo.RoleName = ctx.GetAD_Role_Name();
                        lInfo.ClientName = ctx.GetAD_Client_Name();
                        lInfo.Token = tokenKey;
                        lInfo.IsDemoCheck = false;
                        lInfo.IsEntCheck = false;
                        lInfo = ProcessUpdateLicense(lInfo);
                        if (lInfo.IsValid == true && lInfo.IsMarketExpired == false)
                        {
                            isExpired = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.SaveError("KeyEvaluationError", ex);
                    }

                    //object finalDate = key.GetValue(SecureEngineE.Encrypt("ED"));
                    //if (finalDate == null)
                    //{
                    //    isExpired = true;
                    //    //return aHelper;
                    //}
                    //else
                    //{
                    //    finalDate = SecureEngineE.Decrypt(finalDate.ToString());
                    //    string[] fDate = finalDate.ToString().Split('-');
                    //    DateTime EndDate = new DateTime(Convert.ToInt32(fDate.GetValue(2)), Convert.ToInt32(fDate.GetValue(1)), Convert.ToInt32(fDate.GetValue(0)));
                    //    if (lastAccessedDate < EndDate)
                    //    {
                    //        isExpired = true;
                    //        //aHelper.IsULExceeded = true; // Special case 
                    //    }
                    //    else
                    //    {

                    //        isExpired = false;
                    //    }
                    //}

                    //
                }
                key.SetValue(SecureEngineE.Encrypt("LD"), SecureEngineE.Encrypt(DateTime.Now.Day.ToString()));
                key.SetValue(SecureEngineE.Encrypt("LM"), SecureEngineE.Encrypt(DateTime.Now.Month.ToString()));
                key.SetValue(SecureEngineE.Encrypt("LY"), SecureEngineE.Encrypt(DateTime.Now.Year.ToString()));
            }
            return isExpired;
        }

        internal static bool IsMarketExpired()
        {
            bool isExpired = true;

            try
            {
                string regValue = SecureEngineE.Encrypt(keyName);
                String[] subKeys = Microsoft.Win32.Registry.CurrentUser.GetSubKeyNames();
                if (subKeys.Contains(regValue))
                {
                    Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regValue);
                    //Get From Key
                    //string tokenKey = (string)key.GetValue(SecureEngineE.Encrypt("MK"));





                    object vd = key.GetValue(SecureEngineE.Encrypt("MLD"));
                    object vm = key.GetValue(SecureEngineE.Encrypt("MLM"));
                    object vy = key.GetValue(SecureEngineE.Encrypt("MLY"));
                    DateTime lastAccessedDate = new DateTime(int.Parse(SecureEngineE.Decrypt(vy.ToString())), int.Parse(SecureEngineE.Decrypt(vm.ToString())), int.Parse(SecureEngineE.Decrypt(vd.ToString())));

                    if (DateTime.Now < lastAccessedDate)
                    {
                        isExpired = true;

                    }
                    else
                    {
                        object finalDate = key.GetValue(SecureEngineE.Encrypt("MED"));
                        if (finalDate == null)
                        {
                            isExpired = true;
                            //return aHelper;
                        }
                        else
                        {
                            finalDate = SecureEngineE.Decrypt(finalDate.ToString());
                            string[] fDate = finalDate.ToString().Split('-');
                            DateTime EndDate = new DateTime(Convert.ToInt32(fDate.GetValue(2)), Convert.ToInt32(fDate.GetValue(1)), Convert.ToInt32(fDate.GetValue(0)));
                            if (lastAccessedDate > EndDate)
                            {
                                isExpired = true;
                                //aHelper.IsULExceeded = true; // Special case 
                            }
                            else
                            {

                                isExpired = false;
                            }
                        }
                    }
                    key.SetValue(SecureEngineE.Encrypt("MLD"), SecureEngineE.Encrypt(DateTime.Now.Day.ToString()));
                    key.SetValue(SecureEngineE.Encrypt("MLM"), SecureEngineE.Encrypt(DateTime.Now.Month.ToString()));
                    key.SetValue(SecureEngineE.Encrypt("MLY"), SecureEngineE.Encrypt(DateTime.Now.Year.ToString()));
                }
            }
            catch
            {
            }
            return isExpired;
        }

        internal static bool IsProfessionalEdition()
        {
            string regValue = GetKey();
            String[] subKeys = Microsoft.Win32.Registry.CurrentUser.GetSubKeyNames();
            if (subKeys.Contains(regValue))
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regValue);
                //Get From Key
                string isPaid = SecureEngineE.Decrypt(key.GetValue(SecureEngineE.Encrypt("IP")).ToString());
                if ("Y".Equals(isPaid))
                {
                    return true;
                }
            }
            return false;
        }

        internal static string GetKeyEdition(string url)
        {
            bool isCloudUrl = false;
            if (!string.IsNullOrEmpty(url) && url.ToLower().Contains("softwareonthecloud"))
            {
                isCloudUrl = true;
            }

            VAdvantage.KeyEdition key = VAdvantage.KeyEdition.NotRegisterd;

            if (!isCloudUrl)
            {
                string token = GetTokenKey();
                if (!string.IsNullOrEmpty(token))
                {
                    if (IsProfessionalEdition())
                    {
                        key = VAdvantage.KeyEdition.Professional;
                    }
                    else
                    {
                        key = VAdvantage.KeyEdition.Community;
                    }
                }
            }

            else // fetch from cloud service
            {

                string svcpath = ServiceEndPoint.GetDNSNameForTokenKey() + "AService.svc";

                EndpointAddress endPointAddress = new EndpointAddress(new Uri(svcpath));
                ChannelFactory<IAService> channel = null;
                int result = 0;
                try
                {
                    //if (path.EndsWith("asmx")) throw new Exception();
                    channel = new ChannelFactory<IAService>(new BasicHttpBinding(), endPointAddress);
                    IAService proxy = channel.CreateChannel();
                    System.Net.ServicePointManager.Expect100Continue = false;
                    result = proxy.GetKeyEdition(url, GetAccesskey());
                    channel.Close();
                }
                catch
                {
                    if (channel != null)
                        channel.Close();
                }

                if (result == 2)
                {
                    key = VAdvantage.KeyEdition.Professional;
                }
                else if (result == 1)
                {
                    key = VAdvantage.KeyEdition.Community;
                }
            }

            Env.InitEdition(key);
            return ((int)key).ToString();
        }

        private static void InitKeyName()
        {
            if (keyName != null && keyName.Length != 0)
            {
                return;
            }
            //Set Unique Key Name on Base Of Url

        }

        internal static void Init(string auth)
        {
            if (!String.IsNullOrEmpty(auth) && keyName != auth)
            {
                keyName = auth;
            }
        }

    }
}
