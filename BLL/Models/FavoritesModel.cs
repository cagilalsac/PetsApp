using System.ComponentModel;

namespace BLL.Models
{
    public class FavoritesModel
    {
        public int PetId { get; set; }
        public int UserId { get; set; }

        [DisplayName("Pet Name")]
        public string PetName { get; set; }
    }
}
