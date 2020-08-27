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
        int AD_Client_ID = 0;
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
            else if (po is MInvoice)
            {
            }
            return "";
        }

        public int GetAD_Client_ID()
        {
            return AD_Client_ID;
        }

        public void Initialize(ModelValidationEngine engine, int ClientId)
        {
            AD_Client_ID = ClientId;
            engine.AddDocValidate("C_Order", this);
            engine.AddDocValidate("C_Invoice", this);
            engine.AddModelChange(X_C_Order.Table_Name, this);
            engine.AddModelChange(X_C_Invoice.Table_Name, this);
        }

        public string Login(int AD_Org_ID, int AD_Role_ID, int AD_User_ID)
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
            else if (po is MInvoice)
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