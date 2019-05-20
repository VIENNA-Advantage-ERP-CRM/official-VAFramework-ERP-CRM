/********************************************************
 * Module Name    : Workflow
 * Purpose        : 
 * Class Used     : X_AD_WF_NextCondition
 * Chronological Development
 * Veena Pandey     02-May-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Model;
//using System.Numeric;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using System.Data;
using System.Reflection;

namespace VAdvantage.WF
{
    public class MWFNextCondition : X_AD_WF_NextCondition
    {
        /**	Numeric evaluation		*/
        private bool _numeric = true;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="id">id</param>
        /// <param name="trxName">transaction</param>
        public MWFNextCondition(Ctx ctx, int id, Trx trxName)
            : base(ctx, id, trxName)
        {

        }



        /// <summary>
        /// Load Cosntructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MWFNextCondition(Ctx ctx, System.Data.DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// Is Or Condition
        /// </summary>
        /// <returns>true if OR</returns>
        public bool IsOr()
        {
            return ANDOR_Or.Equals(GetAndOr());
        }

        int AD_WF_Activity_ID = 0;
        /// <summary>
        /// Evaluate Condition
        /// </summary>
        /// <param name="activity">activity</param>
        /// <returns>true if true</returns>
        public bool Evaluate(MWFActivity activity)
        {
            AD_WF_Activity_ID = activity.GetAD_WF_Activity_ID();
            if (GetAD_Column_ID() == 0)
            {
                //throw new IllegalStateException("No Column defined - " + this);
                throw new Exception("No Column defined - " + this);
            }



            PO po = activity.GetPO();
            if (po == null || po.Get_ID() == 0)
            {
                //throw new IllegalStateException("Could not evaluate " + po + " - " + this);
                throw new Exception("Could not evaluate " + po + " - " + this);
            }
            //
            Object valueObj = po.Get_ValueOfColumn(GetAD_Column_ID());
            if (valueObj == null)
                valueObj = "";
            String value1 = GetValue();
            if (value1 == null)
                value1 = "";
            String value2 = GetValue2();
            if (value2 == null)
                value2 = "";

            String resultStr = "PO:{" + valueObj + "} " + GetOperation() + " Condition:{" + value1 + "}";
            if (GetOperation().Equals(OPERATION_Sql))
                throw new ArgumentException("SQL Operator not implemented yet: " + resultStr);
            if (GetOperation().Equals(OPERATION_X))
                resultStr += "{" + value2 + "}";

            bool result = false;

            //Lakhwinder
            if (MColumn.Get(GetCtx(), GetAD_Column_ID()).GetColumnName().ToUpper().Equals("C_GENATTRIBUTESETINSTANCE_ID"))
            {
                return EvaluateAttributeCondition(po);
            }

            //if (valueObj instanceof Number)
            if (valueObj != null && CommonFunctions.IsNumeric(valueObj.ToString()))
                result = CompareNumber(valueObj, value1, value2);
            else
                result = CompareString(valueObj, value1, value2);
            //
            log.Fine(resultStr + " -> " + result + (_numeric ? " (#)" : " ($)"));
            return result;
        }

        /// <summary>
        /// Compare String
        /// </summary>
        /// <param name="valueObj">comparator</param>
        /// <param name="value1S">first value</param>
        /// <param name="value2S">second value</param>
        /// <returns>true if operation</returns>
        private bool CompareString(Object valueObj, String value1S, String value2S)
        {
            _numeric = false;
            String valueObjS = valueObj.ToString().ToLower();
            value1S = value1S.ToLower();
            if (!string.IsNullOrEmpty(value2S))
            {
                value2S = value2S.ToLower();
            }
            //
            String op = GetOperation();
            //if (OPERATION_Eq.Equals(op))   
            //    return valueObjS.CompareTo(value1S) == 0;
            //else if (OPERATION_Gt.Equals(op))
            //    return valueObjS.CompareTo(value1S) > 0;
            //else if (OPERATION_GtEq.Equals(op))
            //    return valueObjS.CompareTo(value1S) >= 0;
            //else if (OPERATION_Le.Equals(op))
            //    return valueObjS.CompareTo(value1S) < 0;
            //else if (OPERATION_LeEq.Equals(op))
            //    return valueObjS.CompareTo(value1S) <= 0;           
            //else if (OPERATION_NotEq.Equals(op))
            //    return valueObjS.CompareTo(value1S) != 0;
            //else if (OPERATION_Like.Equals(op))
            //   return valueObjS.CompareTo(value1S) == 0;

            //specific for DMS
            DataSet ds = DB.ExecuteDataset(@"SELECT col.ColumnName,tab.TableName FROM AD_Column col
                                                                    INNER JOIN AD_Table tab ON (tab.AD_Table_ID=col.AD_Table_ID)
                                                                    WHERE col.IsActive='Y' AND col.AD_Column_ID=" + GetAD_Column_ID());
            if (ds != null
                && ds.Tables[0].Rows.Count > 0
                && ds.Tables[0].Rows[0]["TableName"].ToString().Equals("VADMS_MetaData"))
            {
                return EvaluateLogicForVADMS(valueObjS, value1S, op, ds.Tables[0].Rows[0]["ColumnName"].ToString());
            }
            if ((!OPERATION_Sql.Equals(op))
                && (!OPERATION_X.Equals(op)))
            {
                return EvaluateLogic(valueObjS, value1S, op);
            }

            //
            else if (OPERATION_Sql.Equals(op))
                throw new ArgumentException("SQL not Implemented");
            //
            else if (OPERATION_X.Equals(op))
            {
                if (valueObjS.CompareTo(value1S) < 0)
                    return false;
                //	To
                return valueObjS.CompareTo(value2S) <= 0;
            }
            //
            throw new ArgumentException("Unknown Operation=" + op);
        }



        private bool EvaluateLogic(string valueObj, String value1S, string op)
        {
            StringTokenizer st = new StringTokenizer(value1S.Trim(), "&|", true);
            try
            {
                int it = st.CountTokens();

                if (it > 1)
                {
                    if (((it / 2) - ((it + 1) / 2)) == 0)		//	only uneven arguments
                    {
                        //log.severe("Logic does not comply with format "
                        //    + "'<expression> [<logic> <expression>]' => " + logic);
                        return false;
                    }
                    StringBuilder logic = new StringBuilder();
                    string _operator = null;
                    int res = -1;
                    while (st.HasMoreTokens())
                    {
                        //logic.Append(valueObj);
                        res = valueObj.CompareTo(st.NextToken().Trim());
                        if (OPERATION_Eq.Equals(op))
                        {
                            logic.Append(res == 0 ? "true" : "false");
                        }
                        else if (OPERATION_Gt.Equals(op))
                        {
                            logic.Append(res == 1 ? "true" : "false");
                        }
                        else if (OPERATION_GtEq.Equals(op))
                        {
                            logic.Append(res >= 0 ? "true" : "false");
                        }
                        else if (OPERATION_Le.Equals(op))
                        {
                            logic.Append(res < 0 ? "true" : "false");
                        }
                        else if (OPERATION_LeEq.Equals(op))
                        {
                            logic.Append(res <= 0 ? "true" : "false");
                        }
                        else if (OPERATION_Like.Equals(op))
                        {
                            logic.Append(res != 0 ? "true" : "false");
                        }
                        else if (OPERATION_NotEq.Equals(op))
                        {
                            logic.Append(res == 0 ? "true" : "false");
                        }
                        //logic.Append(st.NextToken().Trim());
                        if (st.HasMoreTokens())
                        {
                            logic.Append(st.NextToken().Trim());
                        }

                    }

                    st = new StringTokenizer(logic.ToString().Trim(), "&|", true);
                    bool retval = false;
                    retval = st.NextToken().Trim().Equals("false") ? false : true;
                    while (st.HasMoreTokens())
                    {

                        _operator = st.NextToken().Trim();
                        if (_operator == "&")
                            retval = retval && (st.NextToken().Trim().Equals("false") ? false : true);
                        else if (_operator == "|")
                            retval = retval || (st.NextToken().Trim().Equals("false") ? false : true);
                    }
                    return retval;
                }
                else
                {
                    if (OPERATION_Eq.Equals(op))
                        return valueObj.CompareTo(value1S) == 0;
                    else if (OPERATION_Gt.Equals(op))
                        return valueObj.CompareTo(value1S) > 0;
                    else if (OPERATION_GtEq.Equals(op))
                        return valueObj.CompareTo(value1S) >= 0;
                    else if (OPERATION_Le.Equals(op))
                        return valueObj.CompareTo(value1S) < 0;
                    else if (OPERATION_LeEq.Equals(op))
                        return valueObj.CompareTo(value1S) <= 0;
                    else if (OPERATION_NotEq.Equals(op))
                        return valueObj.CompareTo(value1S) != 0;
                    else if (OPERATION_Like.Equals(op))
                        return valueObj.CompareTo(value1S) == 0;
                }
                //return Evaluator.EvaluateLogic(null, logic.ToString());

            }
            catch
            {
                return false;
            }

            return false;
        }
        private bool EvaluateNumericLogic(decimal valueObj, string value1S, string op)
        {
            StringTokenizer st = new StringTokenizer(value1S.Trim(), "&|", true);
            try
            {
                int it = st.CountTokens();
                decimal value1SDec = 0;
                if (it > 1)
                {
                    if (((it / 2) - ((it + 1) / 2)) == 0)		//	only uneven arguments
                    {
                        //log.severe("Logic does not comply with format "
                        //    + "'<expression> [<logic> <expression>]' => " + logic);
                        return false;
                    }
                    StringBuilder logic = new StringBuilder();
                    string _operator = null;
                    int res = -1;
                    while (st.HasMoreTokens())
                    {
                        //logic.Append(valueObj);
                        res = valueObj.CompareTo(Convert.ToDecimal(st.NextToken().Trim()));
                        if (OPERATION_Eq.Equals(op))
                        {
                            logic.Append(res == 0 ? "true" : "false");
                        }
                        else if (OPERATION_Gt.Equals(op))
                        {
                            logic.Append(res == 1 ? "true" : "false");
                        }
                        else if (OPERATION_GtEq.Equals(op))
                        {
                            logic.Append(res >= 0 ? "true" : "false");
                        }
                        else if (OPERATION_Le.Equals(op))
                        {
                            logic.Append(res < 0 ? "true" : "false");
                        }
                        else if (OPERATION_LeEq.Equals(op))
                        {
                            logic.Append(res <= 0 ? "true" : "false");
                        }
                        else if (OPERATION_Like.Equals(op))
                        {
                            logic.Append(res != 0 ? "true" : "false");
                        }
                        else if (OPERATION_NotEq.Equals(op))
                        {
                            logic.Append(res == 0 ? "true" : "false");
                        }
                        //logic.Append(st.NextToken().Trim());
                        if (st.HasMoreTokens())
                        {
                            logic.Append(st.NextToken().Trim());
                        }

                    }

                    st = new StringTokenizer(logic.ToString().Trim(), "&|", true);
                    bool retval = false;
                    retval = st.NextToken().Trim().Equals("false") ? false : true;
                    while (st.HasMoreTokens())
                    {

                        _operator = st.NextToken().Trim();
                        if (_operator == "&")
                            retval = retval && (st.NextToken().Trim().Equals("false") ? false : true);
                        else if (_operator == "|")
                            retval = retval || (st.NextToken().Trim().Equals("false") ? false : true);
                    }
                    return retval;
                }
                else
                {
                    value1SDec = Util.GetValueOfDecimal(value1S);
                    if (OPERATION_Eq.Equals(op))
                        return valueObj.CompareTo(value1SDec) == 0;
                    else if (OPERATION_Gt.Equals(op))
                        return valueObj.CompareTo(value1SDec) > 0;
                    else if (OPERATION_GtEq.Equals(op))
                        return valueObj.CompareTo(value1SDec) >= 0;
                    else if (OPERATION_Le.Equals(op))
                        return valueObj.CompareTo(value1SDec) < 0;
                    else if (OPERATION_LeEq.Equals(op))
                        return valueObj.CompareTo(value1SDec) <= 0;
                    else if (OPERATION_NotEq.Equals(op))
                        return valueObj.CompareTo(value1SDec) != 0;
                    else if (OPERATION_Like.Equals(op))
                        return valueObj.CompareTo(value1SDec) == 0;
                }
                //return Evaluator.EvaluateLogic(null, logic.ToString());

            }
            catch
            {
                return false;
            }

            return false;
        }

//        private bool EvaluateLogicForVADMS(string valueObj, String value1S, string op, string columnName)
//        {
//            try
//            {
//                if (columnName.Equals("Name"))//For Name Column
//                {
//                    StringTokenizer st = new StringTokenizer(value1S.Trim(), "&|", true);
//                    String[] sta = valueObj.Split(',');//new StringTokenizer(value1S.Trim(), ",", true);
//                    StringBuilder logic = new StringBuilder();
//                    string _operator = null;
//                    valueObj = valueObj.Replace(',', '|');
//                    if (sta.Length > 1)
//                    {
//                        bool retval = EvaluateLogic(st.NextToken().Trim(), valueObj, op);
//                        bool temp = false;
//                        while (st.HasMoreTokens())
//                        {
//                            _operator = st.NextToken().Trim();
//                            if (_operator.Equals("|"))
//                            {
//                                temp = EvaluateLogic(st.NextToken().Trim(), valueObj, op);
//                                retval = retval || temp;
//                            }
//                            else if (_operator.Equals("&"))
//                            {
//                                temp = EvaluateLogic(st.NextToken().Trim(), valueObj, op);
//                                retval = retval && temp;
//                            }

//                        }
//                        return retval;
//                    }
//                    else
//                    {
//                        return EvaluateLogic(valueObj, value1S, op);
//                    }
//                }
//                else if (columnName.Equals("VADMS_ContainText")) //For ContainsText Column
//                {
//                    log.Fine("DocumentContainText:?");
//                    try
//                    {
//                        Assembly assembly = Assembly.Load("VADMSSvc");
//                        Type type = assembly.GetType("ViennaAdvantage.Classes.DocumentOperation");
//                        if (type != null)
//                        {
//                            MethodInfo methodInfo = type.GetMethod("DocumentContainText");
//                            if (methodInfo != null)
//                            {
//                                object result = null;
//                                ParameterInfo[] parameters = methodInfo.GetParameters();
//                                object classInstance = Activator.CreateInstance(type, null);
//                                if (parameters.Length == 0)
//                                {
//                                    result = methodInfo.Invoke(classInstance, null);
//                                }
//                                else
//                                {
//                                    int docID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT Doc.VADMS_Document_ID
//                                                                FROM VADMS_Document Doc
//                                                                INNER JOIN VADMS_MetaData MD ON (MD.VADMS_Document_ID=Doc.VADMS_Document_ID)
//                                                                INNER JOIN AD_WF_Activity WA ON (WA.Record_ID=MD.VADMS_MetaData_ID)
//                                                                INNER JOIN AD_WF_Node NODE ON (NODE.AD_WF_Node_ID=WA.AD_WF_Node_ID)
//                                                                INNER JOIN AD_WF_NodeNext NN ON (NN.AD_WF_Node_ID=NODE.AD_WF_Node_ID)
//                                                                INNER JOIN AD_WF_NextCondition NC ON (NC.AD_WF_NodeNext_ID=NN.AD_WF_NodeNext_ID)
//                                                                WHERE NC.AD_WF_NextCondition_ID=" + GetAD_WF_NextCondition_ID(), null, Get_TrxName()));
//                                    object[] parametersArray = new object[] { value1S, Env.GetCtx().GetAD_Language(), docID };
//                                    result = methodInfo.Invoke(classInstance, parametersArray);
//                                }
//                                if (result == null)
//                                {
//                                    log.Severe("Mehtod Not Invoked Successfully:ViennaAdvantage.Classes.DocumentOperation.DocumentContainText");
//                                    return false;
//                                }
//                                else if (result.ToString() == "Y")
//                                {
//                                    log.Fine("DocumentContainText Invoked Successfully, Result Is" + result.ToString());
//                                    return true;
//                                }
//                                else
//                                {
//                                    log.Fine("DocumentContainText Invoked Successfully, Result Is" + result.ToString());
//                                    return false;
//                                }
//                            }
//                            log.Severe("Cant Get Method of ViennaAdvantage.Classes.DocumentOperation From VADMSSvc File.");
//                        }
//                        log.Severe("Cant Get Type of ViennaAdvantage.Classes.DocumentOperation From VADMSSvc File.");

//                    }
//                    catch (Exception ex)
//                    {
//                        log.Severe("Error Occured During ConditionEvaluation of DocumentContainText:" + ex.Message);
//                        return false;
//                    }
//                    return false;
//                }

//                else
//                {
//                    return EvaluateLogic(valueObj, value1S, op);
//                }

//            }
//            catch
//            {
//                return false;
//            }
//            return true;
//        }


        private bool EvaluateLogicForVADMS(string valueObj, String value1S, string op, string columnName)
        {
            try
            {
                if (columnName.Equals("Name"))//For Name Column
                {
                    StringTokenizer st = new StringTokenizer(value1S.Trim(), "&|", true);
                    String[] sta = valueObj.Split(',');//new StringTokenizer(value1S.Trim(), ",", true);
                    StringBuilder logic = new StringBuilder();
                    string _operator = null;
                    valueObj = valueObj.Replace(',', '|');
                    if (sta.Length > 1)
                    {
                        bool retval = EvaluateLogic(st.NextToken().Trim(), valueObj, op);
                        bool temp = false;
                        while (st.HasMoreTokens())
                        {
                            _operator = st.NextToken().Trim();
                            if (_operator.Equals("|"))
                            {
                                temp = EvaluateLogic(st.NextToken().Trim(), valueObj, op);
                                retval = retval || temp;
                            }
                            else if (_operator.Equals("&"))
                            {
                                temp = EvaluateLogic(st.NextToken().Trim(), valueObj, op);
                                retval = retval && temp;
                            }

                        }
                        return retval;
                    }
                    else
                    {
                        return EvaluateLogic(valueObj, value1S, op);
                    }
                }
                else if (columnName.Equals("VADMS_ContainText")) //For ContainsText Column
                {
                    log.Fine("DocumentContainText:?");
                    //                    try
                    //                    {
                    //                        Assembly assembly = Assembly.Load("VADMSSvc");
                    //                        Type type = assembly.GetType("ViennaAdvantage.Classes.DocumentOperation");
                    //                        if (type != null)
                    //                        {
                    //                            MethodInfo methodInfo = type.GetMethod("DocumentContainText");
                    //                            if (methodInfo != null)
                    //                            {
                    //                                object result = null;
                    //                                ParameterInfo[] parameters = methodInfo.GetParameters();
                    //                                object classInstance = Activator.CreateInstance(type, null);
                    //                                if (parameters.Length == 0)
                    //                                {
                    //                                    result = methodInfo.Invoke(classInstance, null);
                    //                                }
                    //                                else
                    //                                {
                    //                                    int docID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT Doc.VADMS_Document_ID
                    //                                                                FROM VADMS_Document Doc
                    //                                                                INNER JOIN VADMS_MetaData MD ON (MD.VADMS_Document_ID=Doc.VADMS_Document_ID)
                    //                                                                INNER JOIN AD_WF_Activity WA ON (WA.Record_ID=MD.VADMS_MetaData_ID)
                    //                                                                INNER JOIN AD_WF_Node NODE ON (NODE.AD_WF_Node_ID=WA.AD_WF_Node_ID)
                    //                                                                INNER JOIN AD_WF_NodeNext NN ON (NN.AD_WF_Node_ID=NODE.AD_WF_Node_ID)
                    //                                                                INNER JOIN AD_WF_NextCondition NC ON (NC.AD_WF_NodeNext_ID=NN.AD_WF_NodeNext_ID)
                    //                                                                WHERE NC.AD_WF_NextCondition_ID=" + GetAD_WF_NextCondition_ID(), null, Get_TrxName()));
                    //                                    object[] parametersArray = new object[] { value1S, Env.GetCtx().GetAD_Language(), docID };
                    //                                    result = methodInfo.Invoke(classInstance, parametersArray);
                    //                                }
                    //                                if (result == null)
                    //                                {
                    //                                    log.Severe("Mehtod Not Invoked Successfully:ViennaAdvantage.Classes.DocumentOperation.DocumentContainText");
                    //                                    return false;
                    //                                }
                    //                                else if (result.ToString() == "Y")
                    //                                {
                    //                                    log.Fine("DocumentContainText Invoked Successfully, Result Is" + result.ToString());
                    //                                    return true;
                    //                                }
                    //                                else
                    //                                {
                    //                                    log.Fine("DocumentContainText Invoked Successfully, Result Is" + result.ToString());
                    //                                    return false;
                    //                                }
                    //                            }
                    //                            log.Severe("Cant Get Method of ViennaAdvantage.Classes.DocumentOperation From VADMSSvc File.");
                    //                        }
                    //                        log.Severe("Cant Get Type of ViennaAdvantage.Classes.DocumentOperation From VADMSSvc File.");

                    //                    }
                    //                    catch (Exception ex)
                    //                    {
                    //                        log.Severe("Error Occured During ConditionEvaluation of DocumentContainText:" + ex.Message);
                    //                        return false;
                    //                    }
                    //                    return false;

                    int docID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT Doc.VADMS_Document_ID
                                                FROM VADMS_Document Doc
                                                INNER JOIN VADMS_MetaData MD ON (MD.VADMS_Document_ID=Doc.VADMS_Document_ID)
                                                INNER JOIN AD_WF_Activity WA ON (WA.Record_ID=MD.VADMS_MetaData_ID)
                                                INNER JOIN AD_WF_Node NODE ON (NODE.AD_WF_Node_ID=WA.AD_WF_Node_ID)
                                                INNER JOIN AD_WF_NodeNext NN ON (NN.AD_WF_Node_ID=NODE.AD_WF_Node_ID)
                                                INNER JOIN AD_WF_NextCondition NC ON (NC.AD_WF_NodeNext_ID=NN.AD_WF_NodeNext_ID)
                                                WHERE NC.AD_WF_NextCondition_ID=" + GetAD_WF_NextCondition_ID() +@"
                                                AND WA.AD_WF_Activity_ID="+ AD_WF_Activity_ID, null, Get_TrxName()));
                    string res = string.Empty;
                    //GetPO(Get_TrxName());
                    DocumentAction docAction = new DocumentAction();
                    return docAction.IsDocumentContainsText(docID, value1S, GetCtx().GetAD_Language(), out res);

                    //return ok;
                }
                else
                {
                    return EvaluateLogic(valueObj, value1S, op);
                }

            }
            catch
            {
                return false;
            }
            
        }






        //private bool IsFileContainsText()
        //{
        //    log.Fine("MoveDocument:");
        //    try
        //    {
        //        Assembly assembly = Assembly.Load("VADMSSvc");
        //        Type type = assembly.GetType("ViennaAdvantage.Classes.DocumentOperation");
        //        if (type != null)
        //        {
        //            MethodInfo methodInfo = type.GetMethod("DocumentContainText");
        //            if (methodInfo != null)
        //            {
        //                object result = null;
        //                ParameterInfo[] parameters = methodInfo.GetParameters();
        //                object classInstance = Activator.CreateInstance(type, null);
        //                if (parameters.Length == 0)
        //                {
        //                    result = methodInfo.Invoke(classInstance, null);
        //                }
        //                else
        //                {
        //                    GetPO(Get_TrxName());
        //                    int sourceFolderID = _node.GetVADMS_Folder_ID_1();
        //                    int docID = (int)_po.Get_Value("VADMS_Document_ID");
        //                    System.Collections.Generic.List<System.Int32> metaDataID = new System.Collections.Generic.List<System.Int32>() { docID };
        //                    object[] parametersArray = new object[] { get, _node.GetVADMS_Folder_ID(), sourceFolderID == 0 ? -1 : sourceFolderID, false };
        //                    result = methodInfo.Invoke(classInstance, parametersArray);
        //                }
        //                if (result == null)
        //                {
        //                    SetTextMsg("Mehtod Invoked Successfully.");
        //                }
        //                else
        //                {
        //                    SetTextMsg("Mehtod Invoked Successfully:" + result.ToString());
        //                }
        //                return true;
        //            }
        //            SetTextMsg("Cant Get Method of ViennaAdvantage.Classes.DocumentOperation From VADMSSvc File.");
        //        }
        //        SetTextMsg("Cant Get Type of ViennaAdvantage.Classes.DocumentOperation From VADMSSvc File.");
        //    }
        //    catch (Exception ex)
        //    {
        //        SetTextMsg("Error Occured During Move Document:" + ex.Message);
        //        return false;
        //    }
        //    return false;

        //}



        /// <summary>
        /// Compare Number
        /// </summary>
        /// <param name="valueObj">comparator</param>
        /// <param name="value1">first value</param>
        /// <param name="value2">second value</param>
        /// <returns>true if operation</returns>
        private bool CompareNumber(Object valueObj, String value1, String value2)
        {
            Decimal valueObjB = 0;
            Decimal value1B = 0;
            Decimal value2B = 0;

            try
            {

                //if (valueObj instanceof BigDecimal)
                //    valueObjB = (BigDecimal)valueObj;
                //else if (valueObj instanceof Integer)
                //    valueObjB = new BigDecimal (((Integer)valueObj).intValue());
                //else
                //    valueObjB = new BigDecimal (String.valueOf(valueObj));

                valueObjB = Convert.ToDecimal(valueObj);
            }
            catch (Exception e)
            {
                log.Fine("compareNumber - valueObj=" + valueObj + " - " + e.ToString());
                return CompareString(valueObj, value1, value2);
            }
            String op = GetOperation();
            //if (OPERATION_Eq.Equals(op))
            //    return valueObjB.CompareTo(value1B) == 0;
            //else if (OPERATION_Gt.Equals(op))
            //    return valueObjB.CompareTo(value1B) > 0;
            //else if (OPERATION_GtEq.Equals(op))
            //    return valueObjB.CompareTo(value1B) >= 0;
            //else if (OPERATION_Le.Equals(op))
            //    return valueObjB.CompareTo(value1B) < 0;
            //else if (OPERATION_LeEq.Equals(op))
            //    return valueObjB.CompareTo(value1B) <= 0;
            //else if (OPERATION_Like.Equals(op))
            //    return valueObjB.CompareTo(value1B) == 0;
            //else if (OPERATION_NotEq.Equals(op))
            //    return valueObjB.CompareTo(value1B) != 0;
            //
            if ((!OPERATION_Sql.Equals(op))
               && (!OPERATION_X.Equals(op)))
            {
                return EvaluateNumericLogic(valueObjB, value1, op);
            }
            else if (OPERATION_Sql.Equals(op))
                throw new ArgumentException("SQL not Implemented");
            //
            else if (OPERATION_X.Equals(op))
            {


                try
                {
                    value1B = Convert.ToDecimal(value1);
                }
                catch (Exception e)
                {
                    log.Fine("compareNumber - value1=" + value1 + " - " + e.ToString());
                    return CompareString(valueObj, value1, value2);
                }


                if (valueObjB.CompareTo(value1B) < 0)
                    return false;
                //	To
                try
                {
                    value2B = Convert.ToDecimal(value2);
                    return valueObjB.CompareTo(value2B) <= 0;
                }
                catch (Exception e)
                {
                    log.Fine("compareNumber - value2=" + value2 + " - " + e.ToString());
                    return false;
                }
            }
            //
            throw new ArgumentException("Unknown Operation=" + op);
        }



        private bool EvaluateAttributeCondition(PO po)
        {
            //throw new NotImplementedException();

            int wfcattSet = GetC_GenAttributeSet_ID();
            int rcattSet = Util.GetValueOfInt(po.Get_Value("C_GenAttributeSet_ID"));
            if (rcattSet != wfcattSet)
            {
                return false;
            }

            try
            {
                int wfcattSetIns = Util.GetValueOfInt(GetC_GenAttributeSetInstance_ID());
                int rcattSetIns = Util.GetValueOfInt(po.Get_Value("C_GenAttributeSetInstance_ID"));

                DataSet wfcDs = DB.ExecuteDataset("select c_genattribute_id,c_genattributevalue_id from C_GenAttributeInstance where C_GenAttributeSetInstance_id=" + wfcattSetIns, null);
                DataSet rcDs = DB.ExecuteDataset("select c_genattribute_id,c_genattributevalue_id from C_GenAttributeInstance where C_GenAttributeSetInstance_id=" + rcattSetIns, null);
                bool retVal = true;
                for (int i = 0; i < wfcDs.Tables[0].Rows.Count; i++)
                {
                    if (wfcDs.Tables[0].Rows[i]["c_genattributevalue_id"] == null || wfcDs.Tables[0].Rows[i]["c_genattributevalue_id"] == DBNull.Value)
                    {
                        continue;
                    }
                    for (int j = 0; j < rcDs.Tables[0].Rows.Count; j++)
                    {
                        if (wfcDs.Tables[0].Rows[i]["c_genattribute_id"].Equals(rcDs.Tables[0].Rows[j]["c_genattribute_id"]))
                        {
                            retVal = retVal && (wfcDs.Tables[0].Rows[i]["c_genattributevalue_id"].Equals(rcDs.Tables[0].Rows[j]["c_genattributevalue_id"]));
                            break;
                        }
                    }
                }
                return retVal;
            }
            catch
            {

                return false;
            }
        }
    }
}
