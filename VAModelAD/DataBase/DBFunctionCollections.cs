using CoreLibrary.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Model;

namespace VAModelAD.DataBase
{
    public class DBFunctionCollections
    {

        public static string GetObscureColumn(string obscureType, string tableName, string columnName)
        {
            if (DB.IsOracle())
            {
                if (obscureType.Equals(X_VAF_Column.OBSCURETYPE_ObscureDigitsButLast4))
                {
                    return " REGEXP_REPLACE(SUBSTR(" + tableName + "." + columnName + ",0,LENGTH(" + tableName + "." + columnName + ")-4) ,'[[:digit:]]','*') || SUBSTR(" + tableName + "." + columnName + ",LENGTH(" + tableName + "." + columnName + ")-3) ";
                }
                else if (obscureType.Equals(X_VAF_Column.OBSCURETYPE_ObscureDigitsButFirstLast4))
                {
                    return "SUBSTR(" + tableName + "." + columnName + ",0,4) || REGEXP_REPLACE(SUBSTR(" + tableName + "." + columnName + ",4,LENGTH(" + tableName + "." + columnName + ")-8) ,'[[:digit:]]','*') || SUBSTR(" + tableName + "." + columnName + ",LENGTH(" + tableName + "." + columnName + ")-3)";
                }
                else if (obscureType.Equals(X_VAF_Column.OBSCURETYPE_ObscureAlphaNumericButLast4))
                {
                    return " REGEXP_REPLACE(SUBSTR(" + tableName + "." + columnName + ",0,LENGTH(" + tableName + "." + columnName + ")-4) ,'[[:digit:]]|[[:alpha:]]|[^A-Z0-9 ]|[[:space:]]','*') || SUBSTR(" + tableName + "." + columnName + ",LENGTH(" + tableName + "." + columnName + ")-3) ";
                }
                else if (obscureType.Equals(X_VAF_Column.OBSCURETYPE_ObscureAlphaNumericButFirstLast4))
                {
                    return "SUBSTR(" + tableName + "." + columnName + ",0,4) || REGEXP_REPLACE(SUBSTR(" + tableName + "." + columnName + ",4,LENGTH(" + tableName + "." + columnName + ")-8) ,'[[:digit:]]|[[:alpha:]]|[^A-Z0-9 ]|[[:space:]]','*') || SUBSTR(" + tableName + "." + columnName + ",LENGTH(" + tableName + "." + columnName + ")-3)";
                }
            }
            else if (DB.IsPostgreSQL())
            {
                if (obscureType.Equals(X_VAF_Column.OBSCURETYPE_ObscureDigitsButLast4))
                {
                    return " Case when LENGTH(" + tableName + "." + columnName + ") > 4 then REGEXP_REPLACE(SUBSTR(" + tableName + "." + columnName + ",0,LENGTH(" + tableName + "." + columnName + ")-3) ,'[[:digit:]]','*','g') || SUBSTR(" + tableName + "." + columnName + ",LENGTH(" + tableName + "." + columnName + ")-3) when LENGTH(" + tableName + "." + columnName + ")<=4 then " + tableName + "." + columnName + " END ";
                }
                else if (obscureType.Equals(X_VAF_Column.OBSCURETYPE_ObscureDigitsButFirstLast4))
                {
                    return " Case when LENGTH(" + tableName + "." + columnName + ") > 8 then SUBSTR(" + tableName + "." + columnName + ",0,5) || REGEXP_REPLACE(SUBSTR(" + tableName + "." + columnName + ",4,LENGTH(" + tableName + "." + columnName + ")-8) ,'[[:digit:]]','*','g') || SUBSTR(" + tableName + "." + columnName + ",LENGTH(" + tableName + "." + columnName + ")-3)  when LENGTH(" + tableName + "." + columnName + ")<=8 then " + tableName + "." + columnName + " END ";
                }
                else if (obscureType.Equals(X_VAF_Column.OBSCURETYPE_ObscureAlphaNumericButLast4))
                {
                    return " Case when LENGTH(" + tableName + "." + columnName + ") > 4 then REGEXP_REPLACE(SUBSTR(" + tableName + "." + columnName + ",0,LENGTH(" + tableName + "." + columnName + ")-3) ,'[[:digit:]]|[[:alpha:]]|[^A-Z0-9 ]|[[:space:]]','*','g') || SUBSTR(" + tableName + "." + columnName + ",LENGTH(" + tableName + "." + columnName + ")-3)  when LENGTH(" + tableName + "." + columnName + ")<=4 then " + tableName + "." + columnName + " END ";
                }
                else if (obscureType.Equals(X_VAF_Column.OBSCURETYPE_ObscureAlphaNumericButFirstLast4))
                {
                    return " Case when LENGTH(" + tableName + "." + columnName + ") > 8 then SUBSTR(" + tableName + "." + columnName + ",0,5) || REGEXP_REPLACE(SUBSTR(" + tableName + "." + columnName + ",4,LENGTH(" + tableName + "." + columnName + ")-8) ,'[[:digit:]]|[[:alpha:]]|[^A-Z0-9 ]|[[:space:]]','*','g') || SUBSTR(" + tableName + "." + columnName + ",LENGTH(" + tableName + "." + columnName + ")-3)  when LENGTH(" + tableName + "." + columnName + ")<=8 then " + tableName + "." + columnName + " END ";
                }

            }

            return tableName + "." + columnName;
        }

    }


}
