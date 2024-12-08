using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicam.Paradigmi.Models.Models
{
    public class Booking
    {
        public int Id { get; set; } // ID della prenotazione (chiave primaria)
        public int ResourceId { get; set; } // FK verso la risorsa
        public DateTime Start { get; set; } // Data inizio prenotazione
        public DateTime End { get; set; } // Data fine prenotazione

        // Proprietà di navigazione per la risorsa
        public Resource Resource { get; set; }  // Relazione con la risorsa
    }


}
