import { Link } from 'react-router-dom';
import type { Product } from '../types/product';
import { getImageUrl } from '../utils/urlHelper';

export default function ProductCard({ product }: { product: Product }) {
  const stockStatus = product.stockQty > 0 
    ? `${product.stockQty} in stock`
    : 'Out of stock';
  
  return (
    <div className="card">
      <img src={getImageUrl(product.imageUrl)} alt={product.name} />
      <h3>{product.name}</h3>
      <p>{product.brand}</p>
      <p>₹{product.price}</p>
      <p className={product.stockQty > 0 ? 'in-stock' : 'out-of-stock'}>{stockStatus}</p>
      <Link to={`/products/${product.id}`}>View Details</Link>
    </div>
  );
}