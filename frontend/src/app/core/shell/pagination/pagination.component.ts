import {
  Component,
  input,
  OnChanges,
  output,
  signal,
  SimpleChanges,
} from '@angular/core';

import { IPagination } from '../../../shared/interfaces/IPagination.interface';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [],
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.scss',
})
export class PaginationComponent implements OnChanges {
  public currentPage = signal(1);
  public totalPages = signal(0);
  public dataPerPage: 25 | 50 | 100 = 25;
  public totalData = input.required<any[]>();
  public paginatedData = output<IPagination>();
  public currentRecord = 0;
  public lastRecord = 0;

  ngOnChanges(changes: SimpleChanges): void {
    const totalData: any[] = changes['totalData']?.currentValue;
    if (totalData && totalData?.length === 0) {
      this.currentRecord = 0;
      this.lastRecord = 0;
      return;
    }
    this.updatePagination();
  }

  public goToPrevPage(): void {
    this.currentPage.update((c) => Math.max(1, c - 1));
    this.updatePagination();
  }

  public goToFirstPage(): void {
    this.currentPage.set(1);
    this.updatePagination();
  }

  public goToNextPage(): void {
    this.currentPage.update((c) => Math.min(this.totalPages(), c + 1));
    this.updatePagination();
  }

  public goToLastPage(): void {
    this.currentPage.set(this.totalPages());
    this.updatePagination();
  }

  public onToggleQuantity(event: Event): void {
    const span = event.target as HTMLSpanElement;
    if (!span.classList.contains('btn')) return;

    this.dataPerPage = Number(span.innerHTML) as 25 | 50 | 100;
    this.currentPage.set(1);
    this.updatePagination();
  }

  private updatePagination(): void {
    const data = this.totalData().slice();

    const totalPages = Math.ceil(data.length / this.dataPerPage);
    this.totalPages.set(totalPages);

    const currentPage =
      this.currentPage() <= totalPages ? this.currentPage() : totalPages;
    this.currentPage.set(currentPage);

    const startIndex = (currentPage - 1) * this.dataPerPage;
    const endIndex = Math.min(startIndex + this.dataPerPage, data.length);

    this.currentRecord = startIndex + 1;
    this.lastRecord = endIndex;

    this.paginatedData.emit({ startIndex, endIndex });
  }
}
