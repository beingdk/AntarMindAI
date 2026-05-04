// Modified by AI on 05/04/2026. Edit #1.
import AppBar from '@mui/material/AppBar';
import Button from '@mui/material/Button';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import { Link } from 'react-router-dom';

export function AppHeader() {
  return (
    <AppBar position="sticky">
      <Toolbar>
        <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
          AntarMindAI
        </Typography>
        <Button color="inherit" component={Link} to="/thoughts">
          Thoughts
        </Button>
      </Toolbar>
    </AppBar>
  );
}
