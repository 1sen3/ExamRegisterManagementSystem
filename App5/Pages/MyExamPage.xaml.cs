using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using MySql.Data.MySqlClient;
using App5.Models;
using App5.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App5.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MyExamPage : Page
    {
        private readonly DatabaseService _databaseService;
        private UserInfo currentUser = ((App)App.Current).CurrentUser;
        public MyExamPage()
        {
            this.InitializeComponent();
            _databaseService = new DatabaseService();
            LoadRegisteredExams();
        }
        private async void LoadRegisteredExams()
        {
            int userid = currentUser.id;
            List<ExamInfo> exams = await _databaseService.LoadRegisteredExamAsync(userid);
            // 确保所有考试状态都已经被计算
            foreach(var exam in exams)
            {
                exam.CalculateExamStatus();
            }
            // 按考试状态排序
            var sortedExams = exams.OrderBy(e =>
            {
                switch (e.status)
                {
                    case "进行中":
                        return 0;
                    case "未开始":
                        return 1;
                    case "已过期":
                        return 2;
                    default:
                        return 3;
                }
            }).ToList();

            ExamDataGrid.ItemsSource = sortedExams;
            if (exams.Count == 0)
            {
                NoExamsText.Visibility = Visibility.Visible;
            }
        }
    }
}
 