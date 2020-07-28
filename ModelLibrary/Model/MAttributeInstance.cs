/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAttributeInstance
 * Purpose        : attribute instance stting using x-classes
 * Class Used     : X_M_AttributeInstance
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
using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;

namespace VAdvantage.Model
{
    public class MAttributeInstance : X_M_AttributeInstance
    {
        /// <summary>
        /// Persistency Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ignored">ignored</param>
        /// <param name="trxName">transaction</param>
        public MAttributeInstance(Ctx ctx, int ignored, Trx trxName)
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
        public MAttributeInstance(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// String Value Constructior
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Attribute_ID">attribute</param>
        /// <param name="M_AttributeSetInstance_ID">instance</param>
        /// <param name="Value">string value</param>
        /// <param name="trxName">transaction</param>
        public MAttributeInstance(Ctx ctx, int M_Attribute_ID,
            int M_AttributeSetInstance_ID, String Value, Trx trxName)
            : base(ctx, 0, trxName)
        {
            SetM_Attribute_ID(M_Attribute_ID);
            SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
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
        public MAttributeInstance(Ctx ctx, int M_Attribute_ID,
            int M_AttributeSetInstance_ID, Decimal? BDValue, Trx trxName)
            : base(ctx, 0, trxName)
        {
            SetM_Attribute_ID(M_Attribute_ID);
            SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            SetValueNumber(BDValue);
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
        public MAttributeInstance(Ctx ctx, int M_Attribute_ID,
            int M_AttributeSetInstance_ID, int M_AttributeValue_ID, String Value, Trx trxName)
            : base(ctx, 0, trxName)
        {
            SetM_Attribute_ID(M_Attribute_ID);
            SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            SetM_AttributeValue_ID(M_AttributeValue_ID);
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
