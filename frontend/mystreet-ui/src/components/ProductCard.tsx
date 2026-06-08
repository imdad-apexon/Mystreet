import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import type { Product } from '../types/product';
import { getImageUrl } from '../utils/urlHelper';

export default function ProductCard({ product }: { product: Product }) {
  const [imageError, setImageError] = useState(false);
  const stockStatus = product.stockQty > 0 
    ? `${product.stockQty} in stock`
    : 'Out of stock';
  const hasImage = Boolean(product.imageUrl?.trim()) && !imageError;

  const truncatedName =
    product.name.length > 20 ? `${product.name.slice(0, 20)}...` : product.name;

  useEffect(() => {
    setImageError(false);
  }, [product.id, product.imageUrl]);
  
  return (
    <div className="card product-list-card">
      {hasImage ? (
        <img
          src={getImageUrl(product.imageUrl)}
          alt={product.name}
          className="product-list-card__image"
          onError={() => setImageError(true)}
        />
      ) : (
        <div className="product-list-card__image-placeholder">No image</div>
      )}
      <h3 className="product-list-card__title" title={product.name}>{truncatedName}</h3>
      <p className="product-list-card__brand">{product.brand}</p>
      <p className="product-list-card__price">₹{product.price.toFixed(2)}</p>
      <p className={product.stockQty > 0 ? 'in-stock' : 'out-of-stock'}>{stockStatus}</p>
      <Link to={`/products/${product.id}`} className="product-list-card__cta">View Details</Link>
    </div>
  );
}