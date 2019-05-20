using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.DBPort
{
    class ConvertMap_MSSQL
    {

        private static Dictionary<String, String> s_ms = new Dictionary<String, String>();

        static private void initSQLServer()
        {
            //      Oracle Pattern                  Replacement

            //s_ms.Add("\\bTO_NCHAR\\b",                 "CHAR");

            //  Data Types

            //s_ms.Add("DECIMAL(10,0)",                "INTEGER"); //jz numeric int
            //s_ms.Add("DECIMAL(22,0)",                "BIGINT"); //jz numeric int
            //s_ms.Add("\\bNUMBER(10,0)\\b",                "NUMERIC(10,0)"); //jz numeric int
            //s_ms.Add("\\bNUMBER(22,0)\\b",                "NUMERIC(22,0)"); //jz numeric int
            //s_ms.Add("\\bNUMBER(10)\\b",                "NUMERIC(10)"); //jz numeric int
            //s_ms.Add("\\bNUMBER(22)\\b",                "NUMERIC(22)"); //jz numeric int

            //s_ms.Add("\\bNUMBER\\(",                "NUMERIC("); //jz numeric de
            s_ms.Add("\\bNUMBER\\b", "NUMERIC"); //jz numeric de
            //s_ms.Add("\\bNUMBER,",                "NUMERIC(32,6),"); //jz numeric de
            s_ms.Add("\\bDATE\\b", "DATETIME");
            //s_ms.Add("\\bDATE,",                  "DATETIME,");
            s_ms.Add("\\bVARCHAR2\\b", "VARCHAR");
            s_ms.Add("\\bNVARCHAR2\\b", "NVARCHAR");
            //s_ms.Add("\\bNVARCHAR2,",             "NVARCHAR,");
            //s_ms.Add("\\bNVARCHAR2\\(",             "NVARCHAR(");
            s_ms.Add("\\bBLOB\\b", "VARBINARY(MAX)");                 //  BLOB not directly supported
            s_ms.Add("\\bCLOB\\b", "NTEXT");                 //  CLOB not directly supported

            //  Storage
            //s_ms.Add("\\bCACHE\\b",                 "");
            s_ms.Add("\\bUSING INDEX\\b", "");
            s_ms.Add("\\bTABLESPACE\\s\\w+\\b", "");
            s_ms.Add("\\bSTORAGE\\([\\w\\s]+\\)", "");
            s_ms.Add("\\bBITMAP INDEX\\b", "INDEX");

            //  Functions
            s_ms.Add("\\bSysDate\\b", "getdate()");
            //System.out.println("dummy ...........................");
            s_ms.Add("\\bsysDate\\b", "getdate()");

            /*
            s_ms.Add("\\bacctBalance\\b",      "dbo.acctBalance");
            s_ms.Add("\\baddDays\\b",      "dbo.addDays");
            s_ms.Add("\\bbomPriceLimit\\b",      "dbo.bomPriceLimit");
            s_ms.Add("\\bbomPriceList\\b",      "dbo.bomPriceList");
            s_ms.Add("\\bbomPriceStd\\b",      "dbo.bomPriceStd");
            s_ms.Add("\\bbomQtyAvailable\\b",      "dbo.bomQtyAvailable");
            s_ms.Add("\\bbomQtyOnHand\\b",      "dbo.bomQtyOnHand");
            s_ms.Add("\\bbomQtyOrdered\\b",      "dbo.bomQtyOrdered");
            s_ms.Add("\\bbomQtyReserved\\b",      "dbo.bomQtyReserved");
            s_ms.Add("\\bbpartnerRemitLocation\\b",      "dbo.bpartnerRemitLocation");
            s_ms.Add("\\bcharAt\\b",      "dbo.charAt");
            s_ms.Add("\\bcurrencyBase\\b",      "dbo.currencyBase");
            s_ms.Add("\\bcurrencyConvert\\b",      "dbo.currencyConvert");
            s_ms.Add("\\bcurrencyRate\\(\\b",      "dbo.currencyRate(");
            s_ms.Add("\\bcurrencyRound\\b",      "dbo.currencyRound");
            s_ms.Add("\\bdaysBetween\\b",      "dbo.daysBetween");
            s_ms.Add("\\bfirstOf\\b",      "dbo.firstOf");
            s_ms.Add("\\bgetChars\\b",      "dbo.getChars");
            s_ms.Add("\\bgetDaysBetween\\b",      "dbo.getDaysBetween");
            s_ms.Add("\\binvoiceDiscount\\b",      "dbo.invoiceDiscount");
            s_ms.Add("\\binvoiceOpen\\b",      "dbo.invoiceOpen");
            s_ms.Add("\\binvoicePaid\\b",      "dbo.invoicePaid");
            s_ms.Add("\\bpaymentAllocated\\b",      "dbo.paymentAllocated");
            s_ms.Add("\\bpaymentAvailable\\b",      "dbo.paymentAvailable");
            s_ms.Add("\\bpaymentTermDiscount\\b",      "dbo.paymentTermDiscount");
            s_ms.Add("\\bpaymentTermDueDate\\b",      "dbo.paymentTermDueDate");
            s_ms.Add("\\bpaymentTermDueDays\\b",      "dbo.paymentTermDueDays");
            s_ms.Add("\\bproductAttribute\\b",      "dbo.productAttribute");
            */
            s_ms.Add("\\bacctBalance\\b", "[viennadb].ACCTBALANCE");
            s_ms.Add("\\baddDays\\b", "[viennadb].ADDDAYS");
            s_ms.Add("\\bbomPriceLimit\\b", "[viennadb].BOMPRICELIMIT");
            s_ms.Add("\\bbomPriceList\\b", "[viennadb].BOMPRICELIST");
            s_ms.Add("\\bbomPriceStd\\b", "[viennadb].BOMPRICESTD");
            s_ms.Add("\\bbomQtyAvailable\\b", "[viennadb].BOMQTYAVAILABLE");
            s_ms.Add("\\bbomQtyOnHand\\b", "[viennadb].BOMQTYONHAND");
            s_ms.Add("\\bbomQtyOrdered\\b", "[viennadb].BOMQTYORDERED");
            s_ms.Add("\\bbomQtyReserved\\b", "[viennadb].BOMQTYRESERVED");
            s_ms.Add("\\bbpartnerRemitLocation\\b", "[viennadb].BPARTNERREMITLOCATION");
            s_ms.Add("\\bcharAt\\b", "[viennadb].CHARAT");
            s_ms.Add("\\bcurrencyBase\\b", "[viennadb].CURRENCYBASE");
            s_ms.Add("\\bcurrencyConvert\\b", "[viennadb].CURRENCYCONVERT");
            s_ms.Add("\\bcurrencyRate\\(\\b", "[viennadb].CURRENCYRATE(");
            s_ms.Add("\\bcurrencyRound\\b", "[viennadb].CURRENCYROUND");
            s_ms.Add("\\bdaysBetween\\b", "[viennadb].DAYSBETWEEN");
            s_ms.Add("\\bfirstOf\\b", "[viennadb].FIRSTOF");
            s_ms.Add("\\bgetChars\\b", "[viennadb].GETCHARS");
            s_ms.Add("\\bgetDaysBetween\\b", "[viennadb].GETDAYSBETWEEN");
            s_ms.Add("\\binvoiceDiscount\\b", "[viennadb].INVOICEDISCOUNT");
            s_ms.Add("\\binvoiceOpen\\b", "[viennadb].INVOICEOPEN");
            s_ms.Add("\\binvoicePaid\\b", "[viennadb].INVOICEPAID");
            s_ms.Add("\\bpaymentAllocated\\b", "[viennadb].PAYMENTALLOCATED");
            s_ms.Add("\\bpaymentAvailable\\b", "[viennadb].PAYMENTAVAILABLE");
            s_ms.Add("\\bpaymentTermDiscount\\b", "[viennadb].PAYMENTTERMDISCOUNT");
            s_ms.Add("\\bpaymentTermDueDate\\b", "[viennadb].PAYMENTTERMDUEDATE");
            s_ms.Add("\\bpaymentTermDueDays\\b", "[viennadb].PAYMENTTERMDUEDAYS");
            s_ms.Add("\\bproductAttribute\\b", "[viennadb].PRODUCTATTRIBUTE");

            s_ms.Add("\\bTRUNC_DATE\\b", "[viennadb].TRUNC_DATE");
            s_ms.Add("\\bTRUNC2_DATE\\b", "[viennadb].TRUNC2_DATE");
            s_ms.Add("\\bDUMP\\b", "[viennadb].STRDUMP");
            s_ms.Add("\\bTO_NCHAR\\b", "STR");
            s_ms.Add("\\bTO_CHAR\\b", "STR");
            s_ms.Add("\\bNVL\\b", "COALESCE");

            s_ms.Add("\\bTO_CHAR\\b", "[viennadb].TO_CHAR");
            s_ms.Add("\\bTO_DATE\\b", "[viennadb].TO_DATE");
            s_ms.Add("\\bTRIM\\b", "[viennadb].TRIM");



            //jz    s_ms.Add("\\bTO_DATE\\b",               "TO_TIMESTAMP");
            //s_ms.Add("\\bTO_DATE\\b",               "TIMESTAMP");
            //
            s_ms.Add("\\bDBMS_OUTPUT.PUT_LINE\\b", "PRINT");

            //  Temporary
            s_ms.Add("\\bGLOBAL TEMPORARY\\b", "TEMPORARY");
            s_ms.Add("\\bON COMMIT DELETE ROWS\\b", "");
            s_ms.Add("\\bON COMMIT PRESERVE ROWS\\b", "");


            //  DROP TABLE x CASCADE CONSTRAINTS
            s_ms.Add("\\bCASCADE CONSTRAINTS\\b", "");

            s_ms.Add("\\sFROM\\s+DUAL\\b", "");


            //  Statements
            s_ms.Add("\\bELSIF\\b", "ELSE IF");
            s_ms.Add("\\bEND CASE\\b", "END");
            //s_ms.Add("\\bLINENO\\b",                 "[LINENO]");
            s_ms.Add("\\bLineNo\\b", "[LineNo]");

            //Operators
            //s_ms.Add("\\b||\\b",                 "+");

        }   //  initMS

    }
}
