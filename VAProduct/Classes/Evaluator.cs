/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Evaluator
 * Purpose        : To Evaluate Logic Tuple
 * Class Used     : 
 * Chronological    Development
 * Raghunandan      7-May-2009 
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using VAdvantage.Controller;

namespace VAdvantage.Classes
{
    public class Evaluator
    {
        /// <summary>
        ///Check if All Variables are Defined
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="logic">logic info</param>
        /// <returns>true if fully defined</returns>
        public static bool IsAllVariablesDefined(Evaluatee source, string logic)
        {
            if (logic == null || logic.Length == 0)
                return true;
            //
            int pos = 0;
            while (pos < logic.Length)
            {
                int first = logic.IndexOf('@', pos);
                if (first == -1)
                    return true;
                //int second = logic.Substring(first + 1);
                int second = logic.IndexOf('@', first + 1);
                if (second == -1)
                {
                    //log.severe("No second @ in Logic: " + logic);
                    return false;
                }
                //string variable = logic.substring(first + 1, second - 1);
                string variable = logic.Substring(first + 1, second - 1);
                String eval = source.GetValueAsString(variable);
                //log.finest(variable + "=" + eval);
                if (eval == null || eval.Length == 0)
                    return false;
                //	
                pos = second + 1;
            }
            return true;
        }

       /// <summary>
       ///Evaluate Logic.
       ///<code>
       ///format		:= <expression> [<logic> <expression>]
       ///expression	:= @<context>@<exLogic><value>
       ///logic		:= <|> | <&>
       ///exLogic		:= <=> | <!> | <^> | <<> | <>>
       ///context		:= any global or window context
       ///value		:= strings can be with ' or "
       ///logic operators	:= AND or OR with the prevoius result from left to right
       ///Example	'@AD_Table@=Test | @Language@=GERGER
       /// </code>
       /// </summary>
       /// <param name="source">class implementing get_ValueAsString(variable)</param>
       /// <param name="logic">logic string</param>
       /// <returns>locic result</returns>
        public static bool EvaluateLogic(Evaluatee source, String logic)
        {
            //	Conditional

            StringTokenizer st = new StringTokenizer(logic.Trim(), "&|", true);
            try
            {
                int it = st.CountTokens();
                if (((it / 2) - ((it + 1) / 2)) == 0)		//	only uneven arguments
                {
                    //log.severe("Logic does not comply with format "
                    //    + "'<expression> [<logic> <expression>]' => " + logic);
                    return false;
                }

                bool retValue = EvaluateLogicTuple(source, st.NextToken());
                while (st.HasMoreTokens())
                {
                    String logOp = st.NextToken().Trim();
                    bool temp = EvaluateLogicTuple(source, st.NextToken());
                    if (logOp.Equals("&"))
                        retValue = retValue & temp;
                    else if (logOp.Equals("|"))
                        retValue = retValue | temp;
                    else
                    {
                        //Common.////ErrorLog.FillErrorLog("Evaluatot.EvaluateLogic()", "DynamicDisplay", "Logic operant '|' or '&' expected => " + logic, VAdvantage.Framework.Message.MessageType.ERROR);
                        //log.warning("Logic operant '|' or '&' expected => " + logic);
                        return false;
                    }
                }
                return retValue;
            }
            catch
            {
                return false;
            }

        }  

       /// <summary>
       ///Evaluate	@context@=value or @context@!value or @context@^value.
       ///<pre>
       ///value: strips ' and " always (no escape or mid stream)
       ///value: can also be a context variable
       ///</pre>
       ///@param source 
       /// </summary>
       /// <param name="source">class implementing get_ValueAsString(variable)</param>
       /// <param name="logic">logic tuple</param>
       /// <returns>true or false</returns>
        private static bool EvaluateLogicTuple(Evaluatee source, string logic)
        {
            StringTokenizer st = new StringTokenizer(logic.Trim(), "!=^><", true);
            if (st.CountTokens() != 3)
            {
                //log.warning("Logic tuple does not comply with format "
                //    + "'@context@=value' where operand could be one of '=!^><' => " + logic);
                return false;
            }
            //	First Part
            String first = st.NextToken().Trim();					//	get '@tag@'
            String firstEval = first.Trim();
            if (first.IndexOf('@') != -1)		//	variable
            {
                first = first.Replace('@', ' ').Trim(); 			//	strip 'tag'
                //firstEval = source.get_ValueAsString(first);		//	replace with it's value
                firstEval = source.GetValueAsString(first);
                if (firstEval == null)
                    firstEval = "";
            }
            firstEval = firstEval.Replace('\'', ' ').Replace('"', ' ').Trim();	//	strip ' and "
            //	Comperator
            String operand = st.NextToken();

            //	Second Part
            String second = st.NextToken();							//	get value
            String secondEval = second.Trim();
            if (second.IndexOf('@') != -1)		//	variable
            {
                second = second.Replace('@', ' ').Trim();			// strip tag
                //secondEval = source.get_ValueAsString(second);		//	replace with it's value
                secondEval = source.GetValueAsString(second);		//	replace with it's value
                if (secondEval == null)
                    secondEval = "";
            }
            secondEval = secondEval.Replace('\'', ' ').Replace('"', ' ').Trim();	//	strip ' and "
            //	Handling of ID compare (null => 0)
            if (first.IndexOf("_ID") != -1 && firstEval.Length == 0)
                firstEval = "0";
            if (second.IndexOf("_ID") != -1 && secondEval.Length == 0)
                secondEval = "0";
            //	Logical Comparison
            bool result = EvaluateLogicTuple(firstEval, operand, secondEval);

            //if (log.isLevelFinest())
            //    log.finest(logic
            //        + " => \"" + firstEval + "\" " + operand + " \"" + secondEval + "\" => " + result);
            //
            return result;
        }

