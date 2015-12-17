using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Swappy_V2.Modules
{
    public class SearchRequest
    {
        /// <summary>
        /// Search query
        /// </summary>
        public string Request { get; set; }

        public List<KeyValuePair<Searchable, double>> FullMatch { get; set; }

        /// <summary>
        /// Список пар объектов в которых SearchBy содержит Request
        /// Не входит в FullMath!
        /// </summary>
        public List<KeyValuePair<Searchable, double>> FullSubstringMatch { get; set; }
        
        /// <summary>
        /// Совпавшие не менее чем на SearchModule.Fuzzyness
        /// </summary>
        public List<KeyValuePair<Searchable, double>> IncompleteMatch { get; set; }

        public SearchRequest()
        {
            FullMatch = new List<KeyValuePair<Searchable, double>>();
            FullSubstringMatch = new List<KeyValuePair<Searchable, double>>();
            IncompleteMatch = new List<KeyValuePair<Searchable, double>>();
        }
    }
}