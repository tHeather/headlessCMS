import { Component, OnDestroy, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnDestroy, OnInit {
  isDesktop: boolean = true;
  private desktopQuery: MediaQueryList = window.matchMedia(
    '(min-width: 1000px)'
  );

  ngOnInit(): void {
    this.desktopQuery.addEventListener('change', this.desktopQueryHandler);
  }

  ngOnDestroy(): void {
    this.desktopQuery.removeEventListener('change', this.desktopQueryHandler);
  }

  private desktopQueryHandler = (event: MediaQueryListEvent): void => {
    this.isDesktop = event.matches;
  };
}
