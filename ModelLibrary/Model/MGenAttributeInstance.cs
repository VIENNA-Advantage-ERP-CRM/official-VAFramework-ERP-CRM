using System;
using System.Net;
using System.Windows;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MGenAttributeInstance: X_C_GenAttributeInstance
    {
        public MGenAttributeInstance(Ctx ctx, int C_GenAttributeInstance_ID, Trx trxName)
            : base(ctx, C_GenAttributeInstance_ID, trxName)
        {


        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MGenAttributeInstance(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }
          /// <summary>
        /// Selection Value Constructior
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Attribute_ID">attribute</param>
        /// <param name="M_AttributeSetInstance_ID">instance</param>
        /// <param name="M_AttributeValue_ID">selection</param>
        /// <param name="Value">String representation for fast display</param>
        /// <param name="trxName">transaction</param>
        public MGenAttributeInstance(Ctx ctx, int C_GenAttribute_ID,
            int C_GenAttributeSetInstance_ID, int C_GenAttributeValue_ID, String Value, Trx trxName)
            : base(ctx, 0, trxName)
        {
            SetC_GenAttribute_ID(C_GenAttribute_ID);
            SetC_GenAttributeSetInstance_ID(C_GenAttributeSetInstance_ID);
            SetC_GenAttributeValue_ID(C_GenAttributeValue_ID);
            SetValue(Value);
        }
         /// <summary>
        /// Number Value Constructior
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Attribute_ID">attribute</param>
        /// <param name="M_AttributeSetInstance_ID">instance</param>
        /// <param name="BDValue"> number value</param>
        /// <param name="trxName">transaction</param>
        public MGenAttributeInstance(Ctx ctx, int C_GenAttribute_ID,
            int C_GenAttributeSetInstance_ID, Decimal? BDValue, Trx trxName)
            : base(ctx, 0, trxName)
        {
            SetC_GenAttribute_ID(C_GenAttribute_ID);
            SetC_GenAttributeSetInstance_ID(C_GenAttributeSetInstance_ID);
            SetValueNumber(BDValue);
        }

         /// <summary>
        /// String Value Constructior
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Attribute_ID">attribute</param>
        /// <param name="M_AttributeSetInstance_ID">instance</param>
        /// <param name="Value">string value</param>
        /// <param name="trxName">transaction</param>
        public MGenAttributeInstance(Ctx ctx, int C_GenAttribute_ID,
            int C_GenAttributeSetInstance_ID, String Value, Trx trxName)
            : base(ctx, 0, trxName)
        {
            SetC_GenAttribute_ID(C_GenAttribute_ID);
            SetC_GenAttributeSetInstance_ID(C_GenAttributeSetInstance_ID);
            SetValue(Value);
        }
    }
}
