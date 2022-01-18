using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LiveSplitStats
{
    public class Segment
    {
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlArray(ElementName = "SegmentHistory")]
        public List<Time> SegmentHistory { get; set; }
        [XmlIgnore]
        public double StandardDeviation { get; set; }
    }
}
