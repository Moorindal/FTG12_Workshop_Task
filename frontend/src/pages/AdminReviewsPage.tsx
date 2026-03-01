import { useAdminReviews } from '../hooks/useAdminReviews';
import { AdminReviewsTable } from '../components/admin/AdminReviewsTable';
import { Pagination } from '../components/common/Pagination';
import './AdminReviewsPage.css';

const STATUS_OPTIONS = [
  { value: '', label: 'All' },
  { value: '1', label: 'Pending moderation' },
  { value: '2', label: 'Approved' },
  { value: '3', label: 'Rejected' },
];

export function AdminReviewsPage() {
  const { reviews, loading, error, page, setPage, totalPages, filters, setFilters, refresh } =
    useAdminReviews();

  return (
    <div className="admin-reviews-page">
      <h1>Review Management</h1>

      <div className="filters-panel">
        <div className="filter-group">
          <label htmlFor="status-filter">Status</label>
          <select
            id="status-filter"
            value={filters.statusId?.toString() ?? ''}
            onChange={(e) =>
              setFilters({
                ...filters,
                statusId: e.target.value ? Number(e.target.value) : undefined,
              })
            }
          >
            {STATUS_OPTIONS.map((opt) => (
              <option key={opt.value} value={opt.value}>{opt.label}</option>
            ))}
          </select>
        </div>
        <div className="filter-group">
          <label htmlFor="date-from">From</label>
          <input
            id="date-from"
            type="date"
            value={filters.dateFrom ?? ''}
            onChange={(e) => setFilters({ ...filters, dateFrom: e.target.value || undefined })}
          />
        </div>
        <div className="filter-group">
          <label htmlFor="date-to">To</label>
          <input
            id="date-to"
            type="date"
            value={filters.dateTo ?? ''}
            onChange={(e) => setFilters({ ...filters, dateTo: e.target.value || undefined })}
          />
        </div>
        <button
          className="btn-clear-filters"
          onClick={() => setFilters({})}
        >
          Clear filters
        </button>
      </div>

      {loading && <p className="status-loading">Loading reviews...</p>}
      {error && <p className="status-error" role="alert">{error}</p>}

      {!loading && !error && (
        <AdminReviewsTable reviews={reviews} onStatusChange={refresh} />
      )}

      {!loading && !error && reviews.length === 0 && (
        <p className="status-empty">No reviews match the selected filters.</p>
      )}

      <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
    </div>
  );
}
