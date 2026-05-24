import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { productService } from '../services/productService';

export default function AdminProductFormPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEdit = !!id;

  const [form, setForm] = useState({
    name: '',
    brand: '',
    description: '',
    price: 0,
    sizesCsv: '7,8,9,10',
    stockQty: 0,
    imageUrl: '',
    category: 'Sneakers'
  });

  useEffect(() => {
    if (id) {
      productService.getById(id).then(p => setForm({
        name: p.name,
        brand: p.brand,
        description: p.description,
        price: p.price,
        sizesCsv: p.sizesCsv,
        stockQty: p.stockQty,
        imageUrl: p.imageUrl,
        category: p.category
      }));
    }
  }, [id]);

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (isEdit && id) await productService.update(id, form);
    else await productService.create(form);
    navigate('/admin/products');
  };

  return (
    <div className="container form-page">
      <h1>{isEdit ? 'Edit Product' : 'Add Product'}</h1>
      <form onSubmit={submit}>
        <input placeholder="Name" value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} />
        <input placeholder="Brand" value={form.brand} onChange={e => setForm({ ...form, brand: e.target.value })} />
        <textarea placeholder="Description" value={form.description} onChange={e => setForm({ ...form, description: e.target.value })} />
        <input type="number" placeholder="Price" value={form.price} onChange={e => setForm({ ...form, price: Number(e.target.value) })} />
        <input placeholder="SizesCsv" value={form.sizesCsv} onChange={e => setForm({ ...form, sizesCsv: e.target.value })} />
        <input type="number" placeholder="Stock Qty" value={form.stockQty} onChange={e => setForm({ ...form, stockQty: Number(e.target.value) })} />
        <input placeholder="Image URL" value={form.imageUrl} onChange={e => setForm({ ...form, imageUrl: e.target.value })} />
        <input placeholder="Category" value={form.category} onChange={e => setForm({ ...form, category: e.target.value })} />
        <button type="submit">Save</button>
      </form>
    </div>
  );
}