// Word 2019 Results JavaScript
// Common JavaScript functionality for Word 2019 results pages

document.addEventListener('DOMContentLoaded', function() {
    // Initialize tooltips
    initTooltips();
    
    // Initialize collapsible task rows
    initTaskRows();
    
    // Initialize charts if ApexCharts is available
    initScoreCharts();
    
    // Add hover effects to table rows
    initTableHoverEffects();
    
    // Initialize print functionality
    initPrintFunctionality();
    
    // Initialize expand/collapse all functionality
    initExpandCollapseAll();
    
    // Initialize smooth scrolling
    initSmoothScrolling();
    
    // Initialize progress bar animations
    initProgressBarAnimations();
});

/**
 * Initialize Bootstrap tooltips
 */
function initTooltips() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

/**
 * Initialize collapsible task rows
 */
function initTaskRows() {
    const taskRows = document.querySelectorAll('.task-summary-row');
    taskRows.forEach(row => {
        row.addEventListener('click', function(e) {
            // Don't trigger if clicking on a link or button inside the row
            if (e.target.tagName === 'A' || e.target.tagName === 'BUTTON' || e.target.closest('a') || e.target.closest('button')) {
                return;
            }
            
            const targetId = this.getAttribute('data-bs-target') || 
                            this.getAttribute('href') || 
                            this.querySelector('[data-bs-target]')?.getAttribute('data-bs-target');
            
            if (targetId) {
                const targetElement = document.querySelector(targetId);
                if (targetElement) {
                    const bsCollapse = new bootstrap.Collapse(targetElement, {
                        toggle: true
                    });
                    
                    // Rotate chevron icon
                    const chevron = this.querySelector('.ti-chevron-down, .ti-chevron-up');
                    if (chevron) {
                        if (targetElement.classList.contains('show')) {
                            chevron.classList.remove('ti-chevron-up');
                            chevron.classList.add('ti-chevron-down');
                        } else {
                            chevron.classList.remove('ti-chevron-down');
                            chevron.classList.add('ti-chevron-up');
                        }
                    }
                }
            }
        });
    });
}

/**
 * Initialize score charts using ApexCharts - Enhanced version
 */
