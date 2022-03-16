/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : DocWorkflowMgr
 * Purpose        : 
 * Chronological    Development
 * Raghunandan      06-May-2009 
  ******************************************************/

using VAdvantage.Model;


namespace VAdvantage.WF
{
    public interface DocWorkflowMgr
    {

        /// <summary>
        ///Process Document Value Workflow
        /// </summary>
        /// <param name="document">document</param>
        /// <param name="AD_Table_ID">table</param>
        /// <returns> true if WF started</returns>
        bool Process(PO document, int AD_Table_ID);
    }
}
