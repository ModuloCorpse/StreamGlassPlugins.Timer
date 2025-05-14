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

            StreamGlassCanals.NewCanal<string>("timer_family_ended");

            StreamGlassActions.AddAction(new CreateTimerAction(m_TimerManager));
            StreamGlassActions.AddAction(new StartTimerAction(m_TimerManager));
            StreamGlassActions.AddAction(new StopTimerAction(m_TimerManager));
        }

        protected override void OnInit() { }

        public Dictionary<CorpseLib.Web.Http.Path, AEndpoint> GetEndpoints() => new() {
            { new("/"), new TimerEndpoint(m_TimerManager) }
        };

        protected override void OnUnload() { }
    }
}
