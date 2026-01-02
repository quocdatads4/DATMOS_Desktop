// Initialize tooltips
$(function () {
    $('[data-bs-toggle="tooltip"]').tooltip();
});

// Handle toggle user with SweetAlert2
$(document).ready(function() {
    $('.toggle-record').on('click', function(e) {
        e.preventDefault();
        
        const form = $(this).closest('.toggle-user-form');
        const userId = form.data('user-id');
        const userName = form.data('user-name') || 'người dùng này';
        const isActive = form.data('user-active') === 'True' || form.data('user-active') === true;
        
        const action = isActive ? 'tắt' : 'bật';
        const actionText = isActive ? 'Tắt người dùng' : 'Bật người dùng';
        const icon = isActive ? 'warning' : 'info';
        const confirmButtonColor = isActive ? '#ffab00' : '#71dd37';
        
        Swal.fire({
            title: `Xác nhận ${action} người dùng`,
            html: `Bạn có chắc chắn muốn ${action} <strong>${userName}</strong>?<br><br>
                   <small class="text-muted">${isActive ? 'Người dùng sẽ không thể đăng nhập vào hệ thống Admin.' : 'Người dùng sẽ có thể đăng nhập vào hệ thống Admin.'}</small>`,
            icon: icon,
            showCancelButton: true,
            confirmButtonText: actionText,
            cancelButtonText: 'Hủy',
            confirmButtonColor: confirmButtonColor,
            cancelButtonColor: '#3085d6',
            reverseButtons: true,
            customClass: {
                confirmButton: `btn ${isActive ? 'btn-warning' : 'btn-success'}`,
                cancelButton: 'btn btn-secondary'
            }
        }).then((result) => {
            if (result.isConfirmed) {
                // Show loading state
                const toggleBtn = form.find('.toggle-record');
                const originalHtml = toggleBtn.html();
                toggleBtn.prop('disabled', true).html('<i class="ti ti-loader-2 spinner"></i>');
                
                // Submit the form
                form.submit();
            }
        });
    });
    
    // Handle delete user with SweetAlert2
    $('.delete-record').on('click', function(e) {
        e.preventDefault();
        
        const form = $(this).closest('.delete-user-form');
        const userId = form.data('user-id');
        const userName = form.data('user-name') || 'người dùng này';
        const isDisabled = $(this).prop('disabled');
        
        if (isDisabled) {
            return;
        }
        
        Swal.fire({
            title: 'Xác nhận vô hiệu hóa',
            html: `Bạn có chắc chắn muốn vô hiệu hóa <strong>${userName}</strong>?<br><br>
                   <small class="text-muted">Người dùng sẽ không thể đăng nhập vào hệ thống Admin.</small>`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Vô hiệu hóa',
            cancelButtonText: 'Hủy',
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            reverseButtons: true,
            customClass: {
                confirmButton: 'btn btn-danger',
                cancelButton: 'btn btn-secondary'
            }
        }).then((result) => {
            if (result.isConfirmed) {
                // Show loading state
                const deleteBtn = form.find('.delete-record');
                const originalHtml = deleteBtn.html();
                deleteBtn.prop('disabled', true).html('<i class="ti ti-loader-2 spinner"></i>');
                
                // Submit the form
                form.submit();
            }
        });
    });
    
// Handle form submission to prevent double submission
$('.toggle-user-form').on('submit', function(e) {
    const submitBtn = $(this).find('.toggle-record');
    if (submitBtn.prop('disabled')) {
        e.preventDefault();
        return false;
    }
    submitBtn.prop('disabled', true).html('<i class="ti ti-loader-2 spinner"></i>');
    return true;
});

$('.delete-user-form').on('submit', function(e) {
    const submitBtn = $(this).find('.delete-record');
    if (submitBtn.prop('disabled')) {
        e.preventDefault();
        return false;
    }
    submitBtn.prop('disabled', true).html('<i class="ti ti-loader-2 spinner"></i>');
    return true;
});

// Handle quick filters
$('.quick-filter').on('click', function(e) {
    e.preventDefault();
    
    // Remove active class from all filters
    $('.quick-filter').removeClass('active');
    // Add active class to clicked filter
    $(this).addClass('active');
    
    const filter = $(this).data('filter');
    
    // Show/hide rows based on filter
    $('tbody tr').each(function() {
        const $row = $(this);
        const status = $row.find('.status-badge').text().trim().toLowerCase();
        
        let showRow = false;
        
        switch(filter) {
            case 'all':
                showRow = true;
                break;
            case 'active':
                showRow = status.includes('hoạt động');
                break;
            case 'inactive':
                showRow = status.includes('vô hiệu hóa');
                break;
            case 'pending':
                showRow = status.includes('chờ') || status.includes('pending');
                break;
        }
        
        if (showRow) {
            $row.show();
        } else {
            $row.hide();
        }
    });

    // Also filter mobile grid view
    $('.user-card').each(function() {
        const $card = $(this);
        const status = $card.find('.status-badge').text().trim().toLowerCase();
        
        let showCard = false;
        
        switch(filter) {
            case 'all':
                showCard = true;
                break;
            case 'active':
                showCard = status.includes('hoạt động');
                break;
            case 'inactive':
                showCard = status.includes('vô hiệu hóa');
                break;
            case 'pending':
                showCard = status.includes('chờ') || status.includes('pending');
                break;
        }
        
        if (showCard) {
            $card.show();
        } else {
            $card.hide();
        }
    });
});

// Handle view toggle (list/grid) - only on mobile
$('[data-view]').on('click', function(e) {
    e.preventDefault();
    
    // Only toggle on mobile screens
    if (window.innerWidth > 768) {
        toastr.info('Chế độ xem grid chỉ khả dụng trên thiết bị di động');
        return;
    }
    
    const view = $(this).data('view');
    
    // Update active button
    $('[data-view]').removeClass('active');
    $(this).addClass('active');
    
    if (view === 'list') {
        // Show table view, hide grid view
        $('.admin-users-table').show();
        $('.mobile-users-grid').hide();
    } else {
        // Show grid view, hide table view
        $('.admin-users-table').hide();
        $('.mobile-users-grid').show();
    }
});

// Handle window resize to update view
$(window).on('resize', function() {
    if (window.innerWidth > 768) {
        // On desktop, always show table view
        $('.admin-users-table').show();
        $('.mobile-users-grid').hide();
        $('[data-view="list"]').addClass('active');
        $('[data-view="grid"]').removeClass('active');
    }
});

// Handle select all checkbox
$('.select-all-users').on('change', function() {
    const isChecked = $(this).prop('checked');
    $('.user-checkbox').prop('checked', isChecked);
    updateBulkSelectionCount();
});

// Handle individual checkbox changes
$('.user-checkbox').on('change', function() {
    updateBulkSelectionCount();
    
    // Update select all checkbox state
    const totalCheckboxes = $('.user-checkbox').length;
    const checkedCheckboxes = $('.user-checkbox:checked').length;
    $('.select-all-users').prop('checked', totalCheckboxes === checkedCheckboxes);
});

// Update bulk selection count
function updateBulkSelectionCount() {
    const selectedCount = $('.user-checkbox:checked').length;
    $('.bulk-selection-count').text(`${selectedCount} người dùng được chọn`);
    
    // Enable/disable bulk action buttons
    if (selectedCount > 0) {
        $('.bulk-activate, .bulk-deactivate, .bulk-delete, .bulk-export').prop('disabled', false);
    } else {
        $('.bulk-activate, .bulk-deactivate, .bulk-delete, .bulk-export').prop('disabled', true);
    }
}

// Handle bulk actions
$('.bulk-activate').on('click', function() {
    const selectedIds = getSelectedUserIds();
    if (selectedIds.length === 0) return;
    
    Swal.fire({
        title: 'Kích hoạt hàng loạt',
        html: `Bạn có chắc chắn muốn kích hoạt <strong>${selectedIds.length} người dùng</strong>?`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Kích hoạt',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            // Implement bulk activation logic here
            toastr.success(`Đã kích hoạt ${selectedIds.length} người dùng`);
        }
    });
});

$('.bulk-deactivate').on('click', function() {
    const selectedIds = getSelectedUserIds();
    if (selectedIds.length === 0) return;
    
    Swal.fire({
        title: 'Vô hiệu hóa hàng loạt',
        html: `Bạn có chắc chắn muốn vô hiệu hóa <strong>${selectedIds.length} người dùng</strong>?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Vô hiệu hóa',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            // Implement bulk deactivation logic here
            toastr.success(`Đã vô hiệu hóa ${selectedIds.length} người dùng`);
        }
    });
});

$('.bulk-delete').on('click', function() {
    const selectedIds = getSelectedUserIds();
    if (selectedIds.length === 0) return;
    
    Swal.fire({
        title: 'Xóa hàng loạt',
        html: `Bạn có chắc chắn muốn xóa <strong>${selectedIds.length} người dùng</strong>?<br><br>
               <small class="text-muted">Hành động này không thể hoàn tác.</small>`,
        icon: 'error',
        showCancelButton: true,
        confirmButtonText: 'Xóa',
        cancelButtonText: 'Hủy',
        confirmButtonColor: '#d33'
    }).then((result) => {
        if (result.isConfirmed) {
            // Implement bulk delete logic here
            toastr.success(`Đã xóa ${selectedIds.length} người dùng`);
        }
    });
});

$('.bulk-export').on('click', function() {
    const selectedIds = getSelectedUserIds();
    if (selectedIds.length === 0) return;
    
    toastr.info(`Đang xuất ${selectedIds.length} người dùng sang CSV...`);
    // Implement export logic here
});

// Get selected user IDs
function getSelectedUserIds() {
    const selectedIds = [];
    $('.user-checkbox:checked').each(function() {
        selectedIds.push($(this).val());
    });
    return selectedIds;
}

// Initialize DataTable if available
if ($.fn.DataTable) {
    $('.datatables-users').DataTable({
        responsive: true,
        ordering: true,
        pageLength: 10,
        language: {
            url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/vi.json'
        },
        columnDefs: [
            { orderable: false, targets: [0, 7] } // Disable sorting for checkbox and action columns
        ],
        destroy: true
    });
}

// Initialize bulk actions state
updateBulkSelectionCount();
});