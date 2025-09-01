export interface ICatalogGet {
  catalogId: number;
  price: number;
  reportType: string;
  sampleType: string;
  labelName:string;
  legends:{unit:string,description:string}[]
}
