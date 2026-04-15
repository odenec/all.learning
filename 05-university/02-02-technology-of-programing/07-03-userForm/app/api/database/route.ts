import { NextRequest, NextResponse } from "next/server";
import { FileStorageService, UserData } from "./FileStorageService";

export async function GET() {
  try {
    const service = new FileStorageService();
    const data = await service.load();
    return NextResponse.json({ success: true, data });
  } catch (error) {
    return NextResponse.json(
      { success: false, error: "Ошибка загрузки файла" },
      { status: 500 },
    );
  }
}

export async function POST(request: NextRequest) {
  try {
    const body = await request.json();
    const users: UserData[] = body.users;

    if (!Array.isArray(users)) {
      return NextResponse.json(
        { success: false, error: "Неверный формат данных" },
        { status: 400 },
      );
    }

    const service = new FileStorageService();
    await service.save(users);

    return NextResponse.json({
      success: true,
      message: `Сохранено ${users.length} записей`,
    });
  } catch (error) {
    return NextResponse.json(
      { success: false, error: "Ошибка сохранения файла" },
      { status: 500 },
    );
  }
}
