import './App.css';
import { useHealthCheck } from './hooks/useHealthCheck';

/**
 * Root application component.
 *
 * Displays the backend health status fetched via the {@link useHealthCheck} hook.
 */
function App() {
  const { data, loading, error } = useHealthCheck();

  return (
    <main className="app">
      <h1>Training Project</h1>

      <section className="health-card" aria-label="Backend health status">
        <h2>Backend Health Check</h2>

        {loading && <p className="status loading">Checking...</p>}

        {error && (
          <p className="status error" role="alert">
            Error: {error}
          </p>
        )}

        {data && (
          <dl className="health-details">
            <dt>Status</dt>
            <dd className="status healthy">{data.status}</dd>
            <dt>Timestamp</dt>
            <dd>{data.timestamp}</dd>
          </dl>
        )}
      </section>
    </main>
  );
}

export default App;
