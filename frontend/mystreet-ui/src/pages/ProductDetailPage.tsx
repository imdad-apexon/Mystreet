import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
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
  const [size, setSize] = useState('');
  const [quantity, setQuantity] = useState(1);

  useEffect(() => {
    if (id) productService.getById(id).then(setProduct);
  }, [id]);

  if (!product) return <div className="container">Loading...</div>;

  const sizes = product.sizesCsv.split(',');

  const handleAdd = () => {
    if (!size) return alert('Select a size');
    addItem({
      productId: product.id,
      name: product.name,
      brand: product.brand,
      price: product.price,
      imageUrl: product.imageUrl,
      size,
      quantity
    });
    navigate('/cart');
  };

  return (
    <div className="container detail-page">
      <div className="product-image-section">
        <img src={getImageUrl(product.imageUrl)} alt={product.name} className="product-image" />
      </div>
      <div className="card product-card">
        <div className="product-header">
          <p className="brand-name">{product.brand}</p>
          <h1 className="product-name">{product.name}</h1>
          <p className="product-description">{product.description}</p>
        </div>
        
        <div className="product-price-section">
          <span className="product-price">₹{product.price.toFixed(2)}</span>
          <span className={`stock-status ${product.stockQty > 0 ? 'in-stock' : 'out-of-stock'}`}>
            {product.stockQty > 0 ? `${product.stockQty} in stock` : 'Out of stock'}
          </span>
        </div>
        
        <div className="product-form">
          <div className="form-group">
            <label htmlFor="size">Size</label>
            <select id="size" value={size} onChange={e => setSize(e.target.value)} className="size-select">
              <option value="">Choose a size...</option>
              {sizes.map(s => <option key={s} value={s}>{s}</option>)}
            </select>
          </div>
          
          <div className="form-group">
            <label htmlFor="quantity">Quantity</label>
            <div className="quantity-control">
              <button className="qty-btn" onClick={() => setQuantity(Math.max(1, quantity - 1))}>−</button>
              <input id="quantity" type="number" min="1" value={quantity} onChange={e => setQuantity(Number(e.target.value))} className="qty-input" />
              <button className="qty-btn" onClick={() => setQuantity(quantity + 1)}>+</button>
            </div>
          </div>
        </div>
        
        <button onClick={handleAdd} className="add-to-cart-btn" disabled={product.stockQty === 0}>
          🛒 Add to Cart
        </button>
        
        {!isAuthenticated && <p className="note">💡 You can add to cart as guest, but checkout requires login.</p>}
      </div>
    </div>
  );
}