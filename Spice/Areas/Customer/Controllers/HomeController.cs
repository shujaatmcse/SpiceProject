using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.ViewModel;

namespace Spice.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _context;

        public HomeController (ApplicationDbContext context)
        {
            _context = context;
        }
        //public IActionResult Index()
        //{

        //    return View();
        //}

        //Reciving Category Id, Optional
        public async Task<IActionResult> Index( int ? Id)
        {
            LandingPageViewModel vm = new LandingPageViewModel();
            MenuItem menu = new MenuItem();
            //Id is received when a specific Category is selected.Its the category Id
            if (Id != null)
            {// Checking first if the Id is a genuian
                var menuItems = await _context.MenuItem.Include(s => s.SubCategory).Include(c => c.Category).Where(c => c.CategoryId == Id).ToListAsync();
           
                if (menuItems == null)
                {
                    return NotFound();
                }
                else// If a specific category has been selected
                {
                    vm.Category = await _context.Categories.ToListAsync();
                    //Will display coupons/offers which currently active, adminstrator will the oppertunity to activate/deactivate coupns
                    vm.Coupon = await _context.Coupon.Where(a => a.IsActive == true).ToListAsync();
                    vm.MenuItem = menuItems;
                    return View(vm);
                }
            }
           else// If No Id has been received(no specifi slection), all menuitems are deiplayed
            {
                //Retrieving list of only those copons which are active.
                vm.Category = await _context.Categories.ToListAsync();
                vm.Coupon = await _context.Coupon.Where(a => a.IsActive == true).ToListAsync();
                //Note use the include where there is a relationship involove.
                vm.MenuItem = await _context.MenuItem.Include(c => c.Category).Include(s => s.SubCategory).ToListAsync();
            }
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }


    }
}
