using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicam.Paradigmi.Models.DTOs
{
    public class ResourceAvailabilityRequestDTO
    {
        public DateTime StartDate { get; set; } // Data di inizio
        public DateTime EndDate { get; set; }   // Data di fine
        public string ResourceTypeName { get; set; } // Nome del tipo di risorsa
    }

}
