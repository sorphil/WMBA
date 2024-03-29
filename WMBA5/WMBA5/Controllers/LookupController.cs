﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using WMBA5.CustomControllers;
using WMBA5.Data;

namespace WMBA5.Controllers
{
    public class LookupController : CognizantController
    {
        private readonly WMBAContext _context;

        public LookupController(WMBAContext context)
        {
            _context = context;
        }
        public IActionResult Index(string Tab = "Information-Tab")
        {
            //Note: select the tab you want to load by passing in
            ViewData["Tab"] = Tab;
            return View();
        }
        public PartialViewResult Player()
        {
            ViewData["PlayerID"] = new
                SelectList(_context.Players
                .OrderBy(p => p.LastName).ThenBy(p => p.FirstName), "ID", "FirstName","Nickname" ,"LastName");
            return PartialView("_Player");
        }
        public PartialViewResult Stats()
        {
            ViewData["StatsID"] = new
                SelectList(_context.Stats
                .OrderBy(ps => ps.PlayerID), "ID", "Hits", "RunsScored", "RBI");
            return PartialView("_Stat");
        }
        public PartialViewResult Schedule()
        {
            ViewData["GameID"] = new
                SelectList(_context.Games
                .OrderBy(g => g.StartTime), "ID","StartTime" ,"Oponent");
            return PartialView("_Game");
        }

        public PartialViewResult Location()
        {
            ViewData["GameID"] = new
                SelectList(_context.Games
                .OrderBy(g => g.LocationID), "ID", "LocationName");
            return PartialView("_Location");
        }
    }
}
