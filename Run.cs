using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LiveSplitStats
{
    public class Run
    {
        [XmlArray(ElementName = "AttemptHistory")]
        public List<Attempt> AttemptHistory { get; set; }
        [XmlArray(ElementName = "Segments")]
        public List<Segment> Segments { get; set; }
    }
}
