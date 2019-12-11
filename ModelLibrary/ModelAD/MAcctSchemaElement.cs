/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAcctSchemaElement
 * Purpose        : used for C_AcctSchema_Element table 
 * Class Used     : X_C_AcctSchema_Element
 * Chronological    Development
 * Raghunandan     10-Jun-2009
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
    public class MAcctSchemaElement : X_C_AcctSchema_Element
    {
        #region Private variable
        //Logger						
        private static VLogger	_log = VLogger.GetVLogger (typeof(MAcctSchemaElement).FullName);
        //	Cache						
        private static CCache<int, MAcctSchemaElement[]> s_cache = new CCache<int, MAcctSchemaElement[]>("C_AcctSchema_Element", 10);
        // User Element Column Name		
        private String _ColumnName = null;
        #endregion

        /// <summary>
        /// Factory: Return ArrayList of Account Schema Elements
        /// </summary>
        /// <param name="as1">Accounting Schema</param>
        /// <returns>ArrayList with Elements</returns>
        public static MAcctSchemaElement[] GetAcctSchemaElements(MAcctSchema as1)
        {
            int key = as1.GetC_AcctSchema_ID();
            MAcctSchemaElement[] retValue = (MAcctSchemaElement[])s_cache[key];
            if (retValue != null)
                return retValue;

            _log.Fine("C_AcctSchema_ID=" + as1.GetC_AcctSchema_ID());
            List<MAcctSchemaElement> list = new List<MAcctSchemaElement>();
            //
            String sql = "SELECT * FROM C_AcctSchema_Element "
                + "WHERE C_AcctSchema_ID=" + as1.GetC_AcctSchema_ID() + " AND IsActive='Y' ORDER BY SeqNo";

            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, as1.Get_TrxName());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    MAcctSchemaElement ase = new MAcctSchemaElement(as1.GetCtx(), dr, as1.Get_TrxName());
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

            retValue = new MAcctSchemaElement[list.Count];
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
                return "AD_Org_ID";
            else if (elementType.Equals(ELEMENTTYPE_Account))
                return "Account_ID";
            else if (elementType.Equals(ELEMENTTYPE_BPartner))
                return "C_BPartner_ID";
            else if (elementType.Equals(ELEMENTTYPE_Product))
                return "M_Product_ID";
            else if (elementType.Equals(ELEMENTTYPE_Activity))
                return "C_Activity_ID";
            else if (elementType.Equals(ELEMENTTYPE_LocationFrom))
                return "C_LocFrom_ID";
            else if (elementType.Equals(ELEMENTTYPE_LocationTo))
                return "C_LocTo_ID";
            else if (elementType.Equals(ELEMENTTYPE_Campaign))
                return "C_Campaign_ID";
            else if (elementType.Equals(ELEMENTTYPE_OrgTrx))
                return "AD_OrgTrx_ID";
            else if (elementType.Equals(ELEMENTTYPE_Project))
                return "C_Project_ID";
            else if (elementType.Equals(ELEMENTTYPE_SalesRegion))
                return "C_SalesRegion_ID";
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
                return "SELECT Value,Name FROM AD_Org WHERE AD_Org_ID=";
            else if (elementType.Equals(ELEMENTTYPE_Account))
                return "SELECT Value,Name FROM C_ElementValue WHERE C_ElementValue_ID=";
            else if (elementType.Equals(ELEMENTTYPE_SubAccount))
                return "SELECT Value,Name FROM C_SubAccount WHERE C_SubAccount_ID=";
            else if (elementType.Equals(ELEMENTTYPE_BPartner))
                return "SELECT Value,Name FROM C_BPartner WHERE C_BPartner_ID=";
            else if (elementType.Equals(ELEMENTTYPE_Product))
                return "SELECT Value,Name FROM M_Product WHERE M_Product_ID=";
            else if (elementType.Equals(ELEMENTTYPE_Activity))
                return "SELECT Value,Name FROM C_Activity WHERE C_Activity_ID=";
            else if (elementType.Equals(ELEMENTTYPE_LocationFrom))
                return "SELECT City,Address1 FROM C_Location WHERE C_Location_ID=";
            else if (elementType.Equals(ELEMENTTYPE_LocationTo))
                return "SELECT City,Address1 FROM C_Location WHERE C_Location_ID=";
            else if (elementType.Equals(ELEMENTTYPE_Campaign))
                return "SELECT Value,Name FROM C_Campaign WHERE C_Campaign_ID=";
            else if (elementType.Equals(ELEMENTTYPE_OrgTrx))
                return "SELECT Value,Name FROM AD_Org WHERE AD_Org_ID=";
            else if (elementType.Equals(ELEMENTTYPE_Project))
                return "SELECT Value,Name FROM C_Project WHERE C_Project_ID=";
            else if (elementType.Equals(ELEMENTTYPE_SalesRegion))
                return "SELECT Value,Name FROM C_SalesRegion WHERE C_SalesRegion_ID";
            else if (elementType.Equals(ELEMENTTYPE_UserList1))
                return "SELECT Value,Name FROM C_ElementValue WHERE C_ElementValue_ID=";
            else if (elementType.Equals(ELEMENTTYPE_UserList2))
                return "SELECT Value,Name FROM C_ElementValue WHERE C_ElementValue_ID=";
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
        /// <param name="C_AcctSchema_Element_ID"></param>
        /// <param name="trxName"></param>
        public MAcctSchemaElement(Ctx ctx, int C_AcctSchema_Element_ID, Trx trxName)
            : base(ctx, C_AcctSchema_Element_ID, trxName)
        {
            if (C_AcctSchema_Element_ID == 0)
            {
                //	setC_AcctSchema_Element_ID (0);
                //	setC_AcctSchema_ID (0);
                //	setC_Element_ID (0);
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
        public MAcctSchemaElement(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="as1">accounting schema</param>
        public MAcctSchemaElement(MAcctSchema as1)
            : this(as1.GetCtx(), 0, as1.Get_TrxName())
        {
            SetClientOrg(as1);
            SetC_AcctSchema_ID(as1.GetC_AcctSchema_ID());

            //	setC_Element_ID (0);
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
        /// <param name="C_Element_ID">element</param>
        /// <param name="C_ElementValue_ID">element value</param>
        public void SetTypeAccount(int SeqNo, String Name, int C_Element_ID, int C_ElementValue_ID)
        {
            SetElementType(ELEMENTTYPE_Account);
            SetSeqNo(SeqNo);
            SetName(Name);
            SetC_Element_ID(C_Element_ID);
            SetC_ElementValue_ID(C_ElementValue_ID);
        }

        /// <summary>
        /// Set Type BPartner
        /// </summary>
        /// <param name="SeqNo">sequence</param>
        /// <param name="Name">SeqNo</param>
        /// <param name="C_BPartner_ID">id</param>
        public void SetTypeBPartner(int SeqNo, String Name, int C_BPartner_ID)
        {
            SetElementType(ELEMENTTYPE_BPartner);
            SetSeqNo(SeqNo);
            SetName(Name);
            SetC_BPartner_ID(C_BPartner_ID);
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
        /// <param name="C_Project_ID">id</param>
        public void SetTypeProject(int SeqNo, String Name, int C_Project_ID)
        {
            SetElementType(ELEMENTTYPE_Project);
            SetSeqNo(SeqNo);
            SetName(Name);
            SetC_Project_ID(C_Project_ID);
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
                defaultValue = GetC_ElementValue_ID();
            else if (elementType.Equals(ELEMENTTYPE_BPartner))
                defaultValue = GetC_BPartner_ID();
            else if (elementType.Equals(ELEMENTTYPE_Product))
                defaultValue = GetM_Product_ID();
            else if (elementType.Equals(ELEMENTTYPE_Activity))
                defaultValue = GetC_Activity_ID();
            else if (elementType.Equals(ELEMENTTYPE_LocationFrom))
                defaultValue = GetC_Location_ID();
            else if (elementType.Equals(ELEMENTTYPE_LocationTo))
                defaultValue = GetC_Location_ID();
            else if (elementType.Equals(ELEMENTTYPE_Campaign))
                defaultValue = GetC_Campaign_ID();
            else if (elementType.Equals(ELEMENTTYPE_OrgTrx))
                defaultValue = GetOrg_ID();
            else if (elementType.Equals(ELEMENTTYPE_Project))
                defaultValue = GetC_Project_ID();
            else if (elementType.Equals(ELEMENTTYPE_SalesRegion))
                defaultValue = GetC_SalesRegion_ID();
            else if (elementType.Equals(ELEMENTTYPE_UserList1))
                defaultValue = GetC_ElementValue_ID();
            else if (elementType.Equals(ELEMENTTYPE_UserList2))
                defaultValue = GetC_ElementValue_ID();
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
                    _ColumnName = MColumn.GetColumnName(GetCtx(), GetAD_Column_ID());
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
            if (GetAD_Org_ID() != 0)
                SetAD_Org_ID(0);
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
                if (ELEMENTTYPE_Account.Equals(et) && GetC_ElementValue_ID() == 0)
                    errorField = "C_ElementValue_ID";
                else if (ELEMENTTYPE_Activity.Equals(et) && GetC_Activity_ID() == 0)
                    errorField = "C_Activity_ID";
                else if (ELEMENTTYPE_BPartner.Equals(et) && GetC_BPartner_ID() == 0)
                    errorField = "C_BPartner_ID";
                else if (ELEMENTTYPE_Campaign.Equals(et) && GetC_Campaign_ID() == 0)
                    errorField = "C_Campaign_ID";
                else if (ELEMENTTYPE_LocationFrom.Equals(et) && GetC_Location_ID() == 0)
                    errorField = "C_Location_ID";
                else if (ELEMENTTYPE_LocationTo.Equals(et) && GetC_Location_ID() == 0)
                    errorField = "C_Location_ID";
                else if (ELEMENTTYPE_Organization.Equals(et) && GetOrg_ID() == 0)
                    errorField = "Org_ID";
                else if (ELEMENTTYPE_OrgTrx.Equals(et) && GetOrg_ID() == 0)
                    errorField = "Org_ID";
                else if (ELEMENTTYPE_Product.Equals(et) && GetM_Product_ID() == 0)
                    errorField = "M_Product_ID";
                else if (ELEMENTTYPE_Project.Equals(et) && GetC_Project_ID() == 0)
                    errorField = "C_Project_ID";
                else if (ELEMENTTYPE_SalesRegion.Equals(et) && GetC_SalesRegion_ID() == 0)
                    errorField = "C_SalesRegion_ID";
                if (errorField != null)
                {
                    log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@IsMandatory@: @" + errorField + "@"));
                    return false;
                }
            }
            //
            if (GetAD_Column_ID() == 0
                && (ELEMENTTYPE_UserElement1.Equals(et) || ELEMENTTYPE_UserElement2.Equals(et)))
            {
                log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@IsMandatory@: @AD_Column_ID@"));
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
                    UpdateData("C_Activity_ID", GetC_Activity_ID());
                else if (ELEMENTTYPE_BPartner.Equals(GetElementType()))
                    UpdateData("C_BPartner_ID", GetC_BPartner_ID());
                else if (ELEMENTTYPE_Product.Equals(GetElementType()))
                    UpdateData("M_Product_ID", GetM_Product_ID());
                else if (ELEMENTTYPE_Project.Equals(GetElementType()))
                    UpdateData("C_Project_ID", GetC_Project_ID());
            }
            //	Resequence
            if (newRecord || Is_ValueChanged("SeqNo"))
                MAccount.UpdateValueDescription(GetCtx(), "AD_Client_ID=" + GetAD_Client_ID(), Get_TrxName());
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
            String sql = "UPDATE C_ValidCombination SET " + element + "=" + id
                + " WHERE " + element + " IS NULL AND AD_Client_ID=" + GetAD_Client_ID();
            int noC = Convert.ToInt32(DataBase.DB.ExecuteQuery(sql, null, Get_TrxName()));
            //
            sql = "UPDATE Fact_Acct SET " + element + "=" + id
                + " WHERE " + element + " IS NULL AND C_AcctSchema_ID=" + GetC_AcctSchema_ID();
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
            MAccount.UpdateValueDescription(GetCtx(), "AD_Client_ID=" + GetAD_Client_ID(), Get_TrxName());
            //
            s_cache.Clear();
            return success;
        }
    }
}
