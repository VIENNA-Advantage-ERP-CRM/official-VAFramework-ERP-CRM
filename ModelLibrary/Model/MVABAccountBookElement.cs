/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAcctSchemaElement
 * Purpose        : used for VAB_AccountBook_Element table 
 * Class Used     : X_VAB_AccountBook_Element
 * Chronological    Development
 * Raghunandan     10-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;

using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVABAccountBookElement : X_VAB_AccountBook_Element
    {
        #region Private variable
        //Logger						
        private static VLogger	_log = VLogger.GetVLogger (typeof(MVABAccountBookElement).FullName);
        //	Cache						
        private static CCache<int, MVABAccountBookElement[]> s_cache = new CCache<int, MVABAccountBookElement[]>("VAB_AccountBook_Element", 10);
        // User Element Column Name		
        private String _ColumnName = null;
        #endregion

        /// <summary>
        /// Factory: Return ArrayList of Account Schema Elements
        /// </summary>
        /// <param name="as1">Accounting Schema</param>
        /// <returns>ArrayList with Elements</returns>
        public static MVABAccountBookElement[] GetAcctSchemaElements(MVABAccountBook as1)
        {
            int key = as1.GetVAB_AccountBook_ID();
            MVABAccountBookElement[] retValue = (MVABAccountBookElement[])s_cache[key];
            if (retValue != null)
                return retValue;

            _log.Fine("VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID());
            List<MVABAccountBookElement> list = new List<MVABAccountBookElement>();
            //
            String sql = "SELECT * FROM VAB_AccountBook_Element "
                + "WHERE VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID() + " AND IsActive='Y' ORDER BY SeqNo";

            try
            {
                DataSet ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, as1.Get_TrxName());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    MVABAccountBookElement ase = new MVABAccountBookElement(as1.GetCtx(), dr, as1.Get_TrxName());
                    _log.Fine(" - " + ase);
                    if (ase.IsMandatory() && ase.GetDefaultValue() == 0)
                    {
                        _log.Log(Level.SEVERE, "No default value for " + ase.GetName());
                    }
                    list.Add(ase);
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            retValue = new MVABAccountBookElement[list.Count];
            retValue = list.ToArray();
            s_cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Get Column Name of ELEMENTTYPE 
        /// </summary>
        /// <param name="elementType">ELEMENTTYPE </param>
        /// <returns>column name</returns>
        public static String GetColumnName(String elementType)
        {
            if (elementType.Equals(ELEMENTTYPE_Organization))
                return "VAF_Org_ID";
            else if (elementType.Equals(ELEMENTTYPE_Account))
                return "Account_ID";
            else if (elementType.Equals(ELEMENTTYPE_BPartner))
                return "VAB_BusinessPartner_ID";
            else if (elementType.Equals(ELEMENTTYPE_Product))
                return "M_Product_ID";
            else if (elementType.Equals(ELEMENTTYPE_Activity))
                return "VAB_BillingCode_ID";
            else if (elementType.Equals(ELEMENTTYPE_LocationFrom))
                return "C_LocFrom_ID";
            else if (elementType.Equals(ELEMENTTYPE_LocationTo))
                return "C_LocTo_ID";
            else if (elementType.Equals(ELEMENTTYPE_Campaign))
                return "VAB_Promotion_ID";
            else if (elementType.Equals(ELEMENTTYPE_OrgTrx))
                return "VAF_OrgTrx_ID";
            else if (elementType.Equals(ELEMENTTYPE_Project))
                return "VAB_Project_ID";
            else if (elementType.Equals(ELEMENTTYPE_SalesRegion))
                return "VAB_SalesRegionState_ID";
            else if (elementType.Equals(ELEMENTTYPE_UserList1))
                return "User1_ID";
            else if (elementType.Equals(ELEMENTTYPE_UserList2))
                return "User2_ID";
            else if (elementType.Equals(ELEMENTTYPE_UserElement1))
                return "UserElement1_ID";
            else if (elementType.Equals(ELEMENTTYPE_UserElement2))
                return "UserElement2_ID";
            else if (elementType.Equals(ELEMENTTYPE_UserElement3))
                return "UserElement3_ID";
            else if (elementType.Equals(ELEMENTTYPE_UserElement4))
                return "UserElement4_ID";
            else if (elementType.Equals(ELEMENTTYPE_UserElement5))
                return "UserElement5_ID";
            else if (elementType.Equals(ELEMENTTYPE_UserElement6))
                return "UserElement6_ID";
            else if (elementType.Equals(ELEMENTTYPE_UserElement7))
                return "UserElement7_ID";
            else if (elementType.Equals(ELEMENTTYPE_UserElement8))
                return "UserElement8_ID";
            else if (elementType.Equals(ELEMENTTYPE_UserElement9))
                return "UserElement9_ID";
            //
            return "";
        }

        /// <summary>
        /// Get Value Query for ELEMENTTYPE Type
        /// </summary>
        /// <param name="elementType">ELEMENTTYPE type</param>
        /// <returns>query "SELECT Value,Name FROM Table WHERE ID="</returns>
        public static String GetValueQuery(String elementType)
        {
            if (elementType.Equals(ELEMENTTYPE_Organization))
                return "SELECT Value,Name FROM VAF_Org WHERE VAF_Org_ID=";
            else if (elementType.Equals(ELEMENTTYPE_Account))
                return "SELECT Value,Name FROM VAB_Acct_Element WHERE VAB_Acct_Element_ID=";
            else if (elementType.Equals(ELEMENTTYPE_SubAccount))
                return "SELECT Value,Name FROM C_SubAccount WHERE C_SubAccount_ID=";
            else if (elementType.Equals(ELEMENTTYPE_BPartner))
                return "SELECT Value,Name FROM VAB_BusinessPartner WHERE VAB_BusinessPartner_ID=";
            else if (elementType.Equals(ELEMENTTYPE_Product))
                return "SELECT Value,Name FROM M_Product WHERE M_Product_ID=";
            else if (elementType.Equals(ELEMENTTYPE_Activity))
                return "SELECT Value,Name FROM VAB_BillingCode WHERE VAB_BillingCode_ID=";
            else if (elementType.Equals(ELEMENTTYPE_LocationFrom))
                return "SELECT City,Address1 FROM VAB_Address WHERE VAB_Address_ID=";
            else if (elementType.Equals(ELEMENTTYPE_LocationTo))
                return "SELECT City,Address1 FROM VAB_Address WHERE VAB_Address_ID=";
            else if (elementType.Equals(ELEMENTTYPE_Campaign))
                return "SELECT Value,Name FROM VAB_Promotion WHERE VAB_Promotion_ID=";
            else if (elementType.Equals(ELEMENTTYPE_OrgTrx))
                return "SELECT Value,Name FROM VAF_Org WHERE VAF_Org_ID=";
            else if (elementType.Equals(ELEMENTTYPE_Project))
                return "SELECT Value,Name FROM VAB_Project WHERE VAB_Project_ID=";
            else if (elementType.Equals(ELEMENTTYPE_SalesRegion))
                return "SELECT Value,Name FROM VAB_SalesRegionState WHERE VAB_SalesRegionState_ID";
            else if (elementType.Equals(ELEMENTTYPE_UserList1))
                return "SELECT Value,Name FROM VAB_Acct_Element WHERE VAB_Acct_Element_ID=";
            else if (elementType.Equals(ELEMENTTYPE_UserList2))
                return "SELECT Value,Name FROM VAB_Acct_Element WHERE VAB_Acct_Element_ID=";
            //User Element
            else if (elementType.Equals(ELEMENTTYPE_UserElement1) || elementType.Equals(ELEMENTTYPE_UserElement2) || elementType.Equals(ELEMENTTYPE_UserElement3)
                || elementType.Equals(ELEMENTTYPE_UserElement4) || elementType.Equals(ELEMENTTYPE_UserElement5)
                || elementType.Equals(ELEMENTTYPE_UserElement6) || elementType.Equals(ELEMENTTYPE_UserElement7)
                || elementType.Equals(ELEMENTTYPE_UserElement8) || elementType.Equals(ELEMENTTYPE_UserElement9))
                return null;
            //
            return "";
        }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAB_AccountBook_Element_ID"></param>
        /// <param name="trxName"></param>
        public MVABAccountBookElement(Ctx ctx, int VAB_AccountBook_Element_ID, Trx trxName)
            : base(ctx, VAB_AccountBook_Element_ID, trxName)
        {
            if (VAB_AccountBook_Element_ID == 0)
            {
                //	setVAB_AccountBook_Element_ID (0);
                //	setVAB_AccountBook_ID (0);
                //	setVAB_Element_ID (0);
                //	setElementType (null);
                SetIsBalanced(false);
                SetIsMandatory(false);
                //	setName (null);
                //	setOrg_ID (0);
                //	setSeqNo (0);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rs"></param>
        /// <param name="trxName"></param>
        public MVABAccountBookElement(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="as1">accounting schema</param>
        public MVABAccountBookElement(MVABAccountBook as1)
            : this(as1.GetCtx(), 0, as1.Get_TrxName())
        {
            SetClientOrg(as1);
            SetVAB_AccountBook_ID(as1.GetVAB_AccountBook_ID());

            //	setVAB_Element_ID (0);
            //	setElementType (null);
            //	setName (null);
            //	setSeqNo (0);

        }

        /// <summary>
        /// Set Organization Type
        /// </summary>
        /// <param name="SeqNo">sequence</param>
        /// <param name="Name">name </param>
        /// <param name="Org_ID">id</param>
        public void SetTypeOrg(int SeqNo, String Name, int Org_ID)
        {
            SetElementType(ELEMENTTYPE_Organization);
            SetSeqNo(SeqNo);
            SetName(Name);
            SetOrg_ID(Org_ID);
        }

        /// <summary>
        /// Set Type Account
        /// </summary>
        /// <param name="SeqNo">squence</param>
        /// <param name="Name">name</param>
        /// <param name="VAB_Element_ID">element</param>
        /// <param name="VAB_Acct_Element_ID">element value</param>
        public void SetTypeAccount(int SeqNo, String Name, int VAB_Element_ID, int VAB_Acct_Element_ID)
        {
            SetElementType(ELEMENTTYPE_Account);
            SetSeqNo(SeqNo);
            SetName(Name);
            SetVAB_Element_ID(VAB_Element_ID);
            SetVAB_Acct_Element_ID(VAB_Acct_Element_ID);
        }

        /// <summary>
        /// Set Type BPartner
        /// </summary>
        /// <param name="SeqNo">sequence</param>
        /// <param name="Name">SeqNo</param>
        /// <param name="VAB_BusinessPartner_ID">id</param>
        public void SetTypeBPartner(int SeqNo, String Name, int VAB_BusinessPartner_ID)
        {
            SetElementType(ELEMENTTYPE_BPartner);
            SetSeqNo(SeqNo);
            SetName(Name);
            SetVAB_BusinessPartner_ID(VAB_BusinessPartner_ID);
        }

        /// <summary>
        /// Set Type Product
        /// </summary>
        /// <param name="SeqNo">sequence</param>
        /// <param name="Name">name</param>
        /// <param name="M_Product_ID">id</param>
        public void SetTypeProduct(int SeqNo, String Name, int M_Product_ID)
        {
            SetElementType(ELEMENTTYPE_Product);
            SetSeqNo(SeqNo);
            SetName(Name);
            SetM_Product_ID(M_Product_ID);
        }

        /// <summary>
        /// Set Type Project
        /// </summary>
        /// <param name="SeqNo">sequence</param>
        /// <param name="Name">name</param>
        /// <param name="VAB_Project_ID">id</param>
        public void SetTypeProject(int SeqNo, String Name, int VAB_Project_ID)
        {
            SetElementType(ELEMENTTYPE_Project);
            SetSeqNo(SeqNo);
            SetName(Name);
            SetVAB_Project_ID(VAB_Project_ID);
        }

        /// <summary>
        /// Is Element Type
        /// </summary>
        /// <param name="elementType">type</param>
        /// <returns>ELEMENTTYPE type</returns>
        public bool IsElementType(String elementType)
        {
            if (elementType == null)
                return false;
            return elementType.Equals(GetElementType());
        }

        /// <summary>
        /// Get Default element value
        /// </summary>
        /// <returns>default</returns>
        public int GetDefaultValue()
        {
            String elementType = GetElementType();
            int defaultValue = 0;
            if (elementType.Equals(ELEMENTTYPE_Organization))
                defaultValue = GetOrg_ID();
            else if (elementType.Equals(ELEMENTTYPE_Account))
                defaultValue = GetVAB_Acct_Element_ID();
            else if (elementType.Equals(ELEMENTTYPE_BPartner))
                defaultValue = GetVAB_BusinessPartner_ID();
            else if (elementType.Equals(ELEMENTTYPE_Product))
                defaultValue = GetM_Product_ID();
            else if (elementType.Equals(ELEMENTTYPE_Activity))
                defaultValue = GetVAB_BillingCode_ID();
            else if (elementType.Equals(ELEMENTTYPE_LocationFrom))
                defaultValue = GetVAB_Address_ID();
            else if (elementType.Equals(ELEMENTTYPE_LocationTo))
                defaultValue = GetVAB_Address_ID();
            else if (elementType.Equals(ELEMENTTYPE_Campaign))
                defaultValue = GetVAB_Promotion_ID();
            else if (elementType.Equals(ELEMENTTYPE_OrgTrx))
                defaultValue = GetOrg_ID();
            else if (elementType.Equals(ELEMENTTYPE_Project))
                defaultValue = GetVAB_Project_ID();
            else if (elementType.Equals(ELEMENTTYPE_SalesRegion))
                defaultValue = GetVAB_SalesRegionState_ID();
            else if (elementType.Equals(ELEMENTTYPE_UserList1))
                defaultValue = GetVAB_Acct_Element_ID();
            else if (elementType.Equals(ELEMENTTYPE_UserList2))
                defaultValue = GetVAB_Acct_Element_ID();
            else if (elementType.Equals(ELEMENTTYPE_UserElement1))
                defaultValue = 0;
            else if (elementType.Equals(ELEMENTTYPE_UserElement2))
                defaultValue = 0;
            // enhanced By Amit 23-3-2016
            else if (elementType.Equals(ELEMENTTYPE_UserElement3))
                defaultValue = 0;
            else if (elementType.Equals(ELEMENTTYPE_UserElement4))
                defaultValue = 0;
            else if (elementType.Equals(ELEMENTTYPE_UserElement5))
                defaultValue = 0;
            else if (elementType.Equals(ELEMENTTYPE_UserElement6))
                defaultValue = 0;
            else if (elementType.Equals(ELEMENTTYPE_UserElement7))
                defaultValue = 0;
            else if (elementType.Equals(ELEMENTTYPE_UserElement8))
                defaultValue = 0;
            else if (elementType.Equals(ELEMENTTYPE_UserElement9))
                defaultValue = 0;
            //end
            return defaultValue;
        }

        /// <summary>
        /// Get Acct Fact ColumnName
        /// </summary>
        /// <returns>column name</returns>
        public String GetColumnName()
        {
            String et = GetElementType();
            return GetColumnName(et);
        }

        /// <summary>
        /// Get Display ColumnName
        /// </summary>
        /// <returns>column name</returns>
        public String GetDisplayColumnName()
        {
            String et = GetElementType();
            // Changed By Amit 23-3-2016 
            //if (ELEMENTTYPE_UserElement1.Equals(et) || ELEMENTTYPE_UserElement2.Equals(et))
            if (ELEMENTTYPE_UserElement1.Equals(et) || ELEMENTTYPE_UserElement2.Equals(et) ||
                ELEMENTTYPE_UserElement3.Equals(et) || ELEMENTTYPE_UserElement4.Equals(et) ||
                ELEMENTTYPE_UserElement5.Equals(et) || ELEMENTTYPE_UserElement6.Equals(et) ||
                ELEMENTTYPE_UserElement7.Equals(et) || ELEMENTTYPE_UserElement8.Equals(et) ||
                ELEMENTTYPE_UserElement9.Equals(et))
            {
                if (_ColumnName == null)
                    _ColumnName = MVAFColumn.GetColumnName(GetCtx(), GetVAF_Column_ID());
                return _ColumnName;
            }
            return GetColumnName(et);
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            return "AcctSchemaElement[" + Get_ID()
                + "-" + GetName()
                + "(" + GetElementType() + ")=" + GetDefaultValue()
                + ",Pos=" + GetSeqNo() + "]";
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true if it can be saved</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetVAF_Org_ID() != 0)
                SetVAF_Org_ID(0);
            String et = GetElementType();
            if (IsMandatory() &&
                (ELEMENTTYPE_UserList1.Equals(et) || ELEMENTTYPE_UserList2.Equals(et)
                || ELEMENTTYPE_UserElement1.Equals(et) || ELEMENTTYPE_UserElement2.Equals(et)
                || ELEMENTTYPE_UserElement3.Equals(et) || ELEMENTTYPE_UserElement4.Equals(et)
                || ELEMENTTYPE_UserElement5.Equals(et) || ELEMENTTYPE_UserElement6.Equals(et)
                || ELEMENTTYPE_UserElement7.Equals(et) || ELEMENTTYPE_UserElement8.Equals(et)
                || ELEMENTTYPE_UserElement9.Equals(et)))
                SetIsMandatory(false);
            else if (IsMandatory())
            {
                String errorField = null;
                if (ELEMENTTYPE_Account.Equals(et) && GetVAB_Acct_Element_ID() == 0)
                    errorField = "VAB_Acct_Element_ID";
                else if (ELEMENTTYPE_Activity.Equals(et) && GetVAB_BillingCode_ID() == 0)
                    errorField = "VAB_BillingCode_ID";
                else if (ELEMENTTYPE_BPartner.Equals(et) && GetVAB_BusinessPartner_ID() == 0)
                    errorField = "VAB_BusinessPartner_ID";
                else if (ELEMENTTYPE_Campaign.Equals(et) && GetVAB_Promotion_ID() == 0)
                    errorField = "VAB_Promotion_ID";
                else if (ELEMENTTYPE_LocationFrom.Equals(et) && GetVAB_Address_ID() == 0)
                    errorField = "VAB_Address_ID";
                else if (ELEMENTTYPE_LocationTo.Equals(et) && GetVAB_Address_ID() == 0)
                    errorField = "VAB_Address_ID";
                else if (ELEMENTTYPE_Organization.Equals(et) && GetOrg_ID() == 0)
                    errorField = "Org_ID";
                else if (ELEMENTTYPE_OrgTrx.Equals(et) && GetOrg_ID() == 0)
                    errorField = "Org_ID";
                else if (ELEMENTTYPE_Product.Equals(et) && GetM_Product_ID() == 0)
                    errorField = "M_Product_ID";
                else if (ELEMENTTYPE_Project.Equals(et) && GetVAB_Project_ID() == 0)
                    errorField = "VAB_Project_ID";
                else if (ELEMENTTYPE_SalesRegion.Equals(et) && GetVAB_SalesRegionState_ID() == 0)
                    errorField = "VAB_SalesRegionState_ID";
                if (errorField != null)
                {
                    log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@IsMandatory@: @" + errorField + "@"));
                    return false;
                }
            }
            //
            if (GetVAF_Column_ID() == 0
                && (ELEMENTTYPE_UserElement1.Equals(et) || ELEMENTTYPE_UserElement2.Equals(et)))
            {
                log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@IsMandatory@: @VAF_Column_ID@"));
                return false;
            }
            return true;
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            //	Default Value
            if (IsMandatory() && Is_ValueChanged("IsMandatory"))
            {
                if (ELEMENTTYPE_Activity.Equals(GetElementType()))
                    UpdateData("VAB_BillingCode_ID", GetVAB_BillingCode_ID());
                else if (ELEMENTTYPE_BPartner.Equals(GetElementType()))
                    UpdateData("VAB_BusinessPartner_ID", GetVAB_BusinessPartner_ID());
                else if (ELEMENTTYPE_Product.Equals(GetElementType()))
                    UpdateData("M_Product_ID", GetM_Product_ID());
                else if (ELEMENTTYPE_Project.Equals(GetElementType()))
                    UpdateData("VAB_Project_ID", GetVAB_Project_ID());
            }
            //	Resequence
            if (newRecord || Is_ValueChanged("SeqNo"))
                MAccount.UpdateValueDescription(GetCtx(), "VAF_Client_ID=" + GetVAF_Client_ID(), Get_TrxName());
            //	Clear Cache
            s_cache.Clear();
            return success;
        }

        /// <summary>
        /// Update ValidCombination and Fact with mandatory value
        /// </summary>
        /// <param name="element">element</param>
        /// <param name="id">new default</param>
        private void UpdateData(String element, int id)
        {
            MAccount.UpdateValueDescription(GetCtx(), element + "=" + id, Get_TrxName());
            //
            String sql = "UPDATE VAB_Acct_ValidParameter SET " + element + "=" + id
                + " WHERE " + element + " IS NULL AND VAF_Client_ID=" + GetVAF_Client_ID();
            int noC = Convert.ToInt32(DataBase.DB.ExecuteQuery(sql, null, Get_TrxName()));
            //
            sql = "UPDATE Actual_Acct_Detail SET " + element + "=" + id
                + " WHERE " + element + " IS NULL AND VAB_AccountBook_ID=" + GetVAB_AccountBook_ID();
            int noF = Convert.ToInt32(DataBase.DB.ExecuteQuery(sql, null, Get_TrxName()));
            //
            log.Fine("ValidCombination=" + noC + ", Fact=" + noF);
        }

        /// <summary>
        /// After Delete
        /// </summary>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterDelete(bool success)
        {
            //	Update Account Info
            MAccount.UpdateValueDescription(GetCtx(), "VAF_Client_ID=" + GetVAF_Client_ID(), Get_TrxName());
            //
            s_cache.Clear();
            return success;
        }
    }
}
