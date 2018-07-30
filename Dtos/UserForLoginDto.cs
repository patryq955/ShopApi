using System.ComponentModel.DataAnnotations;

namespace ShopApi.Dtos
{
    public class UserForLoginDto
    {
        [Required(ErrorMessage = "Nazwa użytkownika jest wymagana")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "Hasło jest wymagane")]
        [StringLength(40, MinimumLength = 4, ErrorMessage = "Password is too short")]
        public string Password { get; set; }
    }
}