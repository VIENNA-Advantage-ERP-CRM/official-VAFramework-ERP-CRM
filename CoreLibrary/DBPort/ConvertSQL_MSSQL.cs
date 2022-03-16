using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.DBPort
{
    class ConvertSQL_MSSQL :ConvertSQL_SQL92
    {
        protected override List<string> ConvertStatement(string sqlStatement)
        {
            throw new NotImplementedException();
        }
    }
}
