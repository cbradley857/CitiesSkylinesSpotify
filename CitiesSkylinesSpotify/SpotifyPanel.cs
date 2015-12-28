using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CitiesSkylinesSpotify
{
    public class SpotifyPanel : UIPanel
    {
        private UIButton _backButton;

        private UIButton _playPauseButton;

        private UIButton _nextButton;

        public UIPanel ParentUI { get; set; }

        public override void Awake()
        {
            base.Awake();

            _backButton = AddUIComponent<UIButton>();
            _playPauseButton = AddUIComponent<UIButton>();
            _nextButton = AddUIComponent<UIButton>();
        }

        public override void Start()
        {
 	        base.Start();

            // Layout options
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Horizontal;
            autoLayoutPadding = new UnityEngine.RectOffset(1, 0, 0, 0);
            autoLayoutStart = LayoutStart.TopLeft;

            SetupUIButtons();
        }
            
        private void SetupUIButtons()
        {

            StyleButton(ref _backButton);
            _backButton.text = "<";
            _backButton.width = 50;
            _backButton.height = 50;
            _backButton.eventClick += PrevSong_ClickEvent;

            StyleButton(ref _playPauseButton);
            _playPauseButton.text = "+";
            _playPauseButton.width = 50;
            _playPauseButton.height = 50;
            _playPauseButton.eventClick += PlayPause_ClickEvent;

            StyleButton(ref _nextButton);
            _nextButton.text = ">";
            _nextButton.width = 50;
            _nextButton.height = 50;
            _nextButton.eventClick += NextSong_ClickEvent;
        }

        private void NextSong_ClickEvent(UIComponent component, UIMouseEventParameter eventParam)
        {
            LoadingExtension.spotifyControls.NextTrack();
        }

        private void PlayPause_ClickEvent(UIComponent component, UIMouseEventParameter eventParam)
        {
            LoadingExtension.spotifyControls.PausePlay();
        }

        private void PrevSong_ClickEvent(UIComponent component, UIMouseEventParameter eventParam)
        {
            LoadingExtension.spotifyControls.PreviousTrack();
        }

        private void StyleButton(ref UIButton button)
        {
            // Style the button to look like a menu button.
            button.normalBgSprite = "ButtonMenu";
            button.disabledBgSprite = "ButtonMenuDisabled";
            button.hoveredBgSprite = "ButtonMenuHovered";
            button.focusedBgSprite = "ButtonMenuFocused";
            button.pressedBgSprite = "ButtonMenuPressed";
            button.textColor = new Color32(255, 255, 255, 255);
            button.disabledTextColor = new Color32(7, 7, 7, 255);
            button.hoveredTextColor = new Color32(7, 132, 255, 255);
            button.focusedTextColor = new Color32(255, 255, 255, 255);
            button.pressedTextColor = new Color32(30, 30, 44, 255);

            // Enable button sounds.
            button.playAudioEvents = true;
        }
    }
}
