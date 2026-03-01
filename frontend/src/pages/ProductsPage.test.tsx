import { render, screen, waitFor } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { ProductsPage } from './ProductsPage';
import { http, HttpResponse } from 'msw';
import { server } from '../test/server';
import { createProduct, createPaginatedResponse } from '../test/factories';

describe('ProductsPage', () => {
  it('shows loading state initially', () => {
    render(
      <MemoryRouter>
        <ProductsPage />
      </MemoryRouter>
    );
    expect(screen.getByText('Loading products...')).toBeInTheDocument();
  });

  it('renders product list from API', async () => {
    server.use(
      http.get('http://localhost:7100/api/products', () => {
        return HttpResponse.json(
          createPaginatedResponse([
            createProduct({ id: 1, name: 'Alpha' }),
            createProduct({ id: 2, name: 'Beta' }),
          ]),
        );
      }),
    );
    render(
      <MemoryRouter>
        <ProductsPage />
      </MemoryRouter>
    );
    await waitFor(() => {
      expect(screen.getByText('Alpha')).toBeInTheDocument();
    });
    expect(screen.getByText('Beta')).toBeInTheDocument();
  });

  it('shows error state on API failure', async () => {
    server.use(
      http.get('http://localhost:7100/api/products', () => {
        return HttpResponse.json({ title: 'Server Error', status: 500 }, { status: 500 });
      }),
    );
    render(
      <MemoryRouter>
        <ProductsPage />
      </MemoryRouter>
    );
    await waitFor(() => {
      expect(screen.getByRole('alert')).toBeInTheDocument();
    });
  });

  it('shows empty state when no products', async () => {
    server.use(
      http.get('http://localhost:7100/api/products', () => {
        return HttpResponse.json(createPaginatedResponse([]));
      }),
    );
    render(
      <MemoryRouter>
        <ProductsPage />
      </MemoryRouter>
    );
    await waitFor(() => {
      expect(screen.getByText('No products found.')).toBeInTheDocument();
    });
  });

  it('renders pagination when multiple pages', async () => {
    server.use(
      http.get('http://localhost:7100/api/products', () => {
        return HttpResponse.json(
          createPaginatedResponse([createProduct()], { totalPages: 3, page: 1 }),
        );
      }),
    );
    render(
      <MemoryRouter>
        <ProductsPage />
      </MemoryRouter>
    );
    await waitFor(() => {
      expect(screen.getByText('Page 1 of 3')).toBeInTheDocument();
    });
  });
});
