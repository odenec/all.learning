export default function BackgroundDecoration() {
  return (
    <>
      <div className="absolute top-0 left-0 w-full h-full bg-[radial-gradient(circle_at_30%_20%,#e0e7ff_0%,#f8fafc_100%)] -z-10" />
      <div className="absolute -top-24 -right-24 w-96 h-96 bg-teal-100 blur-[120px] rounded-full -z-10" />
    </>
  );
}
