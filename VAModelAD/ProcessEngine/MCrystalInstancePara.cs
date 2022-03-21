using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.ProcessEngine
{
    public class MCrystalInstancePara : X_AD_CrystalInstance_Para
    {
        /// <summary>
        /// Persistency Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ignored">no trans</param>
        /// <param name="txtName">trx name</param>
        public MCrystalInstancePara(Ctx ctx, int ignored, Trx txtName)
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
        public MCrystalInstancePara(Ctx ctx, int AD_CrystalInstace_ID, int SeqNo)
            : base(ctx, 0, null)
        {
            SetAD_CrystalInstance_ID(AD_CrystalInstace_ID);
            SetSeqNo(SeqNo);
        }

                /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="instance">intance</param>
        /// <param name="SeqNo"></param>
        public MCrystalInstancePara(MCrystalInstance instance, int SeqNo)
            : base(instance.GetCtx(), 0, instance.Get_TrxName())
        {
            SetAD_CrystalInstance_ID(instance.GetAD_CrystalInstance_ID());
            SetSeqNo(SeqNo);
        }

                /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="txtName"></param>
        public MCrystalInstancePara(Ctx ctx, DataRow dr, Trx txtName)
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
