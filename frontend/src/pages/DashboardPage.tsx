// Modified by AI on 05/04/2026. Edit #1.
import { useQuery } from '@tanstack/react-query';
import Alert from '@mui/material/Alert';
import Box from '@mui/material/Box';
import Chip from '@mui/material/Chip';
import CircularProgress from '@mui/material/CircularProgress';
import Container from '@mui/material/Container';
import Divider from '@mui/material/Divider';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import Typography from '@mui/material/Typography';
import { getWeeklyReflection } from '../api/reflections';
import { ActivityHeatmap } from '../components/dashboard/ActivityHeatmap';
import { CategoryChart } from '../components/dashboard/CategoryChart';
import { PatternHighlights } from '../components/dashboard/PatternHighlights';
import { SentimentTrendChart } from '../components/dashboard/SentimentTrendChart';

export function DashboardPage() {
  const { data, isLoading, isError } = useQuery({
    queryKey: ['weeklyReflection'],
    queryFn: () => getWeeklyReflection(),
  });

  if (isLoading) {
    return (
      <Container maxWidth="md" sx={{ py: 4, display: 'flex', justifyContent: 'center' }}>
        <CircularProgress />
      </Container>
    );
  }

  if (isError || !data) {
    return (
      <Container maxWidth="md" sx={{ py: 4 }}>
        <Typography color="error">Failed to load weekly reflection.</Typography>
      </Container>
    );
  }

  const weekLabel = `${new Date(data.weekStart).toLocaleDateString()} – ${new Date(data.weekEnd).toLocaleDateString()}`;

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        Weekly Dashboard
      </Typography>
      <Typography variant="subtitle1" color="text.secondary" gutterBottom>
        {weekLabel} · {data.totalThoughts} thought{data.totalThoughts !== 1 ? 's' : ''}
      </Typography>

      <Divider sx={{ my: 2 }} />

      {/* AI Summary */}
      <Box mb={3}>
        <Typography variant="h6" gutterBottom>AI Summary</Typography>
        {data.aiSummary ? (
          <Typography variant="body1">{data.aiSummary}</Typography>
        ) : (
          <Chip label="AI Summary Not Configured" variant="outlined" />
        )}
      </Box>

      <Divider sx={{ my: 2 }} />

      {/* Category Distribution */}
      <Box mb={3}>
        <Typography variant="h6" gutterBottom>Category Distribution</Typography>
        <CategoryChart data={data.categoryDistribution} />
      </Box>

      <Divider sx={{ my: 2 }} />

      {/* Sentiment Trend */}
      <Box mb={3}>
        <Typography variant="h6" gutterBottom>Sentiment Trend</Typography>
        <SentimentTrendChart data={data.dailySentimentTrend} />
      </Box>

      <Divider sx={{ my: 2 }} />

      {/* Peak Activity */}
      <Box mb={3}>
        <Typography variant="h6" gutterBottom>Peak Activity by Hour</Typography>
        <ActivityHeatmap data={data.peakActivityByHour} />
      </Box>

      <Divider sx={{ my: 2 }} />

      {/* Pattern Highlights */}
      <Box mb={3}>
        <Typography variant="h6" gutterBottom>Pattern Highlights</Typography>
        <PatternHighlights insights={data.topInsights} />
      </Box>

      {data.recommendations.length > 0 && (
        <>
          <Divider sx={{ my: 2 }} />
          <Box mb={3}>
            <Typography variant="h6" gutterBottom>Personalized Recommendations</Typography>
            <List disablePadding>
              {data.recommendations.map((rec, i) => (
                <ListItem key={i} disableGutters>
                  <Alert severity="info" sx={{ width: '100%' }}>
                    <ListItemText primary={rec} />
                  </Alert>
                </ListItem>
              ))}
            </List>
          </Box>
        </>
      )}
    </Container>
  );
}
