import { useEffect, useState } from 'react';
import { productService } from '../services/productService';
import type { Product } from '../types/product';
import ProductCard from '../components/ProductCard';

export default function HomePage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [brand, setBrand] = useState('');
  const [size, setSize] = useState('');
  const [loading, setLoading] = useState(false);

  const loadProducts = async () => {
    setLoading(true);
    const data = await productService.getAll({
      brand: brand || undefined,
      size: size || undefined
    });
    setProducts(data);
    setLoading(false);
  };

  useEffect(() => {
    loadProducts();
  }, []);

  return (
    <div className="container">
      <h1>Products</h1>
      <div className="filters">
        <input placeholder="Brand" value={brand} onChange={e => setBrand(e.target.value)} />
        <input placeholder="Size" value={size} onChange={e => setSize(e.target.value)} />
        <button onClick={loadProducts}>Filter</button>
      </div>
      {loading ? <p>Loading...</p> : <div className="grid">{products.map(p => <ProductCard key={p.id} product={p} />)}</div>}
    </div>
  );
}