/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAMPFeatueInstance
 * Purpose        : attribute instance stting using x-classes
 * Class Used     : X_VAM_PFeatue_Instance
 * Chronological    Development
 * Raghunandan     04-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
//////using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;

namespace VAdvantage.Model
{
    public class MVAMPFeatueInstance : X_VAM_PFeatue_Instance
    {
        /// <summary>
        /// Persistency Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ignored">ignored</param>
        /// <param name="trxName">transaction</param>
        public MVAMPFeatueInstance(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, 0, trxName)
        {
            if (ignored != 0)
                throw new ArgumentException("Multi-Key");
        }

        /// <summary>
        /// Load Cosntructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MVAMPFeatueInstance(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// String Value Constructior
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_ProductFeature_ID">attribute</param>
        /// <param name="VAM_PFeature_SetInstance_ID">instance</param>
        /// <param name="Value">string value</param>
        /// <param name="trxName">transaction</param>
        public MVAMPFeatueInstance(Ctx ctx, int VAM_ProductFeature_ID,
            int VAM_PFeature_SetInstance_ID, String Value, Trx trxName)
            : base(ctx, 0, trxName)
        {
            SetVAM_ProductFeature_ID(VAM_ProductFeature_ID);
            SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
            SetValue(Value);
        }

        /// <summary>
        /// Number Value Constructior
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_ProductFeature_ID">attribute</param>
        /// <param name="VAM_PFeature_SetInstance_ID">instance</param>
        /// <param name="BDValue"> number value</param>
        /// <param name="trxName">transaction</param>
        public MVAMPFeatueInstance(Ctx ctx, int VAM_ProductFeature_ID,
            int VAM_PFeature_SetInstance_ID, Decimal? BDValue, Trx trxName)
            : base(ctx, 0, trxName)
        {
            SetVAM_ProductFeature_ID(VAM_ProductFeature_ID);
            SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
            SetValueNumber(BDValue);
        }

        /// <summary>
        /// Selection Value Constructior
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_ProductFeature_ID">attribute</param>
        /// <param name="VAM_PFeature_SetInstance_ID">instance</param>
        /// <param name="VAM_PFeature_Value_ID">selection</param>
        /// <param name="Value">String representation for fast display</param>
        /// <param name="trxName">transaction</param>
        public MVAMPFeatueInstance(Ctx ctx, int VAM_ProductFeature_ID,
            int VAM_PFeature_SetInstance_ID, int VAM_PFeature_Value_ID, String Value, Trx trxName)
            : base(ctx, 0, trxName)
        {
            SetVAM_ProductFeature_ID(VAM_ProductFeature_ID);
            SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
            SetVAM_PFeature_Value_ID(VAM_PFeature_Value_ID);
            SetValue(Value);
        }

        /// <summary>
        /// 	Set ValueNumber
        /// </summary>
        /// <param name="ValueNumber">number</param>
        public new void SetValueNumber(Decimal? ValueNumber)
        {
            base.SetValueNumber(ValueNumber);
            if (ValueNumber == null)
            {
                SetValue(null);
                return;
            }
            if (ValueNumber == 0)
            {
                SetValue("0");
                return;
            }
            //	Display number w/o decimal 0
            char[] chars = ValueNumber.ToString().ToCharArray();
            StringBuilder display = new StringBuilder();
            bool add = false;
            for (int i = chars.Length - 1; i >= 0; i--)
            {
                char c = chars[i];
                if (add)
                    display.Insert(0, c.ToString());
                else
                {
                    if (c == '0')
                        continue;
                    else if (c == '.')	//	decimal point
                        add = true;
                    else
                    {
                        display.Insert(0, c.ToString());
                        add = true;
                    }
                }
            }
            SetValue(display.ToString());
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            return GetValue();
        }
    }
}
