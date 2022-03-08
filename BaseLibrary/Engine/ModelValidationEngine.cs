using System;
using System.Collections.Generic;
using System.Text;
using VAdvantage.Logging;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Classes;
using System.Data;

namespace VAdvantage.Model
{
    /// <summary>
    /// Model Validator
    /// </summary>
    public interface ModelValidator
    {
        /// <summary>
        /// Initialize Validation
        /// </summary>
        /// <param name="engine">validation engine</param>
        /// //client ID <0 for global Validator
        /// 
        void Initialize(ModelValidationEngine engine, int ClientId);

        /// <summary>
        /// Get Client to be monitored
        /// </summary>
        /// <returns>AD_Client_ID or 0 for ALL</returns>
        int GetAD_Client_ID();

        /// <summary>
        /// User logged in 
        /// Called before preferences are set
        /// </summary>
        /// <param name="AD_Org_ID">AD_Org_ID</param>
        /// <param name="AD_Role_ID">AD_Role_ID</param>
        /// <param name="AD_User_ID">AD_User_ID</param>
        /// <returns>error message or null</returns>
        String Login(int AD_Org_ID, int AD_Role_ID, int AD_User_ID);

        /// <summary>
        /// Model Change of a monitored Table.
        /// Called after PO.beforeSave/PO.beforeDelete
        /// when you called addModelChange for the table
        /// </summary>
        /// <param name="po">persistent object</param>
        /// <param name="changeType">CHANGETYPE_</param>
        /// <returns>error message or null</returns>
        String ModelChange(PO po, int changeType);

        /// <summary>
        /// Validate Document.
        /// Called as first step of DocAction.prepareIt
        /// or at the end of DocAction.completeIt
        /// when you called addDocValidate for the table.
        /// Note that totals, etc. may not be correct before the prepare stage.
        /// </summary>
        /// <param name="po">persistent object</param>
        /// <param name="docTiming">see DOCTIMING_ constants</param>
        /// <returns>error message or null -</returns>
        String DocValidate(PO po, int docTiming);

        /// <summary>
        /// Update Info Window Columns.
        /// - add new Columns
        /// - remove columns
        /// - change display sequence
        /// </summary>
        /// <param name="columns">array of columns</param>
        /// <param name="sqlFrom">from clause, can be modified</param>
        /// <param name="sqlOrder">order by clause, can me modified</param>
        /// <returns>true if you updated columns, sequence or sql From clause</returns>
        bool UpdateInfoColumns(List<Info_Column> columns, StringBuilder sqlFrom, StringBuilder sqlOrder);
    }

    /// <summary>
    /// Model validator variable(s), events flag
    /// </summary>
    public class ModalValidatorVariables
    {
        /** Model Change Type New		*/
        public const int CHANGETYPE_NEW = 1;

        /** Mo      del Change Type New		*/
        public const int TYPE_NEW = 1;

        /** Model Change Type Change	*/
        public const int CHANGETYPE_CHANGE = 2;

        /** Model Change Type Delete	*/
        public const int CHANGETYPE_DELETE = 3;

        /* Model change Type after new */
        public const int TYPE_AFTER_NEW = 4;

        /* Model after change */
        public const int TYPE_AFTER_CHANGE = 5;

        /* Model change Type delete */
        public const int TYPE_AFTER_DELETE = 6;

        /** Called before document is prepared		*/
        public const int DOCTIMING_BEFORE_PREPARE = 1;
        /** Called after document is processed		*/
        public const int DOCTIMING_AFTER_COMPLETE = 9;
    }

    /// <summary>
    /// Model Validation Engine
    /// 
    /// </summary>
    public class ModelValidationEngine
    {
        /** Engine Singleton				*/
        private static ModelValidationEngine _engine = null;
        /* Model validator lists */
        private List<ModelValidator> _validators = new List<ModelValidator>();
        /* global validator*/
        private List<ModelValidator> _globalValidators = new List<ModelValidator>();
        /**	Model Change Listeners			*/
        private Dictionary<String, List<ModelValidator>> _modelChangeListeners = new Dictionary<String, List<ModelValidator>>();
        /**	Document Validation Listeners			*/
        private Dictionary<String, List<ModelValidator>> _docValidateListeners = new Dictionary<String, List<ModelValidator>>();
        //log class object
        private static VLogger s_log = VLogger.GetVLogger(typeof(ModelValidationEngine).FullName);

