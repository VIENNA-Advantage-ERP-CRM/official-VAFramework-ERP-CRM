using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Login
{
    public abstract class LanguageProcess : LanguageCall
    {
        abstract public string GetString(string key);

        /// <summary>
        /// Gets the resource
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>resourcce text</returns>
        public string GetResource(string key)
        {
            string msg = null;
            try
            {
                msg = GetString(key);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return msg;
        }
    }
}
