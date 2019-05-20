/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRegistration
 * Purpose        : Asset Registration Model
 * Class Used     : X_A_Registration
 * Chronological    Development
 * Deepak           03-feb-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Data.SqlClient;

namespace VAdvantage.Model
{
    public class MRegistration : X_A_Registration
    {
       /// <summary>
       ///	Standard Constructor
       /// </summary>
       /// <param name="ctx">context</param>
       /// <param name="A_Registration_ID">id</param>
       /// <param name="trxName">trx</param>
        public MRegistration(Ctx ctx, int A_Registration_ID, Trx trxName):base(ctx, A_Registration_ID, trxName)
        {
            if (A_Registration_ID == 0)
            {
                SetIsRegistered(true);
            }
        }	

        /// <summary>
        /// New Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="Name">name</param>
        /// <param name="IsAllowPublish">allow publication</param>
        /// <param name="IsInProduction">production</param>
        /// <param name="AssetServiceDate">date</param>
        /// <param name="trxName">trx</param>
        public MRegistration(Ctx ctx, String Name, bool IsAllowPublish,
            bool IsInProduction,DateTime? AssetServiceDate, Trx trxName):this(ctx, 0, trxName)
        {
            
            SetName(Name);
            SetIsAllowPublish(IsAllowPublish);
            SetIsInProduction(IsInProduction);
            SetAssetServiceDate(AssetServiceDate);
        }	//	MRegistration


       /// <summary>
       /// 	Load Constructor
       /// </summary>
       /// <param name="ctx">context</param>
       /// <param name="rs">datarow</param>
       /// <param name="trxName">trx</param>
        public MRegistration(Ctx ctx,DataRow dr, Trx trxName):base(ctx, dr, trxName)
        {
        
        }
        public MRegistration(Ctx ctx,IDataReader idr, Trx trxName)
            : base(ctx,idr, trxName)
        {

        }	
        /**	All Attributes						*/
        private MRegistrationAttribute[] _allAttributes = null;

        /// <summary>
        /// Get All Attributes
        /// </summary>
        /// <returns>Registration Attributes</returns>
        public MRegistrationAttribute[] GetAttributes()
        {
            if (_allAttributes == null)
            {
                _allAttributes = MRegistrationAttribute.GetAll(GetCtx());
            }
            return _allAttributes;
        }	//	getAttributes

        /// <summary>
        ///	Get All active Self Service Attribute Values
        /// </summary>
        /// <returns>Registration Attribute Values</returns>
        public MRegistrationValue[] GetValues()
        {
            return GetValues(true);
        }	//	getValues

