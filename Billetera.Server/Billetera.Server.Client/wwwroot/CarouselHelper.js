window.initCarousel = function () {
    var carouselElement = document.getElementById('carouselPublicidad');
    if (carouselElement) {
        // Cambiar imagen cada 3 segundos
        var currentIndex = 0;
        var items = carouselElement.querySelectorAll('.carousel-item');
        var totalItems = items.length;

        setInterval(function () {
            items[currentIndex].classList.remove('active');
            currentIndex = (currentIndex + 1) % totalItems;
            items[currentIndex].classList.add('active');
        }, 3000); // 3000 = 3 segundos
    }
};