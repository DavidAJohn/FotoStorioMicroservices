IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('[dbo].[GetAllDiscounts]'))
EXEC(N'
    CREATE PROCEDURE [dbo].[GetAllDiscounts] 
    AS
    BEGIN
	    SET NOCOUNT ON;

        SELECT p.[Id]
            ,[Sku]
            ,[SalePrice]
            ,c.[Name] as Campaign
            ,p.CampaignId as [CampaignId]
        FROM [ProductDiscounts] p
        INNER JOIN Campaigns c ON p.CampaignId = c.Id
    END
    ')

IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('[dbo].[GetCurrentDiscounts]'))
EXEC(N'
    CREATE PROCEDURE [dbo].[GetCurrentDiscounts]
    AS
    BEGIN
	    SET NOCOUNT ON;

        SELECT p.[Id]
            ,[Sku]
            ,[SalePrice]
            ,c.[Name] as Campaign
            ,p.CampaignId as [CampaignId]
        FROM [ProductDiscounts] p
        INNER JOIN Campaigns c ON p.CampaignId = c.Id
        WHERE c.StartDate < GETDATE() AND c.EndDate > DATEADD(dd,1, GETDATE())
    END
    ')

IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('[dbo].[GetCurrentAndFutureDiscounts]'))
EXEC(N'
    CREATE PROCEDURE [dbo].[GetCurrentAndFutureDiscounts]
    AS
    BEGIN
	    SET NOCOUNT ON;

        SELECT p.[Id]
            ,[Sku]
            ,[SalePrice]
            ,c.[Name] as Campaign
            ,p.CampaignId as [CampaignId]
        FROM [ProductDiscounts] p
        INNER JOIN Campaigns c ON p.CampaignId = c.Id
        WHERE c.EndDate > GETDATE()
    END
    ')

IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('[dbo].[GetCurrentDiscountById]'))
EXEC(N'
    CREATE PROCEDURE [dbo].[GetCurrentDiscountById]
	    @Id int
    AS
    BEGIN
	    SET NOCOUNT ON;

        SELECT p.[Id]
            ,[Sku]
            ,[SalePrice]
            ,[CampaignId]
            ,c.[Name] as [Campaign]
        FROM [ProductDiscounts] p
        INNER JOIN [Campaigns] c ON p.CampaignId = c.Id
        WHERE (c.StartDate < GETDATE() AND c.EndDate > DATEADD(dd,1, GETDATE()))
        AND p.[Id] = @Id
    END
    ')

IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('[dbo].[GetCurrentDiscountBySku]'))
EXEC(N'
    CREATE PROCEDURE [dbo].[GetCurrentDiscountBySku]
	    @Sku nvarchar(50)
    AS
    BEGIN
	    SET NOCOUNT ON;

        SELECT p.[Id]
            ,[Sku]
            ,[SalePrice]
        FROM [ProductDiscounts] p
        INNER JOIN [Campaigns] c ON p.CampaignId = c.Id
        WHERE (c.StartDate < GETDATE() AND c.EndDate > DATEADD(dd,1, GETDATE()))
        AND p.[Sku] = @Sku
    END
    ')

IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('[dbo].[GetDiscountsForSkuByDate]'))
EXEC(N'
    CREATE PROCEDURE [dbo].[GetDiscountsForSkuByDate]
	    @Sku nvarchar(50),
	    @Date datetime
    AS
    BEGIN
	    SET NOCOUNT ON;
	
        SELECT p.[Id]
                ,[Sku]
                ,[SalePrice]
        FROM [ProductDiscounts] p
        INNER JOIN [Campaigns] c ON p.CampaignId = c.Id
        WHERE (c.StartDate < @Date AND c.EndDate > DATEADD(dd,1, @Date))
        AND p.[Sku] = @Sku
    END
    ')

IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('[dbo].[CreateDiscount]'))
EXEC(N'
    CREATE PROCEDURE [dbo].[CreateDiscount]
	    @Sku nvarchar(50),
	    @CampaignId int,
	    @SalePrice decimal(18,2)
    AS
    BEGIN
	    SET NOCOUNT ON;

        INSERT INTO [dbo].[ProductDiscounts]([Sku], [CampaignId], [SalePrice])
        VALUES (@Sku, @CampaignId, @SalePrice)
    END
    ')

IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('[dbo].[UpdateDiscount]'))
EXEC(N'
    CREATE PROCEDURE [dbo].[UpdateDiscount]
	    @Id int,
	    @Sku nvarchar(50),
	    @CampaignId int,
	    @SalePrice decimal(18,2)
    AS
    BEGIN
	    SET NOCOUNT ON;

        UPDATE [dbo].[ProductDiscounts] 
        SET
            [Sku]=@Sku,
            [CampaignId]=@CampaignId,
            [SalePrice]=@SalePrice
        WHERE [Id]=@Id
    END
    ')

IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('[dbo].[DeleteDiscount]'))
EXEC(N'
    CREATE PROCEDURE [dbo].[DeleteDiscount]
	    @Id int
    AS
    BEGIN
	    SET NOCOUNT ON;

        DELETE FROM [dbo].[ProductDiscounts] 
	    WHERE Id = @Id
    END
    ')
