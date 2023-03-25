using CitiesHarmony.API;
using ColossalFramework;
using ColossalFramework.IO;
using ColossalFramework.UI;
using ICities;
using System;
using System.IO;
using System.Windows.Forms;
using TrainScheduler.Patches;
using TrainScheduler.TimeTable;
using UnifiedUI.Helpers;
using UnityEngine;

namespace TrainScheduler
{
    public class TrainScheduler : LoadingExtensionBase, IUserMod
    {
        public string version = "0.2.4-alpha";

        public string Name => $"TrainScheduler v{version}";

        public string Description => "Trains leave on specific time";

        #region Lifecycle

        public void OnEnabled()
        {
            HarmonyHelper.EnsureHarmonyInstalled();
        }

        public override void OnCreated(ILoading loading)
        {
            if (HarmonyHelper.IsHarmonyInstalled) TrainSchedulerPatcher.PatchAll();
            base.OnCreated(loading);
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            // マップエディタなどでは不要であるため、ゲームモード以外でTrainSchedulerを起動しないようにする
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame && mode != LoadMode.NewGameFromScenario)
            {
                return;
            }
            try
            {
                TrainSchedulerSettings.Init();
                TimeTableManager.Init();

                // Add UUI button.
                //UUI.Setup();
            }
            catch (Exception e)
            {
                Debug.Log("An error occurred while processing OnLevelLoaded.");
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
            }
        }

        public override void OnLevelUnloading()
        {
            TimeTableManager.Deinit();
            TrainSchedulerSettings.Deinit();
            base.OnLevelUnloading();
        }

        public override void OnReleased()
        {
            if (HarmonyHelper.IsHarmonyInstalled) TrainSchedulerPatcher.UnpatchAll();
            base.OnReleased();
        }

        public void OnDisabled()
        {
        }
        #endregion


        public void OnSettingsUI(UIHelper helper)
        {

            UIHelper group = helper.AddGroup("Train Scheduler Options") as UIHelper;
            UIPanel panel = group.self as UIPanel;

            // TimeTables.xmlの表示
            string timetablePathStr = Path.Combine(TrainSchedulerSettings.CurrentSaveRecord.TimetablePath, TrainSchedulerSettings.CurrentSaveRecord.TimetableFileName);
            string templatePathStr = Path.Combine(TrainSchedulerSettings.CurrentSaveRecord.TimetablePath, "TimeTables_Template.xml");

            //group.AddButton("Create Template", () => TimeTableManager.CreateTemplate(templatePathStr));
            group.AddSpace(5);
            // 1.0.0あたりにはこちらのアプリケーションフォルダの方を使うかも
            UITextField timetableFilePath = (UITextField)group.AddTextfield("Timetable file:", TrainSchedulerSettings.CurrentSaveRecord.TimetableFileName, _ => { }, _ => { });
            timetableFilePath.width = panel.width - 30;
            group.AddButton("Show in File Explorer", () => System.Diagnostics.Process.Start("explorer.exe", "/select," + DataLocation.executableDirectory));

            Debug.Log("Reload Timetable");
            group.AddButton("Reload Timetable", () =>
            {
                var fileName = timetableFilePath.text;
                Debug.Log("timetableFilePath.text: " + fileName);
                if (!string.IsNullOrEmpty(fileName) && fileName.EndsWith(".xml"))
                {
                    Debug.Log("TraomScheduler Line104");
                    TrainSchedulerSettings.CurrentSaveRecord.TimetableFileName = fileName;

                    Debug.Log("TraomScheduler Before Deinit");
                    TimeTableManager.Deinit();
                    Debug.Log("TraomScheduler Before Init After Deinit");
                    TimeTableManager.Init();
                }
                else
                {
                    //TODO 将来的にはエラーメッセージをだしたい
                }
            });
            group.AddSpace(5);

            Debug.Log("Reflect the latest routes in the timetable.");
            group.AddButton("Reflect the latest routes in the timetable.", () =>
            {
                var fileName = timetableFilePath.text;
                if (!string.IsNullOrEmpty(fileName) && fileName.EndsWith(".xml"))
                {
                    TrainSchedulerSettings.CurrentSaveRecord.TimetableFileName = fileName;
                    string newPath = Path.Combine(TrainSchedulerSettings.CurrentSaveRecord.TimetablePath, TrainSchedulerSettings.CurrentSaveRecord.TimetableFileName);
                    TimeTableManager.UpdateTimeTableFile(newPath);
                }
                else
                {
                    //TODO 将来的にはエラーメッセージをだしたい
                }
            });

            group.AddSpace(5);


        }
    }

    public static class ModSettings
    {
        const string SETTINGS_FILE_NAME = "TrainScheduler";

        static ModSettings()
        {
            if (GameSettings.FindSettingsFileByName(SETTINGS_FILE_NAME) == null)
            {
                GameSettings.AddSettingsFile(new SettingsFile[] { new SettingsFile() { fileName = SETTINGS_FILE_NAME } });
            }
        }

        public static SavedInputKey Hotkey = new SavedInputKey(
            "TrainScheduler HotKey", SETTINGS_FILE_NAME,
            key: KeyCode.T, control: true, shift: false, alt: false, true);
    }
}