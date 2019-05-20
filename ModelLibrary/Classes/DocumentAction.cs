using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Classes
{
    class DocumentAction
    {
        public bool MoveDocument(int VADMS_Document_ID,int VADMS_Folder_ID, int VADMS_Folder_ID_1, Ctx _ctx,out string resultstr)
        {
            
            try
            {
                Assembly assembly = Assembly.Load("VADMSSvc");
                Type type = assembly.GetType("ViennaAdvantage.Classes.DocumentOperation");
                if (type != null)
                {
                    MethodInfo methodInfo = type.GetMethod("MoveDocument");
                    if (methodInfo != null)
                    {
                        object result = null;
                        ParameterInfo[] parameters = methodInfo.GetParameters();
                        object classInstance = Activator.CreateInstance(type, null);
                        if (parameters.Length == 0)
                        {
                            result = methodInfo.Invoke(classInstance, null);
                        }
                        else
                        {
                           // GetPO(Get_TrxName());
                            //int sourceFolderID = VADMS_Folder_ID_1;
                            //int docID = (int)_po.Get_Value("VADMS_Document_ID");
                            System.Collections.Generic.List<System.Int32> metaDataID = new System.Collections.Generic.List<System.Int32>() { VADMS_Document_ID };
                            object[] parametersArray = new object[] 
                            { 
                                metaDataID,
                                VADMS_Folder_ID, 
                                (VADMS_Folder_ID_1 == 0 ? -1 : VADMS_Folder_ID_1).ToString(), 
                                false,
                                //(Dictionary<string, string>)_ctx.GetMap() ,
                                _ctx ,
                                true
                            };
                            result = methodInfo.Invoke(classInstance, parametersArray);
                        }
                        if (result == null)
                        {
                            resultstr="Mehtod Invoked Successfully.";
                        }
                        else
                        {
                            resultstr="Mehtod Invoked Successfully:" + result.ToString();
                        }
                        return true;
                    }
                    resultstr="Cant Get Method of ViennaAdvantage.Classes.DocumentOperation From VADMSSvc File.";                   
                }
                resultstr="Cant Get Type of ViennaAdvantage.Classes.DocumentOperation From VADMSSvc File.";
            }
            catch (Exception ex)
            {
                resultstr="Error Occured During Move Document:" + ex.Message;
                return false;
            }
            return false;
        }

        public bool ForwardDocument(List<int> users,int VADMS_Document_ID, Ctx _ctx, out string resultstr)
        {
            try
            {
                StringBuilder userNames = new StringBuilder();
                Assembly assembly = Assembly.Load("VADMSSvc");
                Type type = assembly.GetType("ViennaAdvantage.Classes.DocumentOperation");
                if (type != null)
                {
                    MethodInfo methodInfo = type.GetMethod("ForwardDocument");
                    if (methodInfo != null)
                    {
                        object result = null;
                        ParameterInfo[] parameters = methodInfo.GetParameters();
                        object classInstance = Activator.CreateInstance(type, null);
                        if (parameters.Length == 0)
                        {
                            result = methodInfo.Invoke(classInstance, null);
                        }
                        else
                        {
                            //GetPO(Get_TrxName());
                            //int sourceFolderID = _node.GetVADMS_Folder_ID_1();
                            //int docID = (int)_po.Get_Value("VADMS_Document_ID");
                            System.Collections.Generic.List<System.Int32> metaDataID = new System.Collections.Generic.List<System.Int32>() { VADMS_Document_ID };
                            //users = GetRecipientUser();
                            object[] parametersArray = new object[] 
                            { 
                                users,
                                _ctx.GetAD_User_ID(),
                                metaDataID, 
                                null, 
                                //(Dictionary<string, string>)_ctx.GetMap(),
                                _ctx,
                                false,
                                null,
                                true
                            };
                            result = methodInfo.Invoke(classInstance, parametersArray);
                        }

                        if (users != null && users.Count > 0)
                        {
                            X_AD_User user = null;
                            for (int i = 0; i < users.Count; i++)
                            {
                                user = new X_AD_User(_ctx, users[i], null);
                                userNames.Append(user.GetName() + ",");

                            }

                        }
                        if (result == null)
                        {
                            resultstr=("Mehtod Invoked Successfully. Dcoument Forward to :" + userNames.ToString());
                        }
                        else
                        {
                            resultstr = ("Mehtod Invoked Successfully:" + result.ToString() + ". Dcoument Forward to :" + userNames.ToString());
                        }
                        return true;
                    }
                    resultstr = ("Cant Get Method of ViennaAdvantage.Classes.DocumentOperation From VADMSSvc File.");
                }
                resultstr = ("Cant Get Type of ViennaAdvantage.Classes.DocumentOperation From VADMSSvc File.");

            }
            catch (Exception ex)
            {
                resultstr = ("Error Occured During Forward Document:" + ex.Message);
                return false;
            }
            return false;
        }

        public bool AllocateAccess(List<int> users,List<int> roles, int VADMS_Document_ID,string access, Ctx _ctx, out string resultstr)
        {
            try
            {
                Assembly assembly = Assembly.Load("VADMSSvc");
                Type type = assembly.GetType("ViennaAdvantage.Classes.DocumentOperation");
                if (type != null)
                {
                    MethodInfo methodInfo = type.GetMethod("AllocateDocAccesstoUser");
                    if (methodInfo != null)
                    {
                        object result = null;
                        ParameterInfo[] parameters = methodInfo.GetParameters();
                        object classInstance = Activator.CreateInstance(type, null);
                        if (parameters.Length == 0)
                        {
                            result = methodInfo.Invoke(classInstance, null);
                        }
                        else
                        {
                            // GetPO(Get_TrxName());
                            //int sourceFolderID = VADMS_Folder_ID_1;
                            //int docID = (int)_po.Get_Value("VADMS_Document_ID");
                            //System.Collections.Generic.List<System.Int32> metaDataID = new System.Collections.Generic.List<System.Int32>() { VADMS_Document_ID };
                            //object[] parametersArray = new object[] { users,roles, VADMS_Document_ID,access,(Dictionary<string, string>)_ctx.GetMap(),true };
                            object[] parametersArray = new object[] { users, roles, VADMS_Document_ID, access,_ctx, true };
                            result = methodInfo.Invoke(classInstance, parametersArray);
                        }
                        if (result == null)
                        {
                            resultstr = "Mehtod Invoked Successfully.";
                        }
                        else
                        {
                            resultstr = "Mehtod Invoked Successfully:" + result.ToString();
                        }
                        return true;
                    }
                    resultstr = "Cant Get Method of ViennaAdvantage.Classes.DocumentOperation From VADMSSvc File.";
                }
                resultstr = "Cant Get Type of ViennaAdvantage.Classes.DocumentOperation From VADMSSvc File.";
            }
            catch (Exception ex)
            {
                resultstr = "Error Occured During Allocate Acess to Document:" + ex.Message;
                return false;
            }
            return false;
        }

        public bool IsDocumentContainsText(int docID, string value, string AD_Language, out string retVal)
        {
            try
            {

                Assembly assembly = Assembly.Load("VADMSSvc");
                Type type = assembly.GetType("ViennaAdvantage.Classes.DocumentOperation");
                if (type != null)
                {
                    MethodInfo methodInfo = type.GetMethod("DocumentContainText");
                    if (methodInfo != null)
                    {
                        object result = null;
                        ParameterInfo[] parameters = methodInfo.GetParameters();
                        object classInstance = Activator.CreateInstance(type, null);
                        if (parameters.Length == 0)
                        {
                            result = methodInfo.Invoke(classInstance, null);
                        }
                        else
                        {

                            object[] parametersArray = new object[] { value, AD_Language, docID };
                            result = methodInfo.Invoke(classInstance, parametersArray);
                        }
                        if (result == null)
                        {
                            retVal = ("Mehtod Not Invoked Successfully:ViennaAdvantage.Classes.DocumentOperation.DocumentContainText");
                            return false;
                        }
                        else if (result.ToString() == "Y")
                        {
                            retVal = ("DocumentContainText Invoked Successfully, Result Is" + result.ToString());
                            return true;
                        }
                        else
                        {
                            retVal = ("DocumentContainText Invoked Successfully, Result Is" + result.ToString());
                            return false;
                        }
                    }
                    retVal = ("Cant Get Method of ViennaAdvantage.Classes.DocumentOperation From VADMSSvc File.");
                }
                retVal = ("Cant Get Type of ViennaAdvantage.Classes.DocumentOperation From VADMSSvc File.");

            }
            catch (Exception ex)
            {
                retVal = ("Error Occured During ConditionEvaluation of DocumentContainText:" + ex.Message);
                return false;
            }
            return false;
        }


        public bool SaveActionLog(Ctx _ctx, int documentID,int fromUser,int toUser,
                            string description,string emailTo,Trx trxName, out string resultstr)
        {
            try
            {
                Assembly assembly = Assembly.Load("VADMSSvc");
                Type type = assembly.GetType("ViennaAdvantage.Classes.DocumentOperation");
                if (type != null)
                {
                    MethodInfo methodInfo = type.GetMethod("SaveActionLog");
                    if (methodInfo != null)
                    {
                        object result = null;
                        ParameterInfo[] parameters = methodInfo.GetParameters();
                        object classInstance = Activator.CreateInstance(type, null);
                        if (parameters.Length == 0)
                        {
                            result = methodInfo.Invoke(classInstance, null);
                        }
                        else
                        {
                            // GetPO(Get_TrxName());
                            //int sourceFolderID = VADMS_Folder_ID_1;
                            //int docID = (int)_po.Get_Value("VADMS_Document_ID");
                            //System.Collections.Generic.List<System.Int32> metaDataID = new System.Collections.Generic.List<System.Int32>() { VADMS_Document_ID };
                            object[] parametersArray = new object[] {
                                //(Dictionary<string, string>)_ctx.GetMap(),
                                _ctx,
                                documentID,
                                "WFEM",
                                 fromUser,
                                 toUser,
                                 description,
                                 emailTo,
                                 trxName};
                            result = methodInfo.Invoke(classInstance, parametersArray);
                        }
                        if (result == null)
                        {
                            resultstr = "Mehtod Invoked Successfully.";
                        }
                        else
                        {
                            resultstr = "Mehtod Invoked Successfully:" + result.ToString();
                        }
                        return true;
                    }
                    resultstr = "Cant Get Method of ViennaAdvantage.Classes.DocumentOperation From VADMSSvc File.";
                }
                resultstr = "Cant Get Type of ViennaAdvantage.Classes.DocumentOperation From VADMSSvc File.";
            }
            catch (Exception ex)
            {
                resultstr = "Error Occured During SaveActionLog:" + ex.Message;
                return false;
            }
            return false;
        }

    }
}
