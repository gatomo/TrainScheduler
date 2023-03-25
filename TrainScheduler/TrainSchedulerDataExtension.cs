using ICities;
using ColossalFramework.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using ColossalFramework.Globalization;

namespace TrainScheduler
{
    public class TrainSchedulerDataExtension : SerializableDataExtensionBase
    {
        //private const string MyDataKey = "MyData";

        //protected override XElement GetSaveData() => SingletonManager<Manager>.Instance.ToXml();

        //public override void OnLoadData()
        //{
        //}

        //public override void OnSaveData()
        //{
        //    base.OnSaveData();

        //    serializableDataManager
        //    string saveName = SaveGame.Title;

        //    byte[] data = new byte[/* データサイズ */];

        //    // データを保存する

        //    serializableDataManager.SaveData(MyDataKey, data);



        //}
        //public string GetSaveName()
        //{
        //    var lastSaveField = AccessTools.Field(typeof(SavePanel), "m_LastSaveName");
        //    var lastSave = lastSaveField.GetValue(null) as string;
        //    if (string.IsNullOrEmpty(lastSave))
        //        lastSave = Locale.Get("DEFAULTSAVENAMES", "NewSave");

        //    return lastSave;
        //}
    }
}
