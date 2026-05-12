import fs from "fs";
import path from "path";

export class BinaryRezWriter {
  private outputDir: string;

  constructor(outputDir: string = path.join(process.cwd(), "output")) {
    this.outputDir = outputDir;
    if (!fs.existsSync(this.outputDir)) {
      fs.mkdirSync(this.outputDir, { recursive: true });
    }
  }

  writeRezFile(
    dataSetNumber: number,
    x_values: number[],
    y_values: number[],
    matrix: number[][],
  ): string {
    const filename = `G${dataSetNumber.toString().padStart(4, "0")}.rez`;
    const filePath = path.join(this.outputDir, filename);

    const Nx = x_values.length;
    const Ny = y_values.length;

    const bufferSize = 8 + Nx * 8 + Ny * 8 + Nx * Ny * 8;
    //сколько место на всё
    const buffer = Buffer.alloc(bufferSize);

    let offset = 0;

    // cnt point
    buffer.writeUInt32LE(Nx, offset);
    offset += 4;
    buffer.writeUInt32LE(Ny, offset);
    offset += 4;

    // x
    for (const x of x_values) {
      buffer.writeDoubleLE(x, offset);
      offset += 8;
    }

    // y
    for (const y of y_values) {
      buffer.writeDoubleLE(y, offset);
      offset += 8;
    }

    // матрица
    for (let y = 0; y < Ny; y++) {
      for (let x = 0; x < Nx; x++) {
        buffer.writeDoubleLE(matrix[y][x] ?? 0, offset);
        offset += 8;
      }
    }

    fs.writeFileSync(filePath, buffer);
    return filename;
  }
}
