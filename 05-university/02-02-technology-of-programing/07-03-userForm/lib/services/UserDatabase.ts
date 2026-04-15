import { User } from "@/lib/models/User";

export class UserDatabase {
  private users: Map<string, User> = new Map();

  add(user: User): { success: boolean; error?: string } {
    if (this.users.has(user.id)) {
      return { success: false, error: "ФИО уже существует" };
    }
    if (!user.validatePercent()) {
      return { success: false, error: "Некорректный процент" };
    }
    this.users.set(user.id, user);
    return { success: true };
  }

  getAll(): User[] {
    return Array.from(this.users.values());
  }

  getById(id: string): User | undefined {
    return this.users.get(id);
  }

  replaceAll(users: User[]): void {
    this.users.clear();
    users.forEach((u) => this.users.set(u.id, u));
  }

  merge(users: User[]): void {
    users.forEach((u) => {
      if (!this.users.has(u.id)) {
        this.users.set(u.id, u);
      }
    });
  }

  clear(): void {
    this.users.clear();
  }

  get count(): number {
    return this.users.size;
  }
}
