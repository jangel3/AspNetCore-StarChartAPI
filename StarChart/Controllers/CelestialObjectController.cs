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

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            var celestialEntity = _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new {celestialObject.Id}, celestialEntity.Entity);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var celestialBody = _context.CelestialObjects.FirstOrDefault(f => f.Id == id);
            if (celestialBody == null)
            {
                return NotFound();
            }

            celestialBody.Name = celestialObject.Name;
            celestialBody.OrbitalPeriod = celestialObject.OrbitalPeriod;
            celestialBody.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(celestialBody);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialBody = _context.CelestialObjects.Find(id);
            if (celestialBody == null)
            {
                return NotFound();
            }

            celestialBody.Name = name;
            _context.CelestialObjects.Update(celestialBody);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialBodies = _context.CelestialObjects.Where(i => i.Id == id).ToList();
            celestialBodies.AddRange(_context.CelestialObjects.Where(obj=> obj.OrbitedObjectId==id));

            if (celestialBodies.Count == 0)
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(celestialBodies);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
