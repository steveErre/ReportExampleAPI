using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShpAzfService.Models
{
    /// <summary>
    /// Gestire il valore dei filtri
    /// </summary>
    public class FilterItems
    {
        public string? SearchInput { get; set; }

        public string SiteUrl { get; set; }
        public string ListName { get; set; }
    }

    public class RequestBody
    {
        public FilterItems Filter { get; set; } = new FilterItems();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SortField { get; set; } = string.Empty;
        public bool IsSortedDescending { get; set; }
    }

}
