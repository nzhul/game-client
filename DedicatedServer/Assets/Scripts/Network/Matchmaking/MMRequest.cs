﻿using System;
using Assets.Scripts.Shared.Models;
using UnityEngine.SocialPlatforms;

namespace Assets.Scripts.Network.Matchmaking
{
    public class MMRequest
    {
        public ServerConnection Connection { get; set; }

        public DateTime SearchStart { get; set; }

        public HeroClass StartingClass { get; set; }

        public bool MatchFound { get; set; }

        public int SearchRadius
        {
            get
            {
                return (int)TimeInQueue.TotalSeconds * 10; // 60 seconds * 10 = 600
            }
        }

        public Range SearchRange
        {
            get
            {
                return new Range(this.Connection.MMR - this.SearchRadius, this.Connection.MMR + this.SearchRadius);
            }
        }

        public TimeSpan TimeInQueue
        {
            get
            {
                return DateTime.UtcNow - this.SearchStart;
            }
        }
    }
}
