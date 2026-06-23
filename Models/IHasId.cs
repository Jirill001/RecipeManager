using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeManager.Models
{
    public interface IHasId
    {
        string Id { get; set; }
    }
}
