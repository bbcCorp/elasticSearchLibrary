using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace elasticSearchLibrary.Core.Model
{
    public class AdvancedSearchFilter
    {
        public String SearchField { get; set; }
        public String SearchQuery { get; set; }
        public bool MatchCriteria { get; set; }
    }
}
