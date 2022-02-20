using ModelsApi;
using ShopClient.Core;
using ShopClient.Views.Add;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShopClient.ViewModels
{
    public class PhysicalClientViewModel : BaseViewModel
    {
        private List<PhysicalClientApi> physicalClients;
        public List<PhysicalClientApi> PhysicalClients
        {
            get => physicalClients;
            set
            {  
                Set(ref physicalClients, value);
                SignalChanged();
            }
        }
        private List<ClientApi> clients;
        public List<ClientApi> Clients
        {
            get => clients;
            set
            { 
                Set(ref clients, value);
                SignalChanged();
              
            }
        }

        private PhysicalClientApi selectedPhysicalClient= new PhysicalClientApi { };
        public PhysicalClientApi SelectedPhysicalClient
        {
            get => selectedPhysicalClient;
            set
            {
                selectedPhysicalClient = value;
                SignalChanged();
            }
        }

        public CustomCommand AddPhysicalClient { get; set; }
        public CustomCommand EditPhysicalClient { get; set; }
        public CustomCommand DeletePhysicalClient { get; set; }

        public PhysicalClientViewModel()
        {    
            Clients = new List<ClientApi>();
            PhysicalClients = new List<PhysicalClientApi>();
            GetList();
        


            AddPhysicalClient = new CustomCommand(() =>
            {
                AddPhysicalClient addPhysicalClient = new AddPhysicalClient();
                addPhysicalClient.ShowDialog();
                Update();
            });
            EditPhysicalClient = new CustomCommand(() =>
            {
                if (SelectedPhysicalClient == null) return;
                AddPhysicalClient addPhysicalClient = new AddPhysicalClient(SelectedPhysicalClient);
                addPhysicalClient.ShowDialog();
                Update();

            });
            DeletePhysicalClient = new CustomCommand(() =>
            {
                MessageBoxResult result = MessageBox.Show("Удалить запись?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (SelectedPhysicalClient == null) return;
                    try
                    {
                        Delete(SelectedPhysicalClient);
                        Update();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    };
                }
                else return;

            });
        }

        private async Task Delete(PhysicalClientApi physicalClient)
        {
            var res = await Api.DeleteAsync<PhysicalClientApi>(physicalClient, "PhysicalClient");
            GetList();
        } 
        private async Task GetList()
        {
            PhysicalClients = await Api.GetListAsync<List<PhysicalClientApi>>("PhysicalClient");
        }

        private void Update()
        {
            SignalChanged("Clients");
            GetList();
            SignalChanged("PhysicalClients");
        }
    }
}