        /// <summary>
        /// Add model validation classes
        /// </summary>
        private void AddClasses()
        {

            try
            {
                //DataSet dsSys = DB.ExecuteDataset("SELECT ModelValidationClass, AD_Client_ID FROM" +
                //" AD_ModlelValidator WHERE IsActive='Y' AND AD_Client_ID =0 Order By SeqNo ");
                //foreach (DataRow dr in dsSys.Tables[0].Rows)
                //{
                //    String className = dr["ModelValidationClass"].ToString();
                //    if (className == null || className.Length == 0)
                //        continue;
                //    LoadValidatorClass(-1, className);
                //}
            }
            catch (Exception e)
            {
                //logging to db will try to init ModelValidationEngine again!
                s_log.Severe("Error loading System ModelValidator" + e.ToString());
            }

            DataSet ds = DB.ExecuteDataset("SELECT ModelValidationClasses, AD_Client_ID FROM" +
               " AD_Client WHERE IsActive='Y' ");
            // Go through all Clients and start Validators

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                String classNames = dr["ModelValidationClasses"].ToString();
                if (classNames == null || classNames.Length == 0)
                    continue;
                LoadValidatorClasses(Util.GetValueOfInt(dr["AD_Client_ID"]), classNames);
            }
        }

        /// <summary>
        /// std constructor
        /// </summary>
        private ModelValidationEngine()
        {
            AddClasses();
        }

        private void LoadValidatorClasses(int client, String classNames)
        {
            StringTokenizer st = new StringTokenizer(classNames, ";");
            while (st.HasMoreTokens())
            {
                String className = null;
                try
                {
                    className = st.NextToken();
                    if (className == null)
                        continue;
                    className = className.Trim();
                    if (className.Length == 0)
                        continue;
                    //
                    LoadValidatorClass(client, className);
                }
                catch (Exception e)
                {
                    //log error
                    s_log.Severe("Error in Client ModelValidation class =>" + client + " " + e.ToString());
                }
            }
        }

        private void LoadValidatorClass(int client, String className)
        {
            try
            {
                // namespace and assembly must be same
                ModelValidator validator = null;
                string asmName = className.Substring(0, className.IndexOf("."));
                //Assembly
                System.Reflection.Assembly asm = System.Reflection.Assembly.Load(asmName);

                validator = (ModelValidator)Activator.CreateInstance(asmName, className).Unwrap();
                if (validator == null)
                {
                    s_log.Severe("no modelvalidator class found =>" + className);
                }
                else
                {
                    Initialize(validator, client);
                }
            }
            catch (Exception e)
            {
                //logging to db will try to init ModelValidationEngine again!
                s_log.Severe("error in invoking ModelValidator class =>" + className);
            }
        }

        /// <summary>
        /// Initalize Model validaton class
        /// </summary>
        /// <param name="validator">model validator object</param>
        /// <param name="client">client id</param>
        private void Initialize(ModelValidator validator, int client)
        {
            if (client < 0)//global
                _globalValidators.Add(validator);
            _validators.Add(validator);
            validator.Initialize(this, client);
        }	//	initialize

        /// <summary>
        /// Get Singleton object
        /// </summary>
        /// <returns></returns>
        public static ModelValidationEngine Get()
        {
            if (_engine == null)
                _engine = new ModelValidationEngine();
            return _engine;
        }	//	get

        /// <summary>
        /// Called when Login is complete 
        /// </summary>
        /// <param name="AD_Client_ID">client id</param>
        /// <param name="AD_Org_ID">organization Id</param>
        /// <param name="AD_Role_ID">role id</param>
        /// <param name="AD_User_ID">user id</param>
        /// <returns>error message if any</returns>
        public String LoginComplete(int AD_Client_ID, int AD_Org_ID, int AD_Role_ID, int AD_User_ID)
        {
            for (int i = 0; i < _validators.Count; i++)
            {
                ModelValidator validator = (ModelValidator)_validators[i];
                if (AD_Client_ID == validator.GetAD_Client_ID())
                {
                    String error = validator.Login(AD_Org_ID, AD_Role_ID, AD_User_ID);
                    if (error != null && error.Length > 0)
                        return error;
                }
            }
            return null;
        }   //	loginComplete

        /// <summary>
        /// Update Info Window Columns.
        /// - add new Columns
        /// - remove columns
        /// - change dispay sequence
        /// </summary>
        /// <param name="AD_Client_ID">AD_Client_ID</param>
        /// <param name="columns">columns</param>
        /// <param name="sqlFrom">sqlFrom</param>
        /// <param name="sqlOrder">sqlOrder</param>
        /// <returns>true if you updated columns, sequence or sql From clause</returns>
        public bool UpdateInfoColumns(int AD_Client_ID, List<Info_Column> columns,
            StringBuilder sqlFrom, StringBuilder sqlOrder)
        {
            bool retValue = true;
            for (int i = 0; i < _validators.Count; i++)
            {
                ModelValidator validator = (ModelValidator)_validators[i];
                if (validator.GetAD_Client_ID() == 0 || validator.GetAD_Client_ID() == AD_Client_ID)
                {
                    try
                    {
                        bool bb = validator.UpdateInfoColumns(columns, sqlFrom, sqlOrder);
                        if (bb)
                            retValue = true;
                    }
                    catch (Exception e)
                    {
                        s_log.Warning(validator.ToString() + ": " + e);
                    }
                }
            }
            return retValue;
        }	//	updateInfoColumns

