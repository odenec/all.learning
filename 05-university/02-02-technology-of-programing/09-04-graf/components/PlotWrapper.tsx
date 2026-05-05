"use client";
import dynamic from "next/dynamic";

const Plot = dynamic(() => import("react-plotly.js"), {
  ssr: false,
  loading: () => (
    <div className="h-full w-full flex items-center justify-center animate-pulse text-slate-400">
      Загрузка...
    </div>
  ),
});

export default function PlotWrapper({ data, layout }: any) {
  return (
    <Plot
      data={data}
      layout={layout}
      useResizeHandler={true}
      className="w-full h-full"
      config={{
        responsive: true,
        displayModeBar: false,
        scrollZoom: false,
      }}
      style={{ width: "100%", height: "100%" }}
      onHover={() => {
        const tooltip = document.querySelector(".hoverlayer");
        if (tooltip) {
          tooltip.setAttribute("style", "pointer-events: none;");
        }
      }}
    />
  );
}
