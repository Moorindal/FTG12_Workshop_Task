import { useUsers } from '../hooks/useUsers';
import { useAuth } from '../hooks/useAuth';
import { UserManagement } from '../components/admin/UserManagement';
import './AdminUsersPage.css';

export function AdminUsersPage() {
  const { users, loading, error, banUser, unbanUser } = useUsers();
  const { user } = useAuth();

  return (
    <div className="admin-users-page">
      <h1>User Management</h1>

      {loading && <p className="status-loading">Loading users...</p>}
      {error && <p className="status-error" role="alert">{error}</p>}

      {!loading && !error && (
        <UserManagement users={users} currentUserId={user?.id} onBan={banUser} onUnban={unbanUser} />
      )}
    </div>
  );
}
