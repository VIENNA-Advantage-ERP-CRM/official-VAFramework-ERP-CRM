using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VAdvantage.DataBase;

using System.Data.SqlClient;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Common;
using System.Threading;
using VAdvantage.Model;
using VAdvantage.Process;
using VAdvantage.Logging;
namespace VAdvantage.Classes
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
        void Initialize(ModelValidationEngine engine);

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

    public class ModalValidatorVariables
    {
        /** Model Change Type New		*/
        public const int CHANGETYPE_NEW = 1;
        /** Model Change Type Change	*/
        public const int CHANGETYPE_CHANGE = 2;
        /** Model Change Type Delete	*/
        public const int CHANGETYPE_DELETE = 3;

        /** Called before document is prepared		*/
        public const int DOCTIMING_BEFORE_PREPARE = 1;
        /** Called after document is processed		*/
        public const int DOCTIMING_AFTER_COMPLETE = 9;
    }

    /// <summary>
    /// Model Validation Engine
    /// </summary>
    public class ModelValidationEngine
    {

        /** Engine Singleton				*/
        private static ModelValidationEngine _engine = null;

        private List<ModelValidator> _validators = new List<ModelValidator>();

        /**	Model Change Listeners			*/
        private Dictionary<String, List<ModelValidator>> _modelChangeListeners = new Dictionary<String, List<ModelValidator>>();
        /**	Document Validation Listeners			*/
        private Dictionary<String, List<ModelValidator>> _docValidateListeners = new Dictionary<String, List<ModelValidator>>();
        private static VLogger s_log = VLogger.GetVLogger(typeof(ModelValidationEngine).FullName);

        /// <summary>
        /// 
        /// </summary>
        public void AddClasses()
        {
            //Context ctx = Utility.Env.GetContext();
            //VAdvantage.Model.MClient[] clients = VAdvantage.Model.MClient.GetAll(ctx);

            ////	Go through all Clients and start Validators 
            //for (int i = 0; i < clients.Count(); i++)
            //{
            //    String classNames = clients[i].GetModelValidationClasses();
            //    if (classNames == null || classNames.Length == 0)
            //        continue;


            //    string[] sep = {";: "};
            //    string[] st = classNames.Split(sep);
            //}

        }

        /// <summary>
        /// Get Singleton
        /// </summary>
        /// <returns></returns>
        public static ModelValidationEngine Get()
        {
            if (_engine == null)
                _engine = new ModelValidationEngine();
            return _engine;
        }	//	get

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
        }	//	loginComplete
	

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
                list = new List<ModelValidator>();
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
                list = new List<ModelValidator>();
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

    }
}

