import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

import { Discount, DiscountedProduct, Product } from '@app/_models';
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

  optionsVisible = false;
  optionsText = 'Current Discounts';

  constructor(private discountService: DiscountService, private productService: ProductService) {
    this.getCurrentDiscounts();
  }

  toggleDropdown() {
    this.optionsVisible = !this.optionsVisible;
  }

  handleOptionClick(option: number) {
    this.discounts = [];
    this.discountedProducts = [];

    switch (option) {
      case 0:
        this.getCurrentDiscounts();
        this.optionsText = 'Current Discounts';
        break;
      case 1:
        this.getCurrentAndFutureDiscounts();
        this.optionsText = 'Current & Future Discounts';
        break;
      case 2:
        this.getAllDiscounts();
        this.optionsText = 'All Discounts';
        break;
      default:
        this.getCurrentDiscounts();
        this.optionsText = 'Current Discounts';
        break;
    }

    this.optionsVisible = false;
  }

  getCurrentDiscounts() {
    this.discountService.getCurrentDiscounts()
      .subscribe({
        next: discounts => {
          this.discounts = discounts;

          // Get details of the products that are discounted
          this.discounts.forEach(discount => {
            this.productService.getProductBySku(discount.sku)
              .subscribe({
                next: product => {
                  let discountedProduct = this.createDiscountedProduct(product, discount);
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

  getCurrentAndFutureDiscounts() {
    this.discountService.getCurrentAndFutureDiscounts()
      .subscribe({
        next: discounts => {
          this.discounts = discounts;

          // Get details of the products that are discounted
          this.discounts.forEach(discount => {
            this.productService.getProductBySku(discount.sku)
              .subscribe({
                next: product => {
                  let discountedProduct = this.createDiscountedProduct(product, discount);
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

  getAllDiscounts() {
    this.discountService.getAllDiscounts()
      .subscribe({
        next: discounts => {
          this.discounts = discounts;

          // Get details of the products that are discounted
          this.discounts.forEach(discount => {
            this.productService.getProductBySku(discount.sku)
              .subscribe({
                next: product => {
                  let discountedProduct = this.createDiscountedProduct(product, discount);
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

  private createDiscountedProduct(product: Product, discount: Discount): DiscountedProduct {
    let discountedProduct = new DiscountedProduct();

    discountedProduct.productId = product.id;
    discountedProduct.sku = product.sku;
    discountedProduct.name = product.name;
    discountedProduct.price = product.price;
    discountedProduct.campaign = discount.campaign;
    discountedProduct.imageUrl = product.imageUrl;
    discountedProduct.category = product.category;
    discountedProduct.salePrice = discount.salePrice;

    return discountedProduct;
  }
}
