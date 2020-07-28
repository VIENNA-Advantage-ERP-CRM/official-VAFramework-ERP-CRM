/********************************************************
// Module Name    : Run Time Show Window
// Purpose        : Query Descriptor.
                    Maintains QueryRestrictions (WHERE clause)
// Class Used     : Ctx.cs
// Created By     : Harwinder 
// Date           : 
**********************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using VAdvantage.Model;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Logging;
using VAdvantage.Login;

namespace VAdvantage.Classes
{
    /****************************************************************************
      *     
      *        Query Class
      * 
     ******************************************************************************/
    public interface IQuery
    {

        int GetRecordCount();

        int GetRestrictionCount();

        String GetTableName();
        String GetWhereClause(bool fullyQualified);

        String GetWhereClause(int index);
        String GetWhereClause();
    }
}

   