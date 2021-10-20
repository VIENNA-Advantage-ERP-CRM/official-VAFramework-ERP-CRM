using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VAdvantage.Classes
{
    public class SessionEventHandler
    {
        public static void SessionEnd(Ctx ctx)
        {
            try
            {
                //VAdvantage.Logging.VLogMgt.Shutdown(ctx);
                MSession s = MSession.Get(ctx);
                if (s != null)
                {
                    s.Logout();
                }
                ModelLibrary.PushNotif.SessionManager.Get().RemoveSession(ctx.GetAD_Session_ID());
              }
            catch
            {

            }
        }
    }
}