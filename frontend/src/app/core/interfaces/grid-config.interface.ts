export interface GridConfig {
  pageSize: number;
  allowSorting: boolean;
  allowPaging: boolean;
  rows: Record<string, any>[];
}
