using CorpseLib;
using CorpseLib.Json;

namespace TimerPlugin
{
    public class Timer(string id, string filepath, string format, string finishMessage, string scene, int duration, Timer.AdsInfo? ads)
    {
        public class JsonSerializer : AJsonSerializer<Timer>
        {
            protected override OperationResult<Timer> Deserialize(JsonObject reader)
            {
                if (reader.TryGet("duration", out int duration) &&
                    reader.TryGet("id", out string? id) && id != null &&
                    reader.TryGet("path", out string? path) && path != null)
                {
                    string endMessage = reader.GetOrDefault("end", string.Empty);
                    string format = reader.GetOrDefault("format", "${mm}:${ss}");
                    AdsInfo? ads = null;
                    if (reader.TryGet("ads", out JsonObject? adsObject) && adsObject != null &&
                        adsObject.TryGet("duration", out int adsDuration))
                    {
                        if (adsObject.TryGet("delay", out int adsDelay))
                            ads = new(adsDuration, adsDelay);
                        else
                            ads = new(adsDuration);
                    }
                    string scene = reader.GetOrDefault("scene", string.Empty);
                    return new(new(id, path, format, endMessage, scene, duration, ads));
                }
                return new("Deserialization error", "Cannot deserialize timer");
            }

            protected override void Serialize(Timer obj, JsonObject writer)
            {
                writer["id"] = obj.m_ID;
                writer["path"] = obj.m_FilePath;
                writer["duration"] = obj.m_Duration;
                writer["end"] = obj.m_FinishMessage;
                writer["format"] = obj.m_Format;
                writer["scene"] = obj.m_Scene;
                if (obj.m_Ads != null)
                {
                    JsonObject adsObj = new() { { "duration", obj.m_Ads.Duration } };
                    if (obj.m_Ads.Delay != 0)
                        adsObj["delay"] = obj.m_Ads.Delay;
                    writer["ads"] = adsObj;
                }
            }
        }

        public class AdsInfo(int duration)
        {
            private readonly int m_Duration = duration;
            private readonly int m_Delay = 0;

            public int Duration => m_Duration;
            public int Delay => m_Delay;

            public AdsInfo(int duration, int delay) : this(duration) => m_Delay = delay;
        }

        private readonly AdsInfo? m_Ads = ads;
        private readonly string m_ID = id;
        private readonly string m_FilePath = filepath;
        private readonly string m_Format = format;
        private readonly string m_FinishMessage = finishMessage;
        private readonly string m_Scene = scene;
        private readonly int m_Duration = duration;

        public AdsInfo? Ads => m_Ads;
        public string ID => m_ID;
        public string FilePath => m_FilePath;
        public string Format => m_Format;
        public string FinishMessage => m_FinishMessage;
        public string Scene => m_Scene;
        public int Duration => m_Duration;
    }
}
