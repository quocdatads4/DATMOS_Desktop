// Admin Courses - JavaScript for admin courses management

(function($) {
    'use strict';

    // Course management module
    const CourseManagement = {
        init: function() {
            this.bindEvents();
            this.initializeDataTable();
        },

        bindEvents: function() {
            // Handle create course button
            $(document).on('click', '.btn-create-course', this.handleCreateCourse);

            // Handle edit course button
            $(document).on('click', '.edit-record', this.handleEditCourse);

            // Handle quick filters
            $(document).on('click', '.quick-filter', this.handleQuickFilter);

            // Handle view toggle
            $(document).on('click', '[data-view]', this.handleViewToggle);

            // Handle bulk actions
            $(document).on('click', '.bulk-activate', this.handleBulkActivate);
            $(document).on('click', '.bulk-deactivate', this.handleBulkDeactivate);
            $(document).on('click', '.bulk-delete', this.handleBulkDelete);
            $(document).on('click', '.bulk-export', this.handleBulkExport);

            // Handle select all checkbox
            $(document).on('change', '.select-all-courses', this.handleSelectAll);

            // Handle individual checkbox changes
            $(document).on('change', '.course-checkbox', this.handleCheckboxChange);

            // Handle window resize
            $(window).on('resize', this.handleWindowResize);
        },

        initializeDataTable: function() {
            // Check if DataTable is available and table exists
            if (typeof DataTable !== 'undefined' && $('.datatables-courses').length) {
                const tableElement = $('.datatables-courses')[0];
                
                // Check if DataTable is already initialized on this element
                if (!$.fn.dataTable.isDataTable(tableElement)) {
                    console.log('Initializing DataTable for courses...');
                    
                    // Use the same pattern as other files in the project
                    const dtCourses = new DataTable(tableElement, {
                        responsive: true,
                        ordering: true,
                        pageLength: 10,
                        language: {
                            url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/vi.json'
                        },
                        columnDefs: [
                            { orderable: false, targets: [0, 7] }
                        ],
                        initComplete: function() {
                            // Add custom class to table
                            this.table().container().classList.add('custom-datatable');
                            console.log('DataTable initialization complete');
                        }
                    });
                    
                    // Store reference to the DataTable instance
                    window.dtCourses = dtCourses;
                } else {
                    console.log('DataTable already initialized, skipping...');
                }
            } else {
                console.log('DataTable library not loaded or no courses table found');
            }
        },

        handleCreateCourse: function(e) {
            e.preventDefault();
            
            // Show loading state
            const btn = $(this);
            const originalHtml = btn.html();
            btn.prop('disabled', true).html('<i class="ti ti-loader-2 spinner me-2"></i>Đang tải...');

            // Load create modal
            $.ajax({
                url: '/Admin/Courses/Create',
                type: 'GET',
                success: function(response) {
                    $('#courseModalContainer').html(response);
                    $('#courseModal').modal('show');
                    btn.prop('disabled', false).html(originalHtml);
                },
                error: function() {
                    toastr.error('Không thể tải form tạo khóa học');
                    btn.prop('disabled', false).html(originalHtml);
                }
            });
        },

        handleEditCourse: function(e) {
            e.preventDefault();
            
            const courseId = $(this).closest('tr').find('.course-checkbox').val() || 
                            $(this).closest('.course-card').find('.delete-course-form').data('course-id');
            
            if (!courseId) {
                toastr.error('Không tìm thấy ID khóa học');
                return;
            }

            // Show loading state
            const btn = $(this);
            const originalHtml = btn.html();
            btn.prop('disabled', true).html('<i class="ti ti-loader-2 spinner"></i>');

            // Load edit modal
            $.ajax({
                url: '/Admin/Courses/Edit/' + courseId,
                type: 'GET',
                success: function(response) {
                    $('#courseModalContainer').html(response);
                    $('#courseModal').modal('show');
                    btn.prop('disabled', false).html(originalHtml);
                },
                error: function() {
                    toastr.error('Không thể tải form chỉnh sửa');
                    btn.prop('disabled', false).html(originalHtml);
                }
            });
        },

        handleQuickFilter: function(e) {
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
                        showRow = status.includes('đang bán');
                        break;
                    case 'inactive':
                        showRow = status.includes('ngừng bán');
                        break;
                    case 'pending':
                        showRow = status.includes('chờ') || status.includes('pending');
                        break;
                }
                
                $row.toggle(showRow);
            });

            // Also filter mobile grid view
            $('.course-card').each(function() {
                const $card = $(this);
                const status = $card.find('.status-badge').text().trim().toLowerCase();
                
                let showCard = false;
                
                switch(filter) {
                    case 'all':
                        showCard = true;
                        break;
                    case 'active':
                        showCard = status.includes('đang bán');
                        break;
                    case 'inactive':
                        showCard = status.includes('ngừng bán');
                        break;
                    case 'pending':
                        showCard = status.includes('chờ') || status.includes('pending');
                        break;
                }
                
                $card.toggle(showCard);
            });
        },

        handleViewToggle: function(e) {
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
                $('.admin-courses-table').show();
                $('.mobile-courses-grid').hide();
            } else {
                // Show grid view, hide table view
                $('.admin-courses-table').hide();
                $('.mobile-courses-grid').show();
            }
        },

        handleBulkActivate: function() {
            const selectedIds = CourseManagement.getSelectedCourseIds();
            if (selectedIds.length === 0) {
                toastr.warning('Vui lòng chọn ít nhất một khóa học');
                return;
            }
            
            Swal.fire({
                title: 'Kích hoạt hàng loạt',
                html: `Bạn có chắc chắn muốn kích hoạt <strong>${selectedIds.length} khóa học</strong>?`,
                icon: 'question',
                showCancelButton: true,
                confirmButtonText: 'Kích hoạt',
                cancelButtonText: 'Hủy'
            }).then((result) => {
                if (result.isConfirmed) {
                    CourseManagement.performBulkAction('activate', selectedIds);
                }
            });
        },

        handleBulkDeactivate: function() {
            const selectedIds = CourseManagement.getSelectedCourseIds();
            if (selectedIds.length === 0) {
                toastr.warning('Vui lòng chọn ít nhất một khóa học');
                return;
            }
            
            Swal.fire({
                title: 'Ngừng bán hàng loạt',
                html: `Bạn có chắc chắn muốn ngừng bán <strong>${selectedIds.length} khóa học</strong>?`,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Ngừng bán',
                cancelButtonText: 'Hủy'
            }).then((result) => {
                if (result.isConfirmed) {
                    CourseManagement.performBulkAction('deactivate', selectedIds);
                }
            });
        },

        handleBulkDelete: function() {
            const selectedIds = CourseManagement.getSelectedCourseIds();
            if (selectedIds.length === 0) {
                toastr.warning('Vui lòng chọn ít nhất một khóa học');
                return;
            }
            
            Swal.fire({
                title: 'Xóa hàng loạt',
                html: `Bạn có chắc chắn muốn xóa <strong>${selectedIds.length} khóa học</strong>?<br><br>
                       <small class="text-muted">Hành động này không thể hoàn tác.</small>`,
                icon: 'error',
                showCancelButton: true,
                confirmButtonText: 'Xóa',
                cancelButtonText: 'Hủy',
                confirmButtonColor: '#d33'
            }).then((result) => {
                if (result.isConfirmed) {
                    CourseManagement.performBulkAction('delete', selectedIds);
                }
            });
        },

        handleBulkExport: function() {
            const selectedIds = CourseManagement.getSelectedCourseIds();
            if (selectedIds.length === 0) {
                toastr.warning('Vui lòng chọn ít nhất một khóa học');
                return;
            }
            
            toastr.info(`Đang xuất ${selectedIds.length} khóa học sang CSV...`);
            // Implement export logic here
            // CourseManagement.exportToCSV(selectedIds);
        },

        handleSelectAll: function() {
            const isChecked = $(this).prop('checked');
            $('.course-checkbox').prop('checked', isChecked);
            CourseManagement.updateBulkSelectionCount();
        },

        handleCheckboxChange: function() {
            CourseManagement.updateBulkSelectionCount();
            
            // Update select all checkbox state
            const totalCheckboxes = $('.course-checkbox').length;
            const checkedCheckboxes = $('.course-checkbox:checked').length;
            $('.select-all-courses').prop('checked', totalCheckboxes === checkedCheckboxes);
        },

        handleWindowResize: function() {
            if (window.innerWidth > 768) {
                // On desktop, always show table view
                $('.admin-courses-table').show();
                $('.mobile-courses-grid').hide();
                $('[data-view="list"]').addClass('active');
                $('[data-view="grid"]').removeClass('active');
            }
        },

        getSelectedCourseIds: function() {
            const selectedIds = [];
            $('.course-checkbox:checked').each(function() {
                selectedIds.push($(this).val());
            });
            return selectedIds;
        },

        updateBulkSelectionCount: function() {
            const selectedCount = $('.course-checkbox:checked').length;
            $('.bulk-selection-count').text(`${selectedCount} khóa học được chọn`);
            
            // Enable/disable bulk action buttons
            const bulkButtons = $('.bulk-activate, .bulk-deactivate, .bulk-delete, .bulk-export');
            if (selectedCount > 0) {
                bulkButtons.prop('disabled', false);
            } else {
                bulkButtons.prop('disabled', true);
            }
        },

        performBulkAction: function(action, ids) {
            // Show loading
            const loadingToast = toastr.info(`Đang xử lý ${ids.length} khóa học...`, null, { timeOut: 0 });
            
            // Simulate API call
            setTimeout(() => {
                toastr.clear(loadingToast);
                
                switch(action) {
                    case 'activate':
                        toastr.success(`Đã kích hoạt ${ids.length} khóa học`);
                        break;
                    case 'deactivate':
                        toastr.success(`Đã ngừng bán ${ids.length} khóa học`);
                        break;
                    case 'delete':
                        toastr.success(`Đã xóa ${ids.length} khóa học`);
                        // Remove deleted rows from UI
                        ids.forEach(id => {
                            $(`.course-checkbox[value="${id}"]`).closest('tr, .course-card').remove();
                        });
                        break;
                }
                
                // Update counts
                CourseManagement.updateBulkSelectionCount();
                CourseManagement.updateStatistics();
            }, 1500);
        },

        updateStatistics: function() {
            // This would typically make an API call to get updated statistics
            // For now, we'll just update based on visible items
            const total = $('.course-checkbox').length;
            const active = $('.status-badge-active:visible').length;
            const inactive = $('.status-badge-inactive:visible').length;
            const pending = $('.status-badge-pending:visible').length;
            
            $('.stat-total .stat-value').text(total);
            $('.stat-active .stat-value').text(active);
            $('.stat-inactive .stat-value').text(inactive);
            $('.stat-pending .stat-value').text(pending);
        }
    };

    // Initialize when document is ready
    $(document).ready(function() {
        CourseManagement.init();
        
        // Initialize tooltips
        if ($.fn.tooltip) {
            $('[data-bs-toggle="tooltip"]').tooltip();
        }
        
        // Initialize bulk actions state
        CourseManagement.updateBulkSelectionCount();
    });

})(jQuery);
