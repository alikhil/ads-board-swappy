using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Swappy_V2.Models
{
    public class ResendEmailConfirmationModel
    {
        [Required(ErrorMessage ="Поле {0} должно быть указано")]
        [EmailAddress]
        public string Email { get; set; }
    }
}