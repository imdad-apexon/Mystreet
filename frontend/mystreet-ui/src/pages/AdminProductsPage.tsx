import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { productService } from '../services/productService';
import type { Product } from '../types/product';
import { getImageUrl } from '../utils/urlHelper';

export default function AdminProductsPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [deleteId, setDeleteId] = useState<string | null>(null);
  const [search, setSearch] = useState('');
  const [stockFilter, setStockFilter] = useState<'all' | 'in-stock' | 'low-stock' | 'out-of-stock'>('all');

  const load = async () => {
    setLoading(true);
    setProducts(await productService.getAll());
    setLoading(false);
  };

  useEffect(() => {
    load();
  }, []);

  useEffect(() => {
    const onInventoryUpdated = () => {
      load();
    };

    const onVisibilityChange = () => {
      if (document.visibilityState === 'visible') {
        load();
      }
    };

    const onWindowFocus = () => {
      load();
    };

    window.addEventListener('inventory-updated', onInventoryUpdated);
    document.addEventListener('visibilitychange', onVisibilityChange);
    window.addEventListener('focus', onWindowFocus);

    return () => {
      window.removeEventListener('inventory-updated', onInventoryUpdated);
      document.removeEventListener('visibilitychange', onVisibilityChange);
      window.removeEventListener('focus', onWindowFocus);
    };
  }, []);

  const remove = async (id: string) => {
    await productService.remove(id);
    setDeleteId(null);
    load();
  };

  const getStockStatus = (qty: number): { label: string; color: string } => {
    if (qty === 0) return { label: 'Out of Stock', color: '#c0392b' };
    if (qty < 10) return { label: 'Low Stock', color: '#f39c12' };
    return { label: 'In Stock', color: '#27ae60' };
  };

  const truncateName = (name: string) =>
    name.length > 30 ? `${name.slice(0, 30)}...` : name;

  const matchesStockFilter = (qty: number) => {
    if (stockFilter === 'all') return true;
    if (stockFilter === 'out-of-stock') return qty === 0;
    if (stockFilter === 'low-stock') return qty > 0 && qty < 10;
    return qty >= 10;
  };

  const filteredProducts = products.filter(p =>
    (p.name.toLowerCase().includes(search.toLowerCase()) ||
      p.brand.toLowerCase().includes(search.toLowerCase()))
    && matchesStockFilter(p.stockQty)
  );

  return (
    <div className="container">
      <h1>Admin Products</h1>
      
      <div className="admin-header">
        <input
          type="text"
          placeholder="Search by name or brand..."
          value={search}
          onChange={e => setSearch(e.target.value)}
          className="search-input"
        />
        <select
          value={stockFilter}
          onChange={e => setStockFilter(e.target.value as 'all' | 'in-stock' | 'low-stock' | 'out-of-stock')}
          className="search-input"
          aria-label="Filter by stock status"
        >
          <option value="all">All Stock Statuses</option>
          <option value="in-stock">In Stock (10+)</option>
          <option value="low-stock">Low Stock (1-9)</option>
          <option value="out-of-stock">Out of Stock (0)</option>
        </select>
        <button
          type="button"
          className="btn-small btn-secondary"
          onClick={() => {
            setSearch('');
            setStockFilter('all');
          }}
          disabled={!search && stockFilter === 'all'}
        >
          Clear Filters
        </button>
        <Link to="/admin/products/new" className="btn-amazon">
          ➕ Add Product
        </Link>
      </div>

      {loading ? (
        <p>Loading products...</p>
      ) : products.length === 0 ? (
        <div className="empty">
          <p>No products yet.</p>
          <Link to="/admin/products/new" className="btn-amazon" style={{ marginTop: '12px' }}>
            ➕ Add First Product
          </Link>
        </div>
      ) : filteredProducts.length === 0 ? (
        <p className="no-products-found">No Product Found</p>
      ) : (
        <div className="admin-products-table">
          <table>
            <thead>
              <tr>
                <th>Image</th>
                <th>Name</th>
                <th>Brand</th>
                <th>Price</th>
                <th>Stock</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {filteredProducts.map(p => {
                const stockStatus = getStockStatus(p.stockQty);
                const hasImage = Boolean(p.imageUrl?.trim());
                return (
                  <tr key={p.id}>
                    <td className="img-cell">
                      {hasImage ? (
                        <img
                          src={getImageUrl(p.imageUrl)}
                          alt={p.name}
                          className="product-thumb"
                          onError={e => {
                            const target = e.target as HTMLImageElement;
                            target.style.display = 'none';
                            const fallback = target.nextElementSibling as HTMLElement | null;
                            if (fallback) fallback.style.display = 'flex';
                          }}
                        />
                      ) : null}
                      <div
                        className="product-thumb-placeholder"
                        style={{ display: hasImage ? 'none' : 'flex' }}
                      >
                        No image
                      </div>
                    </td>
                    <td>
                      <strong title={p.name}>{truncateName(p.name)}</strong>
                    </td>
                    <td>{p.brand}</td>
                    <td>₹{p.price.toFixed(2)}</td>
                    <td>{p.stockQty}</td>
                    <td>
                      <span
                        className="stock-badge"
                        style={{ backgroundColor: stockStatus.color }}
                      >
                        {stockStatus.label}
                      </span>
                    </td>
                    <td >
                      <Link to={`/admin/products/${p.id}/edit`} className="btn-small btn-primary">
                        ✏️ Edit
                      </Link>
                      &nbsp;&nbsp;
                      <button
                        className="btn-small btn-danger"
                        onClick={() => setDeleteId(p.id)}
                      >
                        🗑️ Delete
                      </button>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      )}

      {deleteId && (
        <div className="modal-overlay" onClick={() => setDeleteId(null)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <h2>Confirm Delete</h2>
            <p>Are you sure you want to delete this product? This action cannot be undone.</p>
            <div className="modal-actions">
              <button className="btn-small btn-secondary" onClick={() => setDeleteId(null)}>
                Cancel
              </button>
              <button
                className="btn-small btn-danger"
                onClick={() => remove(deleteId)}
              >
                Delete
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}