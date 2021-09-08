using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Controllers
{
    public class BookslistController : Controller
    {
        private readonly Context _db;

        [BindProperty]
        public Book Books { get; set; }

        public BookslistController(Context db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Books = new Book();
            if (id == null)
            {
                return View(Books);
            }
            Books = _db.Books.FirstOrDefault(u=>u.Id == id);
            if(Books == null)
            {
                return NotFound();
            }
            return View(Books);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                if (Books.Id == 0)
                {
                    _db.Books.Add(Books);
                    
                }
                else
                {
                    _db.Books.Update(Books);
                    
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Books);
        }


        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _db.Books.ToListAsync() });
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var bookFromDb = await _db.Books.FirstOrDefaultAsync(u => u.Id == id);
            if (bookFromDb == null)
            {
                return Json(new { success = false, message = "Erorr" });
            }

            _db.Books.Remove(bookFromDb);
            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
