﻿using Code.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Code.Views.NenTang.Youtube
{
    /// <summary>
    /// Interaction logic for TangLuotXemView.xaml
    /// </summary>
    public partial class TangLuotXemView : UserControl
    {
        public TangLuotXemView()
        {
            var viewModel = TangLuotXemViewModel.GetInstance();
            InitializeComponent();
            this.DataContext = viewModel;
            deviceStatus.ItemsSource = viewModel.devices;
            JobProgress.ItemsSource = viewModel.jobProgress;
        }
    }
}
