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
    <div className="container detail">
      <img src={getImageUrl(product.imageUrl)} alt={product.name} />
      <div>
        <h2>{product.name}</h2>
        <p>{product.brand}</p>
        <p>{product.description}</p>
        <p>₹{product.price}</p>
        <select value={size} onChange={e => setSize(e.target.value)}>
          <option value="">Select size</option>
          {sizes.map(s => <option key={s} value={s}>{s}</option>)}
        </select>
        <input type="number" min="1" value={quantity} onChange={e => setQuantity(Number(e.target.value))} />
        <button onClick={handleAdd}>Add to Cart</button>
        {!isAuthenticated && <p className="note">You can add to cart as guest, checkout requires login.</p>}
      </div>
    </div>
  );
}