import { Link } from 'react-router-dom';
import type { Product } from '../types/product';
import { getImageUrl } from '../utils/urlHelper';

export default function ProductCard({ product }: { product: Product }) {
  const stockStatus = product.stockQty > 0 
    ? `${product.stockQty} in stock`
    : 'Out of stock';
  
  return (
    <div className="card product-list-card">
      <img src={getImageUrl(product.imageUrl)} alt={product.name} className="product-list-card__image" />
      <h3 className="product-list-card__title">{product.name}</h3>
      <p className="product-list-card__brand">{product.brand}</p>
      <p className="product-list-card__price">₹{product.price.toFixed(2)}</p>
      <p className={product.stockQty > 0 ? 'in-stock' : 'out-of-stock'}>{stockStatus}</p>
      <Link to={`/products/${product.id}`} className="product-list-card__cta">View Details</Link>
    </div>
  );
}