using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using System.Reflection;
using VAdvantage.Logging;

namespace ModelLibrary.Classes
{
    public class AdvanceDocumentType
    {
        #region Private Variables
        private static Dictionary<string, bool> _docModule = new Dictionary<string, bool>();
        [NonSerialized]
        protected static VLogger log = null;
        private static Assembly asm = null;
        private static Type type = null;
        private static MethodInfo methodInfo = null;
        private static string _moduleVersion = "";
        #endregion

        /// <summary>
        /// Checks for the Advance Document Control Module and
        /// Returns True or false based on the conditions
        /// </summary>
        /// <param name="po"></param>
        /// <returns></returns>
        public static bool GetAdvanceDocument(PO po)
        {
            if (log == null)
                log = VLogger.GetVLogger("AdvanceDocumentType");

            LoadAssembly();
            if (methodInfo != null)
            {
                object result = "";
                ParameterInfo[] parameters = methodInfo.GetParameters();
                object classInstance = Activator.CreateInstance(type, null);
                if (parameters.Length == 3)
                {
                    object[] parametersArray = new object[] { po, po.Get_Value("DocAction"), po.Get_Trx() };
                    result = methodInfo.Invoke(classInstance, parametersArray);
                }
                if (Util.GetValueOfString(result) != "")
                {
                    log.Warning("Save failed - ");
                    log.SaveError(Convert.ToString(result), Convert.ToString(result), false, true);
                    return false;
                }
            }
           
            return true;
        }

        /// <summary>
        /// Loads Assembly FADOC and calls class to perform certain operations
        /// </summary>
        private static void LoadAssembly()
        {
            try
            {
                Tuple<String, String,string> aInfo = null;
                if (Env.HasModulePrefix("ED001_", out aInfo))
                {
                   
                    if (methodInfo == null || Env.GetModuleVersion("ED001_") != _moduleVersion)
                    {
                        _moduleVersion = Env.GetModuleVersion("ED001_");
                        string cName = "ViennaAdvantage.Classes.ED001_DocumentControl";
                        asm = System.Reflection.Assembly.Load(aInfo.Item1);
                        type = asm.GetType(cName);
                        if (type != null)
                        {
                            methodInfo = type.GetMethod("ControlDocument");
                        }
                    }
                }
            }
            catch
            {
                _moduleVersion = "";
                methodInfo = null;
            }
        }

    }
}
