using ModelsApi;
using ShopClient.Core;
using ShopClient.Views.Add;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShopClient.ViewModels
{
    public class FabricatorViewModel : BaseViewModel
    {
        private ObservableCollection<FabricatorApi> fabricators;
        public ObservableCollection<FabricatorApi> Fabricators
        {
            get => fabricators;
            set
            {
                SignalChanged();
                Set(ref fabricators, value);
            }
        }

        private FabricatorApi selectedFabricator = new FabricatorApi { };
        public FabricatorApi SelectedFabricator
        {
            get => selectedFabricator;
            set
            {
                selectedFabricator = value;
                SignalChanged();
            }
        }

        public CustomCommand AddFabricator { get; set; }
        public CustomCommand EditFabricator { get; set; }
        public CustomCommand DeleteFabricator { get; set; }

        public FabricatorViewModel()
        {
            Fabricators = new ObservableCollection<FabricatorApi>();
            GetList();

            AddFabricator = new CustomCommand(() =>
            {
                AddFabricator addFabricator = new AddFabricator();
                addFabricator.ShowDialog();
                GetList();
            });
            EditFabricator = new CustomCommand(() =>
            {
                if (SelectedFabricator == null) return;
                AddFabricator addunit = new AddFabricator(SelectedFabricator);
                addunit.ShowDialog();
                GetList();
            });
            DeleteFabricator = new CustomCommand(() =>
            {
                MessageBoxResult result = MessageBox.Show("Удалить запись?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (SelectedFabricator == null) return;
                    try
                    {
                        Delete(SelectedFabricator);
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
            var fabricatorsList = await Api.GetListAsync<List<FabricatorApi>>("Fabricator");
            Fabricators.Clear();
            foreach (FabricatorApi fabricator in fabricatorsList)
            {
                Fabricators.Add(fabricator);
            }
        }
        private async Task Delete(FabricatorApi fabricator)
        {
            var res = await Api.DeleteAsync<FabricatorApi>(fabricator, "Fabricator");
            GetList();
        }

    }
}
