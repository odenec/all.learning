"use client";
import { useState, useMemo } from "react";
import { getLocalDatabase, getSurfaceData } from "@/data/db";

export type ChartType = "scatter" | "line" | "surface";

export function useChartLogic(colIndex: number = 1) {
  const [chartType, setChartType] = useState<ChartType>("line");
  const [axisThickness, setAxisThickness] = useState(2);
  const [dataThickness, setDataThickness] = useState(3);
  const [chartBgColor, setChartBgColor] = useState("#e2e8f0");
  const [isGridVisible, setIsGridVisible] = useState(true);

  const db = useMemo(() => getLocalDatabase(), []);
  const surfaceZ = useMemo(() => getSurfaceData(), []);

  const plotData = useMemo(() => {
    if (chartType === "surface") {
      return [
        {
          z: surfaceZ,
          type: "surface",
          colorscale: "Viridis",
          contours: {
            z: { show: isGridVisible, width: dataThickness, color: "#444" },
          },
        },
      ];
    }

    return [
      {
        x: db.map((d) => d.x),
        y: db.map((d) => d[`y${colIndex}`]),
        mode: chartType === "scatter" ? "markers" : "lines",
        type: "scatter",
        name: `Ряд ${colIndex}`,
        line: { width: dataThickness },
        marker: { size: dataThickness + 4 },
      },
    ];
  }, [chartType, dataThickness, isGridVisible, db, surfaceZ, colIndex]);

  return {
    chartType,
    setChartType,
    axisThickness,
    setAxisThickness,
    dataThickness,
    setDataThickness,
    chartBgColor,
    setChartBgColor,
    isGridVisible,
    setIsGridVisible,
    plotData,
  };
}
