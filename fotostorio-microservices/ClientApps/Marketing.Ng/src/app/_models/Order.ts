import { Product } from './Product';

export class Order {
    id!: number;
    buyerEmail!: string;
    orderDate!: Date;
    sendToAddress!: Address;
    items!: OrderItem[];
    total!: number;
    status!: string;
}

export class Address {
    firstName: string = '';
    lastName: string = '';
    street!: string;
    secondLine: string = '';
    city!: string;
    county!: string;
    postCode!: string;
}

export class OrderItem {
    id!: number;
    product!: Product;
    quantity!: number;
    total!: number;
}