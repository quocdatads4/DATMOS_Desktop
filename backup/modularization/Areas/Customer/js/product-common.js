/* Product Common JavaScript - Shared functionality for Practice and Test */

// Common utility functions
const ProductCommon = {
  // Initialize tooltips
  initTooltips: function() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function(tooltipTriggerEl) {
      return new bootstrap.Tooltip(tooltipTriggerEl);
    });
  },

  // Format seconds to HH:MM:SS
  formatTime: function(seconds) {
    const hours = Math.floor(seconds / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    const secs = seconds % 60;
    return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
  },

  // Save answer to localStorage
  saveAnswer: function(taskId, answer, sessionType = 'practice') {
    const key = `${sessionType}_answer_${taskId}`;
    try {
      localStorage.setItem(key, answer);
      return true;
    } catch (e) {
      console.error('Error saving answer:', e);
      return false;
    }
  },

  // Load answer from localStorage
  loadAnswer: function(taskId, sessionType = 'practice') {
    const key = `${sessionType}_answer_${taskId}`;
    return localStorage.getItem(key) || '';
  },

  // Show success toast
  showToast: function(message, type = 'success') {
    // Check if Toastify is available
    if (typeof Toastify === 'function') {
      Toastify({
        text: message,
        duration: 3000,
        gravity: "top",
        position: "right",
        backgroundColor: type === 'success' ? "#28c76f" : 
                        type === 'error' ? "#ea5455" : 
                        type === 'warning' ? "#ff9f43" : "#7367f0",
        stopOnFocus: true
      }).showToast();
    } else {
      // Fallback to alert
      alert(message);
    }
  },

  // Confirm dialog
  confirmDialog: function(message, title = 'Xác nhận') {
    return new Promise((resolve) => {
      if (typeof Swal !== 'undefined') {
        Swal.fire({
          title: title,
          text: message,
          icon: 'question',
          showCancelButton: true,
          confirmButtonText: 'Đồng ý',
          cancelButtonText: 'Hủy'
        }).then((result) => {
          resolve(result.isConfirmed);
        });
      } else {
        resolve(confirm(message));
      }
    });
  },

  // Calculate progress percentage
  calculateProgress: function(completed, total) {
    return total > 0 ? Math.round((completed / total) * 100) : 0;
  },

  // Navigate between tasks
  navigateTask: function(direction, currentIndex, totalTasks) {
    let newIndex = currentIndex + direction;
    
    if (newIndex < 0) newIndex = 0;
    if (newIndex >= totalTasks) newIndex = totalTasks - 1;
    
    return newIndex;
  },

  // Navigate between projects
  navigateProject: function(direction, currentIndex, totalProjects) {
    let newIndex = currentIndex + direction;
    
    if (newIndex < 0) newIndex = 0;
    if (newIndex >= totalProjects) newIndex = totalProjects - 1;
    
    return newIndex;
  },

  // Mark task as complete
  markTaskComplete: function(taskId, sessionType = 'practice') {
    const key = `${sessionType}_completed_${taskId}`;
    localStorage.setItem(key, 'true');
    
    // Update UI
    const taskIndicator = document.querySelector(`.task-indicator[data-task-id="${taskId}"]`);
    if (taskIndicator) {
      taskIndicator.classList.remove('btn-outline-primary', 'btn-outline-danger');
      taskIndicator.classList.add('btn-success');
      taskIndicator.innerHTML = `<i class="icon-base ti ti-check"></i>`;
    }
  },

  // Check if task is completed
  isTaskCompleted: function(taskId, sessionType = 'practice') {
    const key = `${sessionType}_completed_${taskId}`;
    return localStorage.getItem(key) === 'true';
  },

  // Get all completed tasks
  getCompletedTasks: function(sessionType = 'practice') {
    const completed = [];
    for (let i = 0; i < localStorage.length; i++) {
      const key = localStorage.key(i);
      if (key.startsWith(`${sessionType}_completed_`) && localStorage.getItem(key) === 'true') {
        const taskId = key.replace(`${sessionType}_completed_`, '');
        completed.push(parseInt(taskId));
      }
    }
    return completed;
  },

  // Clear session data
  clearSessionData: function(sessionType = 'practice') {
    const keysToRemove = [];
    for (let i = 0; i < localStorage.length; i++) {
      const key = localStorage.key(i);
      if (key.startsWith(`${sessionType}_`)) {
        keysToRemove.push(key);
      }
    }
    
    keysToRemove.forEach(key => {
      localStorage.removeItem(key);
    });
    
    return keysToRemove.length;
  },

  // Initialize answer auto-save
  initAutoSave: function(textareaSelector, sessionType = 'practice') {
    const textareas = document.querySelectorAll(textareaSelector);
    
    textareas.forEach(textarea => {
      const taskId = textarea.dataset.taskId;
      
      // Load saved answer
      const savedAnswer = this.loadAnswer(taskId, sessionType);
      if (savedAnswer) {
        textarea.value = savedAnswer;
      }
      
      // Auto-save on input
      let saveTimeout;
      textarea.addEventListener('input', (e) => {
        clearTimeout(saveTimeout);
        saveTimeout = setTimeout(() => {
          const success = this.saveAnswer(taskId, e.target.value, sessionType);
          if (success) {
            // Show subtle feedback
            const saveBtn = document.querySelector(`.save-${sessionType}-answer-btn[data-task-id="${taskId}"]`);
            if (saveBtn) {
              const originalText = saveBtn.innerHTML;
              saveBtn.innerHTML = '<i class="icon-base ti ti-check me-1"></i> Đã lưu';
              saveBtn.classList.add('btn-success');
              saveBtn.classList.remove('btn-outline-info', 'btn-outline-danger');
              
              setTimeout(() => {
                saveBtn.innerHTML = originalText;
                saveBtn.classList.remove('btn-success');
                saveBtn.classList.add(sessionType === 'practice' ? 'btn-outline-info' : 'btn-outline-danger');
              }, 2000);
            }
          }
        }, 1000);
      });
    });
  },

  // Initialize task navigation
  initTaskNavigation: function() {
    const taskItems = document.querySelectorAll('.task-item');
    const taskIndicators = document.querySelectorAll('.task-indicator');
    const prevBtn = document.getElementById('prevTaskBtn') || document.getElementById('testPrevTaskBtn');
    const nextBtn = document.getElementById('nextTaskBtn') || document.getElementById('testNextTaskBtn');
    
    if (taskItems.length === 0) return;
    
    let currentTaskIndex = 0;
    
    // Show first task by default
    taskItems.forEach((item, index) => {
      if (index === 0) {
        item.classList.add('active');
      } else {
        item.classList.remove('active');
      }
    });
    
    // Update indicator
    const updateIndicators = () => {
      taskIndicators.forEach((indicator, index) => {
        if (index === currentTaskIndex) {
          indicator.classList.remove('btn-outline-primary', 'btn-outline-danger');
          indicator.classList.add(document.querySelector('.task-item.active')?.dataset.taskId ? 'btn-primary' : 'btn-danger');
        } else {
          indicator.classList.remove('btn-primary', 'btn-danger');
          indicator.classList.add(document.querySelector('.task-item.active')?.dataset.taskId ? 'btn-outline-primary' : 'btn-outline-danger');
        }
      });
    };
    
    // Task indicator click
    taskIndicators.forEach((indicator, index) => {
      indicator.addEventListener('click', () => {
        currentTaskIndex = index;
        
        taskItems.forEach((item, i) => {
          if (i === currentTaskIndex) {
            item.classList.add('active');
          } else {
            item.classList.remove('active');
          }
        });
        
        updateIndicators();
        
        // Scroll to task
        taskItems[currentTaskIndex].scrollIntoView({ behavior: 'smooth', block: 'start' });
      });
    });
    
    // Previous button
    if (prevBtn) {
      prevBtn.addEventListener('click', () => {
        if (currentTaskIndex > 0) {
          currentTaskIndex--;
          
          taskItems.forEach((item, i) => {
            if (i === currentTaskIndex) {
              item.classList.add('active');
            } else {
              item.classList.remove('active');
            }
          });
          
          updateIndicators();
          taskItems[currentTaskIndex].scrollIntoView({ behavior: 'smooth', block: 'start' });
        }
      });
    }
    
    // Next button
    if (nextBtn) {
      nextBtn.addEventListener('click', () => {
        if (currentTaskIndex < taskItems.length - 1) {
          currentTaskIndex++;
          
          taskItems.forEach((item, i) => {
            if (i === currentTaskIndex) {
              item.classList.add('active');
            } else {
              item.classList.remove('active');
            }
          });
          
          updateIndicators();
          taskItems[currentTaskIndex].scrollIntoView({ behavior: 'smooth', block: 'start' });
        }
      });
    }
    
    updateIndicators();
  }
};

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
  ProductCommon.initTooltips();
  
  // Auto-save for practice answers
  if (document.querySelector('.answer-input')) {
    ProductCommon.initAutoSave('.answer-input', 'practice');
  }
  
  // Auto-save for test answers
  if (document.querySelector('.test-answer-input')) {
    ProductCommon.initAutoSave('.test-answer-input', 'test');
  }
  
  // Initialize task navigation
  ProductCommon.initTaskNavigation();
});

// Export for use in other modules
if (typeof module !== 'undefined' && module.exports) {
  module.exports = ProductCommon;
} else {
  window.ProductCommon = ProductCommon;
}
