// Site-wide JavaScript for AXDD Admin

(function ($) {
    'use strict';

    // Initialize tooltips
    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });

    // Initialize popovers
    $(function () {
        $('[data-toggle="popover"]').popover();
    });

    // Auto-hide alerts after 5 seconds
    $(function () {
        setTimeout(function () {
            $('.alert:not(.alert-permanent)').fadeOut('slow');
        }, 5000);
    });

    // Confirm delete action
    window.confirmDelete = function (message) {
        return confirm(message || 'Are you sure you want to delete this item?');
    };

    // Loading overlay
    window.showLoading = function () {
        if ($('.loading-overlay').length === 0) {
            $('body').append(`
                <div class="loading-overlay">
                    <div class="loading-spinner"></div>
                </div>
            `);
        }
    };

    window.hideLoading = function () {
        $('.loading-overlay').remove();
    };

    // Toast notification
    window.showToast = function (message, type = 'success') {
        const iconMap = {
            success: 'fa-check-circle',
            error: 'fa-times-circle',
            warning: 'fa-exclamation-triangle',
            info: 'fa-info-circle'
        };

        const bgMap = {
            success: 'bg-success',
            error: 'bg-danger',
            warning: 'bg-warning',
            info: 'bg-info'
        };

        const toast = $(`
            <div class="toast ${bgMap[type]}" role="alert" aria-live="assertive" aria-atomic="true" data-autohide="true" data-delay="3000">
                <div class="toast-header ${bgMap[type]} text-white">
                    <i class="fas ${iconMap[type]} mr-2"></i>
                    <strong class="mr-auto">${type.charAt(0).toUpperCase() + type.slice(1)}</strong>
                    <button type="button" class="ml-2 mb-1 close text-white" data-dismiss="toast" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="toast-body">
                    ${message}
                </div>
            </div>
        `);

        $('body').append(toast);
        toast.toast('show');

        toast.on('hidden.bs.toast', function () {
            $(this).remove();
        });
    };

    // Format file size
    window.formatFileSize = function (bytes) {
        const sizes = ['B', 'KB', 'MB', 'GB', 'TB'];
        if (bytes === 0) return '0 B';
        const i = Math.floor(Math.log(bytes) / Math.log(1024));
        return Math.round(bytes / Math.pow(1024, i) * 100) / 100 + ' ' + sizes[i];
    };

    // Format date
    window.formatDate = function (date) {
        return new Date(date).toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'short',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    };

    // Relative time
    window.getRelativeTime = function (date) {
        const now = new Date();
        const diff = now - new Date(date);
        const seconds = Math.floor(diff / 1000);
        const minutes = Math.floor(seconds / 60);
        const hours = Math.floor(minutes / 60);
        const days = Math.floor(hours / 24);

        if (seconds < 60) return 'just now';
        if (minutes < 60) return minutes + ' minute' + (minutes > 1 ? 's' : '') + ' ago';
        if (hours < 24) return hours + ' hour' + (hours > 1 ? 's' : '') + ' ago';
        if (days < 7) return days + ' day' + (days > 1 ? 's' : '') + ' ago';
        return window.formatDate(date);
    };

    // AJAX error handler
    $(document).ajaxError(function (event, jqxhr, settings, thrownError) {
        console.error('AJAX Error:', thrownError);
        if (jqxhr.status === 401) {
            window.location.href = '/Account/Login';
        } else if (jqxhr.status === 403) {
            window.showToast('You do not have permission to perform this action', 'error');
        } else {
            window.showToast('An error occurred. Please try again.', 'error');
        }
    });

    // DataTables default configuration
    if ($.fn.DataTable) {
        $.extend(true, $.fn.dataTable.defaults, {
            language: {
                search: '_INPUT_',
                searchPlaceholder: 'Search...',
                lengthMenu: '_MENU_ records per page',
                info: 'Showing _START_ to _END_ of _TOTAL_ entries',
                paginate: {
                    first: '<i class="fas fa-angle-double-left"></i>',
                    previous: '<i class="fas fa-angle-left"></i>',
                    next: '<i class="fas fa-angle-right"></i>',
                    last: '<i class="fas fa-angle-double-right"></i>'
                }
            },
            responsive: true,
            autoWidth: false,
            pageLength: 10,
            lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]]
        });
    }

    // File input preview
    $(document).on('change', 'input[type="file"]', function () {
        const input = this;
        if (input.files && input.files[0]) {
            const fileName = input.files[0].name;
            const fileSize = window.formatFileSize(input.files[0].size);
            $(input).next('.custom-file-label').html(fileName);

            // Show preview if container exists
            const previewContainer = $(input).closest('form').find('.file-preview');
            if (previewContainer.length) {
                previewContainer.html(`
                    <div class="alert alert-info">
                        <i class="fas fa-file mr-2"></i>
                        <strong>${fileName}</strong> (${fileSize})
                    </div>
                `);
            }
        }
    });

    // Character counter for textareas
    $(document).on('input', 'textarea[maxlength]', function () {
        const textarea = $(this);
        const maxLength = textarea.attr('maxlength');
        const currentLength = textarea.val().length;
        const remaining = maxLength - currentLength;

        let counter = textarea.next('.character-counter');
        if (counter.length === 0) {
            counter = $('<small class="character-counter text-muted"></small>');
            textarea.after(counter);
        }

        counter.text(`${currentLength} / ${maxLength} characters`);

        if (remaining < 50) {
            counter.removeClass('text-muted').addClass('text-warning');
        } else {
            counter.removeClass('text-warning').addClass('text-muted');
        }
    });

    // Prevent double submission
    $('form').on('submit', function () {
        const submitButton = $(this).find('button[type="submit"], input[type="submit"]');
        submitButton.prop('disabled', true);
        submitButton.html('<i class="fas fa-spinner fa-spin mr-1"></i> Processing...');

        // Re-enable after 5 seconds (in case of validation errors)
        setTimeout(function () {
            submitButton.prop('disabled', false);
            submitButton.html(submitButton.data('original-text') || 'Submit');
        }, 5000);
    });

    // Store original button text
    $('button[type="submit"], input[type="submit"]').each(function () {
        $(this).data('original-text', $(this).html());
    });

    console.log('AXDD Admin scripts loaded successfully');
})(jQuery);
