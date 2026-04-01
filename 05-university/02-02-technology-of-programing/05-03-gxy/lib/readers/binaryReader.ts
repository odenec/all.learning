import fs from "fs";
import path from "path";
import { BinaryReadResult, RezRowResult } from "../types";
import { defaultComputer } from "@/lib";

export class BinaryRezReader {
  private outputDir: string;

  constructor(outputDir: string = path.join(process.cwd(), "output")) {
    this.outputDir = outputDir;
  }

  readSelectedRows(
    dataSetNumber: number,
    rowIndices: number[],
  ): BinaryReadResult {
    const filename = `G${dataSetNumber.toString().padStart(4, "0")}.rez`;
    const filePath = path.join(this.outputDir, filename);

    if (!fs.existsSync(filePath)) {
      throw new Error(`Файл ${filename} не найден`);
    }

    const buffer = fs.readFileSync(filePath);

    let offset = 0;

    const Nx = buffer.readUInt32LE(offset);
    offset += 4;
    const Ny = buffer.readUInt32LE(offset);
    offset += 4;

    const x_values: number[] = [];
    for (let i = 0; i < Nx; i++) {
      x_values.push(buffer.readDoubleLE(offset));
      offset += 8;
    }

    const y_values: number[] = [];
    for (let i = 0; i < Ny; i++) {
      y_values.push(buffer.readDoubleLE(offset));
      offset += 8;
    }

    const matrixOffset = offset;

    const selectedRows: RezRowResult[] = [];

    for (const rowIdx of rowIndices) {
      if (rowIdx < 0 || rowIdx >= Ny) {
        throw new Error(`Строка ${rowIdx} вне диапазона (0..${Ny - 1})`);
      }

      const rowOffset = matrixOffset + rowIdx * Nx * 8;
      const values: number[] = [];

      for (let x = 0; x < Nx; x++) {
        values.push(buffer.readDoubleLE(rowOffset + x * 8));
      }

      selectedRows.push({
        rowIndex: rowIdx,
        y: y_values[rowIdx],
        values,
      });
    }

    const functionExpression = defaultComputer.functionExpression;

    return {
      dataSetNumber,
      x_values,
      selectedRows,
      functionExpression,
    };
  }
}
