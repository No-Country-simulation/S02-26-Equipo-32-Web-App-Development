import { Navigate, Outlet } from 'react-router-dom';
import { getCurrentUser, isAdmin } from '../store';

interface Props {
  requireAdmin?: boolean;
}

export const ProtectedRoute = ({ requireAdmin = false }: Props) => {
  const user = getCurrentUser();
  const userIsAdmin = isAdmin();

  // 1. Si no hay usuario logueado -> Mándalo al Login
  if (!user) {
    return <Navigate to="/login" replace />;
  }

  // 2. Si se requiere Admin pero el usuario NO es admin -> Mándalo al Home normal
  if (requireAdmin && !userIsAdmin) {
    return <Navigate to="/" replace />;
  }

  // 3. Si pasa las pruebas -> Renderiza la ruta hija (Outlet)
  return <Outlet />;
};