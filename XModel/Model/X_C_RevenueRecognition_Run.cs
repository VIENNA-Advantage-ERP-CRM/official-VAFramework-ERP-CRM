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
/** Generated Model for C_RevenueRecognition_Run
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_RevenueRecognition_Run : PO
{
public X_C_RevenueRecognition_Run (Context ctx, int C_RevenueRecognition_Run_ID, Trx trxName) : base (ctx, C_RevenueRecognition_Run_ID, trxName)
{
/** if (C_RevenueRecognition_Run_ID == 0)
{
SetC_RevenueRecognition_Plan_ID (0);
SetC_RevenueRecognition_Run_ID (0);
SetGL_Journal_ID (0);
SetRecognizedAmt (0.0);
}
 */
}
public X_C_RevenueRecognition_Run (Ctx ctx, int C_RevenueRecognition_Run_ID, Trx trxName) : base (ctx, C_RevenueRecognition_Run_ID, trxName)
{
/** if (C_RevenueRecognition_Run_ID == 0)
{
SetC_RevenueRecognition_Plan_ID (0);
SetC_RevenueRecognition_Run_ID (0);
SetGL_Journal_ID (0);
SetRecognizedAmt (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RevenueRecognition_Run (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RevenueRecognition_Run (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RevenueRecognition_Run (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_RevenueRecognition_Run()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514374648L;
/** Last Updated Timestamp 7/29/2010 1:07:37 PM */
public static long updatedMS = 1280389057859L;
/** AD_Table_ID=444 */
public static int Table_ID;
 // =444;

/** TableName=C_RevenueRecognition_Run */
public static String Table_Name="C_RevenueRecognition_Run";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(1);
/** AccessLevel
@return 1 - Org 
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
StringBuilder sb = new StringBuilder ("X_C_RevenueRecognition_Run[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Revenue Recognition Plan.
@param C_RevenueRecognition_Plan_ID Plan for recognizing or recording revenue */
public void SetC_RevenueRecognition_Plan_ID (int C_RevenueRecognition_Plan_ID)
{
if (C_RevenueRecognition_Plan_ID < 1) throw new ArgumentException ("C_RevenueRecognition_Plan_ID is mandatory.");
Set_ValueNoCheck ("C_RevenueRecognition_Plan_ID", C_RevenueRecognition_Plan_ID);
}
/** Get Revenue Recognition Plan.
@return Plan for recognizing or recording revenue */
public int GetC_RevenueRecognition_Plan_ID() 
{
Object ii = Get_Value("C_RevenueRecognition_Plan_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetC_RevenueRecognition_Plan_ID().ToString());
}
/** Set Revenue Recognition Run.
@param C_RevenueRecognition_Run_ID Revenue Recognition Run or Process */
public void SetC_RevenueRecognition_Run_ID (int C_RevenueRecognition_Run_ID)
{
if (C_RevenueRecognition_Run_ID < 1) throw new ArgumentException ("C_RevenueRecognition_Run_ID is mandatory.");
Set_ValueNoCheck ("C_RevenueRecognition_Run_ID", C_RevenueRecognition_Run_ID);
}
/** Get Revenue Recognition Run.
@return Revenue Recognition Run or Process */
public int GetC_RevenueRecognition_Run_ID() 
{
Object ii = Get_Value("C_RevenueRecognition_Run_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Journal.
@param GL_Journal_ID General Ledger Journal */
public void SetGL_Journal_ID (int GL_Journal_ID)
{
if (GL_Journal_ID < 1) throw new ArgumentException ("GL_Journal_ID is mandatory.");
Set_ValueNoCheck ("GL_Journal_ID", GL_Journal_ID);
}
/** Get Journal.
@return General Ledger Journal */
public int GetGL_Journal_ID() 
{
Object ii = Get_Value("GL_Journal_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Recognized Amount.
@param RecognizedAmt Recognized Amount */
public void SetRecognizedAmt (Decimal? RecognizedAmt)
{
if (RecognizedAmt == null) throw new ArgumentException ("RecognizedAmt is mandatory.");
Set_ValueNoCheck ("RecognizedAmt", (Decimal?)RecognizedAmt);
}
/** Get Recognized Amount.
@return Recognized Amount */
public Decimal GetRecognizedAmt() 
{
Object bd =Get_Value("RecognizedAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}
