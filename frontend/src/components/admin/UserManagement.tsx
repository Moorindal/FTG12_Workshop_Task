import type { User } from '../../types/user';
import { UserRow } from './UserRow';
import './UserManagement.css';

interface UserManagementProps {
  users: User[];
  currentUserId?: number;
  onBan: (userId: number) => Promise<void>;
  onUnban: (userId: number) => Promise<void>;
}

export function UserManagement({ users, currentUserId, onBan, onUnban }: UserManagementProps) {
  if (users.length === 0) {
    return <p className="status-empty">No users found.</p>;
  }

  return (
    <div className="user-management-table-wrapper">
      <table className="user-management-table">
        <thead>
          <tr>
            <th>Username</th>
            <th>Role</th>
            <th>Status</th>
            <th>Banned At</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {users.map((user) => (
            <UserRow key={user.id} user={user} currentUserId={currentUserId} onBan={onBan} onUnban={onUnban} />
          ))}
        </tbody>
      </table>
    </div>
  );
}
