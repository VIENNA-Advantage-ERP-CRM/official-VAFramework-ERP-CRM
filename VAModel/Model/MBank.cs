/********************************************************
 * Module Name    : VFramwork (Model class)
 * Purpose        : Get records from table C_Table
 * Class Used     : MBank
 * Chronological Development
 * Raghunandan    24-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Common;
using VAdvantage.Utility;
using System.Data;
using System.Windows.Forms;
using VAdvantage.SqlExec;
using VAdvantage.DataBase;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MBank : X_C_Bank
    {
        #region variables
        // Verification Class				
        private BankVerificationInterface _verify = null;
        // Searched for verification class	
        private bool _verifySearched = false;
        // Bank Location		
        private MLocation _loc = null;
        //	Cache
        private static CCache<int, MBank> _cache = new CCache<int, MBank>("C_Bank", 3);
        //	Logger
        private static VLogger _log = VLogger.GetVLogger(typeof(MBPBankAccount).FullName);
        #endregion

        /*	Get MBank from Cache
        *	@param ctx context
        *	@param C_Bank_ID id
        *	@return MBank
        */
        public static MBank Get(Ctx ctx, int C_Bank_ID)
        {
            int key = C_Bank_ID;
            MBank retValue = (MBank)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MBank(ctx, C_Bank_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }

        /***
         * 	Standard Constructor
         *	@param ctx context
         *	@param C_Bank_ID bank
         *	@param trxName trx
         */
        public MBank(Ctx ctx, int C_Bank_ID, Trx trxName)
            : base(ctx, C_Bank_ID, trxName)
        {

        }

        /**
         * 	get Bank by RoutingNo
         *	@param ctx
         *	@param routingNo
         *	@return Array of banks with this RoutingNo
         */
        public static MBank[] GetByRoutingNo(Ctx ctx, String routingNo)
        {
            String sql = "SELECT * FROM C_Bank WHERE RoutingNo LIKE '" + routingNo + "' AND IsActive='Y'";
            List<MBank> list = new List<MBank>();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MBank(ctx, dr, null));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally {
                dt = null;
            }

            MBank[] retValue = new MBank[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName trx
         */
        public MBank(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }



        /**
         * 	Get Verification Class
         *	@return verification class
         */
        public BankVerificationInterface GetVerificationClass()
        {
            if (_verify == null && !_verifySearched)
            {
                String className = GetBankVerificationClass();
                if (className == null || className.Length == 0)
                    className = MClientInfo.Get(GetCtx(), GetAD_Client_ID()).GetBankVerificationClass();
                if (className != null && className.Length > 0)
                {
                    try
                    {
                        //Class clazz = Class.forName(className);
                        Type clazz = Type.GetType(className);
                        _verify = (BankVerificationInterface)Activator.CreateInstance(clazz);
                    }
                    catch (Exception e)
                    {
                        log.Log(Level.SEVERE, className, e);
                    }
                }
                _verifySearched = true;
            }
            return _verify;
        }

        /**
         * 	Get C_Country_ID
         *	@return C_Country_ID
         */
        public int GetC_Country_ID()
        {
            if (_loc == null)
                _loc = MLocation.Get(GetCtx(), GetC_Location_ID(), null);
            if (_loc == null)
                return 0;
            return _loc.GetC_Country_ID();
        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MBank[");
            sb.Append(Get_ID()).Append("-").Append(GetName()).Append("]");
            return sb.ToString();
        }

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true if valid
         */
        protected override bool BeforeSave(bool newRecord)
        {
            BankVerificationInterface verify = GetVerificationClass();
            if (verify != null)
            {
                String errorMsg = verify.VerifyRoutingNo(GetC_Country_ID(), GetRoutingNo());
                if (errorMsg != null)
                {
                    log.SaveError("Error", "@Invalid@ @RoutingNo@ " + errorMsg);
                    return false;
                }
                errorMsg = verify.VerifySwiftCode(GetSwiftCode());
                if (errorMsg != null)
                {
                    log.SaveError("Error", "@Invalid@ @SwiftCode@ " + errorMsg);
                    return false;
                }
            }
            return true;
        }
    }
}
