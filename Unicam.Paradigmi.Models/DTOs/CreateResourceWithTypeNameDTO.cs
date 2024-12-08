using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicam.Paradigmi.Models.DTOs
{
    public class CreateResourceWithTypeNameDTO
    {
        public string Name { get; set; } = string.Empty; // Nome della risorsa
        public string ResourceTypeName { get; set; } = string.Empty; // Nome del tipo di risorsa
    }

}
