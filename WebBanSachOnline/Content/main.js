

// Check if user is logged in
function isLoggedIn() {
    return localStorage.getItem('user') !== null;
}

// Get logged in user
function getLoggedInUser() {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
}

// Update navigation based on auth state
function updateNavigation() {
    const authButtons = document.getElementById('auth-buttons');
    const userDropdown = document.getElementById('user-dropdown');
    
    if (isLoggedIn()) {
        const user = getLoggedInUser();
        if (authButtons) authButtons.style.display = 'none';
        if (userDropdown) {
            userDropdown.style.display = 'block';
            const userNameElement = document.getElementById('user-name');
            if (userNameElement) userNameElement.textContent = `Xin chào, ${user.name}`;
        }
    } else {
        if (authButtons) authButtons.style.display = 'block';
        if (userDropdown) userDropdown.style.display = 'none';
    }
}

// Logout function
function logout() {
    localStorage.removeItem('user');
    localStorage.removeItem('cart');
    window.location.href = '/';
}

// Cart functions
function getCart() {
    const cartStr = localStorage.getItem('cart');
    return cartStr ? JSON.parse(cartStr) : [];
}

function saveCart(cart) {
    localStorage.setItem('cart', JSON.stringify(cart));
}

// Replace the showToast function with showCartModal
function showCartModal(book) {
    // Check if modal already exists, remove it if it does
    let existingModal = document.getElementById('cartSuccessModal');
    if (existingModal) {
        existingModal.remove();
    }
    
    // Create modal HTML
    const modalHTML = `
    <div class="modal fade" id="cartSuccessModal" tabindex="-1" aria-labelledby="cartSuccessModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content cart-success-modal">
                <div class="modal-header">
                    <h5 class="modal-title" id="cartSuccessModalLabel">Thêm thành công</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="cart-success-message">
                        Sách đã được thêm vào giỏ hàng
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-outline-primary cart-confirm-btn" data-bs-dismiss="modal">Xác nhận</button>
                </div>
            </div>
        </div>
    </div>`;
    
    // Append modal to body
    document.body.insertAdjacentHTML('beforeend', modalHTML);
    
    // Show the modal
    const modal = new bootstrap.Modal(document.getElementById('cartSuccessModal'));
    modal.show();
}

// Update addToCart function to use the modal instead of toast
function addToCart(book) {
    if (!isLoggedIn()) {
        alert('Vui lòng đăng nhập để thêm vào giỏ hàng!');
        window.location.href = '/pages/sign-in.html';
        return;
    }
    
    let cart = getCart();
    const existingItem = cart.find(item => item.id === book.id);
    
    if (existingItem) {
        // Check if adding another item would exceed stock
        if (existingItem.quantity >= book.stock) {
            alert('Không thể thêm sản phẩm vì đã đạt số lượng tối đa trong kho!');
            return;
        }
        existingItem.quantity++;
    } else {
        cart.push({...book, quantity: 1});
    }
    
    saveCart(cart);
    updateCartCount();
    showCartModal(book); // Replace showToast with showCartModal
}

function updateCartCount() {
    const cart = getCart();
    const count = cart.reduce((sum, item) => sum + item.quantity, 0);
    const cartCountElement = document.getElementById('cart-count');
    if (cartCountElement) {
        cartCountElement.textContent = count;
    }
}

// Format currency
function formatCurrency(amount) {
    return amount.toLocaleString('vi-VN', {
        style: 'currency',
        currency: 'VND'
    });
}

// Toast notification (keep for other notifications)
function showToast(message) {
    const toastContainer = document.getElementById('toast-container');
    if (!toastContainer) {
        const container = document.createElement('div');
        container.id = 'toast-container';
        container.style.position = 'fixed';
        container.style.bottom = '20px';
        container.style.right = '20px';
        container.style.zIndex = '1050';
        document.body.appendChild(container);
    }

    const toastId = `toast-${Date.now()}`;
    const toastElement = document.createElement('div');
    toastElement.id = toastId;
    toastElement.classList.add('toast', 'align-items-center', 'text-white', 'bg-success', 'border-0', 'show');
    toastElement.setAttribute('role', 'alert');
    toastElement.setAttribute('aria-live', 'assertive');
    toastElement.setAttribute('aria-atomic', 'true');

    toastElement.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">
                <i class="fas fa-check-circle me-2"></i> ${message}
            </div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
    `;

    document.getElementById('toast-container').appendChild(toastElement);

    const toast = new bootstrap.Toast(toastElement, {
        delay: 3000
    });
    toast.show();

    toastElement.addEventListener('hidden.bs.toast', () => {
        toastElement.remove();
    });
}

// Initialize on page load
document.addEventListener('DOMContentLoaded', () => {
    updateNavigation();
    updateCartCount();
});

// Order Row Click Handler - allows clicking anywhere in the row to open details
function setupOrderRowClickHandlers() {
    const orderRows = document.querySelectorAll('.order-row');
    orderRows.forEach(row => {
        row.addEventListener('click', function(e) {
            // Don't trigger if user clicked on a button or link
            if (e.target.tagName === 'BUTTON' || e.target.tagName === 'A' || 
                e.target.closest('button') || e.target.closest('a')) {
                return;
            }
            
            const orderId = this.getAttribute('data-order-id');
            showOrderDetails(orderId);
        });
    });
}

// Helper function to get status color
function getStatusColor(status) {
    switch (status) {
        case "delivered":
            return "#28a745";
        case "processing":
            return "#ffc107";
        case "cancelled":
            return "#dc3545";
        default:
            return "#6c757d";
    }
}

// Initialize tooltips
function initTooltips() {
    const tooltips = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    tooltips.forEach(tooltip => {
        new bootstrap.Tooltip(tooltip);
    });
} 