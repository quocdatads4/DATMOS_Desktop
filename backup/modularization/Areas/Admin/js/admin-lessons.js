// Admin Lessons - JavaScript for admin lessons management

(function($) {
    'use strict';

    // Lesson management module
    const LessonManagement = {
        init: function() {
            this.bindEvents();
            this.initializeDataTable();
        },

        bindEvents: function() {
            // Handle create lesson button
            $(document).on('click', '.btn-create-lesson', this.handleCreateLesson);

            // Handle edit lesson button
            $(document).on('click', '.edit-record', this.handleEditLesson);

            // Handle preview lesson button
            $(document).on('click', '.preview-record', this.handlePreviewLesson);

            // Handle quick filters
            $(document).on('click', '.quick-filter', this.handleQuickFilter);

            // Handle course filter
            $(document).on('change', '.course-filter', this.handleCourseFilter);

            // Handle view toggle
            $(document).on('click', '[data-view]', this.handleViewToggle);

            // Handle bulk actions
            $(document).on('click', '.bulk-activate', this.handleBulkActivate);
            $(document).on('click', '.bulk-hide', this.handleBulkHide);
            $(document).on('click', '.bulk-delete', this.handleBulkDelete);
            $(document).on('click', '.bulk-export', this.handleBulkExport);

            // Handle select all checkbox
            $(document).on('change', '.select-all-lessons', this.handleSelectAll);

            // Handle individual checkbox changes
            $(document).on('change', '.lesson-checkbox', this.handleCheckboxChange);

            // Handle window resize
            $(window).on('resize', this.handleWindowResize);
        },

        initializeDataTable: function() {
            // Check if DataTable is available and table exists
            if (typeof DataTable !== 'undefined' && $('.datatables-lessons').length) {
                const tableElement = $('.datatables-lessons')[0];
                
                // Check if DataTable is already initialized on this element
                if (!$.fn.dataTable.isDataTable(tableElement)) {
                    console.log('Initializing DataTable for lessons...');
                    
                    // Use the same pattern as other files in the project
                    const dtLessons = new DataTable(tableElement, {
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
                    window.dtLessons = dtLessons;
                } else {
                    console.log('DataTable already initialized, skipping...');
                }
            } else {
                console.log('DataTable library not loaded or no lessons table found');
            }
        },

        handleCreateLesson: function(e) {
            e.preventDefault();
            
            // Show loading state
            const btn = $(this);
            const originalHtml = btn.html();
            btn.prop('disabled', true).html('<i class="ti ti-loader-2 spinner me-2"></i>Đang tải...');

            // Load create modal
            $.ajax({
                url: '/Admin/Lessons/Create',
                type: 'GET',
                success: function(response) {
                    $('#lessonModalContainer').html(response);
                    $('#lessonModal').modal('show');
                    btn.prop('disabled', false).html(originalHtml);
                },
                error: function() {
                    toastr.error('Không thể tải form tạo bài học');
                    btn.prop('disabled', false).html(originalHtml);
                }
            });
        },

        handleEditLesson: function(e) {
            e.preventDefault();
            
            const lessonId = $(this).closest('tr').find('.lesson-checkbox').val() || 
                            $(this).closest('.lesson-card').find('.delete-lesson-form').data('lesson-id');
            
            if (!lessonId) {
                toastr.error('Không tìm thấy ID bài học');
                return;
            }

            // Show loading state
            const btn = $(this);
            const originalHtml = btn.html();
            btn.prop('disabled', true).html('<i class="ti ti-loader-2 spinner"></i>');

            // Load edit modal
            $.ajax({
                url: '/Admin/Lessons/Edit/' + lessonId,
                type: 'GET',
                success: function(response) {
                    $('#lessonModalContainer').html(response);
                    $('#lessonModal').modal('show');
                    btn.prop('disabled', false).html(originalHtml);
                },
                error: function() {
                    toastr.error('Không thể tải form chỉnh sửa');
                    btn.prop('disabled', false).html(originalHtml);
                }
            });
        },

        handlePreviewLesson: function(e) {
            e.preventDefault();
            
            const lessonId = $(this).closest('tr').find('.lesson-checkbox').val() || 
                            $(this).closest('.lesson-card').find('.delete-lesson-form').data('lesson-id');
            
            if (!lessonId) {
                toastr.error('Không tìm thấy ID bài học');
                return;
            }

            toastr.info('Đang mở bài học xem trước...');
            // In a real application, this would open a preview modal or new tab
            // window.open(`/Admin/Lessons/Preview/${lessonId}`, '_blank');
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
                        showRow = status.includes('đang hiển thị');
                        break;
                    case 'hidden':
                        showRow = status.includes('đã ẩn');
                        break;
                    case 'pending':
                        showRow = status.includes('chờ') || status.includes('pending');
                        break;
                }
                
                $row.toggle(showRow);
            });

            // Also filter mobile grid view
            $('.lesson-card').each(function() {
                const $card = $(this);
                const status = $card.find('.status-badge').text().trim().toLowerCase();
                
                let showCard = false;
                
                switch(filter) {
                    case 'all':
                        showCard = true;
                        break;
                    case 'active':
                        showCard = status.includes('đang hiển thị');
                        break;
                    case 'hidden':
                        showCard = status.includes('đã ẩn');
                        break;
                    case 'pending':
                        showCard = status.includes('chờ') || status.includes('pending');
                        break;
                }
                
                $card.toggle(showCard);
            });
        },

        handleCourseFilter: function() {
            const courseId = $(this).val();
            
            if (!courseId) {
                // Show all lessons
                $('tbody tr').show();
                $('.lesson-card').show();
                return;
            }
            
            // Filter by course
            $('tbody tr').each(function() {
                const $row = $(this);
                const rowCourseCode = $row.find('td:nth-child(3) small.text-muted').text().trim();
                
                if (rowCourseCode === courseId) {
                    $row.show();
                } else {
                    $row.hide();
                }
            });

            // Also filter mobile grid view
            $('.lesson-card').each(function() {
                const $card = $(this);
                const cardCourseName = $card.find('.lesson-card-info small.text-muted').text().trim();
                
                // Simple check - in real app would need course ID
                if (cardCourseName.includes(courseId) || !courseId) {
                    $card.show();
                } else {
                    $card.hide();
                }
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
                $('.admin-lessons-table').show();
                $('.mobile-lessons-grid').hide();
            } else {
                // Show grid view, hide table view
                $('.admin-lessons-table').hide();
                $('.mobile-lessons-grid').show();
            }
        },

        handleBulkActivate: function() {
            const selectedIds = LessonManagement.getSelectedLessonIds();
            if (selectedIds.length === 0) {
                toastr.warning('Vui lòng chọn ít nhất một bài học');
                return;
            }
            
            Swal.fire({
                title: 'Hiển thị hàng loạt',
                html: `Bạn có chắc chắn muốn hiển thị <strong>${selectedIds.length} bài học</strong>?`,
                icon: 'question',
                showCancelButton: true,
                confirmButtonText: 'Hiển thị',
                cancelButtonText: 'Hủy'
            }).then((result) => {
                if (result.isConfirmed) {
                    LessonManagement.performBulkAction('activate', selectedIds);
                }
            });
        },

        handleBulkHide: function() {
            const selectedIds = LessonManagement.getSelectedLessonIds();
            if (selectedIds.length === 0) {
                toastr.warning('Vui lòng chọn ít nhất một bài học');
                return;
            }
            
            Swal.fire({
                title: 'Ẩn hàng loạt',
                html: `Bạn có chắc chắn muốn ẩn <strong>${selectedIds.length} bài học</strong>?`,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Ẩn',
                cancelButtonText: 'Hủy'
            }).then((result) => {
                if (result.isConfirmed) {
                    LessonManagement.performBulkAction('hide', selectedIds);
                }
            });
        },

        handleBulkDelete: function() {
            const selectedIds = LessonManagement.getSelectedLessonIds();
            if (selectedIds.length === 0) {
                toastr.warning('Vui lòng chọn ít nhất một bài học');
                return;
            }
            
            Swal.fire({
                title: 'Xóa hàng loạt',
                html: `Bạn có chắc chắn muốn xóa <strong>${selectedIds.length} bài học</strong>?<br><br>
                       <small class="text-muted">Hành động này không thể hoàn tác.</small>`,
                icon: 'error',
                showCancelButton: true,
                confirmButtonText: 'Xóa',
                cancelButtonText: 'Hủy',
                confirmButtonColor: '#d33'
            }).then((result) => {
                if (result.isConfirmed) {
                    LessonManagement.performBulkAction('delete', selectedIds);
                }
            });
        },

        handleBulkExport: function() {
            const selectedIds = LessonManagement.getSelectedLessonIds();
            if (selectedIds.length === 0) {
                toastr.warning('Vui lòng chọn ít nhất một bài học');
                return;
            }
            
            toastr.info(`Đang xuất ${selectedIds.length} bài học sang CSV...`);
            // Implement export logic here
            // LessonManagement.exportToCSV(selectedIds);
        },

        handleSelectAll: function() {
            const isChecked = $(this).prop('checked');
            $('.lesson-checkbox').prop('checked', isChecked);
            LessonManagement.updateBulkSelectionCount();
        },

        handleCheckboxChange: function() {
            LessonManagement.updateBulkSelectionCount();
            
            // Update select all checkbox state
            const totalCheckboxes = $('.lesson-checkbox').length;
            const checkedCheckboxes = $('.lesson-checkbox:checked').length;
            $('.select-all-lessons').prop('checked', totalCheckboxes === checkedCheckboxes);
        },

        handleWindowResize: function() {
            if (window.innerWidth > 768) {
                // On desktop, always show table view
                $('.admin-lessons-table').show();
                $('.mobile-lessons-grid').hide();
                $('[data-view="list"]').addClass('active');
                $('[data-view="grid"]').removeClass('active');
            }
        },

        getSelectedLessonIds: function() {
            const selectedIds = [];
            $('.lesson-checkbox:checked').each(function() {
                selectedIds.push($(this).val());
            });
            return selectedIds;
        },

        updateBulkSelectionCount: function() {
            const selectedCount = $('.lesson-checkbox:checked').length;
            $('.bulk-selection-count').text(`${selectedCount} bài học được chọn`);
            
            // Enable/disable bulk action buttons
            const bulkButtons = $('.bulk-activate, .bulk-hide, .bulk-delete, .bulk-export');
            if (selectedCount > 0) {
                bulkButtons.prop('disabled', false);
            } else {
                bulkButtons.prop('disabled', true);
            }
        },

        performBulkAction: function(action, ids) {
            // Show loading
            const loadingToast = toastr.info(`Đang xử lý ${ids.length} bài học...`, null, { timeOut: 0 });
            
            // Simulate API call
            setTimeout(() => {
                toastr.clear(loadingToast);
                
                switch(action) {
                    case 'activate':
                        toastr.success(`Đã hiển thị ${ids.length} bài học`);
                        break;
                    case 'hide':
                        toastr.success(`Đã ẩn ${ids.length} bài học`);
                        break;
                    case 'delete':
                        toastr.success(`Đã xóa ${ids.length} bài học`);
                        // Remove deleted rows from UI
                        ids.forEach(id => {
                            $(`.lesson-checkbox[value="${id}"]`).closest('tr, .lesson-card').remove();
                        });
                        break;
                }
                
                // Update counts
                LessonManagement.updateBulkSelectionCount();
                LessonManagement.updateStatistics();
            }, 1500);
        },

        updateStatistics: function() {
            // This would typically make an API call to get updated statistics
            // For now, we'll just update based on visible items
            const total = $('.lesson-checkbox').length;
            const active = $('.status-badge-active:visible').length;
            const hidden = $('.status-badge-inactive:visible').length;
            const pending = $('.status-badge-pending:visible').length;
            
            $('.stat-total .stat-value').text(total);
            $('.stat-active .stat-value').text(active);
            $('.stat-inactive .stat-value').text(hidden);
            $('.stat-pending .stat-value').text(pending);
        }
    };

    // Initialize when document is ready
    $(document).ready(function() {
        LessonManagement.init();
        
        // Initialize tooltips
        if ($.fn.tooltip) {
            $('[data-bs-toggle="tooltip"]').tooltip();
        }
        
        // Initialize bulk actions state
        LessonManagement.updateBulkSelectionCount();
    });

})(jQuery);
