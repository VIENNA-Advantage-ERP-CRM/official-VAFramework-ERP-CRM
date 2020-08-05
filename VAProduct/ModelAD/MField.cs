/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MField
 * Purpose        : to get fields from current/base table. 
 * Class Used     : MField inherits X_AD_Field class
 * Chronological    Development
 * Raghunandan      30-March-2009 & 07-May-2009
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
using VAdvantage.Utility;
using VAdvantage.ProcessEngine;
namespace VAdvantage.Model
{
   public class MField : X_AD_Field
    {
        //Column
        private MColumn _column = null;

        /// <summary>
        ///	Parent Constructor
        /// </summary>
        /// <param name="parent">parent parent</param>
        public MField(MTab parent)
            : base(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            //this(parent.GetCtx(), 0, parent.Get_TrxName());
            SetClientOrg(parent);
            SetAD_Tab_ID(parent.GetAD_Tab_ID());
        }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Field_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MField(Ctx ctx, int AD_Field_ID, Trx trxName)
            : base(ctx, AD_Field_ID, trxName)
        {
            if (AD_Field_ID == 0)
            {
                //	setAD_Tab_ID (0);	//	parent
                //	setAD_Column_ID (0);
                //	setName (null);
                SetEntityType(ENTITYTYPE_UserMaintained);	// U
                SetIsCentrallyMaintained(true);	// Y
                SetIsDisplayed(true);	// Y
                SetIsEncrypted(false);
                SetIsFieldOnly(false);
                SetIsHeading(false);
                SetIsReadOnly(false);
                SetIsSameLine(false);
                //	setObscureType(OBSCURETYPE_ObscureDigitsButLast4);
                //	setIsMandatory (null);
            }
        }

        /// <summary>
        ///	Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">Data row</param>
        /// <param name="trxName">transaction</param>
        public MField(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
            //super(ctx, rs, trxName);
        }

        /// <summary>
        ///	Copy Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="from">from copy from</param>
        public MField(MTab parent, MField from)
            : base(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            //this(parent.getCtx(), 0, parent.get_TrxName());
            // copyValues(from, this);
            SetClientOrg(parent);
            SetAD_Tab_ID(parent.GetAD_Tab_ID());
            SetEntityType(parent.GetEntityType());
        }

        /// <summary>
        /// Set Column Values
        /// </summary>
        /// <param name="column">column column</param>
        public void SetColumn(MColumn column)
        {
            _column = column;
            SetAD_Column_ID(column.GetAD_Column_ID());
            SetName(column.GetName());
            SetDescription(column.GetDescription());
            SetHelp(column.GetHelp());
            SetDisplayLength(column.GetFieldLength());
            SetEntityType(column.GetEntityType());
        }

        /// <summary>
        ///	Get Column
        /// </summary>
        /// <returns>column</returns>
        public MColumn GetColumn()
        {
            if (_column == null
                || _column.GetAD_Column_ID() != GetAD_Column_ID())
                _column = MColumn.Get(GetCtx(), GetAD_Column_ID());
            return _column;
        }

        /// <summary>
        ///	Set AD_Column_ID
        /// </summary>
        /// <param name="AD_Column_ID">column</param>
        public new void SetAD_Column_ID(int AD_Column_ID)
        {
            if (_column != null && _column.GetAD_Column_ID() != AD_Column_ID)
                _column = null;
            base.SetAD_Column_ID(AD_Column_ID);
        }

        /// <summary>
        ///	Mandatory UI
        /// </summary>
        /// <returns>true if mandatory</returns>
        protected bool IsMandatoryUI()
        {
            String m = GetIsMandatoryUI();
            if (m == null)
                m = GetColumn().IsMandatoryUI() ? "Y" : "N";
            return m.Equals("Y");
        }

        /// <summary>
        ///	Before Save
        /// </summary>
        /// <param name="newRecord">newRecord new</param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (DB.UseMigratedConnection)
            {
                return true;
            }

            //	Sync Terminology
            if ((newRecord || Is_ValueChanged("AD_Column_ID"))
                && IsCentrallyMaintained())
            {
                M_Element element = M_Element.GetOfColumn(GetCtx(), GetAD_Column_ID());
                SetName(element.GetName());
                SetDescription(element.GetDescription());
                SetHelp(element.GetHelp());
            }
            return true;
        }	

        /// <summary>
        ///	String Info
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MField[");
            sb.Append(Get_ID())
                .Append("-").Append(GetName())
                .Append("]");
            return sb.ToString();
        }

        
    }
}
