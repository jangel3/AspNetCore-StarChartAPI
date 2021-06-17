using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        [ActionName("GetById")]
        public IActionResult GetById(int id)
        {
            var celestialBody = _context.CelestialObjects.FirstOrDefault(f=> f.Id==id);
            if (celestialBody == null)
            {
                return NotFound();
            }

            if (!celestialBody.OrbitedObjectId.HasValue)
            {
                celestialBody.OrbitedObjectId = celestialBody.Id;
                celestialBody.Satellites = new List<CelestialObject> {celestialBody};
            }

            return Ok(celestialBody);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialBodies = _context.CelestialObjects.Where(f => f.Name == name).ToList();
            if (celestialBodies.Count == 0)
            {
                return NotFound();
            }

            foreach (var celestialBody in celestialBodies)
            {
                if (!celestialBody.OrbitedObjectId.HasValue)
                {
                    celestialBody.OrbitedObjectId = celestialBody.Id;
                    celestialBody.Satellites = new List<CelestialObject> {celestialBody};
                }
            }

            return Ok(celestialBodies);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialBodies = _context.CelestialObjects;

            foreach (var celestialObject in celestialBodies)
            {
                if (!celestialObject.OrbitedObjectId.HasValue)
                {
                    celestialObject.OrbitedObjectId = celestialObject.Id;
                    celestialObject.Satellites = new List<CelestialObject> {celestialObject};
                }
            }

            return Ok(celestialBodies);
        }
    }
}
