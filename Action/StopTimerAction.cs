using CorpseLib.Actions;

namespace TimerPlugin.Action
{
    public class StopTimerAction(TimerManager manager) : AAction(ms_Definition)
    {
        private static readonly ActionDefinition ms_Definition = new ActionDefinition("TimerStop")
            .AddArgument<string>("family");

        private readonly TimerManager m_Manager = manager;

        public override object?[] Call(object?[] args)
        {
            m_Manager.StopTimer((string)args[0]!);
            return [];
        }
    }
}
