import { Component, OnDestroy, OnInit } from '@angular/core';

@Component({
  selector: 'app-carousel',
  templateUrl: './carousel.component.html',
  styleUrls: ['./carousel.component.scss']
})
export class CarouselComponent implements OnInit, OnDestroy {
  interval;
  slideIndex: number = 1;
  constructor() { }

  ngOnDestroy(): void {
    clearInterval(this.interval);
  }

  ngOnInit(): void {
    this.createInterval();
  }

  createInterval() {
    this.interval = setInterval(() => this.plusSlides(1), 5000);
  }

  plusSlides(n) {
    this.showSlides(this.slideIndex += n);
    clearInterval(this.interval);
    this.createInterval();
  }

  // Thumbnail image controls
  currentSlide(n) {
    this.showSlides(this.slideIndex = n);
    clearInterval(this.interval);
    this.createInterval();
  }

  showSlides(n) {
    if (this.slideIndex > 3)
      this.slideIndex = 1;
    if (this.slideIndex < 1)
      this.slideIndex = 3;
  }

}
