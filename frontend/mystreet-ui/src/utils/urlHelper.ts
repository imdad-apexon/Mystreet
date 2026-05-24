export const getImageUrl = (url?: string) => {
//   if (!url) return '';

//   // Already full URL
//   if (url.startsWith('http')) return url;

  return `${import.meta.env.VITE_API_BASE_URL}${url}`;
};