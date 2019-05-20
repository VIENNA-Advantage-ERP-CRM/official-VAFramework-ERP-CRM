using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.DBPort
{
    class ConvertSQL_Oracle : ConvertSQL
    {
        public ConvertSQL_Oracle() {
            m_isOracle = true;
        }

        protected override List<string> ConvertStatement(string sqlStatement)
        {
            List<String> result = new List<String>();
            result.Add(sqlStatement);
            return result;
        }

        
    }
}
