// Blog Feature Scripts

export function initializeBlogFeatures() {
    initializeReadMore();
    initializeShareButtons();
    trackViewCount();
}

// Initialize "Read More" functionality
function initializeReadMore() {
    $('.read-more-btn').on('click', function(e) {
        e.preventDefault();
        const content = $(this).siblings('.post-content');
        content.toggleClass('expanded');
        
        if (content.hasClass('expanded')) {
            $(this).text('Read Less');
        } else {
            $(this).text('Read More');
        }
    });
}

// Initialize Social Share Buttons
function initializeShareButtons() {
    $('.share-facebook').on('click', function(e) {
        e.preventDefault();
        const url = encodeURIComponent(window.location.href);
        window.open(`https://www.facebook.com/sharer/sharer.php?u=${url}`, '_blank', 'width=600,height=400');
    });

    $('.share-twitter').on('click', function(e) {
        e.preventDefault();
        const url = encodeURIComponent(window.location.href);
        const title = encodeURIComponent(document.title);
        window.open(`https://twitter.com/intent/tweet?url=${url}&text=${title}`, '_blank', 'width=600,height=400');
    });

    $('.share-linkedin').on('click', function(e) {
        e.preventDefault();
        const url = encodeURIComponent(window.location.href);
        window.open(`https://www.linkedin.com/shareArticle?mini=true&url=${url}`, '_blank', 'width=600,height=400');
    });
}

// Track Blog Post View Count
function trackViewCount() {
    if ($('.blog-post-detail').length > 0) {
        // View count is tracked server-side, but we can add analytics here
        console.log('Blog post viewed');
    }
}

// Export functions
export default {
    initializeBlogFeatures
};