export interface User {
  id: number;
  username: string;
  isAdministrator: boolean;
  isBanned: boolean;
  bannedAt: string | null;
  createdAt: string;
}
