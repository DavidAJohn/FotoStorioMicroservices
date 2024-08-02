namespace Admin.BlazorServer.Pages.Products;

using Admin.BlazorServer.Contracts;
using System.Net.NetworkInformation;
using Admin.BlazorServer.DTOs;
using Admin.BlazorServer.Models;
using Microsoft.AspNetCore.Components;

public partial class AddProduct
{
    private AddNewProductModel AddProductModel = new AddNewProductModel();

    private List<Brand> brands;
    private List<Category> categories;
    private List<Mount> mounts;

    private string errorMessage = "";

    protected override async Task OnInitializedAsync()
    {
        await GetBrands();
        await GetCategories();
        await GetMounts();
    }

    private async Task GetBrands()
    {
        brands = await productService.GetProductBrandsAsync();
    }

    private async Task GetCategories()
    {
        categories = await productService.GetProductCategoriesAsync();
    }

    private async Task GetMounts()
    {
        mounts = await productService.GetProductMountsAsync();
    }

    private void CheckGenerateSku(ChangeEventArgs obj)
    {
        Console.WriteLine("AddProductModel.BrandId: " + AddProductModel.BrandId);
        Console.WriteLine("AddProductModel.CategoryId: " + AddProductModel.CategoryId);
        Console.WriteLine("AddProductModel.Name: " + AddProductModel.Name);

        if (AddProductModel.BrandId != 0 && AddProductModel.CategoryId != 0 && !string.IsNullOrEmpty(AddProductModel.Name))
        {
            GenerateSku();
        }
    }

    private void GenerateSku()
    {
        // generate a suggested SKU based on brand, category and name

        var brandId = AddProductModel.BrandId;
        var brand = brands.Find(b => b.Id == brandId);

        var categoryId = AddProductModel.CategoryId;
        var category = categories.Find(c => c.Id == categoryId);

        if (brand != null && category != null)
        {
            if (string.IsNullOrWhiteSpace(brand.Name) || string.IsNullOrWhiteSpace(category.Name) || string.IsNullOrWhiteSpace(AddProductModel.Name))
            {
                AddProductModel.Sku = "";
            }
            else
            {
                AddProductModel.Sku = skuService.GenerateSku(brand, category, AddProductModel.Name);
            }
        }
        else
        {
            AddProductModel.Sku = "";
        }
    }

    private async Task SubmitNewProduct()
    {
        var productToCreate = new ProductCreateDTO
        {
            Sku = AddProductModel.Sku,
            Name = AddProductModel.Name,
            Description = AddProductModel.Description,
            Price = AddProductModel.Price,
            ImageUrl = AddProductModel.ImageUrl,
            BrandId = AddProductModel.BrandId,
            CategoryId = AddProductModel.CategoryId,
            MountId = AddProductModel.MountId
        };

        var createdProduct = await productService.CreateProductAsync(productToCreate);

        if (createdProduct != null)
        {
            // post a stock entry to Inventory.API, setting initial stock
            var newStock = new ProductStock
            {
                Sku = createdProduct.Sku,
                Name = createdProduct.Name,
                CurrentStock = 0, // the subsequent update will add the correct stock level
                LastUpdated = DateTime.Now
            };

            var newStockResponse = await inventoryService.CreateNewStock(newStock);

            if (newStockResponse != null && newStockResponse.Successful)
            {
                // post an update to Inventory.API
                var newUpdate = new UpdateStockModel
                {
                    Sku = createdProduct.Sku,
                    Added = AddProductModel.Stock,
                    Removed = 0
                };

                var newUpdateResponse = await inventoryService.CreateNewUpdate(newUpdate);

                if (newUpdateResponse != null && newUpdateResponse.Successful)
                {
                    errorMessage = "";
                    toastService.ShowSuccess($"'{createdProduct.Name}' has now been added to the product range", "Success");
                }
                else
                {
                    errorMessage = "There was a problem adding this product";
                    toastService.ShowError("There was a problem adding this product", "Error");
                }
            }
            else
            {
                errorMessage = "There was a problem adding this product";
                toastService.ShowError("There was a problem adding this product", "Error");
            }
        }
        else
        {
            errorMessage = "There was a problem adding this product";
            toastService.ShowError("There was a problem adding this product", "Error");
        }
    }

    private void Cancel()
    {
        navigationManager.NavigateTo("sitestatus");
    }
}
