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
   public class LegalClientViewModel : BaseViewModel
    {
        private List<LegalClientApi> legalClients;
        public List<LegalClientApi> LegalClients
        {
            get => legalClients;
            set
            {
                Set(ref legalClients, value);
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

        private LegalClientApi selectedLegalClient = new LegalClientApi { };
        public LegalClientApi SelectedLegalClient
        {
            get => selectedLegalClient;
            set
            {
                selectedLegalClient = value;
                SignalChanged();
            }
        }

        public CustomCommand AddLegalClient { get; set; }
        public CustomCommand EditLegalClient { get; set; }
        public CustomCommand DeleteLegalClient { get; set; }

        public LegalClientViewModel()
        {
            Clients = new List<ClientApi>();
            LegalClients = new List<LegalClientApi>();
            GetList();



            AddLegalClient = new CustomCommand(() =>
            {
                AddLegalClient addLegalClient = new AddLegalClient();
                addLegalClient.ShowDialog();
                Update();
            });
            EditLegalClient = new CustomCommand(() =>
            {
                if (SelectedLegalClient == null) return;
                AddLegalClient addLegalClient = new AddLegalClient(SelectedLegalClient);
                addLegalClient.ShowDialog();
                Update();

            });
            DeleteLegalClient = new CustomCommand(() =>
            {
                MessageBoxResult result = MessageBox.Show("Удалить запись?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (SelectedLegalClient == null) return;
                    try
                    {
                        Delete(SelectedLegalClient);
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

        private async Task Delete(LegalClientApi legalClient)
        {
            var res = await Api.DeleteAsync<LegalClientApi>(legalClient, "LegalClient");
            Update();
        }
        private async Task GetList()
        {
            LegalClients = await Api.GetListAsync<List<LegalClientApi>>("LegalClient");
        }

        private void Update()
        {
            SignalChanged("Clients");
            GetList();
            SignalChanged("LegalClients");
        }
    }
}