function initScoreCharts() {
    const chartElements = document.querySelectorAll('.score-chart');
    
    chartElements.forEach(chartEl => {
        if (typeof ApexCharts !== 'undefined') {
            const passed = chartEl.dataset.passed === 'true';
            const score = parseInt(chartEl.dataset.score) || 0;
            const maxScore = parseInt(chartEl.dataset.maxScore) || 100;
            const percentage = parseInt(chartEl.dataset.percentage) || 0;
            
            const colors = {
                primary: '#7367f0',
                success: '#28c76f',
                danger: '#ea5455',
                warning: '#ff9f43',
                info: '#00cfe8',
                secondary: '#82868b'
            };
            
            const chartColor = passed ? colors.success : colors.danger;
            const gradientColor = passed 
                ? ['#28c76f', '#20b85c', '#18a94a']
                : ['#ea5455', '#e42728', '#d11516'];
            
            const chartConfig = {
                chart: {
                    height: '100%',
                    type: 'radialBar',
                    toolbar: {
                        show: false
                    },
                    animations: {
                        enabled: true,
                        speed: 1500,
                        animateGradually: {
                            enabled: true,
                            delay: 150
                        },
                        dynamicAnimation: {
                            enabled: true,
                            speed: 350
                        }
                    }
                },
                series: [percentage],
                plotOptions: {
                    radialBar: {
                        hollow: {
                            size: '70%',
                            margin: 0,
                            background: 'transparent'
                        },
                        track: {
                            background: 'rgba(255, 255, 255, 0.1)',
                            strokeWidth: '100%',
                            margin: 0
                        },
                        dataLabels: {
                            name: {
                                show: false
                            },
                            value: {
                                show: false // We have our own badge
                            }
                        }
                    }
                },
                fill: {
                    type: 'gradient',
                    gradient: {
                        shade: 'dark',
                        type: 'horizontal',
                        shadeIntensity: 0.5,
                        gradientToColors: gradientColor,
                        inverseColors: false,
                        opacityFrom: 1,
                        opacityTo: 1,
                        stops: [0, 50, 100]
                    }
                },
                colors: [chartColor],
                stroke: {
                    lineCap: 'round',
                    width: 8
                },
                labels: ['Điểm số'],
                tooltip: {
                    enabled: true,
                    theme: 'light',
                    fillSeriesColor: false,
                    y: {
                        formatter: function(val) {
                            return val + '% (' + score + '/' + maxScore + ' điểm)';
                        },
                        title: {
                            formatter: function() {
                                return 'Tỷ lệ hoàn thành';
                            }
                        }
                    }
                },
                states: {
                    hover: {
                        filter: {
                            type: 'lighten',
                            value: 0.15
                        }
                    },
                    active: {
                        filter: {
                            type: 'darken',
                            value: 0.35
                        }
                    }
                }
            };
            
            const chart = new ApexCharts(chartEl, chartConfig);
            chart.render();
            
            // Add animation after chart renders
            setTimeout(() => {
                chart.updateSeries([percentage]);
            }, 500);
            
            // Store chart instance for potential updates
            chartEl.chartInstance = chart;
            
            // Add hover effect
            chartEl.addEventListener('mouseenter', function() {
                if (chartEl.chartInstance) {
                    chartEl.chartInstance.updateOptions({
                        chart: {
                            animations: {
                                dynamicAnimation: {
                                    speed: 100
                                }
                            }
                        }
                    });
                }
            });
            
            chartEl.addEventListener('mouseleave', function() {
                if (chartEl.chartInstance) {
                    chartEl.chartInstance.updateOptions({
                        chart: {
                            animations: {
                                dynamicAnimation: {
                                    speed: 350
                                }
                            }
                        }
                    });
                }
            });
        } else {
            // Fallback if ApexCharts is not available
            console.warn('ApexCharts library not loaded. Using fallback display.');
            chartEl.innerHTML = `
                <div style="width: 100%; height: 100%; display: flex; align-items: center; justify-content: center; background: conic-gradient(${passed ? '#28c76f' : '#ea5455'} ${percentage}%, #e9ecef ${percentage}% 100%); border-radius: 50%;">
                    <div style="background: white; width: 70%; height: 70%; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 2rem; font-weight: bold; color: ${passed ? '#28c76f' : '#ea5455'};">
                        ${percentage}%
                    </div>
                </div>
            `;
        }
    });
}

/**
 * Initialize hover effects for table rows
 */
function initTableHoverEffects() {
    const tableRows = document.querySelectorAll('tbody tr:not(.task-summary-row)');
    tableRows.forEach(row => {
        row.addEventListener('mouseenter', function() {
            this.style.backgroundColor = '#f8f9fa';
            this.style.transition = 'background-color 0.2s ease';
        });
        
        row.addEventListener('mouseleave', function() {
            this.style.backgroundColor = '';
        });
    });
}

/**
 * Initialize print functionality
 */
function initPrintFunctionality() {
    const printButtons = document.querySelectorAll('.btn-print-results, #printResults');
    
    printButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            
            // Store original styles
            const originalStyles = {};
            const elementsToHide = document.querySelectorAll('.no-print, .action-buttons, .navbar, .footer');
            
            elementsToHide.forEach(el => {
                originalStyles[el] = el.style.display;
                el.style.display = 'none';
            });
            
            // Add print-specific styles
            const printStyle = document.createElement('style');
            printStyle.innerHTML = `
                @media print {
                    body * {
                        visibility: hidden;
                    }
                    .results-card, .results-card * {
                        visibility: visible;
                    }
                    .results-card {
                        position: absolute;
                        left: 0;
                        top: 0;
                        width: 100%;
                        box-shadow: none !important;
                        border: 1px solid #ddd !important;
                        background: white !important;
                    }
                    .results-header {
                        background: #f8f9fa !important;
                        color: #000 !important;
                        -webkit-print-color-adjust: exact;
                    }
                    .btn, .badge {
                        border: 1px solid #000 !important;
                        color: #000 !important;
                        background-color: transparent !important;
                    }
                    .task-header {
                        cursor: default !important;
                    }
                    .student-avatar {
                        background: #ddd !important;
                        color: #000 !important;
                        border-color: #999 !important;
                    }
                    .stats-card {
                        box-shadow: none !important;
                        border: 1px solid #ddd !important;
                    }
                }
            `;
            document.head.appendChild(printStyle);
            
            // Trigger print
            window.print();
            
            // Restore original styles
            elementsToHide.forEach(el => {
                el.style.display = originalStyles[el];
            });
            
            // Remove print style
            document.head.removeChild(printStyle);
        });
    });
}

