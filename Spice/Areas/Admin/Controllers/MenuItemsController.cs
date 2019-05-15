using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        //Step1 in Controller, include the ASP.NETCore hosting enviroment.
        private readonly IHostingEnvironment _hostingEnvironment;
        public MenuItemsController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            //Step1 in Controller, include the ASP.NETCore hosting enviroment.
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Admin/MenuItems
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.MenuItem.Include(m => m.Category).Include(m => m.SubCategory);
            return View(await applicationDbContext.ToListAsync());
        }

       
        public JsonResult MyAjaxAction()
        {
            return Json("this is my test");
        }

     
        //Get Sub Categories for Main Category, return json for page
        //[ActionName("GetSubCategories")]
        
        public/* JsonResult*/ async Task<IActionResult> GetSubCategories(int? CategoryIdInController)
        {
            if (CategoryIdInController == null)
            {
                return NotFound();
            }
            var subCategory = await _context.SubCategories
                .Include(s => s.Category).Where(c=>c.CategoryId== CategoryIdInController).ToListAsync();
            if (subCategory == null)
            {
                return NotFound();
            }

            return Json(new SelectList(subCategory, "Id", "Name"));
        }
        // GET: Admin/MenuItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = await _context.MenuItem
                .Include(m => m.Category)
                .Include(m => m.SubCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menuItem == null)
            {
                return NotFound();
            }

            return View(menuItem);
        }

        // GET: Admin/MenuItems/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            //ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "Id", "Name");
            return View();
        }

        // POST: Admin/MenuItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        //Here we are saving file on server not on into DB, th ereceiving is as usual
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Image,Spycness,Price,CategoryId,SubCategoryId")] MenuItem menu)
        {
        // If Modle is valid
            if (ModelState.IsValid)
            {
                // lets save the model first to the database.
                //We made a copy of the object, so when it is saved into DB, it will have the Id assign to it.
                var newMenuItem = menu;
                _context.Add(newMenuItem);
                await _context.SaveChangesAsync();
               
                // declare the root path ,, wich is www
                string webRootPath = _hostingEnvironment.WebRootPath;
                // files variable
                var files = HttpContext.Request.Form.Files;

                //if user has uploaded file
                if (files.Count > 0)
                {
                    // Creating Path where you want the image to be saved, it will be to root folder wwww 
                    var path = Path.Combine(webRootPath, @"images\Products\");
                    // Get the extension of the uploaded file i.e Jpg
                    var extension = Path.GetExtension(files[0].FileName);

                    // Creating the actual file in the specified directory/path  with Model.Id & its extension, you may choose it with diffrent id as you plan
                    using (var filesStream = new FileStream(Path.Combine(path, newMenuItem.Id + extension), FileMode.Create))
                    {
                        //Copying single file
                        files[0].CopyTo(filesStream);
                    }
                    //Saving Path of the Image into Database
                    newMenuItem.Image = @"\images\Products\" + newMenuItem.Id + extension;
                }
                //no file was uploaded, so use default image which is on the server, note the sc is just a path.
                else
                {
                   // This will copy the default file and save it with Id name 
                    //var uploads = Path.Combine(webRootPath, @"images\" + SC.DefaultFoodImage);
                    //System.IO.File.Copy(uploads, webRootPath + @"\images\Products\" + newMenuItem.Id + ".png");

                    ////Saving Path of the Image into Database
                    //newMenuItem.Image = @"\images\Products\" + newMenuItem.Id + ".png";

                    // Instead of copying the default file again and again, you could point to the defualt file once only
                    // Hence no need to copy
                    newMenuItem.Image = @"\images\" + SC.DefaultFoodImage;
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If Model is not Valid
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", menu.CategoryId);
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "Name", "Name", menu.SubCategoryId);
            return View(menu);
        }

        // GET: Admin/MenuItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = await _context.MenuItem.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", menuItem.CategoryId);
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "Id", "Id", menuItem.SubCategoryId);
            return View(menuItem);
        }

        // POST: Admin/MenuItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Image,Spycness,Price,CategoryId,SubCategoryId")] MenuItem menuItem)
        {
            if (id != menuItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(menuItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuItemExists(menuItem.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", menuItem.CategoryId);
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "Id", "Id", menuItem.SubCategoryId);
            return View(menuItem);
        }

        // GET: Admin/MenuItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = await _context.MenuItem
                .Include(m => m.Category)
                .Include(m => m.SubCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menuItem == null)
            {
                return NotFound();
            }

            return View(menuItem);
        }

        // POST: Admin/MenuItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menuItem = await _context.MenuItem.FindAsync(id);
            string webRootPath = _hostingEnvironment.WebRootPath;
            if (menuItem != null)
            {
                //Checking if the MenuItem has a default image/picture or one was uploaded by user.
                //We do not want to delete the default image on server as this will be used by menuitem 
                //as default when user does't upload one
                //Path of the item in Db
                var menuItemPathInDb = menuItem.Image.ToString().TrimStart('\\');
                // The default image path on server
                var pathOnServer = (@"images\" + SC.DefaultFoodImage);
                // If there both path are not the same then we want to delete the  image
                // so we only delete the user uploaded image.
                if (menuItemPathInDb != pathOnServer)
                {
                    var imagePath = Path.Combine(webRootPath, menuItemPathInDb);

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                _context.MenuItem.Remove(menuItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MenuItemExists(int id)
        {
            return _context.MenuItem.Any(e => e.Id == id);
        }
    }
}
