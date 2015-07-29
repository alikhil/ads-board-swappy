using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Swappy_V2.Modules;
using Swappy_V2.Classes;

namespace Swappy_V2.Models
{
    public class DealModel : Searchable
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
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

        [Display(Name="Город")]
        public string City { get; set; }

        public string SearchBy()
        {
            return this.ItemToChange.Title;
        }
    }
}