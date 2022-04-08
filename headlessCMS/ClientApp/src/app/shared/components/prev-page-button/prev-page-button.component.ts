import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { NavigationStart, Router } from '@angular/router';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-prev-page-button',
  templateUrl: './prev-page-button.component.html',
  styleUrls: ['./prev-page-button.component.scss'],
})
export class PrevPageButtonComponent implements OnInit, OnDestroy {
  private routerSubscription: Subscription | undefined;
  private prevPageUrl = '';
  @Input() defaultUrl = '/';

  constructor(private router: Router) {}

  ngOnInit(): void {
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationStart) {
        this.prevPageUrl = event.url;
      }
    });
  }

  ngOnDestroy(): void {
    this.routerSubscription?.unsubscribe();
  }

  async prevPage(): Promise<void> {
    const url = this.prevPageUrl ? this.prevPageUrl : this.defaultUrl;
    const isRedirected = await this.router.navigate([url]);
    if (!isRedirected) this.router.navigate([this.defaultUrl]);
  }
}
