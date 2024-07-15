import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

import { Discount, DiscountedProduct } from '@app/_models';
import { DiscountService, ProductService } from '@app/_services';

@Component({
  selector: 'app-discount-list',
  standalone: true,
  imports: [RouterLink, CommonModule],
  templateUrl: './discount-list.component.html',
  styleUrl: './discount-list.component.css'
})
export class DiscountListComponent {
  errorMessage = '';
  discounts: Discount[] = [];
  discountedProducts: DiscountedProduct[] = [];

  constructor(private discountService: DiscountService, private productService: ProductService) {
    this.discountService.getCurrentDiscounts()
      .subscribe({
        next: discounts => {
          this.discounts = discounts;

          // Get details of the products that are discounted
          this.discounts.forEach(discount => {
            this.productService.getProductBySku(discount.sku)
              .subscribe({
                next: product => {
                  let discountedProduct = new DiscountedProduct();
                  discountedProduct.productId = product.id;
                  discountedProduct.discountId = discount.id;
                  discountedProduct.sku = product.sku;
                  discountedProduct.name = product.name;
                  discountedProduct.price = product.price;
                  discountedProduct.imageUrl = product.imageUrl;
                  discountedProduct.category = product.category;
                  discountedProduct.campaignId = discount.campaignId;
                  discountedProduct.campaign = discount.campaign;
                  discountedProduct.salePrice = product.salePrice;

                  this.discountedProducts.push(discountedProduct);
                },
                error: error => {
                  console.error('Error receiving discounted product: ', error);
                  this.errorMessage = 'Error receiving discounted product: ' + error;
                }
              });
          });
        },
        error: error => {
          console.error('Error receiving discounts: ', error);
          this.errorMessage = 'Error receiving discounts: ' + error;
        }
      });
  }
}
