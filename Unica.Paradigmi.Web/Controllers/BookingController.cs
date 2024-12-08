using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unicam.Paradigmi.Models.Models;
using Unicam.Paradigmi.Models.Context;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Unicam.Paradigmi.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("api/Booking")]
        public IActionResult CreateBooking([FromBody] BookingRequest request)
        {
            // Controlla se la richiesta è valida
            if (request == null)
                return BadRequest("I dati della prenotazione sono mancanti.");

            // Controllo che il ResourceId sia valido
            if (request.ResourceId <= 0)
                return BadRequest("Il ResourceId deve essere maggiore di 0.");

            // Controllo che la data di inizio sia inferiore alla data di fine
            if (request.Start >= request.End)
                return BadRequest("La data di inizio deve essere prima della data di fine.");

            // Verifica che la data di inizio sia nel futuro
            if (request.Start < DateTime.UtcNow)
                return BadRequest("La data di inizio deve essere nel futuro.");

            // Verifica che la risorsa esista nel database
            if (!_context.Resources.Any(r => r.Id == request.ResourceId))
                return NotFound("La risorsa specificata non esiste.");

            // Verifica disponibilità della risorsa nel periodo richiesto
            var isAvailable = !_context.Bookings.Any(b =>
                b.ResourceId == request.ResourceId &&
                (b.Start < request.End && request.Start < b.End)); // Controlla sovrapposizione

            if (!isAvailable)
            {
                return Conflict("La risorsa non è disponibile per l'intervallo di date specificato.");
            }

            // Creazione della prenotazione
            var booking = new Booking
            {
                ResourceId = request.ResourceId,
                Start = request.Start,
                End = request.End
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return Ok("Prenotazione effettuata con successo.");
        }

        // Endpoint per ottenere le prenotazioni
        [HttpGet]
        [Route("api/Booking")]
        public IActionResult GetBookings(int? resourceId)
        {
            // Iniziamo la query per recuperare le prenotazioni
            var query = _context.Bookings.AsQueryable();

            // Se viene fornito un resourceId, applica il filtro per quella risorsa
            if (resourceId.HasValue)
            {
                query = query.Where(b => b.ResourceId == resourceId.Value);
            }

            // Eseguiamo il join con la tabella delle risorse per ottenere il nome della risorsa
            var bookings = query
                .Include(b => b.Resource) // Include la risorsa per accedere al nome
                .Select(b => new
                {
                    b.Id,               // ID della prenotazione
                    b.ResourceId,       // ID della risorsa
                    b.Start,            // Data di inizio
                    b.End,              // Data di fine
                    ResourceName = b.Resource.Name // Nome della risorsa
                })
                .ToList();  // Eseguiamo la query e la recuperiamo in una lista

            // Restituiamo le prenotazioni
            return Ok(bookings);
        }

    }
}