        /// <summary>
        /// Evaluate Logic Tuple
        /// </summary>
        /// <param name="value1">value</param>
        /// <param name="operand">operand = ~ ^ ! > <</param>
        /// <param name="value2">value2</param>
        /// <returns>evaluation</returns>
        public static bool EvaluateLogicTuple(string value1, string operand, string value2)
        {
            if (value1 == null || operand == null || value2 == null)
                return false;

            double value1bd = 0;
            double value2bd = 0;
            try
            {
                if (!value1.StartsWith("'"))
                    value1bd = Convert.ToDouble(value1);
                if (!value2.StartsWith("'"))
                    value2bd = Convert.ToDouble(value2);
            }
            catch 
            {
                value1bd = 0;
                value2bd = 0;
            }
            //
            if (operand.Equals("="))
            {
                if (value1bd != 0 && value2bd != 0)
                    return value1bd.CompareTo(value2bd) == 0;
                return value1.CompareTo(value2) == 0;
            }
            else if (operand.Equals("<"))
            {
                if (value1bd != 0 && value2bd != 0)
                    return value1bd.CompareTo(value2bd) < 0;
                return value1.CompareTo(value2) < 0;
            }
            else if (operand.Equals(">"))
            {
                if (value1bd != 0 && value2bd != 0)
                    return value1bd.CompareTo(value2bd) > 0;
                return value1.CompareTo(value2) > 0;
            }
            else //	interpreted as not
            {
                if (value1bd != 0 && value2bd != 0)
                    return value1bd.CompareTo(value2bd) != 0;
                return value1.CompareTo(value2) != 0;
            }
        }

        /// <summary>
        ///Parse String and add variables with @ to the list.
        /// </summary>
        /// <param name="list">list to be added to</param>
        /// <param name="parseString">string to parse for variables</param>
        public static void ParseDepends(List<string> list, string parseString)
        {
            if (parseString == null || parseString.Length == 0)
                return;
            //	log.fine(parseString);
            String s = parseString;
            //  while we have variables 
            while (s.IndexOf("@") != -1)
            {
                int pos = s.IndexOf("@");
                s = s.Substring(pos + 1);
                pos = s.IndexOf("@");
                if (pos == -1)
                    continue;	//	error number of @@ not correct......................
                string variable = s.Substring(0, pos);
                s = s.Substring(pos + 1);
                //	log.fine(variable);
                if (!list.Contains(variable))
                    list.Add(variable);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="ctx"></param>
        /// <param name="windowCtx"></param>
        /// <returns></returns>
        public static String ReplaceVariables(string raw, Context ctx, WindowCtx windowCtx)
        {
            string result = raw;
            List<string> variables = new List<string>();
            ParseDepends(variables, raw);
            //log.finest("The variables are:"+ variables);
            for (int i = 0; i < variables.Count; i++)
            {
                string var = variables[i].ToString();
                string param = null;
                if (windowCtx != null)
                    //param = windowCtx[var].;
                    if (param == null)
                        param = ctx.GetContext(var);
                if (param != null && param.Length > 0)
                {
                    double num = 0;
                    try
                    {
                        num = Convert.ToDouble(param);
                    }
                    catch  { }
                    if ( num != 0)
                        result = result.Replace("@" + var + "@", num.ToString());
                    else
                        result = result.Replace("@" + var + "@", param.Replace("'", "''"));
                }
                else
                {
                    result = result.Replace("@" + var + "@", "NULL");
                }
            }
            return result;
        }

        public static String ReplaceVariables(string raw, VAdvantage.Utility.Ctx ctx, WindowCtx windowCtx)
        {
            string result = raw;
            List<string> variables = new List<string>();
            ParseDepends(variables, raw);
            //log.finest("The variables are:"+ variables);
            for (int i = 0; i < variables.Count; i++)
            {
                string var = variables[i].ToString();
                string param = null;
                if (windowCtx != null)
                    //param = windowCtx[var].;
                    if (param == null)
                        param = ctx.GetContext(var);
                if (param != null && param.Length > 0)
                {
                    double num = 0;
                    try
                    {
                        num = Convert.ToDouble(param);
                    }
                    catch  { }
                    if ( num != 0)
                        result = result.Replace("@" + var + "@", num.ToString());
                    else
                        result = result.Replace("@" + var + "@", param.Replace("'", "''"));
                }
                else
                {
                    result = result.Replace("@" + var + "@", "NULL");
                }
            }
            return result;
        }

        /// <summary>
        /// string value form tokenizer
        /// </summary>
        /// <param name="raw">Raw</param>
        /// <returns>string</returns>
        public static String StripVariables(String raw)
        {
            StringBuilder buf = new StringBuilder();
            StringTokenizer st = new StringTokenizer(raw, "@");
            while (st.HasMoreTokens())
            {
                buf.Append(st.NextToken());
                if (st.HasMoreTokens())
                {
                    buf.Append("?");
                    st.NextToken();
                }
            }
            return buf.ToString();
        }

        /// <summary>
        /// get valriable
        /// </summary>
        /// <param name="raw">raw</param>
        /// <returns>list</returns>
        public static ArrayList GetVariables(String raw)
        {
            ArrayList variables = new ArrayList();
            StringBuilder buf = new StringBuilder();
            StringTokenizer st = new StringTokenizer(raw, "@");
            while (st.HasMoreTokens())
            {
                buf.Append(st.NextToken());
                if (st.HasMoreTokens())
                {
                    buf.Append("?");
                    variables.Add(st.NextToken());
                }
            }
            return variables;
        }
    }
}
