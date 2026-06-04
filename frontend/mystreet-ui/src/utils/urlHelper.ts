export const getImageUrl = (url?: string) => {
  if (!url) return '';

  // Already full URL
  if (url.startsWith('http')) return url;

  // Ensure proper path construction with /
  const baseUrl = import.meta.env.VITE_API_BASE_URL || '';
  const path = url.startsWith('/') ? url : `/${url}`;
  return `${baseUrl}${path}`;
};