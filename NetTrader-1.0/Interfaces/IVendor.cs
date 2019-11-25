using Interfaces.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IVendor : IProperties
    {
        string VendorName { get; }
        bool CreateAccount(string login, string id, string pass);
        bool RemoveAccount(string id);
        string DataType { get; set; }
        List<string> GetDataTypes();
        Dictionary<string, IInstrument> GetAllInstruments();
        IInstrument GetInstrumentByName(string id);
        Task<IResult<T>> SendMessageToVendor<T>(Message mess);
        Quote3Message GetLevel2(IInstrument instrument);
        Task<List<Quote3Message>> GetLevel2FromServer(IInstrument instrument, int source);
        Task<IResult<List<IAsset>>> GetAssetsAsync(IAccount account);
        Task<IResult<List<IOrder>>> GetOrdersByAccount(IAccount account);
        IOrder GetOrderById(IInstrument instr, string id);
        void Subscribe(Subscription subscr);
        void UnSubscribe(Subscription subscr);
        event Action<Quote3Message> OnNewQuoteEvent;
        event Action LoadedEvent;
    }
}
