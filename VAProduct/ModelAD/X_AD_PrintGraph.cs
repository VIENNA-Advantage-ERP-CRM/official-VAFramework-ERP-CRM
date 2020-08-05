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
/** Generated Model for AD_PrintGraph
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_PrintGraph : PO
{
public X_AD_PrintGraph (Context ctx, int AD_PrintGraph_ID, Trx trxName) : base (ctx, AD_PrintGraph_ID, trxName)
{
/** if (AD_PrintGraph_ID == 0)
{
SetAD_PrintFormat_ID (0);
SetAD_PrintGraph_ID (0);
SetData_PrintFormatItem_ID (0);
SetDescription_PrintFormatItem_ID (0);
SetGraphType (null);
SetName (null);
}
 */
}
public X_AD_PrintGraph (Ctx ctx, int AD_PrintGraph_ID, Trx trxName) : base (ctx, AD_PrintGraph_ID, trxName)
{
/** if (AD_PrintGraph_ID == 0)
{
SetAD_PrintFormat_ID (0);
SetAD_PrintGraph_ID (0);
SetData_PrintFormatItem_ID (0);
SetDescription_PrintFormatItem_ID (0);
SetGraphType (null);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PrintGraph (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PrintGraph (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PrintGraph (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_PrintGraph()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514362878L;
/** Last Updated Timestamp 7/29/2010 1:07:26 PM */
public static long updatedMS = 1280389046089L;
/** AD_Table_ID=521 */
public static int Table_ID;
 // =521;

/** TableName=AD_PrintGraph */
public static String Table_Name="AD_PrintGraph";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(7);
/** AccessLevel
@return 7 - System - Client - Org 
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
StringBuilder sb = new StringBuilder ("X_AD_PrintGraph[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Print Format.
@param AD_PrintFormat_ID Data Print Format */
public void SetAD_PrintFormat_ID (int AD_PrintFormat_ID)
{
if (AD_PrintFormat_ID < 1) throw new ArgumentException ("AD_PrintFormat_ID is mandatory.");
Set_Value ("AD_PrintFormat_ID", AD_PrintFormat_ID);
}
/** Get Print Format.
@return Data Print Format */
public int GetAD_PrintFormat_ID() 
{
Object ii = Get_Value("AD_PrintFormat_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Graph.
@param AD_PrintGraph_ID Graph included in Reports */
public void SetAD_PrintGraph_ID (int AD_PrintGraph_ID)
{
if (AD_PrintGraph_ID < 1) throw new ArgumentException ("AD_PrintGraph_ID is mandatory.");
Set_ValueNoCheck ("AD_PrintGraph_ID", AD_PrintGraph_ID);
}
/** Get Graph.
@return Graph included in Reports */
public int GetAD_PrintGraph_ID() 
{
Object ii = Get_Value("AD_PrintGraph_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Data1_PrintFormatItem_ID AD_Reference_ID=264 */
public static int DATA1_PRINTFORMATITEM_ID_AD_Reference_ID=264;
/** Set Data Column 2.
@param Data1_PrintFormatItem_ID Data Column for Line Charts */
public void SetData1_PrintFormatItem_ID (int Data1_PrintFormatItem_ID)
{
if (Data1_PrintFormatItem_ID <= 0) Set_Value ("Data1_PrintFormatItem_ID", null);
else
Set_Value ("Data1_PrintFormatItem_ID", Data1_PrintFormatItem_ID);
}
/** Get Data Column 2.
@return Data Column for Line Charts */
public int GetData1_PrintFormatItem_ID() 
{
Object ii = Get_Value("Data1_PrintFormatItem_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Data2_PrintFormatItem_ID AD_Reference_ID=264 */
public static int DATA2_PRINTFORMATITEM_ID_AD_Reference_ID=264;
/** Set Data Column 3.
@param Data2_PrintFormatItem_ID Data Column for Line Charts */
public void SetData2_PrintFormatItem_ID (int Data2_PrintFormatItem_ID)
{
if (Data2_PrintFormatItem_ID <= 0) Set_Value ("Data2_PrintFormatItem_ID", null);
else
Set_Value ("Data2_PrintFormatItem_ID", Data2_PrintFormatItem_ID);
}
/** Get Data Column 3.
@return Data Column for Line Charts */
public int GetData2_PrintFormatItem_ID() 
{
Object ii = Get_Value("Data2_PrintFormatItem_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Data3_PrintFormatItem_ID AD_Reference_ID=264 */
public static int DATA3_PRINTFORMATITEM_ID_AD_Reference_ID=264;
/** Set Data Column 4.
@param Data3_PrintFormatItem_ID Data Column for Line Charts */
public void SetData3_PrintFormatItem_ID (int Data3_PrintFormatItem_ID)
{
if (Data3_PrintFormatItem_ID <= 0) Set_Value ("Data3_PrintFormatItem_ID", null);
else
Set_Value ("Data3_PrintFormatItem_ID", Data3_PrintFormatItem_ID);
}
/** Get Data Column 4.
@return Data Column for Line Charts */
public int GetData3_PrintFormatItem_ID() 
{
Object ii = Get_Value("Data3_PrintFormatItem_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Data4_PrintFormatItem_ID AD_Reference_ID=264 */
public static int DATA4_PRINTFORMATITEM_ID_AD_Reference_ID=264;
/** Set Data Column 5.
@param Data4_PrintFormatItem_ID Data Column for Line Charts */
public void SetData4_PrintFormatItem_ID (int Data4_PrintFormatItem_ID)
{
if (Data4_PrintFormatItem_ID <= 0) Set_Value ("Data4_PrintFormatItem_ID", null);
else
Set_Value ("Data4_PrintFormatItem_ID", Data4_PrintFormatItem_ID);
}
/** Get Data Column 5.
@return Data Column for Line Charts */
public int GetData4_PrintFormatItem_ID() 
{
Object ii = Get_Value("Data4_PrintFormatItem_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Data_PrintFormatItem_ID AD_Reference_ID=264 */
public static int DATA_PRINTFORMATITEM_ID_AD_Reference_ID=264;
/** Set Data Column.
@param Data_PrintFormatItem_ID Data Column for Pie and Line Charts */
public void SetData_PrintFormatItem_ID (int Data_PrintFormatItem_ID)
{
if (Data_PrintFormatItem_ID < 1) throw new ArgumentException ("Data_PrintFormatItem_ID is mandatory.");
Set_Value ("Data_PrintFormatItem_ID", Data_PrintFormatItem_ID);
}
/** Get Data Column.
@return Data Column for Pie and Line Charts */
public int GetData_PrintFormatItem_ID() 
{
Object ii = Get_Value("Data_PrintFormatItem_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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

/** Description_PrintFormatItem_ID AD_Reference_ID=264 */
public static int DESCRIPTION_PRINTFORMATITEM_ID_AD_Reference_ID=264;
/** Set Description Column.
@param Description_PrintFormatItem_ID Description Column for Pie/Line/Bar Charts */
public void SetDescription_PrintFormatItem_ID (int Description_PrintFormatItem_ID)
{
if (Description_PrintFormatItem_ID < 1) throw new ArgumentException ("Description_PrintFormatItem_ID is mandatory.");
Set_Value ("Description_PrintFormatItem_ID", Description_PrintFormatItem_ID);
}
/** Get Description Column.
@return Description Column for Pie/Line/Bar Charts */
public int GetDescription_PrintFormatItem_ID() 
{
Object ii = Get_Value("Description_PrintFormatItem_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** GraphType AD_Reference_ID=265 */
public static int GRAPHTYPE_AD_Reference_ID=265;
/** Bar Chart = B */
public static String GRAPHTYPE_BarChart = "B";
/** Line Chart = L */
public static String GRAPHTYPE_LineChart = "L";
/** Pie Chart = P */
public static String GRAPHTYPE_PieChart = "P";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsGraphTypeValid (String test)
{
return test.Equals("B") || test.Equals("L") || test.Equals("P");
}
/** Set Graph Type.
@param GraphType Type of graph to be painted */
public void SetGraphType (String GraphType)
{
if (GraphType == null) throw new ArgumentException ("GraphType is mandatory");
if (!IsGraphTypeValid(GraphType))
throw new ArgumentException ("GraphType Invalid value - " + GraphType + " - Reference_ID=265 - B - L - P");
if (GraphType.Length > 1)
{
log.Warning("Length > 1 - truncated");
GraphType = GraphType.Substring(0,1);
}
Set_Value ("GraphType", GraphType);
}
/** Get Graph Type.
@return Type of graph to be painted */
public String GetGraphType() 
{
return (String)Get_Value("GraphType");
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
}

}
