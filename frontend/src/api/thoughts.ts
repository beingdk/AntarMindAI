// Modified by AI on 05/04/2026. Edit #1.

export interface ThoughtResponse {
  id: string;
  text: string;
  createdAt: string;
  tags: string[];
  sentiment: 'Positive' | 'Negative' | 'Neutral';
  intensityScore: number;
}

export interface PagedThoughtsResponse {
  items: ThoughtResponse[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface CreateThoughtRequest {
  text: string;
}

const BASE_URL = '/api/thoughts';

export async function createThought(request: CreateThoughtRequest): Promise<ThoughtResponse> {
  const response = await fetch(BASE_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  });

  if (!response.ok) {
    const error = await response.json().catch(() => ({ title: 'Request failed' }));
    throw new Error(error.title ?? 'Failed to create thought');
  }

  return response.json();
}

export async function getThoughts(page = 1, pageSize = 20): Promise<PagedThoughtsResponse> {
  const response = await fetch(`${BASE_URL}?page=${page}&pageSize=${pageSize}`);

  if (!response.ok) {
    throw new Error('Failed to fetch thoughts');
  }

  return response.json();
}

export async function getThoughtById(id: string): Promise<ThoughtResponse> {
  const response = await fetch(`${BASE_URL}/${id}`);

  if (!response.ok) {
    throw new Error('Failed to fetch thought');
  }

  return response.json();
}

export async function deleteThought(id: string): Promise<void> {
  const response = await fetch(`${BASE_URL}/${id}`, { method: 'DELETE' });

  if (!response.ok) {
    const error = await response.json().catch(() => ({ title: 'Request failed' }));
    throw new Error(error.title ?? 'Failed to delete thought');
  }
}
