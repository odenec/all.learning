export class User {
  constructor(
    public fullName: string,
    public city: string,
    public address: string,
    public percent: number,
    public photo: string | null = null,
  ) {}

  get id(): string {
    return this.fullName;
  }

  validatePercent(): boolean {
    return !isNaN(this.percent) && this.percent >= 0;
  }

  toPlainObject() {
    return {
      fullName: this.fullName,
      city: this.city,
      address: this.address,
      percent: this.percent,
      photo: this.photo,
    };
  }

  static fromPlainObject(obj: any): User {
    return new User(
      obj.fullName,
      obj.city,
      obj.address,
      obj.percent,
      obj.photo,
    );
  }
}
