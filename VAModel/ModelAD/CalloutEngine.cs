using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Reflection;
using VAdvantage.Classes;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.Controller;

namespace VAdvantage.Model
{
    public class CalloutEngine : Callout
    {
        private static bool _calloutActive = false;
        /** Logger					*/
        protected VLogger log = null;

        public CalloutEngine()
            : base()
        {
            log = VLogger.GetVLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Start Callout.
        ///	Callout's are used for cross field validation and setting values in other fields
        ///	when returning a non empty (error message) string, an exception is raised
        ///	When invoked, the Tab model has the new value!
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="methodName">method name</param>
        /// <param name="windowNo">current window number</param>
        /// <param name="tab">model Tab</param>
        /// <param name="field">modal field</param>
        /// <param name="newValue">new value</param>
        /// <param name="oldValue">old value</param>
        /// <returns>Error message or ""</returns>
        public String Start(Ctx ctx, String methodName, int windowNo, GridTab tab,
            GridField field, Object newValue, Object oldValue)
        {
            if (methodName == null || methodName.Length == 0)
                throw new ArgumentException("No Method Name");
            //
            String retValue = "";
            StringBuilder msg = new StringBuilder(methodName).Append(" - ")
                .Append(field.GetColumnName())
                .Append("=").Append(newValue)
                .Append(" (old=").Append(oldValue)
                .Append(") {active=").Append(IsCalloutActive()).Append("}");
            if (!IsCalloutActive())
            {
                log.Info(msg.ToString());
            }

            //	Find Method
            MethodInfo method = GetMethod(methodName);
            if (method == null)
                throw new ArgumentException("Method not found: " + methodName);
            int argLength = method.GetParameters().Length;
            if (!(argLength == 5 || argLength == 6))
                throw new ArgumentException("Method " + methodName + " has invalid no of arguments: " + argLength);

            //	Call Method
            try
            {
                Object[] args = null;
                if (argLength == 6)
                    args = new Object[] { ctx, windowNo, tab, field, newValue, oldValue };
                else
                    args = new Object[] { ctx, windowNo, tab, field, newValue };
                retValue = (String)method.Invoke(this, args);
            }
            catch (Exception e)
            {
                SetCalloutActive(false);
                //Throwable ex = e.getCause();	//	InvocationTargetException
                //if (ex == null)
                //    ex = e;
                ////log.log(Level.SEVERE, "start: " + methodName, ex);
                //ex.printStackTrace(System.err);
                //retValue = ex.getLocalizedMessage();

                retValue = e.Message;
            }
            return retValue;
        }

        /// <summary>
        /// Conversion Rules. Convert a String
        /// </summary>
        /// <param name="methodName">in notation User_Function</param>
        /// <param name="value">the value</param>
        /// <returns>converted String or Null if no method found</returns>
        public String Convert(String methodName, String value)
        {
            if (methodName == null || methodName.Length == 0)
                throw new ArgumentException("No Method Name");
            //
            String retValue = null;
            StringBuilder msg = new StringBuilder(methodName).Append(" - ").Append(value);
            log.Info(msg.ToString());
            //
            //	Find Method
            MethodInfo method = GetMethod(methodName);
            if (method == null)
                throw new ArgumentException("Method not found: " + methodName);
            int argLength = method.GetParameters().Length;
            if (argLength != 1)
                throw new ArgumentException("Method " + methodName + " has invalid no of arguments: " + argLength);

            //	Call Method
            try
            {
                Object[] args = new Object[] { value };
                retValue = (String)method.Invoke(this, args);
            }
            catch (Exception e)
            {
                SetCalloutActive(false);
                log.Log(Level.SEVERE, "convert: " + methodName, e);
                //e.printStackTrace(System.err);
            }
            return retValue;
        }

        /// <summary>
        /// Get Method
        /// </summary>
        /// <param name="methodName">method name</param>
        /// <returns>method or null</returns>
        private MethodInfo GetMethod(String methodName)
        {
            //MethodInfo[] allMethods = getClass().getMethods();
            MethodInfo[] allMethods = GetType().GetMethods();

            for (int i = 0; i < allMethods.Length; i++)
            {
                if (methodName.ToLower().Equals(allMethods[i].Name.ToLower()))
                    return allMethods[i];
            }
            return null;
        }

        /// <summary>
        /// Is Callout Active
        /// </summary>
        /// <returns>true if active</returns>
        public static bool IsCalloutActive()
        {
            return _calloutActive;
        }

        /// <summary>
        /// Set Callout (in)active
        /// </summary>
        /// <param name="active">active</param>
        protected static void SetCalloutActive(bool active)
        {
            _calloutActive = active;
        }

        /// <summary>
        /// Set Account Date to the date of the calling column.
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="windowNo">window no</param>
        /// <param name="tab">tab</param>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <returns>null or error message</returns>
        public String DateAcct(Ctx ctx, int windowNo, GridTab tab, GridField field, Object value)
        {
            if (IsCalloutActive())		//	assuming it is resetting value
                return "";
            //	setCalloutActive(true);
            if (value == null || !(value.GetType() == typeof(DateTime)))
                return "";
            tab.SetValue("DateAcct", value);
            //	setCalloutActive(false);
            return "";
        }

        /// <summary>
        /// Rate - set Multiply Rate from Divide Rate and vice versa
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="windowNo">window no</param>
        /// <param name="tab">tab</param>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <returns>null or error message</returns>
        public String Rate(Ctx ctx, int windowNo, GridTab tab, GridField field, Object value)
        {
            if (IsCalloutActive() || value == null)		//	assuming it is Conversion_Rate
            {
                return "";
            }
            SetCalloutActive(true);

            Decimal rate1 = (Decimal)value;
            Decimal rate2 = Utility.Env.ZERO;
            Decimal one = new Decimal(1.0);

            if (System.Convert.ToDouble(rate1) != 0.0)	//	no divide by zero
            {
                rate2 = Decimal.Round(Decimal.Divide(one, rate1), 12, MidpointRounding.AwayFromZero);
            }
            //
            if (field.GetColumnName().Equals("MultiplyRate"))
            {
                tab.SetValue("DivideRate", rate2);
            }
            else
            {
                tab.SetValue("MultiplyRate", rate2);
            }
            log.Info(field.GetColumnName() + "=" + rate1 + " => " + rate2);
            SetCalloutActive(false);
            return "";
        }
    }
}
