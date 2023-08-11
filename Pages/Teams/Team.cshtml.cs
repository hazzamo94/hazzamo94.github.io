using LeagueProject.Data;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using LeagueProject.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LeagueProject.Pages.Teams
{
    public class TeamModel : PageModel
    {
        private readonly LeagueContext _context;

        public TeamModel (LeagueContext context)
        {
            _context = context;
        }

        public Team Team { get; set; }
        public async Task OnGetAsync(string Id)
        {
               Team = await _context.Teams
              .Include(t => t.Players)
              .Include(t => t.Division)
              .AsNoTracking()
              .FirstOrDefaultAsync(t => t.TeamId == Id);
        }
    }
}
