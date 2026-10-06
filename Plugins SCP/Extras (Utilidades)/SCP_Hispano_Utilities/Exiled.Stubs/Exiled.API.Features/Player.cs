using System;
using System.Collections.Generic;

namespace Exiled.API.Features
{
    public class Player
    {
        private static readonly List<Player> Players = new List<Player>();

        public static IReadOnlyCollection<Player> List => Players.AsReadOnly();

        public string UserId { get; set; } = Guid.NewGuid().ToString();
        public string Nickname { get; set; } = "Player";
        public bool IsStaff { get; set; }

        public Player()
        {
            Players.Add(this);
        }

        public void Broadcast(float duration, string message)
        {
            Console.WriteLine($"[BROADCAST:{duration}s] {Nickname}: {message}");
        }

        public void Broadcast(int duration, string message)
        {
            Console.WriteLine($"[BROADCAST:{duration}s] {Nickname}: {message}");
        }

        public void Kick(string reason)
        {
            Console.WriteLine($"[KICK] {Nickname}: {reason}");
        }
    }
}