/**
 * Export results data as JSON
 */
function exportResultsAsJson(data, filename = 'word2019-results.json') {
    const dataStr = JSON.stringify(data, null, 2);
    const dataBlob = new Blob([dataStr], { type: 'application/json' });
    
    const downloadUrl = URL.createObjectURL(dataBlob);
    const downloadLink = document.createElement('a');
    downloadLink.href = downloadUrl;
    downloadLink.download = filename;
    
    document.body.appendChild(downloadLink);
    downloadLink.click();
    document.body.removeChild(downloadLink);
    
    URL.revokeObjectURL(downloadUrl);
}

/**
 * Filter table rows based on search input
 */
function filterResultsTable(searchInputId, tableId) {
    const searchInput = document.getElementById(searchInputId);
    const table = document.getElementById(tableId);
    
    if (!searchInput || !table) return;
    
    searchInput.addEventListener('input', function() {
        const filter = this.value.toLowerCase();
        const rows = table.querySelectorAll('tbody tr');
        
        rows.forEach(row => {
            const text = row.textContent.toLowerCase();
            row.style.display = text.includes(filter) ? '' : 'none';
        });
    });
}

/**
 * Sort table by column
 */
function sortTable(tableId, columnIndex, isNumeric = false) {
    const table = document.getElementById(tableId);
    if (!table) return;
    
    const tbody = table.querySelector('tbody');
    const rows = Array.from(tbody.querySelectorAll('tr'));
    
    const isAscending = !tbody.getAttribute('data-sorted-asc');
    
    rows.sort((a, b) => {
        const aCell = a.cells[columnIndex]?.textContent.trim();
        const bCell = b.cells[columnIndex]?.textContent.trim();
        
        if (isNumeric) {
            const aNum = parseFloat(aCell) || 0;
            const bNum = parseFloat(bCell) || 0;
            return isAscending ? aNum - bNum : bNum - aNum;
        } else {
            return isAscending 
                ? aCell.localeCompare(bCell)
                : bCell.localeCompare(aCell);
        }
    });
    
    // Remove existing rows
    rows.forEach(row => tbody.removeChild(row));
    
    // Add sorted rows
    rows.forEach(row => tbody.appendChild(row));
    
    // Update sort indicator
    tbody.setAttribute('data-sorted-asc', isAscending);
    
    // Update header sort indicators
    const headers = table.querySelectorAll('th');
    headers.forEach((header, index) => {
        header.classList.remove('sorted-asc', 'sorted-desc');
        if (index === columnIndex) {
            header.classList.add(isAscending ? 'sorted-asc' : 'sorted-desc');
        }
    });
}

/**
 * Initialize expand/collapse all functionality
 */
function initExpandCollapseAll() {
    const expandAllBtn = document.getElementById('expandAll');
    const collapseAllBtn = document.getElementById('collapseAll');
    
    if (expandAllBtn) {
        expandAllBtn.addEventListener('click', function() {
            const collapseElements = document.querySelectorAll('.collapse:not(.show)');
            collapseElements.forEach(el => {
                const bsCollapse = new bootstrap.Collapse(el, {
                    show: true
                });
                
                // Update chevron icon
                const trigger = document.querySelector(`[data-bs-target="#${el.id}"]`);
                if (trigger) {
                    const chevron = trigger.querySelector('.ti-chevron-down, .ti-chevron-up');
                    if (chevron) {
                        chevron.classList.remove('ti-chevron-down');
                        chevron.classList.add('ti-chevron-up');
                    }
                }
            });
        });
    }
    
    if (collapseAllBtn) {
        collapseAllBtn.addEventListener('click', function() {
            const collapseElements = document.querySelectorAll('.collapse.show');
            collapseElements.forEach(el => {
                const bsCollapse = new bootstrap.Collapse(el, {
                    hide: true
                });
                
                // Update chevron icon
                const trigger = document.querySelector(`[data-bs-target="#${el.id}"]`);
                if (trigger) {
                    const chevron = trigger.querySelector('.ti-chevron-down, .ti-chevron-up');
                    if (chevron) {
                        chevron.classList.remove('ti-chevron-up');
                        chevron.classList.add('ti-chevron-down');
                    }
                }
            });
        });
    }
}

