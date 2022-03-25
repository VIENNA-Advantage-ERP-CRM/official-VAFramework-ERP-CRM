using BaseLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Reflection;

namespace VAModelAD.Classes
{

    public class ModelFactoryLoader
    {
        private static ModelFactoryLoader _loader = null;
        private List<IModelFactory> _modelList = new List<IModelFactory>();

        private ModelFactoryLoader()
        {
            _modelList.Clear();
            _modelList.Add(new ModelFactory());
        }

        public static List<IModelFactory> GetList()
        {
            if (_loader == null)
                _loader = new ModelFactoryLoader();
            return _loader._modelList;
        }
    }


    class ModelFactory : IModelFactory
    {
        private static CCache<String, Type> s_classCache = new CCache<String, Type>("PO_Class", 20);
        private static VLogger log = VLogger.GetVLogger(typeof(ModelFactory).Name);



        /**	Special Classes				*/
        private static String[] _special = new String[] {
        "AD_Element", "VAdvantage.Model.M_Element",
        "AD_Registration", "VAdvantage.Model.M_Registration",
        "AD_Tree", "VAdvantage.Model.MTree_Base",
        "R_Category", "VAdvantage.Model.MRequestCategory",
        "GL_Category", "VAdvantage.Model.MGLCategory",
        "K_Category", "VAdvantage.Model.MKCategory",
        "C_ValidCombination", "VAdvantage.Model.MAccount",
        "C_Phase", "VAdvantage.Model.MProjectTypePhase",
        "C_Task", "VAdvantage.Model.MProjectTypeTask",
        "K_Source", "VAdvantage.Model.X_K_Source",
        "RC_ViewColumn","VAdvantage.Model.X_RC_ViewColumn",
	//	AD_Attribute_Value, AD_TreeNode
	    };

        private static String[] _projectClasses = new String[]{
            "ViennaAdvantage.Model.M",
            "ViennaAdvantage.Process.M",
            "ViennaAdvantage.CMFG.Model.M",
            "ViennaAdvantage.CMRP.Model.M",
            "ViennaAdvantage.CWMS.Model.M",
            "ViennaAdvantage.Model.X_"

        };

        private static String[] _productClasses = new String[]{
            "VAdvantage.Model.M",
            "VAdvantage.Model.X_",
            "VAdvantage.Process.M",
            "VAdvantage.WF.M",
            "VAdvantage.Report.M",
            "VAdvantage.ProcessEngine.M",
            "VAdvantage.Print.M"

        };


        private static String[] _moduleClasses = new String[]{
            ".Model.M",
            ".Process.M",
            ".WF.M",
            ".Report.M",
            ".Print.M",
            ".CMFG.Model.M",
            ".CMRP.Model.M",
            ".CWMS.Model.M",
            ".Model.X_",

        };



        /// <summary>
        /// Get Persistency Class for table
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public Type GetClass(string tableName)
        {
            //	Not supported
            if (tableName == null || tableName.EndsWith("_Trl"))
                return null;
            //check cache
            Type cache = s_classCache[tableName];
            if (cache != null)
            {
                return cache;
            }

            //	Import Tables (Name conflict)
            if (tableName.StartsWith("I_"))
            {
                Type className = GetPOclass("VAdvantage.Process.X_" + tableName);
                if (className != null)
                {
                    s_classCache.Add(tableName, className);
                    return className;
                }
                log.Warning("No class for table: " + tableName);
                return null;
            }

            //Special Naming
            for (int i = 0; i < _special.Length; i++)
            {
                if (_special[i++].Equals(tableName))
                {
                    Type clazzsn = GetPOclass(_special[i]);
                    if (clazzsn != null)
                    {
                        s_classCache.Add(tableName, clazzsn);
                        return clazzsn;
                    }
                    break;
                }
            }

            //	Strip table name prefix (e.g. AD_) Customizations are 3/4
            String classNm = tableName;
            int index = classNm.IndexOf('_');
            if (index > 0)
            {
                if (index < 3)		//	AD_, A_
                    classNm = classNm.Substring(index + 1);
                else
                {
                    String prefix = classNm.Substring(0, index);
                    if (prefix.Equals("Fact"))		//	keep custom prefix
                        classNm = classNm.Substring(index + 1);
                }
            }
            //	Remove underlines
            classNm = classNm.Replace("_", "");

            //	Search packages
            //String[] packages = getPackages(GetCtx());
            //for (int i = 0; i < packages.length; i++)
            //{

            string namspace = "";

            /*********** Module Section  **************/

            Tuple<String, String, String> moduleInfo;
            Assembly asm = null;

            //////Check MClasses through list
            for (int i = 0; i < _projectClasses.Length; i++)
            {
                namspace = _projectClasses[i] + classNm;
                if (_projectClasses.Contains("X_"))
                {
                    namspace = _projectClasses[i] + tableName;
                }

                Type clazzsn = GetFromCustomizationPOclass(namspace);
                if (clazzsn != null)
                {
                    s_classCache.Add(tableName, clazzsn);
                    return clazzsn;
                }
            }

            //Modules
            if (Env.HasModulePrefix(tableName, out moduleInfo))
            {
                asm = GetAssembly(moduleInfo.Item1);
                if (asm != null)
                {
                    for (int i = 0; i < _moduleClasses.Length; i++)
                    {
                        namspace = moduleInfo.Item2 + _moduleClasses[i] + classNm;
                        if (_moduleClasses.Contains("X_"))
                        {
                            namspace = moduleInfo.Item2 + _moduleClasses[i] + tableName;
                        }
                        Type clazzsn = GetClassFromAsembly(asm, namspace);
                        if (clazzsn != null)
                        {
                            s_classCache.Add(tableName, clazzsn);
                            return clazzsn;
                        }
                    }
                }
            }
            /********  End  **************/
            for (int i = 0; i < _productClasses.Length; i++)
            {
                namspace = _productClasses[i] + classNm;
                if (_productClasses.Contains("X_"))
                {
                    namspace = _productClasses[i] + tableName;
                }

                Type clazzsn = Env.GetTypeFromPackage(namspace);

                //Type clazzsn = GetPOclass(namspace);
                if (clazzsn != null)
                {
                    s_classCache.Add(tableName, clazzsn);
                    return clazzsn;
                }
            }
            return null;
        }

