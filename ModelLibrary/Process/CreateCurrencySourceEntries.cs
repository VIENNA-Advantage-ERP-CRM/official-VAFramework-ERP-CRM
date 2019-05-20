using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using System.ServiceModel;
using VAdvantage.Model;

namespace VAdvantage.Process
{
    class CreateCurrencySourceEntries:ProcessEngine.SvrProcess
    {
         string KEY = DataBase.GlobalVariable.ACCESSKEY;//"caff4eb4fbd6273e37e8a325e19f0991";
        protected override void Prepare()
        {
            //throw new NotImplementedException();
        }

        protected override string DoIt()
        {
            //throw new NotImplementedException();
            string sql=@"DELETE C_CURRENCYSOURCE WHERE AD_CLIENT_ID="+GetCtx().GetAD_Client_ID();
            if (DB.ExecuteQuery(sql) == -1)
            {
                return "ErrorInDeleteEntries";
            }

            String URL = "http://localhost/CloudService55/AccountService.asmx";
            //String CloudURL = "http://cloudservice.viennaadvantage.com/AccountService.asmx";
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None)
            {
                CloseTimeout = new TimeSpan(00, 20, 00),
                SendTimeout = new TimeSpan(00, 20, 00),
                OpenTimeout = new TimeSpan(00, 20, 00),
                ReceiveTimeout = new TimeSpan(00, 20, 00),
                MaxReceivedMessageSize = int.MaxValue,
                MaxBufferSize = int.MaxValue
            };
            var client= new ModelLibrary.AcctService.AccountServiceSoapClient(binding, new EndpointAddress(URL));

            ModelLibrary.AcctService.CurrencyRateConversionUrlInfo urlInfo= client.GetCurrencySourceUrl(KEY);

            client.Close();
            if (urlInfo != null)            
            {
                int count=0;
                for (int i = 0; i < urlInfo.IDs.Count; i++)
                {
                    MCurrencySource src = new MCurrencySource(GetCtx(), 0, null);
                    src.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
                    src.SetAD_Org_ID(0);
                    src.SetName(urlInfo.Names[i]);
                    src.SetDescription(urlInfo.Descriptions[i]);
                    src.SetIsActive(true);
                    src.SetURL(urlInfo.URLs[i]);
                    if (src.Save())
                    {
                        count++;
                    }

                }

                return  count+" RowsCreated";
            }

           



            return "NoRowCreated";
        }


    }
}
