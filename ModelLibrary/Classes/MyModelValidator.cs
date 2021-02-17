/* Sample Classs 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Classes;
using VAdvantage.Model;

namespace ViennaAdvantageSvc.Classes
{

    class MyModelValidator : ModelValidator
    {
        int VAF_Client_ID = 0;
        public string DocValidate(PO po, int docTiming)
        {
            if (po is MOrder)//Morder
            {
                if (docTiming == ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE)
                {
                    //fail
                }
                if (docTiming == ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE)
                {
                }
            }
            else if (po is MVABInvoice)
            {
            }
            return "";
        }

        public int GetVAF_Client_ID()
        {
            return VAF_Client_ID;
        }

        public void Initialize(ModelValidationEngine engine, int ClientId)
        {
            VAF_Client_ID = ClientId;
            engine.AddDocValidate("VAB_Order", this);
            engine.AddDocValidate("VAB_Invoice", this);
            engine.AddModelChange(X_VAB_Order.Table_Name, this);
            engine.AddModelChange(X_VAB_Invoice.Table_Name, this);
        }

        public string Login(int VAF_Org_ID, int VAF_Role_ID, int VAF_UserContact_ID)
        {
            return "";
        }

        public string ModelChange(PO po, int changeType)
        {
            if (po is MOrder)
            {
                if (ModalValidatorVariables.CHANGETYPE_NEW == changeType)
                {
                    MOrder ord = (MOrder)po;
                    return "error ";
                }
            }
            else if (po is MVABInvoice)
            { 

            }
        return "";
    }

            public bool UpdateInfoColumns(List<Info_Column> columns, StringBuilder sqlFrom, StringBuilder sqlOrder)
            {
                throw new NotImplementedException();
            }
        }
  
}
*/