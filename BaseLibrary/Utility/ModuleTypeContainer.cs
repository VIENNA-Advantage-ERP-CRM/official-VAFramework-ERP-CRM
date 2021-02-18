/********************************************************
 * Module Name    : Vienna Framework
 * Purpose        : Load Module Class/Type
 * Author         : Harwinder Singh
 * Date           : 03-Nov-2009
 ******************************************************/
using System;
using VAdvantage.DataBase;
using VAdvantage.Logging;

namespace VAdvantage.Utility
{
    public class ModuleTypeConatiner
    {
        /// <summary>
        /// store Type of class 
        /// </summary>
        static VAdvantage.Classes.CCache<String, Type> _typeModule = new Classes.CCache<string, Type>("TypeCache", 0);

        static VAdvantage.Classes.CCache<String, Type> _typeAssembles = new Classes.CCache<string, Type>("TypeAssembliesCache", 0);

        #region Get TypeAt RunTime
        /// <summary>
        /// Get class type object from assemblies 
        /// <para>look in ViennaAdvantage</para>
        /// <para> look in Modules (if has matching prefix)</para>
        /// <para>look in base Assembly</para>
        /// </summary>
        /// <param name="asmName">Name of assembly</param>
        /// <param name="fullyQlfiedclassame">fully qalified Class name (eg ViennaAdvantage.Aforms.TestForm)</param>
        /// <param name="name">name of type( often has module prefix)</param>
        /// <returns>class type else null </returns>
        /// 
        public static Type GetClassType(string fqClassame, string name = "", string className = "", bool ignoreModuleAsemblies = false)
        {
            Type type = null;
            System.Reflection.Assembly asm = null;

            //step 1 ViennaAdvantage
            try
            {
                asm = System.Reflection.Assembly.Load(GlobalVariable.PRODUCT_NAME);
                type = asm.GetType(fqClassame);
            }
            catch
            {
                //blank
            }

            //End ViennaAdvanatage

            //step 2 Module 
            if (type == null && !ignoreModuleAsemblies)
            {
                try
                {
                    Tuple<String, String, String> aInfo = null;
                    if (Env.HasModulePrefix(name, out aInfo))
                    {
                        asm = System.Reflection.Assembly.Load(aInfo.Item1);
                        type = asm.GetType(fqClassame);
                    }
                    else if (!string.IsNullOrEmpty(className) && Env.HasModulePrefix(className, out aInfo))
                    {
                        asm = System.Reflection.Assembly.Load(aInfo.Item1);
                        type = asm.GetType(fqClassame);
                    }
                }
                catch
                {
                    // blank
                }
            }
            // END Module
            //step 3
            if (type == null)
            {
                type= Env.GetTypeFroMVAMPackaging(fqClassame);
            }
            else
            {
                //customization
            }
            return type;
        }
        #endregion
    }

    /********************************************************
 * Module Name    : Vienna Frameowrk
 * Purpose        : Load and cache class type 
 * Chronological Development
 * Harwinder     19-Aug-2020
 ******************************************************/
    public class ClassTypeContainer
    {

        /// <summary>
        /// store Type of class 
        /// </summary>
        static VAdvantage.Classes.CCache<String, Type> _classType = new Classes.CCache<string, Type>("VISClassTypeCache", 0);


        static VAdvantage.Classes.CCache<String, Type> _typeAssembles = new Classes.CCache<string, Type>("CISClassTypeAssembliesCache", 0);


        /// <summary>
        ///  Get TypeAt RunTime
        /// <summary>
        /// <param name="fqClassame">fully qualified class name</param>
        /// <param name="asmName">Assembly name</param>
        /// <param name="useCache">use cache default is true</param>
        /// <returns>class type</returns>
        public static Type GetClassType(string fqClassame, string asmName, bool useCache = true)
        {

            Type type = null;
            System.Reflection.Assembly asm = null;
            string key = fqClassame + "_" + asmName;


            if (useCache && _classType.ContainsKey(key))
                return _classType[key];


            //step 1 ViennaAdvantage
            try
            {
                asm = System.Reflection.Assembly.Load(asmName);
                type = asm.GetType(fqClassame);
            }
            catch
            {
                VLogger.Get().Warning("Error loading type " + fqClassame);
            }
            if (type != null && useCache)
            {
                if (_classType.ContainsKey(key))
                    _classType[key] = type;
                else
                    _classType.Add(key, type);
            }
            return type;
        }
       
    }
}
