using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using App5.Models;
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
using System.Threading.Tasks;
using Windows.System;
using App5.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App5.Pages
{
    public sealed partial class ExamListPage : Page
    {
        private readonly DatabaseService _databaseService;
        private UserInfo currentUser = ((App)App.Current).CurrentUser;
        public ExamListPage()
        {
            this.InitializeComponent();
            _databaseService = new DatabaseService();
            LoadExams();
        }

        // 加载考试数据
        private async void LoadExams()
        {
            var exams = await _databaseService.GetUnfinishedExamsAsync();
            
            // 绑定数据到 DataGrid
            ExamDataGrid.ItemsSource = exams;
            // 如果没有考试，显示提示信息
            if (exams.Count == 0)
            {
                NoExamsText.Visibility = Visibility.Visible;
            }
        }

        private async void RegistButton_Click(object sender, RoutedEventArgs e)
        {
            //获取用户ID
            int userid = currentUser.id;
            //获取考试ID
            var button=sender as Button;
            int examid = (int)button.Tag;
            if(await _databaseService.IsStudentRegisteredAsync(userid, examid))
            {
                await ShowMessageDialog("提示", "您已报名过该考试！");
                return;
            }
            // 弹出确认对话框
            var confirmDialog = new ContentDialog
            {
                Title = "确认报名",
                Content = "确定要报名该考试吗？",
                PrimaryButtonText = "确定",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot=this.XamlRoot
            };
            var result = await confirmDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // 执行报名操作
                if (await _databaseService.RegisterExamAsync(userid, examid))
                {
                    await ShowMessageDialog("成功", "报名成功！");
                }
                else
                {
                    await ShowMessageDialog("失败", "报名失败，请稍后重试。");
                }
            }

        }

        private async Task ShowMessageDialog(string title, string message)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "确定",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }
}