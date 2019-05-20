/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAttributeValue
 * Purpose        : Set the attribute vlue from Table
 * Class Used     : X_M_AttributeValue
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
    public class MAttributeValue : X_M_AttributeValue
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_AttributeValue_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MAttributeValue(Ctx ctx, int M_AttributeValue_ID, Trx trxName)
            : base(ctx, M_AttributeValue_ID, trxName)
        {
            /** if (M_AttributeValue_ID == 0)
            {
            setM_AttributeValue_ID (0);
            setM_Attribute_ID (0);
            setName (null);
            setValue (null);
            }
            **/
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MAttributeValue(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override  String ToString()
        {
            return GetName();
        }
    }
}
