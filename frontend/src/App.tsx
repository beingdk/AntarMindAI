// Modified by AI on 05/04/2026. Edit #2.
import { Route, Routes } from 'react-router-dom';
import { AppHeader } from './components/AppHeader';
import { LandingPage } from './pages/LandingPage';
import { ThoughtsPage } from './pages/ThoughtsPage';
import { InsightsPage } from './pages/InsightsPage';

function App() {
  return (
    <>
      <AppHeader />
      <Routes>
        <Route path="/" element={<LandingPage />} />
        <Route path="/thoughts" element={<ThoughtsPage />} />
        <Route path="/insights" element={<InsightsPage />} />
      </Routes>
    </>
  );
}

export default App;
