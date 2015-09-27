using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Swappy_V2.Models
{
    public class ImageModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }

        public DealModel Deal { get; set; }
    }
}
