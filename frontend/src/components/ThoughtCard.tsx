// Modified by AI on 05/04/2026. Edit #1.
import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import CircularProgress from '@mui/material/CircularProgress';
import IconButton from '@mui/material/IconButton';
import Typography from '@mui/material/Typography';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { deleteThought, type ThoughtResponse } from '../api/thoughts';

function DeleteIcon() {
  return (
    <svg xmlns="http://www.w3.org/2000/svg" height="20" width="20" viewBox="0 0 24 24" fill="currentColor">
      <path d="M6 19c0 1.1.9 2 2 2h8c1.1 0 2-.9 2-2V7H6v12zM19 4h-3.5l-1-1h-5l-1 1H5v2h14V4z" />
    </svg>
  );
}

interface ThoughtCardProps {
  thought: ThoughtResponse;
}

function formatRelativeTime(dateStr: string): string {
  const date = new Date(dateStr);
  const now = new Date();
  const diffMs = now.getTime() - date.getTime();
  const diffMins = Math.floor(diffMs / 60000);

  if (diffMins < 1) return 'just now';
  if (diffMins < 60) return `${diffMins} minute${diffMins === 1 ? '' : 's'} ago`;
  const diffHours = Math.floor(diffMins / 60);
  if (diffHours < 24) return `${diffHours} hour${diffHours === 1 ? '' : 's'} ago`;
  const diffDays = Math.floor(diffHours / 24);
  return `${diffDays} day${diffDays === 1 ? '' : 's'} ago`;
}

function sentimentColor(sentiment: ThoughtResponse['sentiment']): 'success' | 'error' | 'default' {
  if (sentiment === 'Positive') return 'success';
  if (sentiment === 'Negative') return 'error';
  return 'default';
}

export function ThoughtCard({ thought }: ThoughtCardProps) {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: () => deleteThought(thought.id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['thoughts'] });
    },
  });

  return (
    <Card variant="outlined" sx={{ mb: 1 }}>
      <CardContent>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
          <Box sx={{ flex: 1 }}>
            <Typography variant="body1" sx={{ whiteSpace: 'pre-wrap', wordBreak: 'break-word' }}>
              {thought.text}
            </Typography>
            <Typography variant="caption" color="text.secondary" sx={{ mt: 0.5, display: 'block' }}>
              {formatRelativeTime(thought.createdAt)}
            </Typography>
            <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5, mt: 1 }}>
              <Chip
                label={thought.sentiment}
                color={sentimentColor(thought.sentiment)}
                size="small"
                variant="filled"
              />
              {thought.tags.map((tag) => (
                <Chip key={tag} label={tag} size="small" variant="outlined" />
              ))}
            </Box>
          </Box>
          <IconButton
            aria-label="delete thought"
            onClick={() => mutation.mutate()}
            disabled={mutation.isPending}
            size="small"
            sx={{ ml: 1 }}
          >
            {mutation.isPending ? <CircularProgress size={16} /> : <DeleteIcon />}
          </IconButton>
        </Box>
      </CardContent>
    </Card>
  );
}
