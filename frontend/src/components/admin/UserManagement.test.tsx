import { render, screen } from '@testing-library/react';
import { UserManagement } from './UserManagement';
import type { User } from '../../types/user';

describe('UserManagement', () => {
  const users: User[] = [
    {
      id: 1, username: 'admin', isAdministrator: true,
      isBanned: false, bannedAt: null, createdAt: '2024-01-01T00:00:00Z',
    },
    {
      id: 2, username: 'user1', isAdministrator: false,
      isBanned: true, bannedAt: '2024-06-01T00:00:00Z', createdAt: '2024-01-01T00:00:00Z',
    },
  ];

  it('renders user data in table', () => {
    render(<UserManagement users={users} onBan={vi.fn()} onUnban={vi.fn()} />);
    expect(screen.getByText('admin')).toBeInTheDocument();
    expect(screen.getByText('user1')).toBeInTheDocument();
  });

  it('shows empty message when no users', () => {
    render(<UserManagement users={[]} onBan={vi.fn()} onUnban={vi.fn()} />);
    expect(screen.getByText('No users found.')).toBeInTheDocument();
  });

  it('shows Active and Banned status badges', () => {
    render(<UserManagement users={users} onBan={vi.fn()} onUnban={vi.fn()} />);
    expect(screen.getByText('Active')).toBeInTheDocument();
    expect(screen.getByText('Banned')).toBeInTheDocument();
  });

  it('renders table headers', () => {
    render(<UserManagement users={users} onBan={vi.fn()} onUnban={vi.fn()} />);
    expect(screen.getByText('Username')).toBeInTheDocument();
    expect(screen.getByText('Role')).toBeInTheDocument();
    expect(screen.getByText('Status')).toBeInTheDocument();
  });
});
