using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace BaseLibrary.Engine
{

    public interface POAction
    {
        bool BeforeSave(PO po);
        bool AfterSave(bool newRecord, bool succes, PO po);
        bool BeforeDelete(PO po);
        bool AfterDelete(PO po, bool success);
        bool IsAutoUpdateTrl(Ctx ctx,string tableName);
        string GetDocumentNo(int id, PO pO);
        int GetNextID(int AD_Client_ID, string TableName, Trx trx);
       string  GetDocumentNo(PO po);
        Lookup GetLookup(POInfoColumn colInfo);

    }



    public class POActionEngine : POAction
    {

        /** Engine Singleton				*/
        private static POActionEngine _engine = null;

        private static POAction _action = null;
        private const string CLASS = "VAdvantage.Model.POValidator";

        private static VLogger s_log = VLogger.GetVLogger(typeof(POActionEngine).FullName);

        private POActionEngine()
        {
            try
            {
                //Libraray name
                //Loaq lib 
                _action = (POAction)Activator.GetObject(typeof(POAction), CLASS);
            }
            catch
            {
                s_log.Severe("PO Action Class not found or initlized");
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

        public bool BeforeSave(PO po)
        {
            if (_action != null)
                return _action.BeforeSave(po);
            return false;
        }

        public bool AfterSave(bool newRecord, bool success, PO po)
        {
            if (_action != null)
                return _action.AfterSave(newRecord,success, po);
            return false;
        }

        public bool BeforeDelete(PO po)
        {
            if (_action != null)
                return _action.BeforeDelete(po);
            return false; ;
        }

        public bool AfterDelete(PO po, bool success)
        {
            if (_action != null)
                return _action.AfterDelete(po,success);
            return false; ;
        }

        public bool IsAutoUpdateTrl(Ctx ctx,String tableName)
        {
            if (_action != null)
                return _action.IsAutoUpdateTrl(ctx,tableName);
            return false;
        }

        public string GetDocumentNo(int id,  PO po)
        {
            if (_action != null)
                return _action.GetDocumentNo(id,po);
            return null;

        }

        public int GetNextID(int AD_Client_ID, string TableName, Trx trx)
        {
            if (_action != null)
                return _action.GetNextID(AD_Client_ID,TableName,trx);
            return 0;
        }

        public string GetDocumentNo(PO po)
        {
            if (_action != null)
              return   _action.GetDocumentNo(po);
            return null;
        }

        public Lookup GetLookup(POInfoColumn colInfo)
        {
            if (_action != null)
                return _action.GetLookup(colInfo);
            return null;
        }
    }
}
