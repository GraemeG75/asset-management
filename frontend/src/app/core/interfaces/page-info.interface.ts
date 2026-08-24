import { PageFormSummary } from './page-form-summary.interface';

export interface PageInfo {
  pageId: string;
  title: string;
  description?: string;
  forms: PageFormSummary[];
}
