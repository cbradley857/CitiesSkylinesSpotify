using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CitiesSkylinesSpotify
{
    public class SpotifyMod : IUserMod
    {
        public string Name
        {
            get { return "Cities Skylines Spotify";  }
        }

        public string Description
        {
            get { return "Control Spotify in-game"; }
        }
    }
}
