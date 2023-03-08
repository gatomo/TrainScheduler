using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
