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
    setupPasswordToggle('ConfirmPassword', 'toggleConfirmPassword');

    // --- Password Strength Meter ---
    const passwordInput = document.getElementById('Password');
    const strengthBar = document.getElementById('passwordStrengthBar');
    
    if (passwordInput && strengthBar) {
        passwordInput.addEventListener('input', function () {
            const password = passwordInput.value;
            let strength = 0;
            
            // Add points for different criteria
            if (password.length >= 8) strength++; // Length
            if (password.match(/[a-z]/)) strength++; // Lowercase
            if (password.match(/[A-Z]/)) strength++; // Uppercase
            if (password.match(/[0-9]/)) strength++; // Numbers
            if (password.match(/[^a-zA-Z0-9]/)) strength++; // Symbols

            let barColor = '';
            let barWidth = (strength * 20) + '%';
            
            switch (strength) {
                case 0:
                case 1:
                    barColor = 'bg-danger';
                    break;
                case 2:
                    barColor = 'bg-warning';
                    break;
                case 3:
                    barColor = 'bg-info';
                    break;
                case 4:
                case 5:
                    barColor = 'bg-success';
                    break;
                default:
                    barColor = 'bg-secondary';
            }

            strengthBar.style.width = barWidth;
            strengthBar.className = 'password-strength-bar ' + barColor;
        });
    }

    // --- Password Confirmation Matcher ---
    const confirmPasswordInput = document.getElementById('ConfirmPassword');
    const passwordMatchFeedback = document.getElementById('passwordMatch');

    function checkPasswordMatch() {
        if (passwordInput && confirmPasswordInput && passwordMatchFeedback) {
            const password = passwordInput.value;
            const confirmPassword = confirmPasswordInput.value;

            if (confirmPassword.length === 0) {
                passwordMatchFeedback.textContent = '';
                passwordMatchFeedback.className = 'validation-feedback';
                return;
            }

            if (password === confirmPassword) {
                passwordMatchFeedback.textContent = 'Mật khẩu trùng khớp!';
                passwordMatchFeedback.className = 'validation-feedback match';
            } else {
                passwordMatchFeedback.textContent = 'Mật khẩu không trùng khớp.';
                passwordMatchFeedback.className = 'validation-feedback no-match';
            }
        }
    }
    
    if (passwordInput) passwordInput.addEventListener('input', checkPasswordMatch);
    if (confirmPasswordInput) confirmPasswordInput.addEventListener('input', checkPasswordMatch);

    // --- Form Submission Spinner ---
    const form = document.getElementById('registerForm');
    const registerButton = document.getElementById('registerButton');

    if (form && registerButton) {
        form.addEventListener('submit', function() {
            // Basic check for client-side validation before showing spinner
            if (form.checkValidity()) {
                registerButton.classList.add('loading');
                registerButton.disabled = true;
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
});