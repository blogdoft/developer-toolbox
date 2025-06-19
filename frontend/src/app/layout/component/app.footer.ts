import { Component } from '@angular/core';

@Component({
    standalone: true,
    selector: 'app-footer',
    template: `<div class="layout-footer">
        <img src="favicon.png" width="25px" alt="Blog do FT logo" />
        <a href="https://blogdoft.com.br" target="_blank" rel="noopener noreferrer" class="text-primary  hover:underline">Blog do FT</a>
    </div>`
})
export class AppFooter {}
