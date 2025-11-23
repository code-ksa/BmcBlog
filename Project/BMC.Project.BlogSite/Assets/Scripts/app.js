// Import Feature Scripts
import { initializeBlogFeatures } from './Feature/blog.js';
import { initializeSearchFeatures } from './Feature/search.js';
import { initializeNewsletterFeatures } from './Feature/newsletter.js';
import { initializeCommentsFeatures } from './Feature/comments.js';

(function ($) {
    'use strict';

    // Initialize on document ready
    $(document).ready(function () {
        initializeBootstrapComponents();
        initializeHeroSlider();
        initializeScrollToTop();
        initializeLanguageSwitcher();
        initializeLazyLoading();
        
        // Initialize Feature Scripts
        initializeBlogFeatures();
        initializeSearchFeatures();
        initializeNewsletterFeatures();
        initializeCommentsFeatures();
    });

    // Initialize Bootstrap Components
    function initializeBootstrapComponents() {
        // Initialize tooltips
        $('[data-toggle="tooltip"]').tooltip();

        // Initialize popovers
        $('[data-toggle="popover"]').popover();

        // Initialize dropdowns
        $('.dropdown-toggle').dropdown();

        // Close mobile menu when clicking outside
        $(document).on('click', function (e) {
            if (!$(e.target).closest('.navbar').length) {
                $('.navbar-collapse').collapse('hide');
            }
        });
    }

    // Initialize Hero Slider
    function initializeHeroSlider() {
        if ($('#heroCarousel').length) {
            $('#heroCarousel').carousel({
                interval: 5000,
                ride: 'carousel',
                pause: 'hover',
                wrap: true
            });

            // Add animation on slide
            $('#heroCarousel').on('slide.bs.carousel', function (e) {
                const $e = $(e.relatedTarget);
                const $animatedElements = $e.find('[data-animation]');
                
                $animatedElements.each(function () {
                    const $this = $(this);
                    const animationClass = $this.data('animation');
                    const animationDelay = $this.data('delay') || 0;
                    
                    $this.css({
                        'animation-delay': animationDelay + 'ms',
                        'animation-name': animationClass
                    });
                });
            });
        }
    }

    // Scroll to Top Button
    function initializeScrollToTop() {
        const scrollToTopBtn = $('<button>', {
            id: 'scrollToTop',
            class: 'btn btn-primary',
            html: '<i class="fas fa-arrow-up"></i>',
            css: {
                position: 'fixed',
                bottom: '20px',
                right: '20px',
                display: 'none',
                zIndex: '9999',
                width: '50px',
                height: '50px',
                borderRadius: '50%'
            }
        });

        $('body').append(scrollToTopBtn);

        $(window).scroll(function () {
            if ($(this).scrollTop() > 300) {
                $('#scrollToTop').fadeIn();
            } else {
                $('#scrollToTop').fadeOut();
            }
        });

        $('#scrollToTop').on('click', function () {
            $('html, body').animate({ scrollTop: 0 }, 800);
        });
    }

    // Language Switcher Functionality
    function initializeLanguageSwitcher() {
        $('.language-switcher a').on('click', function (e) {
            e.preventDefault();
            const language = $(this).data('language');
            const currentUrl = window.location.href;
            const separator = currentUrl.includes('?') ? '&' : '?';
            window.location.href = currentUrl + separator + 'sc_lang=' + language;
        });
    }

    // Lazy Loading for Images
    function initializeLazyLoading() {
        if ('IntersectionObserver' in window) {
            const imageObserver = new IntersectionObserver(function (entries, observer) {
                entries.forEach(function (entry) {
                    if (entry.isIntersecting) {
                        const image = entry.target;
                        image.src = image.dataset.src;
                        image.classList.remove('lazy');
                        imageObserver.unobserve(image);
                    }
                });
            });

            document.querySelectorAll('img.lazy').forEach(function (img) {
                imageObserver.observe(img);
            });
        } else {
            // Fallback for browsers that don't support IntersectionObserver
            $('.lazy').each(function () {
                $(this).attr('src', $(this).data('src')).removeClass('lazy');
            });
        }
    }

    // Global Event Handlers
    $(window).on('load', function () {
        // Hide page loader if exists
        $('.page-loader').fadeOut();
    });

    // Handle AJAX errors globally
    $(document).ajaxError(function (event, jqxhr, settings, thrownError) {
        console.error('AJAX Error:', thrownError);
    });

})(jQuery);     