using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Controllers
{
    public class ValuePreferenceModel
    {
        public String Attribute { get; set; }
        public String DisplayAttribute { get; set; }
        public String Value { get; set; }
        public String DisplayValue { get; set; }
        public int DisplayType { get; set; }

        public int AD_Client_ID { get; set; }
        public int AD_Org_ID { get; set; }
        public int AD_User_ID { get; set; }
        public int AD_Window_ID { get; set; }
        public int AD_Reference_ID { get; set; }

        //Repository

        /// <summary>
        /// delete prefrence accouring to there value
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="preferenceId"></param>
        /// <returns></returns>
        public bool DeletePrefrence(Ctx ctx, string preferenceId)
        {
            bool success = false;
            int AD_Preference_ID = Convert.ToInt32(preferenceId);

            MPreference pref = new MPreference(ctx, AD_Preference_ID, null);
            // delete the preference
            success = pref.Delete(true);

            return success;
        }

        /// <summary>
        /// Save logic for value prefrences
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="preferenceId"></param>
        /// <param name="clientId"></param>
        /// <param name="orgId"></param>
        /// <param name="chkWindow"></param>
        /// <param name=
        /// ></param>
        /// <param name="chkUser"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public bool SavePrefrence(Ctx ctx, string preferenceId, string clientId, string orgId, string chkWindow, string AD_Window_ID, string chkUser, string attribute, string userId, string value)
        {
            bool success = false;

            int AD_Preference_ID = Convert.ToInt32(preferenceId);
            int _AD_Window_ID = Convert.ToInt32(AD_Window_ID);
            int _AD_User_ID = Convert.ToInt32(userId);
            bool _chkUser, _chkWindow;
            _chkUser = Convert.ToBoolean(chkUser);
            _chkWindow = Convert.ToBoolean(chkWindow);

            MPreference pref = new MPreference(ctx, AD_Preference_ID, null);
            // if preference id=0
            if (AD_Preference_ID == 0)
            {
                // if inserting a new record, then set initial values
                // set client id
                int Client_ID = Convert.ToInt32(clientId);
                // set organization id
                int Org_ID = Convert.ToInt32(orgId);

                pref.SetClientOrg(Client_ID, Org_ID);
                // set window id
                if (_chkWindow)
                {
                    pref.SetAD_Window_ID(_AD_Window_ID);
                }
                // set user id
                if (_chkUser)
                {
                    pref.SetAD_User_ID(_AD_User_ID);
                }

                // set attribute(columnname)
                pref.SetAttribute(attribute);
            }
            // set value of attribute
            pref.SetValue(value);
            // save the record
            success = pref.Save();
            return success;
        }
    }
}