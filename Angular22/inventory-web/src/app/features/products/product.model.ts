export interface ProductSupplier {
  supplierId: number;
  supplierName: string;
  price: number;
  stock: number;
}

export interface ProductSupplierInput {
  supplierId?: number;
  supplierName?: string;
  price: number;
  stock: number;
}

export interface Product {
  id?: number;
  name: string;
  description?: string;
  category: string;
  price: number;
  stock: number;
  createdAt?: string;
  updatedAt?: string;
  suppliers: ProductSupplier[];
}

export interface ProductInput {
  id?: number;
  name: string;
  description?: string;
  category: string;
  suppliers: ProductSupplierInput[];
}
