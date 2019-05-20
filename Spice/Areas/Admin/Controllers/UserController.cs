using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;

namespace Spice.Areas.Admin.Controllers
{

    //We created an empty UserController, becouase we do not have the model of user.
    //You will need to add Entity Framwork otherwise method such as asynch would not work.
    [Area("Admin")]
    public class UserController : Controller
    {

        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task <IActionResult> Index()
        {
            // Checking if the user has logged in, if user has logged in there will be a value in claim.value
            //claim Identity will give us teh Identity of the user.
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            //Now we can use the claim Identity to use as name identify and check if the claim has a value in it.
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //We are creating a list of the user  except the one who has currently logged in
            //We will have to create view which bydefault is empty, but note that we are passing list , so it will Ienumrable list
            // so it will be as this : @model IEnumerable<ApplicationUser>
            return View( await _context.ApplicationUser.Where(u=> u.Id != claim.Value).ToListAsync());
        }
    }
}