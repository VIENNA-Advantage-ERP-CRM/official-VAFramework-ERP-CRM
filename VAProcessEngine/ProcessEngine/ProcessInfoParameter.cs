/********************************************************
 * Module Name    : Process
 * Purpose        : Execute the process
 * Author         : Jagmohan Bhatt
 * Date           : 12 feb 2009
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.ProcessEngine
{
    [Serializable]
    public class ProcessInfoParameter
    {
        private String _parameterName;
        private Object _parameter;
        private Object _parameter_To;
        private String _info = "";
        private String _info_To = "";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameterName">parameterName</param>
        /// <param name="parameter">parameter</param>
        /// <param name="parameter_To">parameter_To</param>
        /// <param name="info">info</param>
        /// <param name="info_To">info_To</param>
        public ProcessInfoParameter(String parameterName, Object parameter, Object parameter_To, String info, String info_To)
        {
            SetParameterName(parameterName);
            SetParameter(parameter);
            SetParameter_To(parameter_To);
            SetInfo(info);
            SetInfo_To(info_To);
        } 

        /// <summary>
        /// Override tostring method
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            //	From .. To
            if (_parameter_To != null || _info_To.Length > 0)
                return "ProcessInfoParameter[" + _parameterName + "=" + _parameter
                    + (_parameter == null ? "" : "{" + _parameter.GetType().Name + "}")
                    + " (" + _info + ") - "
                    + _parameter_To
                    + (_parameter_To == null ? "" : "{" + _parameter_To.GetType().Name + "}")
                    + " (" + _info_To + ")";
            //	Value
            return "ProcessInfoParameter[" + _parameterName + "=" + _parameter
                + (_parameter == null ? "" : "{" + _parameter.GetType().Name + "}")
                + " (" + _info + ")";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public String GetInfo()
        {
            return _info;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public String GetInfo_To()
        {
            return _info_To;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Object GetParameter()
        {
            return _parameter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int  GetParameterAsInt()
        {
            if (_parameter == null || _parameter.ToString().Trim() == "")
                return 0;
            if (_parameter is int)
                return Utility.Util.GetValueOfInt(_parameter.ToString());

            Decimal bd = Decimal.Parse(_parameter.ToString());
            return int.Parse(bd.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Object GetParameter_To()
        {
            return _parameter_To;
        }

        /// <summary>
        /// GetParameter_To As Int
        /// </summary>
        /// <returns>value as int</returns>
        public int GetParameter_ToAsInt()
        {
            if (_parameter_To == null || _parameter_To.ToString().Trim() == "")
                return 0;
            if (_parameter_To is int)
                return int.Parse(_parameter_To.ToString());

            Decimal bd = Decimal.Parse(_parameter_To.ToString());
            return int.Parse(bd.ToString());
        }

        public String GetParameterName()
        {
            return _parameterName;
        }

        /// <summary>
        /// SetInfo
        /// </summary>
        /// <param name="Info">Info</param>
        public void SetInfo(String Info)
        {
            if (Info == null)
                _info = "";
            else
                _info = Info;
        }

        /// <summary>
        /// Set Info to
        /// </summary>
        /// <param name="Info_To">Info_To</param>
        public void SetInfo_To(String Info_To)
        {
            if (Info_To == null)
                _info_To = "";
            else
                _info_To = Info_To;
        }

        /// <summary>
        /// Set param
        /// </summary>
        /// <param name="Parameter">Parameter</param>
        public void SetParameter(Object Parameter)
        {
            _parameter = Parameter;
        }

        /// <summary>
        /// Set param to
        /// </summary>
        /// <param name="parameter_To">parameter_To</param>
        public void SetParameter_To(Object parameter_To)
        {
            _parameter_To = parameter_To;
        }

        /// <summary>
        /// Set param name
        /// </summary>
        /// <param name="parameterName">parameterName</param>
        public void SetParameterName(String parameterName)
        {
            _parameterName = parameterName;
        }
    }
}
