
using ShpAzfService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShpAzfService.MyModels
{
    /// <summary>
    /// utilizzata per recuperare tutte le dropdown dei filtri e dei campi dropdown del task
    /// </summary>
    public class ResultItems
    {
        public List<MyListItems> Items { get; set; } = new List<MyListItems>();
        public int TotalItems { get; set; }
    }    
}
