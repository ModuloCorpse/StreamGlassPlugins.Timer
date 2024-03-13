using CorpseLib.Json;
using CorpseLib.Web.API;
using StreamGlass.Core.Plugin;
using StreamGlass.Core.Settings;

namespace TimerPlugin
{
    public class TimerPlugin : APlugin
    {
        private readonly TimerManager m_TimerManager = new();

        static TimerPlugin() => JsonHelper.RegisterSerializer(new Timer.JsonSerializer());

        public TimerPlugin() : base("timer") { }

        protected override PluginInfo GeneratePluginInfo() => new("1.0.0-beta", "ModuloCorpse<https://www.twitch.tv/chaporon_>");

        protected override void InitTranslation() { }

        protected override void InitSettings()
        {
            JsonObject obj = JsonParser.LoadFromFile(GetFilePath("timers.json"));
            List<Timer> timers = obj.GetList<Timer>("timers");
            foreach (Timer timer in timers)
                m_TimerManager.RegisterTimer(timer);
        }

        protected override void InitPlugin() { }

        protected override void InitCommands() { }

        protected override void InitCanals() { }

        protected override AEndpoint[] GetEndpoints() => [
            new TimerEndpoint(m_TimerManager)
        ];

        protected override void Unregister() { }

        protected override void Update(long deltaTime) { }

        protected override TabItemContent[] GetSettings() => [];

        protected override void TestPlugin() => m_TimerManager.Test(GetFilePath("timer_test.txt"));
    }
}
