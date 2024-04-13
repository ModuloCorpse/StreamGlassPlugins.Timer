using CorpseLib.Json;
using CorpseLib.Web.API;
using StreamGlass.Core.Plugin;

namespace TimerPlugin
{
    public class TimerPlugin() : APlugin("Timer"), IAPIPlugin
    {
        private readonly TimerManager m_TimerManager = new();

        static TimerPlugin() => JsonHelper.RegisterSerializer(new Timer.JsonSerializer());

        protected override PluginInfo GeneratePluginInfo() => new("1.0.0-beta", "ModuloCorpse<https://www.twitch.tv/chaporon_>");

        protected override void OnLoad()
        {
            JsonObject obj = JsonParser.LoadFromFile(GetFilePath("settings.json"));
            List<Timer> timers = obj.GetList<Timer>("timers");
            foreach (Timer timer in timers)
                m_TimerManager.RegisterTimer(timer);
        }

        protected override void OnInit() { }

        public AEndpoint[] GetEndpoints() => [
            new TimerEndpoint(m_TimerManager)
        ];

        protected override void OnUnload() { }
    }
}
