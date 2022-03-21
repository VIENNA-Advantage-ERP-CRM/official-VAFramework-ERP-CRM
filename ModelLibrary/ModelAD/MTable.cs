using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Common;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using VAdvantage.SqlExec;
using System.Reflection;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MTable : X_AD_Table
    {

       


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
            " VAdvantage.ProcessEngine.M",
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



       
        private bool isHasKeyColumn = false;
        /**
	 * 	Load Constructor
	 *	@param ctx context
	 *	@param rs result set
	 *	@param trxName transaction
	 */
     

       

       

        /// <summary>
        ///Get PO Class Instance
        /// </summary>
        /// <param name="ctx">context for PO</param>
        /// <param name="rs">Datarow</param>
        /// <param name="trxName">trxName transaction</param>
        /// <returns>PO for Record or null</returns>
        //public PO GetPO(Ctx ctx, DataRow rs, Trx trxName)
        //{
        //    String tableName = GetTableName();
        //    Type clazz = GetClass(tableName);
        //    if (clazz == null)
        //    {
        //        //log.log(Level.SEVERE, "(rs) - Class not found for " + tableName);
        //        //ErrorLog.FillErrorLog("MTable.GetPO", "(rs) - Class not found for " + tableName, "", VAdvantage.Framework.Message.MessageType.ERROR);
        //        //return null;

        //        //Updateby--Raghu
        //        //to run work flow with virtual M_ or X_ classes
        //        log.Log(Level.INFO, "Using GenericPO for " + tableName);
        //        GenericPO po = new GenericPO(tableName, ctx, rs, trxName);
        //        return po;
        //    }
        //    bool errorLogged = false;
        //    try
        //    {
        //        ConstructorInfo constructor = clazz.GetConstructor(new Type[] { typeof(Ctx), typeof(DataRow), typeof(Trx) });
        //        PO po = (PO)constructor.Invoke(new object[] { ctx, rs, trxName });
        //        return po;
        //    }
        //    catch (Exception e)
        //    {
        //        ////ErrorLog.FillErrorLog("MTable.GetPO", "(rs) - Table=" + tableName + ",Class=" + clazz, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
        //        log.Log(Level.SEVERE, "(rs) - Table=" + tableName + ",Class=" + clazz, e);
        //        errorLogged = true;
        //        log.SaveError("Error", "Table=" + tableName + ",Class=" + clazz);
        //    }
        //    if (!errorLogged)
        //    {
        //        ////ErrorLog.FillErrorLog("Mtable", "(rs) - Not found - Table=" + tableName, "", VAdvantage.Framework.Message.MessageType.INFORMATION);
        //        log.Log(Level.SEVERE, "(rs) - Not found - Table=" + tableName);
        //    }
        //    return null;
        //}

        

        //public PO GetPO(Ctx ctx, int Record_ID, Trx trxName, bool isNew)
        //{
        //    //return GetPO(ctx, Record_ID, trxName, true);
        //    string tableName = GetTableName();
        //    if (Record_ID != 0 && !IsSingleKey())
        //    {
        //        log.Log(Level.WARNING, "(id) - Multi-Key " + tableName);
        //        return null;

        //        //Updateby--Raghu
        //        //to run work flow with virtual M_ or X_ classes
        //        //log.Log(Level.INFO, "Using GenericPO for " + tableName);
        //        //GenericPO po = new GenericPO(tableName, ctx, Record_ID, trxName);
        //        //return po;
        //    }

        //    Type className = GetClass(tableName);

        //    if (className == null)
        //    {
        //        //log.log(Level.WARNING, "(id) - Class not found for " + tableName);
        //        //to run work flow with virtual M_ or X_ classes
        //        log.Log(Level.INFO, "Using GenericPO for " + tableName);
        //        GenericPO po = new GenericPO(tableName, ctx, Record_ID, trxName);
        //        return po;
        //    }
        //    bool errorLogged = false;
        //    try
        //    {
        //        ConstructorInfo constructor = null;
        //        try
        //        {
        //            constructor = className.GetConstructor(new Type[] { typeof(Ctx), typeof(int), typeof(Trx) });
        //        }
        //        catch (Exception e)
        //        {
        //            log.Warning("No transaction Constructor for " + className.FullName + " (" + e.ToString() + ")");
        //        }

        //        if (constructor != null)
        //        {
        //            PO po = (PO)constructor.Invoke(new object[] { ctx, Record_ID, trxName });
        //            //	Load record 0 - valid for System/Client/Org/Role/User
        //            if (!isNew && Record_ID == 0)
        //                po.Load(trxName);
        //            //	Check if loaded correctly
        //            if (po != null && po.Get_ID() != Record_ID && IsSingleKey())
        //            {
        //                // Common.//ErrorLog.FillErrorLog("MTable", "", po.Get_TableName() + "_ID=" + po.Get_ID() + " <> requested=" + Record_ID, VAdvantage.Framework.Message.MessageType.INFORMATION);
        //                return null;
        //            }
        //            return po;
        //        }
        //        else
        //        {
        //            throw new Exception("No Std Constructor");
        //        }
        //    }
        //    catch (Exception ex1)
        //    {
        //        log.Severe(ex1.ToString());
        //        //exception handling
        //    }
        //    if (!errorLogged)
        //    {
        //        //log.log(Level.SEVERE, "(id) - Not found - Table=" + tableName
        //        //    + ", Record_ID=" + Record_ID);
        //    }
        //    return null;
        //}


    

        /// <summary>
        /// function to check whether table has the key column
        /// </summary>
        /// <returns>true/false</returns>
        public bool HasPKColumn()
        {
            GetColumns(false);
            return isHasKeyColumn;
        }


       



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

            //	Import Tables (Name conflict)
            if (tableName.StartsWith("I_"))
            {
                Type className = GetPOclass("VAdvantage.Process.X_" + tableName);
                if (className != null)
                {
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
                        return clazzsn;
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
                    return clazzsn;


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
                            return clazzsn;
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

                Type clazzsn = GetPOclass(namspace);
                if (clazzsn != null)
                    return clazzsn;
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
                    baseClass = type.BaseType;

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

                Type classObject = Type.GetType(className);

                Type baseClass = null;
                if (classObject != null)
                    baseClass = classObject.BaseType;

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


        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        /// 


       

        


    }
}
