// Modified by AI on 05/04/2026. Edit #2.
import AppBar from '@mui/material/AppBar';
import Button from '@mui/material/Button';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import { Link, useLocation } from 'react-router-dom';

export function AppHeader() {
  const { pathname } = useLocation();

  return (
    <AppBar position="sticky">
      <Toolbar>
        <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
          AntarMindAI
        </Typography>
        <Button
          color="inherit"
          component={Link}
          to="/thoughts"
          sx={{ fontWeight: pathname === '/thoughts' ? 'bold' : 'normal' }}
        >
          Thoughts
        </Button>
        <Button
          color="inherit"
          component={Link}
          to="/insights"
          sx={{ fontWeight: pathname === '/insights' ? 'bold' : 'normal' }}
        >
          Insights
        </Button>
      </Toolbar>
    </AppBar>
  );
}
