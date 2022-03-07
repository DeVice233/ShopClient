﻿using ModelsApi;
using ShopClient.ViewModels;
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
using System.Windows.Shapes;

namespace ShopClient.Views
{
    /// <summary>
    /// Логика взаимодействия для OrderOutDetails.xaml
    /// </summary>
    public partial class OrderOutDetails : Window
    {
        public OrderOutDetails(OrderApi order)
        {
            InitializeComponent();
            DataContext = new OrderOutDetailsViewModels(order);
        }
    }
}
