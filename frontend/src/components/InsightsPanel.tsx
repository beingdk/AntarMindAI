// Modified by AI on 05/04/2026. Edit #1.
import { useQuery } from '@tanstack/react-query';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import Typography from '@mui/material/Typography';
import Chip from '@mui/material/Chip';
import CircularProgress from '@mui/material/CircularProgress';
import Box from '@mui/material/Box';
import { getInsights } from '../api/insights';

const categoryColor: Record<string, 'primary' | 'secondary' | 'warning' | 'error' | 'success' | 'default'> = {
  Frequency: 'primary',
  TimeTrend: 'secondary',
  Repetition: 'warning',
  Trigger: 'error',
};

export function InsightsPanel() {
  const { data, isLoading, isError } = useQuery({
    queryKey: ['insights'],
    queryFn: getInsights,
  });

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" py={2}>
        <CircularProgress size={24} />
      </Box>
    );
  }

  if (isError) {
    return (
      <Typography color="error" variant="body2">
        Failed to load insights.
      </Typography>
    );
  }

  if (!data || data.insights.length === 0) {
    return (
      <Typography variant="body2" color="text.secondary">
        No insights yet. Keep logging thoughts — insights appear after {data?.thoughtCount !== undefined ? `${10 - data.thoughtCount} more` : 'enough'} entries.
      </Typography>
    );
  }

  return (
    <List disablePadding>
        {data.insights.map((insight, idx) => (
          <ListItem key={idx} disableGutters alignItems="flex-start" sx={{ py: 0.5 }}>
            <ListItemText
              primary={
                <Box display="flex" alignItems="center" gap={1} flexWrap="wrap">
                  <Chip
                    label={insight.category}
                    size="small"
                    color={categoryColor[insight.category] ?? 'default'}
                  />
                  <Typography variant="body2">{insight.message}</Typography>
                </Box>
              }
            />
          </ListItem>
        ))}
      </List>
  );
}
