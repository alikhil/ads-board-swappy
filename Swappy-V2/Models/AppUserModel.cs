using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Swappy_V2.Models
{
    public class AppUserModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Имя")]
        [MaxLength(50, ErrorMessage = "{0} должно иметь не более 50 символов")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Фамилия")]
        public string Surname { get; set; }

        [Required]
        [Display(Name = "Номер телефона")]
        [RegularExpression(@"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$", ErrorMessage = "Укажите корректный номер телефона")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Город")]
        public string City { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Адрес электронной почты")]
        public string Email { get; set; }

    }
}