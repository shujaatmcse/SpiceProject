using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;

namespace Spice.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customer/ShoppingCarts
        public async Task<IActionResult> Index()
        {
            var cart = await _context.shoppingCart.ToListAsync();
            return View(cart);
     
        }

        // GET: Customer/ShoppingCarts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCart = await _context.shoppingCart
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shoppingCart == null)
            {
                return NotFound();
            }

            return View(shoppingCart);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(ShoppingCart CartObject)
        {
            CartObject.Id = 0;
            if (ModelState.IsValid)
            {
                // Getting the user Identiy  to know its User Id
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                CartObject.AppliUserId = claim.Value;

                // Retreiving cartfrom Db which is added by this user and has this menu Item
                ShoppingCart cartFromDb = await _context.shoppingCart.Where(c => c.AppliUserId == CartObject.AppliUserId
                                                && c.MenuItemId == CartObject.MenuItemId).FirstOrDefaultAsync();
                // No cart data found than we add the user selection i.e Cart object with count field
                if (cartFromDb == null)
                {
                    await _context.shoppingCart.AddAsync(CartObject);
                }

                // In case where user has already entered the cart data and it is in DB, we want to just changed the Quantity
                else
                {
                    cartFromDb.Count = cartFromDb.Count + CartObject.Count;
                }

                // Saving changes
                await _context.SaveChangesAsync();


                // Saving the quatity in a session variable
                var count = _context.shoppingCart.Where(c => c.AppliUserId == CartObject.AppliUserId).ToList().Count();
                HttpContext.Session.SetInt32(SC.sSsCartCount, count);
                

                return RedirectToAction("Index","Home");
            }

            //If Model is not Valid we need to send back the model to user 
            else
            {

                var menuItemFromDb = await _context.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == CartObject.MenuItemId).FirstOrDefaultAsync();

                ShoppingCart cartObj = new ShoppingCart()
                {
                    menuItem = menuItemFromDb,
                    MenuItemId = menuItemFromDb.Id
                };

                return View(cartObj);
            }
        }
        // GET: Customer/ShoppingCarts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customer/ShoppingCarts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AppliUserId,MenuItemId,Count")] ShoppingCart shoppingCart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shoppingCart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shoppingCart);
        }

        // GET: Customer/ShoppingCarts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCart = await _context.shoppingCart.FindAsync(id);
            if (shoppingCart == null)
            {
                return NotFound();
            }
            return View(shoppingCart);
        }

        // POST: Customer/ShoppingCarts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppliUserId,MenuItemId,Count")] ShoppingCart shoppingCart)
        {
            if (id != shoppingCart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shoppingCart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShoppingCartExists(shoppingCart.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(shoppingCart);
        }

        // GET: Customer/ShoppingCarts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCart = await _context.shoppingCart
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shoppingCart == null)
            {
                return NotFound();
            }

            return View(shoppingCart);
        }

        // POST: Customer/ShoppingCarts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shoppingCart = await _context.shoppingCart.FindAsync(id);
            _context.shoppingCart.Remove(shoppingCart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShoppingCartExists(int id)
        {
            return _context.shoppingCart.Any(e => e.Id == id);
        }
    }
}
