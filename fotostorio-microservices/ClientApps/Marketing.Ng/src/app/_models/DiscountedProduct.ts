export class DiscountedProduct {
    productId!: number;
    discountId!: number;
    sku!: string;
    name!: string;
    price!: number;
    salePrice!: number;
    campaignId!: number;
    campaign?: string;
    imageUrl!: string;
    category!: string;
}