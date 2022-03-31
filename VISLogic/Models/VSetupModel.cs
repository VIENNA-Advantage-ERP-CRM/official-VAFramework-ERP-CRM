using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class VSetupModel
    {
        public InitialData GetInitialData(Ctx ctx)
        {

            bool isBaseLanguage = VAdvantage.Utility.Env.IsBaseLanguage(ctx, "C_Currency");
            string sqlCu = null;
            string sqlCo = null;
            string sqlRe = null;
            if (isBaseLanguage)
            {
                sqlCu = "SELECT C_Currency_ID, Description FROM C_Currency ORDER BY 1";
                sqlCo = "SELECT C_Country_ID, Name FROM C_Country WHERE IsSummary='N' ORDER BY 1";
                sqlRe = "SELECT C_Region_ID, Name FROM C_Region ORDER BY C_Country_ID, Name";
            }
            else
            {
                sqlCu = @"SELECT C.C_Currency_ID, CL.Description
                            FROM C_Currency C
                            INNER JOIN C_Currency_Trl CL
                            ON (C.C_Currency_ID=CL.C_Currency_ID
                            AND CL.ad_language ='" + ctx.GetAD_Language() + "') ORDER BY 1";
               // sqlCo = "SELECT C_Country_ID, Name FROM C_Country WHERE IsSummary='N' ORDER BY 1";
                sqlCo=@"SELECT C.C_Country_ID, CL.Name
                            FROM C_Country C
                            INNER JOIN C_Country_Trl CL
                            ON (C.C_Country_ID=CL.C_Country_ID
                            AND CL.ad_language ='" + ctx.GetAD_Language() + "') ORDER BY 1";
                sqlRe = "SELECT C_Region_ID, Name FROM C_Region ORDER BY C_Country_ID, Name";
                
            }
            InitialData ini = new InitialData();
            DataSet ds = DBase.DB.ExecuteDataset(sqlCu);
            List<Currency> curr = new List<Currency>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                curr.Add(new Currency()
                {
                    ID = Util.GetValueOfInt(ds.Tables[0].Rows[i][0]),
                    Name = Util.GetValueOfString(ds.Tables[0].Rows[i][1])
                });
            }
            ini.currency = curr;

            ds = DBase.DB.ExecuteDataset(sqlCo);
            List<Country> country = new List<Country>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                country.Add(new Country()
                {
                    ID = Util.GetValueOfInt(ds.Tables[0].Rows[i][0]),
                    Name = Util.GetValueOfString(ds.Tables[0].Rows[i][1])
                });
            }
            ini.country = country;

            ds = DBase.DB.ExecuteDataset(sqlRe);
            List<Region> region = new List<Region>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                region.Add(new Region()
                {
                    ID = Util.GetValueOfInt(ds.Tables[0].Rows[i][0]),
                    Name = Util.GetValueOfString(ds.Tables[0].Rows[i][1])
                });
            }
            ini.region = region;

            return ini;
        }


        public TenantInfo InitailizeClientSetup(string clientName, string orgName, string userClient, string userOrg, string city,
            int currencyID, string currencyName, int countryID, string countryName, int regionID, string regionName,
            bool cfProduct, bool cfBPartner, bool cfProject, bool cfMCampaign, bool cfSRegion, string fileName, string folderKey, Ctx ctx)
        {

            TenantInfo tInfo = new TenantInfo();
            FileStream m_file = null;

            try
            {
                tInfo.TenantName = clientName;
               
                //Functinality moved to FRPT
                //if (string.IsNullOrEmpty(fileName))
                //{
                //    fileName = "AccountingUS1.csv";
                //    m_file = new FileStream(HostingEnvironment.ApplicationPhysicalPath + fileName, FileMode.Open);
                //}
                //else
                //{
                //    m_file = new FileStream(HostingEnvironment.ApplicationPhysicalPath + "TempDownload\\" + folderKey + "\\" + fileName, FileMode.Open);
                //}

                string retVal = "";

                // worker.ReportProgress(0);
                Context context = new Context((Dictionary<string, string>)ctx.GetMap());
                MSetup ms = new MSetup(context, 0);
                //  Step 1
                //bool ok = ms.CreateClient(fClientName.Text, fOrgName.Text, fUserClient.Text, fUserOrg.Text);
                bool ok = false;
                TenantInfoM clientInfo = ms.CreateClient(clientName, orgName, userClient, userOrg);
                if (string.IsNullOrEmpty(clientInfo.Log))
                {
                    ok = true;
                }
                tInfo.TenantName = clientInfo.TenantName;
                tInfo.OrgName = clientInfo.OrgName;
                tInfo.AdminRole = clientInfo.AdminRole;
                tInfo.UserRole = clientInfo.UserRole;
                tInfo.AdminUser = clientInfo.AdminUser;
                tInfo.AdminUserPwd = clientInfo.AdminUserPwd;
                tInfo.OrgUser = clientInfo.OrgUser;
                tInfo.OrgUserPwd = clientInfo.OrgUserPwd;
                tInfo.TenantID = clientInfo.TenantID;
                // worker.ReportProgress(10);
                String info = ms.GetInfo();
                tInfo.Log = clientInfo.Log;
                if (ok)
                {
                    //  Generate Accounting
                    // worker.ReportProgress(15);
                    // if (!ms.CreateAccounting(currency, cfProduct, cfBPartner, cfProject, cfMCampaign, cfSRegion, m_file))
                    // KeyNamePair currency = new KeyNamePair(clientSetup.currency.Key, clientSetup.currency.Value);
                    KeyNamePair currency = new KeyNamePair(currencyID, currencyName);
                    string res = "";
                    if (!ms.CreateAccounting(currency, cfProduct, cfBPartner, cfProject, cfMCampaign, cfSRegion, m_file, out res))
                    {
                        //ShowMessage.Error("AccountSetupError", false);
                        //Dispose();
                        retVal = "AccountSetupError" + res;
                        tInfo.Log = retVal;
                    }
                    res = null;
                    // worker.ReportProgress(45);
                    //  Generate Entities

                    int C_Country_ID = countryID;

                    int C_Region_ID = regionID;
                    //  worker.ReportProgress(75);
                    ms.CreateEntities(C_Country_ID, city, C_Region_ID, currencyID);
                    //worker.ReportProgress(90);
                    info += ms.GetInfo();
                    //	Create Print Documents
                    ms.SetupPrintForm(ms.GetAD_Client_ID());
                    // worker.ReportProgress(100);
                }

                //Functinality moved to FRPT
                //try
                //{
                //    if (m_file != null)
                //        m_file.Close();
                //    System.IO.File.Delete(HostingEnvironment.ApplicationPhysicalPath + "TempDownload\\" + folderKey + "\\" + fileName);
                //    System.IO.Directory.Delete(HostingEnvironment.ApplicationPhysicalPath + "TempDownload\\" + folderKey, true);
                //}
                //catch (Exception)
                //{
                //    //retVal += (ex.Message);
                //}

                if (tInfo.Log == null)
                {
                    tInfo.Log =  retVal;
                }
               
                //try
                //{
                //    if (m_file != null)
                //        m_file.Close();
                //}
                //catch { }



            }
            catch (Exception ex)
            {
                //if (m_file != null)
                //    m_file.Close();
                //System.IO.File.Delete(HostingEnvironment.ApplicationPhysicalPath + "TempDownload\\" + folderKey + "\\" + fileName);
                //System.IO.Directory.Delete(HostingEnvironment.ApplicationPhysicalPath + "TempDownload\\" + folderKey, true);
                tInfo.Log = ex.Message;

            }

            return tInfo;// retVal;


        }
    }
    public class InitialData
    {
        public List<Currency> currency
        {
            get;
            set;
        }
        public List<Region> region
        {
            get;
            set;
        }
        public List<Country> country
        {
            get;
            set;
        }
    }
    public class Currency
    {
        public int ID
        {
            get;
            set;

        }
        public String Name
        {
            get;
            set;
        }
    }

    public class Region
    {
        public int ID
        {
            get;
            set;

        }
        public String Name
        {
            get;
            set;
        }
    }
    public class Country
    {
        public int ID
        {
            get;
            set;

        }
        public String Name
        {
            get;
            set;
        }
    }



    public class TenantInfo
    {

        public string TenantName
        {
            get;
            set;
        }

        public string OrgName
        {
            get;
            set;
        }

        public string AdminRole
        {
            get;
            set;
        }

        public string UserRole
        {
            get;
            set;
        }


        public string AdminUser
        {
            get;
            set;
        }


        public string AdminUserPwd
        {
            get;
            set;
        }


        public string OrgUser
        {
            get;
            set;
        }


        public string OrgUserPwd
        {
            get;
            set;
        }

        public string Log
        {
            get;
            set;
        }

        public int TenantID
        {
            get;
            set;
        }
    }
}