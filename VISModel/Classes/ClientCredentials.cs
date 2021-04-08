using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIS.Classes
{
    /// <summary>
    /// This class provides the client credentials for all the samples in this solution.
    /// In order to run all of the samples, you have to enable API access for every API 
    /// you want to use, enter your credentials here.
    /// 
    /// You can find your credentials here:
    ///  https://code.google.com/apis/console/#:access
    /// 
    /// For your own application you should find a more secure way than just storing your client secret inside a string,
    /// as it can be lookup up easily using a reflection tool.
    /// </summary>
    public static class ClientCredentials
    {
        /// <summary>
        /// The OAuth2.0 Client ID of your project.
        /// </summary>
       // public static readonly string ClientID = "919078660135-vphkrlc7c2jj4iejge10j3cnbasfkfvd.apps.googleusercontent.com";
        public static readonly string ClientID = "422197090448-u4gdh89n27j3cfs444vpm2esohkl7kep.apps.googleusercontent.com";
        /// <summary>
        /// The OAuth2.0 Client secret of your project.
        /// </summary>
       // public static readonly string ClientSecret = "hXKYgbaPT7u3ukl-_IjfzGk5";
        public static readonly string ClientSecret = "2mVB2q64mH-GvrmIzClCyjak";
        /// <summary>
        /// Your Api/Developer key.
        /// </summary>
     //   public static readonly string ApiKey = "AIzaSyAhlU6fUGZdsJvxdQZDU8CABfdw4ZwUwlY";
        public static readonly string ApiKey = "AIzaSyAG6BAI5GPGxO7LEI7DBn0nhrmF7BxhApE";
        #region Verify Credentials
        //static ClientCredentials()
        //{
        //    ReflectionUtils.VerifyCredentials(typeof(ClientCredentials));
        //}
        #endregion
    }
}