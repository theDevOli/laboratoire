import { Injectable, signal, WritableSignal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LoaderService {
  private isLoading = signal<boolean>(false);

  constructor() {}

  public get loading(): WritableSignal<boolean> {
    return this.isLoading;
  }

  public setLoading(): void {
    this.isLoading.update((current) => !current);
  }
}
