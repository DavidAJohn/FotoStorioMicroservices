function initialiseCarousel() {
    const promoCarousel = document.querySelector('#promo-carousel');

    const carousel = new bootstrap.Carousel(promoCarousel, {
        interval: 5000 // interval between slides in ms
    });

    // this shifts focus to and from a separate page element 
    // to help improve consistency of carousel initialisation,
    // although the user clicking on the page should be sufficient 
    const navElement = document.querySelector('#navbar-top');
    navElement.focus();
    navElement.blur();
}
