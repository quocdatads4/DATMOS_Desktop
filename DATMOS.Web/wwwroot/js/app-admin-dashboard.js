// Admin Dashboard JavaScript - Sample Data
document.addEventListener('DOMContentLoaded', function() {
    console.log('Admin Dashboard initialized with sample data');
    
    // Initialize system uptime chart if element exists
    const systemUptimeChartEl = document.getElementById('systemUptimeChart');
    if (systemUptimeChartEl) {
        initializeSystemUptimeChart();
    }
    
    // Initialize horizontal bar chart if element exists
    const horizontalBarChartEl = document.getElementById('horizontalBarChart');
    if (horizontalBarChartEl) {
        initializeHorizontalBarChart();
    }
    
    // Initialize DataTable if element exists
    const datatableEl = document.querySelector('.datatables-admin-system');
    if (datatableEl) {
        initializeSystemDataTable();
    }
    
    // Initialize progress charts
    initializeProgressCharts();
});

function initializeSystemUptimeChart() {
    // Check if ApexCharts is available
    if (typeof ApexCharts === 'undefined') {
        console.warn('ApexCharts is not loaded. Chart will not be rendered.');
        return;
    }
    
    // Data for system uptime chart - weekly data
    const uptimeChartOptions = {
        series: [{
            name: 'Uptime',
            data: [98, 97, 99, 98, 96, 99, 98]
        }],
        chart: {
            height: 100,
            type: 'area',
            toolbar: {
                show: false
            },
            sparkline: {
                enabled: true
            },
            zoom: {
                enabled: false
            }
        },
        colors: ['#7367F0'],
        stroke: {
            curve: 'smooth',
            width: 2,
            lineCap: 'round'
        },
        fill: {
            type: 'gradient',
            gradient: {
                shadeIntensity: 1,
                opacityFrom: 0.7,
                opacityTo: 0.2,
                stops: [0, 90, 100]
            }
        },
        xaxis: {
            categories: ['T2', 'T3', 'T4', 'T5', 'T6', 'T7', 'CN'],
            labels: {
                show: false
            },
            axisBorder: {
                show: false
            },
            axisTicks: {
                show: false
            }
        },
        yaxis: {
            min: 90,
            max: 100,
            labels: {
                show: false
            }
        },
        grid: {
            show: false,
            padding: {
                left: 0,
                right: 0,
                top: 0,
                bottom: 0
            }
        },
        tooltip: {
            enabled: true,
            x: {
                show: true,
                formatter: function(val, opts) {
                    return opts.w.globals.categoryLabels[opts.dataPointIndex];
                }
            },
            y: {
                formatter: function(val) {
                    return val + '%';
                },
                title: {
                    formatter: function() {
                        return 'Uptime:';
                    }
                }
            },
            marker: {
                show: true
            }
        },
        dataLabels: {
            enabled: false
        }
    };
    
    // Create and render the chart
    try {
        const chartElement = document.getElementById('systemUptimeChart');
        if (chartElement) {
            const chart = new ApexCharts(chartElement, uptimeChartOptions);
            chart.render();
            console.log('System uptime chart initialized successfully');
            
            // Store chart instance for potential updates
            window.systemUptimeChart = chart;
        } else {
            console.warn('Chart element #systemUptimeChart not found');
        }
    } catch (error) {
        console.error('Error initializing system uptime chart:', error);
    }
}

