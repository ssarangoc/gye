import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NotificationsComponent } from './shared/components/notifications.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NotificationsComponent],
  template: `
    <app-notifications />
    <router-outlet />
  `,
  styles: []
})
export class App {}

