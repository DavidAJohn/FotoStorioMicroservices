namespace Discount.minAPI;

public static class ApiEndpoints
{
    public static void ConfigureApiEndpoints(this WebApplication app)
    {
        // Discount endpoints
        app.MapGet("api/Discounts/sku/{sku:maxlength(9)}", GetCurrentDiscountBySku);
        app.MapGet("api/Discounts/id/{id:int}", GetCurrentDiscountById);
        app.MapGet("api/Discounts/current", GetCurrentDiscounts);
        app.MapGet("api/Discounts", GetAllDiscounts);
        app.MapPost("api/Discounts", CreateDiscount);
        app.MapPut("api/Discounts", UpdateDiscount);
        app.MapDelete("api/Discounts/{id:int}", DeleteDiscount);

        // Campaign endpoints
        app.MapGet("api/Campaigns/id/{id:int}", GetCampaignById);
        app.MapGet("api/Campaigns/current", GetCurrentCampaigns);
        app.MapGet("api/Campaigns", GetAllCampaigns);
        app.MapPost("api/Campaigns", CreateCampaign);
        app.MapPut("api/Campaigns", UpdateCampaign);
        app.MapDelete("api/Campaigns/{id:int}", DeleteCampaign);
    }

    private static async Task<IResult> GetAllDiscounts(IDiscountData data)
    {
        try
        {
            return Results.Ok(await data.GetAllDiscounts());
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> GetCurrentDiscounts(IDiscountData data)
    {
        try
        {
            return Results.Ok(await data.GetCurrentDiscounts());
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> GetCurrentDiscountBySku(string sku, IDiscountData data)
    {
        try
        {
            var results = await data.GetCurrentDiscountBySku(sku);
            if (results == null) return Results.NotFound();
            return Results.Ok(results);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> GetCurrentDiscountById(int id, IDiscountData data)
    {
        try
        {
            var results = await data.GetCurrentDiscountById(id);
            if (results == null) return Results.NotFound();
            return Results.Ok(results);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> CreateDiscount(ProductDiscount discount, IDiscountData data)
    {
        try
        {
            await data.CreateDiscount(discount);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> UpdateDiscount(ProductDiscount discount, IDiscountData data)
    {
        try
        {
            await data.UpdateDiscount(discount);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> DeleteDiscount(int id, IDiscountData data)
    {
        try
        {
            await data.DeleteDiscount(id);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> GetAllCampaigns(ICampaignData data)
    {
        try
        {
            return Results.Ok(await data.GetAllCampaigns());
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> GetCurrentCampaigns(ICampaignData data)
    {
        try
        {
            return Results.Ok(await data.GetCurrentCampaigns());
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> GetCampaignById(int id, ICampaignData data)
    {
        try
        {
            var results = await data.GetCampaignById(id);
            if (results == null) return Results.NotFound();
            return Results.Ok(results);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> CreateCampaign(Campaign campaign, ICampaignData data)
    {
        try
        {
            await data.CreateCampaign(campaign);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> UpdateCampaign(Campaign campaign, ICampaignData data)
    {
        try
        {
            await data.UpdateCampaign(campaign);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> DeleteCampaign(int id, ICampaignData data)
    {
        try
        {
            await data.DeleteCampaign(id);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
}
