using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wali_Express.Core.Models;

namespace Wali_Express.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly DatabaseContext _db;

        public ReservationsController(DatabaseContext context)
        {
            _db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> Get()
        {
            return await _db.Reservations.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> Get(int id)
        {
            Reservation reservation = await _db.Reservations.FirstOrDefaultAsync(x => x.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }
            return new ObjectResult(reservation);
        }

        [HttpPost]
        public async Task<ActionResult<Reservation>> Post(Reservation reservation)
        {
            if (reservation == null)
            {
                return BadRequest();
            }

            var product = _db.Products.FirstOrDefault(x => x.Name == reservation.ProductName);
            if (product == null)
            {
                return NotFound();
            }

            ShopStorage shopStorage = new ShopStorage(_db);
            await shopStorage.Reserve(reservation.ProductName, reservation.Amount, reservation.ClientFullName);


            await _db.SaveChangesAsync();
            return Ok(reservation);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Reservation>> Delete(int id)
        {
            ShopStorage shopStorage = new ShopStorage(_db);
            shopStorage.CancelReserve(id);

            return Ok();
        }
    }
}