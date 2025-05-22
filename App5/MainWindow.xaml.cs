using System;
using System.Threading.Tasks;
using App5.Models;
using App5.Services;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MySql.Data.MySqlClient;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App5
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly DatabaseService _databaseService;
        public MainWindow()
        {
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;

            _databaseService = new DatabaseService();
        }

        private string connectionString = "Server=localhost;Database=exam_manage;User ID=root;Password=123456";

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username, password;
            username = UsernameTextBox.Text;
            password = PasswordBox.Password;

            //验证用户信息
            UserInfo user_login= await _databaseService.Login(username, password);

            if (user_login != null)
            {
                ((App)App.Current).CurrentUser = user_login;
                if (user_login.Role == "admin")
                {
                    ((App)App.Current).OpenAdminWindow();
                    return;
                }
                ((App)App.Current).OpenNewWindow();
            }
            else
            {
                await ShowDialog("登录失败", "用户名或密码错误，请重试！", Content.XamlRoot);
            }
        }


        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // 创建一个 ContentDialog 用于注册
            var registerDialog = new ContentDialog
            {
                Title = "注册",
                PrimaryButtonText = "注册",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = Content.XamlRoot,
                IsPrimaryButtonEnabled = false // 默认禁用“确定”按钮
            };

            // 创建一个 StackPanel 用于布局
            var stackPanel = new StackPanel { Spacing = 16 };

            // 添加用户名输入框
            var usernamePanel = new StackPanel { Spacing = 12 };
            var usernameTitle = new TextBlock
            {
                Text = "用户名",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 10, 0, 0)
            };
            var usernameTextBox = new TextBox
            {
                PlaceholderText = "请输入用户名",
                Width = 300
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
                PlaceholderText = "请选择性别",
                Width = 300,
                Items = { "男", "女" }
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
            var birthDatePicker = new CalendarDatePicker
            {
                PlaceholderText = "请选择出生日期",
                Width = 300
            };
            birthdatePanel.Children.Add(birthdateTitle);
            birthdatePanel.Children.Add(birthDatePicker);

            // 添加身份证号输入框
            var idNumberPanel = new StackPanel { Spacing = 12 };
            var idNumebrTitle = new TextBlock
            {
                Text = "请输入身份证号",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var idNumberBox = new TextBox
            {
                PlaceholderText = "请输入身份证号",
                Width = 300
            };
            idNumberPanel.Children.Add(idNumebrTitle);
            idNumberPanel.Children.Add(idNumberBox);

            // 添加手机号输入框
            var phonePanel = new StackPanel { Spacing = 12 };
            var phoneTitle = new TextBlock
            {
                Text = "请输入手机号",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var phoneBox = new TextBox
            {
                PlaceholderText = "请输入手机号",
                Width = 300
            };
            phonePanel.Children.Add(phoneTitle);
            phonePanel.Children.Add(phoneBox);

            // 添加密码输入框
            var passwordPanel = new StackPanel { Spacing = 12 };
            var passwordTitle = new TextBlock
            {
                Text = "密码",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var passwordBox = new PasswordBox
            {
                PlaceholderText = "请输入密码",
                Width = 300
            };
            passwordPanel.Children.Add(passwordTitle);
            passwordPanel.Children.Add(passwordBox);

            // 添加确认密码输入框
            var confirmPasswordPanel = new StackPanel { Spacing = 12 };
            var confirmPasswordTitle = new TextBlock
            {
                Text = "确认密码",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var confirmPasswordBox = new PasswordBox
            {
                PlaceholderText = "请再次输入密码",
                Width = 300
            };
            confirmPasswordPanel.Children.Add(confirmPasswordTitle);
            confirmPasswordPanel.Children.Add(confirmPasswordBox);

            // 将各个面板添加到主 StackPanel
            stackPanel.Children.Add(usernamePanel);
            stackPanel.Children.Add(genderPanel);
            stackPanel.Children.Add(birthdatePanel);
            stackPanel.Children.Add(idNumberPanel);
            stackPanel.Children.Add(phonePanel);
            stackPanel.Children.Add(passwordPanel);
            stackPanel.Children.Add(confirmPasswordPanel);

            // 将 StackPanel 设置为 ContentDialog 的内容
            registerDialog.Content = stackPanel;

            // 监听输入框内容变化
            usernameTextBox.TextChanged += (s, e) => ValidateInput(registerDialog, usernameTextBox, genderComboBox, birthDatePicker, passwordBox, confirmPasswordBox, idNumberBox, phoneBox);
            genderComboBox.SelectionChanged += (s, e) => ValidateInput(registerDialog, usernameTextBox, genderComboBox, birthDatePicker, passwordBox, confirmPasswordBox, idNumberBox, phoneBox);
            birthDatePicker.DateChanged += (s, e) => ValidateInput(registerDialog, usernameTextBox, genderComboBox, birthDatePicker, passwordBox, confirmPasswordBox, idNumberBox, phoneBox);
            passwordBox.PasswordChanged += (s, e) => ValidateInput(registerDialog, usernameTextBox, genderComboBox, birthDatePicker, passwordBox, confirmPasswordBox, idNumberBox, phoneBox);
            confirmPasswordBox.PasswordChanged += (s, e) => ValidateInput(registerDialog, usernameTextBox, genderComboBox, birthDatePicker, passwordBox, confirmPasswordBox, idNumberBox, phoneBox);


            // 显示 ContentDialog 并等待用户操作
            var result = await registerDialog.ShowAsync();

            // 如果用户点击了注册按钮
            if (result == ContentDialogResult.Primary)
            {
                string username = usernameTextBox.Text;
                string password = passwordBox.Password;
                string confirmPassword = confirmPasswordBox.Password;
                string gender = genderComboBox.SelectedItem?.ToString();
                DateTimeOffset? birthDate = birthDatePicker.Date;
                string idNumber = idNumberBox.Text;
                string phone = phoneBox.Text;

                // 检查输入是否有效
                if (!string.IsNullOrEmpty(username) &&
                    !string.IsNullOrEmpty(password) &&
                    !string.IsNullOrEmpty(confirmPassword) &&
                    !string.IsNullOrEmpty(gender) &&
                    birthDate.HasValue)
                {
                    // 检查密码和确认密码是否一致
                    if (password != confirmPassword)
                    {
                        var passwordMismatchDialog = new ContentDialog
                        {
                            Title = "注册失败",
                            Content = "密码和确认密码不一致，请重试！",
                            CloseButtonText = "确定",
                            XamlRoot = registerDialog.XamlRoot
                        };

                        await passwordMismatchDialog.ShowAsync();
                        return;
                    }

                    // 创建 UserInfo 对象
                    var newuserInfo = new UserInfo
                    {
                        Name = username,
                        Password = password,
                        Gender = gender,
                        BirthDate = birthDate.Value.ToString("yyyy-MM-dd"),
                        Role = "Student",
                        ID_number = idNumber,
                        Phone = phone
                    };

                    await _databaseService.AddStudentAsync(newuserInfo);

                    // 显示注册成功的消息
                    await ShowDialog("注册成功", "您已成功注册！", registerDialog.XamlRoot);
                }
                else
                {
                    // 显示注册失败的消息
                    var failureDialog = new ContentDialog
                    {
                        Title = "注册失败",
                        Content = "请填写所有信息！",
                        CloseButtonText = "确定",
                        XamlRoot = Content.XamlRoot
                    };

                    await failureDialog.ShowAsync();
                }
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
        private void ValidateInput(ContentDialog dialog, TextBox usernameTextBox, ComboBox genderComboBox, CalendarDatePicker birthDatePicker, PasswordBox passwordBox, PasswordBox confirmPasswordBox, TextBox idNumebrBox, TextBox phoneBox)
        {
            string username = usernameTextBox.Text;
            string gender = genderComboBox.SelectedItem?.ToString();
            DateTimeOffset? birthDate = birthDatePicker.Date;
            string password = passwordBox.Password;
            string confirmPassword = confirmPasswordBox.Password;
            string idnumber = idNumebrBox.Text;
            string phone = phoneBox.Text;

            // 检查输入是否有效
            bool isValid = !string.IsNullOrEmpty(username) &&
                           !string.IsNullOrEmpty(gender) &&
                           birthDate.HasValue &&
                           !string.IsNullOrEmpty(password) &&
                           !string.IsNullOrEmpty(confirmPassword) &&
                           !string.IsNullOrEmpty(idnumber) &&
                           !string.IsNullOrEmpty(phone);

            // 动态启用或禁用“确定”按钮
            dialog.IsPrimaryButtonEnabled = isValid;
        }
    }
}
