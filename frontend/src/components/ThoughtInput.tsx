// Modified by AI on 05/04/2026. Edit #2.
import Alert from '@mui/material/Alert';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import IconButton from '@mui/material/IconButton';
import TextField from '@mui/material/TextField';
import Tooltip from '@mui/material/Tooltip';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useCallback, useEffect, useRef, useState } from 'react';
import { createThought } from '../api/thoughts';

// Browser SpeechRecognition API — not in standard TS lib, accessed via `any` cast
type SpeechRecognitionResult = { isFinal: boolean; 0: { transcript: string } };
type SpeechRecognitionResultEvent = {
  resultIndex: number;
  results: SpeechRecognitionResult[] & { length: number };
};

type AnySpeechRecognition = {
  continuous: boolean;
  interimResults: boolean;
  lang: string;
  start(): void;
  stop(): void;
  onresult: ((event: SpeechRecognitionResultEvent) => void) | null;
  onend: (() => void) | null;
  onerror: (() => void) | null;
};

const AnyWindow = window as Window & {
  SpeechRecognition?: new () => AnySpeechRecognition;
  webkitSpeechRecognition?: new () => AnySpeechRecognition;
};

const SpeechRecognitionCtor = AnyWindow.SpeechRecognition ?? AnyWindow.webkitSpeechRecognition;
const speechAvailable = !!SpeechRecognitionCtor;

export function ThoughtInput() {
  const [text, setText] = useState('');
  const [isRecording, setIsRecording] = useState(false);
  const recognitionRef = useRef<AnySpeechRecognition | null>(null);
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: createThought,
    onSuccess: () => {
      setText('');
      queryClient.invalidateQueries({ queryKey: ['thoughts'] });
    },
  });

  const stopRecording = useCallback(() => {
    recognitionRef.current?.stop();
    recognitionRef.current = null;
    setIsRecording(false);
  }, []);

  const startRecording = useCallback(() => {
    if (!SpeechRecognitionCtor) return;
    const recognition = new SpeechRecognitionCtor();
    recognition.continuous = true;
    recognition.interimResults = true;
    recognition.lang = 'en-US';

    recognition.onresult = (event: SpeechRecognitionResultEvent) => {
      let interimTranscript = '';
      let finalTranscript = '';
      for (let i = event.resultIndex; i < event.results.length; i++) {
        const transcript = event.results[i][0].transcript;
        if (event.results[i].isFinal) {
          finalTranscript += transcript;
        } else {
          interimTranscript += transcript;
        }
      }
      setText((prev) => prev + finalTranscript + interimTranscript);
    };

    recognition.onend = () => {
      setIsRecording(false);
      recognitionRef.current = null;
    };

    recognition.onerror = () => {
      stopRecording();
    };

    recognitionRef.current = recognition;
    recognition.start();
    setIsRecording(true);
  }, [stopRecording]);

  // Clean up on unmount
  useEffect(() => () => stopRecording(), [stopRecording]);

  const handleSubmit = () => {
    if (text.trim().length === 0) return;
    mutation.mutate({ text: text.trim() });
  };

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1, mb: 3 }}>
      <Box sx={{ position: 'relative' }}>
        <TextField
          label="What's on your mind?"
          multiline
          minRows={3}
          value={text}
          onChange={(e) => setText(e.target.value)}
          inputProps={{ maxLength: 2000 }}
          disabled={mutation.isPending}
          fullWidth
        />
        {speechAvailable && (
          <Tooltip title={isRecording ? 'Stop recording' : 'Record voice'}>
            <IconButton
              onClick={isRecording ? stopRecording : startRecording}
              disabled={mutation.isPending}
              sx={{
                position: 'absolute',
                bottom: 8,
                right: 8,
                color: isRecording ? 'error.main' : 'action.active',
                animation: isRecording ? 'pulse 1.2s infinite' : 'none',
                '@keyframes pulse': {
                  '0%, 100%': { opacity: 1 },
                  '50%': { opacity: 0.4 },
                },
              }}
              aria-label={isRecording ? 'Stop recording' : 'Start voice input'}
            >
              <span style={{ fontSize: '1.2rem' }}>{isRecording ? '⏹' : '🎤'}</span>
            </IconButton>
          </Tooltip>
        )}
      </Box>
      {mutation.isError && (
        <Alert severity="error">{mutation.error?.message ?? 'Something went wrong'}</Alert>
      )}
      <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
        <Button
          variant="contained"
          onClick={handleSubmit}
          disabled={mutation.isPending || text.trim().length === 0}
          startIcon={mutation.isPending ? <CircularProgress size={16} color="inherit" /> : null}
        >
          {mutation.isPending ? 'Saving…' : 'Save Thought'}
        </Button>
      </Box>
    </Box>
  );
}
