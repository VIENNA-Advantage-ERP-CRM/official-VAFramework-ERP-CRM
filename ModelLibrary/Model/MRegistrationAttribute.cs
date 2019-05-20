/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRegistrationAttribute
 * Purpose        : Asset Registration Attribute
 * Class Used     : X_A_RegistrationAttribute
 * Chronological    Development
 * Deepak           03-feb-2010
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
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Data.SqlClient;
namespace VAdvantage.Model
{
    public class MRegistrationAttribute : X_A_RegistrationAttribute
    {
   /// <summary>
   ///  Get All Asset Registration Attributes (not cached).	Refreshes Cache for direct addess
   /// </summary>
   /// <param name="ctx">context</param>
   /// <returns>array of Registration Attributes</returns>
	public static MRegistrationAttribute[] GetAll(Ctx ctx)
	{
		//	Store/Refresh Cache and add to List
		List<MRegistrationAttribute> list = new List<MRegistrationAttribute>();
		String sql = "SELECT * FROM A_RegistrationAttribute "
			+ "WHERE AD_Client_ID=@param "
			+ "ORDER BY SeqNo";
		int AD_Client_ID = ctx.GetAD_Client_ID();
		SqlParameter[] param=new SqlParameter[1];
        IDataReader idr=null;
		try
		{
			//pstmt = DataBase.prepareStatement(sql, null);
			//pstmt.setInt(1, AD_Client_ID);
            param[0]=new SqlParameter("@param",AD_Client_ID);
			idr=DataBase.DB.ExecuteReader(sql,param,null);
			while (idr.Read())
			{
				MRegistrationAttribute value = new MRegistrationAttribute(ctx,idr, null);
				int key = Utility.Util.GetValueOfInt(value.GetA_RegistrationAttribute_ID());
				//s_cache.put(key, value);
                _cache.Add(key, value);
				list.Add(value);
			}
			idr.Close();
		}
		catch (Exception e)
		{
            if(idr!=null)
            {
                idr.Close();
            }
			_log.Log(Level.SEVERE, sql, e);
		}
	
		//
		MRegistrationAttribute[] retValue = new MRegistrationAttribute[list.Count];
		retValue=list.ToArray();
		return retValue;
	}	//	getAll

	/// <summary>
	/// Get Registration Attribute (cached)
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="A_RegistrationAttribute_ID">id</param>
	/// <param name="trxName">trx</param>
	/// <returns>Registration Attribute</returns>
	public static MRegistrationAttribute Get(Ctx ctx, int A_RegistrationAttribute_ID, Trx trxName)
	{
		int key = Utility.Util.GetValueOfInt(A_RegistrationAttribute_ID);
		MRegistrationAttribute retValue = (MRegistrationAttribute)_cache[key];// .get(key);
		if (retValue == null)
		{
			retValue = new MRegistrationAttribute (ctx, A_RegistrationAttribute_ID, trxName);
			_cache.Add(key, retValue);
		}
		return retValue;
	}	//	getAll

	/** Static Logger					*/
	private static VLogger _log = VLogger.GetVLogger(typeof(MRegistrationAttribute).FullName);//.class);
	/**	Cache						*/
	private static CCache<int,MRegistrationAttribute> _cache 
		= new CCache<int,MRegistrationAttribute>("A_RegistrationAttribute", 20);

	/// <summary>
	/// Standard Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="A_RegistrationAttribute_ID">id</param>
	/// <param name="trxName">trx</param>
	public MRegistrationAttribute (Ctx ctx, int A_RegistrationAttribute_ID, Trx trxName):base(ctx, A_RegistrationAttribute_ID, trxName)
	{
		
	}	
    /// <summary>
    /// Load Constructor
    /**
     * 	
     *	@param ctx context
     *	@param rs result set
     */
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="rs">datarow</param>
    /// <param name="trxName">trx</param>
	public MRegistrationAttribute (Ctx ctx,DataRow dr, Trx trxName):base(ctx, dr, trxName)
	{
		
	}
    public MRegistrationAttribute(Ctx ctx,IDataReader idr, Trx trxName)
        : base(ctx, idr, trxName)
    { }

}	

}
