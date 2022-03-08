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
/** Generated Model for C_Campaign
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_Campaign : PO
{
public X_C_Campaign (Context ctx, int C_Campaign_ID, Trx trxName) : base (ctx, C_Campaign_ID, trxName)
{
/** if (C_Campaign_ID == 0)
{
SetC_Campaign_ID (0);
SetCosts (0.0);
SetIsSummary (false);
SetName (null);
SetValue (null);
}
 */
}
public X_C_Campaign (Ctx ctx, int C_Campaign_ID, Trx trxName) : base (ctx, C_Campaign_ID, trxName)
{
/** if (C_Campaign_ID == 0)
{
SetC_Campaign_ID (0);
SetCosts (0.0);
SetIsSummary (false);
SetName (null);
SetValue (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Campaign (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Campaign (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Campaign (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_Campaign()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514370965L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054176L;
/** AD_Table_ID=274 */
public static int Table_ID;
 // =274;

/** TableName=C_Campaign */
public static String Table_Name="C_Campaign";

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
StringBuilder sb = new StringBuilder ("X_C_Campaign[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Campaign.
@param C_Campaign_ID Marketing Campaign */
public void SetC_Campaign_ID (int C_Campaign_ID)
{
if (C_Campaign_ID < 1) throw new ArgumentException ("C_Campaign_ID is mandatory.");
Set_ValueNoCheck ("C_Campaign_ID", C_Campaign_ID);
}
/** Get Campaign.
@return Marketing Campaign */
public int GetC_Campaign_ID() 
{
Object ii = Get_Value("C_Campaign_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Channel.
@param C_Channel_ID Sales Channel */
public void SetC_Channel_ID (int C_Channel_ID)
{
if (C_Channel_ID <= 0) Set_Value ("C_Channel_ID", null);
else
Set_Value ("C_Channel_ID", C_Channel_ID);
}
/** Get Channel.
@return Sales Channel */
public int GetC_Channel_ID() 
{
Object ii = Get_Value("C_Channel_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Costs.
@param Costs Costs in accounting currency */
public void SetCosts (Decimal? Costs)
{
if (Costs == null) throw new ArgumentException ("Costs is mandatory.");
Set_Value ("Costs", (Decimal?)Costs);
}
/** Get Costs.
@return Costs in accounting currency */
public Decimal GetCosts() 
{
Object bd =Get_Value("Costs");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
/** Set End Date.
@param EndDate Last effective date (inclusive) */
public void SetEndDate (DateTime? EndDate)
{
Set_Value ("EndDate", (DateTime?)EndDate);
}
/** Get End Date.
@return Last effective date (inclusive) */
public DateTime? GetEndDate() 
{
return (DateTime?)Get_Value("EndDate");
}
/** Set Summary Level.
@param IsSummary This is a summary entity */
public void SetIsSummary (Boolean IsSummary)
{
Set_Value ("IsSummary", IsSummary);
}
/** Get Summary Level.
@return This is a summary entity */
public Boolean IsSummary() 
{
Object oo = Get_Value("IsSummary");
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
/** Set Start Date.
@param StartDate First effective day (inclusive) */
public void SetStartDate (DateTime? StartDate)
{
Set_Value ("StartDate", (DateTime?)StartDate);
}
/** Get Start Date.
@return First effective day (inclusive) */
public DateTime? GetStartDate() 
{
return (DateTime?)Get_Value("StartDate");
}
/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value)
{
if (Value == null) throw new ArgumentException ("Value is mandatory.");
if (Value.Length > 40)
{
log.Warning("Length > 40 - truncated");
Value = Value.Substring(0,40);
}
Set_Value ("Value", Value);
}
/** Get Search Key.
@return Search key for the record in the format required - must be unique */
public String GetValue() 
{
return (String)Get_Value("Value");
}

/** Set C_CampaignType_ID.
@param C_CampaignType_ID C_CampaignType_ID */
public void SetC_CampaignType_ID(int C_CampaignType_ID)
{
    if (C_CampaignType_ID <= 0) Set_Value("C_CampaignType_ID", null);
    else
        Set_Value("C_CampaignType_ID", C_CampaignType_ID);
}
/** Get C_CampaignType_ID.
@return C_CampaignType_ID */
public int GetC_CampaignType_ID()
{
    Object ii = Get_Value("C_CampaignType_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}

/** SalesRep_ID AD_Reference_ID=190 */
public static int SALESREP_ID_AD_Reference_ID = 190;
/** Set Owner.
@param SalesRep_ID Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
public void SetSalesRep_ID(int SalesRep_ID)
{
    if (SalesRep_ID <= 0) Set_Value("SalesRep_ID", null);
    else
        Set_Value("SalesRep_ID", SalesRep_ID);
}
/** Get Owner.
@return Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
public int GetSalesRep_ID()
{
    Object ii = Get_Value("SalesRep_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}

/** Set Create Campaign Plan.
@param GenerateProject Create Campaign Plan */
public void SetGenerateProject(String GenerateProject)
{
    if (GenerateProject != null && GenerateProject.Length > 50)
    {
        log.Warning("Length > 50 - truncated");
        GenerateProject = GenerateProject.Substring(0, 50);
    }
    Set_Value("GenerateProject", GenerateProject);
}
/** Get Create Campaign Plan.
@return Create Campaign Plan */
public String GetGenerateProject()
{
    return (String)Get_Value("GenerateProject");
}

/** Set Expected Customers.
@param ExpectedCustomers Expected Customers */
public void SetExpectedCustomers(int ExpectedCustomers)
{
    Set_Value("ExpectedCustomers", ExpectedCustomers);
}
/** Get Expected Customers.
@return Expected Customers */
public int GetExpectedCustomers()
{
    Object ii = Get_Value("ExpectedCustomers");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set Expected Leads.
@param ExpectedLeads Expected Leads */
public void SetExpectedLeads(int ExpectedLeads)
{
    Set_Value("ExpectedLeads", ExpectedLeads);
}
/** Get Expected Leads.
@return Expected Leads */
public int GetExpectedLeads()
{
    Object ii = Get_Value("ExpectedLeads");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set Expected Opportunity.
@param ExpectedOpportunity Expected Opportunity */
public void SetExpectedOpportunity(int ExpectedOpportunity)
{
    Set_Value("ExpectedOpportunity", ExpectedOpportunity);
}
/** Get Expected Opportunity.
@return Expected Opportunity */
public int GetExpectedOpportunity()
{
    Object ii = Get_Value("ExpectedOpportunity");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set Expected Prospects.
@param ExpectedProspects Expected Prospects */
public void SetExpectedProspects(int ExpectedProspects)
{
    Set_Value("ExpectedProspects", ExpectedProspects);
}
/** Get Expected Prospects.
@return Expected Prospects */
public int GetExpectedProspects()
{
    Object ii = Get_Value("ExpectedProspects");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set Expected Revenue.
@param ExpectedRevenue Expected Revenue */
public void SetExpectedRevenue(Decimal? ExpectedRevenue)
{
    Set_Value("ExpectedRevenue", (Decimal?)ExpectedRevenue);
}
/** Get Expected Revenue.
@return Expected Revenue */
public Decimal GetExpectedRevenue()
{
    Object bd = Get_Value("ExpectedRevenue");
    if (bd == null) return Env.ZERO;
    return Convert.ToDecimal(bd);
}
/** Set Approved.
@param IsApproved Indicates if this document requires approval */
public void SetIsApproved(Boolean IsApproved)
{
    Set_Value("IsApproved", IsApproved);
}
/** Get Approved.
@return Indicates if this document requires approval */
public Boolean IsApproved()
{
    Object oo = Get_Value("IsApproved");
    if (oo != null)
    {
        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        return "Y".Equals(oo);
    }
    return false;
}
/** Set Create Invitee List.
@param CreateInviteeList Create Invitee List */
public void SetCreateInviteeList(String CreateInviteeList)
{
    if (CreateInviteeList != null && CreateInviteeList.Length > 50)
    {
        log.Warning("Length > 50 - truncated");
        CreateInviteeList = CreateInviteeList.Substring(0, 50);
    }
    Set_Value("CreateInviteeList", CreateInviteeList);
}
/** Get Create Invitee List.
@return Create Invitee List */
public String GetCreateInviteeList()
{
    return (String)Get_Value("CreateInviteeList");
}
/** Set Actual Cost.
@param ActualCost Actual Cost */
public void SetActualCost(Decimal? ActualCost)
{
    throw new ArgumentException("ActualCost Is virtual column");
}
/** Get Actual Cost.
@return Actual Cost */
public Decimal GetActualCost()
{
    Object bd = Get_Value("ActualCost");
    if (bd == null) return Env.ZERO;
    return Convert.ToDecimal(bd);
}
/** Set Actual Customer.
@param ActualCustomer Actual Customer */
public void SetActualCustomer(int ActualCustomer)
{
    throw new ArgumentException("ActualCustomer Is virtual column");
}
/** Get Actual Customer.
@return Actual Customer */
public int GetActualCustomer()
{
    Object ii = Get_Value("ActualCustomer");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set Actual Leads.
@param ActualLeads Actual Leads */
public void SetActualLeads(int ActualLeads)
{
    throw new ArgumentException("ActualLeads Is virtual column");
}
/** Get Actual Leads.
@return Actual Leads */
public int GetActualLeads()
{
    Object ii = Get_Value("ActualLeads");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set Actual Opportunity.
@param ActualOpportunity Actual Opportunity */
public void SetActualOpportunity(int ActualOpportunity)
{
    throw new ArgumentException("ActualOpportunity Is virtual column");
}
/** Get Actual Opportunity.
@return Actual Opportunity */
public int GetActualOpportunity()
{
    Object ii = Get_Value("ActualOpportunity");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set ActualProspect.
@param ActualProspect ActualProspect */
public void SetActualProspect(int ActualProspect)
{
    throw new ArgumentException("ActualProspect Is virtual column");
}
/** Get ActualProspect.
@return ActualProspect */
public int GetActualProspect()
{
    Object ii = Get_Value("ActualProspect");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set Actual Revenue.
@param ActualRevenue Actual Revenue */
public void SetActualRevenue(Decimal? ActualRevenue)
{
    throw new ArgumentException("ActualRevenue Is virtual column");
}
/** Get Actual Revenue.
@return Actual Revenue */
public Decimal GetActualRevenue()
{
    Object bd = Get_Value("ActualRevenue");
    if (bd == null) return Env.ZERO;
    return Convert.ToDecimal(bd);
}
/** Set C_CampaignTemplate_ID.
@param C_CampaignTemplate_ID C_CampaignTemplate_ID */
public void SetC_CampaignTemplate_ID(int C_CampaignTemplate_ID)
{
    if (C_CampaignTemplate_ID <= 0) Set_Value("C_CampaignTemplate_ID", null);
    else
        Set_Value("C_CampaignTemplate_ID", C_CampaignTemplate_ID);
}
/** Get C_CampaignTemplate_ID.
@return C_CampaignTemplate_ID */
public int GetC_CampaignTemplate_ID()
{
    Object ii = Get_Value("C_CampaignTemplate_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}

}

}
