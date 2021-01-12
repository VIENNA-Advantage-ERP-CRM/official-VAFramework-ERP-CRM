/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MBPartnerLocation
 * Purpose        : Partner Location Model
 * Class Used     : X_C_BPartner_Location
 * Chronological    Development
 * Raghunandan     05-Jun-2009
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
    public class MBPartnerLocation : X_C_BPartner_Location
    {
        //	Cached Location			
        private MLocation _location = null;
        //	Unique Name				
        private String _uniqueName = null;
        private int _unique = 0;
        // Static Logger					
        private static VLogger _log = VLogger.GetVLogger(typeof(MBPartnerLocation).FullName);


        /* 	Get Locations for BPartner
        *	@param ctx context
        *	@param C_BPartner_ID bp
        *	@return array of locations
        */
        public MBPartnerLocation[] GetForBPartner(Ctx ctx, int C_BPartner_ID, Trx trxName)
        {
            List<MBPartnerLocation> list = new List<MBPartnerLocation>();
            String sql = "SELECT * FROM C_BPartner_Location WHERE C_BPartner_ID=" + C_BPartner_ID;
            DataSet ds = new DataSet();
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    //jz list.add(new MBPartnerLocation(ctx, dr, null));
                    list.Add(new MBPartnerLocation(ctx, dr, trxName));
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            MBPartnerLocation[] retValue = new MBPartnerLocation[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /****
         * 	Default Constructor
         *	@param ctx context
         *	@param C_BPartner_Location_ID id
         *	@param trxName transaction
         */
        public MBPartnerLocation(Ctx ctx, int C_BPartner_Location_ID, Trx trxName)
            : base(ctx, C_BPartner_Location_ID, trxName)
        {
            if (C_BPartner_Location_ID == 0)
            {
                SetName(".");
                //
                SetIsShipTo(true);
                SetIsRemitTo(true);
                SetIsPayFrom(true);
                SetIsBillTo(true);
            }
            // Added By Bharat - Name not updated on Location Tab.
            else
            {
                // change by Amit
                if (string.IsNullOrEmpty(GetName()))
                {
                    SetName(".");
                }
            }
        }

        /**
         * 	BP Parent Constructor
         * 	@param bp partner
         */
        public MBPartnerLocation(MBPartner bp)
            : this(bp.GetCtx(), 0, bp.Get_TrxName())
        {

            SetClientOrg(bp);
            //	may (still) be 0
            Set_ValueNoCheck("C_BPartner_ID", (int)(bp.GetC_BPartner_ID()));
        }

        /**
         * 	Constructor from ResultSet row
         *	@param ctx context
         * 	@param dr current row of result set to be loaded
         *	@param trxName transaction
         */
        public MBPartnerLocation(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
         * 	Get Loaction/Address
         * 	@param requery requery
         *	@return location
         */
        public MLocation GetLocation(bool requery)
        {
            if (_location == null || requery)
                _location = MLocation.Get(GetCtx(), GetC_Location_ID(), Get_TrxName());
            return _location;
        }

        /**
         *	String Representation
         * 	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MBPartner_Location[ID=")
                .Append(Get_ID())
                .Append(",C_Location_ID=").Append(GetC_Location_ID())
                .Append(",Name=").Append(GetName())
                .Append("]");
            return sb.ToString();
        }

        /****
         * 	Before Save.
         * 	- Set Name
         *	@param newRecord new
         *	@return save
         */
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetC_Location_ID() == 0)
                return false;

            // change by amit 
            //	Set New Name
            //if (!newRecord)
            //    return true;


            if (Util.GetValueOfString(Get_ValueOld("Name")) == GetName() && Util.GetValueOfInt(Get_ValueOld("C_Location_ID")) == GetC_Location_ID())
            {
                return true;
            }

            MLocation address = GetLocation(true);
            _uniqueName = GetName();
            //if (_uniqueName != null && _uniqueName.Equals("."))	//	default    change by amit
            _uniqueName = null;
            _unique = 0;
            // Changes Done By Vivek on 10/12/2015
            //Set City Name at Name Field
            if (GetName() == ".")
            {
                MakeUnique(address);
            }
            //else Set Manually Edited Name by User at name field
            else
            {
                SetName(GetName());
                return true;
            }

            //if (Util.GetValueOfString(Get_ValueOld("Name")) != GetName())
            //{
            //    _uniqueName = GetName();
            //    SetName(_uniqueName);
            //    return true;
            //}
            //MakeUnique(address);

            //	Check uniqueness
            MBPartnerLocation[] locations = GetForBPartner(GetCtx(), GetC_BPartner_ID(), Get_TrxName());
            bool unique = locations.Length == 0;
            while (!unique)
            {
                unique = true;
                for (int i = 0; i < locations.Length; i++)
                {
                    MBPartnerLocation location = locations[i];
                    if (location.GetC_BPartner_Location_ID() == Get_ID())
                        continue;
                    if (_uniqueName.Equals(location.GetName()))
                    {
                        MakeUnique(address);
                        unique = false;
                        break;
                    }
                }
            }
            SetName(_uniqueName);
            return true;
        }

        /**
         * 	Make name Unique
         * 	@param address address
         */
        private void MakeUnique(MLocation address)
        {
            //	_uniqueName = address.toString();
            //	return;

            if (_uniqueName == null)
                _uniqueName = "";
            _unique++;

            // 0 + Address1 
            // to set address1
            if (_uniqueName.Length == 0)
            {
                String xx = address.GetAddress1();
                if (xx != null && xx.Length > 0)
                {
                    _uniqueName = xx;
                }
                _unique = 0;
            }

            //	0 - City
            // to set address1 and City as Name
            if (_unique == 0 && _uniqueName.Length >= 0)
            {
                String xx = address.GetCity();
                if (xx != null && xx.Length > 0)
                {
                    if (_uniqueName.Length > 0)
                        _uniqueName += " ";
                    _uniqueName += xx;
                }
                _unique = 0;
            }

            //	1 + Address1
            if (_unique == 1 || _uniqueName.Length == 0)
            {
                String xx = address.GetAddress1();
                if (xx != null && xx.Length > 0)
                {
                    if (_uniqueName.Length > 0)
                        _uniqueName += " ";
                    _uniqueName += xx;
                }
                _unique = 1;
            }
            //	2 + Address2
            if (_unique == 2 || _uniqueName.Length == 0)
            {
                String xx = address.GetAddress2();
                if (xx != null && xx.Length > 0)
                {
                    if (_uniqueName.Length > 0)
                        _uniqueName += " ";
                    _uniqueName += xx;
                }
                _unique = 2;
            }
            //	3 - Region	
            if (_unique == 3 || _uniqueName.Length == 0)
            {
                String xx = address.GetRegionName(true);
                {
                    if (_uniqueName.Length > 0)
                        _uniqueName += " ";
                    _uniqueName += xx;
                }
                _unique = 3;
            }
            // 5 - Country
            if (_unique == 5 || _uniqueName.Length == 0)
            {
                String xx = address.GetCountryName();
                {
                    if (_uniqueName.Length > 0)
                        _uniqueName += " ";
                    _uniqueName += xx;
                }
                _unique = 5;
            }
            //	4 - ID	
            if (_unique == 4 || _uniqueName.Length == 0)
            {
                int id = Get_ID();
                if (id == 0)
                    id = address.Get_ID();
                _uniqueName += "#" + id;
                _unique = 4;
            }
        }

        /// <summary>
        /// Set Credit Status
        /// </summary>
        public void SetSOCreditStatus()
        {
            MBPartner bp = new MBPartner(GetCtx(), GetC_BPartner_ID(), Get_TrxName());
            Decimal creditLimit = GetSO_CreditLimit();
            //	Nothing to do
            if (SOCREDITSTATUS_NoCreditCheck.Equals(GetSOCreditStatus())
                || SOCREDITSTATUS_CreditStop.Equals(GetSOCreditStatus())
                || Env.ZERO.CompareTo(creditLimit) == 0)
                return;

            //	Above Credit Limit
            // changed this function for fetching open balance because in case of void it calculates again and fetches wrong value of open balance // Lokesh Chauhan
            //if (creditLimit.CompareTo(GetTotalOpenBalance(!_TotalOpenBalanceSet)) < 0)
            if (creditLimit.CompareTo(GetTotalOpenBalance()) <= 0)
                SetSOCreditStatus(SOCREDITSTATUS_CreditHold);
            else
            {
                //	Above Watch Limit
                //Peference check if credit watch per is zero on header then gets the value from bpgroup 
                Decimal watchAmt;
                if (bp.Get_ColumnIndex("CreditWatchPercent") > 0)
                {
                    if (bp.GetCreditWatchPercent() == 0)
                    {
                        watchAmt = Decimal.Multiply(creditLimit, bp.GetCreditWatchRatio());

                    }
                    else
                        watchAmt = Decimal.Multiply(creditLimit, bp.GetCustomerCreditWatch());
                }
                else
                {
                    watchAmt = Decimal.Multiply(creditLimit, bp.GetCreditWatchRatio());
                }
                if (watchAmt.CompareTo(GetTotalOpenBalance()) < 0)
                    SetSOCreditStatus(SOCREDITSTATUS_CreditWatch);
                else	//	is OK
                    SetSOCreditStatus(SOCREDITSTATUS_CreditOK);
            }
            log.Fine("SOCreditStatus=" + GetSOCreditStatus());
        }
    }
}

