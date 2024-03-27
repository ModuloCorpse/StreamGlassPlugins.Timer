using CorpseLib.Json;
using CorpseLib.Web.API;
using CorpseLib.Web.Http;

namespace TimerPlugin
{
    public class TimerEndpoint(TimerManager timerManager) : AHTTPEndpoint("/")
    {
        private readonly TimerManager m_TimerManager = timerManager;
        private static readonly string FORMAT = "${mm}:${ss}";

        protected override Response OnPostRequest(Request request)
        {
            if (request.HaveParameter("id"))
            {
                string id = request.GetParameter("id");
                if (!string.IsNullOrEmpty(id))
                {
                    if (m_TimerManager.StartTimer(request.GetParameter("id")))
                        return new(200, "Ok");
                    return new(404, "Not Found", string.Format("Timer \"{0}\" not found", id));
                }
                return new(400, "Bad Request", "Missing id value in request parameters");
            }
            else
            {
                try
                {
                    JsonObject jfile = JsonParser.Parse(request.Body);
                    if (jfile.TryGet("path", out string? path) && path != null &&
                        jfile.TryGet("duration", out int duration))
                    {
                        string endMessage = jfile.GetOrDefault("end", string.Empty);
                        Timer.AdsInfo? adsInfo = null;
                        if (jfile.TryGet("ads_duration", out int adsDuration))
                        {
                            if (jfile.TryGet("ads_delay", out int adsDelay))
                                adsInfo = new(adsDuration, adsDelay);
                            else
                                adsInfo = new(adsDuration);
                        }
                        Timer timer = new(path, path, FORMAT, endMessage, string.Empty, duration, adsInfo);
                        m_TimerManager.StartTimer(timer);
                        return new(200, "Ok");
                    }
                    return new(400, "Bad Request", "Missing path and/or duration in request body");
                }
                catch
                {
                    return new(400, "Bad Request", "Request body isn't a well-formed json");
                }
            }
        }
    }
}
