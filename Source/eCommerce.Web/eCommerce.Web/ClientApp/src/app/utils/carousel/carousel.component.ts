import { Component, OnDestroy, OnInit } from '@angular/core';

@Component({
  selector: 'app-carousel',
  templateUrl: './carousel.component.html',
  styleUrls: ['./carousel.component.scss']
})
export class CarouselComponent implements OnInit, OnDestroy {
  interval;
  slideIndex: number = 0;
  images = [
    'https://i.pinimg.com/originals/d8/80/4f/d8804f454d8041073ec2d883b4e70afb.jpg',
    'https://i.pinimg.com/originals/54/d5/d3/54d5d31c30a900a2e9b18659c2b7d4f5.jpg',
    'https://img.wattpad.com/06cfc8e7101b95e749749441372cd76cd349607f/68747470733a2f2f73332e616d617a6f6e6177732e636f6d2f776174747061642d6d656469612d736572766963652f53746f7279496d6167652f3831774436586e53766c765152673d3d2d3133352e313463386539366531636366646162393834373739363138313331322e676966'
  ]
  constructor() { }

  ngOnDestroy(): void {
    clearInterval(this.interval);
  }

  ngOnInit(): void {
    this.createInterval();
  }

  createInterval() {
    this.interval = setInterval(() => this.plusSlides(0), 5000);
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
    if (this.slideIndex > this.images.length - 1)
      this.slideIndex = 0;
    if (this.slideIndex < 0)
      this.slideIndex = this.images.length - 1;
  }

}
