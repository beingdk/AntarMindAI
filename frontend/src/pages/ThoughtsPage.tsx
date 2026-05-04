// Modified by AI on 05/05/2026. Edit #3.
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Container from '@mui/material/Container';
import Divider from '@mui/material/Divider';
import Typography from '@mui/material/Typography';
import { ThoughtInput } from '../components/ThoughtInput';
import { ThoughtList } from '../components/ThoughtList';

export function ThoughtsPage() {
  function handleExport(format: 'csv' | 'json') {
    window.location.href = `/api/export?format=${format}`;
  }

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1 }}>
        <Typography variant="h4" component="h1">
          My Thoughts
        </Typography>
        <Box sx={{ display: 'flex', gap: 1 }}>
          <Button variant="outlined" size="small" onClick={() => handleExport('csv')}>
            ↓ Export CSV
          </Button>
          <Button variant="outlined" size="small" onClick={() => handleExport('json')}>
            ↓ Export JSON
          </Button>
        </Box>
      </Box>
      <ThoughtInput />
      <Divider sx={{ mb: 3 }} />
      <ThoughtList />
    </Container>
  );
}
