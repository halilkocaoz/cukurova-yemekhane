import { IFood } from "./food";

export interface IMenu {
  date: string;
  foods: IFood[];
  totalCalories: number;
  detail: string;
}

export interface IMenuApiResponse {
  success: boolean;
  errorMessage: string | null;
  data: IMenu;
}

export interface IMenuListApiResponse {
  success: boolean;
  errorMessage: string | null;
  data: IMenu[];
}
