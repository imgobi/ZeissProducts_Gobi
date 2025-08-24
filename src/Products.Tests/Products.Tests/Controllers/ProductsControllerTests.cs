using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Products.Api.Controllers;
using Products.Api.Data;
using Products.Api.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Products.Tests
{
    public class ProductsControllerTests
    {
        private AppDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var dbContext = new AppDbContext(options);

            if (!dbContext.Products.Any())
            {
                dbContext.Products.AddRange(
                    new Product { ProductId = 1, Name = "Test Product 1",  StockAvailable = 5 },
                    new Product { ProductId = 2, Name = "Test Product 2", StockAvailable = 10 }
                );
                dbContext.SaveChanges();
            }

            return dbContext;
        }

        [Fact]
        public async Task GetProducts_ReturnsAllProducts()
        {
            var context = GetDbContext("GetProductsTest");
            var controller = new ProductsController(context);

            var result = await controller.GetProducts();

            var actionResult = Assert.IsType<ActionResult<IEnumerable<Product>>>(result);
            var returnValue = Assert.IsType<List<Product>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetProduct_ReturnsCorrectProduct()
        {
            var context = GetDbContext("GetProductTest");
            var controller = new ProductsController(context);

            var result = await controller.GetProduct(1);

            var actionResult = Assert.IsType<ActionResult<Product>>(result);
            var product = Assert.IsType<Product>(actionResult.Value);
            Assert.Equal("Test Product 1", product.Name);
        }

        [Fact]
        public async Task GetProduct_ReturnsNotFound_WhenIdDoesNotExist()
        {
            var context = GetDbContext("GetProductNotFoundTest");
            var controller = new ProductsController(context);

            var result = await controller.GetProduct(99);

            var actionResult = Assert.IsType<ActionResult<Product>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task PostProduct_CreatesProduct()
        {
            var context = GetDbContext("PostProductTest");
            var controller = new ProductsController(context);

            var newProduct = new Product { Name = "New Product", StockAvailable = 20 };

            var result = await controller.PostProduct(newProduct);

            var actionResult = Assert.IsType<ActionResult<Product>>(result);
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var product = Assert.IsType<Product>(createdResult.Value);
            Assert.Equal("New Product", product.Name);
            Assert.Equal(3, context.Products.Count());
        }

        [Fact]
        public async Task PutProduct_UpdatesProduct()
        {
            var context = GetDbContext("PutProductTest");
            var controller = new ProductsController(context);

            var existingProduct = await context.Products.FindAsync(1);

            context.Entry(existingProduct).State = EntityState.Detached;

            var updatedProduct = new Product
            {
                ProductId = 1,  
                Name = "Updated Product",
                Description = "Updated Description",
                Price = 2000,
                StockAvailable = 20
            };

            var result = await controller.PutProduct(1, updatedProduct);

            Assert.IsType<NoContentResult>(result);

            var productInDb = await context.Products.FindAsync(1);
            Assert.NotNull(productInDb);
            Assert.Equal("Updated Product", productInDb.Name);
            Assert.Equal("Updated Description", productInDb.Description);
            Assert.Equal(2000, productInDb.Price);
            Assert.Equal(20, productInDb.StockAvailable);
        }

        [Fact]
        public async Task PutProduct_ReturnsBadRequest_WhenIdMismatch()
        {
            var context = GetDbContext("PutProductBadRequestTest");
            var controller = new ProductsController(context);

            var updatedProduct = new Product { ProductId = 99,  Name = "Mismatch", StockAvailable = 5 };

            var result = await controller.PutProduct(1, updatedProduct);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_RemovesProduct()
        {
            var context = GetDbContext("DeleteProductTest");
            var controller = new ProductsController(context);

            var result = await controller.DeleteProduct(1);

            Assert.IsType<NoContentResult>(result);
            Assert.Null(await context.Products.FindAsync(1));
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNotFound_WhenIdDoesNotExist()
        {
            var context = GetDbContext("DeleteProductNotFoundTest");
            var controller = new ProductsController(context);

            var result = await controller.DeleteProduct(99);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddToStock_IncreasesStock()
        {
            var context = GetDbContext("AddToStockTest");
            var controller = new ProductsController(context);

            var result = await controller.AddToStock(1, 5);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var product = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(10, product.StockAvailable); // 5 + 5
        }

        public async Task DecrementStock_DecreasesStock()
        {
            var context = GetDbContext("DecrementStockTest");
            var controller = new ProductsController(context);

            var result = await controller.DecrementStock(1, 3);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var product = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(2, product.StockAvailable); // original 5 - 3
        }
    }
}
