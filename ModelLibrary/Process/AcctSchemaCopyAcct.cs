/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : AcctSchemaCopyAcct
 * Purpose        : Copy Accounts from one Acct Schema to another
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           23-Nov-2009
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
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.ProcessEngine;

namespace VAdvantage.Process
{
    public class AcctSchemaCopyAcct : ProcessEngine.SvrProcess
    {

        private int _SourceAcctSchema_ID = 0;
        private int _TargetAcctSchema_ID = 0;
        /// <summary>
        /// Prepare 
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("VAB_AccountBook_ID"))
                {
                    _SourceAcctSchema_ID = para[i].GetParameterAsInt();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            _TargetAcctSchema_ID = GetRecord_ID();
        }	//	prepare

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>Info</returns>
        protected override String DoIt()
        {
            log.Info("SourceAcctSchema_ID=" + _SourceAcctSchema_ID
                + ", TargetAcctSchema_ID=" + _TargetAcctSchema_ID);

            if (_SourceAcctSchema_ID == 0 || _TargetAcctSchema_ID == 0)
            {
                throw new Exception("ID=0");
            }
            if (_SourceAcctSchema_ID == _TargetAcctSchema_ID)
            {
                throw new Exception("Account Schema must be different");
            }

            MAcctSchema source = MAcctSchema.Get(GetCtx(), _SourceAcctSchema_ID, null);
            if (source.Get_ID() == 0)
            {
                throw new Exception("@NotFound@ Source @VAB_AccountBook_ID@=" + _SourceAcctSchema_ID);
            }
            MAcctSchema target = new MAcctSchema(GetCtx(), _TargetAcctSchema_ID, Get_Trx());
            if (target.Get_ID() == 0)
            {
                throw new Exception("@NotFound@ Target @VAB_AccountBook_ID@=" + _TargetAcctSchema_ID);
            }
            //

            //	MAcctSchemaElement[] sourceElements = source.getAcctSchemaElements();
            MAcctSchemaElement[] targetElements = target.GetAcctSchemaElements();
            if (targetElements.Length == 0)
            {
                throw new Exception("@NotFound@ Target VAB_AccountBook_Element");
            }
            //	Accounting Element must be the same
            MAcctSchemaElement sourceAcctElement = source.GetAcctSchemaElement(MAcctSchemaElement.ELEMENTTYPE_Account);
            if (sourceAcctElement == null)
            {
                throw new Exception("NotFound Source AC VAB_AccountBook_Element");
            }
            MAcctSchemaElement targetAcctElement = target.GetAcctSchemaElement(MAcctSchemaElement.ELEMENTTYPE_Account);

            if (targetAcctElement == null)
            {
                throw new Exception("NotFound Target AC VAB_AccountBook_Element");
            }
            if (sourceAcctElement.GetVAB_Element_ID() != targetAcctElement.GetVAB_Element_ID())
            {
                throw new Exception("@VAB_Element_ID@ different");
            }
            if (MAcctSchemaGL.Get(GetCtx(), _TargetAcctSchema_ID) == null)
            {
                CopyGL(target);
            }
            if (MAcctSchemaDefault.Get(GetCtx(), _TargetAcctSchema_ID) == null)
            {
                CopyDefault(target);
            }
            return "@OK@";
        }	//	doIt

        /// <summary>
        /// Copy GL 	  
        /// </summary>
        /// <param name="targetAS">target</param>
        private void CopyGL(MAcctSchema targetAS)
        {
            MAcctSchemaGL source = MAcctSchemaGL.Get(GetCtx(), _SourceAcctSchema_ID);
            MAcctSchemaGL target = new MAcctSchemaGL(GetCtx(), 0, Get_Trx());
            target.SetVAB_AccountBook_ID(_TargetAcctSchema_ID);
            //ArrayList<KeyNamePair> list = source.getAcctInfo();
            List<KeyNamePair> list = source.GetAcctInfo();
            for (int i = 0; i < list.Count; i++)
            {
                //KeyNamePair pp = list.get(i);
                KeyNamePair pp = list[i];
                int sourceC_ValidCombination_ID = pp.GetKey();
                String columnName = pp.GetName();
                MAccount sourceAccount = MAccount.Get(GetCtx(), sourceC_ValidCombination_ID);
                MAccount targetAccount = CreateAccount(targetAS, sourceAccount);
                target.SetValue(columnName, Utility.Util.GetValueOfInt(targetAccount.GetC_ValidCombination_ID()));
            }
            if (!target.Save())
            {
                throw new Exception("Could not Save GL");
            }
        }	//	copyGL