function initializeHorizontalBarChart() {
    // Check if ApexCharts is available
    if (typeof ApexCharts === 'undefined') {
        console.warn('ApexCharts is not loaded. Chart will not be rendered.');
        return;
    }
    
    // Data matching the UI: Users, Activities, System Errors, Visits, Data, Others
    const chartData = {
        categories: ['Người dùng', 'Hoạt động', 'Lỗi hệ thống', 'Truy cập', 'Dữ liệu', 'Khác'],
        series: [{
            name: 'Phần trăm',
            data: [45, 28, 5, 15, 7, 3]
        }],
        colors: ['#7367F0', '#28C76F', '#EA5455', '#00CFE8', '#A8AAAE', '#FF9F43']
    };
    
    const chartOptions = {
        series: chartData.series,
        chart: {
            type: 'bar',
            height: 320,
            toolbar: {
                show: false
            },
            animations: {
                enabled: true,
                speed: 800,
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
        plotOptions: {
            bar: {
                horizontal: true,
                barHeight: '70%',
                borderRadius: 8,
                borderRadiusApplication: 'end',
                dataLabels: {
                    position: 'top'
                }
            }
        },
        dataLabels: {
            enabled: true,
            textAnchor: 'start',
            style: {
                fontSize: '13px',
                fontWeight: 600,
                colors: ['#fff']
            },
            formatter: function(val) {
                return val + '%';
            },
            offsetX: 10
        },
        colors: chartData.colors,
        xaxis: {
            categories: chartData.categories,
            labels: {
                style: {
                    fontSize: '13px',
                    fontWeight: 500
                }
            },
            axisBorder: {
                show: false
            },
            axisTicks: {
                show: false
            },
            max: 50 // Set max to 50% for better visualization
        },
        yaxis: {
            labels: {
                style: {
                    fontSize: '13px',
                    fontWeight: 500
                }
            }
        },
        grid: {
            borderColor: '#e0e0e0',
            strokeDashArray: 4,
            padding: {
                top: 0,
                right: 20,
                bottom: 0,
                left: 20
            }
        },
        tooltip: {
            y: {
                formatter: function(val) {
                    return val + '%';
                }
            },
            style: {
                fontSize: '13px'
            }
        },
        responsive: [{
            breakpoint: 768,
            options: {
                chart: {
                    height: 280
                },
                dataLabels: {
                    style: {
                        fontSize: '11px'
                    }
                },
                xaxis: {
                    labels: {
                        style: {
                            fontSize: '11px'
                        }
                    }
                },
                yaxis: {
                    labels: {
                        style: {
                            fontSize: '11px'
                        }
                    }
                }
            }
        }]
    };
    
    // Create and render the chart
    try {
        const chartElement = document.getElementById('horizontalBarChart');
        if (chartElement) {
            const chart = new ApexCharts(chartElement, chartOptions);
            chart.render();
            console.log('Horizontal bar chart initialized successfully');
            
            // Store chart instance for potential updates
            window.horizontalBarChart = chart;
        } else {
            console.warn('Chart element #horizontalBarChart not found');
        }
    } catch (error) {
        console.error('Error initializing horizontal bar chart:', error);
    }
}

function initializeSystemDataTable() {
    // Sample data for system services table
    const sampleData = [
        {
            id: 1,
            service: 'Web Server',
            time: '24/7',
            progress: 95,
            status: 'Active'
        },
        {
            id: 2,
            service: 'Database',
            time: '24/7',
            progress: 88,
            status: 'Active'
        },
        {
            id: 3,
            service: 'API Gateway',
            time: '24/7',
            progress: 92,
            status: 'Active'
        },
        {
            id: 4,
            service: 'Cache Server',
            time: '24/7',
            progress: 78,
            status: 'Warning'
        },
        {
            id: 5,
            service: 'File Storage',
            time: '24/7',
            progress: 85,
            status: 'Active'
        },
        {
            id: 6,
            service: 'Email Service',
            time: 'Business Hours',
            progress: 65,
            status: 'Maintenance'
        },
        {
            id: 7,
            service: 'Backup System',
            time: 'Nightly',
            progress: 100,
            status: 'Active'
        }
    ];
    
    console.log('DataTable sample data:', sampleData);
    
    // In a real implementation, you would initialize DataTables here
    // $('.datatables-admin-system').DataTable({
    //     data: sampleData,
    //     columns: [
    //         { data: 'id' },
    //         { data: 'service' },
    //         { data: 'time' },
    //         { data: 'progress' },
    //         { data: 'status' }
    //     ]
    // });
}

function initializeProgressCharts() {
    // Initialize circular progress charts
    const progressElements = document.querySelectorAll('.chart-progress');
    
    progressElements.forEach(element => {
        const color = element.getAttribute('data-color') || 'primary';
        const series = parseInt(element.getAttribute('data-series')) || 50;
        const isVariant = element.getAttribute('data-progress_variant') === 'true';
        
        console.log(`Progress chart: color=${color}, series=${series}%, variant=${isVariant}`);
        
        // In a real implementation, you would initialize progress charts here
        // This could be using ApexCharts, Chart.js, or a custom implementation
    });
}

// Utility function to format numbers
function formatNumber(num) {
    return num.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
}

// Sample data export for other components
window.adminDashboardData = {
    statistics: {
        users: 1234,
        activities: 568,
        completion: 89,
        uptime: 98.2
    },
    recentActivities: [
        { user: 'Nguyễn Văn A', action: 'Đăng nhập hệ thống', time: '5 phút' },
        { user: 'Trần Thị B', action: 'Cập nhật hồ sơ', time: '12 phút' },
        { user: 'Lê Văn C', action: 'Tạo đơn hàng mới', time: '25 phút' },
        { user: 'Phạm Thị D', action: 'Tải lên tài liệu', time: '1 giờ' }
    ],
    quickActions: [
        { name: 'Quản lý người dùng', count: 1200 },
        { name: 'Cài đặt hệ thống', count: 834 },
        { name: 'Báo cáo thống kê', count: 3700 },
        { name: 'Bảo mật hệ thống', count: 2500 },
        { name: 'Thông báo hệ thống', count: 948 }
    ],
    systemStatus: {
        online: true,
        uptime: '99.8%'
    }
};
