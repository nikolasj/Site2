using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace todon.ViewModels.Categories
{
    public class CategoriesListViewModel
    {
        public List<CategoryViewModel> Categories { get; set; }
        public bool IsEditable { get; set; }
    }
}
