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

		#endregion
	}
}
