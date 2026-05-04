// Modified by AI on 05/04/2026. Edit #4.
import { Navigate, Route, Routes } from 'react-router-dom';
import { AppHeader } from './components/AppHeader';
import { LandingPage } from './pages/LandingPage';
import { ThoughtsPage } from './pages/ThoughtsPage';
import { InsightsPage } from './pages/InsightsPage';
import { DashboardPage } from './pages/DashboardPage';

function App() {
  return (
    <>
      <AppHeader />
      <Routes>
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
        <Route path="/landing" element={<LandingPage />} />
        <Route path="/thoughts" element={<ThoughtsPage />} />
        <Route path="/insights" element={<InsightsPage />} />
        <Route path="/dashboard" element={<DashboardPage />} />
      </Routes>
    </>
  );
}

export default App;
