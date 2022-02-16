using Basket.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Basket.UnitTests.Entities
{
    public class BasketItemTests
    {
        [Fact]
        public void Create_new_basket_item_success()
        {
            //Arrange
            var product = new Product
            {
                Id = 1,
                Sku = "TEST_SKU",
                Name = "Product Name",
                Price = 1,
                ImageUrl = "image.jpg",
                Brand = "Product Brand",
                Category = "Product Category",
                Mount = "Product Mount"
            };

            //Act
            var basketItem = new BasketItem
            {
                Quantity = 1,
                Product = product
            };

            //Assert
            Assert.NotNull(basketItem);
        }

        [Fact]
        public void Total_new_basket_item_correct()
        {
            //Arrange
            var product = new Product
            {
                Id = 1,
                Sku = "TEST_SKU",
                Name = "Product Name",
                Price = 10
            };

            //Act
            var basketItem = new BasketItem
            {
                Quantity = 2,
                Product = product
            };

            //Assert
            Assert.NotNull(basketItem);
            Assert.Equal(20, basketItem.Total);
        }
    }
}
