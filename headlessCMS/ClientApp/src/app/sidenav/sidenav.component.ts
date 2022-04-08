import { Component } from '@angular/core';
import { SideNavLikGroup } from './sideNavLinkGroup';

const LINKS: SideNavLikGroup[] = [
  {
    name: 'Collections',
    icon: 'table_view',
    links: [
      { link: 'collection/create', label: 'Create' },
      { link: 'collection/list', label: 'Collection list' },
    ],
  },
];

@Component({
  selector: 'app-sidenav',
  templateUrl: './sidenav.component.html',
  styleUrls: ['./sidenav.component.scss'],
})
export class SidenavComponent {
  linkGroups = LINKS;
}
