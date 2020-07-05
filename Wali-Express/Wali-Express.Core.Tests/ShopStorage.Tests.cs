using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wali_Express.Core.Models;

namespace Wali_Express.Core.Tests
{
    [TestClass]
    public class ShopStorageTests
    {
        private static DbContextOptions<DatabaseContext> _options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=waliexpress;Trusted_Connection=True;")
            .Options;
        
        private ShopStorage _shopStorage;
        private string _productName = "Тестовый продукт";
        private readonly AutoResetEvent _waitHandler = new AutoResetEvent(true);

        [TestMethod]
        public void Reserve_10Threads_1000Times_Test()
        {
            var context = new DatabaseContext(_options);
            _shopStorage = new ShopStorage(context);

            var result = _shopStorage.DeleteAllReserve(_productName).Result;

            var product = _shopStorage.AddProduct(_productName, 100).Result ?? _shopStorage.UpdateProduct(_productName, 100).Result;

            Task[] tasks = new Task[10];
            for (int i = 0; i < 10; i++)
            {
                tasks[i] = new Task(Reserve_RndAmount_1000Times);
                tasks[i].Start();
            }

            Task.WaitAll(tasks);
        }

        public void Reserve_RndAmount_1000Times()
        {
            for (int i = 0; i < 1000; i++)
            {
                var reservationAmount = new Random().Next(1, 3);

                _waitHandler.WaitOne();

                var product = _shopStorage.GetProduct(_productName).Result;
                var productAmount = product.Amount;

                var reservation = _shopStorage.Reserve(_productName, reservationAmount, "Task"+Task.CurrentId).Result;

                product = _shopStorage.GetProduct(_productName).Result;
                _waitHandler.Set();
                if (reservation != null)
                {
                    Assert.IsTrue(productAmount - reservationAmount == product.Amount);
                }
            }


        }
    }
}
