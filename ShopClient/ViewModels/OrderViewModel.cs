using ModelsApi;
using ShopClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopClient.ViewModels
{
    public class OrderViewModel : BaseViewModel
    {
        private List<OrderApi> orders;
        public List<OrderApi> Orders
        {
            get => orders;
            set
            {
                Set(ref orders, value);
                SignalChanged();
            }
        }
        private List<ActionTypeApi> actionTypes;
        public List<ActionTypeApi> ActionTypes
        {
            get => actionTypes;
            set
            {
                Set(ref actionTypes, value);
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
        private OrderApi selectedOrder = new OrderApi { };
        public OrderApi SelectedOrder
        {
            get => selectedOrder;
            set
            {
                selectedOrder = value;
                SignalChanged();
            }
        }
        public OrderViewModel()
        {
            Clients = new List<ClientApi>();
            ActionTypes = new List<ActionTypeApi>();
            Orders = new List<OrderApi>();
            GetList();


        }

        private async Task GetList()
        {
            Clients = await Api.GetListAsync<List<ClientApi>>("Client");
            ActionTypes = await Api.GetListAsync<List<ActionTypeApi>>("ActionType");
            Orders = await Api.GetListAsync<List<OrderApi>>("Order");
            foreach (OrderApi order in Orders)
            {
                order.Client = Clients.First(s => s.Id == order.IdClient);
                order.ActionType = ActionTypes.First(s => s.Id == order.IdActionType);
            }
        }
        private void Update()
        {
            GetList();
            SignalChanged("Clients");
            SignalChanged("ActionTypes");
            SignalChanged("Orders");
        }
    }
}
