"use client";
import { useState } from "react";

export function useChartGrid(initialCount: number = 3) {
  const [chartCount, setChartCount] = useState(initialCount);
  return { chartCount, setChartCount };
}
