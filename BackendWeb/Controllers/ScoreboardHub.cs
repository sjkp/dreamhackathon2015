using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackendWeb.Controllers
{
    public class ScoreboardHub : Hub
    {
       private readonly Scoreboard _scoreboard;

        public ScoreboardHub() : this(Scoreboard.Instance) { }

        public ScoreboardHub(Scoreboard scoreboard)
        {
            _scoreboard = scoreboard;
        }

        public IEnumerable<string> GetAllUsers()
        {
            return _scoreboard.Users.Select(s => s.Key);
        }

        public void Start()
        {
            _scoreboard.Start();
        }
    }
}