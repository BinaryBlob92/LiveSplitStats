using System;
using System.Globalization;
using System.Xml.Serialization;

namespace LiveSplitStats
{
    public class Attempt
    {
        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }
        [XmlAttribute(AttributeName = "started")]
        public string StartedString
        {
            get => Started?.ToString();
            set => Started = DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime dateTime) ? (DateTime?)dateTime : null;
        }
        [XmlAttribute(AttributeName = "ended")]
        public string EndedString
        {
            get => Ended?.ToString();
            set => Ended = DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime dateTime) ? (DateTime?)dateTime : null;
        }
        [XmlIgnore]
        public DateTime? Started { get; set; }
        [XmlIgnore]
        public DateTime? Ended { get; set; }
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
