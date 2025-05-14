using CorpseLib.Web.API;
using CorpseLib.Web.Http;

namespace TimerPlugin
{
    public class TimerEndpoint(TimerManager timerManager) : AHTTPEndpoint()
    {
        private readonly TimerManager m_TimerManager = timerManager;

        protected override Response OnPostRequest(Request request)
        {
            if (request.HaveParameter("id") && request.HaveParameter("stop"))
                return new(400, "Bad Request", "Request have both id and stop parameter");

            if (request.HaveParameter("id"))
            {
                string id = request.GetParameter("id");
                if (!string.IsNullOrEmpty(id))
                {
                    if (m_TimerManager.StartTimer(id))
                        return new(200, "Ok");
                    return new(404, "Not Found", string.Format("Timer \"{0}\" not found", id));
                }
                return new(400, "Bad Request", "Missing id value in request parameters");
            }
            else if (request.HaveParameter("stop"))
            {
                string stop = request.GetParameter("stop");
                if (!string.IsNullOrEmpty(stop))
                {
                    if (m_TimerManager.StopTimer(stop))
                        return new(200, "Ok");
                    return new(404, "Not Found", string.Format("Timer family \"{0}\" not running", stop));
                }
                return new(400, "Bad Request", "Missing stop value in request parameters");
            }
            else
                return new(400, "Bad Request", "Missing id or stop parameter");
        }
    }
}
