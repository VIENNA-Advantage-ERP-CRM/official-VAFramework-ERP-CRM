/********************************************************
 * Module Name    :     Report
 * Purpose        :     Generate Reports
 * Author         :     Jagmohan Bhatt
 * Date           :     23-Jun-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace VAdvantage.Print
{
    public class PrintDataGroup
    {
        /**	Column-Function Delimiter		*/
        static public String DELIMITER = "~";
        /**	Grand Total Indicator			*/
        static public String TOTAL = "=TOTAL=";
        /**	NULL substitute value			*/
        static private Object NULL=null;


        /**	List of group columns			*/
        private List<String> _groups = new List<String>();
        /** Map of group column & value		*/
        private Dictionary<String, Object> _groupMap = new Dictionary<String, Object>();
        /**	List of column_function			*/
        private List<String> _functions = new List<String>();
        /** Map of group_function column & function	*/
        private Dictionary<String, PrintDataFunction> _groupFunction = new Dictionary<String, PrintDataFunction>();


        public void AddGroupColumn(String groupColumnName)
        {
            _groups.Add(groupColumnName);
        }	//	addGroup
        
        public void AddFunction(String functionColumnName, char function)
        {
            _functions.Add(functionColumnName + DELIMITER + function);
            if (!_groups.Contains(TOTAL))
                _groups.Add(TOTAL);
        }	//	addFunction


        public int GetGroupColumnCount()
        {
            return _groups.Count;
        }	//	getGroupColumnCount

        public bool IsGroupColumn(String columnName)
        {
            if (columnName == null || _groups.Count == 0)
                return false;
            for (int i = 0; i < _groups.Count; i++)
            {
                if (columnName.Equals(_groups[i]))
                    return true;
            }
            return false;
        }	//	isGroupColumn


        public Object GroupChange(String groupColumnName, Object value)
        {
            if (!IsGroupColumn(groupColumnName))
                return null;
            Object newValue = value;
            if (newValue == null)
                newValue = NULL;
            //
            if (_groupMap.ContainsKey(groupColumnName))
            {
                Object oldValue = _groupMap[groupColumnName];
                if (newValue.Equals(oldValue))
                    return null;
                _groupMap[groupColumnName] =  newValue;
                return oldValue;
            
            }

            _groupMap[groupColumnName] =  newValue; 
            return null;
        }	//	groupChange


        public bool IsFunctionColumn(String columnName)
        {
            if (columnName == null || _functions.Count == 0)
                return false;
            for (int i = 0; i < _functions.Count; i++)
            {
                String f = (String)_functions[i];
                if (f.StartsWith(columnName))
                    return true;
            }
            return false;
        }	//	isFunctionColumn


        public char[] GetFunctions(String columnName)
        {
            
            List<String> list = new List<String>();	//	the final function List
            //IEnumerator it = _groupFunction.Values.GetEnumerator();
            
            //while (it.MoveNext())

            foreach(KeyValuePair<String, PrintDataFunction> pair in _groupFunction)
            {
                String group_function = pair.Key;
                if (group_function.StartsWith(columnName))
                {
                    group_function = group_function.Substring(group_function.LastIndexOf(DELIMITER) + 1);	//	LoadSeq
                    for (int i = 0; i < _functions.Count; i++)
                    {
                        String col_function = ((String)_functions[i]);	//	LoadSeq~A
                        if (col_function.StartsWith(group_function))
                        {
                            String function = col_function.Substring(col_function.LastIndexOf(DELIMITER) + 1);
                            if (!list.Contains(function))
                                list.Add(function);
                        }
                    }
                }
            }
            //	Return Value
            char[] retValue = new char[list.Count];
            for (int i = 0; i < retValue.Length; i++)
                retValue[i] = ((String)list[i])[0];
            //	log.finest( "PrintDataGroup.getFunctions for " + columnName + "/" + retValue.length, new String(retValue));
            return retValue;
        }	//	getFunctions


        public bool IsFunctionColumn(String columnName, char function)
        {
            if (columnName == null || _functions.Count == 0)
                return false;
            String key = columnName + DELIMITER + function;
            for (int i = 0; i < _functions.Count; i++)
            {
                String f = (String)_functions[i];
                if (f.Equals(key))
                    return true;
            }
            return false;
        }	//	isFunctionColumn


        public void AddValue(String functionColumnName, Decimal functionValue)
        {
            if (!IsFunctionColumn(functionColumnName))
                return;
            //	Group Breaks
            for (int i = 0; i < _groups.Count; i++)
            {
                String groupColumnName = (String)_groups[i];
                String key = groupColumnName + DELIMITER + functionColumnName;

                PrintDataFunction pdf = null;
                if(_groupFunction.ContainsKey(key))
                    pdf = (PrintDataFunction)_groupFunction[key];
                if (pdf == null)
                    pdf = new PrintDataFunction();
                pdf.AddValue(functionValue);


                if (_groupFunction.ContainsKey(key))
                    _groupFunction[key] = pdf;
                else
                    _groupFunction.Add(key, pdf);
                //_groupFunction.Add("one", pdf);
            }
        }	//	addValue


        public Decimal? GetValue(String groupColumnName, String functionColumnName,
            char function)
        {
            String key = groupColumnName + DELIMITER + functionColumnName;
            PrintDataFunction pdf = null;
            if(_groupFunction.ContainsKey(key))
                pdf = (PrintDataFunction)_groupFunction[key];
            if (pdf == null)
                return null;
            return pdf.GetValue(function);
        }	//	getValue

        public void Reset(String groupColumnName, String functionColumnName)
        {
            String key = groupColumnName + DELIMITER + functionColumnName;
            PrintDataFunction pdf = null;
            if (_groupFunction.ContainsKey(key))
                 pdf = (PrintDataFunction)_groupFunction[key];
            if (pdf != null)
                pdf.Reset();
        }	//	reset

    }
}
