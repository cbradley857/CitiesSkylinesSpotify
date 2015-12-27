using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CitiesSkylinesSpotify
{
    public class SpotifyUI : UIPanel
    {
        public override void Start()
        {
            this.backgroundSprite = "GenericPanel";
            this.color = new Color32(255, 0, 0, 100);
            this.width = 100;
            this.height = 200;

            UILabel l1 = this.AddUIComponent<UILabel>();
            l1.text = "Hello World";
        }

        public override void Update()
        {
        }
    }
}
