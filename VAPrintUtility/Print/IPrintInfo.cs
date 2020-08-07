using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAdvantage.Print
{
    public interface IPrintInfo
    {
        string GetName();
        bool IsReport();
        //
        int GetAD_Process_ID();
        int GetAD_Table_ID();
        int GetRecord_ID();
        int GetC_BPartner_ID();
    }
}
