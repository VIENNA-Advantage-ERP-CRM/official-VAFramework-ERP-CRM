/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MTaxDeclarationLine
 * Purpose        : Tax Declaration Line Model
 * Class Used     : X_C_TaxDeclarationLine
 * Chronological    Development
 * Deepak           20-Nov-2009
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

namespace VAdvantage.Model
{
    public class MTaxDeclarationLine : X_C_TaxDeclarationLine
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="C_TaxDeclarationLine_ID">id</param>
        /// <param name="trxName">trx</param>
        public MTaxDeclarationLine(Ctx ctx, int C_TaxDeclarationLine_ID, Trx trxName):base(ctx, C_TaxDeclarationLine_ID, trxName)
        {
            //super(ctx, C_TaxDeclarationLine_ID, trxName);
            if (C_TaxDeclarationLine_ID == 0)
            {
                SetIsManual(true);
                SetTaxAmt(Env.ZERO);
                SetTaxBaseAmt(Env.ZERO);
            }
        }	//	MTaxDeclarationLine

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">trx</param>
        public MTaxDeclarationLine(Ctx ctx, DataRow dr, Trx trxName):base(ctx, dr, trxName)
        {
            //super(ctx, rs, trxName);
        }	//	MTaxDeclarationLine

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="invoice">invoice</param>
        /// <param name="iLine">invoice line</param>
        public MTaxDeclarationLine(MTaxDeclaration parent, MInvoice invoice, MInvoiceLine iLine): this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
           // this(parent.getCtx(), 0, parent.get_TrxName());
            SetClientOrg(invoice);
            SetC_TaxDeclaration_ID(parent.GetC_TaxDeclaration_ID());
            SetIsManual(false);
            //
            SetC_Invoice_ID(invoice.GetC_Invoice_ID());
            SetC_BPartner_ID(invoice.GetC_BPartner_ID());
            SetC_Currency_ID(invoice.GetC_Currency_ID());
            SetDateAcct(invoice.GetDateAcct());
            //
            SetC_InvoiceLine_ID(iLine.GetC_InvoiceLine_ID());
            SetC_Tax_ID(iLine.GetC_Tax_ID());
            if (invoice.IsTaxIncluded())
            {
                SetTaxBaseAmt(iLine.GetLineNetAmt());
                SetTaxAmt(iLine.GetTaxAmt());
            }
            else
            {
                SetTaxBaseAmt(iLine.GetLineNetAmt());
                SetTaxAmt(iLine.GetTaxAmt());
            }
        }	//	MTaxDeclarationLine

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="invoice">invoice</param>
        /// <param name="tLine">tax line</param>
        public MTaxDeclarationLine(MTaxDeclaration parent, MInvoice invoice, MInvoiceTax tLine):this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            //this(parent.getCtx(), 0, parent.get_TrxName());
            SetClientOrg(invoice);
            SetC_TaxDeclaration_ID(parent.GetC_TaxDeclaration_ID());
            SetIsManual(false);
            //
            SetC_Invoice_ID(invoice.GetC_Invoice_ID());
            SetC_BPartner_ID(invoice.GetC_BPartner_ID());
            SetC_Currency_ID(invoice.GetC_Currency_ID());
            SetDateAcct(invoice.GetDateAcct());
            //
            SetC_Tax_ID(tLine.GetC_Tax_ID());
            SetTaxBaseAmt(tLine.GetTaxBaseAmt());
            SetTaxAmt(tLine.GetTaxAmt());
        }	//	MTaxDeclarationLine


    }	//	MTaxDeclarationLine

}
