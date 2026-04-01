import { NextResponse } from "next/server";
import { BinaryRezReader } from "@/lib/readers/binaryReader";
import fs from "fs";
import path from "path";
import { BinaryReadResult } from "@/lib/types";

interface RezInfo {
  file: string;
  createdAt: string;
}

interface DatasetResult {
  dataSetNumber: number;
  data?: BinaryReadResult;
  rezInfo?: RezInfo;
  error?: string;
}

export async function POST(req: Request) {
  try {
    // Ждём массив наборов
    const datasets: Array<{
      dataSetNumber: number;
      rowIndices: number[] | "all";
    }> = await req.json();

    if (!Array.isArray(datasets)) {
      return NextResponse.json(
        { error: "Должен быть массив наборов" },
        { status: 400 },
      );
    }

    const reader = new BinaryRezReader();
    const results: DatasetResult[] = [];

    for (const item of datasets) {
      const { dataSetNumber, rowIndices } = item;
      const result: DatasetResult = { dataSetNumber };

      try {
        // Проверка номера набора
        if (
          typeof dataSetNumber !== "number" ||
          !Number.isInteger(dataSetNumber)
        ) {
          throw new Error("dataSetNumber должен быть целым числом");
        }

        // Проверка rowIndices
        if (
          rowIndices !== "all" &&
          (!Array.isArray(rowIndices) ||
            !rowIndices.every((n) => typeof n === "number" && n >= 0))
        ) {
          throw new Error(
            "rowIndices должен быть массивом неотрицательных чисел или 'all'",
          );
        }

        // Если all, читаем все строки
        let finalRowIndices: number[] = [];
        if (rowIndices === "all") {
          // Получаем размер файла
          const filename = `G${String(dataSetNumber).padStart(4, "0")}.rez`;
          const filePath = path.join(process.cwd(), "output", filename);

          if (!fs.existsSync(filePath)) {
            throw new Error(`Файл ${filename} не найден`);
          }

          const buffer = fs.readFileSync(filePath);
          const Nx = buffer.readUInt32LE(0);
          const Ny = buffer.readUInt32LE(4);

          finalRowIndices = Array.from({ length: Ny }, (_, i) => i);
        } else {
          finalRowIndices = rowIndices;
        }

        // Чтение выбранных строк
        const data = reader.readSelectedRows(dataSetNumber, finalRowIndices);
        result.data = data;

        // Информация о файле
        const filename = `G${String(dataSetNumber).padStart(4, "0")}.rez`;
        const filePath = path.join(process.cwd(), "output", filename);

        const stats = fs.statSync(filePath);
        result.rezInfo = {
          file: filename,
          createdAt: stats.birthtime.toISOString(),
        };
      } catch (error) {
        result.error =
          error instanceof Error ? error.message : "Неизвестная ошибка";
      }

      results.push(result);
    }

    return NextResponse.json({ success: true, results });
  } catch (error) {
    return NextResponse.json(
      { error: error instanceof Error ? error.message : "Ошибка обработки" },
      { status: 500 },
    );
  }
}
