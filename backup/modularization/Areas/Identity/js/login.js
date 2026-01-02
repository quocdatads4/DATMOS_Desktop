document.addEventListener('DOMContentLoaded', function () {
    // --- Password Visibility Toggle ---
    function setupPasswordToggle(inputId, toggleId) {
        const passwordInput = document.getElementById(inputId);
        const toggleButton = document.getElementById(toggleId);
        const eyeIcon = toggleButton.querySelector('i');

        if (passwordInput && toggleButton) {
            toggleButton.addEventListener('click', function () {
                if (passwordInput.type === 'password') {
                    passwordInput.type = 'text';
                    eyeIcon.classList.remove('fa-eye');
                    eyeIcon.classList.add('fa-eye-slash');
                } else {
                    passwordInput.type = 'password';
                    eyeIcon.classList.remove('fa-eye-slash');
                    eyeIcon.classList.add('fa-eye');
                }
            });
        }
    }
    
    setupPasswordToggle('Password', 'togglePassword');

    // --- Form Submission Spinner ---
    const form = document.getElementById('loginForm');
    const loginButton = document.getElementById('loginButton');

    if (form && loginButton) {
        form.addEventListener('submit', function() {
            // Basic check for client-side validation before showing spinner
            if (form.checkValidity()) {
                loginButton.classList.add('loading');
                loginButton.disabled = true;
            }
        });
    }

    // --- Email Real-time Validation ---
    const emailInput = document.getElementById('Email');
    const emailFeedback = document.getElementById('emailValidationFeedback');

    if (emailInput && emailFeedback) {
        emailInput.addEventListener('input', function() {
            const email = emailInput.value;
            // Regular expression for basic email validation
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

            if (email.length === 0) {
                emailFeedback.textContent = '';
                emailFeedback.className = 'validation-feedback';
            } else if (emailRegex.test(email)) {
                emailFeedback.textContent = 'Email hợp lệ.';
                emailFeedback.className = 'validation-feedback match';
            } else {
                emailFeedback.textContent = 'Email không hợp lệ.';
                emailFeedback.className = 'validation-feedback no-match';
            }
        });
    }

    // --- Remember Me Checkbox Styling ---
    const rememberMeCheckbox = document.querySelector('input[name="RememberMe"]');
    const rememberMeLabel = document.querySelector('label[for="RememberMe"]');

    if (rememberMeCheckbox && rememberMeLabel) {
        rememberMeCheckbox.addEventListener('change', function() {
            if (this.checked) {
                rememberMeLabel.classList.add('text-primary');
            } else {
                rememberMeLabel.classList.remove('text-primary');
            }
        });
    }

    // --- Auto-focus on email field ---
    if (emailInput) {
        emailInput.focus();
    }

    // --- Enter key to submit form ---
    if (form) {
        form.addEventListener('keypress', function(e) {
            if (e.key === 'Enter' && !loginButton.disabled) {
                // Check if the focused element is not a button or textarea
                const activeElement = document.activeElement;
                if (activeElement.tagName !== 'BUTTON' && activeElement.tagName !== 'TEXTAREA') {
                    loginButton.click();
                }
            }
        });
    }
});
