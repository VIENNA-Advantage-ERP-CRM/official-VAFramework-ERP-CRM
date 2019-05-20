/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ListBoxVO
 * Purpose        : 
 * Class Used     : Serializable
 * Chronological    Development
 * Raghunandan     18-Dec-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    [Serializable]
    public class ListBoxVO
    {
        #region PrivateVariable

        
        public static VLogger log = VLogger.GetVLogger(typeof(ListBoxVO).FullName);
        // SQL query     
        public String query = null;
        // Table Name     
        public String tableName = "";
        // Key Column     
        public String keyColumn = "";
        // Zoom Window    
        public int zoomWindow;
        // Zoom Window    
        public int zoomWindowPO;
        // Zoom query     
        //public QueryVO		ZoomQuery = null;

        //Direct Access query
        public String queryDirect = "";
        //Parent Flag     
        public bool isParent = false;
        //Key Flag     	
        public bool isKey = false;
        // Validation code 
        public String validationCode = "";
        // Validation flag 
        public bool isValidated = false;

        //	AD_Column_Info or AD_Process_Para	
        public int Column_ID;
        // Real AD_Reference_ID				
        public int AD_Reference_Value_ID;
        // CreadedBy?updatedBy				
        public bool isCreadedUpdatedBy = false;

        //private static long serialVersionUID = 1L;

        private String _defaultKey = null;

        // do not change to HashMap; using an array to maintain sorted order
        
        //private ArrayList _options = null;
        private List<NamePair> _options = null;
        /**
         
         */
        //private ArrayList _exception_options = null;
        private List<NamePair> _exception_options = null;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exceptionOptions"></param>
        /// public void SetExceptionOptions(ArrayList exceptionOptions)
        public void SetExceptionOptions(List<NamePair> exceptionOptions)
        {
            if (_exception_options == null)
            {
                _exception_options = new List<NamePair>();//  ArrayList();
            }
            ConcatNamePairArray(_exception_options, exceptionOptions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validOptions"></param>
        /// public void SetOptions(ArrayList validOptions)
        public void SetOptions(List<NamePair> validOptions)
        {
            if (Build.IsDebug())
            {
                log.Finest("setOptions(before):" + this);
            }
            _options = validOptions;
            if (Build.IsDebug())
            {
                log.Finest("setOptions(after):" + this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        public void AddOption(NamePair option)
        {
            if (option != null)
            {
                if (NamePair.IndexOfKey(_options, option.GetID()) == -1)
                {
                    _options.Add(option);
                }
            }
            else
            {
                throw new ArgumentException("option should not be null");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        public void PushExceptionOption(NamePair option)
        {

            if (_exception_options == null)
            {
                _exception_options = new List<NamePair>();// ArrayList(10);
            }
            if (option != null)
            {
                if (NamePair.IndexOfKey(_exception_options, option.GetID()) == -1)
                {
                    _exception_options.Add(option);
                }
            }
            else
            {
                throw new ArgumentException("option should not be null");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// public void PushExceptionOptions(ArrayList options)
        public void PushExceptionOptions(List<NamePair> options)
        {
            if (Build.IsDebug())
                log.Finest("pushExceptionOptions(before):" + this);

            for (int i = 0; i < options.Count; i++)
            {
                PushExceptionOption((NamePair)options[i]);
            }
            if (Build.IsDebug())
            {
                log.Finest("pushExceptionOptions(before):" + this);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="c"></param>
        ///  public static void ConcatNamePairArray(ArrayList options, ArrayList c)
        public static void ConcatNamePairArray(List<NamePair> options, List<NamePair> c)
        {
            if (c == null)
            {
                return;
            }
            if (options == null)
            {
                throw new ArgumentException("options cannot be null");
            }
            if (options != null)
            {
                for (int i = 0; i < c.Count; i++)
                {
                    if (NamePair.IndexOfKey(options, ((NamePair)c[i]).GetID()) == -1)
                    {
                        options.Add(c[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ListBoxVO()
            : this(true)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="init"></param>
        public ListBoxVO(bool init)
        {
            if (init)
            {
                _options = new List<NamePair>(4);//     ArrayList(4);
                this._defaultKey = "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_options"></param>
        /// <param name="_defaultKey"></param>
        public ListBoxVO(NamePair[] _options, String _defaultKey)
        {
            //ArrayList options = new ArrayList(_options.Length);
            List<NamePair> options = new List<NamePair>(_options.Length);
            for (int i = 0; i < _options.Length; i++)
            {
                options.Add(_options[i]);
            }
            Create(options, _defaultKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_options"></param>
        /// <param name="p_defaultKey"></param>
        /// public ListBoxVO(ArrayList _options, String _defaultKey)
        public ListBoxVO(List<NamePair> _options, String _defaultKey)
        {
            Create(_options, _defaultKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_options"></param>
        /// <param name="p_defaultKey"></param>
        /// private void Create(ArrayList p_options, String p_defaultKey)
        private void Create(List<NamePair> p_options, String p_defaultKey)
        {
            //for(int i=0; i<_options.size(); i++) 
            _options = p_options;
            //ZD, toArray doesn't work in GWT     _options = (NamePair[])p_options.toArray();

            if (null == _options)
            {
                _options = new List<NamePair>();//  ArrayList();
            }
            _defaultKey = p_defaultKey;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public String GetDefaultKey()
        {
            return _defaultKey;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultKey"></param>
        public void SetDefaultKey(String defaultKey)
        {
            _defaultKey = defaultKey;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// public ArrayList GetOptions()
        public List<NamePair> GetOptions()
        {
            return _options;
        }

        /// <summary>
        ///  Returns the index of the first option found for the key, or -1 if not found.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int IndexOfKey(String key)
        {
            return NamePair.IndexOfKey(_options, key);
        }

        /// <summary>
        /// Returns the index of the first option found for the key, or -1 if not found.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOfValue(String value)
        {
            return NamePair.IndexOfValue(_options, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public String GetValue(String key)
        {
            NamePair option = GetOption(key);
            if (option == null)
            {
                return null;
            }
            return option.GetName();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public NamePair GetValidOption(String key)
        {
            int index = NamePair.IndexOfKey(_options, key);
            if (index >= 0)
            {
                return ((NamePair)_options[index]);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public NamePair GetOption(String key)
        {
            int index = NamePair.IndexOfKey(_options, key);
            if (index >= 0)
            {
                return ((NamePair)_options[index]);
            }
            else
            {
                if (_exception_options == null)
                {
                    return null;
                }
                index = NamePair.IndexOfKey(_exception_options, key);
                if (index >= 0)
                {
                    return ((NamePair)_exception_options[index]);
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// public ArrayList GetExceptionOptions()
        public List<NamePair> GetExceptionOptions()
        {
            return this._exception_options;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return "_options:" + _options + "_exception_options:" + _exception_options;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsOptionsSet()
        {
            return _options != null;
        }
    }



    public class Build
    {
        public const int OFFICIAL = 0;
        public const int DEBUG = 1;
        public const int VERBOSE = 2;
        public static int mode = OFFICIAL;

        /// <summary>
        /// VerBose
        /// </summary>
        /// <returns></returns>
        public static bool IsVerbose()
        {
            return (mode >= VERBOSE);
        }

        /// <summary>
        /// Debuge check
        /// </summary>
        /// <returns></returns>
        public static bool IsDebug()
        {
            return (mode >= DEBUG);
        }

        /// <summary>
        /// Official
        /// </summary>
        /// <returns></returns>
        public static bool IsOfficial()
        {
            return (mode == OFFICIAL);
        }

    }
}
