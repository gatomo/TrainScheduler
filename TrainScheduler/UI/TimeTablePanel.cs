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

            ////
            //AddLineSettings(this);

            float y = 100f;
            foreach (var line in TimeTableManager.TimeTable)
            {
                UICheckBox checkbox = AddUIComponent<UICheckBox>();
                checkbox.text = "Enable line: " + line.Name;
                checkbox.enabled = true;
                checkbox.relativePosition = new Vector2(100f, y);
                //checkbox.height = 50f;
                //checkbox.width = this.width - 150f;
                checkbox.size = new Vector2(50f, this.width - 150f);

                y += 100f;
            }


            //// Decorative icon (top-left).
            //UISprite iconSprite = AddUIComponent<UISprite>();
            //iconSprite.relativePosition = new Vector2(5, 5);
            //iconSprite.height = 32f;
            //iconSprite.width = 32f;
            //iconSprite.atlas = UITextures.LoadSingleSpriteAtlas("ACME-UUI");
            //iconSprite.spriteName = "normal";

            //// Set camera references.
            //_controller = CameraUtils.Controller;
            //_mainCamera = CameraUtils.MainCamera;

            //// X-position slider.
            //_xPosSlider = AddCameraSlider(this, Margin, XSliderY, PanelWidth - (Margin * 2f), "CAM_XPOS", -8500f, 8500f, 0.01f, _controller.m_targetPosition.x, "N1", "xPos");

            //// Z-position slider.
            //_zPosSlider = AddCameraSlider(this, Margin, ZSliderY, PanelWidth - (Margin * 2f), "CAM_ZPOS", -8500f, 8500f, 0.01f, _controller.m_targetPosition.z, "N1", "zPos");

            //// Rotation around target slider.
            //_rotSlider = AddCameraSlider(this, Margin, RotSliderY, PanelWidth - (Margin * 2f), "CAM_ROT", -180f, 180f, 0.01f, _controller.m_targetAngle.x, "N2", "rot");

            //// Zoom slider.
            //_zoomSlider = AddCameraSlider(this, Margin, ZoomSliderY, PanelWidth - (Margin * 2f), "CAM_SIZE", _controller.m_minDistance, _controller.m_maxDistance, 1f, _controller.m_targetSize, "N1", "size");

            //// Tilt slider.
            //_tiltSlider = AddCameraSlider(this, Margin, TiltSliderY, PanelWidth - (Margin * 2f), "CAM_TILT", -90f, 90f, 0.01f, _controller.m_targetAngle.y, "N2", "tilt");

            //// FOV slider.
            //_fovSlider = AddCameraSlider(this, Margin, FovSliderY, PanelWidth - (Margin * 2f), "CAM_FOV", MinFOV, MaxFOV, 0.01f, _mainCamera.fieldOfView, "N1", "fov");

            //// Shadow sliders.
            //_shadowMaxSlider = AddCameraSlider(this, Margin, ShadowMaxSliderY, PanelWidth - (Margin * 2f), "CAM_SHD_MAX", CameraUtils.MinMaxShadowDistance, CameraUtils.MaxMaxShadowDistance, 100f, CameraUtils.MaxShadowDistance, "N0", "shadMax");
            //_shadowMinSlider = AddCameraSlider(this, Margin, ShadowMinSliderY, PanelWidth - (Margin * 2f), "CAM_SHD_MIN", CameraUtils.MinMinShadowDistance, CameraUtils.MaxMinShadowDistance, 10f, CameraUtils.MinShadowDistance, "N0", "shadMin");

            //// Building collision checkbox.
            //UICheckBox buildingCollisionCheck = UICheckBoxes.AddLabelledCheckBox(this, Check1X, Check1Y, Translations.Translate("CAM_COL_BLD"));
            //buildingCollisionCheck.isChecked = HeightOffset.BuildingCollision;
            //buildingCollisionCheck.eventCheckChanged += (c, value) => { HeightOffset.BuildingCollision = value; };

            //// Network collision checkbox.
            //UICheckBox netCollisionCHeck = UICheckBoxes.AddLabelledCheckBox(this, Check1X, Check2Y, Translations.Translate("CAM_COL_NET"));
            //netCollisionCHeck.isChecked = HeightOffset.NetworkCollision;
            //netCollisionCHeck.eventCheckChanged += (c, value) => { HeightOffset.NetworkCollision = value; };

            //// Prop collision checkbox.
            //UICheckBox propCollisionCHeck = UICheckBoxes.AddLabelledCheckBox(this, Check2X, Check1Y, Translations.Translate("CAM_COL_PRO"));
            //propCollisionCHeck.isChecked = HeightOffset.PropCollision;
            //propCollisionCHeck.eventCheckChanged += (c, value) => { HeightOffset.PropCollision = value; };

            //// Tree collision checkbox.
            //UICheckBox treeCollisionCheck = UICheckBoxes.AddLabelledCheckBox(this, Check2X, Check2Y, Translations.Translate("CAM_COL_TRE"));
            //treeCollisionCheck.isChecked = HeightOffset.TreeCollision;
            //treeCollisionCheck.eventCheckChanged += (c, value) => { HeightOffset.TreeCollision = value; };

            //// Zoom to cursor.
            //UICheckBox zoomToCursorCheck = UICheckBoxes.AddLabelledCheckBox(this, Check1X, Check3Y, Translations.Translate("CAM_OPT_ZTC"));
            //zoomToCursorCheck.isChecked = ModSettings.ZoomToCursor;
            //zoomToCursorCheck.eventCheckChanged += (c, value) => { ModSettings.ZoomToCursor = value; };
            //zoomToCursorCheck.tooltipBox = UIToolTips.WordWrapToolTip;
            //zoomToCursorCheck.tooltip = Translations.Translate("CAM_OPT_ZTC_TIP");
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
