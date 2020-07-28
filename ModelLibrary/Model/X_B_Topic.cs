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
/** Generated Model for B_Topic
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_B_Topic : PO
{
public X_B_Topic (Context ctx, int B_Topic_ID, Trx trxName) : base (ctx, B_Topic_ID, trxName)
{
/** if (B_Topic_ID == 0)
{
SetB_TopicCategory_ID (0);
SetB_TopicType_ID (0);
SetB_Topic_ID (0);
SetDecisionDate (DateTime.Now);
SetDocumentNo (null);
SetIsPublished (false);
SetName (null);
SetProcessed (false);	// N
SetTopicAction (null);
SetTopicStatus (null);
}
 */
}
public X_B_Topic (Ctx ctx, int B_Topic_ID, Trx trxName) : base (ctx, B_Topic_ID, trxName)
{
/** if (B_Topic_ID == 0)
{
SetB_TopicCategory_ID (0);
SetB_TopicType_ID (0);
SetB_Topic_ID (0);
SetDecisionDate (DateTime.Now);
SetDocumentNo (null);
SetIsPublished (false);
SetName (null);
SetProcessed (false);	// N
SetTopicAction (null);
SetTopicStatus (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_B_Topic (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_B_Topic (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_B_Topic (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_B_Topic()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514367517L;
/** Last Updated Timestamp 7/29/2010 1:07:30 PM */
public static long updatedMS = 1280389050728L;
/** AD_Table_ID=679 */
public static int Table_ID;
 // =679;

/** TableName=B_Topic */
public static String Table_Name="B_Topic";

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
StringBuilder sb = new StringBuilder ("X_B_Topic[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Topic Category.
@param B_TopicCategory_ID Auction Topic Category */
public void SetB_TopicCategory_ID (int B_TopicCategory_ID)
{
if (B_TopicCategory_ID < 1) throw new ArgumentException ("B_TopicCategory_ID is mandatory.");
Set_ValueNoCheck ("B_TopicCategory_ID", B_TopicCategory_ID);
}
/** Get Topic Category.
@return Auction Topic Category */
public int GetB_TopicCategory_ID() 
{
Object ii = Get_Value("B_TopicCategory_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Topic Type.
@param B_TopicType_ID Auction Topic Type */
public void SetB_TopicType_ID (int B_TopicType_ID)
{
if (B_TopicType_ID < 1) throw new ArgumentException ("B_TopicType_ID is mandatory.");
Set_ValueNoCheck ("B_TopicType_ID", B_TopicType_ID);
}
/** Get Topic Type.
@return Auction Topic Type */
public int GetB_TopicType_ID() 
{
Object ii = Get_Value("B_TopicType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Topic.
@param B_Topic_ID Auction Topic */
public void SetB_Topic_ID (int B_Topic_ID)
{
if (B_Topic_ID < 1) throw new ArgumentException ("B_Topic_ID is mandatory.");
Set_ValueNoCheck ("B_Topic_ID", B_Topic_ID);
}
/** Get Topic.
@return Auction Topic */
public int GetB_Topic_ID() 
{
Object ii = Get_Value("B_Topic_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Decision date.
@param DecisionDate Decision date */
public void SetDecisionDate (DateTime? DecisionDate)
{
if (DecisionDate == null) throw new ArgumentException ("DecisionDate is mandatory.");
Set_Value ("DecisionDate", (DateTime?)DecisionDate);
}
/** Get Decision date.
@return Decision date */
public DateTime? GetDecisionDate() 
{
return (DateTime?)Get_Value("DecisionDate");
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 255)
{
log.Warning("Length > 255 - truncated");
Description = Description.Substring(0,255);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
/** Set Document No.
@param DocumentNo Document sequence number of the document */
public void SetDocumentNo (String DocumentNo)
{
if (DocumentNo == null) throw new ArgumentException ("DocumentNo is mandatory.");
if (DocumentNo.Length > 30)
{
log.Warning("Length > 30 - truncated");
DocumentNo = DocumentNo.Substring(0,30);
}
Set_Value ("DocumentNo", DocumentNo);
}
/** Get Document No.
@return Document sequence number of the document */
public String GetDocumentNo() 
{
return (String)Get_Value("DocumentNo");
}
/** Set Published.
@param IsPublished The Topic is published and can be viewed */
public void SetIsPublished (Boolean IsPublished)
{
Set_Value ("IsPublished", IsPublished);
}
/** Get Published.
@return The Topic is published and can be viewed */
public Boolean IsPublished() 
{
Object oo = Get_Value("IsPublished");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name = Name.Substring(0,60);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetName());
}
/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed)
{
Set_Value ("Processed", Processed);
}
/** Get Processed.
@return The document has been processed */
public Boolean IsProcessed() 
{
Object oo = Get_Value("Processed");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
}
/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() 
{
Object oo = Get_Value("Processing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Details.
@param TextDetails Details */
public void SetTextDetails (String TextDetails)
{
Set_Value ("TextDetails", TextDetails);
}
/** Get Details.
@return Details */
public String GetTextDetails() 
{
return (String)Get_Value("TextDetails");
}
/** Set Text Message.
@param TextMsg Text Message */
public void SetTextMsg (String TextMsg)
{
if (TextMsg != null && TextMsg.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
TextMsg = TextMsg.Substring(0,2000);
}
Set_Value ("TextMsg", TextMsg);
}
/** Get Text Message.
@return Text Message */
public String GetTextMsg() 
{
return (String)Get_Value("TextMsg");
}
/** Set Topic Action.
@param TopicAction Topic Action */
public void SetTopicAction (String TopicAction)
{
if (TopicAction == null) throw new ArgumentException ("TopicAction is mandatory.");
if (TopicAction.Length > 2)
{
log.Warning("Length > 2 - truncated");
TopicAction = TopicAction.Substring(0,2);
}
Set_Value ("TopicAction", TopicAction);
}
/** Get Topic Action.
@return Topic Action */
public String GetTopicAction() 
{
return (String)Get_Value("TopicAction");
}
/** Set Topic Status.
@param TopicStatus Topic Status */
public void SetTopicStatus (String TopicStatus)
{
if (TopicStatus == null) throw new ArgumentException ("TopicStatus is mandatory.");
if (TopicStatus.Length > 2)
{
log.Warning("Length > 2 - truncated");
TopicStatus = TopicStatus.Substring(0,2);
}
Set_Value ("TopicStatus", TopicStatus);
}
/** Get Topic Status.
@return Topic Status */
public String GetTopicStatus() 
{
return (String)Get_Value("TopicStatus");
}
}

}
