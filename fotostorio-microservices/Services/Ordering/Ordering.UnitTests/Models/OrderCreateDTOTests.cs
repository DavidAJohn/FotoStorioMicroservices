using Ordering.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ordering.UnitTests.Models
{
    public class OrderCreateDTOTests
    {
        [Fact]
        public void Create_new_OrderCreateDTO_NotNull()
        {
            //Arrange
            var sendToAddress = new Address
            {
                FirstName = "John",
                LastName = "Doe",
                Street = "1 Street Name",
                SecondLine = "",
                City = "City",
                County = "County",
                PostCode = "PC1 9XL"
            };

            var product1 = new ProductItemOrdered
            {
                Id = 1,
                Sku = "TEST_SKU1",
                Name = "Product Name",
                Price = 1,
                ImageUrl = "image.jpg"
            };

            var product2 = new ProductItemOrdered
            {
                Id = 2,
                Sku = "TEST_SKU2",
                Name = "Product Name",
                Price = 1,
                ImageUrl = "image.jpg"
            };

            var orderItem1 = new OrderItem
            {
                Quantity = 1,
                Product = product1
            };

            var orderItem2 = new OrderItem
            {
                Quantity = 1,
                Product = product2
            };

            var orderItems = new List<OrderItem>();
            orderItems.Add(orderItem1);
            orderItems.Add(orderItem2);

            //Act
            var order = new OrderCreateDTO
            {
                Items = orderItems,
                SendToAddress = sendToAddress
            };

            //Assert
            Assert.NotNull(order);
        }
    }
}
