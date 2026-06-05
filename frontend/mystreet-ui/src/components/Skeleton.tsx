import '../styles/skeleton.css';

interface SkeletonProps {
  variant?: 'card' | 'text' | 'circle' | 'button';
  width?: string | number;
  height?: string | number;
  count?: number;
  className?: string;
}

export default function Skeleton({
  variant = 'text',
  width = '100%',
  height = '1rem',
  count = 1,
  className = ''
}: SkeletonProps) {
  const skeletons = Array.from({ length: count });

  const widthStyle = typeof width === 'number' ? `${width}px` : width;
  const heightStyle = typeof height === 'number' ? `${height}px` : height;

  const baseClass = `skeleton skeleton-${variant}`;

  if (variant === 'card') {
    return (
      <div className={`${baseClass} ${className}`}>
        <div className="skeleton skeleton-circle" style={{ width: '100px', height: '100px' }} />
        <div className="skeleton-content">
          <div className="skeleton skeleton-text" style={{ width: '80%', marginBottom: '0.5rem' }} />
          <div className="skeleton skeleton-text" style={{ width: '60%' }} />
        </div>
      </div>
    );
  }

  return (
    <>
      {skeletons.map((_, i) => (
        <div
          key={i}
          className={baseClass}
          style={{
            width: widthStyle,
            height: heightStyle
          }}
        />
      ))}
    </>
  );
}
