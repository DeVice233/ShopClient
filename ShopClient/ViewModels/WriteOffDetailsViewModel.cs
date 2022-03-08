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
    public class WriteOffDetailsViewModel : BaseViewModel
    {
        private List<ProductOrderOutApi> productOrderOuts;
        public List<ProductOrderOutApi> ProductOrderOuts
        {
            get => productOrderOuts;
            set
            {
                SignalChanged();
                Set(ref productOrderOuts, value);
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

        public CustomCommand Cancel { get; set; }

        public List<ProductApi> Products = new List<ProductApi>();
        public List<OrderOutApi> FullOrderOuts = new List<OrderOutApi>();
        public List<ProductOrderInApi> FullProductOrderIns = new List<ProductOrderInApi>();
        public OrderOutApi ThisOrderOut;

        public WriteOffDetailsViewModel(OrderApi order)
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
            Products = await Api.GetListAsync<List<ProductApi>>("Product");
            FullProductOrderIns = await Api.GetListAsync<List<ProductOrderInApi>>("ProductOrderIn");
            FullOrderOuts = await Api.GetListAsync<List<OrderOutApi>>("OrderOut");
            ThisOrderOut = FullOrderOuts.First(s => s.IdOrder == order.Id);
            ProductOrderOuts = ThisOrderOut.ProductOrderOuts;
            foreach(ProductOrderOutApi productOrderOut in ProductOrderOuts)
            {
                var prodOrderIn = FullProductOrderIns.First(s=>s.Id == productOrderOut.IdProductOrderIn);
                productOrderOut.Product = Products.First(s=>s.Id == prodOrderIn.IdProduct);
            }
            OrderDate = order.Date.ToString();
            OrderDate = OrderDate.Substring(0, OrderDate.Length - 8);

            SignalChanged("OrderDate");
            SignalChanged("ProductOrderOuts");
        }
    }
}
