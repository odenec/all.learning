import { ComputeParams } from "../types";

export class FunctionValidator {
  validateParams(params: ComputeParams): void {
    const { x_values, y_values } = params;

    if (!x_values || x_values.length === 0) {
      throw new Error("Массив x_values не может быть пустым");
    }

    if (!y_values || y_values.length === 0) {
      throw new Error("Массив y_values не может быть пустым");
    }

    for (const x of x_values) {
      if (x <= 0 || Math.abs(x - 1) < 1e-12) {
        console.warn(
          `Внимание: x = ${x} не соответствует ограничениям (x > 0 и x != 1)`,
        );
      }
    }
  }
}
