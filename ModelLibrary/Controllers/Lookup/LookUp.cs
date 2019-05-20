/********************************************************
// Module Name    : Show field 
// Purpose        : Base Class for MLookup, MLocator.
                    as well as for MLocation, MAccount (only single value)
                    Maintains selectable data as NamePairs in ArrayList
                    The objects itself may be shared by the lookup implementation (ususally HashMap)
// Class Used     : GlobalVariable.cs, CommonFunctions.cs,Ctx.cs
// Created By     : Harwinder 
// Date           : -----   
**********************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.Model;

namespace VAdvantage.Model
{
    /*******************************************************************************
      * 
      *  Lookup Class 
      * 
      ********************************************************************************/

    /// <summary>
    ///  Base Class for MLookup, MLocator.
    ///  as well as for MLocation, MAccount (only single value)
    ///  Maintains selectable data as NamePairs in ArrayList
    ///  The objects itself may be shared by the lookup implementation (ususally HashMap)
    /// </summary>
    public abstract class Lookup
    {

        #region "declaration"
        /**	Ctx					*/
        private Ctx _ctx;
        /**	Window No				*/
        public int _WindowNo;
        /**	Display Type			*/
        public int _displayType;
        /** Disable Validation		*/
        public bool _validationDisabled = false;

        /** The Selected Item       */

        /** The Data List           */
        public volatile List<NamePair> _data = new List<NamePair>();

        /** The Selected Item       */
        private volatile Object _selectedObject;

        /** Temporary Data          */
        private NamePair[] _tempData = null;

        //logger
        protected VLogger log = null;



        #endregion

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="windowNo"></param>
        /// <param name="displayType"></param>
        public Lookup(Ctx ctx, int windowNo, int displayType)
        {
            if (log == null)
            {
                log = VLogger.GetVLogger(this.GetType().FullName);
            }
            SetContext(ctx, windowNo);
            SetDisplayType(displayType);
        }

        /// <summary>
        ///Set Ctx
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="windowNo"></param>
        public void SetContext(Ctx ctx, int windowNo)
        {
            _ctx = ctx;
            _WindowNo = windowNo;
        }

        /// <summary>
        ///Set Display Type
        /// </summary>
        /// <param name="displayType"></param>
        public void SetDisplayType(int displayType)
        {
            _displayType = displayType;
        }	//	SetDisplayType

        /// <summary>
        ///Get Display Type
        /// </summary>
        /// <returns></returns>
        public int GetDisplayType()
        {
            return _displayType;
        }
        /// <summary>
        /// Get Ctx
        /// </summary>
        /// <returns></returns>
        public Ctx GetCtx()
        {
            return _ctx;
        }	//	getCtx

        /// <summary>
        ///Get Window No
        /// </summary>
        /// <returns></returns>
        public int GetWindowNo()
        {
            return _WindowNo;
        }	//	getWindowNo

        /// <summary>
        ///Disable Validation
        /// </summary>
        public virtual void DisableValidation()
        {
            //String validationCode = getValidation();
            //if (validationCode != null && validationCode.length() > 0)
            //{
            //}
            _validationDisabled = true;
        }

        public bool IsValidationDisabled()
        {
            return _validationDisabled;
        }

        /// <summary>
        /// Fill ComboBox with lookup data (async using Worker).
        /// </summary>
        /// <param name="mandatory">has mandatory data only (i.e. no "null" selection)</param>
        /// <param name="onlyValidated">only validated</param>
        /// <param name="onlyActive">only active</param>
        /// <param name="temporary">save current values - restore via fillComboBox (true)</param>
        public void FillComboBox(bool mandatory, bool onlyValidated,
            bool onlyActive, bool temporary)
        {
            //  Save current data
            if (temporary)
            {
                int size = _data.Count;
                _tempData = new NamePair[size];//  KeyValuePair<string,Object>[size];
                //  We need to do a deep copy, so store it in Array
                _tempData = _data.ToArray();
                //	for (int i = 0; i < size; i++)
                //		m_tempData[i] = p_data.get(i);
            }

            //Object obj = _selectedObject;
            if (_data != null)
                _data.Clear();

            //  may cause delay *** The Actual Work ***
            _data = GetData(mandatory, onlyValidated, onlyActive, temporary);

        }

        /// <summary>
        ///Fill ComboBox with old saved data (if exists) or all data available
        /// </summary>
        /// <param name="restore">if true, use saved data - else fill it with all data</param>
        public void FillComboBox(bool restore)
        {
            if (restore && _tempData != null)
            {
                Object obj = _selectedObject;
                _data.Clear();
                //  restore old data
                _data = new List<NamePair>(_tempData.Length);
                for (int i = 0; i < _tempData.Length; i++)
                    _data.Add(_tempData[i]);
                _tempData = null;

                //  if nothing selected, select first
                if (obj == null && _data.Count > 0)
                    obj = _data[0];

                //setSelectedItem(obj);
                //fireContentsChanged(this, 0, p_data.size());
                return;
            }
            if (_data != null)
                FillComboBox(false, false, false, false);
        }   //  fillComboBox

        /// <summary>
        ///Fill ComboBox with Data (Value/KeyNamePair)
        /// </summary>
        /// <param name="mandatory"></param>
        /// <param name="onlyValidated"></param>
        /// <param name="onlyActive"></param>
        /// <param name="temporary"></param>
        /// <returns></returns>

        public abstract List<NamePair> GetData(bool mandatory,
            bool onlyValidated, bool onlyActive, bool temporary);

        public abstract String GetColumnName();

        public abstract NamePair Get(Object key);

        public abstract string GetDisplay(object key);

        /// <summary>
        /// Get Zoom Query String - default implementation
        /// </summary>
        /// <returns>Zoom Query</returns>
        public virtual Query GetZoomQuery()
        {
            return null;
        }

        /// <summary>
        /// Get Zoom - default implementation
        /// </summary>
        /// <returns>Zoom AD_Window_ID</returns>
        public virtual int GetZoomWindow()
        {
            return 0;
        }

        public virtual int GetZoomWindow(Query query)
        {
            return 0;
        }

        /// <summary>
        /// Refresh Values - default implementation
        /// </summary>
        /// <returns>size</returns>
        public virtual int Refresh()
        {
            return 0;
        }

        /// <summary>
        /// Is Validated - default implementation
        /// </summary>
        /// <returns>true if validated</returns>
        public virtual bool IsValidated()
        {
            return true;
        }

        public virtual bool HasInactive()
        {
            return false;
        }


        /// <summary>
        /// Get Data Direct from Table.
        /// Default implementation - does not requery
        /// </summary>
        /// <param name="key"></param>
        /// <param name="saveInCache"></param>
        /// <param name="cacheLocal"></param>
        /// <returns></returns>

        public virtual NamePair GetDirect(object key, bool saveInCache, bool cacheLocal)
        {
           return Get(key);
        }

        /// <summary>
        /// Get dynamic Validation SQL (none)
        /// </summary>
        /// <returns>validation</returns>
        public virtual  string GetValidation()
        {
            return "";
        }

        /// <summary>
        /// Dispose - clear items w/o firing events
        /// </summary>
        public virtual void Dispose()
        {
            if (_data != null)
                _data.Clear();
            _data = null;
            _selectedObject = null;
            _tempData = null;
        }
    }
}
