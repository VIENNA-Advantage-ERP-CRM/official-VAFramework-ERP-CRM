using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VAdvantage.Model;
using VIS.Helpers;

namespace VIS.Controllers
{
    public class LoginController : ApiController
    {
        // GET api/login
        public List<KeyNamePair> GetOrg(int role,int user,int client)
        {
            return LoginHelper.GetOrgs(role, user, client);

           
            
        }


        public List<KeyNamePair> GetClients(int id)
        {
            return LoginHelper.GetClients(id);
        }
    }
}
