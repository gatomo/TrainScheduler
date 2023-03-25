using ColossalFramework;
using ColossalFramework.Globalization;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using TrainScheduler.TimeTable;
using UnityEngine;

namespace TrainScheduler
{
    public static class Utility
    {
        public static ushort GetCurrentStationBuildingIdByVehicle(ref Vehicle vehicleData, ushort vehicleId = 0)
        {
            return Utility.GetNearestStationBuildingId(Singleton<VehicleManager>.instance.m_vehicles.m_buffer[vehicleData.GetFirstVehicle(vehicleId)].m_segment.a);
        }

        public static ushort GetNextStationBuildinIdByVehicle(ref Vehicle vehicleData, ushort vehicleId = 0)
        {
            return GetNearestStationBuildingIdByNode(GetNextStationNodeId(ref vehicleData, vehicleId));
        }

        public static string GetNearestStationNameByNode(ushort nodeId)
        {
            return GetBuildingName(GetNearestStationBuildingIdByNode(nodeId));
        }

        public static ushort GetNearestStationBuildingIdByNode(ushort nodeId)
        {
            return GetNearestStationBuildingId(Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId].m_position);
        }

        public static ushort GetNearestStationBuildingId(Vector3 position)
        {
            return Singleton<BuildingManager>.instance.FindBuilding(
                position,
                100.0f,
                ItemClass.Service.PublicTransport,
                ItemClass.SubService.PublicTransportTrain,
                Building.Flags.Active,
                Building.Flags.Untouchable);
        }

        public static ushort GetNextStationNodeId(ref Vehicle vehicleData, ushort vehicleId = 0)
        {
            return Singleton<VehicleManager>.instance.m_vehicles.m_buffer[vehicleData.GetFirstVehicle(vehicleId)].m_targetBuilding;
        }

        public static string GetBuildingName(ushort buildingId)
        {
            return Singleton<BuildingManager>.instance.GetBuildingName(buildingId, InstanceID.Empty);
        }

        public static string GetSaveName()
        {
            var lastSaveField = AccessTools.Field(typeof(SavePanel), "m_LastSaveName");
            var lastSave = lastSaveField.GetValue(null) as string;
            if (string.IsNullOrEmpty(lastSave))
                lastSave = Locale.Get("DEFAULTSAVENAMES", "NewSave");

            return lastSave;
        }

        public static string GetCityName()
        {
            return Singleton<SimulationManager>.instance?.m_metaData?.m_CityName ?? null;
        }


        public static void SaveXml<T>(string fileName, T data, bool createBackup = false)
        {
            if (File.Exists(fileName))
            {
                if (createBackup)
                {
                    string newFileName = string.Format("{0}_{1}.xml", Path.GetFileNameWithoutExtension(fileName), DateTime.Now.ToString("yyyyMMddHHmmss"));
                    File.Copy(fileName, newFileName);
                }
                else
                {
                    // ファイルが存在する場合は一旦削除する
                    File.Delete(fileName);
                }
            }

            using (StreamWriter streamWriter = new StreamWriter(fileName))
            {
                // 鉄道情報をXMLに書き出す
                new XmlSerializer(typeof(T)).Serialize((TextWriter)streamWriter, data);
            }
        }
    }
}
