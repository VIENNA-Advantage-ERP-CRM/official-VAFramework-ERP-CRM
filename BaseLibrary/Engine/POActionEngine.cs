/********************************************************
 * Module/Class Name    : POActionEngine, POAction
 * Purpose              : PO action Engine,create and call POActions  
 * Chronological Development
 * Harwinder Singh 
 ******************************************************/

using System;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace BaseLibrary.Engine
{

    /// <summary>
    /// PO action interface
    /// </summary>
    public interface POAction
    {
        bool BeforeSave(PO po);
        bool AfterSave(bool newRecord, bool succes, PO po);
        bool BeforeDelete(PO po);
        bool AfterDelete(PO po, bool success);
        bool IsAutoUpdateTrl(Ctx ctx, string tableName);
        string GetDocumentNo(int id, PO pO);
        int GetNextID(int AD_Client_ID, string TableName, Trx trx);
        string GetDocumentNo(PO po);
        Lookup GetLookup(Ctx ctx, POInfoColumn colInfo);
        dynamic GetAttachment(Ctx ctx, int aD_Table_ID, int id);
        dynamic CreateAttachment(Ctx ctx, int aD_Table_ID, int id, Trx trx);
    }

    /// <summary>
    /// POAction Class Engine
    /// </summary>
    public class POActionEngine : POAction
    {

        /** Engine Singleton				*/
        private static POActionEngine _engine = null;

        /* PO action Object*/
        private static POAction _action = null;
        /* Class Path */
        private const string CLASS = "VAModelAD.Model.POValidator";
        /* log object */
        private static VLogger s_log = VLogger.GetVLogger(typeof(POActionEngine).FullName);

        /// <summary>
        /// std const
        /// </summary>
        private POActionEngine()
        {
            try
            {
                //Invoke POAtion Class Object
                _action = (POAction)Activator.CreateInstance("VAModelAD", CLASS).Unwrap();
            }
            catch (Exception ex)
            {
                s_log.Severe("PO Action Class not found or initlized" + ex.Message);
            }
        }
        /// <summary>
        /// Get Singleton
        /// </summary>
        /// <returns></returns>
        public static POActionEngine Get()
        {
            if (_engine == null)
            {
                _engine = new POActionEngine();
            }
            return _engine;
        }   //	get

        /// <summary>
        /// Before save action of Model class
        /// </summary>
        /// <param name="po">persistant object</param>
        /// <returns>true if all fine</returns>
        public bool BeforeSave(PO po)
        {
            if (_action != null)
                return _action.BeforeSave(po);
            return false;
        }

        /// <summary>
        /// After save action of model class
        /// </summary>
        /// <param name="newRecord">true if new record</param>
        /// <param name="success">true if before save passed</param>
        /// <param name="po">persistant object</param>
        /// <returns>true if passed</returns>
        public bool AfterSave(bool newRecord, bool success, PO po)
        {
            if (_action != null)
                return _action.AfterSave(newRecord, success, po);
            return false;
        }

        /// <summary>
        /// Before delete action of model class
        /// </summary>
        /// <param name="po">persistant object</param>
        /// <returns>true if passed</returns>
        public bool BeforeDelete(PO po)
        {
            if (_action != null)
                return _action.BeforeDelete(po);
            return false; ;
        }

        /// <summary>
        /// After delete action of model
        /// </summary>
        /// <param name="po">persistant object</param>
        /// <param name="success">true if passed</param>
        /// <returns>true if passed</returns>
        public bool AfterDelete(PO po, bool success)
        {
            if (_action != null)
                return _action.AfterDelete(po, success);
            return false; ;
        }

        /// <summary>
        /// Is Auto Translation
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="tableName">table name</param>
        /// <returns>true if set</returns>
        public bool IsAutoUpdateTrl(Ctx ctx, String tableName)
        {
            if (_action != null)
                return _action.IsAutoUpdateTrl(ctx, tableName);
            return false;
        }

        /// <summary>
        /// Get Document Number sequence
        /// </summary>
        /// <param name="id">doc </param>
        /// <param name="po"></param>
        /// <returns></returns>
        public string GetDocumentNo(int id, PO po)
        {
            if (_action != null)
                return _action.GetDocumentNo(id, po);
            return null;

        }

        /// <summary>
        /// Get next record id 
        /// </summary>
        /// <param name="AD_Client_ID">client id</param>
        /// <param name="TableName">table name</param>
        /// <param name="trx">transaction</param>
        /// <returns>record id</returns>
        public int GetNextID(int AD_Client_ID, string TableName, Trx trx)
        {
            if (_action != null)
                return _action.GetNextID(AD_Client_ID, TableName, trx);
            return 0;
        }

        /// <summary>
        /// Get Document Number
        /// </summary>
        /// <param name="po">persistant object</param>
        /// <returns>document number</returns>
        public string GetDocumentNo(PO po)
        {
            if (_action != null)
                return _action.GetDocumentNo(po);
            return null;
        }

        /// <summary>
        /// Get Record lookup (data)
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="colInfo">colInfo</param>
        /// <returns></returns>
        public Lookup GetLookup(Ctx ctx, POInfoColumn colInfo)
        {
            if (_action != null)
                return _action.GetLookup(ctx, colInfo);
            return null;
        }

        /// <summary>
        /// Get Attachments if any
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Table_ID">table id</param>
        /// <param name="id">record id</param>
        /// <returns>dynamic MAttachment object</returns>
        public dynamic GetAttachment(Ctx ctx, int AD_Table_ID, int id)
        {
            if (_action != null)
                return _action.GetAttachment(ctx, AD_Table_ID, id);
            return null;
        }

        /// <summary>
        /// Create Attachment 
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Table_ID">table id</param>
        /// <param name="id">record id</param>
        /// <param name="trx">transaction object</param>
        /// <returns>dynamic MAttachment object</returns>
        public dynamic CreateAttachment(Ctx ctx, int AD_Table_ID, int id, Trx trx)
        {
            if (_action != null)
                return _action.CreateAttachment(ctx, AD_Table_ID, id, trx);
            return null;
        }
    }

}