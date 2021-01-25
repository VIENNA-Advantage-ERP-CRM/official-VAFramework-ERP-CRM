using System;
using System.Net;
using System.Windows;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MGenAttributeInstance: X_VAB_GenFeatureInstance
    {
        public MGenAttributeInstance(Ctx ctx, int VAB_GenFeatureInstance_ID, Trx trxName)
            : base(ctx, VAB_GenFeatureInstance_ID, trxName)
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
        public MGenAttributeInstance(Ctx ctx, int VAB_GenFeature_ID,
            int VAB_GenFeatureSetInstance_ID, int VAB_GenFeatureValue_ID, String Value, Trx trxName)
            : base(ctx, 0, trxName)
        {
            SetVAB_GenFeature_ID(VAB_GenFeature_ID);
            SetVAB_GenFeatureSetInstance_ID(VAB_GenFeatureSetInstance_ID);
            SetVAB_GenFeatureValue_ID(VAB_GenFeatureValue_ID);
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
        public MGenAttributeInstance(Ctx ctx, int VAB_GenFeature_ID,
            int VAB_GenFeatureSetInstance_ID, Decimal? BDValue, Trx trxName)
            : base(ctx, 0, trxName)
        {
            SetVAB_GenFeature_ID(VAB_GenFeature_ID);
            SetVAB_GenFeatureSetInstance_ID(VAB_GenFeatureSetInstance_ID);
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
        public MGenAttributeInstance(Ctx ctx, int VAB_GenFeature_ID,
            int VAB_GenFeatureSetInstance_ID, String Value, Trx trxName)
            : base(ctx, 0, trxName)
        {
            SetVAB_GenFeature_ID(VAB_GenFeature_ID);
            SetVAB_GenFeatureSetInstance_ID(VAB_GenFeatureSetInstance_ID);
            SetValue(Value);
        }
    }
}
