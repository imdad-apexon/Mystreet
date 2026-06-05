import { useEffect, useState } from 'react';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { productService } from '../services/productService';

interface ProductForm {
  name: string;
  brand: string;
  description: string;
  price: number;
  sizesCsv: string;
  stockQty: number;
  imageUrl: string;
  category: string;
}

export default function AdminProductFormPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const isEdit = Boolean(id);

  const [loading, setLoading] = useState(isEdit);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  const [form, setForm] = useState<ProductForm>({
    name: '',
    brand: '',
    description: '',
    price: 0,
    sizesCsv: '7,8,9,10',
    stockQty: 0,
    imageUrl: '',
    category: 'Sneakers'
  });

  useEffect(() => {
    if (!id) return;

    const loadProduct = async () => {
      try {
        setLoading(true);
        setError('');

        const p = await productService.getById(id);

        setForm({
          name: p.name ?? '',
          brand: p.brand ?? '',
          description: p.description ?? '',
          price: p.price ?? 0,
          sizesCsv: p.sizesCsv ?? '7,8,9,10',
          stockQty: p.stockQty ?? 0,
          imageUrl: p.imageUrl ?? '',
          category: p.category ?? 'Sneakers'
        });
      } catch (err) {
        console.error(err);
        setError('Failed to load product');
      } finally {
        setLoading(false);
      }
    };

    loadProduct();
  }, [id]);

  const submit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    setError('');
    setSubmitting(true);

    try {
      if (!form.name.trim() || !form.brand.trim()) {
        setError('Name and brand are required');
        return;
      }

      if (form.price < 0) {
        setError('Price must be positive');
        return;
      }

      if (form.stockQty < 0) {
        setError('Stock quantity must be non-negative');
        return;
      }

      if (isEdit && id) {
        await productService.update(id, form);
      } else {
        await productService.create(form);
      }

      navigate('/admin/products');
    } catch (err) {
      console.error(err);
      setError('Failed to save product. Please try again.');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="container form-page">
      <Link to="/admin/products" className="back-link">
        ← Back to Products
      </Link>

      <h1>{isEdit ? 'Edit Product' : 'Add Product'}</h1>

      {loading ? (
        <p>Loading product...</p>
      ) : (
        <form onSubmit={submit} className="admin-form">
          {error && <p className="error">{error}</p>}

          <div className="form-group">
            <label htmlFor="name">Product Name *</label>
            <input
              id="name"
              placeholder="e.g., Nike Air Force 1"
              value={form.name}
              onChange={(e) =>
                setForm({ ...form, name: e.target.value })
              }
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="brand">Brand *</label>
            <input
              id="brand"
              placeholder="e.g., Nike, Adidas, Puma"
              value={form.brand}
              onChange={(e) =>
                setForm({ ...form, brand: e.target.value })
              }
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="category">Category</label>
            <select
              id="category"
              value={form.category}
              onChange={(e) =>
                setForm({ ...form, category: e.target.value })
              }
            >
              <option value="Sneakers">Sneakers</option>
              <option value="Boots">Boots</option>
              <option value="Sandals">Sandals</option>
              <option value="Casual">Casual</option>
              <option value="Sports">Sports</option>
              <option value="Formal">Formal</option>
            </select>
          </div>

          <div className="form-group">
            <label htmlFor="description">Description</label>
            <textarea
              id="description"
              placeholder="Product description, features, materials..."
              value={form.description}
              onChange={(e) =>
                setForm({ ...form, description: e.target.value })
              }
              rows={4}
            />
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="price">Price (₹) *</label>
              <input
                id="price"
                type="number"
                placeholder="0.00"
                value={form.price}
                onChange={(e) =>
                  setForm({
                    ...form,
                    price: Number(e.target.value)
                  })
                }
                step="0.01"
                min="0"
                required
              />
            </div>

            <div className="form-group">
              <label htmlFor="stockQty">Stock Quantity *</label>
              <input
                id="stockQty"
                type="number"
                placeholder="0"
                value={form.stockQty}
                onChange={(e) =>
                  setForm({
                    ...form,
                    stockQty: Number(e.target.value)
                  })
                }
                min="0"
                required
              />
            </div>
          </div>

          <div className="form-group">
            <label htmlFor="sizesCsv">
              Available Sizes (comma-separated)
            </label>
            <input
              id="sizesCsv"
              placeholder="e.g., 7,8,9,10,11,12"
              value={form.sizesCsv}
              onChange={(e) =>
                setForm({
                  ...form,
                  sizesCsv: e.target.value
                })
              }
            />
          </div>

          <div className="form-group">
            <label htmlFor="imageUrl">Image URL</label>
            <input
              id="imageUrl"
              placeholder="https://example.com/image.jpg"
              value={form.imageUrl}
              onChange={(e) =>
                setForm({
                  ...form,
                  imageUrl: e.target.value
                })
              }
            />

            {form.imageUrl && (
              <div className="image-preview">
                <img
                  src={form.imageUrl}
                  alt="Preview"
                  onError={(e) => {
                    e.currentTarget.style.display = 'none';
                  }}
                />
              </div>
            )}
          </div>

          <div className="form-actions">
            <Link
              to="/admin/products"
              className="btn-small btn-secondary"
            >
              Cancel
            </Link>

            <button
              type="submit"
              className="btn-amazon"
              disabled={submitting}
            >
              {submitting
                ? 'Saving...'
                : isEdit
                ? '💾 Update Product'
                : '➕ Add Product'}
            </button>
          </div>
        </form>
      )}
    </div>
  );
}