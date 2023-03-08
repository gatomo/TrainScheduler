using ColossalFramework;
using HarmonyLib;
using System;
using TrainScheduler.TimeTable;
using UnityEngine;

namespace TrainScheduler.AIExtend.PassengerTrainAI
{
    [HarmonyPatch(typeof(global::PassengerTrainAI), "CanLeave")]
    public static class PassengerTrainAICanLeave
    {
        /// <summary>
        /// Harmonyを利用してPassengerTrainAI#CanLeaveメソッドを書き換え
        /// </summary>
        /// <param name="__result"></param>
        /// <param name="vehicleID"></param>
        /// <param name="vehicleData"></param>
        /// <returns></returns>
        public static bool Prefix(ref bool __result, ushort vehicleID, ref Vehicle vehicleData)
        {
            /*
             * vehicleData.m_transportLineにたまに0が入ってくることがある。
             * 列車を発車させる際には、動き出すまで連続でCanLeaveでTrue処理をしなければいけないが
             * m_transportLineに0が入ると判定がずれる。
             * その場合はvehicleDataから一旦先頭車両を取得し、そのTransportLineを参考に判定することで回避する。
             * 理由はわからないが、この手法でうまくいっているためしばらくはこのままにする。
             * →やはりうまくいかないので0の場合は無条件でTrue判定。
             */
            var transportLine = (vehicleData.m_transportLine != 0) ? 
                vehicleData.m_transportLine : 
                Singleton<VehicleManager>.instance.m_vehicles.m_buffer[vehicleData.GetFirstVehicle(vehicleID)].m_transportLine;

            var lineRecord = TimeTableManager.TimeTable.FindIndex(item => item.LineID == Convert.ToString(transportLine));
            if (transportLine == 0)
            {
                // transportLine = 0の場合はなにかおかしいのでとりあえずスキップ処理にする。
                __result = true;
                return false;
            }
            else if (lineRecord < 0 || !TimeTableManager.TimeTable[lineRecord].Enabled)
            {
                //時刻表にtransportLineがない場合やEnabledじゃない場合はMODとして干渉しないようにする(バニラのロジックを実行する）
                __result = true;
                return true;
            }

            // 時刻表を取得
            var departures = TimeTableManager.GetDepartures(transportLine, ref vehicleData, vehicleID);
            // 時刻表を取得できなかったときはあえて設定していないという判断であるため、バニラのロジックを実行する
            if (departures == null)
            {
                __result = true;
                return true;
            }

            // SimulationManager.instance.m_currentGameTimeはゲーム内時間なのでHHmm形式に加工する。
            var time = SimulationManager.instance.m_currentGameTime.ToString("HHmm");

#if DEBUG
            // ▼デバッグ用の出力 判断ロジックに影響しない
            string dept = string.Empty;
            departures.ForEach(item => dept += item + " ");
            Debug.Log($"Departures of line {transportLine} got at {time}: {dept}");

            //// Vehicle情報から次の駅の名前を取得できるかお試し
            ////ushort stNum = Singleton<TransportManager>.instance.m_lines.m_buffer[(ushort)transportLine].GetStop(0);
            ////ushort stNum = Singleton<TransportManager>.instance.m_lines.m_buffer[(ushort)transportLine].GetStop(0);
            //ushort nextStationNodeId = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[vehicleData.GetFirstVehicle(vehicleID)].m_targetBuilding;
            //Vector3 nextStationPosition = Singleton<NetManager>.instance.m_nodes.m_buffer[nextStationNodeId].m_position;
            //ushort nextStationBuilding = Singleton<BuildingManager>.instance.FindBuilding(nextStationPosition, 100.0f, ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTrain, Building.Flags.Active, Building.Flags.Untouchable);
            //var nextStationName = Singleton<BuildingManager>.instance.GetBuildingName(nextStationBuilding, InstanceID.Empty);
            //Debug.Log($"Vehicle {vehicleID} will stop at {nextStationName}");

            //// Vehicleの現在地から次の駅の名前を取得する方法
            //// m_sourceBuildingは値が変わらないため使えない。
            //ushort tempStationBuilding = Singleton<BuildingManager>.instance.FindBuilding(Singleton<VehicleManager>.instance.m_vehicles.m_buffer[vehicleData.GetFirstVehicle(vehicleID)].m_segment.a, 100.0f, ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTrain, Building.Flags.Active, Building.Flags.Untouchable);
            //var tempStationName = Singleton<BuildingManager>.instance.GetBuildingName(tempStationBuilding, InstanceID.Empty);
            //Debug.Log($"Vehicle {vehicleID} is stopping at {tempStationName}");
#endif
            if (transportLine != 0)
            {
                __result = departures.Contains(time);
            }
            else
            {
                // transportLine = 0の場合はなにかおかしいのでとりあえずスキップ処理にする。
                // タイミングの問題の可能性もあるため改めてtransportLineを取得する
                transportLine = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[vehicleData.GetFirstVehicle(vehicleID)].m_transportLine;
                __result = true;
            }
            if (__result)
            {
                // バニラは乗り込みが終わるまで待つが、このMODでは定刻で発車させたいため強制的に市民の乗り込みを停止させる。
                StopEnteringVehicle(ref vehicleData);

                var canleavestop = Singleton<TransportManager>.instance.m_lines.m_buffer[transportLine].CanLeaveStop(vehicleData.m_targetBuilding, (int)vehicleData.m_waitCounter);

                var from = Utility.GetBuildingName(Utility.GetCurrentStationBuildingIdByVehicle(ref vehicleData, vehicleID));
                var to = Utility.GetBuildingName(Utility.GetNextStationBuildinIdByVehicle(ref vehicleData, vehicleID));

                Debug.Log($"leaving: vehicle {vehicleID} on Line={transportLine} from {from} to {to} at {time}(HHmm)");
            }

            // オリジナルのCanLeave処理をスキップするためにfalseを返す（Harmonyの仕様）
            return false;

        }

