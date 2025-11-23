// Newsletter Feature Scripts

export function initializeNewsletterFeatures() {
    initializeNewsletterForm();
    initializeNewsletterValidation();
}

// Initialize Newsletter Form Submission
function initializeNewsletterForm() {
    $('#newsletterForm').on('submit', function(e) {
        e.preventDefault();

        const form = $(this);
        const submitButton = form.find('button[type="submit"]');
        const originalText = submitButton.html();

        // Disable submit button
        submitButton.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Subscribing...');

        $.ajax({
            url: form.attr('action'),
            method: 'POST',
            data: form.serialize(),
            success: function(response) {
                showNewsletterMessage('success', 'Thank you for subscribing! Please check your email.');
                form[0].reset();
            },
            error: function() {
                showNewsletterMessage('error', 'Subscription failed. Please try again.');
            },
            complete: function() {
                submitButton.prop('disabled', false).html(originalText);
            }
        });
    });
}

// Initialize Newsletter Form Validation
function initializeNewsletterValidation() {
    $('#newsletterEmail').on('blur', function() {
        const email = $(this).val();
        if (email && !isValidEmail(email)) {
            $(this).addClass('is-invalid');
            $(this).siblings('.invalid-feedback').text('Please enter a valid email address');
        } else {
            $(this).removeClass('is-invalid');
        }
    });
}

// Show Newsletter Message
function showNewsletterMessage(type, message) {
    const alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
    const iconClass = type === 'success' ? 'fa-check-circle' : 'fa-exclamation-circle';
    
    const alertHtml = `
        <div class="alert ${alertClass} alert-dismissible fade show" role="alert">
            <i class="fas ${iconClass}"></i> ${message}
            <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                <span aria-hidden="true">&times;</span>
            </button>
        </div>
    `;

    $('#newsletterMessage').html(alertHtml);

    setTimeout(function() {
        $('.alert').fadeOut(function() {
            $(this).remove();
        });
    }, 5000);
}

// Email Validation Helper
function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

// Export functions
export default {
    initializeNewsletterFeatures
};