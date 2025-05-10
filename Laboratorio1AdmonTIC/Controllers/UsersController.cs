using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace Laboratorio1AdmonTIC.Controllers
{
	public class UsersController : Controller
	{
		private readonly UserManager<IdentityUser> _userManager;

		public UsersController(UserManager<IdentityUser> userManager)
		{
			_userManager = userManager;
		}

		// Acción para listar usuarios
		public IActionResult Index()
		{
			var users = _userManager.Users.ToList();
			return View(users);
		}
	}
}
