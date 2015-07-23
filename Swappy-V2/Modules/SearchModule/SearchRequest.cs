using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Swappy_V2.Modules
{
    public class SearchRequest
    {
        public string Request { get; set; }
        public List<KeyValuePair<Searchable, double>> FullMatch { get; set; }
        public List<KeyValuePair<Searchable, double>> FullSubstringMatch { get; set; }
        public List<KeyValuePair<Searchable, double>> IncompleteMatch { get; set; }

        public SearchRequest()
        {
            FullMatch = new List<KeyValuePair<Searchable, double>>();
            FullSubstringMatch = new List<KeyValuePair<Searchable, double>>();
            IncompleteMatch = new List<KeyValuePair<Searchable, double>>();
        }
    }
}