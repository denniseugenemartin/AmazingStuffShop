using AmazingStuffShop.DataAccess.Repository.IRepository;
using AmazingStuffShop.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AmazingStuff.Web.ViewComponents
{
    public class ShoppingCartViewComponent: ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Function that will be used to display the quantity of items
        // in user's shopping cart via the layout view.
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Get user identity from database.
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // If user is logged in check to see if they have anything stored in the
            // session variable. If so return that as an int to the view. Otherwise
            // get cart items from the database based on user id and count them.
            // Set the session variable to that number and return it to the view.
            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionCart) != null)
                {
                    return View(HttpContext.Session.GetInt32(SD.SessionCart));
                }
                else
                {
                    HttpContext.Session.SetInt32(SD.SessionCart,
                        _unitOfWork.ShoppingCart.GetAll(
                        u => u.ApplicationUserId == claim.Value).ToList().Count);
                    return View(HttpContext.Session.GetInt32(SD.SessionCart));
                }
            }

            // If user is not logged in just return 0 to the view.
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }

        }
    }
}
