import { API_URL } from '../constants/config';
import { authFetch } from '../api/utils/authFetch';

export interface PageContentResponse {
  id: string;
  pageSlug: string;
  title: string;
  contentHtml: string;
  lastModifiedBy: string | null;
  updatedAt: string;
}

export interface PageContentUpdateRequest {
  title: string;
  contentHtml: string;
}

interface ApiResponse<T> {
  success: boolean;
  data: T;
  message: string;
  errors: string[];
}

export async function getPageContent(slug: string): Promise<PageContentResponse> {
  const response = await authFetch(`${API_URL}/page-content/${encodeURIComponent(slug)}`, {
    method: 'GET',
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || 'Failed to fetch page content.');
  }

  const data: ApiResponse<PageContentResponse> = await response.json();
  return data.data;
}

export async function updatePageContent(
  slug: string,
  payload: PageContentUpdateRequest,
): Promise<PageContentResponse> {
  const response = await authFetch(`${API_URL}/page-content/${encodeURIComponent(slug)}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || 'Failed to update page content.');
  }

  const data: ApiResponse<PageContentResponse> = await response.json();
  return data.data;
}

