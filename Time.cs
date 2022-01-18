using System;
using System.Xml.Serialization;

namespace LiveSplitStats
{
    public class Time
    {
        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }
        [XmlElement(ElementName = "RealTime")]
        public string RealTimeString
        {
            get => RealTime?.ToString();
            set => RealTime = TimeSpan.TryParse(value, out TimeSpan timeSpan) ? (TimeSpan?)timeSpan : null;
        }
        [XmlElement(ElementName = "GameTime")]
        public string GameTimeString
        {
            get => GameTime?.ToString();
            set => GameTime = TimeSpan.TryParse(value, out TimeSpan timeSpan) ? (TimeSpan?)timeSpan : null;
        }
        [XmlIgnore]
        public TimeSpan? RealTime { get; set; }
        [XmlIgnore]
        public TimeSpan? GameTime { get; set; }
    }
}
