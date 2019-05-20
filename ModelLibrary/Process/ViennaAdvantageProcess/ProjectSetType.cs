/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ProjectSetType
 * Purpose        : Set Project Type
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           07-Dec-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;

namespace ViennaAdvantage.Process
{
    public class ProjectSetType:VAdvantage.ProcessEngine.SvrProcess
    {
        /**	Project directly from Project	*/
        private int				_C_Project_ID = 0;
        /** Project Type Parameter			*/
        private int				_C_ProjectType_ID = 0;

        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    continue;
                }
                else if (name.Equals("C_ProjectType_ID"))
                {
                    //_C_ProjectType_ID = ((Decimal)para[i].GetParameter()).intValue();
                    _C_ProjectType_ID = Util.GetValueOfInt(Util.GetValueOfDecimal(para[i].GetParameter()));
                }
                else
                {
                    log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
                }
              }
        }	//	prepare

        /// <summary>
        /// Perrform Process.
        /// </summary>
        /// <returns>Message (clear text)</returns>
        protected override String DoIt()
        {
            _C_Project_ID = GetRecord_ID();
            log.Info("doIt - C_Project_ID=" + _C_Project_ID + ", C_ProjectType_ID=" + _C_ProjectType_ID);
            //
            MProject project = new MProject (GetCtx(), _C_Project_ID, Get_Trx());
            if (project.GetC_Project_ID() == 0 || project.GetC_Project_ID() != _C_Project_ID)
            {
                throw new ArgumentException("Project not found C_Project_ID=" + _C_Project_ID);
            }

            if (project.GetC_ProjectType_ID_Int() > 0)
            {
                throw new ArgumentException("Project already has Type (Cannot overwrite) " + project.GetC_ProjectType_ID());
            }
            //
            MProjectType type = new MProjectType (GetCtx(), _C_ProjectType_ID, Get_Trx());
            if (type.GetC_ProjectType_ID() == 0 || type.GetC_ProjectType_ID() != _C_ProjectType_ID)
            {
                throw new ArgumentException("Project Type not found C_ProjectType_ID=" + _C_ProjectType_ID);
            }
            //	Set & Copy if Service
            project.SetProjectType(type);
            if (!project.Save())
            {
                throw new Exception("@Error@");
            }
            //
            return "@OK@";
        }	//	doIt

}	//	ProjectSetType

}
