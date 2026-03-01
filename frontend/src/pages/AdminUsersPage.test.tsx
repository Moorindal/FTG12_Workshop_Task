import { render, screen } from '@testing-library/react';
import { AdminUsersPage } from './AdminUsersPage';

const mockBanUser = vi.fn();
const mockUnbanUser = vi.fn();
const mockRefresh = vi.fn();
let mockResult = {
  users: [] as unknown[],
  loading: true,
  error: null as string | null,
  banUser: mockBanUser,
  unbanUser: mockUnbanUser,
  refresh: mockRefresh,
};

vi.mock('../hooks/useUsers', () => ({
  useUsers: () => mockResult,
}));

const mockUseAuth = vi.fn();
vi.mock('../hooks/useAuth', () => ({
  useAuth: () => mockUseAuth(),
}));

describe('AdminUsersPage', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockUseAuth.mockReturnValue({ user: { id: 99, username: 'admin', isAdministrator: true }, token: 'tok' });
  });

  it('shows loading state', () => {
    mockResult = { users: [], loading: true, error: null, banUser: mockBanUser, unbanUser: mockUnbanUser, refresh: mockRefresh };
    render(<AdminUsersPage />);
    expect(screen.getByText('Loading users...')).toBeInTheDocument();
  });

  it('renders heading', () => {
    mockResult = { users: [], loading: true, error: null, banUser: mockBanUser, unbanUser: mockUnbanUser, refresh: mockRefresh };
    render(<AdminUsersPage />);
    expect(screen.getByRole('heading', { name: 'User Management' })).toBeInTheDocument();
  });

  it('renders user table when loaded', () => {
    mockResult = {
      users: [
        { id: 1, username: 'admin', isAdministrator: true, isBanned: false, bannedAt: null, createdAt: '2024-01-01T00:00:00Z' },
        { id: 2, username: 'user1', isAdministrator: false, isBanned: false, bannedAt: null, createdAt: '2024-01-01T00:00:00Z' },
      ],
      loading: false, error: null, banUser: mockBanUser, unbanUser: mockUnbanUser, refresh: mockRefresh,
    };
    render(<AdminUsersPage />);
    expect(screen.getByText('admin')).toBeInTheDocument();
    expect(screen.getByText('user1')).toBeInTheDocument();
  });

  it('shows error state', () => {
    mockResult = { users: [], loading: false, error: 'Failed to load users', banUser: mockBanUser, unbanUser: mockUnbanUser, refresh: mockRefresh };
    render(<AdminUsersPage />);
    expect(screen.getByRole('alert')).toBeInTheDocument();
    expect(screen.getByText('Failed to load users')).toBeInTheDocument();
  });

  it('shows ban/restore buttons based on user status', () => {
    mockResult = {
      users: [
        { id: 1, username: 'active_user', isAdministrator: false, isBanned: false, bannedAt: null, createdAt: '2024-01-01T00:00:00Z' },
        { id: 2, username: 'banned_user', isAdministrator: false, isBanned: true, bannedAt: '2024-06-01T00:00:00Z', createdAt: '2024-01-01T00:00:00Z' },
      ],
      loading: false, error: null, banUser: mockBanUser, unbanUser: mockUnbanUser, refresh: mockRefresh,
    };
    render(<AdminUsersPage />);
    expect(screen.getByText('active_user')).toBeInTheDocument();
    expect(screen.getByText('banned_user')).toBeInTheDocument();
    expect(screen.getByText('Ban')).toBeInTheDocument();
    expect(screen.getByText('Restore')).toBeInTheDocument();
  });
});