        /// <summary>
        /// Get All Attribute Values
        /// </summary>
        /// <param name="onlySelfService">only Active Self Service</param>
        /// <returns> sorted Registration Attribute Values</returns>
        public MRegistrationValue[] GetValues(bool onlySelfService)
        {
            CreateMissingValues();
            //
            String sql = "SELECT * FROM A_RegistrationValue rv "
                + "WHERE A_Registration_ID=@param";
            if (onlySelfService)
            {
                sql += " AND EXISTS (SELECT * FROM A_RegistrationAttribute ra WHERE rv.A_RegistrationAttribute_ID=ra.A_RegistrationAttribute_ID"
                    + " AND ra.IsActive='Y' AND ra.IsSelfService='Y')";
            }
            //	sql += " ORDER BY A_RegistrationAttribute_ID";

            List<MRegistrationValue> list = new List<MRegistrationValue>();
            SqlParameter[] param = new SqlParameter[1];
            IDataReader idr = null;
            try
            {
                //pstmt = DataBase.prepareStatement(sql, get_TrxName());
                //pstmt.setInt(1, getA_Registration_ID());
                param[0] = new SqlParameter("@param", GetA_Registration_ID());
                idr = DataBase.DB.ExecuteReader(sql, param, Get_TrxName());
                while (idr.Read())
                {
                    list.Add(new MRegistrationValue(GetCtx(), idr, Get_TrxName()));
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            
            //	Convert and Sort
            MRegistrationValue[] retValue = new MRegistrationValue[list.Count];
            retValue=list.ToArray();
            //Arrays.sort(retValue);
            Array.Sort(retValue);
            return retValue;
        }	//	getValues

        /// <summary>
        ///	Create Missing Attribute Values
        /// </summary>
        private void CreateMissingValues()
        {
            String sql = "SELECT ra.A_RegistrationAttribute_ID "
                + "FROM A_RegistrationAttribute ra"
                + " LEFT OUTER JOIN A_RegistrationProduct rp ON (rp.A_RegistrationAttribute_ID=ra.A_RegistrationAttribute_ID)"
                + " LEFT OUTER JOIN A_Registration r ON (r.M_Product_ID=rp.M_Product_ID) "
                + "WHERE r.A_Registration_ID=@param"
                //	Not in Registration
                + " AND NOT EXISTS (SELECT A_RegistrationAttribute_ID FROM A_RegistrationValue v "
                    + "WHERE ra.A_RegistrationAttribute_ID=v.A_RegistrationAttribute_ID AND r.A_Registration_ID=v.A_Registration_ID)";
            SqlParameter[] param = new SqlParameter[1];
            IDataReader idr = null;
            try
            {
                //pstmt = DataBase.prepareStatement(sql, get_TrxName());
                //pstmt.setInt(1, getA_Registration_ID());
                param[0] = new SqlParameter("@param", GetA_Registration_ID());
                idr = DataBase.DB.ExecuteReader(sql, param, Get_TrxName());
                while (idr.Read())
                {
                    MRegistrationValue v = new MRegistrationValue(this, Utility.Util.GetValueOfInt(idr[0]), "?");
                    v.Save();
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, null, e);
            }
           
        }

        /// <summary>
        /// Load Attributes from Request
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>number of attributes read</returns>
        //public int? LoadAttributeValues(HttpServletRequest request)
        //{
        //    //	save if not saved
        //    if (Get_ID() == 0)
        //    {
        //        Save();
        //    }
        //    int count = 0;
        //    //	read values for all attributes
        //    MRegistrationAttribute[] attributes = GetAttributes();
        //    for (int i = 0; i < attributes.Length; i++)
        //    {
        //        MRegistrationAttribute attribute = attributes[i];
        //        String value = WebUtil.getParameter(request, attribute.getName());
        //        if (value == null)
        //            continue;
        //        MRegistrationValue regValue = new MRegistrationValue(this,
        //            attribute.getA_RegistrationAttribute_ID(), value);
        //        if (regValue.save())
        //            count++;
        //    }
        //    log.fine("loadAttributeValues - #" + count + " (of " + attributes.length + ")");
        //    return count;
        //}	//	loadAttrubuteValues

        ///**
        // * 	Update Attributes from Request
        // *	@param request request
        // *	@return number of attributes read
        // */
        //public int updateAttributeValues(HttpServletRequest request)
        //{
        //    //	save if not saved
        //    if (get_ID() == 0)
        //        save();
        //    int count = 0;

        //    //	Get All Values
        //    MRegistrationValue[] regValues = getValues(false);
        //    for (int i = 0; i < regValues.length; i++)
        //    {
        //        MRegistrationValue regValue = regValues[i];
        //        String attributeName = regValue.getRegistrationAttribute();
        //        //	
        //        String dataValue = WebUtil.getParameter(request, attributeName);
        //        if (dataValue == null)
        //            continue;
        //        regValue.setDescription("Previous=" + regValue.getName());
        //        regValue.setName(dataValue);
        //        if (regValue.save())
        //            count++;
        //    }
        //    log.fine("updateAttributeValues - #" + count + " (of " + regValues.length + ")");
        //    return count;
        //}	//	updateAttrubuteValues

    }
}
