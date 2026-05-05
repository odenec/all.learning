export type Matrix = (number | string)[][];

//  a_ij = 10 * (-1)^(i+j) + i + j/10
export const generateMatrix = (n: number, m: number): Matrix => {
  const matrix: Matrix = [];
  for (let i = 1; i <= n; i++) {
    const row: (number | string)[] = [];
    for (let j = 1; j <= m; j++) {
      const val = 10 * Math.pow(-1, i + j) + i + j / 10;
      row.push(Number(val.toFixed(1)));
    }
    matrix.push(row);
  }
  return matrix;
};
export const generateMatrixTest = (n: number, m: number): Matrix => {
  const matrix: Matrix = [];
  for (let i = 1; i <= n; i++) {
    const row: (number | string)[] = [];
    for (let j = 1; j <= m; j++) {
      if (i === 1 && j === 1) {
        row.push("тест");
      } else {
        const val = 10 * Math.pow(-1, i + j) + i + j / 10;
        row.push(Number(val.toFixed(1)));
      }
    }
    matrix.push(row);
  }
  return matrix;
};

export const isInteger = (val: any) => Number.isInteger(Number(val));
