/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MDistribution
 * Purpose        : GL Distribution model.
 * Class Used     : X_VAGL_Distribution class
 * Chronological    Development
 * Deepak           19-Nov-2009
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
    public class MDistribution : X_VAGL_Distribution
    {

        /**	Static Logger	*/
	private static VLogger	_log	= VLogger.GetVLogger (typeof(MDistribution).FullName);
  /// <summary>
  /// 	Get Distribution for combination
  /// </summary>
  /// <param name="acct">account (ValidCombination)</param>
  /// <param name="PostingType">only posting type</param>
  /// <param name="VAB_DocTypes_ID">only document type</param>
    /// <returns>array of distributions</returns>
	public static MDistribution[] Get (MVABAccount acct,  
		String PostingType, int VAB_DocTypes_ID)
	{
		return Get (acct.GetCtx(), acct.GetVAB_AccountBook_ID(), 
			PostingType, VAB_DocTypes_ID,
			acct.GetVAF_Org_ID(), acct.GetAccount_ID(),
			acct.GetVAM_Product_ID(), acct.GetVAB_BusinessPartner_ID(), acct.GetVAB_Project_ID(),
			acct.GetVAB_Promotion_ID(), acct.GetVAB_BillingCode_ID(), acct.GetVAF_OrgTrx_ID(),
			acct.GetVAB_SalesRegionState_ID(), acct.GetC_LocTo_ID(), acct.GetC_LocFrom_ID(),
			acct.GetUser1_ID(), acct.GetUser2_ID());
	}	//	get
        /// <summary>
        /// Get Distributions for combination 
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_AccountBook_ID">schema</param>
        /// <param name="PostingType">posting type</param>
        /// <param name="VAB_DocTypes_ID">document type</param>
        /// <param name="VAF_Org_ID">org</param>
        /// <param name="Account_ID">account</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAB_BusinessPartner_ID">partner</param>
        /// <param name="VAB_Project_ID">project</param>
        /// <param name="VAB_Promotion_ID">campaign</param>
        /// <param name="VAB_BillingCode_ID">activity</param>
        /// <param name="VAF_OrgTrx_ID">trx org</param>
        /// <param name="VAB_SalesRegionState_ID">VAB_SalesRegionState_ID</param>
        /// <param name="C_LocTo_ID">location to</param>
        /// <param name="C_LocFrom_ID">from</param>
        /// <param name="User1_ID">user 1</param>
        /// <param name="User2_ID">user 2</param>
       /// <returns>array of distributions or null</returns>
	public static MDistribution[] Get (Ctx ctx, int VAB_AccountBook_ID, 
		String PostingType, int VAB_DocTypes_ID,
		int VAF_Org_ID, int Account_ID,
		int VAM_Product_ID, int VAB_BusinessPartner_ID, int VAB_Project_ID,
		int VAB_Promotion_ID, int VAB_BillingCode_ID, int VAF_OrgTrx_ID,
		int VAB_SalesRegionState_ID, int C_LocTo_ID, int C_LocFrom_ID,
		int User1_ID, int User2_ID)
	{
		MDistribution[] acctList = Get (ctx, Account_ID);
		if (acctList == null || acctList.Length == 0)
        {
			return null;
        }
		//
		//ArrayList<MDistribution> list = new ArrayList<MDistribution>();
        List<MDistribution> list=new List<MDistribution>();
		for (int i = 0; i < acctList.Length; i++)
		{
			MDistribution distribution = acctList[i];
			if (!distribution.IsActive() || !distribution.IsValid())
            {
				continue;
            }
			//	Mandatory Acct Schema
			if (distribution.GetVAB_AccountBook_ID() != VAB_AccountBook_ID)
            {
				continue;
            }
			//	Only Posting Type / DocType
			if (distribution.GetPostingType() != null && !distribution.GetPostingType().Equals(PostingType))
            {
				continue;
            }
			if (distribution.GetVAB_DocTypes_ID() != 0 && distribution.GetVAB_DocTypes_ID() != VAB_DocTypes_ID)
            {
				continue;
			}
			//	Optional Elements - "non-Any"
			if (!distribution.IsAnyOrg() && distribution.GetVAF_Org_ID() != VAF_Org_ID)
            {
				continue;
            }
			if (!distribution.IsAnyAcct() && distribution.GetAccount_ID() != Account_ID)
            {
				continue;
            }
			if (!distribution.IsAnyProduct() && distribution.GetVAM_Product_ID() != VAM_Product_ID)
            {
				continue;
            }
			if (!distribution.IsAnyBPartner() && distribution.GetVAB_BusinessPartner_ID() != VAB_BusinessPartner_ID)
            {
				continue;
            }
			if (!distribution.IsAnyProject() && distribution.GetVAB_Project_ID() != VAB_Project_ID)
            {
				continue;
            }
			if (!distribution.IsAnyCampaign() && distribution.GetVAB_Promotion_ID() != VAB_Promotion_ID)
            {
				continue;
            }
			if (!distribution.IsAnyActivity() && distribution.GetVAB_BillingCode_ID() != VAB_BillingCode_ID)
            {
				continue;
            }
			if (!distribution.IsAnyOrgTrx() && distribution.GetVAF_OrgTrx_ID() != VAF_OrgTrx_ID)
            {
				continue;
            }
			if (!distribution.IsAnySalesRegion() && distribution.GetVAB_SalesRegionState_ID() != VAB_SalesRegionState_ID)
            {
				continue;
            }
			if (!distribution.IsAnyLocTo() && distribution.GetC_LocTo_ID() != C_LocTo_ID)
            {
				continue;
            }
			if (!distribution.IsAnyLocFrom() && distribution.GetC_LocFrom_ID() != C_LocFrom_ID)
            {
				continue;
            }
			if (!distribution.IsAnyUser1() && distribution.GetUser1_ID() != User1_ID)
            {
				continue;
            }
			if (!distribution.IsAnyUser2() && distribution.GetUser2_ID() != User2_ID)
            {
				continue;
            }
			//
			list.Add (distribution);
		}	//	 for all distributions with acct
		//
		MDistribution[] retValue = new MDistribution[list.Count];
		//list.toArray (retValue);
        retValue=list.ToArray();
		return retValue;
	}	//	get
	
	/// <summary>
	/// Get Distributions for Account
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="Account_ID">id</param>
    /// <returns>array of distributions</returns>
	public static MDistribution[] Get (Ctx ctx, int Account_ID)
	{
		//Integer key = new Integer (Account_ID);
        int key=Account_ID;
		//MDistribution[] retValue = (MDistribution[])s_accounts.get(key);
        MDistribution[] retValue = (MDistribution[])s_accounts[key];
		if (retValue != null) 
        {  
			return retValue;
		}
		String sql = "SELECT * FROM VAGL_Distribution "
			+ "WHERE Account_ID=@Param1";
		//ArrayList<MDistribution> list = new ArrayList<MDistribution>();
        List<MDistribution> list=new List<MDistribution>();
        SqlParameter[] Param=new SqlParameter[1];
        IDataReader idr=null;
        DataTable dt=null;
		//PreparedStatement pstmt = null;
		try
		{
			//pstmt = DataBase.prepareStatement (sql, null);
			//pstmt.setInt (1, Account_ID);
            Param[0]=new SqlParameter("@Param1",Account_ID);
			//ResultSet rs = pstmt.executeQuery ();
            idr=DataBase.DB.ExecuteReader(sql,Param,null);
            dt=new DataTable();
            dt.Load(idr);
            idr.Close();
            foreach(DataRow dr in dt.Rows)
            {
				list.Add (new MDistribution (ctx, dr, null));
            }
		}
		catch (Exception e)
		{
            if (idr != null)
            {
                idr.Close();
            }
			_log.Log(Level.SEVERE, sql, e); 
		}
		finally
        {
            dt=null;
        }
		//
        retValue = new MDistribution[list.Count];
		//list.toArray (retValue);
        retValue = list.ToArray();
		//s_accounts.put(key, retValue);
        s_accounts.Add(key, retValue);
		return retValue;
	}	//	get
	
	
	/**	Distributions by Account			*/
	private static CCache<int,MDistribution[]> s_accounts 
		= new CCache<int,MDistribution[]>("VAGL_Distribution", 100);
	
	/// <summary>
    ///Standard Constructor 
    /// </summary>
    /// <param name="ctx">context</param>
	/// <param name="VAGL_Distribution_ID">id</param>
    /// <param name="trxName">transaction</param>
	public MDistribution (Ctx ctx, int VAGL_Distribution_ID, Trx trxName):base(ctx, VAGL_Distribution_ID, trxName)
	{
		//super (ctx, VAGL_Distribution_ID, trxName);
		if (VAGL_Distribution_ID == 0)
		{
		//	setVAB_AccountBook_ID (0);
		//	setName (null);
			//
			SetAnyAcct (true);	// Y
			SetAnyActivity (true);	// Y
			SetAnyBPartner (true);	// Y
			SetAnyCampaign (true);	// Y
			SetAnyLocFrom (true);	// Y
			SetAnyLocTo (true);	// Y
			SetAnyOrg (true);	// Y
			SetAnyOrgTrx (true);	// Y
			SetAnyProduct (true);	// Y
			SetAnyProject (true);	// Y
			SetAnySalesRegion (true);	// Y
			SetAnyUser1 (true);	// Y
			SetAnyUser2 (true);	// Y
			//
			SetIsValid (false);	// N
			SetPercentTotal (Env.ZERO);
		}
	}	//	MDistribution

	/// <summary>
	/// Load Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="dr">datarow</param>
	/// <param name="trxName">transaction</param>
	public MDistribution (Ctx ctx, DataRow dr, Trx trxName):base(ctx, dr, trxName)
	{
		//super(ctx, rs, trxName);
	}	//	MDistribution

	/**	The Lines						*/
	private MDistributionLine[]		_lines = null;
	
	/// <summary>
	/// Get Lines and calculate total
	/// </summary>
	/// <param name="reload">reload data</param>
	/// <returns>array of lines</returns>
	public MDistributionLine[] GetLines (Boolean reload)
	{
		if (_lines != null && !reload)
        {
			return _lines;
		}
		Decimal PercentTotal = Env.ZERO;
		//ArrayList<MDistributionLine> list = new ArrayList<MDistributionLine>();
        List<MDistributionLine> list=new List<MDistributionLine>();
		String sql = "SELECT * FROM VAGL_DistributionLine "
			+ "WHERE VAGL_Distribution_ID=@Param1 ORDER BY Line";
		Boolean hasNullRemainder = false;

        SqlParameter[] Param=new SqlParameter[1];
        IDataReader idr=null;
        DataTable dt=null;
		//PreparedStatement pstmt = null;
		try
		{
			//pstmt = DataBase.prepareStatement (sql, get_TrxName());
			//pstmt.setInt (1, getVAGL_Distribution_ID());
            Param[0]=new SqlParameter("@Param1",GetVAGL_Distribution_ID());
			//ResultSet rs = pstmt.executeQuery ();
            idr=DataBase.DB.ExecuteReader(sql,Param,Get_TrxName());
            dt=new DataTable();
            dt.Load(idr);
            idr.Close();
            foreach(DataRow dr in dt.Rows)
			{
				MDistributionLine dl = new MDistributionLine (GetCtx(),dr, Get_TrxName());
				if (dl.IsActive())
				{
					//PercentTotal = PercentTotal.add(dl.GetPercentDistribution());

                    PercentTotal =Decimal.Add(PercentTotal,dl.GetPercentDistribution());
					//hasNullRemainder = dl.getPercentDistribution().signum() == 0;
                    hasNullRemainder =Env.Signum(dl.GetPercentDistribution()) == 0;
				}
				dl.SetParent(this);
				list.Add (dl);
			}
			
		}
		catch (Exception e)
		{
            if (idr != null)
            {
                idr.Close();
            }
			log.Log(Level.SEVERE, "getLines", e);
		}
		finally
        {
            dt=null;
        }
		//	Update Ratio when saved and difference
		if (hasNullRemainder)
        {
			PercentTotal = Env.ONEHUNDRED;
        }
		if (Get_ID() != 0 && PercentTotal.CompareTo(GetPercentTotal()) != 0)
		{
			SetPercentTotal(PercentTotal);
			Save();
		}
		//	return
		_lines = new MDistributionLine[list.Count];
		//list.toArray (_lines);
        _lines=list.ToArray();
		return _lines;
	}	//	getLines
	
	/// <summary>
	///	Validate Distribution
	/// </summary>
	/// <returns>error message or null</returns>
	public String Validate()
	{
		String retValue = null;
		GetLines(true);
		if (_lines.Length == 0)
        {
			retValue = "@NoLines@";
        }
		else if (GetPercentTotal().CompareTo(Env.ONEHUNDRED) != 0)
        {
			retValue = "@PercentTotal@ <> 100";
        }
		else
		{
			//	More then one line with 0
			int lineFound = -1;
			for (int i = 0; i < _lines.Length; i++)
			{
				//if (_lines[i].getPercentDistribution().signum() == 0)
                if (Env.Signum( _lines[i].GetPercentDistribution()) == 0)
				{
					if (lineFound >= 0 && Env.Signum(_lines[i].GetPercentDistribution()) == 0)
					{
						retValue = "@Line@ " + lineFound 
							+ " + " + _lines[i].GetLine() + ": == 0";
						break;
					}
					lineFound = _lines[i].GetLine();
				}
			}	//	for all lines
		}
		
		SetIsValid (retValue == null);
		return retValue;
	}	//	validate
	
	
	/// <summary>
	/// Distribute Amount to Lines
	/// </summary>
	/// <param name="acct">account</param>
	/// <param name="Amt">amount</param>
	/// <param name="VAB_Currency_ID">currency</param>
	public void Distribute (MVABAccount acct, Decimal Amt, int VAB_Currency_ID)
	{
		log.Info("Amt=" + Amt + " - " + acct);
		GetLines(false);
		int precision = MVABCurrency.GetStdPrecision(GetCtx(), VAB_Currency_ID);
		//	First Round
		Decimal total = Env.ZERO;
		int indexBiggest = -1;
		int indexZeroPercent = -1;
		for (int i = 0; i < _lines.Length; i++)
		{
			MDistributionLine dl = _lines[i];
			if (!dl.IsActive())
            {
				continue;
            }
			dl.SetAccount(acct);
			//	Calculate Amount
			dl.CalculateAmt (Amt, precision);	
			//total = total.add(dl.GetAmt());
            total =Decimal.Add(total,dl.GetAmt());
		//	log.fine("distribute - Line=" + dl.getLine() + " - " + dl.getPercent() + "% " + dl.getAmt() + " - Total=" + total);
			//	Remainder
			if (Env.Signum( dl.GetPercentDistribution()) == 0)
            {
				indexZeroPercent = i;
            }
			if (indexZeroPercent == -1)
			{
				if (indexBiggest == -1)
                {
					indexBiggest = i;
                }
				else if (dl.GetAmt().CompareTo(_lines[indexBiggest].GetAmt()) > 0)
                {
					indexBiggest = i;
                }
			}
		}
		//	Adjust Remainder
		//Decimal difference = Amt.subtract(total);
        Decimal difference =Decimal.Subtract(Amt,total);
		if (difference.CompareTo(Env.ZERO) != 0)
		{
			if (indexZeroPercent != -1)
			{
			//	log.fine("distribute - Difference=" + difference + " - 0%Line=" + _lines[indexZeroPercent]); 
				_lines[indexZeroPercent].SetAmt (difference);
			}
			else if (indexBiggest != -1)
			{
			//	log.fine("distribute - Difference=" + difference + " - MaxLine=" + _lines[indexBiggest] + " - " + _lines[indexBiggest].getAmt()); 
				//_lines[indexBiggest].SetAmt (_lines[indexBiggest].GetAmt().add(difference));
                _lines[indexBiggest].SetAmt(Decimal.Add(_lines[indexBiggest].GetAmt(), difference));
			}  
			else
            {
				log.Warning("Remaining Difference=" + difference); 
            }
		}
		//
        if (VLogMgt.IsLevelFinest()) //if (CLogMgt.isLevelFinest())
		{
			for (int i = 0; i < _lines.Length; i++)
			{
				if (_lines[i].IsActive())
					log.Fine("Amt=" + _lines[i].GetAmt() + " - " + _lines[i].GetAccount());
			}
		}
	}	//	distribute
	
	
	/// <summary>
	/// Before Save
	/// </summary>
	/// <param name="newRecord">new</param>
	/// <returns>true</returns>
	protected Boolean beforeSave (Boolean newRecord)
	{
		//	Reset not selected Any
		if (IsAnyAcct() && GetAccount_ID() != 0)
        {
			SetAccount_ID(0);
        }
		if (IsAnyActivity() && GetVAB_BillingCode_ID() != 0)
        {
			SetVAB_BillingCode_ID(0);
        }
		if (IsAnyBPartner() && GetVAB_BusinessPartner_ID() != 0)
        {
			SetVAB_BusinessPartner_ID(0);
        }
		if (IsAnyCampaign() && GetVAB_Promotion_ID() != 0)
        {
			SetVAB_Promotion_ID(0);
        }
		if (IsAnyLocFrom() && GetC_LocFrom_ID() != 0)
        {
			SetC_LocFrom_ID(0);
        }
		if (IsAnyLocTo() && GetC_LocTo_ID() != 0)
        {
			SetC_LocTo_ID(0);
        }
		if (IsAnyOrg() && GetOrg_ID() != 0)
        {
			SetOrg_ID(0);
        }
		if (IsAnyOrgTrx() && GetVAF_OrgTrx_ID() != 0)
        {
			SetVAF_OrgTrx_ID(0);
        }
		if (IsAnyProduct() && GetVAM_Product_ID() != 0)
        {
			SetVAM_Product_ID(0);
        }
		if (IsAnyProject() && GetVAB_Project_ID() != 0)
        {
			SetVAB_Project_ID(0);
        }
		if (IsAnySalesRegion() && GetVAB_SalesRegionState_ID() != 0)
        {
			SetVAB_SalesRegionState_ID(0);
        }
		if (IsAnyUser1() && GetUser1_ID() != 0)
        {
			SetUser1_ID(0);
        }
		if (IsAnyUser2() && GetUser2_ID() != 0)
        {
			SetUser2_ID(0);
        }
		return true;
	}	//	beforeSave
	
}	//	MDistribution

}
