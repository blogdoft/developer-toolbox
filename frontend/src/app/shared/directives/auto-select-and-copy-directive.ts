import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
    selector: '[autoSelectAndCopy]',
    standalone: true
})
export class AutoSelectAndCopyDirective {
    constructor(private el: ElementRef<HTMLTextAreaElement>) {}

    @HostListener('click')
    onClick() {
        const textarea = this.el.nativeElement;
        const success = navigator.clipboard.writeText(textarea.value);
    }
}