        public PO GetPO(string tableName, Ctx ctx, DataRow rs, Trx trxName)
        {
            Type clazz = GetClass(tableName);
            if (clazz == null)
            {
                return null;
            }
            bool errorLogged = false;
            try
            {
                ConstructorInfo constructor = clazz.GetConstructor(new Type[] { typeof(Ctx), typeof(DataRow), typeof(Trx) });
                PO po = (PO)constructor.Invoke(new object[] { ctx, rs, trxName });
                return po;
            }
            catch (Exception e)
            {
                ////ErrorLog.FillErrorLog("MTable.GetPO", "(rs) - Table=" + tableName + ",Class=" + clazz, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
                log.Log(Level.SEVERE, "(rs) - Table=" + tableName + ",Class=" + clazz, e);
                errorLogged = true;
                log.SaveError("Error", "Table=" + tableName + ",Class=" + clazz);
            }
            if (!errorLogged)
            {
                ////ErrorLog.FillErrorLog("Mtable", "(rs) - Not found - Table=" + tableName, "", VAdvantage.Framework.Message.MessageType.INFORMATION);
                log.Log(Level.SEVERE, "(rs) - Not found - Table=" + tableName);
            }
            return null;
        }

        public PO GetPO(string tableName, Ctx ctx, int Record_ID, Trx trxName)
        {
            Type clazz = GetClass(tableName);
            if (clazz == null)
            {
                return null;
            }
            bool errorLogged = false;
            try
            {
                ConstructorInfo constructor = clazz.GetConstructor(new Type[] { typeof(Ctx), typeof(int), typeof(Trx) });
                PO po = (PO)constructor.Invoke(new object[] { ctx, Record_ID, trxName });
                return po;
            }
            catch (Exception e)
            {
                ////ErrorLog.FillErrorLog("MTable.GetPO", "(rs) - Table=" + tableName + ",Class=" + clazz, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
                log.Log(Level.SEVERE, "(rs) - Table=" + tableName + ",Class=" + clazz, e);
                errorLogged = true;
                log.SaveError("Error", "Table=" + tableName + ",Class=" + clazz);
            }
            if (!errorLogged)
            {
                ////ErrorLog.FillErrorLog("Mtable", "(rs) - Not found - Table=" + tableName, "", VAdvantage.Framework.Message.MessageType.INFORMATION);
                log.Log(Level.SEVERE, "(rs) - Not found - Table=" + tableName);
            }
            return null;
        }





        /// <summary>
        /// Get Assembly 
        /// </summary>
        /// <param name="AssemblyInfo"> Assembly name And Version</param>
        /// <returns></returns>
        private Assembly GetAssembly(string AssemblyInfo)
        {
            Assembly asm = null;
            try
            {
                asm = Assembly.Load(AssemblyInfo);
            }
            catch (Exception e)
            {
                log.Info(e.Message);
                asm = null;
            }
            return asm;
        }

        /// <summary>
        /// Get Class From Assembly
        /// </summary>
        /// <param name="asm">Assembly</param>
        /// <param name="className">Fully Qulified Class Name</param>
        /// <returns>Class Object</returns>
        private Type GetClassFromAsembly(Assembly asm, string className)
        {
            Type type = null;
            try
            {
                type = asm.GetType(className);
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, e.Message);
            }

            if (type == null)
            {
                return null;
            }

            Type baseClass = null;
            if (type != null)
            {
                baseClass = type.BaseType;
            }

            while (baseClass != null)
            {
                if (baseClass == typeof(PO))
                {
                    return type;
                }
                baseClass = baseClass.BaseType;
            }
            return null;
        }



        /// <summary>
        /// Get type from ViennaAdvnatgeSVC/Model
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private Type GetFromCustomizationPOclass(string className)
        {

            try
            {
                Assembly asm = null;
                Type type = null;

                try
                {
                    asm = Assembly.Load(GlobalVariable.PRODUCT_NAME);
                    type = asm.GetType(className);
                }
                catch
                {
                    // asm = Assembly.Load(GlobalVariable.ASSEMBLY_NAME);
                }

                /*EndCustomization*/

                if (type == null)
                {
                    type = Type.GetType(className);
                }

                Type baseClass = null;
                if (type != null)
                {
                    baseClass = type.BaseType;
                }

                while (baseClass != null)
                {
                    if (baseClass == typeof(PO))
                    {
                        return type;
                    }
                    baseClass = baseClass.BaseType;
                }
            }
            catch
            {
                log.Finest("Not found: " + className);
            }

            return null;

        }

        /// <summary>
        ///  Get PO class
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private Type GetPOclass(string className)
        {
            try
            {

                Type classObject = Env.GetTypeFromPackage(className);//    Type.GetType(className);

                Type baseClass = classObject.BaseType;

                while (baseClass != null)
                {
                    if (baseClass == typeof(PO))
                    {
                        return classObject;
                    }
                    baseClass = baseClass.BaseType;
                }
            }
            catch
            {
                log.Finest("Not found: " + className);
            }

            return null;
        }	//	getPOclass

    }
}