        /// <summary>
        /// Fire Document Validation.
        /// Call docValidate method of added validators
        /// </summary>
        /// <param name="po">persistent objects</param>
        /// <param name="docTiming">see ModalValidatorVariables.DOCTIMING_ constants</param>
        /// <returns>error message or null</returns>
        public String FireDocValidate(PO po, int docTiming)
        {
            if (po == null || _docValidateListeners.Count == 0)
                return null;
            //
            String propertyName = po.Get_TableName() + po.GetAD_Client_ID();
            List<ModelValidator> list = null;
            if (_docValidateListeners.ContainsKey(propertyName))
            {
                list = _docValidateListeners[propertyName];
            }
            if (list == null || list.Count == 0)
                return null;

            //
            for (int i = 0; i < list.Count; i++)
            {
                ModelValidator validator = null;
                try
                {
                    validator = (ModelValidator)list[i];
                    if (validator.GetAD_Client_ID() == po.GetAD_Client_ID())
                    {
                        String error = validator.DocValidate(po, docTiming);
                        if (error != null && error.Length > 0)
                            return error;
                    }
                }
                catch (Exception e)
                {

                    s_log.Log(Level.SEVERE, validator.ToString(), e);
                }
            }
            return null;
        }

        /// <summary>
        /// Fire Model Change.
	    ///	Call modelChange method of added validators
        /// </summary>
        /// <param name="po">persistent objects</param>
        /// <param name="changeType">ModalValidatorVariables.CHANGETYPE_*</param>
        /// <returns>error message or NULL for no veto</returns>
        public String FireModelChange(PO po, int changeType)
        {
            if (po == null || _modelChangeListeners.Count == 0)
                return null;
            //
            String propertyName = po.Get_TableName() + po.GetAD_Client_ID();
            List<ModelValidator> list = null;
            if (_modelChangeListeners.ContainsKey(propertyName))
            {
                list = _modelChangeListeners[propertyName];
            }
            if (list == null || list.Count == 0)
                return null;

            //
            for (int i = 0; i < list.Count; i++)
            {
                try
                {
                    ModelValidator validator = (ModelValidator)list[i];
                    if (validator.GetAD_Client_ID() == po.GetAD_Client_ID())
                    {
                        String error = validator.ModelChange(po, changeType);
                        if (error != null && error.Length > 0)
                            return error;
                    }
                }
                catch (Exception e)
                {
                    String error = e.Message;
                    if (error == null)
                        error = e.ToString();
                    return error;
                }
            }
            return null;
        }

        /// <summary>
        /// Add Model change Listner
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="listener">listner</param>
        public void AddModelChange(String tableName, ModelValidator listener)
        {
            if (tableName == null || listener == null)
                return;
            //
            String propertyName =
                _globalValidators.Contains(listener)
                    ? tableName + "*"
                    : tableName + listener.GetAD_Client_ID();

            if (!_modelChangeListeners.ContainsKey(propertyName))
            {
                _modelChangeListeners.Add(propertyName, new List<ModelValidator>());
            }

            _modelChangeListeners[propertyName].Add(listener);
        }   //	AddModelValidator

        /// <summary>
        /// Remove model change listner
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="listener">listner</param>
        public void RemoveModelChange(String tableName, ModelValidator listener)
        {
            if (tableName == null || listener == null)
                return;
            String propertyName =
               _globalValidators.Contains(listener)
                    ? tableName + "*"
                    : tableName + listener.GetAD_Client_ID();

            if (!_modelChangeListeners.ContainsKey(propertyName))
                return;

            _modelChangeListeners[propertyName].Remove(listener);
            if (_modelChangeListeners[propertyName].Count == 0)
                _modelChangeListeners.Remove(propertyName);
        }   //RemoveModelValidator

        /// <summary>
        /// Add Document validation listner
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="listener">listner</param>
        public void AddDocValidate(String tableName, ModelValidator listener)
        {
            if (tableName == null || listener == null)
                return;
            //
            String propertyName =
                _globalValidators.Contains(listener)
                    ? tableName + "*"
                    : tableName + listener.GetAD_Client_ID();

            if (!_docValidateListeners.ContainsKey(propertyName))
            {
                _docValidateListeners.Add(propertyName, new List<ModelValidator>());
            }

            _docValidateListeners[propertyName].Add(listener);
        }   //	AddModelValidator

        /// <summary>
        /// Remove document validation listner
        /// </summary>
        /// <param name="tableName">tablename</param>
        /// <param name="listener">listner</param>
        public void RemoveDocValidate(String tableName, ModelValidator listener)
        {
            if (tableName == null || listener == null)
                return;
            String propertyName =
               _globalValidators.Contains(listener)
                    ? tableName + "*"
                    : tableName + listener.GetAD_Client_ID();

            if (!_docValidateListeners.ContainsKey(propertyName))
                return;

            _docValidateListeners[propertyName].Remove(listener);
            if (_docValidateListeners[propertyName].Count == 0)
                _docValidateListeners.Remove(propertyName);
        }	//RemoveModelValidator

    }
}


