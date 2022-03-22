/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAccessProfile
 * Purpose        : Media Access Profile
 * Class Used     : X_CM_AccessProfile
 * Chronological    Development
 * Deepak           12-Feb-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MAccessProfile : X_CM_AccessProfile
    {
        /// <summary>
        /// Access to Container
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="CM_Container_ID">id</param>
        /// <param name="AD_User_ID">id</param>
        /// <returns>ture if access container</returns>
        public static bool IsAccessContainer(Ctx ctx, int CM_Container_ID,
            int AD_User_ID)
        {
            //	NIT
            return true;
        }	//	isAccessContainer

        /// <summary>
        /// Access to Container
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="CM_Container_ID">id</param>
        /// <param name="AD_Role_ID"> 0 or role check</param>
        /// <param name="C_BPGroup_ID"> 0 or bpartner to check</param>
        /// <returns>true if access to container</returns>
        public static bool IsAccessContainer(Ctx ctx, int CM_Container_ID,
            int AD_Role_ID, int C_BPGroup_ID)
        {
            //	NIT
            return true;
        }	//	isAccessContainer

        /// <summary>
        ///	Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="CM_AccessProfile_ID">id</param>
        /// <param name="trxName">transation</param>
        public MAccessProfile(Ctx ctx, int CM_AccessProfile_ID,
            Trx trxName)
            : base(ctx, CM_AccessProfile_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">trx</param>
        public MAccessProfile(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }	//	MAccessProfile
        public MAccessProfile(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        { }

    }	//	MAccessProfile
}
