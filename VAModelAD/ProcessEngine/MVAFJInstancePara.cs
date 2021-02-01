/********************************************************
 * Module Name    : Process
 * Purpose        : 
 * Class Used     : X_VAF_Job_Rights
 * Chronological Development
 * Jagmohan Bhatt    12-May-2009
 ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Model;
using VAdvantage.Utility;

using VAdvantage.ProcessEngine;
namespace VAdvantage.ProcessEngine
{
    /// <summary>
    /// Process Instance Parameter Model
    /// </summary>
    public class MVAFJInstancePara : X_VAF_JInstance_Para
    {
        /// <summary>
        /// Persistency Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ignored">no trans</param>
        /// <param name="txtName">trx name</param>
        public MVAFJInstancePara(Ctx ctx, int ignored, Trx txtName)
            : base(ctx, 0, txtName)
        {
            if (ignored != 0)
                throw new ArgumentException("Multi-Key");
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_PInstace_ID">instance id</param>
        /// <param name="SeqNo">sql no</param>
        public MVAFJInstancePara(Ctx ctx, int AD_PInstace_ID, int SeqNo)
            : base(ctx, 0, null)
        {
            SetVAF_JInstance_ID(AD_PInstace_ID);
            SetSeqNo(SeqNo);
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="instance">intance</param>
        /// <param name="SeqNo"></param>
        public MVAFJInstancePara(MVAFJInstance instance, int SeqNo)
            : base(instance.GetCtx(), 0, instance.Get_Trx())
        {
            SetVAF_JInstance_ID(instance.GetVAF_JInstance_ID());
            SetSeqNo(SeqNo);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="txtName"></param>
        public MVAFJInstancePara(Ctx ctx, DataRow dr, Trx txtName)
            : base(ctx, dr, txtName)
        {
        }

        public void SetP_Number(int P_Number)
        {
            base.SetP_Number(P_Number);
        }

        public void SetP_Number_To(int P_Number_To)
        {
            base.SetP_Number_To(P_Number_To);
        }

        public void setParameter(string parameterName, string stringParameter)
        {
            SetParameterName(parameterName);
            SetP_String(stringParameter);
        }

        public void setParameter(string parameterName, decimal bdParameter)
        {
            SetParameterName(parameterName);
            SetP_Number((decimal)bdParameter);
        }

        public void setParameter(string parameterName, int iParameter)
        {
            SetParameterName(parameterName);
            SetP_Number((int)iParameter);
        }


    }
}
