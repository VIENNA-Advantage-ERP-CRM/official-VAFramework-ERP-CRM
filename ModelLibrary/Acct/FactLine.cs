/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : FactLine
 * Purpose        : Accounting Fact Entry.
 * Class Used     : X_Fact_Acct
 * Chronological    Development
 * Raghunandan      19-Jan-2010
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
using System.Data.SqlClient;
using VAdvantage.Acct;

namespace VAdvantage.Acct
{
    public sealed class FactLine : X_Fact_Acct
    {
        //Account					
        private MAccount _acct = null;
        // Accounting Schema		
        private MAcctSchema _acctSchema = null;
        // Document Header			
        private Doc _doc = null;
        // Document Line 			
        private DocLine _docLine = null;
        // conversion Rate
        private Decimal _ConversionRate = Env.ZERO;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Table_ID"></param>
        /// <param name="Record_ID"></param>
        /// <param name="Line_ID"></param>
        /// <param name="trxName"></param>
        public FactLine(Ctx ctx, int AD_Table_ID, int Record_ID, int Line_ID, Trx trxName)
            : base(ctx, 0, trxName)
        {
            SetAD_Client_ID(0);							//	do not derive
            SetAD_Org_ID(0);							//	do not derive
            //
            SetAmtAcctCr(Env.ZERO);
            SetAmtAcctDr(Env.ZERO);
            SetAmtSourceCr(Env.ZERO);
            SetAmtSourceDr(Env.ZERO);
            SetAD_Table_ID(AD_Table_ID);
            SetRecord_ID(Record_ID);
            SetLine_ID(Line_ID);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="AD_Table_ID">Table</param>
        /// <param name="Record_ID">record</param>
        /// <param name="Line_ID">Line</param>
        /// <param name="AD_Window_ID">Window</param>
        /// <param name="trxName">transaction</param>
        public FactLine(Ctx ctx, int AD_Table_ID, int Record_ID, int Line_ID, int AD_Window_ID, Trx trxName)
        : base(ctx, 0, trxName)
        {
            SetAD_Client_ID(0);							//	do not derive
            SetAD_Org_ID(0);							//	do not derive
            SetAmtAcctCr(Env.ZERO);
            SetAmtAcctDr(Env.ZERO);
            SetAmtSourceCr(Env.ZERO);
            SetAmtSourceDr(Env.ZERO);
            SetAD_Table_ID(AD_Table_ID);
            SetRecord_ID(Record_ID);
            SetLine_ID(Line_ID);
            Set_Value("AD_Window_ID", AD_Window_ID);
        }


        /// <summary>
        /// Create Reversal (negate DR/CR) of the line
        /// </summary>
        /// <param name="description"> new description</param>
        /// <returns>reversal line</returns>
        public FactLine Reverse(String description)
        {
            FactLine reversal = new FactLine(GetCtx(), GetAD_Table_ID(), GetRecord_ID(), GetLine_ID(), Get_TrxName());
            reversal.SetClientOrg(this);	//	needs to be set explicitly
            reversal.SetDocumentInfo(_doc, _docLine);
            reversal.SetAccount(_acctSchema, _acct);
            reversal.SetPostingType(GetPostingType());
            reversal.SetAmtSource(GetC_Currency_ID(), Decimal.Negate(GetAmtSourceDr()), Decimal.Negate(GetAmtSourceCr()));
            reversal.Convert();
            reversal.SetDescription(description);
            return reversal;
        }

        /// <summary>
        /// Create Accrual (flip CR/DR) of the line
        /// </summary>
        /// <param name="description">new description</param>
        /// <returns>accrual line</returns>
        public FactLine Accrue(String description)
        {
            FactLine accrual = new FactLine(GetCtx(), GetAD_Table_ID(), GetRecord_ID(), GetLine_ID(), Get_TrxName());
            accrual.SetClientOrg(this);	//	needs to be set explicitly
            accrual.SetDocumentInfo(_doc, _docLine);
            accrual.SetAccount(_acctSchema, _acct);
            accrual.SetPostingType(GetPostingType());
            accrual.SetAmtSource(GetC_Currency_ID(), GetAmtSourceCr(), GetAmtSourceDr());
            accrual.Convert();
            accrual.SetDescription(description);
            return accrual;
        }

        /// <summary>
        /// Set Account Info
        /// </summary>
        /// <param name="acctSchema">account schema</param>
        /// <param name="acct">account</param>
        public void SetAccount(MAcctSchema acctSchema, MAccount acct)
        {
            _acctSchema = acctSchema;
            SetC_AcctSchema_ID(acctSchema.GetC_AcctSchema_ID());
            //
            _acct = acct;
            if (GetAD_Client_ID() == 0)
            {
                SetAD_Client_ID(_acct.GetAD_Client_ID());
            }
            SetAccount_ID(_acct.GetAccount_ID());
            SetC_SubAcct_ID(_acct.GetC_SubAcct_ID());

            //	User Defined References
            MAcctSchemaElement ud1 = _acctSchema.GetAcctSchemaElement(
                    X_C_AcctSchema_Element.ELEMENTTYPE_UserElement1);
            if (ud1 != null && GetUserElement1_ID() <= 0)
            {
                String ColumnName1 = ud1.GetDisplayColumnName();
                if (ColumnName1 != null)
                {
                    int ID1 = 0;
                    if (_docLine != null)
                    {
                        ID1 = _docLine.GetValue(ColumnName1);
                    }
                    if (ID1 == 0)
                    {
                        if (_doc == null)
                        {
                            throw new ArgumentException("Document not set yet");
                        }
                        ID1 = _doc.GetValue(ColumnName1);
                    }
                    if (ID1 != 0)
                    {
                        SetUserElement1_ID(ID1);
                    }
                }
            }
            MAcctSchemaElement ud2 = _acctSchema.GetAcctSchemaElement(
                    X_C_AcctSchema_Element.ELEMENTTYPE_UserElement2);
            if (ud2 != null && GetUserElement2_ID() <= 0)
            {
                String ColumnName2 = ud2.GetDisplayColumnName();
                if (ColumnName2 != null)
                {
                    int ID2 = 0;
                    if (_docLine != null)
                    {
                        ID2 = _docLine.GetValue(ColumnName2);
                    }
                    if (ID2 == 0)
                    {
                        if (_doc == null)
                        {
                            throw new ArgumentException("Document not set yet");
                        }
                        ID2 = _doc.GetValue(ColumnName2);
                    }
                    if (ID2 != 0)
                    {
                        SetUserElement2_ID(ID2);
                    }
                }
            }

            #region change by mohit to consider userelements3 to userelements9 16/12/2016

            //user element 3
            MAcctSchemaElement ud3 = _acctSchema.GetAcctSchemaElement(
                   X_C_AcctSchema_Element.ELEMENTTYPE_UserElement3);
            if (ud3 != null && GetUserElement3_ID() <= 0)
            {
                String ColumnName3 = ud3.GetDisplayColumnName();
                if (ColumnName3 != null)
                {
                    int ID3 = 0;
                    if (_docLine != null)
                    {
                        ID3 = _docLine.GetValue(ColumnName3);
                    }
                    if (ID3 == 0)
                    {
                        if (_doc == null)
                        {
                            throw new ArgumentException("Document not set yet");
                        }
                        ID3 = _doc.GetValue(ColumnName3);
                    }
                    if (ID3 != 0)
                    {
                        SetUserElement3_ID(ID3);
                    }
                }
            }
            //user element 4
            MAcctSchemaElement ud4 = _acctSchema.GetAcctSchemaElement(
                   X_C_AcctSchema_Element.ELEMENTTYPE_UserElement4);
            if (ud4 != null && GetUserElement4_ID() <= 0)
            {
                String ColumnName4 = ud4.GetDisplayColumnName();
                if (ColumnName4 != null)
                {
                    int ID4 = 0;
                    if (_docLine != null)
                    {
                        ID4 = _docLine.GetValue(ColumnName4);
                    }
                    if (ID4 == 0)
                    {
                        if (_doc == null)
                        {
                            throw new ArgumentException("Document not set yet");
                        }
                        ID4 = _doc.GetValue(ColumnName4);
                    }
                    if (ID4 != 0)
                    {
                        SetUserElement4_ID(ID4);
                    }
                }
            }

            //user element 5
            MAcctSchemaElement ud5 = _acctSchema.GetAcctSchemaElement(
                   X_C_AcctSchema_Element.ELEMENTTYPE_UserElement5);
            if (ud5 != null && GetUserElement5_ID() <= 0)
            {
                String ColumnName5 = ud5.GetDisplayColumnName();
                if (ColumnName5 != null)
                {
                    int ID5 = 0;
                    if (_docLine != null)
                    {
                        ID5 = _docLine.GetValue(ColumnName5);
                    }
                    if (ID5 == 0)
                    {
                        if (_doc == null)
                        {
                            throw new ArgumentException("Document not set yet");
                        }
                        ID5 = _doc.GetValue(ColumnName5);
                    }
                    if (ID5 != 0)
                    {
                        SetUserElement5_ID(ID5);
                    }
                }
            }
            //user element 6
            MAcctSchemaElement ud6 = _acctSchema.GetAcctSchemaElement(
                 X_C_AcctSchema_Element.ELEMENTTYPE_UserElement6);
            if (ud6 != null && GetUserElement6_ID() <= 0)
            {
                String ColumnName6 = ud6.GetDisplayColumnName();
                if (ColumnName6 != null)
                {
                    int ID6 = 0;
                    if (_docLine != null)
                    {
                        ID6 = _docLine.GetValue(ColumnName6);
                    }
                    if (ID6 == 0)
                    {
                        if (_doc == null)
                        {
                            throw new ArgumentException("Document not set yet");
                        }
                        ID6 = _doc.GetValue(ColumnName6);
                    }
                    if (ID6 != 0)
                    {
                        SetUserElement6_ID(ID6);
                    }
                }
            }
            //user element 7
            MAcctSchemaElement ud7 = _acctSchema.GetAcctSchemaElement(
                 X_C_AcctSchema_Element.ELEMENTTYPE_UserElement7);
            if (ud7 != null && GetUserElement7_ID() <= 0)
            {
                String ColumnName7 = ud7.GetDisplayColumnName();
                if (ColumnName7 != null)
                {
                    int ID7 = 0;
                    if (_docLine != null)
                    {
                        ID7 = _docLine.GetValue(ColumnName7);
                    }
                    if (ID7 == 0)
                    {
                        if (_doc == null)
                        {
                            throw new ArgumentException("Document not set yet");
                        }
                        ID7 = _doc.GetValue(ColumnName7);
                    }
                    if (ID7 != 0)
                    {
                        SetUserElement7_ID(ID7);
                    }
                }
            }

            //user element 8
            MAcctSchemaElement ud8 = _acctSchema.GetAcctSchemaElement(
                 X_C_AcctSchema_Element.ELEMENTTYPE_UserElement8);
            if (ud8 != null && GetUserElement8_ID() <= 0)
            {
                String ColumnName8 = ud8.GetDisplayColumnName();
                if (ColumnName8 != null)
                {
                    int ID8 = 0;
                    if (_docLine != null)
                    {
                        ID8 = _docLine.GetValue(ColumnName8);
                    }
                    if (ID8 == 0)
                    {
                        if (_doc == null)
                        {
                            throw new ArgumentException("Document not set yet");
                        }
                        ID8 = _doc.GetValue(ColumnName8);
                    }
                    if (ID8 != 0)
                    {
                        SetUserElement8_ID(ID8);
                    }
                }
            }

            //user element 9
            MAcctSchemaElement ud9 = _acctSchema.GetAcctSchemaElement(
                 X_C_AcctSchema_Element.ELEMENTTYPE_UserElement9);
            if (ud9 != null && GetUserElement9_ID() <= 0)
            {
                String ColumnName9 = ud9.GetDisplayColumnName();
                if (ColumnName9 != null)
                {
                    int ID9 = 0;
                    if (_docLine != null)
                    {
                        ID9 = _docLine.GetValue(ColumnName9);
                    }
                    if (ID9 == 0)
                    {
                        if (_doc == null)
                        {
                            throw new ArgumentException("Document not set yet");
                        }
                        ID9 = _doc.GetValue(ColumnName9);
                    }
                    if (ID9 != 0)
                    {
                        SetUserElement9_ID(ID9);
                    }
                }
            }
            #endregion


        }

        /// <summary>
        /// Set Source Amounts
        /// </summary>
        /// <param name="C_Currency_ID">currency</param>
        /// <param name="AmtSourceDr">source amount dr</param>
        /// <param name="AmtSourceCr">source amount cr</param>
        /// <returns>true, if any if the amount is not zero</returns>
        public bool SetAmtSource(int C_Currency_ID, Decimal? AmtSourceDr, Decimal? AmtSourceCr)
        {
            SetC_Currency_ID(C_Currency_ID);
            if (AmtSourceDr != null)
            {
                SetAmtSourceDr(AmtSourceDr);
            }
            if (AmtSourceCr != null)
            {
                SetAmtSourceCr(AmtSourceCr);
            }
            //  one needs to be non zero
            if (GetAmtSourceDr().Equals(Env.ZERO) && GetAmtSourceCr().Equals(Env.ZERO))
            {
                return false;
            }
            //	Currency Precision
            int precision = MCurrency.GetStdPrecision(GetCtx(), C_Currency_ID);
            if (AmtSourceDr != null && Env.Scale(AmtSourceDr.Value) > precision)
            {
                Decimal AmtSourceDr1 = Decimal.Round(AmtSourceDr.Value, precision, MidpointRounding.AwayFromZero);
                log.Warning("Source DR Precision " + AmtSourceDr.Value + " -> " + AmtSourceDr1);
                SetAmtSourceDr(AmtSourceDr1);
            }
            if (AmtSourceCr != null && Env.Scale(AmtSourceCr.Value) > precision)
            {
                Decimal AmtSourceCr1 = Decimal.Round(AmtSourceCr.Value, precision, MidpointRounding.AwayFromZero);
                log.Warning("Source CR Precision " + AmtSourceCr + " -> " + AmtSourceCr1);
                SetAmtSourceCr(AmtSourceCr1);
            }
            return true;
        }

        /// <summary>
        /// Set Accounted Amounts (alternative: call convert)
        /// </summary>
        /// <param name="AmtAcctDr">acct amount dr</param>
        /// <param name="AmtAcctCr">acct amount cr</param>
        public void SetAmtAcct(Decimal? AmtAcctDr, Decimal? AmtAcctCr)
        {
            SetAmtAcctDr(AmtAcctDr.Value);
            SetAmtAcctCr(AmtAcctCr.Value);
        }

        /// <summary>
        /// Get Conversion Rate
        /// </summary>
        /// <returns>Conversion Rate Value</returns>
        public Decimal GetConversionRate()
        {
            return _ConversionRate;
        }

        /// <summary>
        /// Set Conversion Rate
        /// </summary>
        /// <param name="conversionRate">Conversion Rate Value</param>
        public void SetConversionRate(Decimal conversionRate)
        {
            _ConversionRate = conversionRate;
        }

        /// <summary>
        /// Set Document Info
        /// </summary>
        /// <param name="doc">document</param>
        /// <param name="docLine">doc line</param>
        public void SetDocumentInfo(Doc doc, DocLine docLine)
        {
            _doc = doc;
            _docLine = docLine;

            // Get Dimension which is to be posted respective to Accounting Schema
            Dictionary<string, string> acctSchemaElementRecord = new Dictionary<string, string>();
            acctSchemaElementRecord = GetDimenssion(GetC_AcctSchema_ID());

            //	reset
            SetAD_Org_ID(0);
            SetC_SalesRegion_ID(0);
            //	Client
            if (GetAD_Client_ID() == 0)
            {
                SetAD_Client_ID(_doc.GetAD_Client_ID());
            }
            //	Date Trx
            SetDateTrx(_doc.GetDateDoc());
            if (_docLine != null && _docLine.GetDateDoc() != null)
            {
                SetDateTrx(_docLine.GetDateDoc());
            }
            //	Date Acct
            SetDateAcct(_doc.GetDateAcct());
            if (_docLine != null && _docLine.GetDateAcct() != null)
            {
                SetDateAcct(_docLine.GetDateAcct());
            }
            //	Period, Tax
            if (_docLine != null && _docLine.GetC_Period_ID() != 0)
            {
                SetC_Period_ID(_docLine.GetC_Period_ID());
            }
            else
            {
                SetC_Period_ID(_doc.GetC_Period_ID());
            }
            // Set Line Table ID
            if (_docLine != null && _docLine.GetLineTable_ID() > 0 && Get_ColumnIndex("LineTable_ID") > -1)
            {
                Set_Value("LineTable_ID", _docLine.GetLineTable_ID());
            }
            else if (Get_ColumnIndex("LineTable_ID") > -1)
            {
                Set_Value("LineTable_ID", _doc.Get_Table_ID());
            }
            if (_docLine != null)
            {
                SetC_Tax_ID(_docLine.GetC_Tax_ID());
            }
            //	Description
            StringBuilder description = new StringBuilder();
            if (_docLine != null)
            {
                //description.Append(" #").Append(_docLine.GetLine());
                if (_docLine.GetDescription() != null)
                {
                    //description.Append(" (").Append(_docLine.GetDescription()).Append(")");
                    description.Append(_docLine.GetDescription());
                }
                else if (_doc.GetDescription() != null && _doc.GetDescription().Length > 0)
                {
                    description.Append(_doc.GetDocumentNo());
                    description.Append(" #").Append(_docLine.GetLine());
                    description.Append(" (").Append(_doc.GetDescription()).Append(")");
                }
                else
                {
                    // if on header - description not defined then post document No and line as description
                    description.Append(_doc.GetDocumentNo());
                    description.Append(" #").Append(_docLine.GetLine());
                }
            }
            else if (_doc.GetDescription() != null && _doc.GetDescription().Length > 0)
            {
                description.Append(" (").Append(_doc.GetDescription()).Append(")");
            }
            else
            {
                // if on header - description not defined then post document No as description
                description.Append(_doc.GetDocumentNo());
            }
            SetDescription(description.ToString());
            //	Journal Info
            SetGL_Budget_ID(_doc.GetGL_Budget_ID());
            SetGL_Category_ID(_doc.GetGL_Category_ID());

            //	Product
            if (acctSchemaElementRecord.ContainsKey(MAcctSchemaElement.ELEMENTTYPE_Product))
            {
                if (_docLine != null)
                {
                    SetM_Product_ID(_docLine.GetM_Product_ID());
                }
                if (GetM_Product_ID() == 0)
                {
                    SetM_Product_ID(_doc.GetM_Product_ID());
                }
            }
            //	UOM
            if (_docLine != null)
            {
                SetC_UOM_ID(_docLine.GetC_UOM_ID());
            }
            //	Qty
            if (Get_Value("Qty") == null)	// not previously set
            {
                SetQty(_doc.GetQty());	//	neg = outgoing
                if (_docLine != null)
                {
                    SetQty(_docLine.GetQty());
                }
            }

            if (acctSchemaElementRecord.ContainsKey(MAcctSchemaElement.ELEMENTTYPE_LocationFrom))
            {
                //	Loc From (maybe set earlier)
                if (GetC_LocFrom_ID() == 0 && _docLine != null)
                {
                    SetC_LocFrom_ID(_docLine.GetC_LocFrom_ID());
                }
                if (GetC_LocFrom_ID() == 0)
                {
                    SetC_LocFrom_ID(_doc.GetC_LocFrom_ID());
                }
            }

            if (acctSchemaElementRecord.ContainsKey(MAcctSchemaElement.ELEMENTTYPE_LocationTo))
            {
                //	Loc To (maybe set earlier)
                if (GetC_LocTo_ID() == 0 && _docLine != null)
                {
                    SetC_LocTo_ID(_docLine.GetC_LocTo_ID());
                }
                if (GetC_LocTo_ID() == 0)
                {
                    SetC_LocTo_ID(_doc.GetC_LocTo_ID());
                }
            }

            if (acctSchemaElementRecord.ContainsKey(MAcctSchemaElement.ELEMENTTYPE_BPartner))
            {
                //	BPartner
                if (_docLine != null)
                {
                    SetC_BPartner_ID(_docLine.GetC_BPartner_ID());
                }
                if (GetC_BPartner_ID() == 0)
                {
                    SetC_BPartner_ID(_doc.GetC_BPartner_ID());
                }
            }

            if (acctSchemaElementRecord.ContainsKey(MAcctSchemaElement.ELEMENTTYPE_OrgTrx))
            {
                //	Sales Region from BPLocation/Sales Rep
                //	Trx Org
                if (_docLine != null)
                {
                    SetAD_OrgTrx_ID(_docLine.GetAD_OrgTrx_ID());
                }
                if (GetAD_OrgTrx_ID() == 0)
                {
                    SetAD_OrgTrx_ID(_doc.GetAD_OrgTrx_ID());
                }
            }

            //	Set User Dimension
            if (acctSchemaElementRecord.ContainsKey(MAcctSchemaElement.ELEMENTTYPE_UserElement1) && _docLine != null)
            {
                SetUserElement1_ID(_docLine.GetUserElement1());
            }
            if (acctSchemaElementRecord.ContainsKey(MAcctSchemaElement.ELEMENTTYPE_UserElement2) && _docLine != null)
            {
                SetUserElement2_ID(_docLine.GetUserElement2());
            }
            if (acctSchemaElementRecord.ContainsKey(MAcctSchemaElement.ELEMENTTYPE_UserElement3) && _docLine != null)
            {
                SetUserElement3_ID(_docLine.GetUserElement3());
            }
            if (acctSchemaElementRecord.ContainsKey(MAcctSchemaElement.ELEMENTTYPE_UserElement4) && _docLine != null)
            {
                SetUserElement4_ID(_docLine.GetUserElement4());
            }
            if (acctSchemaElementRecord.ContainsKey(MAcctSchemaElement.ELEMENTTYPE_UserElement5) && _docLine != null)
            {
                SetUserElement5_ID(_docLine.GetUserElement5());
            }
            if (acctSchemaElementRecord.ContainsKey(MAcctSchemaElement.ELEMENTTYPE_UserElement6) && _docLine != null)
            {
                SetUserElement6_ID(_docLine.GetUserElement6());
            }
            if (acctSchemaElementRecord.ContainsKey(MAcctSchemaElement.ELEMENTTYPE_UserElement7) && _docLine != null)
            {
                SetUserElement7_ID(_docLine.GetUserElement7());
            }
            if (acctSchemaElementRecord.ContainsKey(MAcctSchemaElement.ELEMENTTYPE_UserElement8) && _docLine != null)
            {
                SetUserElement8_ID(_docLine.GetUserElement8());
            }
            if (acctSchemaElementRecord.ContainsKey(MAcctSchemaElement.ELEMENTTYPE_UserElement9) && _docLine != null)
            {
                SetUserElement9_ID(_docLine.GetUserElement9());
            }
            if (_docLine != null && _docLine.GetConversionRate() > 0)
            {
                SetConversionRate(_docLine.GetConversionRate());
            }

            if (acctSchemaElementRecord.ContainsKey(MAcctSchemaElement.ELEMENTTYPE_Project))
            {
                //	Project
                if (_docLine != null)
                {
                    SetC_Project_ID(_docLine.GetC_Project_ID());
                }
                if (GetC_Project_ID() == 0)
                {
                    SetC_Project_ID(_doc.GetC_Project_ID());
                }
            }

            if (acctSchemaElementRecord.ContainsKey(MAcctSchemaElement.ELEMENTTYPE_Campaign))
            {
                //	Campaign
                if (_docLine != null)
                {
                    SetC_Campaign_ID(_docLine.GetC_Campaign_ID());
                }
                if (GetC_Campaign_ID() == 0)
                {
                    SetC_Campaign_ID(_doc.GetC_Campaign_ID());
                }
            }

            if (acctSchemaElementRecord.ContainsKey(MAcctSchemaElement.ELEMENTTYPE_Activity))
            {
                //	Activity
                if (_docLine != null)
                {
                    SetC_Activity_ID(_docLine.GetC_Activity_ID());
                }
                if (GetC_Activity_ID() == 0)
                {
                    SetC_Activity_ID(_doc.GetC_Activity_ID());
                }
            }

            //	User List 1
            if (_docLine != null)
            {
                SetUser1_ID(_docLine.GetUser1_ID());
            }
            if (GetUser1_ID() == 0)
            {
                SetUser1_ID(_doc.GetUser1_ID());
            }
            //	User List 2
            if (_docLine != null)
            {
                SetUser2_ID(_docLine.GetUser2_ID());
            }
            if (GetUser2_ID() == 0)
            {
                SetUser2_ID(_doc.GetUser2_ID());
            }
            //	References in setAccount
        }


        /// <summary>
        /// Get Dimension define on accounting schema
        /// </summary>
        /// <param name="as1">accounting schema</param>
        /// <returns>dimensions</returns>
        private Dictionary<string, string> GetDimenssion(int C_AcctSchema_ID)
        {
            Dictionary<string, string> acctSchemaElementRecord = new Dictionary<string, string>();
            try
            {
                string sql = @"SELECT ase.ad_client_id ,   ase.ElementType ,   ase.c_activity_id ,   ase.c_bpartner_id ,
                                     ase.c_campaign_id ,   ase.c_location_id ,   ase.c_project_id ,   ase.c_salesregion_id ,
                                     ase.m_product_id ,   ase.org_id ,   c.columnname
                             FROM C_AcctSchema_Element ase LEFT JOIN ad_column c ON ase.ad_column_id   = c.ad_column_id 
                             WHERE ase.C_AcctSchema_ID = " + C_AcctSchema_ID + " AND ase.IsActive = 'Y'";
                DataSet dsAcctSchemaElement = DB.ExecuteDataset(sql, null, null);
                if (dsAcctSchemaElement != null && dsAcctSchemaElement.Tables.Count > 0 && dsAcctSchemaElement.Tables[0].Rows.Count > 0)
                {
                    for (int ase = 0; ase < dsAcctSchemaElement.Tables[0].Rows.Count; ase++)
                    {
                        if (System.Convert.ToString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"]) == "AY")
                        {
                            acctSchemaElementRecord[Util.GetValueOfString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"])] = "C_Activity_ID";
                        }
                        else if (System.Convert.ToString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"]) == "BP")
                        {
                            acctSchemaElementRecord[Util.GetValueOfString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"])] = "C_BPartner_ID";
                        }
                        else if (System.Convert.ToString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"]) == "LF" ||
                                 System.Convert.ToString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"]) == "LT")
                        {
                            acctSchemaElementRecord[Util.GetValueOfString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"])] = "C_Location_ID";
                        }
                        else if (System.Convert.ToString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"]) == "MC")
                        {
                            acctSchemaElementRecord[Util.GetValueOfString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"])] = "C_Campaign_ID";
                        }
                        else if (System.Convert.ToString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"]) == "OT")
                        {
                            acctSchemaElementRecord[Util.GetValueOfString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"])] = "AD_OrgTrx_ID";
                        }
                        else if (System.Convert.ToString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"]) == "PJ")
                        {
                            acctSchemaElementRecord[Util.GetValueOfString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"])] = "C_Project_ID";
                        }
                        else if (System.Convert.ToString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"]) == "PR")
                        {
                            acctSchemaElementRecord[Util.GetValueOfString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"])] = "M_Product_ID";
                        }
                        else if (System.Convert.ToString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"]) == "SR")
                        {
                            acctSchemaElementRecord[Util.GetValueOfString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"])] = "C_SalesRegion_ID";
                        }
                        else if (System.Convert.ToString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"]) == "X1" ||
                                 System.Convert.ToString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"]) == "X2" ||
                                 System.Convert.ToString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"]) == "X3" ||
                                 System.Convert.ToString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"]) == "X4" ||
                                 System.Convert.ToString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"]) == "X5" ||
                                 System.Convert.ToString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"]) == "X6" ||
                                 System.Convert.ToString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"]) == "X7" ||
                                 System.Convert.ToString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"]) == "X8" ||
                                 System.Convert.ToString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"]) == "X9")
                        {
                            acctSchemaElementRecord[Util.GetValueOfString(dsAcctSchemaElement.Tables[0].Rows[ase]["ElementType"])] = System.Convert.ToString(dsAcctSchemaElement.Tables[0].Rows[ase]["columnname"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, "GetDimenssion: error occure -> " + ex.ToString(), ex);
            }
            return acctSchemaElementRecord;
        }

        /// <summary>
        /// Get Document Line
        /// </summary>
        /// <returns>doc line</returns>
        public DocLine GetDocLine()
        {
            return _docLine;
        }

        /// <summary>
        /// Set Description
        /// </summary>
        /// <param name="description">description</param>
        public void AddDescription(String description)
        {
            String original = GetDescription();
            if (original == null || original.Trim().Length == 0)
            {
                base.SetDescription(description);
            }
            else
            {
                base.SetDescription(original + " - " + description);
            }
        }

        /// <summary>
        /// Set Warehouse Locator.
        /// - will overwrite Organization -
        /// </summary>
        /// <param name="M_Locator_ID">locator</param>
        public new void SetM_Locator_ID(int M_Locator_ID)
        {
            base.SetM_Locator_ID(M_Locator_ID);
            SetAD_Org_ID(0);	//	reset
        }


        /// <summary>
        /// Set Location
        /// </summary>
        /// <param name="C_Location_ID"></param>
        /// <param name="isFrom"></param>
        public void SetLocation(int C_Location_ID, bool isFrom)
        {
            if (isFrom)
            {
                SetC_LocFrom_ID(C_Location_ID);
            }
            else
            {
                SetC_LocTo_ID(C_Location_ID);
            }
        }

        /// <summary>
        /// Set Location from Locator
        /// </summary>
        /// <param name="M_Locator_ID"></param>
        /// <param name="isFrom"></param>
        public void SetLocationFromLocator(int M_Locator_ID, bool isFrom)
        {
            if (M_Locator_ID == 0)
            {
                return;
            }
            int C_Location_ID = 0;
            String sql = "SELECT w.C_Location_ID FROM M_Warehouse w, M_Locator l "
                + "WHERE w.M_Warehouse_ID=l.M_Warehouse_ID AND l.M_Locator_ID=" + M_Locator_ID;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                if (idr.Read())
                {
                    C_Location_ID = Utility.Util.GetValueOfInt(idr[0]);//.getInt(1);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                log.Log(Level.SEVERE, sql, e);
                return;
            }
            if (C_Location_ID != 0)
                SetLocation(C_Location_ID, isFrom);
        }

        /// <summary>
        /// Set Location from Busoness Partner Location
        /// </summary>
        /// <param name="C_BPartner_Location_ID"></param>
        /// <param name="isFrom"></param>
        public void SetLocationFromBPartner(int C_BPartner_Location_ID, bool isFrom)
        {
            if (C_BPartner_Location_ID == 0)
            {
                return;
            }
            int C_Location_ID = 0;
            String sql = "SELECT C_Location_ID FROM C_BPartner_Location WHERE C_BPartner_Location_ID=" + C_BPartner_Location_ID;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                if (idr.Read())
                {
                    C_Location_ID = Utility.Util.GetValueOfInt(idr[0]);//.getInt(1);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                    idr.Close();
                log.Log(Level.SEVERE, sql, e);
                return;
            }
            if (C_Location_ID != 0)
            {
                SetLocation(C_Location_ID, isFrom);
            }
        }

        /// <summary>
        /// Set Location from Organization
        /// </summary>
        /// <param name="AD_Org_ID"></param>
        /// <param name="isFrom"></param>
        public void SetLocationFromOrg(int AD_Org_ID, bool isFrom)
        {
            if (AD_Org_ID == 0)
            {
                return;
            }
            int C_Location_ID = 0;
            String sql = "SELECT C_Location_ID FROM AD_OrgInfo WHERE AD_Org_ID=" + AD_Org_ID;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                if (idr.Read())
                {
                    C_Location_ID = Utility.Util.GetValueOfInt(idr[0]);//.getInt(1);
                }
                idr.Close();

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                log.Log(Level.SEVERE, sql, e);
                return;
            }
            if (C_Location_ID != 0)
            {
                SetLocation(C_Location_ID, isFrom);
            }
        }


        /// <summary>
        /// Returns Source Balance of line
        /// </summary>
        /// <returns>source balance</returns>
        public Decimal GetSourceBalance()
        {
            //if (GetAmtSourceDr() == null)
            //{
            //    SetAmtSourceDr(Env.ZERO);
            //}
            //if (GetAmtSourceCr() == null)
            //{
            //    SetAmtSourceCr(Env.ZERO);
            //}
            //
            if (_doc != null && _doc is Doc_GLJournal)
            {
                return Decimal.Subtract(GetAmtAcctDr(), GetAmtAcctCr());
            }
            else
            {
                return Decimal.Subtract(GetAmtSourceDr(), GetAmtSourceCr());
            }
        }

        /// <summary>
        /// Is Debit Source Balance
        /// </summary>
        /// <returns>true if DR source balance</returns>
        public bool IsDrSourceBalance()
        {
            return Env.Signum(GetSourceBalance()) != -1;
        }

        /// <summary>
        /// Get Accounted Balance
        /// </summary>
        /// <returns>accounting balance</returns>
        public Decimal GetAcctBalance()
        {
            //if (GetAmtAcctDr() == null)
            //{
            //    SetAmtAcctDr(Env.ZERO);
            //}
            //if (GetAmtAcctCr() == null)
            //{
            //    SetAmtAcctCr(Env.ZERO);
            //}
            return Decimal.Subtract(GetAmtAcctDr(), GetAmtAcctCr());
        }

        /// <summary>
        /// Is Account on Balance Sheet
        /// </summary>
        /// <returns>true if account is a balance sheet account</returns>
        public bool IsBalanceSheet()
        {
            return _acct.IsBalanceSheet();
        }

        /// <summary>
        /// Currect Accounting Amount.
        /// <pre>
        /// Example:    1       -1      1       -1
        /// Old         100/0   100/0   0/100   0/100
        /// New         99/0    101/0   0/99    0/101
        /// </pre>
        /// </summary>
        /// <param name="deltaAmount">delta amount</param>
        public void CurrencyCorrect(Decimal deltaAmount)
        {
            bool negative = deltaAmount.CompareTo(Env.ZERO) < 0;
            bool adjustDr = Math.Abs(GetAmtAcctDr()).CompareTo(Math.Abs(GetAmtAcctCr())) > 0;

            log.Fine(deltaAmount.ToString()
                + "; Old-AcctDr=" + GetAmtAcctDr() + ",AcctCr=" + GetAmtAcctCr()
                + "; Negative=" + negative + "; AdjustDr=" + adjustDr);

            if (adjustDr)
            {
                if (negative)
                {
                    SetAmtAcctDr(Decimal.Subtract(GetAmtAcctDr(), deltaAmount));
                }
                else
                {
                    SetAmtAcctDr(Decimal.Subtract(GetAmtAcctDr(), deltaAmount));
                }
            }
            else
            {
                if (negative)
                {
                    SetAmtAcctCr(Decimal.Add(GetAmtAcctCr(), deltaAmount));
                }
                else
                {
                    SetAmtAcctCr(Decimal.Add(GetAmtAcctCr(), deltaAmount));
                }
            }

            log.Fine("New-AcctDr=" + GetAmtAcctDr() + ",AcctCr=" + GetAmtAcctCr());
        }

        /// <summary>
        /// Convert to Accounted Currency
        /// </summary>
        /// <returns>true if converted</returns>
        public bool Convert()
        {
            //  Document has no currency
            if (GetC_Currency_ID() == Doc.NO_CURRENCY)
            {
                SetC_Currency_ID(_acctSchema.GetC_Currency_ID());
            }

            if (_acctSchema.GetC_Currency_ID() == GetC_Currency_ID())
            {
                SetAmtAcctDr(GetAmtSourceDr());
                SetAmtAcctCr(GetAmtSourceCr());
                return true;
            }
            //	Get Conversion Type from Line or Header
            int C_ConversionType_ID = 0;
            int AD_Org_ID = 0;
            if (_docLine != null)			//	get from line
            {
                C_ConversionType_ID = _docLine.GetC_ConversionType_ID();
                AD_Org_ID = _docLine.GetAD_Org_ID();
            }
            if (C_ConversionType_ID == 0)	//	get from header
            {
                if (_doc == null)
                {
                    log.Severe("No Document VO");
                    return false;
                }
                C_ConversionType_ID = _doc.GetC_ConversionType_ID();
                if (AD_Org_ID == 0)
                {
                    AD_Org_ID = _doc.GetAD_Org_ID();
                }
            }

            DateTime? convDate = GetDateAcct();

            // For sourceforge bug 1718381: Use transaction date instead of
            // accounting date for currency conversion when the document is Bank
            // Statement. Ideally this should apply to all "reconciliation"
            // accounting entries, but doing just Bank Statement for now to avoid
            // breaking other things.
            if (_doc is Doc_Bank)
            {
                convDate = GetDateTrx();
            }
            else if (_doc is Doc_GLJournal)
            {
                SetAmtAcctDr(Decimal.Round(Decimal.Multiply(GetAmtSourceDr(), _docLine != null ? Util.GetValueOfDecimal(_docLine.GetConversionRate()) : Util.GetValueOfDecimal(GetConversionRate())), _acctSchema.GetStdPrecision()));
                SetAmtAcctCr(Decimal.Round(Decimal.Multiply(GetAmtSourceCr(), _docLine != null ? Util.GetValueOfDecimal(_docLine.GetConversionRate()) : Util.GetValueOfDecimal(GetConversionRate())), _acctSchema.GetStdPrecision()));
                return true;
            }

            SetAmtAcctDr(MConversionRate.Convert(GetCtx(),
                GetAmtSourceDr(), GetC_Currency_ID(), _acctSchema.GetC_Currency_ID(),
                convDate, C_ConversionType_ID, _doc.GetAD_Client_ID(), AD_Org_ID));
            //if (GetAmtAcctDr() == null)
            //{
            //    return false;
            //}
            SetAmtAcctCr(MConversionRate.Convert(GetCtx(),
                GetAmtSourceCr(), GetC_Currency_ID(), _acctSchema.GetC_Currency_ID(),
                convDate, C_ConversionType_ID, _doc.GetAD_Client_ID(), AD_Org_ID));
            return true;
        }

        /// <summary>
        /// Get Account
        /// </summary>
        /// <returns>account</returns>
        public MAccount GetAccount()
        {
            return _acct;
        }

        /// <summary>
        /// To String
        /// </summary>
        /// <returns>String</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("FactLine=[");
            sb.Append(GetAD_Table_ID()).Append(":").Append(GetRecord_ID())
                .Append(",").Append(_acct)
                .Append(",Cur=").Append(GetC_Currency_ID())
                .Append(", DR=").Append(GetAmtSourceDr()).Append("|").Append(GetAmtAcctDr())
                .Append(", CR=").Append(GetAmtSourceCr()).Append("|").Append(GetAmtAcctCr())
                .Append("]");
            return sb.ToString();
        }


        /// <summary>
        /// Get AD_Org_ID (balancing segment).
        /// (if not set directly - from document line, document, account, locator)
        /// <p>
        /// Note that Locator needs to be set before - otherwise
        /// segment balancing might produce the wrong results
        /// </summary>
        /// <returns>AD_Org_ID</returns>
        public new int GetAD_Org_ID()
        {
            if (base.GetAD_Org_ID() != 0)      //  set earlier
            {
                return base.GetAD_Org_ID();
            }
            //	Prio 1 - get from locator - if exist
            if (GetM_Locator_ID() != 0)
            {
                String sql = "SELECT AD_Org_ID FROM M_Locator WHERE M_Locator_ID=" + GetM_Locator_ID() + " AND AD_Client_ID=" + GetAD_Client_ID();
                IDataReader idr = null;
                try
                {
                    idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                    if (idr.Read())
                    {
                        SetAD_Org_ID(Utility.Util.GetValueOfInt(idr[0]));//.getInt(1));
                        log.Finer("AD_Org_ID=" + base.GetAD_Org_ID() + " (1 from M_Locator_ID=" + GetM_Locator_ID() + ")");
                    }
                    else
                    {
                        log.Log(Level.SEVERE, "AD_Org_ID - Did not find M_Locator_ID=" + GetM_Locator_ID());
                    }
                    idr.Close();
                }
                catch (Exception e)
                {
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                    log.Log(Level.SEVERE, sql, e);
                }
            }   //  M_Locator_ID != 0

            //	Prio 2 - get from doc line - if exists (document context overwrites)
            if (_docLine != null && base.GetAD_Org_ID() == 0)
            {
                SetAD_Org_ID(_docLine.GetAD_Org_ID());
                log.Finer("AD_Org_ID=" + base.GetAD_Org_ID() + " (2 from DocumentLine)");
            }
            //	Prio 3 - get from doc - if not GL
            if (_doc != null && base.GetAD_Org_ID() == 0)
            {
                if (MDocBaseType.DOCBASETYPE_GLJOURNAL.Equals(_doc.GetDocumentType()))
                {
                    SetAD_Org_ID(_acct.GetAD_Org_ID()); //	inter-company GL
                    log.Finer("AD_Org_ID=" + base.GetAD_Org_ID() + " (3 from Acct)");
                }
                else
                {
                    SetAD_Org_ID(_doc.GetAD_Org_ID());
                    log.Finer("AD_Org_ID=" + base.GetAD_Org_ID() + " (3 from Document)");
                }
            }
            //	Prio 4 - get from account - if not GL
            if (_doc != null && base.GetAD_Org_ID() == 0)
            {
                if (MDocBaseType.DOCBASETYPE_GLJOURNAL.Equals(_doc.GetDocumentType()))
                {
                    SetAD_Org_ID(_doc.GetAD_Org_ID());
                    log.Finer("AD_Org_ID=" + base.GetAD_Org_ID() + " (4 from Document)");
                }
                else
                {
                    SetAD_Org_ID(_acct.GetAD_Org_ID());
                    log.Finer("AD_Org_ID=" + base.GetAD_Org_ID() + " (4 from Acct)");
                }
            }
            return base.GetAD_Org_ID();
        }

        /// <summary>
        /// Get/derive Sales Region
        /// </summary>
        /// <returns>Sales Region</returns>
        public new int GetC_SalesRegion_ID()
        {
            if (base.GetC_SalesRegion_ID() != 0)
            {
                return base.GetC_SalesRegion_ID();
            }
            //
            if (_docLine != null)
            {
                SetC_SalesRegion_ID(_docLine.GetC_SalesRegion_ID());
            }
            if (_doc != null)
            {
                if (base.GetC_SalesRegion_ID() == 0)
                {
                    SetC_SalesRegion_ID(_doc.GetC_SalesRegion_ID());
                }
                if (base.GetC_SalesRegion_ID() == 0 && _doc.GetBP_C_SalesRegion_ID() > 0)
                {
                    SetC_SalesRegion_ID(_doc.GetBP_C_SalesRegion_ID());
                }
                //	derive SalesRegion if AcctSegment
                if (base.GetC_SalesRegion_ID() == 0
                    && _doc.GetC_BPartner_Location_ID() != 0
                    && _doc.GetBP_C_SalesRegion_ID() == -1)	//	never tried
                //	&& _acctSchema.isAcctSchemaElement(MAcctSchemaElement.ELEMENTTYPE_SalesRegion))
                {
                    String sql = "SELECT COALESCE(C_SalesRegion_ID,0) FROM C_BPartner_Location WHERE C_BPartner_Location_ID=@param1";
                    SetC_SalesRegion_ID(DataBase.DB.GetSQLValue(null, sql, _doc.GetC_BPartner_Location_ID()));

                    if (base.GetC_SalesRegion_ID() != 0)		//	save in VO
                    {
                        _doc.SetBP_C_SalesRegion_ID(base.GetC_SalesRegion_ID());
                        log.Fine("C_SalesRegion_ID=" + base.GetC_SalesRegion_ID() + " (from BPL)");
                    }
                    else	//	From Sales Rep of Document -> Sales Region
                    {
                        sql = "SELECT COALESCE(MAX(C_SalesRegion_ID),0) FROM C_SalesRegion WHERE SalesRep_ID=@param1";
                        SetC_SalesRegion_ID(DataBase.DB.GetSQLValue(null, sql, _doc.GetSalesRep_ID()));
                        if (base.GetC_SalesRegion_ID() != 0)		//	save in VO
                        {
                            _doc.SetBP_C_SalesRegion_ID(base.GetC_SalesRegion_ID());
                            log.Fine("C_SalesRegion_ID=" + base.GetC_SalesRegion_ID() + " (from SR)");
                        }
                        else
                        {
                            _doc.SetBP_C_SalesRegion_ID(-2);	//	don't try again
                        }
                    }
                }
                if (_acct != null && base.GetC_SalesRegion_ID() == 0)
                {
                    SetC_SalesRegion_ID(_acct.GetC_SalesRegion_ID());
                }
            }
            //
            //	log.Fine("C_SalesRegion_ID=" + base.getC_SalesRegion_ID() 
            //		+ ", C_BPartner_Location_ID=" + m_docVO.C_BPartner_Location_ID
            //		+ ", BP_C_SalesRegion_ID=" + m_docVO.BP_C_SalesRegion_ID 
            //		+ ", SR=" + _acctSchema.isAcctSchemaElement(MAcctSchemaElement.ELEMENTTYPE_SalesRegion));
            return base.GetC_SalesRegion_ID();
        }


        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (newRecord)
            {
                log.Fine(ToString());
                //
                GetAD_Org_ID();
                GetC_SalesRegion_ID();
                //  Set Default Account Info
                if (GetM_Product_ID() == 0)
                {
                    SetM_Product_ID(_acct.GetM_Product_ID());
                }
                if (GetC_LocFrom_ID() == 0)
                {
                    SetC_LocFrom_ID(_acct.GetC_LocFrom_ID());
                }
                if (GetC_LocTo_ID() == 0)
                {
                    SetC_LocTo_ID(_acct.GetC_LocTo_ID());
                }
                if (GetC_BPartner_ID() == 0)
                {
                    SetC_BPartner_ID(_acct.GetC_BPartner_ID());
                }
                if (GetAD_OrgTrx_ID() == 0)
                {
                    SetAD_OrgTrx_ID(_acct.GetAD_OrgTrx_ID());
                }
                if (GetC_Project_ID() == 0)
                {
                    SetC_Project_ID(_acct.GetC_Project_ID());
                }
                if (GetC_Campaign_ID() == 0)
                {
                    SetC_Campaign_ID(_acct.GetC_Campaign_ID());
                }
                if (GetC_Activity_ID() == 0)
                {
                    SetC_Activity_ID(_acct.GetC_Activity_ID());
                }
                if (GetUser1_ID() == 0)
                {
                    SetUser1_ID(_acct.GetUser1_ID());
                }
                if (GetUser2_ID() == 0)
                {
                    SetUser2_ID(_acct.GetUser2_ID());
                }
                if (GetUserElement1_ID() == 0)
                {
                    SetUserElement1_ID(_acct.GetUserElement1_ID());
                }
                if (GetUserElement2_ID() == 0)
                {
                    SetUserElement2_ID(_acct.GetUserElement2_ID());
                }
                if (GetUserElement3_ID() == 0)
                {
                    SetUserElement3_ID(_acct.GetUserElement3_ID());
                }
                if (GetUserElement4_ID() == 0)
                {
                    SetUserElement4_ID(_acct.GetUserElement4_ID());
                }
                if (GetUserElement5_ID() == 0)
                {
                    SetUserElement5_ID(_acct.GetUserElement5_ID());
                }
                if (GetUserElement6_ID() == 0)
                {
                    SetUserElement6_ID(_acct.GetUserElement6_ID());
                }
                if (GetUserElement7_ID() == 0)
                {
                    SetUserElement7_ID(_acct.GetUserElement7_ID());
                }
                if (GetUserElement8_ID() == 0)
                {
                    SetUserElement8_ID(_acct.GetUserElement8_ID());
                }
                if (GetUserElement9_ID() == 0)
                {
                    SetUserElement9_ID(_acct.GetUserElement9_ID());
                }


                //  Revenue Recognition for AR Invoices
                if (_doc.GetDocumentType().Equals(MDocBaseType.DOCBASETYPE_ARINVOICE)
                    && _docLine != null
                    && _docLine.GetC_RevenueRecognition_ID() != 0)
                {
                    int AD_User_ID = 0;
                    SetAccount_ID(
                        CreateRevenueRecognition(
                            _docLine.GetC_RevenueRecognition_ID(), _docLine.Get_ID(),
                            GetAD_Client_ID(), GetAD_Org_ID(), AD_User_ID,
                            GetAccount_ID(), GetC_SubAcct_ID(),
                            GetM_Product_ID(), GetC_BPartner_ID(), GetAD_OrgTrx_ID(),
                            GetC_LocFrom_ID(), GetC_LocTo_ID(),
                            GetC_SalesRegion_ID(), GetC_Project_ID(),
                            GetC_Campaign_ID(), GetC_Activity_ID(),
                            GetUser1_ID(), GetUser2_ID(),
                            GetUserElement1_ID(), GetUserElement2_ID())
                        );
                }
            }
            return true;
        }

        /// <summary>
        /// Revenue Recognition.
        /// Called from FactLine.save
        /// <p>
        /// Create Revenue recognition plan and return Unearned Revenue account
        /// to be used instead of Revenue Account. If not found, it returns
        /// the revenue account.
        /// </summary>
        /// <param name="C_RevenueRecognition_ID">revenue recognition</param>
        /// <param name="C_InvoiceLine_ID">invoice line</param>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="AD_Org_ID">Org</param>
        /// <param name="AD_User_ID">user</param>
        /// <param name="Account_ID">of Revenue Account</param>
        /// <param name="C_SubAcct_ID"> sub account</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="C_BPartner_ID">bpartner</param>
        /// <param name="AD_OrgTrx_ID"> trx org</param>
        /// <param name="C_LocFrom_ID">loc from</param>
        /// <param name="C_LocTo_ID">loc to</param>
        /// <param name="C_SRegion_ID">sales region</param>
        /// <param name="C_Project_ID">project</param>
        /// <param name="C_Campaign_ID">campaign</param>
        /// <param name="C_Activity_ID">activity</param>
        /// <param name="User1_ID"></param>
        /// <param name="User2_ID"></param>
        /// <param name="UserElement1_ID">user element 1</param>
        /// <param name="UserElement2_ID">user element 2</param>
        /// <returns></returns>
        private int CreateRevenueRecognition(
            int C_RevenueRecognition_ID, int C_InvoiceLine_ID,
            int AD_Client_ID, int AD_Org_ID, int AD_User_ID,
            int Account_ID, int C_SubAcct_ID,
            int M_Product_ID, int C_BPartner_ID, int AD_OrgTrx_ID,
            int C_LocFrom_ID, int C_LocTo_ID, int C_SRegion_ID, int C_Project_ID,
            int C_Campaign_ID, int C_Activity_ID,
            int User1_ID, int User2_ID, int UserElement1_ID, int UserElement2_ID)
        {
            log.Fine("From Accout_ID=" + Account_ID);
            //  get VC for P_Revenue (from Product)
            MAccount revenue = MAccount.Get(GetCtx(),
                AD_Client_ID, AD_Org_ID, GetC_AcctSchema_ID(), Account_ID, C_SubAcct_ID,
                M_Product_ID, C_BPartner_ID, AD_OrgTrx_ID, C_LocFrom_ID, C_LocTo_ID, C_SRegion_ID,
                C_Project_ID, C_Campaign_ID, C_Activity_ID,
                User1_ID, User2_ID, UserElement1_ID, UserElement2_ID);
            if (revenue != null && revenue.Get_ID() == 0)
            {
                revenue.Save();
            }
            if (revenue == null || revenue.Get_ID() == 0)
            {
                log.Severe("Revenue_Acct not found");
                return Account_ID;
            }
            int P_Revenue_Acct = revenue.Get_ID();

            //  get Unearned Revenue Acct from BPartner Group
            int unearnedRevenue_Acct = 0;
            int new_Account_ID = 0;

            String sql = "SELECT ga.UnearnedRevenue_Acct, vc.Account_ID "
                + "FROM C_BP_Group_Acct ga, C_BPartner p, C_ValidCombination vc "
                + "WHERE ga.C_BP_Group_ID=p.C_BP_Group_ID"
                + " AND ga.UnearnedRevenue_Acct=vc.C_ValidCombination_ID"
                + " AND ga.C_AcctSchema_ID=" + GetC_AcctSchema_ID() + " AND p.C_BPartner_ID=" + C_BPartner_ID;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                if (idr.Read())
                {
                    unearnedRevenue_Acct = Utility.Util.GetValueOfInt(idr[0]);///.getInt(1);
                    new_Account_ID = Utility.Util.GetValueOfInt(idr[1]);//.getInt(2);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                    idr.Close();
                log.Log(Level.SEVERE, sql, e);
            }
            if (new_Account_ID == 0)
            {
                log.Severe("UnearnedRevenue_Acct not found");
                return Account_ID;
            }

            MRevenueRecognitionPlan plan = new MRevenueRecognitionPlan(GetCtx(), 0, null);
            plan.SetC_RevenueRecognition_ID(C_RevenueRecognition_ID);
            plan.SetC_AcctSchema_ID(GetC_AcctSchema_ID());
            plan.SetC_InvoiceLine_ID(C_InvoiceLine_ID);
            plan.SetUnEarnedRevenue_Acct(unearnedRevenue_Acct);
            plan.SetP_Revenue_Acct(P_Revenue_Acct);
            plan.SetC_Currency_ID(GetC_Currency_ID());
            plan.SetTotalAmt(GetAcctBalance());
            if (!plan.Save(Get_TrxName()))
            {
                log.Severe("Plan NOT created");
                return Account_ID;
            }
            log.Fine("From Acctount_ID=" + Account_ID + " to " + new_Account_ID
                + " - Plan from UnearnedRevenue_Acct=" + unearnedRevenue_Acct + " to Revenue_Acct=" + P_Revenue_Acct);
            return new_Account_ID;
        }


        /// <summary>
        /// Update Line with reversed Original Amount in Accounting Currency.
        /// Also copies original dimensions like Project, etc.
        /// Called from Doc_MatchInv
        /// </summary>
        /// <param name="AD_Table_ID"></param>
        /// <param name="Record_ID"></param>
        /// <param name="Line_ID"></param>
        /// <param name="multiplier">targetQty/documentQty</param>
        /// <returns>true if success</returns>
        public bool UpdateReverseLine(int AD_Table_ID, int Record_ID, int Line_ID, Decimal multiplier)
        {
            bool success = false;

            String sql = "SELECT * "
                + "FROM Fact_Acct "
                + "WHERE C_AcctSchema_ID=" + GetC_AcctSchema_ID() + " AND AD_Table_ID=" + AD_Table_ID + " AND Record_ID=" + Record_ID
                + " AND Line_ID=" + Line_ID + " AND Account_ID=" + _acct.GetAccount_ID();
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                //pstmt.setInt(1, GetC_AcctSchema_ID());
                //pstmt.setInt(2, AD_Table_ID);
                //pstmt.setInt(3, Record_ID);
                //pstmt.setInt(4, Line_ID);
                //pstmt.setInt(5, _acct.GetAccount_ID());

                if (idr.Read())
                {
                    MFactAcct fact = new MFactAcct(GetCtx(), idr, Get_TrxName());
                    //  Accounted Amounts - reverse
                    Decimal dr = fact.GetAmtAcctDr();
                    Decimal cr = fact.GetAmtAcctCr();
                    SetAmtAcctDr(Decimal.Multiply(cr, multiplier));
                    SetAmtAcctCr(Decimal.Multiply(dr, multiplier));
                    //  Source Amounts
                    SetAmtSourceDr(GetAmtAcctDr());
                    SetAmtSourceCr(GetAmtAcctCr());
                    //
                    success = true;
                    log.Fine(new StringBuilder("(Table=").Append(AD_Table_ID)
                        .Append(",Record_ID=").Append(Record_ID)
                        .Append(",Line=").Append(Record_ID)
                        .Append(", Account=").Append(_acct)
                        .Append(",dr=").Append(dr).Append(",cr=").Append(cr)
                        .Append(") - DR=").Append(GetAmtSourceDr()).Append("|").Append(GetAmtAcctDr())
                        .Append(", CR=").Append(GetAmtSourceCr()).Append("|").Append(GetAmtAcctCr())
                        .ToString());
                    //	Dimensions
                    SetAD_OrgTrx_ID(fact.GetAD_OrgTrx_ID());
                    SetC_Project_ID(fact.GetC_Project_ID());
                    SetC_Activity_ID(fact.GetC_Activity_ID());
                    SetC_Campaign_ID(fact.GetC_Campaign_ID());
                    SetC_SalesRegion_ID(fact.GetC_SalesRegion_ID());
                    SetC_LocFrom_ID(fact.GetC_LocFrom_ID());
                    SetC_LocTo_ID(fact.GetC_LocTo_ID());
                    SetM_Product_ID(fact.GetM_Product_ID());
                    SetM_Locator_ID(fact.GetM_Locator_ID());
                    SetUser1_ID(fact.GetUser1_ID());
                    SetUser2_ID(fact.GetUser2_ID());
                    SetC_UOM_ID(fact.GetC_UOM_ID());
                    SetC_Tax_ID(fact.GetC_Tax_ID());
                    //	Org for cross charge
                    SetAD_Org_ID(fact.GetAD_Org_ID());
                }
                else
                {
                    log.Warning(new StringBuilder("Not Found (try later) ")
                        .Append(",C_AcctSchema_ID=").Append(GetC_AcctSchema_ID())
                        .Append(", AD_Table_ID=").Append(AD_Table_ID)
                        .Append(",Record_ID=").Append(Record_ID)
                        .Append(",Line_ID=").Append(Line_ID)
                        .Append(", Account_ID=").Append(_acct.GetAccount_ID()).ToString());
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                log.Log(Level.SEVERE, sql, e);
            }
            return success;
        }

    }
}
