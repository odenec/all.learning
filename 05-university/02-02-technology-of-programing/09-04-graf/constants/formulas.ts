// Набор формул для Y-колонок
export const COLUMN_FORMULAS: {
  name: string;
  fn: (x: number, j: number, cols: number) => number;
}[] = [
  {
    name: "Синус + косинус",
    fn: (x, j, cols) =>
      Math.sin(x + j) * (cols - j + 1) + Math.cos(x * 0.5) * 2,
  },
  {
    name: "Экспонента",
    fn: (x, j) => Math.exp(-x * 0.3) * Math.sin(x * j) * 5,
  },
  {
    name: "Квадратичная",
    fn: (x, j) => (x - j) ** 2 * 0.5 + Math.cos(x) * 3,
  },
  {
    name: "Логарифмическая",
    fn: (x, j) => Math.log(x + j + 1) * Math.sin(x) * 4,
  },
];

export const SURFACE_FORMULAS: {
  name: string;
  fn: (x: number, y: number) => number;
}[] = [
  {
    name: "Волны",
    fn: (x, y) => Math.sin(x) * Math.cos(y) * 4,
  },
  {
    name: "Седло",
    fn: (x, y) => x ** 2 - y ** 2,
  },
  {
    name: "Рябь",
    fn: (x, y) => Math.sin(Math.sqrt(x ** 2 + y ** 2)) * 3,
  },
  {
    name: "Подушка",
    fn: (x, y) => Math.sin(x) * Math.sin(y) * 5,
  },
];
