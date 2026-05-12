import { NextResponse } from "next/server";
import { FunctionComputer } from "@/lib";

type CalculationInput = {
  x_values: number[];
  y_values: number[];
};

const computer = new FunctionComputer(16, "y / lg(x)");

export async function POST(req: Request) {
  try {
    const body = await req.json();
    const { calculations } = body as { calculations: CalculationInput[] };

    if (!Array.isArray(calculations)) {
      return NextResponse.json(
        { error: "Нужен массив calculations" },
        { status: 400 },
      );
    }

    const results = computer.computeMultiple(calculations);

    return NextResponse.json({
      success: true,
      data: results.map((result) => ({
        variant: result.variant,
        functionExpression: result.functionExpression,
        results: result.results,
        dataFile: result.dataFile,
      })),
    });
  } catch (error) {
    const message =
      error instanceof Error ? error.message : "Неизвестная ошибка";
    return NextResponse.json({ error: message }, { status: 400 });
  }
}
