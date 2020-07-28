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
using System.Data;
    using VAdvantage.Utility;
/** Generated Model for testtable
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_testtable : PO
{
public X_testtable (Context ctx, int testtable_ID, Trx trx) : base (ctx, testtable_ID, trx)
{
/** if (testtable_ID == 0)
{
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_testtable (Context ctx, DataRow rs, Trx trx) : base(ctx, rs, trx)
{
}
/** Serial Version No */
//static long serialVersionUID 27519927019081L;
/** Last Updated Timestamp 3/23/2009 3:18:22 PM */
public static long updatedMS = 1237801702292L;
/** AD_Table_ID=1000000 */
public static int Table_ID=1000000;

/** TableName=testtable */
public static String Table_Name="testtable";

protected static KeyNamePair model = new KeyNamePair(1000000,"testtable");

protected Decimal accessLevel = new Decimal(4);
/** AccessLevel
@return 4 - System 
*/
protected override int Get_AccessLevel()
{
return int.Parse(accessLevel.ToString());
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
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_testtable[").Append(Get_ID()).Append("]");
return sb.ToString();
}
}

}
