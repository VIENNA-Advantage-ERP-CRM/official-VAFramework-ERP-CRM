/* Sample Classs */
using ModelLibrary.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Classes;
using VAdvantage.Model;

namespace ModelLibrary.Classes
{

    /// <summary>
    /// Custome Model Validator class 
    /// used to perform additional check 
    /// and inject new Model class to override base functionality
    /// </summary>
    class MyModelValidator : ModelValidator
    {
        int AD_Client_ID = 0;
       

        public int GetAD_Client_ID()
        {
            return AD_Client_ID;
        }

      /// <summary>
      /// Initlilize model and doc validation for tables
      /// </summary>
      /// <param name="engine"></param>
      /// <param name="ClientId"></param>
        public void Initialize(ModelValidationEngine engine, int ClientId)
        {
            AD_Client_ID = ClientId;

            engine.AddDocValidate("C_Order", this);
            engine.AddDocValidate("C_Invoice", this); //aditional check

            engine.AddModelChange(X_C_Order.Table_Name, this);
            engine.AddModelChange(X_C_Invoice.Table_Name, this);

            engine.AddModelAction(X_C_Order.Table_Name, this);
        }

        /// <summary>
        /// initialize model action class to override base function(optional)
        /// </summary>
        /// <param name="po"></param>
        public void InitializeModelAction(PO po)
        {
            if (po is MOrder)
                po.ModelAction = new MMOrder(po);
        }

        public string Login(int AD_Org_ID, int AD_Role_ID, int AD_UserContact_ID)
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
                if (ModalValidatorVariables.CHANGETYPE_CHANGE == changeType)
                {
                    MOrder ord = (MOrder)po;
                    return "error ";
                }
                if (ModalValidatorVariables.CHANGETYPE_DELETE == changeType)
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

        //public bool UpdateInfoColumns(List<Info_Column> columns, StringBuilder sqlFrom, StringBuilder sqlOrder)
        //{
        //    throw new NotImplementedException();
        //}
    }
  
}
