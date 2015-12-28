using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CitiesSkylinesSpotify
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private bool bPanelVisible = false;
        public static SpotifyAPI spotifyControls;

        private UIView _view;

        private LoadMode _mode;

        private GameObject _spotifyPanelGO;

        private SpotifyUIPanel _spotifyPanel;

        private UIButton _spotifyButton;

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            if (mode != LoadMode.NewGame && mode != LoadMode.LoadGame)
                return;

            _mode = mode;

            LogMessage("Initialising Spotify API Now");
            spotifyControls = new SpotifyAPI();

            _view = UIView.GetAView();

            SetUpUI();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();

            if (_mode != LoadMode.LoadGame && _mode != LoadMode.NewGame)
                return;

            if (_spotifyButton)
                GameObject.Destroy(_spotifyButton.gameObject);
            if (_spotifyPanel)
                GameObject.Destroy(_spotifyPanel.gameObject);
        }

        private void SetUpUI()
        {
            if (_view == null)
            {
                LogMessage("Can't find UI View!");
                return;
            }

            _spotifyPanelGO = new GameObject("SpotifyPanel");
            _spotifyPanel = _spotifyPanelGO.AddComponent<SpotifyUIPanel>();
            _spotifyPanel.transform.parent = _view.transform;

            CreateButton();
        }

        private void CreateButton()
        {
            // Create button in the UI
            _spotifyButton = (UIButton)_view.AddUIComponent(typeof(UIButton));

            _spotifyButton.text = "Spotify";

            // Set the button dimensions.
            _spotifyButton.width = 100;
            _spotifyButton.height = 30;

            // Style the button to look like a menu button.
            _spotifyButton.normalBgSprite = "ButtonMenu";
            _spotifyButton.disabledBgSprite = "ButtonMenuDisabled";
            _spotifyButton.hoveredBgSprite = "ButtonMenuHovered";
            _spotifyButton.focusedBgSprite = "ButtonMenuFocused";
            _spotifyButton.pressedBgSprite = "ButtonMenuPressed";
            _spotifyButton.textColor = new Color32(255, 255, 255, 255);
            _spotifyButton.disabledTextColor = new Color32(7, 7, 7, 255);
            _spotifyButton.hoveredTextColor = new Color32(7, 132, 255, 255);
            _spotifyButton.focusedTextColor = new Color32(255, 255, 255, 255);
            _spotifyButton.pressedTextColor = new Color32(30, 30, 44, 255);

            // Enable button sounds.
            _spotifyButton.playAudioEvents = true;

            // Place the button.
            _spotifyButton.transformPosition = new Vector3(-1.70f, 0.85f);

            // Respond to button click.
            _spotifyButton.eventClick += button_eventClick;
        }

        private void button_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            bPanelVisible = !bPanelVisible;

            _spotifyPanel.isVisible = bPanelVisible;
        }

        private void LogMessage(string message)
        {
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, message);
        }
    }
}
