using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using VAdvantage.Utility;

namespace VAdvantage.Classes
{
    public class CommonFunctions
    {
        /// <summary>
        /// Gets whether specified value is a number
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>true if given string is a number</returns>
        /// <author>Veena</author>
        public static bool IsNumeric(string value)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(value, @"^-?(?:0|[1-9][0-9]*)(?:\.[0-9]+)?$");
        }

        /// <summary>
        /// Gets time in mili seconds since 1970
        /// </summary>
        /// <returns>long</returns>
        public static long CurrentTimeMillis()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            return (long)ts.TotalMilliseconds;
        }

        /// <summary>
        /// Gets time in mili seconds since 1970
        /// </summary>
        /// <param name="start">datetime</param>
        /// <returns>Returns the number of milliseconds since January 1, 1970, 00:00:00 GMT represented by passed object</returns>
        public static long CurrentTimeMillis(DateTime start)
        {
            TimeSpan ts = (start - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            long mils = (long)ts.TotalMilliseconds;
            //long l = dt.Ticks / TimeSpan.TicksPerMillisecond;
            DateTime myDate = new DateTime((mils * TimeSpan.TicksPerMillisecond) + 621355968000000000);
            return mils;
        }

        /// <summary>
        /// covertmillisecond to date
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns>DateTime</returns>
        public static DateTime CovertMilliToDate(long milliseconds)
        {
            DateTime myDate = new DateTime((milliseconds * TimeSpan.TicksPerMillisecond) + 621355968000000000);
            //new DateTime(

            return myDate;

        }

        /// <summary>
        /// covertmillisecond to date
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns>DateTime</returns>
        public static String CovertMilliToDateString(long milliseconds)
        {
            DateTime myDate = new DateTime((milliseconds * TimeSpan.TicksPerMillisecond) + 621355968000000000);
            //new DateTime(

            return myDate.ToString();

        }






        private static int counter = -1;
        /// <summary>
        /// Generate random Number
        /// </summary>
        /// <returns>randam number</returns>
        public static int GenerateRandomNo()
        {
            if (counter == -1)
            {
                counter = new Random().Next() & 0xffff;
            }
            counter++;
            return counter;
        }

        /// <summary>
        /// Return string value
        /// </summary>
        /// <param name="obj">value to convert</param>
        /// <returns>Object</returns>
        /// <Author>Harwinder</Author> 
        public static string GetString(object obj)
        {
            if (obj == null)
            {
                return "";
            }
            return obj.ToString();
        }

        /// <summary>
        /// return Int value
        /// </summary>
        /// <param name="obj">value to convert</param>
        /// <returns></returns>
        /// <Author>Harwinder</Author> 
        public static int GetInt(object obj)
        {
            if (obj == null || obj.ToString() == "")
            {
                return 0;
            }
            return int.Parse(obj.ToString());
        }

        /// <summary>
        /// Return database server's Datatype 
        /// </summary>
        /// <param name="strVal"></param>
        /// <param name="strLength"></param>
        /// <returns></returns>
        public static string CheckDataType(string strVal, string strLength)
        {
            string strDataTyp = "";
            switch (strVal)
            {
                case "string":
                    strDataTyp = "nvarchar (" + ((strLength == "0") ? "1" : strLength).ToString() + ")";
                    break;
                case "button":
                    strDataTyp = "char (2)";
                    break;
                case "id":
                    strDataTyp = "int";
                    break;
                case "tabledirect":
                    strDataTyp = "int";
                    break;
                case "yes-no":
                    strDataTyp = "tinyint";
                    break;
                default:
                    strDataTyp = strVal;
                    break;
            }
            return strDataTyp;
        }

        /// <summary>
        /// Validate text according to passed pattren
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="strVal"></param>
        /// <returns></returns>
        public static bool ValidateInput(string txt, string strVal)
        {
            Regex objRegx;
            if (txt == null)
                txt = "";
            switch (strVal)
            {
                case "AlphaNumeric":
                    objRegx = new Regex("^[0-9a-zA-z]");
                    break;
                case "String":
                    objRegx = new Regex("^[a-zA-Z]");
                    break;
                default:
                    objRegx = new Regex("^[0-9]");
                    break;
            }
            return !objRegx.IsMatch(txt);
        }

        public static DateTime? SetDateTimeUTC(DateTime? dateTime)
        {
            if (dateTime != null)
            {
                if (dateTime.Value.Kind == DateTimeKind.Unspecified)
                {
                    dateTime = new DateTime(dateTime.Value.Ticks, DateTimeKind.Local);// .ToUniversalTime().ToLocalTime().ToUniversalTime();
                    dateTime = dateTime.Value.ToUniversalTime();
                }
                else if (dateTime.Value.Kind == DateTimeKind.Local)
                {
                    dateTime = dateTime.Value.ToUniversalTime();
                }

            }
            return dateTime;
        }

        /// <summary>
        /// Containing Validate Options
        /// </summary>
        public enum enmValidation
        {
            Number = 0,
            String = 1,
            AlphaNumeric = 2
        }

        public const char YES = '1';
        public const char NO = '0';

        private static string _machineIP = "";

        /// <summary>
        /// Function to return Machine IP and Port from where link is running
        /// </summary>
        /// <returns>string of Machine IP and Port</returns>
        public static string GetMachineIPPort()
        {
            if (_machineIP != "")
                return _machineIP;
            string hostName = Environment.MachineName; // System.Net.Dns.GetHostName();

            var host = System.Net.Dns.GetHostEntry(hostName);
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    _machineIP = ip.ToString();
                    break;
                }
            }
            int port = HttpContext.Current.Request.Url.Port;
            if (port != 80)
                _machineIP = _machineIP + ":" + port.ToString();
            return _machineIP;
        }

        /// <summary>
        /// Adds specified number of value to current datetime
        /// </summary>
        /// <param name="duration">integer number from CommonFuctions.Calendar enum</param>
        /// <param name="time"></param>
        /// <returns>new date</returns>
        /// <author>Veena</author>
        public static DateTime AddDate(int duration, object time)
        {
            if (duration == EnvConstants.DayOfYear)
            {
                return DateTime.Now.AddDays(Convert.ToDouble(time));
            }
            else if (duration == EnvConstants.Month)
            {
                return DateTime.Now.AddMonths(Utility.Util.GetValueOfInt(time.ToString()));
            }
            else if (duration == EnvConstants.Hour)
            {
                return DateTime.Now.AddHours(Convert.ToDouble(time));
            }
            else if (duration == EnvConstants.Minute)
            {
                return DateTime.Now.AddMinutes(Convert.ToDouble(time));
            }
            else if (duration == EnvConstants.Second)
            {
                return DateTime.Now.AddSeconds(Convert.ToDouble(time));
            }
            else if (duration == EnvConstants.Year)
            {
                return DateTime.Now.AddYears(Utility.Util.GetValueOfInt(time.ToString()));
            }
            return DateTime.Now;
        }
    }
}
