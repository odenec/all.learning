import { ComputeResult } from "../types";

export class TableFormatter {
  formatTable(
    x_values: number[],
    y_values: number[],
    results: ComputeResult[],
  ): string {
    const resultMap = new Map();
    results.forEach((r) => {
      resultMap.set(`${r.x},${r.y}`, r.result);
    });

    const lines: string[] = [];

    let maxWidth = 8;

    x_values.forEach((x) => {
      const xStr = x.toFixed(3);
      if (xStr.length > maxWidth) maxWidth = xStr.length;
    });

    y_values.forEach((y) => {
      const yStr = y.toFixed(3);
      if (yStr.length > maxWidth) maxWidth = yStr.length;
    });

    results.forEach((r) => {
      const rStr = r.result.toFixed(6);
      if (rStr.length > maxWidth) maxWidth = rStr.length;
    });

    maxWidth += 2;

    const formatX = (x: number) => x.toFixed(3).padStart(maxWidth);
    const formatY = (y: number) => y.toFixed(3).padEnd(maxWidth);
    const formatResult = (r: number) => r.toFixed(6).padStart(maxWidth);

    const headerX = x_values.map(formatX).join("");
    lines.push("y\\x".padEnd(maxWidth) + headerX);

    y_values.forEach((y) => {
      const yStr = formatY(y);
      const row = x_values
        .map((x) => {
          const result = resultMap.get(`${x},${y}`);
          return result !== undefined
            ? formatResult(result)
            : "N/A".padStart(maxWidth);
        })
        .join("");
      lines.push(yStr + row);
    });

    return lines.join("\n");
  }
}
