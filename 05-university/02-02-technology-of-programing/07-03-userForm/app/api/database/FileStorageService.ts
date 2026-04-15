import fs from "fs/promises";
import path from "path";

export interface UserData {
  fullName: string;
  city: string;
  address: string;
  percent: number;
  photo: string | null;
}

export class FileStorageService {
  private filePath: string;

  constructor(filename: string = "database.dat") {
    this.filePath = path.join(process.cwd(), "data", filename);
  }

  async save(users: UserData[]): Promise<void> {
    try {
      await fs.mkdir(path.dirname(this.filePath), { recursive: true });
      const data = JSON.stringify(users, null, 2);
      await fs.writeFile(this.filePath, data, "utf-8");
    } catch (error) {
      console.error("Ошибка сохранения:", error);
      throw new Error("Не удалось сохранить файл");
    }
  }

  async load(): Promise<UserData[]> {
    try {
      const data = await fs.readFile(this.filePath, "utf-8");
      const parsed = JSON.parse(data);
      return parsed;
    } catch (error) {
      console.error("Ошибка загрузки:", error);
      return [];
    }
  }
}
