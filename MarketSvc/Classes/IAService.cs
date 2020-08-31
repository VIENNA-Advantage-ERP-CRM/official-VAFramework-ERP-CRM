using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Data;
using System.Runtime.Serialization;

namespace MarketSvc
{
    [ServiceContract(Namespace = "VAdvantage")]
    public interface IAService
    {
        [OperationContract]
        ActivationInfo CheckUpdateLicence(ActivationInfo aInfo, string accessKey);

        [OperationContract]
        ActivationInfo UpdateMarketLicence(ActivationInfo aInfo, string accessKey);
       
        [OperationContract]
        int GetKeyEdition(string url, string accessKey);

    }
}

public class ActivationInfo
{
    [DataMember]
    public DateTime? EndDate
    {
        get;
        set;
    }
    [DataMember]
    public bool IsPaid
    {
        get;
        set;
    }
    [DataMember]
    public int UserCount
    {
        get;
        set;
    }
   
    [DataMember]
    public string UserName
    {
        get;
        set;
    }
    [DataMember]
    public string ClientName
    {
        get;
        set;
    }
    [DataMember]
    public string A_Asset_ID
    {
        get;
        set;
    }
    [DataMember]
    public string TokenKey
    {
        get;
        set;
    }
    [DataMember]
    public string Machine_ID
    {
        get;
        set;
    }
    [DataMember]
    public bool IsActivated
    {
        get;
        set;
    }

    [DataMember]
    public bool IsEntKey
    {
        get;
        set;
    }

    [DataMember]
    public bool IsMarketActivated
    {
        get;
        set;
    }

    [DataMember]
    public DateTime? MarketEndDate
    {
        get;
        set;
    }
    [DataMember]
    public bool EvaluationOnly
    {
        get;
        set;
    }
}
