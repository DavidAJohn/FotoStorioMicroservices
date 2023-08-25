using Microsoft.AspNetCore.Components;
using Store.BlazorWasm.Models;
using Store.BlazorWasm.Contracts;
using Blazored.SessionStorage;
using Store.BlazorWasm.Shared;

namespace Store.BlazorWasm.Pages.Products;

public partial class Index
{
    [CascadingParameter]
    public ErrorLogger Error { get; set; }

    [Parameter]
    public int brandId { get; set; } = 0;

    [Parameter]
    public int categoryId { get; set; } = 0;

    [Parameter]
    public string searchTerm { get; set; }

    List<Product> products;

    List<DropdownItem> dropdownOptions = new();

    private bool collapseDropdown = true;

    private void ToggleDropdown()
    {
        collapseDropdown = !collapseDropdown;
    }

    private string SortBySelected = "Name (A-Z)";

    private PagingMetadata metadata = new();
    private ProductParameters productParams = new();

    private string errorMessage = "";

    private bool ClearSearch = false;

    public List<Category> Categories = new();
    public List<Brand> Brands = new();

    protected override async Task OnParametersSetAsync()
    {
        if (brandId != 0) await AddBrandFilter();
        if (categoryId != 0) await AddCategoryFilter();

        if (!string.IsNullOrWhiteSpace(searchTerm)) await SearchChanged(searchTerm);

        if (string.IsNullOrEmpty(productParams.Search) && productParams.CategoryId == 0 && productParams.BrandId == 0)
        {
            var storedParams = await GetProductParamsFromSessionStorage();

            if (storedParams != null)
            {
                productParams = storedParams;
            }
        }

        await SelectedPage(productParams.PageIndex);
    }

    protected override async Task OnInitializedAsync()
    {
        await GetCategories();
        await GetBrands();
    }

    protected override void OnInitialized()
    {
        SetSortByDropdownOptions();
    }

    private async Task GetProducts()
    {
        try
        {
            var productsWithMetadata = await productService.GetProductsAsync(productParams);
            products = productsWithMetadata.Items;
            metadata = productsWithMetadata.Metadata;
            errorMessage = "";
        }
        catch (Exception ex)
        {
            Error.ProcessError(ex, "Pages/Products/Index.GetProducts()");
            errorMessage = "Could not retrieve list of products";
        }
    }

    private async Task SelectedPage(int page = 1)
    {
        productParams.PageIndex = page;
        await SaveProductParamsToSessionStorage(productParams);
        await GetProducts();
    }

    private async Task SearchChanged(string searchTerm = "")
    {
        productParams.PageIndex = 1;
        productParams.Search = searchTerm;
        await SaveProductParamsToSessionStorage(productParams);
        await GetProducts();
    }

    private async Task AddSort(string SortBy, string SortByName)
    {
        productParams.SortBy = SortBy;
        SortBySelected = SortByName;
        await SaveProductParamsToSessionStorage(productParams);
        ToggleDropdown();
        await GetProducts();
    }

    private async Task AddBrandFilter()
    {
        productParams.BrandId = brandId;
        productParams.PageIndex = 1;
        await SaveProductParamsToSessionStorage(productParams);
        await GetProducts();
    }

    private async Task AddCategoryFilter()
    {
        productParams.CategoryId = categoryId;
        productParams.PageIndex = 1;
        await SaveProductParamsToSessionStorage(productParams);
        await GetProducts();
    }

    private async Task DeleteDisplayParam(string displayParam)
    {
        if ((string)displayParam == productParams.Search)
        {
            productParams.Search = "";
            productParams.PageIndex = 1;
            await SaveProductParamsToSessionStorage(productParams);
            await GetProducts();
        }
    }

    private async Task DeleteDisplayParam(int displayParam)
    {
        if ((int)displayParam == productParams.CategoryId)
        {
            productParams.CategoryId = 0;
            productParams.PageIndex = 1;
            await GetProducts();
        }

        if ((int)displayParam == productParams.BrandId)
        {
            productParams.BrandId = 0;
            productParams.PageIndex = 1;
            await GetProducts();
        }

        await SaveProductParamsToSessionStorage(productParams);
    }

    private async Task ResetSearch()
    {
        ClearSearch = true;
        await SearchChanged();
    }

    private void SetSortByDropdownOptions()
    {
        dropdownOptions.Add(new DropdownItem { Id = 1, OptionName = "Name (A-Z)", OptionRef = "nameAsc" });
        dropdownOptions.Add(new DropdownItem { Id = 2, OptionName = "Price (Highest to Lowest)", OptionRef = "priceDesc" });
        dropdownOptions.Add(new DropdownItem { Id = 3, OptionName = "Price (Lowest to Highest)", OptionRef = "priceAsc" });
        dropdownOptions.Add(new DropdownItem { Id = 4, OptionName = "Name (Z-A)", OptionRef = "nameDesc" });
    }

    private void GoToHomePage()
    {
        navigationManager.NavigateTo("/");
    }

    private async Task GetCategories()
    {
        Categories = await productService.GetProductCategoriesAsync();
    }

    private async Task GetBrands()
    {
        Brands = await productService.GetProductBrandsAsync();
    }

    private string ReturnCategoryNameFromId(int categoryId)
    {
        var category = Categories.FirstOrDefault(c => c.Id == categoryId);

        return category.Name;
    }

    private string ReturnBrandNameFromId(int brandId)
    {
        var brand = Brands.FirstOrDefault(b => b.Id == brandId);

        return brand.Name;
    }

    private async Task SaveProductParamsToSessionStorage(ProductParameters productParameters)
    {
        await sessionStorage.SetItemAsync("productParams", productParameters);
    }

    private async Task<ProductParameters> GetProductParamsFromSessionStorage()
    {
        var storedParams = await sessionStorage.GetItemAsync<ProductParameters>("productParams");

        return storedParams;
    }
}
