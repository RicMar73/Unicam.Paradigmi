using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unicam.Paradigmi.Models;
using Unicam.Paradigmi.Models.Context;
using Unicam.Paradigmi.Models.DTOs;
using Unicam.Paradigmi.Models.Models;

namespace Unicam.Paradigmi.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ResourceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ResourceController(AppDbContext context)
        {
            _context = context;
        }

        // Endpoint per la creazione di una risorsa

        [HttpPost("new")]
        public IActionResult CreateResource([FromBody] CreateResourceWithTypeNameDTO resourceDto)
        {
            if (resourceDto == null)
                return BadRequest("I dati della risorsa sono mancanti.");

            // Validazione del nome della risorsa
            if (string.IsNullOrEmpty(resourceDto.Name))
                return BadRequest("Il nome della risorsa non può essere vuoto.");

            // Validazione del nome del tipo di risorsa
            if (string.IsNullOrEmpty(resourceDto.ResourceTypeName))
                return BadRequest("Il nome del tipo di risorsa non può essere vuoto.");

            // Controllo se il tipo di risorsa esiste
            var resourceType = _context.ResourceTypes
                .FirstOrDefault(rt => rt.Type.ToLower() == resourceDto.ResourceTypeName.ToLower());

            if (resourceType == null)
            {
                // Creazione di un nuovo tipo di risorsa
                resourceType = new ResourceType
                {
                    Type = resourceDto.ResourceTypeName
                };

                _context.ResourceTypes.Add(resourceType);
                _context.SaveChanges(); // Salviamo il nuovo tipo di risorsa nel database
            }

            // Creazione della nuova risorsa (l'ID sarà generato automaticamente)
            var resource = new Resource
            {
                ResourceTypeId = resourceType.Id,
                Name = resourceDto.Name
            };

            _context.Resources.Add(resource);
            _context.SaveChanges();

            return Ok("Risorsa e tipo di risorsa creati con successo.");
        }

        // Endpoint per la ricerca delle risorse disponibili

        [HttpGet("all")]
        public IActionResult GetAllResources()
        {
            // Eseguiamo una query per ottenere tutte le risorse
            var resources = _context.Resources
                                    .Select(r => new
                                    {
                                        r.Id,              // ID della risorsa
                                        r.Name,            // Nome della risorsa
                                        r.ResourceTypeId,  // Tipo di risorsa (ID del tipo)
                                        ResourceTypeName = r.ResourceType.Type // Nome del tipo di risorsa
                                    })
                                    .ToList();  // Eseguiamo la query e la recuperiamo in una lista

            // Se non ci sono risorse, restituiamo un 404
            if (resources == null || !resources.Any())
            {
                return NotFound("Nessuna risorsa trovata.");
            }

            // Restituiamo le risorse trovate
            return Ok(resources);
        }

        // Endpoint per la verifica di risorse disponibili dato il tipo di risorsa e il lasso di tempo

        [HttpPost("availability")]
        public IActionResult GetAvailableResources([FromBody] ResourceAvailabilityRequestDTO request)
        {
            // Controllo che la richiesta non sia nulla
            if (request == null)
                return BadRequest("I dati della richiesta sono mancanti.");

            // Controllo per assicurarsi che le date siano valide
            if (request.StartDate == DateTime.MinValue || request.EndDate == DateTime.MinValue)
                return BadRequest("Le date di inizio e fine devono essere valide.");

            if (request.StartDate >= request.EndDate)
                return BadRequest("La data di inizio deve essere precedente alla data di fine.");

            // Verifica che il nome del tipo di risorsa sia fornito
            if (string.IsNullOrWhiteSpace(request.ResourceTypeName))
                return BadRequest("Il nome del tipo di risorsa è obbligatorio.");

            // Trova il tipo di risorsa corrispondente al nome fornito
            var resourceType = _context.ResourceTypes
                .FirstOrDefault(rt => rt.Type.ToLower() == request.ResourceTypeName.ToLower());

            if (resourceType == null)
            {
                return NotFound($"Nessun tipo di risorsa trovato con il nome '{request.ResourceTypeName}'.");
            }

            // Ricerca delle risorse disponibili
            var query = _context.Resources.AsQueryable();

            // Filtra per tipo di risorsa
            query = query.Where(r => r.ResourceTypeId == resourceType.Id);

            // Trova le risorse disponibili nel periodo specificato
            var availableResources = query
                .Where(r =>
                    !_context.Bookings.Any(b => b.ResourceId == r.Id && b.Start < request.EndDate && b.End > request.StartDate))
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    ResourceType = new
                    {
                        r.ResourceType.Id,
                        r.ResourceType.Type
                    }
                }).ToList();

            // Se nessuna risorsa è disponibile, restituisce 404
            if (!availableResources.Any())
            {
                return NotFound("Nessuna risorsa disponibile per il periodo specificato.");
            }

            // Restituisce le risorse disponibili
            return Ok(availableResources);
        }
    }
}
