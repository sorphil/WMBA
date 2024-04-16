using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using WMBA5.CustomControllers;
using WMBA5.Data;
using WMBA5.Models;
using WMBA5.Utilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WMBA5.Controllers
{
    [Authorize(Roles = "Admin, Rookie Convenor, Intermediate Convenor, Senior Convenor")]
    public class CoachTeamController : ElephantController
    {
        private readonly WMBAContext _context;

        public CoachTeamController(WMBAContext context)
        {
            _context = context;
        }

        // GET: TeamPlayer
        public async Task<IActionResult> Index(int? CoachID, int? page, int? pageSizeID)
        {
            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, "Coach");
            if (!CoachID.HasValue)
            {
                return Redirect(ViewData["returnURL"].ToString());
            }

            var teams = from p in _context.Teams
                           .Include(t => t.Division)
                           .Include(t => t.Coach)
                          where p.CoachID == CoachID.GetValueOrDefault()
                          select p;


            Coach coach = await _context.Coaches
                .Include(t => t.Teams)
                .Where(t => t.ID == CoachID.GetValueOrDefault())
                .AsNoTracking()
                .FirstOrDefaultAsync();
            ViewBag.Coach = coach;

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Team>.CreateAsync(teams.AsNoTracking(), page ?? 1, pageSize);
            return View(pagedData);
        }

        private bool PlayerExists(int id)
        {
            return _context.Players.Any(e => e.ID == id);
        }
    }
}
