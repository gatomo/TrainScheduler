using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TrainScheduler
{
    public class SettingRecord
    {
            [XmlArray("Saves")]
            [XmlArrayItem("Save")]
            public List<SaveRecord> Saves { get; set; }
    }

    public class SaveRecord
    {
        /// <summary>
        /// Name of Game Save
        /// </summary>
        [XmlAttribute("SaveName")]
        public string SaveName { get; set; }

        /// <summary>
        /// Timetable File Name (only file name)
        /// </summary>
        [XmlAttribute("TimetableFileName")]
        public string TimetableFileName { get; set; }

        /// <summary>
        /// Path of Timetable File Name (not include file name)
        /// </summary>
        [XmlAttribute("TimetablePath")]
        public string TimetablePath { get; set; }
    }
}
