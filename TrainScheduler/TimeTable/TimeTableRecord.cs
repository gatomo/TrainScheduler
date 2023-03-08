using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TrainScheduler.TimeTable
{
    public class TimeTableRecord
    {
        [XmlArray("Lines")]
        [XmlArrayItem("Line")]
        public List<LineRecord> Lines { get; set; }
    }

    public class LineRecord
    {
        [XmlAttribute("LineID")]
        public string LineID { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("StopCount")]
        public string StopCount { get; set; }

        [XmlAttribute("TransportType")]
        public string TransportType { get; set; }

        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; } = false;

        [XmlAttribute("UseDefaultTimeTable")]
        public bool UseDefaultTimeTable { get; set; } = false;

        /// <summary>
        /// FirstToAll = 最初の駅の出発時刻を路線内の全駅に適用
        /// EachStop = 駅ごとに設定
        /// </summary>
        [XmlAttribute("Mode")]
        public string Mode { get; set; } = "EachStop";

        [XmlArray("Stops")]
        [XmlArrayItem("Stop")]
        public List<StopRecord> Stops { get; set; }
    }
    public class StopRecord
    {
        /// <summary>
        /// 駅の番号（0から）
        /// </summary>
        [XmlAttribute("Index")]
        public string Index { get; set; }

        /// <summary>
        /// この停車場で制御をするかどうか
        /// </summary>
        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; } = false;

        [XmlAttribute("UseDefaultTimeTable")]
        public bool UseDefaultTimeTable { get; set; } = false;

        /// <summary>
        /// 駅ビルのID
        /// </summary>
        [XmlAttribute("Id")]
        public string Id { get; set; }

        /// <summary>
        /// 駅名
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 次の駅ビルのID
        /// </summary>
        [XmlAttribute("NextId")]
        public string NextId { get; set; }

        /// <summary>
        /// 次の駅名
        /// </summary>
        [XmlAttribute("NextName")]
        public string NextName { get; set; }

        /// <summary>
        /// Indivisually = 出発時刻指定
        /// IntervalTime = 出発間隔指定
        /// </summary>
        [XmlAttribute("Mode")]
        public string Mode { get; set; } = "IntervalTime";

        /// <summary>
        /// Interval(分)
        /// </summary>
        [XmlAttribute("Interval")]
        public string Interval { get; set; } = "10";

        /// <summary>
        /// End(HHmm)
        /// </summary>
        [XmlAttribute("End")]
        public string End { get; set; } = "0000";

        [XmlArray("Departures")]
        [XmlArrayItem("Departure")]
        public List<string> Departures { get; set; }

        public string DeptText { get; set; }
    }
}
