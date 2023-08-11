using LeagueProject.Models;
using LeagueProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using SQLitePCL;
using Microsoft.AspNetCore.Http;

namespace LeagueProject.Pages.Teams
{
    public class TeamsIndexModel : PageModel
    {
        private readonly LeagueContext _context;

        public TeamsIndexModel(LeagueContext context)
        {
            _context = context;
        }

        //Vars to store all conferences, divisions and teams.
        public List<Conference> conferences { get; set; }
        public List<Division> divisions { get; set; }
        public List<Team> teams { get; set; }

        //Getting all divisions within a conference, and teams within a division.
        public List<Division> divisionsInConference(string ConferenceId)
        {
            var divisions = (from d in _context.Divisions
                            where d.ConferenceId == ConferenceId
                            orderby d.Name
                            select d).ToList();
            return divisions;
        }
        public List<Team> teamsInDivision(string DivisionId)
        {
            var teams = (from d in _context.Teams
                         where d.DivisionId == DivisionId
                         orderby d.Win
                         select d).ToList();
            return teams;
        }

        //For favourite team dropdown
        public SelectList AllTeams { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FavouriteTeam { get; set; }


        public async Task OnGetAsync()
        {
            //Request relevant data from context.
            //A ConferenceId that exists in the db is "AFC", for testing purposes.
            conferences = await _context.Conferences.ToListAsync();
            divisions = await _context.Divisions.ToListAsync();
            teams = await _context.Teams.ToListAsync();

            //Populate AllTeams SelectList with Team IDs.
            IQueryable<string> teamQuery = from t in _context.Teams
                                           orderby t.TeamId
                                           select t.TeamId;

            AllTeams = new SelectList(await teamQuery.ToListAsync());

            //Checking if favouriteteam has value and managing cookie.
            if (FavouriteTeam != null) 
            {
                HttpContext.Session.SetString("_Favourite", FavouriteTeam);
            } 
            else {
                FavouriteTeam = HttpContext.Session.GetString("_Favourite");
            }
        }
    }
}
