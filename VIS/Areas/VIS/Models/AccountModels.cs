using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Web.Security;

using VAdvantage.Model;
using VAdvantage.Utility;

////// <summary>
/// login Models
/// </summary>
namespace VIS.Models
{

    /// <summary>
    /// Login model class 
    /// - Login1Model model for first login tab
    /// - Login2Model model for second login tab
    /// </summary>
    public class LoginModel
    {
        public Login1Model Login1Model { get; set; }
        public Login2Model Login2Model { get; set; }
        public bool Step2 { get; set; }
        public bool Success { get; set; }
    }

    /// <summary>
    /// model class 
    /// </summary>
    public class Login1Model
    {
        [Required(ErrorMessage = "UserPwdError")]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "UserPwdError")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
         [Required]
        [Display(Name = "LoginLanguage")]
        public string LoginLanguage { get; set; }

       public int AD_User_ID { get; set; }

    }

    /// <summary>
    /// login model for second login tab
    /// </summary>
    public class Login2Model
    {

        public Login2Model()
        {

            Date = DateTime.Now.Date;
        }

        [Display(Name = "Role")]
        [Range(0, int.MaxValue, ErrorMessage = "SelectRole")]
        [Required(ErrorMessage = "SelectRole")]
        public String Role { get; set; }
        
        public String RoleName { get; set; }

      

        [Display(Name = "Client")]
        [Range(0, int.MaxValue, ErrorMessage = "SelectClient")]
        [Required(ErrorMessage = "SelectClient")]
        public String Client { get; set; }
        public String ClientName { get; set; }

        [Display(Name = "Org")]
        [Range(0, int.MaxValue, ErrorMessage = "SelectOrg")]
        [Required(ErrorMessage = "SelectOrg")]
        public String Org
        {
            get;
            set;
        }
        public String OrgName
        {
            get;
            set;
        }
        public int HomePageID
        {
            get;
            set;
        }

        public bool DisableMenu
        {
            get;
            set;
        }


        
        [Display(Name = "WareHouse")]
        
        public string Warehouse { get; set; }
        public String WarehouseName { get; set; }

        [Display(Name = "Date")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }
        
        public String Login1Data { get; set; }
        public string LoginLanguage { get; set; }
    }

    /// <summary>
    /// store login value in dictionary
    /// - persist between login tab switching
    /// - pass values to actual context on home page
    /// </summary>
    public class LoginContext
    {
        public Dictionary<string, string> ctxMap = new Dictionary<string, string>();
        public void SetContext(string key, string value)
        {
            ctxMap[key] = value;
        }
    }

    
    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }


    public class CloudLogin
    {
        public static string IsAllowedToLogin(string url)
        {
            string retUrl = "";
            ModelLibrary.CloudService.ServiceSoapClient cloud = null;
          
            try
            {
                cloud = VAdvantage.Classes.ServerEndPoint.GetCloudClient();

                if (cloud == null || cloud.ToString() == "")
                {
                    //Response.Redirect("http://demo.viennaadvantage.com",true);
                    retUrl = GenerateUrl(url);
                    return retUrl ;
                }
            }
            catch
            {
            }
            //string result = "";
            try
            {
                //System.Net.ServicePointManager.Expect100Continue = false;
                try
                {
                    System.Net.ServicePointManager.Expect100Continue = false;
                    retUrl = cloud.isAllowedToContinue(url, SecureEngine.Encrypt(System.Web.Configuration.WebConfigurationManager.AppSettings["accesskey"].ToString()));
                }
                catch
                {

                }
                cloud.Close();
                try
                {
                    if (retUrl != "True")
                    {

                         return retUrl;
                    }
                    else
                    {
                        retUrl  =  GenerateUrl(url);
                    }
                }
                catch
                {

                }
            }
            catch
            {

                return retUrl;
            }
            return retUrl;
        }

        private static string GenerateUrl(string urlIn)
        {
            string urlOut = "";

            if (VAdvantage.Classes.ServerEndPoint.IsSSLEnabled())
            {

                if (urlIn.Contains("http://"))
                {
                    urlOut = urlIn.Replace("http://", "https://");
                    //  Response.Redirect(url, false);
                }
                else if (urlIn.Contains("https://"))
                {
                    urlOut = "";
                }
                else
                {
                    urlOut = "https://" + urlIn;
                }
            }
            return urlOut;
        }
    }
}
