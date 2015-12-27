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

            height = 50;
            width = 550;
        }

        public override void Start()
        {
            base.Start();

            _titleText.relativePosition = new Vector3(125, 0);
            _titleText.text = "Spotify";
            _titleText.textScale = 1.5f;

            _dragHandle.width = width - 50;
            _dragHandle.height = height;
            _dragHandle.relativePosition = Vector3.zero;
            _dragHandle.target = ParentUI;
        }
    }
}
