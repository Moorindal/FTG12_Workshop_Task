import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { TopBar } from './TopBar';

const mockUseAuth = vi.fn();
vi.mock('../../hooks/useAuth', () => ({
  useAuth: () => mockUseAuth(),
}));

describe('TopBar', () => {
  it('shows admin navigation links for admin users', () => {
    mockUseAuth.mockReturnValue({
      user: { id: 1, username: 'Admin', isAdministrator: true, isBanned: false },
      isAdmin: true,
      logout: vi.fn(),
    });
    render(
      <MemoryRouter>
        <TopBar />
      </MemoryRouter>
    );
    expect(screen.getByRole('link', { name: 'Reviews' })).toBeInTheDocument();
    expect(screen.getByRole('link', { name: 'Users' })).toBeInTheDocument();
    expect(screen.queryByRole('link', { name: 'My Reviews' })).not.toBeInTheDocument();
  });

  it('shows user navigation links for regular users', () => {
    mockUseAuth.mockReturnValue({
      user: { id: 2, username: 'User1', isAdministrator: false, isBanned: false },
      isAdmin: false,
      logout: vi.fn(),
    });
    render(
      <MemoryRouter>
        <TopBar />
      </MemoryRouter>
    );
    expect(screen.getByRole('link', { name: 'Products' })).toBeInTheDocument();
    expect(screen.getByRole('link', { name: 'My Reviews' })).toBeInTheDocument();
    expect(screen.queryByRole('link', { name: 'Users' })).not.toBeInTheDocument();
  });

  it('displays the username', () => {
    mockUseAuth.mockReturnValue({
      user: { id: 2, username: 'TestUser', isAdministrator: false, isBanned: false },
      isAdmin: false,
      logout: vi.fn(),
    });
    render(
      <MemoryRouter>
        <TopBar />
      </MemoryRouter>
    );
    expect(screen.getByText('TestUser')).toBeInTheDocument();
  });

  it('renders sign out button', () => {
    mockUseAuth.mockReturnValue({
      user: { id: 1, username: 'Admin', isAdministrator: true, isBanned: false },
      isAdmin: true,
      logout: vi.fn(),
    });
    render(
      <MemoryRouter>
        <TopBar />
      </MemoryRouter>
    );
    expect(screen.getByText('Sign out')).toBeInTheDocument();
  });
});
