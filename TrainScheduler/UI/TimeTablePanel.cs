using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using TrainScheduler.TimeTable;
using UnityEngine;

namespace TrainScheduler
{
    internal class TimeTablePanel : UIPanel
    {
        private static GameObject s_gameObject;
        private static TimeTablePanel s_panel;
        // 各種設定
        private const float PanelWidth = 400f;
        private const float PanelHeight = 600f;


        internal TimeTablePanel()
        {
            // Basic behaviour.
            autoLayout = false;
            canFocus = true;
            isInteractive = true;

            // Appearance.
            backgroundSprite = "MenuPanel2";
            opacity = 1f;

            // Size.
            size = new Vector2(PanelWidth, PanelHeight);

            // Default position - centre in screen.
            relativePosition = new Vector2(Mathf.Floor((GetUIView().fixedWidth - width) / 2), Mathf.Floor((GetUIView().fixedHeight - height) / 2));

            // Drag bar.
            UIDragHandle dragHandle = AddUIComponent<UIDragHandle>();
            dragHandle.width = this.width - 50f;
            dragHandle.height = this.height;
            dragHandle.relativePosition = Vector3.zero;
            dragHandle.target = this;

            // Title label.
            UILabel titleLabel = AddUIComponent<UILabel>();
            titleLabel.relativePosition = new Vector2(50f, 13f);
            titleLabel.text = "TrainScheduler TimeTables";

            // Close button.
            UIButton closeButton = AddUIComponent<UIButton>();
            closeButton.relativePosition = new Vector2(width - 35, 2);
            closeButton.normalBgSprite = "buttonclose";
            closeButton.hoveredBgSprite = "buttonclosehover";
            closeButton.pressedBgSprite = "buttonclosepressed";
            closeButton.eventClick += (component, clickEvent) => Close();


            // Reload button


            // Link to Web Editor
            //UICheckBox checkbox = AddUIComponent<UICheckBox>();
            //checkbox.text = "Enable line: " + line.Name;
            //checkbox.enabled = true;
            //checkbox.relativePosition = new Vector2(100f, y);
            ////checkbox.height = 50f;
            ////checkbox.width = this.width - 150f;
            //checkbox.size = new Vector2(50f, this.width - 150f);
        }

        internal static TimeTablePanel Panel => s_panel;

        public override void Update()
        {
            base.Update();
        }
        internal static void Create()
        {
            try
            {
                // If no GameObject instance already set, create one.
                if (s_gameObject == null)
                {
                    // Give it a unique name for easy finding with ModTools.
                    s_gameObject = new GameObject("TrainSchedulerTimeTablePanel");
                    s_gameObject.transform.parent = UIView.GetAView().transform;

                    // Create new panel instance and add it to GameObject.
                    s_panel = s_gameObject.AddComponent<TimeTablePanel>();
                }
            }
            catch (Exception e)
            {
                Debug.Log("exception creating train scheduler timetable panel");
                Debug.Log(e.StackTrace);
            }

            // Press UUI button.
            UUI.UUIButton.IsPressed = true;
        }
        internal static void Close()
        {
            // Don't do anything if no panel.
            if (s_panel == null)
            {
                return;
            }

            // Destroy game objects.
            Destroy(s_panel);
            Destroy(s_gameObject);

            // Let the garbage collector do its work (and also let us know that we've closed the object).
            s_panel = null;
            s_gameObject = null;

            // Unpress UUI button.
            UUI.UUIButton.IsPressed = false;
        }
        internal static void SetState(bool visible)
        {
            if (visible)
            {
                Create();
            }
            else
            {
                Close();
            }
        }

        private LineComponent AddLineSettings(UIComponent parent)
        {
            // Title label.
            LineComponent sampleLine = parent.AddUIComponent<LineComponent>();
            sampleLine.text = "line[0]";
            sampleLine.size = new Vector2(100f, 100f);
            sampleLine.name = "Line Component";
            sampleLine.relativePosition = new Vector2(150f, 150f);
            UIButton B0000 = sampleLine.AddUIComponent<UIButton>();
            B0000.text = "00";

            return sampleLine;
        }
    }
}
