document.addEventListener('DOMContentLoaded', function() {
    console.log('GMetrix footer script loaded - DOM ready');
    
    // Global variables for Gmetrix data
    let gmetrixData = null;
    let projectsListData = null;
    let currentProjectId = 1;
    let currentTaskIndex = -1;
    let jsonLoadAttempted = false;
    
    // Simple backup: if dropdown still shows "loading" after 1 second, use fallback
    setTimeout(() => {
        const projectSelect = document.getElementById('project-select');
        if (projectSelect && projectSelect.options.length === 1 && 
            projectSelect.options[0].value === 'loading') {
            console.log('Dropdown still shows loading after 1 second, using fallback data');
            if (!gmetrixData) {
                initializeWithFallbackData();
            }
        }
    }, 1000);
    
    // Load JSON data
    async function loadJsonData() {
        jsonLoadAttempted = true;
        console.log('Starting to load JSON data...');
        
        // Set a timeout to fallback if loading takes too long
        const timeoutPromise = new Promise((_, reject) => {
            setTimeout(() => reject(new Error('JSON load timeout after 2 seconds')), 2000);
        });
        
        try {
            // Use Razor to resolve the path correctly
            // Add timestamp to prevent caching
            const jsonPath = '/areas/customer/json/gmetrix-mos-word-2019.json' + '?v=' + new Date().getTime();
            
            console.log(`Trying to load JSON from: ${jsonPath}`);
            const gmetrixResponse = await Promise.race([
                fetch(jsonPath),
                timeoutPromise
            ]);

            if (!gmetrixResponse || !gmetrixResponse.ok) {
                throw new Error(`HTTP error! status: ${gmetrixResponse ? gmetrixResponse.status : 'unknown'}`);
            }
            
            const rawData = await gmetrixResponse.json();
            console.log('Raw JSON data received:', rawData);
            
            // Initialize gmetrixData structure
            gmetrixData = { projects: [] };
            
            // Determine where the projects array is
            let rawProjects = [];
            if (Array.isArray(rawData)) {
                rawProjects = rawData;
            } else if (rawData.projects && Array.isArray(rawData.projects)) {
                rawProjects = rawData.projects;
            } else if (rawData.Projects && Array.isArray(rawData.Projects)) {
                rawProjects = rawData.Projects;
            }
            
            // Map and normalize projects
            if (rawProjects.length > 0) {
                gmetrixData.projects = rawProjects.map(p => {
                    let rawTasks = p.tasks || p.Tasks || [];
                    if (!Array.isArray(rawTasks)) rawTasks = [];
                    
                    return {
                        id: p.id || p.Id,
                        name: p.name || p.Name || `Project ${p.id || p.Id}`,
                        description: p.description || p.Description || '',
                        estimatedTime: p.estimatedTime || p.EstimatedTime || 'Unknown',
                        difficulty: p.difficulty || p.Difficulty || 'Normal',
                        tasks: rawTasks.map(t => ({
                            id: t.id || t.Id,
                            name: t.name || t.Name || `Task ${t.id || t.Id}`,
                            description: t.description || t.Description || '',
                            detailedInstructions: t.detailedInstructions || t.DetailedInstructions || t.description || t.Description || '',
                            isCompleted: false
                        }))
                    };
                });
            }
            
            console.log('Gmetrix data loaded successfully:', gmetrixData);
            
            // Load projects list data (optional)
            try {
                const projectsResponse = await fetch('/json/projects-list.json');
                projectsListData = await projectsResponse.json();
                console.log('Projects list data loaded successfully');
            } catch (e) {
                console.log('Projects list JSON not available, using Gmetrix data only');
            }
            
            initializeGmetrixUI();
        } catch (error) {
            console.error('Error loading JSON data:', error);
            console.log('Falling back to hard-coded data');
            // Fallback to hard-coded data
            initializeWithFallbackData();
        }
    }
    
    // Initialize UI with loaded data
    function initializeGmetrixUI() {
        if (!gmetrixData || !gmetrixData.projects || gmetrixData.projects.length === 0) {
            console.error('No project data available');
            initializeWithFallbackData();
            return;
        }
        
        // Filter projects that have tasks (tasks array exists and has length > 0)
        const projectsWithTasks = gmetrixData.projects.filter(project => 
            project.tasks && Array.isArray(project.tasks) && project.tasks.length > 0
        );
        
        if (projectsWithTasks.length === 0) {
            console.error('No projects with tasks available');
            initializeWithFallbackData();
            return;
        }
        
        // Update gmetrixData.projects to only include projects with tasks
        gmetrixData.projects = projectsWithTasks;
        
        // Set current project, update dropdown and tabs
        currentProjectId = gmetrixData.projects[0].id;
        const currentProject = getCurrentProject();
        
        updateProjectSelect(currentProject);
        updateTabs(currentProject);
        
        // Initialize with overview, suppress toast on initial load
        handleTabClick('overview', -1, true);
    }
    
    // Fallback to hard-coded data if JSON fails
    function initializeWithFallbackData() {
        console.log('Using fallback hard-coded data');
        
        // Create fallback project data
        const fallbackProjects = [
            {
                id: 1,
                name: "MOS Word 2019 - Project 1",
                tasks: [
                    { id: 1, detailedInstructions: "Dưới tiêu đề <strong>Landscaping Made Easy</strong>, chèn một ảnh chụp màn hình của bức ảnh hiển thị trên tài liệu <strong>Project</strong>.", isCompleted: false },
                    { id: 2, detailedInstructions: "Task 2 fallback instruction.", isCompleted: false },
                    { id: 3, detailedInstructions: "Task 3 fallback instruction.", isCompleted: false },
                    { id: 4, detailedInstructions: "Task 4 fallback instruction.", isCompleted: false },
                    { id: 5, detailedInstructions: "Task 5 fallback instruction.", isCompleted: false }
                ]
            },
            {
                id: 2,
                name: "MOS Word 2019 - Project 2", 
                tasks: [
                    { id: 6, detailedInstructions: "Task 6 fallback instruction.", isCompleted: false },
                    { id: 7, detailedInstructions: "Task 7 fallback instruction.", isCompleted: false },
                    { id: 8, detailedInstructions: "Task 8 fallback instruction.", isCompleted: false }
                ]
            }
        ];
        
        // Use fallback data
        gmetrixData = { projects: fallbackProjects };
        currentProjectId = 1;
        const currentProject = getCurrentProject();
        
        // Update project select dropdown
        updateProjectSelect(currentProject);
        
        // Update tabs based on current project tasks
        updateTabs(currentProject);
        
        // Initialize with overview
        handleTabClick('overview', -1);
        
        // Show warning toast
        showToast('Đang sử dụng dữ liệu mẫu', 'warning', 3000);
    }
    
    // Get current project by ID
    function getCurrentProject() {
        return gmetrixData.projects.find(p => p.id == currentProjectId) || gmetrixData.projects[0];
    }
    
    // Get current task by index
    function getCurrentTask() {
        const project = getCurrentProject();
        return project.tasks[currentTaskIndex] || project.tasks[0];
    }
    
    // Update project select dropdown
    function updateProjectSelect(currentProject) {
        const projectSelect = document.getElementById('project-select');
        if (!projectSelect) {
            console.error('Project select element not found');
            return;
        }
        
        console.log('Updating project select with data:', gmetrixData);
        
        // Clear existing options
        projectSelect.innerHTML = '';
        
        // Check if we have projects data
        if (!gmetrixData || !gmetrixData.projects || gmetrixData.projects.length === 0) {
            console.error('No project data available for dropdown');
            const option = document.createElement('option');
            option.value = 'error';
            option.textContent = 'Không có dữ liệu project';
            projectSelect.appendChild(option);
            return;
        }
        
        // Add options from Gmetrix data
        gmetrixData.projects.forEach(project => {
            const option = document.createElement('option');
            option.value = project.id;
            option.textContent = `${project.name} (${project.tasks.length} tasks)`;
            option.selected = project.id == currentProjectId;
            projectSelect.appendChild(option);
        });
        
        console.log(`Added ${gmetrixData.projects.length} projects to dropdown`);
    }
    
    // Update tabs based on project tasks
    function updateTabs(currentProject) {
        const tabsContainer = document.querySelector('.tabs-container');
        if (!tabsContainer) return;

        // Add click listener to overview tab if not already there
        const overviewTab = tabsContainer.querySelector('.tab[data-tab="overview"]');
        if (overviewTab && !overviewTab.dataset.listener) {
            overviewTab.addEventListener('click', () => handleTabClick('overview', -1));
            overviewTab.dataset.listener = 'true';
        }
        
        // Clear existing task tabs
        tabsContainer.querySelectorAll('.tab[data-tab^="task"]').forEach(t => t.remove());
        
        // Add tabs for each task
        currentProject.tasks.forEach((task, index) => {
            const tabId = `task${index + 1}`;
            const tab = document.createElement('button');
            tab.className = 'tab'; // Active class is now managed by handleTabClick
            tab.setAttribute('data-tab', tabId);
            tab.setAttribute('tabindex', '0');
            tab.setAttribute('aria-label', task.shortName || task.name || `Nhiệm vụ ${index + 1}`);
            
            const tabText = document.createElement('span');
            tabText.className = 'tab-text';
            tabText.textContent = task.shortName || task.name || `Nhiệm vụ ${index + 1}`;
            
            tab.appendChild(tabText);
            
            // Append to container to maintain order
            tabsContainer.appendChild(tab);
            
            // Add click event
            tab.addEventListener('click', function() {
                handleTabClick(tabId, index);
            });
        });
        
        // Update ARIA labels
        updateTabAriaLabels();
    }
    
    // Handle tab click
    function handleTabClick(tabId, taskIndex, suppressToast = false) {
        const oldActive = document.querySelector('.tab.active');
        if (oldActive) oldActive.classList.remove('active');
        
        const clickedTab = document.querySelector(`[data-tab="${tabId}"]`);
        if (clickedTab) clickedTab.classList.add('active');
        
        currentTaskIndex = taskIndex;
        updateInstruction(tabId);
        updateProgressIndicator();
        
        // Show toast with actual task name
        if (!suppressToast) {
            let taskName = '';
            if (tabId === 'overview') {
                taskName = 'Tổng quan';
            } else {
                const currentProject = getCurrentProject();
                const task = currentProject.tasks[taskIndex];
                taskName = task ? (task.shortName || task.name || `Nhiệm vụ ${taskIndex + 1}`) : `Nhiệm vụ ${taskIndex + 1}`;
            }
            
            showToast(`Đã chuyển đến ${taskName}`, 'info', 2000);
        }
    }
    
    // Update tab ARIA labels
    function updateTabAriaLabels() {
        document.querySelectorAll('.tab').forEach((tab, index) => {
            if (index === 0) {
                tab.setAttribute('aria-label', 'Tổng quan bài thi');
            } else {
                // Get task name from tab text content
                const tabText = tab.querySelector('.tab-text');
                const taskName = tabText ? tabText.textContent : `Nhiệm vụ số ${index}`;
                tab.setAttribute('aria-label', taskName);
            }
        });
    }
    
    // Update instruction based on tab/task
    function updateInstruction(tabId) {
        const instructionEl = document.getElementById('instruction-text');
        if (!instructionEl) return;
        
        let instructionHtml = '';
        
        if (tabId === 'overview') {
            const currentProject = getCurrentProject();
            instructionHtml = `
                <div class="instruction-content">
                    <strong>${currentProject.name}</strong><br>
                    ${currentProject.description}<br>
                    Tổng số task: ${currentProject.tasks.length}<br>
                    Thời gian ước tính: ${currentProject.estimatedTime}<br>
                    Độ khó: ${currentProject.difficulty}
                </div>
            `;
        } else {
            const taskIndex = parseInt(tabId.replace('task', '')) - 1;
            const currentProject = getCurrentProject();
            const task = currentProject.tasks[taskIndex];
            
            if (task) {
                instructionHtml = `
                    <div class="instruction-content">
                        ${task.detailedInstructions || task.description}
                    </div>
                `;
            }
        }
        
        // Fade effect
        instructionEl.style.opacity = '0.5';
        setTimeout(() => {
            instructionEl.innerHTML = instructionHtml;
            instructionEl.style.opacity = '1';
        }, 200);
    }
    
    // Update progress indicator
    function updateProgressIndicator() {
        const project = getCurrentProject();
        if (!project) return;
        const tasks = project.tasks;
        const total = tasks.length;
        const progressEl = document.getElementById('progress-indicator');

        if (progressEl) {
            let progressHtml = '<div class="progress-checkboxes">';
            tasks.forEach((task, index) => {
                const isCompleted = task.isCompleted;
                const isActive = index === currentTaskIndex;
                progressHtml += `<div class="progress-check ${isCompleted ? 'completed' : ''} ${isActive ? 'active' : ''}" data-task-index="${index}" title="Task ${index + 1}"></div>`;
            });
            progressHtml += '</div>';
            progressEl.innerHTML = progressHtml;
        }
    }
    
    // Start loading data
    loadJsonData();
    
    // Footer Toggle Logic
    const toggleBtn = document.getElementById('toggleFooter');
    const footer = document.getElementById('gmetrixFooter');
    const body = document.body;
    const toastContainer = document.getElementById('toast-container');
    
    if (toggleBtn && footer) {
        toggleBtn.addEventListener('click', function() {
            footer.classList.toggle('collapsed');
            const icon = this.querySelector('i');
            
            if (footer.classList.contains('collapsed')) {
                icon.classList.remove('fa-chevron-down');
                icon.classList.add('fa-chevron-up');
                body.style.paddingBottom = '60px';
                if (toastContainer) toastContainer.style.bottom = '70px';
            } else {
                icon.classList.remove('fa-chevron-up');
                icon.classList.add('fa-chevron-down');
                body.style.paddingBottom = '260px';
                if (toastContainer) toastContainer.style.bottom = '280px';
            }
        });
    }

    // Toast Notification System
    function showToast(message, type = 'info', duration = 4000) {
        const toastContainer = document.getElementById('toast-container') || createToastContainer();
        const toast = document.createElement('div');
        toast.className = `toast ${type}`;
        toast.innerHTML = `
            <i class="fas ${type === 'success' ? 'fa-check-circle' : type === 'warning' ? 'fa-exclamation-triangle' : 'fa-info-circle'}"></i>
            <span>${message}</span>
        `;
        
        toastContainer.appendChild(toast);
        
        // Trigger animation
        setTimeout(() => toast.classList.add('show'), 10);
        
        // Auto remove
        setTimeout(() => {
            toast.classList.remove('show');
            setTimeout(() => {
                if (toast.parentNode) {
                    toast.parentNode.removeChild(toast);
                }
            }, 400);
        }, duration);
        
        // Click to dismiss
        toast.addEventListener('click', () => {
            toast.classList.remove('show');
            setTimeout(() => {
                if (toast.parentNode) {
                    toast.parentNode.removeChild(toast);
                }
            }, 400);
        });
    }
    
    function createToastContainer() {
        const container = document.createElement('div');
        container.id = 'toast-container';
        container.className = 'toast-container';
        document.body.appendChild(container);
        return container;
    }
    
    // Tab switching with animation - using new dynamic tabs
    // Note: Tabs are now created dynamically in updateTabs() function
    // Event listeners are attached when tabs are created
    
    // Project select change handler - updated to use JSON data
    const projectSelect = document.getElementById('project-select');
    if (projectSelect) {
        projectSelect.addEventListener('change', function() {
            const newProjectId = parseInt(this.value);
            
            if (newProjectId !== currentProjectId && gmetrixData) {
                currentProjectId = newProjectId;
                
                const currentProject = getCurrentProject();
                
                // Update tabs for the new project
                updateTabs(currentProject);
                
                // Set view to overview for the new project, suppressing the default toast
                handleTabClick('overview', -1, true);
                
                // Show a toast message indicating the project has changed
                showToast(`Đã chuyển đến ${currentProject.name}`, 'info', 2000);
            }
        });
    }
    
    // Button event handlers with loading states
    document.getElementById('prev-task').addEventListener('click', function() {
        const btn = this;
        btn.classList.add('loading');
        setTimeout(() => {
            btn.classList.remove('loading');
            showToast('Đã chuyển đến nhiệm vụ trước', 'success');
        }, 800);
    });
    
    document.getElementById('next-task').addEventListener('click', function() {
        const btn = this;
        btn.classList.add('loading');
        setTimeout(() => {
            btn.classList.remove('loading');
            showToast('Đã chuyển đến nhiệm vụ tiếp theo', 'success');
        }, 800);
    });
    
    document.getElementById('mark-completed').addEventListener('click', function() {
        const btn = this;
        btn.classList.add('loading');

        const project = getCurrentProject();
        if (!project || !project.tasks[currentTaskIndex]) return;
        
        const task = project.tasks[currentTaskIndex];
        task.isCompleted = !task.isCompleted; // Toggle completion

        updateProgressIndicator(); // Re-render progress

        setTimeout(() => {
            btn.classList.remove('loading');
            showToast(task.isCompleted ? 'Đã đánh dấu hoàn thành' : 'Đã bỏ đánh dấu hoàn thành', 'success');
        }, 300);
    });
    
    document.getElementById('mark-review').addEventListener('click', function() {
        const btn = this;
        btn.classList.add('loading');
        setTimeout(() => {
            btn.classList.remove('loading');
            showToast('Đã đánh dấu cần xem lại', 'warning');
        }, 800);
    });
    
    document.getElementById('help-btn').addEventListener('click', function() {
        showToast('Đang tải trợ giúp...', 'info');
    });
    
    // Top link handlers
    document.getElementById('restart-link').addEventListener('click', function() {
        if (confirm('Bạn có chắc muốn khởi động lại bài thi? Mọi tiến độ chưa lưu sẽ bị mất.')) {
            showToast('Đang khởi động lại bài thi...', 'warning', 3000);
        }
    });
    
    
    document.getElementById('grade-link').addEventListener('click', function() {
        const btn = this;
        btn.classList.add('loading');
        setTimeout(() => {
            btn.classList.remove('loading');
            showToast('Đang chấm điểm... Vui lòng đợi', 'info', 3000);
        }, 800);
    });
    
    // Open project button handler
    document.getElementById('open-project-btn').addEventListener('click', function() {
        const btn = this;
        btn.classList.add('loading');
        
        // Lấy project ID được chọn
        const projectSelect = document.getElementById('project-select');
        const projectId = projectSelect ? projectSelect.value : '1';
        
        // Kiểm tra nếu WebView2 bridge tồn tại
        if (window.chrome && window.chrome.webview) {
            // Gửi message đến ứng dụng WinForms với project ID
            window.chrome.webview.postMessage(`OPEN_PROJECT_${projectId}`);
            
            // Hiển thị thông báo đang xử lý
            showToast(`Đang mở bài làm Project ${projectId}...`, 'info', 2000);
        } else {
            // Fallback cho trình duyệt thông thường
            showToast('Không thể mở bài làm trong môi trường này', 'warning');
            btn.classList.remove('loading');
        }
    });

    // Submit project button handler
    document.getElementById('submit-project-btn').addEventListener('click', function() {
        const btn = this;
        btn.classList.add('loading');
        
        // Lấy project ID được chọn
        const projectSelect = document.getElementById('project-select');
        const projectId = projectSelect ? projectSelect.value : '1';
        
        // Kiểm tra nếu WebView2 bridge tồn tại
        if (window.chrome && window.chrome.webview) {
            // Gửi message đến ứng dụng WinForms
            window.chrome.webview.postMessage(`SUBMIT_PROJECT_${projectId}`);
            
            // Hiển thị thông báo
            showToast(`Đang nộp bài Project ${projectId}...`, 'info', 3000);
        } else {
            // Fallback cho trình duyệt thông thường
            showToast('Không thể nộp bài trong môi trường này', 'warning');
            btn.classList.remove('loading');
        }
    });

    // Listen for word open result from WinForms
    window.addEventListener('word-open-result', function(event) {
        const result = event.detail;
        const btn = document.getElementById('open-project-btn');
        btn.classList.remove('loading');
        
        // Extract project ID from result (e.g., "WORD_OPENED_SUCCESS_1")
        const parts = result.split('_');
        const resultType = parts.slice(0, 3).join('_'); // "WORD_OPENED_SUCCESS"
        const projectId = parts.length > 3 ? parts[3] : '1';
        
        switch(resultType) {
            case 'WORD_OPENED_SUCCESS':
                showToast(`Đã mở bài làm Project ${projectId} thành công`, 'success');
                break;
            case 'WORD_FILE_NOT_FOUND':
                showToast(`Không tìm thấy file bài làm Project ${projectId}`, 'error');
                break;
            case 'WORD_OPEN_ERROR':
                showToast(`Lỗi khi mở file Word Project ${projectId}`, 'error');
                break;
            case 'WORD_CLOSED_PREVIOUS':
                const wordCount = parts.length > 3 ? parts[3] : '0';
                showToast(`Đã đóng ${wordCount} file Word trước đó`, 'info');
                break;
        }
    });
    
    // Listen for grading result from WinForms
    window.addEventListener('word-grading-result', function(event) {
        const result = event.detail; 
        // Remove loading state from submit button
        document.getElementById('submit-project-btn').classList.remove('loading');
        
        // Check if result is JSON (New Format)
        if (result.startsWith('GRADING_JSON:')) {
            const jsonStr = result.substring(13); // Remove "GRADING_JSON:" prefix
            try {
                const data = JSON.parse(jsonStr);
                showGradingResultModalJson(data);
            } catch (e) {
                console.error("Error parsing grading JSON", e);
                showToast('Lỗi hiển thị kết quả chấm điểm', 'error');
            }
        } else {
            // Fallback for Old Format: "GRADING_RESULT_1_85_100_true"
            const parts = result.split('_');
            showGradingResultModal(parts[3], parts[4], parts[5] === 'true', parts[2]);
        }
    });

    // Show grading result modal
    function showGradingResultModal(score, maxScore, passed, projectId) {
        // Create modal HTML
        const modalHtml = `
            <div class="grading-modal" style="position:fixed;top:0;left:0;width:100%;height:100%;background:rgba(0,0,0,0.7);z-index:2000;display:flex;align-items:center;justify-content:center;">
                <div style="background:white;border-radius:10px;padding:30px;max-width:600px;width:90%;max-height:80vh;overflow-y:auto;">
                    <h2 style="color:${passed ? '#28a745' : '#dc3545'};margin-top:0;">
                        <i class="fas fa-${passed ? 'check-circle' : 'times-circle'}"></i>
                        KẾT QUẢ CHẤM ĐIỂM
                    </h2>
                    <div style="text-align:center;margin:20px 0;">
                        <div style="font-size:48px;font-weight:bold;color:${passed ? '#28a745' : '#dc3545'}">
                            ${score}/${maxScore}
                        </div>
                        <div style="font-size:24px;margin:10px 0;">
                            ${passed ? '✅ ĐẬU' : '❌ RỚT'}
                        </div>
                        <div style="background:${passed ? '#d4edda' : '#f8d7da'};padding:15px;border-radius:5px;margin:20px 0;">
                            <strong>Project ${projectId}:</strong> ${passed ? 'Chúc mừng! Bạn đã hoàn thành bài thi.' : 'Cần cải thiện kỹ năng Word.'}
                        </div>
                    </div>
                    
                    <h3><i class="fas fa-list-check"></i> Chi tiết chấm điểm</h3>
                    <div id="grading-details" style="margin:15px 0;">
                        <div style="text-align:center;padding:20px;color:#6c757d;">Chi tiết chưa khả dụng ở chế độ này.</div>
                    </div>
                    
                    <div style="display:flex;gap:10px;justify-content:flex-end;margin-top:20px;">
                        <button onclick="closeGradingModal()" style="padding:10px 20px;background:#6c757d;color:white;border:none;border-radius:5px;cursor:pointer;">
                            <i class="fas fa-times"></i> Đóng
                        </button>
                        <button onclick="reviewMistakes()" style="padding:10px 20px;background:#007bff;color:white;border:none;border-radius:5px;cursor:pointer;">
                            <i class="fas fa-redo"></i> Xem lại lỗi
                        </button>
                    </div>
                </div>
            </div>
        `;
        
        // Add modal to body
        const modalContainer = document.createElement('div');
        modalContainer.id = 'grading-modal-container';
        modalContainer.innerHTML = modalHtml;
        document.body.appendChild(modalContainer);
        
        // Request grading details
        if (window.chrome && window.chrome.webview) {
            window.chrome.webview.postMessage(`GET_GRADING_DETAILS_${projectId}`);
        }
    }

    // Show grading result modal with JSON data (Detailed)
    function showGradingResultModalJson(data) {
        // data structure: { projectId, score, maxScore, isPassed, tasks: [{ name, isCorrect, message }] }
        const passed = data.isPassed;
        
        let tasksHtml = '<div style="max-height:300px;overflow-y:auto;border:1px solid #eee;border-radius:5px;">';
        
        if (data.tasks && data.tasks.length > 0) {
            data.tasks.forEach((task, index) => {
                const icon = task.isCorrect ? '<i class="fas fa-check-circle" style="color:#28a745"></i>' : '<i class="fas fa-times-circle" style="color:#dc3545"></i>';
                const bg = index % 2 === 0 ? '#f8f9fa' : '#fff';
                const message = task.isCorrect ? '' : `<div style="font-size:12px;color:#dc3545;margin-top:4px;margin-left:24px;">${task.message || 'Sai yêu cầu đề bài'}</div>`;
                
                tasksHtml += `
                    <div style="padding:10px;background:${bg};border-bottom:1px solid #eee;">
                        <div style="display:flex;align-items:center;gap:10px;font-weight:500;">
                            ${icon} <span>${task.name}</span>
                        </div>
                        ${message}
                    </div>
                `;
            });
        } else {
            tasksHtml += '<div style="padding:15px;text-align:center;">Không có chi tiết task.</div>';
        }
        tasksHtml += '</div>';

        const modalHtml = `
            <div class="grading-modal" style="position:fixed;top:0;left:0;width:100%;height:100%;background:rgba(0,0,0,0.7);z-index:2000;display:flex;align-items:center;justify-content:center;">
                <div style="background:white;border-radius:10px;padding:30px;max-width:600px;width:90%;max-height:80vh;overflow-y:auto;box-shadow:0 10px 25px rgba(0,0,0,0.2);">
                    <h2 style="color:${passed ? '#28a745' : '#dc3545'};margin-top:0;text-align:center;">
                        <i class="fas fa-${passed ? 'check-circle' : 'times-circle'}"></i>
                        ${passed ? 'ĐẠT' : 'CHƯA ĐẠT'}
                    </h2>
                    <div style="text-align:center;margin:10px 0 20px;">
                        <div style="font-size:42px;font-weight:bold;color:${passed ? '#28a745' : '#dc3545'}">
                            ${data.score}/${data.maxScore}
                        </div>
                    </div>
                    
                    <h4 style="border-bottom:2px solid #eee;padding-bottom:10px;margin-bottom:10px;">
                        <i class="fas fa-list-check"></i> Chi tiết bài làm
                    </h4>
                    ${tasksHtml}
                    
                    <div style="display:flex;gap:10px;justify-content:flex-end;margin-top:20px;">
                        <button onclick="closeGradingModal()" style="padding:10px 20px;background:#6c757d;color:white;border:none;border-radius:5px;cursor:pointer;font-weight:600;">
                            <i class="fas fa-times"></i> Đóng
                        </button>
                    </div>
                </div>
            </div>
        `;
        
        const modalContainer = document.createElement('div');
        modalContainer.id = 'grading-modal-container';
        modalContainer.innerHTML = modalHtml;
        document.body.appendChild(modalContainer);
    }

    // Close grading modal
    function closeGradingModal() {
        const modal = document.getElementById('grading-modal-container');
        if (modal) {
            modal.remove();
        }
    }

    // Review mistakes
    function reviewMistakes() {
        showToast('Đang tải chi tiết lỗi...', 'info');
        // Could navigate to tabs with incorrect questions
    }

    
    // Keyboard navigation
    document.addEventListener('keydown', function(e) {
        // Tab navigation with arrow keys
        if (e.key === 'ArrowRight' || e.key === 'ArrowLeft') {
            const tabs = Array.from(document.querySelectorAll('.tab'));
            const currentIndex = tabs.findIndex(tab => tab.classList.contains('active'));
            
            if (e.key === 'ArrowRight' && currentIndex < tabs.length - 1) {
                tabs[currentIndex + 1].click();
                tabs[currentIndex + 1].focus();
            } else if (e.key === 'ArrowLeft' && currentIndex > 0) {
                tabs[currentIndex - 1].click();
                tabs[currentIndex - 1].focus();
            }
        }
        
        // Space/Enter for buttons
        if (e.key === ' ' || e.key === 'Enter') {
            const focused = document.activeElement;
            if (focused.classList.contains('btn') || focused.classList.contains('tab')) {
                focused.click();
                e.preventDefault();
            }
        }
        
        // Shortcuts
        if (e.ctrlKey || e.metaKey) {
            switch(e.key) {
                case 'r':
                    e.preventDefault();
                    document.getElementById('restart-link').click();
                    break;
                case 'g':
                    e.preventDefault();
                    document.getElementById('grade-link').click();
                    break;
            }
        }
    });
    
    // Add tabindex for keyboard navigation
    document.querySelectorAll('.btn, .tab, .top-link').forEach(el => {
        el.setAttribute('tabindex', '0');
    });
    
    // Add ARIA labels for accessibility - will be updated by updateTabAriaLabels()
    // Initial labels set here, will be updated when tabs are created
    document.querySelectorAll('.tab').forEach((tab, index) => {
        if (index === 0) {
            tab.setAttribute('aria-label', 'Tổng quan bài thi');
        } else {
            // Temporary label, will be updated by updateTabAriaLabels()
            tab.setAttribute('aria-label', `Nhiệm vụ số ${index}`);
        }
    });
    
    document.getElementById('prev-task').setAttribute('aria-label', 'Nhiệm vụ trước');
    document.getElementById('next-task').setAttribute('aria-label', 'Nhiệm vụ tiếp theo');
    document.getElementById('mark-completed').setAttribute('aria-label', 'Đánh dấu hoàn thành');
    document.getElementById('mark-review').setAttribute('aria-label', 'Đánh dấu cần xem lại');
    document.getElementById('help-btn').setAttribute('aria-label', 'Trợ giúp');
    document.getElementById('open-project-btn').setAttribute('aria-label', 'Mở bài làm');
    
});
