using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wali_Express.Core.Models;

namespace Wali_Express.Core
{
    public class ShopStorage
    {
        private readonly DatabaseContext _db;


        public ShopStorage(DatabaseContext context)
        {
            _db = context;
        }

        /// <summary>
        /// Adds a product to the database.
        /// </summary>
        /// <param name="name">A name of the product.</param>
        /// <param name="amount">An amount of the product.</param>
        /// <returns>Returns a product object if everything is okay, returns null otherwise.</returns>
        public async Task<Product> AddProduct(string name, int amount)
        {
            if (await _db.Products.FirstOrDefaultAsync(x => x.Name == name) != null)
            {
                return null;
            }
            var product = new Product { Name = name, Amount = amount };
            return await AddProduct(product);
        }

        /// <summary>
        /// Adds a product to the database.
        /// </summary>
        /// <param name="product">A product object.</param>
        /// <returns>Returns a product object if everything is okay, returns null otherwise</returns>
        public async Task<Product> AddProduct(Product product)
        {
            if (product == null) return null;

            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return product;
        }

        /// <summary>
        /// Returns a product from the database.
        /// </summary>
        /// <param name="name">A name of the product.</param>
        /// <returns>Returns a product if it exists.</returns>
        public async Task<Product> GetProduct(string name)
        {
            var product = await _db.Products.FirstOrDefaultAsync(x => x.Name == name);
            return product;
        }

        /// <summary>
        /// Updates product in the database.
        /// </summary>
        /// <param name="name">A name of the product.</param>
        /// <param name="amount">An amount of the product.</param>
        /// <returns>Returns a product if everything is okay, returns null otherwise.</returns>
        public async Task<Product> UpdateProduct(string name, int amount)
        {
            var product = await _db.Products.FirstOrDefaultAsync(x => x.Name == name);
            if (product == null)
            {
                return null;
            }

            return await UpdateProduct(product, amount);
        }

        /// <summary>
        /// Updates a product in the database.
        /// </summary>
        /// <param name="product">The Product object.</param>
        /// <param name="amount">An amount of the product.</param>
        /// <returns>Returns a product object if everything is okay, returns null otherwise.</returns>
        public async Task<Product> UpdateProduct(Product product, int amount)
        {
            if (product == null) return null;

            product.Amount = amount;
            await _db.SaveChangesAsync();
            return product;
        }

        /// <summary>
        /// Adds a new reservation to the database.
        /// </summary>
        /// <param name="productName">A name of the product.</param>
        /// <param name="amount">An amount of the product.</param>
        /// <param name="client">A client's fullname.</param>
        /// <returns>Returns a Reservation object if everything is okay, returns null otherwise.</returns>
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

        /// <summary>
        /// Adds a new reservation to the database.
        /// </summary>
        /// <param name="reservation">A reservation object.</param>
        /// <returns>Returns a reservation object if everything is okay, returns null otherwise.</returns>
        public async Task<Reservation> Reserve(Reservation reservation)
        {
            if (reservation == null) return null;

            _db.Reservations.Add(reservation);
            await _db.SaveChangesAsync();
            return reservation;
        }

        /// <summary>
        /// Cancels reservation in the database.
        /// </summary>
        /// <param name="id">Id of the reservation</param>
        public async void CancelReserve(int id)
        {
            var reservation = await _db.Reservations.FirstAsync(x => x.Id == id);
            if (reservation != null)
            {
                CancelReserve(reservation);
            }
        }

        /// <summary>
        /// Cancels reservation in the database.
        /// </summary>
        /// <param name="reservation">A reservation object.</param>
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

        /// <summary>
        /// Deletes all reservation with specific product name.
        /// </summary>
        /// <param name="productName">A name of the product.</param>
        /// <returns>Returns 0 if everything is okay.</returns>
        public async Task<int> DeleteAllReserve(string productName)
        {
            var reservations = _db.Reservations.Where(x => x.ProductName == productName);
            _db.Reservations.RemoveRange(reservations);
            await _db.SaveChangesAsync();
            return 0;
        }
    }
}