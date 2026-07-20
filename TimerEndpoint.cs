using CorpseLib.Network.API;
using CorpseLib.Network.Http;

namespace TimerPlugin
{
    public class TimerEndpoint(TimerManager timerManager) : AHTTPEndpoint(new())
    {
        private readonly TimerManager m_TimerManager = timerManager;

        protected override async Task<Response> OnPostRequest(Request request)
        {
            if (request.HasParameter("id") && request.HasParameter("stop"))
                return new(400, "Bad Request", "Request have both id and stop parameter");

            if (request.HasParameter("id"))
            {
                string id = request.GetParameter("id");
                if (!string.IsNullOrEmpty(id))
                {
                    if (await m_TimerManager.StartTimer(id))
                        return new(200, "Ok");
                    return new(404, "Not Found", string.Format("Timer \"{0}\" not found", id));
                }
                return new(400, "Bad Request", "Missing id value in request parameters");
            }
            else if (request.HasParameter("stop"))
            {
                string stop = request.GetParameter("stop");
                if (!string.IsNullOrEmpty(stop))
                {
                    if (await m_TimerManager.StopTimer(stop))
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
