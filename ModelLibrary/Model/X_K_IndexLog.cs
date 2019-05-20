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
/** Generated Model for K_IndexLog
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_K_IndexLog : PO
{
public X_K_IndexLog (Context ctx, int K_IndexLog_ID, Trx trxName) : base (ctx, K_IndexLog_ID, trxName)
{
/** if (K_IndexLog_ID == 0)
{
SetIndexQuery (null);
SetIndexQueryResult (0);
SetK_IndexLog_ID (0);
SetQuerySource (null);
}
 */
}
public X_K_IndexLog (Ctx ctx, int K_IndexLog_ID, Trx trxName) : base (ctx, K_IndexLog_ID, trxName)
{
/** if (K_IndexLog_ID == 0)
{
SetIndexQuery (null);
SetIndexQueryResult (0);
SetK_IndexLog_ID (0);
SetQuerySource (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_IndexLog (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_IndexLog (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_IndexLog (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_K_IndexLog()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514378064L;
/** Last Updated Timestamp 7/29/2010 1:07:41 PM */
public static long updatedMS = 1280389061275L;
/** AD_Table_ID=899 */
public static int Table_ID;
 // =899;

/** TableName=K_IndexLog */
public static String Table_Name="K_IndexLog";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(6);
/** AccessLevel
@return 6 - System - Client 
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
StringBuilder sb = new StringBuilder ("X_K_IndexLog[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Index Query.
@param IndexQuery Text Search Query */
public void SetIndexQuery (String IndexQuery)
{
if (IndexQuery == null) throw new ArgumentException ("IndexQuery is mandatory.");
if (IndexQuery.Length > 255)
{
log.Warning("Length > 255 - truncated");
IndexQuery = IndexQuery.Substring(0,255);
}
Set_ValueNoCheck ("IndexQuery", IndexQuery);
}
/** Get Index Query.
@return Text Search Query */
public String GetIndexQuery() 
{
return (String)Get_Value("IndexQuery");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetIndexQuery());
}
/** Set Query Result.
@param IndexQueryResult Result of the text query */
public void SetIndexQueryResult (int IndexQueryResult)
{
Set_ValueNoCheck ("IndexQueryResult", IndexQueryResult);
}
/** Get Query Result.
@return Result of the text query */
public int GetIndexQueryResult() 
{
Object ii = Get_Value("IndexQueryResult");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Index Log.
@param K_IndexLog_ID Text search log */
public void SetK_IndexLog_ID (int K_IndexLog_ID)
{
if (K_IndexLog_ID < 1) throw new ArgumentException ("K_IndexLog_ID is mandatory.");
Set_ValueNoCheck ("K_IndexLog_ID", K_IndexLog_ID);
}
/** Get Index Log.
@return Text search log */
public int GetK_IndexLog_ID() 
{
Object ii = Get_Value("K_IndexLog_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** QuerySource AD_Reference_ID=391 */
public static int QUERYSOURCE_AD_Reference_ID=391;
/** Collaboration Management = C */
public static String QUERYSOURCE_CollaborationManagement = "C";
/** HTML Client = H */
public static String QUERYSOURCE_HTMLClient = "H";
/** Java Client = J */
public static String QUERYSOURCE_JavaClient = "J";
/** Self Service = W */
public static String QUERYSOURCE_SelfService = "W";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsQuerySourceValid (String test)
{
return test.Equals("C") || test.Equals("H") || test.Equals("J") || test.Equals("W");
}
/** Set Query Source.
@param QuerySource Source of the Query */
public void SetQuerySource (String QuerySource)
{
if (QuerySource == null) throw new ArgumentException ("QuerySource is mandatory");
if (!IsQuerySourceValid(QuerySource))
throw new ArgumentException ("QuerySource Invalid value - " + QuerySource + " - Reference_ID=391 - C - H - J - W");
if (QuerySource.Length > 1)
{
log.Warning("Length > 1 - truncated");
QuerySource = QuerySource.Substring(0,1);
}
Set_Value ("QuerySource", QuerySource);
}
/** Get Query Source.
@return Source of the Query */
public String GetQuerySource() 
{
return (String)Get_Value("QuerySource");
}
}

}