/**
 * Initialize smooth scrolling for task details
 */
function initSmoothScrolling() {
    document.querySelectorAll('.task-header').forEach(header => {
        header.addEventListener('click', function() {
            const content = this.nextElementSibling;
            if (content.classList.contains('show')) {
                setTimeout(() => {
                    content.scrollIntoView({ 
                        behavior: 'smooth', 
                        block: 'nearest',
                        inline: 'nearest'
                    });
                }, 350); // Wait for collapse animation
            }
        });
    });
}

/**
 * Initialize progress bar animations
 */
function initProgressBarAnimations() {
    const observerOptions = {
        threshold: 0.5,
        rootMargin: '0px 0px -50px 0px'
    };
    
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const progressBars = entry.target.querySelectorAll('.task-progress-bar');
                progressBars.forEach(bar => {
                    const width = bar.style.width;
                    bar.style.width = '0';
                    setTimeout(() => {
                        bar.style.width = width;
                    }, 300);
                });
                observer.unobserve(entry.target);
            }
        });
    }, observerOptions);
    
    // Observe task cards
    document.querySelectorAll('.task-card').forEach(card => {
        observer.observe(card);
    });
    
    // Observe stats cards
    document.querySelectorAll('.stats-card').forEach(card => {
        observer.observe(card);
    });
}

/**
 * Add copy to clipboard functionality
 */
function initCopyToClipboard() {
    const copyButtons = document.querySelectorAll('.btn-copy');
    
    copyButtons.forEach(button => {
        button.addEventListener('click', function() {
            const textToCopy = this.dataset.copyText || 
                              this.closest('.copy-container')?.querySelector('.copy-content')?.textContent;
            
            if (textToCopy) {
                navigator.clipboard.writeText(textToCopy).then(() => {
                    // Show success feedback
                    const originalHtml = this.innerHTML;
                    this.innerHTML = '<i class="ti ti-check me-1"></i>Đã sao chép';
                    this.classList.remove('btn-outline-secondary');
                    this.classList.add('btn-success');
                    
                    setTimeout(() => {
                        this.innerHTML = originalHtml;
                        this.classList.remove('btn-success');
                        this.classList.add('btn-outline-secondary');
                    }, 2000);
                }).catch(err => {
                    console.error('Failed to copy text: ', err);
                });
            }
        });
    });
}

/**
 * Initialize task filtering by status
 */
function initTaskFiltering() {
    const filterButtons = document.querySelectorAll('.task-filter-btn');
    
    filterButtons.forEach(button => {
        button.addEventListener('click', function() {
            const filter = this.dataset.filter;
            const taskCards = document.querySelectorAll('.task-card');
            
            // Update active button
            filterButtons.forEach(btn => btn.classList.remove('active'));
            this.classList.add('active');
            
            // Filter tasks
            taskCards.forEach(card => {
                if (filter === 'all') {
                    card.style.display = '';
                } else if (filter === 'correct' && card.classList.contains('correct')) {
                    card.style.display = '';
                } else if (filter === 'incorrect' && card.classList.contains('incorrect')) {
                    card.style.display = '';
                } else {
                    card.style.display = 'none';
                }
            });
        });
    });
}

// Export functions for use in other scripts
window.Word2019Results = {
    initTooltips,
    initTaskRows,
    initScoreCharts,
    initTableHoverEffects,
    initPrintFunctionality,
    initExpandCollapseAll,
    initSmoothScrolling,
    initProgressBarAnimations,
    initCopyToClipboard,
    initTaskFiltering,
    exportResultsAsJson,
    filterResultsTable,
    sortTable
};
