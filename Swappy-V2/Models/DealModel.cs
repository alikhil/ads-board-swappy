using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Swappy_V2.Models
{
    public class DealModel
    {
        public int Id { get; set; }
        public int AppUserModelId { get; set; }
        public int? ItemToChangeId { get; set; }

        [Required]
        [Display(Name = "Вещь которую вы хотите обменять")]
        public ItemModel ItemToChange { get; set; }

        [Required]
        [Display(Name = "Варианты обмена")]
        public ICollection<ItemModel> Variants { get; set; }

        [Required]
        [Display(Name="Рассматрю другие варианты")]
        public bool AnotherVariants { get; set; }

        [Required]
        [Display(Name="Город")]
        public string City { get; set; }
    }
}