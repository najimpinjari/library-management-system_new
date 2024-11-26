using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using library_management_system_new.Models;

namespace library_management_system_new.Controllers
{
    public class BooksController : Controller
    {
        private AjiteshDbEntities db = new AjiteshDbEntities();

        // GET: Books
        public ActionResult Index()
        {
            var books = db.tblBooks.Include(b => b.BookCategory).ToList();
            return View(books);
        }

        // GET: Books/Create
        public ActionResult Create()
        {
            ViewBag.CategoryType = new SelectList(db.BookCategories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,CategoryType,AuthorName,PublicationName,ISBN,CreatedBy,CreatedDate")] tblBook tblBook)
        {
            if (ModelState.IsValid)
            {
                // Prevent duplicate ISBN
                if (db.tblBooks.Any(b => b.ISBN == tblBook.ISBN))
                {
                    ModelState.AddModelError("ISBN", "ISBN already exists.");
                    return View(tblBook);
                }

                // Validate ISBN for certain categories
                if (tblBook.CategoryType == "Horror" && !tblBook.ISBN.StartsWith("978"))
                {
                    ModelState.AddModelError("ISBN", "ISBN for Horror category should start with 978.");
                    return View(tblBook);
                }

                tblBook.CreatedDate = DateTime.Now;  // Setting current date
                db.tblBooks.Add(tblBook);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryType = new SelectList(db.BookCategories, "Id", "Name", tblBook.CategoryType);
            return View(tblBook);
        }

        // Other actions (Edit, Delete, etc.) will remain the same
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
