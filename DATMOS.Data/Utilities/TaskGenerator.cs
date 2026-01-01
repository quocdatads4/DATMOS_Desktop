using DATMOS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DATMOS.Data.Utilities
{
    public class TaskGenerator
    {
        private readonly List<string> _wordTaskTemplates = new()
        {
            "Định dạng {element} thành {format}",
            "Chèn {object} vào vị trí {location}",
            "Thay đổi {property} của {element} thành {value}",
            "Áp dụng {style} cho {element}",
            "Tạo {object} mới với {parameters}"
        };

        private readonly List<string> _excelTaskTemplates = new()
        {
            "Tạo công thức tính {calculation}",
            "Áp dụng định dạng có điều kiện cho {range}",
            "Tạo biểu đồ {chartType} cho dữ liệu {data}",
            "Sử dụng hàm {function} để {action}",
            "Thiết lập {setting} cho {element}"
        };

        private readonly List<string> _powerPointTaskTemplates = new()
        {
            "Thêm hiệu ứng {effect} cho {element}",
            "Chỉnh sửa bố cục {layout} của slide",
            "Chèn {mediaType} vào slide {slideNumber}",
            "Áp dụng theme {themeName}",
            "Thiết lập chuyển tiếp {transition} giữa các slide"
        };

        private readonly Random _random = new();

        public List<ExamTask> GenerateTasksForProject(int projectId, string subjectType, int numberOfTasks = 5)
        {
            var tasks = new List<ExamTask>();
            var templates = GetTemplatesBySubject(subjectType);
            
            for (int i = 1; i <= numberOfTasks; i++)
            {
                var template = templates[_random.Next(templates.Count)];
                var taskName = GenerateTaskName(template, i);
                
                tasks.Add(new ExamTask
                {
                    ExamProjectId = projectId,
                    Name = taskName,
                    Description = GenerateDescription(taskName, subjectType),
                    Instructions = GenerateInstructions(taskName),
                    OrderIndex = i,
                    MaxScore = Math.Round(100.0 / 6.0, 2),
                    TaskType = InferTaskType(taskName),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }
            
            // Add the mandatory "Save & Close" task
            tasks.Add(new ExamTask
            {
                ExamProjectId = projectId,
                Name = "Task 6: Save & Close",
                Description = "Lưu tài liệu và đóng cửa sổ làm việc.",
                Instructions = "Bước 1: Nhấn vào biểu tượng Save trên thanh Quick Access Toolbar.\nBước 2: Vào File > Close để đóng tài liệu.",
                OrderIndex = 6,
                MaxScore = Math.Round(100.0 / 6.0, 2),
                TaskType = "File",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
            
            return tasks;
        }

        private List<string> GetTemplatesBySubject(string subjectType)
        {
            return subjectType.ToLower() switch
            {
                "word" => _wordTaskTemplates,
                "excel" => _excelTaskTemplates,
                "powerpoint" => _powerPointTaskTemplates,
                _ => _wordTaskTemplates
            };
        }

        private string GenerateTaskName(string template, int taskNumber)
        {
            var elements = new[] { "văn bản", "bảng", "hình ảnh", "biểu đồ", "tiêu đề" };
            var formats = new[] { "in đậm", "màu đỏ", "cỡ chữ 14", "căn giữa", "gạch chân" };
            var objects = new[] { "hình ảnh", "bảng", "biểu đồ", "SmartArt", "Text Box" };
            var locations = new[] { "đầu trang", "cuối trang", "bên trái", "bên phải", "giữa" };
            var properties = new[] { "màu sắc", "kích thước", "kiểu chữ", "căn lề", "độ dày" };
            var values = new[] { "xanh dương", "16px", "Arial", "giữa", "2pt" };
            var styles = new[] { "Heading 1", "Normal", "Quote", "Title", "Subtitle" };
            var parameters = new[] { "kiểu Modern", "màu sắc tươi sáng", "kích thước vừa", "định dạng chuẩn", "bố cục cân đối" };
            var calculations = new[] { "tổng doanh thu", "trung bình điểm", "phần trăm tăng trưởng", "lợi nhuận", "chi phí" };
            var ranges = new[] { "A1:D10", "B5:F20", "C2:C50", "D1:D100", "E5:E30" };
            var chartTypes = new[] { "cột", "đường", "tròn", "thanh", "phân tán" };
            var data = new[] { "doanh số", "ngân sách", "hiệu suất", "thống kê", "phân tích" };
            var functions = new[] { "SUM", "AVERAGE", "IF", "VLOOKUP", "COUNTIF" };
            var actions = new[] { "tính tổng", "tìm trung bình", "kiểm tra điều kiện", "tìm kiếm", "đếm" };
            var settings = new[] { "định dạng", "bảo vệ", "công thức", "tham chiếu", "tính năng" };
            var effects = new[] { "xuất hiện", "biến mất", "di chuyển", "phóng to", "thu nhỏ" };
            var layouts = new[] { "tiêu đề và nội dung", "hai cột", "so sánh", "trống", "ảnh với chú thích" };
            var mediaTypes = new[] { "hình ảnh", "video", "âm thanh", "biểu đồ", "bảng" };
            var slideNumbers = new[] { "1", "2", "3", "4", "5" };
            var themeNames = new[] { "Integral", "Facet", "Ion", "Retrospect", "Slice" };
            var transitions = new[] { "Fade", "Push", "Wipe", "Split", "Cover" };
            
            var taskName = template
                .Replace("{element}", elements[_random.Next(elements.Length)])
                .Replace("{format}", formats[_random.Next(formats.Length)])
                .Replace("{object}", objects[_random.Next(objects.Length)])
                .Replace("{location}", locations[_random.Next(locations.Length)])
                .Replace("{property}", properties[_random.Next(properties.Length)])
                .Replace("{value}", values[_random.Next(values.Length)])
                .Replace("{style}", styles[_random.Next(styles.Length)])
                .Replace("{parameters}", parameters[_random.Next(parameters.Length)])
                .Replace("{calculation}", calculations[_random.Next(calculations.Length)])
                .Replace("{range}", ranges[_random.Next(ranges.Length)])
                .Replace("{chartType}", chartTypes[_random.Next(chartTypes.Length)])
                .Replace("{data}", data[_random.Next(data.Length)])
                .Replace("{function}", functions[_random.Next(functions.Length)])
                .Replace("{action}", actions[_random.Next(actions.Length)])
                .Replace("{setting}", settings[_random.Next(settings.Length)])
                .Replace("{effect}", effects[_random.Next(effects.Length)])
                .Replace("{layout}", layouts[_random.Next(layouts.Length)])
                .Replace("{mediaType}", mediaTypes[_random.Next(mediaTypes.Length)])
                .Replace("{slideNumber}", slideNumbers[_random.Next(slideNumbers.Length)])
                .Replace("{themeName}", themeNames[_random.Next(themeNames.Length)])
                .Replace("{transition}", transitions[_random.Next(transitions.Length)]);
            
            return $"Task {taskNumber}: {taskName}";
        }

        private string GenerateDescription(string taskName, string subjectType)
        {
            return $"Thực hiện yêu cầu: {taskName}. Đây là bài tập thực hành cho {subjectType}.";
        }

        private string GenerateInstructions(string taskName)
        {
            return $"Bước 1: Mở tài liệu cần chỉnh sửa.\nBước 2: Tìm đối tượng cần xử lý.\nBước 3: Thực hiện thao tác theo yêu cầu: {taskName}.\nBước 4: Kiểm tra kết quả.";
        }

        private string InferTaskType(string taskName)
        {
            if (taskName.Contains("định dạng") || taskName.Contains("format")) return "Format";
            if (taskName.Contains("chèn") || taskName.Contains("insert")) return "Insert";
            if (taskName.Contains("bảng") || taskName.Contains("table")) return "Table";
            if (taskName.Contains("biểu đồ") || taskName.Contains("chart")) return "Chart";
            if (taskName.Contains("công thức") || taskName.Contains("formula")) return "Formula";
            if (taskName.Contains("hiệu ứng") || taskName.Contains("effect")) return "Effect";
            if (taskName.Contains("theme") || taskName.Contains("chủ đề")) return "Theme";
            if (taskName.Contains("chuyển tiếp") || taskName.Contains("transition")) return "Transition";
            return "General";
        }

        public int CalculateMissingTasks(int totalProjects, int projectsWithTasks)
        {
            int projectsWithoutTasks = totalProjects - projectsWithTasks;
            return projectsWithoutTasks * 6; // 6 tasks per project
        }

        public List<ExamTask> GenerateMissingTasks(List<int> projectIds, string subjectType)
        {
            var allTasks = new List<ExamTask>();
            
            foreach (var projectId in projectIds)
            {
                var tasks = GenerateTasksForProject(projectId, subjectType);
                allTasks.AddRange(tasks);
            }
            
            return allTasks;
        }
    }
}
