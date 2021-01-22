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
/** Generated Model for VAF_ModuleProcess
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_ModuleProcess : PO
{
public X_VAF_ModuleProcess (Context ctx, int VAF_ModuleProcess_ID, Trx trxName) : base (ctx, VAF_ModuleProcess_ID, trxName)
{
/** if (VAF_ModuleProcess_ID == 0)
{
SetVAF_ModuleInfo_ID (0);
SetVAF_ModuleProcess_ID (0);
}
 */
}
public X_VAF_ModuleProcess (Ctx ctx, int VAF_ModuleProcess_ID, Trx trxName) : base (ctx, VAF_ModuleProcess_ID, trxName)
{
/** if (VAF_ModuleProcess_ID == 0)
{
SetVAF_ModuleInfo_ID (0);
SetVAF_ModuleProcess_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleProcess (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleProcess (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleProcess (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_ModuleProcess()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27622811907283L;
/** Last Updated Timestamp 6/26/2012 10:26:30 AM */
public static long updatedMS = 1340686590494L;
/** VAF_TableView_ID=1000057 */
public static int Table_ID;
 // =1000057;

/** TableName=VAF_ModuleProcess */
public static String Table_Name="VAF_ModuleProcess";

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
StringBuilder sb = new StringBuilder ("X_VAF_ModuleProcess[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Module.
@param VAF_ModuleInfo_ID Module */
public void SetVAF_ModuleInfo_ID (int VAF_ModuleInfo_ID)
{
if (VAF_ModuleInfo_ID < 1) throw new ArgumentException ("VAF_ModuleInfo_ID is mandatory.");
Set_ValueNoCheck ("VAF_ModuleInfo_ID", VAF_ModuleInfo_ID);
}
/** Get Module.
@return Module */
public int GetVAF_ModuleInfo_ID() 
{
Object ii = Get_Value("VAF_ModuleInfo_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAF_ModuleProcess_ID.
@param VAF_ModuleProcess_ID VAF_ModuleProcess_ID */
public void SetVAF_ModuleProcess_ID (int VAF_ModuleProcess_ID)
{
if (VAF_ModuleProcess_ID < 1) throw new ArgumentException ("VAF_ModuleProcess_ID is mandatory.");
Set_ValueNoCheck ("VAF_ModuleProcess_ID", VAF_ModuleProcess_ID);
}
/** Get VAF_ModuleProcess_ID.
@return VAF_ModuleProcess_ID */
public int GetVAF_ModuleProcess_ID() 
{
Object ii = Get_Value("VAF_ModuleProcess_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Process.
@param VAF_Job_ID Process or Report */
public void SetVAF_Job_ID (int VAF_Job_ID)
{
if (VAF_Job_ID <= 0) Set_Value ("VAF_Job_ID", null);
else
Set_Value ("VAF_Job_ID", VAF_Job_ID);
}
/** Get Process.
@return Process or Report */
public int GetVAF_Job_ID() 
{
Object ii = Get_Value("VAF_Job_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 200)
{
log.Warning("Length > 200 - truncated");
Description = Description.Substring(0,200);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
}

}

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
    /** Generated Model for VAF_ModuleTable
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAF_ModuleTable : PO
    {
        public X_VAF_ModuleTable(Context ctx, int VAF_ModuleTable_ID, Trx trxName)
            : base(ctx, VAF_ModuleTable_ID, trxName)
        {
            /** if (VAF_ModuleTable_ID == 0)
            {
            SetVAF_ModuleInfo_ID (0);
            SetVAF_ModuleTable_ID (0);
            }
             */
        }
        public X_VAF_ModuleTable(Ctx ctx, int VAF_ModuleTable_ID, Trx trxName)
            : base(ctx, VAF_ModuleTable_ID, trxName)
        {
            /** if (VAF_ModuleTable_ID == 0)
            {
            SetVAF_ModuleInfo_ID (0);
            SetVAF_ModuleTable_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_ModuleTable(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_ModuleTable(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_ModuleTable(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAF_ModuleTable()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID = 27624292225707L;
        /** Last Updated Timestamp 7/13/2012 1:38:28 PM */
        public static long updatedMS = 1342166908918L;
        /** VAF_TableView_ID=1000063 */
        public static int Table_ID;
        // =1000063;

        /** TableName=VAF_ModuleTable */
        public static String Table_Name = "VAF_ModuleTable";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(4);
        /** AccessLevel
        @return 4 - System 
        */
        protected override int Get_AccessLevel()
        {
            return Convert.ToInt32(accessLevel.ToString());
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Context ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_VAF_ModuleTable[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Module.
        @param VAF_ModuleInfo_ID Module */
        public void SetVAF_ModuleInfo_ID(int VAF_ModuleInfo_ID)
        {
            if (VAF_ModuleInfo_ID < 1) throw new ArgumentException("VAF_ModuleInfo_ID is mandatory.");
            Set_ValueNoCheck("VAF_ModuleInfo_ID", VAF_ModuleInfo_ID);
        }
        /** Get Module.
        @return Module */
        public int GetVAF_ModuleInfo_ID()
        {
            Object ii = Get_Value("VAF_ModuleInfo_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set VAF_ModuleTable_ID.
        @param VAF_ModuleTable_ID VAF_ModuleTable_ID */
        public void SetVAF_ModuleTable_ID(int VAF_ModuleTable_ID)
        {
            if (VAF_ModuleTable_ID < 1) throw new ArgumentException("VAF_ModuleTable_ID is mandatory.");
            Set_ValueNoCheck("VAF_ModuleTable_ID", VAF_ModuleTable_ID);
        }
        /** Get VAF_ModuleTable_ID.
        @return VAF_ModuleTable_ID */
        public int GetVAF_ModuleTable_ID()
        {
            Object ii = Get_Value("VAF_ModuleTable_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Table.
        @param VAF_TableView_ID Database Table information */
        public void SetVAF_TableView_ID(int VAF_TableView_ID)
        {
            if (VAF_TableView_ID <= 0) Set_Value("VAF_TableView_ID", null);
            else
                Set_Value("VAF_TableView_ID", VAF_TableView_ID);
        }
        /** Get Table.
        @return Database Table information */
        public int GetVAF_TableView_ID()
        {
            Object ii = Get_Value("VAF_TableView_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Export.
        @param Export_ID Export */
        public void SetExport_ID(String Export_ID)
        {
            if (Export_ID != null && Export_ID.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Export_ID = Export_ID.Substring(0, 50);
            }
            Set_ValueNoCheck("Export_ID", Export_ID);
        }
        /** Get Export.
        @return Export */
        public String GetExport_ID()
        {
            return (String)Get_Value("Export_ID");
        }
    }

}


