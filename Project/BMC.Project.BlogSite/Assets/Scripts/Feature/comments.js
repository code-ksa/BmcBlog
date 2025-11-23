// Comments Feature Scripts

export function initializeCommentsFeatures() {
    initializeCommentForm();
    initializeReplyFunctionality();
    initializeCharacterCounter();
}

// Initialize Comment Form Submission
function initializeCommentForm() {
    $('.comment-form form').on('submit', function(e) {
        e.preventDefault();

        const form = $(this);
        const submitButton = form.find('button[type="submit"]');
        const originalText = submitButton.html();

        // Validate form
        if (!validateCommentForm(form)) {
            return;
        }

        // Disable submit button
        submitButton.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Submitting...');

        $.ajax({
            url: form.attr('action'),
            method: 'POST',
            data: form.serialize(),
            success: function(response) {
                showCommentMessage('success', 'Your comment has been submitted and is awaiting approval.');
                form[0].reset();
                updateCharacterCount($('#commentContent'), 0);
            },
            error: function() {
                showCommentMessage('error', 'Failed to submit comment. Please try again.');
            },
            complete: function() {
                submitButton.prop('disabled', false).html(originalText);
            }
        });
    });
}

// Initialize Reply Functionality
function initializeReplyFunctionality() {
    $('.reply-btn').on('click', function() {
        const commentId = $(this).data('comment-id');
        const replyFormContainer = $(`#reply-form-${commentId}`);
        
        // Toggle reply form
        replyFormContainer.toggle();
        
        // Hide other reply forms
        $('.reply-form-container').not(replyFormContainer).hide();
        
        // Focus on comment textarea
        if (replyFormContainer.is(':visible')) {
            replyFormContainer.find('textarea').focus();
        }
    });
}

// Initialize Character Counter
function initializeCharacterCounter() {
    $('#commentContent').on('input', function() {
        const length = $(this).val().length;
        updateCharacterCount($(this), length);
    });
}

// Update Character Count
function updateCharacterCount(textarea, length) {
    const charCount = $('#charCount');
    const maxLength = 1000;
    
    charCount.text(length);
    
    if (length >= maxLength) {
        charCount.addClass('text-danger');
    } else {
        charCount.removeClass('text-danger');
    }
}

// Validate Comment Form
function validateCommentForm(form) {
    let isValid = true;
    
    // Validate name
    const name = form.find('input[name="AuthorName"]').val().trim();
    if (name === '') {
        showFieldError(form.find('input[name="AuthorName"]'), 'Name is required');
        isValid = false;
    }
    
    // Validate email
    const email = form.find('input[name="AuthorEmail"]').val().trim();
    if (email === '' || !isValidEmail(email)) {
        showFieldError(form.find('input[name="AuthorEmail"]'), 'Valid email is required');
        isValid = false;
    }
    
    // Validate comment
    const comment = form.find('textarea[name="Content"]').val().trim();
    if (comment === '') {
        showFieldError(form.find('textarea[name="Content"]'), 'Comment is required');
        isValid = false;
    }
    
    return isValid;
}

// Show Field Error
function showFieldError(field, message) {
    field.addClass('is-invalid');
    field.siblings('.invalid-feedback').text(message).show();
    
    field.on('input', function() {
        $(this).removeClass('is-invalid');
        $(this).siblings('.invalid-feedback').hide();
    });
}

// Show Comment Message
function showCommentMessage(type, message) {
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

    $('.comments-section').prepend(alertHtml);

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
    initializeCommentsFeatures
};