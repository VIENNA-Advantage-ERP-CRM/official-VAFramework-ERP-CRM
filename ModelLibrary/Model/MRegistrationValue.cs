/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRegistrationValue
 * Purpose        : Asset Registration Attribute Value 
 * Class Used     : X_A_RegistrationValue
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
    public class MRegistrationValue : X_A_RegistrationValue
    {
        /// <summary>
        /// Persistency Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ignored">ignored</param>
        /// <param name="trxName">trx</param>
        public MRegistrationValue(Ctx ctx, int ignored, Trx trxName):base(ctx, 0, trxName)
        {
            if (ignored != 0)
            { 
                throw new ArgumentException("Multi-Key");
            }
        }	//	MRegistrationValue

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">trx</param>
        public MRegistrationValue(Ctx ctx, DataRow dr, Trx trxName): base(ctx, dr, trxName)
        {
           
        }
        public MRegistrationValue(Ctx ctx,IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }
        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="registration">parent</param>
        /// <param name="A_RegistrationAttribute_ID">attribute</param>
        /// <param name="Name">value</param>
        public MRegistrationValue(MRegistration registration,
            int A_RegistrationAttribute_ID, String Name):base(registration.GetCtx(), 0, registration.Get_TrxName())
        {
           
            SetClientOrg(registration);
            SetA_Registration_ID(registration.GetA_Registration_ID());
            //
            SetA_RegistrationAttribute_ID(A_RegistrationAttribute_ID);
            SetName(Name);
        }	//	MRegistrationValue

        /**	Cached Attribute Name				*/
        private String _registrationAttribute = null;
        /**	Cached Attribute Description		*/
        private String _registrationAttributeDescription = null;
        /**	Cached Attribute Sequence	*/
        private int _seqNo = -1;

        /// <summary>
        /// Get Registration Attribute
        /// </summary>
        /// <returns>name of registration attribute</returns>
        public String GetRegistrationAttribute()
        {
            if (_registrationAttribute == null)
            {
                int A_RegistrationAttribute_ID = GetA_RegistrationAttribute_ID();
                MRegistrationAttribute att = MRegistrationAttribute.Get(GetCtx(), A_RegistrationAttribute_ID, Get_TrxName());
                _registrationAttribute = att.GetName();
                _registrationAttributeDescription = att.GetDescription();
                _seqNo = att.GetSeqNo();
            }
            return _registrationAttribute;
        }	//	getRegistrationAttribute

        /// <summary>
        /// Get Registration Attribute Description
        /// </summary>
        /// <returns> Description of registration attribute </returns>
        public String GetRegistrationAttributeDescription()
        {
            if (_registrationAttributeDescription == null)
            {
                GetRegistrationAttribute();
            }
            return _registrationAttributeDescription;
        }	//	getRegistrationAttributeDescription

        /// <summary>
        /// Get Attribute SeqNo
        /// </summary>
        /// <returns>seq no</returns>
        public int GetSeqNo()
        {
            if (_seqNo == -1)
            {
                GetRegistrationAttribute();
            }
            return _seqNo;
        }	//	getSeqNo

       /// <summary>
       ///  Compare To
       /// </summary>
       /// <param name="o">the Object to be compared.</param>
       /// <returns>a negative integer, zero, or a positive integer as this object
         		//is less than, equal to, or greater than the specified object.</returns>
        public int CompareTo(Object o)
        {
            if (o == null)
            {
                return 0;
            }
            MRegistrationValue oo = (MRegistrationValue)o;
            int compare = GetSeqNo() - oo.GetSeqNo();
            return compare;
        }	//	compareTo

       /// <summary>
       /// 	String Representation
       /// </summary>
       /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetSeqNo()).Append(": ")
                .Append(GetRegistrationAttribute()).Append("=").Append(GetName());
            return sb.ToString();
        }	//	toString

    }	

}
