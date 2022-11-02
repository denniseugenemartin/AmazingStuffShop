    

using AmazingStuffShop.DataAccess;
using AmazingStuffShop.DataAccess.Repository;
using AmazingStuffShop.DataAccess.Repository.IRepository;
using AmazingStuffShop.Models;
using AmazingStuffShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmazingStuffShop.Web.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _unitOfWork.Category.GetAll();
            return View(objCategoryList);
        }
        public IActionResult Create()
        {
           
            return View();
        }


        // Validation to check if fields are acceptable. If so add object and save to
        // database. Return success message to view.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
 
            if(obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The DisplayOrder cannot exactly match the Name");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // If passed a null or 0 id, then return not found error. Otherwise get 
        // category from database based on id passed in as arguement. If not
        // found in database, return not found error, otherwise return category
        // to view.
        public IActionResult Edit(int? id)
        {
            if (id == null || id ==0)
            {
                return NotFound();
            }
            var category = _unitOfWork.Category.GetFirstOrDefault(u=>u.Id==id);  
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }


        // Validation to check if fields are acceptable. If so edit object and save to
        // database. Return success message to view.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The DisplayOrder cannot exactly match the Name");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category edited successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        public IActionResult Delete(int? id)
        {
            // If passed a null or 0 id, then return not found error. Otherwise get 
            // category from database based on id passed in as arguement. If not
            // found in database, return not found error, otherwise return category
            // to view.
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var category = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }


        // On post, remove object from database and save. Return success message to view.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Category obj)
        {

            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
                
        }

    }
}
