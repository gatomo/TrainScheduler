using CitiesHarmony.API;
using ColossalFramework;
using ColossalFramework.IO;
using ColossalFramework.UI;
using ICities;
using System;
using System.IO;
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

            UIHelperBase group = helper.AddGroup("TrainScheduler Options");
            group.AddButton("Reload Timetable", () =>
            {
                TimeTableManager.Deinit();
                TimeTableManager.Init();
            });

            group.AddButton("Create Template", () =>
            {

                //string file = Path.Combine(DataLocation.modsPath, @"TrainScheduler\TimeTables_Template.xml");
                string file = @"TimeTables_Template.xml";
                TimeTableManager.CreateTemplate(file);
            });
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