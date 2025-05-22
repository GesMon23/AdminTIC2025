using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Laboratorio1AdmonTIC.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Laboratorio1AdmonTIC.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UsersController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        //GET
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, IdentityUser model)
        {
            if (id != model.Id) {
                TempData["Error"] = "Ocurrio un error al actualizar.";
                return RedirectToAction(nameof(Edit), new { id = id });
            }
                

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.Email = model.Email;
            user.UserName = model.UserName;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Usuario actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

       

        // ELIMINAR - POST
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Usuario eliminado correctamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar el usuario.";
            }

            return RedirectToAction("Index");
        }



        // GET: Users/CambiarPassword/{id}
        [HttpGet]
        public async Task<IActionResult> CambiarPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var model = new CambiarPasswordViewModel
            {
                UserId = user.Id,
                UserName = user.UserName
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPassword(CambiarPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                Console.WriteLine("Error en user == null");
                return NotFound();
            }

            var removeResult = await _userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
            {
                //ModelState.AddModelError("", "Error al eliminar la contraseña anterior.");
                Console.WriteLine("Error en !removeResult.Succeeded");
                TempData["Error"] = "No se pudo actualizar la contraseña.";
                return RedirectToAction("Index");
            }

            var addResult = await _userManager.AddPasswordAsync(user, model.NuevaPassword);
            if (!addResult.Succeeded)
            {
                foreach (var error in addResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                TempData["Error"] = "No se pudo actualizar la contraseña.";
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Contraseña actualizada correctamente.";
            return RedirectToAction("Index");
        }


    }
}
