import { useEffect, useMemo, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useCart } from '../context/CartContext';
import { productService } from '../services/productService';
import { getImageUrl } from '../utils/urlHelper';

export default function CartPage() {
  const { items, updateQty, removeItem, totalAmount } = useCart();
  const navigate = useNavigate();
  const [stockByProductId, setStockByProductId] = useState<Record<string, number>>({});
  const [stockError, setStockError] = useState('');
  const [stockNotice, setStockNotice] = useState('');

  useEffect(() => {
    const productIds = [...new Set(items.map(x => x.productId))];

    if (productIds.length === 0) {
      setStockByProductId({});
      setStockError('');
      setStockNotice('');
      return;
    }

    let cancelled = false;
    const loadStock = async () => {
      const results = await Promise.all(productIds.map(async (id) => {
        try {
          const product = await productService.getById(id);
          return { id, product };
        } catch {
          return { id, product: null };
        }
      }));

      if (cancelled) return;

      const stockMap: Record<string, number> = {};
      const missingProductIds = new Set<string>();

      for (const result of results) {
        if (!result.product) {
          missingProductIds.add(result.id);
          continue;
        }

        stockMap[result.product.id] = result.product.stockQty;
      }

      if (missingProductIds.size > 0) {
        const removedItems = items.filter(item => missingProductIds.has(item.productId));
        for (const removed of removedItems) {
          removeItem(removed.productId, removed.size);
        }

        setStockNotice('Some deleted products were removed from your cart.');
      } else {
        setStockNotice('');
      }

      setStockByProductId(stockMap);
      setStockError('');
    };

    void loadStock();
    return () => {
      cancelled = true;
    };
  }, [items, removeItem]);

  const hasStockIssues = useMemo(() => {
    return items.some(item => {
      const stockQty = stockByProductId[item.productId];
      if (stockQty === undefined) return false;
      return stockQty < 1 || item.quantity > stockQty;
    });
  }, [items, stockByProductId]);

  const checkoutDisabled = !!stockError || hasStockIssues;

  return (
    <div className="container">
      <h1>Your Cart</h1>
      {items.length === 0 ? (
        <p>Cart is empty.</p>
      ) : (
        <>
          <div className="cart-list">
            {items.map(item => (
              <div key={`${item.productId}-${item.size}`} className="cart-row">
                <img src={getImageUrl(item.imageUrl)} alt={item.name} />
                <div>
                  <h3>{item.name}</h3>
                  <p>{item.brand}</p>
                  <p>Size: {item.size}</p>
                  <p>₹{item.price.toFixed(2)}</p>
                  {stockByProductId[item.productId] === 0 && <p className="error">Out of stock</p>}
                  {stockByProductId[item.productId] !== undefined && stockByProductId[item.productId] > 0 && item.quantity > stockByProductId[item.productId] && (
                    <p className="error">Only {stockByProductId[item.productId]} in stock</p>
                  )}
                </div>
                <input
                  type="number"
                  min="1"
                  max={Math.max(1, stockByProductId[item.productId] ?? 1)}
                  value={item.quantity}
                  onChange={e => {
                    const parsed = Number.parseInt(e.target.value, 10);
                    const minQty = 1;
                    const maxQty = Math.max(minQty, stockByProductId[item.productId] ?? minQty);
                    const bounded = Number.isFinite(parsed) ? Math.min(Math.max(minQty, parsed), maxQty) : minQty;
                    updateQty(item.productId, item.size, bounded);
                  }}
                />
                <button onClick={() => removeItem(item.productId, item.size)}>Remove</button>
              </div>
            ))}
          </div>
          {stockError && <p className="error">{stockError}</p>}
          {stockNotice && <p className="note">{stockNotice}</p>}
          {hasStockIssues && <p className="error">Some items exceed available stock. Update your cart to continue.</p>}
          <h3>Total: ₹{totalAmount.toFixed(2)}</h3>
          <button onClick={() => navigate('/checkout')} disabled={checkoutDisabled}>Checkout</button>
        </>
      )}
      <div className="spacer">
        <Link to="/">Continue Shopping</Link>
      </div>
    </div>
  );
}