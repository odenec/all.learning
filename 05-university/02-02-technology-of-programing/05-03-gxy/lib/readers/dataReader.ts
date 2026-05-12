import fs from "fs";
import path from "path";
import { GridData } from "../types";

export class DataReader {
  private outputDir: string;

  constructor(outputDir: string = path.join(process.cwd(), "output")) {
    this.outputDir = outputDir;
  }

  readAllDataFiles(): GridData[] {
    const results: GridData[] = [];

    try {
      const logPath = path.join(this.outputDir, "myProgram.log");
      if (!fs.existsSync(logPath)) {
        console.error("myProgram.log не найден");
        return [];
      }

      const logContent = fs.readFileSync(logPath, "utf-8");

      const dataFiles: string[] = [];
      const lines = logContent.split("\n");
      for (const line of lines) {
        const match = line.match(/G\d{4}\.dat/);
        if (match) {
          dataFiles.push(match[0]);
        }
      }

      for (const filename of dataFiles) {
        try {
          const data = this.readDataFile(filename);
          results.push(data);
        } catch (error) {
          console.error(`Ошибка при чтении ${filename}:`, error);
        }
      }
    } catch (error) {
      console.error("Ошибка при чтении myProgram.log:", error);
    }

    return results;
  }

  readDataFile(filename: string): GridData {
    const filePath = path.join(this.outputDir, filename);

    if (!fs.existsSync(filePath)) {
      throw new Error(`Файл ${filename} не найден`);
    }

    const content = fs.readFileSync(filePath, "utf-8");
    const lines = content.split("\n").filter((line) => line.trim() !== "");

    if (lines.length < 4) {
      throw new Error("Файл содержит недостаточно данных");
    }

    // Парсим заголовки
    const functionExpression = this.parseFunctionExpression(lines[0]);

    // Ищем вариант (может быть на 2-й или 3-й строке)
    let variant = 0;
    let tableStartIndex = -1;

    for (let i = 0; i < lines.length; i++) {
      if (lines[i].includes("Вариант:")) {
        const variantMatch = lines[i].match(/Вариант:\s*(\d+)/);
        if (variantMatch) {
          variant = parseInt(variantMatch[1], 10);
        }
      }
      // Ищем начало таблицы
      if (lines[i].trim().startsWith("y\\x")) {
        tableStartIndex = i;
        break;
      }
    }

    if (tableStartIndex === -1) {
      throw new Error("Не найден заголовок таблицы");
    }

    // Парсим заголовок X - разбиваем по пробелам, а не по табуляции
    const headerLine = lines[tableStartIndex];
    // Разбиваем по пробелам и фильтруем пустые элементы
    const headerParts = headerLine
      .split(/\s+/)
      .filter((part) => part.trim() !== "");

    const x_values: number[] = [];
    // Первый элемент "y\x" пропускаем
    for (let i = 1; i < headerParts.length; i++) {
      const x = parseFloat(headerParts[i]);
      if (!isNaN(x)) {
        x_values.push(x);
      }
    }

    // Парсим строки с Y и значениями
    const y_values: number[] = [];
    const matrix: number[][] = [];

    for (let i = tableStartIndex + 1; i < lines.length; i++) {
      const line = lines[i].trim();
      if (
        line === "" ||
        line.startsWith("---") ||
        line.startsWith("Количество")
      ) {
        break;
      }

      // Разбиваем по пробелам
      const parts = line.split(/\s+/).filter((part) => part.trim() !== "");
      if (parts.length < 2) continue;

      // Первый элемент - Y
      const y = parseFloat(parts[0]);
      if (isNaN(y)) {
        console.warn(`Некорректное значение Y: ${parts[0]}`);
        continue;
      }
      y_values.push(y);

      // Остальные элементы - результаты
      const row: number[] = [];
      for (let j = 1; j < parts.length; j++) {
        const val = parseFloat(parts[j]);
        if (!isNaN(val)) {
          row.push(val);
        }
      }

      matrix.push(row);
    }

    if (matrix.length === 0) {
      throw new Error("Не удалось распарсить данные таблицы");
    }

    if (matrix.length !== y_values.length) {
      console.warn(
        `Количество строк матрицы (${matrix.length}) не соответствует количеству Y (${y_values.length})`,
      );
    }

    if (matrix[0] && matrix[0].length !== x_values.length) {
      console.warn(
        `Количество столбцов матрицы (${matrix[0].length}) не соответствует количеству X (${x_values.length})`,
      );
    }

    return {
      x_values,
      y_values,
      matrix,
      functionExpression,
      variant,
      dataFile: filename,
    };
  }

  private parseFunctionExpression(line: string): string {
    const match = line.match(/G\(x,y\)\s*=\s*(.+)/);
    if (match) {
      return match[1].trim();
    }
    const functionMatch = line.match(/Функция:\s*(.+)/);
    if (functionMatch) {
      return functionMatch[1].trim();
    }
    return "Неизвестно";
  }

  private parseVariant(line: string): number {
    const match = line.match(/Вариант:\s*(\d+)/);
    if (match) {
      return parseInt(match[1], 10);
    }
    return 0;
  }

  getValue(data: GridData, x: number, y: number): number | null {
    const xIndex = data.x_values.indexOf(x);
    const yIndex = data.y_values.indexOf(y);

    if (xIndex === -1 || yIndex === -1) {
      return null;
    }

    return data.matrix[yIndex]?.[xIndex] ?? null;
  }
}
