import { useUsers } from '../hooks/useUsers';
import { UserManagement } from '../components/admin/UserManagement';
import './AdminUsersPage.css';

export function AdminUsersPage() {
  const { users, loading, error, banUser, unbanUser } = useUsers();

  return (
    <div className="admin-users-page">
      <h1>User Management</h1>

      {loading && <p className="status-loading">Loading users...</p>}
      {error && <p className="status-error" role="alert">{error}</p>}

      {!loading && !error && (
        <UserManagement users={users} onBan={banUser} onUnban={unbanUser} />
      )}
    </div>
  );
}
