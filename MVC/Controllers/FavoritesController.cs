using BLL.Controllers.Bases;
using BLL.DAL;
using BLL.Models;
using BLL.Services.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    [Authorize]
    public class FavoritesController : MvcController
    {
        const string SESSIONKEY = "Favorites";

        private readonly HttpServiceBase _httpService;
        private readonly IService<Pet, PetModel> _petService;

        public FavoritesController(HttpServiceBase httpService, IService<Pet, PetModel> petService)
        {
            _httpService = httpService;
            _petService = petService;
        }

        private int GetUserId() => Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == "Id").Value);

        private List<FavoritesModel> GetSession(int userId)
        {
            var favorites = _httpService.GetSession<List<FavoritesModel>>(SESSIONKEY);
            return favorites?.Where(f => f.UserId == userId).ToList();
        }

        public IActionResult Get()
        {
            return View("List", GetSession(GetUserId()));
        }

        public IActionResult Remove(int petId)
        {
            var favorites = GetSession(GetUserId());
            var favoritesItem = favorites.FirstOrDefault(c => c.PetId == petId);
            favorites.Remove(favoritesItem);
            _httpService.SetSession(SESSIONKEY, favorites);
            return RedirectToAction(nameof(Get));
        }

        // GET: /Favorites/Add?petId=17
        public IActionResult Add(int petId)
        {
            int userId = GetUserId();
            var favorites = GetSession(userId);
            favorites = favorites ?? new List<FavoritesModel>();
            if (!favorites.Any(f => f.PetId == petId))
            {
                var pet = _petService.Query().SingleOrDefault(p => p.Record.Id == petId);
                var favoritesItem = new FavoritesModel()
                {
                    PetId = petId,
                    UserId = userId,
                    PetName = pet.Name
                };
                favorites.Add(favoritesItem);
                _httpService.SetSession(SESSIONKEY, favorites);
                TempData["Message"] = $"\"{pet.Name}\" added to favorites.";
            }
            return RedirectToAction("Index", "Pets");
        }
    }
}
