﻿using ModelsApi;
using ShopClient.Core;
using ShopClient.Views;
using ShopClient.Views.Add;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShopClient.ViewModels
{
    public class UnitViewModel : BaseViewModel
    {
        private List<UnitApi> unitApis;
        public List<UnitApi> Units
        {
            get => unitApis;
            set
            {
                SignalChanged();
                Set(ref unitApis, value);
            }
        }

        private UnitApi selectedUnit = new UnitApi { };
        public UnitApi SelectedUnit
        {
            get => selectedUnit;
            set
            {
                selectedUnit = value;
                SignalChanged();
            }
        }

        public CustomCommand AddUnit { get; set; }
        public CustomCommand EditUnit { get; set; }
        public CustomCommand DeleteUnit { get; set; }

        public UnitViewModel()
        {
            Units = new List<UnitApi>();
            GetList();

            AddUnit = new CustomCommand(() =>
            {
                AddUnit addunit = new AddUnit();
                addunit.ShowDialog();
                GetList();
            });
            EditUnit = new CustomCommand(() =>
            {
                if (SelectedUnit == null) return;
                AddUnit addunit = new AddUnit(SelectedUnit);
                addunit.ShowDialog();
                GetList();
            });
            DeleteUnit = new CustomCommand(() =>
            {
                MessageBoxResult result = MessageBox.Show("Удалить запись?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                { 
                    if (SelectedUnit == null) return;
                    try
                    {
                        Delete(SelectedUnit);
                        SignalChanged("Units");
                        GetList();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    };
                }
                else return;
            
            });
        }


        private async Task GetList()
        {
            Units = await Api.GetListAsync<List<UnitApi>>("Unit");
        }
        private async Task Delete(UnitApi unit)
        {
            var res = await Api.DeleteAsync<UnitApi>(unit, "Unit");
            GetList();
        }

        
    }
}