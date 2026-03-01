import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { UserRow } from './UserRow';
import type { User } from '../../types/user';

describe('UserRow', () => {
  const activeUser: User = {
    id: 2, username: 'john', isAdministrator: false,
    isBanned: false, bannedAt: null, createdAt: '2024-01-01T00:00:00Z',
  };

  const bannedUser: User = {
    id: 3, username: 'jane', isAdministrator: false,
    isBanned: true, bannedAt: '2024-06-01T00:00:00Z', createdAt: '2024-01-01T00:00:00Z',
  };

  function renderRow(user: User, onBan = vi.fn(), onUnban = vi.fn()) {
    return render(
      <table>
        <tbody>
          <UserRow user={user} onBan={onBan} onUnban={onUnban} />
        </tbody>
      </table>
    );
  }

  it('renders username', () => {
    renderRow(activeUser);
    expect(screen.getByText('john')).toBeInTheDocument();
  });

  it('shows Ban button for active users', () => {
    renderRow(activeUser);
    expect(screen.getByText('Ban')).toBeInTheDocument();
    expect(screen.queryByText('Restore')).not.toBeInTheDocument();
  });

  it('shows Restore button for banned users', () => {
    renderRow(bannedUser);
    expect(screen.getByText('Restore')).toBeInTheDocument();
    expect(screen.queryByText('Ban')).not.toBeInTheDocument();
  });

  it('calls onBan after confirmation', async () => {
    const user = userEvent.setup();
    const onBan = vi.fn().mockResolvedValue(undefined);
    vi.spyOn(window, 'confirm').mockReturnValue(true);
    renderRow(activeUser, onBan);
    await user.click(screen.getByText('Ban'));
    expect(onBan).toHaveBeenCalledWith(2);
    vi.restoreAllMocks();
  });

  it('does not call onBan when confirmation is cancelled', async () => {
    const user = userEvent.setup();
    const onBan = vi.fn();
    vi.spyOn(window, 'confirm').mockReturnValue(false);
    renderRow(activeUser, onBan);
    await user.click(screen.getByText('Ban'));
    expect(onBan).not.toHaveBeenCalled();
    vi.restoreAllMocks();
  });

  it('calls onUnban after confirmation', async () => {
    const user = userEvent.setup();
    const onUnban = vi.fn().mockResolvedValue(undefined);
    vi.spyOn(window, 'confirm').mockReturnValue(true);
    renderRow(bannedUser, vi.fn(), onUnban);
    await user.click(screen.getByText('Restore'));
    expect(onUnban).toHaveBeenCalledWith(3);
    vi.restoreAllMocks();
  });
});
