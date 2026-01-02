// Blank Layout JS for Admin Area
console.log('Blank layout JS loaded');

// Loading overlay functionality
document.addEventListener('DOMContentLoaded', function() {
    const loadingOverlay = document.getElementById('loadingOverlay');
    
    // Function to show loading overlay
    window.showLoading = function() {
        if (loadingOverlay) {
            loadingOverlay.style.display = 'flex';
        }
    };
    
    // Function to hide loading overlay
    window.hideLoading = function() {
        if (loadingOverlay) {
            loadingOverlay.style.display = 'none';
        }
    };
    
    // Auto-hide loading overlay after page load (with delay for demonstration)
    setTimeout(function() {
        window.hideLoading();
    }, 500);
    
    // Example: Show loading on form submissions
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function() {
            window.showLoading();
        });
    });
    
    // Example: Show loading on link clicks (if data-loading attribute is present)
    const loadingLinks = document.querySelectorAll('a[data-loading="true"]');
    loadingLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            window.showLoading();
        });
    });
    
    console.log('Blank layout initialized');
});
