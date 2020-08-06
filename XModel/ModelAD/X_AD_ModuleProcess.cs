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
/** Generated Model for AD_ModuleProcess
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_ModuleProcess : PO
{
public X_AD_ModuleProcess (Context ctx, int AD_ModuleProcess_ID, Trx trxName) : base (ctx, AD_ModuleProcess_ID, trxName)
{
/** if (AD_ModuleProcess_ID == 0)
{
SetAD_ModuleInfo_ID (0);
SetAD_ModuleProcess_ID (0);
}
 */
}
public X_AD_ModuleProcess (Ctx ctx, int AD_ModuleProcess_ID, Trx trxName) : base (ctx, AD_ModuleProcess_ID, trxName)
{
/** if (AD_ModuleProcess_ID == 0)
{
SetAD_ModuleInfo_ID (0);
SetAD_ModuleProcess_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ModuleProcess (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ModuleProcess (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ModuleProcess (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_ModuleProcess()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27622811907283L;
/** Last Updated Timestamp 6/26/2012 10:26:30 AM */
public static long updatedMS = 1340686590494L;
/** AD_Table_ID=1000057 */
public static int Table_ID;
 // =1000057;

/** TableName=AD_ModuleProcess */
public static String Table_Name="AD_ModuleProcess";

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
StringBuilder sb = new StringBuilder ("X_AD_ModuleProcess[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Module.
@param AD_ModuleInfo_ID Module */
public void SetAD_ModuleInfo_ID (int AD_ModuleInfo_ID)
{
if (AD_ModuleInfo_ID < 1) throw new ArgumentException ("AD_ModuleInfo_ID is mandatory.");
Set_ValueNoCheck ("AD_ModuleInfo_ID", AD_ModuleInfo_ID);
}
/** Get Module.
@return Module */
public int GetAD_ModuleInfo_ID() 
{
Object ii = Get_Value("AD_ModuleInfo_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set AD_ModuleProcess_ID.
@param AD_ModuleProcess_ID AD_ModuleProcess_ID */
public void SetAD_ModuleProcess_ID (int AD_ModuleProcess_ID)
{
if (AD_ModuleProcess_ID < 1) throw new ArgumentException ("AD_ModuleProcess_ID is mandatory.");
Set_ValueNoCheck ("AD_ModuleProcess_ID", AD_ModuleProcess_ID);
}
/** Get AD_ModuleProcess_ID.
@return AD_ModuleProcess_ID */
public int GetAD_ModuleProcess_ID() 
{
Object ii = Get_Value("AD_ModuleProcess_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Process.
@param AD_Process_ID Process or Report */
public void SetAD_Process_ID (int AD_Process_ID)
{
if (AD_Process_ID <= 0) Set_Value ("AD_Process_ID", null);
else
Set_Value ("AD_Process_ID", AD_Process_ID);
}
/** Get Process.
@return Process or Report */
public int GetAD_Process_ID() 
{
Object ii = Get_Value("AD_Process_ID");
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
    /** Generated Model for AD_ModuleTable
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_ModuleTable : PO
    {
        public X_AD_ModuleTable(Context ctx, int AD_ModuleTable_ID, Trx trxName)
            : base(ctx, AD_ModuleTable_ID, trxName)
        {
            /** if (AD_ModuleTable_ID == 0)
            {
            SetAD_ModuleInfo_ID (0);
            SetAD_ModuleTable_ID (0);
            }
             */
        }
        public X_AD_ModuleTable(Ctx ctx, int AD_ModuleTable_ID, Trx trxName)
            : base(ctx, AD_ModuleTable_ID, trxName)
        {
            /** if (AD_ModuleTable_ID == 0)
            {
            SetAD_ModuleInfo_ID (0);
            SetAD_ModuleTable_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_ModuleTable(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_ModuleTable(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_ModuleTable(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_AD_ModuleTable()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID = 27624292225707L;
        /** Last Updated Timestamp 7/13/2012 1:38:28 PM */
        public static long updatedMS = 1342166908918L;
        /** AD_Table_ID=1000063 */
        public static int Table_ID;
        // =1000063;

        /** TableName=AD_ModuleTable */
        public static String Table_Name = "AD_ModuleTable";

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
            StringBuilder sb = new StringBuilder("X_AD_ModuleTable[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Module.
        @param AD_ModuleInfo_ID Module */
        public void SetAD_ModuleInfo_ID(int AD_ModuleInfo_ID)
        {
            if (AD_ModuleInfo_ID < 1) throw new ArgumentException("AD_ModuleInfo_ID is mandatory.");
            Set_ValueNoCheck("AD_ModuleInfo_ID", AD_ModuleInfo_ID);
        }
        /** Get Module.
        @return Module */
        public int GetAD_ModuleInfo_ID()
        {
            Object ii = Get_Value("AD_ModuleInfo_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set AD_ModuleTable_ID.
        @param AD_ModuleTable_ID AD_ModuleTable_ID */
        public void SetAD_ModuleTable_ID(int AD_ModuleTable_ID)
        {
            if (AD_ModuleTable_ID < 1) throw new ArgumentException("AD_ModuleTable_ID is mandatory.");
            Set_ValueNoCheck("AD_ModuleTable_ID", AD_ModuleTable_ID);
        }
        /** Get AD_ModuleTable_ID.
        @return AD_ModuleTable_ID */
        public int GetAD_ModuleTable_ID()
        {
            Object ii = Get_Value("AD_ModuleTable_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Table.
        @param AD_Table_ID Database Table information */
        public void SetAD_Table_ID(int AD_Table_ID)
        {
            if (AD_Table_ID <= 0) Set_Value("AD_Table_ID", null);
            else
                Set_Value("AD_Table_ID", AD_Table_ID);
        }
        /** Get Table.
        @return Database Table information */
        public int GetAD_Table_ID()
        {
            Object ii = Get_Value("AD_Table_ID");
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


