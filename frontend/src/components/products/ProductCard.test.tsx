import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { ProductCard } from './ProductCard';

describe('ProductCard', () => {
  const product = { id: 1, name: 'Test Product' };

  it('renders the product name', () => {
    render(
      <MemoryRouter>
        <ProductCard product={product} />
      </MemoryRouter>
    );
    expect(screen.getByText('Test Product')).toBeInTheDocument();
  });

  it('links to the product details page', () => {
    render(
      <MemoryRouter>
        <ProductCard product={product} />
      </MemoryRouter>
    );
    const link = screen.getByRole('link', { name: 'Test Product' });
    expect(link).toHaveAttribute('href', '/products/1');
  });
});
