using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicam.Paradigmi.Models.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Proprietà di navigazione verso il tipo di risorsa
        public ResourceType ResourceType { get; set; }
        public int ResourceTypeId { get; set; } // FK
    }

}
