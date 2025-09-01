export interface ICropGet {
  cropId: number;
  cropName: string;
  nitrogenCover: number;
  nitrogenFoundation: number;
  phosphorus: {
    min: number;
    med: number;
    max: number;
  };
  potassium: {
    min: number;
    med: number;
    max: number;
  };
  minV: number;
}
