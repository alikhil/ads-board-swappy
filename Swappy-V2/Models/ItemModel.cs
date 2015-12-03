using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Swappy_V2.Models
{
    public class ItemModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Поле {0} должно быть указано")]
        [Display(Name="Название предмета")]
        [MaxLength(50, ErrorMessage = "Поле {0} должно иметь не более {1} символов")]
        public string Title { get; set; }

        [Display(Name = "Примечание")]
        [MaxLength(300, ErrorMessage = "Поле {0} должно иметь не более {1} символов")]
        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public DealModel DealModel { get; set; }
    }
}