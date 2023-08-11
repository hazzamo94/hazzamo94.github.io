using LeagueProject.Data;
using LeagueProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace LeagueProject.Pages.Players
{
    public class PlayersIndexModel : PageModel
    {
        private readonly LeagueContext _context;

        public PlayersIndexModel(LeagueContext context)
        {
            _context = context;
        }

        //List of players to be viewed on page (modified by filtering).
        public List<Player> AllPlayers { get; set; }

        //Lists to populate dropdown filters.
        public SelectList AllTeamIds { get; set; }
        public SelectList PlayerPositions { get; set; }


        //Capture selected filter and sort options.
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedTeam { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedPosition { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = "Name";


        //Stores favourite team cookie.
        public string FavouriteTeam { get; set; }
        
        
        
        public async Task OnGetAsync()
        {
            //Retrieve all players.
            var players = from p in _context.Players
                                select p;

            //Retrieve FavouriteTeam cookie.
            FavouriteTeam = HttpContext.Session.GetString("_Favourite");

            //Populate SelectLists for filters.
            IQueryable<string> teamQuery = from t in _context.Teams
                                           orderby t.TeamId
                                           select t.TeamId;

            AllTeamIds = new SelectList(await teamQuery.ToListAsync());

            IQueryable<string> positionQuery = from p in _context.Players
                                               orderby p.Position
                                               select p.Position;

            PlayerPositions = new SelectList(await positionQuery.Distinct().ToListAsync());

            //Modify query if filters selected/searchstring exists.
            if (!string.IsNullOrEmpty(SearchString))
            {
                players = players.Where(p => p.Name.ToLower().Contains(SearchString.ToLower()));
            }

            if (!string.IsNullOrEmpty(SelectedTeam))
            {
                players = players.Where(p => p.TeamId == SelectedTeam);
            }

            if (!string.IsNullOrEmpty(SelectedPosition))
            {
                players = players.Where(p => p.Position == SelectedPosition);
            }


            //Order depending on selected SortOrder.
            switch (SortOrder) {
                case "Number": players = players.OrderBy(p => p.Number).ThenBy(p => p.TeamId); break;
                case "Name": players = players.OrderBy(p => p.Name).ThenBy(p => p.TeamId); break;
                case "Position": players = players.OrderBy(p => p.Position).ThenBy(p => p.TeamId); break;
            }
            
            //Finally, retrieve players based on current filtering/search options.
            AllPlayers = await players.ToListAsync();
        }
    }
}