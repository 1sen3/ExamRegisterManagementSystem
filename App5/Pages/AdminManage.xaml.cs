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
using WinRT;
using System.Runtime.InteropServices;
using App5.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App5.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdminManage : Page
    {
        private readonly DatabaseService _databaseService;
        public AdminManage()
        {
            this.InitializeComponent();
            _databaseService = new DatabaseService();
            LoadExam();
            LoadStudent(); 

            MainSegmented.SelectedIndex = 0;
        }
        private async Task LoadExam()
        {
            try
            {
                var exams = await _databaseService.GetExamsAsync();
                ExamDataGrid.ItemsSource = exams;
                if(exams.Count == 0)
                {
                    NoExamsText.Visibility = Visibility.Visible;
                }
            }
            catch(Exception ex)
            {
                NoExamsText.Text = "加载考试数据失败，请稍后重试。";
                NoExamsText.Visibility = Visibility.Visible;
            }
        }
        private async Task LoadStudent()
        {
            try
            {
                var students = await _databaseService.GetStudentAsync();
                StudentDataGrid.ItemsSource = students;
                if(students.Count == 0)
                {
                    NoStudentText.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                NoStudentText.Text = "加载学生数据失败，请稍后重试。";
                NoStudentText.Visibility = Visibility.Visible;
            }
        }

        private async void AddStudentButton_Click(object sender, RoutedEventArgs e)
        {
            var addstudentDialog = new ContentDialog
            {
                Title="添加学生",
                PrimaryButtonText="确定",
                CloseButtonText="取消",
                DefaultButton=ContentDialogButton.Primary,
                XamlRoot=this.XamlRoot,
                IsPrimaryButtonEnabled=false
            };

            var stackPanel = new StackPanel { Spacing = 16 };

            var namePanel = new StackPanel { Spacing = 12 };
            var nameTitle = new TextBlock
            {
                Text = "姓名",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 10, 0, 0)
            };
            var nameTextBox = new TextBox
            {
                PlaceholderText = "请输入考生姓名",
                Width = 300
            };
            namePanel.Children.Add(nameTitle);
            namePanel.Children.Add(nameTextBox);

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
                Width=300,
                Items =
                {
                    "男",
                    "女"
                }
            };
            genderPanel.Children.Add(genderTitle);
            genderPanel.Children.Add(genderComboBox);

            var birthdatePanel = new StackPanel { Spacing = 12 };
            var birthdateTitle = new TextBlock
            {
                Text = "出生日期",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var birthdatePicker = new CalendarDatePicker
            {
                PlaceholderText = "请选择出生日期",
                Width = 300
            };
            birthdatePanel.Children.Add(birthdateTitle);
            birthdatePanel.Children.Add(birthdatePicker);

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

            var passwordPanel = new StackPanel { Spacing = 12 };
            var passwordTitle = new TextBlock
            {
                Text = "请输入密码",
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

            stackPanel.Children.Add(namePanel);
            stackPanel.Children.Add(genderPanel);
            stackPanel.Children.Add(birthdatePanel);
            stackPanel.Children.Add(idNumberPanel);
            stackPanel.Children.Add(phonePanel);
            stackPanel.Children.Add(passwordPanel);

            addstudentDialog.Content = stackPanel;

            nameTextBox.TextChanged += (s, e) => ValidateInput(addstudentDialog, nameTextBox, genderComboBox, birthdatePicker, passwordBox);
            genderComboBox.SelectionChanged += (s, e) => ValidateInput(addstudentDialog, nameTextBox, genderComboBox, birthdatePicker, passwordBox);
            birthdatePicker.DateChanged += (s, e) => ValidateInput(addstudentDialog, nameTextBox, genderComboBox, birthdatePicker, passwordBox);
            passwordBox.PasswordChanged += (s, e) => ValidateInput(addstudentDialog, nameTextBox, genderComboBox, birthdatePicker, passwordBox);

            var result = await addstudentDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                string name = nameTextBox.Text;
                string gender = genderComboBox.SelectedItem?.ToString();
                DateTimeOffset? birthdate = birthdatePicker.Date;
                string birthDate = birthdate.Value.ToString("yyyy-MM-dd");
                string id_number = idNumberBox.Text;
                string phone = phoneBox.Text;
                string password = passwordBox.Password;
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(gender) && !string.IsNullOrEmpty(birthDate) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(id_number) && !string.IsNullOrEmpty(phone))
                {
                    var newstudent = new UserInfo
                    {
                        Name = name,
                        Gender = gender,
                        BirthDate = birthDate,
                        Password = password,
                        Role = "Student",
                        ID_number = id_number,
                        Phone = phone
                    };
                    await _databaseService.AddStudentAsync(newstudent);
                    await ShowDialog("添加成功", "成功添加学生", addstudentDialog.XamlRoot);
                    LoadStudent();
                }
                else
                {
                    await ShowDialog("添加失败", "添加失败，请重试。", addstudentDialog.XamlRoot);
                }
            }
        }
        private async void DeleteExamButton_Click(object sender, RoutedEventArgs args)
        {
            var deleteExamDialog = new ContentDialog
            {
                Title = "删除考试",
                PrimaryButtonText = "确定",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            var stackPanel = new StackPanel { Spacing = 16 };

            var exams = await _databaseService.GetExamsAsync();
            var chooseExamPanel = new StackPanel { Spacing = 12 };
            var chooseExamTitle = new TextBlock
            {
                Text = "请选择要删除的考试",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var chooseExamComboBox = new ComboBox
            {
                PlaceholderText = "请选择考试",
                Width = 300,
                DisplayMemberPath = "IdAndName"
            };
            chooseExamComboBox.ItemsSource = exams;

            chooseExamPanel.Children.Add(chooseExamTitle);
            chooseExamPanel.Children.Add(chooseExamComboBox);

            stackPanel.Children.Add(chooseExamPanel);
            deleteExamDialog.Content = stackPanel;

            var result = await deleteExamDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var selectedExam = chooseExamComboBox.SelectedItem as ExamInfo;
                if (selectedExam != null)
                {
                    await _databaseService.DeleteExamAsync(selectedExam);
                    await ShowDialog("删除成功", "成功删除" + selectedExam.id + "号" + selectedExam.name + "考试", deleteExamDialog.XamlRoot);
                    LoadExam();
                }
            }
        }
        private async void DeleteStudentButton_Click(object sender, RoutedEventArgs args)
        {
            var deleteStudentDialog = new ContentDialog
            {
                Title = "删除考生",
                PrimaryButtonText = "确定",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            var stackPanel = new StackPanel { Spacing = 16 };

            var students = await _databaseService.GetStudentAsync();
            var chooseStudentPanel = new StackPanel { Spacing = 12 };
            var chooseStudentTitle = new TextBlock
            {
                Text = "请选择要删除的考生",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var chooseStudentComboBox = new ComboBox
            {
                PlaceholderText = "请选择考生",
                Width = 300,
                DisplayMemberPath = "idAndName"
            };
            chooseStudentComboBox.ItemsSource = students;

            chooseStudentPanel.Children.Add(chooseStudentTitle);
            chooseStudentPanel.Children.Add(chooseStudentComboBox);

            stackPanel.Children.Add(chooseStudentPanel);
            deleteStudentDialog.Content = stackPanel;

            var result = await deleteStudentDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var selectedStudent = chooseStudentComboBox.SelectedItem as UserInfo;
                if (selectedStudent != null)
                {
                    await _databaseService.DeleteStudentAsync(selectedStudent);
                    await ShowDialog("删除成功", "成功删除" + selectedStudent.id + "号学生" + selectedStudent.Name, deleteStudentDialog.XamlRoot);
                    LoadStudent();
                }
            }
        }
        private async void AddExamButton_Click(object sender, RoutedEventArgs e)
        {
            var addExamDialog = new ContentDialog
            {
                Title = "添加考试",
                PrimaryButtonText = "确定",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot,
                IsPrimaryButtonEnabled = false
            };

            var stackPanel = new StackPanel { Spacing = 16 };

            var namePanel = new StackPanel { Spacing = 12 };
            var nameTitle = new TextBlock
            {
                Text = "考试科目",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 10, 0, 0)
            };
            var nameTextBox = new TextBox
            {
                PlaceholderText = "请输入考试科目",
                Width = 300
            };
            namePanel.Children.Add(nameTitle);
            namePanel.Children.Add(nameTextBox);

            var datePanel = new StackPanel { Spacing = 12 };
            var dateTitle = new TextBlock
            {
                Text = "考试日期",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var datePicker = new CalendarDatePicker
            {
                PlaceholderText = "请选择考试日期",
                Width = 300
            };
            datePanel.Children.Add(dateTitle);
            datePanel.Children.Add(datePicker);

            var startTimePanel = new StackPanel { Spacing = 12 };
            var startTimeTitle = new TextBlock
            {
                Text = "开始时间",
                FontSize = 16,
                HorizontalAlignment=HorizontalAlignment.Left
            };
            var startTimePicker = new TimePicker();
            startTimePanel.Children.Add(startTimeTitle);
            startTimePanel.Children.Add(startTimePicker);

            var endTimePanel = new StackPanel { Spacing = 12 };
            var endTimeTitle = new TextBlock
            {
                Text = "结束时间",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var endTimePicker = new TimePicker();
            endTimePanel.Children.Add(endTimeTitle);
            endTimePanel.Children.Add(endTimePicker);

            var roomPanel = new StackPanel { Spacing = 12 };
            var roomTitle = new TextBlock
            {
                Text = "考场",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 10, 0, 0)
            };
            var roomTextBox = new TextBox
            {
                PlaceholderText = "请输入考场",
                Width = 300
            };
            roomPanel.Children.Add(roomTitle);
            roomPanel.Children.Add(roomTextBox);

            stackPanel.Children.Add(namePanel);
            stackPanel.Children.Add(datePanel);
            stackPanel.Children.Add(startTimePanel);
            stackPanel.Children.Add(endTimePanel);
            stackPanel.Children.Add(roomPanel);

            addExamDialog.Content = stackPanel;

            nameTextBox.TextChanged += (s, e) => ExamValidateInput(addExamDialog,nameTextBox,datePicker,startTimePicker,endTimePicker,roomTextBox);
            datePicker.DateChanged += (s, e) => ExamValidateInput(addExamDialog, nameTextBox, datePicker, startTimePicker, endTimePicker, roomTextBox);
            startTimePicker.TimeChanged += (s, e) => ExamValidateInput(addExamDialog, nameTextBox, datePicker, startTimePicker, endTimePicker, roomTextBox);
            endTimePicker.TimeChanged += (s, e) => ExamValidateInput(addExamDialog, nameTextBox, datePicker, startTimePicker, endTimePicker, roomTextBox);
            roomTextBox.TextChanged += (s, e) => ExamValidateInput(addExamDialog, nameTextBox, datePicker, startTimePicker, endTimePicker, roomTextBox);

            var result = await addExamDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                string name = nameTextBox.Text;
                DateTimeOffset? dateoffset = datePicker.Date;
                string date = dateoffset.Value.ToString("yyyy-MM-dd");
                string startTime = startTimePicker.Time.ToString(@"hh\:mm");
                string endTime = endTimePicker.Time.ToString(@"hh\:mm");
                string room = roomTextBox.Text;
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime)&&!string.IsNullOrEmpty(room))
                {
                    var newexam = new ExamInfo
                    {
                        name = name,
                        date = date,
                        timestart = startTime,
                        timeend = endTime,
                        room = room
                    };
                    await _databaseService.AddExamAsync(newexam);
                    await ShowDialog("添加成功", "成功添加考试", addExamDialog.XamlRoot);
                    LoadExam();
                }
                else
                {
                    await ShowDialog("添加失败", "添加失败，请重试。", addExamDialog.XamlRoot);
                }
            }
        }
        // 验证输入是否有效
        private void ValidateInput(ContentDialog dialog, TextBox usernameTextBox, ComboBox genderComboBox, CalendarDatePicker birthDatePicker, PasswordBox passwordBox)
        {
            string username = usernameTextBox.Text;
            string gender = genderComboBox.SelectedItem?.ToString();
            DateTimeOffset? birthDate = birthDatePicker.Date;
            string password = passwordBox.Password;

            // 检查输入是否有效
            bool isValid = !string.IsNullOrEmpty(username) &&
                           !string.IsNullOrEmpty(gender) &&
                           birthDate.HasValue &&
                           !string.IsNullOrEmpty(password);

            // 动态启用或禁用“确定”按钮
            dialog.IsPrimaryButtonEnabled = isValid;
        }
        private void ExamValidateInput(ContentDialog dialog, TextBox nameTextBox, CalendarDatePicker DatePicker, TimePicker startTimePicker,TimePicker endTimePicker,TextBox roomTextBox)
        {
            string name = nameTextBox.Text;
            DateTimeOffset? dateoffset = DatePicker.Date;
            string date = dateoffset.HasValue ? dateoffset.Value.ToString("yyyy-MM-dd") : string.Empty;
            string startTime = startTimePicker.Time.ToString(@"hh\:mm");
            string endTime = endTimePicker.Time.ToString(@"hh\:mm");
            string room = roomTextBox.Text;

            // 检查输入是否有效
            bool isValid = !string.IsNullOrEmpty(name) &&
                           !string.IsNullOrEmpty(date) &&
                           !string.IsNullOrEmpty(startTime) &&
                           !string.IsNullOrEmpty(endTime) &&
                           !string.IsNullOrEmpty(room);

            // 动态启用或禁用“确定”按钮
            dialog.IsPrimaryButtonEnabled = isValid;
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

        private void MainSegmented_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainSegmented.SelectedIndex == 0)
            {
                ExamContent.Visibility = Visibility.Visible;
                StudentContent.Visibility = Visibility.Collapsed;
            }
            else if (MainSegmented.SelectedIndex == 1)
            {
                ExamContent.Visibility = Visibility.Collapsed;
                StudentContent.Visibility = Visibility.Visible;
            }
        }
        private void UpdateButtonState()
        {
            // 根据当前选中的内容更新按钮的事件处理程序
            if(MainSegmented.SelectedIndex == 0)
            {
                // 考试管理模式
                AddButton.Content = "添加考试";
                DeleteButton.Content = "删除考试";
            }
            else
            {
                // 考生管理模式
                AddButton.Content = "添加考生";
                DeleteButton.Content = "删除考生";
            }
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainSegmented.SelectedIndex == 0)
            {
                // 考试管理模式 - 调用添加考试方法
                AddExamButton_Click(sender, e);
            }
            else
            {
                // 考生管理模式 - 调用添加考生方法
                AddStudentButton_Click(sender, e);
            }
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainSegmented.SelectedIndex == 0)
            {
                // 考试管理模式 - 调用删除考试方法
                DeleteExamButton_Click(sender, e);
            }
            else
            {
                // 考生管理模式 - 调用删除考生方法
                DeleteStudentButton_Click(sender, e);
            }
        }
    }
}
