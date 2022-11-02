using AmazingStuffShop.DataAccess.Repository.IRepository;
using AmazingStuffShop.Models;
using AmazingStuffShop.Models.ViewModels;
using AmazingStuffShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmazingStuff.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Get all company objects from database as an IEnumerable object. Return to view
        public IActionResult Index()
        {
            IEnumerable<Company> objCompanyList = _unitOfWork.Company.GetAll();
            return View(objCompanyList);
        }
        

        // If id passed in is null return a new company object.  If id is found in
        // database, return that company object instead.
        public IActionResult Upsert(int? id)
        {
            Company company = new();
            

            if (id == null || id == 0)
            {

                return View(company);

            }

            else
            {
                company = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
                return View(company);
            }

        }


        // Check if company object passed in as arguement is valid. If it is and
        // the id is 0, create a new company object and save to database. If it is
        // not 0, then the object already exists in database, so update instead. In
        // either case pass a relevant success message back to the next view
        // and save changes. If the object passed in is not valid simply return the
        // object in view.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company obj)
        {
            if (ModelState.IsValid)
            {

                if (obj.Id == 0)
                {
                    _unitOfWork.Company.Add(obj);
                    TempData["success"] = "Company created successfully";

                }
                else
                {
                    _unitOfWork.Company.Update(obj);
                    TempData["success"] = "Company updated successfully";


                }
                _unitOfWork.Save();

                return RedirectToAction("Index");

            }

            return View(obj);
        }

        #region API Calls
        // Return all the company objects found in database as a Json object.
        [HttpGet]
        public IActionResult GetAll()
        {
            var companyList = _unitOfWork.Company.GetAll();
            return Json(new { data = companyList });
        }

        // Get company object by id from database. If the object is null, 
        // return an error message, otherwise remove the object form the database
        // and return a success message.
        [HttpDelete]
        public IActionResult Delete(int? id)
        {

            var obj = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Company.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });

        }

        #endregion

    }
}
