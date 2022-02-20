﻿using Ordering.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ordering.UnitTests.Models
{
    public class BasketTests
    {
        [Fact]
        public void Create_new_basket_success()
        {
            //Arrange
            var product1 = new Product
            {
                Id = 1,
                Sku = "TEST_SKU1",
                Name = "Product Name",
                Price = 1
            };

            var product2 = new Product
            {
                Id = 2,
                Sku = "TEST_SKU2",
                Name = "Product Name",
                Price = 1
            };

            var basketItem1 = new BasketItem
            {
                Quantity = 1,
                Product = product1
            };

            var basketItem2 = new BasketItem
            {
                Quantity = 1,
                Product = product2
            };

            var basketItems = new List<BasketItem>();
            basketItems.Add(basketItem1);
            basketItems.Add(basketItem2);

            //Act
            var basket = new Basket
            {
                Id = "Basket1",
                BasketItems = basketItems
            };

            //Assert
            Assert.NotNull(basket);
        }

        [Fact]
        public void Total_new_basket_correct()
        {
            //Arrange
            var product1 = new Product
            {
                Id = 1,
                Sku = "TEST_SKU1",
                Name = "Product Name",
                Price = 5
            };

            var product2 = new Product
            {
                Id = 2,
                Sku = "TEST_SKU2",
                Name = "Product Name",
                Price = 10
            };

            var basketItem1 = new BasketItem
            {
                Quantity = 1,
                Product = product1
            };

            var basketItem2 = new BasketItem
            {
                Quantity = 2,
                Product = product2
            };

            var basketItems = new List<BasketItem>();
            basketItems.Add(basketItem1);
            basketItems.Add(basketItem2);

            //Act
            var basket = new Basket
            {
                Id = "Basket1",
                BasketItems = basketItems
            };

            //Assert
            Assert.NotNull(basket);
            Assert.Equal(25, basket.BasketTotal);
        }
    }
}