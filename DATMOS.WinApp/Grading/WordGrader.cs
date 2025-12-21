using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DATMOS.WinApp.Grading
{
    /// <summary>
    /// Class for automatically grading MOS Word 2019 documents
    /// </summary>
    public class WordGrader
    {
        public class GradingResult
        {
            public int TotalScore { get; set; }
            public int MaxScore { get; set; }
            public List<GradingItem> Items { get; set; } = new();
            public bool Passed { get; set; }
            public string Summary { get; set; }
        }

        public class GradingItem
        {
            public string Description { get; set; }
            public int Score { get; set; }
            public int MaxScore { get; set; }
            public bool IsCorrect { get; set; }
            public string Feedback { get; set; }
        }

        /// <summary>
        /// Grade a Word document based on project requirements
        /// </summary>
        public GradingResult GradeDocument(string filePath, string projectId)
        {
            var result = new GradingResult();
            
            try
            {
                using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, false))
                {
                    var body = doc.MainDocumentPart?.Document?.Body;
                    if (body == null)
                    {
                        result.Items.Add(new GradingItem
                        {
                            Description = "File không hợp lệ",
                            Score = 0,
                            MaxScore = 0,
                            IsCorrect = false,
                            Feedback = "Không thể đọc nội dung file Word"
                        });
                        return result;
                    }
                    
                    // Grade based on project ID
                    switch (projectId)
                    {
                        case "1": // Bicycles project
                            GradeBicyclesProject(body, result);
                            break;
                        case "2": // DNetworking project
                            GradeDNetworkingProject(body, result);
                            break;
                        default:
                            GradeBicyclesProject(body, result); // Default to Project 1
                            break;
                    }
                }
                
                // Calculate total score
                result.TotalScore = result.Items.Sum(i => i.Score);
                result.MaxScore = result.Items.Sum(i => i.MaxScore);
                result.Passed = result.TotalScore >= (result.MaxScore * 0.7); // 70% to pass
                
                // Generate summary
                result.Summary = GenerateSummary(result);
            }
            catch (Exception ex)
            {
                result.Items.Add(new GradingItem
                {
                    Description = "Lỗi khi đọc file",
                    Score = 0,
                    MaxScore = 0,
                    IsCorrect = false,
                    Feedback = $"Lỗi: {ex.Message}"
                });
            }
            
            return result;
        }

        /// <summary>
        /// Grade Project 1: Bicycles.docx
        /// </summary>
        private void GradeBicyclesProject(Body body, GradingResult result)
        {
            string fullText = GetFullText(body);
            
            // 1. Check main title "Bellows Bicycle Barn" with Title style
            CheckTitleStyle(body, result, "Bellows Bicycle Barn");
            
            // 2. Check bullet list for "Bicycle Advantages" (5 items)
            CheckBulletList(body, result, "Bicycle Advantages", 5);
            
            // 3. Check table "Rental Prices" with 2 columns and 3 rows
            CheckRentalPricesTable(body, result);
            
            // 4. Check table style "Grid Table 4 - Accent 1"
            CheckTableStyle(body, result);
            
            // 5. Check table alignment and caption
            CheckTableAlignmentAndCaption(body, result);
            
            // 6. Check formatting (bold/italic)
            CheckFormatting(body, result);
            
            // 7. Check required content
            CheckRequiredContent(fullText, result);
        }

        /// <summary>
        /// Grade Project 2: DNetworking.docx
        /// </summary>
        private void GradeDNetworkingProject(Body body, GradingResult result)
        {
            string fullText = GetFullText(body);
            
            // Basic checks for Project 2
            result.Items.Add(new GradingItem
            {
                Description = "Tiêu đề chính",
                Score = CheckTextExists(fullText, "DNetworking") ? 10 : 0,
                MaxScore = 10,
                IsCorrect = CheckTextExists(fullText, "DNetworking"),
                Feedback = CheckTextExists(fullText, "DNetworking") ? "✓ Có tiêu đề DNetworking" : "✗ Thiếu tiêu đề DNetworking"
            });
            
            result.Items.Add(new GradingItem
            {
                Description = "Nội dung mạng máy tính",
                Score = CheckTextExists(fullText, "network") ? 15 : 0,
                MaxScore = 15,
                IsCorrect = CheckTextExists(fullText, "network"),
                Feedback = CheckTextExists(fullText, "network") ? "✓ Có nội dung về mạng" : "✗ Thiếu nội dung về mạng"
            });
            
            result.Items.Add(new GradingItem
            {
                Description = "Bảng hoặc danh sách",
                Score = (body.Elements<Table>().Any() || body.Descendants<NumberingProperties>().Any()) ? 25 : 0,
                MaxScore = 25,
                IsCorrect = body.Elements<Table>().Any() || body.Descendants<NumberingProperties>().Any(),
                Feedback = (body.Elements<Table>().Any() || body.Descendants<NumberingProperties>().Any()) 
                    ? "✓ Có bảng hoặc danh sách" 
                    : "✗ Thiếu bảng/danh sách"
            });
        }

        private void CheckTitleStyle(Body body, GradingResult result, string expectedTitle)
        {
            bool hasTitle = false;
            bool hasTitleStyle = false;
            
            // Check paragraphs for title
            var paragraphs = body.Elements<Paragraph>();
            foreach (var paragraph in paragraphs)
            {
                string text = GetParagraphText(paragraph);
                if (text.Contains(expectedTitle, StringComparison.OrdinalIgnoreCase))
                {
                    hasTitle = true;
                    
                    // Check if paragraph has Title style
                    var style = paragraph.ParagraphProperties?.ParagraphStyleId?.Val?.Value;
                    if (style != null && (style.Contains("Title", StringComparison.OrdinalIgnoreCase) || 
                                          style.Contains("Heading1", StringComparison.OrdinalIgnoreCase)))
                    {
                        hasTitleStyle = true;
                    }
                    break;
                }
            }
            
            int score = 0;
            if (hasTitle && hasTitleStyle) score = 20;
            else if (hasTitle) score = 10;
            
            result.Items.Add(new GradingItem
            {
                Description = "Tiêu đề với Style Title",
                Score = score,
                MaxScore = 20,
                IsCorrect = hasTitle && hasTitleStyle,
                Feedback = hasTitle 
                    ? (hasTitleStyle ? "✓ Tiêu đề đúng với Style Title" : "✓ Có tiêu đề nhưng chưa đúng style")
                    : "✗ Thiếu tiêu đề chính"
            });
        }

        private void CheckBulletList(Body body, GradingResult result, string sectionName, int expectedItems)
        {
            var paragraphs = body.Elements<Paragraph>().ToList();
            bool foundSection = false;
            int bulletCount = 0;
            
            for (int i = 0; i < paragraphs.Count; i++)
            {
                string text = GetParagraphText(paragraphs[i]);
                
                // Look for the section name
                if (text.Contains(sectionName, StringComparison.OrdinalIgnoreCase))
                {
                    foundSection = true;
                    
                    // Count bullet items after the section
                    for (int j = i + 1; j < Math.Min(i + 10, paragraphs.Count); j++)
                    {
                        var para = paragraphs[j];
                        // Check if paragraph has bullet/numbering
                        if (para.Descendants<NumberingProperties>().Any() || 
                            para.Descendants<ParagraphProperties>().Any(p => p.NumberingProperties != null))
                        {
                            bulletCount++;
                        }
                        else if (!string.IsNullOrWhiteSpace(GetParagraphText(para)))
                        {
                            // Stop counting if we hit non-bullet text
                            break;
                        }
                    }
                    break;
                }
            }
            
            int score = Math.Min(bulletCount * 4, 20); // 4 points per item, max 20
            bool isCorrect = bulletCount >= expectedItems;
            
            result.Items.Add(new GradingItem
            {
                Description = $"Danh sách {sectionName} ({expectedItems} mục)",
                Score = score,
                MaxScore = 20,
                IsCorrect = isCorrect,
                Feedback = foundSection 
                    ? $"Tìm thấy {bulletCount}/{expectedItems} mục trong danh sách"
                    : $"Không tìm thấy phần '{sectionName}'"
            });
        }

        private void CheckRentalPricesTable(Body body, GradingResult result)
        {
            var tables = body.Elements<Table>().ToList();
            bool hasRentalTable = false;
            int columnCount = 0;
            int rowCount = 0;
            
            foreach (var table in tables)
            {
                string tableText = GetTableText(table);
                if (tableText.Contains("Rental", StringComparison.OrdinalIgnoreCase) || 
                    tableText.Contains("Price", StringComparison.OrdinalIgnoreCase))
                {
                    hasRentalTable = true;
                    
                    // Count rows
                    var rows = table.Elements<TableRow>();
                    rowCount = rows.Count();
                    
                    // Count columns (from first row)
                    var firstRow = rows.FirstOrDefault();
                    if (firstRow != null)
                    {
                        columnCount = firstRow.Elements<TableCell>().Count();
                    }
                    break;
                }
            }
            
            bool hasCorrectStructure = columnCount >= 2 && rowCount >= 3;
            int score = hasRentalTable ? (hasCorrectStructure ? 20 : 10) : 0;
            
            result.Items.Add(new GradingItem
            {
                Description = "Bảng Rental Prices (2 cột, 3 hàng)",
                Score = score,
                MaxScore = 20,
                IsCorrect = hasRentalTable && hasCorrectStructure,
                Feedback = hasRentalTable 
                    ? $"Bảng có {columnCount} cột, {rowCount} hàng"
                    : "Không tìm thấy bảng giá thuê"
            });
        }

        private void CheckTableStyle(Body body, GradingResult result)
        {
            var tables = body.Elements<Table>();
            bool hasStyledTable = false;
            
            foreach (var table in tables)
            {
                // Check for table style properties
                var tableProperties = table.Descendants<DocumentFormat.OpenXml.Wordprocessing.TableProperties>().FirstOrDefault();
                if (tableProperties != null)
                {
                    // Check for table style (simplified check)
                    var tableStyle = tableProperties.TableStyle;
                    if (tableStyle != null && !string.IsNullOrEmpty(tableStyle.Val))
                    {
                        hasStyledTable = true;
                        break;
                    }
                    
                    // Check for any styling by looking for any table properties
                    // If table has properties, assume some styling is applied
                    var anyProperties = tableProperties.GetAttributes().Any();
                    if (anyProperties)
                    {
                        hasStyledTable = true;
                        break;
                    }
                }
            }
            
            result.Items.Add(new GradingItem
            {
                Description = "Kiểu bảng (Grid Table 4 - Accent 1)",
                Score = hasStyledTable ? 15 : 0,
                MaxScore = 15,
                IsCorrect = hasStyledTable,
                Feedback = hasStyledTable ? "✓ Bảng có áp dụng kiểu" : "✗ Bảng chưa có kiểu"
            });
        }

        private void CheckTableAlignmentAndCaption(Body body, GradingResult result)
        {
            var tables = body.Elements<Table>();
            bool hasCenteredTable = false;
            bool hasCaption = false;
            
            foreach (var table in tables)
            {
                // Check table alignment
                var tableProperties = table.Descendants<DocumentFormat.OpenXml.Wordprocessing.TableProperties>().FirstOrDefault();
                if (tableProperties?.TableJustification?.Val?.Value == DocumentFormat.OpenXml.Wordprocessing.TableRowAlignmentValues.Center)
                {
                    hasCenteredTable = true;
                }
                
                // Check for caption above table
                var previousSibling = table.PreviousSibling();
                if (previousSibling is Paragraph prevPara)
                {
                    string prevText = GetParagraphText(prevPara);
                    if (prevText.Contains("Rental", StringComparison.OrdinalIgnoreCase) || 
                        prevText.Contains("Price", StringComparison.OrdinalIgnoreCase))
                    {
                        hasCaption = true;
                    }
                }
            }
            
            int score = 0;
            if (hasCenteredTable && hasCaption) score = 15;
            else if (hasCenteredTable || hasCaption) score = 7;
            
            result.Items.Add(new GradingItem
            {
                Description = "Căn giữa bảng và chú thích",
                Score = score,
                MaxScore = 15,
                IsCorrect = hasCenteredTable && hasCaption,
                Feedback = hasCenteredTable 
                    ? (hasCaption ? "✓ Bảng căn giữa có chú thích" : "✓ Bảng căn giữa, thiếu chú thích")
                    : (hasCaption ? "✓ Có chú thích, bảng chưa căn giữa" : "✗ Thiếu cả căn giữa và chú thích")
            });
        }

        private void CheckFormatting(Body body, GradingResult result)
        {
            var runs = body.Descendants<Run>();
            int boldCount = runs.Count(r => r.RunProperties?.Bold != null);
            int italicCount = runs.Count(r => r.RunProperties?.Italic != null);
            
            bool hasFormatting = boldCount > 0 || italicCount > 0;
            int score = Math.Min((boldCount + italicCount) * 2, 10);
            
            result.Items.Add(new GradingItem
            {
                Description = "Định dạng Bold/Italic",
                Score = score,
                MaxScore = 10,
                IsCorrect = hasFormatting,
                Feedback = $"Bold: {boldCount}, Italic: {italicCount}"
            });
        }

        private void CheckRequiredContent(string fullText, GradingResult result)
        {
            List<string> requiredKeywords = new List<string>
            {
                "Bicycle", "Bellows", "Barn", "rental", "mountain", "price", "advantages", "builds"
            };
            
            int foundCount = requiredKeywords.Count(k => 
                fullText.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0);
            
            int score = (foundCount * 10) / requiredKeywords.Count;
            
            result.Items.Add(new GradingItem
            {
                Description = "Nội dung đầy đủ",
                Score = score,
                MaxScore = 10,
                IsCorrect = foundCount >= requiredKeywords.Count / 2,
                Feedback = $"Tìm thấy {foundCount}/{requiredKeywords.Count} từ khóa quan trọng"
            });
        }

        // Helper methods
        private string GetFullText(Body body)
        {
            return body.InnerText ?? "";
        }

        private string GetParagraphText(Paragraph paragraph)
        {
            return paragraph.InnerText ?? "";
        }

        private string GetTableText(Table table)
        {
            return table.InnerText ?? "";
        }

        private bool CheckTextExists(string text, string searchText)
        {
            return text.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private string GenerateSummary(GradingResult result)
        {
            int correctCount = result.Items.Count(i => i.IsCorrect);
            int totalCount = result.Items.Count;
            
            return $"Kết quả: {correctCount}/{totalCount} tiêu chí đạt. " +
                   $"Điểm: {result.TotalScore}/{result.MaxScore}. " +
                   $"{(result.Passed ? "ĐẬU" : "RỚT")}";
        }
    }
}
