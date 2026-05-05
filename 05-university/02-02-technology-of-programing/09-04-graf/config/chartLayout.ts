import { COLOR_PALETTE } from "@/constants/colors";

type ChartState = {
  chartBgColor: string;
  isGridVisible: boolean;
  axisThickness: number;
};

export function getChartLayout(s: ChartState, title?: string) {
  const activeTheme =
    COLOR_PALETTE.find((c) => c.bg === s.chartBgColor) || COLOR_PALETTE[0];
  const textColor = activeTheme.text;
  const gridColor = activeTheme.grid;

  return {
    title: { text: title || "График", font: { size: 14, color: textColor } },
    plot_bgcolor: s.chartBgColor,
    paper_bgcolor: "transparent",
    font: { family: "Inter, sans-serif", color: textColor },
    xaxis: {
      title: "X",
      showgrid: s.isGridVisible,
      linewidth: s.axisThickness,
      tickwidth: s.axisThickness,
      gridcolor: gridColor,
      hoverformat: ".2f",
    },
    yaxis: {
      title: "Y",
      showgrid: s.isGridVisible,
      linewidth: s.axisThickness,
      tickwidth: s.axisThickness,
      gridcolor: gridColor,
      hoverformat: ".2f",
    },
    autosize: true,
    margin: { t: 40, b: 30, l: 40, r: 20 },
    hoverlabel: {
      bgcolor: s.chartBgColor,
      font: { color: textColor, size: 12, family: "Inter, sans-serif" },
      bordercolor: gridColor,
    },
  };
}
