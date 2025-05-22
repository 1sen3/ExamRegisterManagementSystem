using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using App5.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MySql.Data.MySqlClient;
using Windows.Foundation;
using Windows.Foundation.Collections;
using App5.Models;
using App5.Services;
using Windows.Storage.Search;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App5.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MyInfoPage : Page
    {
        private readonly DatabaseService _databaseService;
        public MyInfoPage()
        {
            this.InitializeComponent();
            _databaseService = new DatabaseService();
            LoadUserInfo();
        }

        private UserInfo CurrentUser = ((App)App.Current).CurrentUser;

        private void LoadUserInfo()
        {
            if(CurrentUser != null)
            {
                IdTextBlock.Text = $"{CurrentUser.id}";
                UserNameTextBlock.Text = $"{CurrentUser.Name}";
                GenderTextBlock.Text = $"{CurrentUser.Gender}";
                BirthDateTextBlock.Text = $"{CurrentUser.BirthDate}";
                ID_NumberTextBlock.Text = $"{CurrentUser.ID_number}";
                PhoneTextBlock.Text = $"{CurrentUser.Phone}";
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var editDialog = new ContentDialog
            {
                Title = "修改个人信息",
                PrimaryButtonText = "保存",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = Content.XamlRoot,
                IsPrimaryButtonEnabled = false // 默认禁用“确定”按钮
            };

            var stackPanel = new StackPanel { Spacing = 16 };

            // 添加用户名输入框
            var usernamePanel = new StackPanel { Spacing = 12 };
            var usernameTitle = new TextBlock
            {
                Text = "用户名",
                FontSize = 16,
                Width = 300,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 10, 0, 0)
            };
            var usernameTextBox = new TextBox
            {
                PlaceholderText = "请输入用户名",
                Text = CurrentUser.Name
            };
            usernamePanel.Children.Add(usernameTitle);
            usernamePanel.Children.Add(usernameTextBox);

            // 添加性别选择框
            var genderPanel = new StackPanel { Spacing = 12 };
            var genderTitle = new TextBlock
            {
                Text = "性别",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var genderComboBox = new ComboBox
            {
                Items = { "男", "女" },
                Width = 300,
                SelectedItem = CurrentUser.Gender
            };
            genderPanel.Children.Add(genderTitle);
            genderPanel.Children.Add(genderComboBox);

            // 添加出生日期选择框
            var birthdatePanel = new StackPanel { Spacing = 12 };
            var birthdateTitle = new TextBlock
            {
                Text = "出生日期",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var birthdatePicker = new CalendarDatePicker
            {
                Width = 300,
                Date = DateTimeOffset.Parse(CurrentUser.BirthDate)
            };
            birthdatePanel.Children.Add(birthdateTitle);
            birthdatePanel.Children.Add(birthdatePicker);

            // 添加身份证号输入框
            var idNumberPanel = new StackPanel { Spacing = 12 };
            var idNumberTitle = new TextBlock
            {
                Text = "身份证号",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var idNumberBox = new TextBox
            {
                PlaceholderText = "请输入身份证号",
                Text = CurrentUser.ID_number
            };
            idNumberPanel.Children.Add(idNumberTitle);
            idNumberPanel.Children.Add(idNumberBox);

            // 添加手机号输入框
            var phonePanel = new StackPanel { Spacing = 12 };
            var phoneTitle = new TextBlock
            {
                Text = "手机号",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var phoneBox = new TextBox
            {
                PlaceholderText = "请输入手机号",
                Text = CurrentUser.Phone
            };
            phonePanel.Children.Add(phoneTitle);
            phonePanel.Children.Add(phoneBox);

            stackPanel.Children.Add(usernamePanel);
            stackPanel.Children.Add(genderPanel);
            stackPanel.Children.Add(birthdatePanel);
            stackPanel.Children.Add(idNumberPanel);
            stackPanel.Children.Add(phonePanel);

            editDialog.Content = stackPanel;

            // 监听输入框内容变化
            usernameTextBox.TextChanged += (s, e) => EditValidateInput(editDialog, usernameTextBox, genderComboBox, birthdatePicker,idNumberBox,phoneBox);
            genderComboBox.SelectionChanged += (s, e) => EditValidateInput(editDialog, usernameTextBox, genderComboBox, birthdatePicker, idNumberBox, phoneBox);
            birthdatePicker.DateChanged += (s, e) => EditValidateInput(editDialog, usernameTextBox, genderComboBox, birthdatePicker, idNumberBox, phoneBox);
            idNumberBox.TextChanged += (s, e) => EditValidateInput(editDialog, usernameTextBox, genderComboBox, birthdatePicker, idNumberBox, phoneBox);
            phoneBox.TextChanged += (s, e) => EditValidateInput(editDialog, usernameTextBox, genderComboBox, birthdatePicker, idNumberBox, phoneBox);

            var result = await editDialog.ShowAsync();

            // 如果用户点击保存按钮
            if (result == ContentDialogResult.Primary)
            {
                string newUsername = usernameTextBox.Text;
                string newGender = genderComboBox.SelectedItem?.ToString();
                DateTimeOffset? newbirthDate = birthdatePicker.Date;
                string newbirthDateString = newbirthDate.Value.ToString("yyyy-MM-dd");
                string newidNumber = idNumberBox.Text;
                string newphone = phoneBox.Text;

                // 更新用户信息
                CurrentUser.Name = newUsername;
                CurrentUser.Gender = newGender;
                CurrentUser.BirthDate = newbirthDateString;
                CurrentUser.ID_number = newidNumber;
                CurrentUser.Phone = newphone;

                // 保存到数据库
                await _databaseService.UpdateStudentInfoAsync(CurrentUser);

                // 重新加载用户信息
                LoadUserInfo();

                // 显示保存成功的消息
                await ShowDialog("保存成功", "个人信息已更新！", editDialog.XamlRoot);
            }
        }


        private async void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var changePasswordDialog = new ContentDialog
            {
                Title = "修改密码",
                PrimaryButtonText = "保存",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = Content.XamlRoot,
                IsPrimaryButtonEnabled = false // 默认禁用“确定”按钮
            };

            var stackPanel = new StackPanel { Spacing = 16 };

            // 添加旧密码输入框
            var oldPasswordPanel = new StackPanel { Spacing = 12 };
            var oldPasswordTitle = new TextBlock
            {
                Text = "旧密码",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var oldPasswordBox = new PasswordBox
            {
                PlaceholderText = "请输入旧密码",
                Width = 300
            };
            oldPasswordPanel.Children.Add(oldPasswordTitle);
            oldPasswordPanel.Children.Add(oldPasswordBox);

            // 添加新密码输入框
            var newPasswordPanel = new StackPanel { Spacing = 12 };
            var newPasswordTitle = new TextBlock
            {
                Text = "新密码",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var newPasswordBox = new PasswordBox
            {
                PlaceholderText = "请输入新密码",
                Width = 300
            };
            newPasswordPanel.Children.Add(newPasswordTitle);
            newPasswordPanel.Children.Add(newPasswordBox);

            // 添加确认新密码输入框
            var confirmPasswordPanel = new StackPanel { Spacing = 12 };
            var confirmPasswordTitle = new TextBlock
            {
                Text = "确认新密码",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var confirmPasswordBox = new PasswordBox
            {
                PlaceholderText = "请再次输入新密码",
                Width = 300
            };
            confirmPasswordPanel.Children.Add(confirmPasswordTitle);
            confirmPasswordPanel.Children.Add(confirmPasswordBox);

            stackPanel.Children.Add(oldPasswordPanel);
            stackPanel.Children.Add(newPasswordPanel);
            stackPanel.Children.Add(confirmPasswordPanel);

            changePasswordDialog.Content = stackPanel;

            // 监听输入框内容变化
            oldPasswordBox.PasswordChanged += (s, e) => ValidateInput(changePasswordDialog, oldPasswordBox, newPasswordBox, confirmPasswordBox);
            newPasswordBox.PasswordChanged += (s, e) => ValidateInput(changePasswordDialog, oldPasswordBox, newPasswordBox, confirmPasswordBox);
            confirmPasswordBox.PasswordChanged += (s, e) => ValidateInput(changePasswordDialog, oldPasswordBox, newPasswordBox, confirmPasswordBox);

            var result = await changePasswordDialog.ShowAsync();

            // 如果用户点击保存按钮
            if (result == ContentDialogResult.Primary)
            {
                string oldPassword = oldPasswordBox.Password;
                string newPassword = newPasswordBox.Password;
                string confirmPassword = confirmPasswordBox.Password;

                // 检查旧密码是否正确
                if (oldPassword != CurrentUser.Password)
                {
                    await ShowDialog("修改失败", "旧密码错误，请重试！", changePasswordDialog.XamlRoot);
                    return;
                }

                // 更新密码
                CurrentUser.Password = newPassword;
                await _databaseService.UpdateStudentInfoAsync(CurrentUser);

                // 显示保存成功的消息
                await ShowDialog("修改成功", "密码已更新！", changePasswordDialog.XamlRoot);
            }
        }
        private async Task ShowDialog(string title, string content, XamlRoot xamlRoot)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "确定",
                XamlRoot = xamlRoot
            };

            await dialog.ShowAsync();
        }
        // 验证输入是否有效
        private void ValidateInput(ContentDialog dialog, PasswordBox oldPasswordBox, PasswordBox newPasswordBox, PasswordBox confirmPasswordBox)
        {
            string oldPassword = oldPasswordBox.Password;
            string newPassword = newPasswordBox.Password;
            string confirmPassword = confirmPasswordBox.Password;

            // 检查输入是否有效
            bool isValid = !string.IsNullOrEmpty(oldPassword) &&
                           !string.IsNullOrEmpty(newPassword) &&
                           !string.IsNullOrEmpty(confirmPassword) &&
                           newPassword == confirmPassword;

            // 动态启用或禁用“确定”按钮
            dialog.IsPrimaryButtonEnabled = isValid;
        }
        private void EditValidateInput(ContentDialog dialog, TextBox usernameTextBox, ComboBox genderComboBox, CalendarDatePicker birthdatePicker, TextBox idNumberBox, TextBox phoneBox)
        {
            string username = usernameTextBox.Text;
            string gender = genderComboBox.SelectedItem?.ToString();
            DateTimeOffset? birthdate = birthdatePicker.Date;
            string id_number = idNumberBox.Text;
            string phone = phoneBox.Text;

            // 检查输入是否有效
            bool isValid = !string.IsNullOrEmpty(username) &&
                           !string.IsNullOrEmpty(gender) &&
                           !string.IsNullOrEmpty(id_number) &&
                           !string.IsNullOrEmpty(phone) &&
                           birthdate.HasValue;

            // 动态启用或禁用“确定”按钮
            dialog.IsPrimaryButtonEnabled = isValid;
        }
    }
}
