using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using App5.Models;
using App5.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App5.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();

            //设置用户名称
            if(CurrentUser!=null)
            {
                this.UserNameTextBlock.Text = $"{CurrentUser.Name},欢迎使用考试报名管理系统!";
            }
        }
        private UserInfo CurrentUser = ((App)App.Current).CurrentUser;
        private void ViewAllExams_Click(object sender, RoutedEventArgs e)
        {
            // 导航到考试列表页面
            Frame.Navigate(typeof(ExamListPage));
        }

        private void ViewPersonalInfo_Click(object sender, RoutedEventArgs e)
        {
            // 导航到个人信息页面
            Frame.Navigate(typeof(MyInfoPage));
        }

        private void RegisterNewExam_Click(object sender, RoutedEventArgs e)
        {
            // 导航到考试列表页面
            Frame.Navigate(typeof(ExamListPage));
        }

        private void ViewMyExams_Click(object sender, RoutedEventArgs e)
        {
            // 导航到我的考试页面
            Frame.Navigate(typeof(MyExamPage));
        }

        private void EditPersonalInfo_Click(object sender, RoutedEventArgs e)
        {
            // 导航到个人信息页面
            Frame.Navigate(typeof(MyInfoPage));
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            // 显示修改密码对话框
            // 实现修改密码功能
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // 获取当前用户
            var currentUser = ((App)App.Current).CurrentUser;
            if (currentUser != null)
            {
                // 更新欢迎信息
                UserNameTextBlock.Text = $"{currentUser.Name}，欢迎使用考试报名管理系统";

                // 更新个人信息
                StudentIdTextBlock.Text = $"考生号：{currentUser.id}";
                NameTextBlock.Text = $"姓名：{currentUser.Name}";
                GenderTextBlock.Text = $"性别：{currentUser.Gender}";

                // 加载考试数据
                await LoadExamData(currentUser.id);
            }
        }

        private async Task LoadExamData(int userId)
        {
            try
            {
                var databaseService = new DatabaseService();

                // 获取已报名考试
                var registeredExams = await databaseService.LoadRegisteredExamAsync(userId);
                RegisteredExamsTextBlock.Text = $"已报名考试：{registeredExams.Count}";

                // 计算即将到来的考试
                var upcomingExams = registeredExams.Where(e =>
                    DateTime.Parse(e.date) > DateTime.Today ||
                    (DateTime.Parse(e.date) == DateTime.Today && TimeSpan.Parse(e.timestart) > DateTime.Now.TimeOfDay)
                ).ToList();
                UpcomingExamsTextBlock.Text = $"即将到来的考试：{upcomingExams.Count}";

                // 更新最近考试列表 - 只显示未过期的考试
                foreach (var exam in registeredExams)
                {
                    exam.CalculateExamStatus(); // 确保状态已计算
                }

                // 过滤掉已过期的考试，只保留未开始或进行中的考试
                var activeExams = registeredExams.Where(e => e.status != "已过期").ToList();

                // 按日期排序，显示最近的考试
                var recentExams = activeExams.OrderBy(e => DateTime.Parse(e.date)).Take(5).ToList();
                RecentExamsListView.ItemsSource = recentExams;

                // 如果没有未过期的考试，可以显示一个提示信息
                if (recentExams.Count == 0)
                {
                    NoneTestText.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                // 处理异常
            }
        }
    }
}
