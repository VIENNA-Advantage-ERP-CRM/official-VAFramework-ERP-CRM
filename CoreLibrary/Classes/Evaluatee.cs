using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAdvantage.Classes
{
    public interface Evaluatee
    {
        /// <summary>
        /// Get Variable Value
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        string GetValueAsString(string variableName);
    }
}
