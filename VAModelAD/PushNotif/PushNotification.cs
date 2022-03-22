using System;
using System.Collections.Generic;
using System.Reflection;
using VAdvantage.Logging;
using VAdvantage.Utility;
namespace VAdvantage.PushNotif
{
    public static class PushNotification
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(PushNotification).FullName);

        /// <summary>
        /// Send push notification to the specified user with the specified data
        /// </summary>
        /// <param name="AD_User_ID"></param>
        /// <param name="AD_Window_ID"></param>
        /// <param name="AD_Record_ID"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="notifType"></param>
        public static void SendNotificationToUser(int AD_User_ID, int AD_Window_ID, int AD_Record_ID, string title, string message, string notifType)
        {
            if (!Env.IsModuleInstalled("VA074_"))
            {
                _log.SaveError("PushNotif", "Module VA074_ is not installed!");
                return;
            }

            try
            {
                Assembly assembly = Assembly.Load("VA074Svc");
                Type type = assembly.GetType("VA074Svc.Classes.PushNotification");

                if (type != null)
                {
                    object[] param = new object[6];
                    param[0] = AD_User_ID;
                    param[1] = AD_Window_ID;
                    param[2] = AD_Record_ID;
                    param[3] = title;
                    param[4] = message;
                    param[5] = notifType;
                    var successCount = type.GetMethod("SendNotificationToUser").Invoke(null, param);
                }
            }
            catch (Exception e)
            {
                _log.SaveError("PushNotif", "Exception: " + e.Message);
            }
        }

        /// <summary>
        /// Send push notification to the specified user with the additional data
        /// </summary>
        /// <param name="AD_User_ID"></param>
        /// <param name="AD_Window_ID"></param>
        /// <param name="AD_Record_ID"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="notifType"></param>
        /// <param name="keyVals"></param>
        public static void SendNotificationDetails(int AD_User_ID, int AD_Window_ID, int AD_Record_ID, string title, string message, string notifType, Dictionary<string, object> keyVals)
        {
            if (!Env.IsModuleInstalled("VA074_"))
            {
                _log.SaveError("PushNotif", "Module VA074_ is not installed!");
                return;
            }

            try
            {
                Assembly assembly = Assembly.Load("VA074Svc");
                Type type = assembly.GetType("VA074Svc.Classes.PushNotification");

                if (type != null)
                {
                    object[] param = new object[7];
                    param[0] = AD_User_ID;
                    param[1] = AD_Window_ID;
                    param[2] = AD_Record_ID;
                    param[3] = title;
                    param[4] = message;
                    param[5] = notifType;
                    param[6] = keyVals;
                    var successCount = type.GetMethod("SendNotificationDetails").Invoke(null, param);
                }
            }
            catch (Exception e)
            {
                _log.SaveError("PushNotif", "Exception: " + e.Message);
            }
        }
    }
}
