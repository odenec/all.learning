import { NextResponse } from "next/server";
import { defaultComputer } from "@/lib";

export async function GET() {
  try {
    const functionInfo = {
      expression: defaultComputer.functionExpression,
      variant: defaultComputer.variant,
    };

    return NextResponse.json(functionInfo);
  } catch {
    return NextResponse.json(
      { error: "Не удалось получить информацию о функции" },
      { status: 500 },
    );
  }
}
