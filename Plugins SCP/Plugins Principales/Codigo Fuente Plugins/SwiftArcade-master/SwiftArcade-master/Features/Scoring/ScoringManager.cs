using Hints;
using LabApi.Features.Wrappers;
using SwiftArcadeMode.Features.Humans.Perks.Content.SixthSense;
using SwiftArcadeMode.Utils.Extensions;
using System;
using System.Collections.Generic;

namespace SwiftArcadeMode.Features.Scoring
{
    public static class ScoringManager
    {
        /// <summary>
        /// User ID (Steam ID) to score number.
        /// </summary>
        public static readonly Dictionary<string, int> Scores = [];
        public static readonly Dictionary<string, string> IDToName = [];

        public static readonly HashSet<ScoreEventBase> Events = [];

        public static HashSet<Type> ScoreEventsCache
        {
            get
            {
                _scoreEventsCache ??= ReflectionExtensions.GetAllNonAbstractSubclasses<ScoreEventBase>();
                return _scoreEventsCache;
            }
        }

        private static HashSet<Type> _scoreEventsCache;

        public static void Enable()
        {
            foreach (Type t in ScoreEventsCache)
            {
                ScoreEventBase b = (ScoreEventBase)Activator.CreateInstance(t);
                Events.Add(b);
                b.Enable();
            }
        }

        public static void Tick()
        {
            foreach (ScoreEventBase b in Events)
                b.Tick();
        }

        public static void Disable()
        {
            foreach (ScoreEventBase b in Events)
                b.Disable();
        }

        public static void AddScore(this Player p, int amount)
        {
            if (p.IsDummy || string.IsNullOrWhiteSpace(p.UserId) || p.DoNotTrack)
                return;

            if (!IDToName.ContainsKey(p.UserId))
                IDToName.Add(p.UserId, p.Nickname);
            else if (p.Nickname != IDToName[p.UserId])
                IDToName[p.UserId] = p.Nickname;

            if (!Scores.ContainsKey(p.UserId))
                Scores.Add(p.UserId, amount);
            else
                Scores[p.UserId] += amount;
        }

        public static int GetScore(this Player p) => Scores.ContainsKey(p.UserId) ? Scores[p.UserId] : 0;
    }
}
