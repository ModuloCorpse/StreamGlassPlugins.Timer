﻿using CorpseLib;
using CorpseLib.DataNotation;

namespace TimerPlugin
{
    public class Timer(string id, string family, string stringSource, string format, string finishMessage, string scene, int duration, Timer.AdsInfo? ads)
    {
        public class DataSerializer : ADataSerializer<Timer>
        {
            protected override OperationResult<Timer> Deserialize(DataObject reader)
            {
                if (reader.TryGet("duration", out int duration) &&
                    reader.TryGet("id", out string? id) && id != null &&
                    reader.TryGet("family", out string? family) && family != null &&
                    reader.TryGet("string_source", out string? stringSource) && stringSource != null)
                {
                    string endMessage = reader.GetOrDefault("end", string.Empty);
                    string format = reader.GetOrDefault("format", "${mm}:${ss}");
                    AdsInfo? ads = null;
                    if (reader.TryGet("ads", out DataObject? adsObject) && adsObject != null &&
                        adsObject.TryGet("duration", out int adsDuration))
                    {
                        if (adsObject.TryGet("delay", out int adsDelay))
                            ads = new(adsDuration, adsDelay);
                        else
                            ads = new(adsDuration);
                    }
                    string scene = reader.GetOrDefault("scene", string.Empty);
                    return new(new(id, family, stringSource, format, endMessage, scene, duration, ads));
                }
                return new("Deserialization error", "Cannot deserialize timer");
            }

            protected override void Serialize(Timer obj, DataObject writer)
            {
                writer["id"] = obj.m_ID;
                writer["family"] = obj.m_Family;
                writer["string_source"] = obj.m_StringSource;
                writer["duration"] = obj.m_Duration;
                writer["end"] = obj.m_FinishMessage;
                writer["format"] = obj.m_Format;
                writer["scene"] = obj.m_Scene;
                if (obj.m_Ads != null)
                {
                    DataObject adsObj = new() { { "duration", obj.m_Ads.Duration } };
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
        private readonly string m_Family = family;
        private readonly string m_StringSource = stringSource;
        private readonly string m_Format = format;
        private readonly string m_FinishMessage = finishMessage;
        private readonly string m_Scene = scene;
        private readonly int m_Duration = duration;

        public AdsInfo? Ads => m_Ads;
        public string ID => m_ID;
        public string Family => m_Family;
        public string StringSource => m_StringSource;
        public string Format => m_Format;
        public string FinishMessage => m_FinishMessage;
        public string Scene => m_Scene;
        public int Duration => m_Duration;
    }
}
