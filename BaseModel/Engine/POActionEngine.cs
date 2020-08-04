using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace BaseModel.Engine
{

    public interface POAction
    {
        bool BeforeSave(PO po);
        bool AfterSave(bool newRecord, bool succes, PO po);
        bool BeforeDelete(PO po);
        bool AfterDelete(PO po, bool success);
        bool IsAutoUpdateTrl(Ctx ctx);
        string GetDocumentNo(int id, PO pO);
        int GetNextID(int AD_Client_ID, string TableName, Trx trx);
       string  GetDocumentNo(PO po);
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
            throw new NotImplementedException();
        }

        public bool AfterSave(PO po, bool success, bool newRecord)
        {
            throw new NotImplementedException();
        }

        public bool BeforeDelete(PO po)
        {
            throw new NotImplementedException();
        }

        public bool AfterDelete(PO po, bool success)
        {
            throw new NotImplementedException();
        }

        public bool IsAutoUpdateTrl(Ctx ctx)
        {
            throw new NotImplementedException();
        }

        public string GetDocumentNo(int id,  PO pO)
        {
            throw new NotImplementedException();
        }

        public int GetNextID(int AD_Client_ID, string TableName, Trx trx)
        {
            throw new NotImplementedException();
        }

        public string GetDocumentNo(PO po)
        {
            
        }
    }
}
