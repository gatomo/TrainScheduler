using ColossalFramework.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace TrainScheduler
{
    public class TrainSchedulerSettings
    {
        private static SaveRecord currentSaveRecord;
        public static SaveRecord CurrentSaveRecord 
        { 
            get
            {
                if(currentSaveRecord == null)
                {
                    Init();
                }
                return currentSaveRecord;
            } 
            private set
            {
                currentSaveRecord = value;
            }
        }

        private const string SETTING_FILE_NAME = "TrainSchedulerSettings.xml";

        private static string SettingFile { get; set; }

        public static List<SaveRecord> Settings { get; set; }

        public static void Init()
        {
            SettingFile = Path.Combine(DataLocation.executableDirectory, SETTING_FILE_NAME);
            var saveName = Utility.GetCityName();
            if (saveName == null)
            {
                saveName = Utility.GetSaveName();
            }

            if (!File.Exists(SettingFile))
            {
                Settings = new List<SaveRecord>()
                {
                    CreateDefault()
                };
            }
            else
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(SettingFile))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SettingRecord));
                    Settings = ((SettingRecord)serializer.Deserialize(reader)).Saves;
                }
            }

            currentSaveRecord = Settings.FirstOrDefault(item => item.SaveName == saveName);
            if (currentSaveRecord == null)
            {
                currentSaveRecord = CreateDefault();
            }
            Debug.Log("Use Timetable data from: " + currentSaveRecord.TimetableFileName);
        }

        private static SaveRecord CreateDefault()
        {
            var saveName = Utility.GetCityName();
            if (saveName == null)
            {
                saveName = Utility.GetSaveName();
            }
            return new SaveRecord
            {
                SaveName = saveName,
                TimetablePath = DataLocation.executableDirectory,
                //TimetableFileName = string.Format("Timetables_{0}.xml", saveName)
                // もともとTimeTables.xmlでやってたので、初期値はとりあえずこれ。β版か製品版で変えたい。
                TimetableFileName = string.Format("Timetables.xml", saveName)
            };
        }

        public static void Deinit()
        {
            var settings = new SettingRecord() { Saves = Settings };
            Utility.SaveXml(SettingFile, settings);

            SettingFile = null;
            CurrentSaveRecord = null;
            Settings = null;
        }

    }
}
