﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using VKLib.Model;

namespace VKLib
{
  
    public class CommandsGenerator
    {
        private static ProfileCommands profileCommands;
        public static ProfileCommands ProfileCommands
        {
            get
            {
                if (profileCommands == null) profileCommands = new ProfileCommands();
                return profileCommands;
            }
            set
            {
                profileCommands = value;
            }
        }

        private static AudioCommands audioCommands;
        public static AudioCommands AudioCommands
        {
            get
            {
                if (audioCommands == null) audioCommands = new AudioCommands();
                return audioCommands;
            }
            set
            {
                audioCommands = value;
            }
        }


        private static WallCommands wallCommands;
        public static WallCommands WallCommands
        {
            get
            {
                if (wallCommands == null) wallCommands = new WallCommands();
                return wallCommands;
            }
            set
            {
                wallCommands = value;
            }
        }

				private static GeolocationCommands geolocationCommands;
				public static GeolocationCommands GeolocationCommands
				{
					get
					{
						if (geolocationCommands == null) geolocationCommands = new GeolocationCommands();
						return geolocationCommands;
					}
					set
					{
						geolocationCommands = value;
					}
				}
      
    }
}
