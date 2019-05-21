using Microsoft.AspNetCore.Mvc;
using Spice.Data;
using Spice.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = SC.ManagerRole)]
    [Area("Admin")]
    public class HomeController : Controller
    {

        public IActionResult Index()
        {

            return View();
        }

        //private readonly ApplicationDbContext _context;

        //public HomeController(ApplicationDbContext context)
        //{
        //    _context = context;
        //}
        //public async Task<IActionResult> Index()
        //{
        //    LandingPageViewModel vm = new LandingPageViewModel()
        //    {
        //        MenuItem = await _context.MenuItem.Include(c => c.Category).Include(s => s.SubCategory).ToArrayAsync(),
        //        Category = await _context.Categories.ToListAsync(),
        //        Coupon = await _context.Coupon.Where(c => c.IsActive == true).ToListAsync()
        //    };


        //    return View(vm);
        //}
    }
}