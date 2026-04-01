import path from "path";
import { ComputeParams, ComputeResult } from "./types";
import { FunctionValidator } from "./validators/functionValidators";
import { FunctionCalculator } from "./calculators/functionCalculator";
import { TableFormatter } from "./formatters/tableFormatter";
import { FileLogger } from "./loggers/fileLogger";
import { BinaryRezWriter } from "./writers/binaryRezWriter";

export class FunctionComputer {
  public variant: number;
  public functionExpression: string;
  private programName: string;
  private validator: FunctionValidator;
  private calculator: FunctionCalculator;
  private formatter: TableFormatter;
  private logger: FileLogger;
  private binaryWriter: BinaryRezWriter;

  constructor(variant: number = 16, functionExpression: string = "y / lg(x)") {
    this.variant = variant;
    this.functionExpression = functionExpression;
    this.programName = "G(x,y) Calculator";
    this.validator = new FunctionValidator();
    this.calculator = new FunctionCalculator();
    this.formatter = new TableFormatter();
    this.logger = new FileLogger(path.join(process.cwd(), "output"));
    this.binaryWriter = new BinaryRezWriter();
  }

  public computeFunction(
    params: ComputeParams,
    dataSetNumber: number = 1,
  ): {
    variant: number;
    functionExpression: string;
    results: ComputeResult[];
    dataFile: string;
    rezFile: string;
  } {
    const errors: Array<{
      filename: string;
      x: number;
      y: number;
      error: string;
    }> = [];
    let results: ComputeResult[] = [];

    try {
      this.validator.validateParams(params);

      try {
        results = this.calculator.computeAll(params);
      } catch (error) {
        const filename = this.logger.getDataFilename(dataSetNumber);
        errors.push({
          filename,
          x: params.x_values[0],
          y: params.y_values[0],
          error: error instanceof Error ? error.message : "Неизвестная ошибка",
        });
      }

      if (errors.length > 0) {
        this.logger.writeErrorLog(this.functionExpression, errors);
      }

      const dataFilename = this.logger.getDataFilename(dataSetNumber);

      this.logger.clearDataFile(dataFilename);

      const tableContent = this.formatter.formatTable(
        params.x_values,
        params.y_values,
        results,
      );

      const functionHeader = `G(x,y) = ${this.functionExpression}`;

      const dataContent = [
        functionHeader,
        `Количество точек: x=${params.x_values.length}, y=${params.y_values.length}`,

        "",
        tableContent,
        "",
      ].join("\n");

      this.logger.writeDataFile(dataFilename, dataContent);
      //lab 6

      const matrix: number[][] = [];
      for (let yIdx = 0; yIdx < params.y_values.length; yIdx++) {
        const row: number[] = [];
        for (let xIdx = 0; xIdx < params.x_values.length; xIdx++) {
          const res = results.find(
            (r) =>
              Math.abs(r.x - params.x_values[xIdx]) < 1e-9 &&
              Math.abs(r.y - params.y_values[yIdx]) < 1e-9,
          );
          row.push(res ? res.result : NaN);
        }
        matrix.push(row);
      }

      const rezFilename = this.binaryWriter.writeRezFile(
        dataSetNumber,
        params.x_values,
        params.y_values,
        matrix,
      );

      return {
        variant: this.variant,
        functionExpression: this.functionExpression,
        results,
        dataFile: dataFilename,
        rezFile: rezFilename,
      };
    } catch (error) {
      const errorMessage =
        error instanceof Error ? error.message : "Неизвестная ошибка";
      throw new Error(`Ошибка вычисления: ${errorMessage}`);
    }
  }

  public computeMultiple(paramsArray: ComputeParams[]): Array<{
    variant: number;
    functionExpression: string;
    results: ComputeResult[];
    dataFile: string;
  }> {
    const startTime = new Date();
    this.logger.clearOutputDir();
    const results = [];
    const dataFiles: string[] = [];

    for (let i = 0; i < paramsArray.length; i++) {
      try {
        const result = this.computeFunction(paramsArray[i], i + 1);
        results.push(result);
        dataFiles.push(result.dataFile);
      } catch (error) {
        console.error(`Ошибка при обработке набора ${i + 1}:`, error);
      }
    }

    this.logger.writeProgramLog(
      this.programName,
      this.variant,
      this.functionExpression,
      startTime,
      dataFiles,
    );

    return results;
  }
}

export const defaultComputer = new FunctionComputer(16, "y / lg(x)");
