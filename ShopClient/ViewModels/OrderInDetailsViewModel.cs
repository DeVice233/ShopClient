using ModelsApi;
using ShopClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShopClient.ViewModels
{
    public class OrderInDetailsViewModel : BaseViewModel
    {
      
        private List<ProductOrderInApi> productOrderIns;
        public List<ProductOrderInApi> ProductOrderIns
        {
            get => productOrderIns;
            set
            {
                SignalChanged();
                Set(ref productOrderIns, value);
            }
        }

        private ProductTypeApi selectedProductOrderIn;
        public ProductTypeApi SelectedProductOrderIn
        {
            get => selectedProductOrderIn;
            set
            {
                selectedProductOrderIn = value;
                SignalChanged();
            }
        }
        private string orderDate;
        public string OrderDate
        { 
            get => orderDate;
            set
            {
                orderDate = value;
                SignalChanged();
            }
        }

        private string supplier;
        public string Supplier 
        { 
            get => supplier;
            set
            {
                supplier = value;
                SignalChanged();
            }
        }

        public List<ProductOrderInApi> FullProductOrderIns = new List<ProductOrderInApi>();
        public List<ProductApi> Products = new List<ProductApi>();
        public List<ClientApi> Clients = new List<ClientApi>();
        public List<PhysicalClientApi> PhysicalClients = new List<PhysicalClientApi>();
        public List<LegalClientApi> LegalClients = new List<LegalClientApi>();

       public CustomCommand Cancel { get; set; }

        public OrderInDetailsViewModel(OrderApi order)
        {
            GetList(order);

            Cancel = new CustomCommand(() =>
            {
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.DataContext == this)
                        {
                            CloseWin(window);
                        }
                    }
            });
        }
        public void CloseWin(object obj)
        {
            Window win = obj as Window;
            win.Close();
        }
        private async Task GetList(OrderApi order)
        {
            FullProductOrderIns = await Api.GetListAsync<List<ProductOrderInApi>>("ProductOrderIn");
            Products = await Api.GetListAsync<List<ProductApi>>("Product");
            Clients = await Api.GetListAsync<List<ClientApi>>("Client");
            PhysicalClients = await Api.GetListAsync<List<PhysicalClientApi>>("PhysicalClient");
            LegalClients = await Api.GetListAsync<List<LegalClientApi>>("LegalClient");
            OrderDate = order.Date.ToString();
            OrderDate = OrderDate.Substring(0, OrderDate.Length - 8);
            GenerateProductOrderIns(order);
        }
        public void GenerateProductOrderIns(OrderApi order)
        {
            ProductOrderIns = new List<ProductOrderInApi>();

            ProductOrderIns.AddRange(FullProductOrderIns.Where(s=>s.IdOrder == order.Id).ToList());
            foreach (var item in ProductOrderIns)
            {
                item.Product = Products.First(s => s.Id == item.IdProduct);
            }
            var legalcli = LegalClients.Find(s => s.IdClient == order.IdClient);
            Supplier = legalcli.Title;
            SignalChanged("Supplier");
            SignalChanged("OrderDate");
            SignalChanged("ProductOrderIns");
        }
    }
}
