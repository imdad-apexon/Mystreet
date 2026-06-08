import { useEffect, useState } from 'react';
import { productService } from '../services/productService';
import type { Product } from '../types/product';
import ProductCard from '../components/ProductCard';

export default function HomePage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [brand, setBrand] = useState('');
  const [category, setCategory] = useState('');
  const [size, setSize] = useState('');
  const [loading, setLoading] = useState(false);

  const loadProducts = async () => {
    setLoading(true);
    const data = await productService.getAll({
      brand: brand || undefined,
      category: category || undefined,
      size: size || undefined
    });
    setProducts(data);
    setLoading(false);
  };

  useEffect(() => {
    document.title = 'MyStreet - Products';
    loadProducts();
  }, []);

  return (
    <div className="container">
      <h1>Products</h1>
      <div className="filters">
        <input
          className="filter-input"
          placeholder="Brand"
          value={brand}
          onChange={e => setBrand(e.target.value)}
        />
        <input
          className="filter-input"
          placeholder="Category"
          value={category}
          onChange={e => setCategory(e.target.value)}
        />
        <input
          className="filter-input"
          placeholder="Size"
          value={size}
          onChange={e => setSize(e.target.value)}
        />
        <button className="filter-btn" onClick={loadProducts}>Filter</button>
      </div>
      {loading ? (
        <p>Loading...</p>
      ) : products.length === 0 ? (
        <p className="no-products-found">No Product Found</p>
      ) : (
        <div className="grid">{products.map(p => <ProductCard key={p.id} product={p} />)}</div>
      )}
    </div>
  );
}