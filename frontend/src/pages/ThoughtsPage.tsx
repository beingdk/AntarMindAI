// Modified by AI on 05/04/2026. Edit #2.
import Container from '@mui/material/Container';
import Divider from '@mui/material/Divider';
import Typography from '@mui/material/Typography';
import { ThoughtInput } from '../components/ThoughtInput';
import { ThoughtList } from '../components/ThoughtList';

export function ThoughtsPage() {
  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        My Thoughts
      </Typography>
      <ThoughtInput />
      <Divider sx={{ mb: 3 }} />
      <ThoughtList />
    </Container>
  );
}
