import { NextResponse } from "next/server";
import { DataReader } from "@/lib/readers/dataReader";

export async function GET() {
  try {
    const reader = new DataReader();
    const allData = reader.readAllDataFiles();

    return NextResponse.json({
      success: true,
      data: allData,
    });
  } catch {
    return NextResponse.json(
      { error: "Ошибка при чтении данных" },
      { status: 500 },
    );
  }
}
