using CorpseLib.Json;
using CorpseLib.Web.API;
using StreamGlass.Core.Plugin;

namespace TimerPlugin
{
    public class TimerPlugin : APlugin, IAPIPlugin, ITestablePlugin
    {
        private readonly TimerManager m_TimerManager = new();

        static TimerPlugin() => JsonHelper.RegisterSerializer(new Timer.JsonSerializer());

        public TimerPlugin() : base("Timer") { }

        protected override PluginInfo GeneratePluginInfo() => new("1.0.0-beta", "ModuloCorpse<https://www.twitch.tv/chaporon_>");

        protected override void OnLoad()
        {
            JsonObject obj = JsonParser.LoadFromFile(GetFilePath("timers.json"));
            List<Timer> timers = obj.GetList<Timer>("timers");
            foreach (Timer timer in timers)
                m_TimerManager.RegisterTimer(timer);
        }

        protected override void OnInit() { }

        public AEndpoint[] GetEndpoints() => [
            new TimerEndpoint(m_TimerManager)
        ];

        protected override void OnUnload() { }

        public void Test() => m_TimerManager.Test(GetFilePath("timer_test.txt"));
    }
}
