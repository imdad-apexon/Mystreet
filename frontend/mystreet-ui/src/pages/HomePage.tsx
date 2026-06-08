import { useEffect, useState } from 'react';
import { productService } from '../services/productService';
import type { Product } from '../types/product';
import ProductCard from '../components/ProductCard';

export default function HomePage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [allProducts, setAllProducts] = useState<Product[]>([]);
  const [brand, setBrand] = useState('');
  const [category, setCategory] = useState('');
  const [size, setSize] = useState('');
  const [loading, setLoading] = useState(false);

  const loadProducts = async (filters?: { brand?: string; category?: string; size?: string }) => {
    const selectedBrand = filters?.brand ?? brand;
    const selectedCategory = filters?.category ?? category;
    const selectedSize = filters?.size ?? size;

    setLoading(true);
    const data = await productService.getAll({
      brand: selectedBrand || undefined,
      category: selectedCategory || undefined,
      size: selectedSize || undefined
    });
    setProducts(data);
    setLoading(false);
  };

  const clearFilters = async () => {
    setBrand('');
    setCategory('');
    setSize('');
    await loadProducts({ brand: '', category: '', size: '' });
  };

  useEffect(() => {
    document.title = 'MyStreet - Products';
    const init = async () => {
      const all = await productService.getAll();
      setAllProducts(all);
      await loadProducts();
    };

    void init();
  }, []);

  const brandOptions = [...new Set(allProducts.map(p => p.brand).filter(Boolean))].sort();
  const categoryOptions = [...new Set(allProducts.map(p => p.category).filter(Boolean))].sort();
  const sizeOptions = [...new Set(allProducts.flatMap(p => p.sizesCsv.split(',').map(s => s.trim())).filter(Boolean))].sort((a, b) => a.localeCompare(b, undefined, { numeric: true }));

  return (
    <div className="container">
      <h1>Products</h1>
      <div className="filters">
        <select
          className="filter-input"
          value={brand}
          onChange={e => setBrand(e.target.value)}
        >
          <option value="">All Brands</option>
          {brandOptions.map(option => (
            <option key={option} value={option}>{option}</option>
          ))}
        </select>
        <select
          className="filter-input"
          value={category}
          onChange={e => setCategory(e.target.value)}
        >
          <option value="">All Categories</option>
          {categoryOptions.map(option => (
            <option key={option} value={option}>{option}</option>
          ))}
        </select>
        <select
          className="filter-input"
          value={size}
          onChange={e => setSize(e.target.value)}
        >
          <option value="">All Sizes</option>
          {sizeOptions.map(option => (
            <option key={option} value={option}>{option}</option>
          ))}
        </select>
        <button className="filter-btn" onClick={() => void loadProducts()}>Filter</button>
        <button
          className="filter-btn filter-btn--secondary"
          onClick={() => void clearFilters()}
          disabled={!brand && !category && !size}
        >
          Clear
        </button>
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