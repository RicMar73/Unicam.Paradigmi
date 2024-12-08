using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicam.Paradigmi.Models.Models
{
    public class BookingRequest
    {
        public int ResourceId { get; set; } // ID della risorsa da prenotare
        public DateTime Start { get; set; } // Data di inizio prenotazione
        public DateTime End { get; set; } // Data di fine prenotazione
    }

}
