/* Product Test JavaScript - Test mode functionality */

// Test session management
const TestSession = {
  // Initialize test session
  init: function() {
    if (!window.testSession) {
      console.error('Test session data not found');
      return;
    }

    this.session = window.testSession;
    this.currentProjectIndex = this.getCurrentProjectIndex();
    this.markedTasks = new Set(JSON.parse(localStorage.getItem('test_marked_tasks') || '[]'));
    this.initEventListeners();
    this.loadSavedData();
    this.updateProgress();
    this.showInstructionsModal();
    this.startAutoSave();
  },

  // Get current project index
  getCurrentProjectIndex: function() {
    return this.session.projects.findIndex(p => p.Id === this.session.currentProjectId);
  },

  // Initialize event listeners
  initEventListeners: function() {
    // Project navigation
    document.querySelectorAll('.test-project-item').forEach(item => {
      item.addEventListener('click', (e) => {
        e.preventDefault();
        const projectId = parseInt(item.dataset.projectId);
        this.switchProject(projectId);
      });
    });

    // Previous/Next project buttons
    const prevProjectBtn = document.getElementById('testPrevProjectBtn');
    const nextProjectBtn = document.getElementById('testNextProjectBtn');
    
    if (prevProjectBtn) {
      prevProjectBtn.addEventListener('click', () => {
        if (this.currentProjectIndex > 0) {
          const prevProject = this.session.projects[this.currentProjectIndex - 1];
          this.switchProject(prevProject.Id);
        }
      });
    }

    if (nextProjectBtn) {
      nextProjectBtn.addEventListener('click', () => {
        if (this.currentProjectIndex < this.session.projects.length - 1) {
          const nextProject = this.session.projects[this.currentProjectIndex + 1];
          this.switchProject(nextProject.Id);
        }
      });
    }

    // Save answer buttons
    document.querySelectorAll('.save-test-answer-btn').forEach(btn => {
      btn.addEventListener('click', (e) => {
        e.preventDefault();
        const taskId = parseInt(btn.dataset.taskId);
        const answerInput = document.querySelector(`.test-answer-input[data-task-id="${taskId}"]`);
        if (answerInput) {
          this.saveAnswer(taskId, answerInput.value);
        }
      });
    });

    // Mark for review buttons
    document.querySelectorAll('.mark-for-review-btn').forEach(btn => {
      btn.addEventListener('click', (e) => {
        e.preventDefault();
        const taskId = parseInt(btn.dataset.taskId);
        this.toggleMarkForReview(taskId);
      });
    });

    // Mark complete button
    const markCompleteBtn = document.getElementById('testMarkCompleteBtn');
    if (markCompleteBtn) {
      markCompleteBtn.addEventListener('click', () => {
        this.markCurrentProjectComplete();
      });
    }

    // Show marked tasks button
    const showMarkedBtn = document.getElementById('showMarkedBtn');
    if (showMarkedBtn) {
      showMarkedBtn.addEventListener('click', () => {
        this.showMarkedTasks();
      });
    }

    // Submit test buttons
    const submitTestBtn = document.getElementById('submitTestBtn');
    const finalSubmitBtn = document.getElementById('finalSubmitBtn');
    
    if (submitTestBtn) {
      submitTestBtn.addEventListener('click', () => {
        this.confirmSubmit();
      });
    }
    
    if (finalSubmitBtn) {
      finalSubmitBtn.addEventListener('click', () => {
        this.confirmSubmit();
      });
    }

    // Window beforeunload - warn about unsaved changes
    window.addEventListener('beforeunload', (e) => {
      if (this.hasUnsavedChanges()) {
        e.preventDefault();
        e.returnValue = 'Bạn có câu trả lời chưa lưu. Bạn có chắc muốn rời đi?';
      }
    });

    // Handle test timer expiration
    window.addEventListener('testTimeExpired', () => {
      this.autoSubmit();
    });
  },

  // Load saved data from localStorage
  loadSavedData: function() {
    // Load saved answers
    document.querySelectorAll('.test-answer-input').forEach(input => {
      const taskId = parseInt(input.dataset.taskId);
      const savedAnswer = ProductCommon.loadAnswer(taskId, 'test');
      if (savedAnswer) {
        input.value = savedAnswer;
      }
    });

    // Load marked tasks
    this.markedTasks.forEach(taskId => {
      this.updateMarkedTaskUI(taskId, true);
    });

    // Load completed tasks
    const completedTasks = ProductCommon.getCompletedTasks('test');
    completedTasks.forEach(taskId => {
      ProductCommon.markTaskComplete(taskId, 'test');
    });
  },

  // Switch to different project
  switchProject: function(projectId) {
    // Save current answers before switching
    this.saveAllCurrentAnswers();

    // Update URL without page reload
    const newUrl = `${window.location.pathname}?projectId=${projectId}`;
    window.history.pushState({ projectId }, '', newUrl);

    // Update current project
    this.session.currentProjectId = projectId;
    this.currentProjectIndex = this.getCurrentProjectIndex();

    // Update UI
    this.updateProjectUI();
    this.updateProgress();

    // Scroll to top
    window.scrollTo({ top: 0, behavior: 'smooth' });

    ProductCommon.showToast(`Đã chuyển đến project mới`, 'info');
  },

  // Update project UI
  updateProjectUI: function() {
    // Update active project in sidebar
    document.querySelectorAll('.test-project-item').forEach(item => {
      const itemProjectId = parseInt(item.dataset.projectId);
      if (itemProjectId === this.session.currentProjectId) {
        item.classList.add('active');
        
        // Update avatar color
        const avatar = item.querySelector('.avatar-initial');
        if (avatar) {
          avatar.classList.remove('bg-label-secondary');
          avatar.classList.add('bg-label-danger');
        }
      } else {
        item.classList.remove('active');
        
        // Update avatar color
        const avatar = item.querySelector('.avatar-initial');
        if (avatar) {
          avatar.classList.remove('bg-label-danger');
          avatar.classList.add('bg-label-secondary');
        }
      }
    });

    // Update project badge
    const projectBadge = document.querySelector('.badge.bg-label-danger');
    if (projectBadge) {
      projectBadge.textContent = `Project ${this.currentProjectIndex + 1}/${this.session.projects.length}`;
    }

    // Update project title and description
    const currentProject = this.session.projects[this.currentProjectIndex];
    if (currentProject) {
      const titleElement = document.querySelector('.card-title.m-0');
      const descElement = document.querySelector('.text-muted.mb-0');
      
      if (titleElement) {
        titleElement.textContent = currentProject.Name;
      }
      if (descElement) {
        descElement.textContent = currentProject.Description;
      }
    }
  },

  // Save answer
  saveAnswer: function(taskId, answer) {
    const success = ProductCommon.saveAnswer(taskId, answer, 'test');
    if (success) {
      ProductCommon.showToast('Đã lưu câu trả lời', 'success');
      
      // Mark task as completed if answer is not empty
      if (answer.trim() !== '') {
        ProductCommon.markTaskComplete(taskId, 'test');
        this.updateProgress();
      }
    } else {
      ProductCommon.showToast('Lỗi khi lưu câu trả lời', 'error');
    }
  },

  // Save all current answers
  saveAllCurrentAnswers: function() {
    const currentProject = this.session.projects[this.currentProjectIndex];
    if (!currentProject || !currentProject.Tasks) return;

    let savedCount = 0;
    currentProject.Tasks.forEach(task => {
      const answerInput = document.querySelector(`.test-answer-input[data-task-id="${task.Id}"]`);
      if (answerInput && answerInput.value.trim() !== '') {
        if (ProductCommon.saveAnswer(task.Id, answerInput.value, 'test')) {
          savedCount++;
        }
      }
    });

    if (savedCount > 0) {
      console.log(`Đã lưu ${savedCount} câu trả lời`);
    }
  },

  // Toggle mark for review
  toggleMarkForReview: function(taskId) {
    if (this.markedTasks.has(taskId)) {
      this.markedTasks.delete(taskId);
      this.updateMarkedTaskUI(taskId, false);
      ProductCommon.showToast('Đã bỏ đánh dấu xem lại', 'info');
    } else {
      this.markedTasks.add(taskId);
      this.updateMarkedTaskUI(taskId, true);
      ProductCommon.showToast('Đã đánh dấu để xem lại', 'warning');
    }
    
    // Save to localStorage
    localStorage.setItem('test_marked_tasks', JSON.stringify(Array.from(this.markedTasks)));
  },

  // Update marked task UI
  updateMarkedTaskUI: function(taskId, isMarked) {
    const flagElement = document.getElementById(`review-flag-${taskId}`);
    if (flagElement) {
      if (isMarked) {
        flagElement.classList.remove('d-none');
      } else {
        flagElement.classList.add('d-none');
      }
    }
    
    // Update button text
    const markBtn = document.querySelector(`.mark-for-review-btn[data-task-id="${taskId}"]`);
    if (markBtn) {
      if (isMarked) {
        markBtn.innerHTML = '<i class="icon-base ti ti-flag-off me-1"></i> Bỏ đánh dấu';
        markBtn.classList.remove('btn-outline-secondary');
        markBtn.classList.add('btn-warning');
      } else {
        markBtn.innerHTML = '<i class="icon-base ti ti-flag me-1"></i> Đánh dấu để xem lại';
        markBtn.classList.remove('btn-warning');
        markBtn.classList.add('btn-outline-secondary');
      }
    }
  },

  // Mark current project as complete
  markCurrentProjectComplete: function() {
    const currentProject = this.session.projects[this.currentProjectIndex];
    if (!currentProject || !currentProject.Tasks) return;

    ProductCommon.confirmDialog('Đánh dấu toàn bộ project này là đã hoàn thành?').then(confirmed => {
      if (confirmed) {
        currentProject.Tasks.forEach(task => {
          ProductCommon.markTaskComplete(task.Id, 'test');
        });
        
        // Update project completion in sidebar
        const projectItem = document.querySelector(`.test-project-item[data-project-id="${currentProject.Id}"]`);
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

  // Show marked tasks
  showMarkedTasks: function() {
    if (this.markedTasks.size === 0) {
      ProductCommon.showToast('Bạn chưa đánh dấu task nào để xem lại', 'info');
      return;
    }

    let message = `Bạn đã đánh dấu ${this.markedTasks.size} task để xem lại:\n\n`;
    
    this.session.projects.forEach((project, projectIndex) => {
      if (project.Tasks) {
        project.Tasks.forEach(task => {
          if (this.markedTasks.has(task.Id)) {
            message += `• Project ${projectIndex + 1}, Task ${task.Order}\n`;
          }
        });
      }
    });

    message += '\nNhấn vào task indicator để chuyển đến task đã đánh dấu.';
    
    if (typeof Swal !== 'undefined') {
      Swal.fire({
        title: 'Các task đã đánh dấu',
        text: message,
        icon: 'info',
        confirmButtonText: 'Đóng'
      });
    } else {
      alert(message);
    }
  },

  // Update progress
  updateProgress: function() {
    const completedTasks = ProductCommon.getCompletedTasks('test').length;
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

  // Check for unsaved changes
  hasUnsavedChanges: function() {
    let hasUnsaved = false;
    
    document.querySelectorAll('.test-answer-input').forEach(input => {
      const taskId = parseInt(input.dataset.taskId);
      const savedAnswer = ProductCommon.loadAnswer(taskId, 'test');
      if (input.value !== savedAnswer) {
        hasUnsaved = true;
      }
    });
    
    return hasUnsaved;
  },

  // Start auto-save interval
  startAutoSave: function() {
    // Auto-save every 30 seconds
    this.autoSaveInterval = setInterval(() => {
      this.saveAllCurrentAnswers();
      console.log('Auto-saved test answers');
    }, 30000);
  },

  // Confirm submission
  confirmSubmit: function() {
    const unansweredTasks = this.getUnansweredTasksCount();
    
    let message = 'Bạn có chắc muốn nộp bài thi?';
    if (unansweredTasks > 0) {
      message += `\n\nCòn ${unansweredTasks} task chưa có câu trả lời.`;
    }
    
    ProductCommon.confirmDialog(message, 'Nộp bài thi').then(confirmed => {
      if (confirmed) {
        this.submitTest();
      }
    });
  },

  // Get unanswered tasks count
  getUnansweredTasksCount: function() {
    let unanswered = 0;
    
    this.session.projects.forEach(project => {
      if (project.Tasks) {
        project.Tasks.forEach(task => {
          const savedAnswer = ProductCommon.loadAnswer(task.Id, 'test');
          if (!savedAnswer || savedAnswer.trim() === '') {
            unanswered++;
          }
        });
      }
    });
    
    return unanswered;
  },

  // Submit test
  submitTest: function() {
    // Clear auto-save interval
    if (this.autoSaveInterval) {
      clearInterval(this.autoSaveInterval);
    }
    
    // Clear test timer interval
    if (window.testTimerInterval) {
      clearInterval(window.testTimerInterval);
    }
    
    // Calculate score (mock - in real app this would be server-side)
    const totalTasks = this.session.projects.reduce((total, project) => {
      return total + (project.Tasks ? project.Tasks.length : 0);
    }, 0);
    
    const completedTasks = ProductCommon.getCompletedTasks('test').length;
    const score = totalTasks > 0 ? Math.round((completedTasks / totalTasks) * 100) : 0;
    
    // Adjust score based on answer quality (mock)
    const adjustedScore = Math.min(100, score + Math.floor(Math.random() * 20));
    
    // Show results
    if (typeof Swal !== 'undefined') {
      Swal.fire({
        title: 'Kết quả thi thử',
        html: `
          <div class="text-center">
            <h1 class="text-${adjustedScore >= 70 ? 'success' : adjustedScore >= 50 ? 'warning' : 'danger'}">${adjustedScore}%</h1>
            <p>Điểm số của bạn</p>
            
            <div class="row mt-4">
              <div class="col-6">
                <div class="card border-info">
                  <div class="card-body">
                    <h5 class="text-info">${completedTasks}</h5>
                    <small>Task đã hoàn thành</small>
                  </div>
                </div>
              </div>
              <div class="col-6">
                <div class="card border-warning">
                  <div class="card-body">
                    <h5 class="text-warning">${this.markedTasks.size}</h5>
                    <small>Task đánh dấu xem lại</small>
                  </div>
                </div>
              </div>
            </div>
            
            <div class="progress mt-4" style="height: 20px;">
              <div class="progress-bar bg-${adjustedScore >= 70 ? 'success' : adjustedScore >= 50 ? 'warning' : 'danger'}" 
                   role="progressbar" style="width: ${adjustedScore}%">
                ${adjustedScore}%
              </div>
            </div>
            
            <p class="mt-3">${this.getScoreMessage(adjustedScore)}</p>
            
            <div class="mt-4">
              <button class="btn btn-outline-primary" onclick="TestSession.showDetailedResults()">
                <i class="icon-base ti ti-eye me-1"></i>
                Xem đáp án chi tiết
              </button>
            </div>
          </div>
        `,
        icon: adjustedScore >= 70 ? 'success' : adjustedScore >= 50 ? 'warning' : 'error',
        confirmButtonText: 'Hoàn thành',
        showCancelButton: true,
        cancelButtonText: 'Quay lại'
      }).then((result) => {
        if (result.isConfirmed) {
          // Clear session data
          ProductCommon.clearSessionData('test');
          localStorage.removeItem('test_marked_tasks');
          localStorage.removeItem('test_instructions_seen');
          
          // Redirect to product details page
          setTimeout(() => {
            window.location.href = `/Customer/Product/Details/${this.session.productId}`;
          }, 2000);
        }
      });
    } else {
      alert(`Kết quả: ${adjustedScore}% - Hoàn thành ${completedTasks}/${totalTasks} task`);
      
      // Clear session data
      ProductCommon.clearSessionData('test');
      localStorage.removeItem('test_marked_tasks');
      localStorage.removeItem('test_instructions_seen');
    }
  },

  // Auto submit when time expires
  autoSubmit: function() {
    ProductCommon.showToast('Thời gian làm bài đã hết! Bài thi sẽ tự động nộp.', 'warning');
    setTimeout(() => {
      this.submitTest();
    }, 3000);
  },

  // Get score message
  getScoreMessage: function(score) {
    if (score >= 90) return 'Xuất sắc! Bạn đã sẵn sàng cho kỳ thi thật.';
    if (score >= 70) return 'Tốt! Bạn cần ôn tập thêm một chút.';
    if (score >= 50) return 'Đạt yêu cầu! Cần ôn tập nhiều hơn.';
    return 'Cần ôn tập lại toàn bộ bài học.';
  },

  // Show detailed results
  showDetailedResults: function() {
    // This would typically show a detailed results page
    // For now, just show an alert with more details
    const completedTasks = ProductCommon.getCompletedTasks('test').length;
    const totalTasks = this.session.projects.reduce((total, project) => {
      return total + (project.Tasks ? project.Tasks.length : 0);
    }, 0);
    
    let message = `Kết quả chi tiết:\n\n`;
    message += `• Tổng số task: ${totalTasks}\n`;
    message += `• Task đã hoàn thành: ${completedTasks}\n`;
    message += `• Task đánh dấu xem lại: ${this.markedTasks.size}\n`;
    message += `• Tỷ lệ hoàn thành: ${Math.round((completedTasks / totalTasks) * 100)}%\n\n`;
    message += `Đáp án chi tiết sẽ được hiển thị trong phiên bản đầy đủ.`;
    
    alert(message);
  },

  // Show instructions modal on first visit
  showInstructionsModal: function() {
    const hasSeenInstructions = localStorage.getItem('test_instructions_seen');
    if (!hasSeenInstructions) {
      const modal = new bootstrap.Modal(document.getElementById('testInstructionsModal'));
      modal.show();
      localStorage.setItem('test_instructions_seen', 'true');
    }
  }
};

// Initialize test session
function initTestSession() {
  TestSession.init();
}

// Make available globally
window.TestSession = TestSession;
window.initTestSession = initTestSession;
