import { render, screen } from '@testing-library/react';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import { MainLayout } from './MainLayout';

const mockUseAuth = vi.fn();
vi.mock('../../hooks/useAuth', () => ({
  useAuth: () => mockUseAuth(),
}));

describe('MainLayout', () => {
  beforeEach(() => {
    mockUseAuth.mockReturnValue({
      user: { id: 1, username: 'Admin', isAdministrator: true, isBanned: false },
      isAdmin: true,
      logout: vi.fn(),
    });
  });

  it('renders TopBar and child route content', () => {
    render(
      <MemoryRouter initialEntries={['/test']}>
        <Routes>
          <Route element={<MainLayout />}>
            <Route path="/test" element={<div>Page Content</div>} />
          </Route>
        </Routes>
      </MemoryRouter>
    );
    expect(screen.getByText('FTG12 Reviews')).toBeInTheDocument();
    expect(screen.getByText('Page Content')).toBeInTheDocument();
  });

  it('renders main element', () => {
    render(
      <MemoryRouter initialEntries={['/test']}>
        <Routes>
          <Route element={<MainLayout />}>
            <Route path="/test" element={<div>Content</div>} />
          </Route>
        </Routes>
      </MemoryRouter>
    );
    expect(document.querySelector('main')).toBeInTheDocument();
  });
});
