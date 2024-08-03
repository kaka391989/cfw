export const AlbumArtwork = ({
  album,
  className,
  aspectRatio,
  width,
  height,
}: any) => {
  return (
    <div className={className}>
      <img
        src={album.coverUrl}
        alt={album.name}
        width={width}
        height={height}
        style={{ aspectRatio }}
      />
    </div>
  );
};
