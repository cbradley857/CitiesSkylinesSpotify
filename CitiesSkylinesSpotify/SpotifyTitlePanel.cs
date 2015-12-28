using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CitiesSkylinesSpotify
{
    public class SpotifyTitlePanel : UIPanel
    {
        private UILabel _titleText;

        private UIDragHandle _dragHandle;

        public UIPanel ParentUI { get; set; }

        public override void Awake()
        {
            base.Awake();

            _titleText = AddUIComponent<UILabel>();
            _dragHandle = AddUIComponent<UIDragHandle>();

            height = 40;
            width = 160;
        }

        public override void Start()
        {
            base.Start();

            _titleText.relativePosition = new Vector3(10, 10);
            _titleText.text = "Spotify";
            _titleText.textScale = 1f;

            _dragHandle.width = width - 50;
            _dragHandle.height = height;
            _dragHandle.relativePosition = Vector3.zero;
            _dragHandle.target = ParentUI;
        }
    }
}
