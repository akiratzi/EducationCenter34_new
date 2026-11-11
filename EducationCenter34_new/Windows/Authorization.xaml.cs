using EducationCenter34_new.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EducationCenter34_new.Windows
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {

        EducationCenter34Context context = new EducationCenter34Context();
        private int failedAttemptCount;

        public Authorization()
        {
            InitializeComponent();

            LoginTb.Text = Properties.Settings.Default.UserLogin;
            PasswordPb.Password = Properties.Settings.Default.UserPassword;
        }
        private void VBtn_Click(object sender, RoutedEventArgs e)
        {
               if (string.IsNullOrWhiteSpace(LoginTb.Text))
            {
                MessageBox.Show("Все поля обязательны для заполнения. Заполните поле\"Логин\"", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordPb.Password))
            {
                MessageBox.Show("Все поля обязательны для заполнения. Заполните поле\"Пароль\"", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            User? user = context.Users
                .Include(u => u.Employees)
                .FirstOrDefault(u => u.Login == LoginTb.Text && u.Password == PasswordPb.Password);

            if (user == null)
            {
                MessageBox.Show("Пользователь не найден. Проверьте правильность ввода логина или пароля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                failedAttemptCount++;

                return;
            }
            else
            {
                if (user.Employees.IsActive == false)
                { 
                    MessageBox.Show("Вы заблокированы. Обратитесь к координатору","Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }

                Captcha captcha = new Captcha();
                if (captcha.ShowDialog() == true)
                {
                    MessageBox.Show("Вы успешно авторизовались", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (IsRememberCb.IsChecked == true)    
                    {
                        Properties.Settings.Default.UserLogin = LoginTb.Text;
                        Properties.Settings.Default.UserPassword = PasswordPb.Password;
                    }
                    else
                    {
                        Properties.Settings.Default.UserLogin = "";
                        Properties.Settings.Default.UserPassword = "";
                    }

                    Properties.Settings.Default.Save();

                    if (user.RoleId == 1)
                    {
                        Coordinator coordinator = new Coordinator();
                        coordinator.Show();

                        Close();
                    }
                }
                else
                {
                    MessageBox.Show("Капча собрана неверно. Попробуйте ещё раз", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                    failedAttemptCount++;

                    if (failedAttemptCount >=3)
                    {
                        user.Employees.IsActive = false;
                        context.SaveChanges();

                        MessageBox.Show("Вы заблокированы. Обратитесь к координатору.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    return;
                }
            }


                
            
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
