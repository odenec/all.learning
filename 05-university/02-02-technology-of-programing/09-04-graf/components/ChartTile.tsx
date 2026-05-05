"use client";
import { useChartLogic } from "@/hooks/useChartLogic";
import { getChartLayout } from "@/config/chartLayout";
import PlotWrapper from "@/components/PlotWrapper";
import ColorPicker from "@/components/ColorPicker";
import SliderControl from "@/components/SliderControl";
import ChartTypeButtons from "@/components/ChartTypeButtons";

type ChartTileProps = {
  colIndex: number;
};

export default function ChartTile({ colIndex }: ChartTileProps) {
  const s = useChartLogic(colIndex);
  const layout = getChartLayout(s, `Ряд ${colIndex}`);

  return (
    <div className="bg-white/70 backdrop-blur-xl border border-white p-4 rounded-3xl shadow-xl flex flex-col gap-4">
      <div
        className="w-full h-64 rounded-2xl overflow-hidden shadow-inner transition-colors duration-500"
        style={{ backgroundColor: s.chartBgColor }}
      >
        <PlotWrapper data={s.plotData} layout={layout} />
      </div>

      <div className="flex flex-wrap items-center gap-3 text-xs">
        <ChartTypeButtons chartType={s.chartType} onChange={s.setChartType} />

        <SliderControl
          label="Оси"
          value={s.axisThickness}
          min={1}
          max={10}
          onChange={s.setAxisThickness}
        />

        <SliderControl
          label="Данные"
          value={s.dataThickness}
          min={1}
          max={15}
          onChange={s.setDataThickness}
        />

        <ColorPicker
          currentColor={s.chartBgColor}
          onChange={s.setChartBgColor}
        />

        <label className="flex items-center gap-1 cursor-pointer">
          <span>Сетка</span>
          <input
            type="checkbox"
            checked={s.isGridVisible}
            onChange={(e) => s.setIsGridVisible(e.target.checked)}
            className="w-4 h-4 accent-indigo-600"
          />
        </label>
      </div>
    </div>
  );
}
