﻿#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace Assets.HeroEditorWorkshop.Common.Scripts.Utils
{
    /// <summary>
    /// Used to pick a color using EditorWindow. Works only inside Unity!
    /// </summary>
    public class ColorPicker : EditorWindow
    {
        public static Color Color;
        public static Action<Color> OnValueChanged;
        public static Action OnApply;

        private static EditorWindow _window;
        private static bool _initialized;
        private static Color _originalColor;
        private static bool _apply;

        public static void Open(Color originalColor)
        {
            _originalColor = Color = originalColor;
            _apply = false;
            _window = GetWindow(typeof(ColorPicker), true, "Select color");
            _window.Show();
            _window.position = Rect.zero;
            _initialized = false;
        }

        public void Update()
        {
            if (!Application.isPlaying)
            {
                Close();
            }
        }

        public void OnDestroy()
        {
            if (Application.isPlaying && !_apply)
            {
                OnValueChanged(_originalColor);
            }
        }

        public void OnGUI()
        {
            if (Application.isPlaying)
            {
                if (!_initialized)
                {
                    var mouse = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);

                    _window.position = new Rect(mouse.x - 240 - 20, mouse.y, 240, 80);
                    _initialized = true;
                }

                Color = EditorGUI.ColorField(new Rect(5, 5, position.width - 6, 15), "Color:", Color);

                OnValueChanged(Color);

                var buttonWidth = (_window.position.width - 6) / 2;

                if (GUI.Button(new Rect(3, 25, buttonWidth, 20), "Revert"))
                {
                    Color = _originalColor;
                }

                if (GUI.Button(new Rect(3 + buttonWidth, 25, buttonWidth, 20), "Apply"))
                {
                    _apply = true;
                    OnApply();
                    Close();
                }
            }
            else
            {
                Close();
            }
        }
    }
}

#endif