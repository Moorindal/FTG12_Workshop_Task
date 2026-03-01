import { NavLink } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import './TopBar.css';

export function TopBar() {
  const { user, isAdmin, logout } = useAuth();

  return (
    <header className="top-bar">
      <div className="top-bar-inner">
        <span className="top-bar-title">FTG12 Reviews</span>
        <nav className="top-bar-nav">
          {isAdmin ? (
            <>
              <NavLink to="/products">Products</NavLink>
              <NavLink to="/admin/reviews">Reviews</NavLink>
              <NavLink to="/admin/users">Users</NavLink>
            </>
          ) : (
            <>
              <NavLink to="/products">Products</NavLink>
              <NavLink to="/my-reviews">My Reviews</NavLink>
            </>
          )}
        </nav>
        <div className="top-bar-user">
          <span>{user?.username}</span>
          <button onClick={logout} className="btn-sign-out">Sign out</button>
        </div>
      </div>
    </header>
  );
}
