export interface Review {
  id: number;
  productId: number;
  productName: string;
  userId: number;
  username: string;
  statusId: number;
  statusName: string;
  rating: number;
  text: string;
  createdAt: string;
}