        /// <summary>
        /// 牽引されている車両も含めて全車両の乗り込みを停止する
        /// </summary>
        /// <param name="vehicleData">先頭車両のVehicleデータ</param>
        private static void StopEnteringVehicle(ref Vehicle vehicleData)
        {
            VehicleManager vehicleInstance = Singleton<VehicleManager>.instance;
            ushort trailingVehicle = vehicleData.m_trailingVehicle;
            int num = 0;

            // 牽引されている車両全てで乗り込み停止処理を回す
            while (trailingVehicle != 0)
            {
                // ↓VehicleAI#CanLeaveの処理を転用だけなので詳細は不明
                CitizenManager citizenInstance = Singleton<CitizenManager>.instance;
                uint num3 = vehicleData.m_citizenUnits;
                int num2 = 0;
                while (num3 != 0u)
                {
                    uint nextUnit = citizenInstance.m_units.m_buffer[(int)((UIntPtr)num3)].m_nextUnit;
                    for (int i = 0; i < 5; i++)
                    {
                        uint citizen = citizenInstance.m_units.m_buffer[(int)((UIntPtr)num3)].GetCitizen(i);
                        if (citizen != 0u)
                        {
                            ushort instance2 = citizenInstance.m_citizens.m_buffer[(int)((UIntPtr)citizen)].m_instance;
                            if (instance2 != 0 && (citizenInstance.m_instances.m_buffer[(int)instance2].m_flags & CitizenInstance.Flags.EnteringVehicle) != CitizenInstance.Flags.None)
                            {
                                //フラグを反転させて乗り込み中のステータスを消す
                                citizenInstance.m_instances.m_buffer[(int)instance2].m_flags &= ~CitizenInstance.Flags.EnteringVehicle;
                            }
                        }
                    }
                    num3 = nextUnit;
                    if (++num2 > 524288)
                    {
                        CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                        break;
                    }
                }
                // ↑VehicleAI#CanLeaveの処理を転用

                // 次の車両に移動
                trailingVehicle = vehicleInstance.m_vehicles.m_buffer[(int)trailingVehicle].m_trailingVehicle;
                if (++num > 16384)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                    break;
                }
            }
        }
    }
}
