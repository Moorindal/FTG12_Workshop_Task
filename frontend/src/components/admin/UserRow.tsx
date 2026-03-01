import { useState } from 'react';
import type { User } from '../../types/user';
import './UserRow.css';

interface UserRowProps {
  user: User;
  onBan: (userId: number) => Promise<void>;
  onUnban: (userId: number) => Promise<void>;
}

function formatDate(dateStr: string | null): string {
  if (!dateStr) return '-';
  return new Date(dateStr).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  });
}

export function UserRow({ user, onBan, onUnban }: UserRowProps) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleBan = async () => {
    const warning = user.isAdministrator
      ? `This user is an administrator. Are you sure you want to ban ${user.username}?`
      : `Are you sure you want to ban ${user.username}?`;
    if (!confirm(warning)) return;
    setLoading(true);
    setError(null);
    try {
      await onBan(user.id);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to ban user');
    } finally {
      setLoading(false);
    }
  };

  const handleUnban = async () => {
    if (!confirm(`Are you sure you want to restore ${user.username}?`)) return;
    setLoading(true);
    setError(null);
    try {
      await onUnban(user.id);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to restore user');
    } finally {
      setLoading(false);
    }
  };

  return (
    <tr>
      <td>{user.username}</td>
      <td>
        <span className={`role-badge ${user.isAdministrator ? 'role-admin' : 'role-user'}`}>
          {user.isAdministrator ? 'Admin' : 'User'}
        </span>
      </td>
      <td>
        <span className={`status-badge ${user.isBanned ? 'badge-rejected' : 'badge-approved'}`}>
          {user.isBanned ? 'Banned' : 'Active'}
        </span>
      </td>
      <td>{formatDate(user.bannedAt)}</td>
      <td>
        {error && <span className="row-error">{error}</span>}
        {user.isBanned ? (
          <button className="btn-restore" onClick={handleUnban} disabled={loading}>
            {loading ? '...' : 'Restore'}
          </button>
        ) : (
          <button className="btn-ban" onClick={handleBan} disabled={loading}>
            {loading ? '...' : 'Ban'}
          </button>
        )}
      </td>
    </tr>
  );
}
