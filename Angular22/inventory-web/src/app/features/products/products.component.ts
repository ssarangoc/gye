import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { ProductService } from './product.service';
import { Product, ProductInput, ProductSupplierInput } from './product.model';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="page">
      <div class="header">
        <h2>Inventario</h2>
        <button (click)="logout()">Cerrar sesión</button>
      </div>

      <form class="card" [formGroup]="form" (ngSubmit)="saveProduct()">
        <h3>{{ editingId ? 'Editar producto' : 'Nuevo producto' }}</h3>
        <div class="grid">
          <input placeholder="Nombre" formControlName="name" />
          <input placeholder="Categoría" formControlName="category" />
          <textarea placeholder="Descripción" formControlName="description"></textarea>
        </div>

        <div class="suppliers-header">
          <h4>Proveedores</h4>
          <button type="button" class="secondary" (click)="addSupplierRow()">Agregar proveedor</button>
        </div>
        <div formArrayName="suppliers">
          <div class="supplier-row" *ngFor="let supplier of suppliers.controls; let i = index" [formGroupName]="i">
            <input placeholder="Nombre proveedor" formControlName="supplierName" />
            <input placeholder="Precio" type="number" formControlName="price" />
            <input placeholder="Stock" type="number" formControlName="stock" />
            <button type="button" class="danger" (click)="removeSupplierRow(i)">Quitar</button>
          </div>
        </div>
        <p class="error" *ngIf="error()">{{ error() }}</p>

        <div class="actions">
          <button type="submit" [disabled]="form.invalid">{{ editingId ? 'Actualizar' : 'Crear' }}</button>
          <button type="button" class="secondary" (click)="resetForm()">Limpiar</button>
        </div>
      </form>

      <div class="card table-card">
        <div class="table-scroll">
        <table>
          <thead>
            <tr><th>Nombre</th><th>Categoría</th><th>Precio</th><th>Stock</th><th>Proveedores</th><th></th></tr>
          </thead>
          <tbody>
            <tr *ngFor="let product of products()">
              <td>{{ product.name }}</td>
              <td>{{ product.category }}</td>
              <td>{{ product.price | currency }}</td>
              <td>{{ product.stock }}</td>
              <td>
                <ul class="supplier-list">
                  <li *ngFor="let supplier of product.suppliers">
                    {{ supplier.supplierName }} — {{ supplier.price | currency }} ({{ supplier.stock }})
                  </li>
                </ul>
              </td>
              <td>
                <button class="secondary" (click)="editProduct(product)">Editar</button>
                <button class="danger" (click)="deleteProduct(product.id!)">Eliminar</button>
              </td>
            </tr>
          </tbody>
        </table>
        </div>
      </div>
    </div>
  `,
  styles: [
    `.page{padding:24px;display:flex;flex-direction:column;gap:16px;background:#f4f6fb;min-height:100vh;}`,
    `.header{display:flex;justify-content:space-between;align-items:center;}`,
    `.card{background:white;padding:20px;border-radius:16px;box-shadow:0 8px 24px rgba(0,0,0,.08);}`,
    `.grid{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:12px;}`,
    `textarea,input{padding:10px;border:1px solid #cbd5e1;border-radius:8px;width:100%;}`,
    `textarea{grid-column:1 / -1;min-height:80px;}`,
    `.suppliers-header{display:flex;justify-content:space-between;align-items:center;margin-top:16px;}`,
    `.supplier-row{display:grid;grid-template-columns:2fr 1fr 1fr auto;gap:10px;margin-top:10px;align-items:center;}`,
    `.supplier-list{margin:0;padding-left:16px;}`,
    `.actions{display:flex;gap:10px;margin-top:12px;}`,
    `button{padding:10px 12px;border:none;border-radius:8px;background:#2563eb;color:white;cursor:pointer;}`,
    `button:disabled{background:#94a3b8;cursor:not-allowed;}`,
    `.secondary{background:#64748b;}`,
    `.danger{background:#dc2626;}`,
    `.error{color:#dc2626;margin-top:8px;}`,
    `.table-scroll{overflow-x:auto;}`,
    `table{width:100%;border-collapse:collapse;min-width:640px;}`,
    `th,td{padding:10px;border-bottom:1px solid #e2e8f0;text-align:left;vertical-align:top;}`,
    `@media (max-width: 720px){
      .page{padding:14px;}
      .header{flex-direction:column;align-items:flex-start;gap:10px;}
      .grid{grid-template-columns:1fr;}
      .supplier-row{grid-template-columns:1fr;}
      .card{padding:14px;}
    }`
  ]
})
export class ProductsComponent implements OnInit {
  private readonly fb = inject(FormBuilder);

  readonly products = signal<Product[]>([]);
  readonly error = signal('');
  editingId?: number;

  readonly form: FormGroup = this.fb.group({
    name: ['', Validators.required],
    category: ['', Validators.required],
    description: [''],
    suppliers: this.fb.array([this.createSupplierGroup()])
  });

  constructor(private productService: ProductService) {}

  get suppliers(): FormArray {
    return this.form.get('suppliers') as FormArray;
  }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.productService.getProducts().subscribe((products) => this.products.set(products));
  }

  createSupplierGroup(supplier?: ProductSupplierInput): FormGroup {
    return this.fb.group({
      supplierId: [supplier?.supplierId],
      supplierName: [{ value: supplier?.supplierName ?? '', disabled: !!supplier?.supplierId }, Validators.required],
      price: [supplier?.price ?? 0, [Validators.required, Validators.min(0)]],
      stock: [supplier?.stock ?? 0, [Validators.required, Validators.min(0)]]
    });
  }

  addSupplierRow(): void {
    this.suppliers.push(this.createSupplierGroup());
  }

  removeSupplierRow(index: number): void {
    this.suppliers.removeAt(index);
  }

  saveProduct(): void {
    this.error.set('');

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload: ProductInput = { ...this.form.getRawValue(), id: this.editingId };
    const onSuccess = () => {
      this.loadProducts();
      this.resetForm();
    };
    const onError = (err: HttpErrorResponse) => this.error.set(err?.error ?? 'Ocurrió un error al guardar el producto');

    if (this.editingId) {
      this.productService.updateProduct(this.editingId, payload).subscribe({ next: onSuccess, error: onError });
    } else {
      this.productService.createProduct(payload).subscribe({ next: onSuccess, error: onError });
    }
  }

  editProduct(product: Product): void {
    this.editingId = product.id;
    this.suppliers.clear();
    product.suppliers.forEach((supplier) => this.suppliers.push(this.createSupplierGroup(supplier)));

    this.form.patchValue({
      name: product.name,
      category: product.category,
      description: product.description
    });
  }

  deleteProduct(id: number): void {
    this.productService.deleteProduct(id).subscribe(() => this.loadProducts());
  }

  resetForm(): void {
    this.editingId = undefined;
    this.error.set('');
    this.form.reset({ name: '', category: '', description: '' });
    this.suppliers.clear();
    this.suppliers.push(this.createSupplierGroup());
  }

  logout(): void {
    localStorage.removeItem('token');
    window.location.href = '/login';
  }
}
