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
        int GetVAF_Job_ID();
        int GetVAF_TableView_ID();
        int GetRecord_ID();
        int GetVAB_BusinessPartner_ID();
    }
}
