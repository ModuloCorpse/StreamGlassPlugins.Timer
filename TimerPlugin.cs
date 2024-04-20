using CorpseLib.DataNotation;
using CorpseLib.Json;
using CorpseLib.Web.API;
using StreamGlass.Core;
using StreamGlass.Core.Plugin;
using TimerPlugin.Action;

namespace TimerPlugin
{
    public class TimerPlugin() : APlugin("Timer"), IAPIPlugin
    {
        private readonly TimerManager m_TimerManager = new();

        static TimerPlugin() => DataHelper.RegisterSerializer(new Timer.DataSerializer());

        protected override PluginInfo GeneratePluginInfo() => new("1.0.0-beta", "ModuloCorpse<https://www.twitch.tv/chaporon_>");

        protected override void OnLoad()
        {
            DataObject obj = JsonParser.LoadFromFile(GetFilePath("settings.json"));
            List<Timer> timers = obj.GetList<Timer>("timers");
            foreach (Timer timer in timers)
                m_TimerManager.RegisterTimer(timer);

            StreamGlassActions.AddAction(new StartTimerAction(m_TimerManager), true, true, true);
            StreamGlassActions.AddAction(new StopTimerAction(m_TimerManager), true, true, true);
        }

        protected override void OnInit() { }

        public AEndpoint[] GetEndpoints() => [
            new TimerEndpoint(m_TimerManager)
        ];

        protected override void OnUnload() { }
    }
}
