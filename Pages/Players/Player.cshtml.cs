using LeagueProject.Data;
using LeagueProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SQLitePCL;

namespace LeagueProject.Pages.Players
{
    public class PlayerModel : PageModel
    {
        //Inject context.
        private readonly LeagueContext _context;
        public PlayerModel(LeagueContext context) => _context = context;

        public Player Player { get; set; }
        public async Task OnGetAsync(string Id)
        {
            Player = await _context.Players
                .Include(p => p.Team)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PlayerId == Id);
        }
    }
}
