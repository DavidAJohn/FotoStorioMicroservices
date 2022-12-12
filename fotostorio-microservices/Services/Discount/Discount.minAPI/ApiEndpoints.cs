namespace Discount.minAPI;

public static class ApiEndpoints
{
    public static void ConfigureApiEndpoints(this WebApplication app)
    {
        // Discount endpoints
        var discounts = app.MapGroup("api/Discounts")
            .AddEndpointFilterFactory((handlerContext, next) =>
            {
                var loggerFactory = handlerContext.ApplicationServices.GetService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("RequestAuditor");

                return (invocationContext) =>
                {
                    logger.LogInformation("Discounts endpoint - received a request for {path}", invocationContext.HttpContext.Request.Path);
                    return next(invocationContext);
                };
            });

        discounts.MapGet("/sku/{sku:maxlength(9)}", GetCurrentDiscountBySku);
        discounts.MapGet("/id/{id:int}", GetCurrentDiscountById);
        discounts.MapGet("/currentfuture", GetCurrentAndFutureDiscounts);
        discounts.MapGet("/current", GetCurrentDiscounts);
        discounts.MapGet("", GetAllDiscounts);
        discounts.MapPost("", CreateDiscount);
        discounts.MapPut("", UpdateDiscount);
        discounts.MapDelete("/{id:int}", DeleteDiscount);

        // Campaign endpoints
        var campaigns = app.MapGroup("api/Campaigns")
            .AddEndpointFilterFactory((handlerContext, next) =>
            {
                var loggerFactory = handlerContext.ApplicationServices.GetService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("RequestAuditor");

                return (invocationContext) =>
                {
                    logger.LogInformation("Campaigns endpoint - received a request for {path}", invocationContext.HttpContext.Request.Path);
                    return next(invocationContext);
                };
            });

        campaigns.MapGet("/id/{id:int}", GetCampaignById);
        campaigns.MapGet("/current", GetCurrentCampaigns);
        campaigns.MapGet("", GetAllCampaigns);
        campaigns.MapPost("", CreateCampaign);
        campaigns.MapPut("", UpdateCampaign);
        campaigns.MapDelete("/{id:int}", DeleteCampaign);
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

    private static async Task<IResult> GetCurrentAndFutureDiscounts(IDiscountData data)
    {
        try
        {
            return Results.Ok(await data.GetCurrentAndFutureDiscounts());
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
