using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CitiesSkylinesSpotify
{
    public class SpotifyUIPanel : UIPanel
    {
        private SpotifyTitlePanel TitlePanel;

        private SpotifyPanel ButtonPanel;

        public override void Start()
        {
            base.Start();

            // Create UI
            backgroundSprite = "MenuPanel";
            width = 160;
            height = 115;
            isVisible = false;
            canFocus = true;
            isInteractive = true;

            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            autoLayoutPadding = new UnityEngine.RectOffset(1, 0, 1, 0);
            autoLayoutStart = LayoutStart.TopLeft;

            TitlePanel = AddUIComponent<SpotifyTitlePanel>();
            TitlePanel.ParentUI = this;

            ButtonPanel = AddUIComponent<SpotifyPanel>();
            ButtonPanel.ParentUI = this;
        }
    }
}
