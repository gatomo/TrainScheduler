using ColossalFramework;
using ColossalFramework.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace TrainScheduler.TimeTable
{
    public static class TimeTableManager
    {
        //public static Dictionary<string, List<string>> TimeTableDic { get; private set; }
        public static List<LineRecord> TimeTable { get; private set; }

        // 時刻表が設定されていない場合に備えて10分おきに発車する時刻表を備えておく
        public static List<string> DefaultTimeTable { get; private set; }
        public static void Init()
        {
            // デフォルトの発車時刻を初期化
            DefaultTimeTable = new List<string>() {
                "0000", "0010", "0020", "0030", "0040", "0050",
                "0100", "0110", "0120", "0130", "0140", "0150",
                "0200", "0210", "0220", "0230", "0240", "0250",
                "0300", "0310", "0320", "0330", "0340", "0350",
                "0400", "0410", "0420", "0430", "0440", "0450",
                "0500", "0510", "0520", "0530", "0540", "0550",
                "0600", "0610", "0620", "0630", "0640", "0650",
                "0700", "0710", "0720", "0730", "0740", "0750",
                "0800", "0810", "0820", "0830", "0840", "0850",
                "0900", "0910", "0920", "0930", "0940", "0950",
                "1000", "1010", "1020", "1030", "1040", "0050",
                "1100", "1110", "1120", "1130", "1140", "1150",
                "1200", "1210", "1220", "1230", "1240", "1250",
                "1300", "1310", "1320", "1330", "1340", "1350",
                "1400", "1410", "1420", "1430", "1440", "1450",
                "1500", "1510", "1520", "1530", "1540", "1550",
                "1600", "1610", "1620", "1630", "1640", "1650",
                "1700", "1710", "1720", "1730", "1740", "1750",
                "1800", "1810", "1820", "1830", "1840", "1850",
                "1900", "1910", "1920", "1930", "1940", "1950",
                "2000", "2010", "2020", "2030", "2040", "2050",
                "2100", "2110", "2120", "2130", "2140", "2150",
                "2200", "2210", "2220", "2230", "2240", "2250",
                "2300", "2310", "2320", "2330", "2340", "2350"
            };

            PrintTrainLines();

            try
            {
                // TODO このあたりの処理を綺麗にしたい
                //string file = Path.Combine(DataLocation.modsPath, @"TrainScheduler\TimeTables.xml");
                string file = @"TimeTables.xml";
                //if (!File.Exists(file))
                //{
                //    file = Path.Combine(DataLocation.modsPath, @"TimeTables.xml");
                //}
                LoadTimeTable(file);
                Debug.Log($"Loading TimeTableRecord from {file}");

            }
            catch (Exception e)
            {
                // 時刻表ファイルを読み込めない場合はデフォルト値を利用し、エラー落ちさせない
                Debug.Log("Failed to load/parse TimeTables.xml... Continue with default schedule which trains depart every 10 minutes.");
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
            }
        }

        public static void Deinit()
        {
            TimeTable = null;
            DefaultTimeTable = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static void LoadTimeTable(string filePath)
        {
            if (!File.Exists(filePath))
            {
                // 時刻表がないと動かしたときにエラーになる。これを回避するため、存在しない場合は、CreateTemplateで現在のマップ情報から作ってしまう。
                //throw new Exception("File is not found");
                CreateTemplate(filePath);
            }

            using (System.IO.StreamReader reader = new System.IO.StreamReader(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TimeTableRecord));
                TimeTable = ((TimeTableRecord)serializer.Deserialize(reader)).Lines;

                foreach (var line in TimeTable)
                {
                    if (line.Stops.Count > 0)
                    {
                        // ログ出力
                        Debug.Log($"=== Line[{line.LineID}] has {line.StopCount} stop(s) ===");

                        for (int i = 0; i < line.Stops.Count; i++)
                        {
                            if (line.Mode == "FirstToAll" && i > 0)
                            {
                                line.Stops[i].Departures = line.Stops[0].Departures;
                                continue;
                            }
                            if (!line.Enabled || !line.Stops[i].Enabled)
                            {
                                line.Stops[i].Departures = null;
                                continue;
                            }
                            if (line.UseDefaultTimeTable || line.Stops[i].UseDefaultTimeTable)
                            {
                                line.Stops[i].Departures = DefaultTimeTable;
                                continue;
                            }

                            string departures = string.Empty;
                            if (line.Stops[i].Mode == "IntervalTime")
                            {
                                // インターバルで発車設定する場合はここで出発時刻を作成
                                DateTime departureTime = new DateTime(2000, 1, 1, Convert.ToInt32(line.Stops[i].Departures[0].Substring(0, 2)), Convert.ToInt32(line.Stops[i].Departures[0].Substring(2, 2)), 0);
                                DateTime endTime = string.IsNullOrEmpty(line.Stops[i].End) ?
                                    new DateTime(2000, 1, 1, 23, 59, 0) :
                                    new DateTime(2000, 1, 1, Convert.ToInt32(line.Stops[i].End.Substring(0, 2)), Convert.ToInt32(line.Stops[i].End.Substring(2, 2)), 0);

                                // 終発の時刻が始発の時刻よりも早い場合は日付跨ぎの設定と判断する
                                if (endTime < departureTime) endTime = endTime.AddDays(1);

                                var interval = Convert.ToInt32(line.Stops[i].Interval);
                                var departuresList = new List<string>();
                                while (departureTime <= endTime)
                                {
                                    var deptTime = departureTime.ToString("HHmm");
                                    departuresList.Add(deptTime);
                                    departures += deptTime + " / ";

                                    // 間隔（分）を出発時刻に追加
                                    departureTime = departureTime.AddMinutes(interval);
                                }
                                line.Stops[i].Departures = departuresList;
                            }
                            else
                            {
                                foreach (var departure in line.Stops[i].Departures)
                                {
                                    departures += departure + " / ";
                                }
                            }

                            Debug.Log($"Stops[{i}] Departures: {departures}");
                        }
                    }
                }
                Debug.Log("Succeeded in initializing process at TimeTableManager");
            }
        }

        // TODOきたいないのでリファクタリングする
        // Enabled、DefaultTimeTable関連の仕様を明確化する
        public static List<string> GetDepartures(ushort lineId, ref Vehicle vehicleData, ushort vehicleId = 0)
        {
            // 最初の駅データが路線全体になってるパターンとデフォルト時刻表を用いるパターンはさっさとreturn
            LineRecord lineData = null;
            try
            {
                lineData = TimeTable.Find(item => item.LineID == Convert.ToString(lineId));

                if (!lineData.Enabled)
                {
                    Debug.Log($"Line {lineId} Scheduler is disabled.");
                    // 路線のライン設定が無効であれば発車時刻を制御しない
                    return null;
                }

                if (lineData.UseDefaultTimeTable)
                {
                    return DefaultTimeTable;
                }

                if (lineData.Mode == "FirstToAll")
                {
                    if (lineData.Stops[0].Enabled)
                    {
                        Debug.Log($"Stops[0] Scheduler is disabled.");
                        // 設定が無効であれば発車時刻を制御しない
                        return null;
                    }
                    if (lineData.Stops[0].UseDefaultTimeTable)
                    {
                        return DefaultTimeTable;
                    }
                    Debug.Log($"Line {lineId} Scheduler is set as first station to all.");
                    return lineData.Stops[0].Departures;
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Timetable of Line {lineId} has some troubles. Using vanilla mode.");
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
                return null;
            }

            // ここから駅ごとに異なる発車時間の設定を時刻表に落とし込む
            try
            {
                var nextNode = Utility.GetNextStationNodeId(ref vehicleData, vehicleId).ToString();
                var index = lineData.Stops.FindIndex(item => item.NextId == nextNode);

                //string nextStationBuilding = Utility.GetNextStationBuildinIdByVehicle(ref vehicleData, vehicleId).ToString(); 
                //string currentStationBuilding = Utility.GetCurrentStationBuildingIdByVehicle(ref vehicleData, vehicleId).ToString();

                //var index = lineData.Stops.FindIndex(item => item.Id == currentStationBuilding && item.NextId == nextStationBuilding);

                return lineData.Stops[index].Departures;
            }
            catch (Exception e)
            {
                Debug.Log($"Timetable of Line {lineId} has some troubles. Using vanilla mode.");
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
                return null;
            }
        }

        public static void PrintTrainLines()
        {
            Debug.Log($"===Listing up transport lines===");
            foreach (var line in Singleton<TransportManager>.instance.m_lines.m_buffer)
            {
                if (line.Info.m_vehicleReason.Equals(TransferManager.TransferReason.PassengerTrain))
                {
                    Debug.Log($"lineNumber: {line.m_lineNumber}, category: {line.Info.category}, name {line.Info.name}");
                }
            }
        }

        /// <summary>
        /// 最新の時刻表テンプレートのデータを取得する
        /// 例えば、駅名や経路変更をした場合に用いる。
        /// ■更新するデータ
        /// 駅名
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public static TimeTableRecord GetCurrentTimeTableRecord(bool update = false)
        {
            try
            {
                // 鉄道情報をXMLデータに変換する
                var timeTableData = new TimeTableRecord() { Lines = new List<LineRecord>() };

                var lineCount = TransportManager.instance.m_lines.m_buffer.Length;

                // lineIndexはlineIDとしてそのまま使えるのであえてfor文
                for (int lineIndex = 0; lineIndex < lineCount; lineIndex++)
                {
                    var lineInfo = TransportManager.instance.m_lines.m_buffer[lineIndex];

                    string transportType = string.Empty;

                    // 公共交通の種別を判定（今のところは電車のみ）
                    switch (lineInfo.Info.GetSubService())
                    {
                        //case ItemClass.SubService.PublicTransportMetro:
                        //    transportType = Enum.GetName(typeof(ItemClass.SubService), ItemClass.SubService.PublicTransportMetro);
                        //    break;
                        case ItemClass.SubService.PublicTransportTrain:
                            transportType = Enum.GetName(typeof(ItemClass.SubService), ItemClass.SubService.PublicTransportTrain);
                            break;
                            //case ItemClass.SubService.PublicTransportTram:
                            //    transportType = Enum.GetName(typeof(ItemClass.SubService), ItemClass.SubService.PublicTransportTram);
                            //    break;
                            //case ItemClass.SubService.PublicTransportMonorail:
                            //    transportType = Enum.GetName(typeof(ItemClass.SubService), ItemClass.SubService.PublicTransportMonorail);
                            //    break;
                    }

                    // 指定した公共交通であればXMLに書き出す
                    // データが壊れて停車場がない公共交通期間もあるため、その場合はテンプレートに出さない
                    if (!string.IsNullOrEmpty(transportType) && lineInfo.CountStops((ushort)lineIndex) > 0)
                    {
                        // 路線の概要情報を作成
                        var line = new LineRecord
                        {
                            LineID = Convert.ToString(lineIndex),
                            Name = TransportManager.instance.GetLineName((ushort)lineIndex),
                            StopCount = Convert.ToString(lineInfo.CountStops((ushort)lineIndex)),
                            Stops = new List<StopRecord>(),
                            TransportType = transportType
                        };

                        // 一つ目（Index=0）の駅のノードIDを設定
                        var stopNetId = lineInfo.m_stops;

                        // 駅の情報を作成
                        for (int stopIndex = 0; stopIndex < lineInfo.CountStops((ushort)lineIndex); stopIndex++)
                        {
                            // stopIndex番目の駅を取得する
                            string currentStationName = InstanceManager.instance.GetName(new InstanceID { NetNode = stopNetId });
                            if (string.IsNullOrEmpty(currentStationName))
                            {
                                ushort currentStationBuilding = Utility.GetNearestStationBuildingIdByNode(stopNetId);
                                currentStationName = Utility.GetBuildingName(currentStationBuilding);
                            }
                            if (string.IsNullOrEmpty(currentStationName))
                            {
                                currentStationName = NetManager.instance.GetSegmentName(stopNetId);
                            }
                            if (string.IsNullOrEmpty(currentStationName))
                            {
                                ref NetNode currentNode = ref NetManager.instance.m_nodes.m_buffer[stopNetId];
                                ref NetLane currentLane = ref NetManager.instance.m_lanes.m_buffer[currentNode.m_lane];
                                currentStationName = NetManager.instance.GetSegmentName(currentLane.m_segment);
                            }

                            var nextNetId = TransportLine.GetNextStop(stopNetId);
                            string nextStationName = InstanceManager.instance.GetName(new InstanceID { NetNode = nextNetId });
                            if (string.IsNullOrEmpty(nextStationName))
                            {
                                ushort nextStationBuilding = Utility.GetNearestStationBuildingIdByNode(nextNetId);
                                nextStationName = Utility.GetBuildingName(nextStationBuilding);
                            }
                            if (string.IsNullOrEmpty(nextStationName))
                            {
                                nextStationName = NetManager.instance.GetSegmentName(nextNetId);
                            }
                            if (string.IsNullOrEmpty(nextStationName))
                            {
                                ref NetNode nextNode = ref NetManager.instance.m_nodes.m_buffer[nextNetId];
                                ref NetLane nextLane = ref NetManager.instance.m_lanes.m_buffer[nextNode.m_lane];
                                nextStationName = NetManager.instance.GetSegmentName(nextLane.m_segment);
                            }

                            line.Stops.Add(new StopRecord
                            {
                                Index = Convert.ToString(stopIndex),
                                Id = Convert.ToString(stopNetId),
                                //Id = Convert.ToString(currentStationBuilding),
                                Name = currentStationName,
                                NextId = Convert.ToString(nextNetId),
                                //NextId = Convert.ToString(nextStationBuilding),
                                NextName = nextStationName,
                                Mode = "Indivisually",
                                Interval = "0",
                                Departures = new List<string>() { "0000" }
                            });

                            // 次の駅ノードを設定し、次のループへ。
                            // ここで変えないといつまでたっても先頭駅の情報しか出てこない。
                            stopNetId = nextNetId;
                        }


                        timeTableData.Lines.Add(line);
                    }
                }

                return timeTableData;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void CreateTemplate(string fileName)
        {
            try
            {
                // 既存のファイルは日付をつけてバックアップ
                if (File.Exists(fileName))
                {
                    File.Copy(fileName, fileName + "." + DateTime.Now.ToString("yyyyMMddHHmmss"));
                }
                var timeTableData = GetCurrentTimeTableRecord();

                SaveXml(fileName, timeTableData);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void UpdateTimeTableFile(string fileName)
        {
            // 基本的な考え方は、Templateの方が最新の駅名や経路を含んでいるはずなので、そこに現状の時刻表をなるべく反映させる
            var timeTableData = GetCurrentTimeTableRecord();

            foreach (var line in timeTableData.Lines)
            {
                var ttLine = TimeTable.FirstOrDefault(e => e.LineID == line.LineID);

                // プレイ中に路線が追加されたパターンはもともとの時刻表にデータがないためエラー回避のためスキップ
                if (ttLine == null) continue;

                line.Enabled = ttLine.Enabled;
                line.UseDefaultTimeTable = ttLine.UseDefaultTimeTable;
                line.Mode = ttLine.Mode;

                foreach (var stop in line.Stops)
                {
                    var ttStop = ttLine.Stops.FirstOrDefault(e => e.Id == stop.Id);

                    // プレイ中に停車場が追加されたパターンはもともとの時刻表にデータがないためエラー回避のためスキップ
                    if (ttStop == null) continue;

                    stop.Enabled = ttStop.Enabled;
                    stop.UseDefaultTimeTable = ttStop.UseDefaultTimeTable;
                    stop.Mode = ttStop.Mode;
                    stop.Interval = ttStop.Interval;
                    stop.End = ttStop.End;
                    if (stop.Mode == "IntervalTime" && ttStop.Departures.Count > 0)
                    {
                        stop.Departures = new List<string>{ ttStop.Departures[0] };
                    }
                    else
                    {
                        stop.Departures = ttStop.Departures;
                    }
                }
            }

            try
            {
                // 既存のファイルは日付をつけてバックアップ
                if (File.Exists(fileName))
                {
                    File.Copy(fileName, fileName + "." + DateTime.Now.ToString("yyyyMMddHHmmss"));
                }

                SaveXml(fileName, timeTableData);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void SaveXml(string fileName, TimeTableRecord tables)
        {
            if (File.Exists(fileName))
            {
                // ファイルが存在する場合は一旦削除する
                File.Delete(fileName);
            }

            using (StreamWriter streamWriter = new StreamWriter(fileName))
            {
                // 鉄道情報をXMLに書き出す
                new XmlSerializer(typeof(TimeTableRecord)).Serialize((TextWriter)streamWriter, tables);
            }
        }

        /// <summary>
        /// 位置をもとに最寄りの駅ビルIDを取得する
        /// </summary>
        /// <param name="position">位置情報</param>
        /// <returns></returns>

        public static int GetLineCount()
        {
            int count = 0;
            foreach (var line in Singleton<TransportManager>.instance.m_lines.m_buffer)
            {
                if (line.Info.m_vehicleReason.Equals(TransferManager.TransferReason.PassengerTrain)) count++;
            }
            return count;
        }
    }
}
