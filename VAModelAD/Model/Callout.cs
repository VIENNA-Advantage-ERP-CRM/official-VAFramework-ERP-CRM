/********************************************************
 * Module Name    : Callout
 * Purpose        : 
 * Class Used     : 
 * Chronological Development
 * Veena Pandey     11-May-2009
 ******************************************************/

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
    /// <summary>
    /// Callout Interface for Callout.
    /// </summary>
    public interface Callout
    {
        /// <summary>
        /// Start Callout.
        ///	Callout's are used for cross field validation and setting values in other fields
        ///	when returning a non empty (error message) string, an exception is raised
        ///	When invoked, the Tab model has the new value!
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="method">method name</param>
        /// <param name="windowNo">current window number</param>
        /// <param name="tab">model Tab</param>
        /// <param name="field">modal field</param>
        /// <param name="newValue">new value</param>
        /// <param name="oldValue">old value</param>
        /// <returns>Error message or ""</returns>
        String Start(Ctx ctx, String method, int windowNo, GridTab tab,
                GridField field, Object newValue, Object oldValue);

        /// <summary>
        /// Conversion Rules. Convert a String
        /// </summary>
        /// <param name="method">in notation User_Function</param>
        /// <param name="value">the value</param>
        /// <returns>converted String or Null if no method found</returns>
        String Convert(String method, String value);
    }



}