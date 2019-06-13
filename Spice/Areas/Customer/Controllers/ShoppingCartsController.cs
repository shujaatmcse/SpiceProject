using System;
using System.Collections;
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
using Spice.ViewModel;

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
        // Geting the shopping cart items.
        public async Task<IActionResult> Index()
        {
            // Creating an object of Orderheader and initializing it to zero, becouase 
            // we will be adding OrderHeader.OrderTotal previous value to a new value. so if no previous value then it will give error
            OrderHeader orderHeader = new OrderHeader();
           // Initialized to zero, to avoid error
            orderHeader.OrderTotal = 0;
            OrderHeaderShoppingCartViewModel detailCart = new OrderHeaderShoppingCartViewModel()
            {
                OrderHeader= orderHeader 
            };
            // Initialized order total to Zero.
            orderHeader.OrderTotal = 0;

            // Getting the user Identiy  to know its User Id
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // Retreiving cartfrom Db which is added by this user
            // and assiging it to the detailCard Shopping cart List
             detailCart.ShoppingCart = await _context.shoppingCart.Where(c => c.AppliUserId == claim.Value).ToListAsync();
            if (detailCart.ShoppingCart != null)
            {
                // Adding the Order Total 
                // And since the Menu Item is not mapped we can not use the Include statement above and will not be able to navigate to
                // Include Menu Item so we have to assign manually MenuItem object to the detailCart.Shopping Cart List
                // We know we have the menu Item Id so we will  get Menu Item Object for each of that Id and we also have MenuItem Object
                // but we cannot not navigate through it.
                foreach (var item in detailCart.ShoppingCart)
                {
                    //since we have not mappped the menuItem we have to manually add each MenuItem details to the Shopping cart list object
                    // Creating the Menu Item object
                     var menuItem = await _context.MenuItem.Where(m => m.Id == item.MenuItemId).FirstAsync();
                    // Important thing to note he is that we are change the menuItem itself through for each loop.
                    // Assigning and changing the MenuItem
                    item.menuItem = menuItem;
                    // now adding details of menu Item.
                     detailCart.OrderHeader.OrderTotal = detailCart.OrderHeader.OrderTotal + item.menuItem.Price * item.Count;  
                }
                // At this time coupon code is not yet applied
                return View(detailCart);
            }

            return View();
     
        // You may be thinking why we are not mapping the MenuItem  to advoid the manual addition.
        // You can do that but then when you want to add menu Item to the List, it will give error since the incoming Object will 
        // not have Id of Category or subcategory. If you map it, then the only solution may be to retriview the MenuItem Object for that Id
        // and then add that object to Database.
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
