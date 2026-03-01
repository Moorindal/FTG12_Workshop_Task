import { Outlet } from 'react-router-dom';
import { TopBar } from './TopBar';
import './MainLayout.css';

export function MainLayout() {
  return (
    <div className="main-layout">
      <TopBar />
      <main className="main-content">
        <Outlet />
      </main>
    </div>
  );
}
