/********************************************************
// Module Name    : Run Time Show Window
// Purpose        : Query Descriptor.
                    Maintains QueryRestrictions (WHERE clause)
// Class Used     : Ctx.cs
// Created By     : Harwinder 
// Date           : 
**********************************************************/

using System;


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