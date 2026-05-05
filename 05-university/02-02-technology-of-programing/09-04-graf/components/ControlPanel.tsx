import ChartTypeButtons from "@/components/ChartTypeButtons";
import SliderControl from "@/components/SliderControl";
import ColorPicker from "@/components/ColorPicker";

type ControlPanelProps = {
  chartType: "scatter" | "line" | "surface";
  setChartType: (type: "scatter" | "line" | "surface") => void;
  activeCols: number;
  setActiveCols: (n: number) => void;
  axisThickness: number;
  setAxisThickness: (n: number) => void;
  dataThickness: number;
  setDataThickness: (n: number) => void;
  chartBgColor: string;
  setChartBgColor: (color: string) => void;
  isGridVisible: boolean;
  setIsGridVisible: (v: boolean) => void;
};

export default function ControlPanel(props: ControlPanelProps) {
  return (
    <section className="bg-white/70 backdrop-blur-xl border border-white p-8 rounded-3xl shadow-xl flex flex-col gap-8 h-fit">
      <h2 className="text-xl font-black bg-gradient-to-r from-indigo-600 to-teal-500 bg-clip-text text-transparent uppercase tracking-tight">
        Настройки 9
      </h2>

      <div className="space-y-6">
        <ChartTypeButtons
          chartType={props.chartType}
          onChange={props.setChartType}
        />

        <div className="space-y-4 pt-4 border-t border-slate-100">
          <label className="flex flex-col gap-2 text-xs font-bold text-slate-500">
            Количество серий: {props.activeCols}
            <input
              type="range"
              min="1"
              max="10"
              value={props.activeCols}
              onChange={(e) => props.setActiveCols(Number(e.target.value))}
              className="accent-indigo-600"
            />
          </label>

          <SliderControl
            label="Толщина осей"
            value={props.axisThickness}
            min={1}
            max={10}
            onChange={props.setAxisThickness}
            accentClass="accent-indigo-600"
          />

          <SliderControl
            label="Толщина данных"
            value={props.dataThickness}
            min={1}
            max={15}
            onChange={props.setDataThickness}
            accentClass="accent-indigo-600"
          />

          <ColorPicker
            currentColor={props.chartBgColor}
            onChange={props.setChartBgColor}
          />

          <label className="flex items-center justify-between pt-2 cursor-pointer">
            <span className="text-xs font-bold text-slate-500">Сетка (13)</span>
            <input
              type="checkbox"
              checked={props.isGridVisible}
              onChange={(e) => props.setIsGridVisible(e.target.checked)}
              className="w-5 h-5 accent-indigo-600 rounded"
            />
          </label>
        </div>
      </div>
    </section>
  );
}
