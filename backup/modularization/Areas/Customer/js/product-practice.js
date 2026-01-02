/* Product Practice JavaScript - Practice mode functionality */

// Practice session management
const PracticeSession = {
  // Initialize practice session
  init: function() {
    if (!window.practiceSession) {
      console.error('Practice session data not found');
      return;
    }

    this.session = window.practiceSession;
    this.currentProjectIndex = this.getCurrentProjectIndex();
    this.initEventListeners();
    this.loadSavedData();
    this.updateProgress();
    this.showInstructionsModal();
  },

  // Get current project index
  getCurrentProjectIndex: function() {
    return this.session.projects.findIndex(p => p.Id === this.session.currentProjectId);
  },

  // Initialize event listeners
  initEventListeners: function() {
    // Project navigation (updated for new structure)
    document.querySelectorAll('#projectList .list-group-item-action').forEach(item => {
      item.addEventListener('click', (e) => {
        e.preventDefault();
        const projectId = parseInt(item.dataset.projectId);
        this.switchProject(projectId);
      });
    });

    // Task navigation buttons
    document.querySelectorAll('.task-nav-btn').forEach(btn => {
      btn.addEventListener('click', (e) => {
        e.preventDefault();
        const taskIndex = parseInt(btn.dataset.taskIndex);
        this.switchTask(taskIndex);
      });
    });

    // Nav actions
    const overviewBtn = document.getElementById('overviewBtn');
    if (overviewBtn) overviewBtn.addEventListener('click', () => this.showOverview());

    const testBtn = document.getElementById('testBtn');
    if (testBtn) testBtn.addEventListener('click', () => this.switchToTestMode());

    const summaryBtn = document.getElementById('summaryBtn');
    if (summaryBtn) summaryBtn.addEventListener('click', () => this.showSummary());

    const resetBtn = document.getElementById('resetBtn');
    if (resetBtn) resetBtn.addEventListener('click', () => this.resetSession());

    const saveBtn = document.getElementById('saveBtn');
    if (saveBtn) saveBtn.addEventListener('click', () => this.saveCurrentState());

    const gradeBtn = document.getElementById('gradeBtn');
    if (gradeBtn) gradeBtn.addEventListener('click', () => this.gradeCurrentTask());

    // Action buttons
    const previewBtn = document.getElementById('previewBtn');
    if (previewBtn) previewBtn.addEventListener('click', () => this.previewTask());

    const markCompleteBtn = document.getElementById('markCompleteBtn');
    if (markCompleteBtn) markCompleteBtn.addEventListener('click', () => this.markTaskCompleteWithScreenshot());

    const insertScreenshotBtn = document.getElementById('insertScreenshotBtn');
    if (insertScreenshotBtn) insertScreenshotBtn.addEventListener('click', () => this.insertScreenshot());

    const displayHelpBtn = document.getElementById('displayHelpBtn');
    if (displayHelpBtn) displayHelpBtn.addEventListener('click', () => this.showTaskHelp());

    // Save answer buttons (updated for new textarea)
    document.querySelectorAll('textarea[data-task-id]').forEach(textarea => {
      // Add save button dynamically if not exists
      if (!textarea.nextElementSibling || !textarea.nextElementSibling.classList.contains('save-answer-btn')) {
        const saveBtn = document.createElement('button');
        saveBtn.className = 'btn btn-sm btn-outline-info save-answer-btn mt-2';
        saveBtn.innerHTML = '<i class="ti ti-check me-1"></i>Lưu câu trả lời';
        saveBtn.dataset.taskId = textarea.dataset.taskId;
        textarea.parentNode.appendChild(saveBtn);
      }

      // Event listener for save
      const taskId = parseInt(textarea.dataset.taskId);
      const saveBtn = document.querySelector(`.save-answer-btn[data-task-id="${taskId}"]`);
      if (saveBtn) {
        saveBtn.addEventListener('click', (e) => {
          e.preventDefault();
          this.saveAnswer(taskId, textarea.value);
        });
      }
    });

    // Hint and answer collapse events (updated IDs)
    document.addEventListener('show.bs.collapse', (e) => {
      const targetId = e.target.id;
      if (targetId.includes('hint-')) {
        const taskId = targetId.replace('hint-', '');
        this.trackHintView(taskId);
      } else if (targetId.includes('answer-')) {
        const taskId = targetId.replace('answer-', '');
        this.trackAnswerView(taskId);
      }
    });

    // Submit practice button (if still present)
    const submitPracticeBtn = document.getElementById('submitPracticeBtn');
    if (submitPracticeBtn) {
      submitPracticeBtn.addEventListener('click', () => {
        this.submitPractice();
      });
    }
  },

  // Load saved data from localStorage
  loadSavedData: function() {
    // Load saved answers
    document.querySelectorAll('.answer-input').forEach(input => {
      const taskId = parseInt(input.dataset.taskId);
      const savedAnswer = ProductCommon.loadAnswer(taskId, 'practice');
      if (savedAnswer) {
        input.value = savedAnswer;
      }
    });

    // Load saved feedback
    this.session.projects.forEach(project => {
      if (project.Tasks) {
        project.Tasks.forEach(task => {
          const savedFeedback = localStorage.getItem(`practice_feedback_${task.Id}`);
          if (savedFeedback) {
            const radio = document.querySelector(`input[name="feedback-${task.Id}"][value="${savedFeedback}"]`);
            if (radio) {
              radio.checked = true;
            }
          }
        });
      }
    });

    // Load completed tasks
    const completedTasks = ProductCommon.getCompletedTasks('practice');
    completedTasks.forEach(taskId => {
      ProductCommon.markTaskComplete(taskId, 'practice');
    });
  },

  // Switch to different project (updated)
  switchProject: function(projectId) {
    // Update URL without page reload
    const newUrl = `${window.location.pathname}?projectId=${projectId}`;
    window.history.pushState({ projectId }, '', newUrl);

    // Update current project
    this.session.currentProjectId = projectId;
    this.currentProjectIndex = this.getCurrentProjectIndex();

    // Reload page to load new tasks (since tasks are server-rendered)
    window.location.href = newUrl;

    ProductCommon.showToast(`Đã chuyển đến project mới`, 'info');
  },

  // Update project UI (updated for new structure)
  updateProjectUI: function() {
    // Update active project in sidebar
    document.querySelectorAll('#projectList .list-group-item-action').forEach(item => {
      const itemProjectId = parseInt(item.dataset.projectId);
      if (itemProjectId === this.session.currentProjectId) {
        item.classList.add('active');
      } else {
        item.classList.remove('active');
      }
    });

    // Update project title
    const currentProject = this.session.projects[this.currentProjectIndex];
    if (currentProject) {
      const titleElement = document.querySelector('.gmetrix-header h5');
      if (titleElement) {
        titleElement.textContent = currentProject.Name;
      }
    }

    // Update task navigation (will be handled on page load)
  },

  // New: Switch between tasks
  switchTask: function(taskIndex) {
    // Hide all task items
    document.querySelectorAll('.task-item').forEach(item => {
      item.classList.remove('active');
      item.classList.add('d-none');
    });

    // Show selected task
    const selectedTask = document.querySelector(`.task-item[data-task-index="${taskIndex}"]`);
    if (selectedTask) {
      selectedTask.classList.add('active');
      selectedTask.classList.remove('d-none');
    }

    // Update active button
    document.querySelectorAll('.task-nav-btn').forEach(btn => {
      btn.classList.remove('btn-primary');
      btn.classList.add('btn-outline-primary');
    });
    const activeBtn = document.querySelector(`.task-nav-btn[data-task-index="${taskIndex}"]`);
    if (activeBtn) {
      activeBtn.classList.remove('btn-outline-primary');
      activeBtn.classList.add('btn-primary');
    }

    // Scroll to task area
    const taskContent = document.querySelector('.col-md-9');
    if (taskContent) {
      taskContent.scrollIntoView({ behavior: 'smooth' });
    }

    ProductCommon.showToast(`Đã chuyển đến Task ${taskIndex + 1}`, 'info');
  },

  // New: Show overview
  showOverview: function() {
    // Implement overview modal or section
    ProductCommon.showToast('Tổng quan sẽ hiển thị tại đây', 'info');
  },

  // New: Switch to test mode
  switchToTestMode: function() {
    // Redirect to test page
    window.location.href = window.location.pathname.replace('Practice', 'Test');
  },

  // New: Show summary
  showSummary: function() {
    // Implement summary
    this.submitPractice(); // For now, show results
  },

  // New: Reset session
  resetSession: function() {
    ProductCommon.confirmDialog('Bạn có chắc muốn reset session? Tất cả tiến độ sẽ bị mất.').then(confirmed => {
      if (confirmed) {
        ProductCommon.clearSessionData('practice');
        window.location.reload();
      }
    });
  },

  // New: Save current state
  saveCurrentState: function() {
    // Save all current answers and progress
    document.querySelectorAll('textarea[data-task-id]').forEach(textarea => {
      const taskId = parseInt(textarea.dataset.taskId);
      this.saveAnswer(taskId, textarea.value);
    });
    ProductCommon.showToast('Đã lưu toàn bộ tiến độ', 'success');
  },

  // New: Grade current task
  gradeCurrentTask: function() {
    // Auto-grade based on answer matching expected
    const activeTask = document.querySelector('.task-item.active');
    if (activeTask) {
      const taskId = activeTask.dataset.taskId;
      const userAnswer = document.querySelector(`textarea[data-task-id="${taskId}"]`).value;
      // Simple check - in real impl, compare with model.Answer
      ProductCommon.showToast(userAnswer.trim() ? 'Task được chấm điểm tự động' : 'Hãy nhập câu trả lời trước', userAnswer.trim() ? 'success' : 'warning');
    }
  },

  // New: Preview task
  previewTask: function() {
    // Open preview modal or full screen
    const activeTask = document.querySelector('.task-item.active .task-simulation');
    if (activeTask) {
      // For now, just scroll and highlight
      activeTask.scrollIntoView({ behavior: 'smooth' });
      activeTask.style.border = '2px solid #0078d4';
      setTimeout(() => { activeTask.style.border = ''; }, 2000);
      ProductCommon.showToast('Xem trước task', 'info');
    }
  },

  // New: Mark task complete with screenshot
  markTaskCompleteWithScreenshot: function() {
    const activeTask = document.querySelector('.task-item.active');
    if (activeTask) {
      const taskId = activeTask.dataset.taskId;
      // Capture screenshot of work area
      this.captureScreenshot(taskId);
      // Mark complete
      ProductCommon.markTaskComplete(taskId, 'practice');
      this.updateProgress();
      ProductCommon.showToast('Task đã hoàn thành với screenshot', 'success');
    }
  },

  // New: Insert screenshot
  insertScreenshot: function() {
    // Use file input or capture screen
    const input = document.createElement('input');
    input.type = 'file';
    input.accept = 'image/*';
    input.onchange = (e) => {
      const file = e.target.files[0];
      if (file) {
        const reader = new FileReader();
        reader.onload = (event) => {
          const activeWorkArea = document.querySelector('.task-item.active #taskWorkArea-' + activeTaskId);
          if (activeWorkArea) {
            const img = document.createElement('img');
            img.src = event.target.result;
            img.style.maxWidth = '100%';
            img.style.borderRadius = '0.375rem';
            activeWorkArea.innerHTML += '<br><label>Inserted Screenshot:</label><br>';
            activeWorkArea.appendChild(img);
            this.saveAnswer(activeTaskId, 'Screenshot inserted: ' + file.name);
          }
        };
        reader.readAsDataURL(file);
      }
    };
    input.click();
  },

  // New: Show task help
  showTaskHelp: function() {
    const activeTask = document.querySelector('.task-item.active');
    if (activeTask) {
      const helpBtn = activeTask.querySelector('[data-bs-target^="#hint-"]');
      if (helpBtn) {
        const collapse = new bootstrap.Collapse(helpBtn.getAttribute('data-bs-target'), { toggle: true });
        ProductCommon.showToast('Hiển thị hỗ trợ task', 'info');
      }
    }
  },

  // New: Capture screenshot (placeholder - needs html2canvas library)
  captureScreenshot: function(taskId) {
    // Placeholder - implement with html2canvas
    console.log('Screenshot captured for task', taskId);
    // Save to localStorage or send to server
    localStorage.setItem(`screenshot_${taskId}`, 'captured');
  },

  // Save answer
  saveAnswer: function(taskId, answer) {
    const success = ProductCommon.saveAnswer(taskId, answer, 'practice');
    if (success) {
      ProductCommon.showToast('Đã lưu câu trả lời', 'success');
      
      // Mark task as completed if answer is not empty
      if (answer.trim() !== '') {
        ProductCommon.markTaskComplete(taskId, 'practice');
        this.updateProgress();
      }
    } else {
      ProductCommon.showToast('Lỗi khi lưu câu trả lời', 'error');
    }
  },

  // Save feedback
  saveFeedback: function(taskId, feedback) {
    localStorage.setItem(`practice_feedback_${taskId}`, feedback);
    ProductCommon.showToast('Đã lưu đánh giá', 'success');
  },

  // Track hint view
  trackHintView: function(taskId) {
    const hintsViewed = JSON.parse(localStorage.getItem('practice_hints_viewed') || '[]');
    if (!hintsViewed.includes(taskId)) {
      hintsViewed.push(taskId);
      localStorage.setItem('practice_hints_viewed', JSON.stringify(hintsViewed));
    }
  },

  // Track answer view
  trackAnswerView: function(taskId) {
    const answersViewed = JSON.parse(localStorage.getItem('practice_answers_viewed') || '[]');
    if (!answersViewed.includes(taskId)) {
      answersViewed.push(taskId);
      localStorage.setItem('practice_answers_viewed', JSON.stringify(answersViewed));
    }
  },

  // Toggle all hints
  toggleAllHints: function(show) {
    const hints = document.querySelectorAll('[id^="hint-"]');
    hints.forEach(hint => {
      const collapse = new bootstrap.Collapse(hint, {
        toggle: show
      });
    });
    
    ProductCommon.showToast(show ? 'Đã hiển thị tất cả gợi ý' : 'Đã ẩn tất cả gợi ý', 'info');
  },

  // Toggle all answers
  toggleAllAnswers: function(show) {
    const answers = document.querySelectorAll('[id^="answer-"]');
    answers.forEach(answer => {
      const collapse = new bootstrap.Collapse(answer, {
        toggle: show
      });
    });
    
    ProductCommon.showToast(show ? 'Đã hiển thị tất cả đáp án' : 'Đã ẩn tất cả đáp án', 'info');
  },

  // Mark current project as complete
  markCurrentProjectComplete: function() {
    const currentProject = this.session.projects[this.currentProjectIndex];
    if (!currentProject || !currentProject.Tasks) return;

    ProductCommon.confirmDialog('Đánh dấu toàn bộ project này là đã hoàn thành?').then(confirmed => {
      if (confirmed) {
        currentProject.Tasks.forEach(task => {
          ProductCommon.markTaskComplete(task.Id, 'practice');
        });
        
        // Update project completion in sidebar
        const projectItem = document.querySelector(`.practice-project-item[data-project-id="${currentProject.Id}"]`);
        if (projectItem) {
          const badge = projectItem.querySelector('.badge');
          if (badge) {
            badge.textContent = `${currentProject.Tasks.length}/${currentProject.Tasks.length}`;
            badge.classList.remove('bg-secondary');
            badge.classList.add('bg-success');
          }
        }
        
        this.updateProgress();
        ProductCommon.showToast('Đã đánh dấu project hoàn thành', 'success');
      }
    });
  },

  // Update progress
  updateProgress: function() {
    const completedTasks = ProductCommon.getCompletedTasks('practice').length;
    const totalTasks = this.session.projects.reduce((total, project) => {
      return total + (project.Tasks ? project.Tasks.length : 0);
    }, 0);
    
    const progressPercentage = ProductCommon.calculateProgress(completedTasks, totalTasks);
    
    // Update progress bar
    const progressBar = document.querySelector('.progress-bar');
    if (progressBar) {
      progressBar.style.width = `${progressPercentage}%`;
      progressBar.setAttribute('aria-valuenow', progressPercentage);
    }
    
    // Update progress text
    const progressText = document.querySelector('.text-muted');
    if (progressText && progressText.textContent.includes('Tiến độ:')) {
      progressText.textContent = `Tiến độ: ${completedTasks}/${totalTasks} task đã hoàn thành`;
    }
  },

  // Submit practice session
  submitPractice: function() {
    ProductCommon.confirmDialog('Bạn có chắc muốn nộp bài ôn luyện? Kết quả sẽ được hiển thị ngay lập tức.').then(confirmed => {
      if (confirmed) {
        // Calculate score
        const totalTasks = this.session.projects.reduce((total, project) => {
          return total + (project.Tasks ? project.Tasks.length : 0);
        }, 0);
        
        const completedTasks = ProductCommon.getCompletedTasks('practice').length;
        const score = totalTasks > 0 ? Math.round((completedTasks / totalTasks) * 100) : 0;
        
        // Show results
        if (typeof Swal !== 'undefined') {
          Swal.fire({
            title: 'Kết quả ôn luyện',
            html: `
              <div class="text-center">
                <h1 class="text-${score >= 70 ? 'success' : score >= 50 ? 'warning' : 'danger'}">${score}%</h1>
                <p>Bạn đã hoàn thành ${completedTasks}/${totalTasks} task</p>
                <div class="progress mt-3" style="height: 20px;">
                  <div class="progress-bar bg-${score >= 70 ? 'success' : score >= 50 ? 'warning' : 'danger'}" 
                       role="progressbar" style="width: ${score}%">
                    ${score}%
                  </div>
                </div>
                <p class="mt-3">${this.getScoreMessage(score)}</p>
              </div>
            `,
            icon: score >= 70 ? 'success' : score >= 50 ? 'warning' : 'error',
            confirmButtonText: 'Xem chi tiết',
            showCancelButton: true,
            cancelButtonText: 'Đóng'
          }).then((result) => {
            if (result.isConfirmed) {
              // Redirect to results page or show detailed results
              this.showDetailedResults();
            }
          });
        } else {
          alert(`Kết quả: ${score}% - Hoàn thành ${completedTasks}/${totalTasks} task`);
        }
        
        // Clear session data after submission
        setTimeout(() => {
          ProductCommon.clearSessionData('practice');
        }, 5000);
      }
    });
  },

  // Get score message
  getScoreMessage: function(score) {
    if (score >= 90) return 'Xuất sắc! Bạn đã nắm vững kiến thức.';
    if (score >= 70) return 'Tốt! Bạn đã hiểu rõ các khái niệm.';
    if (score >= 50) return 'Khá! Cần ôn tập thêm một số phần.';
    return 'Cần ôn tập lại toàn bộ bài học.';
  },

  // Show detailed results
  showDetailedResults: function() {
    // This would typically show a detailed results page
    // For now, just show an alert
    alert('Trang kết quả chi tiết sẽ hiển thị tại đây.');
  },

  // Show instructions modal on first visit
  showInstructionsModal: function() {
    const hasSeenInstructions = localStorage.getItem('practice_instructions_seen');
    if (!hasSeenInstructions) {
      const modal = new bootstrap.Modal(document.getElementById('practiceInstructionsModal'));
      modal.show();
      localStorage.setItem('practice_instructions_seen', 'true');
    }
  }
};

// Initialize practice session
function initPracticeSession() {
  PracticeSession.init();
}

// Make available globally
window.PracticeSession = PracticeSession;
window.initPracticeSession = initPracticeSession;
