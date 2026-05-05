import { COLUMN_FORMULAS, SURFACE_FORMULAS } from "@/constants/formulas";

export interface DataRow {
  x: number;
  [key: string]: number;
}

export const getLocalDatabase = (
  rows: number = 100,
  cols: number = 10,
  formulaIndex: number = 0, // какую формулу использовать
): DataRow[] => {
  const formula = COLUMN_FORMULAS[formulaIndex]?.fn || COLUMN_FORMULAS[0].fn;

  return Array.from({ length: rows }, (_, i) => {
    const x = i * 0.2;
    const row: DataRow = { x: parseFloat(x.toFixed(2)) };

    for (let j = 1; j <= cols; j++) {
      row[`y${j}`] = parseFloat(formula(x, j, cols).toFixed(4));
    }
    return row;
  });
};

export const getSurfaceData = (
  size: number = 30,
  formulaIndex: number = 0, // какую формулу использовать
): number[][] => {
  const formula = SURFACE_FORMULAS[formulaIndex]?.fn || SURFACE_FORMULAS[0].fn;
  const z: number[][] = [];

  for (let i = 0; i < size; i++) {
    const row: number[] = [];
    for (let j = 0; j < size; j++) {
      row.push(parseFloat(formula(i / 5, j / 5).toFixed(4)));
    }
    z.push(row);
  }
  return z;
};
