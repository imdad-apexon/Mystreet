import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { productService } from '../services/productService';
import type { Product } from '../types/product';

export default function AdminProductsPage() {
  const [products, setProducts] = useState<Product[]>([]);

  const load = async () => setProducts(await productService.getAll());

  useEffect(() => {
    load();
  }, []);

  const remove = async (id: string) => {
    await productService.remove(id);
    load();
  };

  return (
    <div className="container">
      <h1>Admin Products</h1>
      <Link to="/admin/products/new">Add Product</Link>
      <div className="list">
        {products.map(p => (
          <div key={p.id} className="list-item">
            <strong>{p.name}</strong>
            <p>{p.brand}</p>
            <p>Stock: {p.stockQty}</p>
            <div className="row">
              <Link to={`/admin/products/${p.id}/edit`}>Edit</Link>
              <button onClick={() => remove(p.id)}>Delete</button>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}