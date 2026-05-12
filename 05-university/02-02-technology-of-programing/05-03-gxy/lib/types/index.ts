export type ComputeParams = {
  x_values: number[];
  y_values: number[];
};

export type ComputeResult = {
  x: number;
  y: number;
  result: number;
};

export type GridData = {
  x_values: number[];
  y_values: number[];
  matrix: number[][];
  functionExpression: string;
  variant: number;
  dataFile: string;
};

export type RezRowResult = {
  rowIndex: number;
  y: number;
  values: number[];
};

export type BinaryReadResult = {
  dataSetNumber: number;
  x_values: number[];
  selectedRows: RezRowResult[];
  functionExpression: string;
};
