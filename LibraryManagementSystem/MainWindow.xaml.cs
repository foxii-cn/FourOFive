﻿using LibraryManagementSystem.Config;
using LibraryManagementSystem.Controller;
using LibraryManagementSystem.DAO;
using LibraryManagementSystem.Entity;
using LibraryManagementSystem.LogSys;
using LibraryManagementSystem.Service;
using System.Windows;

namespace LibraryManagementSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Configuration.Save();
            System.Diagnostics.Trace.WriteLine(Configuration.Instance.PBKDF2Prf);
        }
    }
}