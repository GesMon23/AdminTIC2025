using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laboratorio1AdmonTIC.Controllers
{
	[Authorize]
	public class UsuariosController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
		//hay que cmabiar todo a applicationusers
	}
}
