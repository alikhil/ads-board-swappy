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

        [Required(ErrorMessage = "Поле {0} должно быть указано")]
        [Display(Name = "Заголовок объявления")]
        [MaxLength(50, ErrorMessage = "{0} должно иметь не более {1} символов")]
        public string Title { get; set; }

        [Display(Name = "Примечание")]
        [MaxLength(300, ErrorMessage = "{0} должно иметь не более {1} символов")]
        public string Description { get; set; }

        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Поле {0} должно быть указано")]
        [Display(Name = "Варианты обмена")]
        public ICollection<ItemModel> Variants { get; set; }

        [Required]
        [Display(Name="Рассматрю другие варианты")]
        public bool AnotherVariants { get; set; }

        [Display(Name="Город")]
        public string City { get; set; }

        public DateTime DealCreated { get; set; }

        public DateTime? DealUpdated { get; set; }

        public DealState State { get; set; }

        public int Price { get; set; }

        public ICollection<ImageModel> Images { get; set; }

        public string SearchBy()
        {
            return this.Title;
        }
    }

    public enum DealState
    {
        Public,
        Hidden,
        HiddenByAdmin,
        Deleted
    }
}