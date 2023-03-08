using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnifiedUI.Helpers;
using UnityEngine;

namespace TrainScheduler
{
    internal static class UUI
    {
        // UUI Button.
        private static UUICustomButton s_uuiButton;

        /// <summary>
        /// Gets the UUI button instance.
        /// </summary>
        internal static UUICustomButton UUIButton => s_uuiButton;

        /// <summary>
        /// Performs initial setup and creates the UUI button.
        /// </summary>
        internal static void Setup()
        {
            // Add UUI button.
            if (s_uuiButton == null)
            {
                s_uuiButton = UUIHelpers.RegisterCustomButton(
                    name: "TrainScheduler",
                    groupName: null, // default group
                    tooltip: "WOW WOW",
                    icon: UUIHelpers.LoadTexture(UUIHelpers.GetFullPath<TrainScheduler>("Resources", "mod-icon.png")),
                    onToggle: (value) => TimeTablePanel.SetState(value),
                    hotkeys: new UUIHotKeys { ActivationKey = ModSettings.Hotkey });

                // Set initial state.
                s_uuiButton.IsPressed = false;
            }
        }
    }
}
