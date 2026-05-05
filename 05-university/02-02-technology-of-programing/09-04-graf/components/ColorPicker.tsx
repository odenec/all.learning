import { COLOR_PALETTE } from "@/constants/colors";

type ColorPickerProps = {
  currentColor: string;
  onChange: (color: string) => void;
};

export default function ColorPicker({
  currentColor,
  onChange,
}: ColorPickerProps) {
  return (
    <div className="space-y-2">
      <span className="text-xs font-bold text-slate-500">Фон (9)</span>
      <div className="grid grid-cols-4 gap-y-3 gap-x-1 w-fit">
        {COLOR_PALETTE.map((color) => (
          <button
            key={color.bg}
            title={color.name}
            onClick={() => onChange(color.bg)}
            className={`w-6 h-6 rounded-full border-2 transition-all ${
              currentColor === color.bg
                ? "border-indigo-500 scale-125 z-10 shadow-sm"
                : "border-transparent hover:scale-110"
            }`}
            style={{ backgroundColor: color.bg }}
          />
        ))}
      </div>
    </div>
  );
}
