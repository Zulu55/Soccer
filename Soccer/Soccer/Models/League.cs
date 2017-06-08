using System.Collections.Generic;

namespace Soccer.Models
{

    public class League
    {
        public int LeagueId { get; set; }

        public string Name { get; set; }

        public string Logo { get; set; }

        public List<Team> Teams { get; set; }
    }
}
