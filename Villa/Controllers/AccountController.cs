using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Villa.Application.Common.Interfaces;
using Villa.Application.Common.Utility;
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

        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!_roleManager.RoleExistsAsync(Const.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(Const.Role_Admin)).Wait();//e krijon rolin admin ne aspnetroles
                _roleManager.CreateAsync(new IdentityRole(Const.Role_Customer)).Wait();//roli customer
            }//ne klasen Const i marrum konstantet

            //dropdown mi shfaq rolet
            RegisterVM registerVM = new ()
            {
                RoleList=_roleManager.Roles.Select(x=> new SelectListItem
                {
                    Text=x.Name,
                    Value=x.Name
                }),
                RedirectUrl = returnUrl
            };
            return View(registerVM);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {
                User user = new()
                {
                    Name = registerVM.Name,
                    Email = registerVM.Email,
                    PhoneNumber = registerVM.PhoneNumber,
                    NormalizedEmail = registerVM.Email.ToUpper(),
                    EmailConfirmed = true,
                    UserName = registerVM.Name,
                    CreatedAt = DateTime.Now
                };//kur e bojna await te metoda shtoheet async

                var result = await _userManager.CreateAsync(user, registerVM.Password);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(registerVM.Role))
                    {
                        await _userManager.AddToRoleAsync(user, registerVM.Role);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, Const.Role_Customer);
                    }
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    if (string.IsNullOrEmpty(registerVM.RedirectUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return LocalRedirect(registerVM.RedirectUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            //dropdown mi shfaq rolet
            registerVM.RoleList = _roleManager.Roles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Name
            });
            return View(registerVM);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginVM.Email);
                if (user != null)
                {
                    if (await _userManager.IsEmailConfirmedAsync(user))
                    {
                        var result = await _signInManager.PasswordSignInAsync(user.UserName, loginVM.Password, loginVM.RememberMe, lockoutOnFailure: false);
                        if (result.Succeeded)
                        {
                            if (string.IsNullOrEmpty(loginVM.RedirectUrl))
                            {
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                return LocalRedirect(loginVM.RedirectUrl);
                            }
                        }
                        else if (result.IsLockedOut)
                        {
                            ModelState.AddModelError("", "Account locked out due to too many failed login attempts");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Invalid login attempt");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email not confirmed yet");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt");
                }
            }
            return View(loginVM);
        }

    }
}
