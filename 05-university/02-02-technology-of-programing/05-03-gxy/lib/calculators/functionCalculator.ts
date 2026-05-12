import { ComputeParams, ComputeResult } from "../types";

export class FunctionCalculator {
  compute(x: number, y: number): number {
    return y / Math.log10(x);
  }

  computeAll(params: ComputeParams): ComputeResult[] {
    const { x_values, y_values } = params;
    const results: ComputeResult[] = [];

    for (let i = 0; i < x_values.length; i++) {
      for (let j = 0; j < y_values.length; j++) {
        const x = x_values[i];
        const y = y_values[j];
        const result = this.compute(x, y);

        if (!isFinite(result)) {
          throw new Error("Переполнение");
        }

        results.push({ x, y, result });
      }
    }

    return results;
  }
}
