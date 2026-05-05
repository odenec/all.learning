type ChartTypeButtonsProps = {
  chartType: "scatter" | "line" | "surface";
  onChange: (type: "scatter" | "line" | "surface") => void;
};

const TYPES = ["scatter", "line", "surface"] as const;

export default function ChartTypeButtons({
  chartType,
  onChange,
}: ChartTypeButtonsProps) {
  return (
    <div className="space-y-2">
      <label className="text-[10px] font-bold text-slate-400 uppercase tracking-widest">
        Тип диаграммы
      </label>
      <div className="grid grid-cols-1 gap-2">
        {TYPES.map((type) => (
          <button
            key={type}
            onClick={() => onChange(type)}
            className={`py-2 px-4 rounded-xl text-sm font-bold transition-all ${
              chartType === type
                ? "bg-slate-800 text-white shadow-lg"
                : "bg-white text-slate-500 hover:bg-slate-100 border border-slate-200"
            }`}
          >
            {type.toUpperCase()}
          </button>
        ))}
      </div>
    </div>
  );
}
