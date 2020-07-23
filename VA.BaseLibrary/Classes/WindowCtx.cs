/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : WindowCtx
 * Purpose        : To get where close records
 * Class Used     : 
 * Chronological    Development
 * Raghunandan      10-April-2009 
  ******************************************************/
//Class not tested

/********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Collections;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.SqlExec;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using System.Threading;


namespace VAdvantage.Classes
{
    [Serializable]
    public class WindowCtx// : ISerializable
    {

        //private static long serialVersionUID = 1L;
        //HashMap changed to Hashtable
        private Hashtable _strMap;
        private Hashtable _objMap;

        /// <summary>
        /// Class Constructor
        /// </summary>
        public WindowCtx()
        {
            _strMap = new Hashtable();
            _objMap = new Hashtable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strMap"></param>
        /// <param name="objMap"></param>
        public WindowCtx(Hashtable strMap, Hashtable objMap)
        {
            _strMap = strMap;
            _objMap = objMap;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetStringContext(string name)
        {
            //return (String) m_strMap.get( name );
            return _strMap[name].ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Object GetObjectContext(string name)
        {
            //return _objMap.get( name );
            return _objMap[name];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string Get(string name)
        {
            return GetStringContext(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetAsInt(string name)
        {
            int value = 0;
            try
            {
                //value = int.Parse( get( name ) );
            }
            catch 
            {
            }
            return value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool GetAsBoolean(string name)
        {
            //return "Y".Equals( get( name ) );
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Put(string name, string value)
        {
            //_strMap.put( name, value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void PutObject(string name, object value)
        {
            //_objMap.put( name, value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Hashtable GetStringMap()
        {
            return _strMap;
        }
    }
    
}
