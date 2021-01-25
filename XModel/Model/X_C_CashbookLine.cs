namespace VAdvantage.Model
{

/** Generated Model - DO NOT CHANGE */
using System;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Common;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Data;
/** Generated Model for VAB_CashbookLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_CashbookLine : PO
{
public X_VAB_CashbookLine (Context ctx, int VAB_CashbookLine_ID, Trx trxName) : base (ctx, VAB_CashbookLine_ID, trxName)
{
/** if (VAB_CashbookLine_ID == 0)
{
SetVAB_CashbookLine_ID (0);
SetVAB_CashBook_ID (0);
SetDateAcct (DateTime.Now);	// @#Date@
}
 */
}
public X_VAB_CashbookLine (Ctx ctx, int VAB_CashbookLine_ID, Trx trxName) : base (ctx, VAB_CashbookLine_ID, trxName)
{
/** if (VAB_CashbookLine_ID == 0)
{
SetVAB_CashbookLine_ID (0);
SetVAB_CashBook_ID (0);
SetDateAcct (DateTime.Now);	// @#Date@
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_CashbookLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_CashbookLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_CashbookLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_CashbookLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27628437988487L;
/** Last Updated Timestamp 8/30/2012 1:14:31 PM */
public static long updatedMS = 1346312671698L;
/** VAF_TableView_ID=1000366 */
public static int Table_ID;
 // =1000366;

/** TableName=VAB_CashbookLine */
public static String Table_Name="VAB_CashbookLine";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(3);
/** AccessLevel
@return 3 - Client - Org 
*/
protected override int Get_AccessLevel()
{
return Convert.ToInt32(accessLevel.ToString());
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO(Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_VAB_CashbookLine[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Cashbook Line.
@param VAB_CashbookLine_ID Cashbook Line */
public void SetVAB_CashbookLine_ID (int VAB_CashbookLine_ID)
{
if (VAB_CashbookLine_ID < 1) throw new ArgumentException ("VAB_CashbookLine_ID is mandatory.");
Set_ValueNoCheck ("VAB_CashbookLine_ID", VAB_CashbookLine_ID);
}
/** Get Cashbook Line.
@return Cashbook Line */
public int GetVAB_CashbookLine_ID() 
{
Object ii = Get_Value("VAB_CashbookLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Cash Book.
@param VAB_CashBook_ID Cash Book for recording petty cash transactions */
public void SetVAB_CashBook_ID (int VAB_CashBook_ID)
{
if (VAB_CashBook_ID < 1) throw new ArgumentException ("VAB_CashBook_ID is mandatory.");
Set_ValueNoCheck ("VAB_CashBook_ID", VAB_CashBook_ID);
}
/** Get Cash Book.
@return Cash Book for recording petty cash transactions */
public int GetVAB_CashBook_ID() 
{
Object ii = Get_Value("VAB_CashBook_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Account Date.
@param DateAcct General Ledger Date */
public void SetDateAcct (DateTime? DateAcct)
{
if (DateAcct == null) throw new ArgumentException ("DateAcct is mandatory.");
Set_Value ("DateAcct", (DateTime?)DateAcct);
}
/** Get Account Date.
@return General Ledger Date */
public DateTime? GetDateAcct() 
{
return (DateTime?)Get_Value("DateAcct");
}
/** Set Statement difference.
@param StatementDifference Difference between statement ending balance and actual ending balance */
public void SetStatementDifference (Decimal? StatementDifference)
{
Set_Value ("StatementDifference", (Decimal?)StatementDifference);
}
/** Get Statement difference.
@return Difference between statement ending balance and actual ending balance */
public Decimal GetStatementDifference() 
{
Object bd =Get_Value("StatementDifference");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Ending balance.
@param actual ending balance */
public void SetEndingBalance(Decimal? EndingBalance)
{
    Set_Value("EndingBalance", (Decimal?)EndingBalance);
}
/** Get Ending balance.
@return actual ending balance */
public Decimal GetEndingBalance()
{
    Object bd = Get_Value("EndingBalance");
    if (bd == null) return Env.ZERO;
    return Convert.ToDecimal(bd);
}

}

}
