"use client";
import { useChartGrid } from "@/hooks/useChartGrid";
import ChartTile from "@/components/ChartTile";
import BackgroundDecoration from "@/components/BackgroundDecoration";

export default function Home() {
  const { chartCount, setChartCount } = useChartGrid(1);

  return (
    <main className="min-h-screen p-4 md:p-10 relative overflow-hidden">
      <BackgroundDecoration />

      <div className="max-w-7xl mx-auto space-y-6">
        <div className="flex items-center gap-4">
          <h1 className="text-2xl font-black bg-gradient-to-r from-indigo-600 to-teal-500 bg-clip-text text-transparent uppercase tracking-tight">
            Лаб 9
          </h1>
          <div className="flex items-center gap-2 text-sm text-slate-500">
            <span>Холстов:</span>
            <input
              type="range"
              min="1"
              max="10"
              value={chartCount}
              onChange={(e) => setChartCount(Number(e.target.value))}
              className="accent-indigo-600 w-24"
            />
            <span className="font-bold">{chartCount}</span>
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
          {Array.from({ length: chartCount }, (_, i) => (
            <ChartTile key={i} colIndex={i + 1} />
          ))}
        </div>
      </div>
    </main>
  );
}
