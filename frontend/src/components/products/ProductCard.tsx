import { Link } from 'react-router-dom';
import type { Product } from '../../types/product';

interface ProductCardProps {
  product: Product;
}

export function ProductCard({ product }: ProductCardProps) {
  return (
    <div className="product-card">
      <Link to={`/products/${product.id}`}>
        <h3>{product.name}</h3>
      </Link>
    </div>
  );
}
