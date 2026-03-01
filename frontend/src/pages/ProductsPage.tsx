import { useProducts } from '../hooks/useProducts';
import { ProductCard } from '../components/products/ProductCard';
import { Pagination } from '../components/common/Pagination';
import './ProductsPage.css';

export function ProductsPage() {
  const { products, loading, error, page, setPage, totalPages } = useProducts();

  return (
    <div className="products-page">
      <h1>Products</h1>
      {loading && <p className="status-loading">Loading products...</p>}
      {error && <p className="status-error" role="alert">{error}</p>}
      {!loading && !error && products.length === 0 && (
        <p className="status-empty">No products found.</p>
      )}
      <div className="products-grid">
        {products.map((product) => (
          <ProductCard key={product.id} product={product} />
        ))}
      </div>
      <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
    </div>
  );
}
