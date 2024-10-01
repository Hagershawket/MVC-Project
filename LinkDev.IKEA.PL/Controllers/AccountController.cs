using LinkDev.IKEA.DAL.Entities.Identity;
using LinkDev.IKEA.PL.ViewModels.Common;
using LinkDev.IKEA.PL.ViewModels.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkDev.IKEA.PL.Controllers
{
    public class AccountController : Controller
    {
		#region Services

		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		} 

		#endregion

		#region Sign Up

		[HttpGet] // GET: /Account/SignUp
        public IActionResult SignUp()
        {
            return View();
        }

		[HttpPost] // POST
		public async Task<IActionResult> SignUp(SignUpViewModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest();

			var user = await _userManager.FindByNameAsync(model.UserName);

			if (user is { })
			{
				ModelState.AddModelError(nameof(SignUpViewModel.UserName), "This Username is Already in use for another account.");
				return View(model);
			}

			user = new ApplicationUser()
			{
				FName = model.FirstName,
				LName = model.LastName,
				UserName = model.UserName,
				Email = model.Email,
				IsAgree = model.IsAgree,
			};

			var result = await _userManager.CreateAsync(user, model.Password);

			if (result.Succeeded)
				return RedirectToAction(nameof(SignIn));

			foreach (var error in result.Errors)
				ModelState.AddModelError(string.Empty, error.Description);

			return View();
		}

		#endregion

		#region Sign In

		[HttpGet] // GET: /Account/SignIn
		public IActionResult SignIn()
		{
			return View();
		}

		[HttpPost] // Post
		public async Task<IActionResult> SignIn(SignInViewModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest();
			var user = await _userManager.FindByEmailAsync(model.Email);

			if (user is { })
			{
				var flag = await _userManager.CheckPasswordAsync(user, model.Password);
				if (flag)
				{
					var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);

					if (result.IsNotAllowed)
						ModelState.AddModelError(string.Empty, "Your Account is not confimed yet!!");
					if (result.IsLockedOut)
						ModelState.AddModelError(string.Empty, "Your Account is locked!!");

					/// if(result.RequiresTwoFactor)
					/// {
					/// 
					/// }

					if (result.Succeeded)
						return RedirectToAction(nameof(HomeController.Index), "Home");

				}
			}
			else
				ModelState.AddModelError(string.Empty, "Invalid Login Attempt.");

			return View(model);
		}

		#endregion
	}
}
