﻿namespace Products.API.Contracts;

public interface IProductRepository : IRepositoryBase<Product>
{
    Task<Product> GetBySkuAsync(string sku);
}
