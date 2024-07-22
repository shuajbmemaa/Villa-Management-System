using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Villa.Application.Common.Interfaces;
using Villa.Domain.Entities;
using Villa.ViewModels;

namespace Villa.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;//access to the db
        private readonly UserManager<User> _userManager;//manage user
        private readonly SignInManager<User> _signInManager;//login user
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        //get
        public IActionResult Login(string returnUrl=null)
        {
            returnUrl ??= Url.Content("~/");

            LoginVM loginVM = new ()
            {
                RedirectUrl = returnUrl
            };

            return View(loginVM);
        }

        public IActionResult Register()
        {
            if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole("Admin")).Wait();//e krijon rolin admin ne aspnetroles
                _roleManager.CreateAsync(new IdentityRole("Customer")).Wait();//roli customer
            }

            //dropdown mi shfaq rolet
            RegisterVM registerVM = new ()
            {
                RoleList=_roleManager.Roles.Select(x=> new SelectListItem
                {
                    Text=x.Name,
                    Value=x.Name
                })
            };
            return View(registerVM);
        }
    }
}