        /// <summary>
        /// Copy Default
        /// </summary>
        /// <param name="targetAS">target</param>
        private void CopyDefault(MAcctSchema targetAS)
        {
            MAcctSchemaDefault source = MAcctSchemaDefault.Get(GetCtx(), _SourceAcctSchema_ID);
            MAcctSchemaDefault target = new MAcctSchemaDefault(GetCtx(), 0, Get_Trx());
            target.SetVAB_AccountBook_ID(_TargetAcctSchema_ID);
            target.SetVAB_AccountBook_ID(_TargetAcctSchema_ID);
            //ArrayList<KeyNamePair> list = source.getAcctInfo();
            List<KeyNamePair> list = source.GetAcctInfo();
            for (int i = 0; i < list.Count; i++)
            {
                //KeyNamePair pp = list.get(i);
                KeyNamePair pp = list[i];
                int sourceC_ValidCombination_ID = pp.GetKey();
                String columnName = pp.GetName();
                MAccount sourceAccount = MAccount.Get(GetCtx(), sourceC_ValidCombination_ID);
                MAccount targetAccount = CreateAccount(targetAS, sourceAccount);
                target.SetValue(columnName, Utility.Util.GetValueOfInt(targetAccount.GetC_ValidCombination_ID()));
                
            }
            if (!target.Save())
            {
                throw new Exception("Could not Save Default");
            }
        }	//	copyDefault

        /// <summary>
        /// Create Account
        /// </summary>
        /// <param name="targetAS">target AS</param>
        /// <param name="sourceAcct">source account</param>
        /// <returns>target account</returns>
        private MAccount CreateAccount(MAcctSchema targetAS, MAccount sourceAcct)
        {
            int VAF_Client_ID = targetAS.GetVAF_Client_ID();
            int VAB_AccountBook_ID = targetAS.GetVAB_AccountBook_ID();
            //
            int VAF_Org_ID = 0;
            int Account_ID = 0;
            int C_SubAcct_ID = 0;
            int M_Product_ID = 0;
            int VAB_BusinessPartner_ID = 0;
            int VAF_OrgTrx_ID = 0;
            int C_LocFrom_ID = 0;
            int C_LocTo_ID = 0;
            int C_SalesRegion_ID = 0;
            int C_Project_ID = 0;
            int VAB_Promotion_ID = 0;
            int VAB_BillingCode_ID = 0;
            int User1_ID = 0;
            int User2_ID = 0;
            int UserElement1_ID = 0;
            int UserElement2_ID = 0;
            //
            //  Active Elements
            MAcctSchemaElement[] elements = targetAS.GetAcctSchemaElements();
            for (int i = 0; i < elements.Length; i++)
            {
                MAcctSchemaElement ase = elements[i];
                String elementType = ase.GetElementType();
                //
                if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_Organization))
                {
                    VAF_Org_ID = sourceAcct.GetVAF_Org_ID();
                }
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_Account))
                {
                    Account_ID = sourceAcct.GetAccount_ID();
                }
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_SubAccount))
                {
                    C_SubAcct_ID = sourceAcct.GetC_SubAcct_ID();
                }
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_BPartner))
                {
                    VAB_BusinessPartner_ID = sourceAcct.GetVAB_BusinessPartner_ID();
                }
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_Product))
                {
                    M_Product_ID = sourceAcct.GetM_Product_ID();
                }
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_Activity))
                {
                    VAB_BillingCode_ID = sourceAcct.GetVAB_BillingCode_ID();
                }
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_LocationFrom))
                {
                    C_LocFrom_ID = sourceAcct.GetC_LocFrom_ID();
                }
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_LocationTo))
                {
                    C_LocTo_ID = sourceAcct.GetC_LocTo_ID();
                }
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_Campaign))
                {
                    VAB_Promotion_ID = sourceAcct.GetVAB_Promotion_ID();
                }
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_OrgTrx))
                {
                    VAF_OrgTrx_ID = sourceAcct.GetVAF_OrgTrx_ID();
                }
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_Project))
                {
                    C_Project_ID = sourceAcct.GetC_Project_ID();
                }
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_SalesRegion))
                {
                    C_SalesRegion_ID = sourceAcct.GetC_SalesRegion_ID();
                }
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_UserList1))
                {
                    User1_ID = sourceAcct.GetUser1_ID();
                }
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_UserList2))
                {
                    User2_ID = sourceAcct.GetUser2_ID();
                }
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_UserElement1))
                {
                    UserElement1_ID = sourceAcct.GetUserElement1_ID();
                }
                else if (elementType.Equals(MAcctSchemaElement.ELEMENTTYPE_UserElement2))
                {
                    UserElement2_ID = sourceAcct.GetUserElement2_ID();
                }
                //	No UserElement
            }
            //
            return MAccount.Get(GetCtx(), VAF_Client_ID, VAF_Org_ID,
                VAB_AccountBook_ID, Account_ID, C_SubAcct_ID,
                M_Product_ID, VAB_BusinessPartner_ID, VAF_OrgTrx_ID,
                C_LocFrom_ID, C_LocTo_ID, C_SalesRegion_ID,
                C_Project_ID, VAB_Promotion_ID, VAB_BillingCode_ID,
                User1_ID, User2_ID, UserElement1_ID, UserElement2_ID);
        }	//	createAccount


    }	//	AcctSchemaCopyAcct
}













