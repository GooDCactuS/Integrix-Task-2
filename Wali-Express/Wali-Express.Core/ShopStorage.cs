using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Wali_Express.Core.Models;

namespace Wali_Express.Core
{
    public class ShopStorage
    {
        private readonly DatabaseContext _db;
        public static IHost Host { get; set; }

        public ShopStorage(DatabaseContext context)
        {

            _db = context;
        }

        public async Task<Product> AddProduct(string name, int amount)
        {
            if (await _db.Products.FirstOrDefaultAsync(x => x.Name == name) != null)
            {
                return null;
            }
            var product = new Product { Name = name, Amount = amount };
            return await AddProduct(product);
        }

        public async Task<Product> AddProduct(Product product)
        {
            if (product == null) return null;

            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return product;
        }

        public async Task<Product> GetProduct(string name)
        {
            var product = await _db.Products.FirstOrDefaultAsync(x => x.Name == name);
            return product;
        }


        public async Task<Product> UpdateProduct(string name, int amount)
        {
            var product = await _db.Products.FirstOrDefaultAsync(x => x.Name == name);
            if (product == null)
            {
                return null;
            }

            return await UpdateProduct(product, amount);
        }

        public async Task<Product> UpdateProduct(Product product, int amount)
        {
            if (product == null) return null;

            product.Amount = amount;
            await _db.SaveChangesAsync();
            return product;
        }

        public async Task<Reservation> Reserve(string productName, int amount, string client)
        {
            var product = await _db.Products.FirstAsync(x => x.Name == productName);
            if (product == null || product.Amount < amount)
            {
                return null;
            }

            product.Amount -= amount;

            Reservation reservation = new Reservation { Amount = amount, ProductName = productName, ClientFullName = client};
            return await Reserve(reservation);

        }

        public async Task<Reservation> Reserve(Reservation reservation)
        {
            if (reservation == null) return null;

            _db.Reservations.Add(reservation);
            await _db.SaveChangesAsync();
            return reservation;
        }

        public async void CancelReserve(int id)
        {
            var reservation = await _db.Reservations.FirstAsync(x => x.Id == id);
            if (reservation != null)
            {
                CancelReserve(reservation);
            }
        }

        public async void CancelReserve(Reservation reservation)
        {
            if (reservation == null) return;

            var product = await _db.Products.FirstAsync(x => x.Name == reservation.ProductName);
            if (product != null)
            {
                product.Amount += reservation.Amount;
            }
            _db.Reservations.Remove(reservation);
            await _db.SaveChangesAsync();
        }

        public async Task<int> DeleteAllReserve(string productName)
        {
            var reservations = _db.Reservations.Where(x => x.ProductName == productName);
            _db.Reservations.RemoveRange(reservations);
            await _db.SaveChangesAsync();
            return 0;
        }
    }
}