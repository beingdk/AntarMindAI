// Modified by AI on 05/04/2026. Edit #1.
import { Route, Routes } from 'react-router-dom';
import { AppHeader } from './components/AppHeader';
import { LandingPage } from './pages/LandingPage';
import { ThoughtsPage } from './pages/ThoughtsPage';

function App() {
  return (
    <>
      <AppHeader />
      <Routes>
        <Route path="/" element={<LandingPage />} />
        <Route path="/thoughts" element={<ThoughtsPage />} />
      </Routes>
    </>
  );
}

export default App;
