import { useEffect, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { productService } from '../services/productService';
import type { Product } from '../types/product';
import { useCart } from '../context/CartContext';
import { useAuth } from '../context/AuthContext';
import { getImageUrl } from '../utils/urlHelper';


export default function ProductDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { addItem } = useCart();
  const { isAuthenticated } = useAuth();

  const [product, setProduct] = useState<Product | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [size, setSize] = useState('');
  const [sizeError, setSizeError] = useState('');
  const [quantity, setQuantity] = useState(1);
  const [imageError, setImageError] = useState(false);

  useEffect(() => {
    if (id) {
      setLoading(true);
      setError('');
      productService.getById(id)
        .then(setProduct)
        .catch(() => setError('Product not found'))
        .finally(() => setLoading(false));
    }
  }, [id]);

  useEffect(() => {
    setImageError(false);
  }, [product?.id]);

  if (!product && !loading) {
    return (
      <div className="container detail-page">
        <Link to="/products" className="back-link">← Back to Products</Link>
        <p className="error">{error || 'Product not found'}</p>
      </div>
    );
  }

  if (loading) {
    return (
      <div className="container detail-page">
        <Link to="/products" className="back-link">← Back to Products</Link>
        <p>Loading product details...</p>
      </div>
    );
  }

  const sizes = product!.sizesCsv.split(',');
  const hasImage = Boolean(product!.imageUrl?.trim()) && !imageError;

  const handleAdd = () => {
    if (!size) {
      setSizeError('Please select a size');
      return;
    }
    setSizeError('');
    if (!Number.isFinite(quantity) || quantity < 1) return alert('Quantity must be at least 1');
    if (quantity > product!.stockQty) return alert(`Only ${product!.stockQty} item(s) available in stock`);
    addItem({
      productId: product!.id,
      name: product!.name,
      brand: product!.brand,
      price: product!.price,
      imageUrl: product!.imageUrl,
      size,
      quantity
    });
    navigate('/cart');
  };

  return (
    <div className="container detail-page">
      <Link to="/products" className="back-link">← Back to Products</Link>
      <div className="product-image-section">
        {hasImage ? (
          <img
            src={getImageUrl(product!.imageUrl)}
            alt={product!.name}
            className="product-image"
            onError={() => setImageError(true)}
          />
        ) : (
          <div className="product-image-placeholder">No image</div>
        )}
      </div>
      <div className="card product-card">
        <div className="product-header">
          <p className="brand-name">{product!.brand}</p>
          <h1 className="product-name">{product!.name}</h1>
          <p className="product-description">{product!.description}</p>
        </div>
        
        <div className="product-price-section">
          <span className="product-price">₹{product!.price.toFixed(2)}</span>
          <span className={`stock-status ${product!.stockQty > 0 ? 'in-stock' : 'out-of-stock'}`}>
            {product!.stockQty > 0 ? `${product!.stockQty} in stock` : 'Out of stock'}
          </span>
        </div>
        
        <div className="product-form">
          <div className="form-group">
            <label htmlFor="size">Size</label>
            <select
              id="size"
              value={size}
              onChange={e => {
                setSize(e.target.value);
                if (e.target.value) setSizeError('');
              }}
              className="size-select"
            >
              <option value="">Choose a size...</option>
              {sizes.map(s => <option key={s} value={s}>{s}</option>)}
            </select>
            {sizeError && <p className="error">{sizeError}</p>}
          </div>
          
          <div className="form-group">
            <label htmlFor="quantity">Quantity</label>
            <div className="quantity-control">
              <button className="qty-btn" onClick={() => setQuantity(Math.max(1, quantity - 1))}>−</button>
              <input
                id="quantity"
                type="number"
                min="1"
                max={Math.max(1, product!.stockQty)}
                value={quantity}
                onChange={e => {
                  const parsed = Number.parseInt(e.target.value, 10);
                  const bounded = Number.isFinite(parsed)
                    ? Math.min(Math.max(1, parsed), Math.max(1, product!.stockQty))
                    : 1;
                  setQuantity(bounded);
                }}
                className="qty-input"
              />
              <button
                className="qty-btn"
                onClick={() => setQuantity(Math.min(Math.max(1, product!.stockQty), quantity + 1))}
                disabled={quantity >= Math.max(1, product!.stockQty)}
              >
                +
              </button>
            </div>
          </div>
        </div>
        
        <button onClick={handleAdd} className="add-to-cart-btn" disabled={product!.stockQty === 0}>
          🛒 Add to Cart
        </button>
        
        {!isAuthenticated && <p className="note">💡 You can add to cart as guest, but checkout requires login.</p>}
      </div>
    </div>
  );
}