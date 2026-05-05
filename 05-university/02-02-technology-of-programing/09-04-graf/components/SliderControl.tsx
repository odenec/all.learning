type SliderControlProps = {
  label: string;
  value: number;
  min: number;
  max: number;
  onChange: (value: number) => void;
  accentClass?: string;
};

export default function SliderControl({
  label,
  value,
  min,
  max,
  onChange,
  accentClass = "accent-indigo-600",
}: SliderControlProps) {
  return (
    <label className="flex flex-col gap-2 text-xs font-bold text-slate-500">
      {label}: {value}px
      <input
        type="range"
        min={min}
        max={max}
        value={value}
        onChange={(e) => onChange(Number(e.target.value))}
        className={accentClass}
      />
    </label>
  );
}
